namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddDoctorCorporationContractLinkTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorCorporationContractLinks",
                c => new
                {
                    DoctorCorporationContractLinkId = c.Guid(nullable: false),
                    ContractLineofBusinessId = c.Guid(nullable: false),
                    DoctorId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.DoctorCorporationContractLinkId)
                .ForeignKey("dbo.ContractLineofBusinesses", t => t.ContractLineofBusinessId)//, cascadeDelete: true
                .ForeignKey("dbo.Doctors", t => t.DoctorId)//, cascadeDelete: true
                .Index(t => t.ContractLineofBusinessId)
                .Index(t => t.DoctorId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.DoctorCorporationContractLinks", "DoctorId", "dbo.Doctors");
            DropForeignKey("dbo.DoctorCorporationContractLinks", "ContractLineofBusinessId", "dbo.ContractLineofBusinesses");
            DropIndex("dbo.DoctorCorporationContractLinks", new[] { "DoctorId" });
            DropIndex("dbo.DoctorCorporationContractLinks", new[] { "ContractLineofBusinessId" });
            DropTable("dbo.DoctorCorporationContractLinks");
        }
    }
}
