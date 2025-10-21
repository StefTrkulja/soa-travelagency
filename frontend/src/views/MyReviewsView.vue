<template>
  <v-container class="reviews-container">
    <v-row>
      <v-col cols="12">
        <div class="d-flex justify-space-between align-center mb-6">
          <div>
            <h1 class="text-h4 font-weight-bold text-primary mb-2">My Tour Reviews</h1>
            <p class="text-body-1 text-grey-darken-1">All the reviews you've written for tours</p>
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
        <p class="mt-4 text-h6">Loading your reviews...</p>
      </v-col>
    </v-row>

    <!-- Empty State -->
    <v-row v-if="!loading && reviews.length === 0 && !errorMessage">
      <v-col cols="12" class="text-center">
        <v-icon size="120" color="grey-lighten-1" class="mb-4">mdi-star-off</v-icon>
        <h2 class="text-h5 text-grey-darken-1 mb-2">No reviews yet</h2>
        <p class="text-body-1 text-grey-darken-1 mb-4">You haven't written any reviews for tours yet.</p>
        <v-btn 
          color="primary" 
          prepend-icon="mdi-map-marker-multiple" 
          to="/tours/public"
          variant="elevated"
        >
          Explore Tours
        </v-btn>
      </v-col>
    </v-row>

    <!-- Reviews List -->
    <v-row v-if="!loading && reviews.length > 0">
      <v-col cols="12">
        <v-card class="reviews-list">
          <v-card-title class="text-h6 pa-4 pb-2">
            <v-icon class="mr-2" color="primary">mdi-format-list-bulleted</v-icon>
            Your Reviews ({{ reviews.length }})
          </v-card-title>
          
          <v-divider></v-divider>

          <v-list lines="three">
            <template v-for="(review, index) in reviews" :key="review.id">
              <v-list-item class="review-item pa-4">
                <template v-slot:prepend>
                  <v-avatar size="48" color="primary" class="mr-4">
                    <v-icon color="white" size="24">mdi-star</v-icon>
                  </v-avatar>
                </template>

                <v-list-item-title class="text-h6 font-weight-bold mb-2">
                  {{ review.tourName || `Tour ID: ${review.tourId}` }}
                </v-list-item-title>

                <v-list-item-subtitle class="mb-3">
                  <v-row no-gutters class="align-center">
                    <v-col cols="auto" class="mr-4" v-if="review.rating">
                      <v-chip size="small" color="warning" variant="elevated">
                        <v-icon start size="16">mdi-star</v-icon>
                        {{ review.rating }}/5
                      </v-chip>
                    </v-col>
                    <v-col cols="auto" class="mr-4">
                      <v-chip size="small" color="info" variant="outlined">
                        <v-icon start size="16">mdi-calendar-check</v-icon>
                        Visited: {{ formatDate(review.visitationTime) }}
                      </v-chip>
                    </v-col>
                    <v-col cols="auto">
                      <v-chip size="small" color="grey" variant="outlined">
                        <v-icon start size="16">mdi-clock</v-icon>
                        Reviewed: {{ formatDate(review.createdAt) }}
                      </v-chip>
                    </v-col>
                  </v-row>
                </v-list-item-subtitle>

                <div class="review-content">
                  <v-card variant="tonal" color="grey-lighten-4" class="pa-3 mb-3">
                    <p class="text-body-1 mb-0">{{ review.comment }}</p>
                  </v-card>

                  <div v-if="review.imageUrl" class="mb-3">
                    <v-img
                      :src="review.imageUrl"
                      max-height="200"
                      max-width="300"
                      class="rounded-lg"
                      cover
                    >
                      <template v-slot:error>
                        <v-card color="grey-lighten-2" height="100" class="d-flex align-center justify-center">
                          <v-icon color="grey-darken-1">mdi-image-broken</v-icon>
                        </v-card>
                      </template>
                    </v-img>
                  </div>

                  <v-row no-gutters class="align-center">
                    <v-col cols="auto">
                      <v-btn
                        size="small"
                        color="primary"
                        variant="outlined"
                        prepend-icon="mdi-pencil"
                        @click="editReview(review)"
                        class="mr-2"
                      >
                        Edit
                      </v-btn>
                      <v-btn
                        size="small"
                        color="error"
                        variant="outlined"
                        prepend-icon="mdi-delete"
                        @click="confirmDeleteReview(review)"
                      >
                        Delete
                      </v-btn>
                    </v-col>
                    <v-spacer></v-spacer>
                    <v-col cols="auto">
                      <p class="text-caption text-grey-darken-1 mb-0">
                        <span v-if="review.updatedAt">Last updated: {{ formatDate(review.updatedAt) }}</span>
                      </p>
                    </v-col>
                  </v-row>
                </div>
              </v-list-item>

              <v-divider v-if="index < reviews.length - 1" class="mx-4"></v-divider>
            </template>
          </v-list>
        </v-card>
      </v-col>
    </v-row>

    <!-- Edit Review Dialog -->
    <v-dialog v-model="editDialog" max-width="600px" persistent>
      <v-card>
        <v-card-title class="text-h5 pa-6 pb-4">
          <v-icon class="mr-3" color="primary">mdi-pencil</v-icon>
          Edit Review
        </v-card-title>

        <v-card-text class="pa-6 pt-0">
          <v-form ref="editForm" v-model="editFormValid">
            <v-row>
              <v-col cols="12">
                <v-textarea
                  v-model="editForm.comment"
                  label="Your Review Comment"
                  rows="4"
                  counter="2000"
                  maxlength="2000"
                  :rules="[v => !!v || 'Review comment is required', v => v.length <= 2000 || 'Comment must be less than 2000 characters']"
                  required
                  variant="outlined"
                ></v-textarea>
              </v-col>

              <v-col cols="12" md="6">
                <v-select
                  v-model="editForm.rating"
                  label="Rating (Optional)"
                  :items="ratingOptions"
                  item-title="text"
                  item-value="value"
                  variant="outlined"
                  clearable
                >
                  <template v-slot:selection="{ item }">
                    <v-chip color="warning" size="small">
                      <v-icon start>mdi-star</v-icon>
                      {{ item.raw.text }}
                    </v-chip>
                  </template>
                </v-select>
              </v-col>

              <v-col cols="12" md="6">
                <v-text-field
                  v-model="editForm.imageUrl"
                  label="Image URL (Optional)"
                  placeholder="https://example.com/your-image.jpg"
                  variant="outlined"
                ></v-text-field>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>

        <v-card-actions class="pa-6 pt-0">
          <v-spacer></v-spacer>
          <v-btn
            variant="outlined"
            @click="closeEditDialog"
            :disabled="updatingReview"
          >
            Cancel
          </v-btn>
          <v-btn
            color="primary"
            variant="elevated"
            @click="updateReview"
            :loading="updatingReview"
            :disabled="!editFormValid"
          >
            Update Review
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Delete Confirmation Dialog -->
    <v-dialog v-model="deleteDialog" max-width="400px">
      <v-card>
        <v-card-title class="text-h5 pa-6 pb-4">
          <v-icon class="mr-3" color="error">mdi-delete</v-icon>
          Delete Review
        </v-card-title>

        <v-card-text class="pa-6 pt-0">
          Are you sure you want to delete this review? This action cannot be undone.
        </v-card-text>

        <v-card-actions class="pa-6 pt-0">
          <v-spacer></v-spacer>
          <v-btn
            variant="outlined"
            @click="deleteDialog = false"
            :disabled="deletingReview"
          >
            Cancel
          </v-btn>
          <v-btn
            color="error"
            variant="elevated"
            @click="deleteReview"
            :loading="deletingReview"
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
  name: 'MyReviewsView',
  computed: {
    store() {
      return store;
    }
  },
  data() {
    return {
      reviews: [],
      loading: false,
      errorMessage: '',
      successMessage: '',
      // Edit dialog
      editDialog: false,
      editFormValid: false,
      updatingReview: false,
      selectedReview: null,
      editForm: {
        comment: '',
        rating: null,
        imageUrl: ''
      },
      // Delete dialog
      deleteDialog: false,
      deletingReview: false,
      reviewToDelete: null,
      ratingOptions: [
        { text: '1 Star - Poor', value: 1 },
        { text: '2 Stars - Fair', value: 2 },
        { text: '3 Stars - Good', value: 3 },
        { text: '4 Stars - Very Good', value: 4 },
        { text: '5 Stars - Excellent', value: 5 }
      ]
    };
  },
  methods: {
    async fetchMyReviews() {
      this.loading = true;
      this.errorMessage = '';

      try {
        const response = await axios.get('/tours/reviews/my');
        this.reviews = response.data;
      } catch (error) {
        console.error('Error fetching reviews:', error);
        this.errorMessage = error.response?.data?.message || error.message || 'Failed to load your reviews.';
      } finally {
        this.loading = false;
      }
    },
    formatDate(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false
      });
    },
    editReview(review) {
      this.selectedReview = review;
      this.editForm = {
        comment: review.comment,
        rating: review.rating,
        imageUrl: review.imageUrl || ''
      };
      this.editDialog = true;
    },
    closeEditDialog() {
      this.editDialog = false;
      this.selectedReview = null;
      this.editFormValid = false;
    },
    async updateReview() {
      if (!this.$refs.editForm.validate()) {
        return;
      }

      this.updatingReview = true;
      try {
        const updateData = {
          comment: this.editForm.comment,
          rating: this.editForm.rating,
          imageUrl: this.editForm.imageUrl || null
        };

        await axios.put(`/tours/reviews/${this.selectedReview.id}`, updateData);
        
        this.successMessage = 'Review updated successfully!';
        this.closeEditDialog();
        
        // Refresh reviews
        await this.fetchMyReviews();
        
      } catch (error) {
        console.error('Error updating review:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to update review. Please try again.';
      } finally {
        this.updatingReview = false;
      }
    },
    confirmDeleteReview(review) {
      this.reviewToDelete = review;
      this.deleteDialog = true;
    },
    async deleteReview() {
      this.deletingReview = true;
      try {
        await axios.delete(`/tours/reviews/${this.reviewToDelete.id}`);
        
        this.successMessage = 'Review deleted successfully!';
        this.deleteDialog = false;
        this.reviewToDelete = null;
        
        // Refresh reviews
        await this.fetchMyReviews();
        
      } catch (error) {
        console.error('Error deleting review:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to delete review. Please try again.';
      } finally {
        this.deletingReview = false;
      }
    }
  },
  mounted() {
    // Only fetch reviews if user is a tourist
    if (this.store.role === 'Tourist') {
      this.fetchMyReviews();
    } else {
      this.$router.push('/');
    }
  }
};
</script>

<style scoped>
.reviews-container {
  max-width: 1200px;
  padding-top: 32px;
}

.reviews-list {
  border-radius: 16px;
}

.review-item {
  border-radius: 8px;
  transition: background-color 0.2s ease;
}

.review-item:hover {
  background-color: rgba(0, 0, 0, 0.02);
}

.review-content {
  margin-left: 64px;
}

@media (max-width: 960px) {
  .reviews-container {
    padding-top: 20px;
  }
  
  .review-content {
    margin-left: 0;
    margin-top: 16px;
  }
}
</style>