namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class AtendimentoMap : ClassMap<Atendimento>
    {
        public AtendimentoMap()
        {
            base.Table("atendimentoCred");
            base.Id(a => a.ID).Column("atendimentocred_id").GeneratedBy.Identity();

            base.References(a => a.Contrato).Column("atendimentocred_contratoId");
            base.References(a => a.Unidade).Column("atendimentocred_unidadeId");
            base.References(a => a.Vigencia).Column("atendimentocred_vigenciaId");

            base.Map(a => a.Data).Column("atendimentocred_data");
            base.Map(a => a.NumeroCartao).Column("atendimentocred_numeroCartao");
            base.Map(a => a.ValorBase).Column("atendimentocred_valorBase");

            base.Map(a => a.Pago).Column("atendimentocred_pago").Not.Nullable();

            base.Map(a => a.FormaPagto).Column("atendimentocred_formapago").CustomType(typeof(int));

            base.References(c => c.UsuarioMaster).Column("atendimentocred_usuarioId").Nullable();

            base.HasMany<AtendimentoProcedimento>(a => a.Procedimentos)
                .Table("atendimentoCred_procedimento")
                .KeyColumn("atendimentocredproc_atendimentoId"); //[atendimentoproc_atendimentoId]nome da FK em atendimento_procedimento
        }
    }
}