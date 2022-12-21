import { expect, fixture, html, nextFrame, oneEvent } from '@open-wc/testing'

import { UrlTrackerDashboard } from "../../js/dashboard/dashboard.lit"
import "../../js/dashboard/dashboard.lit"
import { IDashboardTab } from '../../js/dashboard/dashboardTab';


describe("dashboard", () => {

    const tabs: Array<IDashboardTab> = [
        { name: 'test', template: '/app_plugins/test' },
        { name: 'test2', template: '/app_plugins/test2' }
    ]

    it('should activate the first tab', async () => {

        // arrange
        let testSubject: UrlTrackerDashboard = await fixture(html`<urltracker-dashboard></urltracker-dashboard>`);
        let listener = oneEvent(testSubject, 'tabchange');

        // act
        testSubject.tabs = tabs;

        // assert
        const { detail } = await listener;
        expect(detail).to.be.equal(testSubject.tabs[0]);
    });

    it('should activate the tab that is clicked on', async () => {

        // arrange
        let testSubject: UrlTrackerDashboard = await fixture(html`<urltracker-dashboard .tabs="${tabs}"></urltracker-dashboard>`);

        let listener = oneEvent(testSubject, 'tabchange');

        // act
        (testSubject.shadowRoot?.querySelectorAll('uui-tab')[1] as HTMLElement).click();

        // assert
        const { detail } = await listener;
        expect(detail).to.be.equal(testSubject.tabs![1]);
    });

    it('should show the loading bar while loading', async () => {

        // arrange
        // ... the arrangement is the act
        
        // act
        let testSubject: UrlTrackerDashboard = await fixture(html`<urltracker-dashboard loading></urltracker-dashboard>`);

        // assert
        expect(testSubject.shadowRoot?.querySelector('uui-loader-bar')).to.not.be.null;
        expect(testSubject.shadowRoot?.querySelector('uui-tab-group')).to.be.null;
    });

    it('should hide the navigation if only one tab exists', async () => {

        // arrange
        // ... the arrangement is the act

        // act
        let testSubject: UrlTrackerDashboard = await fixture(html`<urltracker-dashboard .tabs="${[tabs[0]]}"></urltracker-dashboard>`);

        // assert
        expect(testSubject.shadowRoot?.querySelector('uui-tab-group')).to.be.null;
    })
})