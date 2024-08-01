export declare global {
  interface Window {
    URL_TRACKER: {
      TabBuilder: TabBuilder;
    };
  }
}

declare module 'angular' {
  typeof import('angular');
}
