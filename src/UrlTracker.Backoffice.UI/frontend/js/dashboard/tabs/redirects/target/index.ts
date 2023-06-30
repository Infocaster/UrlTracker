import variableResource from '../../../../util/tools/variableresource.service';
import { ContentTargetStrategyFactory } from './contenttarget';
import strategyCollection from './target.strategy';
import { UrlTargetStrategyFactory } from './urltarget';

strategyCollection.registerFactory(new UrlTargetStrategyFactory(variableResource));
strategyCollection.registerFactory(new ContentTargetStrategyFactory(variableResource));