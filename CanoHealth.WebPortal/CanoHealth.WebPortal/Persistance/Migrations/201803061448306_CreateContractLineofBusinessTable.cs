namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateContractLineofBusinessTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContractLineofBusinesses",
                c => new
                {
                    ContractLineofBusinessId = c.Guid(nullable: false),
                    ContractId = c.Guid(nullable: false),
                    PlanTypeId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ContractLineofBusinessId)
                .ForeignKey("dbo.Contracts", t => t.ContractId)//, cascadeDelete: true
                .ForeignKey("dbo.PlanTypes", t => t.PlanTypeId)//, cascadeDelete: true
                .Index(t => t.ContractId)
                .Index(t => t.PlanTypeId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.ContractLineofBusinesses", "PlanTypeId", "dbo.PlanTypes");
            DropForeignKey("dbo.ContractLineofBusinesses", "ContractId", "dbo.Contracts");
            DropIndex("dbo.ContractLineofBusinesses", new[] { "PlanTypeId" });
            DropIndex("dbo.ContractLineofBusinesses", new[] { "ContractId" });
            DropTable("dbo.ContractLineofBusinesses");
        }
    }
}
