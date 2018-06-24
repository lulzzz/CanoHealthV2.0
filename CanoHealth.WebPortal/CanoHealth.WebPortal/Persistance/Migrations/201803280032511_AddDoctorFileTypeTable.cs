namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDoctorFileTypeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorFileTypes",
                c => new
                    {
                        DoctorFileTypeId = c.Guid(nullable: false),
                        DoctorFileTypeName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.DoctorFileTypeId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DoctorFileTypes");
        }
    }
}
