namespace WcfServiceBooks.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TimeStamps12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Image", c => c.Binary(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Image");
        }
    }
}
