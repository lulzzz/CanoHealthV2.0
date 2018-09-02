namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddScheduleTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Schedules",
                c => new
                {
                    ScheduleId = c.Guid(nullable: false),
                    Title = c.String(),
                    StartDateTime = c.DateTime(nullable: false),
                    EndDateTime = c.DateTime(nullable: false),
                    StartTimezone = c.String(),
                    EndTimeZone = c.String(),
                    Description = c.String(),
                    IsAllDay = c.Boolean(nullable: false),
                    PlaceOfServiceId = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.ScheduleId)
                .ForeignKey("dbo.PlaceOfServices", t => t.PlaceOfServiceId)//, cascadeDelete: true
                .Index(t => t.PlaceOfServiceId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Schedules", "PlaceOfServiceId", "dbo.PlaceOfServices");
            DropIndex("dbo.Schedules", new[] { "PlaceOfServiceId" });
            DropTable("dbo.Schedules");
        }
    }
}
