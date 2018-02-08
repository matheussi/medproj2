namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    class TabelaCoberturaMap : ClassMap<TabelaCobertura>
    {
        public TabelaCoberturaMap()
        {
            base.Table("tabela_cobertura");

            base.Id(c => c.ID).Column("tabela_id").GeneratedBy.Identity();

            base.Map(c => c.Nome).Column("tabela_nome");
            //base.Map(c => c.ValorPorVida).Column("tabela_valorPorVida");

            base.References(c => c.AssociadoPj).Column("tabela_associadoPjId");
            base.References(c => c.ContratoAdm).Column("tabela_contratoAdmId");

            base.HasMany<ItemCobertura>(c => c.Itens)
                .Table("tabela_cobertura_item")
                .KeyColumn("itemcobertura_tabelaId"); //nome da FK em tabela_cobertura_item

            base.HasMany<VigenciaCobertura>(c => c.Vigencias)
                .Table("tabela_cobertura_vigencia")
                .KeyColumn("vigcobertura_tabelaId");
        }
    }

    class VigenciaCoberturaMap : ClassMap<VigenciaCobertura>
    {
        public VigenciaCoberturaMap()
        {
            base.Table("tabela_cobertura_vigencia");
            base.Id(c => c.ID).Column("vigcobertura_id").GeneratedBy.Identity();

            base.Map(c => c.Valor).Column("vigcobertura_valor");
            base.Map(c => c.ValorNet).Column("vigcobertura_valorNet");

            base.Map(c => c.Inicio).Column("vigcobertura_inicio");

            base.References(c => c.Tabela).Column("vigcobertura_tabelaId").Not.Nullable();
        }
    }

    class ItemCoberturaMap : ClassMap<ItemCobertura>
    {
        public ItemCoberturaMap()
        {
            base.Table("tabela_cobertura_item");
            base.Id(c => c.ID).Column("itemcobertura_id").GeneratedBy.Identity();

            base.Map(c => c.Descricao).Column("itemcobertura_descricao");
            base.Map(c => c.Status).Column("status_");
            base.Map(c => c.Valor).Column("itemcobertura_valor").Nullable();

            base.References(c => c.Tabela).Column("itemcobertura_tabelaId").Not.Nullable();
        }
    }
}