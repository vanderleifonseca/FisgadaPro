# Login e acessos — versão 0.2.0

## 1. Atualizar e compilar

Encerre API e painel com Ctrl+C para liberar os arquivos. Na raiz C:\maui\FisgadaPro:

~~~powershell
git pull --ff-only
dotnet build FisgadaPro.slnx
~~~

Resultado esperado: compilação sem erros. Se o Git apontar alterações locais, preserve-as antes de continuar; não use reset para descartá-las.

## 2. Configurar as contas locais

~~~powershell
dotnet run --project tools/FisgadaPro.Setup
~~~

O programa pergunta e-mail e senha para cada perfil:

| Perfil | E-mail sugerido (pode alterar) | Vínculo fictício |
| --- | --- | --- |
| Administrador | administrador@fisgada.test | Administração do Clube Lago |
| Socio | socio@fisgada.test | Alex Exemplo, titular de Bia |
| Plataforma | plataforma@fisgada.test | Consulta dos clubes |

Pressione Enter para aceitar cada e-mail sugerido. Escolha uma senha de 4 a 256 caracteres, conforme a regra temporária para estas contas locais de teste, e confirme-a; os caracteres não aparecem enquanto você digita. Use e-mails diferentes para os perfis.

Resultado esperado: “Contas de teste configuradas”. Somente hashes de senha são gravados no user-secrets FisgadaPro.Api.Local, fora do Git. Não envie suas senhas na conversa.

O configurador pode ser executado novamente para alterar as três contas. Reinicie a API depois. Tokens já emitidos expiram em até 20 minutos; nesta etapa não há revogação remota imediata.

## 3. Abrir API e painel

Primeiro terminal:

~~~powershell
dotnet run --project src/FisgadaPro.Api --launch-profile http
~~~

Segundo terminal:

~~~powershell
dotnet run --project src/FisgadaPro.Web --launch-profile http
~~~

Abra http://localhost:5216/entrar e teste cada conta. Use Sair antes de trocar de usuário.

- Administrador: lista apenas os associados do Clube Lago e consulta suas prévias.
- Socio: mostra Área do associado e somente a conta de Alex com despesas de Bia.
- Plataforma: lista os clubes, sem expor sócios ou extratos.

Sem configurar contas, nenhum login é aceito. Cinco tentativas de login por minuto por IP são permitidas; após isso aguarde um minuto. A sessão dura 20 minutos; entre novamente quando expirar.

## 4. Conectar o aplicativo à API pela rede

A conexão ADB por Wi-Fi serve para instalar e depurar o app. O login usa outra conexão: celular → API do computador.

No computador, execute:

~~~powershell
ipconfig
~~~

Identifique o IPv4 do adaptador conectado à mesma rede do celular. Use o IP do computador, não o IP 192.168.86.82 do celular e não a porta da depuração ADB.

Encerre a API anterior e execute:

~~~powershell
dotnet run --project src/FisgadaPro.Api --launch-profile http -- --urls http://0.0.0.0:5187
~~~

Resultado esperado: servidor ouvindo na porta 5187. A rede precisa permitir comunicação entre os dispositivos e o Firewall do Windows deve permitir a API na rede privada usada no teste.

No navegador do celular, teste http://IP-DO-COMPUTADOR:5187/api/v1/status, substituindo o IP. A resposta deve mostrar Fisgada Pro e versão 0.2.0.

## 5. Atualizar o app

Com o celular já listado como device:

~~~powershell
dotnet build src/FisgadaPro.App/FisgadaPro.App.csproj -t:Run -p:AndroidSdkDirectory="C:\Program Files (x86)\Android\android-sdk"
~~~

Na tela do aplicativo:

1. Informe http://IP-DO-COMPUTADOR:5187.
2. Digite o e-mail e a senha escolhidos para Socio.
3. Toque Entrar.
4. Consulte a conta de Alex e as despesas familiares.
5. Toque Sair; a sessão e os dados devem desaparecer.

A conta Administrador deve receber orientação para usar o painel web. O app não oferece mais a lista geral de associados.

O endereço da API é lembrado; senha e token ficam apenas em memória. Ao fechar o processo do app será necessário entrar novamente. O modo Debug permite HTTP para o teste local; Release bloqueia tráfego sem HTTPS. Não use senhas de serviços reais nesse ambiente local de demonstração.

## 6. Validar as restrições

~~~powershell
dotnet run --project tests/FisgadaPro.Domain.Checks
dotnet run --project tests/FisgadaPro.Access.Checks
~~~

Resultados: 13 verificações de domínio e 22 de acesso. Os testes verificam login, senha incorreta, exigência de token, permissões, separação de clubes, composição familiar, formulário protegido contra CSRF e limite de tentativas.

## Escopo e próximos passos

Esta entrega autentica contas locais vinculadas a pessoas fictícias. Não é ainda um cadastro de usuários persistente nem um sistema pronto para operação real. A API e o painel continuam restritos ao ambiente Development. Não há recuperação de senha, MFA, cadastro público ou gestão de sessões.

A próxima etapa será escolher/configurar a persistência e implementar os cadastros reais e vínculos, mantendo a mesma verificação de permissões na API.
