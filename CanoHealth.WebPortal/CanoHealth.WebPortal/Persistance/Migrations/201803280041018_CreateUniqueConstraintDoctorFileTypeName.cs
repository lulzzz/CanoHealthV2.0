namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUniqueConstraintDoctorFileTypeName : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[DoctorFileTypes] ADD CONSTRAINT UK_DoctorFileTypeName UNIQUE([DoctorFileTypeName])");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[DoctorFileTypes] DROP CONSTRAINT UK_DoctorFileTypeName");
        }
    }
}
