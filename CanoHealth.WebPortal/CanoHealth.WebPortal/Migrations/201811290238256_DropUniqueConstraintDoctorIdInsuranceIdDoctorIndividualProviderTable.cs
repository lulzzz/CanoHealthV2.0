namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DropUniqueConstraintDoctorIdInsuranceIdDoctorIndividualProviderTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[DoctorIndividualProviders] DROP CONSTRAINT UK_DoctorIndividualProviders_DoctorId_InsuranceId");           
        }
        
        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[DoctorIndividualProviders] ADD CONSTRAINT UK_DoctorIndividualProviders_DoctorId_InsuranceId UNIQUE([DoctorId],[InsuranceId])");
        }
    }
}
