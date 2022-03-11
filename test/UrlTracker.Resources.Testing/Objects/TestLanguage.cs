﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Umbraco.Cms.Core.Models;

namespace UrlTracker.Resources.Testing.Objects
{
    public class TestLanguage : ILanguage
    {
        public string IsoCode { get; set; }
        public string CultureName { get; set; }

        public CultureInfo CultureInfo { get; set; }

        public bool IsDefault { get; set; }
        public bool IsMandatory { get; set; }
        public int? FallbackLanguageId { get; set; }
        public int Id { get; set; }
        public Guid Key { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }

        public bool HasIdentity { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

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
