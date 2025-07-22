<template>
  <div class="terminal-container">
    <CommandsComponent
      :messages="messages"
      @load-more-messages="handleLoadMoreMessages"
    />

    <CommandLine @send-message="handleSendMessage" />
  </div>
</template>

<script setup>
import CommandsComponent from "@/components/CommandsComponent.vue";
import CommandLine from "@/components/CommandLine.vue";
import { onMounted, computed } from "vue";
import Hub from "../Hub";
import { useAuthStore } from "@/stores/auth";
import handleCommand from "@/helpers/commandHandler";
import handleMessage from "@/helpers/messageHandler";
import { useChatStore } from "@/stores/chat";
import { useI18n } from "vue-i18n";
import { useMessagesStore } from "@/stores/messages";
import { getMessages } from "@/api/getMessages";

const { t } = useI18n();

const _hub = new Hub();
const messagesStore = useMessagesStore();
const messages = computed(() => messagesStore.messages);

const authStore = useAuthStore();
const chatStore = useChatStore();
const user = computed(() => authStore.user);
const chat = computed(() => chatStore.chat);

function handleSendMessage(content) {
  handleMessage(content, _hub.connection, user.value, chat.value, t);
}

function handleLoadMoreMessages() {
  const firstMessageId = messages.value[0]?.id;

  if (!firstMessageId) {
    return;
  }

  getMessages(firstMessageId).then((newMessages) => {
    messagesStore.prependMessages(newMessages);
  });
}

onMounted(() => {
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
      alert(t("alerts.connection-failed"), 1);
    });

  getMessages().then((messages) => {
    messagesStore.setMessages(messages);
  });
});
</script>
