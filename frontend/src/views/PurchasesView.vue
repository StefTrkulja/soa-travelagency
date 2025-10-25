<template>
  <v-container class="purchases-container" max-width="1000px">
    <v-card class="purchases-card" elevation="3">
      <v-card-title class="purchases-title">
        <v-icon class="mr-3" color="primary">mdi-receipt</v-icon>
        My Purchases
      </v-card-title>

      <!-- Loading state -->
      <v-card-text v-if="loading" class="text-center py-8">
        <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
        <div class="mt-4 text-h6">Loading your purchases...</div>
      </v-card-text>

      <!-- Error state -->
      <v-card-text v-else-if="error" class="text-center py-8">
        <v-icon color="error" size="64" class="mb-4">mdi-alert-circle</v-icon>
        <div class="text-h6 error--text mb-2">Error loading purchases</div>
        <div class="text-body-1 mb-4">{{ error }}</div>
        <v-btn color="primary" @click="loadPurchases">
          <v-icon class="mr-2">mdi-refresh</v-icon>
          Try Again
        </v-btn>
      </v-card-text>

      <!-- No purchases state -->
      <v-card-text v-else-if="!purchases || purchases.length === 0" class="text-center py-8">
        <v-icon color="grey" size="64" class="mb-4">mdi-receipt-outline</v-icon>
        <div class="text-h6 mb-2">No purchases yet</div>
        <div class="text-body-1 mb-4 grey--text">Start exploring tours and make your first purchase!</div>
        <v-btn color="primary" to="/tours/public" variant="outlined">
          <v-icon class="mr-2">mdi-map-marker-multiple</v-icon>
          Browse Tours
        </v-btn>
      </v-card-text>

      <!-- Purchases List -->
      <div v-else>
        <!-- Purchases Summary -->
        <v-card-text class="pb-2">
          <v-row class="purchases-summary">
            <v-col cols="12" md="6">
              <div class="text-subtitle-1">
                <v-icon class="mr-2" color="primary">mdi-package-variant</v-icon>
                {{ purchases.length }} {{ purchases.length === 1 ? 'purchase' : 'purchases' }} total
              </div>
            </v-col>
            <v-col cols="12" md="6" class="text-md-right">
              <div class="text-h6 primary--text">
                Total Spent: ${{ totalSpent.toFixed(2) }}
              </div>
            </v-col>
          </v-row>
        </v-card-text>

        <v-divider></v-divider>

        <!-- Purchases Grid -->
        <div class="purchases-grid pa-4">
          <v-row>
            <v-col
              v-for="purchase in purchases"
              :key="purchase.id"
              cols="12"
              md="6"
              lg="4"
            >
              <v-card class="purchase-card" elevation="2" hover>
                <v-card-title class="purchase-card-title">
                  <v-icon class="mr-2" color="success">mdi-check-circle</v-icon>
                  {{ purchase.tourName }}
                </v-card-title>

                <v-card-text>
                  <div class="purchase-details">
                    <div class="detail-row">
                      <strong>Purchase Date:</strong>
                      <span>{{ formatDate(purchase.purchaseDate) }}</span>
                    </div>
                    <div class="detail-row">
                      <strong>Price Paid:</strong>
                      <span class="price-tag">${{ purchase.tourPrice.toFixed(2) }}</span>
                    </div>
                    <div class="detail-row">
                      <strong>Tour ID:</strong>
                      <span class="tour-id">{{ purchase.tourId }}</span>
                    </div>
                    <div class="detail-row">
                      <strong>Purchase ID:</strong>
                      <span class="purchase-id">{{ purchase.id }}</span>
                    </div>
                  </div>
                </v-card-text>

                <v-card-actions>
                  <v-btn
                    color="primary"
                    variant="outlined"
                    size="small"
                    @click="viewTourDetails(purchase.tourId)"
                  >
                    <v-icon class="mr-1" size="small">mdi-eye</v-icon>
                    View Tour
                  </v-btn>
                  <v-spacer></v-spacer>
                  <v-chip
                    color="success"
                    size="small"
                    variant="flat"
                  >
                    <v-icon start size="small">mdi-check</v-icon>
                    Purchased
                  </v-chip>
                </v-card-actions>
              </v-card>
            </v-col>
          </v-row>
        </div>

        <!-- Back to Shopping Button -->
        <v-card-actions class="pa-4 bg-grey-lighten-5">
          <v-btn
            color="primary"
            to="/tours/public"
            variant="elevated"
            size="large"
          >
            <v-icon class="mr-2">mdi-shopping</v-icon>
            Continue Shopping
          </v-btn>
          <v-spacer></v-spacer>
          <v-btn
            color="grey"
            to="/cart"
            variant="outlined"
          >
            <v-icon class="mr-2">mdi-cart</v-icon>
            View Cart
          </v-btn>
        </v-card-actions>
      </div>
    </v-card>

    <!-- Success Snackbar -->
    <v-snackbar
      v-model="snackbar.show"
      :color="snackbar.color"
      :timeout="4000"
      location="top"
    >
      {{ snackbar.message }}
      <template v-slot:actions>
        <v-btn variant="text" @click="snackbar.show = false">
          Close
        </v-btn>
      </template>
    </v-snackbar>
  </v-container>
</template>

<script>
import axiosInstance from '@/utils/axiosInstance';

export default {
  name: 'PurchasesView',
  data() {
    return {
      purchases: [],
      loading: false,
      error: null,
      snackbar: {
        show: false,
        message: '',
        color: 'success'
      }
    };
  },
  computed: {
    totalSpent() {
      if (!this.purchases || this.purchases.length === 0) return 0;
      return this.purchases.reduce((total, purchase) => total + purchase.tourPrice, 0);
    }
  },
  async created() {
    await this.loadPurchases();
  },
  methods: {
    async loadPurchases() {
      this.loading = true;
      this.error = null;
      
      try {
        const response = await axiosInstance.get('/purchases/my');
        this.purchases = response.data || [];
        
        // Sort purchases by date (most recent first)
        this.purchases.sort((a, b) => new Date(b.purchaseDate) - new Date(a.purchaseDate));
        
      } catch (error) {
        console.error('Error loading purchases:', error);
        if (error.response && error.response.status === 404) {
          // No purchases found - this is normal for new users
          this.purchases = [];
        } else {
          this.error = error.response?.data?.message || 'Failed to load purchases';
        }
      } finally {
        this.loading = false;
      }
    },

    viewTourDetails(tourId) {
      // Navigate to tour details page
      this.$router.push(`/tours/${tourId}`);
    },

    formatDate(dateString) {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
      });
    },

    showSnackbar(message, color = 'success') {
      this.snackbar = {
        show: true,
        message,
        color
      };
    }
  }
};
</script>

<style scoped>
.purchases-container {
  padding-top: 20px;
}

.purchases-card {
  border-radius: 12px;
  overflow: hidden;
}

.purchases-title {
  background: linear-gradient(135deg, #1976d2, #1565c0);
  color: white;
  font-size: 1.5rem;
  font-weight: bold;
  padding: 20px 24px;
}

.purchases-summary {
  background-color: #f8f9fa;
  margin: 0;
  padding: 16px;
  border-radius: 8px;
}

.purchases-grid {
  min-height: 200px;
}

.purchase-card {
  height: 100%;
  display: flex;
  flex-direction: column;
  border-radius: 10px;
  transition: all 0.3s ease;
  border: 1px solid #e0e0e0;
}

.purchase-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.15) !important;
  border-color: #1976d2;
}

.purchase-card-title {
  background: linear-gradient(135deg, #e8f5e8, #f1f8e9);
  color: #2e7d32;
  font-size: 1.1rem;
  font-weight: 600;
  padding: 16px;
  border-bottom: 1px solid #e0e0e0;
}

.purchase-details {
  flex-grow: 1;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
  padding: 4px 0;
}

.detail-row:last-child {
  margin-bottom: 0;
}

.price-tag {
  font-weight: bold;
  color: #2e7d32;
  font-size: 1.1rem;
}

.tour-id, .purchase-id {
  font-family: monospace;
  background-color: #f5f5f5;
  padding: 2px 6px;
  border-radius: 4px;
  font-size: 0.9rem;
  color: #666;
}

.bg-grey-lighten-5 {
  background-color: #f8f9fa !important;
  border-top: 1px solid #e0e0e0;
}

@media (max-width: 600px) {
  .purchases-summary {
    text-align: center;
  }
  
  .detail-row {
    flex-direction: column;
    align-items: flex-start;
    gap: 2px;
  }
  
  .purchase-card-title {
    font-size: 1rem;
    padding: 12px;
  }
}
</style>