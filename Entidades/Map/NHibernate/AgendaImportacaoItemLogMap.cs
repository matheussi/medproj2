namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class AgendaImportacaoItemLogMap : ClassMap<AgendaImportacaoItemLog>
    {
        public AgendaImportacaoItemLogMap()
        {
            base.Table("importacao_log");
            base.Id(i => i.ID).Column("importacaolog_id").GeneratedBy.Identity();

            base.Map(i => i.Linha).Column("importacaolog_linha");

            base.Map(i => i.Data).Column("importacaolog_data");
            base.Map(i => i.Chave).Column("importacaolog_chave");
            base.Map(i => i.Mensagem).Column("importacaolog_mensagem");
            base.Map(i => i.Status).Column("importacaolog_status").CustomType(typeof(int));

            base.References(i => i.Agenda).Column("importacaolog_agendaId").Not.Nullable();
            base.References(i => i.Titular).Column("importacaolog_titularId");
        }
    }
}