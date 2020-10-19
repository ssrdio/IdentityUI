using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using SSRD.Audit.Attributes;
using SSRD.Audit.Models;
using SSRD.Audit.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SSRD.Audit.Data
{
    public static class ChangeTrackerAuditService
    {
        public static ProccessChangeTrackerResult ProccessChangeTracker(ChangeTracker changeTracker, AuditOptions auditOptions = null)
        {
            if(auditOptions == null)
            {
                auditOptions = new AuditOptions();
            }

            Task<ProccessChangeTrackerResult> task = ProccessChangeTrackerAsync(changeTracker, auditOptions);

            task.Wait();

            return task.Result;
        }

        public static async Task<ProccessChangeTrackerResult> ProccessChangeTrackerAsync(ChangeTracker changeTracker, AuditOptions auditOptions = null, CancellationToken cancellationToken = default)
        {
            if (auditOptions == null)
            {
                auditOptions = new AuditOptions();
            }

            List<AuditObjectData> dbAuditDataList = new List<AuditObjectData>();
            bool requiresCustomBatch = false;

            foreach (EntityEntry entry in changeTracker.Entries())
            {
                if(entry.Entity.GetType().GetCustomAttributes(typeof(AuditIgnoreAttribute), true).Any())
                {
                    continue;
                }

                switch (entry.State)
                {
                    case Microsoft.EntityFrameworkCore.EntityState.Added:
                        {
                            AuditObjectData auditData;

                            if (entry.IsKeySet)
                            {
                                auditData = new AuditObjectData(
                                    actionType: ActionTypes.Add,
                                    objectType: entry.Entity.GetType().Name,
                                    objectIdentifier: entry.GetPrimaryKey(),
                                    objectMetadata: entry.GetMetadata());
                            }
                            else
                            {
                                PropertyEntry objectIdentifierProperty = entry.Metadata.FindPrimaryKey().Properties
                                    .Select(x => entry.Property(x.Name))
                                    .FirstOrDefault();

                                auditData = new AuditObjectData(
                                    actionType: ActionTypes.Add,
                                    objectType: entry.Entity.GetType().Name,
                                    objectIdentifierProperty: objectIdentifierProperty,
                                    objectMetadata: entry.GetMetadata());

                                requiresCustomBatch = true;
                            }

                            dbAuditDataList.Add(auditData);

                            break;
                        }
                    case Microsoft.EntityFrameworkCore.EntityState.Modified:
                        {
                            AuditObjectData auditData = new AuditObjectData(
                                    actionType: ActionTypes.Update,
                                    objectType: entry.Entity.GetType().Name,
                                    objectIdentifier: entry.GetPrimaryKey(),
                                    objectMetadata: entry.GetMetadata(onlyModified: true));

                            dbAuditDataList.Add(auditData);

                            break;
                        }
                    case Microsoft.EntityFrameworkCore.EntityState.Deleted:
                        {
                            if (auditOptions.AuditCascadeDelete)
                            {
                                IEnumerable<AuditObjectData> cascadeDeleteAuditData = await CascadeDelete(entry, changeTracker, cancellationToken);
                                dbAuditDataList.AddRange(cascadeDeleteAuditData);
                            }

                            AuditObjectData auditData = new AuditObjectData(
                                    actionType: ActionTypes.Delete,
                                    objectType: entry.Entity.GetType().Name,
                                    objectIdentifier: entry.GetPrimaryKey(),
                                    objectMetadata: entry.GetMetadata());

                            dbAuditDataList.Add(auditData);

                            break;
                        }
                }
            }

            return new ProccessChangeTrackerResult(
                requiresCustomBatch: requiresCustomBatch,
                auditObjectData: dbAuditDataList);
        }

        private static string GetMetadata(this EntityEntry entry, bool onlyModified = false)
        {
            Dictionary<string, object> metadataDictionary = new Dictionary<string, object>();

            PropertyInfo[] propertyInfos = entry.Entity.GetType().GetProperties();

            foreach (PropertyEntry property in entry.Properties)
            {
                PropertyInfo propertyInfo = propertyInfos.Where(x => x.Name == property.Metadata.Name).SingleOrDefault();
                if (propertyInfo != null && Attribute.IsDefined(propertyInfo, typeof(AuditIgnoreAttribute)))
                {
                    continue;
                }

                if(!property.IsModified && onlyModified)
                {
                    continue;
                }

                metadataDictionary.Add(property.Metadata.Name, property.CurrentValue);
            }

            return JsonConvert.SerializeObject(metadataDictionary);
        }

        private static string GetPrimaryKey(this EntityEntry entry)
        {
            var primaryKeys = entry.Metadata.FindPrimaryKey().Properties
                .Select(x => new { x.Name, entry.Property(x.Name).CurrentValue });

            int keys = primaryKeys.Count();

            if(keys == 0)
            {
                return null;
            }
            else if(keys == 1)
            {
                return primaryKeys.First().CurrentValue.ToString();
            }

            Dictionary<string, object> primaryKeysDictionary = new Dictionary<string, object>();
            foreach (var key in primaryKeys)
            {
                primaryKeysDictionary.Add(key.Name, key.CurrentValue);
            }

            return JsonConvert.SerializeObject(primaryKeysDictionary);
        }

        private static async Task<IEnumerable<AuditObjectData>> CascadeDelete(EntityEntry entityEntry, ChangeTracker changeTracker, CancellationToken cancellationToken)
        {
            if (entityEntry.Entity.GetType().GetCustomAttributes(typeof(AuditIgnoreCascadeAttribute), true).Any())
            {
                return new List<AuditObjectData>();
            }

            List<AuditObjectData> auditObjects = new List<AuditObjectData>();

            foreach (NavigationEntry navigation in entityEntry.Navigations)
            {
                if (navigation.Metadata.ForeignKey.PrincipalEntityType.ClrType == entityEntry.Entity.GetType() 
                    && navigation.Metadata.ForeignKey.DeleteBehavior == Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade)
                {
                    if(navigation.Metadata.PropertyInfo.GetCustomAttributes(typeof(AuditIgnoreCascadeAttribute)).Any())
                    {
                        continue;
                    }

                    if (!navigation.IsLoaded)
                    {
                        await navigation.LoadAsync();
                    }

                    Type type;
                    if(navigation.Metadata.ClrType.IsGenericType)
                    {
                        if (navigation.Metadata.ClrType.GetGenericArguments().Length != 1)
                        {
                            //TODO: log unsupported
                            Trace.TraceInformation($"Unsupported navigation type for cascade delete. {navigation.Metadata.Name}");
                            continue;
                        }

                        type = navigation.Metadata.ClrType.GetGenericArguments()[0];
                    }
                    else
                    {
                        type = navigation.Metadata.ClrType;
                    }

                    IEnumerable<EntityEntry> navigationEntries = changeTracker.Entries()
                        .Where(x => x.Entity.GetType() == type);

                    foreach (EntityEntry navigationEntry in navigationEntries)
                    {
                        IEnumerable<AuditObjectData> childrenAuditObjects = await CascadeDelete(navigationEntry, changeTracker, cancellationToken);
                        auditObjects.AddRange(childrenAuditObjects);

                        AuditObjectData auditObjectData = new AuditObjectData(
                                actionType: ActionTypes.Delete,
                                objectType: type.Name,
                                objectIdentifier: navigationEntry.GetPrimaryKey(),
                                objectMetadata: navigationEntry.GetMetadata());

                        auditObjects.Add(auditObjectData);
                    }
                }
            }

            return auditObjects;
        }
    }
}
