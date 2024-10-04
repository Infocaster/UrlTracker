import variableResource from '../../../../util/tools/variableresource.service';
import strategyCollection from './recommendation.strategy';
import { ImageRecommendationTypeStrategyFactory } from './imageRecommendation';
import { FileRecommendationTypeStrategyFactory } from './fileRecommendation';
import { PageRecommendationTypeStrategyFactory } from './pageRecommendation';
import { TechnicalFileRecommendationTypeStrategyFactory } from './technicalFileRecommendation';

strategyCollection.registerFactory(new ImageRecommendationTypeStrategyFactory(variableResource));
strategyCollection.registerFactory(new FileRecommendationTypeStrategyFactory(variableResource));
strategyCollection.registerFactory(new PageRecommendationTypeStrategyFactory(variableResource));
strategyCollection.registerFactory(new TechnicalFileRecommendationTypeStrategyFactory(variableResource));
