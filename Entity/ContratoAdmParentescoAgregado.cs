namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [Serializable()]
    [DBTable("contratoADM_parentesco_agregado")]
    public class ContratoADMParentescoAgregado : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _contratoAdmId;
        //Object _parentescoId;
        String _parentescoDescricao;
        String _parentescoCodigo;
        int _parentescoTipo;

        String _contratoAdmDescricao;

        #region Propriedades 

        [DBFieldInfo("contratoAdmparentescoagregado_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("contratoAdmparentescoagregado_contratoAdmId", FieldType.Single)]
        public Object ContratoAdmID
        {
            get { return _contratoAdmId; }
            set { _contratoAdmId= value; }
        }

        //[DBFieldInfo("contratoAdmparentescoagregado_parentescoId", FieldType.Single)]
        //public Object ParentescoId
        //{
        //    get { return _parentescoId; }
        //    set { _parentescoId= value; }
        //}

        [DBFieldInfo("contratoAdmparentescoagregado_parentescoDescricao", FieldType.Single)]
        public String ParentescoDescricao
        {
            get { return _parentescoDescricao; }
            set { _parentescoDescricao= value; }
        }

        [DBFieldInfo("contratoAdmparentescoagregado_parentescoCodigo", FieldType.Single)]
        public String ParentescoCodigo
        {
            get { return _parentescoCodigo; }
            set { _parentescoCodigo= value; }
        }

        [DBFieldInfo("contratoAdmparentescoagregado_parentescoTipo", FieldType.Single)]
        public int ParentescoTipo
        {
            get { return _parentescoTipo; }
            set { _parentescoTipo= value; }
        }

        [Joinned("contratoAdm_descricao")]
        public String ContratoAdmDescricao
        {
            get { return _contratoAdmDescricao; }
            set { _contratoAdmDescricao= value; }
        }

        //[Joinned("parentesco_descricao")]
        //public String ParentescoDescricao
        //{
        //    get { return _parentescoDescricao; }
        //    set { _parentescoDescricao= value; }
        //}

        #endregion

        public ContratoADMParentescoAgregado() { }
        public ContratoADMParentescoAgregado(Object id) { _id = id; }

        #region Métodos EntityBase 

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

        public static IList<ContratoADMParentescoAgregado> Carregar(Object contratoAdmId, Object parentescoId)
        {
            String query = String.Concat("* FROM contratoAdm_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=", contratoAdmId, " AND contratoAdmparentescoagregado_parentescoId=", parentescoId);

            return LocatorHelper.Instance.ExecuteQuery<ContratoADMParentescoAgregado>(query, typeof(ContratoADMParentescoAgregado));
        }

        public static ContratoADMParentescoAgregado Carregar(Object contratoAdmId, String parentescoDescricao, PersistenceManager pm)
        {
            String query = String.Concat("TOP 1 * FROM contratoAdm_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=", contratoAdmId, " AND contratoAdmparentescoagregado_parentescoDescricao=@contratoAdmparentescoagregado_parentescoDescricao");

            String[] nm = new String[] { "@contratoAdmparentescoagregado_parentescoDescricao" };
            String[] vl = new String[] { parentescoDescricao };

            IList<ContratoADMParentescoAgregado> lista = 
                LocatorHelper.Instance.ExecuteParametrizedQuery<ContratoADMParentescoAgregado>(query, nm, vl, typeof(ContratoADMParentescoAgregado), pm);

            if (lista == null)
                return null;
            else
                return lista[0];
        }

        public static IList<ContratoADMParentescoAgregado> Carregar(Object contratoAdmId, Parentesco.eTipo tipo)
        {
            String query = "";

            if (tipo != Parentesco.eTipo.Indeterminado)
            {
                query = String.Concat("contratoAdm_parentesco_agregado.*,contratoAdm_descricao",
                     " FROM contratoAdm_parentesco_agregado ",
                     " INNER JOIN contratoAdm ON contratoAdm_id=contratoAdmparentescoagregado_contratoAdmId",
                     //" INNER JOIN parentesco ON parentesco_id=contratoAdmparentescoagregado_parentescoId AND parentesco_tipo=", Convert.ToInt32(tipo),
                     " WHERE contratoAdmparentescoagregado_parentescotipo=", Convert.ToInt32(tipo), " AND contratoAdmparentescoagregado_contratoAdmId=", contratoAdmId, " ORDER BY contratoAdmparentescoagregado_parentescoDescricao");
            }
            else
            {
                query = String.Concat("contratoAdm_parentesco_agregado.*,contratoAdm_descricao",
                     " FROM contratoAdm_parentesco_agregado ",
                     " INNER JOIN contratoAdm ON contratoAdm_id=contratoAdmparentescoagregado_contratoAdmId",
                     //" INNER JOIN parentesco ON parentesco_id=contratoAdmparentescoagregado_parentescoId",
                     " WHERE contratoAdmparentescoagregado_contratoAdmId=", contratoAdmId, " ORDER BY contratoAdmparentescoagregado_parentescoDescricao"); //contratoAdmparentescoagregado_parentescotipo=", Convert.ToInt32(tipo), " AND
            }

            return LocatorHelper.Instance.ExecuteQuery<ContratoADMParentescoAgregado>(query, typeof(ContratoADMParentescoAgregado));
        }

        public static Boolean Duplicado(ContratoADMParentescoAgregado item)
        {
            String qry = String.Concat("SELECT DISTINCT(contratoAdmparentescoagregado_id) ",
                "   FROM contratoADM_parentesco_agregado ",
                "   WHERE ",
                "       contratoAdmparentescoagregado_contratoAdmId=", item.ContratoAdmID, " AND ",
//              "       contratoAdmparentescoagregado_parentescoTipo=", item.ParentescoTipo, " AND ",
                "       contratoAdmparentescoagregado_parentescoDescricao=@Descricao");

            if (item.ID != null)
            {
                qry += " AND contratoAdmparentescoagregado_id <> " + item.ID;
            }

            String[] pNames  = new String[] { "@Descricao" };
            String[] pValues = new String[] { item.ParentescoDescricao };

            Object returned = LocatorHelper.Instance.ExecuteScalar(qry, pNames, pValues);

            if (returned == null || returned == DBNull.Value)
                return false;
            else
                return true;
        }
    }
}