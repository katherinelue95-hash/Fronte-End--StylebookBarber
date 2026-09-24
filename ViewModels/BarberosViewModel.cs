using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StyleBookBarberApp.Helpers;
using StyleBookBarberApp.Models;

namespace StyleBookBarberApp.ViewModels;

/// <summary>
/// Agenda del Día del barbero (wireframe Stitch dl3).
/// Progreso de jornada, filtros por estado y citas con acciones INICIAR/FINALIZAR.
/// </summary>
public partial class BarberosViewModel : BaseViewModel
{
    public ObservableCollection<CitaAgenda> Visibles { get; } = new();
    public IReadOnlyList<FiltroCategoria> Filtros { get; }

    private readonly List<CitaAgenda> _todas;

    public string TituloProgreso { get; } = "Jornada al 25% (1 de 4 turnos completados)";
    public string LigeraDerecha { get; } = "3 PENDIENTES";
    public string Especialista { get; } = "ESPECIALISTA";
    public string Sillon { get; } = "Sillón Principal · 01";
    public string Subtitulo { get; } = "Carlos Mendoza · Sesión Mañana";
    public string ContadorHoy { get; } = "4 Citas Hoy";

    public BarberosViewModel()
    {
        Title = "Barbero";

        _todas =
        [
            new("Alejandro Cruz", esVip: true, verificado: false, "Corte Clásico · 45 min",
                "09:00", "AM", IconGlyphs.Done, "Completada",
                "Servicio Finalizado", string.Empty, string.Empty, false, false),

            new("Marcos Delgado", esVip: false, verificado: true, "Corte Degradado Clásico",
                "10:00", "AM", IconGlyphs.Sync, "Proceso",
                "En sillón · iniciado hace 15m", "FINALIZAR SERVICIO", IconGlyphs.TaskAlt, true, true),

            new("Andrés Morales", esVip: false, verificado: false, "Perfilado y Afeitado de Barba · 30 min",
                "11:00", "AM", IconGlyphs.Schedule, "Pendiente",
                string.Empty, "INICIAR CITA", IconGlyphs.PlayArrow, true, true),

            new("Gabriel Torres", esVip: false, verificado: false, "Tratamiento Capilar & Corte · 50 min",
                "12:15", "PM", IconGlyphs.Event, "Pendiente",
                string.Empty, "TURNO PROGRAMADO", IconGlyphs.LockClock, true, false),
        ];

        Filtros = new List<FiltroCategoria>
        {
            new() { Nombre = "TODAS", Count = 4, EstaActivo = true },
            new() { Nombre = "PENDIENTES", Count = 3 },
            new() { Nombre = "COMPLETADAS", Count = 1 },
        };

        AplicarFiltro("TODAS");
    }

    [RelayCommand]
    private void Filtrar(string filtro)
    {
        foreach (var f in Filtros)
            f.EstaActivo = f.Nombre == filtro;
        AplicarFiltro(filtro);
    }

    private void AplicarFiltro(string filtro)
    {
        Visibles.Clear();
        foreach (var c in _todas.Where(c =>
                     filtro == "TODAS" ||
                     (filtro == "PENDIENTES" && c.Estado != "Completada") ||
                     (filtro == "COMPLETADAS" && c.Estado == "Completada")))
        {
            Visibles.Add(c);
        }
    }

    [RelayCommand]
    private void IniciarCita(CitaAgenda cita) => cita.Iniciar();

    [RelayCommand]
    private void FinalizarServicio(CitaAgenda cita) => cita.Finalizar();

    [RelayCommand]
    private void Accion(CitaAgenda cita)
    {
        if (!cita.AccionHabilitada)
            return;

        if (cita.AccionTexto == "INICIAR CITA")
            cita.Iniciar();
        else if (cita.AccionTexto == "FINALIZAR SERVICIO")
            cita.Finalizar();
    }
}