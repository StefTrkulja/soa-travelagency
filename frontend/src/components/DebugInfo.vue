<template>
  <v-card class="debug-info ma-2 pa-4" color="info" variant="outlined">
    <v-card-title class="text-h6">Debug Information</v-card-title>
    <v-card-text>
      <div><strong>Current Role:</strong> {{ store.role }}</div>
      <div><strong>Username:</strong> {{ store.username }}</div>
      <div><strong>Current Route:</strong> {{ $route.path }}</div>
      <div><strong>Is Tourist:</strong> {{ store.role === 'Tourist' ? 'Yes' : 'No' }}</div>
      <div><strong>Token Exists:</strong> {{ hasToken ? 'Yes' : 'No' }}</div>
    </v-card-text>
  </v-card>
</template>

<script>
import { store } from '@/utils/store';

export default {
  name: 'DebugInfo',
  computed: {
    store() {
      return store;
    },
    hasToken() {
      return !!localStorage.getItem('token');
    }
  }
};
</script>

<style scoped>
.debug-info {
  position: fixed;
  bottom: 20px;
  right: 20px;
  z-index: 1000;
  max-width: 300px;
}
</style>