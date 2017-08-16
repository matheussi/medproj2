namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class BeneficiarioMap : ClassMap<Beneficiario>
    {
        public BeneficiarioMap()
        {
            base.Table("beneficiario");
            base.Id(c => c.ID).Column("beneficiario_id").GeneratedBy.Identity();

            base.Map(c => c.Tipo).Column("beneficiario_tipo").CustomType(typeof(int));

            base.Map(c => c.EstadoCivilId).Column("beneficiario_estadoCivilId");
            base.Map(c => c.Nome).Column("beneficiario_nome");
            base.Map(c => c.SexoId).Column("beneficiario_sexo");
            base.Map(c => c.CPF).Column("beneficiario_cpf");
            base.Map(c => c.RG).Column("beneficiario_rg");

            base.Map(c => c.RgUF).Column("beneficiario_rgUF");
            base.Map(c => c.RGOrgaoExp).Column("beneficiario_rgOrgaoExp");
            base.Map(c => c.DataNascimento).Column("beneficiario_dataNascimento");
            base.Map(c => c.DataCasamento).Column("beneficiario_dataCasamento");
            base.Map(c => c.Telefone).Column("beneficiario_telefone");
            base.Map(c => c.Ramal).Column("beneficiario_ramal");
            base.Map(c => c.Telefone2).Column("beneficiario_telefone2");
            base.Map(c => c.Ramal2).Column("beneficiario_ramal2");
            base.Map(c => c.Celular).Column("beneficiario_celular");
            base.Map(c => c.CelularOperadora).Column("beneficiario_celularOperadora");
            base.Map(c => c.Email).Column("beneficiario_email");
            base.Map(c => c.NomeMae).Column("beneficiario_nomeMae");

            base.Map(c => c.Altura).Column("beneficiario_altura");
            base.Map(c => c.Peso).Column("beneficiario_peso");
            base.Map(c => c.Data).Column("beneficiario_data");
            base.Map(c => c.DeclaracaoNascimentoVivo).Column("beneficiario_declaracaoNascimentoVivo");
            base.Map(c => c.CNS).Column("beneficiario_cns");
        }
    }
}
