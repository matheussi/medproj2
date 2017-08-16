namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    /// <summary>
    /// Em desuso.
    /// </summary>
    [DBTable("contacorrenteFatura")]
    sealed class FaturaContaCorrente : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _produtorId;
        Decimal _debito;
        Decimal _credito;
        Decimal _saldo;
        DateTime _data;

        #region propriedades 

        [DBFieldInfo("contacorrentefatura_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("contacorrentefatura_produtorId", FieldType.Single)]
        public Object ProdutorID
        {
            get { return _produtorId; }
            set { _produtorId= value; }
        }

        [DBFieldInfo("contacorrentefatura_debito", FieldType.Single)]
        public Decimal Debito
        {
            get { return _debito; }
            set { _debito= value; }
        }

        [DBFieldInfo("contacorrentefatura_credito", FieldType.Single)]
        public Decimal Credito
        {
            get { return _credito; }
            set { _credito= value; }
        }

        [DBFieldInfo("contacorrentefatura_saldo", FieldType.Single)]
        public Decimal Saldo
        {
            get { return _saldo; }
            set { _saldo= value; }
        }

        [DBFieldInfo("contacorrentefatura_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        #endregion

        #region métodos EntityBase 

        public void Salvar(ref MovimentacaoContaCorrente mcc)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                CategoriaContaCorrente categoria = new CategoriaContaCorrente(mcc.CategoriaID);
                pm.Load(categoria);

                FaturaContaCorrente fatura = FaturaContaCorrente.Carregar(mcc.Data.Month, mcc.Data.Year, pm);
                if (fatura == null)
                {
                    fatura = new FaturaContaCorrente();
                    fatura.Data = mcc.Data;
                    fatura.ProdutorID = mcc.ProdutorID;
                    if (CategoriaContaCorrente.eTipo.Credito == (CategoriaContaCorrente.eTipo)categoria.Tipo)
                    {
                        fatura.Credito = mcc.Valor;
                        fatura.Saldo = mcc.Valor;
                    }
                    else
                    {
                        fatura.Debito = mcc.Valor;
                        fatura.Saldo = (-1) * mcc.Valor;
                        mcc.Valor *= (-1);
                    }
                }
                else
                {
                    if (CategoriaContaCorrente.eTipo.Credito == (CategoriaContaCorrente.eTipo)mcc.CategoriaTipo)
                    {
                        fatura.Credito += mcc.Valor;
                        fatura.Saldo = fatura.Credito - fatura.Debito;
                    }
                    else
                    {
                        fatura.Debito += mcc.Valor;
                        fatura.Saldo = fatura.Credito - fatura.Debito;
                        mcc.Valor *= (-1);
                    }
                }

                pm.Save(fatura);
                pm.Save(mcc);
                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm = null;
            }
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

        public FaturaContaCorrente() { _data = DateTime.Now; }

        public static FaturaContaCorrente Carregar(Int32 mes, Int32 ano, PersistenceManager pm)
        {
            String qry = String.Concat("* FROM contacorrenteFatura WHERE MONTH(contacorrentefatura_data)=", 
                mes, " AND YEAR(contacorrentefatura_data)=", ano);
            IList<FaturaContaCorrente> ret = LocatorHelper.Instance.ExecuteQuery<FaturaContaCorrente>(qry, typeof(FaturaContaCorrente), pm);
            if (ret == null)
                return null;
            else
                return ret[0];
        }

        public IList<FaturaContaCorrente> Carregar(Object produtorId)
        {
            String qry = "* FROM contacorrenteFatura WHERE contacorrentefatura_produtorId=" + produtorId + " ORDER BY contacorrentefatura_data DESC";

            return LocatorHelper.Instance.ExecuteQuery<FaturaContaCorrente>(qry, typeof(FaturaContaCorrente));
        }
    }

    [DBTable("contacorrenteMovimentacao")]
    public sealed class MovimentacaoContaCorrente : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _produtorId;
        Object _categoriaId;
        Object _cobrancaId;
        String _motivo;
        Decimal _valor;
        DateTime _data;
        Object _lisagemFechamentoId;

        String _categoriaDescricao;
        Int32 _categoriaTipo;

        #region propriedades  

        [DBFieldInfo("contacorrentemov_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("contacorrentemov_produtorId", FieldType.Single)]
        public Object ProdutorID
        {
            get { return _produtorId; }
            set { _produtorId= value; }
        }

        [DBFieldInfo("contacorrentemov_categoriaId", FieldType.Single)]
        public Object CategoriaID
        {
            get { return _categoriaId; }
            set { _categoriaId= value; }
        }

        [DBFieldInfo("contacorrentemov_cobrancaId", FieldType.Single)]
        public Object CobrancaID
        {
            get { return _cobrancaId; }
            set { _cobrancaId= value; }
        }

        [DBFieldInfo("contacorrentemov_motivo", FieldType.Single)]
        public String Motivo
        {
            get { return _motivo; }
            set { _motivo= value; }
        }

        [DBFieldInfo("contacorrentemov_valor", FieldType.Single)]
        public Decimal Valor
        {
            get { return _valor; }
            set { _valor= value; }
        }

        [DBFieldInfo("contacorrentemov_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [DBFieldInfo("contacorrentemov_listagemFechamentoId", FieldType.Single)]
        public Object LisagemFechamentoID
        {
            get { return _lisagemFechamentoId; }
            set { _lisagemFechamentoId= value; }
        }

        [Joinned("cccategoria_descricao")]
        public String CategoriaDescricao
        {
            get { return _categoriaDescricao; }
            set { _categoriaDescricao= value; }
        }

        [Joinned("cccategoria_tipo")]
        public Int32 CategoriaTipo
        {
            get { return _categoriaTipo; }
            set { _categoriaTipo= value; }
        }

        public String CategoriaStrTipo
        {
            get
            {
                if (CategoriaContaCorrente.eTipo.Credito == (CategoriaContaCorrente.eTipo)_categoriaTipo)
                    return "Crédito";
                else
                    return "Débito";
            }
        }

        #endregion

        public MovimentacaoContaCorrente() { _data = DateTime.Now; }
        public MovimentacaoContaCorrente(Object id) : this() { _id = id; }

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

        /// <summary>
        /// Checa se o credito da comissao de um produtor, referente pagamento de cobranca, ja foi feito.
        /// </summary>
        public static Boolean CreditoJaFeitoPara(Object produtorId, Object cobrancaId, PersistenceManager pm)
        {
            String qry = String.Concat("SELECT contacorrentemov_id FROM contacorrenteMovimentacao WHERE contacorrentemov_produtorId=",
                produtorId, " AND contacorrentemov_cobrancaId=", cobrancaId, " AND contacorrentemov_categoriaId=", CategoriaContaCorrente.SysPremiacaoCategoriaID);

            Object ret = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);

            if (ret != null && ret != DBNull.Value)
                return true;
            else
                return false;
        }

        public static IList<MovimentacaoContaCorrente> Carregar(Object produtorId, DateTime de, DateTime ate)
        {
            String qry = String.Concat("contacorrenteMovimentacao.*, cccategoria_descricao, cccategoria_tipo",
                "   FROM contacorrenteMovimentacao ",
                "       INNER JOIN contacorrenteCategoria ON contacorrentemov_categoriaId=cccategoria_id ",
                "   WHERE contacorrentemov_produtorId=", produtorId,
                " AND contacorrentemov_data BETWEEN '", de.ToString(EntityBase.DateTimeFormat), "' AND '", ate.ToString(EntityBase.DateTimeFormat), "' ORDER BY contacorrentemov_data");

            return LocatorHelper.Instance.ExecuteQuery<MovimentacaoContaCorrente>(qry, typeof(MovimentacaoContaCorrente));
        }
        public static IList<MovimentacaoContaCorrente> Carregar(Object produtorId, Int32 mes, Int32 ano)
        {
            String qry = String.Concat("contacorrenteMovimentacao.*, cccategoria_descricao, cccategoria_tipo",
                "   FROM contacorrenteMovimentacao ",
                "       INNER JOIN contacorrenteCategoria ON contacorrentemov_categoriaId=cccategoria_id ",
                "   WHERE contacorrentemov_produtorId=", produtorId,
                " AND MONTH(contacorrentemov_data)=", mes, " AND YEAR(contacorrentemov_data)=", ano, " ORDER BY contacorrentemov_data");

            return LocatorHelper.Instance.ExecuteQuery<MovimentacaoContaCorrente>(qry, typeof(MovimentacaoContaCorrente));
        }

        /// <summary>
        /// Carrega as Movimentações de um Produtor em uma determinada Listagem.
        /// </summary>
        /// <param name="ProdutorID">ID do Produtor.</param>
        /// <param name="ListagemID">ID da Listagem.</param>
        /// <returns>Retorna uma lista de Movimentações preenchidas.</returns>
        public static IList<MovimentacaoContaCorrente> Carregar(Object ProdutorID, Object ListagemID)
        {
            if (ProdutorID != null && ListagemID != null)
            {
                String[] strParam = new String[2];
                String[] strValue = new String[2];

                strParam[0] = "@produtor_id";
                strParam[1] = "@listagem_id";

                strValue[0] = ProdutorID.ToString();
                strValue[1] = ListagemID.ToString();

                String qry = String.Concat("SELECT ",
                                           "     contacorrenteMovimentacao.*, cccategoria_tipo ",
                                           " FROM contacorrenteMovimentacao INNER JOIN contacorrenteCategoria ON cccategoria_id=contacorrentemov_categoriaId ",
                                           " WHERE contacorrentemov_produtorId = @produtor_id and contacorrentemov_listagemFechamentoId = @listagem_id AND ",
                                           "       contacorrentemov_categoriaId <> ", CategoriaContaCorrente.SysPremiacaoCategoriaID);

                try
                {
                    return LocatorHelper.Instance.ExecuteParametrizedQuery<MovimentacaoContaCorrente>(qry, strParam, strValue, typeof(MovimentacaoContaCorrente));
                }
                catch (Exception) { throw; }
            }
            else
                throw new ArgumentNullException("O ID do Produtor ou ID da Listagem é nulo.");
        }

        public static IList<MovimentacaoContaCorrente> CarregarEmAberto(Object produtorId, DateTime dataCorte, PersistenceManager pm)
        {
            String qry = String.Concat("contacorrenteMovimentacao.*, cccategoria_descricao, cccategoria_tipo",
                "   FROM contacorrenteMovimentacao ",
                "       INNER JOIN contacorrenteCategoria ON contacorrentemov_categoriaId=cccategoria_id ",
                "   WHERE contacorrentemov_listagemFechamentoId IS NULL AND contacorrentemov_produtorId=", produtorId,
                " AND contacorrentemov_data <= '", dataCorte.ToString("yyyy-MM-dd HH:mm:ss"), "' ORDER BY contacorrentemov_data");

            return LocatorHelper.Instance.ExecuteQuery<MovimentacaoContaCorrente>(qry, typeof(MovimentacaoContaCorrente), pm);
        }

        public static Decimal Totalizar(IList<MovimentacaoContaCorrente> lista)
        {
            if (lista == null) { return Decimal.Zero; }
            Decimal total = Decimal.Zero;
            foreach (MovimentacaoContaCorrente mcc in lista)
            {
                if (CategoriaContaCorrente.eTipo.Credito == (CategoriaContaCorrente.eTipo)mcc.CategoriaTipo)
                    total += mcc.Valor;
                else
                    total -= mcc.Valor;
            }
            return total;
        }
    }

    [DBTable("contacorrenteCategoria")]
    public sealed class CategoriaContaCorrente : EntityBase, IPersisteableEntity
    {
        public enum eTipo : int
        {
            Credito,
            Debito
        }

        Object _id;
        String _descricao;
        Int32 _tipo;
        DateTime _data;
        Boolean _sistema;

        #region propriedades 

        [DBFieldInfo("cccategoria_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("cccategoria_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("cccategoria_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [DBFieldInfo("cccategoria_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        /// <summary>
        /// Indica se essa categoria é de sistema ou foi criada por um usuário.
        /// </summary>
        [DBFieldInfo("cccategoria_sistema", FieldType.Single)]
        public Boolean Sistema
        {
            get { return _sistema; }
            set { _sistema= value; }
        }

        public String strTipo
        {
            get
            {
                if (eTipo.Credito == (eTipo)_tipo)
                    return "Crédito";
                else
                    return "Débito";
            }
        }

        /// <summary>
        /// Retorna o id da categoria de sistema para pagamento automatico de comissao.
        /// </summary>
        public static Int32 SysPremiacaoCategoriaID
        {
            get { return 3; }
        }

        #endregion

        public CategoriaContaCorrente(Object id) : this() { _id = id; }
        public CategoriaContaCorrente() { _data = DateTime.Now; _sistema = false; }

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

        public static IList<CategoriaContaCorrente> CarregarTodas()
        {
            String query = "* FROM contacorrenteCategoria ORDER BY cccategoria_tipo, cccategoria_descricao";
            return LocatorHelper.Instance.ExecuteQuery<CategoriaContaCorrente>(query, typeof(CategoriaContaCorrente));
        }
        public static IList<CategoriaContaCorrente> CarregarTodas(eTipo tipo)
        {
            String query = "* FROM contacorrenteCategoria WHERE cccategoria_tipo=" + Convert.ToInt32(tipo) + " ORDER BY cccategoria_tipo, cccategoria_descricao";
            return LocatorHelper.Instance.ExecuteQuery<CategoriaContaCorrente>(query, typeof(CategoriaContaCorrente));
        }
    }
}