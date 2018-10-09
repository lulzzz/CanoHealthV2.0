namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameStartColumnFromScheduleTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Schedules", "Start", c => c.DateTime(nullable: false));
            DropColumn("dbo.Schedules", "StartDateTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Schedules", "StartDateTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.Schedules", "Start");
        }
    }
}
