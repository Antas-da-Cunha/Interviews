<script setup lang="ts">
import { RouterView } from 'vue-router'
import Header from './components/Header.vue'
import Footer from './components/Footer.vue'
</script>

<template>
  <div class="app-container">
    <Header />
    <main class="content">
      <div class="content-container">
        <RouterView />
      </div>
    </main>
    <Footer />
  </div>
</template>

<style>
body {
  margin: 0;
  padding: 0;
  background-color: #f5f5f7;
}

.app-container {
  display: flex;
  flex-direction: column;
}

.content {
  flex: 1;
  padding: 2rem;
  min-height: 70vh; /* Ensure there's enough space before the footer */
}

.content-container {
  margin: 0 auto;
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  padding: 2rem;
  max-width: 1200px;
}

@media (max-width: 768px) {
  .content {
    padding: 1rem;
  }
  
  .content-container {
    padding: 1rem;
  }
}
</style>
