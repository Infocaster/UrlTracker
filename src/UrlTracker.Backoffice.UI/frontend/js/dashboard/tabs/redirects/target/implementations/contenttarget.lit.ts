import { LitElement, css, html } from "lit";
import { UrlTrackerRedirectTarget } from "../targetbase.mixin";
import { consume } from "@lit-labs/context";
import { customElement, state } from "lit/decorators.js";
import { ITargetService, redirectTargetServiceContext } from "../../../../../context/redirecttargetservice.context";
import { IContentTargetResponse } from "../target.service";
import { IEditorService, editorServiceContext } from "../../../../../context/editorservice.context";
import { IChangeManager, changeManagerContext } from "../../../../../context/changemanager.context";
import { ensureServiceExists } from "../../../../../util/tools/existancecheck";

export class ContentUpdateEvent extends Event {
    
    static event = "content-update";

    constructor(public contentId: string, public contentItem: IContentTargetResponse, eventInitDict?: EventInit) {
        super(ContentUpdateEvent.event,
            {
                bubbles: true,
                composed: true,
                ...eventInitDict,
            });
    }
}

let baseType = UrlTrackerRedirectTarget(LitElement, "urlTrackerRedirectTarget_content");

@customElement('urltracker-redirect-target-content')
export class UrlTrackerContentRedirectTarget extends baseType {

    @consume({context: redirectTargetServiceContext})
    private redirectTargetService?: ITargetService;

    @consume({context: editorServiceContext})
    private editorService?: IEditorService;

    @consume({context: changeManagerContext})
    private changeManager?: IChangeManager;

    @state()
    private contentItem?: IContentTargetResponse;

    @state()
    private contentId?: string;

    @state()
    private loading: number = 0;

    @state()
    private errorText?: string;

    async connectedCallback(): Promise<void> {
        
        await super.connectedCallback();

        ensureServiceExists(this.changeManager, "changeManager");
        this.changeManager.element.addEventListener(ContentUpdateEvent.event, this.onContentUpdate);

        await this.init();
    }

    disconnectedCallback(): void {
        
        super.disconnectedCallback();

        ensureServiceExists(this.changeManager, "changeManager");
        this.changeManager.element.removeEventListener(ContentUpdateEvent.event, this.onContentUpdate);
    }

    private onContentUpdate = (e: Event) => {

        if (!(e instanceof ContentUpdateEvent)) return;
        if (e.contentId !== this.contentId) return;

        this.contentItem = e.contentItem;
    }

    private async init(): Promise<void> {

        ensureServiceExists(this.redirectTargetService, "redirect target resource");
        ensureServiceExists(this.redirect, "redirect");

        this.loading++;
        this.errorText = undefined;
        try {

            let [id, culture] = this.redirect.target.value.split(';');
    
            this.contentItem = await this.redirectTargetService.Content({ id: Number.parseInt(id), culture: culture });
            this.contentId = id;
        }
        catch {

            this.errorText = await this.localizationService?.localize("urlTrackerRedirectTarget_contenterror");
        }
        finally{
            
            this.loading--;
        }
    }

    private onClick = (_: Event) => {

        ensureServiceExists(this.editorService, "editor service");

        const onClose = async () => {
            this.editorService!.close();
            await this.init();
            this.dispatchEvent(new ContentUpdateEvent(this.contentId!, this.contentItem!))
        };

        this.editorService.contentEditor({
            id: this.contentId!,
            create: false,
            submit: onClose,
            close: onClose,
            documentTypeAlias: '',
            allowPublishAndClose: false,
            allowSaveAndClose: false,
            parentId: ''
        });
    }

    protected renderBody(): unknown {
        
        if (this.loading) return html`<uui-loader></uui-loader>`;
        if (this.errorText) return html`<span class="error">${this.errorText}</span>`;
        return html`
            <uui-icon .name=${this.contentItem?.icon}></uui-icon> ${this.contentItem?.name}
            <button @click="${this.onClick}"></button>
        `;
    }

    static styles = [
        ...baseType.styles,
        css`
            uui-icon {
                align-self: center;
            }

            .error {
                font-style: italic;
                color: var(--uui-color-danger);
            }

            :host {
                position: relative;
            }

            button {
                position: absolute;
                top: 0;
                left: 0;
                bottom: 0;
                right: 0;
                background: none;
                border: none;
                cursor: pointer;
            }
        `
    ]
}