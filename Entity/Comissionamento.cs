namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("comissao_modelo")]
    public class Comissionamento : EntityBase, IPersisteableEntity
    {
        public enum eTipo : int
        {
            RecebidoDaOperadora = 0,
            PagoAoOperador
        }

        Object _id;
        //Object _contratoAdmId;
        Object _grupoId;
        Object _categoriaId;
        String _descricao;
        DateTime _data;
        Int32 _idadeEspecial;
        Int32 _tipo;

        //Boolean _vitalicia;
        //Int32 _vitaliciaNumeroParcela;
        //Decimal _vitaliciaPercentual;

        Object _atualId;
        String _categoriaNome;
        String _categoriaPerfilDescricao;

        #region propriedades 

        [DBFieldInfo("comissaomodelo_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("comissaomodelo_grupoId", FieldType.Single)]
        public Object GrupoID
        {
            get { return _grupoId; }
            set { _grupoId= value; }
        }

        //[DBFieldInfo("comissaomodelo_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _categoriaNome + " - " + _data.ToString("dd/MM/yyyy"); }
            set { _descricao= value; }
        }

        //[DBFieldInfo("comissaomodelo_vitalicia", FieldType.Single)]
        //public Boolean Vitalicia
        //{
        //    get { return _vitalicia; }
        //    set { _vitalicia= value; }
        //}

        //[DBFieldInfo("comissaomodelo_vitaliciaNumParcela", FieldType.Single)]
        //public int VitaliciaNumeroParcela
        //{
        //    get { return _vitaliciaNumeroParcela; }
        //    set { _vitaliciaNumeroParcela= value; }
        //}

        //[DBFieldInfo("comissaomodelo_vitaliciaPercentual", FieldType.Single)]
        //public Decimal VitaliciaPercentual
        //{
        //    get { return _vitaliciaPercentual; }
        //    set { _vitaliciaPercentual= value; }
        //}

        [DBFieldInfo("comissaomodelo_categoriaId", FieldType.Single)]
        public Object CategoriaID
        {
            get { return _categoriaId; }
            set { _categoriaId= value; }
        }

        //[DBFieldInfo("comissaomodelo_contratoAdmId", FieldType.Single)]
        [Obsolete("Em desuso.", false)]
        public Object ContratoAdmID
        {
            get { return null;/*_contratoAdmId;*/ }
            set { /*_contratoAdmId = value;*/ }
        }

        [DBFieldInfo("comissaomodelo_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [DBFieldInfo("comissaomodelo_idadeEspecial", FieldType.Single)]
        public Int32 IdadeEspecial
        {
            get { return _idadeEspecial; }
            set { _idadeEspecial= value; }
        }

        [DBFieldInfo("comissaomodelo_tipo", FieldType.Single)]
        public Int32 Tipo
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [Joinned("contratoadm_tabelaComissionamentoAtivaId")]
        public Object AtualID
        {
            get { return _atualId; }
            set { _atualId= value; }
        }

        [Joinned("categoria_descricao")]
        public String CategoriaNome
        {
            get { return _categoriaNome; }
            set { _categoriaNome= value; }
        }

        [Joinned("perfil_descricao")]
        public String Categoria_PerfilDescricao
        {
            get { return _categoriaPerfilDescricao; }
            set { _categoriaPerfilDescricao= value; }
        }
        #endregion

        public ComissionamentoItem ComissionamentoItem
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
        public ComissionamentoItem ComissionamentoItem1
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Comissionamento() { _tipo = (int)eTipo.RecebidoDaOperadora; }
        public Comissionamento(Object id) : this() { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();
            //String query = "DELETE FROM comissao_modelo_item WHERE comissaomodeloitem_grupoId=" + this._id;

            try
            {
                //NonQueryHelper.Instance.ExecuteNonQuery(query, pm);
                //query = "DELETE FROM comissao_modelo_vitaliciedade WHERE comissaovitalicia_comissaoId=" + this.ID;
                //NonQueryHelper.Instance.ExecuteNonQuery(query, pm);
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

        public static IList<Comissionamento> CarregarTodos(eTipo tipo)
        {
            String query = "comissao_modelo.*, contratoadm_tabelaComissionamentoAtivaId, categoria_descricao, perfil_descricao FROM comissao_modelo LEFT JOIN categoria ON comissaomodelo_categoriaId = categoria_id LEFT JOIN perfil ON categoria_perfilId=perfil_id LEFT JOIN contratoADM ON comissaomodelo_id=contratoadm_tabelaComissionamentoAtivaId WHERE comissaomodelo_tipo=" + Convert.ToInt32(tipo) + " ORDER BY comissaomodelo_descricao";
            return LocatorHelper.Instance.ExecuteQuery<Comissionamento>(query, typeof(Comissionamento));
        }

        public static IList<Comissionamento> CarregarTodos(eTipo tipo, Object perfilId)
        {
            String query = "comissao_modelo.*, contratoadm_tabelaComissionamentoAtivaId, categoria_descricao, perfil_descricao FROM comissao_modelo LEFT JOIN categoria ON comissaomodelo_categoriaId = categoria_id LEFT JOIN perfil ON categoria_perfilId=perfil_id LEFT JOIN contratoADM ON comissaomodelo_id=contratoadm_tabelaComissionamentoAtivaId WHERE comissaomodelo_tipo=" + Convert.ToInt32(tipo) + " AND categoria_perfilId=" + perfilId + " ORDER BY comissaomodelo_descricao";
            return LocatorHelper.Instance.ExecuteQuery<Comissionamento>(query, typeof(Comissionamento));
        }

        public static IList<Comissionamento> CarregarPorContratoId(Object contratoAdmId, eTipo tipo)
        {
            String query = String.Concat("comissao_modelo.*, contratoadm_tabelaComissionamentoAtivaId, categoria_descricao FROM comissao_modelo LEFT JOIN categoria ON comissaomodelo_categoriaId = categoria_id LEFT JOIN contratoADM ON comissaomodelo_id=contratoadm_tabelaComissionamentoAtivaId WHERE comissaomodelo_tipo=", Convert.ToInt32(tipo), " AND comissaomodelo_contratoAdmId=", contratoAdmId, " ORDER BY comissaomodelo_descricao");
            return LocatorHelper.Instance.ExecuteQuery<Comissionamento>(query, typeof(Comissionamento));
        }

        public static Boolean ExisteTabelaComVigenciaInformada(Object categoriaId, DateTime vigencia, Object tabelaId)
        {
            String query = "SELECT COUNT(*) FROM comissao_modelo WHERE comissaomodelo_categoriaId=@CategID AND CONVERT(VARCHAR(20), comissaomodelo_data, 103)=@Data";
            if (tabelaId != null)
            {
                query += " AND comissaomodelo_id <> " + tabelaId;
            }

            Int32 result = Convert.ToInt32(LocatorHelper.Instance.ExecuteScalar(query, new String[] 
            { "@CategID", "@Data" }, new String[] { Convert.ToString(categoriaId), vigencia.ToString("dd/MM/yyyy") }));

            return result > 0;
        }
    }

    [Serializable]
    [DBTable("comissao_modelo_item")]
    public class ComissionamentoItem : EntityBase, IPersisteableEntity, IComissionamentoItem
    {
        Object _id;
        Object _comissionamentoId;
        //Object _contratoId;
        Int32 _parcela;
        Decimal _percentual;
        Decimal _percentualCompraCarencia;
        Decimal _percentualMigracao;
        Decimal _percentualADM;
        Decimal _percentualEspecial;
        Decimal _percentualIdade;

        #region propriedades 

        [DBFieldInfo("comissaomodeloitem_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        /// <summary>
        /// ID do grupo de comissionamento à qual pertence este item.
        /// </summary>
        [DBFieldInfo("comissaomodeloitem_grupoId", FieldType.Single)]
        public Object OwnerID
        {
            get { return _comissionamentoId; }
            set { _comissionamentoId= value; }
        }

        //[DBFieldInfo("comissaomodeloitem_contratoid", FieldType.Single)]
        //public Object ContratoID
        //{
        //    get { return _contratoId; }
        //    set { _contratoId= value; }
        //}

        [DBFieldInfo("comissaomodeloitem_parcela", FieldType.Single)]
        public Int32 Parcela
        {
            get { return _parcela; }
            set { _parcela= value; }
        }

        [DBFieldInfo("comissaomodeloitem_percentual", FieldType.Single)]
        public Decimal Percentual
        {
            get { return _percentual; }
            set { _percentual= value; }
        }

        [DBFieldInfo("comissaomodeloitem_percentualCompraCarencia", FieldType.Single)]
        public Decimal PercentualCompraCarencia
        {
            get { return _percentualCompraCarencia; }
            set { _percentualCompraCarencia= value; }
        }

        [DBFieldInfo("comissaomodeloitem_percentualMigracao", FieldType.Single)]
        public Decimal PercentualMigracao
        {
            get { return _percentualMigracao; }
            set { _percentualMigracao= value; }
        }

        [DBFieldInfo("comissaomodeloitem_percentualADM", FieldType.Single)]
        public Decimal PercentualADM
        {
            get { return _percentualADM; }
            set { _percentualADM= value; }
        }

        [DBFieldInfo("comissaomodeloitem_percentualEspecial", FieldType.Single)]
        public Decimal PercentualEspecial
        {
            get { return _percentualEspecial; }
            set { _percentualEspecial= value; }
        }

        [DBFieldInfo("comissaomodeloitem_idade", FieldType.Single)]
        public Decimal Idade
        {
            get { return _percentualIdade; }
            set { _percentualIdade= value; }
        }

        #endregion

        public ComissionamentoItem() { }

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

        public static IList<ComissionamentoItem> CarregarPorGrupoID(Object grupoId)
        {
            String query = "* FROM comissao_modelo_item WHERE comissaomodeloitem_grupoId=" + grupoId + " ORDER BY comissaomodeloitem_parcela";
            return LocatorHelper.Instance.ExecuteQuery<ComissionamentoItem>(query, typeof(ComissionamentoItem));
        }

        public static IList<ComissionamentoItem> Carregar(Object grupoId)
        {
            return Carregar(grupoId, null);
        }
        public static IList<ComissionamentoItem> Carregar(Object grupoId, PersistenceManager pm)
        {
            String query = "* FROM comissao_modelo_item WHERE comissaomodeloitem_grupoId=" + grupoId + " ORDER BY comissaomodeloitem_parcela";
            return LocatorHelper.Instance.ExecuteQuery<ComissionamentoItem>(query, typeof(ComissionamentoItem), pm);
        }
    }

    [DBTable("comissao_modelo_vitaliciedade")]
    public class ComissionamentoVitaliciedade : EntityBase, IPersisteableEntity
    {
        Object  _id;
        Object  _grupoId;
        Int32   _tipoColunaComissao;
        Boolean _vitalicia;
        Int32   _vitaliciaNumeroParcela;
        Decimal _vitaliciaPercentual;

        #region propeties 

        [DBFieldInfo("comissaovitalicia_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("comissaovitalicia_grupoId", FieldType.Single)]
        public Object GrupoID
        {
            get { return _grupoId; }
            set { _grupoId= value; }
        }

        //[DBFieldInfo("comissaovitalicia_contratoId", FieldType.Single)]
        //public Object ContratoID
        //{
        //    get { return _contratoId; }
        //    set { _contratoId= value; }
        //}

        [DBFieldInfo("comissaovitalicia_tipoColunaComissao", FieldType.Single)]
        public Int32 TipoColunaComissao
        {
            get { return _tipoColunaComissao; }
            set { _tipoColunaComissao= value; }
        }

        [DBFieldInfo("comissaovitalicia_vitalicia", FieldType.Single)]
        public Boolean Vitalicia
        {
            get { return _vitalicia; }
            set { _vitalicia= value; }
        }

        [DBFieldInfo("comissaovitalicia_parcelaInicio", FieldType.Single)]
        public Int32 ParcelaInicio
        {
            get { return _vitaliciaNumeroParcela; }
            set { _vitaliciaNumeroParcela= value; }
        }

        [DBFieldInfo("comissaovitalicia_percentual", FieldType.Single)]
        public Decimal Percentual
        {
            get { return _vitaliciaPercentual; }
            set { _vitaliciaPercentual= value; }
        }

        #endregion

        public ComissionamentoVitaliciedade() { _vitaliciaNumeroParcela = 0; _vitalicia = false; _vitaliciaPercentual = 0; }
        public ComissionamentoVitaliciedade(Object id) : this() { this._id = id; }

        #region base methods 

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

        public static ComissionamentoVitaliciedade Carregar(Object grupoId, TipoContrato.TipoComissionamentoProdutorOuOperadora tipo)
        {
            return Carregar(grupoId, tipo, null);
        }
        public static ComissionamentoVitaliciedade Carregar(Object grupoId, TipoContrato.TipoComissionamentoProdutorOuOperadora tipo, PersistenceManager pm)
        {
            String sql = "* FROM comissao_modelo_vitaliciedade WHERE comissaovitalicia_grupoId=" + grupoId + " AND comissaovitalicia_tipoColunaComissao=" + Convert.ToInt32(tipo);
            IList<ComissionamentoVitaliciedade> lista = LocatorHelper.Instance.ExecuteQuery<ComissionamentoVitaliciedade>(sql, typeof(ComissionamentoVitaliciedade), pm);
            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }
    }

    [DBTable("comissao_modelo_idade")]
    public class ComissionamentoIdade : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _comissionamentoId;
        Int32 _idade;

        #region propriedades 

        [DBFieldInfo("comissaoidade_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("comissaoidade_comissaoid", FieldType.Single)]
        public Object ComissionamentoID
        {
            get { return _comissionamentoId; }
            set { _comissionamentoId= value; }
        }

        [DBFieldInfo("comissaoidade_idade", FieldType.Single)]
        public int Idade
        {
            get { return _idade; }
            set { _idade= value; }
        }

        public String Resumo
        {
            get
            {
                return String.Concat("Condição para idade acima de ", _idade, " anos");
            }
        }

        #endregion

        public ComissionamentoIdade() {}
        public ComissionamentoIdade(Object id) { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();
            String query = "DELETE FROM comissao_modelo_idade_item WHERE comissaoidadeitem_comissaoidadeid=" + this.ID;

            try
            {
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

        public static IList<ComissionamentoIdade> Carregar(Object comissaoModeloId)
        {
            String query = "* FROM comissao_modelo_idade WHERE comissaoidade_comissaoid=" + comissaoModeloId;
            return LocatorHelper.Instance.ExecuteQuery<ComissionamentoIdade>(query, typeof(ComissionamentoIdade));
        }
    }

    [Serializable()]
    [DBTable("comissao_modelo_idade_item")]
    public class ComissionamentoIdadeItem : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _comissionamentoIdadeId;
        Int32 _parcela;
        Decimal _percentual;

        #region propriedades 

        [DBFieldInfo("comissaoidadeitem_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("comissaoidadeitem_comissaoidadeid", FieldType.Single)]
        public Object ComissionamentoIdadeID
        {
            get { return _comissionamentoIdadeId; }
            set { _comissionamentoIdadeId= value; }
        }

        [DBFieldInfo("comissaoidadeitem_parcela", FieldType.Single)]
        public int Parcela
        {
            get { return _parcela; }
            set { _parcela= value; }
        }

        [DBFieldInfo("comissaoidadeitem_percentual", FieldType.Single)]
        public Decimal Percentual
        {
            get { return _percentual; }
            set { _percentual= value; }
        }

        #endregion

        public ComissionamentoIdadeItem() { }
        public ComissionamentoIdadeItem(Object id) { _id = id; }

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

        public static IList<ComissionamentoIdadeItem> Carregar(Object comissaoModeloIdadeId)
        {
            String query = "* FROM comissao_modelo_idade_item WHERE comissaoidadeitem_comissaoidadeid=" + comissaoModeloIdadeId;
            return LocatorHelper.Instance.ExecuteQuery<ComissionamentoIdadeItem>(query, typeof(ComissionamentoIdadeItem));
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////

    [DBTable("comissaoGrupo")]
    public class ComissionamentoGrupo : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _tabelaId;
        String _descricao;

        Decimal _totalNormal;

        #region propriedades 

        [DBFieldInfo("comissaogrupo_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("comissaogrupo_tabelaComissaoId", FieldType.Single)]
        public Object TabelaComissionamentoID
        {
            get { return _tabelaId; }
            set { _tabelaId= value; }
        }

        [DBFieldInfo("comissaogrupo_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [Joinned("TotalNormal")]
        public Decimal TotalNormal
        {
            get { return _totalNormal; }
            set { _totalNormal= value; }
        }

        #endregion

        public ComissionamentoGrupo() { }
        public ComissionamentoGrupo(Object id) { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                String cmd = "DELETE FROM comissao_modelo_item WHERE comissaomodeloitem_grupoId=" + this._id;
                NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm);
                cmd = "DELETE FROM comissaoGrupo_contratoExcluido WHERE grupoId=" + this._id;
                NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm);
                cmd = "DELETE FROM comissao_modelo_vitaliciedade WHERE comissaovitalicia_grupoId=" + this._id;
                NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm);

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

        public static IList<ComissionamentoGrupo> CarregarTodos(Object tabelaId)
        {
            return CarregarTodos(tabelaId, null);
        }
        public static IList<ComissionamentoGrupo> CarregarTodos(Object tabelaId, PersistenceManager pm)
        {
            String query = "comissaoGrupo.*, SUM(comissaomodeloitem_percentual) as TotalNormal FROM comissaoGrupo LEFT JOIN comissao_modelo_item ON comissaogrupo_id=comissaomodeloitem_grupoId WHERE comissaogrupo_tabelaComissaoId=" + tabelaId + " GROUP BY comissaogrupo_id,comissaogrupo_tabelaComissaoId,comissaogrupo_descricao ORDER BY comissaogrupo_descricao";
            return LocatorHelper.Instance.ExecuteQuery<ComissionamentoGrupo>(query, typeof(ComissionamentoGrupo), pm);
        }

        public static Object ObterID(Object tabelaId, Object contratoAdmId, PersistenceManager pm)
        {
            String query = "SELECT contratoadmgrupo_grupoId FROM contratoAdmGrupo WHERE contratoadmgrupo_tabelaId=" + tabelaId + " AND contratoadmgrupo_contratoAdmId=" + contratoAdmId;
            return LocatorHelper.Instance.ExecuteScalar(query, null, null, pm);
        }
    }

    #region ComissionamentoGrupoItem - comentado 
    //[Serializable]
    //[DBTable("comissaoGrupo_item")]
    //public class ComissionamentoGrupoItem : EntityBase, IPersisteableEntity, IComissionamentoItem
    //{
    //    Object _id;
    //    Object _grupoId;
    //    Object _contratoId;
    //    Int32 _parcela;
    //    Decimal _percentual;
    //    Decimal _percentualCompraCarencia;
    //    Decimal _percentualMigracao;
    //    Decimal _percentualADM;
    //    Decimal _percentualEspecial;
    //    Decimal _percentualIdade;

    //    #region propriedades 

    //    [DBFieldInfo("comissaogrupoitem_id", FieldType.PrimaryKeyAndIdentity)]
    //    public Object ID
    //    {
    //        get { return _id; }
    //        set { _id= value; }
    //    }

    //    /// <summary>
    //    /// ID do grupo ao qual pertence este item.
    //    /// </summary>
    //    [DBFieldInfo("comissaogrupoitem_grupoid", FieldType.Single)]
    //    public Object OwnerID
    //    {
    //        get { return _grupoId; }
    //        set { _grupoId= value; }
    //    }

    //    [DBFieldInfo("comissaogrupoitem_parcela", FieldType.Single)]
    //    public Int32 Parcela
    //    {
    //        get { return _parcela; }
    //        set { _parcela= value; }
    //    }

    //    [DBFieldInfo("comissaogrupoitem_percentual", FieldType.Single)]
    //    public Decimal Percentual
    //    {
    //        get { return _percentual; }
    //        set { _percentual= value; }
    //    }

    //    [DBFieldInfo("comissaogrupoitem_percentualCompraCarencia", FieldType.Single)]
    //    public Decimal PercentualCompraCarencia
    //    {
    //        get { return _percentualCompraCarencia; }
    //        set { _percentualCompraCarencia= value; }
    //    }

    //    [DBFieldInfo("comissaogrupoitem_percentualMigracao", FieldType.Single)]
    //    public Decimal PercentualMigracao
    //    {
    //        get { return _percentualMigracao; }
    //        set { _percentualMigracao= value; }
    //    }

    //    [DBFieldInfo("comissaogrupoitem_percentualADM", FieldType.Single)]
    //    public Decimal PercentualADM
    //    {
    //        get { return _percentualADM; }
    //        set { _percentualADM= value; }
    //    }

    //    [DBFieldInfo("comissaogrupoitem_percentualEspecial", FieldType.Single)]
    //    public Decimal PercentualEspecial
    //    {
    //        get { return _percentualEspecial; }
    //        set { _percentualEspecial= value; }
    //    }

    //    [DBFieldInfo("comissaogrupoitem_idade", FieldType.Single)]
    //    public Decimal Idade
    //    {
    //        get { return _percentualIdade; }
    //        set { _percentualIdade= value; }
    //    }

    //    #endregion

    //    public ComissionamentoGrupoItem() {}

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

    //    public static IList<ComissionamentoGrupoItem> CarregarPorGrupoID(Object grupoId)
    //    {
    //        String query = "* FROM comissaoGrupo_item WHERE comissaogrupoitem_grupoid=" + grupoId + " ORDER BY comissaogrupoitem_parcela";
    //        return LocatorHelper.Instance.ExecuteQuery<ComissionamentoGrupoItem>(query, typeof(ComissionamentoGrupoItem));
    //    }

    //    public static IList<ComissionamentoGrupo> CarregarPorTabelaComissionamento(Object comissionamentoId, Object contratoId, PersistenceManager pm)
    //    {
    //        String query = String.Concat("comissaoGrupo_item.* ",
    //            "   FROM comissaoGrupo_item ",
    //            "       INNER JOIN comissao_modelo ON comissaomodelo_grupoId = comissaogrupoitem_grupoid ",
    //            "   WHERE ",
    //            "       comissaomodeloitem_comissaomodeloid=", comissionamentoId, " AND ",
    //            "       comissaomodeloitem_contratoid=", contratoId,
    //            "   ORDER BY comissaomodeloitem_parcela");

    //        return LocatorHelper.Instance.ExecuteQuery<ComissionamentoGrupo>(query, typeof(ComissionamentoGrupo), pm);
    //    }

    //    public static IList<ComissionamentoGrupo> CarregarPorTabelaComissionamento(Object comissionamentoId, PersistenceManager pm)
    //    {
    //        String query = String.Concat("comissaoGrupo_item.* ",
    //            "   FROM comissaoGrupo_item ",
    //            "       INNER JOIN comissao_modelo ON comissaomodelo_grupoId = comissaogrupoitem_grupoid ",
    //            "   WHERE ",
    //            "       comissaomodeloitem_comissaomodeloid=", comissionamentoId,
    //            "   ORDER BY comissaomodeloitem_parcela");

    //        return LocatorHelper.Instance.ExecuteQuery<ComissionamentoGrupo>(query, typeof(ComissionamentoGrupo), pm);
    //    }
    //}
    #endregion


    ///////////////////////////////////////////////////////////////////////////////////////

    [DBTable("tabela_excecao")]
    public class TabelaExcecao : EntityBase, IPersisteableEntity
    {
        Object      _id;
        Object      _contratoAdmId;
        Object      _produtorId;
        Object      _tabelaComissionamentoId;
        DateTime    _vigencia;

        Object      _operadoraId;
        String      _operadoraNome;
        String      _contratoAdmDescricao;

        #region propriedades 

        [DBFieldInfo("tabelaexcecao_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("tabelaexcecao_tabelaComissaoId", FieldType.Single)]
        public Object TabelaComissionamentoID
        {
            get { return _tabelaComissionamentoId; }
            set { _tabelaComissionamentoId= value; }
        }

        [DBFieldInfo("tabelaexcecao_contratoAdmId", FieldType.Single)]
        public Object ContratoAdmID
        {
            get { return _contratoAdmId; }
            set { _contratoAdmId= value; }
        }

        [DBFieldInfo("tabelaexcecao_produtorId", FieldType.Single)]
        public Object ProdutorID
        {
            get { return _produtorId; }
            set { _produtorId= value; }
        }

        [DBFieldInfo("tabelaexcecao_vigencia", FieldType.Single)]
        public DateTime Vigencia
        {
            get { return _vigencia; }
            set { _vigencia= value; }
        }

        [Joinned("operadora_id")]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId = value; }
        }

        [Joinned("operadora_nome")]
        public String OperadoraNome
        {
            get { return _operadoraNome; }
            set { _operadoraNome= value; }
        }

        [Joinned("contratoadm_descricao")]
        public String ContratoAdmDescricao
        {
            get { return _contratoAdmDescricao; }
            set { _contratoAdmDescricao= value; }
        }

        #endregion

        public TabelaExcecao() { }
        public TabelaExcecao(Object id) { _id = id; }

        #region métodos EntityBase 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Remover()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.TransactionContext();
            String query = "DELETE FROM tabela_excecao_item WHERE excecaoitem_tabelaid=" + this.ID;

            try
            {
                NonQueryHelper.Instance.ExecuteNonQuery(query, pm);
                query = "DELETE FROM tabela_excecao_vitaliciedade WHERE tabelaexcecaovitalicia_tabelaExcecaoId=" + this.ID;
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

        public static IList<TabelaExcecao> Carregar(Object tabelaComissionamentoId, Object produtorId, DateTime vigencia)
        {
            return Carregar(tabelaComissionamentoId, produtorId, null, vigencia, null);
        }

        public static IList<TabelaExcecao> Carregar(Object tabelaComissionamentoId, Object produtorId, Object contratoAdmId, DateTime vigencia, PersistenceManager pm)
        {
            String contratoAdmCond = "";
            if (contratoAdmId != null)
            {
                contratoAdmCond = " AND tabelaexcecao_contratoadmid=" + contratoAdmId;
            }

            String query = String.Concat("TOP 1 tabelaexcecao_id FROM tabela_excecao ",
                "WHERE tabelaexcecao_vigencia <= '", vigencia.ToString("yyyy-MM-dd"), "' AND tabelaexcecao_produtorId=", produtorId, " AND tabelaexcecao_tabelaComissaoId=", tabelaComissionamentoId, contratoAdmCond,
                " ORDER BY tabelaexcecao_vigencia DESC");

            return LocatorHelper.Instance.ExecuteQuery<TabelaExcecao>(query, typeof(TabelaExcecao), pm);
        }

        public static IList<TabelaExcecao> Carregar(Object tabelaComissionamentoId, Object produtorId)
        {
            return Carregar(tabelaComissionamentoId, produtorId, null);
        }

        public static IList<TabelaExcecao> Carregar(Object tabelaComissionamentoId, Object produtorId, PersistenceManager pm)
        {
            String query = String.Concat("tabela_excecao.*, operadora_id, operadora_nome, contratoadm_descricao ",
                "   FROM operadora ",
                "       INNER JOIN contratoAdm ON operadora_id=contratoadm_operadoraid ",
                "       INNER JOIN tabela_excecao ON tabelaexcecao_contratoAdmId=contratoadm_id ",
                "   WHERE tabelaexcecao_produtorId=", produtorId, " AND tabelaexcecao_tabelaComissaoId=", tabelaComissionamentoId);

            return LocatorHelper.Instance.ExecuteQuery<TabelaExcecao>(query, typeof(TabelaExcecao), pm);
        }

        public static IList<TabelaExcecao> Carregar(Object produtorId)
        {
            String query = String.Concat("tabela_excecao.*, operadora_id, operadora_nome, contratoadm_descricao ",
                "   FROM operadora ",
                "       INNER JOIN contratoAdm ON operadora_id=contratoadm_operadoraid ",
                "       INNER JOIN tabela_excecao ON tabelaexcecao_contratoAdmId=contratoadm_id ",
                "   WHERE tabelaexcecao_produtorId=", produtorId);

            return LocatorHelper.Instance.ExecuteQuery<TabelaExcecao>(query, typeof(TabelaExcecao));
        }
    }

    [Serializable]
    [DBTable("tabela_excecao_item")]
    public class ExcecaoItem : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _tabelaExcecaoId;
        Int32 _parcela;
        Decimal _percentual;
        Decimal _percentualCompraCarencia;
        Decimal _percentualMigracao;
        Decimal _percentualADM;
        Decimal _percentualEspecial;
        Decimal _percentualIdade;

        #region propriedades

        [DBFieldInfo("excecaoitem_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("excecaoitem_tabelaid", FieldType.Single)]
        public Object TabelaExcecaoID
        {
            get { return _tabelaExcecaoId; }
            set { _tabelaExcecaoId= value; }
        }

        [DBFieldInfo("excecaoitem_parcela", FieldType.Single)]
        public Int32 Parcela
        {
            get { return _parcela; }
            set { _parcela= value; }
        }

        [DBFieldInfo("excecaoitem_percentual", FieldType.Single)]
        public Decimal Percentual
        {
            get { return _percentual; }
            set { _percentual= value; }
        }

        [DBFieldInfo("excecaoitem_percentualCompraCarencia", FieldType.Single)]
        public Decimal PercentualCompraCarencia
        {
            get { return _percentualCompraCarencia; }
            set { _percentualCompraCarencia= value; }
        }

        [DBFieldInfo("excecaoitem_percentualMigracao", FieldType.Single)]
        public Decimal PercentualMigracao
        {
            get { return _percentualMigracao; }
            set { _percentualMigracao= value; }
        }

        [DBFieldInfo("excecaoitem_percentualADM", FieldType.Single)]
        public Decimal PercentualADM
        {
            get { return _percentualADM; }
            set { _percentualADM= value; }
        }

        [DBFieldInfo("excecaoitem_percentualEspecial", FieldType.Single)]
        public Decimal PercentualEspecial
        {
            get { return _percentualEspecial; }
            set { _percentualEspecial= value; }
        }

        [DBFieldInfo("excecaoitem_idade", FieldType.Single)]
        public Decimal Idade
        {
            get { return _percentualIdade; }
            set { _percentualIdade= value; }
        }

        #endregion

        public ExcecaoItem() { }

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

        public static IList<ExcecaoItem> CarregarPorTabelaExcecaoID(Object tabelaExcecaoId)
        {
            return CarregarPorTabelaExcecaoID(tabelaExcecaoId, null);
        }

        public static IList<ExcecaoItem> CarregarPorTabelaExcecaoID(Object tabelaExcecaoId, PersistenceManager pm)
        {
            String query = "* FROM tabela_excecao_item WHERE excecaoitem_tabelaid=" + tabelaExcecaoId + " ORDER BY excecaoitem_parcela";
            return LocatorHelper.Instance.ExecuteQuery<ExcecaoItem>(query, typeof(ExcecaoItem), pm);
        }

        public static IList<ExcecaoItem> Carregar(Object produtorId, Object contratoId)
        {
            String query = "* FROM tabela_excecao_item WHERE excecaoitem_produtorId=" + produtorId + " AND excecaoitem_contratoid=" + contratoId + " ORDER BY excecaoitem_parcela";
            return LocatorHelper.Instance.ExecuteQuery<ExcecaoItem>(query, typeof(ExcecaoItem));
        }
    }

    [Serializable]
    [DBTable("tabela_excecao_vitaliciedade")]
    public class TabelaExcecaoVitaliciedade : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _tabelaExcecaoId;
        Int32 _tipoColunaComissao;
        Boolean _vitalicia;
        Int32 _vitaliciaNumeroParcela;
        Decimal _vitaliciaPercentual;

        #region properties 

        [DBFieldInfo("tabelaexcecaovitalicia_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("tabelaexcecaovitalicia_tabelaExcecaoId", FieldType.Single)]
        public Object TabelaExcecaoID
        {
            get { return _tabelaExcecaoId; }
            set { _tabelaExcecaoId= value; }
        }

        [DBFieldInfo("tabelaexcecaovitalicia_tipoColunaComissao", FieldType.Single)]
        public Int32 TipoColunaComissao
        {
            get { return _tipoColunaComissao; }
            set { _tipoColunaComissao= value; }
        }

        [DBFieldInfo("tabelaexcecaovitalicia_vitalicia", FieldType.Single)]
        public Boolean Vitalicia
        {
            get { return _vitalicia; }
            set { _vitalicia= value; }
        }

        [DBFieldInfo("tabelaexcecaovitalicia_parcelaInicio", FieldType.Single)]
        public Int32 ParcelaInicio
        {
            get { return _vitaliciaNumeroParcela; }
            set { _vitaliciaNumeroParcela= value; }
        }

        [DBFieldInfo("tabelaexcecaovitalicia_percentual", FieldType.Single)]
        public Decimal Percentual
        {
            get { return _vitaliciaPercentual; }
            set { _vitaliciaPercentual= value; }
        }

        #endregion

        public TabelaExcecaoVitaliciedade() { _vitaliciaNumeroParcela = 0; _vitalicia = false; _vitaliciaPercentual = 0; }
        public TabelaExcecaoVitaliciedade(Object id) : this() { this._id = id; }

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

        public static TabelaExcecaoVitaliciedade Carregar(Object tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora tipo)
        {
            return Carregar(tabelaId, tipo, null);
        }

        public static TabelaExcecaoVitaliciedade Carregar(Object tabelaId, TipoContrato.TipoComissionamentoProdutorOuOperadora tipo, PersistenceManager pm)
        {
            String sql = "* FROM tabela_excecao_vitaliciedade WHERE tabelaexcecaovitalicia_tabelaExcecaoId=" + tabelaId + " AND tabelaexcecaovitalicia_tipoColunaComissao=" + Convert.ToInt32(tipo);
            IList<TabelaExcecaoVitaliciedade> lista = LocatorHelper.Instance.ExecuteQuery<TabelaExcecaoVitaliciedade>(sql, typeof(TabelaExcecaoVitaliciedade), pm);
            if (lista == null || lista.Count == 0)
                return null;
            else
                return lista[0];
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////

    [DBTable("listagem")]
    public class Listagem : EntityBase, IPersisteableEntity
    {
        Object _id;
        String _nome;
        String _mensagem;
        DateTime _dataCorte;
        DateTime _data;

        #region properties 

        [DBFieldInfo("listagem_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("listagem_nome", FieldType.Single)]
        public String Nome
        {
            get { return _nome; }
            set { _nome= value; }
        }

        [DBFieldInfo("listagem_mensagem", FieldType.Single)]
        public String Mensagem
        {
            get { return _mensagem; }
            set { _mensagem= value; }
        }

        [DBFieldInfo("listagem_dataCorte", FieldType.Single)]
        public DateTime DataCorte
        {
            get { return _dataCorte; }
            set { _dataCorte= value; }
        }

        [DBFieldInfo("listagem_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }
        #endregion

        public Listagem() { _data = DateTime.Now; }
        public Listagem(Object id) : this() { _id = id; }

        #region EntityBase methods 

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

        public static IList<Listagem> CarregaPorParametros(String nome, String[] filiais, String[] operadoras, String[] perfis, DateTime? dataDe, DateTime? dataAte, DateTime? dataCorte, Object produtorId)
        {
            String strWhere = "";
            String[] values = null;
            String[] pnames = null;
            List<String> paramNames = new List<String>();
            List<String> paramValue = new List<String>();
            String dtDe = "", dtAte = "", dtCorte = "";
            DateTime periodoDe = DateTime.MinValue, periodoAte = DateTime.MinValue, dataDeCorte = DateTime.MinValue;

            if (nome.ToString().Trim() != "")
            {
                strWhere += " AND listagem_nome LIKE @listagem_nome";
                paramNames.Add("@listagem_nome");
                paramValue.Add("%" + nome + "%");
                
                pnames = paramNames.ToArray();
                values = paramValue.ToArray();
            }

            if (filiais != null)
            {
                strWhere += " AND listagemfilial_filialId IN (" + String.Join(",", filiais) + ")";
            }

            if (operadoras != null)
            {
                strWhere += " AND listagemoperadora_operadoraId IN (" + String.Join(",", operadoras) + ")";
            }

            if (perfis != null)
            {
                strWhere += " AND listagemperfil_perfilId IN (" + String.Join(",", perfis) + ")";
            }

            if (dataDe != null && dataAte != null)
            {
                dtDe = dataDe.ToString();
                periodoDe = Convert.ToDateTime(dtDe);
                dtDe = periodoDe.ToString("yyyy-MM-dd");

                dtAte = dataAte.ToString();
                periodoAte = Convert.ToDateTime(dtAte);
                dtAte = periodoAte.ToString("yyyy-MM-dd");

                strWhere += " AND listagem_data >= '" + dtDe + " 00:00:00' ";
                strWhere += " AND listagem_data <= '" + dtAte + " 23:59:59' ";
            }

            if (dataCorte != null)
            {
                dtCorte = dataCorte.ToString();
                dataDeCorte = Convert.ToDateTime(dtCorte);
                dtCorte = dataDeCorte.ToString("yyyy-MM-dd");

                strWhere += " AND listagem_dataCorte = '" + dtCorte + "' ";
            }

            //if (produtorId != null)
            //{
            //    strWhere += " AND listagemrelacao_produtorId = " + produtorId;
            //}

            String query = String.Concat("",
                "SELECT DISTINCT(listagem_id), listagem.* ",
                "   FROM listagem ",
                "   INNER JOIN listagem_filial ON listagem_id = listagemfilial_listagemId ",
                "   INNER JOIN listagem_operadora ON listagem_id = listagemoperadora_listagemId ",
                "   INNER JOIN listagem_perfil ON listagem_id = listagemperfil_listagemId ",
                "   WHERE listagem_id > 0 ",
                    strWhere,
                "   ORDER BY listagem_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Listagem>
                (query, pnames, values, typeof(Listagem));
        }

        public static IList<Listagem> CarregaListagemAtual()
        {
            String query = String.Concat("",
                "SELECT TOP 1 * ",
                "   FROM listagem ",
                "   ORDER BY listagem_data DESC");

            return LocatorHelper.Instance.ExecuteParametrizedQuery<Listagem>
                (query, null, null, typeof(Listagem));
        }

        [DBTable("listagem_filial")]
        public class Filial : EntityBase, IPersisteableEntity
        {
            Object _id;
            Object _listagemId;
            Object _filialId;

            #region properties 

            [DBFieldInfo("listagemfilial_id", FieldType.PrimaryKeyAndIdentity)]
            public Object ID
            {
                get { return _id; }
                set { _id= value; }
            }

            [DBFieldInfo("listagemfilial_listagemId", FieldType.Single)]
            public Object ListagemID
            {
                get { return _listagemId; }
                set { _listagemId= value; }
            }

            [DBFieldInfo("listagemfilial_filialId", FieldType.Single)]
            public Object FilialID
            {
                get { return _filialId; }
                set { _filialId= value; }
            }

            #endregion

            public Filial() { }
            public Filial(Object id) { _id = id; }

            #region EntityBase methods

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
        }

        [DBTable("listagem_operadora")]
        public class Operadora : EntityBase, IPersisteableEntity
        {
            Object _id;
            Object _listagemId;
            Object _operadoraId;

            #region properties 

            [DBFieldInfo("listagemoperadora_id", FieldType.PrimaryKeyAndIdentity)]
            public Object ID
            {
                get { return _id; }
                set { _id= value; }
            }

            [DBFieldInfo("listagemoperadora_listagemId", FieldType.Single)]
            public Object ListagemID
            {
                get { return _listagemId; }
                set { _listagemId= value; }
            }

            [DBFieldInfo("listagemoperadora_operadoraId", FieldType.Single)]
            public Object OperadoraID
            {
                get { return _operadoraId; }
                set { _operadoraId= value; }
            }

            #endregion

            public Operadora() {}
            public Operadora(Object id) { _id = id; }

            #region EntityBase methods 

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
        }

        [DBTable("listagem_perfil")]
        public class Perfil : EntityBase, IPersisteableEntity
        {
            Object _id;
            Object _listagemId;
            Object _perfilId;

            #region properties 

            [DBFieldInfo("listagemperfil_id", FieldType.PrimaryKeyAndIdentity)]
            public Object ID
            {
                get { return _id; }
                set { _id= value; }
            }

            [DBFieldInfo("listagemperfil_listagemId", FieldType.Single)]
            public Object ListagemID
            {
                get { return _listagemId; }
                set { _listagemId= value; }
            }

            [DBFieldInfo("listagemperfil_perfilId", FieldType.Single)]
            public Object PerfilID
            {
                get { return _perfilId; }
                set { _perfilId= value; }
            }

            #endregion

            public Perfil() { }
            public Perfil(Object id) { _id = id; }

            #region EntityBase methods 

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
        }
    }

    public interface IComissionamentoItem 
    {
        Object OwnerID
        {
            get;
            set;
        }

        //Object ContratoID
        //{
        //    get;
        //    set;
        //}

        Int32 Parcela
        {
            get;
            set;
        }

        Decimal Percentual
        {
            get;
            set;
        }

        Decimal PercentualCompraCarencia
        {
            get;
            set;
        }

        Decimal PercentualMigracao
        {
            get;
            set;
        }

        Decimal PercentualADM
        {
            get;
            set;
        }

        Decimal PercentualEspecial
        {
            get;
            set;
        }

        Decimal Idade
        {
            get;
            set;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////

    [DBTable("listagem_agendamento")]
    public class ListagemAgendamento : EntityBase, IPersisteableEntity
    {
        public enum eStatusBusca : int
        {
            Todos,
            Pendentes,
            Processados
        }

        Object _id;
        String _descricao;
        Object _filialId;
        String _operadoraIds;
        DateTime _dataCorte;
        DateTime _processarEm;
        Boolean _processado;
        DateTime _processadoData;
        String _competencia;

        #region properties 

        [DBFieldInfo("listagemagenda_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("listagemagenda_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("listagemagenda_filialId", FieldType.Single)]
        public Object FilialID
        {
            get { return _filialId; }
            set { _filialId= value; }
        }

        [DBFieldInfo("listagemagenda_operadoraIds", FieldType.Single)]
        public String OperadoraIDs
        {
            get { return _operadoraIds; }
            set { _operadoraIds= value; }
        }

        [DBFieldInfo("listagemagenda_dataCorte", FieldType.Single)]
        public DateTime DataCorte
        {
            get { return _dataCorte; }
            set { _dataCorte= value; }
        }

        [DBFieldInfo("listagemagenda_processarEm", FieldType.Single)]
        public DateTime ProcessarEm
        {
            get { return _processarEm; }
            set { _processarEm= value; }
        }

        [DBFieldInfo("listagemagenda_processado", FieldType.Single)]
        public Boolean Processado
        {
            get { return _processado; }
            set { _processado= value; }
        }

        [DBFieldInfo("listagemagenda_processadoData", FieldType.Single)]
        public DateTime ProcessadoData
        {
            get { return _processadoData; }
            set { _processadoData= value; }
        }

        [DBFieldInfo("listagemagenda_competencia", FieldType.Single)]
        public String Competencia
        {
            get { return _competencia; }
            set { _competencia= value; }
        }

        public String strProcessadoData
        {
            get
            {
                if (_processadoData == DateTime.MinValue)
                    return String.Empty;
                else
                    return _processadoData.ToString("dd/MM/yyyy HH:mm");
            }
        }

        #endregion

        public ListagemAgendamento() { _processado = false; }
        public ListagemAgendamento(Object id) : this() { _id = id; }

        #region EntityBase methods 

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

        public static IList<ListagemAgendamento> Carregar(DateTime? periodoDe, DateTime? periodoAte, DateTime? dataCorte, eStatusBusca status)
        {
            String qry = "* from listagem_agendamento where listagemagenda_id is not null ";

            if (dataCorte != null)
            {
                qry += String.Concat(" and day(listagemagenda_dataCorte)=", dataCorte.Value.Day, " and month(listagemagenda_dataCorte)=", dataCorte.Value.Month, " and year(listagemagenda_dataCorte)=", dataCorte.Value.Year);
            }

            if (periodoDe != null && periodoAte != null)
            {
                qry += String.Concat(" and listagemagenda_processarEm between '", periodoDe.Value.ToString("yyyy-MM-dd"), "' and '", periodoAte.Value.ToString("yyyy-MM-dd 23:59:59:997"), "'");
            }

            return LocatorHelper.Instance.ExecuteQuery<ListagemAgendamento>(qry, typeof(ListagemAgendamento));
        }

        public static ListagemAgendamento CarregarPendente()
        {
            String qry = "select top 1 * from listagem_agendamento where listagemagenda_processado=0 and listagemagenda_processarEm <= getdate()";

            IList<ListagemAgendamento> ret = LocatorHelper.Instance.ExecuteQuery<ListagemAgendamento>(qry, typeof(ListagemAgendamento));

            if (ret == null)
                return null;
            else
                return ret[0];
        }
    }
}