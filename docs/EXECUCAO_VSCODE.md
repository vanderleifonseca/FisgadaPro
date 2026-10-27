# Execução no Visual Studio Code (azul)

A versão 0.2.0 exige login e API para o painel e o app. Siga [Login e acessos](LOGIN_E_ACESSOS.md) para configurar contas e executar.

## Comandos na raiz do projeto

~~~powershell
dotnet build FisgadaPro.slnx
dotnet run --project tools/FisgadaPro.Setup
dotnet run --project tests/FisgadaPro.Domain.Checks
dotnet run --project tests/FisgadaPro.Access.Checks
~~~

API e painel devem ser executados em terminais separados:

~~~powershell
dotnet run --project src/FisgadaPro.Api --launch-profile http
~~~

~~~powershell
dotnet run --project src/FisgadaPro.Web --launch-profile http
~~~

Painel http://localhost:5216/entrar; status http://localhost:5187/api/v1/status.

A API precisa ser iniciada com --urls http://0.0.0.0:5187 para testes a partir do celular, conforme o guia de login.

## Android

SDK localizado no ambiente de Vanderlei em C:\Program Files (x86)\Android\android-sdk.

~~~powershell
& "C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe" devices
dotnet build src/FisgadaPro.App/FisgadaPro.App.csproj -t:Run -p:AndroidSdkDirectory="C:\Program Files (x86)\Android\android-sdk"
~~~

O estado do celular deve ser device. A conexão ADB pode ser USB ou Wi-Fi; IP e porta de depuração podem mudar. No app, informe o IP do computador e a porta da API (5187), sem confundir com ADB.

A versão 0.1.0 e a correção dos SVGs foram compiladas e executadas pelo usuário em dispositivo, inclusive por Wi-Fi. A atualização Android 0.2.0 ainda depende de compilação e validação local.

Extensões recomendadas: C#, C# Dev Kit e .NET MAUI. A solução principal não inclui o projeto MAUI, para permitir compilar o servidor sem workload Android.

## Encerrar

Ctrl+C nos terminais da API e do painel. Sair encerra a sessão do cliente; tokens já emitidos vencem em até 20 minutos. O modo atual usa dados de demonstração, sem operações bancárias.
