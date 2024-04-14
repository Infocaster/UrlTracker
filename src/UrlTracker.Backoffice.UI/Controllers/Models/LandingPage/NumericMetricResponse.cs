using System.Runtime.Serialization;

namespace UrlTracker.Backoffice.UI.Controllers.Models.LandingPage
{
    [DataContract]
    internal record NumericMetricResponse(
        [property: DataMember(Name = "value")] int Value);
}
