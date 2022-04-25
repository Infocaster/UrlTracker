using System;
using System.Linq;
using System.Threading.Tasks;
using UrlTracker.Core.Domain.Models;
using UrlTracker.Core.Intercepting;
using UrlTracker.Core.Intercepting.Conversion;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core
{
    public class InterceptService
        : IInterceptService
    {
        private readonly IIntermediateInterceptService _intermediateInterceptService;
        private readonly IInterceptConverterCollection _interceptConverters;

        public InterceptService(IIntermediateInterceptService intermediateInterceptService,
                                IInterceptConverterCollection interceptConverters)
        {
            _intermediateInterceptService = intermediateInterceptService;
            _interceptConverters = interceptConverters;
        }

        public async Task<IIntercept> GetAsync(Url url)
        {
            if (!url.AvailableUrlTypes.Contains(UrlType.Absolute))
            {
                throw new ArgumentException("Url must be absolute", nameof(url));
            }

            ICachableIntercept intermediateIntercept = await _intermediateInterceptService.GetAsync(url);

            var result = await _interceptConverters.ConvertAsync(intermediateIntercept);
            return result;
        }
    }
}
