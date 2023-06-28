import { UUIIconRegistry } from "@umbraco-ui/uui";
import { IIconHelper } from "../../umbraco/icon.service";
import { customElement, state } from "lit/decorators.js";
import { consume } from "@lit-labs/context";
import { iconHelperContext } from "../../context/iconhelper.context";
import { LitElement, html, nothing } from "lit";

@customElement('urltracker-angular-icon-registry')
export class AngularIconRegistryElement extends LitElement {

    private _registry: UUIIconRegistry = new UUIIconRegistry();

    @consume({context: iconHelperContext})
    private iconHelper?: IIconHelper;

    @state()
    private loading: number = 0;

    async connectedCallback(): Promise<void> {
        super.connectedCallback();

        if (!this.iconHelper) throw new Error("Icon helper service is required to use this element");

        this.loading++;
        try{

            let icons = await this.iconHelper.getAllIcons();
    
            icons.forEach((icon) => {
                let svg = icon.svgString.$$unwrapTrustedValue();
                this._registry.defineIcon(icon.name, svg);
            });
    
            this._registry.attach(this);
        }
        finally {
            this.loading--;
        }
    }

    disconnectedCallback(): void {
        super.disconnectedCallback();
        this._registry.detach(this);
    }

    protected render(): unknown {
        if (this.loading) return nothing;
        return html`<slot></slot>`;
    }
}