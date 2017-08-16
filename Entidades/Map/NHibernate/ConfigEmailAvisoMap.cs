namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class ConfigEmailAvisoMap : ClassMap<ConfigEmailAviso>
    {
        public ConfigEmailAvisoMap()
        {
            base.Table("config_email");
            base.Id(c => c.ID).Column("ce_id").GeneratedBy.Identity();

            base.Map(c => c.Descricao).Column("ce_descricao");
            base.Map(c => c.Tipo).Column("ce_tipo").CustomType(typeof(int));
            base.Map(c => c.DiasAntesVencimento).Column("ce_diasAntes");
            base.Map(c => c.Frequencia).Column("ce_frequencia");
          //base.Map(c => c.Email).Column("ce_email");
            base.Map(c => c.Ativo).Column("ce_ativo");
            base.Map(c => c.TodosContratos).Column("ce_todosContratos");
            base.Map(c => c.DataCriacao).Column("ce_dataCriacao");

            base.References(c => c.Texto).Column("ce_textoId");

            base.References(c => c.AssociadoPj).Column("ce_estipulanteId");
            base.References(c => c.ContratoAdm).Column("ce_contratoAdmId");

            //base.HasMany<Contrato>(c => c.Contratos)
            //    .Table("config_email_contratos")
            //    .KeyColumn("cec_configId"); //nome da FK em config_email_contratos

            base.HasManyToMany<Contrato>(a => a.Contratos)
                .Table("config_email_contratos")
                .ParentKeyColumn("cec_configId")
                .ChildKeyColumn("cec_contratoId").NotFound.Ignore();
        }
    }

    public class ConfigEmailAvisoINSTANCIAMap : ClassMap<ConfigEmailAvisoINSTANCIA>
    {
        public ConfigEmailAvisoINSTANCIAMap()
        {
            base.Table("config_email_instancia");
            base.Id(c => c.ID).Column("cei_id").GeneratedBy.Identity();
            base.Map(c => c.ConfigID).Column("cei_configId");
            base.Map(c => c.CobrancaID).Column("cei_cobrancaId");
            base.Map(c => c.Tipo).Column("cei_tipo").CustomType(typeof(int));
            base.Map(c => c.MSG).Column("cei_mensagem");
            base.Map(c => c.Email).Column("cei_email");
            base.Map(c => c.Data).Column("cei_data");
        }
    }

    public class ConfigEmailTextoMap : ClassMap<ConfigEmailTexto>
    {
        public ConfigEmailTextoMap()
        {
            base.Table("config_email_texto");
            base.Id(c => c.ID).Column("cet_id").GeneratedBy.Identity();

            base.Map(c => c.Descricao).Column("cet_descricao");
            base.Map(c => c.Texto).Column("cet_texto");
            base.Map(c => c.Ativo).Column("cet_ativo");
            base.Map(c => c.DataCriacao).Column("cet_dataCriacao");
        }
    }
}