namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddIndividualProviderEffectiveDateColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorIndividualProviders", "IndividualProviderEffectiveDate", c => c.DateTime(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.DoctorIndividualProviders", "IndividualProviderEffectiveDate");
        }
    }
}
