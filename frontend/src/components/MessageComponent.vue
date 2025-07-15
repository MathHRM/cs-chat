<template>
  <div class="terminal-line">
    <span v-if="message.responseType === 'Error'">
      <span class="terminal-message-error">
        {{ message.message.content }}
      </span>
    </span>
    <span v-else-if="message.responseType === 'Info'">
      <span class="terminal-message-info">
        {{ message.message.content }}
      </span>
    </span>
    <span v-else-if="message.responseType === 'Success'">
      <span class="terminal-message-success">
        {{ message.message.content }}
      </span>
    </span>
    <span v-else>
      <span class="terminal-user">{{ message.user.username }}</span>
      <span class="terminal-separator">@</span>
      <span class="terminal-chat">{{ message.chat?.id ?? "chat" }}</span>
      <span class="terminal-separator">:~$ </span>
      <span class="terminal-message">{{ message.message.content }}</span>
    </span>
  </div>
</template>

<script setup>
import { defineProps } from "vue";

defineProps({
  message: Object,
});
</script>

<style scoped>
.terminal-message-error {
  background-color: #e33c3c;
  color: #000000;
  padding: 2px 6px;
  border-radius: 3px;
}

.terminal-message-info {
  background-color: #bdc42c;
  color: #000000;
  padding: 2px 6px;
  border-radius: 3px;
}

.terminal-message-success {
  background-color: #43d465;
  color: #000000;
  padding: 2px 6px;
  border-radius: 3px;
}

.terminal-message {
  white-space: pre-wrap;
}
</style>
