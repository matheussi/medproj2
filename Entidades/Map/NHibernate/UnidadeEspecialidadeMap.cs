namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class UnidadeEspecialidadeMap : ClassMap<UnidadeEspecialidade>
    {
        public UnidadeEspecialidadeMap()
        {
            base.Table("unidade_especialidade");
            base.Id(c => c.ID).Column("ID").GeneratedBy.Identity();

            base.References(c => c.Especialidade).Column("Especialidade_ID").Not.Nullable();
            base.References(c => c.Unidade).Column("Unidade_ID").Not.Nullable();
        }
    }
}
