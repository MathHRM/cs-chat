import router from "@/routes";
import { register } from "@/api/register";
import { useAuthStore } from "@/stores/auth";
import allCommands from "@/commands/commands";
import { login } from "@/api/login";

const authStore = useAuthStore();

function saveMessage(messages, message, user, chat) {
  messages.value.push({
    user: {
      username: user,
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

export async function handleRegister({ username, password }, messages) {
  const data = await register(username, password);

  if (!data?.user?.id) {
    alert("register-failed");

    return;
  }

  authStore.setUser(data.user);
  localStorage.setItem("@auth", `${data.token}`);

  router.push("/");
}

export async function handleLogin({ username, password }, messages) {
  const data = await login(username, password);

  if (!data?.user?.id) {
    alert("login-failed");

    return;
  }

  authStore.setUser(data.user);
  localStorage.setItem("@auth", `${data.token}`);

  router.push("/");
}

export function handleHelp(messages, commands) {
  const helpMessage =
    "Available commands:\n\n" +
    Object.entries(commands || allCommands)
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

export function getCommandArgs(command, pageCommands, messages) {
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
    if (commandOnly == "help") {
      handleHelp(messages, pageCommands);
    }

    return {};
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

export function handleCommand(command, messages, user, chat, pageCommands) {
  if (!pageCommands) {
    pageCommands = allCommands;
  }

  saveMessage(messages, command, user, chat);

  command = command.trim();

  if (!command.startsWith("/")) {
    alert("not-a-command");

    return;
  }

  const commandName = getCommandOnly(command);
  const commandArgs = getCommandArgs(command, pageCommands, messages);

  if (!pageCommands[commandName]) {
    alert("command-not-found");

    return;
  }

  if (Object.keys(commandArgs.errors).length > 0) {
    for (const error of Object.keys(commandArgs.errors)) {
      alert(commandArgs.errors[error], {
        arg: error,
      });
    }

    return;
  }

  pageCommands[commandName].handler(commandArgs.args, messages);
}
