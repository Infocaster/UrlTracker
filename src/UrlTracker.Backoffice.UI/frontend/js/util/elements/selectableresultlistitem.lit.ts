import { UrlTrackerResultListItem } from "./resultlistitem.lit";
import { css, html } from "lit";
import { UUIBooleanInputEvent } from "@umbraco-ui/uui";
import { ContextProvider, createContext } from "@lit-labs/context";

export function UrlTrackerSelectableResultListItem<T>(context: ReturnType<typeof createContext<T>>) {

    return class SelectableResultListItem extends UrlTrackerResultListItem {
    
        private _item?: T;
        private _itemProvider = new ContextProvider(this, {context: context});

        public get item(): T | undefined {

            return this._item;
        }

        public set item(value: T) {

            this._item = value;
            this._itemProvider.setValue(value)
            this.requestUpdate('item');
        }
    
        protected renderBody(): unknown {
            return html`<slot></slot>`;
        }
    
        protected render(): unknown {
            
            return html`
                <uui-checkbox @change=${this.onCheckboxPress}></uui-checkbox>
                ${this.renderBody()}
            `;
        }
    
        private onCheckboxPress = (e: UUIBooleanInputEvent) => {
    
            e.stopPropagation();
            if (e.target.checked){
                this.dispatchEvent(new SelectableResultSelectEvent<T>("selected", this.item));
            }
            else{
                this.dispatchEvent(new SelectableResultSelectEvent<T>("deselected", this.item));
            }
        }
    
        static styles = [
            ...UrlTrackerResultListItem.styles,
            css`
                :host {
                    display: flex;
                    flex-direction: row;
                    flex-wrap: nowrap;
                    align-items: center; 
                }
            `
        ];
    }
}

export class SelectableResultSelectEvent<T> extends Event {
    constructor(evName: string, public item?: T, eventInit: any | null = {}){
        super(evName, {
            ...{bubbles: false},
            ...eventInit
        });
    }
}