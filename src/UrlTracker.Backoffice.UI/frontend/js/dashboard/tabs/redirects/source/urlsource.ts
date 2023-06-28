import { html } from "lit";
import { IRedirectResponse } from "../../../../services/redirect.service";
import { IVariableResource } from "../../../../util/tools/variableresource.service";
import { ISourceStrategies } from "./source.constants";
import { IRedirectSourceStrategyFactory, IRedirectSourceStrategy } from "./source.strategy";
import './urlsource.lit'

export class UrlSourceStrategyFactory implements IRedirectSourceStrategyFactory {

    constructor(private variableResource: IVariableResource) { }

    getStrategy(redirect: IRedirectResponse): IRedirectSourceStrategy | undefined {
    
        let key = this.variableResource.get<ISourceStrategies>('redirectSourceStrategies').url;
        if (redirect.source.strategy === key) {

            return {
                getTemplate() {
                    return html`<urltracker-redirect-source-url></urltracker-redirect-source-url>`;
                },
            }
        }
    }
}