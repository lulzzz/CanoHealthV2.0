namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreatePlanTypeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlanTypes",
                c => new
                {
                    PlanTypeId = c.Guid(nullable: false),
                    Code = c.String(maxLength: 100),
                    Name = c.String(nullable: false, maxLength: 100),
                })
                .PrimaryKey(t => t.PlanTypeId);

        }

        public override void Down()
        {
            DropTable("dbo.PlanTypes");
        }
    }
}
