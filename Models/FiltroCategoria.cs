using CommunityToolkit.Mvvm.ComponentModel;
using StyleBookBarberApp.Helpers;

namespace StyleBookBarberApp.Models;

/// <summary>Pestaña de filtro del catálogo o de la agenda.</summary>
public partial class FiltroCategoria : ObservableObject
{
    public string Nombre { get; init; } = string.Empty;
    public string Icono { get; init; } = string.Empty;
    public int Count { get; init; }

    [ObservableProperty]
    private bool estaActivo;

    partial void OnEstaActivoChanged(bool value)
    {
        OnPropertyChanged(nameof(Fondo));
        OnPropertyChanged(nameof(TextoColor));
        OnPropertyChanged(nameof(ContadorFondo));
        OnPropertyChanged(nameof(ContadorTextoColor));
    }

    public Color Fondo => EstaActivo ? Theme.Get("Primary") : Theme.Get("SurfaceContainerHigh");
    public Color TextoColor => EstaActivo ? Theme.Get("OnPrimary") : Theme.Get("OnSurfaceVariant");
    public Color ContadorFondo => EstaActivo ? Theme.Get("OnPrimary") : Theme.Get("SurfaceContainerHighest");
    public Color ContadorTextoColor => EstaActivo ? Theme.Get("Primary") : Theme.Get("OnSurfaceVariant");
}