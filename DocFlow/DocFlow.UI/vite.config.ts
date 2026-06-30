import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(fileURLToPath(new URL('.', import.meta.url)), 'src'),
    },
  },
  server: {
    host: 'localhost',
    port: 5173,
    strictPort: true,
    proxy: {
      '/auth': {
        target: 'https://localhost:7219',
        changeOrigin: true,
        secure: false,
      },
      '/documents': {
        target: 'https://localhost:7219',
        changeOrigin: true,
        secure: false,
      },
      '/approvals': {
        target: 'https://localhost:7219',
        changeOrigin: true,
        secure: false,
      },
      '/audit': {
        target: 'https://localhost:7219',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
