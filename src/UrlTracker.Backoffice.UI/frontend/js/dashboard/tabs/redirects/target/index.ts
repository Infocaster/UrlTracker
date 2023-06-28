import variableResource from '../../../../util/tools/variableresource.service';
import strategyCollection from './target.strategy';
import { UrlTargetStrategyFactory } from './urltarget';

strategyCollection.registerFactory(new UrlTargetStrategyFactory(variableResource))