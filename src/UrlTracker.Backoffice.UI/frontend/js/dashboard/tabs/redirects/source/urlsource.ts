import { IVariableResource } from '../../../../util/tools/variableresource.service';
import { ISourceStrategies } from './source.constants';
import {
  IRedirectSourceStrategyFactory,
  IRedirectSourceStrategy,
  IRedirectSourceStrategyFactoryParameters,
} from './source.strategy';
import { UrlTrackerRedirectSource } from './sourcebase';

export class UrlSourceStrategyFactory implements IRedirectSourceStrategyFactory {
  constructor(private variableResource: IVariableResource) {}

  getStrategy(parameters: IRedirectSourceStrategyFactoryParameters): IRedirectSourceStrategy | undefined {
    const key = this.variableResource.get<ISourceStrategies>('redirectSourceStrategies').url;
    if (parameters.redirect.source.strategy === key) {
      return new UrlTrackerRedirectSource(parameters.element, 'urlTrackerRedirectSource_url', parameters.redirect);
    }
  }
}
