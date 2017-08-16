namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    /// <summary>
    /// 
    /// </summary>
    [DBTable("contratoADM_plano_adicional")]
    public class ContratoADMPlanoAdicional : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _contratoId;
        Object _planoId;
        Object _adicionalId;
        DateTime _data;

        String _adicionalDescricao;

        #region propriedades 

        [DBFieldInfo("contratoplanoadicional_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("contratoplanoadicional_contratoid", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId= value; }
        }

        [DBFieldInfo("contratoplanoadicional_planoid", FieldType.Single)]
        public Object PlanoID
        {
            get { return _planoId; }
            set { _planoId= value; }
        }

        [DBFieldInfo("contratoplanoadicional_adicionalid", FieldType.Single)]
        public Object AdicionalID
        {
            get { return _adicionalId; }
            set { _adicionalId= value; }
        }

        [DBFieldInfo("contratoplanoadicional_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [Joinned("adicional_descricao")]
        public String AdicionalDescricao
        {
            get { return _adicionalDescricao; }
            set { _adicionalDescricao= value; }
        }

        #endregion

        public ContratoADMPlanoAdicional() { _data = DateTime.Now; }
        public ContratoADMPlanoAdicional(Object id) : this() { this._id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        public void Remover()
        {
            base.Remover(this);
        }

        #endregion

        public static IList<ContratoADMPlanoAdicional> Carregar(Object contratoId, Object planoId)
        {
            String query = String.Concat("contratoADM_plano_adicional.*, adicional_descricao ",
                "FROM contratoADM_plano_adicional INNER JOIN adicional ",
                "   ON contratoplanoadicional_adicionalid = adicional_id ", 
                " WHERE contratoplanoadicional_contratoid=", contratoId, " AND contratoplanoadicional_planoid =",
                planoId, " ORDER BY adicional_descricao");

            return LocatorHelper.Instance.ExecuteQuery
                <ContratoADMPlanoAdicional>(query, typeof(ContratoADMPlanoAdicional));
        }

        public static Boolean ExisteRelacionamento(Object contratoAdmId, Object planoId, Object adicionalId, PersistenceManager pm)
        {
            String query = String.Concat("SELECT contratoplanoadicional_id ",
                "FROM contratoADM_plano_adicional INNER JOIN adicional ",
                "   ON contratoplanoadicional_adicionalid = adicional_id ",
                " WHERE contratoplanoadicional_contratoId=", contratoAdmId,
                " AND contratoplanoadicional_adicionalid=", adicionalId, 
                " AND contratoplanoadicional_planoid =", planoId);

            Object ret = LocatorHelper.Instance.ExecuteScalar(query, null, null, pm);

            if (ret == null || ret == DBNull.Value)
                return false;
            else
                return true;
        }
    }
}
