namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    /// <summary>
    /// Representa a associação de um produto adicional e um beneficiário.
    /// </summary>
    [Serializable]
    [DBTable("adicional_beneficiario")]
    public class AdicionalBeneficiario : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _propostaid;
        Object _adicionalId;
        Object _beneficiarioid;

        String _adicionalDescricao;
        String _adicionalCodTitular;
        Object _adicionalOperadoraId;
        Boolean _adicionalDental;

        #endregion

        #region propriedades 

        [DBFieldInfo("adicionalbeneficiario_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("adicionalbeneficiario_propostaId", FieldType.Single)]
        public Object PropostaID
        {
            get { return _propostaid; }
            set { _propostaid= value; }
        }

        [Joinned("adicional_id")]
        [DBFieldInfo("adicionalbeneficiario_adicionalid", FieldType.Single)]
        public Object AdicionalID
        {
            get { return _adicionalId; }
            set { _adicionalId= value; }
        }

        [DBFieldInfo("adicionalbeneficiario_beneficiarioid", FieldType.Single)]
        public Object BeneficiarioID
        {
            get { return _beneficiarioid; }
            set { _beneficiarioid= value; }
        }

        [Joinned("adicional_descricao")]
        public String AdicionalDescricao
        {
            get { return _adicionalDescricao; }
            set { _adicionalDescricao= value; }
        }

        [Joinned("adicional_codTitular")]
        public String AdicionalCodTitular
        {
            get { return _adicionalCodTitular; }
            set { _adicionalCodTitular= value; }
        }

        [Joinned("adicional_operadoraId")]
        public Object AdicionalOperadoraID
        {
            get { return _adicionalOperadoraId; }
            set { _adicionalOperadoraId= value; }
        }

        [Joinned("adicional_dental")]
        public Boolean AdicionalDental
        {
            get { return _adicionalDental; }
            set { _adicionalDental= value; }
        }

        public Boolean Sim
        {
            get { return _beneficiarioid != null; }
        }

        #endregion

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

        public AdicionalBeneficiario() { }

        public static IList<AdicionalBeneficiario> Carregar(Object propostaId, Object beneficiarioId)
        {
            return Carregar(propostaId, beneficiarioId, null);
        }
        public static IList<AdicionalBeneficiario> Carregar(Object propostaId, Object beneficiarioId, PersistenceManager pm)
        {
            String query = "adicional_beneficiario.*, adicional_descricao, adicional_operadoraId, adicional_codTitular,adicional_dental FROM adicional_beneficiario INNER JOIN adicional ON adicionalbeneficiario_adicionalid=adicional_id WHERE adicionalbeneficiario_propostaId=" + propostaId + " AND adicionalbeneficiario_beneficiarioid=" + beneficiarioId + " ORDER BY adicional_descricao";
            return LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario), pm);
        }

        public static Boolean TemDental(IList<AdicionalBeneficiario> lista)
        {
            if (lista == null) { return false; }

            foreach (AdicionalBeneficiario ab in lista)
            {
                if (ab == null) { continue; }
                if (ab.AdicionalDental) { return true; }
            }

            return false;
        }

        public static Boolean EDental(AdicionalBeneficiario ab)
        {
            if (ab != null && ab.AdicionalDental)
                return true;
            else
                return false;
        }

        public static IList<AdicionalBeneficiario> Carregar(Object contratoADMId, Object planoId, Object propostaId, Object beneficiarioId)
        {
            String query = String.Concat("adicional_beneficiario.*, adicional_id, adicional_descricao ",
                "FROM adicional",
                "  INNER JOIN contratoADM_plano_adicional ON adicional_id=contratoplanoadicional_adicionalid",
                "  LEFT JOIN adicional_beneficiario ON adicionalbeneficiario_adicionalid=adicional_id ");
                
                if(propostaId != null) { query += "AND adicionalbeneficiario_propostaid="+ propostaId; }

            query = String.Concat(query, " AND adicionalbeneficiario_beneficiarioid=", beneficiarioId,
                " WHERE contratoplanoadicional_contratoid=", contratoADMId, " AND contratoplanoadicional_planoid=", planoId, " ORDER BY adicional_descricao");

            return LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario));
        }

        public static AdicionalBeneficiario CarregarParaBeneficiario(Object contratoId, Object beneficiarioId, Object adicionalId, PersistenceManager pm)
        {
            String query = String.Concat("* ",
                "FROM adicional_beneficiario ",
                "  WHERE adicionalbeneficiario_propostaId=", contratoId, " AND adicionalbeneficiario_beneficiarioId=", beneficiarioId, " AND adicionalbeneficiario_adicionalId=", adicionalId);

            IList<AdicionalBeneficiario> lista = LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario));

            if (lista == null)
                return null;
            else
                return lista[0];
        }

        public static IList<AdicionalBeneficiario> Carregar(Object contratoADMId, Object planoId, Object propostaId)
        {
            String query = String.Concat("adicional_beneficiario.*, adicional_id, adicional_descricao ",
                "FROM adicional",
                "  INNER JOIN contratoADM_plano_adicional ON adicional_id=contratoplanoadicional_adicionalid",
                "  LEFT JOIN adicional_beneficiario ON adicionalbeneficiario_adicionalid=adicional_id ");

            if (propostaId != null) { query += "AND adicionalbeneficiario_propostaid=" + propostaId; }

            query = String.Concat(query, " WHERE contratoplanoadicional_contratoid=", contratoADMId, " AND contratoplanoadicional_planoid=", planoId, " ORDER BY adicional_descricao");

            return LocatorHelper.Instance.ExecuteQuery<AdicionalBeneficiario>(query, typeof(AdicionalBeneficiario));
        }
    }
}
