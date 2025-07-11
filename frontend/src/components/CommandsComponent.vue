<template>
  <div class="terminal-body" ref="terminalBody">
    <div
      v-for="(message, index) in messages"
      :key="index"
      class="terminal-line"
    >
      <span class="terminal-user">{{ message.user.username }}</span>
      <span class="terminal-separator">@</span>
      <span class="terminal-chat">chat</span>
      <span class="terminal-separator">:~$ </span>
      <span class="terminal-message">{{ message.message.content }}</span>
      <div class="terminal-timestamp">
        {{ formatTimestamp(message.created_at) }}
      </div>
    </div>
  </div>
</template>

<script setup>
import {
  onMounted,
  onUpdated,
  defineProps,
  nextTick,
  useTemplateRef,
} from "vue";

defineProps({
  messages: Array,
});

const terminalBody = useTemplateRef("terminalBody");

const scrollToBottom = () => {
  nextTick(() => {
    if (terminalBody.value) {
      terminalBody.value.scrollTop = terminalBody.value.scrollHeight;
    }
  });
};

const formatTimestamp = (timestamp) => {
  if (!timestamp) return "";
  const date = new Date(timestamp);
  return date.toLocaleTimeString();
};

onMounted(() => {
  scrollToBottom();
});

onUpdated(() => {
  scrollToBottom();
});
</script>

<style scoped>
.terminal-message {
  white-space: pre-wrap;
}
</style>
