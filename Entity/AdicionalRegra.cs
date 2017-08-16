namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [Serializable()]
    [DBTable("adicional_regra")]
    public class AdicionalRegra : EntityBase, IPersisteableEntity
    {
        public enum eTipo : int
        {
            TitularEQualquerDependente,
            TitularETodosDependentes
        }

        Object _id;
        Object _operadoraId;
        Object _adicionalId;
        int _tipo;

        String _adicionalDescricao;

        #region propriedades 

        [DBFieldInfo("adicionalregra_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("adicionalregra_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("adicionalregra_adicionalId", FieldType.Single)]
        public Object AdicionalID
        {
            get { return _adicionalId; }
            set { _adicionalId= value; }
        }

        [DBFieldInfo("adicionalregra_tipo", FieldType.Single)]
        public int Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [Joinned("adicional_descricao")]
        public String AdicionalDescricao
        {
            get { return _adicionalDescricao; }
            set { _adicionalDescricao= value; }
        }

        public String Resumo
        {
            get
            {
                if (((eTipo)_tipo) == eTipo.TitularEQualquerDependente)
                    return "Titular e QUALQUER agregado ou dependente";
                else
                    return "Titular e TODOS os agregados ou dependentes";
            }
        }

        #endregion

        public AdicionalRegra() { }
        public AdicionalRegra(Object id) { _id = id; }

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

        public static IList<AdicionalRegra> Carregar(Object operadoraId)
        {
            String query = "*, adicional_descricao FROM adicional_regra INNER JOIN adicional ON adicionalregra_adicionalId=adicional_id WHERE adicionalregra_operadoraId=" + operadoraId;
            return LocatorHelper.Instance.ExecuteQuery<AdicionalRegra>(query, typeof(AdicionalRegra));
        }

        public static AdicionalRegra Carregar(Object operadoraId, Object adicionalId)
        {
            String query = "*, adicional_descricao FROM adicional_regra INNER JOIN adicional ON adicionalregra_adicionalId=adicional_id WHERE adicionalregra_operadoraId=" + operadoraId + " AND adicional_id=" + adicionalId;
            IList<AdicionalRegra> lista = LocatorHelper.Instance.ExecuteQuery<AdicionalRegra>(query, typeof(AdicionalRegra));
            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }
    }
}
