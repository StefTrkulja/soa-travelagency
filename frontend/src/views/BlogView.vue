<template>
  <v-container class="blogs-container">
    <!-- Header Section -->
    <v-row class="mb-6">
      <v-col cols="12">
        <div class="d-flex justify-space-between align-center">
          <div>
            <h1 class="text-h3 font-weight-bold text-primary mb-2">Blog Posts</h1>
            <p class="text-subtitle-1 text-grey-darken-1">Discover amazing travel stories and experiences</p>
          </div>
          <v-btn
            v-if="store.role === 'Author'"
            size="large"
            color="primary"
            prepend-icon="mdi-plus"
            @click="$router.push('/create-blog')"
            class="create-blog-btn"
          >
            Create New Blog
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
        <p class="mt-4 text-h6">Loading blogs...</p>
      </v-col>
    </v-row>

    <!-- Error State -->
    <v-alert v-if="errorMessage" type="error" class="mb-6" dismissible @click:close="errorMessage = ''">
      {{ errorMessage }}
    </v-alert>

    <!-- Empty State -->
    <v-row v-if="!loading && blogs.length === 0 && !errorMessage" class="mt-8">
      <v-col cols="12">
        <v-card class="empty-state-card" elevation="0">
          <v-card-text class="text-center pa-12">
            <v-icon icon="mdi-blog" size="120" color="grey-lighten-1" class="mb-4"></v-icon>
            <h2 class="text-h4 mb-3">No Blog Posts Yet</h2>
            <p class="text-body-1 text-grey-darken-1 mb-6">Be the first to share your travel experiences!</p>
            <v-btn
              v-if="store.role === 'Author'"
              size="large"
              color="primary"
              prepend-icon="mdi-plus"
              @click="$router.push('/create-blog')"
            >
              Create Your First Blog
            </v-btn>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Blogs Grid -->
    <v-row v-if="!loading && blogs.length > 0">
      <v-col
        v-for="blog in blogs"
        :key="blog.id"
        cols="12"
        md="6"
        lg="4"
      >
        <v-card
          class="blog-card animate-scale-up"
          elevation="4"
          hover
          @click="viewBlogDetails(blog.id)"
        >
          <!-- Blog Image -->
          <div v-if="blog.imageUrls && blog.imageUrls.length > 0" class="blog-image-container">
            <v-img
              :src="blog.imageUrls[0]"
              :alt="blog.title"
              height="200"
              cover
              class="blog-image"
            ></v-img>
            <div class="blog-image-overlay">
              <v-chip
                color="primary"
                size="small"
                variant="flat"
                class="blog-date-chip"
              >
                {{ formatDate(blog.createdAt) }}
              </v-chip>
            </div>
          </div>
          <div v-else class="blog-image-placeholder">
            <v-icon icon="mdi-image" size="48" color="grey-lighten-2"></v-icon>
            <v-chip
              color="primary"
              size="small"
              variant="flat"
              class="blog-date-chip-placeholder"
            >
              {{ formatDate(blog.createdAt) }}
            </v-chip>
          </div>

          <v-card-title class="text-h5 font-weight-bold pa-4 pb-2">
            {{ blog.title }}
          </v-card-title>

          <v-card-text class="pa-4 pt-0">
            <div 
              class="blog-description text-body-2 mb-3"
              v-html="truncateHtml(blog.descriptionHtml, 150)"
            ></div>

            <!-- Meta Info -->
            <v-divider class="my-3"></v-divider>
            <div class="d-flex justify-space-between text-caption text-grey-darken-1">
              <div>
                <v-icon icon="mdi-calendar" size="small" class="mr-1"></v-icon>
                {{ formatDate(blog.createdAt) }}
              </div>
              <div v-if="blog.imageUrls && blog.imageUrls.length > 0">
                <v-icon icon="mdi-image-multiple" size="small" class="mr-1"></v-icon>
                {{ blog.imageUrls.length }} image{{ blog.imageUrls.length !== 1 ? 's' : '' }}
              </div>
            </div>
          </v-card-text>

          <v-card-actions class="pa-4 pt-0">
            <v-btn
              variant="text"
              color="primary"
              prepend-icon="mdi-eye"
              @click.stop="viewBlogDetails(blog.id)"
            >
              Read More
            </v-btn>
            <v-btn
              v-if="canCommentOnBlog(blog)"
              variant="text"
              color="success"
              prepend-icon="mdi-comment-plus"
              @click.stop="addComment(blog.id)"
            >
              Add Comment
            </v-btn>
            <v-spacer></v-spacer>
            <v-btn
              v-if="store.role === 'Author' && isMyBlog(blog)"
              icon="mdi-pencil"
              size="small"
              variant="text"
              color="primary"
              @click.stop="editBlog(blog.id)"
            ></v-btn>
            <v-btn
              v-if="store.role === 'Author' && isMyBlog(blog)"
              icon="mdi-delete"
              size="small"
              variant="text"
              color="error"
              @click.stop="confirmDelete(blog)"
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
          <p class="text-body-1">Are you sure you want to delete this blog post?</p>
          <p class="text-h6 font-weight-bold mt-3">{{ blogToDelete?.title }}</p>
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
            :loading="deletingBlog"
            @click="deleteBlog"
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
      blogs: [],
      loading: false,
      errorMessage: '',
      deleteDialog: false,
      blogToDelete: null,
      deletingBlog: false,
      followingUsers: new Set(),
      currentUserId: null
    };
  },
  methods: {
    async fetchBlogs() {
      this.loading = true;
      this.errorMessage = '';

      try {
        const response = await axios.get('/blogs');
        this.blogs = response.data;
      } catch (error) {
        console.error('Error fetching blogs:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to load blogs. Please try again.';
      } finally {
        this.loading = false;
      }
    },
    viewBlogDetails(blogId) {
      this.$router.push(`/blogs/${blogId}`);
    },
    editBlog(blogId) {
      this.$router.push(`/blogs/${blogId}/edit`);
    },
    confirmDelete(blog) {
      this.blogToDelete = blog;
      this.deleteDialog = true;
    },
    async deleteBlog() {
      if (!this.blogToDelete) return;

      this.deletingBlog = true;
      try {
        await axios.delete(`/blogs/${this.blogToDelete.id}`);
        this.blogs = this.blogs.filter(b => b.id !== this.blogToDelete.id);
        this.deleteDialog = false;
        this.blogToDelete = null;
      } catch (error) {
        console.error('Error deleting blog:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to delete blog.';
      } finally {
        this.deletingBlog = false;
      }
    },
    isMyBlog(blog) {
      // For now, we'll assume all blogs can be edited by authors
      // In a real app, you'd check if blog.authorId === currentUser.id
      return this.store.role === 'Author';
    },
    canCommentOnBlog(blog) {
      // User can comment if they follow the blog author
      return this.followingUsers.has(blog.authorId);
    },
    addComment(blogId) {
      // Placeholder for comment functionality
      this.$router.push(`/blogs/${blogId}#comments`);
    },
    async fetchFollowingUsers() {
      if (!this.currentUserId) return;
      
      try {
        const response = await axios.get(`/followers/${this.currentUserId}/following`);
        const following = response.data || [];
        this.followingUsers = new Set(following.map(user => user.id || user.userId));
      } catch (error) {
        console.error('Error fetching following users:', error);
        this.followingUsers = new Set();
      }
    },
    truncateHtml(html, maxLength) {
      if (!html) return '';
      // Remove HTML tags for truncation
      const text = html.replace(/<[^>]*>/g, '');
      if (text.length <= maxLength) return html;
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
    }
  },
  async mounted() {
    // Get current user ID from token
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        this.currentUserId = payload.id || payload.userId;
      } catch (e) {
        console.error('Error parsing token:', e);
      }
    }
    
    await Promise.all([
      this.fetchBlogs(),
      this.fetchFollowingUsers()
    ]);
  }
};
</script>

<style scoped>
.blogs-container {
  min-height: 80vh;
  padding-top: 40px;
  padding-bottom: 40px;
  background: linear-gradient(135deg, #E3F2FD 0%, #E0F2F1 100%);
}

.create-blog-btn {
  box-shadow: 0 4px 12px rgba(30, 136, 229, 0.3);
}

.blog-card {
  height: 100%;
  display: flex;
  flex-direction: column;
  border-radius: 16px;
  transition: all 0.3s ease;
  position: relative;
  cursor: pointer;
  overflow: hidden;
}

.blog-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 12px 24px rgba(0,0,0,0.15) !important;
}

.blog-image-container {
  position: relative;
  height: 200px;
  overflow: hidden;
}

.blog-image {
  transition: transform 0.3s ease;
}

.blog-card:hover .blog-image {
  transform: scale(1.05);
}

.blog-image-overlay {
  position: absolute;
  top: 12px;
  right: 12px;
  z-index: 1;
}

.blog-image-placeholder {
  height: 200px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f5f5f5;
  position: relative;
}

.blog-date-chip {
  background: rgba(0, 0, 0, 0.7) !important;
  color: white !important;
}

.blog-date-chip-placeholder {
  position: absolute;
  top: 12px;
  right: 12px;
  background: rgba(0, 0, 0, 0.7) !important;
  color: white !important;
}

.blog-description {
  color: #616161;
  line-height: 1.6;
  min-height: 60px;
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

.blog-card:nth-child(1) { animation-delay: 0s; }
.blog-card:nth-child(2) { animation-delay: 0.1s; }
.blog-card:nth-child(3) { animation-delay: 0.2s; }
.blog-card:nth-child(4) { animation-delay: 0.3s; }
.blog-card:nth-child(5) { animation-delay: 0.4s; }
.blog-card:nth-child(6) { animation-delay: 0.5s; }

@media (max-width: 960px) {
  .blogs-container {
    padding-top: 20px;
  }
  
  .text-h3 {
    font-size: 2rem !important;
  }
}
</style>
