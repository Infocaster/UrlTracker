import { LitElement, css, html, nothing } from "lit";
import { customElement, property } from "lit/decorators.js";
import { Ref, createRef, ref } from "lit/directives/ref.js";
import { ensureExists } from "../../tools/existancecheck";
import { repeat } from "lit/directives/repeat.js";

export interface IDropdownValue {
  display: string;
  value: unknown;
}

export class DropdownChangeEvent extends Event {
  static event = "change";
  public data = {} as IDropdownValue;

  constructor(public selected: IDropdownValue, eventInitDict?: EventInit) {
    super(DropdownChangeEvent.event, eventInitDict);
    this.data = selected;
  }
}

@customElement("urltracker-dropdown")
export class UrlTrackerDropdown extends LitElement {
  @property()
  public label?: string;

  @property()
  public options?: IDropdownValue[];

  // Fixme: Refs are commented out because they break the ensureExists check
  //   @property()
  //   public get value(): IDropdownValue {
  //     // ensureExists(this.selectRef.value);
  //     ensureExists(this.options);

  //     return this.options[Number.parseInt(this.selectRef.value.value)];
  //   }

  //   public set value(index: number) {
  //     // ensureExists(this.selectRef.value);
  //     ensureExists(this.options);

  //     if (this.options.length <= index) throw new Error("index is out of range");

  //     // this.selectRef.value.value = index.toString();
  //     this.requestUpdate("value");
  //   }

  //   private selectRef: Ref<HTMLSelectElement> = createRef();

  private onChange = (event: any) => {
    this.dispatchEvent(new DropdownChangeEvent(event.target.value));
  };

  protected render(): unknown {
    return html`
      <label>
        ${this.label}:
        <select>
          ${this.options
            ? repeat(
                this.options,
                (option) => option.value,
                (option: any) =>
                  html`<option
                    .value=${option.value.toString()}
                    @click=${this.onChange}
                  >
                    ${option.display}
                  </option>`
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
