import { HubConnectionState } from "@aspnet/signalr";

export default function handleMessage(
  message,
  hubConnection,
  messages,
  user,
  chat
) {
  message = message.trim();

  if (!message) {
    return;
  }

  if (!hubConnection || hubConnection.state != HubConnectionState.Connected) {
    alert(messages, "Connection not established", 1);
    return;
  }

  if (isCommand(message)) {
    messages.value.push({
      content: message,
      type: 0,
      user: {
        username: user.username,
      },
      chat: {
        id: chat.id,
      },
    });
  }

  hubConnection.invoke("SendMessage", { content: message });
}

export function isCommand(message) {
  return message.trim().startsWith("/");
}

export function alert(messages, content, type) {
  messages.value.push({
    content,
    type,
  });
}