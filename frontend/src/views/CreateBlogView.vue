<template>
  <v-container class="create-blog-container">
    <!-- Header Section -->
    <v-row class="mb-6">
      <v-col cols="12">
        <div class="d-flex justify-space-between align-center">
          <div>
            <h1 class="text-h3 font-weight-bold text-primary mb-2">Create New Blog Post</h1>
            <p class="text-subtitle-1 text-grey-darken-1">Share your travel experiences with the world</p>
          </div>
          <v-btn
            variant="text"
            prepend-icon="mdi-arrow-left"
            @click="$router.push('/blogs')"
          >
            Back to Blogs
          </v-btn>
        </div>
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

    <!-- Blog Form -->
    <v-row>
      <v-col cols="12" lg="8">
        <v-card class="blog-form-card" elevation="4">
          <v-card-title class="text-h5 pa-6 pb-4">
            <v-icon icon="mdi-blog" class="mr-3" size="large"></v-icon>
            Blog Details
          </v-card-title>

          <v-card-text class="pa-6 pt-0">
            <v-form v-model="formValid" ref="blogForm">
              <!-- Title -->
              <v-text-field
                v-model="blogForm.title"
                label="Blog Title *"
                prepend-icon="mdi-format-title"
                variant="outlined"
                :rules="titleRules"
                class="mb-4"
                hint="Enter an engaging title for your blog post"
                persistent-hint
                counter="200"
              ></v-text-field>

              <!-- Description (Markdown) -->
              <div class="mb-4">
                <label class="text-body-2 font-weight-medium mb-2 d-block">
                  <v-icon icon="mdi-text" class="mr-1"></v-icon>
                  Description (Markdown) *
                </label>
                <v-textarea
                  v-model="blogForm.descriptionMarkdown"
                  placeholder="Write your blog post using Markdown syntax..."
                  variant="outlined"
                  rows="15"
                  :rules="descriptionRules"
                  hint="Use Markdown syntax to format your blog post. You can use **bold**, *italic*, # headers, - lists, etc."
                  persistent-hint
                  counter="5000"
                ></v-textarea>
              </div>

              <!-- Markdown Preview Toggle -->
              <div class="mb-4">
                <v-btn
                  :variant="showPreview ? 'elevated' : 'outlined'"
                  :color="showPreview ? 'primary' : 'default'"
                  prepend-icon="mdi-eye"
                  @click="showPreview = !showPreview"
                  size="small"
                >
                  {{ showPreview ? 'Hide Preview' : 'Show Preview' }}
                </v-btn>
              </div>

              <!-- Markdown Preview -->
              <v-card v-if="showPreview" class="mb-4" elevation="2">
                <v-card-title class="text-h6 pa-4 pb-2">
                  <v-icon icon="mdi-eye" class="mr-2"></v-icon>
                  Preview
                </v-card-title>
                <v-card-text class="pa-4 pt-0">
                  <div 
                    class="markdown-preview"
                    v-html="renderedMarkdown"
                  ></div>
                </v-card-text>
              </v-card>
            </v-form>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Sidebar -->
      <v-col cols="12" lg="4">
        <!-- Images Upload -->
        <v-card class="mb-4" elevation="2">
          <v-card-title class="text-h6 pa-4">
            <v-icon icon="mdi-image-multiple" class="mr-2"></v-icon>
            Images (Optional)
          </v-card-title>
          <v-card-text class="pa-4">
            <v-file-input
              v-model="selectedImages"
              multiple
              accept="image/*"
              prepend-icon="mdi-camera"
              label="Select Images"
              variant="outlined"
              show-size
              counter
              @change="handleImageSelection"
            ></v-file-input>
            
            <!-- Image Preview -->
            <div v-if="imagePreviews.length > 0" class="mt-4">
              <div class="text-caption text-grey-darken-1 mb-2">Selected Images:</div>
              <div class="image-preview-grid">
                <div
                  v-for="(preview, index) in imagePreviews"
                  :key="index"
                  class="image-preview-item"
                >
                  <v-img
                    :src="preview"
                    height="80"
                    cover
                    class="rounded"
                  ></v-img>
                  <v-btn
                    icon="mdi-close"
                    size="x-small"
                    color="error"
                    variant="flat"
                    class="image-remove-btn"
                    @click="removeImage(index)"
                  ></v-btn>
                </div>
              </div>
            </div>
          </v-card-text>
        </v-card>

        <!-- Publishing Info -->
        <v-card class="mb-4" elevation="2">
          <v-card-title class="text-h6 pa-4">
            <v-icon icon="mdi-information" class="mr-2"></v-icon>
            Publishing Info
          </v-card-title>
          <v-card-text class="pa-4">
            <div class="info-item mb-3">
              <v-icon icon="mdi-account" size="small" class="mr-2 text-grey"></v-icon>
              <span class="text-caption text-grey-darken-1">Author:</span>
              <div class="text-body-2">{{ store.username }}</div>
            </div>
            <div class="info-item mb-3">
              <v-icon icon="mdi-calendar" size="small" class="mr-2 text-grey"></v-icon>
              <span class="text-caption text-grey-darken-1">Created:</span>
              <div class="text-body-2">{{ formatDate(new Date()) }}</div>
            </div>
            <div class="info-item">
              <v-icon icon="mdi-text" size="small" class="mr-2 text-grey"></v-icon>
              <span class="text-caption text-grey-darken-1">Characters:</span>
              <div class="text-body-2">{{ blogForm.descriptionMarkdown.length }}/5000</div>
            </div>
          </v-card-text>
        </v-card>

        <!-- Markdown Help -->
        <v-card elevation="2">
          <v-card-title class="text-h6 pa-4">
            <v-icon icon="mdi-help-circle" class="mr-2"></v-icon>
            Markdown Help
          </v-card-title>
          <v-card-text class="pa-4">
            <div class="markdown-help">
              <div class="help-item mb-2">
                <code>**bold**</code> → <strong>bold</strong>
              </div>
              <div class="help-item mb-2">
                <code>*italic*</code> → <em>italic</em>
              </div>
              <div class="help-item mb-2">
                <code># Header</code> → <h4>Header</h4>
              </div>
              <div class="help-item mb-2">
                <code>- List item</code> → • List item
              </div>
              <div class="help-item">
                <code>[Link](url)</code> → <a href="#">Link</a>
              </div>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Action Buttons -->
    <v-row class="mt-6">
      <v-col cols="12">
        <div class="d-flex justify-end gap-3">
          <v-btn
            variant="text"
            size="large"
            @click="$router.push('/blogs')"
            :disabled="saving"
          >
            Cancel
          </v-btn>
          <v-btn
            color="primary"
            variant="elevated"
            size="large"
            prepend-icon="mdi-publish"
            @click="createBlog"
            :loading="saving"
            :disabled="!formValid"
          >
            Publish Blog
          </v-btn>
        </div>
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
    renderedMarkdown() {
      if (!this.blogForm.descriptionMarkdown) return '';
      
      return this.blogForm.descriptionMarkdown
        .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
        .replace(/\*(.*?)\*/g, '<em>$1</em>')
        .replace(/^# (.*$)/gm, '<h1>$1</h1>')
        .replace(/^## (.*$)/gm, '<h2>$1</h2>')
        .replace(/^### (.*$)/gm, '<h3>$1</h3>')
        .replace(/^- (.*$)/gm, '<li>$1</li>')
        .replace(/\[([^\]]+)\]\(([^)]+)\)/g, '<a href="$2">$1</a>')
        .replace(/\n/g, '<br>');
    }
  },
  data() {
    return {
      blogForm: {
        title: '',
        descriptionMarkdown: ''
      },
      selectedImages: [],
      imagePreviews: [],
      formValid: false,
      saving: false,
      showPreview: false,
      errorMessage: '',
      successSnackbar: false,
      successMessage: '',
      titleRules: [
        v => !!v || 'Title is required',
        v => (v && v.length >= 5) || 'Title must be at least 5 characters',
        v => (v && v.length <= 200) || 'Title must be less than 200 characters'
      ],
      descriptionRules: [
        v => !!v || 'Description is required',
        v => (v && v.length >= 10) || 'Description must be at least 10 characters',
        v => (v && v.length <= 5000) || 'Description must be less than 5000 characters'
      ]
    };
  },
  methods: {
    handleImageSelection(files) {
      this.imagePreviews = [];
      if (files && files.length > 0) {
        files.forEach(file => {
          if (file && file.type.startsWith('image/')) {
            const reader = new FileReader();
            reader.onload = (e) => {
              this.imagePreviews.push(e.target.result);
            };
            reader.readAsDataURL(file);
          }
        });
      }
    },
    removeImage(index) {
      this.imagePreviews.splice(index, 1);
      this.selectedImages.splice(index, 1);
    },
    async createBlog() {
      if (!this.formValid) return;

      this.saving = true;
      this.errorMessage = '';

      try {
        const formData = new FormData();
        
        // Add the blog data as JSON
        const blogData = {
          title: this.blogForm.title,
          descriptionMarkdown: this.blogForm.descriptionMarkdown
        };
        formData.append('payload', JSON.stringify(blogData));

        // Add images
        if (this.selectedImages && this.selectedImages.length > 0) {
          this.selectedImages.forEach((file, index) => {
            if (file && file.type.startsWith('image/')) {
              formData.append('images', file);
            }
          });
        }

        const response = await axios.post('/blogs', formData, {
          headers: {
            'Content-Type': 'multipart/form-data'
          }
        });

        this.successMessage = 'Blog post created successfully!';
        this.successSnackbar = true;
        
        // Redirect to blogs page after a short delay
        setTimeout(() => {
          this.$router.push('/blogs');
        }, 2000);

      } catch (error) {
        console.error('Error creating blog:', error);
        this.errorMessage = error.response?.data?.message || 'Failed to create blog post. Please try again.';
      } finally {
        this.saving = false;
      }
    },
    formatDate(date) {
      return date.toLocaleDateString('en-US', { 
        year: 'numeric', 
        month: 'long', 
        day: 'numeric' 
      });
    }
  }
};
</script>

<style scoped>
.create-blog-container {
  min-height: 80vh;
  padding-top: 40px;
  padding-bottom: 40px;
  background: linear-gradient(135deg, #E3F2FD 0%, #E0F2F1 100%);
}

.blog-form-card {
  border-radius: 16px;
  transition: all 0.3s ease;
}

.blog-form-card:hover {
  box-shadow: 0 8px 24px rgba(0,0,0,0.12) !important;
}

.markdown-preview {
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 16px;
  background: #fafafa;
  min-height: 200px;
  line-height: 1.6;
}

.markdown-preview h1 {
  font-size: 1.5rem;
  font-weight: bold;
  margin: 16px 0 8px 0;
  color: #1976d2;
}

.markdown-preview h2 {
  font-size: 1.3rem;
  font-weight: bold;
  margin: 12px 0 6px 0;
  color: #1976d2;
}

.markdown-preview h3 {
  font-size: 1.1rem;
  font-weight: bold;
  margin: 8px 0 4px 0;
  color: #1976d2;
}

.markdown-preview li {
  margin: 4px 0;
  padding-left: 8px;
}

.markdown-preview a {
  color: #1976d2;
  text-decoration: none;
}

.markdown-preview a:hover {
  text-decoration: underline;
}

.image-preview-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(80px, 1fr));
  gap: 8px;
}

.image-preview-item {
  position: relative;
  border-radius: 8px;
  overflow: hidden;
}

.image-remove-btn {
  position: absolute;
  top: 4px;
  right: 4px;
  z-index: 1;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.markdown-help {
  font-size: 0.875rem;
}

.help-item code {
  background: #f5f5f5;
  padding: 2px 4px;
  border-radius: 4px;
  font-family: 'Courier New', monospace;
  font-size: 0.8rem;
}

.gap-3 {
  gap: 12px;
}

@media (max-width: 960px) {
  .create-blog-container {
    padding-top: 20px;
  }
  
  .text-h3 {
    font-size: 2rem !important;
  }
}
</style>
