namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class SegmentoMap : ClassMap<Segmento>
    {
        public SegmentoMap()
        {
            base.Table("segmento");
            base.Id(c => c.ID).Column("ID").GeneratedBy.Identity();

            base.Map(c => c.Nome).Column("Nome");
            base.Map(c => c.Ativo).Column("Ativo");
            base.Map(c => c.Detalhamento).Column("Detalhamento");
        }
    }
}
