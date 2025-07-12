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
import { handleMessage } from "@/helpers/commandsHelper";
import { help, joinChat, logout } from "@/commands/commands";

const _hub = new Hub();
let messages = ref([]);

const authStore = useAuthStore();


const user = computed(() => authStore.user);

const pageCommands = {
  help,
  joinChat,
  logout,
};

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

  handleMessage(messages, content, user.value.currentChatId, pageCommands, user.value, _hub.connection);
}

onMounted(() => {
  _hub.connection
    .start()
    .then(() => {
      _hub.connection.on("ReceivedMessage", (msg) => {
        messages.value.push(msg);
      });

      _hub.connection.invoke("JoinChat", user.value.currentChatId || "general");
    })
    .catch((e) => console.log("Error: Connection failed", e));
});
</script>
