namespace Venetian.DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Text = c.String(),
                        Receiver_UserId = c.Int(),
                        Sender_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.Users", t => t.Receiver_UserId)
                .ForeignKey("dbo.Users", t => t.Sender_UserId)
                .Index(t => t.Receiver_UserId)
                .Index(t => t.Sender_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "Sender_UserId", "dbo.Users");
            DropForeignKey("dbo.Messages", "Receiver_UserId", "dbo.Users");
            DropIndex("dbo.Messages", new[] { "Sender_UserId" });
            DropIndex("dbo.Messages", new[] { "Receiver_UserId" });
            DropTable("dbo.Messages");
        }
    }
}
