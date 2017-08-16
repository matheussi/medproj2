namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("contato")]
    public class Contato : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _operadoraId;
        String _nome;
        String _departamento;
        String _ddd;
        String _fone;
        String _ramal;
        String _email;

        #region propriedades 

        [DBFieldInfo("contato_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("contato_operadoraid", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("contato_nome", FieldType.Single)]
        public String Nome
        {
            get { return _nome; }
            set { _nome= value; }
        }

        [DBFieldInfo("contato_departamento", FieldType.Single)]
        public String Departamento
        {
            get { return _departamento; }
            set { _departamento= value; }
        }

        [DBFieldInfo("contato_ddd", FieldType.Single)]
        public String DDD
        {
            get { return _ddd; }
            set { _ddd= value; }
        }

        [DBFieldInfo("contato_fone", FieldType.Single)]
        public String Fone
        {
            get { return _fone; }
            set { _fone= value; }
        }

        [DBFieldInfo("contato_ramal", FieldType.Single)]
        public String Ramal
        {
            get { return _ramal; }
            set { _ramal= value; }
        }

        [DBFieldInfo("contato_email", FieldType.Single)]
        public String Email
        {
            get { return ToLower(_email); }
            set { _email= value; }
        }

        #endregion

        public Contato() { }
        public Contato(Object id) { _id = id; }

        #region métodos EntityBase 

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

        #endregion

        public static IList<Contato> Carregar(Object operadoraId)
        {
            String query = "* FROM contato WHERE contato_operadoraId=" + operadoraId + " ORDER BY contato_nome";
            return LocatorHelper.Instance.ExecuteQuery<Contato>(query, typeof(Contato));
        }
    }
}
