namespace StyleBookBarberApp.Models;

/// <summary>DTO de Reseña que espeja el backend (/api/resenas).</summary>
public class Resena
{
    public int ResenaId { get; set; }
    public int BarberosId { get; set; }
    public int UsuariosId { get; set; }
    public int Estrellas { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Autor { get; set; } = string.Empty;
    public string BarberoNombre { get; set; } = string.Empty;

    /// <summary>Calificación promedio recalculada del barbero (respuesta del backend).</summary>
    public decimal CalificacionPromedio { get; set; }
}