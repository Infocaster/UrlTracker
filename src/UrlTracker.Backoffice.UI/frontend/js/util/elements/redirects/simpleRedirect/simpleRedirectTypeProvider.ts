import { provide } from "@lit/context";
import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import { simpleRedirectContext } from "../../../../context/simpleRedirect.context";

export interface ITypeButton {
  label: string;
  labelFallback: string;
  value: "content" | "media" | "url";
  placeholder: string;
  disabled: boolean;
}

/*
FIXME:
This code is WIP and not yet working.
Trying to create a custom provider for the simple redirect type buttons.
The point of this provider is to have a function accesible from the window object to add more buttons to the list.
*/

@customElement("urltracker-simple-redirect-type-provider")
export class SimpleRedirectTypeProvider extends LitElement {
  @provide({ context: simpleRedirectContext })
  public _typeButtons = [
    {
      label: "urlTrackerRedirectTarget_content",
      value: "content",
      placeholder: "link to content placeholder",
    },
    {
      label: "urlTrackerRedirectTarget_media",
      value: "media",
      placeholder: "link to media placeholder",
    },
    {
      label: "urlTrackerRedirectTarget_url",
      value: "url",
      placeholder: "https://example.com/",
    },
  ] as ITypeButton[];

  render() {
    return html`<slot></slot>`;
  }

  private _addTypeButton = (button: ITypeButton) => {
    this._typeButtons.push(button);
  };
}
