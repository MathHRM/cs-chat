<template>
  <q-layout view="lHh Lpr lFf">
    <q-page-container>
      <div class="terminal-wrapper">
        <router-view />
      </div>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { useAuthStore } from "@/stores/auth";
import { onMounted } from "vue";
import { getUser } from "@/api/getUser";
import { getChat } from "@/api/getChat";
import { useChatStore } from "@/stores/chat";
import router from "@/routes";

const authStore = useAuthStore();
const chatStore = useChatStore();

onMounted(async () => {
  const token = localStorage.getItem("@auth");

  if (token) {
    const userData = await getUser();

    if (userData?.user?.id) {
      authStore.setUser(userData.user);
      const currentChatData = await getChat(userData.user.currentChatId);

      chatStore.setChat(currentChatData);

      router.push("/");

      return;
    }

    localStorage.removeItem("@auth");
  }

  authStore.$reset();
  router.push("/login");
});
</script>

<style>
.username-setup {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.terminal-wrapper {
  padding: 20px;
  min-height: 100vh;
  background: #0f0f0f;
  display: flex;
  justify-content: center;
  align-items: center;
}

.terminal-wrapper .terminal-container {
  width: 100%;
  max-width: 1200px;
}

/* Override Quasar's default styles */
.q-page-container {
  padding: 0 !important;
}

.q-layout__section--marginal {
  color: #fff;
}
</style>
