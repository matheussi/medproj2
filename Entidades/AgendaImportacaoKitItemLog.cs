namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using MedProj.Entidades.Enuns;

    public class AgendaExportacaoKitItemLog : EntidadeBase
    {
        public AgendaExportacaoKitItemLog() { }

        public AgendaExportacaoKitItemLog(AgendaExportacaoKit agenda)
        {
            this.Agenda = agenda;
        }

        public virtual ContratoBeneficiario Titular { get; set; }
        public virtual AgendaExportacaoKit Agenda { get; set; }
    }
}
