# Estado da implementação — 0.1.0

Atualizado em 27/10/2026.

## Entregue

Estrutura C#/.NET 10, API de consulta demonstrativa, painel Blazor, aplicativo Android inicial e regras de domínio para mensalidades e extrato familiar. IDE definida: Visual Studio Code (azul).

- Familiar tem mensalidade adicional zero.
- Despesas preservam consumidor e são atribuídas ao titular.
- Fundador paga a tabela normal, salvo isenção.
- Atleta pode ser fundador ou mensalista e usa tabela diferenciada.
- Fundador isento e atleta simultaneamente não tem cálculo automático: a prioridade continua pendente.
- Registros de clubes diferentes são rejeitados ao compor o extrato.

## Validação realizada

- Solução principal compilada com SDK 10.0.100: zero erros e zero avisos.
- 13 verificações executáveis de regras aprovadas.
- API iniciada e consultada por HTTP; extrato demonstrativo totalizou R$ 255,00.
- Consulta do titular usando outro clube retornou 404.
- HTML inicial do painel renderizado com os dados de exemplo.
- Interação no navegador não verificada: o ambiente não disponibilizou um Chromium executável e o download do navegador falhou. Validar os botões e a troca de clube no ambiente local.
- Android: estrutura baseada no template oficial; sem build ou execução no dispositivo nesta entrega.

## Limitações e próximo incremento

Sem autenticação, permissões reais, cadastro/edição persistente, PostgreSQL, geração de mensalidades por competência, cobranças bancárias ou notificações. O extrato atual é apenas uma prévia dos lançamentos de exemplo, sem calendário de faturamento.

O painel e app consultam um serviço demonstrativo em memória. A futura operação usará API autenticada; não interpretar seleção de clube como autorização. API só publica consultas fictícias em Development; o painel bloqueia inicialização fora desse ambiente.

Não há automatização da idade dos filhos enquanto o marco etário não for definido. Ainda não há operação de eventos, estaleiro, oficina ou cantina além dos lançamentos fictícios.

Próxima entrega recomendada: autenticação, autorização por clube e persistência, seguida do cadastro de clubes, titulares e familiares. Confirmar regras pendentes antes de automatizar cobranças reais.
