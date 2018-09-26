using CanoHealth.WebPortal.CommonTools.CurrentDateTime;
using CanoHealth.WebPortal.CommonTools.User;
using CanoHealth.WebPortal.Core.Domain;
using Elmah;
using System;
using System.Collections.Generic;
using System.Reflection;

/*Use this just on those entities where the primary key is named as ClassName + Id or just Id*/

namespace CanoHealth.WebPortal.Services.AuditLogs
{
    public class Logs<TEntity> : ILogs<TEntity> where TEntity : class
    {
        private readonly IUserService _user;
        private readonly ICurrentDateTimeService _date;

        public Logs(IUserService user, ICurrentDateTimeService date)
        {
            _user = user;
            _date = date;
        }

        public IEnumerable<AuditLog> GenerateLogs(IEnumerable<TEntity> entities)
        {
            var auditLogs = new List<AuditLog>();
            try
            {
                foreach (var entity in entities)
                {
                    Type myType = entity.GetType();
                    List<PropertyInfo> properties = new List<PropertyInfo>(myType.GetProperties());

                    var primaryKey = properties.Find(p => p.Name.Equals(myType.Name + "Id", StringComparison.InvariantCultureIgnoreCase));
                    if (primaryKey == null)
                        primaryKey = properties.Find(p => p.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase));

                    foreach (PropertyInfo property in properties)
                    {
                        if (!property.Name.Equals(primaryKey.Name, StringComparison.InvariantCultureIgnoreCase) && 
                            (property.GetType().IsPrimitive || property.PropertyType.Name == "Guid"))
                        {
                            Guid primarykeyValue;
                            auditLogs.Add(new AuditLog
                            {
                                TableName = myType.Name,
                                ColumnName = property.Name,
                                AuditAction = "Insert",
                                ObjectId = Guid.TryParse(primaryKey.GetValue(entity, null).ToString(), out primarykeyValue) ? primarykeyValue : (Guid?)null,
                                NewValue = property.GetValue(entity, null).ToString(),
                                UpdatedBy = _user.GetUserName(),
                                UpdatedOn = _date.GetCurrentDateTime()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return auditLogs;
        }
    }
}