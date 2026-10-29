namespace Clube360.Contracts;
public sealed record ClubeDto(Guid Id,string Nome,string Sigla="",string Endereco="",string Bairro="",string Cidade="",string Uf="",string Fone="",string Email="",decimal ValorMensalPadrao=0);
public sealed record AssociadoDto(Guid Id, string Nome, string Categoria, bool DireitoVoto, bool Isento, Guid? TitularId);
public sealed record ItemExtratoDto(string Consumidor, DateOnly Data, string Origem, string Descricao, decimal Valor);
public sealed record ExtratoDto(string Titular, decimal Mensalidade, decimal TotalConsumos,
    decimal Total, IReadOnlyList<ItemExtratoDto> Despesas);

public sealed record MeuCadastroDto(PessoaDto Associado,IReadOnlyList<PessoaDto> Dependentes);
