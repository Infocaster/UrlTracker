import { LitElement, css, html } from "lit";
import { customElement, property } from "lit/decorators.js";
import { IPaginationRequestBase } from "../../../services/models/paginationrequestbase";
import { UUIPaginationElement, UUIPaginationEvent } from "@umbraco-ui/uui";
import '@umbraco-ui/uui';
import { DropdownChangeEvent, IDropdownValue, UrlTrackerDropdown } from "./dropdown.lit";
import { Ref, createRef, ref } from "lit/directives/ref.js";
import { ensureExists } from "../../tools/existancecheck";

export class PaginationEvent extends Event {

    static event = "change";

    constructor(public data: IPaginationRequestBase, eventInitDict?: EventInit) {
        super(PaginationEvent.event, eventInitDict)
    }
}

@customElement('urltracker-pagination')
export class UrlTrackerPagination extends LitElement {

    private pageSizes: IDropdownValue[] = [
        {
            display: "10",
            value: 10
        },
        {
            display: "25",
            value: 25
        },
        {
            display: "50",
            value: 50
        },
        {
            display: "100",
            value: 100
        },
        {
            display: "200",
            value: 200
        }
    ];

    @property()
    public get value(): IPaginationRequestBase {

        return {
            page: this.paginationRef.value ? this.paginationRef.value.current - 1 : 0,
            pageSize: (this.dropdownRef.value ? this.dropdownRef.value.value.value : this.pageSizes[0].value) as number
        };
    }

    public set value(val: IPaginationRequestBase) {

        ensureExists(this.paginationRef.value);
        ensureExists(this.dropdownRef.value);

        let oldVal = this.value;

        this.paginationRef.value.current = val.page + 1;
        this.dropdownRef.value.value = this.pageSizes.findIndex(el => el.value === val.pageSize);

        this.requestUpdate('value', oldVal);
    }

    @property()
    public total: number = 0;

    private paginationRef: Ref<UUIPaginationElement> = createRef();
    private dropdownRef: Ref<UrlTrackerDropdown> = createRef();

    private get totalPages(): number {

        let pageSize = (this.dropdownRef.value ? this.dropdownRef.value.value.value : this.pageSizes[0].value) as number

        return this.total / pageSize;
    }

    private onPageChange = (e: Event) => {

        if (!(e instanceof UUIPaginationEvent)) return;

        this.dispatchEvent(new PaginationEvent(this.value));
    }

    private onPageSizeChange = (e: Event) => {

        if (!(e instanceof DropdownChangeEvent)) return;

        ensureExists(this.paginationRef.value);
        this.paginationRef.value.current = 1;
        this.requestUpdate('totalPages');

        this.dispatchEvent(new PaginationEvent(this.value));
    }

    protected render(): unknown {
        
        return html`
            <uui-pagination ${ref(this.paginationRef)} .total=${this.totalPages} @change=${this.onPageChange}></uui-pagination>
            <urltracker-dropdown ${ref(this.dropdownRef)} label="Results" .options=${this.pageSizes} @change=${this.onPageSizeChange}></urltracker-dropdown>
        `;
    }

    static styles = css`
        :host {
            display: flex;
            flex-direction: row;
            justify-content: space-between;
            align-items: center;
            gap: 16px;
        }

        uui-pagination {
            background-color: white;
        }
    `;
}