<template>
  <div class="q-pa-md row justify-center">
    <div style="width: 100%; max-width: 400px">
    <div style="width: 100%" v-for="(message, index) in messages" :key="index">
      <q-chat-message 
        v-if="message.username == userActual"
        sent
        :name="message.username"
        :text="[message.content]"
        :stamp="message.created_at"
      />
      <q-chat-message
        v-else
        :name="message.username"
        :text="[message.content]"
        :stamp="message.created_at"
      /></div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'ChatComponent',
  props: {
    userActual: String,
    messages: Array
  },
}
</script>
