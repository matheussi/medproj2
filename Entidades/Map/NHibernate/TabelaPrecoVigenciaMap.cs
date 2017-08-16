namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class TabelaPrecoVigenciaMap : ClassMap<TabelaPrecoVigencia>
    {
        public TabelaPrecoVigenciaMap()
        {
            base.Table("tabela_preco_vigencia");
            base.Id(c => c.ID).Column("ID").GeneratedBy.Identity();

            base.Map(c => c.Ativa).Column("Ativa");
            base.Map(c => c.DataInicio).Column("DataInicio");
            base.Map(c => c.DataFim).Column("DataFim");
            base.Map(c => c.ValorReal).Column("ValorReal");

            base.References(c => c.Tabela).Column("Tabela_ID").Not.Nullable();
        }
    }
}
