import { css, html, nothing } from "lit";
import { customElement } from "lit/decorators.js";
import { UrlTrackerSelectableResultListItem } from "../../../util/elements/selectableresultlistitem.lit";
import { IRedirectResponse } from "../../../services/redirect.service";
import { redirectContext } from "../../../context/redirectitem.context";
import sourceStrategyResolver from "./source/source.strategy";
import targetStrategyResolver from "./target/target.strategy";

let RedirectListItem = UrlTrackerSelectableResultListItem<IRedirectResponse>(redirectContext);

@customElement('urltracker-redirect-item')
export class UrlTrackerRedirectItem extends RedirectListItem {

    private sourceStrategy = sourceStrategyResolver;
    private targetStrategy = targetStrategyResolver;

    private renderSource(): unknown {

        if (!this.item) return nothing;
        return this.sourceStrategy.getStrategy(this.item).getTemplate();
    }

    private renderTarget(): unknown {

        if (!this.item) return nothing;
        return this.targetStrategy.getStrategy(this.item).getTemplate();
    }

    protected renderBody(): unknown {
        return html`
            <div class="body">
                ${this.renderSource()}
                <div class="target">redirects to: ${this.renderTarget()}</div>
                <div class="actions"><uui-button>edit</uui-button><uui-button>Delete</uui-button></div>
            </div>
        `
    }

    static styles = [
        ...RedirectListItem.styles,
        css`
            .body {
                margin-left: 16px;
            }

            .target {
                line-height: 15px;
                font-size: 12px;
                margin-top: 8px;
            }
        `
    ];
}