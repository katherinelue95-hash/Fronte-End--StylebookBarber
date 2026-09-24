using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace StyleBookBarberApp.ViewModels;

/// <summary>
/// Perfil del usuario: datos personales, foto (MediaPicker) y cerrar sesión.
/// </summary>
public partial class PerfilViewModel : BaseViewModel
{
    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string correo = string.Empty;

    [ObservableProperty]
    private string rol = "Cliente";

    [ObservableProperty]
    private string? fotoPath;

    public string RolEtiqueta => Rol.Equals("Barbero", StringComparison.OrdinalIgnoreCase) ? "BARBERO" : "CLIENTE VIP";

    public PerfilViewModel()
    {
        Title = "Perfil";
        CargarDatos();
    }

    private void CargarDatos()
    {
        Nombre = Preferences.Default.Get("UserNombre", "Cliente Estilos");
        Correo = Preferences.Default.Get("UserCorreo", "cliente@stylebook.app");
        Rol = Preferences.Default.Get("UserRole", "Cliente");
        FotoPath = Preferences.Default.Get("UserFoto", string.Empty);
        if (string.IsNullOrWhiteSpace(FotoPath))
            FotoPath = null;
    }

    partial void OnFotoPathChanged(string? value)
    {
        OnPropertyChanged(nameof(HayFoto));
        OnPropertyChanged(nameof(SinFoto));
    }

    public bool HayFoto => !string.IsNullOrEmpty(FotoPath);
    public bool SinFoto => string.IsNullOrEmpty(FotoPath);

    [RelayCommand]
    private async Task ElegirFotoAsync()
    {
        try
        {
            var fotos = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions
            {
                Title = "Selecciona tu foto de perfil",
            });
            var foto = fotos.FirstOrDefault();

            if (foto is null)
                return;

            var destinoDir = Path.Combine(FileSystem.AppDataDirectory, "perfil");
            Directory.CreateDirectory(destinoDir);

            var destino = Path.Combine(destinoDir, $"{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(foto.FileName)}");
            using (var origen = await foto.OpenReadAsync())
            using (var destinoStream = File.Create(destino))
                await origen.CopyToAsync(destinoStream);

            FotoPath = destino;
            Preferences.Default.Set("UserFoto", destino);
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync(
                "Foto de perfil",
                "No se pudo cargar la imagen. Intenta con otra foto.",
                "Entendido");
        }
    }

    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        Preferences.Default.Remove("UserId");
        Preferences.Default.Remove("UserNombre");
        Preferences.Default.Remove("UserCorreo");
        Preferences.Default.Remove("UserRole");
        Preferences.Default.Remove("UserFoto");
        await Shell.Current.GoToAsync("//LoginPage");
    }
}