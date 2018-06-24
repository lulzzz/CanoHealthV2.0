namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateCorporationInsuranceContractRelation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contracts",
                c => new
                {
                    ContractId = c.Guid(nullable: false),
                    GroupNumber = c.String(nullable: false, maxLength: 20),
                    CorporationId = c.Guid(nullable: false),
                    InsuranceId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ContractId)
                .ForeignKey("dbo.Corporations", t => t.CorporationId)//, cascadeDelete: true
                .ForeignKey("dbo.Insurances", t => t.InsuranceId)//, cascadeDelete: true
                .Index(t => t.CorporationId)
                .Index(t => t.InsuranceId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Contracts", "InsuranceId", "dbo.Insurances");
            DropForeignKey("dbo.Contracts", "CorporationId", "dbo.Corporations");
            DropIndex("dbo.Contracts", new[] { "InsuranceId" });
            DropIndex("dbo.Contracts", new[] { "CorporationId" });
            DropTable("dbo.Contracts");
        }
    }
}
