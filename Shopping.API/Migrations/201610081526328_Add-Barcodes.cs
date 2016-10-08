namespace Lilybot.Shopping.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBarcodes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Barcodes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Barcodes");
        }
    }
}
