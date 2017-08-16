namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class AssociadoPJMap : ClassMap<AssociadoPJ>
    {
        public AssociadoPJMap()
        {
            base.Table("estipulante");
            base.Id(a => a.ID).Column("estipulante_id").GeneratedBy.Identity();

            base.Map(a => a.Nome).Column("estipulante_descricao").Not.Nullable();
            base.Map(a => a.Radical).Column("estipulante_radical");

            base.Map(a => a.BeneficiarioID).Column("estipulante_beneficiarioId");

            base.Map(a => a.Ativo).Column("estipulante_ativo");

            base.Map(a => a.DataValidadeFixa).Column("estipulante_dataValidadeFixa").Nullable();
            base.Map(a => a.MesesAPartirDaVigencia).Column("estipulante_dataValidadeMeses").Nullable();
        }
    }
}