namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class OperadoraMap : ClassMap<Operadora>
    {
        public OperadoraMap()
        {
            base.Table("operadora");
            base.Id(o => o.ID).Column("operadora_id").GeneratedBy.Identity();

            base.Map(o => o.Nome).Column("operadora_nome").Not.Nullable();
            base.Map(o => o.Inativa).Column("operadora_inativa");
        }
    }
}