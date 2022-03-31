using System;
using System.Collections.Generic;
using System.ComponentModel;
using Umbraco.Cms.Core.Models;

namespace UrlTracker.Resources.Testing.Objects
{
    public class TestUmbracoDomain : IDomain
    {
        public TestUmbracoDomain(bool isWildcard)
        {
            IsWildcard = isWildcard;
        }

        public int? LanguageId { get; set; }
        public string? DomainName { get; set; }
        public int? RootContentId { get; set; }

        public bool IsWildcard { get; set; }

        public string? LanguageIsoCode { get; set; }

        public int Id { get; set; }
        public Guid Key { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }

        public bool HasIdentity { get; set; }

        // Cleanly suppress warning for unused event handler. See: https://stackoverflow.com/a/1093351/2853950
        public event PropertyChangedEventHandler? PropertyChanged
        {
            add { throw new NotSupportedException(); }
            remove { }
        }

        public object DeepClone()
        {
            throw new NotImplementedException();
        }

        public void DisableChangeTracking()
        {
            throw new NotImplementedException();
        }

        public void EnableChangeTracking()
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

        public void ResetIdentity()
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
