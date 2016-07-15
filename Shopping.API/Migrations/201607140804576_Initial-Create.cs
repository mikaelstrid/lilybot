namespace Lilybot.Shopping.API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        TimestampUtc = c.DateTime(nullable: false),
                        EventType = c.String(),
                        Payload = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Count = c.Int(nullable: false),
                        CountUpdateTimestampUtc = c.DateTime(nullable: false),
                        Username = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Username = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StoreSections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Order = c.Int(nullable: false),
                        Store_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stores", t => t.Store_Id)
                .Index(t => t.Store_Id);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Friends = c.String(),
                        Username = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IgnoredProducts",
                c => new
                    {
                        StoreId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StoreId, t.ProductId })
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.StoreId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.StoreSectionProducts",
                c => new
                    {
                        StoreSectionId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StoreSectionId, t.ProductId })
                .ForeignKey("dbo.StoreSections", t => t.StoreSectionId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.StoreSectionId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StoreSections", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.StoreSectionProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.StoreSectionProducts", "StoreSectionId", "dbo.StoreSections");
            DropForeignKey("dbo.IgnoredProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.IgnoredProducts", "StoreId", "dbo.Stores");
            DropIndex("dbo.StoreSectionProducts", new[] { "ProductId" });
            DropIndex("dbo.StoreSectionProducts", new[] { "StoreSectionId" });
            DropIndex("dbo.IgnoredProducts", new[] { "ProductId" });
            DropIndex("dbo.IgnoredProducts", new[] { "StoreId" });
            DropIndex("dbo.StoreSections", new[] { "Store_Id" });
            DropTable("dbo.StoreSectionProducts");
            DropTable("dbo.IgnoredProducts");
            DropTable("dbo.Profiles");
            DropTable("dbo.StoreSections");
            DropTable("dbo.Stores");
            DropTable("dbo.Products");
            DropTable("dbo.Events");
        }
    }
}
