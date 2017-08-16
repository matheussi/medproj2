namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using MedProj.Entidades.Enuns;

    public class AgendaImportacaoItemLog : EntidadeBase
    {
        protected AgendaImportacaoItemLog() { }

        public AgendaImportacaoItemLog(AgendaImportacao agenda)
        {
            Agenda = agenda;
            Data = DateTime.Now;
        }

        /// <summary>
        /// Número da linha no arquivo
        /// </summary>
        public virtual long Linha { get; set; }
        /// <summary>
        /// Identificador da linha
        /// </summary>
        public virtual string Chave { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual string Mensagem { get; set; }
        public virtual AgendaImportacao Agenda { get; protected set; }
        public virtual ContratoBeneficiario Titular { get; set; }
        public virtual AgendaImportacaoItemLogStatus Status { get; set; }
    }
}