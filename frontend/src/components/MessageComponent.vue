<template>
  <div class="terminal-line">
    <div v-if="message?.command">
      <span v-if="message.command === 'help'" class="message-wrapper">
        <HelpComponent :commands="message.response" />
      </span>
      <span v-else-if="message.command === 'list'" class="message-wrapper">
        <ListComponent :chats="message.chats" />
      </span>
      <span v-else class="message-wrapper">
        <span class="terminal-message">{{ message.command }}</span>
      </span>
    </div>
    <span v-else-if="message?.type === 0" class="message-wrapper">
      <ChatIdentifier :username="getUsername(message)" :chat="message.chat" />
      <span class="terminal-message">{{ message.content }}</span>
    </span>
    <span v-else-if="message?.type">
      <span class="alert"
      :class="getAlertClass(message.type)">
        {{ message.content }}
      </span>
    </span>
  </div>
</template>

<script setup>
import { defineProps } from "vue";
import HelpComponent from "./HelpComponent.vue";
import ChatIdentifier from "./ChatIdentifier.vue";
import ListComponent from "./ListComponent.vue";

defineProps({
  message: Object,
});

const getAlertClass = (type) => {
  switch (type) {
    case 1: return "alert-error";
    case 2: return "alert-info";
    case 3: return "alert-success";
    default: return "";
  }
}

const getUsername = (message) => {
  if (message.user) {
    return message.user.username;
  }

  return `Visitante-${message.connectionId?.substring(0, 5)}`;
}
</script>

<style scoped>
.terminal-line {
  word-wrap: break-word;
  overflow-wrap: break-word;
}

.message-wrapper {
  display: inline;
}

.user-chat-info {
  white-space: nowrap;
  display: inline;
}

.terminal-message {
  white-space: pre-wrap;
  word-wrap: break-word;
  overflow-wrap: break-word;
  display: inline;
}

.alert {
  display: block;
  width: 100%;
  box-sizing: border-box;
  margin-bottom: 10px;
  margin-top: 10px;
  padding: 2px 6px;
  padding-left: 20px;
  padding-top: 20px;
  padding-bottom: 20px;
  border-radius: 1px;
  word-wrap: break-word;
  overflow-wrap: break-word;
}

.alert-error {
  background-color: #e33c3c;
  color: #000000;
}

.alert-info {
  background-color: #bdc42c;
  color: #000000;
}

.alert-success {
  background-color: #43d465;
  color: #000000;
}
</style>
