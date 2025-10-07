<template>
  <v-container class="users-container">
    <!-- Header Section -->
    <v-row class="mb-6">
      <v-col cols="12">
        <div class="d-flex justify-space-between align-center">
          <div>
            <h1 class="text-h3 font-weight-bold text-primary mb-2">Discover Users</h1>
            <p class="text-subtitle-1 text-grey-darken-1">Find and follow other travelers to see their blog posts</p>
          </div>
          <v-btn
            variant="outlined"
            prepend-icon="mdi-refresh"
            @click="refreshData"
            :loading="loading"
          >
            Refresh
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
        <p class="mt-4 text-h6">Loading users...</p>
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

    <!-- Tabs for different views -->
    <v-row v-if="!loading">
      <v-col cols="12">
        <v-tabs v-model="activeTab" color="primary" align-tabs="start">
          <v-tab value="all">All Users</v-tab>
          <v-tab value="following">Following</v-tab>
          <v-tab value="followers">Followers</v-tab>
          <v-tab value="recommendations">Recommendations</v-tab>
        </v-tabs>

        <v-tabs-window v-model="activeTab">
          <!-- All Users Tab -->
          <v-tabs-window-item value="all">
            <v-row class="mt-4">
              <v-col
                v-for="user in allUsers"
                :key="user.id"
                cols="12"
                md="6"
                lg="4"
              >
                <v-card class="user-card" elevation="4" hover>
                  <v-card-title class="pa-4 pb-2">
                    <v-avatar color="primary" class="mr-3">
                      <span class="text-h6">{{ user.username.charAt(0).toUpperCase() }}</span>
                    </v-avatar>
                    <div>
                      <div class="text-h6">{{ user.username || 'Unknown User' }}</div>
                    </div>
                  </v-card-title>

                  <v-card-text class="pa-4 pt-0">
                    <div class="d-flex justify-space-between text-body-2 mb-3">
                      <div>
                        <v-icon icon="mdi-account-group" size="small" class="mr-1"></v-icon>
                        {{ user.followersCount || 0 }} followers
                      </div>
                      <div>
                        <v-icon icon="mdi-account-plus" size="small" class="mr-1"></v-icon>
                        {{ user.followingCount || 0 }} following
                      </div>
                    </div>
                  </v-card-text>

                  <v-card-actions class="pa-4 pt-0">
                    <v-btn
                      v-if="!isFollowingUser(user.id)"
                      color="primary"
                      variant="elevated"
                      prepend-icon="mdi-account-plus"
                      @click="followUser(user.id)"
                      :loading="followingUsers.has(user.id)"
                      :disabled="user.id === currentUserId"
                    >
                      Follow
                    </v-btn>
                    <v-btn
                      v-else
                      color="error"
                      variant="outlined"
                      prepend-icon="mdi-account-minus"
                      @click="unfollowUser(user.id)"
                      :loading="followingUsers.has(user.id)"
                    >
                      Unfollow
                    </v-btn>
                    <v-spacer></v-spacer>
                  </v-card-actions>
                </v-card>
              </v-col>
            </v-row>
          </v-tabs-window-item>

          <!-- Following Tab -->
          <v-tabs-window-item value="following">
            <v-row class="mt-4">
              <v-col
                v-for="user in followingUsersList"
                :key="user.id"
                cols="12"
                md="6"
                lg="4"
              >
                <v-card class="user-card" elevation="4" hover>
                  <v-card-title class="pa-4 pb-2">
                    <v-avatar color="success" class="mr-3">
                      <span class="text-h6">{{ (user.username || 'U').charAt(0).toUpperCase() }}</span>
                    </v-avatar>
                    <div>
                      <div class="text-h6">{{ user.username || 'Unknown User' }}</div>
                      <div class="text-caption text-grey-darken-1">{{ user.role }}</div>
                      <div v-if="user.name || user.surname" class="text-caption">
                        {{ user.name }} {{ user.surname }}
                      </div>
                    </div>
                  </v-card-title>

                  <v-card-text class="pa-4 pt-0">
                    <div class="d-flex justify-space-between text-body-2 mb-3">
                      <div>
                        <v-icon icon="mdi-account-group" size="small" class="mr-1"></v-icon>
                        {{ user.followersCount || 0 }} followers
                      </div>
                      <div>
                        <v-icon icon="mdi-account-plus" size="small" class="mr-1"></v-icon>
                        {{ user.followingCount || 0 }} following
                      </div>
                    </div>
                  </v-card-text>

                  <v-card-actions class="pa-4 pt-0">
                    <v-btn
                      color="error"
                      variant="outlined"
                      prepend-icon="mdi-account-minus"
                      @click="unfollowUser(user.id)"
                      :loading="followingUsers.has(user.id)"
                    >
                      Unfollow
                    </v-btn>
                    <v-spacer></v-spacer>
                  </v-card-actions>
                </v-card>
              </v-col>
            </v-row>
          </v-tabs-window-item>

          <!-- Followers Tab -->
          <v-tabs-window-item value="followers">
            <v-row class="mt-4">
              <v-col
                v-for="user in followersList"
                :key="user.id"
                cols="12"
                md="6"
                lg="4"
              >
                <v-card class="user-card" elevation="4" hover>
                  <v-card-title class="pa-4 pb-2">
                    <v-avatar color="info" class="mr-3">
                      <span class="text-h6">{{ (user.username || 'U').charAt(0).toUpperCase() }}</span>
                    </v-avatar>
                    <div>
                      <div class="text-h6">{{ user.username || 'Unknown User' }}</div>
                      <div class="text-caption text-grey-darken-1">{{ user.role }}</div>
                      <div v-if="user.name || user.surname" class="text-caption">
                        {{ user.name }} {{ user.surname }}
                      </div>
                    </div>
                  </v-card-title>

                  <v-card-text class="pa-4 pt-0">
                    <div class="d-flex justify-space-between text-body-2 mb-3">
                      <div>
                        <v-icon icon="mdi-account-group" size="small" class="mr-1"></v-icon>
                        {{ user.followersCount || 0 }} followers
                      </div>
                      <div>
                        <v-icon icon="mdi-account-plus" size="small" class="mr-1"></v-icon>
                        {{ user.followingCount || 0 }} following
                      </div>
                    </div>
                  </v-card-text>

                  <v-card-actions class="pa-4 pt-0">
                    <v-btn
                      v-if="!isFollowingUser(user.id)"
                      color="primary"
                      variant="elevated"
                      prepend-icon="mdi-account-plus"
                      @click="followUser(user.id)"
                      :loading="followingUsers.has(user.id)"
                      :disabled="user.id === currentUserId"
                    >
                      Follow Back
                    </v-btn>
                    <v-btn
                      v-else
                      color="success"
                      variant="outlined"
                      prepend-icon="mdi-check"
                      disabled
                    >
                      Following
                    </v-btn>
                    <v-spacer></v-spacer>
                  </v-card-actions>
                </v-card>
              </v-col>
            </v-row>
          </v-tabs-window-item>

          <!-- Recommendations Tab -->
          <v-tabs-window-item value="recommendations">
            <v-row class="mt-4">
              <v-col cols="12" v-if="recommendations.length === 0">
                <v-card class="empty-state-card" elevation="0">
                  <v-card-text class="text-center pa-12">
                    <v-icon icon="mdi-lightbulb-outline" size="120" color="grey-lighten-1" class="mb-4"></v-icon>
                    <h2 class="text-h4 mb-3">No Recommendations Yet</h2>
                    <p class="text-body-1 text-grey-darken-1 mb-6">Start following some users to get personalized recommendations!</p>
                  </v-card-text>
                </v-card>
              </v-col>
              <v-col
                v-for="user in recommendations"
                :key="user.id"
                cols="12"
                md="6"
                lg="4"
              >
                <v-card class="user-card recommendation-card" elevation="4" hover>
                  <v-card-title class="pa-4 pb-2">
                    <v-avatar color="warning" class="mr-3">
                      <span class="text-h6">{{ (user.username || 'U').charAt(0).toUpperCase() }}</span>
                    </v-avatar>
                    <div>
                      <div class="text-h6">{{ user.username || 'Unknown User' }}</div>
                      <div class="text-caption text-grey-darken-1">{{ user.role }}</div>
                      <div v-if="user.name || user.surname" class="text-caption">
                        {{ user.name }} {{ user.surname }}
                      </div>
                    </div>
                    <v-chip
                      color="warning"
                      size="small"
                      variant="outlined"
                      class="ml-auto"
                    >
                      Recommended
                    </v-chip>
                  </v-card-title>

                  <v-card-text class="pa-4 pt-0">
                    <div class="d-flex justify-space-between text-body-2 mb-3">
                      <div>
                        <v-icon icon="mdi-account-group" size="small" class="mr-1"></v-icon>
                        {{ user.followersCount || 0 }} followers
                      </div>
                      <div>
                        <v-icon icon="mdi-account-plus" size="small" class="mr-1"></v-icon>
                        {{ user.followingCount || 0 }} following
                      </div>
                    </div>
                  </v-card-text>

                  <v-card-actions class="pa-4 pt-0">
                    <v-btn
                      color="primary"
                      variant="elevated"
                      prepend-icon="mdi-account-plus"
                      @click="followUser(user.id)"
                      :loading="followingUsers.has(user.id)"
                      :disabled="user.id === currentUserId"
                    >
                      Follow
                    </v-btn>
                    <v-spacer></v-spacer>
                  </v-card-actions>
                </v-card>
              </v-col>
            </v-row>
          </v-tabs-window-item>
        </v-tabs-window>
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
    },
    currentUserId() {
      // Extract user ID from JWT token or store
      const token = localStorage.getItem('token');
      if (token) {
        try {
          const payload = JSON.parse(atob(token.split('.')[1]));
          return payload.id || payload.userId;
        } catch (e) {
          console.error('Error parsing token:', e);
          return null;
        }
      }
      return null;
    }
  },
  data() {
    return {
      activeTab: 'all',
      allUsers: [],
      followingUsersList: [],
      followersList: [],
      recommendations: [],
      followingUsers: new Set(), // Track users currently being followed/unfollowed
      loading: false,
      errorMessage: '',
      successSnackbar: false,
      successMessage: ''
    };
  },
  methods: {
    async refreshData() {
      this.loading = true;
      this.errorMessage = '';
      
      try {
        await Promise.all([
          this.fetchAllUsers(),
          this.fetchFollowingUsers(),
          this.fetchFollowers(),
          this.fetchRecommendations()
        ]);
      } catch (error) {
        console.error('Error refreshing data:', error);
        this.errorMessage = 'Failed to refresh data. Please try again.';
      } finally {
        this.loading = false;
      }
    },
    async fetchAllUsers() {
      try {
        // Get all users from stakeholders service
        const response = await axios.get('/stakeholders/profile/all');
        const users = response.data || [];
        
        // For each user, get their follower counts
        const usersWithCounts = await Promise.all(users.map(async (user) => {
          try {
            const followerResponse = await axios.get(`/followers/${user.id}/followers`);
            const followingResponse = await axios.get(`/followers/${user.id}/following`);
            
            return {
              ...user,
              followersCount: followerResponse.data?.length || 0,
              followingCount: followingResponse.data?.length || 0
            };
          } catch (error) {
            console.error(`Error fetching counts for user ${user.id}:`, error);
            return {
              ...user,
              followersCount: 0,
              followingCount: 0
            };
          }
        }));
        
        this.allUsers = usersWithCounts;
      } catch (error) {
        console.error('Error fetching all users:', error);
        this.allUsers = [];
      }
    },
    async fetchFollowingUsers() {
      if (!this.currentUserId) return;
      
      try {
        const response = await axios.get(`/followers/${this.currentUserId}/following`);
        const users = response.data || [];
        
        // For each user, get their profile info from stakeholders service
        const usersWithProfiles = await Promise.all(users.map(async (user) => {
          try {
            const profileResponse = await axios.get(`/stakeholders/profile/${user.userId || user.id}`);
            const profile = profileResponse.data;
            
            return {
              ...user,
              id: user.userId || user.id,
              username: profile.username || `User ${user.userId || user.id}`,
              name: profile.name,
              surname: profile.surname,
              role: profile.role,
              followersCount: user.followersCount || 0,
              followingCount: user.followingCount || 0
            };
          } catch (error) {
            console.error(`Error fetching profile for user ${user.userId || user.id}:`, error);
            return {
              ...user,
              id: user.userId || user.id,
              username: user.username || `User ${user.userId || user.id}`,
              followersCount: user.followersCount || 0,
              followingCount: user.followingCount || 0
            };
          }
        }));
        
        this.followingUsersList = usersWithProfiles;
      } catch (error) {
        console.error('Error fetching following users:', error);
        this.followingUsersList = [];
      }
    },
    async fetchFollowers() {
      if (!this.currentUserId) return;
      
      try {
        const response = await axios.get(`/followers/${this.currentUserId}/followers`);
        const users = response.data || [];
        
        // For each user, get their profile info from stakeholders service
        const usersWithProfiles = await Promise.all(users.map(async (user) => {
          try {
            const profileResponse = await axios.get(`/stakeholders/profile/${user.userId || user.id}`);
            const profile = profileResponse.data;
            
            return {
              ...user,
              id: user.userId || user.id,
              username: profile.username || `User ${user.userId || user.id}`,
              name: profile.name,
              surname: profile.surname,
              role: profile.role,
              followersCount: user.followersCount || 0,
              followingCount: user.followingCount || 0
            };
          } catch (error) {
            console.error(`Error fetching profile for user ${user.userId || user.id}:`, error);
            return {
              ...user,
              id: user.userId || user.id,
              username: user.username || `User ${user.userId || user.id}`,
              followersCount: user.followersCount || 0,
              followingCount: user.followingCount || 0
            };
          }
        }));
        
        this.followersList = usersWithProfiles;
      } catch (error) {
        console.error('Error fetching followers:', error);
        this.followersList = [];
      }
    },
    async fetchRecommendations() {
      if (!this.currentUserId) return;
      
      try {
        const response = await axios.get(`/followers/${this.currentUserId}/recommendations?limit=10`);
        const users = response.data || [];
        
        // For each user, get their profile info from stakeholders service
        const usersWithProfiles = await Promise.all(users.map(async (user) => {
          try {
            const profileResponse = await axios.get(`/stakeholders/profile/${user.userId || user.id}`);
            const profile = profileResponse.data;
            
            return {
              ...user,
              id: user.userId || user.id,
              username: profile.username || `User ${user.userId || user.id}`,
              name: profile.name,
              surname: profile.surname,
              role: profile.role,
              followersCount: user.followersCount || 0,
              followingCount: user.followingCount || 0
            };
          } catch (error) {
            console.error(`Error fetching profile for user ${user.userId || user.id}:`, error);
            return {
              ...user,
              id: user.userId || user.id,
              username: user.username || `User ${user.userId || user.id}`,
              followersCount: user.followersCount || 0,
              followingCount: user.followingCount || 0
            };
          }
        }));
        
        this.recommendations = usersWithProfiles;
      } catch (error) {
        console.error('Error fetching recommendations:', error);
        this.recommendations = [];
      }
    },
    async followUser(userId) {
      if (!this.currentUserId || userId === this.currentUserId) return;
      
      this.followingUsers.add(userId);
      
      try {
        await axios.post(`/followers/${userId}/follow`);
        this.successMessage = 'Successfully followed user!';
        this.successSnackbar = true;
        
        // Refresh data
        await this.refreshData();
      } catch (error) {
        console.error('Error following user:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to follow user. Please try again.';
      } finally {
        this.followingUsers.delete(userId);
      }
    },
    async unfollowUser(userId) {
      //console.log('Unfollow clicked for user:', userId);
      //console.log('Current user ID:', this.currentUserId);
      //console.log('Following users list:', this.followingUsersList);
      
      if (!this.currentUserId || userId === this.currentUserId) {
        console.log('Cannot unfollow - same user or no current user');
        return;
      }
      
      this.followingUsers.add(userId);
      
      try {
       // console.log('Sending POST request to:', `/followers/${userId}/unfollow`);
        const response = await axios.post(`/followers/${userId}/unfollow`);
        //console.log('Unfollow response:', response);
        
        this.successMessage = 'Successfully unfollowed user!';
        this.successSnackbar = true;
        
        // Remove from following list immediately
        this.followingUsersList = this.followingUsersList.filter(user => user.id !== userId);
        //console.log('Updated following list:', this.followingUsersList);
        
        // Refresh data
        await this.refreshData();
      } catch (error) {
        console.error('Error unfollowing user:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to unfollow user. Please try again.';
      } finally {
        this.followingUsers.delete(userId);
      }
    },
    isFollowingUser(userId) {
      return this.followingUsersList.some(user => user.id === userId || user.userId === userId);
    },
  },
  async mounted() {
    await this.refreshData();
  }
};
</script>

<style scoped>
.users-container {
  min-height: 80vh;
  padding-top: 40px;
  padding-bottom: 40px;
  background: linear-gradient(135deg, #E3F2FD 0%, #E0F2F1 100%);
}

.user-card {
  border-radius: 16px;
  transition: all 0.3s ease;
  height: 100%;
  display: flex;
  flex-direction: column;
}

.user-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0,0,0,0.12) !important;
}

.recommendation-card {
  border: 2px solid #FF9800;
  background: linear-gradient(135deg, #FFF3E0 0%, #FFFFFF 100%);
}

.empty-state-card {
  background: white;
  border-radius: 16px;
  border: 2px dashed #E0E0E0;
}

@media (max-width: 960px) {
  .users-container {
    padding-top: 20px;
  }
  
  .text-h3 {
    font-size: 2rem !important;
  }
}
</style>
