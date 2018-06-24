using CanoHealth.WebPortal.CommonTools.CurrentDateTime;
using CanoHealth.WebPortal.CommonTools.User;
using System;
using System.ComponentModel.DataAnnotations;

namespace CanoHealth.WebPortal.Core.Domain
{
    public class AuditLog
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string TableName { get; set; }

        [StringLength(100)]
        public string ColumnName { get; set; }

        [StringLength(255)]
        public string OldValue { get; set; }

        [StringLength(255)]
        public string NewValue { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public Guid? ObjectId { get; set; }

        [StringLength(100)]
        public string AuditAction { get; set; }

        public static AuditLog AddLog(string tableName, string columnName, string oldValue, string newValue,
            Guid? objectId, string auditAction)
        {
            IUserService user = new UserService();
            ICurrentDateTimeService currentDateTime = new CurrentDateTimeService();
            return new AuditLog
            {
                TableName = tableName,
                ColumnName = columnName,
                OldValue = oldValue,
                NewValue = newValue,
                UpdatedBy = user.GetUserName(),
                UpdatedOn = currentDateTime.GetCurrentDateTime(),
                ObjectId = objectId,
                AuditAction = auditAction
            };
        }
    }
}