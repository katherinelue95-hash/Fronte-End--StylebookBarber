using System.Net.Http.Json;
using StyleBookBarberApp.Models;

namespace StyleBookBarberApp.Services;

/// <summary>Consume el módulo Servicios del backend (/api/servicios).</summary>
public class ServiciosServices
{
    private readonly HttpClient _http;

    public ServiciosServices(HttpClient http) => _http = http;

    public async Task<List<Servicio>> GetServiciosAsync()
        => await _http.GetFromJsonAsync<List<Servicio>>("/api/servicios") ?? new List<Servicio>();

    public async Task<Servicio?> GetServicioByIdAsync(int id)
        => await _http.GetFromJsonAsync<Servicio>($"/api/servicios/{id}");

    public async Task<Servicio?> CrearServicioAsync(Servicio servicio)
    {
        var response = await _http.PostAsJsonAsync("/api/servicios", servicio);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Servicio>();
    }

    public async Task<Servicio?> ActualizarServicioAsync(int id, Servicio servicio)
    {
        var response = await _http.PutAsJsonAsync($"/api/servicios/{id}", servicio);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Servicio>();
    }

    public async Task<bool> EliminarServicioAsync(int id)
    {
        var response = await _http.DeleteAsync($"/api/servicios/{id}");
        return response.StatusCode == System.Net.HttpStatusCode.NoContent ||
               response.StatusCode == System.Net.HttpStatusCode.OK;
    }
}