using System.Net.Http.Json;
using BroadcastServer.Models;

namespace BroadcastServer.Services;

public class AuthService(string baseUrl, HttpClient? httpClient = null)
{
    private readonly HttpClient _httpClient = httpClient ?? new HttpClient();
    private readonly string _loginUrl = $"{baseUrl}/login-users/login";

    public async Task<string?> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync(_loginUrl, new LoginRequestDto(email, password));

        if (!response.IsSuccessStatusCode) return null;
        var data = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        return data?.Token;
    }
}