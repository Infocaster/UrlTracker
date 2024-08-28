import { IVariableResource } from '../../../../util/tools/variableresource.service';
import { ISourceStrategies } from './source.constants';
import {
  IRedirectSourceStrategyFactory,
  IRedirectSourceStrategy,
  IRedirectSourceStrategyFactoryParameters,
} from './source.strategy';
import { UrlTrackerRedirectSource } from './sourcebase';

export class RegexSourceStrategyFactory implements IRedirectSourceStrategyFactory {
  constructor(private variableResource: IVariableResource) {}

  getStrategy(parameters: IRedirectSourceStrategyFactoryParameters): IRedirectSourceStrategy | undefined {
    const key = this.variableResource.get<ISourceStrategies>('redirectSourceStrategies').regex;
    if (parameters.redirect.source.strategy === key) {
      return new UrlTrackerRedirectSource(parameters.element, 'urlTrackerRedirectSource_regex', parameters.redirect);
    }
  }
}
