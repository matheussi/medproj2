using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedProj.Entidades
{
    public class TabelaPrecoVigencia
    {
        public TabelaPrecoVigencia() 
        {
            Ativa = true;
            //Tabela = new TabelaPreco();
        }

        public virtual long ID { get; set; }
        public virtual bool Ativa { get; set; }

        public virtual TabelaPreco Tabela { get; set; }

        public virtual decimal ValorReal { get; set; }
        public virtual DateTime DataInicio { get; set; }
        public virtual DateTime? DataFim { get; set; }
    }
}
