using FisgadaPro.Domain;
using FisgadaPro.Application;
namespace FisgadaPro.Infrastructure;

public sealed class DadosDemonstracao : IDadosDemonstracao
{
    public static readonly Guid ClubeLago = Guid.Parse("10000000-0000-0000-0000-000000000001");
    public static readonly Guid ClubeRio = Guid.Parse("10000000-0000-0000-0000-000000000002");
    private static readonly Guid Titular = Guid.Parse("20000000-0000-0000-0000-000000000001");
    private static readonly Guid Familiar = Guid.Parse("20000000-0000-0000-0000-000000000002");
    public IReadOnlyList<Clube> Clubes { get; } = [
        new(ClubeLago, "Clube Lago — demonstração"), new(ClubeRio, "Clube Rio — demonstração")];
    public IReadOnlyList<Associado> Associados { get; } = [
        new(Titular, ClubeLago, "Alex Exemplo", Categoria.Fundador),
        new(Familiar, ClubeLago, "Bia Exemplo", Categoria.Familiar, TitularId: Titular, Parentesco: Parentesco.Conjuge),
        new(Guid.Parse("20000000-0000-0000-0000-000000000003"), ClubeLago, "Caio Exemplo", Categoria.Mensalista, Atleta: true),
        new(Guid.Parse("20000000-0000-0000-0000-000000000004"), ClubeRio, "Dani Exemplo", Categoria.Fundador, Isento: true)];
    public IReadOnlyList<Despesa> Despesas { get; } = [
        new(Guid.Parse("30000000-0000-0000-0000-000000000001"), ClubeLago, Familiar,
            new(2026,10,27), OrigemDespesa.Cantina, "Lanche — exemplo", 25m),
        new(Guid.Parse("30000000-0000-0000-0000-000000000002"), ClubeLago, Familiar,
            new(2026,10,27), OrigemDespesa.Evento, "Inscrição — exemplo", 80m)];
    public TabelaMensalidade Tabela(Guid clubeId)
    {
        if (Clubes.All(c => c.Id != clubeId)) throw new ArgumentException("Clube inexistente.");
        return clubeId == ClubeLago ? new(150m, 90m) : new(180m, 100m);
    }
}
