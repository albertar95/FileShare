namespace Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Folders",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(),
                        IsLocal = c.Boolean(nullable: false),
                        IsPublic = c.Boolean(nullable: false),
                        IsProtected = c.Boolean(nullable: false),
                        Path = c.String(),
                        UserId = c.Guid(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        LastModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Username = c.String(),
                        Password = c.String(),
                        Fullname = c.String(),
                        LastLoginDate = c.DateTime(),
                        IsDisabled = c.Boolean(nullable: false),
                        IsAdmin = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        LastModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Folders", "UserId", "dbo.Users");
            DropIndex("dbo.Folders", new[] { "UserId" });
            DropTable("dbo.Users");
            DropTable("dbo.Folders");
        }
    }
}
