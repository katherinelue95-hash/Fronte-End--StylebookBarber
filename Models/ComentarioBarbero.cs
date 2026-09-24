namespace StyleBookBarberApp.Models;

/// <summary>Comentario de un cliente sobre un barbero.</summary>
public class ComentarioBarbero
{
    public string Autor { get; init; } = string.Empty;
    public string Inicial { get; init; } = "?";
    public string Calificacion { get; init; } = "5.0";
    public string Texto { get; init; } = string.Empty;
    public string Fecha { get; init; } = string.Empty;
    public int EstrellasLlenas { get; init; } = 5;
}