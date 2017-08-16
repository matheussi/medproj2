namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [Serializable()]
    [DBTable("plano_parentesco_agregado")]
    public class PlanoParentescoAgregado : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _planoId;
        Object _parentescoId;

        String _planoDescricao;
        String _parentescoDescricao;

        #region Propriedades 

        [DBFieldInfo("planoparentescoagregado_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("planoparentescoagregado_planoId", FieldType.Single)]
        public Object PlanoID
        {
            get { return _planoId; }
            set { _planoId= value; }
        }

        [DBFieldInfo("planoparentescoagregado_parentescoId", FieldType.Single)]
        public Object ParentescoId
        {
            get { return _parentescoId; }
            set { _parentescoId= value; }
        }

        [Joinned("plano_descricao")]
        public String PlanoDescricao
        {
            get { return _planoDescricao; }
            set { _planoDescricao= value; }
        }

        [Joinned("parentesco_descricao")]
        public String ParentescoDescricao
        {
            get { return _parentescoDescricao; }
            set { _parentescoDescricao= value; }
        }

        #endregion

        public PlanoParentescoAgregado() { }

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

        public static IList<PlanoParentescoAgregado> Carregar(Object planoId, Object parentescoId)
        {
            String query = String.Concat("* FROM plano_parentesco_agregado WHERE planoparentescoagregado_planoId=", planoId, " AND planoparentescoagregado_parentescoId=", parentescoId);

            return LocatorHelper.Instance.ExecuteQuery<PlanoParentescoAgregado>(query, typeof(PlanoParentescoAgregado));
        }

        public static IList<PlanoParentescoAgregado> Carregar(Object planoId, Parentesco.eTipo tipo)
        {
            String query = "";

            if (tipo != Parentesco.eTipo.Indeterminado)
            {
                query = String.Concat("plano_parentesco_agregado.*,plano_descricao, parentesco_descricao",
                     " FROM plano_parentesco_agregado ",
                     " INNER JOIN plano ON plano_id=planoparentescoagregado_planoId",
                     " INNER JOIN parentesco ON parentesco_id=planoparentescoagregado_parentescoId AND parentesco_tipo=", Convert.ToInt32(tipo),
                     " WHERE planoparentescoagregado_planoId=", planoId, " ORDER BY parentesco_descricao");
            }
            else
            {
                query = String.Concat("plano_parentesco_agregado.*,plano_descricao, parentesco_descricao",
                     " FROM plano_parentesco_agregado ",
                     " INNER JOIN plano ON plano_id=planoparentescoagregado_planoId",
                     " INNER JOIN parentesco ON parentesco_id=planoparentescoagregado_parentescoId",
                     " WHERE planoparentescoagregado_planoId=", planoId, " ORDER BY parentesco_descricao");
            }

            return LocatorHelper.Instance.ExecuteQuery<PlanoParentescoAgregado>(query, typeof(PlanoParentescoAgregado));
        }
    }
}