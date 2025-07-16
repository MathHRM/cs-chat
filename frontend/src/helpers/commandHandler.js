import { alert } from "./messageHandler";
import { useAuthStore } from "@/stores/auth";
import { useChatStore } from "@/stores/chat";
import router from "@/routes";

export default function handleCommand(command, messages) {
  console.log(command);

  if (!command.command) {
    return;
  }

  if (command.result != 0) {
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
      handleJoin(messages, command);
      break;
    default:
      console.log(command);
      alert(messages, "Command could not be processed", 2);
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

function handleJoin(messages, command) {
  const data = command.response;

  if (!data?.chat?.id) {
    alert(messages, command.message, 1);

    return;
  }

  const chatStore = useChatStore();

  chatStore.setChat(data.chat);

  messages.value = [];

  alert(messages, `You have joined the chat ${data.chat.id}`, 3);
}
