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
      },
      password: {
        type: "string",
        description: "The password to login with",
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
      },
      password: {
        type: "string",
        description: "The password to register with",
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

function alert(content) {
  const contentMessage = translate[content];

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
  const argRegex = /--(\w+)=([^\s]+)/g;
  const args = {};
  let match;

  while ((match = argRegex.exec(command)) !== null) {
    console.log(match);

    args[match[1]] = match[2];
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
                    `--${argName}=<${argInfo.type}> - ${argInfo.description}`
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

  if (
    Object.keys(commandArgs).length !==
    Object.keys(commands[commandName].args).length
  ) {
    alert("not-enough-args");

    return;
  }

  commands[commandName].handler(commandArgs);
}

onMounted(() => {
  handleHelp();
});
</script>
