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
import { useI18n } from "vue-i18n";

const { t } = useI18n();

let messages = ref([]);

async function handleInput(message) {
  if (!isCommand(message)) {
    messages.value.push({
      content: message,
      type: 0,
      user: {
        username: "~",
      },
    });

    alert(messages, t("alerts.not-logged-in"), 2);

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

  handleCommand(command, messages, t);
}
</script>
