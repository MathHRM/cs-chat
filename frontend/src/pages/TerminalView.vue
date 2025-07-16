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
import { useAuthStore } from "@/stores/auth";
import handleCommand from "@/helpers/commandHandler";
import handleMessage from "@/helpers/messageHandler";
import { useChatStore } from "@/stores/chat";

const _hub = new Hub();
let messages = ref([]);

const authStore = useAuthStore();
const chatStore = useChatStore();
const user = computed(() => authStore.user);
const chat = computed(() => chatStore.chat);

function handleSendMessage(content) {
  handleMessage(content, _hub.connection, messages, user.value, chat.value);
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
