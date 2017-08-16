namespace MedProj.Entidades.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class M004 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tabela_preco_vigencia", "Tabela_ID", "dbo.tabela_procedimento");
            CreateTable(
                "dbo.tabela_preco",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Nome = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.tabela_preco_vigencia", "ValorReal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tabela_preco_vigencia", "TabelaProcedimento_ID", c => c.Long());
            CreateIndex("dbo.tabela_preco_vigencia", "TabelaProcedimento_ID");
            AddForeignKey("dbo.tabela_preco_vigencia", "TabelaProcedimento_ID", "dbo.tabela_procedimento", "ID");
            DropColumn("dbo.tabela_preco_vigencia", "FatorCH");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tabela_preco_vigencia", "FatorCH", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropForeignKey("dbo.tabela_preco_vigencia", "TabelaProcedimento_ID", "dbo.tabela_procedimento");
            DropIndex("dbo.tabela_preco_vigencia", new[] { "TabelaProcedimento_ID" });
            DropColumn("dbo.tabela_preco_vigencia", "TabelaProcedimento_ID");
            DropColumn("dbo.tabela_preco_vigencia", "ValorReal");
            DropTable("dbo.tabela_preco");
            AddForeignKey("dbo.tabela_preco_vigencia", "Tabela_ID", "dbo.tabela_procedimento", "ID");
        }
    }
}
