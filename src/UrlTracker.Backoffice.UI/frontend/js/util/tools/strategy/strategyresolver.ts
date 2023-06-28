export class StrategyResolver<TItem, TStrategy, TFactory extends {getStrategy(item: TItem): TStrategy | undefined}> {

    private _factoryCollection: Array<TFactory> = [];

    constructor(private _fallback: TFactory) { }

    public registerFactory(factory: TFactory) {

        this._factoryCollection.push(factory);
    }

    public getStrategy(redirect: TItem) : TStrategy {

        let strategy = this._factoryCollection
            .map((item) => item.getStrategy(redirect))
            .find((item) => !!item);

        if (strategy) return strategy;
        return this._fallback.getStrategy(redirect)!;
    }
}