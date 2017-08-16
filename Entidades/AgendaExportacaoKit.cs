namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using MedProj.Entidades.Enuns;

    public class AgendaExportacaoKit : EntidadeBase
    {
        public AgendaExportacaoKit()
        {
            this.Ativa = true;
            this.DataCriacao = DateTime.Now;
        }

        public virtual void InicializarInstancias()
        {
            Autor = new Usuario();

            Filial = new Filial();
            Operadora = new Operadora();
            Contrato = new ContratoADM();
            AssociadoPj = new AssociadoPJ();
        }

        public virtual bool Ativa { get; set; }

        public virtual string Descricao { get; set; }
        public virtual DateTime DataCriacao { get; set; }
        /// <summary>
        /// Data em que deverá ocorrer o processamento
        /// </summary>
        public virtual DateTime DataProcessamento { get; set; }
        public virtual DateTime? DataConclusao { get; set; }

        public virtual Usuario Autor { get; set; }

        public virtual Filial Filial { get; set; }
        public virtual AssociadoPJ AssociadoPj { get; set; }
        public virtual Operadora Operadora { get; set; }
        public virtual ContratoADM Contrato { get; set; }
    }
}