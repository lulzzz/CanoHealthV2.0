namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUniqueConstraintInsuranceBusinessLine : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[InsuranceBusinessLines] ADD CONSTRAINT UK_InsuranceBusinessLines_InsuranceId_PlanTypeId UNIQUE([InsuranceId],[PlanTypeId])");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[InsuranceBusinessLines] DROP CONSTRAINT UK_InsuranceBusinessLines_InsuranceId_PlanTypeId");
        }
    }
}
