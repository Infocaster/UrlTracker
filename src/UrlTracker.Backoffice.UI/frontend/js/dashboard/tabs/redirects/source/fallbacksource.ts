import { html } from "lit";
import { IRedirectSourceStrategyFactory, IRedirectSourceStrategy } from "./source.strategy";
import './fallbacksource.lit';

export class UnknownSourceStrategyFactory implements IRedirectSourceStrategyFactory {

    getStrategy(): IRedirectSourceStrategy | undefined {

        return {
            getTemplate() {
                return html`<urltracker-redirect-source-unknown></urltracker-redirect-source-unknown>`;
            },
        }
    }
}