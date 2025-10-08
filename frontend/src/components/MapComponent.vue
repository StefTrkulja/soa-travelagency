<template>
  <div class="map-container">
    <div ref="mapContainer" class="map" :style="{ height: height + 'px' }"></div>
    <div class="map-controls">
      <v-btn
        color="primary"
        size="small"
        @click="addMarker"
        :disabled="!canAddMarker"
        class="ma-2"
      >
        <v-icon start>mdi-map-marker-plus</v-icon>
        Add Point
      </v-btn>
      <v-btn
        color="error"
        size="small"
        @click="clearMarkers"
        :disabled="markers.length === 0"
        class="ma-2"
      >
        <v-icon start>mdi-delete</v-icon>
        Clear All
      </v-btn>
    </div>
    
    <!-- Marker info dialog -->
    <v-dialog v-model="markerDialog" max-width="500px">
      <v-card>
        <v-card-title>
          <span class="text-h5">{{ editingMarker ? 'Edit Point' : 'Add Point' }}</span>
        </v-card-title>
        <v-card-text>
          <v-text-field
            v-model="markerData.name"
            label="Point Name *"
            :rules="nameRules"
            variant="outlined"
            required
          ></v-text-field>
          <v-textarea
            v-model="markerData.description"
            label="Description"
            variant="outlined"
            rows="3"
          ></v-textarea>
          <v-row>
            <v-col cols="6">
              <v-text-field
                v-model.number="markerData.latitude"
                label="Latitude *"
                type="number"
                step="any"
                variant="outlined"
                readonly
              ></v-text-field>
            </v-col>
            <v-col cols="6">
              <v-text-field
                v-model.number="markerData.longitude"
                label="Longitude *"
                type="number"
                step="any"
                variant="outlined"
                readonly
              ></v-text-field>
            </v-col>
          </v-row>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="grey" variant="text" @click="closeMarkerDialog">
            Cancel
          </v-btn>
          <v-btn color="primary" @click="saveMarker" :disabled="!markerData.name">
            {{ editingMarker ? 'Update' : 'Add' }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script>
import { ref, onMounted, onUnmounted, watch, computed, nextTick } from 'vue'
import Map from 'ol/Map'
import View from 'ol/View'
import TileLayer from 'ol/layer/Tile'
import OSM from 'ol/source/OSM'
import VectorLayer from 'ol/layer/Vector'
import VectorSource from 'ol/source/Vector'
import Feature from 'ol/Feature'
import Point from 'ol/geom/Point'
import { fromLonLat, toLonLat } from 'ol/proj'
import { Style, Icon, Text, Fill, Stroke } from 'ol/style'
import { Select } from 'ol/interaction'

export default {
  name: 'MapComponent',
  props: {
    modelValue: {
      type: Array,
      default: () => []
    },
    height: {
      type: Number,
      default: 400
    },
    center: {
      type: Array,
      default: () => [20.4573, 44.7872] // Belgrade coordinates [lon, lat]
    },
    zoom: {
      type: Number,
      default: 13
    }
  },
  emits: ['update:modelValue', 'update:distance'],
  setup(props, { emit }) {
    const mapContainer = ref(null)
    const map = ref(null)
    const markers = ref(props.modelValue || [])
    const markerDialog = ref(false)
    const editingMarker = ref(null)
    const markerData = ref({
      name: '',
      description: '',
      latitude: null,
      longitude: null
    })

    const nameRules = [
      v => !!v || 'Point name is required',
      v => (v && v.length >= 2) || 'Name must be at least 2 characters'
    ]

    const canAddMarker = computed(() => {
      return markers.value.length < 10 // Max 10 points
    })

    // Calculate distance between two points using Haversine formula
    const calculateDistance = (lat1, lon1, lat2, lon2) => {
      const R = 6371 // Earth's radius in kilometers
      const dLat = (lat2 - lat1) * Math.PI / 180
      const dLon = (lon2 - lon1) * Math.PI / 180
      const a = 
        Math.sin(dLat/2) * Math.sin(dLat/2) +
        Math.cos(lat1 * Math.PI / 180) * Math.cos(lat2 * Math.PI / 180) * 
        Math.sin(dLon/2) * Math.sin(dLon/2)
      const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1-a))
      const distance = R * c
      
      // Debug log
      //console.log(`Distance between (${lat1}, ${lon1}) and (${lat2}, ${lon2}): ${distance.toFixed(3)} km`)
      
      return distance
    }

    // Calculate total tour distance
    const calculateTotalDistance = () => {
      if (!markers.value || markers.value.length < 2) {
        return 0
      }

      let totalDistance = 0
      for (let i = 0; i < markers.value.length - 1; i++) {
        const point1 = markers.value[i]
        const point2 = markers.value[i + 1]
        const segmentDistance = calculateDistance(
          point1.latitude, 
          point1.longitude, 
          point2.latitude, 
          point2.longitude
        )
        totalDistance += segmentDistance
      }
      
      const roundedDistance = Math.round(totalDistance * 1000) / 1000
      //Debug log
      //console.log(`Total tour distance: ${roundedDistance} km`)
      
      return roundedDistance
    }

    let vectorSource
    let vectorLayer
    let selectInteraction

    const initMap = () => {
      if (!mapContainer.value) return

      vectorSource = new VectorSource()
      vectorLayer = new VectorLayer({
        source: vectorSource
      })

      map.value = new Map({
        target: mapContainer.value,
        layers: [
          new TileLayer({
            source: new OSM()
          }),
          vectorLayer
        ],
        view: new View({
          center: fromLonLat(props.center),
          zoom: props.zoom
        })
      })

      // Add click interaction
      map.value.on('click', (event) => {
        const coordinate = toLonLat(event.coordinate)
        markerData.value = {
          name: '',
          description: '',
          latitude: coordinate[1],
          longitude: coordinate[0]
        }
        editingMarker.value = null
        markerDialog.value = true
      })

      // Add select interaction for editing markers
      selectInteraction = new Select({
        style: new Style({
          image: new Icon({
            src: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPHBhdGggZD0iTTEyIDJDOC4xMyAyIDUgNS4xMyA1IDlDNSA5Ljc0IDUuMTEgMTAuNDggNS4zMSAxMS4yTDEyIDIyTDYuNjkgMTEuMkM2Ljg5IDEwLjQ4IDcgOS43NCA3IDlDNyA1LjEzIDkuMTMgMiAxMiAyWk0xMiA2LjVDMTAgNi41IDguNSA4IDguNSA5LjVDOC41IDExIDEwIDEyLjUgMTIgMTIuNUMxNCAxMi41IDE1LjUgMTEgMTUuNSA5LjVDMTUuNSA4IDE0IDYuNSAxMiA2LjVaTTEyIDEwQzExLjQ0IDEwIDExIDkuNTYgMTEgOUMxMSA4LjQ0IDExLjQ0IDggMTIgOEMxMi41NiA4IDEzIDguNDQgMTMgOUMxMyA5LjU2IDEyLjU2IDEwIDEyIDEwWiIgZmlsbD0iI0Y0NDM0NiIvPgo8L3N2Zz4K',
            scale: 1.2
          }),
          text: new Text({
            text: '✏️',
            offsetY: -30,
            font: '16px sans-serif'
          })
        })
      })

      map.value.addInteraction(selectInteraction)

      selectInteraction.on('select', (event) => {
        if (event.selected.length > 0) {
          const feature = event.selected[0]
          const coordinate = toLonLat(feature.getGeometry().getCoordinates())
          const marker = markers.value.find(m => 
            Math.abs(m.longitude - coordinate[0]) < 0.0001 && 
            Math.abs(m.latitude - coordinate[1]) < 0.0001
          )
          
          if (marker) {
            markerData.value = { ...marker }
            editingMarker.value = marker
            markerDialog.value = true
          }
        }
      })

      // Load existing markers
      loadMarkers()
    }

    const createMarkerStyle = (marker, index) => {
      return new Style({
        image: new Icon({
          src: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIHZpZXdCb3g9IjAgMCAyNCAyNCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPHBhdGggZD0iTTEyIDJDOC4xMyAyIDUgNS4xMyA1IDlDNSA5Ljc0IDUuMTEgMTAuNDggNS4zMSAxMS4yTDEyIDIyTDYuNjkgMTEuMkM2Ljg5IDEwLjQ4IDcgOS43NCA3IDlDNyA1LjEzIDkuMTMgMiAxMiAyWk0xMiA2LjVDMTAgNi41IDguNSA4IDguNSA5LjVDOC41IDExIDEwIDEyLjUgMTIgMTIuNUMxNCAxMi41IDE1LjUgMTEgMTUuNSA5LjVDMTUuNSA4IDE0IDYuNSAxMiA2LjVaTTEyIDEwQzExLjQ0IDEwIDExIDkuNTYgMTEgOUMxMSA4LjQ0IDExLjQ0IDggMTIgOEMxMi41NiA4IDEzIDguNDQgMTMgOUMxMyA5LjU2IDEyLjU2IDEwIDEyIDEwWiIgZmlsbD0iIzQzQUZFNCIvPgo8L3N2Zz4K',
          scale: 1
        }),
        text: new Text({
          text: (index + 1).toString(),
          font: 'bold 14px sans-serif',
          fill: new Fill({ color: 'white' }),
          stroke: new Stroke({ color: 'black', width: 2 }),
          offsetY: -25,
          textAlign: 'center',
          textBaseline: 'middle'
        })
      })
    }

    const addMarkerToMap = (marker, index) => {
      const feature = new Feature({
        geometry: new Point(fromLonLat([marker.longitude, marker.latitude]))
      })
      
      feature.setStyle(createMarkerStyle(marker, index))
      vectorSource.addFeature(feature)
    }

    const loadMarkers = () => {
      if (!vectorSource) return
      
      // Always clear existing markers first
      vectorSource.clear()
      
      // Add current markers
      if (markers.value && markers.value.length > 0) {
        markers.value.forEach((marker, index) => {
          addMarkerToMap(marker, index)
        })
      }
      
      // Calculate and emit distance
      const totalDistance = calculateTotalDistance()
      emit('update:distance', totalDistance)
    }

    const addMarker = () => {
      // Center map on Belgrade if no markers exist
      if (!markers.value || markers.value.length === 0) {
        map.value.getView().setCenter(fromLonLat(props.center))
        map.value.getView().setZoom(props.zoom)
      }
      
      // This will be triggered by map click
    }

    const clearMarkers = () => {
      markers.value = []
      if (vectorSource) vectorSource.clear()
      emit('update:modelValue', [])
      emit('update:distance', 0)
    }

    const saveMarker = () => {
      if (!markers.value) markers.value = []
      
      if (editingMarker.value) {
        // Update existing marker
        const index = markers.value.indexOf(editingMarker.value)
        markers.value[index] = { ...markerData.value }
      } else {
        // Add new marker
        markers.value.push({ ...markerData.value })
      }
      
      loadMarkers()
      emit('update:modelValue', [...markers.value])
      closeMarkerDialog()
    }

    const closeMarkerDialog = () => {
      markerDialog.value = false
      editingMarker.value = null
      markerData.value = {
        name: '',
        description: '',
        latitude: null,
        longitude: null
      }
    }

    // Initialize markers from props
    markers.value = props.modelValue || []

    // Watch for external changes
    watch(() => props.modelValue, (newValue, oldValue) => {
      //Debug log
      //console.log('MapComponent: modelValue changed', { newValue, oldValue })
      markers.value = [...(newValue || [])]
      loadMarkers()
    }, { immediate: true, deep: true })

    onMounted(() => {
      nextTick(() => {
        initMap()
      })
    })

    onUnmounted(() => {
      if (map.value) {
        map.value.setTarget(null)
      }
    })

    return {
      mapContainer,
      markers,
      markerDialog,
      editingMarker,
      markerData,
      nameRules,
      canAddMarker,
      addMarker,
      clearMarkers,
      saveMarker,
      closeMarkerDialog
    }
  }
}
</script>

<style scoped>
.map-container {
  position: relative;
  border: 1px solid #ddd;
  border-radius: 8px;
  overflow: hidden;
}

.map {
  width: 100%;
}

.map-controls {
  position: absolute;
  top: 10px;
  right: 10px;
  z-index: 1000;
}

:deep(.ol-popup) {
  position: absolute;
  background-color: white;
  box-shadow: 0 1px 4px rgba(0,0,0,0.2);
  padding: 15px;
  border-radius: 10px;
  border: 1px solid #cccccc;
  bottom: 12px;
  left: -50px;
  min-width: 100px;
}

:deep(.ol-popup:after, .ol-popup:before) {
  top: 100%;
  border: solid transparent;
  content: " ";
  height: 0;
  width: 0;
  position: absolute;
  pointer-events: none;
}

:deep(.ol-popup:after) {
  border-top-color: white;
  border-width: 10px;
  left: 48px;
  margin-left: -10px;
}

:deep(.ol-popup:before) {
  border-top-color: #cccccc;
  border-width: 11px;
  left: 48px;
  margin-left: -11px;
}
</style>
