namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("parentesco")]
    public class Parentesco : EntityBase, IPersisteableEntity
    {
        public enum eTipo : int
        {
            Indeterminado = -1,
            Agregado,
            Dependente
        }

        Object _id;
        String _descricao;
        int _tipo;

        #region Propriedades 

        [DBFieldInfo("parentesco_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("parentesco_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("parentesco_tipo", FieldType.Single)]
        public int Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }
        #endregion

        public Parentesco() { _tipo = (int)eTipo.Agregado; }
        public Parentesco(Object id) : this() { _id = id; }

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

        public static IList<Parentesco> CarregarTodos(eTipo tipo)
        {
            String query = "";

            if(tipo != eTipo.Indeterminado)
                query = "* FROM parentesco WHERE parentesco_tipo=" + Convert.ToInt32(tipo) + " ORDER BY parentesco_descricao";
            else
                query = "* FROM parentesco ORDER BY parentesco_descricao";

            return LocatorHelper.Instance.ExecuteQuery<Parentesco>(query, typeof(Parentesco));
        }

        public static IList<Parentesco> CarregarNaoUsadosPor(Object planoId, eTipo tipo)
        {
            String query = "";
            if (tipo != eTipo.Indeterminado)
                query = "* FROM parentesco WHERE parentesco_tipo=" + Convert.ToInt32(tipo) + " AND parentesco_id NOT IN (SELECT planoparentescoagregado_parentescoId FROM plano_parentesco_agregado WHERE planoparentescoagregado_planoId=" + planoId + ") ORDER BY parentesco_descricao";
            else
                query = "* FROM parentesco WHERE parentesco_id NOT IN (SELECT planoparentescoagregado_parentescoId FROM plano_parentesco_agregado WHERE planoparentescoagregado_planoId=" + planoId + ") ORDER BY parentesco_descricao";

            return LocatorHelper.Instance.ExecuteQuery<Parentesco>(query, typeof(Parentesco));
        }
    }
}