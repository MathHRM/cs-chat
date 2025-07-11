import { createApp } from 'vue'
import App from './App.vue'
import { Quasar } from 'quasar'
import quasarUserOptions from './quasar-user-options'
import './styles/app.css'
import { createPinia } from 'pinia'
import './plugins/axios'
import router from './routes'

const app = createApp(App);

app.use(createPinia());
app.use(router);

app.use(Quasar, quasarUserOptions).mount('#app')
