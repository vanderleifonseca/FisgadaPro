using FisgadaPro.Application;
using FisgadaPro.Contracts;
using FisgadaPro.Infrastructure;
using System.Globalization;
namespace FisgadaPro.App;
public partial class MainPage : ContentPage
{
    private readonly ConsultaDemonstracao _consulta = new(new DadosDemonstracao());
    private readonly CultureInfo _cultura = CultureInfo.GetCultureInfo("pt-BR");
    public MainPage()
    {
        InitializeComponent();
        ClubesPicker.ItemsSource = _consulta.Clubes().ToList();
        ClubesPicker.SelectedIndex = 0;
    }
    private void AoSelecionarClube(object? sender, EventArgs e)
    {
        ListaAssociados.Children.Clear();
        if (ClubesPicker.SelectedItem is not ClubeDto clube) return;
        foreach (var pessoa in _consulta.Associados(clube.Id) ?? [])
        {
            ListaAssociados.Children.Add(new Label { Text = $"{pessoa.Nome} · {pessoa.Categoria}" });
            if (pessoa.TitularId is not null) continue;
            var botao = new Button { Text = $"Consultar prévia de {pessoa.Nome}" };
            botao.Clicked += async (_, _) => {
                var extrato = _consulta.Extrato(clube.Id, pessoa.Id);
                if (extrato is null) return;
                var linhas = extrato.Despesas.Select(d =>
                    $"{d.Consumidor}: {d.Descricao} — {d.Valor.ToString("C", _cultura)}");
                await DisplayAlertAsync("Prévia do titular",
                    $"Mensalidade: {extrato.Mensalidade.ToString("C", _cultura)}\n" +
                    string.Join("\n", linhas) +
                    $"\nTotal: {extrato.Total.ToString("C", _cultura)}\nNão gera cobrança.", "Fechar");
            };
            ListaAssociados.Children.Add(botao);
        }
    }
}
