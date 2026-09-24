using System.Net.Http.Json;
using StyleBookBarberApp.Models;

namespace StyleBookBarberApp.Services;

/// <summary>Consume el módulo Citas del backend (/api/citas).</summary>
public class CitasServices
{
    private readonly HttpClient _http;

    public CitasServices(HttpClient http) => _http = http;

    public async Task<List<Cita>> GetCitasAsync()
        => await _http.GetFromJsonAsync<List<Cita>>("/api/citas") ?? new List<Cita>();

    public async Task<Cita?> GetCitaByIdAsync(int id)
        => await _http.GetFromJsonAsync<Cita>($"/api/citas/{id}");

    public async Task<Cita?> CrearCitaAsync(Cita cita)
    {
        var response = await _http.PostAsJsonAsync("/api/citas", cita);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Cita>();
    }

    public async Task<Cita?> CambiarEstadoAsync(int id, string estado)
    {
        var response = await _http.PatchAsJsonAsync($"/api/citas/{id}/estado", new { estado });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Cita>();
    }

    public async Task<bool> EliminarCitaAsync(int id)
    {
        var response = await _http.DeleteAsync($"/api/citas/{id}");
        return response.StatusCode is System.Net.HttpStatusCode.NoContent or System.Net.HttpStatusCode.OK;
    }
}