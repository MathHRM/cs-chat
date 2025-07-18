<template>
  <div class="terminal-container">
    <CommandsComponent :messages="messages" />

    <CommandLine @send-message="handleInput" :chat="'login-register'" />
  </div>
</template>

<script setup>
import CommandLine from "@/components/CommandLine.vue";
import CommandsComponent from "@/components/CommandsComponent.vue";
import { onMounted, computed } from "vue";
import { isCommand, alert } from "@/helpers/messageHandler";
import { sendCommand } from "@/api/sendCommand";
import handleCommand from "@/helpers/commandHandler";
import { useI18n } from "vue-i18n";
import { useMessagesStore } from "@/stores/messages";

const { t } = useI18n();

const messagesStore = useMessagesStore();
const messages = computed(() => messagesStore.messages);

async function handleInput(message) {
  if (!isCommand(message)) {
    messagesStore.addMessage({
      content: message,
      type: 0,
      user: {
        username: "~",
      },
    });

    alert(t("alerts.not-logged-in"), 2);

    return;
  }

  messagesStore.addMessage({
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

  handleCommand(command, t);
}

onMounted(async () => {
  const command = await sendCommand("/help");

  handleCommand(command, t);
});
</script>
