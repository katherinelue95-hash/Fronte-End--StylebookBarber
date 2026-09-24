using CommunityToolkit.Mvvm.ComponentModel;
using StyleBookBarberApp.Helpers;

namespace StyleBookBarberApp.Models;

/// <summary>Tarjeta de servicio del catálogo (zip Desktop).</summary>
public partial class ServicioItem : ObservableObject
{
    public string Titulo { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;     // "cortes" | "barba" | "combos"
    public string Icono { get; init; } = IconGlyphs.Scissors;
    public string Imagen { get; init; } = string.Empty;        // recurso fotográfico del catálogo
    public string Precio { get; init; } = string.Empty;
    public string Duracion { get; init; } = string.Empty;
    public string Descripcion { get; init; } = string.Empty;
    public string Rating { get; init; } = string.Empty;        // "4.9" o vacío
    public bool EsTendencia { get; init; }
    public bool EsRitual { get; init; }
    public bool EsVip { get; init; }

    public bool TieneRating => !string.IsNullOrEmpty(Rating);

    [ObservableProperty]
    private bool esSeleccionado;

    partial void OnEsSeleccionadoChanged(bool value)
    {
        BotonTexto = value ? "SELECCIONADO" : "SELECCIONAR SERVICIO";
        BotonIcono = value ? IconGlyphs.Check : IconGlyphs.ArrowForward;
    }

    [ObservableProperty]
    private string botonTexto = "SELECCIONAR SERVICIO";

    [ObservableProperty]
    private string botonIcono = IconGlyphs.ArrowForward;
}