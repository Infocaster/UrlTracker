import { html } from 'lit';
import tabStrategy, { createTabStrategy } from './tab';
import './tabs/landingpage.lit';
import './tabs/recommendations.lit';
import './tabs/redirects.lit';

tabStrategy.push(createTabStrategy('dashboard', html`<urltracker-landing-tab></urltracker-landing-tab>`, true));
tabStrategy.push(
  createTabStrategy('recommendations', html`<urltracker-recommendations-tab></urltracker-recommendations-tab>`, true),
);
tabStrategy.push(createTabStrategy('redirects', html`<urltracker-redirect-tab></urltracker-redirect-tab>`));
tabStrategy.push(
  createTabStrategy('advancedRedirects', html`<urltracker-redirect-tab advanced></urltracker-redirect-tab>`),
);
