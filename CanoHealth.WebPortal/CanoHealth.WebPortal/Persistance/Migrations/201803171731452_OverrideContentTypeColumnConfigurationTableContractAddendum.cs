namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class OverrideContentTypeColumnConfigurationTableContractAddendum : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ContractAddendums", "ContentType", c => c.String(maxLength: 255));
        }

        public override void Down()
        {
            AlterColumn("dbo.ContractAddendums", "ContentType", c => c.String(maxLength: 20));
        }
    }
}
