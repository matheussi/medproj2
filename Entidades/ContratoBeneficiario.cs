namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    public class ContratoBeneficiario : EntidadeBase
    {
        public ContratoBeneficiario()
        {
            Status = 0;
            Sequencia = 0;
            Data = DateTime.Now;
        }

        public virtual Contrato Contrato { get; set; }
        public virtual Beneficiario Beneficiario { get; set; }
        public virtual int Tipo { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual int Sequencia { get; set; }

        public virtual int Status { get; protected set; }
        public virtual DateTime Data { get; set; }
        public virtual DateTime Vigencia { get; set; }
    }
}