namespace FisgadaPro.Domain;

public enum Categoria { Mensalista, Fundador, Familiar }
public enum Parentesco { Pai, Mae, Filho, Filha, Avo, Avoa, Conjuge }

public sealed record Clube(Guid Id, string Nome);

public sealed record Associado(
    Guid Id, Guid ClubeId, string Nome, Categoria Categoria,
    bool Atleta = false, bool Isento = false,
    Guid? TitularId = null, Parentesco? Parentesco = null)
{
    public void Validar()
    {
        if (Id == Guid.Empty || ClubeId == Guid.Empty || string.IsNullOrWhiteSpace(Nome))
            throw new ArgumentException("Informe identificação, clube e nome.");
        if (!Enum.IsDefined(Categoria))
            throw new ArgumentException("Categoria inválida.");
        if (Categoria == Categoria.Familiar)
        {
            if (TitularId is null || TitularId == Guid.Empty || TitularId == Id ||
                Parentesco is null || !Enum.IsDefined(Parentesco.Value) || Atleta || Isento)
                throw new ArgumentException("Familiar exige titular e parentesco; não possui mensalidade própria.");
        }
        else if (TitularId is not null || Parentesco is not null)
            throw new ArgumentException("Somente familiar possui vínculo com titular.");
        if (Isento && Categoria != Categoria.Fundador)
            throw new ArgumentException("Nesta etapa, a opção de isenção pertence ao fundador.");
    }
}

public sealed record TabelaMensalidade(decimal Normal, decimal Atleta)
{
    public decimal Calcular(Associado associado)
    {
        associado.Validar();
        if (Normal < 0 || Atleta < 0)
            throw new ArgumentException("Mensalidades não podem ser negativas.");
        if (associado.Categoria == Categoria.Familiar) return 0m;
        if (associado.Isento && associado.Atleta)
            throw new InvalidOperationException("Defina com a diretoria a prioridade entre isenção e condição de atleta.");
        if (associado.Isento) return 0m;
        return associado.Atleta ? Atleta : Normal;
    }
}
