namespace StyleBookBarberApp.Helpers;

/// <summary>
/// Constantes de conexión con el backend StyleBookBarberBD.
/// Android emulador accede al host a través de 10.0.2.2.
/// </summary>
public static class ApiConstants
{
    public static string BaseUrl =>
        DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5211"
            : "http://localhost:5211";
}