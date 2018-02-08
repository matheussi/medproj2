namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    public class TabelaCobertura : EntidadeBase
    {
        public virtual string Nome { get; set; }
        public virtual ContratoADM ContratoAdm { get; set; }
        public virtual AssociadoPJ AssociadoPj { get; set; }

        //public virtual decimal ValorPorVida { get; set; }

        public virtual IList<ItemCobertura> Itens { get; set; }
        public virtual IList<VigenciaCobertura> Vigencias { get; set; }
    }

    public class VigenciaCobertura : EntidadeBase
    {
        public virtual TabelaCobertura Tabela { get; set; }

        public virtual DateTime Inicio { get; set; }
        public virtual decimal Valor { get; set; }
        public virtual decimal ValorNet { get; set; }
    }

    public class ItemCobertura : EntidadeBase
    {
        public virtual TabelaCobertura Tabela { get; set; }

        public virtual string Descricao { get; set; }
        public virtual decimal? Valor { get; set; }
        public virtual string Status { get; set; }
    }
}
