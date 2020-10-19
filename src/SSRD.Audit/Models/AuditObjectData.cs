using Microsoft.EntityFrameworkCore.ChangeTracking;
using SSRD.Audit.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Models
{
    public class AuditObjectData
    {
        public ActionTypes ActionType { get; set; }

        public string ObjectType { get; set; }
        public string ObjectIdentifier { get; set; }
        public string ObjectMetadata { get; set; }

        public PropertyEntry ObjectIdentifierProperty { get; set; }

        public AuditObjectData(ActionTypes actionType, string objectType, string objectIdentifier, string objectMetadata)
        {
            ActionType = actionType;
            ObjectType = objectType;
            ObjectIdentifier = objectIdentifier;
            ObjectMetadata = objectMetadata;
        }

        public AuditObjectData(ActionTypes actionType, string objectType, string objectMetadata, PropertyEntry objectIdentifierProperty)
        {
            ActionType = actionType;
            ObjectType = objectType;
            ObjectMetadata = objectMetadata;
            ObjectIdentifierProperty = objectIdentifierProperty;
        }
    }
}
