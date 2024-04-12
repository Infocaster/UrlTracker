import { ensureExists } from "@/util/tools/existancecheck";
import { UUIPaginationElement } from "@umbraco-ui/uui";
import { LitElement, html } from "lit";
import { customElement, property } from "lit/decorators.js";
import { Ref, createRef, ref } from "lit/directives/ref.js";

export class UrlTrackerPageSelectEvent extends Event {

    static event = "change"

    constructor(public data: number, eventInitDict?: EventInit) {
        super(UrlTrackerPageSelectEvent.event, eventInitDict);
    }
}

@customElement('urltracker-pageselect')
export class UrlTrackerPageSelect extends LitElement
{
    static formAssociated = true;

    constructor() {
        super();
        this._internals = this.attachInternals();
        this.value = 1;
    }

    private paginationRef: Ref<UUIPaginationElement> = createRef();
    private _value: number = 0;
    private _internals: ElementInternals;

    @property({type: Number})
    total: number = 0;

    @property({type: String})
    name?: string

    @property({ type: Number })
    get value(): number {
        return this._value;
    };

    set value(v: number) {

        if (this._value === v)
            return;

        this._value = v;
        this._internals.setFormValue(v.toString());

        if (this.paginationRef.value) {

            this.paginationRef.value.current = v;
        }

        this.requestUpdate('value');
    }

    _onChange = (_: Event) => {

        ensureExists(this.paginationRef.value, "A reference to the pagination element is required before changes can properly be tracked");
        const newValue = this.paginationRef.value.current;

        if (newValue === this.value)
            return;

        this.value = newValue;
        this.dispatchEvent(new UrlTrackerPageSelectEvent(this.value));
    }

    protected render(): unknown {
        
        return html`
            <uui-pagination ${ref(this.paginationRef)} @change=${this._onChange} .total=${this.total}></uui-pagination>
        `;
    }
}