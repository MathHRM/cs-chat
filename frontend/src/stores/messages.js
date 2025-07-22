import { defineStore } from 'pinia';

export const useMessagesStore = defineStore('messages', {
  state: () => ({ messages: [] }),
  actions: {
    setMessages(messages) {
      this.messages = messages;
    },
    addMessage(message) {
      this.messages.push(message);
    },
    prependMessages(messages) {
      this.messages.unshift(...messages);
    },
    clearMessages() {
      this.messages = [];
    },
  },
});