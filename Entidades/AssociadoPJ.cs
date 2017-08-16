namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using MedProj.Entidades.Enuns;

    public class AssociadoPJ : EntidadeBase
    {
        public AssociadoPJ()
        {
        }

        public virtual string Nome { get; set; }
        public virtual string Radical { get; set; }

        public virtual bool Ativo { get; set; }

        public virtual TipoDataValidade TipoDataValidade
        {
            get
            {
                if (this.DataValidadeFixa.HasValue)
                    return Enuns.TipoDataValidade.DataFixa;
                else if (this.MesesAPartirDaVigencia.HasValue)
                    return Enuns.TipoDataValidade.MesesAPartirDaVigencia;
                else
                    return Enuns.TipoDataValidade.Indefinido;
            }
        }

        public virtual long? BeneficiarioID { get; set; }

        public virtual Nullable<DateTime> DataValidadeFixa { get; set; }
        public virtual Nullable<Int32> MesesAPartirDaVigencia { get; set; }
    }
}
