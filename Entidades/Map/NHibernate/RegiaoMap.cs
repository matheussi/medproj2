namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class RegiaoMap : ClassMap<Regiao>
    {
        public RegiaoMap()
        {
            base.Table("regiao");
            base.Id(c => c.ID).Column("ID").GeneratedBy.Identity();

            base.Map(c => c.Nome).Column("Nome");
        }
    }
}
