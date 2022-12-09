using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UrlTracker.Backoffice.UI.Controllers.Models.Recommendations
{
    [DataContract]
    internal class RecommendationResponse
    {
        public RecommendationResponse(int id, bool ignore, string url, Guid strategy)
        {
            Id = id;
            Ignore = ignore;
            Url = url;
            Strategy = strategy;
        }

        [DataMember(Name = "id")]
        public int Id { get; set; }
        
        [DataMember(Name="ignore")]
        public bool Ignore { get; set; }
        
        [DataMember(Name="url")]
        public string Url { get; set; }
        
        [DataMember(Name="strategy")]
        public Guid Strategy { get; set; }
    }
}
