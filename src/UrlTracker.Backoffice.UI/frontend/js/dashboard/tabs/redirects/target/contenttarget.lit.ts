import { LitElement, css, html } from "lit";
import { UrlTrackerRedirectTarget } from "./targetbase.mixin";
import { consume } from "@lit-labs/context";
import { customElement, state } from "lit/decorators.js";
import { ITargetService, redirectTargetServiceContext } from "../../../../context/redirecttargetservice.context";
import { IContentTargetResponse } from "./target.service";

let baseType = UrlTrackerRedirectTarget(LitElement, "urlTrackerRedirectTarget_content");

@customElement('urltracker-redirect-target-content')
export class UrlTrackerContentRedirectTarget extends baseType {

    @consume({context: redirectTargetServiceContext})
    private redirectTargetService?: ITargetService;

    @state()
    private contentItem?: IContentTargetResponse

    async connectedCallback(): Promise<void> {
        
        await super.connectedCallback();

        if (!this.redirectTargetService) throw new Error("This element requires the redirect target resource");
        if (!this.redirect) throw new Error("This element requires a redirect");

        let [id, culture] = this.redirect.target.value.split(';');

        this.contentItem = await this.redirectTargetService.Content({ id: Number.parseInt(id), culture: culture });
    }

    protected renderBody(): unknown {
        
        return html`<uui-icon .name=${this.contentItem?.icon}></uui-icon> ${this.contentItem?.name}`
    }

    static styles = [
        ...baseType.styles,
        css`
            uui-icon {
                align-self: center;
            }
        `
    ]
}