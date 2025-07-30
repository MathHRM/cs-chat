<template>
  <ChatIdentifier :username="`Sistema`" />
  <span class="terminal-message">Comandos disponíveis:</span>
  <div v-for="command in commands" :key="command.name">
    <span class="terminal-message">/{{ command.name }} - {{ command.description }}</span>
    <span class="terminal-message" v-for="argument in command.arguments" :key="argument.name">
      <br>
      <span class="terminal-message argument">{{ argument.name }} - {{ argument.description }}</span>
    </span>
    <span class="terminal-message" v-for="option in command.options" :key="option.name">
      <br>
      <span class="terminal-message argument">{{ getOptionName(option) }} {{ getOptionAlias(option) }} - {{ option.description }}</span>
    </span>
    <br><br>
    <button class="terminal-message" @click="copyToClipboard(getCommandCopyShortcut(command))">
      <span class="terminal-message">Copie o comando: {{ getCommandCopyShortcut(command) }}</span>
    </button>
    <br><br>
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
  const opts = command.options.filter(opt => opt.isRequired).map(opt => `${opt.name}`).join(" ");
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

const getOptionName = (option) => {
  if (option.isRequired) {
    return `${option.name}*`;
  }
  return option.name;
};

const getOptionAlias = (option) => {
  let aliases = "";
  if (option.aliases) {
    aliases = option.aliases.map(alias => `${alias}`).join(" ");
  }

  return `| ${aliases}`;
};

defineProps({
  commands: {
    type: Array,
  },
});
</script>

<style scoped>
.argument {
  margin-left: 30px;
}

button {
  background-color: #212121;
  color: #fff;
  border: none;
  padding: 10px 20px;
  border-radius: 5px;
  cursor: pointer;
}
</style>