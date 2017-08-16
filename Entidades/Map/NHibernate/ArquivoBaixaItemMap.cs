namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class ArquivoBaixaItemMap : ClassMap<ArquivoBaixaItem>
    {
        public ArquivoBaixaItemMap()
        {
            base.Table("arquivo_baixa_item");
            base.Id(c => c.ID).Column("arquivobaixaitem_id").GeneratedBy.Identity();

            base.Map(c => c.Data).Column("arquivobaixaitem_data");
            base.Map(c => c.Identificacao).Column("arquivobaixaitem_identificacao").Not.Nullable();

            base.Map(c => c.DataCredito).Column("arquivobaixaitem_dataCredito").Nullable();
            base.Map(c => c.AgenciaOrigem).Column("arquivobaixaitem_agenciaOrigem");
            base.Map(c => c.DataRemessa).Column("arquivobaixaitem_dataRemessa").Nullable();
            base.Map(c => c.ValorPago).Column("arquivobaixaitem_valorPago");

            //TODO: remover essa property, mapear os objetos corretamente
            base.Map(c => c.Titular).Column("arquivobaixaitem_titular");

            base.Map(c => c.Status).Column("arquivobaixaitem_status").CustomType(typeof(int));
            base.References(c => c.Arquivo).Column("arquivobaixaitem_arquivoId").Not.Nullable();
            base.References(c => c.Contrato).Column("arquivobaixaitem_contratoId").Nullable();
            base.References(c => c.Cobranca).Column("arquivobaixaitem_cobrancaId").Nullable();
        }
    }
}
