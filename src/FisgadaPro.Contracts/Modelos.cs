namespace FisgadaPro.Contracts;
public sealed record ClubeDto(Guid Id, string Nome);
public sealed record AssociadoDto(Guid Id, string Nome, string Categoria, bool Atleta, bool Isento, Guid? TitularId);
public sealed record ItemExtratoDto(string Consumidor, DateOnly Data, string Origem, string Descricao, decimal Valor);
public sealed record ExtratoDto(string Titular, decimal Mensalidade, decimal TotalConsumos,
    decimal Total, IReadOnlyList<ItemExtratoDto> Despesas);
