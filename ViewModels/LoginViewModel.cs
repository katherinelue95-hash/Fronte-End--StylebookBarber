using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StyleBookBarberApp.Services;

namespace StyleBookBarberApp.ViewModels;

/// <summary>
/// ViewModel de Iniciar Sesión (fiel al wireframe Stitch zip7).
/// Solo clientes: los roles Barbero/Admin/Administrativo no se exponen en la interfaz pública.
/// </summary>
public partial class LoginViewModel : BaseViewModel
{
    private const int RolCliente = 1;

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
    private string? errorMessage;

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
            var usuario = await _usuariosServices.LoginAsync(Email.Trim(), Password);

            if (usuario is null)
            {
                ErrorMessage = "Credenciales incorrectas. Verifica tu correo y contraseña.";
                return;
            }

            if (usuario.RolId != RolCliente)
            {
                ErrorMessage = "El acceso está disponible solo para clientes.";
                return;
            }

            // Sesión local
            Preferences.Default.Set("UserId", usuario.UsuariosId);
            Preferences.Default.Set("UserNombre", $"{usuario.Nombre} {usuario.Apellido}".Trim());
            Preferences.Default.Set("UserCorreo", usuario.Correo);
            Preferences.Default.Set("UserRole", "Cliente");

            await Shell.Current.GoToAsync("//MainTab");
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
    private async Task IrARegistroAsync()
        => await Shell.Current.GoToAsync("RegisterPage");

    [RelayCommand]
    private void ShowInfo()
        => ErrorMessage = "La recuperación de contraseña se habilitará próximamente.";
}