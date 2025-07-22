import { defineStore } from 'pinia';

export const useCommandHistoryStore = defineStore('commandHistory', {
  state: () => ({ commandHistory: [], index: 0 }),
  actions: {
    setCommandHistory(commandHistory) {
      this.commandHistory = commandHistory;
      this.index = this.commandHistory.length;
    },
    addCommand(command) {
      const length = this.commandHistory.push(command);
      this.index = length;
    },
    clearCommandHistory() {
      this.commandHistory = [];
      this.index = 0;
    },
    getCommand() {
      return this.commandHistory[this.index];
    },
    getPreviousCommand() {
      if (this.index < this.commandHistory.length - 1) {
        this.index++;
      }

      return this.commandHistory[this.index];
    },
    getNextCommand() {
      if (this.index > 0) {
        this.index--;
      }

      return this.commandHistory[this.index];
    },
  },
});