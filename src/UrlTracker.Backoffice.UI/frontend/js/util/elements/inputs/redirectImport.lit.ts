import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UUIFileDropzoneEvent } from '@umbraco-ui/uui-file-dropzone';
import { LitElement, css, html } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';

@customElement('urltracker-redirect-import')
export class UrlTrackerRedirectImport extends UmbElementMixin(LitElement) {
  @property({ type: String })
  public header?: string;

  @property({ type: Boolean, reflect: true })
  public loading?: boolean = false;

  @state()
  private _headerText: string = '';

  private _localizeHeaderText = async () => {
    const translatedText = this.localize.term('urlTrackerRedirectUpload_header');

    if (this.header) {
      this._headerText = `${this.header}`;
    } else if (translatedText) {
      this._headerText = `${translatedText}`;
    } else {
      this._headerText = '';
    }
  };

  private handleChange = (e: UUIFileDropzoneEvent) => {
    const files = e.detail.files;
    this.dispatchEvent(
      new CustomEvent('import', {
        detail: files[0],
      }),
    );
  };

  private handleDownloadTemplate = () => {
    this.dispatchEvent(new CustomEvent('download-template'));
  };

  async connectedCallback(): Promise<void> {
    super.connectedCallback();
    this._localizeHeaderText();
  }

  private renderBody(): unknown {
    return html`<div class="body">
      <p>
        Drop a csv file in the box below to import redirects.
        <button aria-label="download import template" @click=${this.handleDownloadTemplate}></button>
          You can download a template here <uui-icon name="icon-help-alt"></uui-icon>
        </button>
      </p>
	  ${
      this.loading
        ? html`<div class="loader-container">
            <uui-loader-circle id="loader" style="font-size: 3em"></uui-loader-circle>
          </div>`
        : html`<uui-file-dropzone
            id="browse-dropzone"
            label="Drag / drop a file here"
            accept="csv"
            @change=${this.handleChange}
          >
            Drag / drop a file here
          </uui-file-dropzone>`
    }
    </div>`;
  }

  protected render(): unknown {
    return html`
      <header>${this._headerText}</header>
      ${this.renderBody()}
    `;
  }

  static styles = css`
    uui-file-dropzone {
      --uui-color-default: var(--uui-color-background);
    }

    p {
      margin-top: 0;
    }

    button {
      /* Reset button */
      border: none;
      margin: 0;
      padding: 0;
      width: auto;
      overflow: visible;
      background: transparent;

      /* inherit font & color from ancestor */
      color: inherit;
      font: inherit;

      /* Normalize line-height. Cannot be changed from normal in Firefox 4+. */
      line-height: normal;

      /* Corrects font smoothing for webkit */
      -webkit-font-smoothing: inherit;
      -moz-osx-font-smoothing: inherit;

      /* Corrects inability to style clickable input types in iOS */
      -webkit-appearance: none;

      cursor: pointer;
    }

    button:hover {
      text-decoration: underline;
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

    .loader-container {
      display: flex;
      justify-content: center;
      align-items: center;
    }
  `;
}
