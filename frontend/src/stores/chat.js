import { defineStore } from 'pinia';

export const useChatStore = defineStore('chat', {
  state: () => ({ chat: {} }),
  actions: {
    setChat(chat) {
      this.chat = chat;
    },
  },
});