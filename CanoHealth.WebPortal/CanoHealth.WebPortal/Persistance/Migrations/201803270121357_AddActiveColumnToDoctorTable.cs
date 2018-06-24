namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddActiveColumnToDoctorTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Doctors", "Active", c => c.Boolean());
        }

        public override void Down()
        {
            DropColumn("dbo.Doctors", "Active");
        }
    }
}
