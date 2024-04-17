import { resolve } from "path";
import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: resolve(__dirname, "frontend", "js", "index.ts"),
      name: "script",
      fileName: "script",
      formats: ["iife"],
    },
    outDir: "wwwroot",
    sourcemap: true,
    rollupOptions: {
      external: ["@umbraco-ui/uui"],
    },
  },
  publicDir: resolve(__dirname, "frontend", "public"),
  resolve: {
    alias: {
      "@": resolve(__dirname, "frontend", "js"),
      "@sidebar": resolve(__dirname, "frontend", "js", "dashboard", "sidebars"),
    },
  },
});
