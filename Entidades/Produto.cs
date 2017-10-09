namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    public class Produto : EntidadeBase
    {
        public Produto()
        {
            this.DataCriacao = DateTime.Now;
        }

        public virtual string Nome { get; set; }
        public virtual Operadora Operadora { get; set; } //public virtual long OperadoraId { get; set; }
        /// <summary>
        /// Estipulante
        /// </summary>
        public virtual AssociadoPJ AssociadoPj { get; set; } //public virtual long AssociadoPjId { get; set; }
        public virtual ContratoADM ContratoAdm { get; set; } //public virtual long ContratoAdmId { get; set; }

        public virtual DateTime DataCriacao { get; set; }
        public virtual DateTime? DataAlteracao { get; set; }
        public virtual long UsuarioId { get; set; }
    }

    public class ProdutoItem : EntidadeBase
    {
        public ProdutoItem()
        {
            this.DataCriacao = DateTime.Now;
        }

        public virtual long ProdutoId { get; set; }
        public virtual string Nome { get; set; }
        public virtual decimal Valor { get; set; }
        public virtual decimal ValorNet { get; set; }
        public virtual DateTime Vigencia { get; set; }

        public virtual DateTime DataCriacao { get; set; }
        public virtual DateTime? DataAlteracao { get; set; }
        public virtual long UsuarioId { get; set; }
    }
}
