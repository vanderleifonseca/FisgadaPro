using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using FisgadaPro.Contracts;

var root = Directory.GetCurrentDirectory();
var senha = Guid.NewGuid().ToString("N") + "!aA1";
var logs = new List<Process>();
var checks = 0;
int Porta() { var l = new TcpListener(IPAddress.Loopback, 0); l.Start(); var p = ((IPEndPoint)l.LocalEndpoint).Port; l.Stop(); return p; }
var apiUrl = $"http://127.0.0.1:{Porta()}";
var webUrl = $"http://127.0.0.1:{Porta()}";
void Verificar(bool ok, string mensagem) { if (!ok) throw new Exception(mensagem); checks++; }
Process Iniciar(string projeto, string url)
{
    var info = new ProcessStartInfo("dotnet") { WorkingDirectory = Path.Combine(root, "src", projeto), RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
    info.ArgumentList.Add(Path.Combine(info.WorkingDirectory, "bin", "Debug", "net10.0", projeto + ".dll"));
    info.ArgumentList.Add("--urls"); info.ArgumentList.Add(url);
    info.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
    info.Environment["ApiUrl"] = apiUrl;
    foreach (var perfil in new[] { "Administrador", "Socio", "Plataforma" })
    {
        info.Environment[$"Acessos__{perfil}__Email"] = perfil.ToLowerInvariant() + "@fisgada.test";
        info.Environment[$"Acessos__{perfil}__SenhaHash"] = new PasswordHasher<object>().HashPassword(new object(), senha);
    }
    var p = Process.Start(info)!;
    p.OutputDataReceived += (_, _) => { }; p.ErrorDataReceived += (_, _) => { };
    p.BeginOutputReadLine(); p.BeginErrorReadLine(); logs.Add(p); return p;
}
async Task Aguardar(string url) {
    using var c = new HttpClient();
    for (var i=0;i<80;i++) { try { using var r = await c.GetAsync(url); return; } catch (HttpRequestException) { await Task.Delay(200); } }
    throw new Exception("Servidor não iniciou: " + url);
}
async Task<HttpClient> Entrar(string perfil) {
    var c = new HttpClient { BaseAddress = new Uri(apiUrl) };
    using var r = await c.PostAsJsonAsync("/api/v1/login", new LoginRequest(perfil.ToLowerInvariant()+"@fisgada.test", senha));
    Verificar(r.IsSuccessStatusCode, "Login falhou: "+perfil);
    var t = await r.Content.ReadFromJsonAsync<TokenResponse>();
    c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", t!.AccessToken);
    return c;
}
async Task Codigo(HttpClient c, string path, HttpStatusCode esperado) {
    using var r=await c.GetAsync(path); Verificar(r.StatusCode==esperado, path+": "+r.StatusCode);
}
try
{
    Iniciar("FisgadaPro.Api", apiUrl); Iniciar("FisgadaPro.Web", webUrl);
    await Aguardar(apiUrl+"/api/v1/status"); await Aguardar(webUrl+"/entrar");
    const string lago="10000000-0000-0000-0000-000000000001", rio="10000000-0000-0000-0000-000000000002", alex="20000000-0000-0000-0000-000000000001";
    using var anon=new HttpClient{BaseAddress=new Uri(apiUrl)};
    await Codigo(anon,"/api/v1/sessao",HttpStatusCode.Unauthorized);
    await Codigo(anon,$"/api/v1/clubes/{lago}/associados",HttpStatusCode.Unauthorized);
    await Codigo(anon,"/api/v1/demonstracao/clubes",HttpStatusCode.NotFound);
    using var socio=await Entrar("Socio");
    var s=await socio.GetFromJsonAsync<SessaoDto>("/api/v1/sessao");
    Verificar(s!.Clubes.Count==1 && s.ClubeUnicoId()==Guid.Parse(lago), "Clubes do sócio incorretos.");
    var e=await socio.GetFromJsonAsync<ExtratoDto>($"/api/v1/clubes/{lago}/meu-extrato");
    Verificar(e!.Total==255 && e.Despesas.All(d=>d.Consumidor=="Bia Exemplo"), "Extrato familiar incorreto.");
    await Codigo(socio,$"/api/v1/clubes/{lago}/associados",HttpStatusCode.Forbidden);
    await Codigo(socio,$"/api/v1/clubes/{rio}/meu-extrato",HttpStatusCode.Forbidden);
    await Codigo(socio,$"/api/v1/clubes/{lago}/titulares/{alex}/extrato",HttpStatusCode.Forbidden);
    using var admin=await Entrar("Administrador");
    await Codigo(admin,$"/api/v1/clubes/{lago}/associados",HttpStatusCode.OK);
    await Codigo(admin,$"/api/v1/clubes/{rio}/associados",HttpStatusCode.Forbidden);
    await Codigo(admin,$"/api/v1/clubes/{lago}/titulares/{alex}/extrato",HttpStatusCode.OK);
    using var plataforma=await Entrar("Plataforma");
    var p=await plataforma.GetFromJsonAsync<SessaoDto>("/api/v1/sessao");
    Verificar(p!.Clubes.Count==2,"Plataforma deve listar clubes.");
    await Codigo(plataforma,$"/api/v1/clubes/{lago}/associados",HttpStatusCode.Forbidden);
    using var errado=await anon.PostAsJsonAsync("/api/v1/login",new LoginRequest("socio@fisgada.test","incorreta"));
    Verificar(errado.StatusCode==HttpStatusCode.Unauthorized,"Senha incorreta aceita.");
    using var web=new HttpClient(new HttpClientHandler{CookieContainer=new CookieContainer(),AllowAutoRedirect=false}){BaseAddress=new Uri(webUrl)};
    using var semCsrf=await web.PostAsync("/autenticacao/entrar",new FormUrlEncodedContent(new Dictionary<string,string>{{"email","socio@fisgada.test"},{"senha",senha}}));
    Verificar(semCsrf.StatusCode==HttpStatusCode.BadRequest,"Login sem antiforgery: status inesperado " + semCsrf.StatusCode);
    var html=await web.GetStringAsync("/entrar");
    var csrf=WebUtility.HtmlDecode(Regex.Match(html,"name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value);
    Verificar(!string.IsNullOrEmpty(csrf),"Token antiforgery ausente.");
    using var login=await web.PostAsync("/autenticacao/entrar",new FormUrlEncodedContent(new Dictionary<string,string>{{"email","socio@fisgada.test"},{"senha",senha},{"__RequestVerificationToken",csrf}}));
    Verificar(login.StatusCode==HttpStatusCode.Redirect && login.Headers.Location?.ToString()=="/","Login web falhou.");
    var home=WebUtility.HtmlDecode(await web.GetStringAsync("/"));
    Verificar(home.Contains("Área do associado") && home.Contains("Bia Exemplo") && !home.Contains("Associados do seu clube"),"Tela do sócio incorreta.");
    using var limitado=await anon.PostAsJsonAsync("/api/v1/login",new LoginRequest("socio@fisgada.test","incorreta"));
    Verificar((int)limitado.StatusCode==429,"Limite de tentativas ausente.");
    Console.WriteLine($"{checks} verificações de acesso e fluxo HTTP concluídas com sucesso.");
}
finally { foreach (var p in logs) { if (!p.HasExited) p.Kill(entireProcessTree:true); p.Dispose(); } }

static class ExtensoesSessao { public static Guid ClubeUnicoId(this SessaoDto s) => s.Clubes.Single().Id; }
