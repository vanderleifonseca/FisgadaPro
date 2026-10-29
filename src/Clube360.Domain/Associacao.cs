namespace Clube360.Domain;

public enum Categoria { Associado, Fundador, Convidado, Simpatizante, Dependente }
public enum Parentesco { Pai, Mae, Filho, Filha, Avo, Avoa, Conjuge }

public sealed record Clube(Guid Id, string Nome, string Sigla = "", string Endereco = "",
    string Bairro = "", string Cidade = "", string Uf = "", string Fone = "", string Email = "",
    decimal ValorMensalPadrao = 0m)
{
    public void Validar() {
        if (Id == Guid.Empty || string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Sigla))
            throw new ArgumentException("Informe nome e sigla do clube.");
        if (ValorMensalPadrao < 0 || (Uf.Length != 0 && Uf.Length != 2))
            throw new ArgumentException("Mensalidade ou UF inválida.");
    }
}
public sealed record Associado(Guid Id, Guid ClubeId, string Nome, Categoria Categoria,
    bool Isento = false, Guid? TitularId = null, Parentesco? Parentesco = null,
    string Cpf = "", string Endereco = "", string Bairro = "", string Sexo = "",
    string EstadoCivil = "", string Cidade = "", string Uf = "", string Fone = "",
    string Email = "", DateOnly? DataNascimento = null, string[]? Interesses = null,
    decimal? ValorMensal = null, bool Ativo = true)
{
    public bool DireitoVoto => Ativo && Categoria is Categoria.Associado or Categoria.Fundador;
    public void Validar() {
        if (Id == Guid.Empty || ClubeId == Guid.Empty || string.IsNullOrWhiteSpace(Nome) || !Enum.IsDefined(Categoria))
            throw new ArgumentException("Identificação, clube, nome ou categoria inválidos.");
        if (Categoria == Categoria.Dependente) {
            if (TitularId is null || TitularId == Guid.Empty || TitularId == Id ||
                Parentesco is null || !Enum.IsDefined(Parentesco.Value) || Isento || ValorMensal is > 0)
                throw new ArgumentException("Dependente exige titular e parentesco e não possui taxa fixa adicional.");
        } else if (TitularId is not null || Parentesco is not null)
            throw new ArgumentException("Somente dependente possui titular e parentesco.");
        if (Isento && Categoria != Categoria.Fundador)
            throw new ArgumentException("A opção de isenção pertence ao fundador.");
        if (ValorMensal is < 0) throw new ArgumentException("Mensalidade negativa.");
    }
}
public sealed record TabelaMensalidade(decimal Normal)
{
    public decimal Calcular(Associado associado) {
        associado.Validar();
        if (Normal < 0) throw new ArgumentException("Mensalidade negativa.");
        if (!associado.Ativo || associado.Categoria == Categoria.Dependente || associado.Isento) return 0;
        return associado.ValorMensal ?? (associado.Categoria is Categoria.Associado or Categoria.Fundador ? Normal : 0m);
    }
}
public static class DocumentoCpf
{
    public static string Normalizar(string? cpf) => new((cpf ?? "").Where(char.IsAsciiDigit).ToArray());
    public static bool Valido(string? cpf) {
        var s = Normalizar(cpf);
        if (s.Length != 11 || s.Distinct().Count() == 1) return false;
        for (var n = 9; n <= 10; n++) {
            var soma = Enumerable.Range(0, n).Sum(i => (s[i]-'0')*(n+1-i));
            var digito = soma*10%11; if (digito == 10) digito = 0;
            if (s[n]-'0' != digito) return false;
        }
        return true;
    }
}
