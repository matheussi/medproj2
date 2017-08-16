namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class RegraComissaoMap : ClassMap<RegraComissao>
    {
        public RegraComissaoMap()
        {
            base.Table("regraComissao");
            base.Id(c => c.ID).Column("regracom_id").GeneratedBy.Identity();

            base.Map(c => c.Nome).Column("regracom_nome");

            //base.Map(c => c.EstipulanteID).Column("regracom_estipulanteId");
            base.References(c => c.Estipulante).Column("regracom_estipulanteId").Not.Nullable();

            //base.Map(c => c.CorretorID).Column("regracom_corretorId");
            //base.References(c => c.Corretor).Column("regracom_corretorId"); //.Not.Nullable();

            base.Map(c => c.Data).Column("regracom_data");
            base.Map(c => c.Ativa).Column("regracom_ativo");
        }
    }

    public class RegraComissaoItemMap : ClassMap<RegraComissaoItem>
    {
        public RegraComissaoItemMap()
        {
            base.Table("regraComissaoItem");
            base.Id(c => c.ID).Column("regracomitem_id").GeneratedBy.Identity();

            base.Map(c => c.RegraID).Column("regracomitem_regraId");
            base.References(c => c.Corretor).Column("regracomitem_corretorId");
            base.Map(c => c.Percentual).Column("regracomitem_percentual");
            base.Map(c => c.Parcela).Column("regracomitem_parcela").Nullable();
            base.Map(c => c.Vitalicio).Column("regracomitem_vitalicio");
        }
    }

    //public class RegracomCorretorMap : ClassMap<RegracomCorretor>
    //{
    //    public RegracomCorretorMap()
    //    {
    //        base.Table("regraComissao_corretor");
    //        base.Id(c => c.ID).Column("regracomissaocorretor_id").GeneratedBy.Identity();

    //        base.References(c => c.Regra).Column("regracomissaocorretor_regraId").Not.Nullable();
    //        base.References(c => c.Corretor).Column("regracomissaocorretor_corretorId").Not.Nullable();
    //        base.Map(c => c.UsuarioId).Column("regracomissaocorretor_usuarioId").Not.Nullable();
    //    }
    //}

    public class RegracomContratoMap : ClassMap<RegracomContrato>
    {
        public RegracomContratoMap()
        {
            base.Table("regraComissao_contrato");
            base.Id(c => c.ID).Column("regracomissaocontrato_id").GeneratedBy.Identity();

            base.References(c => c.Regra).Column("regracomissaocontrato_regraId").Not.Nullable();
            base.References(c => c.Contrato).Column("regracomissaocontrato_contratoId").Not.Nullable();
            base.Map(c => c.UsuarioId).Column("regracomissaocontrato_usuarioId").Not.Nullable();
        }
    }

    //public class ComissaoInicioConfMap : ClassMap<ComissaoInicioConf>
    //{
    //    public ComissaoInicioConfMap()
    //    {
    //        base.Table("regraComissaoExcecao");

    //        base.Id(c => c.ID).Column("regracomissaoexcecao_id").GeneratedBy.Identity();

    //        base.Map(c => c.ContratoId).Column("regracomissaoexcecao_contratoId").Not.Nullable();
    //        base.Map(c => c.Tipo).Column("regracomissaoexcecao_tipo").CustomType(typeof(int));
    //        base.Map(c => c.Data).Column("regracomissaoexcecao_dataInicio");
    //        base.Map(c => c.UsuarioId).Column("regracomissaoexcecao_usuarioId");
    //    }
    //}

    public class RegraComissaoItemExcecaoMap : ClassMap<RegraComissaoItemExcecao>
    {
        public RegraComissaoItemExcecaoMap()
        {
            base.Table("regraComissaoExcecao");
            base.Id(c => c.ID).Column("regracomissaoexcecao_id").GeneratedBy.Identity();

            base.Map(c => c.RegraID).Column("regracomissaoexcecao_regraid").Not.Nullable();
            base.References(c => c.Corretor).Column("regracomissaoexcecao_corretorId").Not.Nullable();
            base.Map(c => c.ContratoID).Column("regracomissaoexcecao_contratoId");
            base.Map(c => c.Parcela).Column("regracomissaoexcecao_parcela").Not.Nullable();
            base.Map(c => c.Percentual).Column("regracomissaoexcecao_percentual").Not.Nullable();
            base.Map(c => c.Vitalicio).Column("regracomissaoexcecao_vitalicio").Not.Nullable();
            base.Map(c => c.NaoComissionado).Column("regracomissaoexcecao_naocomissionado").Not.Nullable();
        }
    }

    //public class RegraComissaoItemExcecaoMap2 : ClassMap<RegraComissaoItemExcecao2>
    //{
    //    public RegraComissaoItemExcecaoMap2()
    //    {
    //        base.Table("regraComissaoExcecao");
    //        base.Id(c => c.ID).Column("regracomissaoexcecao_id").GeneratedBy.Identity();

    //        base.Map(c => c.RegraID).Column("regracomissaoexcecao_regraid").Not.Nullable();
    //        base.Map(c => c.CorretorID).Column("regracomissaoexcecao_corretorId").Not.Nullable();
    //        base.Map(c => c.ContratoID).Column("regracomissaoexcecao_contratoId");
    //        base.Map(c => c.Parcela).Column("regracomissaoexcecao_parcela").Not.Nullable();
    //        base.Map(c => c.Percentual).Column("regracomissaoexcecao_percentual").Not.Nullable();
    //        base.Map(c => c.Vitalicio).Column("regracomissaoexcecao_vitalicio").Not.Nullable();
    //        base.Map(c => c.NaoComissionado).Column("regracomissaoexcecao_naocomissionado").Not.Nullable();
    //    }
    //}
}