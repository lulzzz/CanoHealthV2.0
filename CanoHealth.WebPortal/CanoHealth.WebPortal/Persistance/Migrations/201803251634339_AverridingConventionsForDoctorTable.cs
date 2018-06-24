namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AverridingConventionsForDoctorTable : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE [dbo].[Doctors] ADD CONSTRAINT UK_SocialSecurityNumber UNIQUE([SocialSecurityNumber])");

            Sql("ALTER TABLE [dbo].[Doctors] ADD CONSTRAINT UK_NpiNumber UNIQUE([NpiNumber])");

            Sql("ALTER TABLE [dbo].[Doctors] ADD CONSTRAINT UK_CaqhNumber UNIQUE([CaqhNumber])");
        }

        public override void Down()
        {
            Sql("ALTER TABLE [dbo].[Doctors] DROP CONSTRAINT UK_SocialSecurityNumber");

            Sql("ALTER TABLE [dbo].[Doctors] DROP CONSTRAINT UK_NpiNumber");

            Sql("ALTER TABLE [dbo].[Doctors] DROP CONSTRAINT UK_CaqhNumber");
        }
    }
}
