namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUniqueConstraintInProvidersByLocationTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[ProviderByLocations] ADD CONSTRAINT UK_ProvidersByLocation_DoctorCorporationContractLinkId_PlaceOfServiceId UNIQUE([DoctorCorporationContractLinkId],[PlaceOfServiceId])");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[ProviderByLocations] DROP CONSTRAINT UK_ProvidersByLocation_DoctorCorporationContractLinkId_PlaceOfServiceId");
        }
    }
}
