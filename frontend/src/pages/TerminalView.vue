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
import handleCommand from "@/helpers/commandHandler";

const _hub = new Hub();
let messages = ref([]);

const authStore = useAuthStore();
const user = computed(() => authStore.user);

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

  _hub.connection.invoke("SendMessage", {content});
}

onMounted(() => {
  _hub.connection
    .start()
    .then(() => {
      _hub.connection.on("ReceivedMessage", (msg) => {
        messages.value.push(msg);
      });

      _hub.connection.on("ReceivedCommand", (command) => {
        handleCommand(command, messages);
      });
    })
    .catch((e) => console.log("Error: Connection failed", e));
});
</script>
