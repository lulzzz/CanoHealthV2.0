namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DropUniqueConstraintDoctorIdMedicalLicenseTypeIdInMedicalLicensesTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[MedicalLicenses] DROP CONSTRAINT UK_MedicalLicenses_DoctorId_MedicalLicenseTypeId");
        }
        
        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[MedicalLicenses] ADD CONSTRAINT UK_MedicalLicenses_DoctorId_MedicalLicenseTypeId UNIQUE([DoctorId],[MedicalLicenseTypeId])");
        }
    }
}
