<script setup lang="ts">
// Footer component for the teste application
</script>

<template>
  <footer class="teste-footer">
    <div class="footer-content">
      <div class="footer-main">
        <div class="footer-logo-section">
          <img src="@/assets/teste-logo.svg" alt="Antas da Cunha Ecija Logo" class="logo" />
        </div>
        <div class="footer-info">
          <div class="developer-info">
            <h3>Developer</h3>
            <p class="developer-name">Claudio Martins</p>
          </div>
          <div class="contact-info">
            <h3>Contact</h3>
            <p><span class="icon">📧</span> cmartins@adcija.pt</p>
            <p><span class="icon">📱</span> +351 919 999 999</p>
          </div>
        </div>
      </div>
      <div class="footer-bottom">
        <div class="copyright">
          <p>© {{ new Date().getFullYear() }} Antas da Cunha Ecija. All rights reserved.</p>
        </div>
        <div class="legal-links">
          <a href="#">Privacy Policy</a>
          <a href="#">Terms of Service</a>
        </div>
      </div>
    </div>
  </footer>
</template>

<style scoped>
.teste-footer {
  background: linear-gradient(to right, #2c3e50, #4a5568);
  color: #fff;
  padding: 3rem 0 1rem;
  margin-top: 3rem;
  width: 100%;
}

.footer-content {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 2rem;
}

.footer-main {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  gap: 3rem;
  margin-bottom: 3rem;
}

.footer-logo-section {
  flex: 1;
  min-width: 250px;
  display: flex;
  align-items: center;
}

.logo {
  width: 220px;
  height: auto;
  filter: brightness(0) invert(1);
}

.footer-info {
  flex: 2;
  display: flex;
  flex-wrap: wrap;
  gap: 3rem;
}

.developer-info, .contact-info {
  flex: 1;
  min-width: 200px;
}

h3 {
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: #E74C3C;
  position: relative;
  padding-bottom: 0.75rem;
}

h3:after {
  content: '';
  position: absolute;
  left: 0;
  bottom: 0;
  width: 40px;
  height: 2px;
  background-color: #E74C3C;
}

.developer-name {
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 0.75rem;
}

.social-links {
  display: flex;
  gap: 1rem;
  margin-top: 1rem;
}

.social-link {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  background-color: rgba(255, 255, 255, 0.1);
  border-radius: 50%;
  color: #fff;
  transition: all 0.3s ease;
}

.social-link:hover {
  background-color: #E74C3C;
  transform: translateY(-3px);
}

.contact-info p {
  display: flex;
  align-items: center;
  margin-bottom: 0.75rem;
}

.icon {
  margin-right: 0.5rem;
}

.footer-bottom {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
  padding-top: 1.5rem;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
}

.copyright {
  font-size: 0.875rem;
  color: rgba(255, 255, 255, 0.7);
}

.legal-links {
  display: flex;
  gap: 1.5rem;
}

.legal-links a {
  color: rgba(255, 255, 255, 0.7);
  font-size: 0.875rem;
  text-decoration: none;
  transition: color 0.3s ease;
}

.legal-links a:hover {
  color: #E74C3C;
}

@media (max-width: 768px) {
  .footer-main, .footer-info, .footer-bottom {
    flex-direction: column;
    gap: 2rem;
  }
  
  .footer-logo-section {
    justify-content: center;
    margin-bottom: 1rem;
  }
  
  .developer-info, .contact-info {
    text-align: center;
  }
  
  h3:after {
    left: 50%;
    transform: translateX(-50%);
  }
  
  .social-links {
    justify-content: center;
  }
  
  .legal-links {
    margin-top: 1rem;
    justify-content: center;
  }
  
  .copyright {
    text-align: center;
  }
}
</style> 