using FisgadaPro.Contracts;
using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
namespace FisgadaPro.App;
public partial class MainPage : ContentPage
{
    private HttpClient? _client;
    private bool _ocupado;
    private readonly CultureInfo _cultura = CultureInfo.GetCultureInfo("pt-BR");
    public MainPage()
    {
        InitializeComponent();
        ApiUrl.Text = Preferences.Default.Get("api_url", "");
    }
    private async void AoEntrar(object? sender, EventArgs e)
    {
        if (_ocupado) return;
        if (!Uri.TryCreate(ApiUrl.Text?.Trim(), UriKind.Absolute, out var uri) ||
            (uri.Scheme != "http" && uri.Scheme != "https") || !string.IsNullOrEmpty(uri.UserInfo))
        { Mensagem.Text = "Informe o endereço da API do computador."; return; }
        Ocupado(true); Mensagem.Text = "";
        try {
            _client?.Dispose();
            _client = new HttpClient { BaseAddress = new Uri(uri.GetLeftPart(UriPartial.Authority)), Timeout = TimeSpan.FromSeconds(15) };
            using var resposta = await _client.PostAsJsonAsync("/api/v1/login", new LoginRequest(Email.Text ?? "", Senha.Text ?? ""));
            Senha.Text = "";
            if (!resposta.IsSuccessStatusCode) { Encerrar(); Mensagem.Text = "Não foi possível entrar. Confira as credenciais ou aguarde um minuto."; return; }
            var token = await resposta.Content.ReadFromJsonAsync<TokenResponse>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);
            var sessao = await _client.GetFromJsonAsync<SessaoDto>("/api/v1/sessao");
            if (sessao?.Perfil != "Socio") { Encerrar(); Mensagem.Text = "Esta área é dos associados. Use o painel web para administrar."; return; }
            Preferences.Default.Set("api_url", _client.BaseAddress!.ToString());
            Nome.Text = sessao.Nome;
            LoginPainel.IsVisible = false; ContaPainel.IsVisible = true;
            ClubesPicker.ItemsSource = sessao.Clubes.ToList();
            ClubesPicker.SelectedIndex = 0;
            await CarregarExtrato();
        } catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException) {
            Encerrar(); Mensagem.Text = "Não foi possível acessar a API. Confira a rede e o endereço do computador.";
        } finally { Ocupado(false); }
    }
    private async void AoSelecionarClube(object? sender, EventArgs e)
    {
        if (_ocupado || _client is null) return;
        Ocupado(true);
        try { await CarregarExtrato(); }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException) {
            Encerrar(); Mensagem.Text = "Sessão encerrada ou conexão indisponível. Entre novamente.";
        } finally { Ocupado(false); }
    }
    private async Task CarregarExtrato()
    {
        ExtratoPainel.Children.Clear();
        if (_client is null || ClubesPicker.SelectedItem is not ClubeDto clube) return;
        var extrato = await _client.GetFromJsonAsync<ExtratoDto>($"/api/v1/clubes/{clube.Id}/meu-extrato");
        if (extrato is null) throw new HttpRequestException();
        ExtratoPainel.Children.Add(new Label { Text = $"Titular: {extrato.Titular}" });
        ExtratoPainel.Children.Add(new Label { Text = $"Mensalidade: {extrato.Mensalidade.ToString("C", _cultura)}" });
        foreach (var d in extrato.Despesas)
            ExtratoPainel.Children.Add(new Label { Text = $"{d.Consumidor} · {d.Descricao}: {d.Valor.ToString("C", _cultura)}" });
        ExtratoPainel.Children.Add(new Label { Text = $"Total: {extrato.Total.ToString("C", _cultura)}", FontAttributes = FontAttributes.Bold });
    }
    private void AoSair(object? sender, EventArgs e) { Encerrar(); Mensagem.Text = ""; }
    private void Encerrar()
    {
        _client?.Dispose(); _client = null;
        ExtratoPainel.Children.Clear(); ClubesPicker.ItemsSource = null;
        Nome.Text = ""; Senha.Text = "";
        ContaPainel.IsVisible = false; LoginPainel.IsVisible = true;
    }
    private void Ocupado(bool valor) {
        _ocupado = valor; EntrarBotao.IsEnabled = !valor;
        ClubesPicker.IsEnabled = !valor; SairBotao.IsEnabled = !valor;
    }
}
