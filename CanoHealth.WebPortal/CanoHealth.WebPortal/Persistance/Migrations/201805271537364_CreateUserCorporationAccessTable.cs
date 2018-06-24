namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUserCorporationAccessTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserCorporationAccesses",
                c => new
                {
                    AccessId = c.Guid(nullable: false),
                    UserId = c.String(maxLength: 128),
                    CorporationId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.AccessId)
                .ForeignKey("dbo.Corporations", t => t.CorporationId)//, cascadeDelete: true
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CorporationId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.UserCorporationAccesses", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserCorporationAccesses", "CorporationId", "dbo.Corporations");
            DropIndex("dbo.UserCorporationAccesses", new[] { "CorporationId" });
            DropIndex("dbo.UserCorporationAccesses", new[] { "UserId" });
            DropTable("dbo.UserCorporationAccesses");
        }
    }
}
