using CommunityToolkit.Mvvm.ComponentModel;
using StyleBookBarberApp.Helpers;

namespace StyleBookBarberApp.Models;

/// <summary>Cita de la Agenda del Día del barbero (zip dl3).</summary>
public partial class CitaAgenda : ObservableObject
{
    // ===== Datos estáticos =====
    public string Nombre { get; init; } = string.Empty;
    public bool EsVip { get; init; }
    public bool Verificado { get; init; }
    public string Servicio { get; init; } = string.Empty;
    public string Hora { get; init; } = string.Empty;        // "09:00"
    public string Periodo { get; init; } = string.Empty;     // "AM" / "PM"
    public string AvatarIcono { get; init; } = IconGlyphs.Schedule;

    // ===== Estado mutable =====
    [ObservableProperty]
    private string estado;    // "Completada" | "Proceso" | "Pendiente"

    [ObservableProperty]
    private string meta = string.Empty;

    [ObservableProperty]
    private string accionTexto = string.Empty;

    [ObservableProperty]
    private string accionIcono = string.Empty;

    [ObservableProperty]
    private bool accionVisible;

    [ObservableProperty]
    private bool accionHabilitada;

    public CitaAgenda(string nombre, bool esVip, bool verificado, string servicio,
                      string hora, string periodo, string avatarIcono,
                      string estado, string meta, string accionTexto, string accionIcono,
                      bool accionVisible, bool accionHabilitada)
    {
        Nombre = nombre;
        EsVip = esVip;
        Verificado = verificado;
        Servicio = servicio;
        Hora = hora;
        Periodo = periodo;
        AvatarIcono = avatarIcono;
        Estado = estado;
        Meta = meta;
        AccionTexto = accionTexto;
        AccionIcono = accionIcono;
        AccionVisible = accionVisible;
        AccionHabilitada = accionHabilitada;
    }

    partial void OnEstadoChanged(string value) => Refresh();
    partial void OnAccionTextoChanged(string value) => Refresh();
    partial void OnAccionIconoChanged(string value) => Refresh();
    partial void OnAccionHabilitadaChanged(bool value) => Refresh();

    private void Refresh()
    {
        OnPropertyChanged(nameof(Fondo));
        OnPropertyChanged(nameof(EsProceso));
        OnPropertyChanged(nameof(EsCompletada));
        OnPropertyChanged(nameof(TurnoActualVisible));
        OnPropertyChanged(nameof(VerificadoVisible));
        OnPropertyChanged(nameof(HoraColor));
        OnPropertyChanged(nameof(PeriodoColor));
        OnPropertyChanged(nameof(ServicioColor));
        OnPropertyChanged(nameof(EstadoPillFondo));
        OnPropertyChanged(nameof(EstadoPillTexto));
        OnPropertyChanged(nameof(EstadoDotColor));
        OnPropertyChanged(nameof(AvatarBadgeFondo));
        OnPropertyChanged(nameof(AvatarBadgeColor));
        OnPropertyChanged(nameof(AvatarBadgeIcono));
        OnPropertyChanged(nameof(AvatarSize));
        OnPropertyChanged(nameof(MetaIcono));
        OnPropertyChanged(nameof(MetaColor));
        OnPropertyChanged(nameof(MetaVisible));
        OnPropertyChanged(nameof(AccionBrush));
        OnPropertyChanged(nameof(AccionTextoColor));
        OnPropertyChanged(nameof(AccionIconoColor));
        OnPropertyChanged(nameof(OcupadoOpacity));
    }

    public bool EsProceso => Estado == "Proceso";
    public bool EsCompletada => Estado == "Completada";
    public bool TurnoActualVisible => EsProceso;
    public bool VerificadoVisible => Verificado;
    public double OcupadoOpacity => AccionHabilitada || string.IsNullOrEmpty(AccionTexto) ? 0.55 : 1;

    // ===== Colores / iconos calculados =====
    public Color Fondo => EsProceso ? Theme.Get("SurfaceContainerHigh")
                        : EsCompletada ? Theme.Get("SurfaceContainerLow")
                        : Theme.Get("SurfaceContainer");

    public Color HoraColor => EsProceso ? Theme.Get("Primary") : EsCompletada ? Theme.Get("Outline") : Theme.Get("OnSurface");
    public Color PeriodoColor => HoraColor;
    public Color ServicioColor => EsProceso ? Theme.Get("Primary") : Theme.Get("OnSurfaceVariant");

    public Color EstadoPillFondo => EsProceso ? Theme.Get("SecondaryContainer")
                        : EsCompletada ? Theme.Get("SurfaceContainerHighest")
                        : Theme.Get("SurfaceContainerHighest");
    public Color EstadoPillTexto => EsProceso ? Theme.Get("OnSecondaryContainer")
                        : EsCompletada ? Theme.Get("Outline")
                        : Theme.Get("OnSurfaceVariant");
    public Color EstadoDotColor => EsProceso ? Theme.Get("Primary") : Theme.Get("Outline");

    public Color AvatarBadgeFondo => EsProceso ? Theme.Get("Primary") : Theme.Get("SurfaceContainerHighest");
    public Color AvatarBadgeColor => EsProceso ? Theme.Get("OnPrimary") : Theme.Get("Outline");
    public string AvatarBadgeIcono => EsProceso ? IconGlyphs.Sync : EsCompletada ? IconGlyphs.Done : AvatarIcono;
    public double AvatarSize => EsProceso ? 56 : 48;

    public Color MetaColor => EsProceso ? Theme.Get("Primary") : Theme.Get("OnSurfaceVariant");
    public string MetaIcono => EsProceso ? IconGlyphs.Timelapse : IconGlyphs.CheckCircle;
    public bool MetaVisible => !string.IsNullOrEmpty(Meta);

    public Brush AccionBrush
    {
        get
        {
            if (!AccionHabilitada)
                return new SolidColorBrush(Theme.Get("SurfaceContainerHighest"));

            if (AccionTexto == "FINALIZAR SERVICIO")
                return new LinearGradientBrush(new GradientStopCollection
                {
                    new(Theme.Get("PrimaryLight"), 0.0f),
                    new(Theme.Get("Primary"), 0.5f),
                    new(Theme.Get("Secondary"), 1.0f),
                }, new Point(0, 0), new Point(1, 1));

            if (AccionTexto == "INICIAR CITA")
                return new SolidColorBrush(Theme.Get("Primary"));

            // "EN CURSO"
            return new SolidColorBrush(Theme.Get("SecondaryContainer"));
        }
    }

    public Color AccionTextoColor => AccionTexto == "EN CURSO" ? Theme.Get("OnSecondaryContainer") : Theme.Get("OnPrimary");
    public Color AccionIconoColor => AccionTexto == "EN CURSO" ? Theme.Get("Primary") : AccionTextoColor;

    // ===== Acciones de la agenda =====
    public void Iniciar()
    {
        Estado = "Proceso";
        Meta = "En sillón · iniciado hace 0m";
        AccionTexto = "EN CURSO";
        AccionIcono = IconGlyphs.Check;
        AccionHabilitada = false;
    }

    public void Finalizar()
    {
        Estado = "Completada";
        Meta = "Turno Concluido";
        AccionTexto = string.Empty;
        AccionIcono = string.Empty;
        AccionVisible = false;
        AccionHabilitada = false;
    }
}