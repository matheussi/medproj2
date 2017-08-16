namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class EspecialidadeMap : ClassMap<Especialidade>
    {
        public EspecialidadeMap()
        {
            base.Table("especialidade");
            base.Id(c => c.ID).Column("ID").GeneratedBy.Identity();

            base.Map(c => c.Codigo).Column("Codigo");
            base.Map(c => c.Nome).Column("Nome");
            base.Map(c => c.Descricao).Column("Descricao");
            base.Map(c => c.SegmentoId).Column("segmento_id");
        }
    }
}
