namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    public class Endereco : EntidadeBase
    {
        public Endereco()
        {
            DonoTipo = 0; //Beneficiario
        }

        public virtual long DonoId { get; set; }
        public virtual Int32 DonoTipo { get; protected set; }
        public virtual String Logradouro { get; set; }
        public virtual String Numero { get; set; }
        public virtual String Complemento { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String UF  { get; set; }
        public virtual String CEP { get; set; }
        public virtual Int32 Tipo { get; set; }
    }
}