using System;
using Umbraco.Core.Models.Entities;

namespace UrlTracker.Core.Compatibility
{
    internal static class CompatibilityEntityExtensions
    {
        /// <summary>
        /// Updates the entity when it is being saved.
        /// </summary>
        internal static void UpdatingEntity(this IEntity entity)
        {
            var now = DateTime.Now;

            // just in case
            if (entity.CreateDate == default)
            {
                entity.CreateDate = now;
            }

            // set the update date if not already set
            if (entity.UpdateDate == default || entity is ICanBeDirty canBeDirty && canBeDirty.IsPropertyDirty("UpdateDate") == false)
            {
                entity.UpdateDate = now;
            }
        }

        /// <summary>
        /// Updates the entity when it is being saved for the first time.
        /// </summary>
        internal static void AddingEntity(this IEntity entity)
        {
            var now = DateTime.Now;
            var canBeDirty = entity as ICanBeDirty;

            // set the create and update dates, if not already set
            if (entity.CreateDate == default || canBeDirty?.IsPropertyDirty("CreateDate") == false)
            {
                entity.CreateDate = now;
            }
            if (entity.UpdateDate == default || canBeDirty?.IsPropertyDirty("UpdateDate") == false)
            {
                entity.UpdateDate = now;
            }
        }
    }
}
