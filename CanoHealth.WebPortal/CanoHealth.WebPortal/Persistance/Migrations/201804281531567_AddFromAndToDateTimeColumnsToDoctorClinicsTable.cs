namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFromAndToDateTimeColumnsToDoctorClinicsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DoctorClinics", "FromDateTime", c => c.DateTime());
            AddColumn("dbo.DoctorClinics", "ToDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DoctorClinics", "ToDateTime");
            DropColumn("dbo.DoctorClinics", "FromDateTime");
        }
    }
}
