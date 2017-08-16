namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("operadora")]
    public class Operadora : EntityBase, IPersisteableEntity
    {
        public enum StatusOperadora : int
        {
            Ativa,
            Inativa
        }

        #region fields 

        Object _id;
        Object _tabelaReajusteAtualId;
        String _nome;
        String _cnpj;
        String _email;
        String _ddd;
        String _fone;
        String _ramal;
        String _contato;
        Boolean _inativa;
        int _tamanhoMaximoLogradouroBeneficiario;
        int _diaPagamento;
        int _diaRecebimento;
        Boolean _permiteReativacao;
        Boolean _enviaCartaAviso;
        String _mensagemRemessa;

        Endereco _endereco;

        #endregion

        #region propriedades 

        [DBFieldInfo("operadora_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("operadora_tabelaReajusterAtualId", FieldType.Single)]
        public Object TabelaReajusteAtualID
        {
            get { return _tabelaReajusteAtualId; }
            set { _tabelaReajusteAtualId= value; }
        }

        [DBFieldInfo("operadora_cnpj", FieldType.Single)]
        public String CNPJ
        {
            get { return _cnpj; }
            set { _cnpj= value; }
        }

        [DBFieldInfo("operadora_nome", FieldType.Single)]
        public String Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        [DBFieldInfo("operadora_email", FieldType.Single)]
        public String Email
        {
            get { return ToLower(_email); }
            set { _email= value; }
        }

        [DBFieldInfo("operadora_ddd", FieldType.Single)]
        public String DDD
        {
            get { return _ddd; }
            set { _ddd = value; }
        }

        [DBFieldInfo("operadora_fone", FieldType.Single)]
        public String Fone
        {
            get { return _fone; }
            set { _fone= value; }
        }

        public String FFone
        {
            get
            {
                return base.FormataTelefone(_fone);
            }
        }

        [DBFieldInfo("operadora_ramal", FieldType.Single)]
        public String Ramal
        {
            get { return _ramal; }
            set { _ramal = value; }
        }

        [DBFieldInfo("operadora_contato", FieldType.Single)]
        public String Contato
        {
            get { return _contato; }
            set { _contato= value; }
        }

        [DBFieldInfo("operadora_inativa", FieldType.Single)]
        public Boolean Inativa
        {
            get { return _inativa; }
            set { _inativa= value; }
        }

        [DBFieldInfo("operadora_diaPagamento", FieldType.Single)]
        public int DiaPagamento
        {
            get { return _diaPagamento; }
            set { _diaPagamento= value; }
        }

        [DBFieldInfo("operadora_diaRecebimento", FieldType.Single)]
        public int DiaRecebimento
        {
            get { return _diaRecebimento; }
            set { _diaRecebimento= value; }
        }

        [DBFieldInfo("operadora_tamanhoMaximoLogradouroBeneficiario", FieldType.Single)]
        public int TamanhoMaximoLogradouroBeneficiario
        {
            get { return _tamanhoMaximoLogradouroBeneficiario; }
            set { _tamanhoMaximoLogradouroBeneficiario= value; }
        }

        [DBFieldInfo("operadora_permiteReativacao", FieldType.Single)]
        public Boolean PermiteReativacao
        {
            get { return _permiteReativacao; }
            set { _permiteReativacao= value; }
        }

        [DBFieldInfo("operadora_enviaCartaAviso", FieldType.Single)]
        public Boolean EnviaCartaDeAviso
        {
            get { return _enviaCartaAviso; }
            set { _enviaCartaAviso= value; }
        }

        public Endereco Endereco
        {
            get { return _endereco; }
            set { _endereco= value; }
        }

        [DBFieldInfo("operadora_mensagemRemessa", FieldType.Single)]
        public String MensagemRemessa
        {
            get { return _mensagemRemessa; }
            set { _mensagemRemessa = value; }
        }

        #endregion

        public Plano Plano
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        public TabelaReajuste TabelaReajuste
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        public ItemDeclaracaoSaude ItemDeclaracaoSaude
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        public RegraDeclaracaoSaude RegraDeclaracaoSaude
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        public CalendarioAdmissaoVigencia CalendarioAdmissaoVigencia
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        public ComissionamentoOperadora ComissionamentoOperadora
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        public ContratoADM ContratoADM
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        public Contato Contato1
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }


        /// <summary>
        /// ID da Unimed.
        /// </summary>
        public static Object UnimedID
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["OperadoraUnimedID"];
            }
        }

        /// <summary>
        /// ID da Unimed Fortaleza.
        /// </summary>
        public static Object UnimedFortalezaID
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["OperadoraUnimedFortalezaID"];
            }
        }

        /// <summary>
        /// IDs de operadoras Amil.
        /// </summary>
        public static String[] AmilIDs
        {
            get
            {
                return new string[] { };// System.Configuration.ConfigurationManager.AppSettings["OperadoraAmilID"].Split(';');
            }
        }

        public static String[] AmilIDsSemCodPrcJR
        {
            get
            {
                return new string[] { };//System.Configuration.ConfigurationManager.AppSettings["OperadoraAmilID_SemCodJR"].Split(';');
            }
        }

        /// <summary>
        /// Array de prcs válidos para propostas Amil.
        /// </summary>
        public static String[] AmilPRCs
        {
            get
            {
                return new string[] { };//System.Configuration.ConfigurationManager.AppSettings["OperadoraAmilPRCs"].Split(';');
            }
        }

        /// <summary>
        /// Checa se um PRC é válido para o domínio Amil.
        /// </summary>
        public static Boolean ValidaPRCAmil(String prc)
        {
            if (String.IsNullOrEmpty(prc)) { return false; }

            String[] prcsValidos = AmilPRCs;
            if (prcsValidos != null && prcsValidos.Length > 0)
            {
                foreach (String prcValido in prcsValidos)
                {
                    if (prcValido.ToUpper().Equals(prc.ToUpper())) { return true; }
                }
            }

            return false;
        }

        /// <summary>
        /// Verifica se o ID da Operadora é o da Unimed.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>True se for Unimed e False se for outra Operadora.</returns>
        public static Boolean IsUnimed(Object OperadoraID)
        {
            if (OperadoraID != null)
            {
                Int32 intOperadoraID = -1;

                if (Int32.TryParse(OperadoraID.ToString(), out intOperadoraID))
                    return (Convert.ToInt32(UnimedID) == intOperadoraID);
                else
                    throw new ArgumentException("ID da Operadora é inválido");
            }
            else
                throw new ArgumentNullException("ID da Operadora não pode ser nulo.");
        }

        /// <summary>
        /// Verifica se a operadora é Amil através do seu ID.
        /// </summary>
        /// <param name="OperadoraID">ID da operadora.</param>
        /// <returns>True se Amil, do contrário False.</returns>
        public static Boolean IsAmil(Object OperadoraID)
        {
            if (OperadoraID != null)
            {
                String[] ids = AmilIDs;
                if (ids == null || ids.Length == 0) { return false; }

                foreach (String id in ids)
                {
                    if (Convert.ToString(OperadoraID) == id) { return true; }
                }

                return false;
            }
            else
                throw new ArgumentNullException("ID da Operadora não pode ser nulo.");
        }

        /// <summary>
        /// Dado um id de operadora, checa se para essa operadora, durante a exportacao, deve-se remover o
        /// prefixo JR do código PRC.
        /// </summary>
        public static Boolean IsAmil_SemCodPrcJR_NaExportacao(Object OperadoraID)
        {
            if (OperadoraID != null)
            {
                String[] ids = AmilIDsSemCodPrcJR;
                if (ids == null || ids.Length == 0) { return false; }

                foreach (String id in ids)
                {
                    if (Convert.ToString(OperadoraID) == id) { return true; }
                }

                return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Verifica se o ID da Operadora é o da Unimed Fortaleza.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>True se for Unimed Fortaleza e False se for outra Operadora.</returns>
        public static Boolean IsUnimedFortaleza(Object OperadoraID)
        {
            if (OperadoraID != null)
            {
                Int32 intOperadoraID = -1;

                if (Int32.TryParse(OperadoraID.ToString(), out intOperadoraID))
                    return (Convert.ToInt32(UnimedFortalezaID) == intOperadoraID);
                else
                    throw new ArgumentException("ID da Operadora é inválido");
            }
            else
                throw new ArgumentNullException("ID da Operadora não pode ser nulo.");
        }

        public Operadora(Object id) : this() { _id = id; }
        public Operadora() { _inativa = false; _endereco = new Endereco(); }

        #region persistence methods 

        public void Salvar()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.TransactionContext();

            try
            {
                pm.Save(this);

                this._endereco.DonoId = this.ID;
                this._endereco.DonoTipo = Convert.ToInt32(Endereco.TipoDono.Operadora);
                pm.Save(this._endereco);

                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                pm = null;
                throw ex;
            }
        }

        public void Remover()
        {
            this.Carregar();
            PersistenceManager pm = new PersistenceManager();
            pm.TransactionContext();

            IList<Adicional> adicionais = LocatorHelper.Instance.ExecuteQuery<Adicional>("adicional_id FROM adicional WHERE adicional_operadoraId=" + this.ID, typeof(Adicional), pm);

            try
            {
                //Carrega os contratos administrativos para poder deletar todas as dependencias
                IList<ContratoADM> contratos = LocatorHelper.Instance.ExecuteQuery<ContratoADM>(
                    "contratoadm_id FROM contratoAdm WHERE contratoadm_operadoraId=" + this.ID, typeof(ContratoADM), pm);
                if (contratos != null && contratos.Count > 0)
                {
                    foreach (ContratoADM contrato in contratos)
                    {
                        #region deleta tabelas de valores 

                        IList<TabelaValor> tabelas = LocatorHelper.Instance.ExecuteQuery<TabelaValor>(
                            "tabelavalor_id FROM tabela_valor WHERE tabelavalor_contratoId=" + contrato.ID, typeof(TabelaValor), pm);

                        if (tabelas != null && tabelas.Count > 0)
                        {
                            foreach (TabelaValor tabela in tabelas)
                            {
                                NonQueryHelper.Instance.ExecuteNonQuery("DELETE FROM tabela_valor_item WHERE tabelavaloritem_tabelaid=" + tabela.ID, pm);
                                pm.Remove(tabela);
                            }
                        }
                        #endregion

                        #region deleta comissionamentos da operadora x contrato adm 

                        IList<ComissionamentoOperadora> comissionamentos = LocatorHelper.Instance.ExecuteQuery
                            <ComissionamentoOperadora>("comissaooperadora_id FROM comissao_operadora WHERE comissaooperadora_contratoAdmId=" + contrato.ID, typeof(ComissionamentoOperadora), pm);
                        if (comissionamentos != null)
                        {
                            foreach (ComissionamentoOperadora comissionamento in comissionamentos)
                            {
                                NonQueryHelper.Instance.ExecuteNonQuery("DELETE FROM comissao_operadora_item WHERE comissaooperadoraitem_comissaoid=" + comissionamento.ID, pm);
                                NonQueryHelper.Instance.ExecuteNonQuery("DELETE FROM comissao_operadora_vitaliciedade WHERE cov_comissaoId=" + comissionamento.ID, pm);
                                pm.Remove(comissionamento);
                            }
                        }
                        #endregion

                        #region deleta planos e planos X produtos adicionais X contratos 

                        IList<Plano> planos = LocatorHelper.Instance.ExecuteQuery<Plano>("plano_id FROM plano WHERE plano_contratoId=" + contrato.ID, typeof(Plano), pm);

                        if (adicionais != null && planos != null)
                        {
                            foreach (Plano plano in planos)
                            {
                                foreach (Adicional adicional in adicionais)
                                {
                                    NonQueryHelper.Instance.ExecuteNonQuery(String.Concat("DELETE FROM contratoADM_plano_adicional WHERE contratoplanoadicional_contratoid=", contrato.ID, " AND contratoplanoadicional_planoid=", plano.ID, " AND contratoplanoadicional_adicionalid=", adicional.ID), pm);
                                }
                            }
                        }

                        //deleta planos
                        NonQueryHelper.Instance.ExecuteNonQuery("DELETE FROM plano WHERE plano_contratoId=" + contrato.ID, pm);
                        #endregion

                        //deleta calendarios de admissao, vencimento e recebimento
                        NonQueryHelper.Instance.ExecuteNonQuery("DELETE FROM calendario_recebimento WHERE calendariorecebimento_calendarioVenctoId IN (SELECT calendariovencto_id FROM calendarioVencimento WHERE calendariovencto_calendarioAdmissaoId IN (SELECT calendario_id FROM calendario WHERE calendario_contratoId=" + contrato.ID + "))", pm);

                        NonQueryHelper.Instance.ExecuteNonQuery("DELETE FROM calendario_vencimento WHERE calendariovencto_calendarioAdmissaoId IN (SELECT calendario_id FROM calendario WHERE calendario_contratoId=" + contrato.ID + ")", pm);
                        NonQueryHelper.Instance.ExecuteNonQuery("DELETE FROM calendario WHERE calendario_contratoId=" + contrato.ID, pm);

                        //deleta contrato adm
                        pm.Remove(contrato);
                    }
                }

                #region remove adicionais 

                if (adicionais != null)
                {
                    foreach (Adicional adicional in adicionais)
                    {
                        //deleta regras
                        NonQueryHelper.Instance.ExecuteNonQuery(String.Concat("DELETE FROM adicional_regra WHERE adicionalregra_operadoraId=", this.ID, " AND adicionalregra_adicionalId=", adicional.ID), pm);
                        //deleta faixas
                        NonQueryHelper.Instance.ExecuteNonQuery("DELETE FROM adicional_faixa WHERE adicionalfaixa_adicionalid=" + adicional.ID, pm);
                        pm.Remove(adicional);
                    }
                }
                #endregion

                //deleta os contatos
                NonQueryHelper.Instance.ExecuteNonQuery("DELETE FROM contato WHERE contato_operadoraId=" + this.ID, pm);

                pm.Remove(this);
                if (this._endereco != null) { pm.Remove(this._endereco); }

                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                pm = null;
                throw ex;
            }
        }

        public void Carregar()
        {
            base.Carregar(this);

            IList<Endereco> lista = Endereco.CarregarPorDono(this.ID, Endereco.TipoDono.Operadora);
            if (lista != null)
                this._endereco = lista[0];
        }
        #endregion

        public static IList<Operadora> CarregarTodas()
        {
            return CarregarTodas(false);
        }

        public static IList<Operadora> CarregarTodas(Boolean apenasAtivas)
        {
            String condicaoApenasAtivas = "";
            if (apenasAtivas)
            {
                condicaoApenasAtivas = " WHERE operadora_inativa=0";
            }

            String query = "* FROM operadora" + condicaoApenasAtivas + " ORDER BY operadora_nome";

            IList<Operadora> lista = LocatorHelper.
                Instance.ExecuteQuery<Operadora>(query, typeof(Operadora));

            return lista;
        }

        public static IList<Operadora> CarregarTodasQueEnviamCartaCobranca()
        {
            String query = "* FROM operadora WHERE operadora_inativa=0 AND operadora_enviaCartaAviso=1 ORDER BY operadora_nome";

            IList<Operadora> lista = LocatorHelper.
                Instance.ExecuteQuery<Operadora>(query, typeof(Operadora));

            return lista;
        }

        public static String CarregarNome(Object operadoraId, PersistenceManager pm)
        {
            String qry = "SELECT operadora_nome FROM operadora WHERE operadora_id=" + operadoraId;
            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
            if (ret == null || ret == DBNull.Value)
            {
                return String.Empty;
            }
            else
            {
                return Convert.ToString(ret);
            }
        }

        public static Object CarregarIDPorCNPJ(String cnpj, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@cnpj" };
            String[] paramVl = new String[] { cnpj };
            return LocatorHelper.Instance.ExecuteScalar("SELECT operadora_id FROM operadora WHERE operadora_cnpj=@cnpj", paramNm, paramVl, pm);
        }

        public static Object CarregarID(String contratoAdmNumero, PersistenceManager pm)
        {
            String[] paramNm = new String[] { "@numero" };
            String[] paramVl = new String[] { contratoAdmNumero };
            return LocatorHelper.Instance.ExecuteScalar("SELECT contratoadm_operadoraId FROM contratoADM WHERE contratoadm_numero=@numero", paramNm, paramVl, pm);
        }

        public static IList<Operadora> CarregarPorPlanoID(Object planoID)
        {
            String query = "operadora.* FROM operadora INNER JOIN contratoADM ON operadora_id=contratoadm_operadoraid INNER JOIN plano ON plano_contratoId=contratoadm_id  WHERE plano_id=" + planoID;

            IList<Operadora> lista = LocatorHelper.Instance.ExecuteQuery<Operadora>(query, typeof(Operadora));

            return lista;
        }

        public static IList<Operadora> CarregarPorContratoADM_ID(Object contratoId)
        {
            String query = "TOP 1 contratoadm_descricao, operadora.* FROM operadora INNER JOIN contratoADM ON operadora_id=contratoadm_operadoraid WHERE contratoadm_id=" + contratoId + " ORDER BY contratoadm_descricao";

            return LocatorHelper.Instance.ExecuteQuery<Operadora>(query, typeof(Operadora));
        }

        public static Boolean CnpjEmUso(String cnpj, Object operadoraId)
        {
            String qry = "SELECT operadora_id FROM operadora WHERE operadora_cnpj=@CNPJ";

            if (operadoraId != null) { qry += " AND operadora_id <> " + operadoraId; }

            String[] names = new String[] { "@CNPJ" };
            String[] value = new String[] { cnpj };

            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, names, value);

            return ret != null && ret != DBNull.Value;
        }

        public static Boolean NomeEmUso(String nome, Object operadoraId)
        {
            String qry = "SELECT operadora_id FROM operadora WHERE operadora_nome=@NOME";

            if (operadoraId != null) { qry += " AND operadora_id <> " + operadoraId; }

            String[] names = new String[] { "@NOME" };
            String[] value = new String[] { nome };

            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, names, value);

            return ret != null && ret != DBNull.Value;
        }

        public static void AlterarStatus(Object OperadoraID, StatusOperadora status)
        {
            String command = "UPDATE operadora SET operadora_inativa=" + Convert.ToInt32(status) + " WHERE operadora_id=" + OperadoraID;
            NonQueryHelper.Instance.ExecuteNonQuery(command, null);
        }

        //public static void SetaTabelaReajusteAutal(Object operadoraId, Object tabelaReajusteId, PersistenceManager pm)
        //{
        //    String command = "UPDATE operadora SET operadora_tabelaReajusterAtualId=" + tabelaReajusteId + " WHERE operadora_id=" + operadoraId;
        //    NonQueryHelper.Instance.ExecuteNonQuery(command, pm);
        //}

        //public void SetaTabelaReajusteAutal(Object tabelaReajusteId, PersistenceManager pm)
        //{
        //    Operadora.SetaTabelaReajusteAutal(this._id, tabelaReajusteId, pm);
        //}

        public static DataTable CarregarOperadorasOrigem(String nome)
        {
            String qry = "select operadoraorigem_id as ID, operadoraorigem_codigo as Codigo, operadoraorigem_nome as Nome, operadoraorigem_codigoAns as CodigoANS FROM operadoraOrigem where operadoraorigem_nome like @nome";
            String[] name = new String[] { "@nome" }; 
            String[] value = new String[] { "%" + nome + "%" };

            DataTable dt = LocatorHelper.Instance.ExecuteParametrizedQuery(qry, name, value).Tables[0];

            return dt;
        }
    }

    [DBTable("feriado")]
    public class DiaFeriado : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _operadoraId;
        Object _usuarioId;
        String _descricao;
        DateTime _data;
        String _obs;

        public DiaFeriado() { }
        public DiaFeriado(Object id) { _id = id; }

        #region properties 

        [DBFieldInfo("feriado_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("feriado_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("feriado_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId= value; }
        }

        [DBFieldInfo("feriado_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("feriado_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [DBFieldInfo("feriado_obs", FieldType.Single)]
        public String OBS
        {
            get { return _obs; }
            set { _obs = value; }
        }

        #endregion

        #region persistence methods 

        public void Salvar()
        {
            try
            {
                base.Salvar(this);
            }
            catch 
            {
                throw;
            }
        }

        public void Remover()
        {
            try
            {
                base.Remover(this);
            }
            catch 
            {
                throw;
            }
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        #endregion

        public static IList<DiaFeriado> CarregarTodos()
        {
            String qry = "select feriado_id,feriado_operadoraId,feriado_usuarioId,feriado_descricao,feriado_data from feriado order by feriado_data";

            return LocatorHelper.Instance.ExecuteQuery<DiaFeriado>(qry, typeof(DiaFeriado));
        }
    }
}