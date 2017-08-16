using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace MedProj.Entidades
{
    public class UnidadeEspecialidade
    {
        public UnidadeEspecialidade()
        {
            //Unidade = new PrestadorUnidade();
            //Especialidade = new Especialidade();
        }

        public virtual long ID { get; set; }
        public virtual PrestadorUnidade Unidade { get; set; }
        public virtual Especialidade Especialidade { get; set; }
    }
}
