import { alert } from "./messageHandler";
import { useAuthStore } from "@/stores/auth";
// import { useChatStore } from "@/stores/chat";
import router from "@/routes";

export default function handleCommand(command, messages) {
  console.log(command);

  if (!command.command) {
    return;
  }

  if (command.result != 0) {
    alert(messages, command.message, 1);
  }

  switch (command.command) {
    case "help":
      handleHelp(messages, command);
      break;
    case "login":
      handleLogin(messages, command);
      break;
    case "register":
      handleRegister(messages, command);
      break;
    default:
      console.log(command);
      break;
  }
}

function handleHelp(messages, command) {
  messages.value.push(command.response);
}

function handleLogin(messages, command) {
  const authStore = useAuthStore();
  //   const chatStore = useChatStore();
  const data = command.response;

  if (!data?.user?.id) {
    alert(messages, command.message, 1);

    return;
  }

  //   let chatData = await getChat(data.user.currentChatId);

  authStore.setUser(data.user);
  //   chatStore.setChat(chatData);
  localStorage.setItem("@auth", `${data.token}`);

  router.push("/");
}

function handleRegister(messages, command) {
  alert(messages, command.message, 1);
}
