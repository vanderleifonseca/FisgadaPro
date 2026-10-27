using FisgadaPro.Web.Components;
using FisgadaPro.Application;
using FisgadaPro.Infrastructure;
using System.Globalization;
var builder = WebApplication.CreateBuilder(args);
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("pt-BR");
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddSingleton<IDadosDemonstracao, DadosDemonstracao>();
builder.Services.AddSingleton<ConsultaDemonstracao>();
var app = builder.Build();
if (!app.Environment.IsDevelopment())
    throw new InvalidOperationException("Protótipo local: autenticação e autorização ainda não implementadas. Execute em Development.");
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
