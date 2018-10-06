namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateRecurrenceIDColumnToScheduleTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Schedules", "RecurrenceID", c => c.Guid());
            CreateIndex("dbo.Schedules", "RecurrenceID");
            AddForeignKey("dbo.Schedules", "RecurrenceID", "dbo.Schedules", "ScheduleId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Schedules", "RecurrenceID", "dbo.Schedules");
            DropIndex("dbo.Schedules", new[] { "RecurrenceID" });
            DropColumn("dbo.Schedules", "RecurrenceID");
        }
    }
}
