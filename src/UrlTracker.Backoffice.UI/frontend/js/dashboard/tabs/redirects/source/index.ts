import variableResource from '../../../../util/tools/variableresource.service'
import { RegexSourceStrategyFactory } from './regexsource';
import strategyCollection from './source.strategy'
import { UrlSourceStrategyFactory } from './urlsource'

strategyCollection.registerFactory(new UrlSourceStrategyFactory(variableResource));
strategyCollection.registerFactory(new RegexSourceStrategyFactory(variableResource));