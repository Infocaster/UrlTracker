export interface IDashboardFooter {
  version: string;
  links: Array<IDashboardFooterLink>;
}

export interface IDashboardFooterLink {
  url: string;
  title: string;
  target: string;
}
