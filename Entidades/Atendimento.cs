namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using Entidades.Enuns;

    public class Atendimento : EntidadeBase
    {
        public Atendimento()
        {
            Pago = false;
            Data = DateTime.Now;
            FormaPagto = FormaPagtoAtendimento.Cartao;
        }

        public virtual PrestadorUnidade Unidade { get; set; }
        public virtual TabelaPrecoVigencia Vigencia { get; set; }
        public virtual Contrato Contrato { get; set; }

        public virtual string NumeroCartao { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual decimal ValorBase { get; set; }

        /// <summary>
        /// True caso o valor total do atendimento ja tenha sido repassado pelo Clube para o Prestador. 
        /// </summary>
        public virtual bool Pago { get; set; }

        /// <summary>
        /// Caso o atendimento tenha sido efetuado por um usuário master
        /// </summary>
        public virtual Usuario UsuarioMaster { get; set; }

        public virtual IList<AtendimentoProcedimento> Procedimentos { get; set; }

        public virtual FormaPagtoAtendimento FormaPagto { get; set; }
    }
}
