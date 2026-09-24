using StyleBookBarberApp.Helpers;

namespace StyleBookBarberApp.Models;

/// <summary>Ítem de cita para el panel administrativo.</summary>
public class AdminCitaItem
{
    public int CitasId { get; init; }
    public string Cliente { get; init; } = string.Empty;
    public string Barbero { get; init; } = string.Empty;
    public string Servicio { get; init; } = string.Empty;
    public string Fecha { get; init; } = string.Empty;
    public string Precio { get; init; } = "$0.00";
    public string Estado { get; init; } = "Pendiente";
    public string AccionEstado { get; init; } = "MARCAR CONFIRMADA";

    public Color FondoEstado => Estado switch
    {
        "Confirmada" or "En curso" => Theme.Get("Primary"),
        "Cancelada" => Theme.Get("Error"),
        "Completada" => Theme.Get("SurfaceContainerHighest"),
        _ => Theme.Get("SurfaceContainerHighest"),
    };

    public Color TextoEstado => Estado switch
    {
        "Confirmada" or "En curso" or "Cancelada" => Theme.Get("OnPrimary"),
        "Completada" => Theme.Get("OnSurfaceVariant"),
        _ => Theme.Get("Outline"),
    };
}

/// <summary>Ítem de reseña para el panel administrativo.</summary>
public class AdminResenaItem
{
    public int ResenaId { get; init; }
    public string Autor { get; init; } = string.Empty;
    public string Barbero { get; init; } = string.Empty;
    public string Estrellas { get; init; } = "5";
    public string Fecha { get; init; } = string.Empty;
    public string Comentario { get; init; } = string.Empty;
}

/// <summary>Ítem de servicio para el panel administrativo.</summary>
public class AdminServicioItem
{
    public int ServiciosId { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Categoria { get; init; } = string.Empty;
    public string Precio { get; init; } = "$0.00";
    public string Duracion { get; init; } = "30 min";
    public string? FotoUrl { get; init; }

    public bool TieneFoto => !string.IsNullOrWhiteSpace(FotoUrl);
}

/// <summary>Ítem de usuario para el panel administrativo.</summary>
public class AdminUsuarioItem
{
    public int UsuariosId { get; init; }
    public string NombreCompleto { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public string Rol { get; init; } = "Cliente";
    public int RolId { get; init; } = 1;
}