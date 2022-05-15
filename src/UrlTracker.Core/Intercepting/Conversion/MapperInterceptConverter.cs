using System.Threading.Tasks;
using Umbraco.Cms.Core.Mapping;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting.Conversion
{
    public class MapperInterceptConverter<TFrom, TTo>
        : IInterceptConverter
    {
        private readonly IUmbracoMapper _mapper;

        public MapperInterceptConverter(IUmbracoMapper mapper)
        {
            _mapper = mapper;
        }

        public ValueTask<IIntercept?> ConvertAsync(ICachableIntercept cachableIntercept)
        {
            if (cachableIntercept.Info is not TFrom fromInfo) return new ValueTask<IIntercept?>();
            return new ValueTask<IIntercept?>(new InterceptBase<TTo>(_mapper.Map<TTo>(fromInfo)!));
        }
    }
}
