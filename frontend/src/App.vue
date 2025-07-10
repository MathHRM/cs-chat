<template>
  <q-layout view="lHh Lpr lFf">
    <q-page-container>
      <!-- Username Setup (only show if no username is set) -->
      <div v-if="!getLoggedUser" class="terminal-wrapper">
        <LoginRegister />
      </div>

      <!-- Terminal Chat Interface -->
      <div v-else class="terminal-wrapper">
        <TerminalView :userActual="getLoggedUser" />
      </div>
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, computed } from "vue";
import TerminalView from "./pages/TerminalView.vue";
import LoginRegister from "./pages/LoginRegister.vue";

let loggedUser = ref("");

const getLoggedUser = computed(() => {
  return loggedUser.value.trim();
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
