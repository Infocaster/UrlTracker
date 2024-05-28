import { IControllerUrlResource, IUrlResource } from "@/util/tools/urlresource.service";
import { Axios } from "axios";
import { IRecommendationResponse } from "./recommendation.service";

export class ScoringService {

    private resourcePromise?: Promise<void>;
    private redactionScores?: IRedactionScoreResponse[];
    private parameters?: IScoreParametersResponse;

    constructor(private axios: Axios, private urlResource: IUrlResource) {}

    private get controller(): IControllerUrlResource {
        return this.urlResource.getController("Scoring");
    }

    public async getScore(recommendation: IRecommendationResponse): Promise<number> {

        if (!this.redactionScores) {

            if (!this.resourcePromise) {

                this.resourcePromise = this.ensureResources();
                this.resourcePromise.finally(() => this.resourcePromise = undefined);
            }

            // This check is necessary, in case the 'finally' has already ran.
            // It's possible that the promise has already been truncated.
            if (this.resourcePromise) {

                await this.resourcePromise;
            }
        }

        // At this point, we know for certain that the resources exist
        // Calculate: (C1 × SR + C2 × SV) × 0.5^(ST / C3)
        const sv = recommendation.score;
        const sr = this.redactionScores?.find(s => s.key === recommendation.strategy)?.score ?? 0;
        const st = ((new Date()).getTime() - recommendation.updatedate.getTime()) / (1000 * 60 * 60 * 24);

        const adjustedsr = this.parameters!.redactionFactor * sr;
        const adjustedsv = this.parameters!.variableFactor * sv;
        const adjustedst = 0.5 ** (st / this.parameters!.timeFactor);

        const score = 
            (adjustedsr
            + adjustedsv)
            * adjustedst;

        return score;
    }

    private async ensureResources(): Promise<void> {
        
        const [redactionScores, parameters] = await Promise.all([this.getRedactionScores(), this.getParameters()]);
        this.redactionScores = redactionScores;
        this.parameters = parameters;
    }

    private getRedactionScores(): Promise<IRedactionScoreResponse[]> {

        const url = this.controller.getUrl('RedactionScores');
        return this.axios.get<IRedactionScoreResponse[]>(url)
            .then(response => response.data);
    }

    private getParameters(): Promise<IScoreParametersResponse> {

        const url = this.controller.getUrl('ScoreParameters');
        return this.axios.get<IScoreParametersResponse>(url)
            .then(response => response.data);
    }
}

interface IScoreParametersResponse {
    variableFactor: number,
    redactionFactor: number,
    timeFactor: number
}

interface IRedactionScoreResponse {
    key: string,
    score: number
}