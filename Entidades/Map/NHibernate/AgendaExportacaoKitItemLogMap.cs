namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class AgendaExportacaoKitItemLogMap : ClassMap<AgendaExportacaoKitItemLog>
    {
        public AgendaExportacaoKitItemLogMap()
        {
            base.Table("exportacaoKit_log");
            base.Id(i => i.ID).Column("exportacaokitlog_id").GeneratedBy.Identity();

            base.References(i => i.Agenda).Column("exportacaokitlog_agendaId").Not.Nullable();
            base.References(i => i.Titular).Column("exportacaokitlog_titularId").Not.Nullable();
        }
    }
}
