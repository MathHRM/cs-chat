import { createRouter, createWebHistory } from 'vue-router'
import TerminalView from '@/pages/TerminalView.vue'
import LoginRegister from '@/pages/LoginRegister.vue'

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: TerminalView
    },

    {
      path: '/register',
      name: 'register',
      component: LoginRegister,
    },

    {
      path: '/login',
      name: 'login',
      component: LoginRegister,
    }
  ]
})

export default router