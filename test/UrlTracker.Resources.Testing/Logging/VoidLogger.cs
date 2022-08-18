using System;
using UrlTracker.Core.Logging;

namespace UrlTracker.Resources.Testing.Logging
{
    public class VoidLogger : ILogger
    {
        private void Log(Type type, string severity, Exception exception, string message, params object[] propertyValues)
        { }

        public void Debug(Type reporting, string message)
        {
            Log(reporting, "Debug", null, message);
        }

        public void Debug(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            Log(reporting, "Debug", null, messageTemplate, propertyValues);
        }

        public void Error(Type reporting, Exception exception, string message)
        {
            Log(reporting, "Error", exception, message);
        }

        public void Error(Type reporting, Exception exception)
        {
            Log(reporting, "Error", exception, null);
        }

        public void Error(Type reporting, string message)
        {
            Log(reporting, "Error", null, message);
        }

        public void Error(Type reporting, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Log(reporting, "Error", exception, messageTemplate, propertyValues);
        }

        public void Error(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            Log(reporting, "Error", null, messageTemplate, propertyValues);
        }

        public void Fatal(Type reporting, Exception exception, string message)
        {
            Log(reporting, "Fatal", exception, message);
        }

        public void Fatal(Type reporting, Exception exception)
        {
            Log(reporting, "Fatal", exception, null);
        }

        public void Fatal(Type reporting, string message)
        {
            Log(reporting, "Fatal", null, message);
        }

        public void Fatal(Type reporting, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Log(reporting, "Fatal", exception, messageTemplate, propertyValues);
        }

        public void Fatal(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            Log(reporting, "Fatal", null, messageTemplate, propertyValues);
        }

        public void Info(Type reporting, string message)
        {
            Log(reporting, "Info", null, message);
        }

        public void Info(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            Log(reporting, "Info", null, messageTemplate, propertyValues);
        }

        public bool IsEnabled(Type reporting, Umbraco.Core.Logging.LogLevel level)
        {
            return true;
        }

        public void Verbose(Type reporting, string message)
        {
            Log(reporting, "Verbose", null, message);
        }

        public void Verbose(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            Log(reporting, "Verbose", null, messageTemplate, propertyValues);
        }

        public void Warn(Type reporting, string message)
        {
            Log(reporting, "Warning", null, message);
        }

        public void Warn(Type reporting, string messageTemplate, params object[] propertyValues)
        {
            Log(reporting, "Warning", null, messageTemplate, propertyValues);
        }

        public void Warn(Type reporting, Exception exception, string message)
        {
            Log(reporting, "Warning", exception, message);
        }

        public void Warn(Type reporting, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Log(reporting, "Warning", exception, messageTemplate, propertyValues);
        }
    }
}
