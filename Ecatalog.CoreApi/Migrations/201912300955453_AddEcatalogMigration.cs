namespace Ecatalog.CoreApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEcatalogMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Note = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ISBN = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        YearOfPublication = c.Int(nullable: false),
                        ProgrammingLanguage = c.String(nullable: false),
                        ReaderLevel = c.Int(nullable: false),
                        Language = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BookAuthors",
                c => new
                    {
                        Book_Id = c.Guid(nullable: false),
                        Author_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Book_Id, t.Author_Id })
                .ForeignKey("dbo.Books", t => t.Book_Id, cascadeDelete: true)
                .ForeignKey("dbo.Authors", t => t.Author_Id, cascadeDelete: true)
                .Index(t => t.Book_Id)
                .Index(t => t.Author_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookAuthors", "Author_Id", "dbo.Authors");
            DropForeignKey("dbo.BookAuthors", "Book_Id", "dbo.Books");
            DropIndex("dbo.BookAuthors", new[] { "Author_Id" });
            DropIndex("dbo.BookAuthors", new[] { "Book_Id" });
            DropTable("dbo.BookAuthors");
            DropTable("dbo.Books");
            DropTable("dbo.Authors");
        }
    }
}
