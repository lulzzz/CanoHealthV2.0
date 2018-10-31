namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DropUniqueConstraintInDoctorCorporationContractLinksTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[DoctorCorporationContractLinks] DROP CONSTRAINT UK_DoctorCorporationContractLinks_ContractLineofBusinessId_DoctorId");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[DoctorCorporationContractLinks] ADD CONSTRAINT UK_DoctorCorporationContractLinks_ContractLineofBusinessId_DoctorId UNIQUE([ContractLineofBusinessId],[DoctorId])");
        }
    }
}
