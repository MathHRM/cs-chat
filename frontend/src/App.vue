<template>
  <q-layout view="lHh Lpr lFf">
    <q-page-container>
      <q-input
        outlined
        placeholder="Insira seu Username"
        v-model="message.username"
        class="q-mt-sm q-pa-sm"
        dense
      />

      <ChatComponent :messages="messages" :userActual="message.username"/>

      <q-input
        outlined
        @keyup.enter="send"
        placeholder="Digite sua mensagem"
        v-model="message.content"
        class="q-mt-xl q-pa-sm"
        dense
      />
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
    let _hub = new Hub();

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
      message
    }
  }
}
</script>
