namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class UnidadeProcedimentoMap : ClassMap<UnidadeProcedimento>
    {
        public UnidadeProcedimentoMap()
        {
            base.Table("unidade_procedimento");
            base.Id(c => c.ID).Column("ID").GeneratedBy.Identity();

            base.Map(c => c.ValorSobrescrito).Column("ValorSobrescrito").Nullable();

            base.References(c => c.Unidade).Column("Unidade_ID").Not.Nullable();
            base.References(c => c.TabelaPreco).Column("TabelaPreco_ID").Nullable();
            base.References(c => c.Procedimento).Column("Procedimento_ID").Not.Nullable();

            base.Map(c => c.Importado).Column("Importado").Not.Nullable();
        }
    }
}