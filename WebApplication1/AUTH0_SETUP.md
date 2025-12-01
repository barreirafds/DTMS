# Configuração do Auth0

Este documento explica como configurar o Auth0 no projeto.

## ⚠️ IMPORTANTE: URLs Corretas

**As URLs no Auth0 Dashboard devem corresponder EXATAMENTE às portas da sua aplicação!**

As portas configuradas no projeto são:
- **HTTPS**: `7003` (projeto) ou `44369` (IIS Express)  
- **HTTP**: `5150` (projeto) ou `10498` (IIS Express)

## 📋 URLs para Copiar e Colar Diretamente

**Copie EXATAMENTE as linhas abaixo (sem os backticks) e cole no Auth0 Dashboard:**

### Allowed Callback URLs:
```
https://localhost:7003/Callback,http://localhost:5150/Callback,https://localhost:44369/Callback,http://localhost:10498/Callback
```

### Allowed Logout URLs:
```
https://localhost:7003/,http://localhost:5150/,https://localhost:44369/,http://localhost:10498/
```

### Allowed Web Origins:
```
https://localhost:7003,http://localhost:5150,https://localhost:44369,http://localhost:10498
```

**⚠️ DICA**: Se o Auth0 rejeitar com vírgulas, tente colar uma URL por vez ou uma por linha!

**💡 SOLUÇÃO RÁPIDA**: Se estiver dando erro, comece adicionando apenas UMA URL para testar:
- Callback: `https://localhost:7003/Callback`
- Logout: `https://localhost:7003/`
- Web Origin: `https://localhost:7003`

Depois de funcionar, adicione as outras portas.

📄 **Arquivo com URLs prontas**: Veja também o arquivo `AUTH0_URLS.txt` na raiz do projeto.

## Passos de Configuração

### 1. Identificar as Portas da Aplicação

Antes de configurar o Auth0, identifique as portas que sua aplicação está usando:

1. Abra o arquivo `Properties/launchSettings.json`
2. Verifique as portas configuradas:
   - **Perfil HTTPS**: Porta HTTPS (geralmente 7003 para projeto direto, ou 44369 para IIS Express)
   - **Perfil HTTP**: Porta HTTP (geralmente 5150 para projeto direto, ou 10498 para IIS Express)

As portas atuais do projeto são:
- **HTTPS**: `7003` (projeto) ou `44369` (IIS Express)
- **HTTP**: `5150` (projeto) ou `10498` (IIS Express)

### 2. Configurar Auth0 no Dashboard

1. Acesse o [Auth0 Dashboard](https://manage.auth0.com/)
2. Crie uma nova aplicação do tipo "Regular Web Application" (ou selecione a existente)
3. Vá para a aba **Settings**
4. Configure as seguintes URLs (use as portas que você identificou acima):

   - **Allowed Callback URLs**: 
     
     **OPÇÃO 1 - Copie e cole todas as URLs separadas por vírgula:**
     ```
     https://localhost:7003/Callback,http://localhost:5150/Callback,https://localhost:44369/Callback,http://localhost:10498/Callback
     ```
     
     **OPÇÃO 2 - Ou cole uma URL por linha (se o Auth0 aceitar):**
     ```
     https://localhost:7003/Callback
     http://localhost:5150/Callback
     https://localhost:44369/Callback
     http://localhost:10498/Callback
     ```
     
     ⚠️ **IMPORTANTE**: 
     - Remova os backticks (```) antes de colar
     - Não deixe espaços extras antes ou depois das URLs
     - Use vírgula SEM espaços entre as URLs se colar tudo junto

   - **Allowed Logout URLs**: 
     
     **Copie e cole todas as URLs separadas por vírgula:**
     ```
     https://localhost:7003/,http://localhost:5150/,https://localhost:44369/,http://localhost:10498/
     ```

   - **Allowed Web Origins**: 
     
     **Copie e cole todas as URLs separadas por vírgula:**
     ```
     https://localhost:7003,http://localhost:5150,https://localhost:44369,http://localhost:10498
     ```

### 3. Configurar appsettings.json

Abra o arquivo `appsettings.json` e configure os valores do Auth0:

```json
{
  "Auth0": {
    "Domain": "SEU-DOMINIO.auth0.com",
    "ClientId": "SEU-CLIENT-ID"
  }
}
```

**Importante**: Substitua:
- `SEU-DOMINIO.auth0.com` pelo seu domínio Auth0 (ex: `dev-abc123.us.auth0.com`)
- `SEU-CLIENT-ID` pelo Client ID da sua aplicação Auth0

### 4. Configurar appsettings.Development.json

Adicione as mesmas configurações no arquivo `appsettings.Development.json`:

```json
{
  "Auth0": {
    "Domain": "SEU-DOMINIO.auth0.com",
    "ClientId": "SEU-CLIENT-ID"
  }
}
```

## Como Funciona

### Fluxo de Autenticação

1. O usuário acessa a página `/Login`
2. É redirecionado para o Auth0 para fazer login
3. Após o login, o Auth0 redireciona para `/Callback`
4. O Callback:
   - Busca ou cria um usuário no banco de dados local usando o email do Auth0
   - Adiciona o ID do usuário local como claim
   - Redireciona o usuário para a página inicial

### Mapeamento de Usuários

- O sistema usa o **email do Auth0** para identificar o usuário
- O username no banco de dados é criado a partir da parte antes do "@" do email
- Se o usuário não existir no banco de dados, ele é criado automaticamente

### Claims Disponíveis

Após o login, os seguintes claims estão disponíveis:
- `ClaimTypes.NameIdentifier`: ID do usuário no banco de dados local
- `ClaimTypes.Email`: Email do Auth0
- `ClaimTypes.Name`: Nome do usuário do Auth0

## Estrutura de Arquivos

- `Program.cs`: Configuração do Auth0 middleware
- `Pages/Login.cshtml.cs`: Página de login que redireciona para Auth0
- `Pages/Callback.cshtml.cs`: Handler do callback do Auth0
- `Pages/Logout.cshtml.cs`: Página de logout do Auth0
- `Pages/Shared/_Layout.cshtml`: Layout atualizado com menu de usuário

## Notas Importantes

1. **Segurança**: Nunca commite as credenciais do Auth0 no repositório. Use User Secrets ou variáveis de ambiente para produção.

2. **User Secrets (Desenvolvimento)**: Para desenvolver localmente sem expor credenciais:
   ```bash
   dotnet user-secrets set "Auth0:Domain" "SEU-DOMINIO.auth0.com"
   dotnet user-secrets set "Auth0:ClientId" "SEU-CLIENT-ID"
   ```

3. **Produção**: Configure as URLs de callback e logout corretas no Auth0 Dashboard para seu domínio de produção.

4. **Banco de Dados**: O sistema cria automaticamente usuários no banco de dados quando fazem login pela primeira vez. Certifique-se de que o banco de dados está acessível.

## Troubleshooting

### Erro: "Auth0:Domain configuration is missing"
- Verifique se as configurações estão no `appsettings.json` ou `appsettings.Development.json`
- Verifique se está usando User Secrets corretamente

### Erro: "Invalid callback URL", "link errado" ou "callbacks must be a valid uri"

Este erro geralmente acontece quando o formato das URLs está incorreto. Siga estas instruções:

1. **Formato correto das URLs**:
   - Cada URL deve começar com `http://` ou `https://`
   - Não deve haver espaços antes ou depois das URLs
   - Use vírgulas para separar múltiplas URLs (sem espaço após a vírgula)

2. **URLs corretas para copiar diretamente** (copie EXATAMENTE como está abaixo):

   **Allowed Callback URLs** - copie esta linha completa:
   ```
   https://localhost:7003/Callback,http://localhost:5150/Callback,https://localhost:44369/Callback,http://localhost:10498/Callback
   ```

   **Allowed Logout URLs** - copie esta linha completa:
   ```
   https://localhost:7003/,http://localhost:5150/,https://localhost:44369/,http://localhost:10498/
   ```

   **Allowed Web Origins** - copie esta linha completa:
   ```
   https://localhost:7003,http://localhost:5150,https://localhost:44369,http://localhost:10498
   ```

3. **Verificações importantes**:
   - As URLs devem corresponder EXATAMENTE às portas da sua aplicação
   - Verifique as portas no arquivo `Properties/launchSettings.json`
   - O caminho é `/Callback` (com C maiúsculo) - não `/callback` ou `/CALLBACK`
   - Não copie os backticks (```) quando colar no Auth0 Dashboard
   - Se o Auth0 Dashboard aceitar, você pode colar uma URL por linha em vez de separar por vírgula

### Usuário não autenticado após login
- Verifique os logs da aplicação
- Verifique se o Callback está sendo chamado corretamente
- Verifique se o banco de dados está acessível para criar/buscar usuários

