<template>
  <div class="terminal-line">
    <span v-if="message.type === 0">
      <span class="terminal-user">{{ message.user.username }}</span>
      <span class="terminal-separator"> chat:( </span>
      <span class="terminal-chat" v-if="message.chat?.id">{{ `${message.chat?.name} (${message.chat?.id})` }}</span>
      <span class="terminal-chat" v-else>{{ "chat" }}</span>
      <span class="terminal-separator"> )</span>
      <span class="terminal-message">{{ message.content }}</span>
    </span>
    <span v-else>
      <span class="alert"
      :class="getAlertClass(message.type)">
        {{ message.content }}
      </span>
    </span>
  </div>
</template>

<script setup>
import { defineProps } from "vue";

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

.terminal-message {
  white-space: pre-wrap;
}
</style>
