namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameEndColumnFromScheduleTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Schedules", "End", c => c.DateTime(nullable: false));
            DropColumn("dbo.Schedules", "EndDateTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Schedules", "EndDateTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.Schedules", "End");
        }
    }
}
