import { html } from "lit";
import { IRedirectResponse } from "../../../../services/redirect.service";
import { IVariableResource } from "../../../../util/tools/variableresource.service";
import { ISourceStrategies } from "./source.constants";
import { IRedirectSourceStrategyFactory, IRedirectSourceStrategy } from "./source.strategy";
import './regexsource.lit'

export class RegexSourceStrategyFactory implements IRedirectSourceStrategyFactory {

    constructor(private variableResource: IVariableResource) { }

    getStrategy(redirect: IRedirectResponse): IRedirectSourceStrategy | undefined {
    
        const key = this.variableResource.get<ISourceStrategies>('redirectSourceStrategies').regex;
        if (redirect.source.strategy === key) {

            return {
                getTemplate() {
                    return html`<urltracker-redirect-source-regex></urltracker-redirect-source-regex>`;
                },
            }
        }
    }
}