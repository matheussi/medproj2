namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    
    [Serializable]
    public class Corretor : EntidadeBase
    {
        public virtual string Nome { get; set; }

        //public virtual RegraComissao TabelaComissao { get; set; }
    }
}
