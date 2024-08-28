<h3 align="center">
<img height="100" src="https://raw.githubusercontent.com/Infocaster/.github/main/assets/infocaster_nuget_yellow.svg">
</h3>

<h1 align="center">
URL Tracker

[![Downloads](https://img.shields.io/nuget/dt/UrlTracker?color=ff0069)](https://www.nuget.org/packages/UrlTracker/)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/UrlTracker?color=ffc800)](https://www.nuget.org/packages/UrlTracker/)
![GitHub](https://img.shields.io/github/license/Infocaster/UrlTracker?color=ff0069)

</h1>

The URL Tracker makes url management easy. Your visitors will no longer be lost on your website as the URL Tracker watches how your website changes. Page moved or renamed? No problem! The URL Tracker knows this and directs your visitors to the right page. Not only does this provide a nice experience for your visitors, it also helps you maintain your rightful spot at the top in Google (and other search engines).
The URL Tracker puts you in control by giving you the means to manage your own redirects. It can be as simple as a redirect from a url to an umbraco page, but you can make it as sophisticated as you like using Regular Expressions. The URL Tracker is an essential tool for content editors who want to migrate to a new Umbraco website, so they can guide their users to the right place.

## Requirements
The URL Tracker is guaranteed to work with SQL Server databases. The URL Tracker does not work with SQL Compact Edition databases.

## Getting Started
The URL Tracker is available via NuGet. Visit [the URL Tracker on NuGet](https://www.nuget.org/packages/UrlTracker/) for instructions on how to install the URL Tracker package in your website.
Now build your project and you should be ready to make your visitors happy!

## Configuration
The URL Tracker has several configurable properties that can be changed in appsettings.json:

```json
{
    "UrlTracker": {
        "Enable": true,
        "EnableLogging": false,
        "BlockedUrlsList": [],
        "AllowedUserAgents": [],
        "Backoffice": {
            "Notifications":{
                "Enable": true
            }
        },
        "Caching": {
            "Memory": {
                "CacheRegexRedirects": true,
                "InterceptSlidingCacheMinutes": 2880,
                "MaxCachedIntercepts": 5000,
                "EnableInterceptCaching": true,
                "EnableActiveCache": false
            }
        },
        "Pipeline": {
            "Enable": true,
            "EnableClientErrorTracking": true
        }
    }
}
```

|                                                   Name | Type     | Description |
|-------------------------------------------------------:|:---------|:------------|
|                                      UrlTracker:Enable | bool     | Set this value to `false` to completely disable the URL Tracker. The URL Tracker will not intercept any requests nor track any content updates
|                               UrlTracker:EnableLogging | bool     | Set this value to `true` to allow the URL Tracker to write logs to the Umbraco native logger. Most logs from the URL Tracker are written at Debug or Verbose level.
|                             UrlTracker:BlockedUrlsList | string[] | Add strings to this array that should be blocked from the UrlTracker. The strings in this array will be filtered out and shall never appear as a 404. This is great for things that crawlers search for like wpadmin and prevents the UrlTracker from getting clogged. Keep in mind that the filter works with String.Contains so there is no need to use entire urls.
|                           UrlTracker:AllowedUserAgents | string[] | Add strings to this array to use in a User Agent check. The UrlTracker will check if a request has one of these user agents before logging it as a 404 request, this prevents (by best effor) a lot of calls from bots. This defaults to: "IE", "Opera", "Google", "Safari", "Google Chrome", "Edge", "Mozilla", "Firefox". The [uap-csharp](https://github.com/ua-parser/uap-csharp) package is used to check the browser family.
|             UrlTracker:Backoffice:Notifications:Enable | bool     | Set this value to `false` to disable tracking of content changes. The URL Tracker will not automatically create redirects when content is updated
|          UrlTracker:Caching:Memory:CacheRegexRedirects | bool     | Set this value to `false` to disable caching of regex redirects. By default, all regex redirects are cached in memory to improve performance.
| UrlTracker:Caching:Memory:InterceptSlidingCacheMinutes | int?     | Set this value to the time in minutes that all redirects should be cached. By default, all redirects are cached for 2 days. Set to `null` to cache indefinitely.
|          UrlTracker:Caching:Memory:MaxCachedIntercepts | long     | Set this value to the amount of intercepts that should be cached by the UrlTracker. This not only includes redirects, but also 200 OK responses, 410 GONE responses and 404 NOT FOUND responses.
|       UrlTracker:Caching:Memory:EnableInterceptCaching | bool     | Set this value to `false` to completely disable redirect caching.
|            UrlTracker:Caching:Memory:EnableActiveCache | bool     | Set this value to `true` to enable active caching. Active caching should be enabled when you notice bottlenecks on your database. This may significantly increase your performance. Notice that this will load all redirects into the working memory, so your memory usage might increase, depending on the amount redirects that you have
|                             UrlTracker:Pipeline:Enable | bool     | Set this value to `false` to disable all URL Tracker middleware. The URL Tracker will no longer redirect requests nor track client error responses.
|          UrlTracker:Pipeline:EnableClientErrorTracking | bool     | Set this value to `false` to disable tracking of client error responses.

Read more in-depth documentation in [the URL Tracker wiki](https://github.com/Infocaster/UrlTracker/wiki).

## Contributing
The URL Tracker is open for contributions. If you want to contribute to the source code, please check out our [guide to contributing](/docs/CONTRIBUTING.md).  
Many people have already contributed to this awesome project:

<a href="https://github.com/Infocaster/UrlTracker/graphs/contributors">
<img src="https://contrib.rocks/image?repo=Infocaster/UrlTracker" />
</a>

*Made with [contributors-img](https://contrib.rocks).*

<a href="https://infocaster.net">
<img align="right" height="200" src="https://raw.githubusercontent.com/Infocaster/.github/main/assets/Infocaster_Corner.png?raw=true">
</a>
