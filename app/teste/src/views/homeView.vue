<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface ProductData {
  id: number;
  name: string;
  sales: number;
  revenue: number;
  growth: number;
}

// Sample data for tables and charts
const tableData = ref<ProductData[]>([
  { id: 1, name: 'Product A', sales: 120, revenue: 12000, growth: 15 },
  { id: 2, name: 'Product B', sales: 80, revenue: 8500, growth: -5 },
  { id: 3, name: 'Product C', sales: 200, revenue: 18000, growth: 22 },
  { id: 4, name: 'Product D', sales: 150, revenue: 14000, growth: 10 },
  { id: 5, name: 'Product E', sales: 95, revenue: 9200, growth: 3 }
])

// Computed values for chart data
const chartLabels = ref<string[]>([])
const salesData = ref<number[]>([])
const revenueData = ref<number[]>([])

// Prepare chart data from table data
onMounted(() => {
  chartLabels.value = tableData.value.map(item => item.name)
  salesData.value = tableData.value.map(item => item.sales)
  revenueData.value = tableData.value.map(item => item.revenue)
  
  // Here you would typically initialize charts using a library like Chart.js
  // For this example, we'll use a placeholder
  console.log('Chart data prepared:', {
    labels: chartLabels.value,
    salesData: salesData.value,
    revenueData: revenueData.value
  })
})

// Helper methods for chart visualization
function getBarColor(index: number): string {
  const colors = ['#4CAF50', '#2196F3', '#FFC107', '#E91E63', '#9C27B0']
  return colors[index % colors.length]
}

function getSegmentSize(value: number, allValues: number[]): string {
  const total = allValues.reduce((sum: number, val: number) => sum + val, 0)
  return `${(value / total) * 100}%`
}

function getSegmentOffset(index: number, allValues: number[]): string {
  const total = allValues.reduce((sum: number, val: number) => sum + val, 0)
  let offset = 0
  for (let i = 0; i < index; i++) {
    offset += (allValues[i] / total) * 100
  }
  return `${offset}%`
}
</script>

<template>
  <div class="home">
    <h1>Dashboard</h1>
    
    <div class="dashboard-summary">
      <div class="summary-card">
        <h3>Total Sales</h3>
        <p class="summary-value">{{ tableData.reduce((sum, item) => sum + item.sales, 0) }}</p>
      </div>
      <div class="summary-card">
        <h3>Total Revenue</h3>
        <p class="summary-value">€{{ tableData.reduce((sum, item) => sum + item.revenue, 0).toLocaleString() }}</p>
      </div>
      <div class="summary-card">
        <h3>Products</h3>
        <p class="summary-value">{{ tableData.length }}</p>
      </div>
    </div>
    
    <div class="dashboard-charts">
      <div class="chart-container">
        <h2>Sales Chart</h2>
        <div class="chart-placeholder">
          <!-- In a real application, you would render a chart here -->
          <div class="chart-bars">
            <div v-for="(value, index) in salesData" :key="index" 
                 class="chart-bar" 
                 :style="{ height: `${value / 2}px`, backgroundColor: getBarColor(index) }">
              <span class="bar-value">{{ value }}</span>
            </div>
          </div>
          <div class="chart-labels">
            <span v-for="(label, index) in chartLabels" :key="index" class="chart-label">
              {{ label }}
            </span>
          </div>
        </div>
      </div>
      
      <div class="chart-container">
        <h2>Revenue Distribution</h2>
        <div class="chart-placeholder pie-chart">
          <!-- Simplified pie chart visualization -->
          <div class="pie-segments">
            <div v-for="(value, index) in revenueData" :key="index" 
                 class="pie-segment" 
                 :style="{ 
                   '--segment-size': getSegmentSize(value, revenueData),
                   '--segment-offset': getSegmentOffset(index, revenueData),
                   '--segment-color': getBarColor(index)
                 }">
            </div>
          </div>
          <div class="pie-legend">
            <div v-for="(label, index) in chartLabels" :key="index" class="legend-item">
              <span class="legend-color" :style="{ backgroundColor: getBarColor(index) }"></span>
              <span>{{ label }}: €{{ revenueData[index].toLocaleString() }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <div class="dashboard-table">
      <h2>Products Data</h2>
      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Product</th>
            <th>Sales</th>
            <th>Revenue</th>
            <th>Growth</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in tableData" :key="item.id">
            <td>{{ item.id }}</td>
            <td>{{ item.name }}</td>
            <td>{{ item.sales }}</td>
            <td>€{{ item.revenue.toLocaleString() }}</td>
            <td :class="{ 'positive': item.growth > 0, 'negative': item.growth < 0 }">
              {{ item.growth > 0 ? '+' : '' }}{{ item.growth }}%
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
.home {
  padding: 0;
}

h1 {
  color: #333;
  margin-bottom: 2rem;
  text-align: center;
}

h2 {
  color: #555;
  margin-bottom: 1rem;
}

.dashboard-summary {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.summary-card {
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 1rem;
  text-align: center;
}

.summary-value {
  font-size: 2rem;
  font-weight: bold;
  color: #E74C3C;
  margin: 0.5rem 0;
}

.dashboard-charts {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(450px, 1fr));
  gap: 2rem;
  margin-bottom: 2rem;
}

.chart-container {
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 1.5rem;
}

.chart-placeholder {
  height: 300px;
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
  border: 1px solid #eee;
  border-radius: 4px;
  padding: 1rem;
  position: relative;
}

.chart-bars {
  display: flex;
  align-items: flex-end;
  justify-content: space-around;
  height: 250px;
}

.chart-bar {
  width: 40px;
  min-height: 20px;
  position: relative;
  border-radius: 4px 4px 0 0;
  transition: height 0.3s ease;
}

.bar-value {
  position: absolute;
  top: -25px;
  left: 50%;
  transform: translateX(-50%);
  font-size: 0.875rem;
}

.chart-labels {
  display: flex;
  justify-content: space-around;
  margin-top: 0.75rem;
}

.chart-label {
  font-size: 0.875rem;
  color: #666;
}

.pie-chart {
  position: relative;
  display: flex;
  justify-content: space-between;
}

.pie-segments {
  width: 200px;
  height: 200px;
  border-radius: 50%;
  background-color: #f5f5f5;
  position: relative;
  overflow: hidden;
}

.pie-segment {
  position: absolute;
  width: 100%;
  height: 100%;
  clip-path: polygon(50% 50%, 50% 0, calc(50% + var(--segment-size)) 0, 50% 50%);
  background-color: var(--segment-color);
  transform: rotate(calc(3.6deg * var(--segment-offset)));
}

.pie-legend {
  flex: 1;
  margin-left: 2rem;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.legend-item {
  display: flex;
  align-items: center;
  margin-bottom: 0.5rem;
}

.legend-color {
  width: 16px;
  height: 16px;
  border-radius: 4px;
  margin-right: 0.5rem;
}

.dashboard-table {
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  padding: 1.5rem;
  overflow-x: auto;
}

table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
}

th, td {
  padding: 0.75rem 1rem;
  text-align: left;
}

th {
  background-color: #f5f5f5;
  font-weight: 600;
}

tr:nth-child(even) {
  background-color: #f9f9f9;
}

.positive {
  color: #4CAF50;
}

.negative {
  color: #F44336;
}

@media (max-width: 768px) {
  .dashboard-charts,
  .dashboard-summary {
    grid-template-columns: 1fr;
  }
}

h3, th, td, span{
    color: #000000;
}
</style>
