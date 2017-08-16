namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Configuration;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Framework.BusinessLayer;

    [DBTableAttribute("agente")]
    public class Agente : EntityBase, IPersisteableEntity
    {
        Object _id;

        [DBFieldInfo("usuario_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public Usuario Usuario
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Perfil Perfil
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Endereco Endereco
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        #region métodos EntityBase

        /// <summary>
        /// Persiste a entidade
        /// </summary>
        public void Salvar()
        {
            base.Salvar(this);
        }

        /// <summary>
        /// Remove a entidade
        /// </summary>
        public void Remover()
        {
            base.Remover(this);
        }

        /// <summary>
        /// Carrega a entidade
        /// </summary>
        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion
    }
}
