namespace Lilybot.Commute.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPrimaryWayOfCommuting : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "PrimaryWayOfCommuting", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "PrimaryWayOfCommuting");
        }
    }
}
