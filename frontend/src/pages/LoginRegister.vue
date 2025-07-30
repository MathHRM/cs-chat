<template>
  <div class="terminal-container">
    <CommandsComponent :messages="messages" @load-more-messages="handleLoadMoreMessages" />

    <CommandInput @send-message="handleInput" :chat="'guest'" :connection-id="connectionId" />
  </div>
</template>

<script setup>
import CommandInput from "@/components/CommandInput.vue";
import CommandsComponent from "@/components/CommandsComponent.vue";
import { onMounted, computed, onUnmounted, ref } from "vue";
import handleMessage, { alert } from "@/helpers/messageHandler";
import { sendCommand } from "@/api/sendCommand";
import handleCommand from "@/helpers/commandHandler";
import { useI18n } from "vue-i18n";
import { useMessagesStore } from "@/stores/messages";
import Hub from "@/Hub";
import { getGuestMessages } from "@/api/guestMessages";

const { t } = useI18n();

const _hub = new Hub();

const connectionId = ref(null);

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

function handleLoadMoreMessages() {
  const firstMessageId = messages.value[0]?.id;

  if (!firstMessageId) {
    return;
  }

  getGuestMessages(firstMessageId).then((newMessages) => {
    messagesStore.prependMessages(newMessages);
  });
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

      connectionId.value = _hub.connection.connectionId;
    })
    .catch(() => {
      alert(t("connection.failed"), 1);
    });

  getGuestMessages().then((messages) => {
    messagesStore.setMessages(messages);

    alert(t("alerts.unauthenticated"), 1);

    sendCommand("/help").then((command) => {
      handleCommand(command, t);
    });
  });
});

onUnmounted(() => {
  _hub.connection.stop();
});
</script>
