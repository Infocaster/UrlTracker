import { ContextConsumer } from "@lit/context";
import { LitElementConstructor } from "../../../../util/tools/litelementconstructor";
import {
  ILocalizationService,
  localizationServiceContext,
} from "../../../../context/localizationservice.context";
import { css, html, nothing } from "lit";
import {
  IRecommendationResponse,
  recommendationContext,
} from "../../../../context/recommendationitem.context";

export function UrlTrackerRecommendationType<
  TBase extends LitElementConstructor
>(Base: TBase, typeKey: string) {
  //@ts-ignore ignore: A mixin class must have a constructor with a single rest parameter of type 'any[]'. There is a constructor
  return class RecommendationType extends Base {
    constructor(...args: any[]) {
      super(...args);
    }

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

    private _recommendationConsumer = new ContextConsumer(this, {
      context: recommendationContext,
    });
    protected get recommendation(): IRecommendationResponse | undefined {
      return this._recommendationConsumer.value;
    }

    async connectedCallback(): Promise<void> {
      super.connectedCallback();

      this.typeString = await this.localizationService?.localize(typeKey);
    }

    protected render(): unknown {
      if (!this.typeString) return nothing;

      return html`${this.typeString}`;
    }

    static styles = [
      css`
        :host {
          line-height: 20px;
        }
      `,
    ];
  };
}
