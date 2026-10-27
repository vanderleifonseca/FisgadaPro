using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Antiforgery;
using FisgadaPro.Contracts;
using FisgadaPro.Web.Components;
using FisgadaPro.Web.Services;
var builder = WebApplication.CreateBuilder(args);
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("pt-BR");
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o => {
    o.LoginPath = "/entrar"; o.ExpireTimeSpan = TimeSpan.FromMinutes(20); o.SlidingExpiration = false;
    o.Cookie.HttpOnly = true; o.Cookie.SameSite = SameSiteMode.Strict; o.Cookie.Name = "FisgadaPro.Web";
});
builder.Services.AddAuthorization();
builder.Services.AddHttpClient("api", c => {
    c.BaseAddress = new Uri(builder.Configuration["ApiUrl"] ?? "http://localhost:5187");
    c.Timeout = TimeSpan.FromSeconds(15);
});
builder.Services.AddScoped<ApiAutenticada>();
var app = builder.Build();
if (!app.Environment.IsDevelopment())
    throw new InvalidOperationException("Esta entrega usa contas e dados fictícios. Execute em Development.");
app.UseAuthentication(); app.UseAuthorization(); app.UseAntiforgery();
app.MapStaticAssets();
app.MapPost("/autenticacao/entrar", async (HttpContext ctx, IAntiforgery csrf, IHttpClientFactory factory) => {
    try { await csrf.ValidateRequestAsync(ctx); }
    catch (AntiforgeryValidationException) { return Results.BadRequest(); }
    var form = await ctx.Request.ReadFormAsync();
    try {
        using var client = factory.CreateClient("api");
        using var login = await client.PostAsJsonAsync("/api/v1/login",
            new LoginRequest(form["email"].ToString(), form["senha"].ToString()));
        if (!login.IsSuccessStatusCode) return Results.LocalRedirect("/entrar?erro=credenciais");
        var token = await login.Content.ReadFromJsonAsync<TokenResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);
        var sessao = await client.GetFromJsonAsync<SessaoDto>("/api/v1/sessao");
        var principal = new ClaimsPrincipal(new ClaimsIdentity([
            new(ClaimTypes.Name, sessao!.Nome), new(ClaimTypes.Role, sessao.Perfil),
            new("api_token", token.AccessToken)
        ], CookieAuthenticationDefaults.AuthenticationScheme));
        await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties { IsPersistent = false, ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn) });
        return Results.LocalRedirect("/");
    } catch (HttpRequestException) { return Results.LocalRedirect("/entrar?erro=api"); }
      catch (TaskCanceledException) { return Results.LocalRedirect("/entrar?erro=api"); }
});
app.MapPost("/autenticacao/sair", async (HttpContext ctx, IAntiforgery csrf) => {
    try { await csrf.ValidateRequestAsync(ctx); }
    catch (AntiforgeryValidationException) { return Results.BadRequest(); }
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.LocalRedirect("/entrar");
});
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
