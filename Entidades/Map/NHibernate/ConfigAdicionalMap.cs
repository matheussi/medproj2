namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class ConfigAdicionalMap : ClassMap<ConfigAdicional>
    {
        public ConfigAdicionalMap()
        {
            base.Table("config_adicional");
            base.Id(c => c.ID).Column("ca_id").GeneratedBy.Identity();

            base.Map(c => c.Descricao).Column("ca_descricao");
            base.Map(c => c.TextoNoBoleto).Column("ca_texto");
            base.Map(c => c.Valor).Column("ca_valor");
            base.Map(c => c.Ativo).Column("ca_ativo");
            base.Map(c => c.TodosContratos).Column("ca_todosContratos");

            base.References(c => c.AssociadoPj).Column("ca_estipulanteId");
            base.References(c => c.ContratoAdm).Column("ca_contratoAdmId");

            base.HasManyToMany<Contrato>(a => a.Contratos)
                .Table("config_adicional_contratos")
                .ParentKeyColumn("cac_configId")
                .ChildKeyColumn("cac_contratoId").NotFound.Ignore(); //nome da FK em config_adicional_contratos
        }
    }
}
