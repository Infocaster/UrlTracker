import { consume } from "@lit/context";
import { css, html, nothing } from "lit";
import { customElement, state } from "lit/decorators.js";
import {
  ILocalizationService,
  localizationServiceContext,
} from "../../../context/localizationservice.context";
import { redirectContext } from "../../../context/redirectitem.context";
import { IRedirectResponse } from "../../../services/redirect.service";
import "../../../util/elements/buttonLink.lit";
import { UrlTrackerSelectableResultListItem } from "../../../util/elements/selectableresultlistitem.lit";
import sourceStrategyResolver from "./source/source.strategy";
import targetStrategyResolver from "./target/target.strategy";

const RedirectListItem =
  UrlTrackerSelectableResultListItem<IRedirectResponse>(redirectContext);

@customElement("urltracker-redirect-item")
export class UrlTrackerRedirectItem extends RedirectListItem {
  private sourceStrategy = sourceStrategyResolver;
  private targetStrategy = targetStrategyResolver;

  @consume({ context: localizationServiceContext })
  private localizationService?: ILocalizationService;

  @state()
  private redirectToText?: string;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    if (!this.localizationService)
      throw new Error("This element requires the localization service");

    this.redirectToText = await this.localizationService.localize(
      "urlTrackerRedirectTarget_redirectto"
    );
  }

  private handleInspect(e: Event): void {
    e.stopPropagation();
    this.dispatchEvent(new CustomEvent("inspect", { detail: this.item }));
  }

  private handleEdit(e: Event): void {
    e.stopPropagation();
    this.dispatchEvent(new CustomEvent("edit", { detail: this.item }));
  }

  private handleDelete(e: Event): void {
    e.stopPropagation();
    this.dispatchEvent(new CustomEvent("delete", { detail: this.item }));
  }

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
      <div class="body" @click=${this.handleInspect}>
        ${this.renderSource()}
        <div class="target">${this.redirectToText}: ${this.renderTarget()}</div>
        <div class="actions">
          <urltracker-button-link @click=${this.handleEdit} text="Edit">
              <uui-icon name="edit"></uui-icon>
          </urltracker-button-link>
          <urltracker-button-link @click=${this.handleDelete} text="Delete">
              <uui-icon name="delete"></uui-icon>
          </urltracker-button-link>
        </div>
      </div>
    `;
  }

  static styles = [
    ...RedirectListItem.styles,
    css`
      :host {
        transition: background-color 0.25s;
      }

      :host(:hover) {
        background-color: var(--uui-color-surface-alt);
        cursor: pointer;
      }

      .target {
        line-height: 15px;
        font-size: 12px;
        margin-top: 8px;
      }

      .actions {
        display: flex;
        gap: 16px;
        margin-top: 16px;
      }
    `,
  ];
}
