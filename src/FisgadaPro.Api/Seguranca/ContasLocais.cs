using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using FisgadaPro.Infrastructure;
namespace FisgadaPro.Api.Seguranca;

// Contas apenas de desenvolvimento: vínculos fixos com as pessoas fictícias.
public sealed record ContaLocal(string Perfil, string Nome, string Email, string SenhaHash, Guid? ClubeId, Guid? AssociadoId);
public sealed class ContasLocais
{
    private readonly Dictionary<string, ContaLocal> _contas = new(StringComparer.OrdinalIgnoreCase);
    private readonly PasswordHasher<object> _hasher = new();
    private readonly object _usuarioHash = new();
    private readonly string _hashFalso;
    public ContasLocais(IConfiguration config)
    {
        _hashFalso = _hasher.HashPassword(_usuarioHash, Guid.NewGuid().ToString("N"));
        Adicionar("Administrador", "Administração do Clube Lago", DadosDemonstracao.ClubeLago, null);
        Adicionar("Socio", "Alex Exemplo", DadosDemonstracao.ClubeLago, Guid.Parse("20000000-0000-0000-0000-000000000001"));
        Adicionar("Plataforma", "Administração da plataforma", null, null);
        void Adicionar(string perfil, string nome, Guid? clube, Guid? associado)
        {
            var email = config[$"Acessos:{perfil}:Email"]?.Trim();
            var hash = config[$"Acessos:{perfil}:SenhaHash"];
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(hash)) return;
            if (_contas.ContainsKey(email)) throw new InvalidOperationException("Use e-mails diferentes para cada conta local.");
            _contas.Add(email, new(perfil, nome, email, hash, clube, associado));
        }
    }
    public ContaLocal? Validar(string? email, string? senha)
    {
        var conta = _contas.GetValueOrDefault(email?.Trim() ?? "");
        if (string.IsNullOrEmpty(senha) || senha.Length > 256) return null;
        try
        {
            var result = _hasher.VerifyHashedPassword(_usuarioHash, conta?.SenhaHash ?? _hashFalso, senha);
            return conta is not null && result != PasswordVerificationResult.Failed ? conta : null;
        }
        catch (FormatException) { return null; }
    }
    public ContaLocal? Atual(ClaimsPrincipal user) => _contas.Values.FirstOrDefault(c =>
        c.Perfil == user.FindFirstValue(ClaimTypes.NameIdentifier));
}
