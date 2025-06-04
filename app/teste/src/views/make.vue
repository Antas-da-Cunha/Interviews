<script setup lang="ts">
// This is an empty page for users to build their own content
</script>

<template>
  <div class="make-container">
    <h1>Build Your Content</h1>
    <h2>Good Luck!</h2>
    <div class="empty-workspace">
      <p>This is an empty workspace for your test development.</p>
    </div>
  </div>
</template>

<style scoped>
.make-container {
  padding: 1rem;
}

h1, h2{
  color: #333;
  margin-bottom: 2rem;
  text-align: center;
}

.empty-workspace {
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 2rem;
  text-align: center;
  min-height: 300px;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
