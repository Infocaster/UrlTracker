import variableResource from '../../../../util/tools/variableresource.service';
import { ContentTargetStrategyFactory } from './implementations/contenttarget';
import strategyCollection from './target.strategy';
import { UrlTargetStrategyFactory } from './implementations/urltarget';

strategyCollection.registerFactory(new UrlTargetStrategyFactory(variableResource));
strategyCollection.registerFactory(new ContentTargetStrategyFactory(variableResource));