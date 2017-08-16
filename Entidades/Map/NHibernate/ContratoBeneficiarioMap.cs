namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class ContratoBeneficiarioMap : ClassMap<ContratoBeneficiario>
    {
        public ContratoBeneficiarioMap()
        {
            base.Table("contrato_beneficiario");
            base.Id(cb => cb.ID).Column("contratobeneficiario_id").GeneratedBy.Identity();

            base.References(cb => cb.Contrato).Column("contratobeneficiario_contratoId");
            base.References(cb => cb.Beneficiario).Column("contratobeneficiario_beneficiarioId");

            base.Map(cb => cb.Tipo).Column("contratobeneficiario_tipo");
            base.Map(cb => cb.Ativo).Column("contratobeneficiario_ativo");

            base.Map(cb => cb.Data).Column("contratobeneficiario_data");
            base.Map(cb => cb.Status).Column("contratobeneficiario_status");
            base.Map(cb => cb.Vigencia).Column("contratobeneficiario_vigencia");

            base.Map(cb => cb.Sequencia).Column("contratobeneficiario_numeroSequencia");
        }
    }
}
