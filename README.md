# Fisgada Pro

Plataforma para clubes de pesca em C#, API ASP.NET Core, Blazor e .NET MAUI Android. IDE: Visual Studio Code (azul).

**Versão 0.2.0 — login e permissões sobre dados fictícios.** Os dados financeiros continuam demonstrativos. Não há banco de dados, cadastro de usuários reais ou cobrança bancária.

## Começar

Siga [LOGIN_E_ACESSOS.md](docs/LOGIN_E_ACESSOS.md). O login agora é obrigatório; a API deve estar funcionando para o painel e para o celular.

~~~powershell
dotnet build FisgadaPro.slnx
dotnet run --project tools/FisgadaPro.Setup
~~~

O configurador solicita os e-mails e senhas locais de três perfis. Não há senha padrão. Somente hashes são armazenados no user-secrets da API, fora do repositório.

Após configurar, abra a API e o painel em terminais separados:

~~~powershell
dotnet run --project src/FisgadaPro.Api --launch-profile http
~~~

~~~powershell
dotnet run --project src/FisgadaPro.Web --launch-profile http
~~~

Painel: http://localhost:5216/entrar. API: http://localhost:5187/api/v1/status.

## Acessos desta etapa

| Perfil | Acesso na demonstração |
| --- | --- |
| Associado | Alex Exemplo: Clube Lago, próprio extrato e gastos de Bia identificados |
| Administrador do clube | Associados e extratos do Clube Lago; sem acesso ao Clube Rio |
| Plataforma | Nomes dos clubes; sem acesso às listas de sócios ou extratos |

Os vínculos são fixos apenas nesta demonstração. A gestão de cadastros, múltiplos vínculos reais, operadores, diretoria, financeiro e manutenção serão implementados com persistência e permissões específicas.

No app, uma conta administrativa é orientada a usar o painel web. A API também verifica permissões em cada consulta; ocultar elementos na tela não é a única proteção.

## Verificações

~~~powershell
dotnet run --project tests/FisgadaPro.Domain.Checks
dotnet run --project tests/FisgadaPro.Access.Checks
~~~

Resultados esperados: 13 verificações financeiras e 22 verificações de acesso/fluxo HTTP. Os testes de acesso iniciam sua própria API e seu próprio painel em portas temporárias, com credenciais aleatórias, sem alterar suas contas locais.

## Regras mantidas

Fundador paga normalmente, com opção de isenção; atleta pode ser fundador ou mensalista e tem valor definido pela direção. Familiar não possui taxa fixa adicional e seus gastos são atribuídos ao titular com identificação do consumidor. Valores de exemplo: mensalidade de Alex R$ 150,00 e consumos de Bia R$ 105,00; total R$ 255,00.

## Documentação

- [Login, contas locais e conexão do celular](docs/LOGIN_E_ACESSOS.md)
- [Execução no VS Code](docs/EXECUCAO_VSCODE.md)
- [Estado e limitações](docs/STATUS_IMPLEMENTACAO.md)
- [Planejamento](docs/PLANEJAMENTO.md)
- [Decisões pendentes](docs/DECISOES_PENDENTES.md)
- [Histórico](CHANGELOG.md)

SDK .NET 10; global.json aceita bandas posteriores, incluindo 10.0.401. O App compila separadamente da solução principal e precisa do workload Android/MAUI.

Responsável: Vanderlei Fonseca.
