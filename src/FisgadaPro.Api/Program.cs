using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.RateLimiting;
using FisgadaPro.Application;
using FisgadaPro.Infrastructure;
using FisgadaPro.Contracts;
using FisgadaPro.Api.Seguranca;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDadosDemonstracao, DadosDemonstracao>();
builder.Services.AddSingleton<ConsultaDemonstracao>();
builder.Services.AddSingleton<ContasLocais>();
builder.Services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
    .AddBearerToken(o => o.BearerTokenExpiration = TimeSpan.FromMinutes(20));
builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(o => {
    o.RejectionStatusCode = 429;
    o.AddPolicy("login", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "local",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
});
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapGet("/api/v1/status", () => new {
    sistema = "Fisgada Pro", versao = "0.2.0", mensagem = "API em funcionamento",
    demonstracao = app.Environment.IsDevelopment(), dataUtc = DateTimeOffset.UtcNow
});
if (app.Environment.IsDevelopment())
{
    app.MapPost("/api/v1/login", async (LoginRequest login, ContasLocais contas, HttpContext ctx) => {
        var conta = contas.Validar(login.Email, login.Senha);
        if (conta is null) { ctx.Response.StatusCode = 401; return; }
        var principal = new ClaimsPrincipal(new ClaimsIdentity([
            new(ClaimTypes.NameIdentifier, conta.Perfil), new(ClaimTypes.Name, conta.Nome),
            new(ClaimTypes.Role, conta.Perfil)
        ], BearerTokenDefaults.AuthenticationScheme));
        await ctx.SignInAsync(BearerTokenDefaults.AuthenticationScheme, principal);
    }).RequireRateLimiting("login");
    var grupo = app.MapGroup("/api/v1").RequireAuthorization();
    grupo.MapGet("/sessao", (HttpContext ctx, ContasLocais contas, ConsultaDemonstracao consulta) => {
        var c = contas.Atual(ctx.User);
        if (c is null) return Results.Unauthorized();
        var clubes = consulta.Clubes().Where(x => c.Perfil == "Plataforma" || x.Id == c.ClubeId).ToArray();
        return Results.Ok(new SessaoDto(c.Nome, c.Perfil, clubes));
    });
    grupo.MapGet("/clubes/{clubeId:guid}/associados", (Guid clubeId, HttpContext ctx,
        ContasLocais contas, ConsultaDemonstracao consulta) => {
        var c = contas.Atual(ctx.User);
        if (c?.Perfil != "Administrador" || c.ClubeId != clubeId) return Results.Forbid();
        return Results.Ok(consulta.Associados(clubeId));
    });
    grupo.MapGet("/clubes/{clubeId:guid}/meu-extrato", (Guid clubeId, HttpContext ctx,
        ContasLocais contas, ConsultaDemonstracao consulta) => {
        var c = contas.Atual(ctx.User);
        if (c?.Perfil != "Socio" || c.ClubeId != clubeId || c.AssociadoId is null) return Results.Forbid();
        return Results.Ok(consulta.Extrato(clubeId, c.AssociadoId.Value));
    });
    grupo.MapGet("/clubes/{clubeId:guid}/titulares/{titularId:guid}/extrato",
        (Guid clubeId, Guid titularId, HttpContext ctx, ContasLocais contas, ConsultaDemonstracao consulta) => {
        var c = contas.Atual(ctx.User);
        if (c?.Perfil != "Administrador" || c.ClubeId != clubeId) return Results.Forbid();
        return consulta.Extrato(clubeId, titularId) is { } e ? Results.Ok(e) : Results.NotFound();
    });
}
app.Run();
