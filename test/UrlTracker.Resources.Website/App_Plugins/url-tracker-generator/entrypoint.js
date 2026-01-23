import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";
import { client } from "./api-client/client.gen";

export const onInit = (host) => {
  host.consumeContext(UMB_AUTH_CONTEXT, (authContext) => {
    // Get the token info from Umbraco
    const config = authContext?.getOpenApiConfiguration();

    client.setConfig({
      auth: config?.token ?? undefined,
      baseUrl: config?.base ?? "",
      credentials: config?.credentials ?? "same-origin",
    });
  });
};
