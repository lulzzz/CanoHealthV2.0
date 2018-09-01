namespace CanoHealth.WebPortal.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class PopulateRecurrenceTable : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO [dbo].[Recurrences]([RecurrenceId],[RecurrenceName],[Active]) VALUES (NEWID(),'Never',1)");
            Sql("INSERT INTO [dbo].[Recurrences]([RecurrenceId],[RecurrenceName],[Active]) VALUES (NEWID(),'Daily',1)");
            Sql("INSERT INTO [dbo].[Recurrences]([RecurrenceId],[RecurrenceName],[Active]) VALUES (NEWID(),'Weekly',1)");
            Sql("INSERT INTO [dbo].[Recurrences]([RecurrenceId],[RecurrenceName],[Active]) VALUES (NEWID(),'Monthly',1)");
            Sql("INSERT INTO [dbo].[Recurrences]([RecurrenceId],[RecurrenceName],[Active]) VALUES (NEWID(),'Yearly',1)");
        }

        public override void Down()
        {
            Sql("DELETE FROM [dbo].[Recurrences] WHERE Name in ('Never')");
            Sql("DELETE FROM [dbo].[Recurrences] WHERE Name in ('Daily')");
            Sql("DELETE FROM [dbo].[Recurrences] WHERE Name in ('Weekly')");
            Sql("DELETE FROM [dbo].[Recurrences] WHERE Name in ('Monthly')");
            Sql("DELETE FROM [dbo].[Recurrences] WHERE Name in ('Yearly')");
        }
    }
}
