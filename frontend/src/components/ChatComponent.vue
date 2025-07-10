<template>
  <div class="terminal-container">
    <!-- Terminal Header -->
    <div class="terminal-header">
      <div class="terminal-buttons">
        <div class="terminal-btn close"></div>
        <div class="terminal-btn minimize"></div>
        <div class="terminal-btn maximize"></div>
      </div>
      <div class="terminal-title">Terminal</div>
    </div>

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

      <!-- Current Input Line -->
      <div class="terminal-input-line">
        <span class="terminal-user">{{ userActual || "user" }}</span>
        <span class="terminal-separator">@</span>
        <span class="terminal-chat">chat</span>
        <span class="terminal-separator">:~$ </span>
        <input
          ref="terminalInput"
          v-model="currentInput"
          @keyup.enter="handleSend"
          @input="handleInput"
          class="terminal-input"
          :placeholder="messages.length === 0 ? 'Type your message...' : ''"
          autofocus
        />
        <span class="terminal-cursor" :class="{ blink: showCursor }">█</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUpdated, defineProps, defineEmits, nextTick, useTemplateRef } from "vue";

const emit = defineEmits(["send-message"]);
defineProps({
  userActual: String,
  messages: Array,
});

const terminalBody = useTemplateRef("terminalBody");
const currentInput = ref("");
const showCursor = ref(true);

const handleSend = () => {
  if (currentInput.value.trim()) {
    emit("send-message", currentInput.value.trim());
    currentInput.value = "";
  }
};

const handleInput = () => {
  showCursor.value = true;
};

const startCursorBlink = () => {
  setInterval(() => {
    showCursor.value = !showCursor.value;
  }, 500);
};

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
  startCursorBlink();
  scrollToBottom();
});

onUpdated(() => {
  scrollToBottom();
});
</script>
