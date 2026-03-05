# Broadcast CLI 🚀

Um cliente de chat em linha de comando (CLI) moderno e colorido, desenvolvido em C#, que utiliza **SignalR** para comunicação em tempo real e **Spectre.Console** para uma interface de usuário rica no terminal.

## 📌 Funcionalidades

- **Autenticação JWT:** Login seguro via API para obtenção de token de acesso.
- **Mensagens Privadas em Tempo Real:** Comunicação instantânea entre usuários usando SignalR Hubs.
- **Interface Rica (CLI):** Uso de cores, painéis, tabelas e spinners para uma melhor experiência do usuário.
- **Gerenciamento de Estado de Conexão:** Notificações automáticas de reconexão e perda de sinal.
- **Comandos de Chat:**
    - `/target`: Muda o usuário com quem você está conversando.
    - `/exit`: Sai do chat ou do aplicativo.

## 🛠️ Tecnologias Utilizadas

- **C# 14 / .NET 10**
- **SignalR Client:** Para comunicação WebSocket bidirecional.
- **Spectre.Console:** Para renderização de UI no terminal.
- **Microsoft.Extensions.Configuration:** Para gerenciamento de configurações via JSON.

## ⚙️ Configuração

Antes de iniciar, certifique-se de configurar as URLs da sua API e do Hub SignalR no arquivo `BroadcastServer/appsettings.json`:

```json
{
  "SignalR": {
    "HubUrl": "https://sua-api.com/chatHub",
    "ApiUrl": "https://sua-api.com/api"
  }
}
```

## 🚀 Como Executar

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) instalado.

### Passo a Passo
1. Clone o repositório.
2. Navegue até a pasta do projeto:
   ```bash
   cd BroadcastServer/BroadcastServer
   ```
3. Restaure as dependências:
   ```bash
   dotnet restore
   ```
4. Execute o aplicativo:
   ```bash
   dotnet run
   ```

## 📖 Como Usar

1. **Login:** Ao iniciar, insira seu e-mail e senha cadastrados.
2. **Seleção de Alvo:** Digite o ID numérico do usuário com quem deseja conversar.
3. **Conversando:** Digite suas mensagens e pressione `Enter` para enviar.
4. **Comandos:**
    - Para trocar de conversa sem fechar o app, digite `/target`.
    - Para fechar o aplicativo, digite `/exit`.

## 📂 Estrutura do Projeto

- `Models/`: DTOs para transferência de dados (Login, Mensagens).
- `Services/`: Lógica de autenticação (`AuthService`) e cliente SignalR (`SignalRClient`).
- `Utils/`: Helpers para parsing de JWT e componentes de UI (`Logger`).
- `Program.cs`: Ponto de entrada e fluxo principal da aplicação.

---
Desenvolvido como um exemplo de cliente SignalR robusto para terminal.
