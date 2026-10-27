# Fisgada Pro

Sistema destinado a clubes de pesca, com gestão de associados, financeiro, eventos e serviços prestados pelo clube.

**Fase atual: planejamento. Não há aplicação executável nem código de negócio neste repositório.**

## Objetivo

Atender vários clubes na mesma plataforma, permitindo novas adesões. A plataforma cadastra os clubes; a administração de cada clube gerencia seus próprios sócios. O associado acessa o aplicativo e seleciona seu clube.

## Funcionalidades previstas

- Associados fundadores, mensalistas, atletas e familiares.
- Mensalidades, cobranças e relatório de inadimplência.
- Embarcações, guarda em estaleiro e colocação na água.
- Manutenção, reparos e oficina.
- Eventos com preços diferenciados para sócios do organizador, de outros clubes e não associados.
- Comunicação interna e mensagens em massa, com alcance sujeito às permissões.

Familiares admitidos: pais, avós e cônjuges sem limite de idade; filhos até 21 anos. O procedimento exato de encerramento da elegibilidade ainda será definido.

## Tecnologia

Definido: **C# e .NET MAUI**.

Propostas ainda sujeitas à validação: painel Blazor, API ASP.NET Core, PostgreSQL e Entity Framework Core, organizados como monólito modular. Versões, infraestrutura e integrações ainda não foram escolhidas.

## Documentação

- [Planejamento completo e regras confirmadas](docs/PLANEJAMENTO.md)
- [Decisões pendentes](docs/DECISOES_PENDENTES.md)
- [Roteiro de desenvolvimento](docs/ROADMAP.md)
- [Histórico de alterações](CHANGELOG.md)

## Continuidade

Antes de implementar, fechar as regras pendentes, os fluxos e o escopo da primeira versão. Preservar a distinção entre requisitos confirmados e sugestões. O início do código depende de nova autorização do responsável.

Não existem comandos de execução, compilação ou testes nesta fase.

Responsável: Vanderlei Fonseca.
