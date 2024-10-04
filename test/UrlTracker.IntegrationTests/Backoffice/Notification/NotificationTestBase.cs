using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlTracker.IntegrationTests.Backoffice.Notification
{
    public abstract class NotificationTestBase
        : BackofficeIntegrationTestBase
    {
        protected const string _endpointBase = "/umbraco/backoffice/urltracker/notifications";
    }
}
