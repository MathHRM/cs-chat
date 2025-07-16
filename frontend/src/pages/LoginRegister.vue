<template>
  <div class="terminal-container">
    <CommandsComponent :messages="messages" />

    <CommandLine @send-message="handleInput" :chat="'login-register'" />
  </div>
</template>

<script setup>
import CommandLine from "@/components/CommandLine.vue";
import CommandsComponent from "@/components/CommandsComponent.vue";
import { ref } from "vue";
import { isCommand, alert } from "@/helpers/messageHandler";
import { sendCommand } from "@/api/sendCommand";
import handleCommand from "@/helpers/commandHandler";

let messages = ref([]);

async function handleInput(message) {
  if (! isCommand(message)) {
    alert(messages, "You are not logged in, please log or register to start chatting", 2);
    return;
  }

  messages.value.push({
    content: message,
    type: 0,
    user: {
      username: "~",
    },
    chat: {
      id: "login-register",
    },
  });

  const command = await sendCommand(message);

  handleCommand(command, messages);
}
</script>
