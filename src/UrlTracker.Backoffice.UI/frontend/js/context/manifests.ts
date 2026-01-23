export const manifests: Array<UmbExtensionManifest> = [
  {
    type: 'globalContext',
    alias: 'Urltracker.GlobalContext.ServerVariables',
    name: 'Server Variables Context',
    api: () => import('./servervariables.context.js'),
  },
];
