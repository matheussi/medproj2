namespace MedProj.Entidades
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Data;
    using System.Data.OleDb;
    using System.Configuration;
    using System.Collections.Generic;

    public class AgendaPagamentoItem : EntidadeBase
    {
        public virtual decimal Valor { get; set; }
        public virtual AgendaPagamento Agenda { get; set; }
        public virtual Atendimento Atendimento { get; set; }
    }
}
