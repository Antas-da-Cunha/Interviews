<script setup lang="ts">
import { RouterLink } from 'vue-router'
// Header component for the teste application
</script>

<template>
  <header class="teste-header">
    <div class="logo-container">
      <img src="@/assets/teste-logo.svg" alt="Antas da Cunha Ecija Logo" class="logo" />
    </div>
    <nav class="nav-links">
      <RouterLink to="/" class="nav-link">Dashboard</RouterLink>
      <RouterLink to="/make" class="nav-link">Make</RouterLink>
    </nav>
  </header>
</template>

<style scoped>
.teste-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 1.5rem 0;
  background-color: white;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  width: 100%;
  border-bottom: 1px solid #eaeaea;
}

.logo-container {
  flex: 0 0 auto;
  text-align: center;
  margin-bottom: 1rem;
}

.logo {
  width: 350px;
  height: auto;
}

.nav-links {
  display: flex;
  gap: 2rem;
}

.nav-link {
  color: #333;
  text-decoration: none;
  font-weight: 500;
  padding: 0.5rem 0;
  position: relative;
}

.nav-link:after {
  content: '';
  position: absolute;
  width: 0;
  height: 2px;
  bottom: 0;
  left: 0;
  background-color: #E74C3C;
  transition: width 0.3s ease;
}

.nav-link:hover:after,
.nav-link.router-link-active:after {
  width: 100%;
}

.nav-link.router-link-active {
  color: #E74C3C;
}
</style> 