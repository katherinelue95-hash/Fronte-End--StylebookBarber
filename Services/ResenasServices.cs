using System.Net.Http.Json;
using StyleBookBarberApp.Models;

namespace StyleBookBarberApp.Services;

/// <summary>Consume el módulo Reseñas del backend (/api/resenas).</summary>
public class ResenasServices
{
    private readonly HttpClient _http;

    public ResenasServices(HttpClient http) => _http = http;

    public async Task<List<Resena>> GetResenasByBarberoAsync(int barberoId)
        => await _http.GetFromJsonAsync<List<Resena>>($"/api/resenas/barbero/{barberoId}") ?? new List<Resena>();

    public async Task<List<Resena>> GetResenasAsync()
        => await _http.GetFromJsonAsync<List<Resena>>("/api/resenas") ?? new List<Resena>();

    public async Task<Resena?> CrearResenaAsync(Resena resena)
    {
        var response = await _http.PostAsJsonAsync("/api/resenas", resena);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<Resena>();
    }

    public async Task<bool> EliminarResenaAsync(int id)
    {
        var response = await _http.DeleteAsync($"/api/resenas/{id}");
        return response.StatusCode is System.Net.HttpStatusCode.NoContent or System.Net.HttpStatusCode.OK;
    }
}