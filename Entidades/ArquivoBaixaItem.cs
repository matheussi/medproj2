namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using Entidades.Enuns;

    public class ArquivoBaixaItem : EntidadeBase
    {
        public ArquivoBaixaItem()
        {
            Data = DateTime.Now;
            Status = TipoItemBaixa.TituloLiquidado;
        }

        public virtual ArquivoBaixa Arquivo { get; set; }
        public virtual Contrato Contrato { get; set; }
        public virtual Cobranca Cobranca { get; set; }
        public virtual DateTime Data { get; set; }

        /// <summary>
        /// Data do crédito em conta
        /// </summary>
        public virtual DateTime? DataCredito { get; set; }
        public virtual string AgenciaOrigem { get; set; }

        /// <summary>
        /// Data de pagamento
        /// </summary>
        public virtual DateTime? DataRemessa { get; set; }
        public virtual decimal ValorPago { get; set; }

        /// <summary>
        /// Nosso número ou número do contrato
        /// </summary>
        public virtual string Identificacao { get; set; }

        /// <summary>
        /// TODO: remover essa property, mapear os objetos corretamente
        /// </summary>
        public virtual string Titular { get; set; }

        public virtual TipoItemBaixa Status { get; set; }
    }
}