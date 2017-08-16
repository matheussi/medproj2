namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class ProcedimentoMap : ClassMap<Procedimento>
    {
        public ProcedimentoMap()
        {
            base.Table("procedimento");
            base.Id(c => c.ID).Column("ID").GeneratedBy.Identity();

            base.Map(c => c.Codigo).Column("Codigo");
            base.Map(c => c.CH).Column("CH");
            base.Map(c => c.Nome).Column("Nome");
            base.Map(c => c.Porte).Column("Porte");

            base.Map(c => c.Categoria).Column("Categoria");
            base.Map(c => c.Especialidade).Column("Especialidade");

            base.References(c => c.Tabela).Column("Tabela_ID").Nullable();
        }
    }
}
