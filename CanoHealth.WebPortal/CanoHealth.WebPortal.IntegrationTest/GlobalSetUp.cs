using NUnit.Framework;
using System.Data.Entity.Migrations;

namespace CanoHealth.WebPortal.IntegrationTest
{
    [SetUpFixture]
    public class GlobalSetUp
    {
        [SetUp]
        public void SetUp()
        {
            /*If you don't have a database this code is going to create it to the latest version
             and if you have a database, if it's not up to date it would be upgraded*/
            var configuration = new CanoHealth.WebPortal.Migrations.Configuration();
            var migrator = new DbMigrator(configuration);
            migrator.Update();

            /*Now we have to set up the connection string for the integration test database in the app.config*/
        }
    }
}
