namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddActiveColumnToDoctorIndividualProvidersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorIndividualProviders", "Active", c => c.Boolean());
            Sql("ALTER TABLE dbo.DoctorIndividualProviders ADD CONSTRAINT DoctorIndividualProviders_Active DEFAULT 1 FOR Active;");
        }

        public override void Down()
        {
            DropColumn("dbo.DoctorIndividualProviders", "Active");
            Sql("ALTER TABLE dbo.DoctorIndividualProviders DROP CONSTRAINT DoctorIndividualProviders_Active;");
        }
    }
}
