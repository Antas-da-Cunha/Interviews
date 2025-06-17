<template>
  <div class="dashboard">
    <h1>Dashboard de Horas</h1>
    <section>
      <h2>Total de Horas por Advogado</h2>
      <ul>
        <li v-for="adv in totalPorAdvogado" :key="adv.advogadoId">
          {{ adv.nome }}: {{ adv.totalHoras.toFixed(2) }} horas
        </li>
      </ul>
    </section>
    <section>
      <h2>Total Geral de Horas</h2>
      <p>{{ totalGeral.toFixed(2) }} horas</p>
    </section>
    <section>
      <h2>Evolução Mensal de Cada Advogado</h2>
      <div v-for="adv in totalPorAdvogado" :key="adv.advogadoId" class="monthly-evolution">
        <h3>{{ adv.nome }}</h3>
        <ul>
          <li v-for="mes in evolucaoMensal[adv.advogadoId] || []" :key="mes.year + '-' + mes.month">
            {{ mes.year }}/{{ mes.month }}: {{ mes.totalHoras.toFixed(2) }} horas
          </li>
        </ul>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import axios from 'axios';

// Defina o endpoint base correto para os endpoints do controller Advogados
const API_BASE = 'https://localhost:5065/api/advogados';


const totalPorAdvogado = ref<any[]>([]); // Lista de advogados e suas horas
const totalGeral = ref(0); // Total de horas
const evolucaoMensal = ref<Record<number, any[]>>({}); // Evolução mensal por advogado

onMounted(async () => {
  try {
    console.log(API_BASE);
    // Buscar total de horas por advogado
    const advResp = await axios.get(`${API_BASE}/total-hours`);
    console.log('Total por advogado:', advResp.data);
    totalPorAdvogado.value = advResp.data;

    // Buscar total geral de horas
    const geralResp = await axios.get(`${API_BASE}/total-hours/all`);
    console.log('Total geral:', geralResp.data); // Debug
    totalGeral.value = geralResp.data;

    // Buscar evolução mensal de cada advogado
    for (const adv of advResp.data) {
      const evoResp = await axios.get(`${API_BASE}/${adv.advogadoId}/monthly-evolution`);
      console.log(`Evolução mensal para advogado ${adv.advogadoId}:`, evoResp.data);
      evolucaoMensal.value[adv.advogadoId] = evoResp.data;
    }
  } catch (error) {
    console.error('Erro ao buscar dados do backend:', error);
  }
});
</script>

<style scoped>
.dashboard {
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
}
section {
  margin-bottom: 2rem;
}
.monthly-evolution {
  margin-bottom: 1.5rem;
}
</style>
