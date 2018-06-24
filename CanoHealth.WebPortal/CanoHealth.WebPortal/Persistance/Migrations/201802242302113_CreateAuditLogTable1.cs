namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateAuditLogTable1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditLogs",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    TableName = c.String(maxLength: 100),
                    ColumnName = c.String(maxLength: 100),
                    OldValue = c.String(maxLength: 255),
                    NewValue = c.String(maxLength: 255),
                    UpdatedBy = c.String(),
                    UpdatedOn = c.DateTime(),
                    ObjectId = c.Guid(),
                    AuditAction = c.String(maxLength: 100),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.AuditLogs");
        }
    }
}
