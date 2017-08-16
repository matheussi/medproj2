namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class PrestadorUnidadeMap : ClassMap<PrestadorUnidade>
    {
        public PrestadorUnidadeMap()
        {
            base.Table("prestador_unidade");
            base.Id(c => c.ID).Column("ID").GeneratedBy.Identity();

            base.Map(c => c.Tipo).Column("Tipo").CustomType(typeof(int));
            base.Map(c => c.TipoPagto).Column("TipoPagto").CustomType(typeof(int));

            base.Map(c => c.Documento).Column("Documento");

            base.Map(c => c.Nome).Column("Nome");
            base.Map(c => c.Telefone).Column("Telefone");
            base.Map(c => c.Celular).Column("Celular");
            base.Map(c => c.Email).Column("Email");
            base.Map(c => c.Observacoes).Column("Observacoes");

            //base.HasOne(c => c.Banco);
            //base.References(c => c.Banco).Column("");

            base.References(c => c.Owner).Column("Owner_ID").Nullable();
            base.References(c => c.Regiao).Column("Regiao_ID").Nullable();
            base.References(c => c.Tabela).Column("Tabela_ID").Nullable();
            base.References(c => c.TabelaPreco).Column("TabelaPreco_ID").Nullable();

            base.Map(c => c.CEP).Column("CEP");
            base.Map(c => c.Endereco).Column("Endereco");
            base.Map(c => c.Numero).Column("Numero");
            base.Map(c => c.Complemento).Column("Complemento");
            base.Map(c => c.Bairro).Column("Bairro");
            base.Map(c => c.Cidade).Column("Cidade");
            base.Map(c => c.UF).Column("UF");

            base.Map(c => c.Latitude).Column("Latitude");
            base.Map(c => c.Longitude).Column("Longitude");

            base.HasMany<UnidadeProcedimento>(c => c.Procedimentos)
                .Table("unidade_procedimento")
                .KeyColumn("Unidade_ID"); //nome da FK em unidade_procedimento

            base.HasMany<UnidadeEspecialidade>(c => c.Especialidades)
                .Table("unidade_especialidade")
                .KeyColumn("Unidade_ID"); //nome da FK em unidade_especialidade
        }
    }
}