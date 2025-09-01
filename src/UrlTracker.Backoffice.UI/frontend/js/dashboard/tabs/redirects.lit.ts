import { IEditorService, editorServiceContext } from '@/context/editorservice.context';
import {
  IUmbracoNotificationsService,
  umbracoNotificationsServiceContext,
} from '@/context/notificationsservice.context';
import { redirectImportServiceContext } from '@/context/redirectimportservice.context';
import { REDIRECTTYPE_SORT_TYPE, RedirectSortType } from '@/enums/sortType';
import { IRedirectImportService } from '@/services/redirectimport.service';
import { LoadingStatus } from '@/types/loadingStatus';
import { DropdownChangeEvent, IDropdownValue } from '@/util/elements/inputs/dropdown.lit';
import { consume, provide } from '@lit/context';
import { Task } from '@lit/task';
import { LitElement, PropertyValueMap, css, html, nothing } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';
import { ifDefined } from 'lit/directives/if-defined.js';
import { Ref, createRef, ref } from 'lit/directives/ref.js';
import { repeat } from 'lit/directives/repeat.js';
import { IChangeManager, changeManagerContext } from '../../context/changemanager.context';
import { IRedirectService, redirectServiceContext } from '../../context/redirectservice.context';
import redirectService, {
  IRedirectCollectionResponse,
  IRedirectData,
  IRedirectResponse,
} from '../../services/redirect.service';
import '../../util/elements/bulkActions.lit';
import '../../util/elements/inputs/pagination.lit';
import { UrlTrackerPagination } from '../../util/elements/inputs/pagination.lit';
import '../../util/elements/inputs/redirectImport.lit';
import '../../util/elements/redirectActions.lit';
import '../../util/elements/resultlist.lit';
import '../../util/elements/resultlistitem.lit';
import { ensureExists, ensureServiceExists } from '../../util/tools/existancecheck';
import variableResource from '../../util/tools/variableresource.service';
import { UrlTrackerNotificationWrapper } from '../notifications/notifications.mixin';
import { createInspectRedirectEditor } from '../sidebars/inspectRedirect/inspectredirect';
import { createEditRedirectOptions, createNewRedirectOptions } from '../sidebars/simpleRedirect/manageredirect';
import './redirects/redirectitem.lit';
import './redirects/redirectitemSkeleton.lit';
import './redirects/redirectsSearch.lit';
import { IRedirectViewContext, redirectViewContext } from './redirects/redirectview.context';
import { ISourceStrategies } from './redirects/source/source.constants';

type Translations = {
  all: string;
  permanent: string;
  temporary: string;
  newRedirect: string;
  editRedirect: string;
  redirectDeleted: string;
  redirectDeletedMessage: string;
  redirectsExported: string;
  redirectsExportedMessage: string;
  templateDownloaded: string;
  templateDownloadedMessage: string;
  redirectsConverted: string;
  redirectsConvertedMessage: string;
  redirectsDeletedMultiple: string;
  redirectsDeletedMultipleMessage: string;
  redirectsImported: string;
  redirectsImportedMessage: string;
  results: string;
  export: string;
  convertToPermanent: string;
  deleteRedirect: string;
  type: string;
  exporting: string;
};
@customElement('urltracker-redirect-tab')
export class UrlTrackerRedirectTab extends UrlTrackerNotificationWrapper(LitElement) {
  @consume({ context: redirectServiceContext })
  private redirectService?: IRedirectService;

  @consume({ context: redirectImportServiceContext })
  private redirectImportService?: IRedirectImportService;

  @consume({ context: editorServiceContext })
  private editorService?: IEditorService<any>;

  @consume({ context: umbracoNotificationsServiceContext })
  private _notificationsService?: IUmbracoNotificationsService | undefined;
  public get notificationsService(): IUmbracoNotificationsService {
    ensureServiceExists(this._notificationsService, 'notificationsService');
    return this._notificationsService;
  }
  public set notificationsService(value: IUmbracoNotificationsService | undefined) {
    this._notificationsService = value;
  }

  @provide({ context: changeManagerContext })
  public changeManager: IChangeManager = { element: this };

  @provide({ context: redirectViewContext })
  public viewContext: IRedirectViewContext = { advanced: false };

  @property({ type: Boolean, reflect: true })
  public advanced: boolean = false;

  @state()
  private redirectCollection?: IRedirectCollectionResponse;

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
  private importRedirectLoading = false;

  @state()
  private deleteRedirectLoadingIds: Array<number> = [];

  @state()
  private translationTaskKey: number = 0;

  @state()
  private translations: Partial<Translations> = {};

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

  private _translationTask = new Task(this, {
    task: async (): Promise<Partial<Translations>> => {
      const [
        all,
        permanent,
        temporary,
        newRedirect,
        editRedirect,
        redirectDeleted,
        redirectDeletedMessage,
        redirectsExported,
        redirectsExportedMessage,
        templateDownloaded,
        templateDownloadedMessage,
        redirectsConverted,
        redirectsConvertedMessage,
        redirectsDeletedMultiple,
        redirectsDeletedMultipleMessage,
        redirectsImported,
        redirectsImportedMessage,
        deleteRedirect,
        results,
        exportRedirects,
        exportingRedirects,
        convertToPermanent,
        type,
      ] = await Promise.all([
        this.localizationService?.localize('urlTrackerGeneral_all'),
        this.localizationService?.localize('urlTrackerGeneral_permanent'),
        this.localizationService?.localize('urlTrackerGeneral_temporary'),
        this.localizationService?.localize('urlTrackerGeneral_new-redirect'),
        this.localizationService?.localize('urlTrackerGeneral_edit-redirect'),
        this.localizationService?.localize('urlTrackerGeneral_redirect-deleted'),
        this.localizationService?.localize('urlTrackerGeneral_redirect-deleted-message'),
        this.localizationService?.localize('urlTrackerGeneral_redirects-exported'),
        this.localizationService?.localize('urlTrackerGeneral_redirects-exported-message'),
        this.localizationService?.localize('urlTrackerGeneral_template-downloaded'),
        this.localizationService?.localize('urlTrackerGeneral_template-downloaded-message'),
        this.localizationService?.localize('urlTrackerGeneral_redirects-converted'),
        this.localizationService?.localize('urlTrackerGeneral_redirects-converted-message'),
        this.localizationService?.localize('urlTrackerGeneral_redirects-deleted-multiple'),
        this.localizationService?.localize('urlTrackerGeneral_redirects-deleted-multiple-message'),
        this.localizationService?.localize('urlTrackerGeneral_redirects-imported'),
        this.localizationService?.localize('urlTrackerGeneral_redirects-imported-message'),
        this.localizationService?.localize('urlTrackerGeneral_delete'),
        this.localizationService?.localize('urlTrackerGeneral_results'),
        this.localizationService?.localize('urlTrackerRedirectActions_export'),
        this.localizationService?.localize('urlTrackerRedirectActions_exporting'),
        this.localizationService?.localize('urlTrackerRedirectActions_convert-to-permanent'),
        this.localizationService?.localize('urlTrackerGeneral_type'),
      ]);

      const translations: Partial<Translations> = {
        all,
        permanent,
        temporary,
        newRedirect,
        editRedirect,
        redirectDeleted,
        redirectDeletedMessage,
        redirectsExported,
        redirectsExportedMessage,
        templateDownloaded,
        templateDownloadedMessage,
        redirectsConverted,
        redirectsConvertedMessage,
        redirectsDeletedMultiple,
        redirectsDeletedMultipleMessage,
        redirectsImported,
        redirectsImportedMessage,
        results,
        export: exportRedirects,
        exporting: exportingRedirects,
        convertToPermanent,
        deleteRedirect,
        type,
      };

      this.translations = translations;

      return translations;
    },
    args: () => [this.translationTaskKey],
  });

  private get sortOptions(): IDropdownValue[] {
    return [
      {
        display: this.translations.all || 'All',
        value: REDIRECTTYPE_SORT_TYPE.ALL,
        key: REDIRECTTYPE_SORT_TYPE.ALL.toString(),
      },
      {
        display: this.translations.permanent || 'Permanent',
        value: REDIRECTTYPE_SORT_TYPE.PERMANENT,
        key: REDIRECTTYPE_SORT_TYPE.PERMANENT.toString(),
      },
      {
        display: this.translations.temporary || 'Temporary',
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
    ensureServiceExists(this.redirectService, 'redirect service');
    ensureServiceExists(this.redirectImportService, 'redirect import service');
    ensureServiceExists(this.editorService, 'editor service');
    await this.search();
  }

  private async search() {
    this.redirectCollection = undefined;
    ensureExists(this.paginationRef.value);

    const page = {
      page: this.paginationRef.value!.value.page + 1,
      pageSize: this.paginationRef.value!.value.pageSize,
    };
    const type = this.selectedType;
    const query = this.query;

    this.loading++;
    try {
      this.redirectCollection = await this.redirectService?.list({
        ...page,
        types: type,
        query,
        sourceTypes: this.redirectTypes,
        advanced: this.advanced,
      });
    } finally {
      this.loading--;
    }
  }

  private openInspectPanel(data: IRedirectResponse) {
    const options = createInspectRedirectEditor({
      close: this.closePanel,
      redirect: data,
    });
    this.editorService!.open(options);
  }

  private openNewRedirectPanel(data?: IRedirectData) {
    const options = createNewRedirectOptions({
      title: this.translations.newRedirect || 'New redirect',
      submit: this.submitNewRedirectPanel,
      close: this.closePanel,
      advanced: this.viewContext.advanced,
      sourceEditable: true,
      data: data,
    });

    this.editorService!.open(options);
  }

  private openEditRedirectPanel(id: number, data: IRedirectData) {
    const options = createEditRedirectOptions({
      title: (this.translations.editRedirect || 'Edit') + ' ' + data.source.value,
      submit: this.submitNewRedirectPanel,
      close: this.closePanel,
      advanced: this.viewContext.advanced,
      sourceEditable: true,
      data: data,
      id: id,
    });

    this.editorService!.open(options);
  }

  private submitNewRedirectPanel = (_: IRedirectResponse) => {
    this.closePanel();
    this.search();
  };

  private closePanel = () => {
    this.editorService!.close();
  };

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

  private onInspect = (e: CustomEvent<IRedirectResponse>) => {
    this.openInspectPanel(e.detail);
  };

  private onAddRedirect = (_: any) => {
    this.openNewRedirectPanel();
  };

  private onEditRedirect = (e: CustomEvent<IRedirectResponse>) => {
    this.openEditRedirectPanel(e.detail.id, e.detail);
  };

  private onDeleteRedirect = async (e: CustomEvent<IRedirectResponse>) => {
    this.deleteRedirectLoadingIds.push(e.detail.id);
    this.requestUpdate();

    try {
      await this.redirectService?.delete(e.detail.id);
      this.notificationsService.success(
        this.translations.redirectDeleted || 'Redirect deleted',
        this.translations.redirectDeletedMessage || 'The redirect has been successfully deleted',
      );
      this.search();
    } finally {
      this.deleteRedirectLoadingIds = this.deleteRedirectLoadingIds.filter((id) => id !== e.detail.id);
      this.requestUpdate();
    }
  };

  private onExportRedirects = async (_: any) => {
    this.exportState = 'waiting';
    try {
      await this.redirectImportService?.export();
      this.notificationsService.success(
        this.translations.redirectsExported || 'Redirects exported',
        this.translations.redirectsExportedMessage || 'Check your downloads to find the exported redirects',
      );
    } finally {
      this.exportState = undefined;
    }
  };

  private onImportRedirects = async (e: CustomEvent<File>) => {
    this.importRedirectLoading = true;
    try {
      await this.redirectImportService!.import(e.detail);
      this.notificationsService.success(
        this.translations.redirectsImported || 'Redirects imported',
        this.translations.redirectsImportedMessage || 'The redirects have been successfully imported',
      );
    } catch (error) {
      this.notificationsService.error('Redirects import failed', 'The redirects have not been imported');
    } finally {
      this.importRedirectLoading = false;
    }
    this.search();
  };

  private onDownloadImportTemplate = async () => {
    await this.redirectImportService!.exportTemplate();
    this.notificationsService.success(
      this.translations.templateDownloaded || 'Template downloaded',
      this.translations.templateDownloadedMessage || 'Check your downloads to find the template',
    );
  };

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
      this.selectedItems = this.redirectCollection?.results.map((r) => r.id) || [];
    }
  };

  private onClearSelection = (_: any) => {
    this.selectedItems = [];
  };

  private onConvertSelection = async (_: any) => {
    this.convertSelectionState = 'waiting';
    try {
      const selectedRedirects =
        this.redirectCollection?.results.filter((r) => this.selectedItems.some((i) => i === r.id)) || [];
      const bulkToUpdate = selectedRedirects.map((r) => {
        const { id: _, key: __, additionalData: ___, createDate: ____, updateDate: _____, ...data } = r;
        return {
          id: r.id,
          data: data,
        };
      });
      await redirectService.updateBulk(bulkToUpdate);
      this.notificationsService.success(
        this.translations.redirectsConverted || 'Redirects converted to permanent',
        this.translations.redirectsConvertedMessage ||
          'The selected redirects have been successfully converted to permanent',
      );
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
        this.redirectCollection?.results.filter((r) => this.selectedItems.some((i) => i === r.id)) || [];
      const bulkToDelete = selectedRedirects.map((r) => r.id);
      await redirectService.deleteBulk(bulkToDelete);
      this.notificationsService.success(
        this.translations.redirectsDeletedMultiple || 'Redirects deleted',
        this.translations.redirectsDeletedMultipleMessage || 'The selected redirects have been successfully deleted',
      );
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
          <uui-icon name="lock"></uui-icon> ${this.translations.convertToPermanent}
        </uui-button>
        <uui-button look="secondary" .state="${this.deleteSelectionState}" @click=${this.onDeleteSelection}>
          <uui-icon name="delete"></uui-icon>
          ${this.translations.deleteRedirect || 'Delete'}
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
      (redirect) => redirect.id,
      (r) =>
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
          .label="${this.translations.type}"
          .options=${this.sortOptions}
          @change=${this.onTypeChange}
        ></urltracker-dropdown>
      </div>
    `;
  }

  protected renderExport(): unknown {
    if (this.exportState === 'waiting') {
      return html`<uui-menu-item .label="${this.translations.exporting || 'Exporting redirects'}">
        <uui-loader-circle slot="icon"></uui-loader-circle
      ></uui-menu-item>`;
    } else {
      return html`<uui-menu-item
        .label="${this.translations.export || 'Export redirects'}"
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
          ${this._translationTask.render({
            complete: () => html`
              <urltracker-result-list
                .loading=${!!this.loading}
                .header=${`${this.translations.results} (${this.redirectCollection ? this.redirectCollection.total : 0})`}
              >
                ${this.renderRedirects()}
              </urltracker-result-list>
            `,
          })}
          <urltracker-pagination
            ${ref(this.paginationRef)}
            class="pagination"
            total="${ifDefined(this.redirectCollection?.total)}"
            @change=${this.onFilterChange}
          ></urltracker-pagination>
        </div>
        ${this._translationTask.render({
          complete: () => html`
            <div class="functions">
              <urltracker-redirect-actions>
                <uui-menu-item
                  .label="${this.translations.newRedirect || 'New redirect'}"
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
          `,
        })}
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
