using System.Net.Http.Json;
using StyleBookBarberApp.Models;

namespace StyleBookBarberApp.Services;

/// <summary>Consume el módulo Usuarios del backend (/api/usuarios).</summary>
public class UsuariosServices
{
    private readonly HttpClient _http;

    public UsuariosServices(HttpClient http) => _http = http;

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