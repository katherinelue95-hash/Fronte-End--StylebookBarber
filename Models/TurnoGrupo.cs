using System.Collections.ObjectModel;

namespace StyleBookBarberApp.Models;

/// <summary>Grupo de turnos (Mañana / Tarde / Noche) con su franja horaria.</summary>
public class TurnoGrupo
{
    public string Nombre { get; init; } = string.Empty;
    public string Icono { get; init; } = string.Empty;
    public Color IconoColor { get; init; } = Colors.White;
    public string Rango { get; init; } = string.Empty;
    public ObservableCollection<HorarioSlot> Slots { get; } = new();
}