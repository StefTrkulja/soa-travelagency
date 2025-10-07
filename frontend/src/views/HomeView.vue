<template>
  <div class="home-container">
    <!-- Hero Section -->
    <v-container fluid class="hero-section">
      <v-row align="center" justify="center" class="hero-content">
        <v-col cols="12" md="8" class="text-center">
          <h1 class="hero-title animate-slide-up">Discover Your Next Adventure</h1>
          <p class="hero-subtitle animate-fade-in">Explore breathtaking tours and create unforgettable memories</p>
          <v-btn 
            v-if="store.role === 'guest'"
            size="x-large" 
            color="accent" 
            class="mt-6 animate-bounce"
            @click="$router.push('/login')"
          >
            Get Started
            <v-icon end>mdi-arrow-right</v-icon>
          </v-btn>
          <v-btn 
            v-else
            size="x-large" 
            color="accent" 
            class="mt-6 animate-bounce"
            @click="$router.push('/tours')"
          >
            Explore Tours
            <v-icon end>mdi-compass</v-icon>
          </v-btn>
        </v-col>
      </v-row>
    </v-container>

    <!-- Features Section -->
    <v-container class="features-section py-12">
      <v-row>
        <v-col cols="12" class="text-center mb-8">
          <h2 class="section-title">Why Choose Us</h2>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="4" v-for="(feature, index) in features" :key="index" class="feature-card-col">
          <v-card class="feature-card animate-scale-up" elevation="4" hover>
            <v-card-text class="text-center pa-6">
              <v-icon :icon="feature.icon" size="60" :color="feature.color" class="mb-4"></v-icon>
              <h3 class="feature-title mb-3">{{ feature.title }}</h3>
              <p class="feature-description">{{ feature.description }}</p>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </v-container>

    <!-- CTA Section -->
    <v-container fluid class="cta-section">
      <v-row align="center" justify="center">
        <v-col cols="12" md="8" class="text-center">
          <h2 class="cta-title">Ready to Start Your Journey?</h2>
          <p class="cta-subtitle">Join thousands of travelers exploring the world</p>
          <v-btn 
            v-if="store.role === 'guest'"
            size="large" 
            variant="outlined" 
            color="white"
            class="mt-4"
            @click="$router.push('/signup')"
          >
            Sign Up Now
          </v-btn>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script>
import { store } from '@/utils/store';

export default {
  computed: {
    store() {
      return store;
    }
  },
  data() {
    return {
      features: [
        {
          icon: 'mdi-map-marker-radius',
          color: 'primary',
          title: 'Diverse Destinations',
          description: 'Explore tours across amazing locations worldwide with expert guides'
        },
        {
          icon: 'mdi-shield-check',
          color: 'success',
          title: 'Safe & Secure',
          description: 'Book with confidence knowing your adventures are protected and verified'
        },
        {
          icon: 'mdi-star',
          color: 'accent',
          title: 'Top Rated Tours',
          description: 'Join highly-rated experiences curated by professional tour authors'
        }
      ]
    };
  }
};
</script>

<style scoped>
.home-container {
  min-height: 100vh;
}

.hero-section {
  background: linear-gradient(135deg, #1E88E5 0%, #26A69A 100%);
  min-height: 60vh;
  display: flex;
  align-items: center;
  position: relative;
  overflow: hidden;
}

.hero-section::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 320"><path fill="%23ffffff" fill-opacity="0.1" d="M0,96L48,112C96,128,192,160,288,160C384,160,480,128,576,112C672,96,768,96,864,112C960,128,1056,160,1152,160C1248,160,1344,128,1392,112L1440,96L1440,320L1392,320C1344,320,1248,320,1152,320C1056,320,960,320,864,320C768,320,672,320,576,320C480,320,384,320,288,320C192,320,96,320,48,320L0,320Z"></path></svg>') no-repeat bottom;
  background-size: cover;
}

.hero-content {
  position: relative;
  z-index: 1;
}

.hero-title {
  font-size: 3.5rem;
  font-weight: 700;
  color: white;
  text-shadow: 2px 2px 4px rgba(0,0,0,0.2);
  margin-bottom: 1rem;
}

.hero-subtitle {
  font-size: 1.5rem;
  color: rgba(255,255,255,0.95);
  margin-bottom: 2rem;
}

.section-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: #1E88E5;
  margin-bottom: 1rem;
}

.features-section {
  background: #F5F5F5;
}

.feature-card {
  height: 100%;
  transition: all 0.3s ease;
  border-radius: 12px;
}

.feature-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 12px 24px rgba(0,0,0,0.15) !important;
}

.feature-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #212121;
}

.feature-description {
  font-size: 1rem;
  color: #616161;
  line-height: 1.6;
}

.cta-section {
  background: linear-gradient(135deg, #26A69A 0%, #1E88E5 100%);
  padding: 80px 0;
  color: white;
}

.cta-title {
  font-size: 2.5rem;
  font-weight: 700;
  margin-bottom: 1rem;
}

.cta-subtitle {
  font-size: 1.25rem;
  opacity: 0.95;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes scaleUp {
  from {
    opacity: 0;
    transform: scale(0.9);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

@keyframes bounce {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-10px);
  }
}

.animate-slide-up {
  animation: slideUp 0.8s ease-out;
}

.animate-fade-in {
  animation: fadeIn 1s ease-out 0.3s both;
}

.animate-scale-up {
  animation: scaleUp 0.6s ease-out;
}

.animate-bounce {
  animation: bounce 2s ease-in-out infinite;
}

.feature-card-col:nth-child(2) .feature-card {
  animation-delay: 0.2s;
}

.feature-card-col:nth-child(3) .feature-card {
  animation-delay: 0.4s;
}

@media (max-width: 960px) {
  .hero-title {
    font-size: 2.5rem;
  }
  
  .hero-subtitle {
    font-size: 1.2rem;
  }
  
  .section-title {
    font-size: 2rem;
  }
  
  .cta-title {
    font-size: 2rem;
  }
}
</style>