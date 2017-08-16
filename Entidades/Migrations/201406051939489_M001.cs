namespace MedProj.Entidades.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class M001 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.unidade_procedimento", name: "UProcedimento_ID", newName: "Procedimento_ID");
            RenameIndex(table: "dbo.unidade_procedimento", name: "IX_UProcedimento_ID", newName: "IX_Procedimento_ID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.unidade_procedimento", name: "IX_Procedimento_ID", newName: "IX_UProcedimento_ID");
            RenameColumn(table: "dbo.unidade_procedimento", name: "Procedimento_ID", newName: "UProcedimento_ID");
        }
    }
}
