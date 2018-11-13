namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveColumnToLineOfBusiness : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlanTypes", "Active", c => c.Boolean());
            Sql("ALTER TABLE [dbo].[PlanTypes] ADD CONSTRAINT PlanTypes_Active DEFAULT 1 FOR Active");
        }
        
        public override void Down()
        {
            DropColumn("dbo.PlanTypes", "Active");
            Sql("ALTER TABLE [dbo].[PlanTypes] DROP CONSTRAINT PlanTypes_Active");
        }
    }
}
