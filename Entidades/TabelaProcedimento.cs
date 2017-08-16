using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MedProj.Entidades
{
    public class TabelaProcedimento
    {
        public TabelaProcedimento()
        {
            Ativa = true;
            Data = DateTime.Now;
            //Segmento = new Segmento();
            //Procedimentos = new List<Procedimento>();
            //TabelasDeVigencias = new List<TabelaPrecoVigencia>();
        }

        public virtual long ID { get; set; }
        public virtual bool Ativa { get; set; }
        public virtual int Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual DateTime Data { get; set; }

        public virtual Segmento Segmento { get; set; }
        public virtual IList<Procedimento> Procedimentos { get; set; }
        public virtual IList<TabelaPrecoVigencia> TabelasDeVigencias { get; set; }
    }
}
