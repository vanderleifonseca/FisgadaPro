using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Clube360.Infrastructure;
using Clube360.Contracts;
using Clube360.Application;
namespace Clube360.Api.Seguranca;
public sealed record ContaLocal(string Id,string Perfil,string Nome,string Email,string SenhaHash,Guid? ClubeId,Guid? AssociadoId);
public sealed class ContasLocais
{
    private readonly object _gate=new();
    private readonly IRepositorioClubes _dados;
    private readonly Dictionary<string,ContaLocal> _contas=new(StringComparer.OrdinalIgnoreCase);
    private readonly PasswordHasher<object> _hasher=new();
    private readonly object _usuario=new();
    private readonly string _hashFalso;
    private readonly string _arquivo;
    private readonly List<ContaLocal> _criadas;
    public ContasLocais(IConfiguration config,IWebHostEnvironment env,IRepositorioClubes dados) {
        _dados=dados;
        _hashFalso=_hasher.HashPassword(_usuario,Guid.NewGuid().ToString("N"));
        _arquivo=config["Dados:ContasArquivo"]??Path.Combine(env.ContentRootPath,"App_Data","contas.json");
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_arquivo))!);
        _criadas=File.Exists(_arquivo)?JsonSerializer.Deserialize<List<ContaLocal>>(File.ReadAllText(_arquivo))??throw new InvalidDataException("Contas inválidas."):[];
        foreach(var c in _criadas)_contas.Add(c.Email,c);
        Adicionar("Administrador","Administrador","Administração foz 2",DadosDemonstracao.ClubeLago,null);
        Adicionar("Socio","Socio","Alex Exemplo",DadosDemonstracao.ClubeLago,DadosDemonstracao.Titular);
        Adicionar("Plataforma","SuperAdmin","Super administrador",null,null);
        Adicionar("Presidente","Presidente","Presidente foz 2",DadosDemonstracao.ClubeLago,null);
        Adicionar("Tesoureiro","Tesoureiro","Tesoureiro foz 2",DadosDemonstracao.ClubeLago,null);
        void Adicionar(string chave,string perfil,string nome,Guid? clube,Guid? associado) {
            var email=config[$"Acessos:{chave}:Email"]?.Trim();var hash=config[$"Acessos:{chave}:SenhaHash"];
            if(string.IsNullOrEmpty(email)||string.IsNullOrEmpty(hash))return;
            if(_contas.ContainsKey(email))throw new InvalidOperationException("E-mails locais duplicados.");
            _contas.Add(email,new("bootstrap:"+chave,perfil,nome,email,hash,clube,associado));
        }
    }
    public ContaLocal? Validar(string? email,string? senha) {
        lock(_gate) {
            var conta=_contas.GetValueOrDefault(email?.Trim()??"");
            if(string.IsNullOrEmpty(senha)||senha.Length>256)return null;
            try{return _hasher.VerifyHashedPassword(_usuario,conta?.SenhaHash??_hashFalso,senha)!=PasswordVerificationResult.Failed&&Elegivel(conta)?conta:null;}
            catch(FormatException){return null;}
        }
    }
    private bool Elegivel(ContaLocal? c)=>c is not null&&(c.Perfil!="Socio"||_dados.Ler(s=>s.Associados.Any(a=>a.Id==c.AssociadoId&&a.ClubeId==c.ClubeId&&a.Ativo)));
    public ContaLocal? Atual(ClaimsPrincipal user){lock(_gate){var c=_contas.Values.FirstOrDefault(c=>c.Id==user.FindFirstValue(ClaimTypes.NameIdentifier));return Elegivel(c)?c:null;}}
    public object[] Listar(Guid clube){lock(_gate)return _contas.Values.Where(c=>c.ClubeId==clube).Select(c=>(object)new{c.Id,c.Perfil,c.Nome,c.Email,c.AssociadoId}).ToArray();}
    public object Criar(Guid clube,NovoAcessoRequest r,ContaLocal autor) {
        lock(_gate) {
            var permitidos=autor.Perfil=="SuperAdmin"?new[]{"Presidente","Administrador","Tesoureiro","Socio"}:
                autor.Perfil=="Presidente"?new[]{"Administrador","Tesoureiro","Socio"}:new[]{"Socio"};
            if(!permitidos.Contains(r.Perfil))throw new UnauthorizedAccessException();
            if(string.IsNullOrWhiteSpace(r.Nome)||!System.Net.Mail.MailAddress.TryCreate(r.Email,out _)||r.Senha is null||r.Senha.Length<4||r.Senha.Length>256)
                throw new ArgumentException("Informe nome, e-mail e senha de 4 a 256 caracteres.");
            if(_contas.ContainsKey(r.Email.Trim()))throw new ArgumentException("E-mail já cadastrado. Múltiplos vínculos na mesma conta serão tratados em etapa própria.");
            if(r.Perfil=="Socio"&&r.AssociadoId is null)throw new ArgumentException("Vincule a conta ao associado ou dependente.");
            var c=new ContaLocal(Guid.NewGuid().ToString(),r.Perfil,r.Nome.Trim(),r.Email.Trim(),_hasher.HashPassword(_usuario,r.Senha),clube,r.AssociadoId);
            var novas=_criadas.Append(c).ToList();var tmp=_arquivo+".tmp";
            File.WriteAllText(tmp,JsonSerializer.Serialize(novas));File.Move(tmp,_arquivo,true);
            _criadas.Add(c);_contas.Add(c.Email,c);
            return new{c.Id,c.Perfil,c.Nome,c.Email,c.ClubeId,c.AssociadoId};
        }
    }
}
