namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OverrideCodeColumnConventionsInPlanTypeTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PlanTypes", "Code", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PlanTypes", "Code", c => c.String(maxLength: 10));
        }
    }
}
