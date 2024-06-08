import { html } from '@umbraco-cms/backoffice/external/lit';
import tabStrategy, { createTabStrategy } from './tab';

import './tabs/landingpage.lit';
import './tabs/recommendations.lit';
import './tabs/redirects.lit';
import './tabs/redirects/source';
import './tabs/redirects/target';

tabStrategy.push(createTabStrategy('dashboard', html`<urltracker-landing-tab></urltracker-landing-tab>`));
tabStrategy.push(
  createTabStrategy('recommendations', html`<urltracker-recommendations-tab></urltracker-recommendations-tab>`),
);
tabStrategy.push(createTabStrategy('redirects', html`<urltracker-redirect-tab></urltracker-redirect-tab>`));
tabStrategy.push(
  createTabStrategy('advancedRedirects', html`<urltracker-redirect-tab advanced></urltracker-redirect-tab>`),
);
