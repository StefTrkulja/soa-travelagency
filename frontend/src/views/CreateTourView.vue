<template>
  <v-container class="create-tour-container">
    <v-row justify="center">
      <v-col cols="12" md="8" lg="6">
        <v-card class="tour-card animate-slide-up" elevation="8">
          <v-card-title class="text-h4 text-center pa-6 bg-gradient">
            <v-icon icon="mdi-map-plus" size="large" class="mr-3"></v-icon>
            Create New Tour
          </v-card-title>
          
          <v-divider></v-divider>
          
          <v-card-text class="pa-6">
            <v-form v-model="valid" ref="form">
              <!-- Tour Name -->
              <v-text-field
                v-model="tourData.name"
                label="Tour Name *"
                prepend-icon="mdi-text"
                :rules="nameRules"
                counter="200"
                variant="outlined"
                class="mb-4"
                required
              ></v-text-field>

              <!-- Description -->
              <v-textarea
                v-model="tourData.description"
                label="Description *"
                prepend-icon="mdi-text-box"
                :rules="descriptionRules"
                counter="2000"
                variant="outlined"
                rows="5"
                class="mb-4"
                required
              ></v-textarea>

              <!-- Difficulty -->
              <v-select
                v-model="tourData.difficulty"
                label="Difficulty Level *"
                prepend-icon="mdi-gauge"
                :items="difficultyLevels"
                item-title="text"
                item-value="value"
                :rules="difficultyRules"
                variant="outlined"
                class="mb-4"
                required
              >
                <template v-slot:selection="{ item }">
                  <v-chip :color="item.raw.color" size="small">
                    <v-icon start :icon="item.raw.icon"></v-icon>
                    {{ item.raw.text }}
                  </v-chip>
                </template>
                <template v-slot:item="{ props, item }">
                  <v-list-item v-bind="props">
                    <template v-slot:prepend>
                      <v-icon :icon="item.raw.icon" :color="item.raw.color"></v-icon>
                    </template>
                  </v-list-item>
                </template>
              </v-select>

              <!-- Tags -->
              <v-combobox
                v-model="tourData.tags"
                label="Tags *"
                prepend-icon="mdi-tag-multiple"
                :rules="tagsRules"
                variant="outlined"
                multiple
                chips
                closable-chips
                hint="Press Enter to add a tag"
                class="mb-4"
                required
              >
                <template v-slot:chip="{ item, props }">
                  <v-chip v-bind="props" color="primary" size="small">
                    {{ item.title }}
                  </v-chip>
                </template>
              </v-combobox>

              <!-- Key Points Section -->
              <v-divider class="my-6"></v-divider>
              <h3 class="text-h6 mb-4 d-flex align-center">
                <v-icon icon="mdi-map-marker" class="mr-2"></v-icon>
                Key Points
              </h3>

              <!-- Map Component -->
              <v-card variant="outlined" class="mb-4" elevation="2">
                <v-card-title class="d-flex justify-space-between align-center">
                  <span>Interactive Map</span>
                  <div class="d-flex align-center">
                    <v-chip color="info" size="small" class="mr-2">
                      {{ tourData.keyPoints.length }} point(s)
                    </v-chip>
                    <v-chip v-if="tourData.distanceInKm > 0" color="success" size="small">
                      <v-icon icon="mdi-map-marker-distance" size="small" class="mr-1"></v-icon>
                      {{ tourData.distanceInKm }} km
                    </v-chip>
                  </div>
                </v-card-title>
                <v-card-text>
                  <p class="text-body-2 mb-3">
                    <v-icon icon="mdi-information" size="small" class="mr-1"></v-icon>
                    Click on the map to add key points. Click on existing markers to edit them.
                  </p>
                  <MapComponent 
                    v-model="tourData.keyPoints"
                    :height="400"
                    :center="[20.4573, 44.7872]"
                    :zoom="13"
                    @update:distance="updateDistance"
                  />
                </v-card-text>
              </v-card>

              <!-- Key Points List -->
              <v-card v-if="tourData.keyPoints.length > 0" variant="outlined" class="mb-4" elevation="2">
                <v-card-title>
                  <span>Key Points List</span>
                </v-card-title>
                <v-card-text>
                  <v-list>
                    <v-list-item 
                      v-for="(keyPoint, index) in tourData.keyPoints" 
                      :key="index"
                      class="mb-2"
                    >
                      <template v-slot:prepend>
                        <v-chip :color="getPointColor(index)" size="small">
                          {{ index + 1 }}
                        </v-chip>
                      </template>
                      <v-list-item-title>{{ keyPoint.name }}</v-list-item-title>
                      <v-list-item-subtitle v-if="keyPoint.description">
                        {{ keyPoint.description }}
                      </v-list-item-subtitle>
                      <v-list-item-subtitle>
                        <v-icon icon="mdi-map-marker" size="small" class="mr-1"></v-icon>
                        {{ keyPoint.latitude.toFixed(6) }}, {{ keyPoint.longitude.toFixed(6) }}
                      </v-list-item-subtitle>
                      <template v-slot:append>
                        <v-btn
                          icon="mdi-delete"
                          size="small"
                          color="error"
                          variant="text"
                          @click="removeKeyPoint(index)"
                        ></v-btn>
                      </template>
                    </v-list-item>
                  </v-list>
                </v-card-text>
              </v-card>

              <!-- Transport Times Section -->
              <v-divider class="my-6"></v-divider>
              <h3 class="text-h6 mb-4 d-flex align-center">
                <v-icon icon="mdi-clock" class="mr-2"></v-icon>
                Transport Times
              </h3>

              <v-card v-for="(transportTime, index) in tourData.transportTimes" :key="index" 
                      variant="outlined" class="mb-4" elevation="2">
                <v-card-title class="d-flex justify-space-between align-center">
                  <span>Transport Time {{ index + 1 }}</span>
                  <v-btn
                    icon="mdi-delete"
                    size="small"
                    color="error"
                    variant="text"
                    @click="removeTransportTime(index)"
                    v-if="tourData.transportTimes.length > 1"
                  ></v-btn>
                </v-card-title>
                <v-card-text>
                  <v-row>
                    <v-col cols="12" md="6">
                      <v-select
                        v-model="transportTime.transportType"
                        label="Transport Type"
                        :items="transportTypes"
                        item-title="text"
                        item-value="value"
                        variant="outlined"
                      >
                        <template v-slot:selection="{ item }">
                          <v-chip :color="item.raw.color" size="small">
                            <v-icon start :icon="item.raw.icon"></v-icon>
                            {{ item.raw.text }}
                          </v-chip>
                        </template>
                        <template v-slot:item="{ props, item }">
                          <v-list-item v-bind="props">
                            <template v-slot:prepend>
                              <v-icon :icon="item.raw.icon" :color="item.raw.color"></v-icon>
                            </template>
                          </v-list-item>
                        </template>
                      </v-select>
                    </v-col>
                    <v-col cols="12" md="6">
                      <v-text-field
                        v-model.number="transportTime.durationMinutes"
                        label="Duration (minutes)"
                        type="number"
                        variant="outlined"
                      ></v-text-field>
                    </v-col>
                  </v-row>
                </v-card-text>
              </v-card>

              <v-btn
                variant="outlined"
                color="primary"
                @click="addTransportTime"
                class="mb-4"
                prepend-icon="mdi-plus"
              >
                Add Transport Time
              </v-btn>

              <!-- Error Alert -->
              <v-alert v-if="errorMessage" type="error" class="mb-4" dismissible @click:close="errorMessage = ''">
                {{ errorMessage }}
              </v-alert>

              <!-- Success Alert -->
              <v-alert v-if="successMessage" type="success" class="mb-4" dismissible @click:close="successMessage = ''">
                {{ successMessage }}
              </v-alert>
            </v-form>
          </v-card-text>

          <v-divider></v-divider>

          <v-card-actions class="pa-6">
            <v-btn
              variant="outlined"
              color="grey"
              size="large"
              @click="$router.push('/tours')"
            >
              <v-icon start>mdi-arrow-left</v-icon>
              Cancel
            </v-btn>
            <v-spacer></v-spacer>
            <v-btn
              variant="elevated"
              color="success"
              size="large"
              :disabled="!valid || loading"
              :loading="loading"
              @click="createAndPublishTour"
              v-if="canPublish"
            >
              <v-icon start>mdi-publish</v-icon>
              Create & Publish
            </v-btn>
            <v-btn
              variant="elevated"
              color="primary"
              size="large"
              :disabled="!valid || loading"
              :loading="loading"
              @click="createTour"
            >
              <v-icon start>mdi-check</v-icon>
              Create Draft
            </v-btn>
          </v-card-actions>
          
          <!-- Info about tour creation options - positioned below buttons -->
          <v-card-actions class="pt-0 pb-4 px-6">
            <v-spacer></v-spacer>
            <div class="text-center">
              <v-chip 
                v-if="!canPublish" 
                color="warning" 
                size="small" 
                class="mb-2"
              >
                <v-icon start>mdi-information</v-icon>
                Add key points & transport times to publish
              </v-chip>
              <div class="text-caption text-grey-darken-1">
                <div v-if="canPublish">✓ Ready to publish</div>
                <div v-else>Create draft first, then add details later</div>
              </div>
            </div>
            <v-spacer></v-spacer>
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import axios from '@/utils/axiosInstance';
import { store } from '@/utils/store';
import MapComponent from '@/components/MapComponent.vue';

export default {
  components: {
    MapComponent
  },
  computed: {
    store() {
      return store;
    },
    canPublish() {
      return this.tourData.name && 
             this.tourData.description && 
             this.tourData.difficulty !== null &&
             this.tourData.tags.length > 0 &&
             this.tourData.keyPoints.filter(kp => kp.name && kp.latitude !== null && kp.longitude !== null).length >= 2 &&
             this.tourData.transportTimes.filter(tt => tt.transportType !== null && tt.durationMinutes !== null).length >= 1;
    }
  },
  data() {
    return {
      valid: false,
      loading: false,
      tourData: {
        name: '',
        description: '',
        difficulty: null,
        tags: [],
        keyPoints: [],
        transportTimes: [],
        distanceInKm: 0
      },
      difficultyLevels: [
        { text: 'Easy', value: 0, color: 'success', icon: 'mdi-walk' },
        { text: 'Moderate', value: 1, color: 'info', icon: 'mdi-hiking' },
        { text: 'Hard', value: 2, color: 'warning', icon: 'mdi-fire' },
        { text: 'Expert', value: 3, color: 'error', icon: 'mdi-skull' }
      ],
      transportTypes: [
        { text: 'Walking', value: 0, color: 'green', icon: 'mdi-walk' },
        { text: 'Bicycle', value: 1, color: 'orange', icon: 'mdi-bike' },
        { text: 'Car', value: 2, color: 'blue', icon: 'mdi-car' }
      ],
      nameRules: [
        v => !!v || 'Tour name is required',
        v => (v && v.length >= 3) || 'Tour name must be at least 3 characters',
        v => (v && v.length <= 200) || 'Tour name must not exceed 200 characters'
      ],
      descriptionRules: [
        v => !!v || 'Description is required',
        v => (v && v.length >= 10) || 'Description must be at least 10 characters',
        v => (v && v.length <= 2000) || 'Description must not exceed 2000 characters'
      ],
      difficultyRules: [
        v => v !== null && v !== undefined || 'Difficulty level is required'
      ],
      tagsRules: [
        v => (v && v.length > 0) || 'At least one tag is required',
        v => (v && v.length <= 10) || 'Maximum 10 tags allowed'
      ],
      errorMessage: '',
      successMessage: ''
    };
  },
  methods: {
    async createTour() {
      if (!this.valid) {
        return;
      }

      this.loading = true;
      this.errorMessage = '';
      this.successMessage = '';

      try {
        const payload = {
          name: this.tourData.name,
          description: this.tourData.description,
          difficulty: this.tourData.difficulty,
          tags: this.tourData.tags,
          keyPoints: this.tourData.keyPoints.filter(kp => kp.name && kp.latitude !== null && kp.longitude !== null),
          transportTimes: this.tourData.transportTimes.filter(tt => tt.transportType !== null && tt.durationMinutes !== null),
          distanceInKm: this.tourData.distanceInKm
        };

        const response = await axios.post('/tours', payload);
        
        this.successMessage = 'Tour created successfully as draft!';
        
        setTimeout(() => {
          this.$router.push('/tours');
        }, 1500);
        
      } catch (error) {
        console.error('Create tour error:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to create tour. Please try again.';
      } finally {
        this.loading = false;
      }
    },
    async createAndPublishTour() {
      if (!this.valid || !this.canPublish) {
        return;
      }

      this.loading = true;
      this.errorMessage = '';
      this.successMessage = '';

      try {
        // First create the tour
        const createPayload = {
          name: this.tourData.name,
          description: this.tourData.description,
          difficulty: this.tourData.difficulty,
          tags: this.tourData.tags,
          keyPoints: this.tourData.keyPoints.filter(kp => kp.name && kp.latitude !== null && kp.longitude !== null),
          transportTimes: this.tourData.transportTimes.filter(tt => tt.transportType !== null && tt.durationMinutes !== null),
          distanceInKm: this.tourData.distanceInKm
        };

        const createResponse = await axios.post('/tours', createPayload);
        const tourId = createResponse.data.id;

        // Then publish it
        await axios.post(`/tours/${tourId}/publish`);
        
        this.successMessage = 'Tour created and published successfully!';
        
        setTimeout(() => {
          this.$router.push('/tours');
        }, 1500);
        
      } catch (error) {
        console.error('Create and publish tour error:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to create and publish tour. Please try again.';
      } finally {
        this.loading = false;
      }
    },
    removeKeyPoint(index) {
      this.tourData.keyPoints.splice(index, 1);
    },
    getPointColor(index) {
      const colors = ['primary', 'secondary', 'success', 'warning', 'error', 'info'];
      return colors[index % colors.length];
    },
    updateDistance(distance) {
      this.tourData.distanceInKm = distance;
    },
    addTransportTime() {
      this.tourData.transportTimes.push({
        transportType: null,
        durationMinutes: null
      });
    },
    removeTransportTime(index) {
      this.tourData.transportTimes.splice(index, 1);
    },
    resetForm() {
      this.tourData = {
        name: '',
        description: '',
        difficulty: null,
        tags: [],
        keyPoints: [],
        transportTimes: [],
        distanceInKm: 0
      };
      this.$refs.form?.reset();
      this.errorMessage = '';
      this.successMessage = '';
    }
  },
  mounted() {
    if (store.role !== 'Author') {
      this.$router.push('/');
    }
    
    // Initialize with at least one transport time
    this.addTransportTime();
  }
};
</script>

<style scoped>
.create-tour-container {
  min-height: 80vh;
  padding-top: 40px;
  padding-bottom: 40px;
  background: linear-gradient(135deg, #E3F2FD 0%, #E0F2F1 100%);
}

.tour-card {
  border-radius: 16px;
  overflow: hidden;
}

.bg-gradient {
  background: linear-gradient(135deg, #1E88E5 0%, #26A69A 100%);
  color: white !important;
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

.animate-slide-up {
  animation: slideUp 0.6s ease-out;
}

:deep(.v-field) {
  border-radius: 8px;
}

:deep(.v-btn) {
  border-radius: 8px;
  text-transform: none;
  font-weight: 600;
}
</style>
