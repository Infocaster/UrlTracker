using System;

namespace UrlTracker.Core.Compatibility
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CompatibilityMapperForAttribute : Attribute
    {
        public Type EntityType { get; private set; }

        public CompatibilityMapperForAttribute(Type entityType)
        {
            EntityType = entityType;
        }
    }
}
