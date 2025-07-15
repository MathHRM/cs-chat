<template>
  <div class="terminal-container">
    <CommandsComponent :messages="messages" />

    <CommandLine @send-message="handleInput" :chat="'login-register'" />
  </div>
</template>

<script setup>
import CommandLine from "@/components/CommandLine.vue";
import CommandsComponent from "@/components/CommandsComponent.vue";
import { onMounted, ref } from "vue";
import handleMessage, { isCommand, alert } from "@/helpers/messageHandler";
import handleCommand from "@/helpers/commandHandler";
import Hub from "../Hub";

let messages = ref([]);
const _hub = new Hub();

function handleInput(message) {
  if (! isCommand(message)) {
    alert(messages, "You are not logged in, please log or register to start chatting", 2);
    return;
  }

  handleMessage(message, _hub.connection, messages);
}

onMounted(() => {
  _hub.connection.start().then(() => {
    _hub.connection.on("ReceivedMessage", (msg) => {
      messages.value.push(msg);
    });

    _hub.connection.on("ReceivedCommand", (command) => {
      handleCommand(command, messages);
    });

    _hub.connection.invoke("SendMessage", { content: "/help" });
  });
});
</script>
