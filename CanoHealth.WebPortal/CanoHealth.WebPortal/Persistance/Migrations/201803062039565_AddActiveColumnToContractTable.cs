namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddActiveColumnToContractTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contracts", "Active", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Contracts", "Active");
        }
    }
}
