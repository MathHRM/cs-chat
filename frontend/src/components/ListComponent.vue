<template>
  <ChatIdentifier :username="`Sistema`" />
  <span class="terminal-message">-- Chats disponíveis --</span>
  <br /> <br />
  <div v-for="chat in chats" :key="chat.id" class="chat-container">
    <span class="terminal-message">Nome: {{ chat.name }}</span>
    <br />
    <span class="terminal-message">ID: {{ chat.id }}</span>
    <br />
    <span class="terminal-message">Descrição: {{ chat.description || "Sem descrição" }}</span>
    <br />
    <span class="terminal-message">Tipo: #{{ chat.isGroup ? "Grupo" : "Privado" }}</span>
    <br />
    <br />
    <span class="terminal-message">Entrar no chat: </span>
    <button class="terminal-message" @click="copyToClipboard(getChatCopyShortcut(chat))">
      <span class="terminal-message">{{ getChatCopyShortcut(chat) }}</span>
    </button>
    <br />
    <span class="terminal-message">--------------------------------</span>
    <br />
  </div>
</template>

<script setup>
import { alert } from "@/helpers/messageHandler";
import { defineProps } from "vue";
import { useI18n } from "vue-i18n";
import ChatIdentifier from "./ChatIdentifier.vue";

const { t } = useI18n();

const getChatCopyShortcut = (chat) => {
  if (chat.isGroup) {
    return `/join ${chat.id}`;
  }

  return `/chat [username]`;
};

const copyToClipboard = async (message) => {
  try {
    await navigator.clipboard.writeText(message);

    alert(t("alerts.command-copied"), 3);
  } catch (err) {
    console.error("Falha ao copiar o texto: ", err);
  }
};

defineProps({
  chats: {
    type: Array,
  },
});
</script>

<style scoped>
button {
  background-color: #212121;
  color: #fff;
  border: none;
  padding: 10px 20px;
  border-radius: 5px;
  cursor: pointer;
}
</style>
