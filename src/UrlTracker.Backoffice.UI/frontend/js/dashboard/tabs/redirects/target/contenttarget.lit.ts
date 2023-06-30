import { LitElement } from "lit";
import { UrlTrackerRedirectTarget } from "./targetbase.mixin";
import { consume } from "@lit-labs/context";
import { IContentResource } from "../../../../umbraco/content.service";
import { contentResourceContext } from "../../../../context/contentresource.context";
import { customElement } from "lit/decorators.js";

@customElement('urltracker-redirect-target-content')
export class UrlTrackerContentRedirectTarget extends UrlTrackerRedirectTarget(LitElement, "urlTrackerRedirectTarget_content") {

    @consume({context: contentResourceContext})
    private contentResource?: IContentResource;

    async connectedCallback(): Promise<void> {
        
        await super.connectedCallback();

        if (!this.contentResource) throw new Error("This element requires the content resource");
        if (!this.redirect) throw new Error("This element requires a redirect");

        let [id, culture] = this.redirect.target.value.split(';');

        let content = await this.contentResource.getById(Number.parseInt(id), 'Document');

        console.log(content);
    }
}