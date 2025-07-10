<template>
  <!-- Terminal Body -->
  <div class="terminal-body" ref="terminalBody">
    <!-- Chat Messages -->
    <div
      v-for="(message, index) in messages"
      :key="index"
      class="terminal-line"
    >
      <span class="terminal-user">{{ message.username }}</span>
      <span class="terminal-separator">@</span>
      <span class="terminal-chat">chat</span>
      <span class="terminal-separator">:~$ </span>
      <span class="terminal-message">{{ message.content }}</span>
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
  userActual: String,
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
