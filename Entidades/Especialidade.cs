using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace MedProj.Entidades
{
    public class Especialidade
    {
        public virtual long ID { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Descricao { get; set; }

        public virtual long SegmentoId { get; set; }
    }
}
