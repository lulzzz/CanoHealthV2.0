namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUniqueConstraintDoctorIdAndDoctorFileTypeIdInDoctorFileTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[DoctorFiles] ADD CONSTRAINT UK_DoctorFiles_DoctorId_DoctorFileTypeId UNIQUE([DoctorId], [DoctorFileTypeId])");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[DoctorFiles] DROP CONSTRAINT UK_DoctorFiles_DoctorId_DoctorFileTypeId");
        }
    }
}
