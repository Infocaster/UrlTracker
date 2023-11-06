using System.Collections.Generic;
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
        void DeleteBulk(int[] ids);
        RedirectResponse? GetById(int id);
        IEnumerable<IRedirect> Get(int[] ids);
        Task<RedirectCollectionResponse> GetAsync(ListRedirectRequest request);
        RedirectResponse? Update(int id, RedirectRequest request);
        IEnumerable<RedirectResponse?> UpdateBulk(IEnumerable<RedirectBulkRequest> bulkRequest);
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

        public IEnumerable<IRedirect> Get(int[] ids)
        {
            using var scope = _scopeProvider.CreateScope();

           return _redirectRepository.GetMany();
        }

        public RedirectResponse? GetById(int id)
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

            var newEntity = HandleUpdate(id, request);

            scope.Complete();
            return _mapper.Map<RedirectResponse>(newEntity);
        }

        public IEnumerable<RedirectResponse?> UpdateBulk(IEnumerable<RedirectBulkRequest> bulkRequest)
        {
            using var scope = _scopeProvider.CreateScope();
            var response = new List<RedirectResponse?>();

            foreach (var request in bulkRequest)
            {
                response.Add(_mapper.Map<RedirectResponse>(HandleUpdate(request.Id, request.Redirect)));
            }

            scope.Complete();
            return response;
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

        public void DeleteBulk(int[] ids)
        {
            using var scope = _scopeProvider.CreateScope();
            var response = new List<RedirectResponse?>();

            _redirectRepository.DeleteBulk(ids);

            scope.Complete();
        }

        private IRedirect? HandleUpdate(int id, RedirectRequest request)
        {
            var entity = _redirectRepository.Get(id);
            if (entity is null) return null;

            var newEntity = _mapper.Map<IRedirect>(request)!;
            newEntity.Id = entity.Id;
            if (newEntity.Key == default) newEntity.Key = entity.Key;
            newEntity.CreateDate = entity.CreateDate;

            _redirectRepository.Save(newEntity);
            return newEntity;
        }
    }
}
