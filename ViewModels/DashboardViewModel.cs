using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace StyleBookBarberApp.ViewModels;

/// <summary>
/// ViewModel del Dashboard (wireframe Stitch zip1). Datos de ejemplo del diseño,
/// recuperables luego desde CitasServices / UsuariosServices.
/// </summary>
public partial class DashboardViewModel : BaseViewModel
{
    [ObservableProperty]
    private string greeting = "Alejandro";

    [ObservableProperty]
    private string serviceName = "Corte Degradado Clásico";

    [ObservableProperty]
    private string barberName = "Carlos Mendoza";

    [ObservableProperty]
    private string barberRank = "Master";

    [ObservableProperty]
    private string appointmentDay = "Viernes, 24 de Octubre";

    [ObservableProperty]
    private string appointmentTime = "4:30 PM (45 mins)";

    [ObservableProperty]
    private string historyCount = "14 cortes";

    [ObservableProperty]
    private string historyCaption = "Último servicio: Barba & Toalla Caliente el 02 Oct";

    public DashboardViewModel()
    {
        Title = "Inicio";
        RefrescarSesion();
    }

    /// <summary>Actualiza el saludo con el nombre de la cuenta logueada (UserNombre).</summary>
    public void RefrescarSesion()
    {
        var stored = Preferences.Default.Get("UserNombre", string.Empty);
        if (string.IsNullOrWhiteSpace(stored))
        {
            Greeting = "Invitado";
            return;
        }

        Greeting = stored.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault() ?? "Cliente";
    }

    [RelayCommand]
    private async Task GoToCatalogoAsync()
        => await Shell.Current.GoToAsync("//MainTab/Catalogo");

    [RelayCommand]
    private async Task GoToHistorialAsync()
        => await Shell.Current.GoToAsync("//MainTab/Citas");

    [RelayCommand]
    private async Task GoToExpressBookingAsync()
        => await Shell.Current.GoToAsync("Horarios");
}