import { ContextConsumer } from "@lit/context";
import { css, html, nothing } from "lit";
import {
  ILocalizationService,
  localizationServiceContext,
} from "../../../../context/localizationservice.context";
import {
  IRedirectResponse,
  redirectContext,
} from "../../../../context/redirectitem.context";
import { LitElementConstructor } from "../../../../util/tools/litelementconstructor";

export function UrlTrackerRedirectTarget<TBase extends LitElementConstructor>(
  Base: TBase,
  typeKey: string
) {
  return class RedirectTarget extends Base {
    private _typeString?: string;
    private get typeString(): string | undefined {
      return this._typeString;
    }
    private set typeString(value: string | undefined) {
      this._typeString = value;
      this.requestUpdate("typeString");
    }

    private _localizationServiceConsumer = new ContextConsumer(this, {
      context: localizationServiceContext,
    });
    protected get localizationService(): ILocalizationService | undefined {
      return this._localizationServiceConsumer.value;
    }

    private _redirectConsumer = new ContextConsumer(this, {
      context: redirectContext,
    });

    protected get redirect(): IRedirectResponse | undefined {
      return this._redirectConsumer.value;
    }

    async connectedCallback(): Promise<void> {
      super.connectedCallback();

      this.typeString = await this.localizationService?.localize(typeKey);
    }

    protected renderBody(): unknown {
      return html`${this.redirect?.target.value}`;
    }

    protected render(): unknown {
      if (!this.typeString) return nothing;

      return html`<span>${this.typeString}</span> ${this.renderBody()}`;
    }

    static styles = [
      css`
        :host {
          display: inline-flex;
          align-items: baseline;
          row-gap: 8px;
        }

        span {
          color: var(--uui-palette-gravel-dimmed);
        }
      `,
    ];
  };
}
