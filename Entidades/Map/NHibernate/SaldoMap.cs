namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class SaldoMap : ClassMap<Saldo>
    {
        public SaldoMap()
        {
            base.Table("contrato_saldo");
            base.Id(c => c.ID).Column("saldo_id").GeneratedBy.Identity();

            base.Map(c => c.Atual).Column("saldo_atual");
            base.Map(c => c.Credito).Column("saldo_creditoTotal");
            base.Map(c => c.DataMovimentacao).Column("saldo_dataUltimaMovimentacao");
            base.Map(c => c.Debito).Column("saldo_debitoTotal");

            base.References(c => c.Contrato).Column("saldo_contratoId").Not.Nullable();
        }
    }

    public class SaldoMovimentacaoHistoricoMap : ClassMap<SaldoMovimentacaoHistorico>
    {
        public SaldoMovimentacaoHistoricoMap()
        {
            base.Table("contrato_saldo_historico");
            base.Id(c => c.ID).Column("saldohist_id").GeneratedBy.Identity();

            base.Map(c => c.Descricao).Column("saldohist_descricao");
            base.Map(c => c.Data).Column("saldohist_data");
            base.Map(c => c.UsuarioId).Column("saldohist_usuarioId");

            base.References(c => c.Contrato).Column("saldohist_contratoId").Not.Nullable();
        }
    }
}
