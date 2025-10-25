<template>
  <v-container class="cart-container" max-width="800px">
    <v-card class="cart-card" elevation="3">
      <v-card-title class="cart-title">
        <v-icon class="mr-3" color="primary">mdi-cart</v-icon>
        My Shopping Cart
      </v-card-title>

      <!-- Loading state -->
      <v-card-text v-if="loading" class="text-center py-8">
        <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
        <div class="mt-4 text-h6">Loading your cart...</div>
      </v-card-text>

      <!-- Error state -->
      <v-card-text v-else-if="error" class="text-center py-8">
        <v-icon color="error" size="64" class="mb-4">mdi-alert-circle</v-icon>
        <div class="text-h6 error--text mb-2">Error loading cart</div>
        <div class="text-body-1 mb-4">{{ error }}</div>
        <v-btn color="primary" @click="loadCart">
          <v-icon class="mr-2">mdi-refresh</v-icon>
          Try Again
        </v-btn>
      </v-card-text>

      <!-- Empty cart state -->
      <v-card-text v-else-if="!cart || !cart.orderItems || cart.orderItems.length === 0" class="text-center py-8">
        <v-icon color="grey" size="64" class="mb-4">mdi-cart-outline</v-icon>
        <div class="text-h6 mb-2">Your cart is empty</div>
        <div class="text-body-1 mb-4 grey--text">Start exploring tours and add some to your cart!</div>
        <v-btn color="primary" to="/tours/public" variant="outlined">
          <v-icon class="mr-2">mdi-map-marker-multiple</v-icon>
          Browse Tours
        </v-btn>
      </v-card-text>

      <!-- Cart with items -->
      <div v-else>
        <!-- Cart Summary -->
        <v-card-text class="pb-2">
          <v-row class="cart-summary">
            <v-col cols="12" md="6">
              <div class="text-subtitle-1">
                <v-icon class="mr-2" color="primary">mdi-package-variant</v-icon>
                {{ cart.itemCount }} {{ cart.itemCount === 1 ? 'item' : 'items' }} in cart
              </div>
            </v-col>
            <v-col cols="12" md="6" class="text-md-right">
              <div class="text-h6 primary--text">
                Total: ${{ cart.totalPrice.toFixed(2) }}
              </div>
            </v-col>
          </v-row>
        </v-card-text>

        <v-divider></v-divider>

        <!-- Cart Items List -->
        <v-list class="cart-items-list">
          <v-list-item
            v-for="(item, index) in cart.orderItems"
            :key="item.id"
            class="cart-item"
          >
            <template v-slot:prepend>
              <v-avatar color="primary" class="tour-avatar">
                <v-icon color="white">mdi-map-marker</v-icon>
              </v-avatar>
            </template>

            <v-list-item-title class="tour-name">
              {{ item.tourName }}
            </v-list-item-title>

            <v-list-item-subtitle class="tour-details">
              <div class="tour-price">${{ item.tourPrice.toFixed(2) }}</div>
              <div class="tour-id text-caption grey--text">Tour ID: {{ item.tourId }}</div>
              <div class="added-date text-caption grey--text">
                Added: {{ formatDate(item.createdAt) }}
              </div>
            </v-list-item-subtitle>

            <template v-slot:append>
              <v-btn
                icon
                size="small"
                color="error"
                variant="outlined"
                @click="removeItem(item.id)"
                :loading="removingItems.includes(item.id)"
              >
                <v-icon>mdi-delete</v-icon>
              </v-btn>
            </template>

            <v-divider v-if="index < cart.orderItems.length - 1" class="mt-3"></v-divider>
          </v-list-item>
        </v-list>

        <v-divider></v-divider>

        <!-- Cart Actions -->
        <v-card-actions class="cart-actions pa-4">
          <v-btn
            color="error"
            variant="outlined"
            @click="clearCart"
            :loading="clearingCart"
            class="mr-3"
          >
            <v-icon class="mr-2">mdi-cart-remove</v-icon>
            Clear Cart
          </v-btn>

          <v-spacer></v-spacer>

          <v-btn
            color="success"
            size="large"
            variant="elevated"
            class="purchase-btn"
            @click="proceedToPurchase"
            :disabled="!cart || !cart.orderItems || cart.orderItems.length === 0"
          >
            <v-icon class="mr-2">mdi-credit-card</v-icon>
            Purchase (${{ cart.totalPrice.toFixed(2) }})
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
  name: 'MyCartView',
  data() {
    return {
      cart: null,
      loading: false,
      error: null,
      removingItems: [],
      clearingCart: false,
      snackbar: {
        show: false,
        message: '',
        color: 'success'
      }
    };
  },
  async created() {
    await this.loadCart();
  },
  methods: {
    async loadCart() {
      this.loading = true;
      this.error = null;
      
      try {
        const response = await axiosInstance.get('/purchase/cart');
        this.cart = response.data;
      } catch (error) {
        console.error('Error loading cart:', error);
        if (error.response && error.response.status === 404) {
          // No active cart found - this is normal for new users
          this.cart = {
            orderItems: [],
            itemCount: 0,
            totalPrice: 0
          };
        } else {
          this.error = error.response?.data?.message || 'Failed to load cart';
        }
      } finally {
        this.loading = false;
      }
    },

    async removeItem(itemId) {
      if (this.removingItems.includes(itemId)) return;
      
      this.removingItems.push(itemId);
      
      try {
        await axiosInstance.delete(`/purchase/cart/item/${itemId}`);
        
        // Remove item from local cart data
        if (this.cart && this.cart.orderItems) {
          const itemIndex = this.cart.orderItems.findIndex(item => item.id === itemId);
          if (itemIndex > -1) {
            const removedItem = this.cart.orderItems[itemIndex];
            this.cart.orderItems.splice(itemIndex, 1);
            this.cart.itemCount--;
            this.cart.totalPrice -= removedItem.tourPrice;
          }
        }
        
        this.showSnackbar('Item removed from cart', 'success');
      } catch (error) {
        console.error('Error removing item:', error);
        this.showSnackbar('Failed to remove item', 'error');
      } finally {
        this.removingItems = this.removingItems.filter(id => id !== itemId);
      }
    },

    async clearCart() {
      if (this.clearingCart) return;
      
      this.clearingCart = true;
      
      try {
        await axiosInstance.delete('/purchase/cart/clear');
        
        // Clear local cart data
        this.cart = {
          orderItems: [],
          itemCount: 0,
          totalPrice: 0
        };
        
        this.showSnackbar('Cart cleared successfully', 'success');
      } catch (error) {
        console.error('Error clearing cart:', error);
        this.showSnackbar('Failed to clear cart', 'error');
      } finally {
        this.clearingCart = false;
      }
    },

    proceedToPurchase() {
      // TODO: Implement purchase logic later
      this.showSnackbar('Purchase functionality coming soon!', 'info');
    },

    formatDate(dateString) {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
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
.cart-container {
  padding-top: 20px;
}

.cart-card {
  border-radius: 12px;
  overflow: hidden;
}

.cart-title {
  background: linear-gradient(135deg, #1976d2, #1565c0);
  color: white;
  font-size: 1.5rem;
  font-weight: bold;
  padding: 20px 24px;
}

.cart-summary {
  background-color: #f8f9fa;
  margin: 0;
  padding: 16px;
  border-radius: 8px;
}

.cart-items-list {
  padding: 0;
}

.cart-item {
  padding: 16px 24px;
  transition: background-color 0.2s ease;
}

.cart-item:hover {
  background-color: #f8f9fa;
}

.tour-avatar {
  margin-right: 16px;
}

.tour-name {
  font-weight: 600;
  font-size: 1.1rem;
  color: #1976d2;
  margin-bottom: 8px;
}

.tour-details {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.tour-price {
  font-size: 1.2rem;
  font-weight: bold;
  color: #2e7d32;
}

.cart-actions {
  background-color: #f8f9fa;
  border-top: 1px solid #e0e0e0;
}

.purchase-btn {
  font-size: 1.1rem;
  font-weight: bold;
  padding: 12px 24px;
  border-radius: 8px;
}

.purchase-btn:not(:disabled) {
  background: linear-gradient(135deg, #2e7d32, #388e3c) !important;
  color: white !important;
  box-shadow: 0 4px 12px rgba(46, 125, 50, 0.3);
}

.purchase-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, #388e3c, #43a047) !important;
  transform: translateY(-1px);
  box-shadow: 0 6px 16px rgba(46, 125, 50, 0.4);
}

@media (max-width: 600px) {
  .cart-actions {
    flex-direction: column-reverse;
    gap: 12px;
  }
  
  .cart-actions .v-btn {
    width: 100%;
  }
  
  .cart-summary {
    text-align: center;
  }
}
</style>