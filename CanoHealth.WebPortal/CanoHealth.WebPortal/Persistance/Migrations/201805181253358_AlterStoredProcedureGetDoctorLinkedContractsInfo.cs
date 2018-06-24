namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AlterStoredProcedureGetDoctorLinkedContractsInfo : DbMigration
    {
        public override void Up()
        {
            AlterStoredProcedure("[dbo].[GetDoctorLinkedContractsInfo]", parameter => new
            {
                DoctorID = parameter.Guid()
            },
           @"
                BEGIN
	                -- SET NOCOUNT ON added to prevent extra result sets from
	                -- interfering with SELECT statements.
	                SET NOCOUNT ON;

                    -- Insert statements for procedure here
	                SELECT 
		                linked.DoctorCorporationContractLinkId,
		                linked.DoctorId,
                        linked.EffectiveDate,
						linked.Note,
		                contracts.GroupNumber,
		                corp.CorporationName,	
		                insurance.Name as InsuranceName,
		                clb.ContractLineofBusinessId,
		                bl.PlanTypeId,
		                bl.Name as BusinessLine
	                FROM [dbo].[Corporations] corp
		                INNER JOIN [dbo].[Contracts] contracts
			                ON corp.[CorporationId] = contracts.[CorporationId]
		                INNER JOIN [dbo].[Insurances] insurance
			                ON contracts.InsuranceId = insurance.InsuranceId
		                INNER JOIN [dbo].[ContractLineofBusinesses] clb --ContractLineOfBusiness
			                ON contracts.ContractId = clb.ContractId
		                INNER JOIN [dbo].[PlanTypes] bl
			                on clb.PlanTypeId = bl.PlanTypeId
		                INNER JOIN [dbo].[DoctorCorporationContractLinks] linked
			                ON clb.ContractLineofBusinessId = linked.ContractLineofBusinessId
	                WHERE linked.DoctorId = @DoctorId
                END
            ");
        }

        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetDoctorLinkedContractsInfo]");
        }
    }
}
