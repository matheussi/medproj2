namespace MedProj.Entidades.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.beneficiario",
                c => new
                    {
                        beneficiario_id = c.Long(nullable: false, identity: true),
                        beneficiario_estadoCivilId = c.Int(nullable: false),
                        beneficiario_nome = c.String(),
                        beneficiario_sexo = c.String(),
                        beneficiario_cpf = c.String(),
                        beneficiario_rg = c.String(),
                        beneficiario_rgUF = c.String(),
                        beneficiario_rgOrgaoExp = c.String(),
                        beneficiario_dataNascimento = c.DateTime(nullable: false),
                        beneficiario_dataCasamento = c.DateTime(nullable: false),
                        beneficiario_telefone = c.String(),
                        beneficiario_ramal = c.String(),
                        beneficiario_telefone2 = c.String(),
                        beneficiario_ramal2 = c.String(),
                        beneficiario_celular = c.String(),
                        beneficiario_celularOperadora = c.String(),
                        beneficiario_email = c.String(),
                        beneficiario_nomeMae = c.String(),
                        beneficiario_altura = c.Decimal(nullable: false, precision: 18, scale: 2),
                        beneficiario_peso = c.Decimal(nullable: false, precision: 18, scale: 2),
                        beneficiario_data = c.DateTime(nullable: false),
                        beneficiario_declaracaoNascimentoVivo = c.String(),
                        beneficiario_cns = c.String(),
                    })
                .PrimaryKey(t => t.beneficiario_id);
            
            CreateTable(
                "dbo.especialidade",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Codigo = c.String(),
                        Nome = c.String(),
                        Descricao = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.prestador",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Nome = c.String(),
                        Observacoes = c.String(),
                        Segmento_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.segmento", t => t.Segmento_ID)
                .Index(t => t.Segmento_ID);
            
            CreateTable(
                "dbo.segmento",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Nome = c.String(),
                        Ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.prestador_unidade",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Tipo = c.Int(nullable: false),
                        Documento = c.String(),
                        Nome = c.String(),
                        Telefone = c.String(),
                        Celular = c.String(),
                        Email = c.String(),
                        Observacoes = c.String(),
                        Owner_ID = c.Long(),
                        Regiao_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.prestador", t => t.Owner_ID)
                .ForeignKey("dbo.regiao", t => t.Regiao_ID)
                .Index(t => t.Owner_ID)
                .Index(t => t.Regiao_ID);
            
            CreateTable(
                "dbo.unidade_procedimento",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Unidade_ID = c.Long(),
                        UProcedimento_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.prestador_unidade", t => t.Unidade_ID)
                .ForeignKey("dbo.procedimento", t => t.UProcedimento_ID)
                .Index(t => t.Unidade_ID)
                .Index(t => t.UProcedimento_ID);
            
            CreateTable(
                "dbo.procedimento",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Codigo = c.Int(nullable: false),
                        Nome = c.String(),
                        CH = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Porte = c.String(),
                        Tabela_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.tabela_procedimento", t => t.Tabela_ID)
                .Index(t => t.Tabela_ID);
            
            CreateTable(
                "dbo.tabela_procedimento",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Ativa = c.Boolean(nullable: false),
                        Codigo = c.Int(nullable: false),
                        Nome = c.String(),
                        Data = c.DateTime(nullable: false),
                        Segmento_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.segmento", t => t.Segmento_ID)
                .Index(t => t.Segmento_ID);
            
            CreateTable(
                "dbo.tabela_preco_vigencia",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Ativa = c.Boolean(nullable: false),
                        FatorCH = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DataInicio = c.DateTime(nullable: false),
                        DataFim = c.DateTime(),
                        Tabela_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.tabela_procedimento", t => t.Tabela_ID)
                .Index(t => t.Tabela_ID);
            
            CreateTable(
                "dbo.regiao",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nome = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.prestador_unidade", "Regiao_ID", "dbo.regiao");
            DropForeignKey("dbo.unidade_procedimento", "UProcedimento_ID", "dbo.procedimento");
            DropForeignKey("dbo.tabela_preco_vigencia", "Tabela_ID", "dbo.tabela_procedimento");
            DropForeignKey("dbo.tabela_procedimento", "Segmento_ID", "dbo.segmento");
            DropForeignKey("dbo.procedimento", "Tabela_ID", "dbo.tabela_procedimento");
            DropForeignKey("dbo.unidade_procedimento", "Unidade_ID", "dbo.prestador_unidade");
            DropForeignKey("dbo.prestador_unidade", "Owner_ID", "dbo.prestador");
            DropForeignKey("dbo.prestador", "Segmento_ID", "dbo.segmento");
            DropIndex("dbo.tabela_preco_vigencia", new[] { "Tabela_ID" });
            DropIndex("dbo.tabela_procedimento", new[] { "Segmento_ID" });
            DropIndex("dbo.procedimento", new[] { "Tabela_ID" });
            DropIndex("dbo.unidade_procedimento", new[] { "UProcedimento_ID" });
            DropIndex("dbo.unidade_procedimento", new[] { "Unidade_ID" });
            DropIndex("dbo.prestador_unidade", new[] { "Regiao_ID" });
            DropIndex("dbo.prestador_unidade", new[] { "Owner_ID" });
            DropIndex("dbo.prestador", new[] { "Segmento_ID" });
            DropTable("dbo.regiao");
            DropTable("dbo.tabela_preco_vigencia");
            DropTable("dbo.tabela_procedimento");
            DropTable("dbo.procedimento");
            DropTable("dbo.unidade_procedimento");
            DropTable("dbo.prestador_unidade");
            DropTable("dbo.segmento");
            DropTable("dbo.prestador");
            DropTable("dbo.especialidade");
            DropTable("dbo.beneficiario");
        }
    }
}
