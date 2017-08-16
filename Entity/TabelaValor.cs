namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("tabela_valor")]
    public class TabelaValor : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _contratoId;
        String _descricao;
        DateTime _data;
        Object _corrente;

        DateTime _inicio;
        DateTime _fim;

        DateTime _inicioVencimento;
        DateTime _fimVencimento;

        String _contratoDescricao;

        #region propriedades

        [DBFieldInfo("TabelaValor_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("TabelaValor_contratoid", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId = value; }
        }

        //[DBFieldInfo("TabelaValor_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _contratoDescricao; }
            set { _descricao = value; }
        }

        [DBFieldInfo("tabelavalor_inicio", FieldType.Single)]
        public DateTime Inicio
        {
            get { return _inicio; }
            set { _inicio= value; }
        }

        [DBFieldInfo("tabelavalor_fim", FieldType.Single)]
        public DateTime Fim
        {
            get { return _fim; }
            set { _fim= value; }
        }

        [DBFieldInfo("tabelavalor_vencimentoInicio", FieldType.Single)]
        public DateTime VencimentoInicio
        {
            get { return _inicioVencimento; }
            set { _inicioVencimento = value; }
        }

        [DBFieldInfo("tabelavalor_vencimentoFim", FieldType.Single)]
        public DateTime VencimentoFim
        {
            get { return _fimVencimento; }
            set { _fimVencimento = value; }
        }

        public String VencimentoInicioStr
        {
            get { if (_inicioVencimento == DateTime.MinValue) return ""; else return _inicioVencimento.ToString("dd/MM/yyyy"); }
        }

        public String VencimentoFimStr
        {
            get { if (_fimVencimento == DateTime.MinValue) return ""; else return _fimVencimento.ToString("dd/MM/yyyy"); }
        }

        [Obsolete("Em desuso.", true)]
        [DBFieldInfo("TabelaValor_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        /// <summary>
        /// Estará na entidade plano o id da tabela corrente
        /// </summary>
        [Joinned("plano_tabelaValorAtualId")]
        public Object Corrente
        {
            get
            {
                return _corrente;
            }
            set { _corrente = value; }
        }

        [Joinned("contratoadm_descricao")]
        public String ContratoDescricao
        {
            get { return _contratoDescricao; }
            set { _contratoDescricao = value; }
        }

        #endregion

        public TabelaValorItem TabelaValorItem
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public TabelaValor() { _data = DateTime.Now; }
        public TabelaValor(Object id) : this() { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.TransactionContext();

            try
            {
                String query = "DELETE FROM tabela_valor_item WHERE tabelavaloritem_tabelaid=" + this._id;
                NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

                query = "DELETE FROM tabela_valor_data WHERE tvdata_tabelaId=" + this._id;
                NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

                query = "DELETE FROM taxa WHERE taxa_tabelaValorId=" + this._id;
                NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

                pm.Remove(this);
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
            //base.Remover(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<TabelaValor> CarregarPorContratoID(Object contratoAdmId)
        {
            return CarregarPorContratoID(contratoAdmId, null);
        }

        public static IList<TabelaValor> CarregarPorContratoID(Object contratoAdmId, PersistenceManager pm)
        {
            String query = "tabela_valor.*, contratoadm_descricao FROM tabela_valor LEFT JOIN contratoAdm ON tabelavalor_contratoid=contratoAdm_id WHERE tabelavalor_contratoid=" + contratoAdmId + " ORDER BY tabelavalor_inicio DESC, tabelavalor_id DESC";
            return LocatorHelper.Instance.ExecuteQuery<TabelaValor>(query, typeof(TabelaValor), pm);
        }

        public static IList<TabelaValor> CarregarPorContratoID_Parcial(Object contratoAdmId, PersistenceManager pm)
        {
            String query = "tabelavalor_id FROM tabela_valor WHERE tabelavalor_contratoid=" + contratoAdmId + " ORDER BY tabelavalor_inicio DESC, tabelavalor_id DESC";
            return LocatorHelper.Instance.ExecuteQuery<TabelaValor>(query, typeof(TabelaValor), pm);
        }

        public static IList<TabelaValor> CarregarTabelaAtual(Object contratoAdmId)
        {
            return CarregarTabelaAtual(contratoAdmId, null);
        }
        public static IList<TabelaValor> CarregarTabelaAtual(Object contratoAdmId, PersistenceManager pm)
        {
            String query = "TOP 1 tabela_valor.*, contratoadm_descricao FROM tabela_valor INNER JOIN contratoAdm ON tabelavalor_contratoid=contratoAdm_id WHERE tabelavalor_contratoid=" + contratoAdmId + " ORDER BY tabelavalor_inicio DESC, tabelavalor_id DESC";
            return LocatorHelper.Instance.ExecuteQuery<TabelaValor>(query, typeof(TabelaValor), pm);
        }

        public static IList<TabelaValor> CarregarTabelaVigente(Object contratoAdmId, DateTime admissaoProposta, DateTime? vencimentoCobranca, PersistenceManager pm)
        {
            return CarregarTabelaVigente(contratoAdmId, admissaoProposta, vencimentoCobranca, pm, false); ;
        }
        public static IList<TabelaValor> CarregarTabelaVigente(Object contratoAdmId, DateTime admissaoProposta, DateTime? vencimentoCobranca, PersistenceManager pm, Boolean forcaTabelaVigente)
        {
            String vencimentoCond = "";
            if (vencimentoCobranca != null)
            {
                vencimentoCond = String.Concat(" OR '", vencimentoCobranca.Value.ToString("yyyy-MM-dd"), "'  BETWEEN tabelavalor_vencimentoInicio AND tabelavalor_vencimentoFim");
            }

            String query = String.Concat("TOP 1 tabela_valor.*, contratoadm_descricao ",
                "   FROM tabela_valor ",
                "       INNER JOIN contratoAdm ON tabelavalor_contratoid=contratoAdm_id",
                "   WHERE ",
                "       tabelavalor_contratoid=", contratoAdmId,
                "       AND ('", admissaoProposta.ToString("yyyy-MM-dd"), "' BETWEEN tabelavalor_inicio AND tabelavalor_fim ", vencimentoCond, ")",
                "   ORDER BY tabelavalor_inicio DESC, tabelavalor_id DESC");

            IList<TabelaValor> ret = LocatorHelper.Instance.ExecuteQuery<TabelaValor>(query, typeof(TabelaValor), pm);

            if (ret == null) { return null; }

            if (vencimentoCobranca != null && forcaTabelaVigente)
            {
                //se foi informado um vencimento de cobrança
                //se esse vencimento for maior que o limite de vencimento da tabela, retorna null.
                //nesses casos, nao basta estar de acordo com a admissao do contrato:
                ret[0].VencimentoFim = new DateTime(ret[0].VencimentoFim.Year, ret[0].VencimentoFim.Month, ret[0].VencimentoFim.Day, 23, 59, 59, 998);
                vencimentoCobranca = new DateTime(vencimentoCobranca.Value.Year, vencimentoCobranca.Value.Month, vencimentoCobranca.Value.Day, 23, 59, 59, 997);
                if (vencimentoCobranca > ret[0].VencimentoFim) { return null; }
            }

            return ret;
        }

        public static Decimal CalculaValor(Object beneficiarioId, int beneficiarioIdade, Object contratoId, Object planoId, Contrato.eTipoAcomodacao tipoAcomodacao, DateTime admissaoProposta, DateTime? vencimentoCobranca)
        {
            return CalculaValor(beneficiarioId, beneficiarioIdade, contratoId, planoId, tipoAcomodacao, null, admissaoProposta, vencimentoCobranca, false);
        }
        public static Decimal CalculaValor(Object beneficiarioId, int beneficiarioIdade, Object contratoAdmId, Object planoId, Contrato.eTipoAcomodacao tipoAcomodacao, PersistenceManager pm, DateTime admissaoProposta, DateTime? vencimentoCobranca)
        {
            return CalculaValor(beneficiarioId, beneficiarioIdade, contratoAdmId, planoId, tipoAcomodacao, pm, admissaoProposta, vencimentoCobranca, false);
        }
        public static Decimal CalculaValor(Object beneficiarioId, int beneficiarioIdade, Object contratoAdmId, Object planoId, Contrato.eTipoAcomodacao tipoAcomodacao, PersistenceManager pm, DateTime admissaoProposta, DateTime? vencimentoCobranca, Boolean forcaTabelaVigente)
        {
            IList<TabelaValor> lista = TabelaValor.CarregarTabelaVigente(contratoAdmId, admissaoProposta, vencimentoCobranca, pm, forcaTabelaVigente); //TabelaValor.CarregarTabelaAtual(contratoAdmId, pm);
            if (lista == null || lista.Count == 0) { return 0; }

            IList<TabelaValorItem> itens = TabelaValorItem.CarregarPorTabela(lista[0].ID, planoId, pm);
            if (itens == null || itens.Count == 0) { return 0; }

            if (beneficiarioIdade == 0)
            {
                Beneficiario beneficiario = new Beneficiario();
                beneficiario.ID = beneficiarioId;

                if (beneficiario.ID != null)
                {
                    if (pm == null)
                        beneficiario.Carregar();
                    else
                        pm.Load(beneficiario);

                    if (beneficiario.ID == null) { return 0; }

                    beneficiarioIdade = Beneficiario.CalculaIdade(beneficiario.DataNascimento);
                }
            }

            foreach (TabelaValorItem _item in itens)
            {
                if (beneficiarioIdade >= _item.IdadeInicio && _item.IdadeFim == 0)
                {
                    if (tipoAcomodacao == Contrato.eTipoAcomodacao.quartoComun)
                        return _item.QCValor;
                    else
                        return _item.QPValor;
                }
                else if (beneficiarioIdade >= _item.IdadeInicio && beneficiarioIdade <= _item.IdadeFim)
                {
                    if (tipoAcomodacao == Contrato.eTipoAcomodacao.quartoComun)
                        return _item.QCValor;
                    else
                        return _item.QPValor;
                }
            }

            return 0;
        }

        public static Decimal CalculaValorNET(Contrato contrato, PersistenceManager pm)
        {
            return CalculaValorNET(contrato, pm, null, null, null);
        }

        public static Decimal CalculaValorNET(Object contratoId, Object contratoAdmId, Object planoId, Object beneficiarioId, Int32 contratoTipoAcomodacao, DateTime? admissao, DateTime beneficiarioDataNascimento, DateTime? vencimento, DateTime? dataReferencia, out Int32 beneficiarioIdade, PersistenceManager pm)
        {
            Contrato.eTipoAcomodacao tipoAcomodacao = (Contrato.eTipoAcomodacao)contratoTipoAcomodacao;
            IList<TabelaValor> lista = null;
            beneficiarioIdade = -1;

            if (admissao == null || vencimento == null)
                lista = TabelaValor.CarregarTabelaAtual(contratoAdmId, pm);
            else
                lista = TabelaValor.CarregarTabelaVigente(contratoAdmId, admissao.Value, vencimento.Value, pm);

            if (lista == null || lista.Count == 0) { return 0; }

            IList<TabelaValorItem> itens = TabelaValorItem.CarregarPorTabela(lista[0].ID, planoId, pm);
            if (itens == null || itens.Count == 0) { return 0; }

            if (dataReferencia == null)
                beneficiarioIdade = Beneficiario.CalculaIdade(beneficiarioDataNascimento);
            else
                beneficiarioIdade = Beneficiario.CalculaIdade(beneficiarioDataNascimento, dataReferencia.Value);

            Decimal valorTotal = 0;
            foreach (TabelaValorItem _item in itens)
            {
                if (beneficiarioIdade >= _item.IdadeInicio && _item.IdadeFim == 0)
                {
                    if (tipoAcomodacao == Contrato.eTipoAcomodacao.quartoComun)
                        valorTotal += _item.QCValorPagamento;
                    else
                        valorTotal += _item.QPValorPagamento;
                    break;
                }
                else if (beneficiarioIdade >= _item.IdadeInicio && beneficiarioIdade <= _item.IdadeFim)
                {
                    if (tipoAcomodacao == Contrato.eTipoAcomodacao.quartoComun)
                        valorTotal += _item.QCValorPagamento;
                    else
                        valorTotal += _item.QPValorPagamento;
                    break;
                }
            }

            return valorTotal;
        }

        public static Decimal CalculaValorNET(Contrato contrato, PersistenceManager pm, DateTime? admissao, DateTime? vencimento, DateTime? dataReferencia)
        {
            Contrato.eTipoAcomodacao tipoAcomodacao = (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao;

            IList<TabelaValor> lista = null;

            if (admissao == null || vencimento == null)
                lista = TabelaValor.CarregarTabelaAtual(contrato.ContratoADMID, pm);
            else
                lista = TabelaValor.CarregarTabelaVigente(contrato.ContratoADMID, admissao.Value, vencimento.Value, pm);

            if (lista == null || lista.Count == 0) { return 0; }

            IList<TabelaValorItem> itens = TabelaValorItem.CarregarPorTabela(lista[0].ID, contrato.PlanoID, pm);
            if (itens == null || itens.Count == 0) { return 0; }

            IList<ContratoBeneficiario> beneficiarios = ContratoBeneficiario.CarregarPorContratoID_Parcial(contrato.ID, true, false, pm);

            int beneficiarioIdade = 0;
            Decimal valorTotal = 0;
            foreach (ContratoBeneficiario beneficiario in beneficiarios)
            {
                if (beneficiario.ID == null) { return 0; }

                if(dataReferencia == null)
                    beneficiarioIdade = Beneficiario.CalculaIdade(beneficiario.BeneficiarioDataNascimento);
                else
                    beneficiarioIdade = Beneficiario.CalculaIdade(beneficiario.BeneficiarioDataNascimento, dataReferencia.Value);

                foreach (TabelaValorItem _item in itens)
                {
                    if (beneficiarioIdade >= _item.IdadeInicio && _item.IdadeFim == 0)
                    {
                        if (tipoAcomodacao == Contrato.eTipoAcomodacao.quartoComun)
                            valorTotal += _item.QCValorPagamento;
                        else
                            valorTotal += _item.QPValorPagamento;
                        break;
                    }
                    else if (beneficiarioIdade >= _item.IdadeInicio && beneficiarioIdade <= _item.IdadeFim)
                    {
                        if (tipoAcomodacao == Contrato.eTipoAcomodacao.quartoComun)
                            valorTotal += _item.QCValorPagamento;
                        else
                            valorTotal += _item.QPValorPagamento;
                        break;
                    }
                }
            }

            beneficiarios = null;
            return valorTotal;
        }

        public static Boolean ExisteTabelaComVigencia(DateTime inicio, DateTime fim, Object contratoAdmId, Object tabelaId)
        {
            String query = "SELECT COUNT(*) FROM tabela_valor WHERE ((tabelavalor_inicio <= '" + inicio.ToString("yyyy-MM-dd 00:00:00") + "' AND tabelavalor_fim >='" + inicio.ToString("yyyy-MM-dd 23:59:59") + "') OR (tabelavalor_inicio <= '" + fim.ToString("yyyy-MM-dd 00:00:00") + "' AND tabelavalor_fim >='" + fim.ToString("yyyy-MM-dd 23:59:59") + "')) AND tabelavalor_contratoId=" + contratoAdmId;

            if (tabelaId != null)
            {
                query += " AND tabelavalor_id <> " + tabelaId;
            }

            Object ret = LocatorHelper.Instance.ExecuteScalar(query, null, null);

            if (ret == null || ret == DBNull.Value || Convert.ToInt32(ret) == 0)
                return false;
            else
                return true;
        }
    }

    //[DBTable("tabela_valor_data")]
    //public class TabelaValorData : EntityBase, IPersisteableEntity
    //{
    //    Object _id;
    //    Object _tabelaId;
    //    Object _planoId;
    //    DateTime _data;

    //    #region propriedades 

    //    [DBFieldInfo("tvdata_id", FieldType.PrimaryKeyAndIdentity)]
    //    public Object ID
    //    {
    //        get { return _id; }
    //        set { _id= value; }
    //    }

    //    [DBFieldInfo("tvdata_tabelaId", FieldType.Single)]
    //    public Object TabelaID
    //    {
    //        get { return _tabelaId; }
    //        set { _tabelaId= value; }
    //    }

    //    [DBFieldInfo("tvdata_planoId", FieldType.Single)]
    //    public Object PlanoID
    //    {
    //        get { return _planoId; }
    //        set { _planoId= value; }
    //    }

    //    [DBFieldInfo("tvdata_data", FieldType.Single)]
    //    public DateTime Data
    //    {
    //        get { return _data; }
    //        set { _data= value; }
    //    }

    //    public String strData
    //    {
    //        get 
    //        {
    //            if (_data == DateTime.MinValue)
    //                return String.Empty;
    //            else
    //                return _data.ToString("dd/MM/yyyy");
    //        }
    //    }

    //    #endregion

    //    public TabelaValorData() { }
    //    public TabelaValorData(Object id) { _id = id; }

    //    #region métodos EntityBase 

    //    public void Salvar()
    //    {
    //        base.Salvar(this);
    //    }

    //    public void Remover()
    //    {
    //        base.Remover(this);
    //    }

    //    public void Carregar()
    //    {
    //        base.Carregar(this);
    //    }
    //    #endregion

    //    public static IList<TabelaValorData> Carregar(Object tabelaId, Object planoId)
    //    {
    //        String query = "* FROM tabela_valor_data WHERE tvdata_tabelaId=" + tabelaId + " AND tvdata_planoId=" + planoId + " ORDER BY tvdata_data DESC, tvdata_id DESC";
    //        return LocatorHelper.Instance.ExecuteQuery<TabelaValorData>(query, typeof(TabelaValorData));
    //    }
    //}

    [Serializable()]
    [DBTable("tabela_valor_item")]
    public class TabelaValorItem : EntityBase, IPersisteableEntity
    {
        #region campos

        Object _id;
        Object _tabelaId;
        Object _planoId;
        int _idadeInicio;
        int _idadeFim;
        Boolean _calculoAutomatico;
        Decimal _qcValor;
        Decimal _qpValor;
        Decimal _qcValorPagamento;
        Decimal _qpValorPagamento;
        Decimal _qcValorCompraCarencia;
        Decimal _qpValorCompraCarencia;
        Decimal _qcValorMigracao;
        Decimal _qpValorMigracao;
        Decimal _qcValorCondicaoEspecial;
        Decimal _qpValorCondicaoEspecial;
        //DateTime _data;

        #endregion

        #region propriedades

        [DBFieldInfo("TabelaValoritem_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("TabelaValoritem_tabelaid", FieldType.Single)]
        public Object TabelaID
        {
            get { return _tabelaId; }
            set { _tabelaId = value; }
        }

        [DBFieldInfo("tabelaValorItem_planoId", FieldType.Single)]
        public Object PlanoID
        {
            get { return _planoId; }
            set { _planoId = value; }
        }

        [DBFieldInfo("TabelaValoritem_idadeInicio", FieldType.Single)]
        public int IdadeInicio
        {
            get { return _idadeInicio; }
            set { _idadeInicio = value; }
        }

        [DBFieldInfo("TabelaValoritem_idadeFim", FieldType.Single)]
        public int IdadeFim
        {
            get { return _idadeFim; }
            set { _idadeFim = value; }
        }

        /// <summary>
        /// Cliente.
        /// </summary>
        [DBFieldInfo("tabelavaloritem_qComum", FieldType.Single)]
        public Decimal QCValor
        {
            get { return _qcValor; }
            set { _qcValor = value; }
        }

        [DBFieldInfo("tabelavaloritem_calculoAutomatico", FieldType.Single)]
        public Boolean CalculoAutomatico
        {
            get { return _calculoAutomatico; }
            set { _calculoAutomatico= value; }
        }

        public String QCValorSTR
        {
            get { return _qcValor.ToString("N2"); }
        }

        /// <summary>
        /// Cliente.
        /// </summary>
        [DBFieldInfo("tabelavaloritem_qParticular", FieldType.Single)]
        public Decimal QPValor
        {
            get { return _qpValor; }
            set { _qpValor = value; }
        }

        public String QPValorSTR
        {
            get { return _qpValor.ToString("N2"); }
        }

        /// <summary>
        /// Operadora
        /// </summary>
        [DBFieldInfo("tabelavaloritem_qComumPagamento", FieldType.Single)]
        public Decimal QCValorPagamento
        {
            get { return _qcValorPagamento; }
            set { _qcValorPagamento = value; }
        }

        public String QCValorPagamentoSTR
        {
            get { return _qcValorPagamento.ToString("N2"); }
        }

        /// <summary>
        /// Operadora
        /// </summary>
        [DBFieldInfo("tabelavaloritem_qParticularPagamento", FieldType.Single)]
        public Decimal QPValorPagamento
        {
            get { return _qpValorPagamento; }
            set { _qpValorPagamento = value; }
        }

        public String QPValorPagamentoSTR
        {
            get { return _qpValorPagamento.ToString("N2"); }
        }

        [DBFieldInfo("tabelavaloritem_qComumCompraCarencia", FieldType.Single)]
        public Decimal QCValorCompraCarencia
        {
            get { return _qcValorCompraCarencia; }
            set { _qcValorCompraCarencia = value; }
        }

        [DBFieldInfo("tabelavaloritem_qParticularCompraCarencia", FieldType.Single)]
        public Decimal QPValorCompraCarencia
        {
            get { return _qpValorCompraCarencia; }
            set { _qpValorCompraCarencia = value; }
        }

        [DBFieldInfo("tabelavaloritem_qComumMigracao", FieldType.Single)]
        public Decimal QCValorMigracao
        {
            get { return _qcValorMigracao; }
            set { _qcValorMigracao = value; }
        }

        [DBFieldInfo("tabelavaloritem_qParticularMigracao", FieldType.Single)]
        public Decimal QPValorMigracao
        {
            get { return _qpValorMigracao; }
            set { _qpValorMigracao = value; }
        }

        [DBFieldInfo("tabelavaloritem_qComumEspecial", FieldType.Single)]
        public Decimal QCValorCondicaoEspecial
        {
            get { return _qcValorCondicaoEspecial; }
            set { _qcValorCondicaoEspecial = value; }
        }

        [DBFieldInfo("tabelavaloritem_qParticularEspecial", FieldType.Single)]
        public Decimal QPValorCondicaoEspecial
        {
            get { return _qpValorCondicaoEspecial; }
            set { _qpValorCondicaoEspecial = value; }
        }

        //[DBFieldInfo("tabelavaloritem_data", FieldType.Single)]
        //public DateTime Data
        //{
        //    get { return _data; }
        //    set { _data= value; }
        //}

        public String FaixaEtaria
        {
            get
            {
                String ret = "";

                if (_idadeFim > 0)
                    ret = String.Concat("de ", _idadeInicio, " a ", _idadeFim);
                else
                    ret = String.Concat("de ", _idadeInicio, " em diante");

                return ret;
            }
        }

        #endregion

        public TabelaValorItem() { _calculoAutomatico = true; }

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

        public void AplicaTaxa(Taxa taxa, Boolean calculaReajuste)
        {
            //se o calculo é manual
            if (!this._calculoAutomatico)// && calculaReajuste)
            {
                if (taxa.PercentualReajuste > 0 && this.QCValorPagamento != 0 && calculaReajuste) //usado na duplicação
                {
                    this.QCValor          = ((taxa.PercentualReajuste / 100) * this.QCValor) + this.QCValor;
                    this.QCValorPagamento = ((taxa.PercentualReajuste / 100) * this.QCValorPagamento) + this.QCValorPagamento;
                }

                if (taxa.PercentualReajuste > 0 && this.QPValorPagamento != 0 && calculaReajuste) //usado na duplicação§Ã£o
                {
                    this.QPValor          = ((taxa.PercentualReajuste / 100) * this.QPValor) + this.QPValor;
                    this.QPValorPagamento = ((taxa.PercentualReajuste / 100) * this.QPValorPagamento) + this.QPValorPagamento;
                }

                //if (taxa.Over > 0)
                //{
                //    //this.QCValor = taxa.Fixo + this.QCValorPagamento + ((taxa.Over / 100) * this.QCValorPagamento);
                //    this.QCValor = taxa.Fixo + this.QCValor + ((taxa.Over / 100) * this.QCValor);
                //    this.QPValor = taxa.Fixo + this.QPValor + ((taxa.Over / 100) * this.QPValor);

                //    this.QCValorPagamento = taxa.Fixo + this.QCValorPagamento + ((taxa.Over / 100) * this.QCValorPagamento);
                //    this.QPValorPagamento = taxa.Fixo + this.QPValorPagamento + ((taxa.Over / 100) * this.QPValorPagamento);
                //}
                //else
                //{
                //    this.QCValor = this.QCValor + taxa.Fixo;
                //    this.QPValor = this.QPValor + taxa.Fixo;

                //    this.QCValorPagamento = this.QCValorPagamento + taxa.Fixo;
                //    this.QPValorPagamento = this.QPValorPagamento + taxa.Fixo;
                //}
            }
            else //if(this._calculoAutomatico)
            {
                if (taxa.PercentualReajuste > 0 && this.QCValorPagamento != 0 && calculaReajuste) //usado na duplicação
                {
                    this.QCValorPagamento = ((taxa.PercentualReajuste / 100) * this.QCValorPagamento) + this.QCValorPagamento;
                }

                if (taxa.PercentualReajuste > 0 && this.QPValorPagamento != 0 && calculaReajuste) //usado na duplicação
                {
                    this.QPValorPagamento = ((taxa.PercentualReajuste / 100) * this.QPValorPagamento) + this.QPValorPagamento;
                }

                if (taxa.Over > 0)
                {
                    this.QCValor = taxa.Fixo + this.QCValorPagamento + ((taxa.Over / 100) * this.QCValorPagamento);
                }
                else
                    this.QCValor = this.QCValorPagamento + taxa.Fixo;

                if (taxa.Over > 0)
                    this.QPValor = taxa.Fixo + this.QPValorPagamento + ((taxa.Over / 100) * this.QPValorPagamento);
                else
                    this.QPValor = this.QPValorPagamento + taxa.Fixo;


                //if (taxa.PercentualReajuste > 0 && this.QCValorPagamento != 0 && calculaReajuste) //usado na duplicação
                //{
                //    this.QCValorPagamento = ((taxa.PercentualReajuste / 100) * this.QCValorPagamento) + this.QCValorPagamento;
                //    this.QPValorPagamento = ((taxa.PercentualReajuste / 100) * this.QPValorPagamento) + this.QPValorPagamento;
                //}
            }
        }

        public static IList<TabelaValorItem> CarregarPorTabela(Object tabelaID, Object planoId)
        {
            return CarregarPorTabela(tabelaID, planoId, null);
        }

        public static IList<TabelaValorItem> CarregarPorTabela(Object tabelaID, Object planoId, PersistenceManager pm)
        {
            String query = "* FROM tabela_valor_item WHERE TabelaValoritem_tabelaid=" + tabelaID + " AND tabelavaloritem_planoId=" + planoId + " ORDER BY TabelaValoritem_idadeInicio";
            return LocatorHelper.Instance.ExecuteQuery<TabelaValorItem>(query, typeof(TabelaValorItem), pm);
        }

        public static IList<TabelaValorItem> CarregarPorTabela(Object tabelaID, PersistenceManager pm)
        {
            String query = "* FROM tabela_valor_item WHERE TabelaValoritem_tabelaid=" + tabelaID + " ORDER BY tabelaValorItem_planoId, TabelaValoritem_idadeInicio";
            return LocatorHelper.Instance.ExecuteQuery<TabelaValorItem>(query, typeof(TabelaValorItem), pm);
        }
    }
}