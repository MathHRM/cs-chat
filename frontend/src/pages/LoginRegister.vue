<template>
  <div class="terminal-container">
    <CommandsComponent :messages="messages" />

    <CommandLine @send-message="handleCommand" />
  </div>
</template>

<script setup>
import CommandLine from "@/components/CommandLine.vue";
import CommandsComponent from "@/components/CommandsComponent.vue";
import { ref } from "vue";

const translate = {
  'login': 'Logged in successfully',
  'register': 'Registered successfully',
  'login-failed': 'Wrong username or password',
  'register-failed': 'Username already exists',
  'command-not-found': 'Command does not exist',
  'not-a-command': 'Not a command',
  'not-enough-args': 'Not enough arguments',
};

const commands = {
  'login': {
    'description': 'Login to the system',
    'args': {
      'username': {
        'type': 'string',
        'description': 'The username to login with',
      },
      'password': {
        'type': 'string',
        'description': 'The password to login with',
      },
    },
    'handler': login
  },
  'register': {
    'description': 'Register a new user',
    'args': {
      'username': {
        'type': 'string',
        'description': 'The username to register with',
      },
      'password': {
        'type': 'string',
        'description': 'The password to register with',
      },
    },
    'handler': register
  },
};

let messages = ref([]);

function saveMessage(content) {
  messages.value.push({
    username: '~',
    content: content,
    created_at: new Date(),
  });
}

function alert(content) {
  const contentMessage = translate[content];

  messages.value.push({
    username: 'System',
    content: contentMessage,
    created_at: new Date(),
  });

  return contentMessage;
}

function getCommandOnly(command) {
  const match = command.match(/^\/\s*(\w+)/);

  if (!match) {
    console.log('no match');

    return null;
  }

  console.log('match', match[1]);

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

function login({ username, password }) {
  console.log(username, password);
  alert('login');
}

function register({ username, password }) {
  console.log(username, password);
  alert('register');
}

function handleCommand(command) {
  saveMessage(command);

  command = command.trim();

  if (!command.startsWith('/')) {
    alert('not-a-command');

    return;
  }

  const commandName = getCommandOnly(command);
  const commandArgs = getCommandArgs(command);

  if (!commands[commandName]) {
    alert('command-not-found');

    return;
  }

  if (Object.keys(commandArgs).length !== Object.keys(commands[commandName].args).length) {
    alert('not-enough-args');

    return;
  }

  commands[commandName].handler(commandArgs);
}

</script>
