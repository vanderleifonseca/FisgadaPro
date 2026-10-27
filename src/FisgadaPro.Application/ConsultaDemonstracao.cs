using FisgadaPro.Domain;
using FisgadaPro.Contracts;
namespace FisgadaPro.Application;

// Interface temporária para dados fictícios; não representa autorização multiclube.
public interface IDadosDemonstracao
{
    IReadOnlyList<Clube> Clubes { get; }
    IReadOnlyList<Associado> Associados { get; }
    IReadOnlyList<Despesa> Despesas { get; }
    TabelaMensalidade Tabela(Guid clubeId);
}
public sealed class ConsultaDemonstracao(IDadosDemonstracao dados)
{
    public IReadOnlyList<ClubeDto> Clubes() =>
        dados.Clubes.Select(c => new ClubeDto(c.Id, c.Nome)).ToArray();

    public IReadOnlyList<AssociadoDto>? Associados(Guid clubeId) =>
        dados.Clubes.All(c => c.Id != clubeId) ? null :
        dados.Associados.Where(a => a.ClubeId == clubeId)
            .Select(a => new AssociadoDto(a.Id, a.Nome, a.Categoria.ToString(), a.Atleta, a.Isento, a.TitularId)).ToArray();

    public ExtratoDto? Extrato(Guid clubeId, Guid titularId)
    {
        var pessoas = dados.Associados.Where(a => a.ClubeId == clubeId).ToArray();
        if (pessoas.All(a => a.Id != titularId || a.Categoria == Categoria.Familiar)) return null;
        var extrato = ComposicaoExtrato.Calcular(clubeId, titularId, pessoas,
            dados.Despesas.Where(d => d.ClubeId == clubeId).ToArray(), dados.Tabela(clubeId));
        return new(extrato.Titular, extrato.Mensalidade, extrato.TotalConsumos, extrato.Total,
            extrato.Despesas.Select(x => new ItemExtratoDto(x.Consumidor, x.Data, x.Origem, x.Descricao, x.Valor)).ToArray());
    }
}
