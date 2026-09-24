namespace StyleBookBarberApp.Models;

/// <summary>DTO de Servicio que espeja el backend (/api/servicios).</summary>
public class Servicio
{
    public int ServiciosId { get; set; }
    public int CategoriaId { get; set; }
    public string NombreServicio { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int DuracionMin { get; set; }
    public string? FotoUrl { get; set; }
    public string NombreCategoria { get; set; } = string.Empty;
}