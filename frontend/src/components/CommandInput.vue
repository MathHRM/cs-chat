<!--
  CommandInput Component

  Features:
  - Shows a modal with available commands when user types "/"
  - Allows navigation with arrow keys (up/down)
  - Tab key to select the highlighted command
  - Escape key to close the modal
  - Ctrl+Space to toggle the modal
  - Real-time filtering as user types
  - Shows command syntax hints
  - Displays command arguments and options
  - Supports both guest and authenticated users
-->
<template>
  <div class="command-input-container">
    <!-- Command Modal -->
    <CommandModal
      :show="showModal"
      :commands="commands"
      :selectedIndex="selectedIndex"
      :currentInput="currentInput"
      :isLoading="isLoadingCommands"
      @select-command="selectCommand"
    />

    <!-- Input Line -->
    <div class="terminal-input-line">
      <div class="input-wrapper">
        <span class="terminal-prompt">
          <span class="terminal-start">&rarr;</span>
          <span class="user-chat-info">
            <span class="terminal-user">{{ getUser?.username || getGuestUsername || "Usuário" }}</span>
            <span class="terminal-separator"> chat:( </span>
                      <span class="terminal-chat" v-if="getChat?.id">{{ `${getChat.name} (${getChat.id})` }}</span>
          <span class="terminal-chat" v-else-if="chat">{{ chat }}</span>
          <span class="terminal-chat" v-else>chat</span>
            <span class="terminal-separator"> )</span>
          </span>
          <span class="terminal-end"> x </span>
        </span>
        <div class="input-section">
          <input
            ref="commandInput"
            v-model="currentInput"
            @keyup.enter="handleSend"
            @keyup.up="handleUp"
            @keyup.down="handleDown"
            @input="handleInput"
            @keyup.escape="handleEscape"
            @keyup.tab="handleTab"
            @keydown.ctrl.space="handleCtrlSpace"
            class="terminal-input"
            autofocus
          />
          <span class="terminal-cursor" :class="{ blink: showCursor }">█</span>
        </div>
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
import { sendCommand } from "@/api/sendCommand";
import CommandModal from "./CommandModal.vue";

const props = defineProps({
  chat: {
    type: String,
    default: null,
  },
  connectionId: {
    type: String,
    default: null,
  },
});

const authStore = useAuthStore();
const chatStore = useChatStore();

const getUser = computed(() => authStore.user);
const getChat = computed(() => chatStore.chat);

const emit = defineEmits(["send-message"]);

const currentInput = ref("");
const showCursor = ref(true);
const showModal = ref(false);
const commands = ref([]);
const selectedIndex = ref(0);
const commandInput = ref(null);
const isLoadingCommands = ref(false);

const getGuestUsername = computed(() => {
  if (!props.connectionId) return null;
  return `Visitante-${props.connectionId.substring(0, 5)}`;
});

// Filter commands based on current input
const filteredCommands = computed(() => {
  if (!currentInput.value.startsWith("/")) return [];

  const searchTerm = currentInput.value.substring(1).toLowerCase();
  if (!searchTerm) return commands.value;

  return commands.value.filter(command =>
    command.name.toLowerCase().includes(searchTerm)
  );
});

const focusInput = () => {
  if (commandInput.value) {
    commandInput.value.focus();
  }
};

const handleWindowClick = () => {
  focusInput();
};

const handleSend = () => {
  console.log("handleSend", showModal.value, commands.value.length, selectedIndex.value);

  // If modal is open and a command is selected, insert the command instead of sending
  if (showModal.value && filteredCommands.value.length > 0 && selectedIndex.value < filteredCommands.value.length) {
    selectCommand(filteredCommands.value[selectedIndex.value]);
    return;
  }

  const message = currentInput.value.trim();

  if (message) {
    emit("send-message", message);
    currentInput.value = "";
    showModal.value = false;
  }

  if (isCommand(message)) {
    saveCommandHistory(message);
  }

  focusInput();
};

const handleUp = (event) => {
  if (showModal.value && commands.value.length > 0) {
    event.preventDefault();
    selectedIndex.value = selectedIndex.value > 0
      ? selectedIndex.value - 1
      : commands.value.length - 1;
  } else {
    currentInput.value = nextCommand();
  }
};

const handleDown = (event) => {
  if (showModal.value && commands.value.length > 0) {
    event.preventDefault();
    selectedIndex.value = selectedIndex.value < commands.value.length - 1
      ? selectedIndex.value + 1
      : 0;
  } else {
    currentInput.value = previousCommand();
  }
};

const handleEscape = () => {
  showModal.value = false;
  focusInput();
};

const handleTab = (event) => {
  if (showModal.value && filteredCommands.value.length > 0) {
    event.preventDefault();
    selectCommand(filteredCommands.value[selectedIndex.value]);
  }
};

const handleCtrlSpace = (event) => {
  event.preventDefault();
  if (currentInput.value.startsWith("/")) {
    showModal.value = !showModal.value;
  } else {
    currentInput.value = "/";
    showModal.value = true;
  }
  focusInput();
};

const handleInput = () => {
  showCursor.value = true;

  // Show modal when user starts typing "/"
  if (currentInput.value.startsWith("/")) {
    showModal.value = true;
    selectedIndex.value = 0;
  } else {
    showModal.value = false;
  }
};

const selectCommand = (command) => {
  // If user has already typed something after "/", preserve it
  const currentText = currentInput.value;
  const slashIndex = currentText.indexOf("/");

  // Build the command with required arguments and options
  let commandTemplate = `/${command.name}`;

  // Add required arguments
  if (command.arguments && command.arguments.length > 0) {
    commandTemplate += ` ${command.arguments.map(arg => `<${arg.name}>`).join(' ')}`;
  }

  // Add required options
  if (command.options && command.options.length > 0) {
    const requiredOptions = command.options.filter(opt => opt.isRequired);
    if (requiredOptions.length > 0) {
      commandTemplate += ` ${requiredOptions.map(opt => `${opt.name}`).join(' ')}`;
    }
  }

  // Add a space at the end for easy editing
  commandTemplate += ' ';

  if (slashIndex !== -1) {
    // Replace everything from "/" onwards with the selected command
    currentInput.value = currentText.substring(0, slashIndex) + commandTemplate;
  } else {
    currentInput.value = commandTemplate;
  }

  showModal.value = false;
  focusInput();
};

const loadCommands = async () => {
  try {
    isLoadingCommands.value = true;
    const response = await sendCommand("/help");
    if (response && response.response) {
      commands.value = response.response;
    }
  } catch (error) {
    console.error("Failed to load commands:", error);
  } finally {
    isLoadingCommands.value = false;
  }
};

const startCursorBlink = () => {
  setInterval(() => {
    showCursor.value = !showCursor.value;
  }, 500);
};

onMounted(() => {
  startCursorBlink();
  focusInput();
  loadCommands();

  // Add window click event listener to focus input
  window.addEventListener('click', handleWindowClick);
});

onUnmounted(() => {
  // Clean up event listener when component is destroyed
  window.removeEventListener('click', handleWindowClick);
});
</script>

<style scoped>
.command-input-container {
  position: relative;
}

.command-modal {
  position: absolute;
  bottom: 100%;
  left: 0;
  right: 0;
  background: #1a1a1a;
  border: 1px solid #333;
  border-radius: 4px;
  max-height: 300px;
  overflow-y: auto;
  z-index: 1000;
  margin-bottom: 5px;
}

.command-list {
  padding: 8px 0;
}

.command-item {
  padding: 8px 12px;
  cursor: pointer;
  border-bottom: 1px solid #333;
  transition: background-color 0.2s;
}

.command-item:hover,
.command-item.selected {
  background-color: #2a2a2a;
}

.command-item:last-child {
  border-bottom: none;
}

.command-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 4px;
}

.command-name {
  color: #4CAF50;
  font-weight: bold;
  font-family: 'Courier New', monospace;
}

.command-description {
  color: #ccc;
  font-size: 0.9em;
}

.command-arguments,
.command-options {
  margin-left: 20px;
  margin-top: 4px;
}

.argument,
.option {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 2px;
  font-size: 0.8em;
}

.argument-name,
.option-name {
  color: #FF9800;
  font-family: 'Courier New', monospace;
  min-width: 80px;
}

.argument-description,
.option-description {
  color: #999;
}

.no-commands,
.loading-commands {
  padding: 12px;
  color: #999;
  text-align: center;
  font-style: italic;
}

.loading-commands {
  color: #4CAF50;
}

.command-syntax-hint {
  background: #2a2a2a;
  border-bottom: 1px solid #333;
  padding: 8px 12px;
  font-family: 'Courier New', monospace;
  font-size: 0.9em;
}

.syntax-label {
  color: #4CAF50;
  font-weight: bold;
  margin-right: 8px;
}

.syntax-text {
  color: #ccc;
}

.input-wrapper {
  display: flex;
  align-items: center;
  width: 100%;
}

.terminal-prompt {
  display: flex;
  align-items: center;
  margin-right: 8px;
  white-space: nowrap;
}

.terminal-start {
  color: #4CAF50;
  margin-right: 4px;
}

.terminal-end {
  color: #4CAF50;
  margin-left: 4px;
}

.input-section {
  display: flex;
  align-items: center;
  flex: 1;
  position: relative;
}

.terminal-input {
  background: transparent;
  border: none;
  color: #fff;
  font-family: 'Courier New', monospace;
  font-size: 14px;
  outline: none;
  flex: 1;
  padding-right: 20px;
}

.terminal-cursor {
  position: absolute;
  right: 0;
  color: #4CAF50;
  font-weight: bold;
}

.terminal-cursor.blink {
  animation: blink 1s infinite;
}

@keyframes blink {
  0%, 50% { opacity: 1; }
  51%, 100% { opacity: 0; }
}
</style>