import { expect, fixture, html } from '@open-wc/testing';
import { IDashboardFooter } from '../../../js/dashboard/footer/dashboardFooter';
import { DashboardFooter } from '../../../js/dashboard/footer/dashboardFooter.lit'

describe(DashboardFooter.name, () => {

    let testSubject: DashboardFooter;

    beforeEach(async () => {

        testSubject = await fixture(html`<urltracker-dashboard-footer></urltracker-dashboard-footer>`);
    });

    it('shows nothing by default', () => {

        expect(testSubject.shadowRoot?.children).to.be.empty;
    });

    it('shows a footer when a model is provided', async () => {

        // arrange
        let model: IDashboardFooter = {
            logo: '/test.svg',
            logoUrl: 'https://example.com/',
            version: '1.0.0',
            links:[]
        };
        let localTestSubject = await fixture(html`<urltracker-dashboard-footer .model="${model}"></urltracker-dashboard-footer>`);

        // act

        // assert
        expect(localTestSubject.shadowRoot?.firstElementChild).has.tagName('footer');
    });

    describe('properties', () => {

        it('has a model property', () => {

            expect(testSubject).to.have.property('model');
        });
    });
});