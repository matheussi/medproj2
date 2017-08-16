namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class CobrancaMap : ClassMap<Cobranca>
    {
        public CobrancaMap()
        {
            base.Table("cobranca");
            base.Id(c => c.ID).Column("cobranca_id").GeneratedBy.Identity();

            base.Map(c => c.DataPagamento).Column("cobranca_dataPagto");
            base.Map(c => c.DataVencimento).Column("cobranca_dataVencimento");
            base.Map(c => c.DataBaixa).Column("cobranca_dataBaixaAuto");
            base.Map(c => c.DataCriacao).Column("cobranca_dataCriacao");

            base.Map(c => c.ValorCobrado).Column("cobranca_valor");
            base.Map(c => c.ValorPago).Column("cobranca_valorPagto");

            //base.Map(c => c.ContratoID).Column("cobranca_propostaId");
            base.References(c => c.Contrato).Column("cobranca_propostaId");

            base.Map(c => c.Cancelada).Column("cobranca_cancelada");
            base.Map(c => c.NossoNumero).Column("cobranca_nossoNumero");

            base.Map(c => c.Pago).Column("cobranca_pago");

            base.Map(c => c.ArquivoEnviadoId).Column("cobranca_arquivoUltimoEnvioId");
        }
    }

    public class RemessaMap : ClassMap<Remessa>
    {
        public RemessaMap()
        {
            base.Table("remessa");
            base.Id(c => c.ID).Column("remessa_id").GeneratedBy.Identity();

            base.Map(c => c.QTDBoletos).Column("remessa_qtdBoletos");
            base.Map(c => c.Data).Column("remessa_data");
            base.Map(c => c.Arquivo).Column("remessa_arquivo");
            base.Map(c => c.Tipo).Column("remessa_tipo").CustomType(typeof(int));
        }
    }
}
