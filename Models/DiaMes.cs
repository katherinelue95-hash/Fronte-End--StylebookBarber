using CommunityToolkit.Mvvm.ComponentModel;
using StyleBookBarberApp.Helpers;

namespace StyleBookBarberApp.Models;

/// <summary>Día del calendario mensual completo (OCT 2024). Numero = 0 para celdas vacías.</summary>
public partial class DiaMes : ObservableObject
{
    private static readonly string[] Nombres = ["LUN", "MAR", "MIÉ", "JUE", "VIE", "SÁB", "DOM"];

    public int Numero { get; init; }
    public int Columna { get; init; }
    public bool Convertible => Numero > 0;
    public string Fecha { get; init; } = string.Empty;   // "Mié 22 de Octubre"
    public bool TieneDisponibilidad { get; init; }
    public bool EsHoy { get; init; }

    [ObservableProperty]
    private bool esActivo;

    partial void OnEsActivoChanged(bool value)
    {
        OnPropertyChanged(nameof(Fondo));
        OnPropertyChanged(nameof(TextoColor));
        OnPropertyChanged(nameof(PuntoColor));
        OnPropertyChanged(nameof(BordeColor));
    }

    public Color Fondo => !Convertible ? Colors.Transparent
        : EsActivo ? Theme.Get("Primary") : Theme.Get("SurfaceContainerHighest");

    public Color TextoColor => !Convertible ? Colors.Transparent
        : EsActivo ? Theme.Get("OnPrimary") : Theme.Get("OnSurface");

    public Color PuntoColor => !Convertible ? Colors.Transparent
        : EsActivo ? Theme.Get("OnPrimary")
        : TieneDisponibilidad ? Theme.Get("Primary") : Theme.Get("OutlineVariant");

    public Color BordeColor => !Convertible ? Color.FromArgb("#00FFFFFF")
        : EsActivo ? Theme.Get("Primary")
        : EsHoy ? Theme.Get("GoldStrokeStrong")
        : Color.FromArgb("#2ED4AF37");

    public static DiaMes Vacio(int indice) => new() { Numero = 0, Columna = indice % 7 };

    public static string NombreDia(int columna) => Nombres[columna];
}