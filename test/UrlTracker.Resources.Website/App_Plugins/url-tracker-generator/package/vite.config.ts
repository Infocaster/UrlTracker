import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/my-element.ts", // your web component source file
      formats: ["es"],
    },
    outDir: "wwwroot", // all compiled files will be placed here
    emptyOutDir: true,
    sourcemap: true,
    rollupOptions: {
      external: [/^@umbraco/], // ignore the Umbraco Backoffice package in the build
    },
  },
  base: "/App_Plugins/url-tracker-generator/", // the base path of the app in the browser (used for assets)
});
