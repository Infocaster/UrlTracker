import { LitElement, css, html } from "lit";
import { customElement, property } from "lit/decorators.js";

@customElement("urltracker-bulk-actions")
export class UrlTrackerBulkActions extends LitElement {
    @property({ type: Number })
    public selectedCount: number = 0;

    @property({ type: Number })
    public total: number = 0;


    async connectedCallback() {
        super.connectedCallback();
    }

    private onSelectAll() {
        this.dispatchEvent(
            new CustomEvent("select-all", {
                bubbles: true,
                composed: true
            })
        )
    }

    private onClearSelection() {
        this.dispatchEvent(
            new CustomEvent("clear-selection", {
                bubbles: true,
                composed: true
            })
        )
    }

    protected render(): unknown {
        return html`
            <div class="bulk-actions">
                <div class="bulk-actions-main">
                    <uui-button
                        look="secondary"
                        @click=${this.onSelectAll}
                    >
                        Select all
                    </uui-button>
                    <uui-button
                        look="secondary"
                        @click=${this.onClearSelection}
                    >
                        Clear selection
                    </uui-button>
                    <span>${this.selectedCount} of ${this.total} selected</span>
                </div>   
                <div class="bulk-actions-custom">
                    <slot></slot>
                </div>
            </div>
        `;
    }

    static styles = css`
        .bulk-actions {
            display: flex;
            justify-content: space-between;
            align-items: center;
            background: #3544B1;
            padding: 1rem;
            border-radius: 4px;

            span {
                display: flex;
                align-items: center;
                font-family: Lato, sans-serif;
                font-size: 15px;
                font-weight: 800;
                line-height: 20px;
                color: #FFFFFF;
            }
        }

        .bulk-actions-main, .bulk-actions-custom {
            display: flex;
            gap: 1rem;
        }
    `;
}
