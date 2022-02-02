using System;

namespace UrlTracker.Core
{
    public static partial class Defaults
    {
        public static class Parameters
        {
            public static DateTime StartDate => new DateTime(1970, 1, 1);
            public static DateTime EndDate => DateTime.UtcNow;
        }
    }
}
