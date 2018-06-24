namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUniqueConstraintDoctorIdInsuranceIdDoctorIndividualProviderTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[DoctorIndividualProviders] ADD CONSTRAINT UK_DoctorIndividualProviders_DoctorId_InsuranceId UNIQUE([DoctorId],[InsuranceId])");

        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[DoctorIndividualProviders] DROP CONSTRAINT UK_DoctorIndividualProviders_DoctorId_InsuranceId");

        }
    }
}
