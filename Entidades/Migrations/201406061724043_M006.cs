namespace MedProj.Entidades.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class M006 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.unidade_especialidade",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Especialidade_ID = c.Long(),
                        Unidade_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.especialidade", t => t.Especialidade_ID)
                .ForeignKey("dbo.prestador_unidade", t => t.Unidade_ID)
                .Index(t => t.Especialidade_ID)
                .Index(t => t.Unidade_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.unidade_especialidade", "Unidade_ID", "dbo.prestador_unidade");
            DropForeignKey("dbo.unidade_especialidade", "Especialidade_ID", "dbo.especialidade");
            DropIndex("dbo.unidade_especialidade", new[] { "Unidade_ID" });
            DropIndex("dbo.unidade_especialidade", new[] { "Especialidade_ID" });
            DropTable("dbo.unidade_especialidade");
        }
    }
}
