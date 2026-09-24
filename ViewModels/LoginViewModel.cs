using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StyleBookBarberApp.Services;

namespace StyleBookBarberApp.ViewModels;

/// <summary>
/// ViewModel de Iniciar Sesión (fiel al wireframe Stitch zip7).
/// </summary>
public partial class LoginViewModel : BaseViewModel
{
    private readonly UsuariosServices _usuariosServices;

    public LoginViewModel(UsuariosServices usuariosServices)
    {
        _usuariosServices = usuariosServices;
        Title = "Iniciar Sesión";
    }

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isPassword = true;

    [ObservableProperty]
    private string selectedRole = "Cliente";

    [ObservableProperty]
    private string? errorMessage;

    // ===== Estado visual del segmentado Cliente / Barbero / Administrativo =====
    public Color ClienteBgColor => SelectedRole == "Cliente" ? ResourceColor("SurfaceContainerHigh") : Colors.Transparent;
    public Color ClienteTextColor => SelectedRole == "Cliente" ? ResourceColor("PrimaryBright") : ResourceColor("OnSurfaceVariant");
    public Color BarberoBgColor => SelectedRole == "Barbero" ? ResourceColor("SurfaceContainerHigh") : Colors.Transparent;
    public Color BarberoTextColor => SelectedRole == "Barbero" ? ResourceColor("PrimaryBright") : ResourceColor("OnSurfaceVariant");
    public Color AdminBgColor => SelectedRole == "Administrativo" ? ResourceColor("SurfaceContainerHigh") : Colors.Transparent;
    public Color AdminTextColor => SelectedRole == "Administrativo" ? ResourceColor("PrimaryBright") : ResourceColor("OnSurfaceVariant");

    partial void OnSelectedRoleChanged(string value)
    {
        OnPropertyChanged(nameof(ClienteBgColor));
        OnPropertyChanged(nameof(ClienteTextColor));
        OnPropertyChanged(nameof(BarberoBgColor));
        OnPropertyChanged(nameof(BarberoTextColor));
        OnPropertyChanged(nameof(AdminBgColor));
        OnPropertyChanged(nameof(AdminTextColor));
    }

    [RelayCommand]
    private void SelectCliente()
    {
        SelectedRole = "Cliente";
        NotificarSegmentos();
    }

    [RelayCommand]
    private void SelectBarbero()
    {
        SelectedRole = "Barbero";
        NotificarSegmentos();
    }

    [RelayCommand]
    private void SelectAdmin()
    {
        SelectedRole = "Administrativo";
        NotificarSegmentos();
    }

    private void NotificarSegmentos()
    {
        OnPropertyChanged(nameof(ClienteBgColor));
        OnPropertyChanged(nameof(ClienteTextColor));
        OnPropertyChanged(nameof(BarberoBgColor));
        OnPropertyChanged(nameof(BarberoTextColor));
        OnPropertyChanged(nameof(AdminBgColor));
        OnPropertyChanged(nameof(AdminTextColor));
    }

    public string VisibilityGlyph => IsPassword ? "\uE8F4" : "\uE8F5"; // visibility / visibility_off

    partial void OnIsPasswordChanged(bool value) => OnPropertyChanged(nameof(VisibilityGlyph));

    [RelayCommand]
    private void TogglePassword()
    {
        IsPassword = !IsPassword;
        OnPropertyChanged(nameof(VisibilityGlyph));
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Ingresa tu correo y contraseña para continuar.";
            return;
        }

        IsBusy = true;
        try
        {
            var usuarios = await _usuariosServices.GetUsuariosAsync();
            var usuario = usuarios.FirstOrDefault(u =>
                string.Equals(u.Correo, Email.Trim(), StringComparison.OrdinalIgnoreCase) &&
                u.PasswordHash == Password);

            if (usuario is null)
            {
                ErrorMessage = "Credenciales incorrectas. Verifica tu correo y contraseña.";
                return;
            }

            // Sesión local
            Preferences.Default.Set("UserId", usuario.UsuariosId);
            Preferences.Default.Set("UserNombre", $"{usuario.Nombre} {usuario.Apellido}".Trim());
            Preferences.Default.Set("UserCorreo", usuario.Correo);
            Preferences.Default.Set("UserRole", SelectedRole);

            if (SelectedRole == "Administrativo")
            {
                await Shell.Current.GoToAsync("//MainTab");
                await Shell.Current.GoToAsync("Admin");
            }
            else
            {
                await Shell.Current.GoToAsync("//MainTab");
            }
        }
        catch (Exception)
        {
            ErrorMessage = "No se pudo conectar con el servidor. Verifica tu conexión.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ShowInfo()
        => ErrorMessage = "La recuperación de contraseña y el registro se habilitarán próximamente.";
}