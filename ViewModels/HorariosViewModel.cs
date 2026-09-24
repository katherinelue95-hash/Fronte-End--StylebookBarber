using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.Models;
using StyleBookBarberApp.Services;

namespace StyleBookBarberApp.ViewModels;

/// <summary>
/// Selección de Fecha y Hora del barbero (wireframe Stitch dl1, "Paso 3 de 4").
/// Calendario mensual navegable por mes y año + franja de barberos con estrellas,
/// favoritos, comentarios y turnos mañana/tarde/noche.
/// </summary>
public partial class HorariosViewModel : BaseViewModel
{
    private static readonly string[] MesesCorto = ["ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC"];
    private static readonly string[] MesesLargo = ["ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"];

    public ObservableCollection<DiaMes> Mes { get; } = new();
    public ObservableCollection<BarberoOpcion> Barberos { get; } = new();
    public ObservableCollection<TurnoGrupo> Turnos { get; } = new();

    [ObservableProperty]
    private BarberoOpcion? barberoActivo;

    [ObservableProperty]
    private string resumenReserva = string.Empty;

    [ObservableProperty]
    private string resumenDetalles = string.Empty;

    [ObservableProperty]
    private string sillonesTexto = string.Empty;

    [ObservableProperty]
    private string tituloMes = string.Empty;

    [ObservableProperty]
    private string chipMes = string.Empty;

    [ObservableProperty]
    private int estrellasSeleccion = 5;

    [ObservableProperty]
    private string comentarioNuevo = string.Empty;

    [ObservableProperty]
    private string mensajeResena = string.Empty;

    /// <summary>Estrellas interactivas del selector de puntuación (tap para votar).</summary>
    public ObservableCollection<Estrella> EstrellasPuntaje { get; } = new();

    private int _anio = 2024;
    private int _mes = 10;
    private DiaMes? _diaActivo;
    private HorarioSlot? _horaElegida;

    private readonly ResenasServices _resenasService;
    private readonly BarberosServices _barberoService;
    private readonly UsuariosServices _usuariosService;

    public HorariosViewModel(ResenasServices resenasService, BarberosServices barberoService, UsuariosServices usuariosService)
    {
        _resenasService = resenasService;
        _barberoService = barberoService;
        _usuariosService = usuariosService;
        Title = "Horarios";

        CargarBarberos();
        CargarTurnos();
        ReconstruirCalendario();
        _horaElegida = Turnos[0].Slots[1];
        _horaElegida.EsElegido = true;
        EstrellasSeleccion = 5;
        OnEstrellasSeleccionChanged(EstrellasSeleccion);

        ActualizarSillones();
        ActualizarResumen();

        _ = SincronizarBackendAsync();
    }

    /// <summary>
    /// Sincroniza los barberos con el backend (/api/barberos): asigna el Id real
    /// y actualiza la calificación promedio desde la base de datos.
    /// </summary>
    private async Task SincronizarBackendAsync()
    {
        try
        {
            var barberos = await _barberoService.GetBarberosAsync();
            if (barberos.Count == 0)
            {
                AsignarIdsTemporales();
                return;
            }

            var porNombre = barberos.ToDictionary(b => b.NombreCompleto, b => b);
            for (var i = 0; i < Barberos.Count; i++)
            {
                var barbero = Barberos[i];
                if (porNombre.TryGetValue(barbero.NombreCompleto, out var bd))
                {
                    barbero.Id = bd.BarberosId;
                    if (bd.Calificacion > 0)
                        barbero.ActualizarCalificacion((double)bd.Calificacion);
                }
                else if (barbero.Id <= 0)
                {
                    barbero.Id = i + 1;
                }
            }

            await CargarResenasAsync(BarberoActivo);
        }
        catch
        {
            AsignarIdsTemporales();
        }
    }

    private void AsignarIdsTemporales()
    {
        for (var i = 0; i < Barberos.Count; i++)
            if (Barberos[i].Id <= 0)
                Barberos[i].Id = i + 1;
    }

    /// <summary>
    /// Recarga desde el backend los comentarios del barbero activo.
    /// Se invoca al volver a la pantalla para que las reseñas guardadas
    /// por cualquier cliente se mantengan visibles en la aplicación.
    /// </summary>
    public async Task RecargarResenasAsync()
    {
        if (BarberoActivo is not null)
            await CargarResenasAsync(BarberoActivo);
    }

    private async Task CargarResenasAsync(BarberoOpcion? barbero)
    {
        if (barbero is null || barbero.Id <= 0)
            return;

        try
        {
            var resenas = await _resenasService.GetResenasByBarberoAsync(barbero.Id);
            if (resenas.Count == 0)
                return;

            barbero.Comentarios.Clear();
            foreach (var r in resenas)
                barbero.Comentarios.Add(ComentarioDesdeResena(r));
        }
        catch
        {
            // Sin backend: se conservan los comentarios de ejemplo.
        }
    }

    private static ComentarioBarbero ComentarioDesdeResena(Resena r)
    {
        var autor = string.IsNullOrWhiteSpace(r.Autor) ? "Cliente" : r.Autor;
        return new ComentarioBarbero
        {
            Autor = autor,
            Inicial = string.IsNullOrWhiteSpace(autor) ? "C" : autor[..1].ToUpperInvariant(),
            Calificacion = r.Estrellas.ToString("0.0"),
            EstrellasLlenas = Math.Clamp(r.Estrellas, 1, 5),
            Fecha = ResenaFecha(r.Fecha),
            Texto = r.Comentario,
        };
    }

    /// <summary>
    /// Obtiene el Id real del usuario en la BD. Si la sesión local no lo tiene
    /// (p. ej. se entró a Horarios sin pasar por login), lo resuelve por correo.
    /// Devuelve 0 cuando no hay una sesión válida.
    /// </summary>
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

    private static string ResenaFecha(DateTime fecha)
    {
        var dias = (DateTime.Now - fecha).TotalDays;
        if (dias < 1)
            return "ahora";
        if (dias < 2)
            return "hace 1 día";
        if (dias < 7)
            return $"hace {(int)dias} días";
        if (dias < 30)
            return $"hace {(int)(dias / 7)} semanas";
        return fecha.ToString("dd MMM yyyy");
    }

    private void CargarBarberos()
    {
        Barberos.Add(new BarberoOpcion
        {
            Nombre = "Mateo V.", NombreCompleto = "Mateo Valenzuela", Rating = "4.9", Calificacion = 4.9, Rank = "MASTER BARBER",
            Servicio = "Ritual Real: Corte de Autor + Barba a Navaja",
            Duracion = "50 min", Precio = "$38.00",
            EsFavorito = true,
        });
        Barberos[^1].Comentarios.Add(new ComentarioBarbero
        {
            Autor = "Andrés L.", Inicial = "A", Calificacion = "5.0", EstrellasLlenas = 5, Fecha = "hace 2 días",
            Texto = "El mejor corte que me han hecho. Atención impecable y el ritual a navaja es otra cosa.",
        });
        Barberos[^1].Comentarios.Add(new ComentarioBarbero
        {
            Autor = "Rodrigo P.", Inicial = "R", Calificacion = "5.0", EstrellasLlenas = 5, Fecha = "hace 1 semana",
            Texto = "Mateo sabe qué te queda. Salí con un degradado milimétrico, siempre puntual.",
        });
        Barberos[^1].Comentarios.Add(new ComentarioBarbero
        {
            Autor = "Luis F.", Inicial = "L", Calificacion = "4.5", EstrellasLlenas = 4, Fecha = "hace 2 semanas",
            Texto = "Muy profesional, el local es elegante y el servicio completo. Volveré con cita.",
        });

        Barberos.Add(new BarberoOpcion
        {
            Nombre = "Carlos M.", NombreCompleto = "Carlos Mendoza", Rating = "4.8", Calificacion = 4.8, Rank = "MASTER BARBER",
            Servicio = "Corte Degradado de Precisión + Perfilado",
            Duracion = "45 min", Precio = "$32.00",
        });
        Barberos[^1].Comentarios.Add(new ComentarioBarbero
        {
            Autor = "Eduardo R.", Inicial = "E", Calificacion = "5.0", EstrellasLlenas = 5, Fecha = "hace 3 días",
            Texto = "Carlos domina los degradados. Perfilado perfecto y muy buen trato.",
        });
        Barberos[^1].Comentarios.Add(new ComentarioBarbero
        {
            Autor = "Mauricio T.", Inicial = "M", Calificacion = "4.5", EstrellasLlenas = 4, Fecha = "hace 1 semana",
            Texto = "Excelente atención. El corte quedó limpio y el precio justo.",
        });
        Barberos[^1].Comentarios.Add(new ComentarioBarbero
        {
            Autor = "Jorge A.", Inicial = "J", Calificacion = "5.0", EstrellasLlenas = 5, Fecha = "hace 2 semanas",
            Texto = "Recomendado. Sabe asesorar según el tipo de cabello.",
        });

        Barberos.Add(new BarberoOpcion
        {
            Nombre = "Daniel R.", NombreCompleto = "Daniel Rivera", Rating = "4.7", Calificacion = 4.7, Rank = "SENIOR BARBER",
            Servicio = "Afeitado Tradicional + Ritual de Toalla",
            Duracion = "40 min", Precio = "$28.00",
        });
        Barberos[^1].Comentarios.Add(new ComentarioBarbero
        {
            Autor = "Sergio M.", Inicial = "S", Calificacion = "5.0", EstrellasLlenas = 5, Fecha = "hace 4 días",
            Texto = "El ritual de toalla tibia es de otro nivel. Corte fino y relajante.",
        });
        Barberos[^1].Comentarios.Add(new ComentarioBarbero
        {
            Autor = "Gustavo N.", Inicial = "G", Calificacion = "4.5", EstrellasLlenas = 4, Fecha = "hace 1 semana",
            Texto = "Buen servicio, Daniel es atento y cuidadoso con la navaja.",
        });
        Barberos[^1].Comentarios.Add(new ComentarioBarbero
        {
            Autor = "Óscar V.", Inicial = "O", Calificacion = "5.0", EstrellasLlenas = 5, Fecha = "hace 3 semanas",
            Texto = "Muy buena experiencia de afeitado tradicional. Espacio impecable.",
        });

        BarberoActivo = Barberos[0];
    }

    private void CargarTurnos()
    {
        var manana = new TurnoGrupo
        {
            Nombre = "Turno Mañana", Icono = IconGlyphs.LightMode, IconoColor = Color.FromArgb("#F5D77F"),
            Rango = "10:00 AM — 01:00 PM",
        };
        Turnos.Add(manana);
        AddSlot(manana, "10:00 AM", "Sillón 1 disponible", true);
        AddSlot(manana, "10:30 AM", "Horario óptimo", true, esOptimo: true);
        AddSlot(manana, "11:00 AM", "Sillón 2 disponible", true);
        AddSlot(manana, "11:30 AM", "Horario óptimo", true, esOptimo: true);
        AddSlot(manana, "12:00 PM", "No disponible", false);
        AddSlot(manana, "12:30 PM", "Sillón 1 disponible", true);

        var tarde = new TurnoGrupo
        {
            Nombre = "Turno Tarde", Icono = IconGlyphs.WbTwilight, IconoColor = Color.FromArgb("#FDC659"),
            Rango = "02:00 PM — 06:00 PM",
        };
        Turnos.Add(tarde);
        AddSlot(tarde, "02:00 PM", "Sillón 1 disponible", true);
        AddSlot(tarde, "02:30 PM", "Horario óptimo", true, esOptimo: true);
        AddSlot(tarde, "03:00 PM", "Sillón 2 disponible", true);
        AddSlot(tarde, "03:30 PM", "Horario óptimo", true, esOptimo: true);
        AddSlot(tarde, "04:00 PM", "No disponible", false);
        AddSlot(tarde, "04:30 PM", "Sillón 1 disponible", true);
        AddSlot(tarde, "05:00 PM", "Sillón 2 disponible", true);
        AddSlot(tarde, "05:30 PM", "Último turno de la tarde", true);

        var noche = new TurnoGrupo
        {
            Nombre = "Turno Noche", Icono = IconGlyphs.NightsStay, IconoColor = Color.FromArgb("#8FBCE6"),
            Rango = "06:00 PM — 09:00 PM",
        };
        Turnos.Add(noche);
        AddSlot(noche, "06:00 PM", "Sillón 1 disponible", true);
        AddSlot(noche, "06:30 PM", "Horario óptimo", true, esOptimo: true);
        AddSlot(noche, "07:00 PM", "No disponible", false);
        AddSlot(noche, "07:30 PM", "Sillón 2 disponible", true);
        AddSlot(noche, "08:00 PM", "Sillón 1 disponible", true);
        AddSlot(noche, "08:30 PM", "Último turno de la noche", true);
    }

    private HorarioSlot AddSlot(TurnoGrupo grupo, string hora, string nota, bool disponible, bool esOptimo = false)
    {
        var slot = new HorarioSlot { Hora = hora, Nota = nota, Disponible = disponible, EsOptimo = esOptimo };
        grupo.Slots.Add(slot);
        return slot;
    }

    private void ReconstruirCalendario()
    {
        var first = new DateTime(_anio, _mes, 1);
        var offset = ((int)first.DayOfWeek + 6) % 7; // LUN = columna 0
        var dias = DateTime.DaysInMonth(_anio, _mes);

        Mes.Clear();

        // Solo se marca activo el día si pertenece al mes actualmente visible.
        var diaActivoValido = _diaActivo != null && _diaActivo.Numero >= 1 && _diaActivo.Numero <= dias;

        for (var i = 0; i < 42; i++)
        {
            var num = i - offset + 1;
            if (num < 1 || num > dias)
            {
                Mes.Add(DiaMes.Vacio(i));
                continue;
            }

            var col = i % 7;
            var esHoy = _anio == DateTime.Now.Year && _mes == DateTime.Now.Month && num == DateTime.Now.Day;
            Mes.Add(new DiaMes
            {
                Numero = num,
                Columna = col,
                TieneDisponibilidad = col <= 4,  // LUN a VIE
                Fecha = $"{DiaMes.NombreDia(col)} {num} de {MesesLargo[_mes - 1]}",
                EsHoy = esHoy,
            });
        }

        if (diaActivoValido)
            DesdeDia(_diaActivo!.Numero);

        TituloMes = $"{MesesLargo[_mes - 1]} {_anio}";
        ChipMes = $"{MesesCorto[_mes - 1]} {_anio}";
    }

    private void DesdeDia(int numero)
    {
        var dia = Mes.FirstOrDefault(d => d.Numero == numero);
        if (dia is null)
            return;
        foreach (var d in Mes)
            d.EsActivo = ReferenceEquals(d, dia);
        _diaActivo = dia;
    }

    private void ActualizarSillones()
    {
        var libres = Turnos.SelectMany(t => t.Slots).Count(s => s.Disponible);
        SillonesTexto = $"Sillón 1 y Sillón 2 operativos · {libres} turnos libres hoy";
    }

    private void ActualizarResumen()
    {
        var hora = string.IsNullOrEmpty(_horaElegida?.Hora) ? $"Elige un horario de {MesesCorto[_mes - 1]}" : _horaElegida!.Hora;
        var dia = _diaActivo is not null ? _diaActivo.Fecha : $"Selecciona un día de {MesesLargo[_mes - 1]}";
        ResumenReserva = $"{dia} • {hora}";
        ResumenDetalles = $"Sede Central Salamanca • Barbero: {BarberoActivo?.Nombre ?? "Mateo V."}";
    }

    partial void OnBarberoActivoChanged(BarberoOpcion? value)
    {
        foreach (var b in Barberos)
            b.EsActivo = ReferenceEquals(b, value);
        ActualizarResumen();
        if (value is not null)
            _ = CargarResenasAsync(value);
    }

    [RelayCommand]
    private void ElegirBarbero(BarberoOpcion? barbero)
    {
        if (barbero is not null)
            BarberoActivo = barbero;
    }

    [RelayCommand]
    private void ElegirDia(DiaMes dia)
    {
        if (!dia.Convertible)
            return;

        foreach (var d in Mes)
            d.EsActivo = ReferenceEquals(d, dia);
        _diaActivo = dia;
        ActualizarResumen();
    }

    [RelayCommand]
    private void ElegirHora(HorarioSlot slot)
    {
        if (!slot.Disponible)
            return;

        foreach (var s in Turnos.SelectMany(t => t.Slots))
            s.EsElegido = ReferenceEquals(s, slot);
        _horaElegida = slot;
        ActualizarResumen();
    }

    [RelayCommand]
    private void ToggleFavorito()
    {
        if (BarberoActivo is null)
            return;
        BarberoActivo.EsFavorito = !BarberoActivo.EsFavorito;
    }

    partial void OnEstrellasSeleccionChanged(int value)
    {
        EstrellasPuntaje.Clear();
        for (var i = 1; i <= 5; i++)
        {
            EstrellasPuntaje.Add(new Estrella
            {
                Valor = i,
                Color = i <= value ? Theme.Get("Primary") : Theme.Get("OutlineVariant"),
            });
        }
    }

    [RelayCommand]
    private void PuntuarEstrellas(int valor)
        => EstrellasSeleccion = Math.Clamp(valor, 1, 5);

    [RelayCommand]
    private async Task EnviarComentarioAsync()
    {
        var barbero = BarberoActivo;
        var texto = ComentarioNuevo?.Trim() ?? string.Empty;

        if (barbero is null)
            return;

        if (texto.Length == 0)
        {
            MensajeResena = "Escribe tu comentario antes de publicar";
            await Task.Delay(2500);
            MensajeResena = string.Empty;
            return;
        }

        var nombre = Preferences.Default.Get("UserNombre", "Cliente");
        var usuarioId = await ResolverUsuarioIdAsync();

        if (usuarioId <= 0)
        {
            MensajeResena = "Inicia sesión para dejar tu reseña y que se guarde en el estudio.";
            await Task.Delay(3000);
            MensajeResena = string.Empty;
            return;
        }

        Resena creada;
        try
        {
            var resena = new Resena
            {
                BarberosId = barbero.Id,
                UsuariosId = usuarioId,
                Estrellas = EstrellasSeleccion,
                Comentario = texto,
                Autor = nombre,
            };

            creada = await _resenasService.CrearResenaAsync(resena)
                ?? throw new InvalidOperationException("Sin respuesta del servidor");
        }
        catch
        {
            MensajeResena = "No se pudo guardar la reseña. Revisa la conexión al estudio.";
            await Task.Delay(3000);
            MensajeResena = string.Empty;
            return;
        }

        if (creada.CalificacionPromedio > 0)
            barbero.ActualizarCalificacion((double)creada.CalificacionPromedio);

        barbero.Comentarios.Insert(0, ComentarioDesdeResena(creada));

        ComentarioNuevo = string.Empty;
        MensajeResena = $"¡Gracias {nombre}! Reseña de {EstrellasSeleccion} ★ guardada para {barbero.Nombre}.";
        await Task.Delay(3000);
        MensajeResena = string.Empty;
    }

    [RelayCommand]
    private void MesAnterior()
    {
        _mes--;
        if (_mes < 1)
        {
            _mes = 12;
            _anio--;
        }
        _diaActivo = null;
        ReconstruirCalendario();
        ActualizarResumen();
    }

    [RelayCommand]
    private void MesSiguiente()
    {
        _mes++;
        if (_mes > 12)
        {
            _mes = 1;
            _anio++;
        }
        _diaActivo = null;
        ReconstruirCalendario();
        ActualizarResumen();
    }

    [RelayCommand]
    private void AnioAnterior()
    {
        _anio--;
        _diaActivo = null;
        ReconstruirCalendario();
        ActualizarResumen();
    }

    [RelayCommand]
    private void AnioSiguiente()
    {
        _anio++;
        _diaActivo = null;
        ReconstruirCalendario();
        ActualizarResumen();
    }

    [RelayCommand]
    private async Task ContinuarReservaAsync()
    {
        var barbero = BarberoActivo;
        var horaTexto = _horaElegida?.Hora ?? "10:30 AM";
        var dia = _diaActivo;

        Preferences.Default.Set("ReservaBarberoId", barbero?.Id ?? 1);
        Preferences.Default.Set("ReservaBarbero", barbero?.NombreCompleto ?? "Mateo Valenzuela");
        Preferences.Default.Set("ReservaBarberoRank", barbero?.Rank ?? "MASTER BARBER");
        Preferences.Default.Set("ReservaFecha", dia?.Fecha ?? $"{(int)DateTime.Now.Day} de {MesesLargo[DateTime.Now.Month - 1]}");
        Preferences.Default.Set("ReservaHora", horaTexto);

        var fecha = new DateTime(_anio, _mes, dia?.Numero ?? DateTime.Now.Day, 10, 30, 0);
        if (dia is not null &&
            DateTime.TryParseExact(horaTexto, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            fecha = new DateTime(_anio, _mes, dia.Numero, parsed.Hour, parsed.Minute, 0);
        }
        Preferences.Default.Set("ReservaFechaHoraTicks", fecha.Ticks);

        await Shell.Current.GoToAsync("Confirmacion");
    }
}