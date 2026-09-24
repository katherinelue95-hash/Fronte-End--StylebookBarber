using StyleBookBarberApp.Helpers;

namespace StyleBookBarberApp.Models;

/// <summary>Registro del historial de citas del cliente.</summary>
public class RegistroCita
{
    public string Servicio { get; init; } = string.Empty;
    public string Barbero { get; init; } = string.Empty;
    public string Fecha { get; init; } = string.Empty;
    public string Estado { get; init; } = "COMPLETADA";
    public string Precio { get; init; } = "$0.00";

    public Color FondoEstado => Estado switch
    {
        "CONFIRMADA" or "EN CURSO" => Theme.Get("Primary"),
        _ => Theme.Get("SurfaceContainerHighest"),
    };

    public Color TextoEstado => FondoEstado == Theme.Get("Primary")
        ? Theme.Get("OnPrimary")
        : Theme.Get("Outline");

    public string EstadoIcono => Estado switch
    {
        "CONFIRMADA" => IconGlyphs.Verified,
        "EN CURSO" => IconGlyphs.Sync,
        "CANCELADA" => IconGlyphs.Close,
        _ => IconGlyphs.CheckCircle,
    };
}