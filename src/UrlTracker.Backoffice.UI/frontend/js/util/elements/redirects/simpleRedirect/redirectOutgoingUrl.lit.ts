import { IEditorService, editorServiceContext } from "@/context/editorservice.context";
import {
  ILocalizationService,
  localizationServiceContext,
} from "@/context/localizationservice.context";
import { ITargetService, redirectTargetServiceContext } from "@/context/redirecttargetservice.context";
import { ITargetStrategies } from "@/dashboard/tabs/redirects/target/target.constants";
import { IContentTargetResponse } from "@/dashboard/tabs/redirects/target/target.service";
import { IContent } from "@/umbraco/editor.service";
import { debounce } from "@/util/functions/debounce";
import variableresourceService from "@/util/tools/variableresource.service";
import { consume } from "@lit/context";
import { UUIInputEvent } from "@umbraco-ui/uui";
import { LitElement, css, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { Ref, createRef, ref } from "lit/directives/ref.js";
import { repeat } from "lit/directives/repeat.js";
import "./simpleRedirectTypeProvider";
import { ITypeButton } from "./simpleRedirectTypeProvider";

@customElement("urltracker-redirect-outgoing-url")
export class UrlTrackerRedirectOutgoingUrl extends LitElement {
  @property({ type: String })
  private outgoingUrl: string = "";

  @property({ type: String })
  private outgoingStrategy: string = "url";

  @state()
  private _headerText: string = "";

  @state()
  private _infoText: string = "";

  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  @consume({ context: editorServiceContext })
  private editorService?: IEditorService<any>;

  @consume({ context: redirectTargetServiceContext })
  private redirectTargetService?: ITargetService;

  private inputRef: Ref<HTMLInputElement> = createRef();

  public _typeButtons = [
    {
      label: "urlTrackerRedirectTarget_content",
      labelFallback: "Content",
      value: variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').content,
      placeholder: "link to content placeholder",
      disabled: false,
    },
    // {
    //   label: "urlTrackerRedirectTarget_media",
    //   labelFallback: "Media",
    //   value: variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').media,
    //   placeholder: "link to media placeholder",
    //   disabled: true,
    // },
    {
      label: "urlTrackerRedirectTarget_url",
      labelFallback: "URL",
      value: variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').url,
      placeholder: "https://example.com/",
      disabled: false,
    },
  ] as ITypeButton[];

  private contentItem: IContentTargetResponse | undefined = undefined;

  @state()
  private _selectedType: ITypeButton = this._typeButtons[0];

  async connectedCallback(): Promise<void> {
    super.connectedCallback();
    
    this._localizeHeaderText();
    this._localizeInfoText();
    this._localizeButtonLabels();

    this._selectedType = this._typeButtons.find(
      (item) => item.value === this.outgoingStrategy
    ) ?? this._typeButtons.find((item) => item.value === variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').url)!;

    if(this.outgoingStrategy === variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').content) {
      let [id, culture] = this.outgoingUrl.split(";");

      this.contentItem = await this.redirectTargetService!.Content({
        id: Number.parseInt(id),
        culture: culture,
      });

      console.log(this.contentItem);
    }
  }

  private _localizeHeaderText = async () => {
    const text = await this._localizationService?.localize(
      "urlTrackerNewRedirect_outgoing-url"
    );

    this._headerText = text ?? "Outgoing URL fallback";
  };

  private _localizeInfoText = async () => {
    const text = await this._localizationService?.localize(
      "urlTrackerNewRedirect_outgoing-url-info"
    );

    this._infoText = text ?? "Select where the URL should redirect to";
  };

  private _localizeButtonLabels = async () => {
    const labels = await this._localizationService?.localizeMany(
      this._typeButtons.map((item) => item.label)
    );

    this._typeButtons = this._typeButtons.map((item, index) => ({
      ...item,
      label: labels?.[index] ?? item.labelFallback,
    }));
  };

  private openContentPicker = () => {
    this.editorService?.contentPicker({
      multiPicker: false,
      submit: this.submitContentPicker,
      close: () => this.editorService?.close()
    });
  }

  private submitContentPicker = (model: { selection: IContent[] }) => {
    if(model.selection.length === 0) return this.editorService?.close();
    this.contentItem = model.selection[0];;
    this.requestUpdate();
    this.editorService?.close();
  }

  private onInput = (e: UUIInputEvent) => {
    this.dispatchEvent(
      new CustomEvent("input", {
        detail: this.inputRef.value?.shadowRoot?.querySelector('input')?.value ?? '',
        bubbles: true,
        composed: false,
      })
    );
  };

  private _debouncedOnInput = debounce(this.onInput, 500);

  private onTypeChange = (item: ITypeButton, e: Event) => {
    this._selectedType = item;
    this.dispatchEvent(
      new CustomEvent("typechange", {
        detail: item,
        bubbles: true,
        composed: false,
      })
    );
  };

  protected renderOutgoingStrategy(): unknown {
    if (this._selectedType.value === variableresourceService.get<ITargetStrategies>('redirectTargetStrategies').content) {
      if(this.contentItem) {
        return html`
          <div class="content-item">
            <img src="${this.contentItem.icon}" alt="${this.contentItem.name}" />
            <span>${this.contentItem.name}</span>
            <uui-button
              look="outline"
              label="Verwijderen"
              @click=${() => this.contentItem = undefined}
            ></uui-button>
          </div>
        `;
      } 

      return html`
        <uui-button 
          class="w-100"
          look="placeholder" 
          label="Toevoegen"
          @click=${this.openContentPicker}>
          Toevoegen
        </uui-button>
      `;
    }

    return html`
      <uui-input
        ${ref(this.inputRef)}
        .value=${this.outgoingUrl}
        .placeholder=${this._selectedType.placeholder}
        @input=${this._debouncedOnInput}
      ></uui-input>
    `;
  }

  protected render(): unknown {
    return html`
      <p>
        <strong>${this._headerText} <span class="required">*</span></strong>
      </p>
      <p>${this._infoText}</p>
      <uui-button-group>
        ${repeat(
          this._typeButtons,
          (item) => item.value,
          (item) => html` <uui-button
            label=${item.label}
            look=${this._selectedType.value === item.value
              ? "primary"
              : "outline"}
            color="default"
            .disabled=${item.disabled}
            @click=${(e: Event) => this.onTypeChange(item, e)}
          ></uui-button>`
        )}
      </uui-button-group>
      ${this.renderOutgoingStrategy()}
    `;
  }

  static styles = [
    css`
      :host {
        display: block;
      }

      uui-button-group {
        width: 100%;
        margin-bottom: 1rem;
        flex-wrap: wrap;
      }

      uui-input {
        width: 100%;
      }

      .w-100 {
        width: 100%;
      }

      .required {
        color: #ba0000;
      }
    `,
  ];
}
