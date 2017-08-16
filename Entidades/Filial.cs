namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    public class Filial : EntidadeBase
    {
        public Filial() { }
        public Filial(long id) { ID = id; }

        public virtual string Nome { get; set; }
        public virtual bool Ativa { get; set; }
    }
}
