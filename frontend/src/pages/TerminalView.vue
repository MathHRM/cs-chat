<template>
  <div class="terminal-container">
    <CommandsComponent :messages="messages" />

    <CommandLine @send-message="handleSendMessage" />
  </div>
</template>

<script setup>
import CommandsComponent from "@/components/CommandsComponent.vue";
import CommandLine from "@/components/CommandLine.vue";
import { ref, onMounted, reactive, computed } from "vue";
import Hub from "../Hub";
import { HubConnectionState } from "@aspnet/signalr";
import { useAuthStore } from "@/stores/auth";

const _hub = new Hub();
let messages = ref([]);
let message = reactive({
  username: "",
  content: "",
});

const authStore = useAuthStore();
const getUser = computed(() => authStore.user);

function handleSendMessage(content) {
  if (!content.trim()) return;

  message.username = getUser.value.username;
  message.content = content;

  if (_hub.connection.state != HubConnectionState.Connected) {
    messages.value.push({
      username: "System",
      content: "Connection not established",
      created_at: new Date(),
    });

    return;
  }

  console.log(message);
  _hub.connection.invoke("SendMessage", message);
  message.content = "";
}

onMounted(() => {
  console.log('Connecting to hub');

  _hub.connection
    .start()
    .then(() => {
      _hub.connection.on("ReceivedMessage", (msg) => {
        messages.value.push(msg);
      });
    })
    .catch((e) => console.log("Error: Connection failed", e));
});
</script>
