import { ContextConsumer } from "@lit-labs/context";
import { LitElementConstructor } from "../../../../util/tools/litelementconstructor";
import { ILocalizationService, localizationServiceContext } from "../../../../context/localizationservice.context";
import { css, html, nothing } from "lit";
import { IRedirectResponse, redirectContext } from "../../../../context/redirectitem.context";

export function UrlTrackerRedirectSource<TBase extends LitElementConstructor>(Base: TBase, typeKey: string){
    return class RedirectSource extends Base {

        private _typeString?: string;
        private get typeString(): string | undefined {
            return this._typeString;
        }
        private set typeString(value: string | undefined) {
            this._typeString = value;
            this.requestUpdate('typeString');
        }

        private _localizationServiceConsumer = new ContextConsumer(this, {context: localizationServiceContext});
        protected get localizationService(): ILocalizationService | undefined {
            
            return this._localizationServiceConsumer.value;
        };

        private _redirectConsumer = new ContextConsumer(this, {context: redirectContext});
        protected get redirect(): IRedirectResponse | undefined {
            return this._redirectConsumer.value;
        }

        async connectedCallback(): Promise<void> {
            
            super.connectedCallback();

            this.typeString = await this.localizationService?.localize(typeKey);
        }

        protected render(): unknown {

            if (!this.typeString) return nothing;

            return html`${this.typeString}: ${this.redirect?.source.value}`;
        }

        static styles = [css`
            :host {
                line-height: 20px;
            }
        `]
    }
}