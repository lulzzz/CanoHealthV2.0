namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OverridingActiveColumnInDoctorTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Doctors", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Doctors", "Active", c => c.Boolean());
        }
    }
}
