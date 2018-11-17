namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;
    //get doctor corporation contract links by contract([dbo].[DoctorCorporationContractLinks])
    public partial class CreateStoredProcedureGetDoctorCorporationContractLinksByContract : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetDoctorCorporationContractLinksByContract]", parameter => new
            {
                ContractId = parameter.Guid()
            },
            @"
            BEGIN
                SELECT     DCCL.* 
                FROM       [dbo].[Contracts] C (NOLOCK)
                INNER JOIN [dbo].[ContractLineofBusinesses] CLB (NOLOCK)
                           ON C.ContractId = CLB.ContractId
                INNER JOIN [dbo].[DoctorCorporationContractLinks] DCCL (NOLOCK)
		                   ON CLB.ContractLineofBusinessId = DCCL.ContractLineofBusinessId
                WHERE      C.ContractId = @ContractId
                AND        C.Active = 1
                AND        CLB.Active = 1
                AND        DCCL.Active = 1
            END
");
        }

        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetDoctorCorporationContractLinksByContract]");
        }
    }
}
