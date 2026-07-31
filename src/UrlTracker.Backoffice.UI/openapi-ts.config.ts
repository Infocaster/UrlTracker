import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
  input: 'https://localhost:44349/umbraco/openapi/url-tracker.json',
  output: {
    format: 'prettier',
    path: './frontend/js/api-client',
  },
});
