namespace MedProj.Entidades.Map.NHibernate
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using FluentNHibernate.Mapping;

    public class BancoMap : ClassMap<Banco>
    {
        public BancoMap()
        {
            base.Table("Banco");
            base.Id(c => c.ID).Column("ID").GeneratedBy.Identity();

            base.Map(c => c.Codigo).Column("Codigo");
            base.Map(c => c.Nome).Column("Nome");
            base.Map(c => c.Agencia).Column("Agencia");
            base.Map(c => c.AgenciaDAC).Column("AgenciaDAC");
            base.Map(c => c.Conta).Column("Conta");
            base.Map(c => c.ContaDAC).Column("ContaDAC");
            base.Map(c => c.Tipo).Column("Tipo").CustomType(typeof(Int32));

            base.References(c => c.Unidade).Column("Unidade_ID").Not.Nullable();
        }
    }
}
