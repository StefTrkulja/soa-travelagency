<template>
  <v-container fluid class="my-location-container">
    <v-row justify="center">
      <v-col cols="12" lg="10">
        <v-card class="elevation-8 location-card">
          <v-card-title class="text-h4 text-center py-6 golden-header">
            <v-icon size="large" class="mr-3">mdi-map-marker</v-icon>
            My Location
          </v-card-title>
          
          <v-card-text class="pa-6">
            <v-row>
              <!-- Map Section -->
              <v-col cols="12" lg="8">
                <div class="map-section">
                  <h3 class="text-h6 mb-4">�️ Interactive Map</h3>
                  <v-card variant="outlined" class="map-container">
                    <div id="map" class="leaflet-map"></div>
                  </v-card>
                  <div class="d-flex gap-2 mt-3">
                    <v-btn 
                      @click="getCurrentLocation" 
                      color="success" 
                      variant="outlined"
                      :loading="gettingLocation"
                      size="small"
                    >
                      <v-icon start>mdi-crosshairs-gps</v-icon>
                      Use GPS
                    </v-btn>
                    <v-btn 
                      @click="clearLocation"
                      color="error" 
                      variant="outlined" 
                      :loading="loading"
                      size="small"
                      v-if="currentLocation"
                    >
                      <v-icon start>mdi-delete</v-icon>
                      Clear Location
                    </v-btn>
                  </div>
                </div>
              </v-col>

              <!-- Controls Section -->
              <v-col cols="12" lg="4">
                <!-- Current Location Display -->
                <div v-if="currentLocation" class="mb-6">
                  <h3 class="text-h6 mb-4">📍 Current Location</h3>
                  <v-card variant="outlined" class="pa-4 location-display">
                    <p><strong>Latitude:</strong> {{ currentLocation.latitude?.toFixed(6) }}°</p>
                    <p><strong>Longitude:</strong> {{ currentLocation.longitude?.toFixed(6) }}°</p>
                    <p class="text-caption">
                      Last updated: {{ formatDate(currentLocation.locationUpdatedAt) }}
                    </p>
                  </v-card>
                </div>

                <!-- Manual Coordinate Entry -->
                <div class="mb-6">
                  <h3 class="text-h6 mb-4">📝 Manual Entry</h3>
                  <v-form @submit.prevent="updateLocationFromForm">
                    <v-text-field
                      v-model="form.latitude"
                      label="Latitude"
                      type="number"
                      step="any"
                      :rules="[rules.required, rules.latitude]"
                      variant="outlined"
                      density="compact"
                      class="mb-3"
                    />
                    <v-text-field
                      v-model="form.longitude"
                      label="Longitude"
                      type="number"
                      step="any"
                      :rules="[rules.required, rules.longitude]"
                      variant="outlined"
                      density="compact"
                      class="mb-3"
                    />
                    <v-btn 
                      type="submit" 
                      color="primary" 
                      :loading="loading"
                      block
                    >
                      {{ currentLocation ? 'Update' : 'Set' }} Location
                    </v-btn>
                  </v-form>
                </div>

                <!-- Quick Locations -->
                <div>
                  <h3 class="text-h6 mb-4">🎯 Quick Locations</h3>
                  <div class="quick-locations-grid">
                    <v-btn
                      v-for="location in quickLocations" 
                      :key="location.name"
                      variant="outlined"
                      size="small"
                      @click="setQuickLocation(location)"
                      :loading="loading"
                      class="mb-2"
                      block
                    >
                      {{ location.name }}
                    </v-btn>
                  </div>
                </div>
              </v-col>
            </v-row>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Success/Error Snackbars -->
    <v-snackbar v-model="snackbar.show" :color="snackbar.color">
      {{ snackbar.message }}
    </v-snackbar>
  </v-container>
</template>

<script>
import axiosInstance from '@/utils/axiosInstance';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';

export default {
  name: 'MyLocationView',
  data() {
    return {
      currentLocation: null,
      loading: false,
      gettingLocation: false,
      map: null,
      marker: null,
      form: {
        latitude: '',
        longitude: ''
      },
      snackbar: {
        show: false,
        message: '',
        color: 'success'
      },
      rules: {
        required: value => !!value || 'Required',
        latitude: value => {
          const num = parseFloat(value);
          return (num >= -90 && num <= 90) || 'Latitude must be between -90 and 90';
        },
        longitude: value => {
          const num = parseFloat(value);
          return (num >= -180 && num <= 180) || 'Longitude must be between -180 and 180';
        }
      },
      quickLocations: [
        { name: 'Belgrade', latitude: 44.8125, longitude: 20.4612 },
        { name: 'Paris', latitude: 48.8566, longitude: 2.3522 },
        { name: 'New York', latitude: 40.7128, longitude: -74.0060 },
        { name: 'Tokyo', latitude: 35.6762, longitude: 139.6503 },
        { name: 'Sydney', latitude: -33.8688, longitude: 151.2093 },
        { name: 'London', latitude: 51.5074, longitude: -0.1278 }
      ]
    }
  },
  async mounted() {
    await this.loadCurrentLocation();
    this.initMap();
  },
  beforeUnmount() {
    if (this.map) {
      this.map.remove();
    }
  },
  methods: {
    initMap() {
      // Fix for default markers in Leaflet with Vite
      delete L.Icon.Default.prototype._getIconUrl;
      L.Icon.Default.mergeOptions({
        iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon-2x.png',
        iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon.png',
        shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-shadow.png',
      });

      // Initialize map
      const defaultLat = this.currentLocation?.latitude || 44.8125; // Belgrade default
      const defaultLng = this.currentLocation?.longitude || 20.4612;
      
      this.map = L.map('map').setView([defaultLat, defaultLng], 10);
      
      // Add OpenStreetMap tiles
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors',
        maxZoom: 19
      }).addTo(this.map);

      // Add marker if location exists
      if (this.currentLocation?.latitude && this.currentLocation?.longitude) {
        this.addMarker(this.currentLocation.latitude, this.currentLocation.longitude);
      }

      // Add click event to set location
      this.map.on('click', (e) => {
        this.updateLocationFromMap(e.latlng.lat, e.latlng.lng);
      });
    },

    addMarker(lat, lng) {
      if (this.marker) {
        this.map.removeLayer(this.marker);
      }
      
      this.marker = L.marker([lat, lng])
        .addTo(this.map)
        .bindPopup(`Your Location<br>Lat: ${lat.toFixed(6)}<br>Lng: ${lng.toFixed(6)}`)
        .openPopup();
    },

    async loadCurrentLocation() {
      try {
        const response = await axiosInstance.get('/profile/location');
        if (response.data.hasLocation) {
          this.currentLocation = response.data;
          if (this.map && this.currentLocation.latitude && this.currentLocation.longitude) {
            this.map.setView([this.currentLocation.latitude, this.currentLocation.longitude], 13);
            this.addMarker(this.currentLocation.latitude, this.currentLocation.longitude);
          }
        }
      } catch (error) {
        console.error('Error loading location:', error);
      }
    },
    
    async updateLocationFromMap(lat, lng) {
      await this.updateLocation(lat, lng);
    },

    async updateLocationFromForm() {
      if (!this.form.latitude || !this.form.longitude) return;
      await this.updateLocation(parseFloat(this.form.latitude), parseFloat(this.form.longitude));
    },
    
    async updateLocation(lat, lng) {
      this.loading = true;
      try {
        const response = await axiosInstance.post('/profile/location', {
          latitude: lat,
          longitude: lng
        });
        
        this.currentLocation = response.data;
        this.form.latitude = lat.toString();
        this.form.longitude = lng.toString();
        
        // Update map
        this.map.setView([lat, lng], 13);
        this.addMarker(lat, lng);
        
        this.showMessage('Location updated successfully!', 'success');
      } catch (error) {
        this.showMessage('Error updating location: ' + (error.response?.data?.message || error.message), 'error');
      } finally {
        this.loading = false;
      }
    },
    
    async clearLocation() {
      this.loading = true;
      try {
        await axiosInstance.delete('/profile/location');
        this.currentLocation = null;
        this.form.latitude = '';
        this.form.longitude = '';
        
        // Remove marker from map
        if (this.marker) {
          this.map.removeLayer(this.marker);
          this.marker = null;
        }
        
        // Reset map to default view
        this.map.setView([44.8125, 20.4612], 10);
        
        this.showMessage('Location cleared successfully!', 'success');
      } catch (error) {
        this.showMessage('Error clearing location: ' + (error.response?.data?.message || error.message), 'error');
      } finally {
        this.loading = false;
      }
    },
    
    getCurrentLocation() {
      if (!navigator.geolocation) {
        this.showMessage('Geolocation is not supported by this browser.', 'error');
        return;
      }
      
      this.gettingLocation = true;
      navigator.geolocation.getCurrentPosition(
        (position) => {
          const lat = position.coords.latitude;
          const lng = position.coords.longitude;
          
          this.updateLocation(lat, lng);
          this.gettingLocation = false;
          this.showMessage('GPS location captured!', 'success');
        },
        (error) => {
          this.gettingLocation = false;
          this.showMessage('Error getting GPS location: ' + error.message, 'error');
        }
      );
    },
    
    setQuickLocation(location) {
      this.updateLocation(location.latitude, location.longitude);
    },
    
    formatDate(dateString) {
      if (!dateString) return 'Never';
      return new Date(dateString).toLocaleString();
    },
    
    showMessage(message, color) {
      this.snackbar.message = message;
      this.snackbar.color = color;
      this.snackbar.show = true;
    }
  }
}
</script>

<style scoped>
.my-location-container {
  background: linear-gradient(135deg, #E3F2FD 0%, #F3E5F5 100%);
  min-height: 100vh;
  padding-top: 2rem;
}

.location-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-radius: 16px !important;
}

.golden-header {
  background: linear-gradient(135deg, #FFD700, #FFA500);
  color: #1A1A1A;
  font-weight: bold;
  border-radius: 16px 16px 0 0 !important;
}

.location-display {
  background: linear-gradient(135deg, #E8F5E8, #F0F8F0);
}

.map-container {
  height: 450px;
  border-radius: 12px !important;
  overflow: hidden;
  border: 2px solid #E0E0E0;
}

.leaflet-map {
  height: 100%;
  width: 100%;
  border-radius: 10px;
}

.map-section {
  height: 100%;
}

.quick-locations-grid {
  display: grid;
  gap: 8px;
}

/* Fix Leaflet controls in Vuetify */
:deep(.leaflet-control-zoom) {
  border: 2px solid rgba(0,0,0,0.2) !important;
  border-radius: 8px !important;
}

:deep(.leaflet-control-zoom a) {
  background-color: #fff !important;
  border: none !important;
  color: #333 !important;
  font-weight: bold !important;
}

:deep(.leaflet-control-zoom a:hover) {
  background-color: #f4f4f4 !important;
}

:deep(.leaflet-popup-content-wrapper) {
  border-radius: 8px !important;
  box-shadow: 0 4px 12px rgba(0,0,0,0.15) !important;
}

:deep(.leaflet-popup-tip) {
  background: white !important;
}

/* Responsive adjustments */
@media (max-width: 1200px) {
  .map-container {
    height: 350px;
  }
}

@media (max-width: 768px) {
  .map-container {
    height: 300px;
  }
}
</style>