import './assets/main.css'

import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import App from './App.vue'

const routes = [
    {
        path: '/',
        name: 'Login',
        component: () => import('./views/Login.vue')
    },
    {
        path: '/register',
        name: 'Register',
        component: () => import('./views/Register.vue')
    },
    {
        path: '/pokedex',
        name: 'pokedex',
        component: () => import('./views/Pokedex.vue'),
    },
];

const router = new createRouter({
    history: createWebHistory(),
    routes
});

createApp(App)
    .use(router)
    .mount('#app')
