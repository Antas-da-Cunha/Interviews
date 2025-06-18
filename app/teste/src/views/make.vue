<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface Hora {
  id: number
  caso: string
  iD_Advogado: number
  minutos_Registados: number
  departamento: string
  data: string
}

interface Advogado {
  id: number
  nome: string
  sobrenome: string
  email: string
  departamento: string
  escalao: string
  cidade: string
  aniversario: string
  totalHoras?: string
  evolucaoMensal?: EvolucaoMensal[]
}

interface TopAdvogado {
  id: number
  nome: string
  sobrenome: string
  departamento: string
  totalMinutos: number
}

interface EvolucaoMensal {
  mes: string
  ano: number
  totalMinutos: number
  totalHoras: string
  casos: string[]
  maxMinutos?: number
}

interface HorasPorDepartamento {
  departamento: string
  totalMinutos: number
  totalHoras: string
  percentual: number
  quantidade: number
}

const advogados = ref<Advogado[]>([])
const topAdvogados = ref<TopAdvogado[]>([])
const horasPorDepartamento = ref<HorasPorDepartamento[]>([])
const loading = ref(true)
const loadingTop = ref(true)
const loadingDepartamentos = ref(true)
const error = ref<string | null>(null)
const errorTop = ref<string | null>(null)
const errorDepartamentos = ref<string | null>(null)

const fetchHorasPorDepartamento = async () => {
  try {
    loadingDepartamentos.value = true
    errorDepartamentos.value = null
    const response = await fetch('http://localhost:5065/api/horas')
    
    if (!response.ok) {
      throw new Error(`Erro HTTP: ${response.status}`)
    }
    
    const horas: Hora[] = await response.json()
    
    // Agrupar horas por departamento
    const departamentosMap = new Map<string, { minutos: number; quantidade: number }>()
    
    horas.forEach(hora => {
      const dept = hora.departamento || 'Sem Departamento'
      const minutos = hora.minutos_Registados || 0
      
      if (!departamentosMap.has(dept)) {
        departamentosMap.set(dept, { minutos: 0, quantidade: 0 })
      }
      
      const deptData = departamentosMap.get(dept)!
      deptData.minutos += minutos
      deptData.quantidade += 1
    })
    
    // Calcular totais e percentuais
    const totalMinutos = Array.from(departamentosMap.values()).reduce((sum, dept) => sum + dept.minutos, 0)
    
    const resultado = Array.from(departamentosMap.entries())
      .map(([departamento, dados]) => ({
        departamento,
        totalMinutos: dados.minutos,
        totalHoras: formatHoras(dados.minutos),
        percentual: totalMinutos > 0 ? (dados.minutos / totalMinutos) * 100 : 0,
        quantidade: dados.quantidade
      }))
      .sort((a, b) => b.totalMinutos - a.totalMinutos)
    
    horasPorDepartamento.value = resultado
    
  } catch (err) {
    errorDepartamentos.value = err instanceof Error ? err.message : 'Erro ao carregar horas por departamento'
    console.error('Erro ao buscar horas por departamento:', err)
  } finally {
    loadingDepartamentos.value = false
  }
}

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
    
    // Buscar horas para cada advogado
    await Promise.all(advogados.value.map(async (advogado) => {
      try {
        const horasResponse = await fetch(`http://localhost:5065/api/horas/advogado/${advogado.id}`)
        if (horasResponse.ok) {
          const horas: Hora[] = await horasResponse.json()
          const totalMinutos = horas.reduce((sum, hora) => sum + (hora.minutos_Registados || 0), 0)
          advogado.totalHoras = formatHoras(totalMinutos)
          
          // Calcular evolução mensal
          advogado.evolucaoMensal = calcularEvolucaoMensal(horas)
        } else {
          advogado.totalHoras = '0h 0min'
          advogado.evolucaoMensal = []
        }
      } catch (err) {
        advogado.totalHoras = '0h 0min'
        advogado.evolucaoMensal = []
        console.error(`Erro ao buscar horas do advogado ${advogado.id}:`, err)
      }
    }))
    
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erro ao carregar advogados'
    console.error('Erro ao buscar advogados:', err)
  } finally {
    loading.value = false
  }
}

const calcularEvolucaoMensal = (horas: Hora[]): EvolucaoMensal[] => {
  const evolucaoPorMes = new Map<string, { minutos: number; casos: Set<string> }>()
  
  horas.forEach(hora => {
    const data = new Date(hora.data)
    const mesAno = `${data.getFullYear()}-${String(data.getMonth() + 1).padStart(2, '0')}`
    
    if (!evolucaoPorMes.has(mesAno)) {
      evolucaoPorMes.set(mesAno, { minutos: 0, casos: new Set() })
    }
    
    const mes = evolucaoPorMes.get(mesAno)!
    mes.minutos += hora.minutos_Registados || 0
    if (hora.caso) {
      mes.casos.add(hora.caso.trim())
    }
  })
  
  const evolucao = Array.from(evolucaoPorMes.entries())
    .map(([mesAno, dados]) => {
      const [ano, mes] = mesAno.split('-')
      return {
        mes: getNomeMes(parseInt(mes)),
        ano: parseInt(ano),
        totalMinutos: dados.minutos,
        totalHoras: formatHoras(dados.minutos),
        casos: Array.from(dados.casos)
      }
    })
    .sort((a, b) => {
      if (a.ano !== b.ano) return a.ano - b.ano
      return getNumeroMes(a.mes) - getNumeroMes(b.mes)
    })
  
  // Calcular o máximo de minutos para escala dinâmica
  const maxMinutos = Math.max(...evolucao.map(m => m.totalMinutos), 1)
  
  // Adicionar a escala máxima a cada mês
  evolucao.forEach(mes => {
    mes.maxMinutos = maxMinutos
  })
  
  return evolucao
}

const getNomeMes = (numero: number): string => {
  const meses = [
    'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
    'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'
  ]
  return meses[numero - 1] || 'Desconhecido'
}

const getNumeroMes = (nome: string): number => {
  const meses = [
    'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
    'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'
  ]
  return meses.indexOf(nome) + 1
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

const getCoresDepartamento = (index: number) => {
  const cores = [
    '#3498db', '#e74c3c', '#2ecc71', '#f39c12', '#9b59b6',
    '#1abc9c', '#e67e22', '#34495e', '#95a5a6', '#d35400'
  ]
  return cores[index % cores.length]
}

const getPieOffset = (index: number) => {
  let offset = 0
  for (let i = 0; i < index; i++) {
    offset += horasPorDepartamento.value[i].percentual * 3.6
  }
  return offset
}

onMounted(() => {
  fetchAdvogados()
  fetchTopAdvogados()
  fetchHorasPorDepartamento()
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
    
    <!-- Horas por Departamento -->
    <div class="departments-section">
      <h2>🏢 Horas por Departamento</h2>
      
      <div v-if="loadingDepartamentos" class="loading">
        <div class="spinner"></div>
        <p>Carregando horas por departamento...</p>
      </div>
      
      <div v-else-if="errorDepartamentos" class="error">
        <p>Erro: {{ errorDepartamentos }}</p>
        <button @click="fetchHorasPorDepartamento" class="retry-btn">Tentar novamente</button>
      </div>
      
      <div v-else-if="horasPorDepartamento.length === 0" class="empty">
        <p>Nenhum departamento encontrado.</p>
      </div>
      
      <div v-else class="departments-container">
        <!-- Gráfico de Pizza -->
        <div class="chart-section">
          <h3>Distribuição de Horas</h3>
          <div class="pie-chart">
            <svg viewBox="0 0 200 200" class="pie-svg">
              <defs>
                <filter id="shadow" x="-20%" y="-20%" width="140%" height="140%">
                  <feDropShadow dx="2" dy="2" stdDeviation="2" flood-color="rgba(0,0,0,0.3)"/>
                </filter>
              </defs>
              <g transform="translate(100, 100)">
                <circle 
                  v-for="(dept, index) in horasPorDepartamento" 
                  :key="`pie-${dept.departamento}`"
                  :style="{
                    fill: getCoresDepartamento(index),
                    stroke: 'white',
                    strokeWidth: '2',
                    filter: 'url(#shadow)'
                  }"
                  :r="80"
                  :stroke-dasharray="`${dept.percentual * 3.6} ${360 - dept.percentual * 3.6}`"
                  :stroke-dashoffset="getPieOffset(index)"
                  class="pie-segment"
                />
              </g>
            </svg>
          </div>
        </div>
        
        <!-- Lista de Departamentos -->
        <div class="departments-list">
          <h3>Detalhes por Departamento</h3>
          <div class="departments-grid">
            <div 
              v-for="(dept, index) in horasPorDepartamento" 
              :key="`dept-${dept.departamento}`"
              class="department-card"
              :style="{ borderLeftColor: getCoresDepartamento(index) }"
            >
              <div class="department-header">
                <div class="department-color" :style="{ backgroundColor: getCoresDepartamento(index) }"></div>
                <h4>{{ dept.departamento }}</h4>
              </div>
              <div class="department-stats">
                <div class="stat">
                  <span class="stat-label">Total Horas:</span>
                  <span class="stat-value">{{ dept.totalHoras }}</span>
                </div>
                <div class="stat">
                  <span class="stat-label">Percentual:</span>
                  <span class="stat-value">{{ dept.percentual.toFixed(1) }}%</span>
                </div>
                <div class="stat">
                  <span class="stat-label">Registros:</span>
                  <span class="stat-value">{{ dept.quantidade }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Evolução Mensal -->
    <div class="evolution-section">
      <h2>📈 Evolução Mensal das Horas por Advogado</h2>
      
      <div v-if="loading" class="loading">
        <div class="spinner"></div>
        <p>Carregando evolução mensal...</p>
      </div>
      
      <div v-else-if="error" class="error">
        <p>Erro: {{ error }}</p>
        <button @click="fetchAdvogados" class="retry-btn">Tentar novamente</button>
      </div>
      
      <div v-else class="evolution-container">
        <div 
          v-for="advogado in advogados" 
          :key="`evolution-${advogado.id}`" 
          class="advogado-evolution"
        >
          <div class="advogado-header">
            <h3>{{ advogado.nome }} {{ advogado.sobrenome }}</h3>
            <span class="department-badge">{{ advogado.departamento }}</span>
          </div>
          
          <div v-if="!advogado.evolucaoMensal || advogado.evolucaoMensal.length === 0" class="no-evolution">
            <p>Nenhuma evolução mensal disponível.</p>
          </div>
          
          <div v-else class="evolution-chart">
            <div class="chart-container">
              <div 
                v-for="(mes, index) in advogado.evolucaoMensal" 
                :key="`${advogado.id}-${mes.mes}-${mes.ano}`"
                class="month-bar"
              >
                <div class="bar-container">
                  <div 
                    class="bar" 
                    :style="{ height: `${Math.min((mes.totalMinutos / mes.maxMinutos) * 100, 100)}%` }"
                    :title="`${mes.mes} ${mes.ano}: ${mes.totalHoras}`"
                  ></div>
                </div>
                <div class="month-label">{{ mes.mes }}</div>
                <div class="month-hours">{{ mes.totalHoras }}</div>
              </div>
            </div>
            
            <div class="evolution-details">
              <h4>Detalhes por Mês:</h4>
              <div class="month-details">
                <div 
                  v-for="mes in advogado.evolucaoMensal" 
                  :key="`details-${advogado.id}-${mes.mes}-${mes.ano}`"
                  class="month-detail"
                >
                  <div class="month-header">
                    <span class="month-name">{{ mes.mes }} {{ mes.ano }}</span>
                    <span class="month-total">{{ mes.totalHoras }}</span>
                  </div>
                  <div class="casos-list">
                    <span class="casos-label">Casos:</span>
                    <span v-if="mes.casos.length === 0" class="no-casos">Nenhum caso registrado</span>
                    <span v-else class="casos">{{ mes.casos.join(', ') }}</span>
                  </div>
                </div>
              </div>
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
        <p>Carregando advogados e horas...</p>
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
              <th>Total Horas</th>
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
              <td class="hours-cell">{{ advogado.totalHoras || '0h 0min' }}</td>
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

h4 {
  color: #2c3e50;
  margin-bottom: 1rem;
  font-size: 1.1rem;
}

.podium-section {
  margin-bottom: 3rem;
  background-color: white;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  padding: 2rem;
}

.departments-section {
  margin-bottom: 3rem;
  background-color: white;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  padding: 2rem;
}

.evolution-section {
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

/* Departamentos Styles */
.departments-container {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  align-items: start;
}

.chart-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.pie-chart {
  width: 200px;
  height: 200px;
  position: relative;
}

.pie-svg {
  width: 100%;
  height: 100%;
}

.pie-segment {
  transition: all 0.3s ease;
  cursor: pointer;
}

.pie-segment:hover {
  transform: scale(1.05);
}

.departments-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.departments-grid {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.department-card {
  background-color: #f8f9fa;
  border-radius: 8px;
  padding: 1rem;
  border-left: 4px solid;
  transition: transform 0.3s ease;
}

.department-card:hover {
  transform: translateX(5px);
}

.department-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.75rem;
}

.department-color {
  width: 16px;
  height: 16px;
  border-radius: 50%;
}

.department-header h4 {
  margin: 0;
  color: #2c3e50;
  font-size: 1rem;
}

.department-stats {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.stat {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.stat-label {
  color: #7f8c8d;
  font-size: 0.9rem;
}

.stat-value {
  font-weight: bold;
  color: #2c3e50;
}

/* Evolução Mensal Styles */
.evolution-container {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.advogado-evolution {
  border: 1px solid #ecf0f1;
  border-radius: 8px;
  padding: 1.5rem;
  background-color: #f8f9fa;
}

.advogado-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
  padding-bottom: 0.5rem;
  border-bottom: 2px solid #3498db;
}

.advogado-header h3 {
  margin: 0;
  color: #2c3e50;
}

.department-badge {
  background-color: #3498db;
  color: white;
  padding: 0.3rem 0.8rem;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 500;
}

.no-evolution {
  text-align: center;
  color: #7f8c8d;
  padding: 1rem;
}

.evolution-chart {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.chart-container {
  display: flex;
  align-items: flex-end;
  justify-content: space-around;
  gap: 1rem;
  min-height: 200px;
  padding: 1rem;
  background-color: white;
  border-radius: 8px;
  border: 1px solid #ecf0f1;
}

.month-bar {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
  flex: 1;
  max-width: 80px;
}

.bar-container {
  width: 100%;
  height: 150px;
  display: flex;
  align-items: flex-end;
  justify-content: center;
}

.bar {
  width: 100%;
  background: linear-gradient(to top, #3498db, #2980b9);
  border-radius: 4px 4px 0 0;
  min-height: 4px;
  transition: all 0.3s ease;
  cursor: pointer;
}

.bar:hover {
  background: linear-gradient(to top, #e74c3c, #c0392b);
  transform: scaleY(1.05);
}

.month-label {
  font-size: 0.8rem;
  font-weight: 500;
  color: #2c3e50;
  text-align: center;
}

.month-hours {
  font-size: 0.7rem;
  color: #27ae60;
  font-weight: bold;
  text-align: center;
}

.evolution-details {
  background-color: white;
  border-radius: 8px;
  padding: 1rem;
  border: 1px solid #ecf0f1;
}

.month-details {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.month-detail {
  padding: 0.75rem;
  background-color: #f8f9fa;
  border-radius: 6px;
  border-left: 4px solid #3498db;
}

.month-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.month-name {
  font-weight: bold;
  color: #2c3e50;
}

.month-total {
  font-weight: bold;
  color: #27ae60;
}

.casos-list {
  display: flex;
  gap: 0.5rem;
  align-items: flex-start;
}

.casos-label {
  font-weight: 500;
  color: #7f8c8d;
  font-size: 0.9rem;
  min-width: 50px;
}

.casos {
  color: #2c3e50;
  font-size: 0.9rem;
  flex: 1;
}

.no-casos {
  color: #95a5a6;
  font-style: italic;
  font-size: 0.9rem;
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

.hours-cell {
  font-weight: bold;
  color: #27ae60;
  text-align: center;
}

@media (max-width: 768px) {
  .advogados-container {
    padding: 1rem;
  }
  
  .departments-container {
    grid-template-columns: 1fr;
    gap: 1rem;
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
  
  .chart-container {
    flex-direction: column;
    align-items: center;
    min-height: auto;
    gap: 1rem;
  }
  
  .month-bar {
    flex-direction: row;
    max-width: none;
    width: 100%;
    gap: 1rem;
  }
  
  .bar-container {
    width: 60px;
    height: 100px;
  }
  
  .advogado-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }
  
  .month-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.3rem;
  }
  
  .casos-list {
    flex-direction: column;
    gap: 0.3rem;
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
