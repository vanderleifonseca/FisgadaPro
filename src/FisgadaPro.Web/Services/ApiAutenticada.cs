using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using FisgadaPro.Contracts;
namespace FisgadaPro.Web.Services;
public sealed class ApiAutenticada(IHttpClientFactory factory, AuthenticationStateProvider auth)
{
    public async Task<T> Get<T>(string path)
    {
        var state = await auth.GetAuthenticationStateAsync();
        var token = state.User.FindFirst("api_token")?.Value;
        if (string.IsNullOrEmpty(token)) throw new HttpRequestException("Sessão encerrada.");
        using var client = factory.CreateClient("api");
        using var req = new HttpRequestMessage(HttpMethod.Get, path);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = await client.SendAsync(req);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }
}
