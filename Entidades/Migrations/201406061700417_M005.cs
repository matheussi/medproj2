namespace MedProj.Entidades.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class M005 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.prestador_unidade", "TabelaPreco_ID", c => c.Long());
            CreateIndex("dbo.prestador_unidade", "TabelaPreco_ID");
            AddForeignKey("dbo.prestador_unidade", "TabelaPreco_ID", "dbo.tabela_preco", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.prestador_unidade", "TabelaPreco_ID", "dbo.tabela_preco");
            DropIndex("dbo.prestador_unidade", new[] { "TabelaPreco_ID" });
            DropColumn("dbo.prestador_unidade", "TabelaPreco_ID");
        }
    }
}
