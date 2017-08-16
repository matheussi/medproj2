namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class PlanoMap : ClassMap<Plano>
    {
        public PlanoMap()
        {
            base.Table("plano");
            base.Id(p => p.ID).Column("plano_id").GeneratedBy.Identity();

            base.Map(p => p.Descricao).Column("plano_descricao").Not.Nullable();
            base.Map(p => p.Ativo).Column("plano_ativo");

            base.Map(p => p.Data).Column("plano_data");
            base.Map(p => p.QuartoComum).Column("plano_quartoComum");
            base.Map(p => p.QuartoParticular).Column("plano_quartoParticular");

            base.References(p => p.ContratoAdm).Column("plano_contratoId").Nullable();
        }
    }
}