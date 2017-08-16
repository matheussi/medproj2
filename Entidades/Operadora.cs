namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    public class Operadora : EntidadeBase
    {
        public Operadora() { }
        public Operadora(long id) { ID = id; }

        public virtual string Nome { get; set; }
        public virtual bool Inativa { get; set; }
    }
}
