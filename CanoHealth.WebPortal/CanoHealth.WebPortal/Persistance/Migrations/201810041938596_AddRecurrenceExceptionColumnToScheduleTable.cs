namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRecurrenceExceptionColumnToScheduleTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Schedules", "RecurrenceException", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Schedules", "RecurrenceException");
        }
    }
}
