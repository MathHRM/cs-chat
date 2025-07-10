<template>
  <q-layout view="lHh Lpr lFf">
    <q-page-container>
      <!-- Username Setup (only show if no username is set) -->
      <div v-if="!getLoggedUser" class="username-setup">
        <q-card class="q-pa-md q-ma-md">
          <q-card-section>
            <div class="text-h6">Enter Terminal</div>
            <div class="text-subtitle2">Set your username to start chatting</div>
          </q-card-section>
          <q-card-section>
            <q-input outlined placeholder="Enter your username" v-model="usernameInput" @keyup.enter="setUsername"
              class="q-mb-md" dense autofocus />
            <q-btn @click="setUsername" :disable="!usernameInput.trim()" color="primary" label="Enter Terminal"
              class="full-width" />
          </q-card-section>
        </q-card>
      </div>

      <!-- Terminal Chat Interface -->
      <div v-else class="terminal-wrapper">
        <TerminalView :userActual="getLoggedUser" />
      </div>
    </q-page-container>
  </q-layout>
</template>

<script>
import { ref, reactive, computed } from 'vue'
import Hub from './Hub'
import TerminalView from './pages/TerminalView.vue';

export default {
  name: 'LayoutDefault',

  components: {
    TerminalView
  },

  setup() {
    let messages = ref([]);
    let message = reactive({
      username: "",
      content: "",
    });
    let usernameInput = ref("");
    let loggedUser = ref("");
    let _hub = new Hub();

    const getLoggedUser = computed(() => {
      return loggedUser.value.trim();
    });

    function setUsername() {
      if (usernameInput.value.trim()) {
        loggedUser.value = usernameInput.value.trim();
        usernameInput.value = "";
      }
    }

    function handleSendMessage(content) {
      if (!content.trim()) return;

      message.content = content;

      if (_hub.connection.state != "Connected") {
        messages.value.push({
          username: "System",
          content: "Connection not established",
          created_at: new Date()
        });

        return;
      }

      _hub.connection.invoke("SendMessage", message);
      message.content = "";
    }

    return {
      messages,
      message,
      usernameInput,
      setUsername,
      handleSendMessage,
      getLoggedUser
    }
  }
}
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
