<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface Advogado {
  id: number
  nome: string
  sobrenome: string
  email: string
  departamento: string
  escalao: string
  cidade: string
  aniversario: string
}

interface TopAdvogado {
  id: number
  nome: string
  sobrenome: string
  departamento: string
  totalMinutos: number
}

const advogados = ref<Advogado[]>([])
const topAdvogados = ref<TopAdvogado[]>([])
const loading = ref(true)
const loadingTop = ref(true)
const error = ref<string | null>(null)
const errorTop = ref<string | null>(null)

const fetchAdvogados = async () => {
  try {
    loading.value = true
    error.value = null
    const response = await fetch('http://localhost:5065/api/advogados')
    
    if (!response.ok) {
      throw new Error(`Erro HTTP: ${response.status}`)
    }
    
    const data = await response.json()
    advogados.value = data
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erro ao carregar advogados'
    console.error('Erro ao buscar advogados:', err)
  } finally {
    loading.value = false
  }
}

const fetchTopAdvogados = async () => {
  try {
    loadingTop.value = true
    errorTop.value = null
    const response = await fetch('http://localhost:5065/api/dashboard/topadvogados')
    
    if (!response.ok) {
      throw new Error(`Erro HTTP: ${response.status}`)
    }
    
    const data = await response.json()
    topAdvogados.value = data
  } catch (err) {
    errorTop.value = err instanceof Error ? err.message : 'Erro ao carregar top advogados'
    console.error('Erro ao buscar top advogados:', err)
  } finally {
    loadingTop.value = false
  }
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('pt-BR')
}

const formatHoras = (minutos: number) => {
  const horas = Math.floor(minutos / 60)
  const mins = minutos % 60
  return `${horas}h ${mins}min`
}

const getMedalha = (posicao: number) => {
  switch (posicao) {
    case 0: return '🥇'
    case 1: return '🥈'
    case 2: return '🥉'
    default: return `${posicao + 1}º`
  }
}

const getPosicaoClass = (posicao: number) => {
  switch (posicao) {
    case 0: return 'primeiro'
    case 1: return 'segundo'
    case 2: return 'terceiro'
    default: return 'outros'
  }
}

onMounted(() => {
  fetchAdvogados()
  fetchTopAdvogados()
})
</script>

<template>
  <div class="advogados-container">
    <h1>Dashboard de Advogados</h1>
    
    <!-- Pódio dos Top Advogados -->
    <div class="podium-section">
      <h2>🏆 Pódio - Maior Número de Horas</h2>
      
      <div v-if="loadingTop" class="loading">
        <div class="spinner"></div>
        <p>Carregando pódio...</p>
      </div>
      
      <div v-else-if="errorTop" class="error">
        <p>Erro: {{ errorTop }}</p>
        <button @click="fetchTopAdvogados" class="retry-btn">Tentar novamente</button>
      </div>
      
      <div v-else-if="topAdvogados.length === 0" class="empty">
        <p>Nenhum advogado encontrado no pódio.</p>
      </div>
      
      <div v-else class="podium-container">
        <div class="podium">
          <!-- 2º Lugar -->
          <div v-if="topAdvogados[1]" class="podium-place segundo">
            <div class="medal">{{ getMedalha(1) }}</div>
            <div class="avatar">{{ topAdvogados[1].nome.charAt(0) }}{{ topAdvogados[1].sobrenome.charAt(0) }}</div>
            <div class="name">{{ topAdvogados[1].nome }} {{ topAdvogados[1].sobrenome }}</div>
            <div class="department">{{ topAdvogados[1].departamento }}</div>
            <div class="hours">{{ formatHoras(topAdvogados[1].totalMinutos) }}</div>
          </div>
          
          <!-- 1º Lugar -->
          <div v-if="topAdvogados[0]" class="podium-place primeiro">
            <div class="medal">{{ getMedalha(0) }}</div>
            <div class="avatar">{{ topAdvogados[0].nome.charAt(0) }}{{ topAdvogados[0].sobrenome.charAt(0) }}</div>
            <div class="name">{{ topAdvogados[0].nome }} {{ topAdvogados[0].sobrenome }}</div>
            <div class="department">{{ topAdvogados[0].departamento }}</div>
            <div class="hours">{{ formatHoras(topAdvogados[0].totalMinutos) }}</div>
          </div>
          
          <!-- 3º Lugar -->
          <div v-if="topAdvogados[2]" class="podium-place terceiro">
            <div class="medal">{{ getMedalha(2) }}</div>
            <div class="avatar">{{ topAdvogados[2].nome.charAt(0) }}{{ topAdvogados[2].sobrenome.charAt(0) }}</div>
            <div class="name">{{ topAdvogados[2].nome }} {{ topAdvogados[2].sobrenome }}</div>
            <div class="department">{{ topAdvogados[2].departamento }}</div>
            <div class="hours">{{ formatHoras(topAdvogados[2].totalMinutos) }}</div>
          </div>
        </div>
        
        <!-- Lista dos outros advogados -->
        <div v-if="topAdvogados.length > 3" class="other-advogados">
          <h3>Outros Advogados</h3>
          <div class="other-list">
            <div 
              v-for="(advogado, index) in topAdvogados.slice(3)" 
              :key="advogado.id" 
              class="other-item"
            >
              <span class="position">{{ getMedalha(index + 3) }}</span>
              <span class="name">{{ advogado.nome }} {{ advogado.sobrenome }}</span>
              <span class="department">{{ advogado.departamento }}</span>
              <span class="hours">{{ formatHoras(advogado.totalMinutos) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Tabela de Todos os Advogados -->
    <div class="table-section">
      <h2>📋 Lista Completa de Advogados</h2>
      
      <div v-if="loading" class="loading">
        <div class="spinner"></div>
        <p>Carregando advogados...</p>
      </div>
      
      <div v-else-if="error" class="error">
        <p>Erro: {{ error }}</p>
        <button @click="fetchAdvogados" class="retry-btn">Tentar novamente</button>
      </div>
      
      <div v-else-if="advogados.length === 0" class="empty">
        <p>Nenhum advogado encontrado.</p>
      </div>
      
      <div v-else class="table-container">
        <table class="advogados-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Nome</th>
              <th>Sobrenome</th>
              <th>Email</th>
              <th>Departamento</th>
              <th>Escalão</th>
              <th>Cidade</th>
              <th>Aniversário</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="advogado in advogados" :key="advogado.id" class="advogado-row">
              <td>{{ advogado.id }}</td>
              <td>{{ advogado.nome }}</td>
              <td>{{ advogado.sobrenome }}</td>
              <td>{{ advogado.email }}</td>
              <td>{{ advogado.departamento }}</td>
              <td>{{ advogado.escalao }}</td>
              <td>{{ advogado.cidade }}</td>
              <td>{{ formatDate(advogado.aniversario) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<style scoped>
.advogados-container {
  padding: 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

h1 {
  color: #333;
  text-align: center;
  margin-bottom: 2rem;
  font-size: 2.5rem;
}

h2 {
  color: #2c3e50;
  margin-bottom: 1.5rem;
  font-size: 1.8rem;
}

h3 {
  color: #34495e;
  margin-bottom: 1rem;
  font-size: 1.3rem;
}

.podium-section {
  margin-bottom: 3rem;
  background-color: white;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  padding: 2rem;
}

.table-section {
  background-color: white;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  padding: 2rem;
}

.loading {
  text-align: center;
  padding: 2rem;
}

.spinner {
  border: 4px solid #f3f3f3;
  border-top: 4px solid #3498db;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error {
  text-align: center;
  padding: 2rem;
  color: #e74c3c;
}

.retry-btn {
  background-color: #3498db;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  cursor: pointer;
  margin-top: 1rem;
}

.retry-btn:hover {
  background-color: #2980b9;
}

.empty {
  text-align: center;
  padding: 2rem;
  color: #7f8c8d;
}

/* Pódio Styles */
.podium-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 2rem;
}

.podium {
  display: flex;
  align-items: flex-end;
  justify-content: center;
  gap: 1rem;
  min-height: 300px;
}

.podium-place {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 1.5rem;
  border-radius: 12px;
  min-width: 200px;
  position: relative;
  transition: transform 0.3s ease;
}

.podium-place:hover {
  transform: translateY(-5px);
}

.primeiro {
  background: linear-gradient(135deg, #ffd700, #ffed4e);
  height: 280px;
  order: 2;
  box-shadow: 0 8px 25px rgba(255, 215, 0, 0.3);
}

.segundo {
  background: linear-gradient(135deg, #c0c0c0, #e8e8e8);
  height: 220px;
  order: 1;
  box-shadow: 0 6px 20px rgba(192, 192, 192, 0.3);
}

.terceiro {
  background: linear-gradient(135deg, #cd7f32, #daa520);
  height: 180px;
  order: 3;
  box-shadow: 0 4px 15px rgba(205, 127, 50, 0.3);
}

.medal {
  font-size: 2.5rem;
  margin-bottom: 0.5rem;
}

.avatar {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.9);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  font-weight: bold;
  color: #2c3e50;
  margin-bottom: 0.5rem;
}

.name {
  font-weight: bold;
  font-size: 1.1rem;
  text-align: center;
  margin-bottom: 0.3rem;
  color: #2c3e50;
}

.department {
  font-size: 0.9rem;
  color: #7f8c8d;
  text-align: center;
  margin-bottom: 0.5rem;
}

.hours {
  font-weight: bold;
  font-size: 1rem;
  color: #27ae60;
  text-align: center;
}

/* Outros Advogados */
.other-advogados {
  width: 100%;
  max-width: 600px;
}

.other-list {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.other-item {
  display: flex;
  align-items: center;
  padding: 0.75rem;
  background-color: #f8f9fa;
  border-radius: 8px;
  gap: 1rem;
}

.position {
  font-weight: bold;
  color: #7f8c8d;
  min-width: 40px;
}

.name {
  flex: 1;
  font-weight: 500;
  color: #2c3e50;
}

.department {
  color: #7f8c8d;
  font-size: 0.9rem;
  min-width: 120px;
}

.hours {
  font-weight: bold;
  color: #27ae60;
  min-width: 80px;
  text-align: right;
}

/* Tabela Styles */
.table-container {
  overflow: hidden;
}

.advogados-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

.advogados-table th,
.advogados-table td {
  padding: 0.75rem;
  text-align: left;
  border-bottom: 1px solid #ecf0f1;
}

.advogados-table th {
  background-color: #34495e;
  color: white;
  font-weight: 600;
}

.advogado-row:hover {
  background-color: #f8f9fa;
}

@media (max-width: 768px) {
  .advogados-container {
    padding: 1rem;
  }
  
  .podium {
    flex-direction: column;
    align-items: center;
    gap: 1rem;
  }
  
  .podium-place {
    min-width: 150px;
    height: auto !important;
    min-height: 120px;
  }
  
  .primeiro { order: 1; }
  .segundo { order: 2; }
  .terceiro { order: 3; }
  
  .other-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }
  
  .advogados-table {
    font-size: 0.8rem;
  }
  
  .advogados-table th,
  .advogados-table td {
    padding: 0.5rem 0.3rem;
  }
}
</style>
