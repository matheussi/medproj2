namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    /// <summary>
    /// Representa um contrato administrativo (Estipulante x Operadora)
    /// </summary>
    [Serializable]
    [DBTable("contratoADM")]
    public class ContratoADM : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _operadoraId;
        Object _estipulanteId;
        Object _tabelaComissionamentoAtivaId;
        String _descricao;
        String _contratoSaude;
        String _contratoDental;
        String _numero;
        DateTime _data;
        Boolean _ativo;
        String _codFilial;
        String _codUnidade;
        String _codAdministradora;

        String _operadoraDescricao;
        String _estipulanteDescricao;
        Decimal _totalNormal;

        Object _contratoGrupoId;

        #endregion

        #region propriedades 

        [DBFieldInfo("contratoadm_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("contratoadm_tabelaComissionamentoAtivaId", FieldType.Single)]
        public Object TabelaComissionamentoAtivaID
        {
            get { return _tabelaComissionamentoAtivaId; }
            set { _tabelaComissionamentoAtivaId= value; }
        }

        [DBFieldInfo("contratoadm_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("contratoadm_contratoSaude", FieldType.Single)]
        public String ContratoSaude
        {
            get { return _contratoSaude; }
            set { _contratoSaude= value; }
        }

        [DBFieldInfo("contratoadm_contratoDental", FieldType.Single)]
        public String ContratoDental
        {
            get { return _contratoDental; }
            set { _contratoDental= value; }
        }

        [DBFieldInfo("contratoadm_numero", FieldType.Single)]
        public String Numero
        {
            get { return _numero; }
            set { _numero= value; }
        }

        [DBFieldInfo("contratoadm_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("contratoadm_estipulanteId", FieldType.Single)]
        public Object EstipulanteID
        {
            get { return _estipulanteId; }
            set { _estipulanteId= value; }
        }

        [DBFieldInfo("contratoadm_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo= value; }
        }

        [DBFieldInfo("contratoadm_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [DBFieldInfo("contratoadm_codFilial", FieldType.Single)]
        public String CodFilial
        {
            get { return _codFilial; }
            set { _codFilial= value; }
        }

        [DBFieldInfo("contratoadm_codUnidade", FieldType.Single)]
        public String CodUnidade
        {
            get { return _codUnidade; }
            set { _codUnidade= value; }
        }

        [DBFieldInfo("contratoadm_codAdministradora", FieldType.Single)]
        public String CodAdministradora
        {
            get { return _codAdministradora; }
            set { _codAdministradora= value; }
        }

        [Joinned("operadora_nome")]
        public String OperadoraDescricao
        {
            get { return _operadoraDescricao; }
            set { _operadoraDescricao= value; }
        }

        [Joinned("estipulante_descricao")]
        public String EstipulanteDescricao
        {
            get { return _estipulanteDescricao; }
            set { _estipulanteDescricao= value; }
        }

        [Joinned("TotalNormal")]
        public Decimal TotalNormal
        {
            get { return _totalNormal; }
            set { _totalNormal= value; }
        }

        /// <summary>
        /// ID do grupo de comissionamento ao qual este contrato adm pertence.
        /// </summary>
        [Joinned("contratoadmgrupo_id")]
        public Object ContratoGrupoID
        {
            get { return _contratoGrupoId; }
            set { _contratoGrupoId= value; }
        }

        [Joinned("contratoadmgrupo_grupoId")]
        public Object ContratoGrupo_GrupoID
        {
            get { return _contratoGrupoId; }
            set { _contratoGrupoId = value; }
        }

        public String DescricaoCodigoSaudeDental
        {
            get
            {
                String value = _descricao;

                if (!String.IsNullOrEmpty(_contratoDental) || !String.IsNullOrEmpty(_contratoSaude))
                {
                    value = String.Concat(value, " (");

                    if (!String.IsNullOrEmpty(_contratoDental))
                        value = String.Concat(value, "Dental: ", _contratoDental);

                    if (!String.IsNullOrEmpty(_contratoSaude))
                    {
                        if (!String.IsNullOrEmpty(_contratoDental))
                            value = String.Concat(value, " - ");

                        value = String.Concat(value, "Saúde: ", _contratoSaude);
                    }

                    value = String.Concat(value, ")");
                }

                return value;
            }
        }

        #endregion

        public Estipulante Estipulante
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public ContratoADM() { _ativo = true; _data = DateTime.Now; _totalNormal = Decimal.Zero; }
        public ContratoADM(Object id) : this() { this._id = id; }

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

        public static IList<ContratoADM> CarregarTodos()
        {
            String query = String.Concat("contratoADM.*, operadora_nome ",
                "FROM contratoADM ",
                "INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                //"INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                "ORDER BY operadora_nome, contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> CarregarTodos(Object estipulanteId, Object operadoraId, Boolean somenteAtivos)
        {
            String _ativoCond = " and contratoadm_ativo=1 ";
            if (!somenteAtivos) { _ativoCond = ""; }

            String query = String.Concat("contratoADM.*, operadora_nome,  estipulante_descricao ",
                "FROM contratoADM ",
                "INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                "WHERE contratoadm_operadoraId=",
                operadoraId, " AND contratoadm_estipulanteId=", estipulanteId, _ativoCond);

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> CarregarPorTabelaComissionamento(Object tabelaComissionamentoModeloId)
        {
            String query = String.Concat("contratoADM.*, operadora_nome,contratoadmgrupo_grupoId ",
                "   FROM contratoADM ",
                "       INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "       INNER JOIN contratoAdmGrupo ON contratoadmgrupo_contratoAdmId = contratoadm_id ",
                "   WHERE contratoadmgrupo_tabelaId=", tabelaComissionamentoModeloId,
                "   ORDER BY operadora_nome, contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        [Obsolete("Em desuso", true)]
        public static IList<ContratoADM> _CarregarTodos(Object grupoId)
        {
            String query = String.Concat("SUM(comissaomodeloitem_percentual) as TotalNormal, operadora_nome, contratoadm_id, contratoadm_tabelaComissionamentoAtivaId, contratoadm_tabelaReajusteAtualId, contratoadm_descricao,contratoadm_numero, contratoadm_operadoraId, contratoadm_estipulanteId, contratoadm_ativo, contratoadm_data ",
                "   FROM contratoADM ",
                "       INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "       LEFT JOIN comissao_modelo_item ON contratoadm_id=comissaomodeloitem_contratoid AND comissaomodeloitem_comissaomodeloid=", grupoId,
                "   GROUP BY operadora_nome, contratoadm_id, contratoadm_tabelaComissionamentoAtivaId, contratoadm_tabelaReajusteAtualId, contratoadm_descricao,contratoadm_numero, contratoadm_operadoraId, contratoadm_estipulanteId, contratoadm_ativo, contratoadm_data ",
                "   ORDER BY operadora_nome, contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static Object CarregarID(String numero, Object operadoraId, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@numero" };
            String[] paramVl = new String[] { numero };
            return LocatorHelper.Instance.ExecuteScalar("SELECT contratoadm_id FROM contratoADM WHERE contratoadm_numero=@numero AND contratoadm_operadoraId=" + operadoraId, paramNm, paramVl, pm);
        }

        public static Object CarregarID(String numero, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@numero" };
            String[] paramVl = new String[] { numero };
            return LocatorHelper.Instance.ExecuteScalar("SELECT contratoadm_id FROM contratoADM WHERE contratoadm_numero=@numero", paramNm, paramVl, pm);
        }

        public static ContratoADM Carregar(String numero, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@numero" };
            String[] paramVl = new String[] { numero };
            IList<ContratoADM> lista = LocatorHelper.Instance.ExecuteParametrizedQuery<ContratoADM>("SELECT TOP 1 * FROM contratoADM WHERE contratoadm_numero=@numero", paramNm, paramVl, typeof(ContratoADM));

            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }

        public static IList<ContratoADM> Carregar(Object operadoraId)
        {
            String query = String.Concat("contratoADM.*, operadora_nome,  estipulante_descricao ",
                "FROM contratoADM ",
                "INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                "WHERE contratoadm_operadoraId=",
                operadoraId, " ORDER BY contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> Carregar(String[] operadoraIDs, String[] estipulantesIDs)
        {
            String estipulanteCond = "";
            if (estipulantesIDs != null && estipulantesIDs.Length > 0)
                estipulanteCond = String.Concat(" AND contratoadm_estipulanteId IN (", String.Join(",", estipulantesIDs), ") ");

            String query = String.Concat("contratoADM.*, operadora_nome,  estipulante_descricao ",
                "FROM contratoADM ",
                "   INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "   INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                " WHERE ",
                //"   contratoadm_ativo=1 AND ",
                "   contratoadm_operadoraId IN(", String.Join(",", operadoraIDs), ") ", estipulanteCond,
                " ORDER BY contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> Carregar(Object operadoraId, Boolean ativo)
        {
            String _ativo = "1";
            if (!ativo) { _ativo = "0"; }

            String query = String.Concat("contratoADM.*, operadora_nome,  estipulante_descricao ",
                "FROM contratoADM ",
                "INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                "WHERE contratoadm_operadoraId=",
                operadoraId, " AND contratoadm_ativo=", _ativo, " ORDER BY contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> Carregar(Object operadoraId, String param1, String param2, String param3, Boolean ativo)
        {
            String _ativo = "1";
            if (!ativo) { _ativo = "0"; }

            String query = String.Concat("contratoADM.*, operadora_nome,  estipulante_descricao ",
                "FROM contratoADM ",
                "INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                "WHERE contratoadm_operadoraId=",
                operadoraId, 
                " and contratoadm_descricao like '%", param1,
                "%' and contratoadm_descricao like '%", param2, 
                "%' and contratoadm_descricao like '%", param3, "%' AND contratoadm_ativo=", _ativo, " ORDER BY contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> Carregar(Object estipulanteId, Object operadoraId)
        {
            String query = String.Concat("contratoADM.*, operadora_nome,  estipulante_descricao ",
                "FROM contratoADM ",
                "INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                "WHERE contratoadm_operadoraId=",
                operadoraId, " AND contratoadm_estipulanteId=", estipulanteId);

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> Carregar(Object estipulanteId, Object operadoraId, Boolean ativo)
        {
            String _ativo = "1";
            if (!ativo) { _ativo = "0"; }

            String query = String.Concat("contratoADM.*, operadora_nome,  estipulante_descricao ",
                "FROM contratoADM ",
                "INNER JOIN operadora ON operadora_id=contratoadm_operadoraId ",
                "INNER JOIN estipulante ON estipulante_id=contratoadm_estipulanteId ",
                "WHERE contratoadm_operadoraId=",
                operadoraId, " AND contratoadm_estipulanteId=", estipulanteId, " AND contratoadm_ativo=", _ativo);

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static void SetaTabelaComissionamentoAutal(Object contratoId, Object tabelaComissaoId, PersistenceManager pm)
        {
            String command = "UPDATE contratoadm SET contratoadm_tabelaComissionamentoAtivaId=" + tabelaComissaoId + " WHERE contratoadm_id=" + contratoId;
            NonQueryHelper.Instance.ExecuteNonQuery(command, pm);
        }

        public static Boolean ExisteNumero(Object contratoId, String numero, Object estipulanteId, Object operadoraId)
        {
            String qry = "SELECT COUNT(*) FROM contratoADM WHERE contratoadm_numero='" + numero + "' AND contratoadm_estipulanteId=" + estipulanteId + " AND contratoadm_operadoraId=" + operadoraId;

            if (contratoId != null)
            {
                qry += " AND contratoadm_id <> " + contratoId;
            }

            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, null, null);
            if (ret == null || ret == DBNull.Value || Convert.ToInt32(ret) == 0)
                return false;
            else
                return true;
        }

        public static Boolean ExisteDescricao(Object contratoId, String descricao, Object operadoraId)
        {
            String qry = "SELECT COUNT(*) FROM contratoADM WHERE contratoadm_operadoraId=" + operadoraId + " AND contratoadm_descricao=@Descricao";

            if (contratoId != null)
            {
                qry += " AND contratoadm_id <> " + contratoId;
            }

            String[] paramnames = new String[] { "@Descricao" };
            String[] paramvalue = new String[] { descricao };

            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, paramnames, paramvalue);
            if (ret == null || ret == DBNull.Value || Convert.ToInt32(ret) == 0)
                return false;
            else
                return true;
        }

        public void SetaTabelaComissionamentoAutal(Object contratoId, PersistenceManager pm)
        {
            ContratoADM.SetaTabelaReajusteAutal(this._id, contratoId, pm);
        }

        public static void SetaTabelaReajusteAutal(Object contratoId, Object tabelaReajusteId, PersistenceManager pm)
        {
            String command = "UPDATE contratoadm SET contratoadm_tabelaReajusteAtualId=" + tabelaReajusteId + " WHERE contratoadm_id=" + contratoId;
            NonQueryHelper.Instance.ExecuteNonQuery(command, pm);
        }

        public void SetaTabelaReajusteAutal(Object contratoId, PersistenceManager pm)
        {
            ContratoADM.SetaTabelaReajusteAutal(this._id, contratoId, pm);
        }
    }

    [DBTable("contratoAdmGrupo")]
    public class ContratoAdmGrupoComissionamento : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _contratoAdmId;
        Object _grupoId;
        Object _tabelaId;

        #region propriedades 

        [DBFieldInfo("contratoadmgrupo_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }
        [DBFieldInfo("contratoadmgrupo_contratoAdmId", FieldType.Single)]
        public Object ContratoAdmID
        {
            get { return _contratoAdmId; }
            set { _contratoAdmId= value; }
        }
        [DBFieldInfo("contratoadmgrupo_grupoId", FieldType.Single)]
        public Object GrupoID
        {
            get { return _grupoId; }
            set { _grupoId= value; }
        }
        [DBFieldInfo("contratoadmgrupo_tabelaId", FieldType.Single)]
        public Object TabelaID
        {
            get { return _tabelaId; }
            set { _tabelaId= value; }
        }
        #endregion

        public ContratoAdmGrupoComissionamento() { }
        public ContratoAdmGrupoComissionamento(Object id) { _id = id; }

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

        public static void Remover(Object contratoAdmId, Object grupoId)
        {
            String cmd = String.Concat("DELETE FROM contratoAdmGrupo WHERE contratoadmgrupo_contratoAdmId=", contratoAdmId, " AND contratoadmgrupo_grupoId=", grupoId);
            NonQueryHelper.Instance.ExecuteNonQuery(cmd, null);
        }

        public static void Remover(List<String> contratoAdmIds, Object grupoId)
        {
            String cmd = String.Concat("DELETE FROM contratoAdmGrupo WHERE contratoadmgrupo_contratoAdmId IN (", String.Join(",", contratoAdmIds.ToArray()), ") AND contratoadmgrupo_grupoId=", grupoId);

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm);

                StringBuilder sb = new StringBuilder();
                foreach (String contratoId in contratoAdmIds)
                {
                    sb.Append("DELETE FROM comissaoGrupo_contratoExcluido WHERE grupoId=");
                    sb.Append(grupoId); sb.Append(" AND contratoAdmId="); sb.Append(contratoId);
                    sb.Append("; INSERT INTO comissaoGrupo_contratoExcluido(grupoId,contratoAdmId) VALUES (");
                    sb.Append(grupoId); sb.Append(","); sb.Append(contratoId); sb.Append("); ");
                }

                NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }

        #endregion

        [Obsolete("Em desuso devido falha lógica.", false)]
        public static IList<ContratoADM> CarregarDisponiveis(Object grupoId, Object tabelaId)
        {
            String query = String.Concat("contratoADM.*, contratoadmgrupo_id, operadora_nome ",
                "FROM contratoADM ",
                "   INNER JOIN operadora ON operadora_id = contratoadm_operadoraId ",
                "   LEFT JOIN contratoAdmGrupo ON contratoadmgrupo_contratoAdmId = contratoadm_id AND contratoadmgrupo_tabelaId=", tabelaId, " ",
                "WHERE contratoadm_ativo=1 AND (contratoadmgrupo_grupoId IS NULL OR contratoadmgrupo_grupoId=", grupoId, ") AND contratoadm_id NOT IN (SELECT contratoAdmId FROM comissaoGrupo_contratoExcluido WHERE grupoId=", grupoId, ") ",
                "ORDER BY operadora_nome, contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }

        public static IList<ContratoADM> Carregar(Object grupoId, Object tabelaId)
        {
            String query = String.Concat("contratoADM.*, contratoadmgrupo_id, operadora_nome ",
                "FROM contratoADM ",
                "   INNER JOIN operadora ON operadora_id = contratoadm_operadoraId ",
                "   INNER JOIN contratoAdmGrupo ON contratoadmgrupo_contratoAdmId = contratoadm_id ", //AND contratoadmgrupo_tabelaId=", tabelaId, " ",
                "WHERE contratoadm_ativo=1 AND contratoadmgrupo_grupoId=", grupoId, " AND contratoadmgrupo_tabelaId=", tabelaId,
                " ORDER BY operadora_nome, contratoadm_descricao");

            return LocatorHelper.Instance.ExecuteQuery<ContratoADM>(query, typeof(ContratoADM));
        }
    }
}