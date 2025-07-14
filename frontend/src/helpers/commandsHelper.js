import router from "@/routes";
import { register } from "@/api/register";
import { useAuthStore } from "@/stores/auth";
import commands from "@/commands/commands";
import { login } from "@/api/login";
import { joinChat } from "@/api/joinChat";
import { HubConnectionState } from "@aspnet/signalr";
import { useChatStore } from "@/stores/chat";

function saveMessage(messages, message, user, chat) {
  messages.value.push({
    user: {
      username: user?.username || '~',
    },
    chat: {
      id: chat,
    },
    message: {
      content: message,
      created_at: new Date(),
    },
  });
}

export async function handleRegister({ username, password, messages, chat, t }) {
  const authStore = useAuthStore();
  const data = await register(username, password);

  if (!data?.user?.id) {
    alert({messages, chat, content: "commands.register.response.failed", t});

    return;
  }

  authStore.setUser(data.user);
  localStorage.setItem("@auth", `${data.token}`);

  router.push("/");
}

export async function handleLogin({ username, password, messages, chat, t }) {
  const authStore = useAuthStore();
  const data = await login(username, password);

  if (!data?.user?.id) {
    alert({messages, chat, content: "commands.login.response.failed", t});

    return;
  }

  authStore.setUser(data.user);
  localStorage.setItem("@auth", `${data.token}`);

  router.push("/");
}

export async function handleLogout() {
  const authStore = useAuthStore();
  const chatStore = useChatStore();

  authStore.$reset();
  chatStore.$reset();
  localStorage.removeItem("@auth");

  router.push("/login");
}

export async function handleJoinChat({ chatId, hubConnection, messages, chat, t }) {
  const data = await joinChat(chatId);

  if (!data?.chat?.id) {
    alert({messages, chat, content: "commands.join.response.failed", context: {chatId}, t});

    return;
  }

  const authStore = useAuthStore();
  const chatStore = useChatStore();
  const user = authStore.user;

  user.currentChatId = chatId;

  authStore.setUser(user);
  chatStore.setChat(data.chat);

  messages.value = [];
  hubConnection.invoke("JoinChat", chatId);
}

export function handleHelp({ messages, pageCommands }) {
  const helpMessage =
    "Available commands:\n\n" +
    Object.entries(pageCommands || commands())
      .map(([name, cmd]) => {
        const argsDescription =
          Object.entries(cmd.args).length > 0
            ? Object.entries(cmd.args)
                .map(
                  ([argName, argInfo]) =>
                    `--${argName} | -${argName[0]} - ${argInfo.description}`
                )
                .join("\n    ")
            : "No arguments required";

        return `/${name} - ${cmd.description}\n    ${argsDescription}`;
      })
      .join("\n\n");

  saveMessage(messages, helpMessage, "System", "chat");
}

function getCommandOnly(command) {
  const match = command.match(/^\/\s*(\w+)/);

  if (!match) {
    return null;
  }

  return match[1];
}

export function getCommandArgs(command, pageCommands) {
  const args = {
    errors: {},
    args: {},
  };

  let commandOnly = getCommandOnly(command);
  let commandArgs = pageCommands[commandOnly]?.args;

  if (!commandArgs || Object.keys(commandArgs).length == 0) {
    return args;
  }

  for (const arg of Object.keys(commandArgs)) {
    const regex = new RegExp(`(?:--${arg}|-${arg[0]})=([^\\s]+)`);
    const found = command.match(regex);
    let value = found ? found[1] : null;

    if (!args.args[arg] && commandArgs[arg].required && value === null) {
      args.errors[arg] = "required";
    }

    args.args[arg] = value;
  }

  return args;
}

function alert({messages, chat, content, context, t}) {
  const contentMessage = t(content, context);

  messages.value.push({
    user: {
      username: "System",
    },
    chat: {
      id: chat,
    },
    message: {
      content: contentMessage,
      created_at: new Date(),
    },
  });

  return contentMessage;
}

function isCommand(message) {
  return /^\/\w+/.test(message);
}

export default function handleMessage(
  messages,
  message,
  chat,
  pageCommands,
  user,
  hubConnection,
  t
) {
  message = message.trim();

  if (!message) {
    return;
  }

  if (!pageCommands) {
    pageCommands = commands();
  }

  if (!isCommand(message)) {
    if (
      hubConnection &&
      hubConnection.state == HubConnectionState.Connected
    ) {
      hubConnection.invoke("SendMessage", {content: message});

      return;
    }
  }

  saveMessage(messages, message, user, chat);

  if (!isCommand(message)) {
    return;
  }

  handleCommand(messages, message, chat, pageCommands, user, hubConnection, t);
}

function handleCommand(
  messages,
  message,
  chat,
  pageCommands,
  user,
  hubConnection,
  t
) {
  const commandName = getCommandOnly(message);
  const commandArgs = getCommandArgs(message, pageCommands);

  if (!pageCommands[commandName]) {
    alert({messages, chat, content: "commands.errors.command-not-found", t});

    return;
  }

  if (Object.keys(commandArgs.errors).length > 0) {
    for (const error of Object.keys(commandArgs.errors)) {
      alert({
        messages,
        chat,
        content: `commands.arguments.${commandArgs.errors[error]}`,
        context: {
          arg: error,
        },
        t,
      });
    }

    return;
  }

  commandArgs.args.hubConnection = hubConnection;
  commandArgs.args.messages = messages;
  commandArgs.args.pageCommands = pageCommands;
  commandArgs.args.chat = chat;
  commandArgs.args.user = user;
  commandArgs.args.t = t;

  hundle(pageCommands[commandName], commandArgs.args);
}

function hundle(command, commandArgs) {
  const result = command.handler(commandArgs);

  if (!result) {
    return;
  }
}