namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using MedProj.Entidades.Enuns;
    using System.Collections.Generic;

    public class ConfigEmailAviso : EntidadeBase
    {
        public ConfigEmailAviso()
        {
            Ativo = true;
            DataCriacao = DateTime.Now;
        }

        public virtual string Descricao { get; set; }
        public virtual TipoConfig Tipo { get; set; }
        public virtual int Frequencia { get; set; }
        public virtual int DiasAntesVencimento { get; set; }
        //public virtual string Email { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual DateTime DataCriacao { get; set; }

        public virtual ConfigEmailTexto Texto { get; set; }

        /// <summary>
        /// Estipulante
        /// </summary>
        public virtual AssociadoPJ AssociadoPj { get; set; }
        public virtual ContratoADM ContratoAdm { get; set; }

        public virtual bool TodosContratos { get; set; }
        public virtual IList<Contrato> Contratos { get; set; }
    }

    public class ConfigEmailAvisoINSTANCIA : EntidadeBase
    {
        public ConfigEmailAvisoINSTANCIA() { }

        public virtual long ConfigID    { get; set; }
        public virtual long CobrancaID  { get; set; }
        public virtual TipoConfig Tipo  { get; set; }
        public virtual string MSG       { get; set; }
        public virtual string Email     { get; set; }
        public virtual DateTime Data    { get; set; }
    }

    public class InstanciaSrcVO
    {
        public long BeneficiarioID      { get; set; }
        public string BeneficiarioNM    { get; set; }
        public string BeneficiarioMAIL  { get; set; }
        public long CobrancaID          { get; set; }
        public decimal CobrancaValor    { get; set; }
        public long PropostaID          { get; set; }
        public DateTime CobrancaDtPagto { get; set; }
        public DateTime CobrancaDtVenct { get; set; }
        public bool CobrancaPAGA        { get; set; }
        public int QtdVidas             { get; set; }
        public string ERRO              { get; set; }
    }

    public class ConfigEmailTexto : EntidadeBase
    {
        public ConfigEmailTexto()
        {
            Ativo = true;
            DataCriacao = DateTime.Now;
        }

        public virtual string Descricao { get; set; }
        public virtual string Texto { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual DateTime DataCriacao { get; set; }
    }
}