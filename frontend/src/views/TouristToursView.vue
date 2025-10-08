<template>
  <v-container class="tours-container">
    <v-row>
      <v-col cols="12">
        <div class="d-flex justify-space-between align-center mb-6">
          <div>
            <h1 class="text-h4 font-weight-bold text-primary mb-2">Available Tours</h1>
            <p class="text-body-1 text-grey-darken-1">Discover amazing tours and adventures</p>
          </div>
        </div>
      </v-col>
    </v-row>

    <!-- Success Alert -->
    <v-row v-if="successMessage">
      <v-col cols="12">
        <v-alert type="success" dismissible @click:close="successMessage = ''">
          {{ successMessage }}
        </v-alert>
      </v-col>
    </v-row>

    <!-- Error Alert -->
    <v-row v-if="errorMessage">
      <v-col cols="12">
        <v-alert type="error" dismissible @click:close="errorMessage = ''">
          {{ errorMessage }}
        </v-alert>
      </v-col>
    </v-row>

    <!-- Loading State -->
    <v-row v-if="loading">
      <v-col cols="12" class="text-center">
        <v-progress-circular
          indeterminate
          color="primary"
          size="64"
        ></v-progress-circular>
        <p class="mt-4 text-h6">Loading tours...</p>
      </v-col>
    </v-row>

    <!-- Empty State -->
    <v-row v-if="!loading && tours.length === 0 && !errorMessage">
      <v-col cols="12" class="text-center">
        <v-icon size="120" color="grey-lighten-1" class="mb-4">mdi-map-marker-off</v-icon>
        <h2 class="text-h5 text-grey-darken-1 mb-2">No tours available</h2>
        <p class="text-body-1 text-grey-darken-1">There are no published tours at the moment.</p>
      </v-col>
    </v-row>

    <!-- Tours Grid -->
    <v-row v-if="!loading && tours.length > 0">
      <v-col 
        v-for="tour in tours" 
        :key="tour.id" 
        cols="12" 
        md="6" 
        lg="4"
        class="animate-scale-up"
      >
        <v-card 
          class="tour-card h-100" 
          elevation="3"
          hover
          @click="viewTourDetails(tour.id)"
        >
          <!-- Status Badge -->
          <div class="status-badge">
            <v-chip
              color="success"
              size="small"
              variant="flat"
            >
              Published
            </v-chip>
          </div>

          <v-card-title class="text-h5 font-weight-bold pa-4 pb-2">
            {{ tour.name }}
          </v-card-title>

          <v-card-subtitle class="pa-4 pt-0">
            <v-chip
              :color="getDifficultyColor(tour.difficulty)"
              size="small"
              class="mr-2"
              prepend-icon="mdi-gauge"
            >
              {{ tour.difficulty }}
            </v-chip>
          </v-card-subtitle>

          <v-card-text class="pa-4 pt-0">
            <!-- Description -->
            <p class="text-body-2 mb-3">{{ tour.description }}</p>

            <!-- Tags -->
            <div v-if="tour.tags && tour.tags.length > 0" class="mb-3">
              <v-chip
                v-for="tag in tour.tags.slice(0, 3)"
                :key="tag"
                size="x-small"
                color="primary"
                variant="outlined"
                class="mr-1 mb-1"
              >
                {{ tag }}
              </v-chip>
              <v-chip
                v-if="tour.tags.length > 3"
                size="x-small"
                color="grey"
                variant="outlined"
              >
                +{{ tour.tags.length - 3 }} more
              </v-chip>
            </div>

            <!-- First Key Point -->
            <div v-if="tour.keyPoints && tour.keyPoints.length > 0" class="mb-3">
              <v-divider class="my-3"></v-divider>
              <div class="d-flex align-center mb-2">
                <v-icon icon="mdi-map-marker" size="small" class="mr-2" color="primary"></v-icon>
                <span class="text-body-2 font-weight-medium">Starting Point</span>
              </div>
              <div class="starting-point">
                <v-chip
                  size="small"
                  color="primary"
                  variant="outlined"
                  class="mb-1"
                >
                  <v-icon start icon="mdi-map-marker" size="small"></v-icon>
                  {{ tour.keyPoints[0].name }}
                </v-chip>
                <div class="text-caption text-grey-darken-1">
                  {{ tour.keyPoints[0].latitude.toFixed(6) }}, {{ tour.keyPoints[0].longitude.toFixed(6) }}
                </div>
              </div>
            </div>


            <!-- Distance Info -->
            <div v-if="tour.distanceInKm" class="mb-3">
              <v-divider class="my-3"></v-divider>
              <div class="d-flex align-center">
                <v-icon icon="mdi-map-marker-distance" size="small" class="mr-2" color="success"></v-icon>
                <span class="text-body-2 font-weight-medium">{{ tour.distanceInKm }} km</span>
              </div>
            </div>

            <!-- Meta Info -->
            <v-divider class="my-3"></v-divider>
            <div class="d-flex justify-space-between text-caption text-grey-darken-1">
              <div>
                <v-icon icon="mdi-calendar" size="small" class="mr-1"></v-icon>
                Published: {{ formatDate(tour.publishedAt) }}
              </div>
              <div v-if="tour.keyPoints && tour.keyPoints.length > 0">
                <v-icon icon="mdi-map-marker-multiple" size="small" class="mr-1"></v-icon>
                {{ tour.keyPoints.length }} points
              </div>
            </div>
          </v-card-text>

          <v-card-actions class="pa-4 pt-0">
            <v-btn
              variant="elevated"
              color="primary"
              prepend-icon="mdi-eye"
              @click.stop="viewTourDetails(tour.id)"
              block
            >
              View Details
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import axios from '@/utils/axiosInstance';
import { store } from '@/utils/store';

export default {
  name: 'TouristToursView',
  computed: {
    store() {
      return store;
    }
  },
  data() {
    return {
      tours: [],
      loading: false,
      errorMessage: '',
      successMessage: ''
    };
  },
  methods: {
    async fetchTours() {
      this.loading = true;
      this.errorMessage = '';

      try {
        const response = await axios.get('/tours/public');
        this.tours = response.data;
      } catch (error) {
        console.error('Error fetching tours:', error);
        this.errorMessage = error.response?.data?.message || error.message || 'Failed to load tours.';
      } finally {
        this.loading = false;
      }
    },
    viewTourDetails(tourId) {
      // For now, just show an alert - you can implement a detailed view later
      alert(`Tour details for tour ID: ${tourId}`);
    },
    getDifficultyColor(difficulty) {
      const colors = {
        'Easy': 'success',
        'Moderate': 'info',
        'Hard': 'warning',
        'Expert': 'error'
      };
      return colors[difficulty] || 'grey';
    },
    formatDate(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString);
      return date.toLocaleDateString('sr-RS', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false
      });
    }
  },
  mounted() {
    this.fetchTours();
  }
};
</script>

<style scoped>
.tours-container {
  max-width: 1400px;
  padding-top: 32px;
}

.tour-card {
  position: relative;
  border-radius: 16px;
  transition: all 0.3s ease;
  cursor: pointer;
}

.tour-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.15);
}

.status-badge {
  position: absolute;
  top: 16px;
  right: 16px;
  z-index: 2;
}

.starting-point {
  background: #f8f9fa;
  border-radius: 8px;
  padding: 8px;
  border-left: 3px solid #1976d2;
}


.animate-scale-up {
  animation: scaleUp 0.4s ease-out;
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

.tour-card:nth-child(1) { animation-delay: 0s; }
.tour-card:nth-child(2) { animation-delay: 0.1s; }
.tour-card:nth-child(3) { animation-delay: 0.2s; }
.tour-card:nth-child(4) { animation-delay: 0.3s; }
.tour-card:nth-child(5) { animation-delay: 0.4s; }
.tour-card:nth-child(6) { animation-delay: 0.5s; }

@media (max-width: 960px) {
  .tours-container {
    padding-top: 20px;
  }
}
</style>
