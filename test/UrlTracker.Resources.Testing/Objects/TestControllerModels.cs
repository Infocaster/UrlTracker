using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace UrlTracker.Resources.Testing.Objects
{
    public class TestActionDescriptor : HttpActionDescriptor
    {
        public TestActionDescriptor(string actionName, Type returnType)
        {
            ActionName = actionName;
            ReturnType = returnType;
            Parameters = new Collection<HttpParameterDescriptor>();
        }

        public override string ActionName { get; }

        public override Type ReturnType { get; }
        public Collection<HttpParameterDescriptor> Parameters { get; }

        public override Task<object> ExecuteAsync(HttpControllerContext controllerContext, IDictionary<string, object> arguments, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Collection<HttpParameterDescriptor> GetParameters()
        {
            return Parameters;
        }
    }

    public class TestParameterDescriptor : HttpParameterDescriptor
    {
        private bool _isOptional;
        private object _defaultValue;

        public TestParameterDescriptor(string parameterName, Type parameterType)
        {
            ParameterName = parameterName;
            ParameterType = parameterType;
            _isOptional = false;
            _defaultValue = null;
        }

        public override string ParameterName { get; }

        public override Type ParameterType { get; }

        public void SetIsOptional(bool value) => _isOptional = value;
        public override bool IsOptional => _isOptional;

        public void SetDefaultValue(object value) => _defaultValue = value;
        public override object DefaultValue => _defaultValue;
    }
}
