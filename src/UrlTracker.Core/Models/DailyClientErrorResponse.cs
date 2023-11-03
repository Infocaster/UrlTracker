using System;

namespace UrlTracker.Core.Models
{
    public class DailyClientErrorResponse
    {
        public DailyClientErrorResponse(DateTime date, int occurrances)
        {
            Date = date;
            Occurrances = occurrances;
        }

        public DateTime Date { get; }
        public int Occurrances { get; }
    }
}