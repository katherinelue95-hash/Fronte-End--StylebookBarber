using System.Net.Http.Json;
using StyleBookBarberApp.Models;

namespace StyleBookBarberApp.Services;

/// <summary>Consume el módulo Barberos del backend (/api/barberos).</summary>
public class BarberosServices
{
    private readonly HttpClient _http;

    public BarberosServices(HttpClient http) => _http = http;

    public async Task<List<Barberos>> GetBarberosAsync()
        => await _http.GetFromJsonAsync<List<Barberos>>("/api/barberos") ?? new List<Barberos>();

    public async Task<Barberos?> GetBarberoByIdAsync(int id)
        => await _http.GetFromJsonAsync<Barberos>($"/api/barberos/{id}");
}