<template>
  <div class="terminal-container">
    <CommandsComponent :messages="messages" />

    <CommandLine @send-message="handleCommand" :chat="'login-register'" />
  </div>
</template>

<script setup>
import { login } from "@/api/login";
import { register } from "@/api/register";
import CommandLine from "@/components/CommandLine.vue";
import CommandsComponent from "@/components/CommandsComponent.vue";
import { onMounted, ref } from "vue";
import router from "@/routes";
import { useAuthStore } from "@/stores/auth";

const authStore = useAuthStore();

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

const commands = {
  help: {
    description: "Show available commands and their usage",
    args: {},
    handler: handleHelp,
  },
  login: {
    description: "Login to the system",
    args: {
      username: {
        type: "string",
        description: "The username to login with",
        required: true,
      },
      password: {
        type: "string",
        description: "The password to login with",
        required: true,
      },
    },
    handler: handleLogin,
  },
  register: {
    description: "Register a new user",
    args: {
      username: {
        type: "string",
        description: "The username to register with",
        required: true,
      },
      password: {
        type: "string",
        description: "The password to register with",
        required: true,
      },
    },
    handler: handleRegister,
  },
};

let messages = ref([]);

function saveMessage(content, username = "~") {
  messages.value.push({
    user: {
      username: username,
    },
    chat: {
      id: "login-register",
    },
    message: {
      content: content,
      created_at: new Date(),
    },
  });
}

function alert(content, context) {
  const contentMessage = translate[content].replace(
    /:arg/g,
    context?.arg || ""
  );

  messages.value.push({
    user: {
      username: "System",
    },
    chat: {
      id: "login-register",
    },
    message: {
      content: contentMessage,
      created_at: new Date(),
    },
  });

  return contentMessage;
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

function getCommandArgs(command) {
  const args = {
    errors: {},
    args: {},
  };

  let commandOnly = getCommandOnly(command);
  let commandArgs = commands[commandOnly]?.args;

  if (!commandArgs) {
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

async function handleLogin({ username, password }) {
  const data = await login(username, password);

  if (!data?.user?.id) {
    alert("login-failed");

    return;
  }

  authStore.setUser(data.user);
  localStorage.setItem("@auth", `${data.token}`);

  router.push("/");
}

async function handleRegister({ username, password }) {
  const data = await register(username, password);

  if (!data?.user?.id) {
    alert("register-failed");

    return;
  }

  authStore.setUser(data.user);
  localStorage.setItem("@auth", `${data.token}`);

  router.push("/");
}

function handleHelp() {
  const helpMessage =
    "Available commands:\n\n" +
    Object.entries(commands)
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

  saveMessage(helpMessage, "System");
}

function handleCommand(command) {
  saveMessage(command);

  command = command.trim();

  if (!command.startsWith("/")) {
    alert("not-a-command");

    return;
  }

  const commandName = getCommandOnly(command);
  const commandArgs = getCommandArgs(command);

  if (!commands[commandName]) {
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

  commands[commandName].handler(commandArgs.args);
}

onMounted(() => {
  handleHelp();
});
</script>
