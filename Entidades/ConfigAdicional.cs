namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using MedProj.Entidades.Enuns;
    using System.Collections.Generic;

    public class ConfigAdicional : EntidadeBase
    {
        public ConfigAdicional()
        {
            Ativo = true;
        }

        public virtual string Descricao { get; set; }
        public virtual decimal Valor { get; set; }
        public virtual string TextoNoBoleto { get; set; }
        public virtual bool Ativo { get; set; }

        public virtual AssociadoPJ AssociadoPj { get; set; }
        public virtual ContratoADM ContratoAdm { get; set; }

        public virtual bool TodosContratos { get; set; }
        public virtual IList<Contrato> Contratos { get; set; }
    }
}
