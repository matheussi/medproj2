namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class AgendaExportacaoCartaoItemMap : ClassMap<AgendaExportacaoCartaoItem>
    {
        public AgendaExportacaoCartaoItemMap()
        {
            base.Table("exportacaoCartao_log");
            base.Id(i => i.ID).Column("exportacaocartaolog_id").GeneratedBy.Identity();

            base.References(i => i.Agenda).Column("exportacaocartaolog_agendaId").Not.Nullable();
            base.References(i => i.Titular).Column("exportacaocartaolog_titularId").Not.Nullable();
        }
    }
}