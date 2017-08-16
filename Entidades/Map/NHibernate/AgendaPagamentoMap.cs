namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class AgendaPagamentoMap : ClassMap<AgendaPagamento>
    {
        public AgendaPagamentoMap()
        {
            base.Table("agendaPagamento");
            base.Id(i => i.ID).Column("agendapagto_id").GeneratedBy.Identity();

            base.Map(i => i.Descricao).Column("agendapagto_descricao");

            base.Map(i => i.DataCriacao).Column("agendapagto_dataCriacao");
            base.Map(i => i.DataProcessamento).Column("agendapagto_dataExecucao");
            base.Map(i => i.DataConclusao).Column("agendapagto_dataConclusao");

            base.Map(i => i.PeriodoDe).Column("agendapagto_de");
            base.Map(i => i.PeriodoAte).Column("agendapagto_ate");

            base.Map(i => i.Ativa).Column("agendapagto_ativa");
            base.Map(i => i.Processado).Column("agendapagto_processado");

            base.Map(i => i.TipoPagto).Column("agendapagto_tipo").CustomType(typeof(int));

            base.HasMany<AgendaPagamentoItem>(i => i.Itens)
                .Table("agendaPagamento_item")
                .KeyColumn("agendapagtoitem_agendaId"); //[agendapagtoitem_agendaId]nome da FK em agendaPagamento_item
        }
    }
}