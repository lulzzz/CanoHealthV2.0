namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateInsuranceTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Insurances",
                c => new
                {
                    InsuranceId = c.Guid(nullable: false),
                    Code = c.String(),
                    Name = c.String(nullable: false, maxLength: 100),
                    PhoneNumber = c.String(),
                    Address = c.String(),
                    Active = c.Boolean(),
                })
                .PrimaryKey(t => t.InsuranceId);

        }

        public override void Down()
        {
            DropTable("dbo.Insurances");
        }
    }
}
