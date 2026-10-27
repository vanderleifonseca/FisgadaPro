# Histórico de alterações

## 2026-10-27 — Senhas curtas para contas locais de teste

- Reduzido o mínimo de senha no configurador de 12 para 4 caracteres, a pedido do responsável, temporariamente para testes locais.
- Mantidos hash de senha, confirmação e limite de tentativas de login.


## 0.2.0 — 2026-10-27 — Login e separação dos acessos

- Contas locais com hash de senha e configurador sem senhas padrão.
- API protegida por token, autorização por perfil e clube e limite de tentativas.
- Painel conectado à API e sessão web com cookie e antiforgery.
- App consulta autenticada do próprio extrato, sem lista geral de associados.
- Novos testes de acesso, separação entre clubes e fluxo de formulário.
- Dados financeiros e vínculos continuam fictícios; persistência real fica para a próxima etapa.
- Build e execução Android da versão anterior confirmados pelo usuário, inclusive por Wi-Fi.


## 2026-10-27 — Contorno para erro de processamento das imagens Android

- Substituídas letras FP em elementos SVG de texto por contornos vetoriais no ícone e na tela de abertura.
- Evitado o processamento de fontes que disparou MissingMethodException em HarfBuzzSharp.Font.SetVariations no ambiente Android.
- Recursos continuam com dimensões explícitas, sem dependência de fontes.
- A recompilação Android deve ser validada no ambiente do usuário.


## 0.1.0 — 2026-10-27 — Fundação e demonstração local

- Início do código autorizado; Visual Studio Code (azul) definido.
- Solução .NET 10 com Domain, Contracts, Application, Infrastructure, API e painel Blazor.
- Projeto MAUI Android inicial separado da solução principal para não exigir workloads móveis ao compilar o servidor.
- Cálculo de mensalidade, familiar sem taxa fixa, extrato do titular com consumidor identificado e rejeição de mistura de clubes.
- Dados fictícios, consultas somente de leitura e demonstração disponível apenas em Development.
- Tarefas do VS Code, instruções e verificações executáveis de regras.
- Banco, login, cadastros persistentes, cobrança bancária e permissões reais ainda não implementados.


## 2026-10-27 — Familiares sem taxa fixa adicional

- Confirmada a ausência de taxa fixa adicional por familiar.
- Mantida a cobrança dos gastos ao titular, com identificação de quem fez cada despesa.
- Resolvida a pendência DP-03.

## 2026-10-27 — Regras de associados e débitos familiares

- Confirmada mensalidade normal dos fundadores, com opção de isenção.
- Confirmada mensalidade diferenciada de atleta definida pela diretoria e sua coexistência com fundador ou mensalista.
- Confirmada inclusão dos débitos familiares na cobrança mensal do titular, incluindo cantina e eventos, com identificação do consumidor.
- Mantidas abertas a eventual taxa fixa por familiar, a prioridade de isenções e as regras de faturamento entre clubes.

## 2026-10-27 — Documentação inicial

- Definido o nome Fisgada Pro.
- Consolidado o planejamento multiclube e os requisitos confirmados.
- Registradas categorias e regras de familiares.
- Separadas propostas técnicas e decisões pendentes.
- Adicionado roteiro de desenvolvimento.
- Preparadas configurações básicas de edição e exclusão de arquivos locais.
- Nenhum código de aplicação iniciado.
