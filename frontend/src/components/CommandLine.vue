<template>
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
      autofocus
    />
    <span class="terminal-cursor" :class="{ blink: showCursor }">█</span>
  </div>
</template>

<script setup>
import {
  ref,
  onMounted,
  defineProps,
  defineEmits,
} from "vue";

const emit = defineEmits(["send-message"]);
defineProps({
  userActual: String,
});

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

onMounted(() => {
  startCursorBlink();
});
</script>
