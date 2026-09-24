using CommunityToolkit.Mvvm.ComponentModel;
using StyleBookBarberApp.Helpers;

namespace StyleBookBarberApp.Models;

/// <summary>Fecha del selector horizontal (zip dl1).</summary>
public partial class DiaCita : ObservableObject
{
    public string Nombre { get; init; } = string.Empty;   // LUN / MAR / MIÉ...
    public int Numero { get; init; }
    public string Fecha { get; init; } = string.Empty;    // "Lun 20 de Octubre"
    public bool TieneDisponibilidad { get; init; }

    [ObservableProperty]
    private bool esActivo;

    partial void OnEsActivoChanged(bool value) => OnPropertyChanged(nameof(Fondo));

    public Color Fondo => EsActivo ? Theme.Get("Primary") : Theme.Get("SurfaceContainerHigh");
    public Color TextoColor => EsActivo ? Theme.Get("OnPrimary") : Theme.Get("OnSurface");
    public Color NumeroColor => TextoColor;
    public Color PuntoColor => EsActivo ? Theme.Get("OnPrimary") : TieneDisponibilidad ? Theme.Get("Secondary") : Colors.Transparent;
}