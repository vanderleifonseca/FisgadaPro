using FisgadaPro.Domain;
using FisgadaPro.Infrastructure;
using FisgadaPro.Application;
var clube = Guid.NewGuid();
var titular = new Associado(Guid.NewGuid(), clube, "Titular", Categoria.Fundador);
var familiar = new Associado(Guid.NewGuid(), clube, "Familiar", Categoria.Familiar,
    TitularId: titular.Id, Parentesco: Parentesco.Conjuge);
var tabela = new TabelaMensalidade(150, 90);
var checks = 0;
void Igual(decimal atual, decimal esperado) {
    if (atual != esperado) throw new Exception($"Esperado {esperado}; obtido {atual}.");
    checks++;
}
void Falha(Action acao) {
    try { acao(); } catch (ArgumentException) { checks++; return; }
    catch (InvalidOperationException) { checks++; return; }
    throw new Exception("A operação deveria ter sido rejeitada.");
}
Igual(tabela.Calcular(titular), 150);
Igual(tabela.Calcular(titular with { Isento = true }), 0);
Igual(tabela.Calcular(titular with { Atleta = true }), 90);
Igual(tabela.Calcular(titular with { Categoria = Categoria.Mensalista, Atleta = true }), 90);
Igual(tabela.Calcular(familiar), 0);
Falha(() => tabela.Calcular(titular with { Atleta = true, Isento = true }));
var gasto = new Despesa(Guid.NewGuid(), clube, familiar.Id, new(2026,10,27),
    OrigemDespesa.Cantina, "Lanche", 25);
var extrato = ComposicaoExtrato.Calcular(clube, titular.Id, [titular, familiar], [gasto], tabela);
Igual(extrato.Total, 175);
if (extrato.Despesas.Single().ConsumidorId != familiar.Id) throw new Exception("Identificação perdida.");
checks++;
Falha(() => ComposicaoExtrato.Calcular(clube, titular.Id, [titular, familiar],
    [gasto with { ClubeId = Guid.NewGuid() }], tabela));
Falha(() => ComposicaoExtrato.Calcular(clube, titular.Id,
    [titular, familiar with { ClubeId = Guid.NewGuid() }], [], tabela));
Falha(() => ComposicaoExtrato.Calcular(clube, titular.Id, [titular, familiar], [gasto, gasto], tabela));
Falha(() => ComposicaoExtrato.Calcular(clube, titular.Id, [titular, familiar],
    [gasto with { Valor = -10 }], tabela));
var consulta = new ConsultaDemonstracao(new DadosDemonstracao());
var titularLago = consulta.Associados(DadosDemonstracao.ClubeLago)!.First().Id;
if (consulta.Extrato(DadosDemonstracao.ClubeLago, titularLago) is null ||
    consulta.Extrato(DadosDemonstracao.ClubeRio, titularLago) is not null) throw new Exception("Vazamento.");
checks++;
Console.WriteLine($"{checks} verificações de regras concluídas com sucesso.");
