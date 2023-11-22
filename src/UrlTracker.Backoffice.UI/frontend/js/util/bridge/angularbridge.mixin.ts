import { LitElementConstructor } from "@/util/tools/litelementconstructor.ts";
import { ContextProvider, createContext } from "@lit/context";
import { html } from "lit";
import "../elements/angulariconregistry.lit";

export function AngularBridgeMixin<TBase extends LitElementConstructor>(
  Base: TBase,
  contentTemplate: unknown
) {
  return class MainContext extends Base {
    _contextCollection: Record<string, unknown> = {};
    public SetContext<T>(
      service: T,
      context: ReturnType<typeof createContext<T>>,
      key: string
    ) {
      if (this._contextCollection[key]) {
        (
          this._contextCollection[key] as ContextProvider<
            ReturnType<typeof createContext<T>>
          >
        ).setValue(service);
      } else {
        this._contextCollection[key] = new ContextProvider(this, {
          context: context,
          initialValue: service,
        });
      }
    }

    protected render(): unknown {
      return html`
        <urltracker-angular-icon-registry>
          ${contentTemplate}
        </urltracker-angular-icon-registry>
      `;
    }
  };
}
