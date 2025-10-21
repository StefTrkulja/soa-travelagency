<template>
  <v-container class="admin-users-container">
    <!-- Header Section -->
    <v-row class="mb-6">
      <v-col cols="12">
        <div class="d-flex justify-space-between align-center">
          <div>
            <h1 class="text-h3 font-weight-bold text-primary mb-2">User Management</h1>
            <p class="text-subtitle-1 text-grey-darken-1">Manage user accounts and permissions</p>
          </div>
          <v-chip color="primary" variant="outlined" size="large">
            <v-icon start icon="mdi-account-group"></v-icon>
            {{ users.length }} Users
          </v-chip>
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

    <!-- Users Table -->
    <v-row v-if="!loading">
      <v-col cols="12">
        <v-card class="users-card" elevation="4">
          <v-card-title class="text-h5 pa-6 pb-4">
            <v-icon icon="mdi-account-multiple" class="mr-3" size="large"></v-icon>
            User Accounts
          </v-card-title>

          <v-card-text class="pa-0">
            <v-data-table
              :headers="headers"
              :items="users"
              :loading="loading"
              class="elevation-0"
              item-key="id"
            >
              <!-- Username with Avatar -->
              <template v-slot:item.username="{ item }">
                <div class="d-flex align-center py-2">
                  <v-avatar size="40" class="mr-3">
                    <v-img
                      v-if="item.profilePicture"
                      :src="item.profilePicture"
                      :alt="item.username"
                    ></v-img>
                    <v-icon v-else icon="mdi-account" color="grey"></v-icon>
                  </v-avatar>
                  <div>
                    <div class="font-weight-medium">{{ item.username }}</div>
                    <div class="text-caption text-grey">ID: {{ item.id }}</div>
                  </div>
                </div>
              </template>

              <!-- Role Badge -->
              <template v-slot:item.role="{ item }">
                <v-chip
                  :color="getRoleColor(item.role)"
                  variant="outlined"
                  size="small"
                >
                  {{ item.role }}
                </v-chip>
              </template>

              <!-- Status Badge -->
              <template v-slot:item.blocked="{ item }">
                <v-chip
                  :color="item.blocked ? 'error' : 'success'"
                  variant="flat"
                  size="small"
                >
                  <v-icon start :icon="item.blocked ? 'mdi-block-helper' : 'mdi-check-circle'"></v-icon>
                  {{ item.blocked ? 'Blocked' : 'Active' }}
                </v-chip>
              </template>

              <!-- Actions -->
              <template v-slot:item.actions="{ item }">
                <div class="d-flex gap-2">
                  <v-btn
                    v-if="!item.blocked"
                    color="error"
                    variant="outlined"
                    size="small"
                    prepend-icon="mdi-block-helper"
                    @click="confirmBlockUser(item)"
                    :disabled="item.role === 'Administrator'"
                  >
                    Block
                  </v-btn>
                  <v-btn
                    v-else
                    color="success"
                    variant="outlined"
                    size="small"
                    prepend-icon="mdi-check-circle"
                    @click="confirmUnblockUser(item)"
                  >
                    Unblock
                  </v-btn>
                </div>
              </template>

              <!-- No data placeholder -->
              <template v-slot:no-data>
                <div class="text-center pa-6">
                  <v-icon icon="mdi-account-off" size="64" color="grey-lighten-2"></v-icon>
                  <p class="text-h6 mt-4">No users found</p>
                </div>
              </template>
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Confirmation Dialogs -->
    <v-dialog v-model="blockDialog" max-width="400">
      <v-card>
        <v-card-title class="text-h6 text-error">
          <v-icon icon="mdi-alert" class="mr-2"></v-icon>
          Block User
        </v-card-title>
        <v-card-text>
          Are you sure you want to block <strong>{{ selectedUser?.username }}</strong>?
          This will prevent them from accessing the system.
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn variant="text" @click="blockDialog = false">Cancel</v-btn>
          <v-btn color="error" @click="blockUser" :loading="actionLoading">Block</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="unblockDialog" max-width="400">
      <v-card>
        <v-card-title class="text-h6 text-success">
          <v-icon icon="mdi-check-circle" class="mr-2"></v-icon>
          Unblock User
        </v-card-title>
        <v-card-text>
          Are you sure you want to unblock <strong>{{ selectedUser?.username }}</strong>?
          This will restore their access to the system.
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn variant="text" @click="unblockDialog = false">Cancel</v-btn>
          <v-btn color="success" @click="unblockUser" :loading="actionLoading">Unblock</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script>
import axios from '@/utils/axiosInstance';
import { store } from '@/utils/store';

export default {
  name: 'AdminUsersView',
  computed: {
    store() {
      return store;
    }
  },
  data() {
    return {
      users: [],
      loading: false,
      actionLoading: false,
      errorMessage: '',
      successSnackbar: false,
      successMessage: '',
      blockDialog: false,
      unblockDialog: false,
      selectedUser: null,
      headers: [
        {
          title: 'User',
          align: 'start',
          sortable: true,
          key: 'username',
          width: '25%'
        },
        { title: 'Email', key: 'email', sortable: true, width: '25%' },
        { title: 'Role', key: 'role', sortable: true, width: '15%' },
        { title: 'Status', key: 'blocked', sortable: true, width: '15%' },
        { title: 'Actions', key: 'actions', sortable: false, width: '20%' }
      ]
    };
  },
  methods: {
    async fetchUsers() {
      this.loading = true;
      this.errorMessage = '';

      try {
        const response = await axios.get('/stakeholders/administrator/account');
        console.log('API Response:', response.data); // Debug log
        
        // Handle PagedResult structure - data is in Results property
        this.users = response.data?.results || response.data?.Results || [];
        console.log('Users loaded:', this.users); // Debug log
      } catch (error) {
        console.error('Error fetching users:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to load users. Please try again.';
      } finally {
        this.loading = false;
      }
    },

    getRoleColor(role) {
      switch (role) {
        case 'Administrator': return 'purple';
        case 'Author': return 'primary';
        case 'Tourist': return 'green';
        default: return 'grey';
      }
    },

    confirmBlockUser(user) {
      this.selectedUser = user;
      this.blockDialog = true;
    },

    confirmUnblockUser(user) {
      this.selectedUser = user;
      this.unblockDialog = true;
    },

    async blockUser() {
      if (!this.selectedUser) return;

      this.actionLoading = true;
      try {
        await axios.put(`/stakeholders/administrator/account/${this.selectedUser.id}/block`);
        
        // Update local user data
        const userIndex = this.users.findIndex(u => u.id === this.selectedUser.id);
        if (userIndex !== -1) {
          this.users[userIndex].blocked = true;
        }

        this.successMessage = `User ${this.selectedUser.username} has been blocked successfully.`;
        this.successSnackbar = true;
        this.blockDialog = false;
        this.selectedUser = null;
      } catch (error) {
        console.error('Error blocking user:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to block user. Please try again.';
      } finally {
        this.actionLoading = false;
      }
    },

    async unblockUser() {
      if (!this.selectedUser) return;

      this.actionLoading = true;
      try {
        await axios.put(`/stakeholders/administrator/account/${this.selectedUser.id}/unblock`);
        
        // Update local user data
        const userIndex = this.users.findIndex(u => u.id === this.selectedUser.id);
        if (userIndex !== -1) {
          this.users[userIndex].blocked = false;
        }

        this.successMessage = `User ${this.selectedUser.username} has been unblocked successfully.`;
        this.successSnackbar = true;
        this.unblockDialog = false;
        this.selectedUser = null;
      } catch (error) {
        console.error('Error unblocking user:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to unblock user. Please try again.';
      } finally {
        this.actionLoading = false;
      }
    }
  },

  mounted() {
    // Check if user is admin before loading data
    if (this.store.role !== 'Administrator') {
      this.$router.push('/');
      return;
    }
    
    this.fetchUsers();
  }
};
</script>

<style scoped>
.admin-users-container {
  min-height: 80vh;
  padding-top: 40px;
  padding-bottom: 40px;
  background: linear-gradient(135deg, #F3E5F5 0%, #E8F5E8 100%);
}

.users-card {
  border-radius: 16px;
  transition: all 0.3s ease;
  overflow: hidden;
}

.users-card:hover {
  box-shadow: 0 8px 24px rgba(0,0,0,0.12) !important;
}

/* Table styling improvements */
:deep(.v-data-table) {
  background-color: transparent;
}

:deep(.v-data-table-header__content) {
  font-weight: 600;
  color: #424242;
}

:deep(.v-data-table__td) {
  border-bottom: 1px solid #E0E0E0;
}

/* Responsive improvements */
@media (max-width: 960px) {
  .admin-users-container {
    padding-top: 20px;
    padding-left: 16px;
    padding-right: 16px;
  }
  
  .text-h3 {
    font-size: 2rem !important;
  }
}

@media (max-width: 600px) {
  .admin-users-container {
    padding-top: 16px;
  }
  
  .text-h3 {
    font-size: 1.75rem !important;
  }
}
</style>