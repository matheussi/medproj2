using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MedProj.Entidades
{
    public class Procedimento
    {
        public Procedimento()
        {
            ValorSobrescrito = false;
        }

        public virtual long ID { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual decimal CH { get; set; }
        public virtual string Porte { get; set; }

        public virtual string Categoria { get; set; }
        public virtual string Especialidade { get; set; }

        /// <summary>
        /// Tabela de PROCEDIMENTOS
        /// </summary>
        public virtual TabelaProcedimento Tabela { get; set; }

        //não mapeado
        public virtual bool ValorSobrescrito { get; set; }
    }
}
