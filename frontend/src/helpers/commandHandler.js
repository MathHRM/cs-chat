import { alert } from "./messageHandler";
import { useAuthStore } from "@/stores/auth";
import { useChatStore } from "@/stores/chat";
import { useMessagesStore } from "@/stores/messages";
import router from "@/routes";

export default function handleCommand(command, t) {
  console.log(command);

  if (command.result != 0) {
    if (command.errors && Object.keys(command.errors).length > 0) {
      Object.values(command.errors).forEach((message) => {
        alert(message, 1);
      });

      return;
    }

    alert(command.message, 1);

    return;
  }

  switch (command.command) {
    case "help":
      handleHelp(command);
      break;
    case "login":
      handleLogin(command);
      break;
    case "register":
      handleLogin(command);
      break;
    case "logout":
      handleLogout(command);
      break;
    case "join":
      handleJoin(command, t);
      break;
    case "chat":
      handleJoin(command, t);
      break;
    case "create":
      handleJoin(command, t);
      break;
    default:
      console.log(command);
      alert(t("alerts.command-handle"), 2);
      break;
  }
}

function handleHelp(command) {
  const messagesStore = useMessagesStore();

  messagesStore.addMessage(command.response);
}

function handleLogin(command) {
  const authStore = useAuthStore();
  const chatStore = useChatStore();
  const data = command.response;

  if (!data?.user?.id) {
    alert(command.message, 1);

    return;
  }

  authStore.setUser(data.user);
  chatStore.setChat(data.chat);
  localStorage.setItem("@auth", `${data.token}`);

  router.push("/");
}

function handleLogout() {
  const authStore = useAuthStore();
  const chatStore = useChatStore();

  authStore.$reset();
  chatStore.$reset();
  localStorage.removeItem("@auth");

  router.push("/login");
}

function handleJoin(command, t) {
  const data = command.response;

  if (!data?.chat?.id) {
    alert(command.message, 1);

    return;
  }

  const chatStore = useChatStore();

  chatStore.setChat(data.chat);

  const messagesStore = useMessagesStore();
  messagesStore.clearMessages();

  alert(t("alerts.joined-chat", { chatId: data.chat.name }), 3);
}
