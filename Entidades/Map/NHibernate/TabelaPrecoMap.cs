namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class TabelaPrecoMap : ClassMap<TabelaPreco>
    {
        public TabelaPrecoMap()
        {
            base.Table("tabela_preco");
            base.Id(c => c.ID).Column("ID").GeneratedBy.Identity();

            base.Map(c => c.Nome).Column("Nome");

            base.HasMany<TabelaPrecoVigencia>(c => c.Vigencias)
                .Table("tabela_preco_vigencia")
                .KeyColumn("Tabela_ID"); //nome da FK em tabela_preco_vigencia
        }
    }
}