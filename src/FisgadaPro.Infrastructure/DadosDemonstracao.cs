using FisgadaPro.Domain;
using FisgadaPro.Application;
namespace FisgadaPro.Infrastructure;
public sealed class DadosDemonstracao : IDadosDemonstracao
{
    // IDs preservados para manter as contas locais existentes.
    public static readonly Guid ClubeLago=Guid.Parse("10000000-0000-0000-0000-000000000001");
    public static readonly Guid ClubeRio=Guid.Parse("10000000-0000-0000-0000-000000000002");
    public static readonly Guid Titular=Guid.Parse("20000000-0000-0000-0000-000000000001");
    public static readonly Guid Familiar=Guid.Parse("20000000-0000-0000-0000-000000000002");
    public static readonly Guid SalaoFoz=Guid.Parse("40000000-0000-0000-0000-000000000001");
    private readonly EstadoClubes _estado=CriarEstado();
    public IReadOnlyList<Clube> Clubes=>_estado.Clubes.AsReadOnly();
    public IReadOnlyList<Associado> Associados=>_estado.Associados.AsReadOnly();
    public IReadOnlyList<Despesa> Despesas=>_estado.Despesas.AsReadOnly();
    public TabelaMensalidade Tabela(Guid clubeId)=>new(_estado.Clubes.Single(c=>c.Id==clubeId).ValorMensalPadrao);
    public static EstadoClubes CriarEstado() {
        var s=new EstadoClubes {
            Clubes=[new(ClubeLago,"foz 2","FOZ2",Cidade:"Foz do Iguaçu",Uf:"PR",ValorMensalPadrao:150),
                    new(ClubeRio,"guaira2","GUA2",Cidade:"Guaíra",Uf:"PR",ValorMensalPadrao:180)],
            Associados=[
                new(Titular,ClubeLago,"Alex Exemplo",Categoria.Fundador),
                new(Familiar,ClubeLago,"Bia Exemplo",Categoria.Dependente,TitularId:Titular,Parentesco:Parentesco.Conjuge),
                new(Guid.Parse("20000000-0000-0000-0000-000000000003"),ClubeLago,"Caio Exemplo",Categoria.Associado),
                new(Guid.Parse("20000000-0000-0000-0000-000000000004"),ClubeRio,"Dani Exemplo",Categoria.Fundador,Isento:true)],
            Espacos=[new(SalaoFoz,ClubeLago,"Salão social",250),new(Guid.Parse("40000000-0000-0000-0000-000000000002"),ClubeRio,"Salão social",300)],
            Despesas=[
                new(Guid.Parse("30000000-0000-0000-0000-000000000001"),ClubeLago,Familiar,new(2026,10,27),OrigemDespesa.Cantina,"Lanche - exemplo",25),
                new(Guid.Parse("30000000-0000-0000-0000-000000000002"),ClubeLago,Familiar,new(2026,10,27),OrigemDespesa.Evento,"Inscrição - exemplo",80)]
        };
        s.Cobrancas.Add(new(Guid.NewGuid(),ClubeLago,Titular,Titular,"Mensalidade",Titular,"Mensalidade - exemplo",150,new(2026,11,10),Competencia:"2026-10"));
        foreach(var d in s.Despesas)s.Cobrancas.Add(new(Guid.NewGuid(),d.ClubeId,Titular,d.ConsumidorId,d.Origem.ToString(),d.Id,d.Descricao,d.Valor,new(2026,11,10)));
        return s;
    }
}
