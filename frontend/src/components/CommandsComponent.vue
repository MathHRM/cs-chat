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
  ref,
  defineEmits,
} from "vue";

defineProps({
  messages: Array,
});

const emit = defineEmits(["load-more-messages"]);

const terminalBody = ref(null);

const scrollToBottom = () => {
  nextTick(() => {
    if (terminalBody.value) {
      terminalBody.value.scrollTop = terminalBody.value.scrollHeight;
    }
  });
};

const handleScroll = () => {
  if (terminalBody.value.scrollTop === 0) {
    emit("load-more-messages");
  }
};

onMounted(() => {
  scrollToBottom();
  terminalBody.value.addEventListener("scroll", handleScroll);
});

onUpdated(() => {
  scrollToBottom();
});
</script>
