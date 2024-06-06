import { defineConfig } from 'vite';
import tsconfigPaths from 'vite-tsconfig-paths';

export default defineConfig({
  build: {
    lib: {
      entry: 'frontend/js/index.ts', // your web component source file
      formats: ['es'],
    },
    outDir: 'wwwroot', // your web component will be saved in this location
    sourcemap: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
  publicDir: 'frontend/public',
  plugins: [tsconfigPaths()],
});
