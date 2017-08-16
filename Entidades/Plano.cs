namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    public class Plano : EntidadeBase
    {
        public Plano(long id) : this() { ID = id; }
        public Plano() { Data = DateTime.Now; QuartoComum = true; }

        public virtual string Descricao { get; set; }
        public virtual bool Ativo { get; set; }
        public virtual ContratoADM ContratoAdm { get; set; }

        public virtual DateTime Data { get; set; }

        public virtual bool QuartoComum { get; set; }
        public virtual bool QuartoParticular { get; set; }
    }
}
