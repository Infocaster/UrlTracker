import { resolve } from 'path';
import { defineConfig } from 'vite';

export default defineConfig({
  build: {
    lib: {
      entry: [
        resolve(__dirname, 'frontend', 'js', 'index.ts'),
        resolve(__dirname, 'frontend', 'js', 'umbEntrypoint.ts'),
      ],
      name: 'script',
      fileName: (format, entryName) => `${entryName}.js`,
      formats: ['es'],
    },
    outDir: 'wwwroot',
    sourcemap: true,
    emtpyOutDir: true,
    rollupOptions: {
      external: ['@umbraco-ui/uui', /^@umbraco/],
    },
  },
  base: '/App_Plugins/UrlTracker/',
  publicDir: resolve(__dirname, 'frontend', 'public'),
  resolve: {
    alias: {
      '@': resolve(__dirname, 'frontend', 'js'),
      '@sidebar': resolve(__dirname, 'frontend', 'js', 'dashboard', 'sidebars'),
    },
  },
});
