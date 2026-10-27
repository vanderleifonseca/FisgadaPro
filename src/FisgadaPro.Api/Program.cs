using FisgadaPro.Application;
using FisgadaPro.Infrastructure;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDadosDemonstracao, DadosDemonstracao>();
builder.Services.AddSingleton<ConsultaDemonstracao>();
var app = builder.Build();
app.MapGet("/api/v1/status", () => new {
    sistema = "Fisgada Pro", versao = "0.1.0",
    mensagem = "API em funcionamento", ambiente = app.Environment.EnvironmentName,
    demonstracao = app.Environment.IsDevelopment(), dataUtc = DateTimeOffset.UtcNow
});
// Sem autenticação nesta entrega: dados fictícios acessíveis somente em Development.
if (app.Environment.IsDevelopment())
{
    var grupo = app.MapGroup("/api/v1/demonstracao");
    grupo.MapGet("/clubes", (ConsultaDemonstracao consulta) => consulta.Clubes());
    grupo.MapGet("/clubes/{clubeId:guid}/associados", (Guid clubeId, ConsultaDemonstracao consulta) =>
        consulta.Associados(clubeId) is { } itens ? Results.Ok(itens) : Results.NotFound());
    grupo.MapGet("/clubes/{clubeId:guid}/titulares/{titularId:guid}/extrato",
        (Guid clubeId, Guid titularId, ConsultaDemonstracao consulta) =>
            consulta.Extrato(clubeId, titularId) is { } extrato ? Results.Ok(extrato) : Results.NotFound());
}
app.Run();
