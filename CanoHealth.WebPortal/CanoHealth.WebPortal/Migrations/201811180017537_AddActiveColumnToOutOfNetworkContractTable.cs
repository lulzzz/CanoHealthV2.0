namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveColumnToOutOfNetworkContractTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OutOfNetworkContracts", "Active", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OutOfNetworkContracts", "Active");
        }
    }
}
