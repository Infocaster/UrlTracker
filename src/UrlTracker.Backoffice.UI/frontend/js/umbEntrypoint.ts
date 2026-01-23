import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';
import type { UmbEntryPointOnInit } from '@umbraco-cms/backoffice/extension-api';
import { client } from '@umbraco-cms/backoffice/external/backend-api';
import { modalManifests } from './manifests/modal.manifests';

export const onInit: UmbEntryPointOnInit = (host, extensionRegistry) => {
  host.consumeContext(UMB_AUTH_CONTEXT, (authContext) => {
    const config = authContext?.getOpenApiConfiguration();

    client.setConfig({
      auth: config?.token ?? undefined,
      baseUrl: config?.base ?? '',
      credentials: config?.credentials ?? 'same-origin',
    });
  });

  extensionRegistry.registerMany(modalManifests);
};
