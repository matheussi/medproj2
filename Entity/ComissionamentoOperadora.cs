namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("comissao_operadora")]
    public class ComissionamentoOperadora : EntityBase, IPersisteableEntity
    {
        Object   _id;
        Object   _contratoAdmId;
        //String   _descricao;
        DateTime _data;
        Boolean  _ativa;

        Boolean _vitalicia;
        Int32   _vitaliciaNumeroParcela;
        Decimal _vitaliciaPercentual;

        String _estipulanteDescricao;
        String _contratoAdmNumero;
        String _contratoAdmDescricao;

        #region properties 

        [DBFieldInfo("comissaooperadora_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("comissaooperadora_contratoAdmId", FieldType.Single)]
        public Object ContratoAdmID
        {
            get { return _contratoAdmId; }
            set { _contratoAdmId= value; }
        }

        //[DBFieldInfo("comissaooperadora_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _contratoAdmDescricao; }
            //set { _descricao= value; }
        }

        [DBFieldInfo("comissaooperadora_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [DBFieldInfo("comissaooperadora_ativa", FieldType.Single)]
        public Boolean Ativa
        {
            get { return _ativa; }
            set { _ativa= value; }
        }

        //[DBFieldInfo("comissaooperadora_vitalicia", FieldType.Single)]
        //public Boolean Vitalicia
        //{
        //    get { return _vitalicia; }
        //    set { _vitalicia = value; }
        //}

        //[DBFieldInfo("comissaooperadora_vitaliciaNumParcela", FieldType.Single)]
        //public int VitaliciaNumeroParcela
        //{
        //    get { return _vitaliciaNumeroParcela; }
        //    set { _vitaliciaNumeroParcela = value; }
        //}

        //[DBFieldInfo("comissaooperadora_vitaliciaPercentual", FieldType.Single)]
        //public Decimal VitaliciaPercentual
        //{
        //    get { return _vitaliciaPercentual; }
        //    set { _vitaliciaPercentual = value; }
        //}

        [Joinned("estipulante_descricao")]
        public String EstipulanteDescricao
        {
            get { return _estipulanteDescricao; }
            set { _estipulanteDescricao= value; }
        }

        [Joinned("contratoadm_descricao")]
        public String ContratoAdmDescricao
        {
            get { return _contratoAdmDescricao; }
            set { _contratoAdmDescricao= value; }
        }

        [Joinned("contratoadm_numero")]
        public String ContratoAdmNumero
        {
            get { return _contratoAdmNumero; }
            set { _contratoAdmNumero= value; }
        }

        #endregion

        public ComissionamentoOperadoraVitaliciedade ComissionamentoOperadoraVitaliciedade
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public ComissionamentoOperadoraItem ComissionamentoOperadoraItem
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public ComissionamentoOperadora() { }
        public ComissionamentoOperadora(Object id) { this._id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.TransactionContext();
            String query = "DELETE FROM comissao_operadora_item WHERE comissaooperadoraitem_comissaoid=" + this._id;

            try
            {
                NonQueryHelper.Instance.ExecuteNonQuery(query, pm);

                query = "DELETE FROM comissao_operadora_vitaliciedade WHERE cov_comissaoId=" + this._id;
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
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<ComissionamentoOperadora> CarregarTodos()
        {
            String sql = "comissao_operadora.*, contratoadm_descricao FROM comissao_operadora INNER JOIN contratoADM ON comissaooperadora_contratoAdmId = contratoadm_id ORDER BY comissaooperadora_descricao, comissaooperadora_data";
            return LocatorHelper.Instance.ExecuteQuery
                <ComissionamentoOperadora>(sql, typeof(ComissionamentoOperadora));
        }

        public static IList<ComissionamentoOperadora> CarregarPorContratoId(Object contratoAdmId)
        {
            return CarregarPorContratoId(contratoAdmId, null);
        }

        public static IList<ComissionamentoOperadora> CarregarPorContratoId(Object contratoAdmId, PersistenceManager pm)
        {
            String query = String.Concat("comissao_operadora.*, contratoadm_descricao, contratoadm_numero, estipulante_descricao FROM comissao_operadora INNER JOIN contratoADM ON comissaooperadora_contratoAdmId = contratoadm_id INNER JOIN estipulante ON contratoadm_estipulanteId=estipulante_id WHERE comissaooperadora_contratoAdmId=", contratoAdmId, " ORDER BY comissaooperadora_data DESC");
            return LocatorHelper.Instance.ExecuteQuery<ComissionamentoOperadora>(query, typeof(ComissionamentoOperadora), pm);
        }

        public static ComissionamentoOperadora CarregarAtualPorContratoId(Object contratoAdmId, PersistenceManager pm)
        {
            String query = String.Concat(" TOP 1 comissao_operadora.*, contratoadm_descricao, contratoadm_numero, estipulante_descricao FROM comissao_operadora INNER JOIN contratoADM ON comissaooperadora_contratoAdmId = contratoadm_id INNER JOIN estipulante ON contratoadm_estipulanteId=estipulante_id WHERE comissaooperadora_contratoAdmId=", contratoAdmId, " ORDER BY comissaooperadora_data DESC");
            IList<ComissionamentoOperadora> list = LocatorHelper.Instance.ExecuteQuery<ComissionamentoOperadora>(query, typeof(ComissionamentoOperadora), pm);

            if (list == null)
                return null;
            else
                return list[0];
        }

        public static Boolean ExisteTabela(Object contratoAdmId, Object tabelaId, DateTime data)
        {
            String query = "SELECT COUNT(*) FROM comissao_operadora WHERE comissaooperadora_contratoAdmId=@ContratoID AND CONVERT(VARCHAR(20), comissaooperadora_data, 103)=@Data";
            if (tabelaId != null)
            {
                query += " AND comissaooperadora_id <> " + tabelaId;
            }

            Int32 result = Convert.ToInt32(LocatorHelper.Instance.ExecuteScalar(query, new String[] { "@ContratoID", "@Data" }, new String[] { Convert.ToString(contratoAdmId), data.ToString("dd/MM/yyyy") }));

            return result > 0;
        }
    }

    [Serializable]
    [DBTable("comissao_operadora_item")]
    public class ComissionamentoOperadoraItem : EntityBase, IPersisteableEntity
    {
        #region campos 

        Object _id;
        Object _comissionamentoId;
        Object _contratoId;
        Int32 _parcela;
        Decimal _percentual;
        Decimal _percentualCompraCarencia;
        Decimal _percentualMigracao;
        Decimal _percentualADM;
        Decimal _percentualEspecial;
        Decimal _idade;

        #endregion

        #region propriedades 

        [DBFieldInfo("comissaooperadoraitem_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("comissaooperadoraitem_comissaoid", FieldType.Single)]
        public Object ComissionamentoID
        {
            get { return _comissionamentoId; }
            set { _comissionamentoId= value; }
        }

        [DBFieldInfo("comissaooperadoraitem_contratoId", FieldType.Single)]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId= value; }
        }

        [DBFieldInfo("comissaooperadoraitem_parcela", FieldType.Single)]
        public Int32 Parcela
        {
            get { return _parcela; }
            set { _parcela= value; }
        }

        [DBFieldInfo("comissaooperadoraitem_percentual", FieldType.Single)]
        public Decimal Percentual
        {
            get { return _percentual; }
            set { _percentual= value; }
        }

        [DBFieldInfo("comissaooperadoraitem_percentualCompraCarencia", FieldType.Single)]
        public Decimal PercentualCompraCarencia
        {
            get { return _percentualCompraCarencia; }
            set { _percentualCompraCarencia= value; }
        }

        [DBFieldInfo("comissaooperadoraitem_percentualMigracao", FieldType.Single)]
        public Decimal PercentualMigracao
        {
            get { return _percentualMigracao; }
            set { _percentualMigracao= value; }
        }

        [DBFieldInfo("comissaooperadoraitem_percentualADM", FieldType.Single)]
        public Decimal PercentualADM
        {
            get { return _percentualADM; }
            set { _percentualADM= value; }
        }

        [DBFieldInfo("comissaooperadoraitem_percentualEspecial", FieldType.Single)]
        public Decimal PercentualEspecial
        {
            get { return _percentualEspecial; }
            set { _percentualEspecial= value; }
        }

        [DBFieldInfo("comissaooperadoraitem_idade", FieldType.Single)]
        public Decimal PercentualIdade
        {
            get { return _idade; }
            set { _idade= value; }
        }

        #endregion

        public ComissionamentoOperadoraItem() { }
        public ComissionamentoOperadoraItem(Object id) { this._id = id; }

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

        public static IList<ComissionamentoOperadoraItem> Carregar(Object comissionamentoId)
        {
            String query = "* FROM comissao_operadora_item WHERE comissaooperadoraitem_comissaoid=" + comissionamentoId + " ORDER BY comissaooperadoraitem_parcela";
            return LocatorHelper.Instance.ExecuteQuery<ComissionamentoOperadoraItem>(query, typeof(ComissionamentoOperadoraItem));
        }

        public static ComissionamentoOperadoraItem Carregar(Object comissionamentoId, Int32 parcela, PersistenceManager pm)
        {
            String query = "* FROM comissao_operadora_item WHERE comissaooperadoraitem_parcela=" + parcela .ToString() + " AND comissaooperadoraitem_comissaoid=" + comissionamentoId + " ORDER BY comissaooperadoraitem_parcela";
            IList<ComissionamentoOperadoraItem> list = LocatorHelper.Instance.ExecuteQuery<ComissionamentoOperadoraItem>(query, typeof(ComissionamentoOperadoraItem), pm);
            if (list == null)
                return null;
            else
                return list[0];
        }
    }

    [Serializable]
    [DBTable("comissao_operadora_vitaliciedade")]
    public class ComissionamentoOperadoraVitaliciedade : EntityBase, IPersisteableEntity
    {
        #region campos 

        Object _id;
        Object _tabelaId;
        Int32 _tipoColunaComissao;
        Boolean _vitalicia;
        Int32 _vitaliciaNumeroParcela;
        Decimal _vitaliciaPercentual;

        #endregion

        #region propriedades 

        [DBFieldInfo("cov_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("cov_comissaoId", FieldType.Single)]
        public Object TabelaID
        {
            get { return _tabelaId; }
            set { _tabelaId= value; }
        }

        [DBFieldInfo("cov_tipoColunaComissao", FieldType.Single)]
        public Int32 TipoColunaComissao
        {
            get { return _tipoColunaComissao; }
            set { _tipoColunaComissao= value; }
        }

        [DBFieldInfo("cov_vitalicia", FieldType.Single)]
        public Boolean Vitalicia
        {
            get { return _vitalicia; }
            set { _vitalicia= value; }
        }

        [DBFieldInfo("cov_parcelaInicio", FieldType.Single)]
        public Int32 ParcelaInicio
        {
            get { return _vitaliciaNumeroParcela; }
            set { _vitaliciaNumeroParcela= value; }
        }

        [DBFieldInfo("cov_percentual", FieldType.Single)]
        public Decimal Percentual
        {
            get { return _vitaliciaPercentual; }
            set { _vitaliciaPercentual= value; }
        }

        #endregion

        public ComissionamentoOperadoraVitaliciedade() { }
        public ComissionamentoOperadoraVitaliciedade(Object id) { this._id = id; }

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

        public static ComissionamentoOperadoraVitaliciedade Carregar(Object comissionamentoId, TipoContrato.TipoComissionamentoProdutorOuOperadora tipo)
        {
            String sql = "* FROM comissao_operadora_vitaliciedade WHERE cov_comissaoId=" + comissionamentoId + " AND cov_tipoColunaComissao=" + Convert.ToInt32(tipo);
            IList<ComissionamentoOperadoraVitaliciedade> lista = LocatorHelper.Instance.ExecuteQuery<ComissionamentoOperadoraVitaliciedade>(sql, typeof(ComissionamentoOperadoraVitaliciedade));
            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }

        public static ComissionamentoOperadoraVitaliciedade Carregar(Object comissionamentoId, TipoContrato.TipoComissionamentoProdutorOuOperadora tipo, Int32 parcelaInicio, PersistenceManager pm)
        {
            String sql = "* FROM comissao_operadora_vitaliciedade WHERE cov_parcelaInicio > 0 AND cov_comissaoId=" + comissionamentoId + " AND cov_tipoColunaComissao=" + Convert.ToInt32(tipo) + " AND cov_parcelaInicio <= " + parcelaInicio.ToString();
            IList<ComissionamentoOperadoraVitaliciedade> lista = LocatorHelper.Instance.ExecuteQuery<ComissionamentoOperadoraVitaliciedade>(sql, typeof(ComissionamentoOperadoraVitaliciedade), pm);
            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }
    }
}