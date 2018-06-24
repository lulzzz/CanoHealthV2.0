namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEffectiveDateToDoctorCorporationContractLinkTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorCorporationContractLinks", "EffectiveDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DoctorCorporationContractLinks", "EffectiveDate");
        }
    }
}
