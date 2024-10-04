export interface IDashboardPageResponse {
  results: Array<IDashboardPage>;
}

export interface IDashboardPage {
  alias: string;
  view: string;
}
