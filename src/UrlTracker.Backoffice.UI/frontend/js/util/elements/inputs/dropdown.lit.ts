import { LitElement, css, html, nothing } from 'lit';
import { customElement, property } from 'lit/decorators.js';
import { ensureExists } from '../../tools/existancecheck';
import { repeat } from 'lit/directives/repeat.js';

export interface IDropdownValue {
  display: string;
  value: unknown;
  key: string;
}

export class DropdownChangeEvent extends Event {
  static event = 'change';
  public data = {} as IDropdownValue;

  constructor(
    public selected: IDropdownValue,
    eventInitDict?: EventInit,
  ) {
    super(DropdownChangeEvent.event, eventInitDict);
    this.data = selected;
  }
}

@customElement('urltracker-dropdown')
export class UrlTrackerDropdown extends LitElement {
  static formAssociated = true;

  private _internals: ElementInternals;
  private _value: string = '';
  private _options: IDropdownValue[] = [];

  constructor() {
    super();
    this._internals = this.attachInternals();
    this.value = '';
  }

  @property()
  public label?: string;

  @property({ type: String })
  public name?: string;

  @property({ type: Array })
  public get options(): IDropdownValue[] {
    return this._options;
  }

  public set options(value: IDropdownValue[]) {
    this._options = value;
    if (!this.options.find((o) => o.key === this.value)) {
      this.value = this.options.length ? this.options[0].key : '';
    }
  }

  @property({ type: String })
  get value(): string {
    return this._value;
  }

  set value(v: string) {
    if (this._value === v) return;

    this._value = v;
    this._internals.setFormValue(v);
  }

  private onChange = (event: Event) => {
    if (!(event.target instanceof HTMLSelectElement)) return;

    const key = event.target.value;
    const choice = this.options?.find((o) => o.key === key);

    ensureExists(
      choice,
      'It is mandatory that the chosen option exists. This is an indication that something is wrong internally.',
    );

    if (choice.key === this.value) return;

    this.value = choice.key;
    this.dispatchEvent(new DropdownChangeEvent(choice));
  };

  protected render(): unknown {
    return html`
      <label>
        ${this.label}:
        <select @change=${this.onChange}>
          ${this.options
            ? repeat(
                this.options,
                (option) => option.key,
                (option: IDropdownValue) => html`<option .value=${option.key}>${option.display}</option>`,
              )
            : nothing}
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
