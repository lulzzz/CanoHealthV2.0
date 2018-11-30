namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DropUniqueConstraintLicenseNumberInMedicalLicensesTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[MedicalLicenses] DROP CONSTRAINT UK_MedicalLicenses_LicenseNumber");
        }
        
        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[MedicalLicenses] ADD CONSTRAINT UK_MedicalLicenses_LicenseNumber UNIQUE([LicenseNumber])");
        }
    }
}
