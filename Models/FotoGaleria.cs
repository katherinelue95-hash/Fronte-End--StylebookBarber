namespace StyleBookBarberApp.Models;

/// <summary>Fotografía de la galería del estudio (tarjeta del catálogo).</summary>
public class FotoGaleria
{
    public string Archivo { get; init; } = string.Empty;
    public string Titulo { get; init; } = string.Empty;
    public string Etiqueta { get; init; } = string.Empty;   // categoría corta
}