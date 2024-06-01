import { ContextProvider, createContext } from '@lit/context';
import { UUIBooleanInputEvent } from '@umbraco-ui/uui';
import { css, html, nothing } from 'lit';
import { UrlTrackerResultListItem } from './resultlistitem.lit';

export function UrlTrackerSelectableResultListItem<T extends Record<string, any>>(
  context: ReturnType<typeof createContext<T>>,
) {
  return class SelectableResultListItem extends UrlTrackerResultListItem {
    private _item: T = {} as T;
    private _isSelected: boolean = false;
    private _selectable: boolean = true;
    private _itemProvider = new ContextProvider(this, { context: context });

    public get item(): T {
      return this._item;
    }

    public set item(value: T) {
      this._item = value;
      this._itemProvider.setValue(value);
      this.requestUpdate('item');
    }

    public get isSelected(): boolean {
      return this._isSelected;
    }

    public set isSelected(value: boolean) {
      this._isSelected = value;
      this.requestUpdate('isSelected');
    }

    public get selectable(): boolean {
      return this._selectable;
    }

    public set selectable(value: boolean) {
      this._selectable = value;
      this.requestUpdate('selectable');
    }

    protected renderBody(): unknown {
      return html`<slot></slot>`;
    }

    protected renderCheckbox(): unknown {
      if (this._selectable) {
        return html`<uui-checkbox .checked=${this.isSelected} @change=${this.onCheckboxPress}></uui-checkbox>`;
      } else return nothing;
    }

    protected render(): unknown {
      return html` ${this.renderCheckbox()} ${this.renderBody()} `;
    }

    private onCheckboxPress = (e: UUIBooleanInputEvent) => {
      e.stopPropagation();
      if (e.target.checked) {
        this.dispatchEvent(new SelectableResultSelectEvent<T>('selected', this.item));
      } else {
        this.dispatchEvent(new SelectableResultSelectEvent<T>('deselected', this.item));
      }
    };

    static styles = [
      ...UrlTrackerResultListItem.styles,
      css`
        :host {
          display: flex;
          flex-direction: row;
          flex-wrap: nowrap;
          align-items: center;
        }

        uui-checkbox {
          position: relative;
          margin-right: 8px;
          z-index: 1000;
        }
      `,
    ];
  };
}

export class SelectableResultSelectEvent<T> extends Event {
  constructor(
    evName: string,
    public item?: T,
    eventInit: any | null = {},
  ) {
    super(evName, {
      ...{ bubbles: false },
      ...eventInit,
    });
  }
}
