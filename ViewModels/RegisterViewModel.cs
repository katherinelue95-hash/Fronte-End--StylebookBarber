using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StyleBookBarberApp.Services;

namespace StyleBookBarberApp.ViewModels;

/// <summary>
/// ViewModel de registro de cliente. El backend asigna el rol Cliente automáticamente.
/// </summary>
public partial class RegisterViewModel : BaseViewModel
{
    private readonly UsuariosServices _usuariosServices;

    public RegisterViewModel(UsuariosServices usuariosServices)
    {
        _usuariosServices = usuariosServices;
        Title = "Crear cuenta";
    }

    [ObservableProperty]
    private string nombre = string.Empty;

    [ObservableProperty]
    private string apellido = string.Empty;

    [ObservableProperty]
    private string correo = string.Empty;

    [ObservableProperty]
    private string contrasena = string.Empty;

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
    private async Task RegistrarAsync()
    {
        ErrorMessage = null;

        if (!EsValido()) return;

        IsBusy = true;
        try
        {
            var usuario = await _usuariosServices.RegisterAsync(
                Nombre.Trim(), Apellido.Trim(), Correo.Trim(), Contrasena);

            if (usuario is null)
            {
                ErrorMessage = "No se pudo completar el registro. Revisa tus datos o prueba con otro correo.";
                return;
            }

            await Shell.Current.DisplayAlertAsync(
                "Cuenta creada",
                "Tu cuenta de Cliente se creó correctamente. Ya puedes iniciar sesión.",
                "Aceptar");

            await Shell.Current.GoToAsync("//LoginPage");
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
    private async Task VolverAsync()
        => await Shell.Current.GoToAsync("//LoginPage");

    private bool EsValido()
    {
        if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido))
        {
            ErrorMessage = "Ingresa tu nombre y apellido para continuar.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Correo) || !Correo.Contains('@') || !Correo.Contains('.'))
        {
            ErrorMessage = "Ingresa un correo electrónico válido.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Contrasena) || Contrasena.Length < 6)
        {
            ErrorMessage = "La contraseña debe tener al menos 6 caracteres.";
            return false;
        }

        return true;
    }
}