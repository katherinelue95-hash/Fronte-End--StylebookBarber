using CommunityToolkit.Mvvm.ComponentModel;
using StyleBookBarberApp.Helpers;

namespace StyleBookBarberApp.Models;

/// <summary>Horario/hueco disponible, elegido u ocupado (zip dl1).</summary>
public partial class HorarioSlot : ObservableObject
{
    public string Hora { get; init; } = string.Empty;          // "10:00 AM"
    public string Nota { get; init; } = string.Empty;          // "Sillón 1 Disponible"
    public bool Disponible { get; init; }
    public bool EsOptimo { get; init; }                        // "Horario Óptimo"
    public string Estado => Disponible ? "LIBRE" : "OCUPADO";

    [ObservableProperty]
    private bool esElegido;

    partial void OnEsElegidoChanged(bool value)
    {
        OnPropertyChanged(nameof(Fondo));
        OnPropertyChanged(nameof(HoraColor));
        OnPropertyChanged(nameof(FondoEstado));
        OnPropertyChanged(nameof(TextoEstado));
        OnPropertyChanged(nameof(LabelEstado));
        OnPropertyChanged(nameof(NotaColor));
        OnPropertyChanged(nameof(NotaIcono));
    }

    public Color Fondo => EsElegido ? Theme.Get("SurfaceContainerHigh") : Theme.Get("SurfaceContainer");
    public Color HoraColor => EsElegido ? Theme.Get("Primary") : Theme.Get("OnSurface");
    public Color FondoEstado => EsElegido ? Theme.Get("Primary") : Theme.Get("SurfaceContainerHighest");
    public Color TextoEstado => EsElegido ? Theme.Get("OnPrimary") : Theme.Get("Primary");
    public string LabelEstado => EsElegido ? "ELEGIDO" : Estado;
    public Color NotaColor => EsElegido ? Theme.Get("Primary") : Theme.Get("OnSurfaceVariant");
    public string NotaIcono => EsElegido && EsOptimo ? IconGlyphs.CheckCircle : string.Empty;
    public double Opacidad => Disponible ? 1 : 0.62;

    public bool EsOcupado => !Disponible;
    public bool MostrarCandado => !Disponible;
}