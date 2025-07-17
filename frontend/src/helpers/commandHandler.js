import { alert } from "./messageHandler";
import { useAuthStore } from "@/stores/auth";
import { useChatStore } from "@/stores/chat";
import router from "@/routes";

export default function handleCommand(command, messages, t) {
  console.log(command);

  if (command.result != 0) {
    if (command.errors && Object.keys(command.errors).length > 0) {
      Object.values(command.errors).forEach((message) => {
        alert(messages, message, 1);
      });

      return;
    }

    alert(messages, command.message, 1);

    return;
  }

  switch (command.command) {
    case "help":
      handleHelp(messages, command);
      break;
    case "login":
      handleLogin(messages, command);
      break;
    case "register":
      handleLogin(messages, command);
      break;
    case "logout":
      handleLogout(messages, command);
      break;
    case "join":
      handleJoin(messages, command, t);
      break;
    case "chat":
      handleJoin(messages, command, t);
      break;
    default:
      console.log(command);
      alert(messages, t("alerts.command-handle"), 2);
      break;
  }
}

function handleHelp(messages, command) {
  messages.value.push(command.response);
}

function handleLogin(messages, command) {
  const authStore = useAuthStore();
  const chatStore = useChatStore();
  const data = command.response;

  if (!data?.user?.id) {
    alert(messages, command.message, 1);

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

function handleJoin(messages, command, t) {
  const data = command.response;

  if (!data?.chat?.id) {
    alert(messages, command.message, 1);

    return;
  }

  const chatStore = useChatStore();

  chatStore.setChat(data.chat);

  messages.value = [];

  alert(messages, t("alerts.joined-chat", { chatId: data.chat.name }), 3);
}
