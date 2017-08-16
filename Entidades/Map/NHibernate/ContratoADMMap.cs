namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class ContratoADMMap : ClassMap<ContratoADM>
    {
        public ContratoADMMap()
        {
            base.Table("contratoADM");
            base.Id(c => c.ID).Column("contratoadm_id").GeneratedBy.Identity();

            base.Map(c => c.Ativo).Column("contratoadm_ativo");
            base.Map(c => c.Descricao).Column("contratoadm_descricao");

            base.References(c => c.Operadora).Column("contratoadm_operadoraId");
            base.References(c => c.AssociadoPJ).Column("contratoadm_estipulanteId");
        }
    }
}
