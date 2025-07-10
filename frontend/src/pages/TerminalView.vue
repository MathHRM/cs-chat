<template>
  <div class="terminal-container">
    <!-- Terminal Header -->
<!--     <div class="terminal-header">
      <div class="terminal-buttons">
        <div class="terminal-btn close"></div>
        <div class="terminal-btn minimize"></div>
        <div class="terminal-btn maximize"></div>
      </div>
      <div class="terminal-title">Terminal</div>
    </div> -->

    <CommandsComponent :messages="messages" :userActual="userActual" />

    <CommandLine :userActual="userActual" @send-message="handleSendMessage" />
  </div>
</template>

<script setup>
import CommandsComponent from "@/components/CommandsComponent.vue";
import CommandLine from "@/components/CommandLine.vue";
import { ref, defineProps, onMounted, reactive } from "vue";
import Hub from "../Hub";

const _hub = new Hub();
let messages = ref([]);
let message = reactive({
  username: "",
  content: "",
});

defineProps({
  userActual: String,
  messages: Array,
});

function handleSendMessage(content) {
  if (!content.trim()) return;

  message.content = content;

  if (_hub.connection.state != "Connected") {
    messages.value.push({
      username: "System",
      content: "Connection not established",
      created_at: new Date(),
    });

    return;
  }

  _hub.connection.invoke("SendMessage", message);
  message.content = "";
}

onMounted(() => {
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
