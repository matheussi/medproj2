namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class CorretorMap : ClassMap<Corretor>
    {
        public CorretorMap()
        {
            base.Table("corretor");
            base.Id(c => c.ID).Column("corretor_id").GeneratedBy.Identity();

            base.Map(c => c.Nome).Column("corretor_nome");

            //base.References(c => c.TabelaComissao).Column("corretor_tabelaComissaoId");
        }
    }
}
