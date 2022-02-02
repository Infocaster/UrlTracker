<h3 align="center">
<img height="100" src="docs/assets/infocaster_nuget_yellow.svg">
</h3>

<h1 align="center">
URL Tracker

![License](https://img.shields.io/github/license/Infocaster/UrlTracker)
[![Nuget](https://img.shields.io/nuget/v/UrlTracker.svg)](https://www.nuget.org/packages/UrlTracker/)

</h1>

*This project is a continuation of [the original URL Tracker, created by Kipusoep](https://github.com/kipusoep/UrlTracker).*

The URL Tracker makes url management easy. Your visitors will no longer be lost on your website as the URL Tracker watches how your website changes. Page moved or renamed? No problem! The URL Tracker knows this and directs your visitors to the right page. Not only does this provide a nice experience for your visitors, it also helps you maintain your rightful spot at the top in Google (and other search engines).
The URL Tracker puts you in control by giving you the means to manage your own redirects. It can be as simple as a redirect from a url to an umbraco page, but you can make it as sophisticated as you like using Regular Expressions. The URL Tracker is an essential tool for content editors who want to migrate to a new Umbraco website, so they can guide their users to the right place.

## Requirements
The URL Tracker is guaranteed to work with SQL Server databases. The URL Tracker does not work with SQL Compact Edition databases.

## Getting Started
The URL Tracker is available via NuGet. Visit [the URL Tracker on NuGet](https://www.nuget.org/packages/UrlTracker/) for instructions on how to install the URL Tracker package in your website.
Once installed, build your project and you're ready to make your visitors happy.

## Configuration
Upon installation, the URL Tracker will have added several configurations in your Web.config file in the AppSettings section:

```xml
<Add key="urlTracker:disabled" value="false" />
<Add key="urlTracker:trackingDisabled" value="false" />
<Add key="urlTracker:notFoundTrackingDisabled" value="false" />
<Add key="urlTracker:enableLogging" value="false" />
<Add key="urlTracker:appendPortNumber" value="false" />
<Add key="urlTracker:hasDomainOnChildNode" value="false" />
```

- **Disabled** | Set this value to `true` to completely disable the URL Tracker. The URL Tracker will not intercept any requests nor track any content updates
- **Tracking disabled** | Set this value to `true` to disable tracking of content changes. The URL Tracker will not automatically create redirects when content is updated
- **Not found tracking disabled** | Set this value to `true` to disable tracking of Not Found responses.
- **Enable logging** | Set this value to `true` to allow the URL Tracker to write logs to the Umbraco native logger. Most logs from the URL Tracker are written at Debug or Verbose level.
- **Append port number** | Set this value to `true` to add a port number behind the host component of a redirect url. This setting is ignored when the application is hosted on the default port 80.
- **Has domain on child node** | Set this value to `true` if your website has domains configured on pages that are not in the root of the website.

## Contributing
The URL Tracker is open for contributions. If you want to contribute to the source code, please check out our [guide to contributing](https://github.com/Infocaster/.github/blob/main/CONTRIBUTING.md).  
Many people have already contributed to this awesome project:

<a href="https://github.com/Infocaster/UrlTracker/graphs/contributors">
<img src="https://contrib.rocks/image?repo=Infocaster/UrlTracker" />
</a>

*Made with [contributors-img](https://contrib.rocks).*

-----

## Credits ##
Credits from the original developer:
*   **InfoCaster** | Being able to combine 'work' with package development and thanks to colleagues for inspiration.
*   **Richard Soeteman** | Richard came up with the idea for a package which keeps track of URLs of umbraco nodes.
*   **The uComponents project** | For inspiring me to create a single-assembly package solution.
<a href="https://infocaster.net">
<img align="right" height="200" src="docs/assets/Infocaster_Corner.png">
</a>