namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveColumnToContractLineofBusinessTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContractLineofBusinesses", "Active", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ContractLineofBusinesses", "Active");
        }
    }
}
