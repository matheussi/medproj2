namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    public class ContratoADM : EntidadeBase
    {
        public ContratoADM() { Ativo = true; }
        public ContratoADM(long id) : this() { ID = id; }

        public virtual string Descricao { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual Operadora Operadora { get; set; }
        public virtual AssociadoPJ AssociadoPJ { get; set; }
    }
}
