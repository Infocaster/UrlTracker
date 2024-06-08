import { UmbLocalizationDictionary } from '@umbraco-cms/backoffice/localization-api';

const translations: UmbLocalizationDictionary = {
  dashboardTabs: {
    UrlTracker: 'URL Tracker',
  },
  treeHeaders: {
    urlTrackerTreeGroup: 'URL Tracker',
    urlTrackerSettingsTree: 'URL Tracker settings',
  },
  urlTrackerDashboardTabs: {
    dashboard: 'Dashboard',
    recommendations: 'Recommendations',
    redirects: 'Redirects',
    advancedRedirects: 'Advanced redirects',
  },
  urlTrackerDashboardTabLabels: {
    dashboard: 'Dashboard',
    recommendations: 'Recommendations',
    redirects: 'Redirects',
    advancedRedirects: 'Advanced redirects',
  },
  urlTrackerNotifications: {
    landingpagewelcometitle: 'Welcome to the URL Tracker',
    landingpagewelcomebody:
      "On this overview, a selection of the most important recommendations is compiled for your convenience. Improve your visitors' experience by solving these issues",
    recommendationswelcometitle: 'Recommendations overview',
    recommendationswelcomebody:
      'Over here you can see all current recommendations. By default, recommendations are ordered by their importance. Use the inputs and dropdowns to filter and sort the recommendations.',
    redirectswelcometitle: 'Redirects overview',
    redirectswelcomebody:
      "This tab shows the redirects that are configured for your website. The various controls on this page allow you to create, delete and edit your redirects. This page only shows standard redirects. Check out the 'advanced redirects' tab to see all the redirects.",
    advancedredirectswelcometitle: 'Advanced redirects overview',
    advancedredirectswelcomebody:
      'In addition to the standard redirects, this page shows you all redirects with advanced configuration options.',
  },
  urlTrackerDashboardFooter: {
    logo: '/App_Plugins/UrlTracker/assets/layout/footerlogo.svg',
    logourl: 'https://infocaster.net',
    featurelabel: 'Suggest a feature',
    buglabel: 'Report a bug',
    wikilabel: 'Open the wiki',
  },
  urlTrackerRedirectSource: {
    url: 'URL',
    unknown: 'Unknown source type',
    regex: 'Pattern',
  },
  urlTrackerRedirectTarget: {
    redirectto: 'Redirects to',
    unknown: 'Unknown target type',
    url: 'URL',
    content: 'Content',
    media: 'Media',
    contenterror: 'The selected content does not exist or is in the trashcan',
  },
  urlTrackerRedirectActions: {
    header: 'Actions',
    new: 'New redirect',
    export: 'Export redirects',
  },
  urlTrackerRedirectUpload: {
    header: 'Import',
    info: 'Drop a csv file in the box below to import redirects.',
    help: 'You can download template here',
  },
  urlTrackerRecommendationType: {
    image: 'Image was not found',
    imagedescription:
      '\
      This entry indicates that an image could not be found. As a consequence, \
      certain pages may not be displayed correctly and visitors might lack visual context to the content on particular pages. \
      Check out the referrer information to see on which pages the image is requested.\
      Redirect this url to an existing image to restore the user experience. Alternatively, \
      you can check out the referrer overview below to see from which pages the image is requested.\
      After manually repairing the images, this recommendation will eventually disappear.\
      ',
    file: 'File was not found',
    filedescription:
      '\
      This entry indicates that a file could not be found. As a consequence,\
      certain pages may contain broken links to downloadable content. Visitors may not be able to download the files on those pages.\
      Check the referrer information to see on which pages the downloadable content is referenced.\
      Repair the downloads on those pages and this recommendation will eventually disappear.\
      Alternatively, you can create a redirect to redirect all urls to a new file at once.\
      ',
    page: 'Page was not found',
    pagedescription:
      "\
      This entry indicates that a page could not be found. This means that visitors are trying to visit pages that don't exist.\
      Create a redirect to redirect users to a new content page.\
      ",
    technicalFile: 'Technical file was not found',
    technicalFiledescription:
      "\
      This entry indicates that a technical file could not be found. The consequences of this depend on the nature of the file.\
      You should contact your administrator to get instructions on what to do with this.\
      If you really know what you're doing, you may create a redirect for this file, but you likely shouldn't.\
      ",
    unknown: 'Unknown recommendation type',
  },
  urlTrackerRecommendationImportance: {
    VERY_IMPORTANT: 'VERY IMPORTANT',
    IMPORTANT: 'IMPORTANT',
    MODERATELY_IMPORTANT: 'MODERATELY IMPORTANT',
    SLIGHTLY_IMPORTANT: 'SLIGHTLY IMPORTANT',
    NOT_IMPORTANT: 'NOT IMPORTANT',
  },
  urlTrackerRecommendationItem: {
    actions: 'Recommendations',
    'action-temporary': 'Create a temporary redirect',
    'action-permanent': 'Create a permanent redirect',
    'action-ignore': 'Ignore this',
  },
  urlTrackerNewRedirect: {
    permanent: 'Is the redirect permanent?',
    'permanent-info':
      'A permanent redirect is better for your site’s performance, but cannot be changed afterwards. Permanent redirects are a signal to google and other search engines to update their search results to the target that you specify. It makes the internet “forget” your incoming URLs so to speak.',
    'incoming-url': 'Incoming URL',
    'incoming-url-info': 'Enter the URL which should be redirected from',
    'outgoing-url': 'Outgoing URL',
    'outgoing-url-info': 'Select where the URL should redirect to',
  },
  urlTrackerRecommendationFilter: {
    'search-placeholder': 'Type to search...',
  },
};

export default translations;
