using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.Models;
using StyleBookBarberApp.Services;

namespace StyleBookBarberApp.ViewModels;

/// <summary>
/// Panel de administración: gestión de Citas, Reseñas, Servicios y Usuarios.
/// </summary>
public partial class AdminViewModel : BaseViewModel
{
    private const string SeccionCitas = "Citas";
    private const string SeccionResenas = "Resenas";
    private const string SeccionServicios = "Servicios";
    private const string SeccionUsuarios = "Usuarios";

    private static readonly string[] RolesDisponibles = ["Cliente", "Barbero", "Administrador"];

    private readonly CitasServices _citasServices;
    private readonly ResenasServices _resenasServices;
    private readonly ServiciosServices _serviciosServices;
    private readonly UsuariosServices _usuariosServices;
    private readonly HttpClient _http;

    private static readonly CultureInfo Usd = CultureInfo.GetCultureInfo("en-US");

    public AdminViewModel(
        CitasServices citasServices,
        ResenasServices resenasServices,
        ServiciosServices serviciosServices,
        UsuariosServices usuariosServices,
        HttpClient http)
    {
        _citasServices = citasServices;
        _resenasServices = resenasServices;
        _serviciosServices = serviciosServices;
        _usuariosServices = usuariosServices;
        _http = http;
        Title = "Panel de Administración";
    }

    public ObservableCollection<AdminCitaItem> Citas { get; } = new();
    public ObservableCollection<AdminResenaItem> Resenas { get; } = new();
    public ObservableCollection<AdminServicioItem> Servicios { get; } = new();
    public ObservableCollection<AdminUsuarioItem> Usuarios { get; } = new();

    [ObservableProperty]
    private string seccionActiva = SeccionCitas;

    [ObservableProperty]
    private string mensaje = string.Empty;

    public string UsuarioSesion => Preferences.Default.Get("UserNombre", "Administrador");

    // ===== Estado visual del segmentado Citas / Reseñas / Servicios / Usuarios =====
    public Color CitasBgColor => SeccionActiva == SeccionCitas ? ResourceColor("SurfaceContainerHigh") : Colors.Transparent;
    public Color CitasTextColor => SeccionActiva == SeccionCitas ? ResourceColor("PrimaryBright") : ResourceColor("OnSurfaceVariant");
    public Color ResenasBgColor => SeccionActiva == SeccionResenas ? ResourceColor("SurfaceContainerHigh") : Colors.Transparent;
    public Color ResenasTextColor => SeccionActiva == SeccionResenas ? ResourceColor("PrimaryBright") : ResourceColor("OnSurfaceVariant");
    public Color ServiciosBgColor => SeccionActiva == SeccionServicios ? ResourceColor("SurfaceContainerHigh") : Colors.Transparent;
    public Color ServiciosTextColor => SeccionActiva == SeccionServicios ? ResourceColor("PrimaryBright") : ResourceColor("OnSurfaceVariant");
    public Color UsuariosBgColor => SeccionActiva == SeccionUsuarios ? ResourceColor("SurfaceContainerHigh") : Colors.Transparent;
    public Color UsuariosTextColor => SeccionActiva == SeccionUsuarios ? ResourceColor("PrimaryBright") : ResourceColor("OnSurfaceVariant");

    public bool CitasVisible => SeccionActiva == SeccionCitas;
    public bool ResenasVisible => SeccionActiva == SeccionResenas;
    public bool ServiciosVisible => SeccionActiva == SeccionServicios;
    public bool UsuariosVisible => SeccionActiva == SeccionUsuarios;

    partial void OnSeccionActivaChanged(string value)
    {
        OnPropertyChanged(nameof(CitasBgColor));
        OnPropertyChanged(nameof(CitasTextColor));
        OnPropertyChanged(nameof(ResenasBgColor));
        OnPropertyChanged(nameof(ResenasTextColor));
        OnPropertyChanged(nameof(ServiciosBgColor));
        OnPropertyChanged(nameof(ServiciosTextColor));
        OnPropertyChanged(nameof(UsuariosBgColor));
        OnPropertyChanged(nameof(UsuariosTextColor));
        OnPropertyChanged(nameof(CitasVisible));
        OnPropertyChanged(nameof(ResenasVisible));
        OnPropertyChanged(nameof(ServiciosVisible));
        OnPropertyChanged(nameof(UsuariosVisible));
    }

    public async Task InicializarAsync()
    {
        Mensaje = string.Empty;
        OnPropertyChanged(nameof(UsuarioSesion));
        await CargarSeccionAsync(SeccionActiva);
    }

    [RelayCommand]
    private async Task SeleccionarSeccionAsync(string seccion)
    {
        SeccionActiva = seccion;
        Mensaje = string.Empty;
        await CargarSeccionAsync(seccion);
    }

    [RelayCommand]
    private async Task VolverAsync()
        => await Shell.Current.GoToAsync("..");

    private async Task CargarSeccionAsync(string seccion)
    {
        IsBusy = true;
        try
        {
            switch (seccion)
            {
                case SeccionCitas:
                    await CargarCitasAsync();
                    break;
                case SeccionResenas:
                    await CargarResenasAsync();
                    break;
                case SeccionServicios:
                    await CargarServiciosAsync();
                    break;
                case SeccionUsuarios:
                    await CargarUsuariosAsync();
                    break;
            }
        }
        catch
        {
            Mensaje = "No se pudo conectar con el servidor. Verifica tu conexión.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ===================== CITAS =====================
    private async Task CargarCitasAsync()
    {
        var citas = await _citasServices.GetCitasAsync();
        Citas.Clear();
        foreach (var c in citas.OrderByDescending(x => x.FechaHora))
        {
            Citas.Add(new AdminCitaItem
            {
                CitasId = c.CitasId,
                Cliente = c.ClienteNombre,
                Barbero = c.BarberoNombre,
                Servicio = c.ServicioNombre,
                Fecha = c.FechaHora.ToString("ddd dd MMM · hh:mm tt"),
                Precio = c.MontoTotal.ToString("C", Usd),
                Estado = c.Estado,
                AccionEstado = "MARCAR " + SiguienteEstado(c.Estado).ToUpperInvariant(),
            });
        }
    }

    [RelayCommand]
    private async Task CambiarEstadoAsync(AdminCitaItem item)
    {
        var siguiente = SiguienteEstado(item.Estado);
        try
        {
            var actualizada = await _citasServices.CambiarEstadoAsync(item.CitasId, siguiente);
            if (actualizada is null)
            {
                Mensaje = "No se pudo cambiar el estado de la cita.";
                return;
            }
            Mensaje = $"Cita {item.CitasId} ahora está {siguiente}.";
            await CargarCitasAsync();
        }
        catch
        {
            Mensaje = "No se pudo cambiar el estado de la cita.";
        }
    }

    [RelayCommand]
    private async Task EliminarCitaAsync(AdminCitaItem item)
    {
        var confirmar = await Shell.Current.DisplayAlertAsync(
            "Eliminar cita",
            $"¿Eliminar la cita de {item.Cliente} ({item.Fecha})?",
            "Eliminar", "Cancelar");
        if (!confirmar)
            return;

        try
        {
            if (await _citasServices.EliminarCitaAsync(item.CitasId))
            {
                Mensaje = "Cita eliminada.";
                await CargarCitasAsync();
            }
            else
            {
                Mensaje = "No se encontró la cita.";
            }
        }
        catch
        {
            Mensaje = "No se pudo eliminar la cita.";
        }
    }

    private static string SiguienteEstado(string estado) => estado switch
    {
        "Pendiente" => "Confirmada",
        "Confirmada" => "En curso",
        "En curso" => "Completada",
        "Completada" => "Cancelada",
        _ => "Pendiente",
    };

    // ===================== RESEÑAS =====================
    private async Task CargarResenasAsync()
    {
        var resenas = await _resenasServices.GetResenasAsync();
        Resenas.Clear();
        foreach (var r in resenas)
        {
            Resenas.Add(new AdminResenaItem
            {
                ResenaId = r.ResenaId,
                Autor = r.Autor,
                Barbero = r.BarberoNombre,
                Estrellas = r.Estrellas.ToString(),
                Fecha = r.Fecha.ToString("dd MMM yyyy"),
                Comentario = r.Comentario,
            });
        }
    }

    [RelayCommand]
    private async Task EliminarResenaAsync(AdminResenaItem item)
    {
        var confirmar = await Shell.Current.DisplayAlertAsync(
            "Eliminar reseña",
            $"¿Eliminar la reseña de {item.Autor}?",
            "Eliminar", "Cancelar");
        if (!confirmar)
            return;

        try
        {
            if (await _resenasServices.EliminarResenaAsync(item.ResenaId))
            {
                Mensaje = "Reseña eliminada.";
                await CargarResenasAsync();
            }
            else
            {
                Mensaje = "No se encontró la reseña.";
            }
        }
        catch
        {
            Mensaje = "No se pudo eliminar la reseña.";
        }
    }

    // ===================== SERVICIOS =====================
    private async Task CargarServiciosAsync()
    {
        var servicios = await _serviciosServices.GetServiciosAsync();
        Servicios.Clear();
        foreach (var s in servicios.OrderBy(x => x.NombreServicio))
        {
            var categoria = string.IsNullOrWhiteSpace(s.NombreCategoria)
                ? NombreCategoriaPorId(s.CategoriaId)
                : s.NombreCategoria.ToUpperInvariant();
            Servicios.Add(new AdminServicioItem
            {
                ServiciosId = s.ServiciosId,
                Nombre = s.NombreServicio,
                Categoria = categoria,
                Precio = s.Precio.ToString("C", Usd),
                Duracion = $"{s.DuracionMin} min",
                FotoUrl = s.FotoUrl,
            });
        }
    }

    [RelayCommand]
    private async Task NuevoServicioAsync()
    {
        var nombre = await PedirTextoAsync("Nuevo servicio", "Nombre del servicio", string.Empty);
        if (string.IsNullOrWhiteSpace(nombre))
            return;

        var precioTexto = await PedirTextoAsync("Nuevo servicio", "Precio (USD)", "25.00");
        if (!decimal.TryParse(precioTexto, NumberStyles.Any, Usd, out var precio) || precio <= 0)
            return;

        var categoriaId = await ElegirCategoriaAsync();
        if (categoriaId is null)
            return;

        var duracionTexto = await PedirTextoAsync("Nuevo servicio", "Duración (minutos)", "30");
        if (!int.TryParse(duracionTexto, out var duracion) || duracion <= 0)
            return;

        var fotoUrl = await ElegirYSubirFotoAsync();

        var servicio = new Servicio
        {
            CategoriaId = categoriaId.Value,
            NombreServicio = nombre,
            Precio = precio,
            DuracionMin = duracion,
            FotoUrl = fotoUrl,
        };

        try
        {
            var creado = await _serviciosServices.CrearServicioAsync(servicio);
            if (creado is null)
            {
                Mensaje = "No se pudo crear el servicio.";
                return;
            }
            Mensaje = "Servicio creado.";
            await CargarServiciosAsync();
        }
        catch
        {
            Mensaje = "No se pudo crear el servicio.";
        }
    }

    [RelayCommand]
    private async Task EditarServicioAsync(AdminServicioItem item)
    {
        var nombre = await PedirTextoAsync("Editar servicio", "Nombre del servicio", item.Nombre);
        if (string.IsNullOrWhiteSpace(nombre))
            return;

        var precioTexto = await PedirTextoAsync("Editar servicio", "Precio (USD)", item.Precio);
        if (!decimal.TryParse(precioTexto, NumberStyles.Currency | NumberStyles.Number, Usd, out var precio) || precio <= 0)
            return;

        var duracionTexto = await PedirTextoAsync("Editar servicio", "Duración (minutos)", item.Duracion);
        if (!int.TryParse(duracionTexto, out var duracion) || duracion <= 0)
            return;

        var cambiarFoto = await Shell.Current.DisplayActionSheetAsync("¿Cambiar la foto?", "Cancelar", null, "Sí", "No");
        var fotoUrl = cambiarFoto == "Sí" ? await ElegirYSubirFotoAsync() : item.FotoUrl;

        var servicio = new Servicio
        {
            ServiciosId = item.ServiciosId,
            CategoriaId = CategoriaIdPorTexto(item.Categoria),
            NombreServicio = nombre,
            Precio = precio,
            DuracionMin = duracion,
            FotoUrl = fotoUrl,
        };

        try
        {
            var actualizado = await _serviciosServices.ActualizarServicioAsync(item.ServiciosId, servicio);
            if (actualizado is null)
            {
                Mensaje = "No se pudo actualizar el servicio.";
                return;
            }
            Mensaje = "Servicio actualizado.";
            await CargarServiciosAsync();
        }
        catch
        {
            Mensaje = "No se pudo actualizar el servicio.";
        }
    }

    [RelayCommand]
    private async Task EliminarServicioAsync(AdminServicioItem item)
    {
        var confirmar = await Shell.Current.DisplayAlertAsync(
            "Eliminar servicio",
            $"¿Eliminar \"{item.Nombre}\"?",
            "Eliminar", "Cancelar");
        if (!confirmar)
            return;

        try
        {
            if (await _serviciosServices.EliminarServicioAsync(item.ServiciosId))
            {
                Mensaje = "Servicio eliminado.";
                await CargarServiciosAsync();
            }
            else
            {
                Mensaje = "No se encontró el servicio.";
            }
        }
        catch
        {
            Mensaje = "No se pudo eliminar el servicio.";
        }
    }

    private async Task<int?> ElegirCategoriaAsync()
    {
        var opcion = await Shell.Current.DisplayActionSheetAsync("Categoría del servicio", "Cancelar", null, "Cortes (1)", "Barba (2)", "Combos (3)");
        return opcion switch
        {
            "Cortes (1)" => 1,
            "Barba (2)" => 2,
            "Combos (3)" => 3,
            _ => (int?)null,
        };
    }

    private async Task<string?> ElegirYSubirFotoAsync()
    {
        try
        {
            var fotos = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions { Title = "Elegir foto del servicio" });
            var foto = fotos?.FirstOrDefault();
            if (foto is null)
                return null;

            using var photoStream = await foto.OpenReadAsync();
            using var contenido = new MultipartFormDataContent();
            using var streamContent = new StreamContent(photoStream);
            contenido.Add(streamContent, "file", foto.FileName);

            using var respuesta = await _http.PostAsync("/api/archivos/upload", contenido);
            respuesta.EnsureSuccessStatusCode();

            var resultado = await respuesta.Content.ReadFromJsonAsync<SubidaRespuesta>();
            return resultado?.Url;
        }
        catch
        {
            Mensaje = "No se pudo subir la foto.";
            return null;
        }
    }

    // ===================== USUARIOS =====================
    private async Task CargarUsuariosAsync()
    {
        var usuarios = await _usuariosServices.GetUsuariosAsync();
        Usuarios.Clear();
        foreach (var u in usuarios.OrderBy(x => x.UsuariosId))
        {
            Usuarios.Add(new AdminUsuarioItem
            {
                UsuariosId = u.UsuariosId,
                NombreCompleto = $"{u.Nombre} {u.Apellido}".Trim(),
                Correo = u.Correo,
                Rol = string.IsNullOrWhiteSpace(u.NombreRol) ? NombreRolPorId(u.RolId) : u.NombreRol,
                RolId = u.RolId,
            });
        }
    }

    [RelayCommand]
    private async Task CambiarRolAsync(AdminUsuarioItem item)
    {
        var opcion = await Shell.Current.DisplayActionSheetAsync(
            $"Rol de {item.NombreCompleto} (actual: {item.Rol})",
            "Cancelar", null, RolesDisponibles);

        if (opcion is null || opcion == "Cancelar")
            return;

        var rolId = RolIdPorNombre(opcion);
        if (rolId == item.RolId)
            return;

        try
        {
            var actualizado = await _usuariosServices.CambiarRolAsync(item.UsuariosId, rolId);
            if (actualizado is null)
            {
                Mensaje = "No se pudo cambiar el rol.";
                return;
            }
            Mensaje = $"Rol de {item.NombreCompleto} actualizado a {opcion}.";
            await CargarUsuariosAsync();
        }
        catch
        {
            Mensaje = "No se pudo cambiar el rol.";
        }
    }

    // ===================== Helpers =====================
    private static async Task<string?> PedirTextoAsync(string titulo, string mensaje, string inicial)
        => await Shell.Current.DisplayPromptAsync(titulo, mensaje, "Continuar", "Cancelar", initialValue: inicial);

    private static string NombreCategoriaPorId(int id) => id switch
    {
        1 => "CORTES",
        2 => "BARBA",
        3 => "COMBOS",
        _ => "OTROS",
    };

    private static int CategoriaIdPorTexto(string texto)
    {
        var limpio = (texto ?? string.Empty).Split(' ')[0].Trim();
        return limpio switch
        {
            "CORTES" => 1,
            "BARBA" => 2,
            "COMBOS" => 3,
            _ => 1,
        };
    }

    private static string NombreRolPorId(int id) => id switch
    {
        2 => "Barbero",
        3 or 4 => "Administrador",
        _ => "Cliente",
    };

    private static int RolIdPorNombre(string nombre) => nombre switch
    {
        "Barbero" => 2,
        "Administrador" => 3,
        _ => 1,
    };

    private sealed record SubidaRespuesta(string Url);
}