namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class OverridingConventionsForClassificationColumnInMedicalLicenseTypeTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MedicalLicenseTypes", "Classification", c => c.String(nullable: false, maxLength: 100));
        }

        public override void Down()
        {
            AlterColumn("dbo.MedicalLicenseTypes", "Classification", c => c.String());
        }
    }
}
