using System;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace UrlTracker.Resources.Testing.Objects
{
    public class TestUmbracoDomain : IDomain
    {
        public TestUmbracoDomain(bool isWildcard)
        {
            IsWildcard = isWildcard;
        }

        public int? LanguageId { get; set; }
        public string DomainName { get; set; }
        public int? RootContentId { get; set; }

        public bool IsWildcard { get; }

        public string LanguageIsoCode { get; }

        public int Id { get; set; }
        public Guid Key { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime CreateDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime UpdateDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime? DeleteDate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool HasIdentity => throw new NotImplementedException();

        public object DeepClone()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetDirtyProperties()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetWereDirtyProperties()
        {
            throw new NotImplementedException();
        }

        public bool IsDirty()
        {
            throw new NotImplementedException();
        }

        public bool IsPropertyDirty(string propName)
        {
            throw new NotImplementedException();
        }

        public void ResetDirtyProperties(bool rememberDirty)
        {
            throw new NotImplementedException();
        }

        public void ResetDirtyProperties()
        {
            throw new NotImplementedException();
        }

        public void ResetWereDirtyProperties()
        {
            throw new NotImplementedException();
        }

        public bool WasDirty()
        {
            throw new NotImplementedException();
        }

        public bool WasPropertyDirty(string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
