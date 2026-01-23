import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
  client: {
    name: '@hey-api/client-axios',
    baseUrl: '/',
  },
  input: 'https://localhost:44349/umbraco/swagger/url-tracker/swagger.json',
  output: {
    format: 'prettier',
    path: './api-client',
  },
});
