using CommunityToolkit.Mvvm.ComponentModel;
using StyleBookBarberApp.Helpers;

namespace StyleBookBarberApp.Models;

/// <summary>Notificación del centro de notificaciones del usuario.</summary>
public partial class NotificacionItem : ObservableObject
{
    public string Titulo { get; init; } = string.Empty;
    public string Cuerpo { get; init; } = string.Empty;
    public string Hora { get; init; } = string.Empty;   // "Hace 2 min"
    public string Icono { get; init; } = IconGlyphs.Notifications;

    [ObservableProperty]
    private bool esNueva;

    partial void OnEsNuevaChanged(bool value)
    {
        OnPropertyChanged(nameof(FondoIcono));
        OnPropertyChanged(nameof(ColorIcono));
    }

    public Color FondoIcono => EsNueva ? Theme.Get("Primary") : Theme.Get("SurfaceContainerHighest");
    public Color ColorIcono => EsNueva ? Theme.Get("OnPrimary") : Theme.Get("Primary");
}