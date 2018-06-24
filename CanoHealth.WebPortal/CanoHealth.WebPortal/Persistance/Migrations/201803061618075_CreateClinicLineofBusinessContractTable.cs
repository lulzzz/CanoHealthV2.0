namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateClinicLineofBusinessContractTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClinicLineofBusinessContracts",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    PlaceOfServiceId = c.Guid(nullable: false),
                    ContractLineofBusinessId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PlaceOfServices", t => t.PlaceOfServiceId)//, cascadeDelete: true
                .ForeignKey("dbo.ContractLineofBusinesses", t => t.ContractLineofBusinessId)//, cascadeDelete: true
                .Index(t => t.PlaceOfServiceId)
                .Index(t => t.ContractLineofBusinessId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.ClinicLineofBusinessContracts", "ContractLineofBusinessId", "dbo.ContractLineofBusinesses");
            DropForeignKey("dbo.ClinicLineofBusinessContracts", "PlaceOfServiceId", "dbo.PlaceOfServices");
            DropIndex("dbo.ClinicLineofBusinessContracts", new[] { "ContractLineofBusinessId" });
            DropIndex("dbo.ClinicLineofBusinessContracts", new[] { "PlaceOfServiceId" });
            DropTable("dbo.ClinicLineofBusinessContracts");
        }
    }
}
