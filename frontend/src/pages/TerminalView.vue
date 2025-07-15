<template>
  <div class="terminal-container">
    <CommandsComponent :messages="messages" />

    <CommandLine @send-message="handleSendMessage" />
  </div>
</template>

<script setup>
import CommandsComponent from "@/components/CommandsComponent.vue";
import CommandLine from "@/components/CommandLine.vue";
import { ref, onMounted, computed } from "vue";
import Hub from "../Hub";
import { HubConnectionState } from "@aspnet/signalr";
import { useAuthStore } from "@/stores/auth";
import proxy from "@/helpers/handleCommand";
// import handleMessage from "@/helpers/commandsHelper";
// import commands from "@/commands/commands";
// import { useI18n } from "vue-i18n";

const _hub = new Hub();
let messages = ref([]);

const authStore = useAuthStore();
// const { t } = useI18n();

const user = computed(() => authStore.user);

// const { help, join, logout } = commands();
// const pageCommands = {
//   help,
//   join,
//   logout,
// };

function handleSendMessage(content) {
  if (_hub.connection.state != HubConnectionState.Connected) {
    messages.value.push({
      user: {
        username: "System",
      },
      chat: {
        id: user.value.currentChatId,
      },
      message: {
        content: "Connection not established",
        created_at: new Date(),
      },
    });

    return;
  }

  // handleMessage(messages, content, user.value.currentChatId, pageCommands, user.value, _hub.connection, t);
  _hub.connection.invoke("SendMessage", {content});
}

onMounted(() => {
  _hub.connection
    .start()
    .then(() => {
      _hub.connection.on("ReceivedMessage", (msg) => {
        // messages.value.push(msg);
        console.log(msg);
      });

      _hub.connection.on("ReceivedCommand", (command) => {
        console.log(command);
        proxy(messages, command);
      });

      _hub.connection.invoke("JoinChat", user.value.currentChatId || "general");
    })
    .catch((e) => console.log("Error: Connection failed", e));
});
</script>
