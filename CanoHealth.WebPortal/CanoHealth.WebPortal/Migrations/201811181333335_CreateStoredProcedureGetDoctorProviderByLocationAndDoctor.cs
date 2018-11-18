namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateStoredProcedureGetDoctorProviderByLocationAndDoctor : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("[dbo].[GetDoctorProviderByLocationAndDoctor]", parameter => new
            {
                DoctorId = parameter.Guid()
            },
            @"
            BEGIN
                SELECT     PBL.*
                FROM       [dbo].[Doctors] D (NOLOCK)
                INNER JOIN [dbo].[DoctorCorporationContractLinks] DCCL (NOLOCK)
		                   ON D.DoctorId = DCCL.DoctorId
                INNER JOIN [dbo].[ProviderByLocations] PBL (NOLOCK)
                           ON DCCL.DoctorCorporationContractLinkId = PBL.DoctorCorporationContractLinkId
                WHERE      D.DoctorId = @DoctorId
                AND	       D.Active = 1
                AND        DCCL.Active = 1
                AND        PBL.Active = 1
            END
            "
            );
        }

        public override void Down()
        {
            Sql("DROP PROCEDURE [dbo].[GetDoctorProviderByLocationAndDoctor]");
        }
    }
}
