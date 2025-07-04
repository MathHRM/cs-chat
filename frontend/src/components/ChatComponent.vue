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
      <div v-for="(message, index) in messages" :key="index" class="terminal-line">
        <span class="terminal-user">{{ message.username }}</span>
        <span class="terminal-separator">@</span>
        <span class="terminal-chat">chat</span>
        <span class="terminal-separator">:~$ </span>
        <span class="terminal-message">{{ message.content }}</span>
        <div class="terminal-timestamp">{{ formatTimestamp(message.created_at) }}</div>
      </div>
      
      <!-- Current Input Line -->
      <div class="terminal-input-line">
        <span class="terminal-user">{{ userActual || 'user' }}</span>
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
        <span class="terminal-cursor" :class="{ 'blink': showCursor }">█</span>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ChatComponent',
  props: {
    userActual: String,
    messages: Array
  },
  data() {
    return {
      currentInput: '',
      showCursor: true
    }
  },
  mounted() {
    this.startCursorBlink();
    this.scrollToBottom();
  },
  updated() {
    this.scrollToBottom();
  },
  methods: {
    handleSend() {
      if (this.currentInput.trim()) {
        this.$emit('send-message', this.currentInput.trim());
        this.currentInput = '';
      }
    },
    handleInput() {
      // Reset cursor blink on input
      this.showCursor = true;
    },
    startCursorBlink() {
      setInterval(() => {
        this.showCursor = !this.showCursor;
      }, 500);
    },
    scrollToBottom() {
      this.$nextTick(() => {
        if (this.$refs.terminalBody) {
          this.$refs.terminalBody.scrollTop = this.$refs.terminalBody.scrollHeight;
        }
      });
    },
    formatTimestamp(timestamp) {
      if (!timestamp) return '';
      const date = new Date(timestamp);
      return date.toLocaleTimeString();
    }
  }
}
</script>

<style scoped>
.terminal-container {
  font-family: 'Courier New', 'Monaco', 'Menlo', monospace;
  background-color: #1e1e1e;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
  height: 80vh;
  max-height: 600px;
  display: flex;
  flex-direction: column;
}

.terminal-header {
  background: linear-gradient(to bottom, #404040, #2a2a2a);
  padding: 8px 12px;
  display: flex;
  align-items: center;
  border-bottom: 1px solid #333;
}

.terminal-buttons {
  display: flex;
  gap: 8px;
  margin-right: 12px;
}

.terminal-btn {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  cursor: pointer;
}

.terminal-btn.close {
  background-color: #ff5f57;
}

.terminal-btn.minimize {
  background-color: #ffbd2e;
}

.terminal-btn.maximize {
  background-color: #28ca42;
}

.terminal-title {
  color: #ddd;
  font-size: 12px;
  font-weight: 500;
}

.terminal-body {
  flex: 1;
  background-color: #1e1e1e;
  padding: 12px;
  overflow-y: auto;
  color: #00ff00;
  font-size: 14px;
  line-height: 1.4;
}

.terminal-line {
  margin-bottom: 8px;
  position: relative;
}

.terminal-user {
  color: #00ff00;
  font-weight: bold;
}

.terminal-separator {
  color: #ffffff;
}

.terminal-chat {
  color: #ff0000;
  font-weight: bold;
}

.terminal-message {
  color: #ffffff;
  margin-left: 4px;
}

.terminal-timestamp {
  color: #888;
  font-size: 11px;
  margin-top: 2px;
  margin-left: 20px;
}

.terminal-input-line {
  display: flex;
  align-items: center;
  position: relative;
}

.terminal-input {
  background: transparent;
  border: none;
  outline: none;
  color: #ffffff;
  font-family: inherit;
  font-size: inherit;
  flex: 1;
  margin-left: 4px;
}

.terminal-input::placeholder {
  color: #666;
}

.terminal-cursor {
  color: #00ff00;
  font-weight: bold;
  margin-left: 2px;
}

.terminal-cursor.blink {
  animation: blink 1s infinite;
}

@keyframes blink {
  0%, 50% { opacity: 1; }
  51%, 100% { opacity: 0; }
}

/* Custom scrollbar */
.terminal-body::-webkit-scrollbar {
  width: 8px;
}

.terminal-body::-webkit-scrollbar-track {
  background: #2a2a2a;
}

.terminal-body::-webkit-scrollbar-thumb {
  background: #555;
  border-radius: 4px;
}

.terminal-body::-webkit-scrollbar-thumb:hover {
  background: #666;
}

/* Focus styles */
.terminal-input:focus {
  outline: none;
}

/* Responsive design */
@media (max-width: 600px) {
  .terminal-container {
    height: 70vh;
  }
  
  .terminal-body {
    font-size: 12px;
  }
}
</style>
