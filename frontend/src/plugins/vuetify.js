import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'

import { createVuetify } from 'vuetify'

export default createVuetify({
  theme: {
    defaultTheme: 'travelTheme',
    themes: {
      travelTheme: {
        dark: false,
        colors: {
          primary: '#1E88E5',
          secondary: '#26A69A',
          accent: '#FF9800',
          error: '#F44336',
          warning: '#FF9800',
          info: '#2196F3',
          success: '#4CAF50',
          background: '#F5F5F5',
          surface: '#FFFFFF',
          'on-primary': '#FFFFFF',
          'on-secondary': '#FFFFFF',
          'on-background': '#212121',
          'on-surface': '#212121',
        }
      }
    }
  },
})
