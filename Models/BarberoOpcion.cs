using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using StyleBookBarberApp.Helpers;

namespace StyleBookBarberApp.Models;

/// <summary>Barbero seleccionable en la franja "Elige tu Barbero" (pantalla de horarios).</summary>
public partial class BarberoOpcion : ObservableObject
{
    /// <summary>Id del barbero en el backend (/api/barberos).</summary>
    public int Id { get; set; }

    public string Nombre { get; init; } = string.Empty;
    public string NombreCompleto { get; init; } = string.Empty;
    public string Rating { get; init; } = "4.9";
    public string Rank { get; init; } = "MASTER BARBER";
    public string Servicio { get; init; } = string.Empty;
    public string Duracion { get; init; } = "50 min";
    public string Precio { get; init; } = "$38.00";

    private double _calificacion = 4.9;
    public double Calificacion { get => _calificacion; init => _calificacion = value; }

    /// <summary>Actualiza la calificación desde el backend (promedio real de reseñas).</summary>
    public void ActualizarCalificacion(double valor)
    {
        if (valor <= 0 || Math.Abs(_calificacion - valor) < 0.001)
            return;
        _calificacion = valor;
        OnPropertyChanged(nameof(Calificacion));
        OnPropertyChanged(nameof(Rating));
        OnPropertyChanged(nameof(Estrellas));
    }

    public ObservableCollection<ComentarioBarbero> Comentarios { get; init; } = new();

    [ObservableProperty]
    private bool esActivo;

    [ObservableProperty]
    private bool esFavorito;

    partial void OnEsActivoChanged(bool value)
    {
        OnPropertyChanged(nameof(Fondo));
        OnPropertyChanged(nameof(TextoColor));
        OnPropertyChanged(nameof(BordeColor));
    }

    partial void OnEsFavoritoChanged(bool value)
    {
        OnPropertyChanged(nameof(FavoritoColor));
        OnPropertyChanged(nameof(IconoFavorito));
        OnPropertyChanged(nameof(TextoChipFavorito));
    }

    /// <summary>Estrellas (5) rellenas según la calificación del barbero.</summary>
    public IReadOnlyList<Estrella> Estrellas
    {
        get
        {
            var llenas = (int)Math.Floor(Calificacion);
            return Enumerable.Range(0, 5)
                .Select(i => new Estrella { Color = i < llenas ? Theme.Get("Primary") : Theme.Get("OutlineVariant") })
                .ToList();
        }
    }

    public Color FavoritoColor => EsFavorito ? Theme.Get("Primary") : Theme.Get("OnSurfaceVariant");
    public string IconoFavorito => EsFavorito ? IconGlyphs.Favorite : IconGlyphs.FavoriteBorder;
    public string TextoChipFavorito => EsFavorito ? "EN FAVORITOS" : "AGREGAR A FAVORITOS";

    public Color Fondo => EsActivo ? Theme.Get("Primary") : Theme.Get("SurfaceContainerHigh");
    public Color TextoColor => EsActivo ? Theme.Get("OnPrimary") : Theme.Get("OnSurface");
    public Color BordeColor => EsActivo ? Theme.Get("GoldStroke") : Colors.Transparent;
}