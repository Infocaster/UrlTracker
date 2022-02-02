using System;

namespace UrlTracker.Resources.Website.SwaggerExtensions.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    sealed class SwaggerSummaryAttribute : Attribute
    {
        public SwaggerSummaryAttribute(string summary)
        {
            Summary = summary;
        }

        public string Summary { get; }
    }
}