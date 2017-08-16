namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using MedProj.Entidades.Enuns;
    using System.Collections.Generic;

    public class RegraComissao : EntidadeBase
    {
        public RegraComissao() { this.Data = DateTime.Now; this.Ativa = true; }

        public virtual string Nome { get; set; }
        public virtual AssociadoPJ Estipulante { get; set; } 
        //public virtual Corretor Corretor { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual bool Ativa { get; set; }
    }

    public class RegraComissaoItem : EntidadeBase
    {
        public virtual Int64 RegraID { get; set; }
        public virtual decimal Percentual { get; set; }
        public virtual int? Parcela { get; set; }
        public virtual bool Vitalicio { get; set; }
        public virtual Corretor Corretor { get; set; }
    }

    [Obsolete("Em desuso", false)]
    public class RegracomCorretor : EntidadeBase
    {
        public virtual RegraComissao Regra { get; set; }
        public virtual Corretor Corretor { get; set; }
        public virtual long UsuarioId { get; set; }
    }

    public class RegracomContrato : EntidadeBase
    {
        public virtual RegraComissao Regra { get; set; }
        public virtual Contrato Contrato { get; set; }
        public virtual long UsuarioId { get; set; }
    }

    [Obsolete("Em desuso", false)]
    public class ComissaoInicioConf : EntidadeBase
    {
        public virtual DateTime? Data { get; set; }
        public virtual long ContratoId { get; set; }
        public virtual ComissaoInicioConfTipo Tipo { get; set; }
        public virtual int UsuarioId { get; set; }
        //ligar para tatiane - falar que nao vai dar para gerar o arquivo css...
    }

    /// <summary>
    /// Usado para sobrescrever as configurações de comissionamento da tabela para um corretor
    /// </summary>
    public class RegraComissaoItemExcecao : EntidadeBase
    {
        public RegraComissaoItemExcecao()
        {
            this.NaoComissionado = false;
        }

        public virtual Int64 RegraID { get; set; }
        public virtual bool NaoComissionado { get; set; }
        public virtual decimal Percentual { get; set; }
        public virtual int Parcela { get; set; }
        public virtual bool Vitalicio { get; set; }
        public virtual Corretor Corretor { get; set; }
        public virtual Int64 ContratoID { get; set; }
    }

    //public class RegraComissaoItemExcecao2 : EntidadeBase
    //{
    //    public RegraComissaoItemExcecao2()
    //    {
    //        this.NaoComissionado = false;
    //    }

    //    public virtual Int64 RegraID { get; set; }
    //    public virtual bool NaoComissionado { get; set; }
    //    public virtual decimal Percentual { get; set; }
    //    public virtual int Parcela { get; set; }
    //    public virtual bool Vitalicio { get; set; }
    //    public virtual Int64 CorretorID { get; set; }
    //    public virtual Int64 ContratoID { get; set; }
    //}
}
