namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class ArquivoBaixaMap : ClassMap<ArquivoBaixa>
    {
        public ArquivoBaixaMap()
        {
            base.Table("arquivo_baixa");
            base.Id(c => c.ID).Column("arquivobaixa_id").GeneratedBy.Identity();

            base.Map(c => c.Corpo).Column("arquivobaixa_corpo").CustomType("StringClob").CustomSqlType("text");
            base.Map(c => c.Criacao).Column("arquivobaixa_criacao");
            base.Map(c => c.Nome).Column("arquivobaixa_nome").Not.Nullable();
            base.Map(c => c.NumeroLote).Column("arquivobaixa_lote");
            base.Map(c => c.Processamento).Column("arquivobaixa_processamento");
            base.Map(c => c.Tipo).Column("arquivobaixa_tipo").CustomType(typeof(int));
        }
    }
}
