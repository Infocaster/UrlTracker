using System.Threading.Tasks;
using Umbraco.Core.Mapping;
using UrlTracker.Core.Intercepting.Models;

namespace UrlTracker.Core.Intercepting.Conversion
{
    public class MapperInterceptConverter<TFrom, TTo>
        : IInterceptConverter
    {
        private readonly UmbracoMapper _mapper;

        public MapperInterceptConverter(UmbracoMapper mapper)
        {
            _mapper = mapper;
        }

        public ValueTask<IIntercept> ConvertAsync(ICachableIntercept cachableIntercept)
        {
            if (!(cachableIntercept?.Info is TFrom fromInfo)) return new ValueTask<IIntercept>();
            return new ValueTask<IIntercept>(new InterceptBase<TTo>(_mapper.Map<TTo>(fromInfo)));
        }
    }
}
