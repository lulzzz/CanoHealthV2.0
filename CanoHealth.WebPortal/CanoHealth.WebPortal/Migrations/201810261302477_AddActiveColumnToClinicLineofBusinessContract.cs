namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveColumnToClinicLineofBusinessContract : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClinicLineofBusinessContracts", "Active", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClinicLineofBusinessContracts", "Active");
        }
    }
}
