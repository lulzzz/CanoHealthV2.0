namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateStoredProcedureGetSearchResultByCorporationAndInsurance : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetSearchResultByCorporationAndInsurance]", parameters => new
            {
                CorporationId = parameters.Guid(),
                InsuranceId = parameters.Guid()
            },
            @"
                BEGIN
                    SELECT     C.InsuranceId,
                               C.ContractId,
                               C.GroupNumber,
                               CLB.ContractLineofBusinessId,
                               LB.Code,
                               LB.Name
                    FROM       [dbo].[Contracts] C (NOLOCK)
                    INNER JOIN [dbo].[ContractLineofBusinesses] CLB (NOLOCK)
                               ON C.ContractId = CLB.ContractId
                    INNER JOIN [dbo].[PlanTypes] LB (NOLOCK)
                               ON CLB.PlanTypeId = LB.PlanTypeId
                    WHERE      C.CorporationId = @CorporationId
                    AND        C.InsuranceId = @InsuranceId
                    AND        C.Active = 1
                    AND        CLB.Active = 1
                    AND        LB.Active = 1
                END
            ");
        }
        
        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetSearchResultByCorporationAndInsurance]");
        }
    }
}
