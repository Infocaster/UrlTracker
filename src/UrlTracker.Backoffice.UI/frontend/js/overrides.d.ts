export declare global {
  interface Window {
    URL_TRACKER: {
      TabBuilder: TabBuilder;
    };
    Umbraco: any;
  }
}

declare module 'angular' {
  typeof import('angular');
}
