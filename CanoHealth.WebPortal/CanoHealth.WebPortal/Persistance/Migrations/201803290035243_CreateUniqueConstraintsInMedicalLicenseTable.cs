namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUniqueConstraintsInMedicalLicenseTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[MedicalLicenses] ADD CONSTRAINT UK_MedicalLicenses_DoctorId_MedicalLicenseTypeId UNIQUE([DoctorId],[MedicalLicenseTypeId])");

            Sql("ALTER TABLE [dbo].[MedicalLicenses] ADD CONSTRAINT UK_MedicalLicenses_LicenseNumber UNIQUE([LicenseNumber])");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[Doctors] DROP CONSTRAINT UK_MedicalLicenses_DoctorId_MedicalLicenseTypeId");

            Sql("ALTER TABLE [dbo].[Doctors] DROP CONSTRAINT UK_MedicalLicenses_LicenseNumber");
        }
    }
}
