using System.Threading.Tasks;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Backoffice.UI.Controllers.Models.Redirects;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Backoffice.UI.Controllers.RequestHandlers
{
    internal interface IRedirectRequestHandler
    {
        RedirectResponse Create(RedirectRequest request);
        RedirectResponse? Delete(int id);
        RedirectResponse? Get(int id);
        Task<RedirectCollectionResponse> GetAsync(ListRedirectRequest request);
        RedirectResponse? Update(int id, RedirectRequest request);
    }

    internal class RedirectRequestHandler : IRedirectRequestHandler
    {
        private readonly IRedirectRepository _redirectRepository;
        private readonly IScopeProvider _scopeProvider;
        private readonly IUmbracoMapper _mapper;

        public RedirectRequestHandler(IRedirectRepository redirectRepository, IScopeProvider scopeProvider, IUmbracoMapper mapper)
        {
            _redirectRepository = redirectRepository;
            _scopeProvider = scopeProvider;
            _mapper = mapper;
        }

        public async Task<RedirectCollectionResponse> GetAsync(ListRedirectRequest request)
        {
            using var scope = _scopeProvider.CreateScope();

            var entities = await _redirectRepository.GetAsync(request.Page * request.PageSize, request.PageSize, request.Query, true);
            return _mapper.Map<RedirectCollectionResponse>(entities)!;
        }

        public RedirectResponse? Get(int id)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);

            var entity = _redirectRepository.Get(id);
            return _mapper.Map<RedirectResponse>(entity);
        }

        public RedirectResponse Create(RedirectRequest request)
        {
            using var scope = _scopeProvider.CreateScope();

            var entity = _mapper.Map<IRedirect>(request)!;
            _redirectRepository.Save(entity);

            scope.Complete();
            return _mapper.Map<RedirectResponse>(entity)!;
        }

        public RedirectResponse? Update(int id, RedirectRequest request)
        {
            using var scope = _scopeProvider.CreateScope();

            var entity = _redirectRepository.Get(id);
            if (entity is null) return null;

            var newEntity = _mapper.Map<IRedirect>(request)!;
            newEntity.Id = entity.Id;
            if (newEntity.Key == default) newEntity.Key = entity.Key;
            newEntity.CreateDate = entity.CreateDate;

            _redirectRepository.Save(newEntity);

            scope.Complete();
            return _mapper.Map<RedirectResponse>(newEntity);
        }

        public RedirectResponse? Delete(int id)
        {
            using var scope = _scopeProvider.CreateScope();
            var entity = _redirectRepository.Get(id);
            if (entity is null) return null;

            _redirectRepository.Delete(entity);
            scope.Complete();
            return _mapper.Map<RedirectResponse>(entity);
        }
    }
}
