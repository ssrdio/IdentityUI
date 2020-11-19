using SSRD.Audit.Data;
using SSRD.CommonUtils.Specifications;
using System;

namespace SSRD.IdentityUI.Core.Data.Specifications
{
    public static class AuditSpecificationExtensions
    {
        public static IBaseSpecificationBuilder<AuditEntity> WithActionType(this IBaseSpecificationBuilder<AuditEntity> baseBuilder, ActionTypes? actionType)
        {
            if (actionType.HasValue)
            {
                baseBuilder.Where(x => x.ActionType == actionType);
            }

            return baseBuilder;
        }

        public static IBaseSpecificationBuilder<AuditEntity> InRange(this IBaseSpecificationBuilder<AuditEntity> baseBuilder, DateTime? from, DateTime? to)
        {
            if (from.HasValue)
            {
                baseBuilder.Where(x => x.Created >= from);
            }

            if (to.HasValue)
            {
                baseBuilder.Where(x => x.Created < to);
            }

            return baseBuilder;
        }

        public static IBaseSpecificationBuilder<AuditEntity> WithUser(this IBaseSpecificationBuilder<AuditEntity> baseBuilder, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return baseBuilder;
            }

            return baseBuilder
                .Where(x => x.SubjectType == SubjectTypes.Human)
                .Where(x => x.SubjectIdentifier == userId);
        }

        public static IBaseSpecificationBuilder<AuditEntity> SearchByObjectType(this IBaseSpecificationBuilder<AuditEntity> baseBuilder, string search)
        {
            if(string.IsNullOrEmpty(search))
            {
                return baseBuilder;
            }

            string upperSearch = search.ToUpper();

            return baseBuilder.Where(x => x.ObjectType.ToUpper().Contains(upperSearch));
        }

        public static IBaseSpecificationBuilder<AuditEntity> WithObjectType(this IBaseSpecificationBuilder<AuditEntity> baseBuilder, string objectType)
        {
            if(string.IsNullOrEmpty(objectType))
            {
                return baseBuilder;
            }

            return baseBuilder
                .Where(x => x.ObjectType == objectType);
        }

        public static IBaseSpecificationBuilder<AuditEntity> SearchByObjectIdentifier(this IBaseSpecificationBuilder<AuditEntity> baseBuilder, string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return baseBuilder;
            }

            string upperSearch = search.ToUpper();

            return baseBuilder.Where(x => x.ObjectIdentifier.ToUpper().Contains(upperSearch));
        }

        public static IBaseSpecificationBuilder<AuditEntity> WithObjectIdentifier(this IBaseSpecificationBuilder<AuditEntity> baseBuilder, string objectIdentifier)
        {
            if (string.IsNullOrEmpty(objectIdentifier))
            {
                return baseBuilder;
            }

            return baseBuilder
                .Where(x => x.ObjectIdentifier == objectIdentifier);
        }

        public static IBaseSpecificationBuilder<AuditEntity> WithSubjectType(this IBaseSpecificationBuilder<AuditEntity> baseBuilder, SubjectTypes? subjectType)
        {
            if(!subjectType.HasValue)
            {
                return baseBuilder;
            }

            return baseBuilder
                .Where(x => x.SubjectType == subjectType.Value);
        }

        public static IBaseSpecificationBuilder<AuditEntity> WithSubjectIdentifier(this IBaseSpecificationBuilder<AuditEntity> baseBuilder, string subjectIdentifier)
        {
            if(string.IsNullOrEmpty(subjectIdentifier))
            {
                return baseBuilder;
            }

            return baseBuilder
                .Where(x => x.SubjectIdentifier == subjectIdentifier);
        }

        public static IBaseSpecificationBuilder<AuditEntity> SearchBySubjectIdentifier(this IBaseSpecificationBuilder<AuditEntity> baseBuilder, string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return baseBuilder;
            }

            string upperSearch = search.ToUpper();

            return baseBuilder
                .Where(x => x.SubjectIdentifier.ToUpper().Contains(upperSearch));
        }

        public static IBaseSpecificationBuilder<AuditEntity> WithResourceName(this IBaseSpecificationBuilder<AuditEntity> baseBuilder, string resourceName)
        {
            if(string.IsNullOrEmpty(resourceName))
            {
                return baseBuilder;
            }

            return baseBuilder
                .Where(x => x.ResourceName == resourceName);
        }

        public static IBaseSpecificationBuilder<AuditEntity> SearchByResourceName(this IBaseSpecificationBuilder<AuditEntity> baseBuilder, string search)
        {
            if(string.IsNullOrEmpty(search))
            {
                return baseBuilder;
            }

            string upperSearch = search.ToUpper();

            return baseBuilder
                .Where(x => x.ResourceName.ToUpper().Contains(upperSearch));
        }

        public static IBaseSpecificationBuilder<AuditEntity> WithGroupIdentifier(this IBaseSpecificationBuilder<AuditEntity> baseBuilder, string groupId)
        {
            return baseBuilder
                .Where(x => x.GroupIdentifier == groupId);
        }
    }
}
