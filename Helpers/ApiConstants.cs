namespace StyleBookBarberApp.Helpers;

/// <summary>
/// Constantes de conexión con el backend StyleBookBarberBD.
/// Emulador Android accede al host a través de 10.0.2.2.
/// Dispositivo físico por USB: 127.0.0.1 con túnel `adb reverse tcp:5211 tcp:5211`.
/// </summary>
public static class ApiConstants
{
    public static string BaseUrl =>
        DeviceInfo.Platform == DevicePlatform.Android
            ? DeviceInfo.DeviceType == DeviceType.Virtual
                ? "http://10.0.2.2:5211"
                : "http://127.0.0.1:5211"
            : "http://localhost:5211";
}