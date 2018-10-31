namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveColumnToDoctorCorporationContractLink : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorCorporationContractLinks", "Active", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DoctorCorporationContractLinks", "Active");
        }
    }
}
