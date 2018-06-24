namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUniqueConstraintProviderNumberDoctorIndividualProviderTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[DoctorIndividualProviders] ADD CONSTRAINT UK_DoctorIndividualProviders_ProviderNumber UNIQUE([ProviderNumber])");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[DoctorIndividualProviders] DROP CONSTRAINT UK_DoctorIndividualProviders_ProviderNumber");
        }
    }
}
