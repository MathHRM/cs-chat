import { HubConnectionState } from "@aspnet/signalr";
import { useMessagesStore } from "@/stores/messages";

export default function handleMessage(
  message,
  hubConnection,
  user,
  chat,
  t
) {
  const messagesStore = useMessagesStore();
  message = message.trim();

  if (!message) {
    return;
  }

  if (!hubConnection || hubConnection.state != HubConnectionState.Connected) {
    alert(t("alerts.connection-failed"), 1);
    return;
  }

  if (isCommand(message)) {
    messagesStore.addMessage({
      content: message,
      type: 0,
      user: {
        username: user.username,
      },
      chat: {
        id: chat.id,
        name: chat.name,
      },
    });
  }

  hubConnection.invoke("SendMessage", { content: message });
}

export function isCommand(message) {
  return message.trim().startsWith("/");
}

export function alert(content, type) {
  const messagesStore = useMessagesStore();

  messagesStore.addMessage({
    content,
    type,
  });
}
