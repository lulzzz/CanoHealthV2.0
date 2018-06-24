namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateContractAddendumTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContractAddendums",
                c => new
                {
                    ContractAddendumId = c.Guid(nullable: false),
                    ContractId = c.Guid(nullable: false),
                    UniqueFileName = c.String(maxLength: 100),
                    OriginalFileName = c.String(maxLength: 100),
                    FileExtension = c.String(maxLength: 10),
                    FileSize = c.String(maxLength: 10),
                    ContentType = c.String(maxLength: 20),
                    ServerLocation = c.String(maxLength: 100),
                    UploadDateTime = c.DateTime(nullable: false),
                    UploadBy = c.String(),
                    Description = c.String(maxLength: 3000),
                    Active = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.ContractAddendumId)
                .ForeignKey("dbo.Contracts", t => t.ContractId)//, cascadeDelete: true
                .Index(t => t.ContractId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.ContractAddendums", "ContractId", "dbo.Contracts");
            DropIndex("dbo.ContractAddendums", new[] { "ContractId" });
            DropTable("dbo.ContractAddendums");
        }
    }
}
