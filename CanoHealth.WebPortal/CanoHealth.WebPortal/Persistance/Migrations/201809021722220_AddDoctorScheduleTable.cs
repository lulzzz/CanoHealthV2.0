namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDoctorScheduleTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DoctorSchedules",
                c => new
                    {
                        DoctorScheduleId = c.Guid(nullable: false),
                        ScheduleId = c.Guid(nullable: false),
                        DoctorId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.DoctorScheduleId)
                .ForeignKey("dbo.Doctors", t => t.DoctorId)//, cascadeDelete: true
                .ForeignKey("dbo.Schedules", t => t.ScheduleId)//, cascadeDelete: true
                .Index(t => t.ScheduleId)
                .Index(t => t.DoctorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DoctorSchedules", "ScheduleId", "dbo.Schedules");
            DropForeignKey("dbo.DoctorSchedules", "DoctorId", "dbo.Doctors");
            DropIndex("dbo.DoctorSchedules", new[] { "DoctorId" });
            DropIndex("dbo.DoctorSchedules", new[] { "ScheduleId" });
            DropTable("dbo.DoctorSchedules");
        }
    }
}
