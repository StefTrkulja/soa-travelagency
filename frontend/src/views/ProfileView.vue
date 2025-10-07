<template>
  <v-container class="profile-container">
    <!-- Header Section -->
    <v-row class="mb-6">
      <v-col cols="12">
        <div class="d-flex justify-space-between align-center">
          <div>
            <h1 class="text-h3 font-weight-bold text-primary mb-2">My Profile</h1>
            <p class="text-subtitle-1 text-grey-darken-1">Manage your personal information and preferences</p>
          </div>
          <v-btn
            v-if="!isEditing"
            size="large"
            color="primary"
            prepend-icon="mdi-pencil"
            @click="startEditing"
          >
            Edit Profile
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
        <p class="mt-4 text-h6">Loading profile...</p>
      </v-col>
    </v-row>

    <!-- Error State -->
    <v-alert v-if="errorMessage" type="error" class="mb-6" dismissible @click:close="errorMessage = ''">
      {{ errorMessage }}
    </v-alert>

    <!-- Success Message -->
    <v-snackbar v-model="successSnackbar" :timeout="3000" color="success">
      {{ successMessage }}
    </v-snackbar>

    <!-- Profile Content -->
    <v-row v-if="!loading && profile" class="mt-4">
      <v-col cols="12" md="8" lg="6">
        <v-card class="profile-card" elevation="4">
          <v-card-title class="text-h5 pa-6 pb-4">
            <v-icon icon="mdi-account-circle" class="mr-3" size="large"></v-icon>
            Profile Information
          </v-card-title>

          <v-card-text class="pa-6 pt-0">
            <v-form v-model="formValid" ref="profileForm">
              <!-- Profile Picture -->
              <div class="text-center mb-6">
                <v-avatar size="120" class="profile-avatar">
                  <v-img
                    v-if="profile.ProfilePicture"
                    :src="profile.ProfilePicture"
                    alt="Profile Picture"
                  ></v-img>
                  <v-icon v-else icon="mdi-account" size="60" color="grey"></v-icon>
                </v-avatar>
                <div class="mt-3">
                  <v-btn
                    v-if="isEditing"
                    size="small"
                    variant="outlined"
                    prepend-icon="mdi-camera"
                    @click="uploadProfilePicture"
                  >
                    Change Photo
                  </v-btn>
                </div>
              </div>

              <!-- Username (Read-only) -->
              <v-text-field
                label="Username"
                :model-value="profile.Username"
                prepend-icon="mdi-account"
                readonly
                variant="outlined"
                class="mb-4"
              ></v-text-field>

              <!-- Email (Read-only) -->
              <v-text-field
                label="Email"
                :model-value="profile.Email"
                prepend-icon="mdi-email"
                readonly
                variant="outlined"
                class="mb-4"
              ></v-text-field>

              <!-- Role (Read-only) -->
              <v-text-field
                label="Role"
                :model-value="profile.Role"
                prepend-icon="mdi-account-group"
                readonly
                variant="outlined"
                class="mb-4"
              ></v-text-field>

              <!-- Name -->
              <v-text-field
                v-model="editForm.Name"
                label="First Name"
                prepend-icon="mdi-account"
                :readonly="!isEditing"
                variant="outlined"
                :rules="nameRules"
                class="mb-4"
              ></v-text-field>

              <!-- Surname -->
              <v-text-field
                v-model="editForm.Surname"
                label="Last Name"
                prepend-icon="mdi-account"
                :readonly="!isEditing"
                variant="outlined"
                :rules="surnameRules"
                class="mb-4"
              ></v-text-field>

              <!-- Biography -->
              <v-textarea
                v-model="editForm.Biography"
                label="Biography"
                prepend-icon="mdi-text"
                :readonly="!isEditing"
                variant="outlined"
                rows="4"
                :rules="biographyRules"
                class="mb-4"
                hint="Tell us about yourself..."
                persistent-hint
              ></v-textarea>

              <!-- Motto -->
              <v-text-field
                v-model="editForm.Motto"
                label="Motto"
                prepend-icon="mdi-quote"
                :readonly="!isEditing"
                variant="outlined"
                :rules="mottoRules"
                class="mb-4"
                hint="Your personal motto or favorite quote"
                persistent-hint
              ></v-text-field>
            </v-form>
          </v-card-text>

          <!-- Action Buttons -->
          <v-card-actions class="pa-6 pt-0" v-if="isEditing">
            <v-spacer></v-spacer>
            <v-btn
              variant="text"
              @click="cancelEditing"
              :disabled="saving"
            >
              Cancel
            </v-btn>
            <v-btn
              color="primary"
              variant="elevated"
              @click="saveProfile"
              :loading="saving"
              :disabled="!formValid"
            >
              Save Changes
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>

      <!-- Additional Info Card -->
      <v-col cols="12" md="4" lg="6">
        <v-card class="info-card" elevation="2">
          <v-card-title class="text-h6 pa-4">
            <v-icon icon="mdi-information" class="mr-2"></v-icon>
            Account Information
          </v-card-title>
          <v-card-text class="pa-4">
            <div class="info-item mb-3">
              <v-icon icon="mdi-calendar" size="small" class="mr-2 text-grey"></v-icon>
              <span class="text-caption text-grey-darken-1">Member since:</span>
              <div class="text-body-2">{{ formatDate(new Date()) }}</div>
            </div>
            <div class="info-item mb-3">
              <v-icon icon="mdi-shield-check" size="small" class="mr-2 text-grey"></v-icon>
              <span class="text-caption text-grey-darken-1">Account Status:</span>
              <div class="text-body-2 text-success">Active</div>
            </div>
            <div class="info-item">
              <v-icon icon="mdi-account-group" size="small" class="mr-2 text-grey"></v-icon>
              <span class="text-caption text-grey-darken-1">Role:</span>
              <div class="text-body-2">{{ profile.Role }}</div>
            </div>
          </v-card-text>
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
      profile: null,
      loading: false,
      saving: false,
      isEditing: false,
      formValid: false,
      errorMessage: '',
      successSnackbar: false,
      successMessage: '',
      editForm: {
        Name: '',
        Surname: '',
        ProfilePicture: '',
        Biography: '',
        Motto: ''
      },
      nameRules: [
        v => !v || v.length <= 50 || 'Name must be less than 50 characters'
      ],
      surnameRules: [
        v => !v || v.length <= 50 || 'Surname must be less than 50 characters'
      ],
      biographyRules: [
        v => !v || v.length <= 500 || 'Biography must be less than 500 characters'
      ],
      mottoRules: [
        v => !v || v.length <= 200 || 'Motto must be less than 200 characters'
      ]
    };
  },
  methods: {
    async fetchProfile() {
      this.loading = true;
      this.errorMessage = '';

      try {
        const response = await axios.get('/stakeholders/profile');
        this.profile = response.data;
        this.populateEditForm();
      } catch (error) {
        console.error('Error fetching profile:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to load profile. Please try again.';
      } finally {
        this.loading = false;
      }
    },
    populateEditForm() {
      this.editForm = {
        Name: this.profile.Name || '',
        Surname: this.profile.Surname || '',
        ProfilePicture: this.profile.ProfilePicture || '',
        Biography: this.profile.Biography || '',
        Motto: this.profile.Motto || ''
      };
    },
    startEditing() {
      this.isEditing = true;
      this.populateEditForm();
    },
    cancelEditing() {
      this.isEditing = false;
      this.populateEditForm();
      this.errorMessage = '';
    },
    async saveProfile() {
      if (!this.formValid) return;

      this.saving = true;
      this.errorMessage = '';

      try {
        const response = await axios.put('/stakeholders/profile', this.editForm);
        this.profile = response.data;
        this.isEditing = false;
        this.successMessage = 'Profile updated successfully!';
        this.successSnackbar = true;
      } catch (error) {
        console.error('Error updating profile:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to update profile. Please try again.';
      } finally {
        this.saving = false;
      }
    },
    uploadProfilePicture() {
      // TODO: Implement profile picture upload
      this.successMessage = 'Profile picture upload feature coming soon!';
      this.successSnackbar = true;
    },
    formatDate(date) {
      return date.toLocaleDateString('en-US', { 
        year: 'numeric', 
        month: 'long', 
        day: 'numeric' 
      });
    }
  },
  mounted() {
    this.fetchProfile();
  }
};
</script>

<style scoped>
.profile-container {
  min-height: 80vh;
  padding-top: 40px;
  padding-bottom: 40px;
  background: linear-gradient(135deg, #E3F2FD 0%, #E0F2F1 100%);
}

.profile-card {
  border-radius: 16px;
  transition: all 0.3s ease;
}

.profile-card:hover {
  box-shadow: 0 8px 24px rgba(0,0,0,0.12) !important;
}

.info-card {
  border-radius: 12px;
  height: fit-content;
}

.profile-avatar {
  border: 4px solid #E3F2FD;
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

@media (max-width: 960px) {
  .profile-container {
    padding-top: 20px;
  }
  
  .text-h3 {
    font-size: 2rem !important;
  }
}
</style>
