using ClientDependency.Core.Logging;
//using ImageProcessor.Common.Exceptions;
using InfoCaster.Umbraco.UrlTracker.Modules;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Web;
//using umbraco.BusinessLogic;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using ILogger = Umbraco.Core.Logging.ILogger;
//using UmbracoLog = umbraco.BusinessLogic.Log;

namespace InfoCaster.Umbraco.UrlTracker.Helpers
{
    public static class LoggingHelper
    {
        private static ILogger logger = Current.Logger;

        public static void LogException(this Exception ex)
        {
            logger.Error(typeof(UrlTrackerComponent), ex);
        }

        public static void LogInformation(string message, params object[] args)
        {
            LogInformation(string.Format(message, args));
        }

        public static void LogInformation(string message)
        {
            if (UrlTrackerSettings.EnableLogging)
            {
                logger.Debug(typeof(UrlTrackerComponent), message);
            }
        }
    }
}