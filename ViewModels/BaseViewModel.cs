using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace StyleBookBarberApp.ViewModels;

/// <summary>
/// Base MVVM para todos los ViewModels del proyecto.
/// </summary>
public abstract partial class BaseViewModel : ObservableObject
{
    private bool _isBusy;
    private string _title = string.Empty;

    /// <summary>True mientras una operación asíncrona está en curso.</summary>
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    /// <summary>Lee un Color de los recursos de la app según su clave.</summary>
    protected static Color ResourceColor(string key)
        => App.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color
            ? color
            : Colors.DarkGray;

    /// <summary>Abre la pantalla de notificaciones (ícono campana de los headers).</summary>
    [RelayCommand]
    private async Task AbrirNotificacionesAsync()
        => await Shell.Current.GoToAsync("Notificaciones");

    /// <summary>Abre la pantalla de perfil (ícono avatar de los headers).</summary>
    [RelayCommand]
    private async Task AbrirPerfilAsync()
        => await Shell.Current.GoToAsync("Perfil");
}