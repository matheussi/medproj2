using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MedProj.Entidades
{
    public class TabelaPreco
    {
        public TabelaPreco()
        {
            //Vigencias = new List<TabelaPrecoVigencia>();
        }

        public virtual long ID { get; set; }
        public virtual string Nome { get; set; }

        public virtual IList<TabelaPrecoVigencia> Vigencias { get; set; }
    }
}
