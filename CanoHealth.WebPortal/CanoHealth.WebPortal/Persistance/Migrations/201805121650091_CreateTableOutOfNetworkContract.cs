namespace CanoHealth.WebPortal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTableOutOfNetworkContract : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OutOfNetworkContracts",
                c => new
                    {
                        OutOfNetworkContractId = c.Guid(nullable: false),
                        DoctorId = c.Guid(nullable: false),
                        InsurnaceId = c.Guid(nullable: false),
                        EffectiveDate = c.DateTime(),
                        ExpirationDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.OutOfNetworkContractId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OutOfNetworkContracts");
        }
    }
}
