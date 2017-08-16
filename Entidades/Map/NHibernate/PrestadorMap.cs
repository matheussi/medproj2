namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class PrestadorMap : ClassMap<Prestador>
    {
        public PrestadorMap()
        {
            base.Table("Prestador");
            base.Id(c => c.ID).Column("ID").GeneratedBy.Identity();

            base.Map(c => c.Nome).Column("Nome");
            base.Map(c => c.Observacoes).Column("Observacoes");
            base.Map(c => c.Deletado).Column("Deletado");

            base.References(c => c.Segmento).Column("Segmento_ID").Nullable();

            base.HasMany<PrestadorUnidade>(c => c.Unidades)
                .Table("prestador_unidade")
                .KeyColumn("Owner_ID"); ////nome da FK em prestador_unidade
        }
    }
}