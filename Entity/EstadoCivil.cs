namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("estado_civil")]
    public class EstadoCivil : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _operadoraID;
        String _descricao;
        String _codigo;

        #region propriedades

        [DBFieldInfo("estadoCivil_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("estadoCivil_operadoraid", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraID; }
            set { _operadoraID= value; }
        }

        [DBFieldInfo("estadoCivil_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("estadoCivil_codigo", FieldType.Single)]
        public String Codigo
        {
            get { return _codigo; }
            set { _codigo= value; }
        }

        #endregion

        public EstadoCivil() { }
        public EstadoCivil(Object id) { _id = id; }

        public void Salvar()
        {
            base.Salvar(this);
        }
        public void Remover()
        {
            base.Remover(this);
        }
        public void Carregar()
        {
            base.Carregar(this);
        }

        public static IList<EstadoCivil> CarregarTodos(Object operadoraId)
        {
            String query = "* FROM estado_civil WHERE estadocivil_operadoraid=" + operadoraId + " ORDER BY estadoCivil_descricao";
            return LocatorHelper.Instance.ExecuteQuery<EstadoCivil>(query, typeof(EstadoCivil));
        }

        public static Object CarregarID(String descricao, Object operadoraId, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@descricao" };
            String[] paramVl = new String[] { descricao };
            return LocatorHelper.Instance.ExecuteScalar("SELECT estadoCivil_id FROM estado_civil WHERE estadoCivil_operadoraid=" + operadoraId + " AND estadoCivil_descricao=@descricao", paramNm, paramVl, pm);
        }
    }

    /// <summary>
    /// Esta classe representa o estado civil de usuário. A classe EstadoCivil não foi usada pois representa
    /// o estado civil de beneficiário, e está relacionada à operadora.
    /// </summary>
    [DBTable("estado_civilUsuario")]
    public class EstadoCivilUsuario : EntityBase, IPersisteableEntity
    {
        Object _id;
        String _descricao;

        #region propriedades 

        [DBFieldInfo("estadocivilusuario_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("estadocivilusuario_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        #endregion

        public EstadoCivilUsuario() { }

        public void Salvar()
        {
            base.Salvar(this);
        }
        public void Remover()
        {
            base.Remover(this);
        }
        public void Carregar()
        {
            base.Carregar(this);
        }

        public static IList<EstadoCivilUsuario> CarregarTodos()
        {
            String query = "* FROM estado_civilUsuario ORDER BY estadocivilusuario_descricao";
            return LocatorHelper.Instance.ExecuteQuery<EstadoCivilUsuario>(query, typeof(EstadoCivilUsuario));
        }
    }

}