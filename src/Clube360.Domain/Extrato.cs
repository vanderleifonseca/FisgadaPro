namespace Clube360.Domain;

public enum OrigemDespesa { Cantina, Evento, Servico }
public sealed record Despesa(Guid Id, Guid ClubeId, Guid ConsumidorId,
    DateOnly Data, OrigemDespesa Origem, string Descricao, decimal Valor);
public sealed record ItemExtrato(Guid ConsumidorId, string Consumidor, DateOnly Data,
    string Origem, string Descricao, decimal Valor);
public sealed record Extrato(Guid ClubeId, Guid TitularId, string Titular,
    decimal Mensalidade, IReadOnlyList<ItemExtrato> Despesas)
{
    public decimal TotalConsumos => Despesas.Sum(x => x.Valor);
    public decimal Total => Mensalidade + TotalConsumos;
}

public static class ComposicaoExtrato
{
    // Calcula uma prévia sem emitir cobrança, alterar saldo ou confirmar pagamento.
    public static Extrato Calcular(Guid clubeId, Guid titularId,
        IReadOnlyCollection<Associado> associados, IReadOnlyCollection<Despesa> despesas,
        TabelaMensalidade tabela)
    {
        if (associados.Any(a => a.ClubeId != clubeId) || despesas.Any(d => d.ClubeId != clubeId))
            throw new ArgumentException("Não é permitido misturar registros de clubes diferentes.");
        if (associados.Select(a => a.Id).Distinct().Count() != associados.Count ||
            despesas.Select(d => d.Id).Distinct().Count() != despesas.Count)
            throw new ArgumentException("Há identificadores duplicados.");
        foreach (var a in associados) a.Validar();
        var pessoas = associados.ToDictionary(a => a.Id);
        var titular = pessoas.GetValueOrDefault(titularId)
            ?? throw new ArgumentException("Titular não pertence ao clube.");
        if (titular.Categoria == Categoria.Dependente)
            throw new ArgumentException("Selecione um titular.");
        foreach (var familiar in associados.Where(a => a.Categoria == Categoria.Dependente))
        {
            if (!pessoas.TryGetValue(familiar.TitularId!.Value, out var responsavel) ||
                responsavel.Categoria == Categoria.Dependente)
                throw new ArgumentException("Familiar precisa de titular do mesmo clube.");
        }
        var itens = new List<ItemExtrato>();
        foreach (var d in despesas)
        {
            if (d.Id == Guid.Empty || d.Valor <= 0 || string.IsNullOrWhiteSpace(d.Descricao) ||
                !Enum.IsDefined(d.Origem) || !pessoas.TryGetValue(d.ConsumidorId, out var consumidor))
                throw new ArgumentException("Despesa inválida ou consumidor fora do clube.");
            var devedorId = consumidor.TitularId ?? consumidor.Id;
            if (devedorId == titularId)
                itens.Add(new(consumidor.Id, consumidor.Nome, d.Data, d.Origem.ToString(), d.Descricao, d.Valor));
        }
        return new(clubeId, titularId, titular.Nome, tabela.Calcular(titular),
            itens.OrderBy(x => x.Data).ThenBy(x => x.Consumidor).ToArray());
    }
}
