import { TemplateResult } from 'lit';
import TabStrategy from '../../../dashboard/tab';

export class TabBuilder {
  public addTab(alias: string, _: TemplateResult) {
    console.log('public acces test');
    console.log(this._checkTabNameUnqiue(alias));
  }

  private _checkTabNameUnqiue(alias: string) {
    const tabs = TabStrategy;
    console.log(TabStrategy);
    return !tabs.some((tab) => tab.nameKey === alias);
  }
}
