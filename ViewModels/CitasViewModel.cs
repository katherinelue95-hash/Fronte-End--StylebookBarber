using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StyleBookBarberApp.Models;
using StyleBookBarberApp.Services;

namespace StyleBookBarberApp.ViewModels;

/// <summary>
/// Pantalla "Mis Citas": próxima cita + historial + botón para agendar.
/// Carga las citas reales desde el backend (/api/citas).
/// </summary>
public partial class CitasViewModel : BaseViewModel
{
    private readonly CitasServices _citasService;

    [ObservableProperty]
    private string proximaServicio = "Sin citas próximas";

    [ObservableProperty]
    private string proximaBarbero = "Agenda tu primera cita";

    [ObservableProperty]
    private string proximaFecha = "Elige un servicio y agenda hoy";

    [ObservableProperty]
    private string proximaPrecio = "$0.00";

    public ObservableCollection<RegistroCita> Historial { get; } = new();

    public string HistorialResumen => $"{Historial.Count} HISTORIAL";

    public CitasViewModel(CitasServices citasService)
    {
        _citasService = citasService;
        Title = "Mis Citas";
    }

    /// <summary>Recarga las citas del cliente desde el backend.</summary>
    public async Task RefrescarCitasAsync()
    {
        var usuarioId = Preferences.Default.Get("UserId", 0);
        try
        {
            var todas = await _citasService.GetCitasAsync();
            var mias = (usuarioId > 0
                    ? todas.Where(c => c.ClienteId == usuarioId)
                    : todas)
                .OrderBy(c => c.FechaHora)
                .ToList();

            Historial.Clear();

            var proxima = mias.FirstOrDefault(c => c.Estado is "Pendiente" or "Confirmada" or "En curso");
            if (proxima is not null)
            {
                ProximaServicio = NombreServicio(proxima);
                ProximaBarbero = NombreBarbero(proxima);
                ProximaFecha = $"{proxima.FechaHora:ddd dd MMM · h:mm tt}";
                ProximaPrecio = $"{proxima.MontoTotal:00.00}".Insert(0, "$");
            }
            else
            {
                ProximaServicio = "Sin citas próximas";
                ProximaBarbero = "Agenda tu primera cita";
                ProximaFecha = "Elige un servicio y agenda hoy";
                ProximaPrecio = "$0.00";
            }

            foreach (var c in mias)
            {
                Historial.Add(new RegistroCita
                {
                    Servicio = NombreServicio(c),
                    Barbero = NombreBarbero(c),
                    Fecha = c.FechaHora.ToString("ddd dd MMM · h:mm tt"),
                    Estado = NormalizarEstado(c.Estado),
                    Precio = $"{c.MontoTotal:00.00}".Insert(0, "$"),
                });
            }

            OnPropertyChanged(nameof(HistorialResumen));
        }
        catch
        {
            // Sin backend: se conserva el estado vacío.
        }
    }

    private static string NombreServicio(Cita c)
        => string.IsNullOrWhiteSpace(c.ServicioNombre)
            ? $"Servicio #{c.ServiciosId}"
            : c.ServicioNombre;

    private static string NombreBarbero(Cita c)
        => string.IsNullOrWhiteSpace(c.BarberoNombre)
            ? $"Barbero #{c.BarberosId}"
            : c.BarberoNombre;

    private static string NormalizarEstado(string estado) => estado.ToUpperInvariant();

    [RelayCommand]
    private async Task AgendarAsync()
        => await Shell.Current.GoToAsync("Horarios");

    [RelayCommand]
    private async Task VerDetalleAsync()
        => await Shell.Current.DisplayAlertAsync(
            "Cita Confirmada",
            $"{ProximaServicio}\n{ProximaBarbero}\n{ProximaFecha}",
            "Cerrar");
}