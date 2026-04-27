import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router' // Importe o router aqui
import './style.css'

const app = createApp(App)
app.use(createPinia())
app.use(router) // Registre o router
app.mount('#app')