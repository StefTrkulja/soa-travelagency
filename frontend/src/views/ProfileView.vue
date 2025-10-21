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
      <v-col cols="12" md="8" lg="10">
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
                
                <!-- Profile Summary (when not editing) -->
                <div v-if="!isEditing" class="mt-4">
                  <h2 class="text-h5 mb-2">{{ profile.name }} {{ profile.surname }}</h2>
                  <p v-if="profile.motto" class="text-subtitle-1 font-italic mb-2">"{{ profile.motto }}"</p>
                  <p v-if="profile.biography" class="text-body-1">{{ profile.biography }}</p>
                </div>
              </div>

              <!-- Username (Read-only) -->
              <v-text-field
                label="Username"
                :model-value="profile.username"
                prepend-icon="mdi-account"
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

              <!-- Email -->
              <v-text-field
                v-model="editForm.Email"
                label="Email"
                prepend-icon="mdi-email"
                :readonly="!isEditing"
                variant="outlined"
                :rules="emailRules"
                class="mb-4"
              ></v-text-field>

              <!-- Biography -->
              <v-textarea
                v-model="editForm.Biography"
                label="Biography"
                prepend-icon="mdi-text"
                :readonly="!isEditing"
                variant="outlined"
                rows="3"
                class="mb-4"
                hint="Tell us about yourself (max 500 characters)"
                :rules="biographyRules"
                counter="500"
              ></v-textarea>

              <!-- Motto -->
              <v-text-field
                v-model="editForm.Motto"
                label="Personal Motto"
                prepend-icon="mdi-format-quote-close"
                :readonly="!isEditing"
                variant="outlined"
                class="mb-4"
                hint="Your personal motto or favorite quote (max 150 characters)"
                :rules="mottoRules"
                counter="150"
              ></v-text-field>

              <!-- Role (Read-only) -->
              <v-text-field
                label="Role"
                :model-value="profile.role"
                prepend-icon="mdi-account-group"
                readonly
                variant="outlined"
                class="mb-4"
              ></v-text-field>

              <!-- Password Change Section -->
              <v-expansion-panels v-if="isEditing" class="mb-4">
                <v-expansion-panel>
                  <v-expansion-panel-title>
                    <v-icon icon="mdi-lock" class="mr-2"></v-icon>
                    Change Password
                  </v-expansion-panel-title>
                  <v-expansion-panel-text>
                    <v-text-field
                      v-model="passwordForm.currentPassword"
                      label="Current Password"
                      prepend-icon="mdi-lock"
                      type="password"
                      variant="outlined"
                      :rules="passwordRules"
                      class="mb-3"
                    ></v-text-field>
                    <v-text-field
                      v-model="passwordForm.newPassword"
                      label="New Password"
                      prepend-icon="mdi-lock-outline"
                      type="password"
                      variant="outlined"
                      :rules="newPasswordRules"
                      class="mb-3"
                    ></v-text-field>
                    <v-text-field
                      v-model="passwordForm.confirmPassword"
                      label="Confirm New Password"
                      prepend-icon="mdi-lock-check"
                      type="password"
                      variant="outlined"
                      :rules="confirmNewPasswordRules"
                    ></v-text-field>
                  </v-expansion-panel-text>
                </v-expansion-panel>
              </v-expansion-panels>

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
        Email: '',
        Biography: '',
        Motto: ''
      },
      passwordForm: {
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      },
      nameRules: [
        v => !!v || 'First name is required',
        v => (v && v.length >= 2) || 'First name must be at least 2 characters'
      ],
      surnameRules: [
        v => !!v || 'Last name is required',
        v => (v && v.length >= 2) || 'Last name must be at least 2 characters'
      ],
      emailRules: [
        v => !!v || 'Email is required',
        v => /.+@.+\..+/.test(v) || 'Email must be valid'
      ],
      biographyRules: [
        v => !v || v.length <= 500 || 'Biography must be 500 characters or less'
      ],
      mottoRules: [
        v => !v || v.length <= 150 || 'Motto must be 150 characters or less'
      ],
      passwordRules: [
        v => !!v || 'Current password is required'
      ],
      newPasswordRules: [
        v => {
          if (!v && !this.passwordForm.confirmPassword) return true;
          if (!v) return 'New password is required';
          return v.length >= 6 || 'New password must be at least 6 characters';
        }
      ],
      confirmNewPasswordRules: [
        v => {
          if (!v && !this.passwordForm.newPassword) return true;
          if (!v) return 'Please confirm your new password';
          if (!this.passwordForm.newPassword) return 'Please enter a new password first';
          return v === this.passwordForm.newPassword || 'Passwords must match';
        }
      ]
    };
  },
  methods: {
    async fetchProfile() {
      this.loading = true;
      this.errorMessage = '';

      try {
        const response = await axios.get('/stakeholders/profile');
        // Explicitly set profile object to ensure reactivity
        this.profile = {
          id: response.data.id,
          username: response.data.username,
          email: response.data.email,
          role: response.data.role,
          name: response.data.name,
          surname: response.data.surname,
          profilePicture: response.data.profilePicture,
          biography: response.data.biography,
          motto: response.data.motto
        };
        //console.log('Profile data:', this.profile);
        //console.log('Profile email:', this.profile.email);
        //console.log('Profile username:', this.profile.username);
        //console.log('Profile role:', this.profile.role);
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
        Name: this.profile.name || '',
        Surname: this.profile.surname || '',
        Email: this.profile.email || '',
        Biography: this.profile.biography || '',
        Motto: this.profile.motto || ''
      };
      this.passwordForm = {
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      };
    },
    resetPasswordForm() {
      this.passwordForm = {
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      };
    },
    startEditing() {
      this.isEditing = true;
      this.populateEditForm();
    },
    cancelEditing() {
      this.isEditing = false;
      this.populateEditForm();
      this.resetPasswordForm();
      this.errorMessage = '';
    },
    async saveProfile() {
      if (!this.formValid) return;

      this.saving = true;
      this.errorMessage = '';

      try {
        // Update profile data
        const profileData = {
          Name: this.editForm.Name,
          Surname: this.editForm.Surname,
          Email: this.editForm.Email,
          Biography: this.editForm.Biography,
          Motto: this.editForm.Motto
        };
        
        const response = await axios.put('/stakeholders/profile', profileData);
        // Update local profile data with the response
        this.profile = {
          ...this.profile,
          name: response.data.name,
          surname: response.data.surname,
          email: response.data.email,
          biography: response.data.biography,
          motto: response.data.motto
        };
        
        // Update password if provided
        if (this.passwordForm.newPassword) {
          await axios.put('/stakeholders/profile/password', {
            currentPassword: this.passwordForm.currentPassword,
            newPassword: this.passwordForm.newPassword
          });
        }
        
        this.isEditing = false;
        this.resetPasswordForm();
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
  overflow: hidden;
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
  transition: all 0.3s ease;
}

.profile-avatar:hover {
  transform: scale(1.02);
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

/* Profile summary styling */
.profile-container h2 {
  color: #1976D2;
  font-weight: 500;
}

.profile-container .font-italic {
  color: #546E7A;
  font-style: italic;
  font-size: 1.1rem;
}

.profile-container .text-body-1 {
  color: #424242;
  line-height: 1.6;
  max-width: 500px;
  margin: 0 auto;
}

/* Form field styling improvements */
.v-text-field, .v-textarea {
  margin-bottom: 8px;
}

.v-expansion-panels {
  background-color: #F5F5F5;
  border-radius: 12px;
}

/* Action buttons styling */
.v-card-actions {
  background-color: #FAFAFA;
  border-top: 1px solid #E0E0E0;
}

/* Responsive improvements */
@media (max-width: 960px) {
  .profile-container {
    padding-top: 20px;
    padding-left: 16px;
    padding-right: 16px;
  }
  
  .text-h3 {
    font-size: 2rem !important;
  }
  
  .profile-avatar {
    width: 100px !important;
    height: 100px !important;
  }
  
  .profile-container .text-body-1 {
    font-size: 0.95rem;
  }
}

@media (max-width: 600px) {
  .profile-container {
    padding-top: 16px;
  }
  
  .text-h3 {
    font-size: 1.75rem !important;
  }
  
  .profile-avatar {
    width: 80px !important;
    height: 80px !important;
  }
}
</style>
