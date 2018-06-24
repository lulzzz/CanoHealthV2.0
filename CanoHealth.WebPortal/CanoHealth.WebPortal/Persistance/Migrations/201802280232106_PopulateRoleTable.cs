namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class PopulateRoleTable : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO AspNetRoles(Id, Name) VALUES (NEWID(), 'ADMIN')");
        }

        public override void Down()
        {
            Sql("DELETE FROM AspNetRoles WHERE Name in ('Administrator')");
        }
    }
}
