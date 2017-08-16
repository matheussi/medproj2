namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class UsuarioMap : ClassMap<Usuario>
    {
        public UsuarioMap()
        {
            base.Table("usuarios");
            base.Id(c => c.ID).Column("ID").GeneratedBy.Identity();

            base.Map(c => c.Login).Column("Login");
            base.Map(c => c.Nome).Column("Nome");
            base.Map(c => c.Email).Column("Email");
            base.Map(c => c.Senha).Column("Senha");
            base.Map(c => c.Tipo).Column("Tipo").CustomType(typeof(int));

            base.Map(c => c.Ativo).Column("Ativo");
            base.Map(c => c.DataCadastro).Column("DataCadastro");

            base.References(c => c.Unidade).Column("Unidade_ID").Nullable();
        }
    }
}
