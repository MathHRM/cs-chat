<template>
  <q-layout view="lHh Lpr lFf">
    <q-page-container>
      <!-- <div v-if="!getLoggedUser" class="terminal-wrapper">
        <LoginRegister />
      </div>

      <div v-else class="terminal-wrapper">
        <TerminalView :userActual="getLoggedUser" />
      </div> -->
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
import router from "@/routes";

const authStore = useAuthStore();

onMounted(async () => {
  const token = localStorage.getItem('@auth');

  if (token) {
    const user = await getUser();

    if (user?.id) {
      authStore.setUser(user);

      router.push('/');

      return;
    }

    localStorage.removeItem('@auth');

    router.push('/login');
  }
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
