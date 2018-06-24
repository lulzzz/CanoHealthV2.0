namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUniqueConstraintForClassificationColumnInMedicalLicenseTypeTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[MedicalLicenseTypes] ADD CONSTRAINT UK_Classification UNIQUE([Classification])");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[MedicalLicenseTypes] DROP CONSTRAINT UK_Classification");
        }
    }
}
