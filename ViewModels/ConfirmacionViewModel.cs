using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StyleBookBarberApp.Models;
using StyleBookBarberApp.Services;

namespace StyleBookBarberApp.ViewModels;

/// <summary>
/// Confirmación de Cita (wireframe Stitch dl2, "Paso Final").
/// Resumen de la sesión + precio + persistencia de la cita en el backend.
/// </summary>
public partial class ConfirmacionViewModel : BaseViewModel
{
    private readonly CitasServices _citasService;
    private readonly UsuariosServices _usuariosService;

    [ObservableProperty]
    private bool confirmado;

    [ObservableProperty]
    private bool confirmando;

    [ObservableProperty]
    private string servicio = "Corte Degradado Clásico";

    [ObservableProperty]
    private string duracion = "40 min";

    [ObservableProperty]
    private string barbero = "Carlos Mendoza";

    [ObservableProperty]
    private string barberoRank = "Especialista en Fades";

    [ObservableProperty]
    private string fecha = "Sábado, 24 de Octubre";

    [ObservableProperty]
    private string hora = "4:30 PM (Hora Local)";

    [ObservableProperty]
    private string precio = "$25.00";

    public ConfirmacionViewModel(CitasServices citasService, UsuariosServices usuariosService)
    {
        _citasService = citasService;
        _usuariosService = usuariosService;
        Title = "Confirmación";
        CargarResumen();
    }

    private void CargarResumen()
    {
        Servicio = Preferences.Default.Get("ReservaServicio", "Corte Degradado Clásico");
        Duracion = Preferences.Default.Get("ReservaDuracion", "40 min");
        Barbero = Preferences.Default.Get("ReservaBarbero", "Carlos Mendoza");
        BarberoRank = Preferences.Default.Get("ReservaBarberoRank", "Especialista en Fades");
        Fecha = Preferences.Default.Get("ReservaFecha", "Sábado, 24 de Octubre");
        Hora = Preferences.Default.Get("ReservaHora", "4:30 PM");
        Precio = Preferences.Default.Get("ReservaPrecio", "$25.00");
    }

    /// <summary>Id real del cliente en la BD, resolviéndolo por correo si hace falta.</summary>
    private async Task<int> ResolverUsuarioIdAsync()
    {
        var usuarioId = Preferences.Default.Get("UserId", 0);
        if (usuarioId > 0)
            return usuarioId;

        var correo = Preferences.Default.Get("UserCorreo", string.Empty);
        if (string.IsNullOrWhiteSpace(correo))
            return 0;

        try
        {
            var usuarios = await _usuariosService.GetUsuariosAsync();
            var match = usuarios.FirstOrDefault(u =>
                string.Equals(u.Correo, correo.Trim(), StringComparison.OrdinalIgnoreCase));
            usuarioId = match?.UsuariosId ?? 0;
            if (usuarioId > 0)
                Preferences.Default.Set("UserId", usuarioId);
        }
        catch
        {
            usuarioId = 0;
        }

        return usuarioId;
    }

    private static int ResolverServiciosId(string categoria) => categoria switch
    {
        "barba" => 2,
        "combos" => 3,
        _ => 1,
    };

    private static decimal ParsePrecio(string precio)
    {
        if (decimal.TryParse(precio, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US"), out var valor))
            return valor;
        return 25m;
    }

    [RelayCommand]
    private async Task ConfirmarAsync()
    {
        if (Confirmando)
            return;

        Confirmando = true;

        try
        {
            var usuarioId = await ResolverUsuarioIdAsync();
            if (usuarioId <= 0)
            {
                await Shell.Current.DisplayAlertAsync("Sin sesión", "Inicia sesión para poder agendar tu cita.", "Entendido");
                return;
            }

            var barberoId = Preferences.Default.Get("ReservaBarberoId", 0);
            if (barberoId <= 0)
            {
                await Shell.Current.DisplayAlertAsync("Falta información", "No se encontró un barbero seleccionado. Vuelve a elegir fecha y hora.", "Entendido");
                return;
            }

            var categoria = Preferences.Default.Get("ReservaServicioCategoria", "cortes");
            var ticks = Preferences.Default.Get("ReservaFechaHoraTicks", 0L);
            var fechaHora = ticks > 0 ? new DateTime(ticks, DateTimeKind.Local) : DateTime.Now;

            var cita = new Cita
            {
                ClienteId = usuarioId,
                BarberosId = barberoId,
                ServiciosId = ResolverServiciosId(categoria),
                FechaHora = fechaHora,
                MontoTotal = ParsePrecio(Preferences.Default.Get("ReservaPrecio", "$25.00")),
                Estado = "Pendiente",
            };

            var creada = await _citasService.CrearCitaAsync(cita);
            if (creada is null)
                throw new InvalidOperationException("Reserva rechazada por el servidor");
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync(
                "No se pudo agendar",
                "Revisa tu conexión con el estudio, elige otro horario e intenta de nuevo.",
                "Entendido");
            return;
        }
        finally
        {
            Confirmando = false;
        }

        // Feedback visual (toast) antes de navegar a Mis Citas
        Confirmado = true;
        await Task.Delay(1600);
        Confirmado = false;

        await Shell.Current.GoToAsync("//MainTab/Citas");
    }
}