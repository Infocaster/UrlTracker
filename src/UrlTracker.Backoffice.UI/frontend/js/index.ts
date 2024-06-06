import { UmbEntryPointOnInit } from '@umbraco-cms/backoffice/extension-api';
import { dashboardManifests } from '@/dashboard/manifests';
import { manifests as scoringManifests } from './services/scoring/manifest';
import { UMB_AUTH_CONTEXT } from '@umbraco-cms/backoffice/auth';
import { OpenAPI } from './api';

export const onInit: UmbEntryPointOnInit = (_host, extensionRegistry) => {
  extensionRegistry.registerMany([...dashboardManifests, ...scoringManifests]);

  _host.consumeContext(UMB_AUTH_CONTEXT, (authContext) => {
    const config = authContext.getOpenApiConfiguration();

    OpenAPI.BASE = config.base;
    OpenAPI.WITH_CREDENTIALS = config.withCredentials;
    OpenAPI.CREDENTIALS = config.credentials;
    OpenAPI.TOKEN = config.token;
  });
};
