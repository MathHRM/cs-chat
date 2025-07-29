<template>
  <ChatIdentifier :username="`System`" />
  <span class="terminal-chat">Comandos disponíveis:</span>
  <div v-for="command in commands" :key="command.name">
    <span class="terminal-chat">/{{ command.name }} - {{ command.description }}</span>
    <span class="terminal-chat"
      @click="copyToClipboard(getCommandCopyShortcut(command))">Copie e cole o comando: {{ getCommandCopyShortcut(command) }}
    </span>
    <span class="terminal-chat" v-for="argument in command.arguments" :key="argument.name">
      <span class="terminal-chat">{{ argument.name }} - {{ argument.description }}</span>
    </span>
    <span class="terminal-chat" v-for="option in command.options" :key="option.name">
      <span class="terminal-chat">{{ option.name }} - {{ option.description }}</span>
    </span>
  </div>
</template>

<script setup>
import { alert } from "@/helpers/messageHandler";
import { defineProps } from "vue";
import { useI18n } from "vue-i18n";
import ChatIdentifier from "./ChatIdentifier.vue";

const { t } = useI18n();

const getCommandCopyShortcut = (command) => {
  const args = command.arguments.map(arg => `[${arg.name}]`).join(" ");
  const opts = command.options.map(opt => `${opt.name} [valor]`).join(" ");
  return `/${command.name} ${args} ${opts}`;
};

const copyToClipboard = async (message) => {
  try {
    await navigator.clipboard.writeText(message);

    alert(t("alerts.command-copied"), 3);
  } catch (err) {
    console.error('Falha ao copiar o texto: ', err);
  }
};

defineProps({
  commands: {
    type: Array,
  },
});
</script>
