import { LitElement, css, html } from "lit";
import { customElement, property } from "lit/decorators.js";
import { Ref, createRef, ref } from "lit/directives/ref.js";
import { ensureExists } from "../../tools/existancecheck";

export interface IDropdownValue {
    display: string;
    value: unknown;
}

export class DropdownChangeEvent extends Event {

    static event = "change"

    constructor(public selected: IDropdownValue, eventInitDict?: EventInit) {
        super(DropdownChangeEvent.event, eventInitDict);
    }
}

@customElement('urltracker-dropdown')
export class UrlTrackerDropdown extends LitElement {

    @property()
    public label?: string;

    @property()
    public options?: IDropdownValue[];

    @property()
    public get value(): IDropdownValue {

        ensureExists(this.selectRef.value);
        ensureExists(this.options);

        return this.options[Number.parseInt(this.selectRef.value.value)];
    }

    public set value(index: number) {

        ensureExists(this.selectRef.value);
        ensureExists(this.options);

        if (this.options.length <= index) throw new Error("index is out of range");

        this.selectRef.value.value = index.toString();
        this.requestUpdate('value');
    }

    private selectRef: Ref<HTMLSelectElement> = createRef();

    private onChange = (_: Event) => {

        let newValue = this.value;
        this.dispatchEvent(new DropdownChangeEvent(newValue));
    }

    protected render(): unknown {
        
        return html`
            <label>
                ${this.label}:
                <select ${ref(this.selectRef)} @change=${this.onChange}>
                    ${this.options?.map((option, index) => html`<option .value=${index.toString()}>${option.display}</option>`)}
                </select>
            </label>
        `;
    }

    static styles = css`
        select {
            background: none;
            border: none;
            font-weight: bolder;
            font-size: 15px;
        }
    `;
}