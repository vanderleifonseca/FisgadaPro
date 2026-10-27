using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
// Executar na raiz do repositório. Armazena somente hashes no user-secrets da API.
if (Console.IsInputRedirected || !File.Exists("src/FisgadaPro.Api/FisgadaPro.Api.csproj"))
{ Console.Error.WriteLine("Execute em um terminal interativo, na raiz do FisgadaPro."); return 1; }
var valores = new Dictionary<string,string>();
var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
foreach (var perfil in new[] { "Administrador", "Socio", "Plataforma" })
{
    var padrao = perfil.ToLowerInvariant() + "@fisgada.test";
    Console.Write($"E-mail {perfil} (Enter = {padrao}): ");
    var email = Console.ReadLine()?.Trim();
    email = string.IsNullOrEmpty(email) ? padrao : email;
    if (!email.Contains('@') || !emails.Add(email)) { Console.Error.WriteLine("E-mail inválido ou repetido."); return 1; }
    Console.Write($"Senha de {perfil} (mínimo 4 caracteres): ");
    var senha = LerSenha();
    if (senha.Length < 4 || senha.Length > 256) { Console.Error.WriteLine("Use entre 4 e 256 caracteres."); return 1; }
    Console.Write("Confirme a senha: ");
    if (LerSenha() != senha) { Console.Error.WriteLine("As senhas não coincidem."); return 1; }
    valores[$"Acessos:{perfil}:Email"] = email;
    valores[$"Acessos:{perfil}:SenhaHash"] = new PasswordHasher<object>().HashPassword(new object(), senha);
}
var start = new ProcessStartInfo("dotnet") { RedirectStandardInput = true, UseShellExecute = false };
foreach (var arg in new[] { "user-secrets", "set", "--project", "src/FisgadaPro.Api" }) start.ArgumentList.Add(arg);
using var process = Process.Start(start)!;
await process.StandardInput.WriteAsync(JsonSerializer.Serialize(valores));
process.StandardInput.Close();
await process.WaitForExitAsync();
if (process.ExitCode != 0) return process.ExitCode;
Console.WriteLine("Contas de teste configuradas. Reinicie a API. Senhas não foram gravadas no repositório.");
return 0;

static string LerSenha()
{
    var sb = new StringBuilder();
    while (true)
    {
        var key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.Enter) { Console.WriteLine(); return sb.ToString(); }
        if (key.Key == ConsoleKey.Backspace) { if (sb.Length > 0) sb.Length--; }
        else if (!char.IsControl(key.KeyChar) && sb.Length < 257) sb.Append(key.KeyChar);
    }
}
