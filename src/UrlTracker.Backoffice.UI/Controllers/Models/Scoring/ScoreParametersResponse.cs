using System.Runtime.Serialization;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Scoring
{
    [DataContract]
    internal record ScoreParametersResponse(
        [property: DataMember(Name = "variableFactory")] double VariableFactor,
        [property: DataMember(Name = "redactionFactor")] double RedactionFactor,
        [property: DataMember(Name = "timeFactor")] double TimeFactor);
}
