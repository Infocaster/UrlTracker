import '@umbraco-ui/uui';
import { LitElement, css, html } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { Ref, createRef, ref } from 'lit/directives/ref.js';
import { IPaginationRequestBase } from '../../../services/models/paginationrequestbase';
import { ensureExists } from '../../tools/existancecheck';
import { DropdownChangeEvent, IDropdownValue, UrlTrackerDropdown } from './dropdown.lit';
import './pageselect.lit';
import { UrlTrackerPageSelect, UrlTrackerPageSelectEvent } from './pageselect.lit';
export class PaginationEvent extends Event {
  static event = 'change';
  constructor(
    public data: IPaginationRequestBase,
    eventInitDict?: EventInit,
  ) {
    super(PaginationEvent.event, eventInitDict);
  }
}
@customElement('urltracker-pagination')
export class UrlTrackerPagination extends LitElement {
  private pageSizes: IDropdownValue[] = [
    {
      display: '10',
      value: 10,
      key: '10',
    },
    {
      display: '25',
      value: 25,
      key: '25',
    },
    {
      display: '50',
      value: 50,
      key: '50',
    },
    {
      display: '100',
      value: 100,
      key: '100',
    },
    {
      display: '200',
      value: 200,
      key: '200',
    },
  ];
  private paginationRef: Ref<UrlTrackerPageSelect> = createRef();
  private dropdownRef: Ref<UrlTrackerDropdown> = createRef();
  private formRef: Ref<HTMLFormElement> = createRef();
  @property({ type: Number })
  public total: number = 0;
  @property({ type: Object })
  public get value(): IPaginationRequestBase {
    if (!this.formRef.value) {
      return {
        page: 0,
        pageSize: this.pageSizes[0].value as number,
      };
    }
    const formData = new FormData(this.formRef.value);
    const page = formData.get('page')?.toString();
    const pageSizeKey = formData.get('pageSize')?.toString();
    const pageSizeChoice = this.pageSizes.find((ps) => ps.key === pageSizeKey);

    ensureExists(pageSizeChoice, 'The page size must be one of the available options');

    return {
      page: page ? parseInt(page) - 1 : 0,
      pageSize: pageSizeChoice.value as number,
    };
  }
  public set value(val: IPaginationRequestBase) {
    ensureExists(this.paginationRef.value);
    ensureExists(this.dropdownRef.value);
    const oldVal = this.value;
    this.paginationRef.value.value = val.page + 1;
    this.dropdownRef.value.value = (this.pageSizes.find((el) => el.value === val.pageSize) ?? this.pageSizes[0]).key;
    this.requestUpdate('value', oldVal);
  }
  private get totalPages(): number {
    return Math.ceil(this.total / this.value.pageSize);
  }
  private onPageChange = (e: Event) => {
    if (!(e instanceof UrlTrackerPageSelectEvent)) return;
    this.dispatchEvent(new PaginationEvent(this.value));
  };
  private onPageSizeChange = (e: Event) => {
    if (!(e instanceof DropdownChangeEvent)) return;
    ensureExists(this.paginationRef.value);
    this.paginationRef.value.value = 1;
    this.requestUpdate('totalPages');
    this.dispatchEvent(new PaginationEvent(this.value));
  };
  protected render(): unknown {
    return html`
      <form ${ref(this.formRef)}>
        <urltracker-pageselect
          ${ref(this.paginationRef)}
          name="page"
          .total=${this.totalPages}
          @change=${this.onPageChange}
        ></urltracker-pageselect>
        <urltracker-dropdown
          ${ref(this.dropdownRef)}
          name="pageSize"
          label="Results"
          .options=${this.pageSizes}
          @change=${this.onPageSizeChange}
        ></urltracker-dropdown>
      </form>
    `;
  }
  static styles = css`
    form {
      display: flex;
      flex-direction: row;
      justify-content: space-between;
      align-items: center;
      gap: 16px;
    }

    urltracker-pageselect {
      background-color: white;
    }
  `;
}
