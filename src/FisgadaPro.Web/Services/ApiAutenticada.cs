using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
namespace FisgadaPro.Web.Services;
public sealed class ApiAutenticada(IHttpClientFactory factory,AuthenticationStateProvider auth)
{
    public Task<T> Get<T>(string path)=>Enviar<T>(HttpMethod.Get,path,null);
    public Task<T> Post<T>(string path,object? body=null)=>Enviar<T>(HttpMethod.Post,path,body);
    public Task<T> Put<T>(string path,object body)=>Enviar<T>(HttpMethod.Put,path,body);
    private async Task<T> Enviar<T>(HttpMethod metodo,string path,object? body) {
        var state=await auth.GetAuthenticationStateAsync();var token=state.User.FindFirst("api_token")?.Value;
        if(string.IsNullOrEmpty(token))throw new HttpRequestException("Entre novamente.");
        using var client=factory.CreateClient("api");using var req=new HttpRequestMessage(metodo,path);
        req.Headers.Authorization=new AuthenticationHeaderValue("Bearer",token);
        if(body is not null)req.Content=JsonContent.Create(body);
        using var response=await client.SendAsync(req);
        if(!response.IsSuccessStatusCode) {
            var mensagem=response.StatusCode==System.Net.HttpStatusCode.Forbidden?"Seu perfil não permite esta ação.":"Não foi possível executar. Confira a sessão e a API.";
            try{var erro=await response.Content.ReadFromJsonAsync<JsonElement>();if(erro.TryGetProperty("mensagem",out var m))mensagem=m.GetString()??mensagem;}catch(JsonException){}
            throw new HttpRequestException(mensagem,null,response.StatusCode);
        }
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }
}
