namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using Entidades.Enuns;

    public class AtendimentoProcedimento : EntidadeBase
    {
        public AtendimentoProcedimento()
        {
            Cancelado = false;
        }

        public virtual Atendimento Atendimento { get; set; }
        public virtual Procedimento Procedimento { get; set; }
      //public virtual Especialidade Especialidade { get; set; }
        public virtual decimal Valor { get; set; }

        public virtual bool Cancelado { get; set; }
        public virtual DateTime? DataCancelado { get; set; }

        /// <summary>
        /// Usuário que efetuou o cancelamento
        /// </summary>
        public virtual Usuario Usuario { get; set; }
        public virtual bool Duplicado { get; set; }
    }
}
