namespace Lilybot.Shopping.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSlackUserId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "SlackUserId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "SlackUserId");
        }
    }
}
