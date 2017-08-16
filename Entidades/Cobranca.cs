namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using MedProj.Entidades.Enuns;
    using System.Collections.Generic;

    public class Cobranca : EntidadeBase
    {
        //public virtual long ContratoID { get; set; }
        public virtual Contrato Contrato { get; set; }

        public virtual string NossoNumero { get; set; }

        public virtual decimal ValorCobrado { get; set; }
        public virtual decimal ValorPago { get; set; }

        public virtual DateTime DataCriacao { get; set; }
        public virtual DateTime DataVencimento { get; set; }
        public virtual DateTime? DataPagamento { get; set; }
        public virtual DateTime? DataBaixa { get; set; }

        public virtual bool Cancelada { get; set; }

        /// <summary>
        /// Indica o id do último arquivo cnab gerado contendo esta cobraça.
        /// </summary>
        public virtual long? ArquivoEnviadoId { get; set; }

        public virtual bool Pago { get; set; }
    }

    public class Remessa : EntidadeBase
    {
        public Remessa() { Data = DateTime.Now; Tipo = TipoRemessaCnab.Novo; }

        public virtual int QTDBoletos { get; set; }
        public virtual DateTime Data  { get; set; }
        public virtual string Arquivo { get; set; }
        public virtual TipoRemessaCnab Tipo { get; set; }
    }
}
