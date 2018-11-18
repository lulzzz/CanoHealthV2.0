namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveColumnToDoctorScheduleTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorSchedules", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DoctorSchedules", "Active");
        }
    }
}
