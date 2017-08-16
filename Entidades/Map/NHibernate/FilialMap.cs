namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class FilialMap : ClassMap<Filial>
    {
        public FilialMap()
        {
            base.Table("filial");
            base.Id(f => f.ID).Column("filial_id").GeneratedBy.Identity();

            base.Map(f => f.Nome).Column("filial_nome").Not.Nullable();
            base.Map(f => f.Ativa).Column("filial_ativa");
        }
    }
}
