import { TemplateResult } from 'lit';

export function createTabStrategy(alias: string, template: TemplateResult): ITabStrategy {
  return {
    nameKey: 'urlTrackerDashboardTabs_' + alias,
    labelKey: 'urlTrackerDashboardTabLabels_' + alias,
    template: template,
    alias: alias,
  };
}

export interface ITabStrategy {
  nameKey: string;
  labelKey: string;
  template: unknown;
  alias: string;
}

export interface ITab {
  name: string;
  label?: string;
  template: unknown;
  alias: string;
}

export type TabStrategyCollection = ITabStrategy[];

export default [] as TabStrategyCollection;
