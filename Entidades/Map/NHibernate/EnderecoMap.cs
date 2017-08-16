namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class EnderecoMap : ClassMap<Endereco>
    {
        public EnderecoMap()
        {
            base.Table("endereco");
            base.Id(c => c.ID).Column("endereco_id").GeneratedBy.Identity();

            base.Map(c => c.DonoId).Column("endereco_donoId");
            base.Map(c => c.DonoTipo).Column("endereco_donoTipo");
            base.Map(c => c.Logradouro).Column("endereco_logradouro");
            base.Map(c => c.Numero).Column("endereco_numero");
            base.Map(c => c.Complemento).Column("endereco_complemento");
            base.Map(c => c.Bairro).Column("endereco_bairro");
            base.Map(c => c.Cidade).Column("endereco_cidade");
            base.Map(c => c.UF).Column("endereco_uf");
            base.Map(c => c.CEP).Column("endereco_cep");
            base.Map(c => c.Tipo).Column("endereco_tipo");
        }
    }
}