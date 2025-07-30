<template>
  <span class="user-chat-info">
        <span class="terminal-user">{{ username }}</span>
        <span class="terminal-separator"> chat:( </span>
        <span class="terminal-chat" v-if="chat?.id">{{ `${chat?.name} (${chat?.id})` }}</span>
        <span class="terminal-chat" v-else>{{ "chat" }}</span>
        <span class="terminal-separator"> )</span>
    </span>
</template>

<script setup>
import { defineProps } from "vue";

defineProps({
  username: {
    type: String,
    required: true,
  },
  chat: {
    type: Object,
  },
});
</script>
