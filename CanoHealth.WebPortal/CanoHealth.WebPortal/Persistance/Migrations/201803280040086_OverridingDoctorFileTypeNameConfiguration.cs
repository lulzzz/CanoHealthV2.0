namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OverridingDoctorFileTypeNameConfiguration : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DoctorFileTypes", "DoctorFileTypeName", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DoctorFileTypes", "DoctorFileTypeName", c => c.String(nullable: false));
        }
    }
}
