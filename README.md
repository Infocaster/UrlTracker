<h3 align="center">
<img height="100" src="docs/assets/infocaster_nuget_yellow.svg">
</h3>

<h1 align="center">
URL Tracker

[![Downloads](https://img.shields.io/nuget/dt/UrlTracker?color=ff0069)](https://www.nuget.org/packages/UrlTracker/)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/UrlTracker?color=ffc800)](https://www.nuget.org/packages/UrlTracker/)
![GitHub](https://img.shields.io/github/license/Infocaster/UrlTracker?color=ff0069)

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
<add key="urlTracker:disabled" value="false" />
<add key="urlTracker:trackingDisabled" value="false" />
<add key="urlTracker:notFoundTrackingDisabled" value="false" />
<add key="urlTracker:enableLogging" value="false" />
<add key="urlTracker:appendPortNumber" value="false" />
<add key="urlTracker:hasDomainOnChildNode" value="false" />
<add key="urlTracker:maxCachedIntercepts" value="5000" />
<add key="urlTracker:cacheRegexRedirects" value="true" />
<add key="urlTracker:interceptSlidingCacheMinutes" value="2880" />
<add key="urlTracker:enableInterceptCaching" value="true" />
<add key="urlTracker:blockedUrlsList" value="" />
```

|Setting|Type|Description|
|------:|:---|:----------|
|Disabled | bool | Set this value to `true` to completely disable the URL Tracker. The URL Tracker will not intercept any requests nor track any content updates|
|Tracking disabled | bool | Set this value to `true` to disable tracking of content changes. The URL Tracker will not automatically create redirects when content is updated|
|Not found tracking disabled | bool | Set this value to `true` to disable tracking of Not Found responses.|
|Enable logging | bool | Set this value to `true` to allow the URL Tracker to write logs to the Umbraco native logger. Most logs from the URL Tracker are written at Debug or Verbose level.|
|Append port number | bool | Set this value to `true` to add a port number behind the host component of a redirect url. This setting is ignored when the application is hosted on the default port 80.|
|Has domain on child node | bool | Set this value to `true` if your website has domains configured on pages that are not in the root of the website.|
|Cache regex redirects | bool | Set this value to `false` to disable caching of regex redirects. The URL Tracker will read all the regex redirects from the database on each request.|
|Max cached intercepts | long | The maximum amount of intercepts to cache. This not only includes redirects, but also pass through and gone responses.|
|Intercept sliding cache minutes | int? | Set this value to the amount of minutes that an intercept should be cached after the last use before it expires. Set this value to `null` to cache forever|
|Enable intercept caching | bool | set this value to `false` to completely disable caching of intercepts. The intercept will be recalculated for each url on every request.|
|BlockedUrlsList | List of string | Add strings to this array that should be blocked from the UrlTracker. The strings in this array will be filtered out and shall never appear as a 404. This is great for things that crawlers search for like wpadmin and prevents the UrlTracker from getting clogged. Keep in mind that the filter works with String.Contains so there is no need to use entire urls.|

## Contributing
The URL Tracker is open for contributions. If you want to contribute to the source code, please check out our [guide to contributing](/docs/CONTRIBUTING.md).  
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