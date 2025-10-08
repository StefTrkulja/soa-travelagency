<template>
  <v-container class="tours-container">
    <!-- Header Section -->
    <v-row class="mb-6">
      <v-col cols="12">
        <div class="d-flex justify-space-between align-center">
          <div>
            <h1 class="text-h3 font-weight-bold text-primary mb-2">My Tours</h1>
            <p class="text-subtitle-1 text-grey-darken-1">Manage and view all your created tours</p>
          </div>
          <v-btn
            v-if="store.role === 'Author'"
            size="large"
            color="primary"
            prepend-icon="mdi-plus"
            @click="$router.push('/create-tour')"
            class="create-tour-btn"
          >
            Create New Tour
          </v-btn>
        </div>
      </v-col>
    </v-row>

    <!-- Loading State -->
    <v-row v-if="loading" class="mt-8">
      <v-col cols="12" class="text-center">
        <v-progress-circular
          indeterminate
          color="primary"
          size="64"
        ></v-progress-circular>
        <p class="mt-4 text-h6">Loading tours...</p>
      </v-col>
    </v-row>

    <!-- Success Alert -->
    <v-alert v-if="successMessage" type="success" class="mb-6" dismissible @click:close="successMessage = ''">
      {{ successMessage }}
    </v-alert>

    <!-- Error State -->
    <v-alert v-if="errorMessage" type="error" class="mb-6" dismissible @click:close="errorMessage = ''">
      {{ errorMessage }}
    </v-alert>

    <!-- Empty State -->
    <v-row v-if="!loading && tours.length === 0 && !errorMessage" class="mt-8">
      <v-col cols="12">
        <v-card class="empty-state-card" elevation="0">
          <v-card-text class="text-center pa-12">
            <v-icon icon="mdi-compass-outline" size="120" color="grey-lighten-1" class="mb-4"></v-icon>
            <h2 class="text-h4 mb-3">No Tours Yet</h2>
            <p class="text-body-1 text-grey-darken-1 mb-6">Start creating amazing tours for your travelers!</p>
            <v-btn
              v-if="store.role === 'Author'"
              size="large"
              color="primary"
              prepend-icon="mdi-plus"
              @click="$router.push('/create-tour')"
            >
              Create Your First Tour
            </v-btn>
          </v-card-text>
        </v-card>
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
      >
        <v-card
          class="tour-card animate-scale-up"
          elevation="4"
          hover
          @click="viewTourDetails(tour.id)"
        >
          <!-- Status Badge -->
          <div class="status-badge">
            <v-chip
              :color="getStatusColor(tour.status)"
              size="small"
              variant="flat"
            >
              {{ tour.status }}
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
            <v-chip
              color="success"
              size="small"
              prepend-icon="mdi-currency-usd"
            >
              ${{ tour.price }}
            </v-chip>
          </v-card-subtitle>

          <v-card-text class="pa-4 pt-2">
            <p class="tour-description text-body-2 mb-3">
              {{ truncateText(tour.description, 120) }}
            </p>

            <!-- Tags -->
            <div class="tags-container mb-3">
              <v-chip
                v-for="tag in tour.tags"
                :key="tag"
                size="x-small"
                color="primary"
                variant="outlined"
                class="mr-1 mb-1"
              >
                {{ tag }}
              </v-chip>
            </div>

            <!-- Key Points Info -->
            <div v-if="tour.keyPoints && tour.keyPoints.length > 0" class="mb-3">
              <v-divider class="my-3"></v-divider>
              <div class="d-flex align-center mb-2">
                <v-icon icon="mdi-map-marker" size="small" class="mr-2" color="primary"></v-icon>
                <span class="text-body-2 font-weight-medium">Key Points ({{ tour.keyPoints.length }})</span>
              </div>
              <div class="key-points-preview">
                <v-chip
                  v-for="(keyPoint, index) in tour.keyPoints.slice(0, 3)"
                  :key="index"
                  size="x-small"
                  color="primary"
                  variant="outlined"
                  class="mr-1 mb-1"
                >
                  {{ keyPoint.name }}
                </v-chip>
                <v-chip
                  v-if="tour.keyPoints.length > 3"
                  size="x-small"
                  color="grey"
                  variant="outlined"
                >
                  +{{ tour.keyPoints.length - 3 }} more
                </v-chip>
              </div>
            </div>

            <!-- Transport Times Info -->
            <div v-if="tour.transportTimes && tour.transportTimes.length > 0" class="mb-3">
              <v-divider class="my-3"></v-divider>
              <div class="d-flex align-center mb-2">
                <v-icon icon="mdi-clock" size="small" class="mr-2" color="info"></v-icon>
                <span class="text-body-2 font-weight-medium">Transport Times</span>
              </div>
              <div class="transport-times-preview">
                <v-chip
                  v-for="transportTime in tour.transportTimes"
                  :key="transportTime.transportType"
                  size="x-small"
                  :color="getTransportColor(transportTime.transportType)"
                  variant="outlined"
                  class="mr-1 mb-1"
                >
                  <v-icon :icon="getTransportIcon(transportTime.transportType)" size="x-small" class="mr-1"></v-icon>
                  {{ transportTime.durationMinutes }}min
                </v-chip>
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

            <!-- Timeline Info -->
            <v-divider class="my-3"></v-divider>
            <div class="timeline-info">
              <div class="timeline-item">
                <v-icon icon="mdi-calendar-plus" size="small" class="mr-2" color="primary"></v-icon>
                <span class="text-caption text-grey-darken-1">
                  <strong>Created:</strong> {{ formatDate(tour.createdAt) }}
                </span>
              </div>
              
              <div v-if="tour.publishedAt" class="timeline-item mt-1">
                <v-icon icon="mdi-publish" size="small" class="mr-2" color="success"></v-icon>
                <span class="text-caption text-grey-darken-1">
                  <strong>Published:</strong> {{ formatDate(tour.publishedAt) }}
                </span>
              </div>
              
              <div v-if="tour.archivedAt" class="timeline-item mt-1">
                <v-icon icon="mdi-archive" size="small" class="mr-2" color="warning"></v-icon>
                <span class="text-caption text-grey-darken-1">
                  <strong>Archived:</strong> {{ formatDate(tour.archivedAt) }}
                </span>
              </div>
              
              <div v-if="tour.updatedAt && tour.updatedAt !== tour.createdAt" class="timeline-item mt-1">
                <v-icon icon="mdi-update" size="small" class="mr-2" color="info"></v-icon>
                <span class="text-caption text-grey-darken-1">
                  <strong>Last Updated:</strong> {{ formatDate(tour.updatedAt) }}
                </span>
              </div>
            </div>
          </v-card-text>

          <v-card-actions class="pa-4 pt-0">
            <v-btn
              variant="text"
              color="primary"
              prepend-icon="mdi-eye"
              @click.stop="viewTourDetails(tour.id)"
            >
              View Details
            </v-btn>
            <v-spacer></v-spacer>
            
            <!-- Action buttons based on status -->
            <v-btn
              v-if="canPublish(tour)"
              icon="mdi-publish"
              size="small"
              variant="text"
              color="success"
              @click.stop="publishTour(tour)"
              :title="'Publish Tour'"
            ></v-btn>
            <v-btn
              v-if="tour.status === 'Published'"
              icon="mdi-archive"
              size="small"
              variant="text"
              color="warning"
              @click.stop="archiveTour(tour)"
              :title="'Archive Tour'"
            ></v-btn>
            <v-btn
              v-if="tour.status === 'Archived'"
              icon="mdi-restore"
              size="small"
              variant="text"
              color="info"
              @click.stop="activateTour(tour)"
              :title="'Activate Tour'"
            ></v-btn>
            
            <v-btn
              icon="mdi-pencil"
              size="small"
              variant="text"
              color="primary"
              @click.stop="editTour(tour.id)"
            ></v-btn>
            <v-btn
              icon="mdi-delete"
              size="small"
              variant="text"
              color="error"
              @click.stop="confirmDelete(tour)"
            ></v-btn>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="deleteDialog" max-width="500">
      <v-card>
        <v-card-title class="text-h5 bg-error text-white">
          <v-icon icon="mdi-alert" class="mr-2"></v-icon>
          Confirm Delete
        </v-card-title>
        <v-card-text class="pa-6">
          <p class="text-body-1">Are you sure you want to delete this tour?</p>
          <p class="text-h6 font-weight-bold mt-3">{{ tourToDelete?.name }}</p>
          <p class="text-caption text-error mt-2">This action cannot be undone.</p>
        </v-card-text>
        <v-card-actions class="pa-4">
          <v-spacer></v-spacer>
          <v-btn
            variant="text"
            @click="deleteDialog = false"
          >
            Cancel
          </v-btn>
          <v-btn
            color="error"
            variant="elevated"
            :loading="deletingTour"
            @click="deleteTour"
          >
            Delete
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script>
import axios from '@/utils/axiosInstance';
import { store } from '@/utils/store';

export default {
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
      successMessage: '',
      deleteDialog: false,
      tourToDelete: null,
      deletingTour: false
    };
  },
  methods: {
    async fetchTours() {
      this.loading = true;
      this.errorMessage = '';

      try {
        const response = await axios.get('/tours/my');
        console.log(response.data);
        this.tours = response.data;
      } catch (error) {
        console.error('Error fetching tours:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to load tours. Please try again.';
      } finally {
        this.loading = false;
      }
    },
    viewTourDetails(tourId) {
      this.$router.push(`/tours/${tourId}`);
    },
    editTour(tourId) {
      this.$router.push(`/tours/${tourId}/edit`);
    },
    confirmDelete(tour) {
      this.tourToDelete = tour;
      this.deleteDialog = true;
    },
    async deleteTour() {
      if (!this.tourToDelete) return;

      this.deletingTour = true;
      try {
        await axios.delete(`/tours/${this.tourToDelete.id}`);
        this.tours = this.tours.filter(t => t.id !== this.tourToDelete.id);
        this.deleteDialog = false;
        this.tourToDelete = null;
      } catch (error) {
        console.error('Error deleting tour:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to delete tour.';
      } finally {
        this.deletingTour = false;
      }
    },
    truncateText(text, maxLength) {
      if (text.length <= maxLength) return text;
      return text.substring(0, maxLength) + '...';
    },
    formatDate(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', { 
        year: 'numeric', 
        month: 'short', 
        day: 'numeric' 
      });
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
    getStatusColor(status) {
      const colors = {
        'Draft': 'grey',
        'Published': 'success',
        'Archived': 'warning'
      };
      return colors[status] || 'grey';
    },
    getTransportColor(transportType) {
      const colors = {
        'Walking': 'green',
        'Bicycle': 'orange',
        'Car': 'blue'
      };
      return colors[transportType] || 'grey';
    },
    getTransportIcon(transportType) {
      const icons = {
        'Walking': 'mdi-walk',
        'Bicycle': 'mdi-bike',
        'Car': 'mdi-car'
      };
      return icons[transportType] || 'mdi-help';
    },
    canPublish(tour) {
      return tour.name && 
             tour.description && 
             tour.difficulty &&
             tour.tags && tour.tags.length > 0 &&
             tour.keyPoints && tour.keyPoints.length >= 2 &&
             tour.transportTimes && tour.transportTimes.length >= 1;
    },
    async publishTour(tour) {
      try {
        await axios.post(`/tours/${tour.id}/publish`);
        tour.status = 'Published';
        tour.publishedAt = new Date().toISOString();
        this.successMessage = 'Tour published successfully!';
        setTimeout(() => { this.successMessage = ''; }, 3000);
      } catch (error) {
        console.error('Error publishing tour:', error);
        this.errorMessage = error.response?.data?.message || error.message || 'Failed to publish tour.';
        setTimeout(() => { this.errorMessage = ''; }, 5000);
      }
    },
    async archiveTour(tour) {
      try {
        await axios.post(`/tours/${tour.id}/archive`);
        tour.status = 'Archived';
        tour.archivedAt = new Date().toISOString();
        this.successMessage = 'Tour archived successfully!';
        setTimeout(() => { this.successMessage = ''; }, 3000);
      } catch (error) {
        console.error('Error archiving tour:', error);
        this.errorMessage = error.response?.data?.message || error.message || 'Failed to archive tour.';
        setTimeout(() => { this.errorMessage = ''; }, 5000);
      }
    },
    async activateTour(tour) {
      try {
        await axios.post(`/tours/${tour.id}/activate`);
        tour.status = 'Published';
        tour.archivedAt = null;
        this.successMessage = 'Tour activated successfully!';
        setTimeout(() => { this.successMessage = ''; }, 3000);
      } catch (error) {
        console.error('Error activating tour:', error);
        this.errorMessage = error.response?.data?.message || error.message || 'Failed to activate tour.';
        setTimeout(() => { this.errorMessage = ''; }, 5000);
      }
    },
    canPublish(tour) {
      return tour.status === 'Draft' &&
             tour.name &&
             tour.description &&
             tour.difficulty !== null &&
             tour.tags && tour.tags.length > 0 &&
             tour.keyPoints && tour.keyPoints.length >= 2 &&
             tour.transportTimes && tour.transportTimes.length >= 1;
    },
    getStatusColor(status) {
      const colors = {
        'Draft': 'grey',
        'Published': 'success',
        'Archived': 'warning'
      };
      return colors[status] || 'grey';
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
    if (store.role !== 'Author') {
      this.$router.push('/');
      return;
    }
    this.fetchTours();
  }
};
</script>

<style scoped>
.tours-container {
  min-height: 80vh;
  padding-top: 40px;
  padding-bottom: 40px;
  background: linear-gradient(135deg, #E3F2FD 0%, #E0F2F1 100%);
}

.create-tour-btn {
  box-shadow: 0 4px 12px rgba(30, 136, 229, 0.3);
}

.tour-card {
  height: 100%;
  display: flex;
  flex-direction: column;
  border-radius: 16px;
  transition: all 0.3s ease;
  position: relative;
  cursor: pointer;
}

.tour-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 12px 24px rgba(0,0,0,0.15) !important;
}

.status-badge {
  position: absolute;
  top: 12px;
  right: 12px;
  z-index: 1;
}

.tour-description {
  color: #616161;
  line-height: 1.6;
  min-height: 60px;
}

.tags-container {
  min-height: 28px;
}

.key-points-preview,
.transport-times-preview {
  min-height: 24px;
}

.empty-state-card {
  background: white;
  border-radius: 16px;
  border: 2px dashed #E0E0E0;
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

.animate-scale-up {
  animation: scaleUp 0.4s ease-out;
}

.tour-card:nth-child(1) { animation-delay: 0s; }
.tour-card:nth-child(2) { animation-delay: 0.1s; }
.tour-card:nth-child(3) { animation-delay: 0.2s; }
.tour-card:nth-child(4) { animation-delay: 0.3s; }
.tour-card:nth-child(5) { animation-delay: 0.4s; }

/* Timeline styles */
.timeline-info {
  background: #f8f9fa;
  border-radius: 8px;
  padding: 12px;
  border-left: 3px solid #1976d2;
}

.timeline-item {
  display: flex;
  align-items: center;
  padding: 2px 0;
}

.timeline-item:not(:last-child) {
  border-bottom: 1px solid #e0e0e0;
  padding-bottom: 8px;
  margin-bottom: 8px;
}

.timeline-item:last-child {
  margin-bottom: 0;
}
.tour-card:nth-child(6) { animation-delay: 0.5s; }

@media (max-width: 960px) {
  .tours-container {
    padding-top: 20px;
  }
  
  .text-h3 {
    font-size: 2rem !important;
  }
}
</style>
