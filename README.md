# Fisgada Pro

Plataforma para clubes de pesca: associados, financeiro, embarcações, eventos e serviços.

**Versão 0.1.0 — demonstração local em desenvolvimento.** Não utilizar com dados reais. Esta entrega não possui login, banco de dados ou emissão de cobranças.

## Começar pelo Visual Studio Code (azul)

Consulte o [passo a passo com resultados esperados](docs/EXECUCAO_VSCODE.md).

Pré-requisito do servidor: SDK .NET 10. O arquivo global.json aceita SDKs 10.0 de bandas posteriores, incluindo 10.0.401.

~~~powershell
dotnet build FisgadaPro.slnx
dotnet run --project tests/FisgadaPro.Domain.Checks
dotnet run --project src/FisgadaPro.Web --launch-profile http
~~~

Painel: http://localhost:5216. Em outro terminal, execute a API:

~~~powershell
dotnet run --project src/FisgadaPro.Api --launch-profile http
~~~

Status: http://localhost:5187/api/v1/status.

## O que funciona nesta versão

- Seleção entre dois clubes fictícios e consulta dos respectivos associados.
- Prévia de extrato do titular, com gastos de familiar identificados.
- Fundador paga normalmente; opção de isenção.
- Atleta pode também ser fundador ou mensalista, com valor diferenciado.
- Familiar sem taxa fixa adicional; seus gastos são atribuídos ao titular.
- Verificações automatizadas de cálculo e separação dos clubes.
- Aplicativo MAUI Android inicial com a mesma demonstração local; compilação e execução Android ainda precisam ser verificadas no ambiente móvel.

Os valores exibidos são exemplos, não tabelas aprovadas pela diretoria. A combinação fundador isento + atleta é rejeitada até a definição da prioridade. O limite etário dos filhos ainda não é automatizado.

## Estrutura

| Projeto | Responsabilidade |
| --- | --- |
| Domain | Regras de associação, mensalidade e composição do extrato |
| Contracts | Modelos de resposta |
| Application | Consultas da demonstração |
| Infrastructure | Dados fictícios em memória |
| Api | Status e consultas HTTP locais |
| Web | Painel Blazor Interactive Server |
| App | MAUI Android; projeto compilado separadamente |
| Domain.Checks | Verificações executáveis das regras |

O painel e o app usam os dados demonstrativos localmente. A conexão dos clientes à API será implementada junto da autenticação. Selecionar clube na demonstração não representa autorização de acesso.

## Documentação

- [Planejamento e regras](docs/PLANEJAMENTO.md)
- [Decisões pendentes](docs/DECISOES_PENDENTES.md)
- [Execução no VS Code](docs/EXECUCAO_VSCODE.md)
- [Estado da implementação e limitações](docs/STATUS_IMPLEMENTACAO.md)
- [Roteiro](docs/ROADMAP.md)
- [Histórico](CHANGELOG.md)

Responsável: Vanderlei Fonseca.
