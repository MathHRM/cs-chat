<template>
  <q-layout view="lHh Lpr lFf">
    <q-page-container>
      <!-- Username Setup (only show if no username is set) -->
      <div v-if="!message.username" class="username-setup">
        <q-card class="q-pa-md q-ma-md">
          <q-card-section>
            <div class="text-h6">Enter Terminal</div>
            <div class="text-subtitle2">Set your username to start chatting</div>
          </q-card-section>
          <q-card-section>
            <q-input
              outlined
              placeholder="Enter your username"
              v-model="usernameInput"
              @keyup.enter="setUsername"
              class="q-mb-md"
              dense
              autofocus
            />
            <q-btn 
              @click="setUsername"
              :disable="!usernameInput.trim()"
              color="primary"
              label="Enter Terminal"
              class="full-width"
            />
          </q-card-section>
        </q-card>
      </div>

      <!-- Terminal Chat Interface -->
      <div v-else class="terminal-wrapper">
        <ChatComponent 
          :messages="messages" 
          :userActual="message.username"
          @send-message="handleSendMessage"
        />
      </div>
    </q-page-container>
  </q-layout>
</template>

<script>
import { ref, reactive, onMounted } from 'vue'
import ChatComponent from './components/ChatComponent.vue'
import Hub from './Hub'

export default {
  name: 'LayoutDefault',

  components: {
    ChatComponent
  },

  setup () {
    let messages = ref([]);
    let message = reactive({
      username: "",
      content: "",
    });
    let usernameInput = ref("");
    let _hub = new Hub();

    function setUsername() {
      if (usernameInput.value.trim()) {
        message.username = usernameInput.value.trim();
        usernameInput.value = "";
      }
    }

    function handleSendMessage(content) {
      if (!content.trim()) return;
      
      message.content = content;
      _hub.connection.invoke("SendMessage", message);
      message.content = "";
    }

    function send() {
      if(message.content == "")
        return;

      _hub.connection.invoke("SendMessage", message);
      message.content = "";
    }

    onMounted(() => {
      _hub.connection.start()
      .then(() => {
        _hub.connection.on("ReceivedMessage", (msg) => {
          messages.value.push(msg);
        });
      })
      .catch(e => console.log("Error: Connection failed", e))
    });

    return {
      send,
      messages,
      message,
      usernameInput,
      setUsername,
      handleSendMessage
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
