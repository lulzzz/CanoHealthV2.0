namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OverrideConventionsForTimeZonesOnScheduleTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Schedules", "StartTimezone", c => c.String(maxLength: 250));
            AlterColumn("dbo.Schedules", "EndTimezone", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Schedules", "EndTimezone", c => c.String());
            AlterColumn("dbo.Schedules", "StartTimezone", c => c.String());
        }
    }
}
