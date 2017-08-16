using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace MedProj.Entidades
{
    public class Prestador
    {
        public Prestador()
        {
            //Segmento = new Segmento();
            //Unidades = new List<PrestadorUnidade>();
        }

        public virtual long ID { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Observacoes { get; set; }
        public virtual bool Deletado { get; set; }
        //public virtual string Obs { get; set; }

        public virtual Segmento Segmento { get; set; }
        public virtual IList<PrestadorUnidade> Unidades { get; set; }
    }
}
