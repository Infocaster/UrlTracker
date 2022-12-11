using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Persistence;
using Umbraco.Cms.Infrastructure.Scoping;
using UrlTracker.Core.Database;
using UrlTracker.Core.Database.Entities;

namespace UrlTracker.Core
{
    public interface IRedactionScoreService
    {
        IRedactionScore Create(Guid key, decimal score, string? name = null);
        IRedactionScore? Get(Guid key);
        IEnumerable<IRedactionScore> GetAll();
        void Save(IRedactionScore score);
    }

    internal class RedactionScoreService : IRedactionScoreService
    {
        private readonly IRedactionScoreRepository _redactionScoreRepository;
        private readonly IScopeProvider _scopeProvider;

        public RedactionScoreService(IRedactionScoreRepository redactionScoreRepository, IScopeProvider scopeProvider)
        {
            _redactionScoreRepository = redactionScoreRepository;
            _scopeProvider = scopeProvider;
        }

        public IRedactionScore? Get(Guid key)
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);

            return _redactionScoreRepository.Get(key);
        }

        public void Save(IRedactionScore score)
        {
            using var scope = _scopeProvider.CreateScope();

            _redactionScoreRepository.Save(score);

            scope.Complete();
        }

        public IRedactionScore Create(Guid key, decimal score, string? name = null)
        {
            return new RedactionScoreEntity()
            {
                Key = key,
                RedactionScore = score,
                Name = name
            };
        }

        public IEnumerable<IRedactionScore> GetAll()
        {
            using var scope = _scopeProvider.CreateScope(autoComplete: true);

            return (_redactionScoreRepository as IReadRepository<int, IRedactionScore>).GetMany();
        }
    }
}
