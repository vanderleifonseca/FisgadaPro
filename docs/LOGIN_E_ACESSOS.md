# Login e permissões — 0.3

As credenciais já criadas no seu computador são preservadas em user-secrets. Não há senha padrão. Se já acessava o sistema, não precisa reconfigurar.

Os e-mails abaixo são os exemplos anteriores; se foram alterados no configurador, use os que você cadastrou:

| Conta de exemplo | Acesso nesta versão |
| --- | --- |
| plataforma@fisgada.test | SuperAdmin: cadastra clubes e presidentes, gerencia todos os clubes |
| administrador@fisgada.test | Administrador de foz 2 |
| socio@fisgada.test | Alex Exemplo, fundador de foz 2, com dependente Bia |

Saia e entre novamente após atualizar: a identidade de sessão e o perfil de plataforma foram ajustados. As chaves antigas do configurador (Administrador, Socio e Plataforma) permanecem compatíveis. A plataforma agora possui gestão global, conforme o novo requisito.

## Primeira instalação sem contas

```powershell
dotnet run --project tools/FisgadaPro.Setup
```

O configurador pede e-mail e senha dos três perfis iniciais e grava somente hashes em user-secrets, fora do Git. A senha mínima de quatro caracteres é mantida para o desenvolvimento local. Não publique senhas, hashes ou arquivos App_Data.

## Liberar outros usuários

Entre com SuperAdmin, selecione o clube e abra Acessos para cadastrar seu presidente. O presidente pode liberar administrador e tesoureiro do próprio clube e acessos Socio vinculados a pessoas cadastradas. Administrador libera acesso Socio; não cria presidentes nem super administradores. Categoria de associado não é perfil de usuário.

Tesoureiro usa o painel web financeiro. Associado e dependente usam o app ou área própria no navegador. Cada conta criada é um usuário distinto; não compartilhe credenciais de função.

Inativar o cadastro associativo bloqueia a conta Socio nas próximas requisições. Alterar senha, recuperar senha e desativar diretamente contas administrativas ainda são pendências. Tokens vencem em até 20 minutos; sair remove a sessão do cliente.

## Conectar o celular

No PC, execute API em Development, na rede local:

```powershell
dotnet run --project src/FisgadaPro.Api --launch-profile http --urls http://0.0.0.0:5187
```

PC e celular devem estar na mesma rede. No app informe `http://IP_DO_COMPUTADOR:5187`. Esse IP é do PC; IP/porta do pareamento ADB são outra conexão e não devem ser usados como endereço da API. Se necessário, autorize a porta da API apenas na rede privada do Windows.

O transporte HTTP é destinado ao teste local. Para produção será necessário HTTPS e revisão de segurança. Veja [Execução](EXECUCAO_VSCODE.md) e [Roteiro de teste](TESTE_0_3.md).
