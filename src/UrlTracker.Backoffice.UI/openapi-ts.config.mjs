/** @type {import('@hey-api/openapi-ts').UserConfig} */
export default {
  client: 'axios',
  types: {
    dates: true,
    enums: 'typescript',
  },
  output: {
    lint: 'eslint',
    format: 'prettier',
    path: 'frontend/js/api',
  },
};
