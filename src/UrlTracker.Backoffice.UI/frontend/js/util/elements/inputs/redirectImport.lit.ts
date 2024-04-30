import { consume } from "@lit/context";
import "@umbraco-ui/uui";
import { UUIFileDropzoneEvent } from "@umbraco-ui/uui";
import { LitElement, css, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { localizationServiceContext } from "../../../context/localizationservice.context";
import { ILocalizationService } from "../../../umbraco/localization.service";

@customElement("urltracker-redirect-import")
export class UrlTrackerRedirectImport extends LitElement {
  @consume({ context: localizationServiceContext })
  private _localizationService?: ILocalizationService;

  @property({ type: String })
  public header?: string;

  @state()
  private _headerText: string = "";

  private _localizeHeaderText = async () => {
    let translatedText = await this._localizationService?.localize(
      "urlTrackerRedirectUpload_header"
    );

    if (this.header) {
      this._headerText = `${this.header}`;
    } else if (translatedText) {
      this._headerText = `${translatedText}`;
    } else {
      this._headerText = "";
    }
  };

  private handleChange = (e: UUIFileDropzoneEvent) => {
    const files = e.detail.files;
    this.dispatchEvent(
      new CustomEvent("import", {
        detail: files[0]
      })
    );
  }
  
  async connectedCallback(): Promise<void> {
    super.connectedCallback();
    this._localizeHeaderText();
  }

  private renderBody(): unknown {
    return html`<div class="body">
      <uui-file-dropzone id="browse-dropzone" label="Drop files here" accept="csv" @change=${this.handleChange}>
        Drop files here
      </uui-file-dropzone>
    </div>`;
  }

  protected render(): unknown {
    return html`
      <header>${this._headerText}</header>
      ${this.renderBody()}
    `;
  }

  static styles = css`
    div:hover {
      cursor: pointer;
    }

    header {
      padding: 16px 20px;
      color: white;
      background-color: var(--uui-color-header-surface);
      font-weight: bolder;
      border-radius: var(--uui-border-radius) var(--uui-border-radius) 0 0;
    }

    .body {
      background-color: white;
      padding: 16px 20px;
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
      justify-content: center;
      box-shadow: 0px 1px 1px rgba(0, 0, 0, 0.25);
    }
  `;
}
