namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class NumeroCartaoMap : ClassMap<NumeroCartao>
    {
        public NumeroCartaoMap()
        {
            base.Table("numero_contrato");
            base.Id(c => c.ID).Column("numerocontrato_id").GeneratedBy.Identity();

            base.Map(c => c.CV).Column("numerocontrato_cv").Not.Nullable();
            base.Map(c => c.Ativo).Column("numerocontrato_ativo").Not.Nullable();
            base.Map(c => c.Data).Column("numerocontrato_data");
            base.Map(c => c.Numero).Column("numerocontrato_numero").Not.Nullable();

            base.Map(c => c.DV).Column("numerocontrato_dv").Not.Nullable();
            base.Map(c => c.Via).Column("numerocontrato_via").Not.Nullable();

            base.References(c => c.Contrato).Column("numerocontrato_contratoId").Nullable();
        }
    }
}