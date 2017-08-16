namespace MedProj.Entidades.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class M002 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.prestador_unidade", "Tabela_ID", c => c.Long());
            CreateIndex("dbo.prestador_unidade", "Tabela_ID");
            AddForeignKey("dbo.prestador_unidade", "Tabela_ID", "dbo.tabela_procedimento", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.prestador_unidade", "Tabela_ID", "dbo.tabela_procedimento");
            DropIndex("dbo.prestador_unidade", new[] { "Tabela_ID" });
            DropColumn("dbo.prestador_unidade", "Tabela_ID");
        }
    }
}
