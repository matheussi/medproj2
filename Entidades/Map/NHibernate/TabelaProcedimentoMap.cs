namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class TabelaProcedimentoMap : ClassMap<TabelaProcedimento>
    {
        public TabelaProcedimentoMap()
        {
            base.Table("tabela_procedimento");
            base.Id(c => c.ID).Column("ID").GeneratedBy.Identity();

            base.Map(c => c.Codigo).Column("Codigo");
            base.Map(c => c.Ativa).Column("Ativa");
            base.Map(c => c.Nome).Column("Nome");
            base.Map(c => c.Data).Column("Data");

            base.References(c => c.Segmento).Column("Segmento_ID").Nullable();

            base.HasMany<Procedimento>(c => c.Procedimentos)
                .Table("procedimento")
                .KeyColumn("Tabela_ID"); //nome da FK em procedimento

                base.HasMany<TabelaPrecoVigencia>(c => c.TabelasDeVigencias)
                    .Table("tabela_preco_vigencia")
                    .KeyColumn("TabelaProcedimento_ID"); //nome da FK em tabela_preco_vigencia
        }
    }
}
