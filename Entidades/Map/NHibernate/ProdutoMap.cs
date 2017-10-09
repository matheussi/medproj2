namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class ProdutoMap : ClassMap<Produto>
    {
        public ProdutoMap()
        {
            base.Table("produto");

            base.Id(c => c.ID).Column("produto_id").GeneratedBy.Identity();

            base.Map(c => c.Nome).Column("produto_nome");

            base.References(c => c.Operadora).Column("produto_operadoraId");
            base.References(c => c.AssociadoPj).Column("produto_associadoId");
            base.References(c => c.ContratoAdm).Column("produto_contratoAdmId");

            base.Map(c => c.UsuarioId).Column("produto_usuarioId");
            base.Map(c => c.DataCriacao).Column("produto_data");
            base.Map(c => c.DataAlteracao).Column("produto_dataAlteracao");
        }
    }

    public class ProdutoItemMap : ClassMap<ProdutoItem>
    {
        public ProdutoItemMap()
        {
            base.Table("produto_item");

            base.Id(c => c.ID).Column("produtoitem_id").GeneratedBy.Identity();

            base.Map(c => c.ProdutoId).Column("produtoitem_produtoId");
            base.Map(c => c.Nome).Column("produtoitem_nome");
            base.Map(c => c.Vigencia).Column("produtoitem_vigencia");
            base.Map(c => c.Valor).Column("produtoitem_valor");
            base.Map(c => c.ValorNet).Column("produtoitem_valorNet");

            base.Map(c => c.UsuarioId).Column("produtoitem_usuarioId");
            base.Map(c => c.DataCriacao).Column("produtoitem_data");
            base.Map(c => c.DataAlteracao).Column("produtoitem_dataAlteracao");
        }
    }
}
