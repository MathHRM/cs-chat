import { defineStore } from 'pinia';

export const useAuthStore = defineStore('auth', {
  state: () => ({ user: {} }),
  actions: {
    setUser(user) {
      this.user = user;
    },
  },
});