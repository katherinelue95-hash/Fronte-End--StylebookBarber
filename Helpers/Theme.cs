namespace StyleBookBarberApp.Helpers;

/// <summary>
/// Acceso tipado a los recursos de color del tema (Colors.xaml).
/// Permite a modelos/ViewModels calcular colores dinámicos por estado.
/// </summary>
public static class Theme
{
    public static Color Get(string key)
    {
        if (Application.Current != null &&
            Application.Current.Resources.TryGetValue(key, out var value) &&
            value is Color color)
        {
            return color;
        }

        return Color.Parse("#D4AF37");
    }
}