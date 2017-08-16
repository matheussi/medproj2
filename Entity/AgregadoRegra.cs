namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [Serializable()]
    [DBTable("agregado_regra")]
    public class AgregadoRegra : EntityBase, IPersisteableEntity
    {
        public enum eTipoLimite : int
        {
            LimiteDeIdade,
            LimiteDiasDeContrato
        }

        Object _id;
        Object _contratoAdmId;
        int _tipo;
        int _valorLimite;
        int _tipoLimite;

        #region propriedades 

        [DBFieldInfo("agregadoregra_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("agregadoregra_contratoAdmId", FieldType.Single)]
        public Object ContratoAdmID
        {
            get { return _contratoAdmId; }
            set { _contratoAdmId= value; }
        }

        [DBFieldInfo("agregadoregra_tipo", FieldType.Single)]
        public int TipoAgregado
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [DBFieldInfo("agregadoregra_tipoLimite", FieldType.Single)]
        public int TipoLimite
        {
            get { return _tipoLimite; }
            set { _tipoLimite= value; }
        }

        [DBFieldInfo("agregadoregra_valorLimite", FieldType.Single)]
        public int ValorLimite
        {
            get { return _valorLimite; }
            set { _valorLimite= value; }
        }

        public String Resumo
        {
            get { return ToString(); }
        }

        #endregion

        public AgregadoRegra() { }
        public AgregadoRegra(Object id) { _id = id; }

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

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (((Parentesco.eTipo)_tipo) == Parentesco.eTipo.Agregado)
                sb.Append("AGREGADOS");
            else
                sb.Append("DEPENDENTES");

            if (((eTipoLimite)_tipoLimite) == eTipoLimite.LimiteDeIdade)
            {
                sb.Append(" - Até ");
                sb.Append(_valorLimite);
                sb.Append(" anos");
            }
            else
            {
                sb.Append(" - Até ");
                sb.Append(_valorLimite);
                sb.Append(" dias passados da assinatura do contrato");
            }

 	         return sb.ToString();
        }

        public static IList<AgregadoRegra> Carregar(Object contratoId)
        {
            String query = "* FROM agregado_regra WHERE agregadoregra_contratoAdmId=" + contratoId;

            return LocatorHelper.Instance.ExecuteQuery<AgregadoRegra>(query, typeof(AgregadoRegra));
        }

        public static IList<AgregadoRegra> Carregar(Object contratoAdmId, Parentesco.eTipo tipo)
        {
            String query = "* FROM agregado_regra WHERE agregadoregra_contratoAdmId=" + contratoAdmId + " AND agregadoregra_tipo=" + Convert.ToInt32(tipo);

            return LocatorHelper.Instance.ExecuteQuery<AgregadoRegra>(query, typeof(AgregadoRegra));
        }
    }
}