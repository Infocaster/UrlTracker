using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core.Notifications
{
    internal class DecoratorRedirectRepositoryNotifications
        : IRedirectRepository
    {
        private readonly IRedirectRepository _decoratee;
        private readonly IScopeAccessor _scopeAccessor;

        public DecoratorRedirectRepositoryNotifications(
            IRedirectRepository decoratee,
            IScopeAccessor scopeAccessor)
        {
            _decoratee = decoratee;
            _scopeAccessor = scopeAccessor;
        }

        private IScope Scope => _scopeAccessor.AmbientScope ?? throw new InvalidOperationException("Unable to fetch a scope, because no ambient scope is present.");

        public int Count(IQuery<IRedirect> query)
        {
            return _decoratee.Count(query);
        }

        public void Delete(IRedirect entity)
        {
            _decoratee.Delete(entity);
            Scope.Notifications.Publish(new RedirectDeletedNotification(entity));
        }

        public void DeleteBulk(IRedirect[] redirects)
        {
            _decoratee.DeleteBulk(redirects);
            Scope.Notifications.Publish(new RedirectDeletedNotification(redirects));
        }

        public bool Exists(int id)
        {
            return _decoratee.Exists(id);
        }

        public IRedirect? Get(int id)
        {
            return _decoratee.Get(id);
        }

        public IEnumerable<IRedirect> Get(IQuery<IRedirect> query)
        {
            return _decoratee.Get(query);
        }

        public Task<IReadOnlyCollection<IRedirect>> GetAsync(IEnumerable<string> urlsAndPaths)
        {
            return _decoratee.GetAsync(urlsAndPaths);
        }

        public Task<RedirectEntityCollection> GetAsync(uint skip, uint take, string? query, RedirectFilters filters, bool descending)
        {
            return _decoratee.GetAsync(skip, take, query, filters, descending);
        }

        public IEnumerable<IRedirect> GetMany(params int[]? ids)
        {
            return _decoratee.GetMany(ids);
        }

        public Task<IReadOnlyCollection<IRedirect>> GetWithRegexAsync()
        {
            return _decoratee.GetWithRegexAsync();
        }

        public void Save(IRedirect entity)
        {
            _decoratee.Save(entity);

            // We know that an entity was new by checking if the id is dirty,
            // because the id is always only changed when creating a new entity
            if (entity.WasPropertyDirty("Id")) Scope.Notifications.Publish(new RedirectCreatedNotification(entity));
            else Scope.Notifications.Publish(new RedirectUpdatedNotification(entity));
        }
    }
}
