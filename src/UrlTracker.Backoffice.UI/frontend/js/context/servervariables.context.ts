import { UmbObjectState } from '@umbraco-cms/backoffice/observable-api';

export interface ServerVariables {
  redirectSourceStrategies: {
    url: string;
    regex: string;
  };
  redirectTargetStrategies: {
    url: string;
    content: string;
    media: string;
  };
  recommendationTypeStrategies: {
    image: string;
    file: string;
    page: string;
    technicalFile: string;
  };
}

export class UmbServerVariableContext {
  #serverVariables = new UmbObjectState<Partial<ServerVariables>>({});

  public readonly serverVariables = this.#serverVariables;

  constructor() {
    this.#serverVariables.setValue({
      ...this.#serverVariables.getValue(),
      recommendationTypeStrategies: {
        image: 'b3454454-ee3f-4c99-889f-c570eb096544',
        file: 'bf6b11a6-196b-4003-ae61-b4b5525f9110',
        page: 'a23ed85f-d803-4850-baf4-c02203c49b93',
        technicalFile: 'e1de5e1b-c4f4-42ae-a50e-4e0ce3348e62',
      },
      redirectSourceStrategies: {
        url: '6406121c-766c-49eb-a510-8d979ea27ff9',
        regex: '6dd69e53-b8dc-4a4e-84fb-e76c758bc8a5',
      },
      redirectTargetStrategies: {
        url: '603c97d6-57c8-442f-8631-a8fae383fbf5',
        content: 'e9e3a702-54f7-42ae-aadd-5b04185da988',
        media: '376c08bf-c93c-4298-b4b8-61a933c9f99c',
      },
    });
  }

  destroy(): void {
    // Cleanup if needed
  }
}

// export const UMB_SERVER_VARIABLE_CONTEXT = new UmbContextToken<UmbServerVariableContext>('umbServerVariableContext');

export default UmbServerVariableContext;
