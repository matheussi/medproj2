namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using MedProj.Entidades.Enuns;

    public class Banco
    {
        public virtual long ID { get; set; }
        public virtual string Codigo { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Agencia { get; set; }
        public virtual string AgenciaDAC { get; set; }
        public virtual string Conta { get; set; }
        public virtual string ContaDAC { get; set; }
        public virtual TipoConta Tipo { get; set; }

        public virtual PrestadorUnidade Unidade { get; set; }
    }
}
