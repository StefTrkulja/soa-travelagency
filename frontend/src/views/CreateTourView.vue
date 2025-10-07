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
              color="primary"
              size="large"
              :disabled="!valid || loading"
              :loading="loading"
              @click="createTour"
            >
              <v-icon start>mdi-check</v-icon>
              Create Tour
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
  computed: {
    store() {
      return store;
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
        tags: []
      },
      difficultyLevels: [
        { text: 'Easy', value: 0, color: 'success', icon: 'mdi-walk' },
        { text: 'Moderate', value: 1, color: 'info', icon: 'mdi-hiking' },
        { text: 'Hard', value: 2, color: 'warning', icon: 'mdi-fire' },
        { text: 'Expert', value: 3, color: 'error', icon: 'mdi-skull' }
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
          tags: this.tourData.tags
        };

        const response = await axios.post('/tours', payload);
        
        this.successMessage = 'Tour created successfully!';
        
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
    resetForm() {
      this.tourData = {
        name: '',
        description: '',
        difficulty: null,
        tags: []
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
