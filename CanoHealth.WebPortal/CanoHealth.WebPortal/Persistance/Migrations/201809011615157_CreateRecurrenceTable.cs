namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateRecurrenceTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Recurrences",
                c => new
                    {
                        RecurrenceId = c.Guid(nullable: false),
                        RecurrenceName = c.String(maxLength: 50),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RecurrenceId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Recurrences");
        }
    }
}
