using System;
using System.Web;
using Moq;

namespace UrlTracker.Resources.Testing.Mocks
{
    public class HttpContextMock
    {
        public HttpContextMock(Uri incomingUrl = null)
        {
            ContextMock = new Mock<HttpContextBase>();
            RequestMock = new Mock<HttpRequestBase>();
            ResponseMock = new Mock<HttpResponseBase>();

            ContextMock.Setup(obj => obj.Request).Returns(RequestMock.Object);
            ContextMock.Setup(obj => obj.Response).Returns(ResponseMock.Object);
            if (incomingUrl != null)
            {
                if (!incomingUrl.IsAbsoluteUri) throw new ArgumentException("Incoming url must always be absolute");
                RequestMock.Setup(obj => obj.Url).Returns(incomingUrl);
            }
        }

        public HttpContextBase Context => ContextMock.Object;
        public HttpResponseBase Response => ResponseMock.Object;
        public HttpRequestBase Request => RequestMock.Object;

        public Mock<HttpContextBase> ContextMock { get; }
        public Mock<HttpRequestBase> RequestMock { get; }
        public Mock<HttpResponseBase> ResponseMock { get; }
    }
}
