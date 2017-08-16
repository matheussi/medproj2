namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    public class UnidadeProcedimento
    {
        public UnidadeProcedimento()
        {
            Importado = false;
        }

        public virtual long ID { get; set; }
        public virtual PrestadorUnidade Unidade { get; set; }
        public virtual Procedimento Procedimento { get; set; }
        public virtual decimal? ValorSobrescrito { get; set; }

        public virtual TabelaPreco TabelaPreco { get; set; }

        public virtual bool Importado { get; set; }

        public virtual decimal ValorCalculado
        {
            get
            {
                if (this.TabelaPreco != null)
                {
                    TabelaPrecoVigencia vigencia = TabelaPreco.Vigencias.Where(tp => tp.Ativa == true).OrderByDescending(tp => tp.DataInicio).FirstOrDefault();

                    if (vigencia != null)
                        return decimal.Round((Procedimento.CH * vigencia.ValorReal), 2);
                    else
                        return decimal.Zero;
                }
                else if (this.ValorSobrescrito.HasValue)
                    return decimal.Round(this.ValorSobrescrito.Value, 2);
                else
                    return decimal.Zero;
            }
        }
    }
}