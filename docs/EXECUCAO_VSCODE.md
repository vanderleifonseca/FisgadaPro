# Executar o Fisgada Pro no Visual Studio Code (azul)

## 1. Obter o código

Se ainda não houver uma cópia local, abra o PowerShell:

~~~powershell
cd C:\maui
git clone https://github.com/vanderleifonseca/FisgadaPro.git
cd FisgadaPro
code .
~~~

Resultado esperado: VS Code aberto na pasta que contém FisgadaPro.slnx.

Se já houver cópia local, entre nela, confira `git status` e use `git pull --ff-only` somente com as alterações locais preservadas.

## 2. Conferir o SDK e as extensões

~~~powershell
dotnet --version
~~~

Resultado esperado: versão 10.0.x. O SDK 10.0.401 já usado nos seus projetos é compatível com o global.json.

Instale as extensões recomendadas: C#, C# Dev Kit e .NET MAUI. Use Terminal → Novo Terminal, ou Ctrl+Shift+P → Tasks: Run Task para as tarefas Fisgada Pro.

## 3. Compilar o servidor e as regras

~~~powershell
dotnet build FisgadaPro.slnx
~~~

Resultado esperado: compilação concluída sem erros. A solução principal não inclui App, portanto não exige Android SDK nem workload MAUI.

## 4. Verificar os cálculos

~~~powershell
dotnet run --project tests/FisgadaPro.Domain.Checks
~~~

Resultado esperado: “13 verificações de regras concluídas com sucesso.” Este projeto é um verificador executável; use este comando, não dotnet test.

## 5. Abrir o painel

~~~powershell
dotnet run --project src/FisgadaPro.Web --launch-profile http
~~~

Resultado esperado: “Now listening on: http://localhost:5216”. Mantenha o terminal aberto e acesse esse endereço.

Selecione Clube Lago e consulte Alex Exemplo. A prévia apresenta mensalidade de R$ 150,00 e gastos de Bia Exemplo de R$ 25,00 e R$ 80,00, totalizando R$ 255,00. São valores fictícios.

Troque para Clube Rio: os associados e o extrato anterior devem desaparecer. Consulte Dani Exemplo, cujo exemplo tem isenção.

## 6. Abrir a API em outro terminal

~~~powershell
dotnet run --project src/FisgadaPro.Api --launch-profile http
~~~

Resultado esperado: API em http://localhost:5187. Teste:

~~~powershell
Invoke-RestMethod http://localhost:5187/api/v1/status
Invoke-RestMethod http://localhost:5187/api/v1/demonstracao/clubes
~~~

O primeiro comando retorna sistema, versão e estado; o segundo retorna dois clubes fictícios. Não há Swagger nem endpoints de gravação nesta entrega. O painel demonstra os dados em memória sem depender do processo da API.

## 7. Aplicativo Android — etapa de validação local

O projeto App foi gerado do template oficial MAUI e adaptado para Android. Ele ainda não foi compilado ou executado em dispositivo nesta entrega.

~~~powershell
dotnet workload list
adb devices
~~~

Resultado esperado: workload MAUI/Android disponível e dispositivo com estado device. Se faltar o workload, instalar conforme sua configuração do .NET:

~~~powershell
dotnet workload install maui
~~~

Com SDK Android e JDK configurados:

~~~powershell
dotnet build src/FisgadaPro.App/FisgadaPro.App.csproj
dotnet build src/FisgadaPro.App/FisgadaPro.App.csproj -t:Run
~~~

A segunda operação exige um emulador iniciado ou celular conectado; selecione o dispositivo na extensão .NET MAUI quando houver mais de um. O app usa dados fictícios locais, sem depender da API ou de endereços de rede do emulador.

Android é o alvo inicial. iOS poderá ser incluído depois, com o ambiente de compilação apropriado.

## Encerrar

Ctrl+C em cada terminal que estiver executando o painel ou a API. Não publique esta demonstração: autenticação, permissões e persistência serão implementadas antes de operação real.
