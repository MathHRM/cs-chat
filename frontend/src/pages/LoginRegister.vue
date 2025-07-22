<template>
  <div class="terminal-container">
    <CommandsComponent :messages="messages" />

    <CommandLine @send-message="handleInput" :chat="'guest'" />
  </div>
</template>

<script setup>
import CommandLine from "@/components/CommandLine.vue";
import CommandsComponent from "@/components/CommandsComponent.vue";
import { onMounted, computed, onUnmounted } from "vue";
import handleMessage, { alert } from "@/helpers/messageHandler";
import { sendCommand } from "@/api/sendCommand";
import handleCommand from "@/helpers/commandHandler";
import { useI18n } from "vue-i18n";
import { useMessagesStore } from "@/stores/messages";
import Hub from "@/Hub";

const { t } = useI18n();

const _hub = new Hub();

const messagesStore = useMessagesStore();
const messages = computed(() => messagesStore.messages);
const user = {
  username: "Visitante",
  currentChatId: "guest",
};
const chat = {
  id: "guest",
  name: "Chat para visitantes",
};

async function handleInput(message) {
  handleMessage(message, _hub.connection, user, chat, t);
}

onMounted(async () => {
  _hub.connection
    .start()
    .then(() => {
      _hub.connection.on("ReceivedMessage", (msg) => {
        messagesStore.addMessage(msg);
      });

      _hub.connection.on("ReceivedCommand", (command) => {
        handleCommand(command, t);
      });
    })
    .catch(() => {
      alert(t("connection.failed"), 1);
    });

  alert(t("alerts.unauthenticated"), 1);

  const command = await sendCommand("/help");

  handleCommand(command, t);
});

onUnmounted(() => {
  _hub.connection.stop();
});
</script>
