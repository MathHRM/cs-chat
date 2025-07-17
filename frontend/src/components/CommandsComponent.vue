<template>
  <div class="terminal-body" ref="terminalBody">
    <div
      v-for="(message, index) in messages"
      :key="index"
      class="terminal-line"
    >
      <MessageComponent :message="message" />
    </div>
  </div>
</template>

<script setup>
import MessageComponent from "./MessageComponent.vue";

import {
  onMounted,
  onUpdated,
  defineProps,
  nextTick,
  useTemplateRef,
} from "vue";

defineProps({
  messages: Array,
});

const terminalBody = useTemplateRef("terminalBody");

const scrollToBottom = () => {
  nextTick(() => {
    if (terminalBody.value) {
      terminalBody.value.scrollTop = terminalBody.value.scrollHeight;
    }
  });
};

onMounted(() => {
  scrollToBottom();
});

onUpdated(() => {
  scrollToBottom();
});
</script>
