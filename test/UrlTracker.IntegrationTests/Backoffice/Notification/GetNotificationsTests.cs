﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlTracker.Backoffice.UI.UserNotifications;

namespace UrlTracker.IntegrationTests.Backoffice.Notification
{
    public class GetNotificationsTests
        : NotificationTestBase
    {
        private const string _endpoint = _endpointBase + "/get";

        [TestCase(TestName = "Get returns a list of notifications for a given alias")]
        public async Task Get_NormalFlow_ReturnsResult()
        {
            // arrange

            // act
            var response = await WebsiteFactory.CreateStandardClient().GetAsync(_endpoint + "?alias=" + UrlTracker.Backoffice.UI.Defaults.Extensions.Overview);

            // assert
            Assert.That(response.IsSuccessStatusCode, Is.True);
            var result = await DeserializeResponseAsync<UrlTrackerUserNotificationOptions>(response);
            Assert.That(result?.Notifications.Any(), Is.True);
        }
    }
}
