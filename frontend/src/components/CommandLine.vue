<template>
  <div class="terminal-input-line">
    <div class="input-wrapper">
      <span class="terminal-prompt">
        <span class="terminal-start">&rarr;</span>
        <span class="user-chat-info">
          <span class="terminal-user">{{ getUser?.username || getGuestUsername || "Visitante" }}</span>
          <span class="terminal-separator"> chat:( </span>
          <span class="terminal-chat" v-if="getChat?.id">{{ `${getChat.name} (${getChat.id})` }}</span>
          <span class="terminal-chat" v-else>{{ chat || "chat" }}</span>
          <span class="terminal-separator"> )</span>
        </span>
        <span class="terminal-end"> x </span>
      </span>
      <div class="input-section">
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
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, defineEmits, computed, defineProps } from "vue";
import { useAuthStore } from "@/stores/auth";
import { useChatStore } from "@/stores/chat";
import { saveCommandHistory, nextCommand, previousCommand } from "@/helpers/commandHistoryHelper";
import { isCommand } from "@/helpers/messageHandler";

const props = defineProps({
  chat: {
    type: String,
  },
  connectionId: {
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

const getGuestUsername = computed(() => {
  return `Visitante-${props.connectionId?.substring(0, 5)}`;
});

const focusInput = () => {
  if (terminalInput.value) {
    terminalInput.value.focus();
  }
};

const handleWindowClick = () => {
  focusInput();
};

const handleSend = () => {
  const message = currentInput.value.trim();

  if (message) {
    emit("send-message", message);
    currentInput.value = "";
  }

  if (isCommand(message)) {
    saveCommandHistory(message);
  }

  focusInput();
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
  focusInput();

  // Add window click event listener to focus input
  window.addEventListener('click', handleWindowClick);
});

onUnmounted(() => {
  // Clean up event listener when component is destroyed
  window.removeEventListener('click', handleWindowClick);
});
</script>
