import router from "@/routes";
import { register } from "@/api/register";
import { useAuthStore } from "@/stores/auth";
import allCommands from "@/commands/commands";
import { login } from "@/api/login";
import { joinChat } from "@/api/joinChat";
import { HubConnectionState } from "@aspnet/signalr";

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

export async function handleRegister({ username, password }) {
  const authStore = useAuthStore();
  const data = await register(username, password);

  if (!data?.user?.id) {
    alert("register-failed");

    return;
  }

  authStore.setUser(data.user);
  localStorage.setItem("@auth", `${data.token}`);

  router.push("/");
}

export async function handleLogin({ username, password }) {
  const authStore = useAuthStore();
  const data = await login(username, password);

  if (!data?.user?.id) {
    alert("login-failed");

    return;
  }

  authStore.setUser(data.user);
  localStorage.setItem("@auth", `${data.token}`);

  router.push("/");
}

export async function handleJoinChat({ chatId, hubConnection }) {
  const data = await joinChat(chatId);

  if (!data?.user?.id) {
    alert("join-chat-failed");

    return;
  }

  hubConnection.invoke("JoinChat", chatId);
}

export function handleHelp({ messages, pageCommands }) {
  if (!pageCommands) {
    pageCommands = allCommands;
  }

  const helpMessage =
    "Available commands:\n\n" +
    Object.entries(pageCommands || allCommands)
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
    console.log("no match");

    return null;
  }

  console.log("match", match[1]);

  return match[1];
}

export function getCommandArgs(command, pageCommands) {
  const args = {
    errors: {},
    args: {},
  };

  if (!pageCommands) {
    pageCommands = allCommands;
  }

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
      args.errors[arg] = "argument-required";
    }

    args.args[arg] = value;
  }

  return args;
}

function alert(messages, chat, content, context) {
  const translate = {
    login: "Logged in successfully",
    register: "Registered successfully",
    "login-failed": "Wrong username or password",
    "register-failed": "Could not register",
    "command-not-found": "Command does not exist",
    "not-a-command": "Not a command",
    "not-enough-args": "Not enough arguments",
    "argument-required": "The argument :arg is required",
  };

  const contentMessage = translate[content].replace(
    /:arg/g,
    context?.arg || ""
  );

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

export function handleMessage(
  messages,
  message,
  chat,
  pageCommands,
  user,
  hubConnection
) {
  message = message.trim();

  if (!message) {
    return;
  }

  if (!pageCommands) {
    pageCommands = allCommands;
  }

  if (!message.startsWith("/")) {
    if (
      hubConnection &&
      hubConnection.state == HubConnectionState.Connected
    ) {
      hubConnection.invoke("SendMessage", {content: message});

      return;
    }
  }

  saveMessage(messages, message, user, chat);

  if (!/^\/\w+/.test(message)) {
    return;
  }

  const commandName = getCommandOnly(message);
  const commandArgs = getCommandArgs(message, pageCommands);

  if (!pageCommands[commandName]) {
    alert(messages, chat, "command-not-found");

    return;
  }

  if (Object.keys(commandArgs.errors).length > 0) {
    for (const error of Object.keys(commandArgs.errors)) {
      alert(messages, chat, commandArgs.errors[error], {
        arg: error,
      });
    }

    return;
  }

  commandArgs.args.hubConnection = hubConnection;
  commandArgs.args.messages = messages;

  pageCommands[commandName].handler(commandArgs.args);
}
