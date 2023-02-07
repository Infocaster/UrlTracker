export interface IDashboardFooter {
    logo: string,
    logoUrl: string,
    version: string,
    links: Array<IDashboardFooterLink>
};

export interface IDashboardFooterLink {

    url: string,
    title: string,
    target: string
};