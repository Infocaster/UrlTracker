using System;

namespace InfoCaster.Umbraco.UrlTracker.Exceptions
{
	public class DashboardAlreadyInstalledException : Exception
    {
        public DashboardAlreadyInstalledException()
        {
        }

        public DashboardAlreadyInstalledException(string message)
            : base(message)
        {
        }

        public DashboardAlreadyInstalledException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}