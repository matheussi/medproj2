namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("checklist")]
    public class CheckList : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _operadoraId;
        String _descricao;

        #region properties 

        [DBFieldInfo("checklist_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("checklist_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("checklist_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao = value; }
        }

        public String StrTipo
        {
            get
            {
                if (_operadoraId == null)
                    return "Geral";
                else
                    return "Específico";
            }
        }

        #endregion

        public CheckList() { }
        public CheckList(Object id) { _id = id; }

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

        public static IList<CheckList> Carregar(Object operadoraId)
        {
            String query = "* FROM checklist WHERE ";

            if (operadoraId != null)
                query += "checklist_operadoraId=" + operadoraId + " OR ";

            query += "checklist_operadoraId IS NULL ORDER BY checklist_descricao";

            return LocatorHelper.Instance.ExecuteQuery<CheckList>(query, typeof(CheckList));
        }
    }
}