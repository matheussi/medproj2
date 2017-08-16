using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace MedProj.Entidades
{
    public class Segmento
    {
        public virtual long ID { get; set; }
        public virtual string Nome { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual bool Detalhamento { get; set; }
    }
}
