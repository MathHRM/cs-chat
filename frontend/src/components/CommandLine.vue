<template>
  <div class="terminal-input-line">
    <span class="terminal-start">&rarr;</span>
    <span class="terminal-user">{{ getUser.username || "~" }}</span>
    <span class="terminal-separator"> chat:( </span>
    <span class="terminal-chat">{{ getChat.id || "chat" }}</span>
    <span class="terminal-separator"> )</span>
    <span class="terminal-end"> x </span>
    <input
      ref="terminalInput"
      v-model="currentInput"
      @keyup.enter="handleSend"
      @keyup.up="handleUp"
      @keyup.down="handleDown"
      @input="handleInput"
      class="terminal-input"
      autofocus
    />
    <span class="terminal-cursor" :class="{ blink: showCursor }">█</span>
  </div>
</template>

<script setup>
import { ref, onMounted, defineEmits, computed, defineProps } from "vue";
import { useAuthStore } from "@/stores/auth";
import { useChatStore } from "@/stores/chat";
import { saveCommandHistory, nextCommand, previousCommand } from "@/helpers/commandHistoryHelper";
import { isCommand } from "@/helpers/messageHandler";

defineProps({
  chat: {
    type: String,
  },
});

const authStore = useAuthStore();
const chatStore = useChatStore();

const getUser = computed(() => authStore.user);
const getChat = computed(() => chatStore.chat);

const emit = defineEmits(["send-message"]);

const currentInput = ref("");
const showCursor = ref(true);

const terminalInput = ref(null);

const handleSend = () => {
  const message = currentInput.value.trim();

  if (message) {
    emit("send-message", message);
    currentInput.value = "";
  }

  if (isCommand(message)) {
    saveCommandHistory(message);
  }

  if (terminalInput.value) {
    terminalInput.value.focus();
  }
};

const handleUp = () => {
  currentInput.value = nextCommand();
};

const handleDown = () => {
  currentInput.value = previousCommand();
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

  if (terminalInput.value) {
    terminalInput.value.focus();
  }
});
</script>
