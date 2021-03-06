namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUniqueConstraintDoctorPlaceOfServiceInDoctorClinicsTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[DoctorClinics] ADD CONSTRAINT UK_DoctorClinics_DoctorId_PlaceOfServiceId UNIQUE([DoctorId],[PlaceOfServiceId])");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[DoctorClinics] DROP CONSTRAINT UK_DoctorClinics_DoctorId_PlaceOfServiceId");
        }
    }
}
