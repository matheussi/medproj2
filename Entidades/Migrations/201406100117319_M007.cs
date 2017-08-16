namespace MedProj.Entidades.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class M007 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.banco",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Codigo = c.String(),
                        Nome = c.String(),
                        Agencia = c.String(),
                        Conta = c.String(),
                        Tipo = c.Int(nullable: false),
                        Unidade_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.prestador_unidade", t => t.Unidade_ID)
                .Index(t => t.Unidade_ID);
            
            CreateTable(
                "dbo.usuarios",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Nome = c.String(),
                        Login = c.String(),
                        Senha = c.String(),
                        Tipo = c.Int(nullable: false),
                        Unidade_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.prestador_unidade", t => t.Unidade_ID)
                .Index(t => t.Unidade_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.usuarios", "Unidade_ID", "dbo.prestador_unidade");
            DropForeignKey("dbo.banco", "Unidade_ID", "dbo.prestador_unidade");
            DropIndex("dbo.usuarios", new[] { "Unidade_ID" });
            DropIndex("dbo.banco", new[] { "Unidade_ID" });
            DropTable("dbo.usuarios");
            DropTable("dbo.banco");
        }
    }
}
