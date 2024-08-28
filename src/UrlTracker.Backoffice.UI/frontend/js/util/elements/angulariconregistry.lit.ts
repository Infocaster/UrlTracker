import { consume } from '@lit/context';
import { UUIIconRegistry } from '@umbraco-ui/uui';
import { LitElement, html, nothing } from 'lit';
import { customElement, state } from 'lit/decorators.js';
import { iconHelperContext } from '../../context/iconhelper.context';
import { IIconHelper } from '../../umbraco/icon.service';

class AngularIconRegistry extends UUIIconRegistry {
  private _iconHelper?: IIconHelper;

  public setIconHelper(iconHelper: IIconHelper): void {
    this._iconHelper = iconHelper;
  }

  getIcon(iconName: string): Promise<string> | null {
    if (!this._iconHelper) return super.getIcon(iconName);

    return this.getIconFromHelper(iconName);
  }

  private async getIconFromHelper(iconName: string): Promise<string> {
    const result = await this._iconHelper!.getIcon(iconName);
    if (!result) return '';

    return result.svgString.$$unwrapTrustedValue();
  }
}

@customElement('urltracker-angular-icon-registry')
export class AngularIconRegistryElement extends LitElement {
  private _registry: AngularIconRegistry = new AngularIconRegistry();

  constructor() {
    super();
    this._registry.attach(this);
  }

  @consume({ context: iconHelperContext })
  private iconHelper?: IIconHelper;

  @state()
  private loading: number = 0;

  async connectedCallback(): Promise<void> {
    super.connectedCallback();

    if (!this.iconHelper) throw new Error('Icon helper service is required to use this element');

    this._registry.setIconHelper(this.iconHelper);
  }

  disconnectedCallback(): void {
    super.disconnectedCallback();
    this._registry.detach(this);
  }

  protected render(): unknown {
    if (this.loading) return nothing;
    return html`
      <uui-icon-registry-essential>
        <slot></slot>
      </uui-icon-registry-essential>
    `;
  }
}
