namespace StyleBookBarberApp.Models;

/// <summary>Estrella de la calificación de un barbero (rellena según la nota).</summary>
public class Estrella
{
    public int Valor { get; init; }
    public Color Color { get; init; } = Colors.White;
}