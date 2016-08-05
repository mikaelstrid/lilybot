namespace Lilybot.Commute.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedLocationsAndStationIds : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "HomeLocationLatitude", c => c.Double(nullable: false));
            AddColumn("dbo.Profiles", "HomeLocationLongitude", c => c.Double(nullable: false));
            AddColumn("dbo.Profiles", "WorkLocationLatitude", c => c.Double(nullable: false));
            AddColumn("dbo.Profiles", "WorkLocationLongitude", c => c.Double(nullable: false));
            AddColumn("dbo.Profiles", "HomePublicTransportStationId", c => c.String());
            AddColumn("dbo.Profiles", "WorkPublicTransportStationId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "WorkPublicTransportStationId");
            DropColumn("dbo.Profiles", "HomePublicTransportStationId");
            DropColumn("dbo.Profiles", "WorkLocationLongitude");
            DropColumn("dbo.Profiles", "WorkLocationLatitude");
            DropColumn("dbo.Profiles", "HomeLocationLongitude");
            DropColumn("dbo.Profiles", "HomeLocationLatitude");
        }
    }
}
