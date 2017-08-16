namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class AtendimentoProcedimentoMap : ClassMap<AtendimentoProcedimento>
    {
        public AtendimentoProcedimentoMap()
        {
            base.Table("atendimentoCred_procedimento");
            base.Id(ap => ap.ID).Column("atendimentocredproc_id").GeneratedBy.Identity();

            base.References(ap => ap.Atendimento).Column("atendimentocredproc_atendimentoId");
            base.References(ap => ap.Procedimento).Column("atendimentocredproc_procedimentoId");
          //base.References(ap => ap.Especialidade).Column("atendimentocredproc_especialidadeId");

            base.References(ap => ap.Usuario).Column("atendimentocredproc_usuarioId");

            base.Map(ap => ap.Duplicado).Column("atendimentocredproc_duplicado");

            base.Map(ap => ap.Valor).Column("atendimentocredproc_valor");
            base.Map(ap => ap.Cancelado).Column("atendimentocredproc_cancelado");
            base.Map(ap => ap.DataCancelado).Column("atendimentocredproc_dataCancelado");
        }
    }
}