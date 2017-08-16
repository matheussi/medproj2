namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using MedProj.Entidades.Enuns;

    public class ArquivoBaixa : EntidadeBase
    {
        public ArquivoBaixa()
        {
            Criacao = DateTime.Now;
            Tipo = TipoArquivoBaixa.Itau;
        }

        public virtual string Nome { get; set; }
        public virtual string NumeroLote { get; set; }
        public virtual TipoArquivoBaixa Tipo { get; set; }
        public virtual string Corpo { get; set; }
        public virtual DateTime Criacao { get; set; }
        public virtual DateTime? Processamento { get; set; }
    }
}
