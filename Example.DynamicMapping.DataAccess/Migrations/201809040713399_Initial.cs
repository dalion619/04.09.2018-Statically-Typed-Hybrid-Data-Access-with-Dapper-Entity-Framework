namespace Example.DynamicMapping.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Street = c.String(),
                        City = c.String(),
                        State = c.String(),
                        ZipCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        UserName = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        Address_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Address", t => t.Address_Id)
                .Index(t => t.Address_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.People", "Address_Id", "dbo.Address");
            DropIndex("dbo.People", new[] { "Address_Id" });
            DropTable("dbo.People");
            DropTable("dbo.Address");
        }
    }
}
