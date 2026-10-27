# Estado da implementação — 0.2.0

Atualização: 27/10/2026.

## Entregue

- Login com contas de teste configuradas localmente, sem senha padrão no código.
- Hash de senha do ASP.NET Core Identity; segredos locais armazenam somente o hash.
- Tokens opacos do ASP.NET Core na API, com validade de 20 minutos.
- Sessão web por cookie HttpOnly, com proteção antiforgery nos formulários de entrada e saída.
- Limite de cinco tentativas de login por minuto por IP.
- Verificação de perfil e clube em cada endpoint de dados.
- Sócio consulta o próprio extrato; administrador consulta seu clube; plataforma lista clubes sem dados financeiros.
- Painel conectado à API. Aplicativo alterado para login de associado e consulta autenticada do próprio extrato.
- Consumidor identificado e responsabilidade do titular preservados; familiar sem taxa fixa adicional.

## Verificação

- Solução principal e configurador compilados com .NET 10.
- 13 verificações de regras financeiras aprovadas.
- 22 verificações de acesso e fluxo HTTP aprovadas, incluindo cookie web, CSRF, login incorreto, limites e tentativas entre perfis/clubes.
- Fluxo web exercitado por HTTP e conteúdo renderizado. Interação visual da versão 0.2.0 ainda deve ser testada no navegador pelo usuário.
- Versão Android 0.1.0 e correção de SVGs confirmadas pelo usuário em aparelho, via cabo e Wi-Fi.
- Android 0.2.0: fontes alteradas; sem SDK Android no ambiente do agente para compilação. Validar no ambiente local do usuário.

## Limites explícitos

Os três perfis estão vinculados a dados fictícios fixos, e não há banco de usuários. As contas são configuradas por ferramenta local; não há cadastro público, recuperação de senha, MFA ou revogação imediata de tokens. Sair descarta a sessão do cliente; tokens emitidos vencem em até 20 minutos.

API e painel continuam somente para Development. O app Debug permite HTTP para rede local; Release requer HTTPS. Ainda não existe configuração de produção.

Não há persistência dos associados/lançamentos, cadastros reais, gestão de novos clubes, múltiplos vínculos configuráveis, permissões de operador/financeiro/manutenção/diretoria, cobrança bancária, estaleiro, eventos operacionais ou oficina. O limite etário dos filhos permanece pendente.

Próximo incremento: persistência e cadastros reais, preservando autenticação e autorização no servidor.
