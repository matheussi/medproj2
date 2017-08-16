namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("conferencia_itemSaude")]
    public class ConferenciaItemSaude : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _operadoraId;
        String _descricao;

        #region properties

        [DBFieldInfo("itemsaude_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("itemsaude_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("itemsaude_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
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

        public ConferenciaItemSaude() { }
        public ConferenciaItemSaude(Object id) { _id = id; }

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

        public static IList<ConferenciaItemSaude> Carregar(Object operadoraId)
        {
            String query = "* FROM conferencia_itemSaude WHERE ";

            if (operadoraId != null)
                query += "itemsaude_operadoraId=" + operadoraId + " OR ";

            query += "itemsaude_operadoraId IS NULL ORDER BY itemsaude_descricao";

            return LocatorHelper.Instance.ExecuteQuery<ConferenciaItemSaude>(query, typeof(ConferenciaItemSaude));
        }
    }
}