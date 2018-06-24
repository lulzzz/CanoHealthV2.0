namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveColumnToClinicDoctorsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorClinics", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DoctorClinics", "Active");
        }
    }
}
