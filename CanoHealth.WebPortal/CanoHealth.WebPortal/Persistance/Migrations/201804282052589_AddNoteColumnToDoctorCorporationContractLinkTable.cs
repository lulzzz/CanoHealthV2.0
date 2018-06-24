namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddNoteColumnToDoctorCorporationContractLinkTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorCorporationContractLinks", "Note", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.DoctorCorporationContractLinks", "Note");
        }
    }
}
