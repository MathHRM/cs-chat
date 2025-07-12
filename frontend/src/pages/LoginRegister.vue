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
import { help, login, register } from "@/commands/commands";
import { handleHelp, handleMessage } from "@/helpers/commandsHelper";

let messages = ref([]);

const pageCommands = {
  help,
  login,
  register,
};

function handleInput(message) {
  handleMessage(messages, message, "login-register", pageCommands, null, null);
}

onMounted(() => {
  handleHelp({ messages, pageCommands });
});
</script>
