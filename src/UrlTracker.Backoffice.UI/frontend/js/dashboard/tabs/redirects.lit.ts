import { IEditorService, editorServiceContext } from '@/context/editorservice.context';
import { REDIRECTTYPE_SORT_TYPE, RedirectSortType } from '@/enums/sortType';
import { LoadingStatus } from '@/types/loadingStatus';
import { DropdownChangeEvent, IDropdownValue } from '@/util/elements/inputs/dropdown.lit';
import { consume, provide } from '@lit/context';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { umbOpenModal } from '@umbraco-cms/backoffice/modal';
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { LitElement, PropertyValueMap, css, html, nothing } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { ifDefined } from 'lit/directives/if-defined.js';
import { Ref, createRef, ref } from 'lit/directives/ref.js';
import { repeat } from 'lit/directives/repeat.js';
import type { Client } from '../../../../api-client/client/types.gen';
import {
  getUmbracoManagementApiV1UrlTrackerRedirectImportExportexample,
  getUmbracoManagementApiV1UrlTrackerRedirects,
  postUmbracoManagementApiV1UrlTrackerRedirectsByRedirectIdDelete,
  postUmbracoManagementApiV1UrlTrackerRedirectsDeletebulk,
  postUmbracoManagementApiV1UrlTrackerRedirectsUpdatebulk,
} from '../../../../api-client/sdk.gen';
import type { RedirectRequest } from '../../../../api-client/types.gen';
import { RedirectCollectionResponse, RedirectResponse } from '../../../../api-client/types.gen';
import { IChangeManager, changeManagerContext } from '../../context/changemanager.context';
import '../../util/elements/bulkActions.lit';
import '../../util/elements/inputs/pagination.lit';
import { UrlTrackerPagination } from '../../util/elements/inputs/pagination.lit';
import '../../util/elements/inputs/redirectImport.lit';
import '../../util/elements/redirectActions.lit';
import '../../util/elements/resultlist.lit';
import '../../util/elements/resultlistitem.lit';
import variableResource from '../../util/tools/variableresource.service';
import { UrlTrackerNotificationWrapper } from '../notifications/notifications.mixin';
import { URL_TRACKER_INSPECT_REDIRECT_MODAL } from '../sidebars/inspectRedirect-modal.token';
import { URL_TRACKER_SIMPLE_REDIRECT_MODAL } from '../sidebars/simpleRedirect-modal.token';
import './redirects/redirectitem.lit';
import './redirects/redirectitemSkeleton.lit';
import './redirects/redirectsSearch.lit';
import { IRedirectViewContext, redirectViewContext } from './redirects/redirectview.context';
import { ISourceStrategies } from './redirects/source/source.constants';

@customElement('urltracker-redirect-tab')
export class UrlTrackerRedirectTab extends UrlTrackerNotificationWrapper(UmbElementMixin(LitElement), 'redirect') {
  @consume({ context: editorServiceContext })
  private editorService?: IEditorService<any>;

  @provide({ context: changeManagerContext })
  public changeManager: IChangeManager = { element: this };

  @provide({ context: redirectViewContext })
  public viewContext: IRedirectViewContext = { advanced: false };

  @property({ type: Boolean, reflect: true })
  public advanced: boolean = false;

  @state()
  private redirectCollection?: RedirectCollectionResponse;

  @state()
  private loading: number = 0;

  @state()
  private selectedItems: number[] = [];

  @state()
  private exportState: LoadingStatus = undefined;

  @state()
  private deleteSelectionState: LoadingStatus = undefined;

  @state()
  private convertSelectionState: LoadingStatus = undefined;

  @state()
  private deleteRedirectLoadingIds: Array<number> = [];

  #notificationContext?: typeof UMB_NOTIFICATION_CONTEXT.TYPE;

  private get redirectTypes(): string[] | undefined {
    if (this.viewContext.advanced) {
      return undefined;
    }

    const urlStrategy = variableResource.get<ISourceStrategies>('redirectSourceStrategies').url;
    return [urlStrategy];
  }

  private query = '';
  private selectedType: RedirectSortType = REDIRECTTYPE_SORT_TYPE.ALL;
  private paginationRef: Ref<UrlTrackerPagination> = createRef();

  constructor() {
    super();

    this.consumeContext(UMB_NOTIFICATION_CONTEXT, (context) => {
      this.#notificationContext = context;
    });
  }

  private get sortOptions(): IDropdownValue[] {
    return [
      {
        display: this.localize.term('urlTrackerGeneral_all') || 'All',
        value: REDIRECTTYPE_SORT_TYPE.ALL,
        key: REDIRECTTYPE_SORT_TYPE.ALL.toString(),
      },
      {
        display: this.localize.term('urlTrackerGeneral_permanent') || 'Permanent',
        value: REDIRECTTYPE_SORT_TYPE.PERMANENT,
        key: REDIRECTTYPE_SORT_TYPE.PERMANENT.toString(),
      },
      {
        display: this.localize.term('urlTrackerGeneral_temporary') || 'Temporary',
        value: REDIRECTTYPE_SORT_TYPE.TEMPORARY,
        key: REDIRECTTYPE_SORT_TYPE.TEMPORARY.toString(),
      },
    ];
  }

  protected async firstUpdated(_changedProperties: PropertyValueMap<any> | Map<PropertyKey, unknown>): Promise<void> {
    super.firstUpdated(_changedProperties);
    await this.init();
  }

  private async init() {
    await this.search();
  }

  private async search() {
    this.redirectCollection = undefined;
    // ensureExists(this.paginationRef.value);

    const page = {
      page: this.paginationRef.value!.value.page + 1,
      pageSize: this.paginationRef.value!.value.pageSize,
    };
    const type = this.selectedType;
    const query = this.query;

    this.loading++;
    try {
      const { data } = await tryExecute(
        this,
        getUmbracoManagementApiV1UrlTrackerRedirects({
          client: umbHttpClient as unknown as Client,
          query: {
            Page: page.page,
            PageSize: page.pageSize,
            Query: query,
            SourceTypes: this.redirectTypes,
            Advanced: this.advanced,
          },
        }),
      );

      this.redirectCollection = data as RedirectCollectionResponse;
    } finally {
      this.loading--;
    }
  }

  private openInspectPanel(data: RedirectResponse) {
    umbOpenModal(this, URL_TRACKER_INSPECT_REDIRECT_MODAL, {
      data: {
        redirect: data,
      },
    })
      .then(() => {
        this.closePanel();
      })
      .catch(() => this.closePanel());
  }

  private openNewRedirectPanel(data?: RedirectRequest) {
    umbOpenModal(this, URL_TRACKER_SIMPLE_REDIRECT_MODAL, {
      data: {
        title: this.localize.term('urlTrackerGeneral_new-redirect') || 'New redirect',
        advanced: this.viewContext.advanced,
        sourceEditable: true,
        data: data,
      },
    })
      .then(() => this.search())
      .catch(() => undefined);
  }

  private async openEditRedirectPanel(id: number, data: RedirectRequest) {
    umbOpenModal(this, URL_TRACKER_SIMPLE_REDIRECT_MODAL, {
      data: {
        title: (this.localize.term('urlTrackerGeneral_editRedirect') || 'Edit') + ' ' + data.source.value,
        advanced: this.viewContext.advanced,
        sourceEditable: true,
        data: data,
        id: id,
      },
    })
      .then((result) => {
        this.search();
      })
      .catch((error) => {
        // Modal was cancelled or failed
      });
  }

  private closePanel = () => {};

  private onSearch = ({ detail: { query } = {} }: CustomEvent) => {
    this.query = query;
    this.search();
  };

  private onTypeChange = ({ data }: DropdownChangeEvent) => {
    this.selectedType = data.value as RedirectSortType;
    this.search();
  };

  private onFilterChange = (_: Event) => {
    this.selectedItems = [];
    this.search();
  };

  private onInspect = (e: CustomEvent<RedirectResponse>) => {
    this.openInspectPanel(e.detail as unknown as RedirectResponse);
  };

  private onAddRedirect = (_: any) => {
    this.openNewRedirectPanel();
  };

  private onEditRedirect = (e: CustomEvent<RedirectResponse>) => {
    this.openEditRedirectPanel(e.detail.id, e.detail);
  };

  private onDeleteRedirect = async (e: CustomEvent<RedirectResponse>) => {
    this.deleteRedirectLoadingIds.push(e.detail.id);
    this.requestUpdate();

    try {
      await tryExecute(
        this,
        postUmbracoManagementApiV1UrlTrackerRedirectsByRedirectIdDelete({
          client: umbHttpClient as unknown as Client,
          path: { redirectId: e.detail.id },
        }),
      );
      this.#notificationContext?.peek('positive', {
        data: {
          headline: this.localize.term('urlTrackerGeneral_redirectDeleted') || 'Redirect deleted',
          message:
            this.localize.term('urlTrackerGeneral_redirectDeletedMessage') ||
            'The redirect has been successfully deleted',
        },
      });
      this.search();
    } finally {
      this.deleteRedirectLoadingIds = this.deleteRedirectLoadingIds.filter((id) => id !== e.detail.id);
      this.requestUpdate();
    }
  };

  private onExportRedirects = async (_: any) => {
    this.exportState = 'waiting';
    try {
      // TODO: Replace with direct client export call
      // await this.redirectImportService?.export();
      this.#notificationContext?.peek('positive', {
        data: {
          headline: this.localize.term('urlTrackerGeneral_redirectsExported') || 'Redirects exported',
          message:
            this.localize.term('urlTrackerGeneral_redirectsExportedMessage') ||
            'Check your downloads to find the exported redirects',
        },
      });
    } finally {
      this.exportState = undefined;
    }
  };

  private onImportRedirects = async (e: CustomEvent<File>) => {
    try {
      // TODO: Replace with direct client import call
      // await this.redirectImportService!.import(e.detail);
      this.#notificationContext?.peek('positive', {
        data: {
          headline: this.localize.term('urlTrackerGeneral_redirectsImported') || 'Redirects imported',
          message:
            this.localize.term('urlTrackerGeneral_redirectsImportedMessage') ||
            'The redirects have been successfully imported',
        },
      });
    } catch (error) {
      this.#notificationContext?.peek('danger', {
        data: {
          headline: 'Redirects import failed',
          message: 'The redirects have not been imported',
        },
      });
    }
    this.search();
  };

  private onDownloadImportTemplate = async () => {
    // TODO: Replace with direct client export template call
    // await this.redirectImportService!.exportTemplate();

    try {
      const { data } = await tryExecute(
        this,
        getUmbracoManagementApiV1UrlTrackerRedirectImportExportexample({
          client: umbHttpClient as unknown as Client,
        }),
      );

      if (!data) throw new Error('Export failed');

      this.downloadBlob(data, 'redirect-template');

      this.#notificationContext?.peek('positive', {
        data: {
          headline: this.localize.term('urlTrackerGeneral_templateDownloaded') || 'Template downloaded',
          message:
            this.localize.term('urlTrackerGeneral_templateDownloadedMessage') ||
            'Check your downloads to find the template',
        },
      });
    } catch {
      this.#notificationContext?.peek('danger', {
        data: {
          headline: 'error',
          message: 'error',
        },
      });
    }
  };

  private downloadBlob(response: File | Blob, fileName: string): void {
    const blob = new Blob([response], { type: 'text/csv;charset=utf-8;' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', fileName + '.csv');
    document.body.appendChild(link);
    link.click();
  }

  private onSelectItem = (e: any) => {
    this.selectedItems.push(e.item.id);
    this.requestUpdate();
  };

  private onDeselectItem = (e: any) => {
    this.selectedItems = this.selectedItems.filter((i) => i !== e.item.id);
  };

  private onSelectAll = (_: any) => {
    if (this.selectedItems.length === this.redirectCollection?.total) {
      this.selectedItems = [];
    } else {
      this.selectedItems = this.redirectCollection?.results.map((r: RedirectResponse) => r.id) || [];
    }
  };

  private onClearSelection = (_: any) => {
    this.selectedItems = [];
  };

  private onConvertSelection = async (_: any) => {
    this.convertSelectionState = 'waiting';
    try {
      const selectedRedirects =
        this.redirectCollection?.results?.filter((r: RedirectResponse) => this.selectedItems.some((i) => i === r.id)) ||
        [];
      const bulkToUpdate = selectedRedirects.map((r: any) => {
        const { id: _, key: __, additionalData: ___, createDate: ____, updateDate: _____, ...data } = r;
        return {
          id: r.id,
          data: data,
        };
      });
      await tryExecute(
        this,
        postUmbracoManagementApiV1UrlTrackerRedirectsUpdatebulk({
          client: umbHttpClient as unknown as Client,
          body: bulkToUpdate,
        }),
      );
      this.#notificationContext?.peek('positive', {
        data: {
          headline: this.localize.term('urlTrackerGeneral_redirectsConverted') || 'Redirects converted to permanent',
          message:
            this.localize.term('urlTrackerGeneral_redirectsConvertedMessage') ||
            'The selected redirects have been successfully converted to permanent',
        },
      });
      this.selectedItems = [];
      this.search();

      this.convertSelectionState = undefined;
    } catch {
      this.convertSelectionState = 'failed';
    }
  };

  private onDeleteSelection = async (_: any) => {
    this.deleteSelectionState = 'waiting';

    try {
      const selectedRedirects =
        this.redirectCollection?.results.filter((r: RedirectResponse) => this.selectedItems.some((i) => i === r.id)) ||
        [];
      const bulkToDelete = selectedRedirects.map((r: any) => r.id);
      await tryExecute(
        this,
        postUmbracoManagementApiV1UrlTrackerRedirectsDeletebulk({
          client: umbHttpClient as unknown as Client,
          body: bulkToDelete,
        }),
      );
      this.#notificationContext?.peek('positive', {
        data: {
          headline: this.localize.term('urlTrackerGeneral_redirectsDeletedMultiple') || 'Redirects deleted',
          message:
            this.localize.term('urlTrackerGeneral_redirectsDeletedMultipleMessage') ||
            'The selected redirects have been successfully deleted',
        },
      });
      this.selectedItems = [];
      this.search();

      this.deleteSelectionState = undefined;
    } catch {
      this.deleteSelectionState = 'failed';
    }
  };

  connectedCallback(): void {
    super.alias = this.advanced ? 'advancedredirects' : 'redirects';
    super.connectedCallback();

    this.viewContext = { advanced: this.advanced };
  }

  private renderBulkActions() {
    if (!this.selectedItems.length) return nothing;
    return html`
      <urltracker-bulk-actions
        class="bulk"
        .selectedCount=${this.selectedItems.length}
        .total=${this.redirectCollection ? this.redirectCollection.total : 0}
        @select-all=${this.onSelectAll}
        @clear-selection=${this.onClearSelection}
      >
        <uui-button look="secondary" .state="${this.convertSelectionState}" @click=${this.onConvertSelection}>
          <uui-icon name="lock"></uui-icon>
          <umb-localize key="urlTrackerGeneral_convertToPermanent">Convert to permanent redirect</umb-localize>
        </uui-button>
        <uui-button look="secondary" .state="${this.deleteSelectionState}" @click=${this.onDeleteSelection}>
          <uui-icon name="delete"></uui-icon>
          <umb-localize key="urlTrackerGeneral_deleteRedirect">Delete redirect</umb-localize>
        </uui-button>
      </urltracker-bulk-actions>
    `;
  }

  private renderRedirects(): unknown {
    if (this.loading) {
      return html`<urltracker-redirect-item-skeleton></urltracker-redirect-item-skeleton>
        <urltracker-redirect-item-skeleton></urltracker-redirect-item-skeleton>
        <urltracker-redirect-item-skeleton></urltracker-redirect-item-skeleton> `;
    }

    if (!this.redirectCollection?.results) return nothing;

    return repeat(
      this.redirectCollection.results,
      (redirect: any) => redirect.id,
      (r: any) =>
        html` <urltracker-redirect-item
          .item=${r}
          .isSelected=${this.selectedItems.some((i) => i === r.id)}
          .deleteRedirectLoading=${this.deleteRedirectLoadingIds.includes(r.id)}
          @selected=${this.onSelectItem}
          @deselected=${this.onDeselectItem}
          @inspect=${this.onInspect}
          @edit=${this.onEditRedirect}
          @delete=${this.onDeleteRedirect}
        ></urltracker-redirect-item>`,
    );
  }

  protected renderFilters() {
    if (this.selectedItems.length) return nothing;
    return html`
      <div class="filters">
        <urltracker-redirects-search @search=${this.onSearch}></urltracker-redirects-search>
        <urltracker-dropdown
          .label="${this.localize.term('urlTrackerGeneral_type') || 'Type'}"
          .options=${this.sortOptions}
          @change=${this.onTypeChange}
        ></urltracker-dropdown>
      </div>
    `;
  }

  protected renderExport(): unknown {
    if (this.exportState === 'waiting') {
      return html`<uui-menu-item
        .label="${this.localize.term('urlTrackerRedirectActions_exporting') || 'Exporting redirects'}"
      >
        <uui-loader-circle slot="icon"></uui-loader-circle
      ></uui-menu-item>`;
    } else {
      return html`<uui-menu-item
        .label="${this.localize.term('urlTrackerRedirectActions_export') || 'Export redirects'}"
        @click-label=${this.onExportRedirects}
      >
        <uui-icon slot="icon" name="download"></uui-icon>
      </uui-menu-item>`;
    }
  }

  protected renderInternal(): unknown {
    return html`
      <div class="grid-root">
        ${this.renderFilters()} ${this.renderBulkActions()}

        <div class="results">
          <urltracker-result-list
            .loading=${!!this.loading}
            .header=${`${this.localize.term('urlTrackerGeneral_results') || 'Results'} (${this.redirectCollection ? this.redirectCollection.total : 0})`}
          >
            ${this.renderRedirects()}
          </urltracker-result-list>
          <urltracker-pagination
            ${ref(this.paginationRef)}
            class="pagination"
            total="${ifDefined(this.redirectCollection?.total)}"
            @change=${this.onFilterChange}
          ></urltracker-pagination>
        </div>
        <div class="functions">
          <urltracker-redirect-actions>
            <uui-menu-item
              .label="${this.localize.term('urlTrackerGeneral_new-redirect') || 'New redirect'}"
              @click-label=${this.onAddRedirect}
            >
              <uui-icon slot="icon" name="add"></uui-icon>
            </uui-menu-item>
            ${this.renderExport()}
          </urltracker-redirect-actions>
          <urltracker-redirect-import
            @import=${this.onImportRedirects}
            @download-template=${this.onDownloadImportTemplate}
          ></urltracker-redirect-import>
        </div>
      </div>
    `;
  }

  static styles = css`
    .grid-root {
      display: grid;
      gap: 1rem;
      grid-template-columns: 1fr 360px;
    }

    .main {
      display: flex;
      margin-bottom: 2rem;
      gap: 2rem;
      flex-wrap: wrap;
    }

    .filters {
      grid-column: 1 / span 2;
      grid-row: 1;
      display: flex;
      align-items: center;
      gap: 1rem;
      padding: 1rem 0;
    }

    .filters urltracker-redirects-search {
      flex: 0 1 30%;
    }

    .bulk {
      grid-column: 1 / span 2;
      grid-row: 1;
    }

    .results {
      display: flex;
      flex-direction: column;
      gap: 1rem;
      min-width: 0;
    }

    urltracker-result-list {
      flex: 1 1 32rem;
    }

    .functions {
      flex: 1 0 15rem;
      display: flex;
      flex-direction: column;
      gap: 1rem;
    }
  `;
}
