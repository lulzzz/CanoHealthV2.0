namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRecurrenceRuleColumnToScheduleTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Schedules", "RecurrenceRule", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Schedules", "RecurrenceRule");
        }
    }
}
