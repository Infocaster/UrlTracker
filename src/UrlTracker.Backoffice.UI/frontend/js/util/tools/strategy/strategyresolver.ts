type StrategyFactory<TItem, TStrategy> = { getStrategy(item: TItem): TStrategy | undefined };

export class StrategyResolver<TItem, TStrategy> {
  private _unsafeResolver: UnsafeStrategyResolver<TItem, TStrategy> = new UnsafeStrategyResolver<TItem, TStrategy>();

  constructor(private _fallback: StrategyFactory<TItem, TStrategy>) {}

  public registerFactory(factory: StrategyFactory<TItem, TStrategy>) {
    this._unsafeResolver.registerFactory(factory);
  }

  public getStrategy(item: TItem): TStrategy {
    const strategy = this._unsafeResolver.getStrategy(item);

    if (strategy) return strategy;
    return this._fallback.getStrategy(item)!;
  }
}

export class UnsafeStrategyResolver<TItem, TStrategy> {
  private _factoryCollection: Array<StrategyFactory<TItem, TStrategy>> = [];

  public registerFactory(factory: StrategyFactory<TItem, TStrategy>) {
    this._factoryCollection.push(factory);
  }

  public getStrategy(item: TItem): TStrategy | undefined {
    const strategy = this._factoryCollection.map((i) => i.getStrategy(item)).find((i) => !!i);

    return strategy;
  }
}
