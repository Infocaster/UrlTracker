import { TemplateResult } from 'lit';

export function createTabStrategy(
  alias: string,
  template: TemplateResult,
  showQuickCreate: boolean = false,
): ITabStrategy {
  return {
    nameKey: 'urlTrackerDashboardTabs_' + alias,
    labelKey: 'urlTrackerDashboardTabLabels_' + alias,
    template: template,
    alias: alias,
    showQuickCreate: showQuickCreate,
  };
}

export interface ITabStrategy {
  nameKey: string;
  labelKey: string;
  template: unknown;
  alias: string;
  showQuickCreate: boolean;
}

export interface ITab {
  name: string;
  label?: string;
  template: unknown;
  alias: string;
  showQuickCreate: boolean;
}

export type TabStrategyCollection = ITabStrategy[];

export default [] as TabStrategyCollection;
