/** @type {import("eslint").Linter.Config} */
module.exports = {
  root: true,
  extends: ['eslint:recommended', 'plugin:@typescript-eslint/recommended', 'prettier', 'plugin:lit-a11y/recommended'],
  parser: '@typescript-eslint/parser',
  plugins: ['@typescript-eslint', 'lit-a11y'],
  parserOptions: {
    ecmaVersion: 'latest',
    sourceType: 'module',
  },
  rules: {},
  overrides: [
    {
      files: ['**/*.ts', '**/*.tsx'],
    },
  ],
};
