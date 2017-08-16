namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class AgendaPagamentoItemMap : ClassMap<AgendaPagamentoItem>
    {
        public AgendaPagamentoItemMap()
        {
            base.Table("agendaPagamento_item");
            base.Id(ap => ap.ID).Column("agendapagtoitem_id").GeneratedBy.Identity();

            base.References(ap => ap.Agenda).Column("agendapagtoitem_agendaId");
            base.References(ap => ap.Atendimento).Column("agendapagtoitem_atendimentoId");


            base.Map(ap => ap.Valor).Column("agendapagtoitem_valor");
        }
    }
}
