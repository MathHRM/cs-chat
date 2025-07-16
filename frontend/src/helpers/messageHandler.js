import { HubConnectionState } from "@aspnet/signalr";

export default function handleMessage(
  message,
  hubConnection,
  messages
) {
  message = message.trim();

  if (!message) {
    return;
  }

  if (!hubConnection || hubConnection.state != HubConnectionState.Connected) {
    alert(messages, "Connection not established", 1);
    return;
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