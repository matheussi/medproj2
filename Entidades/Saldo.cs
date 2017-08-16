namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using Entidades.Enuns;

    public class Saldo : EntidadeBase
    {
        /// <summary>
        /// Saldo atual do cliente
        /// </summary>
        public virtual decimal Atual { get; protected set; }
        public virtual decimal Credito { get; protected set; }
        public virtual decimal Debito { get; protected set; }
        public virtual DateTime? DataMovimentacao { get; protected set; }

        public virtual Contrato Contrato { get; set; }

        public virtual void Movimentar(TipoMovimentacao tipo, decimal valor, DateTime? dataMovimentacao)
        {
            if (dataMovimentacao.HasValue)
                DataMovimentacao = dataMovimentacao;
            else
                DataMovimentacao = DateTime.Now;

            if (tipo == TipoMovimentacao.Credito)
            {
                Atual += valor;
                Credito += valor;
            }
            else
            {
                Atual -= valor;
                Debito += valor;
            }
        }
    }

    public class SaldoMovimentacaoHistorico : EntidadeBase
    {
        public virtual Contrato Contrato { get; set; }
        public virtual string Descricao { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual long? UsuarioId { get; set; }
    }
}
