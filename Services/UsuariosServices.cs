using System.Net.Http.Json;
using StyleBookBarberApp.Models;

namespace StyleBookBarberApp.Services;

/// <summary>Consume el módulo Usuarios del backend (/api/usuarios).</summary>
public class UsuariosServices
{
    private readonly HttpClient _http;

    public UsuariosServices(HttpClient http) => _http = http;

    public async Task<Usuario?> LoginAsync(string correo, string password)
    {
        var response = await _http.PostAsJsonAsync("/api/auth/login", new { correo, passwordHash = password });

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Usuario>();
    }

    /// <summary>Registra un cliente nuevo. El backend asigna el rol Cliente (RolId = 1) automáticamente.</summary>
    public async Task<Usuario?> RegisterAsync(string nombre, string apellido, string correo, string password)
    {
        var response = await _http.PostAsJsonAsync("/api/usuarios", new
        {
            nombre,
            apellido,
            correo,
            passwordHash = password
        });

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<Usuario>();
    }

    public async Task<List<Usuario>> GetUsuariosAsync()
        => await _http.GetFromJsonAsync<List<Usuario>>("/api/usuarios") ?? new List<Usuario>();

    public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        => await _http.GetFromJsonAsync<Usuario>($"/api/usuarios/{id}");

    public async Task<Usuario?> CambiarRolAsync(int id, int rolId)
    {
        var response = await _http.PutAsJsonAsync($"/api/usuarios/{id}/rol", new { rolId });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Usuario>();
    }
}