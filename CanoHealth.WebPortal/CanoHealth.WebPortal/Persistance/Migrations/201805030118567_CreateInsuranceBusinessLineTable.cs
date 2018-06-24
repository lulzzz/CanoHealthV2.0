namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateInsuranceBusinessLineTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InsuranceBusinessLines",
                c => new
                {
                    InsuranceBusinessLineId = c.Guid(nullable: false),
                    InsuranceId = c.Guid(nullable: false),
                    PlanTypeId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.InsuranceBusinessLineId)
                .ForeignKey("dbo.PlanTypes", t => t.PlanTypeId)//, cascadeDelete: true
                .ForeignKey("dbo.Insurances", t => t.InsuranceId)//, cascadeDelete: true
                .Index(t => t.InsuranceId)
                .Index(t => t.PlanTypeId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.InsuranceBusinessLines", "InsuranceId", "dbo.Insurances");
            DropForeignKey("dbo.InsuranceBusinessLines", "PlanTypeId", "dbo.PlanTypes");
            DropIndex("dbo.InsuranceBusinessLines", new[] { "PlanTypeId" });
            DropIndex("dbo.InsuranceBusinessLines", new[] { "InsuranceId" });
            DropTable("dbo.InsuranceBusinessLines");
        }
    }
}
