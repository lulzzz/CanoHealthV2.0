namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateCorporationTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Corporations",
                c => new
                {
                    CorporationId = c.Guid(nullable: false),
                    CorporationName = c.String(),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.CorporationId);

        }

        public override void Down()
        {
            DropTable("dbo.Corporations");
        }
    }
}
