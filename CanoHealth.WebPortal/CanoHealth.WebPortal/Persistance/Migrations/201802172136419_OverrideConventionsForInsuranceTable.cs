namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class OverrideConventionsForInsuranceTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Insurances", "Code", c => c.String(maxLength: 10));
            AlterColumn("dbo.Insurances", "PhoneNumber", c => c.String(maxLength: 20));
            AlterColumn("dbo.Insurances", "Address", c => c.String(maxLength: 255));
        }

        public override void Down()
        {
            AlterColumn("dbo.Insurances", "Address", c => c.String());
            AlterColumn("dbo.Insurances", "PhoneNumber", c => c.String());
            AlterColumn("dbo.Insurances", "Code", c => c.String());
        }
    }
}
