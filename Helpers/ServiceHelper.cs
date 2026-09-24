namespace StyleBookBarberApp.Helpers;

/// <summary>
/// Punto de acceso al contenedor de DI desde las páginas (patrón Service Locator).
/// Los ViewModels se resuelven aquí, manteniendo MVVM limpio sin new dispersos.
/// </summary>
public static class ServiceHelper
{
    private static IServiceProvider? _services;

    public static void Initialize(IServiceProvider services) => _services = services;

    public static T GetService<T>() where T : notnull
    {
        if (_services is null)
            throw new InvalidOperationException("ServiceHelper no inicializado (App).");

        return _services.GetRequiredService<T>();
    }
}