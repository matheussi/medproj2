namespace MedProj.Entidades.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class M003 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.prestador_unidade", "CEP", c => c.String());
            AddColumn("dbo.prestador_unidade", "Endereco", c => c.String());
            AddColumn("dbo.prestador_unidade", "Numero", c => c.String());
            AddColumn("dbo.prestador_unidade", "Complemento", c => c.String());
            AddColumn("dbo.prestador_unidade", "Bairro", c => c.String());
            AddColumn("dbo.prestador_unidade", "Cidade", c => c.String());
            AddColumn("dbo.prestador_unidade", "UF", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.prestador_unidade", "UF");
            DropColumn("dbo.prestador_unidade", "Cidade");
            DropColumn("dbo.prestador_unidade", "Bairro");
            DropColumn("dbo.prestador_unidade", "Complemento");
            DropColumn("dbo.prestador_unidade", "Numero");
            DropColumn("dbo.prestador_unidade", "Endereco");
            DropColumn("dbo.prestador_unidade", "CEP");
        }
    }
}
