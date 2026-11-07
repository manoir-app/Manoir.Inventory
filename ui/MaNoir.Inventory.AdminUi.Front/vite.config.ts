import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

const apiTarget = {
  target: 'https://localhost:7294',
  changeOrigin: true,
  secure: false,
};

export default defineConfig(({ command }) => ({
  // Relative at build time so the server-injected <base href> drives the deployed prefix; root during dev.
  base: command === 'build' ? './' : '/',
  plugins: [react()],
  server: {
    proxy: {
      '/v1.0': apiTarget,
      '/healthz': apiTarget,
    },
  },
}));
