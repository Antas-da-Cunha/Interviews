import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/homeView.vue' 
import MakeView from '../views/make.vue' // Import the MakeView component
import DashboardView from '../views/DashboardView.vue' // Import the DashboardView component

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView, 
    },
    {
      path: '/make',
      name: 'make',
      component: MakeView, 
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: DashboardView, 
    },
  ],
})

export default router
