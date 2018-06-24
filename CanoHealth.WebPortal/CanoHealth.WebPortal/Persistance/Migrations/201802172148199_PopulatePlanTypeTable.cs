namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class PopulatePlanTypeTable : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO PlanTypes (PlanTypeId, Code, Name) VALUES (NEWID(), 'POS', 'Point of Service')");
            Sql("INSERT INTO PlanTypes (PlanTypeId, Code, Name) VALUES (NEWID(), 'HMO', 'Health Maintenance Organization')");
            Sql("INSERT INTO PlanTypes (PlanTypeId, Code, Name) VALUES (NEWID(), 'EPO', 'Exclusive Provider Organization')");
            Sql("INSERT INTO PlanTypes (PlanTypeId, Code, Name) VALUES (NEWID(), 'PPO', 'Preferred Provider Organization')");
            Sql("INSERT INTO PlanTypes (PlanTypeId, Code, Name) VALUES (NEWID(), 'Manage Care Medicaid', 'Manage Care Medicaid')");
            Sql("INSERT INTO PlanTypes (PlanTypeId, Code, Name) VALUES (NEWID(), 'Manage Care Medicare', 'Manage Care Medicare')");
            Sql("INSERT INTO PlanTypes (PlanTypeId, Code, Name) VALUES (NEWID(), 'OO', 'Out of Network')");
        }

        public override void Down()
        {
            Sql("DELETE FROM PlanTypes WHERE Code in ('POS', 'HMO', 'EPO', 'PPO', 'Manage Care Medicaid', 'Manage Care Medicare', 'OO')");
        }
    }
}
