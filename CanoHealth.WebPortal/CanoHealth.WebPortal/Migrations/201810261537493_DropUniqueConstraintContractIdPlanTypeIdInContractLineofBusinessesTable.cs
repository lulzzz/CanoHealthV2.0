namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DropUniqueConstraintContractIdPlanTypeIdInContractLineofBusinessesTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE ContractLineofBusinesses DROP CONSTRAINT UK_ContractId_PlanTypeId");
        }

        public override void Down()
        {
            Sql("ALTER TABLE ContractLineofBusinesses ADD CONSTRAINT UK_ContractId_PlanTypeId UNIQUE([ContractId], [PlanTypeId])");
        }
    }
}
