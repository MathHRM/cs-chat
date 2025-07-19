<template>
  <div class="terminal-line">
    <span v-if="message.type === 1">
      <span class="terminal-message alert message-error">
        {{ message.content }}
      </span>
    </span>
    <span v-else-if="message.type === 2">
      <span class="terminal-message alert message-info">
        {{ message.content }}
      </span>
    </span>
    <span v-else-if="message.type === 3">
      <span class="terminal-message alert message-success">
        {{ message.content }}
      </span>
    </span>
    <span v-else>
      <span class="terminal-user">{{ message.user.username }}</span>
      <span class="terminal-separator"> chat:( </span>
      <span class="terminal-chat">{{ `${message.chat?.name}-${message.chat?.id}` || "chat" }}</span>
      <span class="terminal-separator"> )</span>
      <span class="terminal-message">{{ message.content }}</span>
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
}

.message-error {
  background-color: #e33c3c;
  color: #000000;
}

.message-info {
  background-color: #bdc42c;
  color: #000000;
}

.message-success {
  background-color: #43d465;
  color: #000000;
}

.terminal-message {
  white-space: pre-wrap;
}
</style>
