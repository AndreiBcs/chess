import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'
import tailwindcss from '@tailwindcss/vite'
import { loadEnv } from 'vite'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '')
  const apiUrl = env.VITE_API_URL || 'http://localhost:5204'

  return {
  plugins: [
      react(),
      tailwindcss()
  ],
  server: {
    proxy: {
      '/gamehub': {
        target: apiUrl,
        ws: true,
      }
    },
  },
  }
})
