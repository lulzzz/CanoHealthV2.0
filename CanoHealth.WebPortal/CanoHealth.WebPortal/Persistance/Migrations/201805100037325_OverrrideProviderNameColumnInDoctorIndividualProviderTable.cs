namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OverrrideProviderNameColumnInDoctorIndividualProviderTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DoctorIndividualProviders", "ProviderNumber", c => c.String(nullable: false, maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DoctorIndividualProviders", "ProviderNumber", c => c.String(nullable: false));
        }
    }
}
