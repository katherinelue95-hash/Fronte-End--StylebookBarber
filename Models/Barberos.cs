namespace StyleBookBarberApp.Models;

/// <summary>DTO de Barbero que espeja el backend (GET /api/barberos).</summary>
public class Barberos
{
    public int BarberosId { get; set; }
    public decimal Calificacion { get; set; }
    public string EstadoDisp { get; set; } = "Disponible";
    public string? FotoUrl { get; set; }
    public string Especialidad { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
}