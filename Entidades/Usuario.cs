namespace MedProj.Entidades
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;

    using MedProj.Entidades.Enuns;

    public class Usuario
    {
        public Usuario()
        {
            Ativo = true;
            DataCadastro = DateTime.Now;
        }

        public virtual long ID { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Email { get; set; }
        public virtual string Login { get; set; }
        public virtual string Senha { get; set; }

        public virtual bool Ativo { get; set; }
        public virtual DateTime DataCadastro { get; set; }

        public virtual TipoUsuario Tipo { get; set; }
        public virtual PrestadorUnidade Unidade { get; set; }
    }
}
