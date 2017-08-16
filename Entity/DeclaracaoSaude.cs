namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    /// <summary>
    /// Itens da declaração de saúde.
    /// </summary>
    [Serializable()]
    [DBTable("declaracao_saude_item")]
    public class ItemDeclaracaoSaude : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _operadoraid;
        String _codigo;
        int _ordem;
        String _texto;
        Boolean _ativo;

        #endregion

        #region propriedades 

        [DBFieldInfo("itemdeclaracaosaude_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("itemdeclaracaosaude_operadoraid", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraid; }
            set { _operadoraid= value; }
        }

        [DBFieldInfo("itemdeclaracaosaude_ordem", FieldType.Single)]
        public int Ordem
        {
            get { return _ordem; }
            set { _ordem= value; }
        }

        [DBFieldInfo("itemdeclaracaosaude_codigo", FieldType.Single)]
        public String Codigo
        {
            get { return _codigo; }
            set { _codigo= value; }
        }

        [DBFieldInfo("itemdeclaracaosaude_texto", FieldType.Single)]
        public String Texto
        {
            get { return _texto; }
            set { _texto= value; }
        }

        [DBFieldInfo("itemdeclaracaosaude_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo= value; }
        }

        public String Resumo
        {
            get
            {
                String retorno = "";

                retorno = this._ordem.ToString() + " - ";

                if (this._texto.Length > 88)
                    retorno += this._texto.Substring(0, 85) + "...";
                else
                    retorno += this._texto;

                return retorno;
            }
        }

        #endregion

        public ItemDeclaracaoSaude() { _ativo = true; }

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

        public static IList<ItemDeclaracaoSaude> Carregar(Object operadoraID)
        {
            String query = "* FROM declaracao_saude_item WHERE itemdeclaracaosaude_operadoraid=" + operadoraID + " ORDER BY itemdeclaracaosaude_ordem";

            return LocatorHelper.Instance.ExecuteQuery
                <ItemDeclaracaoSaude>(query, typeof(ItemDeclaracaoSaude));
        }

        public static ItemDeclaracaoSaude Carregar(Object operadoraId, String codigo, PersistenceManager pm)
        {
            String query = String.Concat(
                "TOP 1 itemdeclaracaosaude_id ",
                " FROM declaracao_saude_item ",
                " WHERE itemdeclaracaosaude_operadoraid=", operadoraId,
                " AND itemdeclaracaosaude_codigo='", codigo, "'");

            IList<ItemDeclaracaoSaude> list = LocatorHelper.Instance.ExecuteQuery
                <ItemDeclaracaoSaude>(query, typeof(ItemDeclaracaoSaude), pm);

            if (list == null || list.Count == 0)
                return null;
            else
                return list[0];
        }

        public static Boolean Duplicado(ItemDeclaracaoSaude item)
        {
            String qry = String.Concat("SELECT DISTINCT(itemdeclaracaosaude_id) ",
                "   FROM declaracao_saude_item ",
                "   WHERE ", 
                "       itemdeclaracaosaude_operadoraid=", item.OperadoraID, " AND (",
                "       (itemdeclaracaosaude_codigo=@codigo AND itemdeclaracaosaude_ordem=@ordem) OR (itemdeclaracaosaude_texto=@texto))");

            if (item.ID != null)
            {
                qry += " AND itemdeclaracaosaude_id <> " + item.ID;
            }

            String[] pNames  = new String[] { "@codigo", "@ordem", "@texto" };
            String[] pValues = new String[] { item.Codigo, item.Ordem.ToString(), item.Texto };

            Object returned = LocatorHelper.Instance.ExecuteScalar(qry, pNames, pValues);

            if (returned == null || returned == DBNull.Value)
                return false;
            else
                return true;
        }
    }

    /// <summary>
    /// Cabeçalho da regra de declaração de saúde.
    /// </summary>
    [Serializable()]
    [DBTable("declaracao_saude_regra")]
    public class RegraDeclaracaoSaude : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _operadoraId;
        String _descricao;
        String _operadorIdade;
        int _idade;
        int _sexoId;

        #endregion

        #region propriedades 

        [DBFieldInfo("declaracaosauderegra_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [DBFieldInfo("declaracaosauderegra_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("declaracaosauderegra_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("declaracaosauderegra_operadorIdade", FieldType.Single)]
        public String OperadorIdade
        {
            get { return _operadorIdade; }
            set { _operadorIdade= value; }
        }

        [DBFieldInfo("declaracaosauderegra_idade", FieldType.Single)]
        public int Idade
        {
            get { return _idade; }
            set { _idade= value; }
        }

        [DBFieldInfo("declaracaosauderegra_sexoId", FieldType.Single)]
        public int SexoID
        {
            get { return _sexoId; }
            set { _sexoId= value; }
        }

        #endregion

        public ItemRegraDeclaracaoSaude ItemRegraDeclaracaoSaude
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public RegraDeclaracaoSaude() { _idade = -1; _sexoId = -1; }

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
                NonQueryHelper.Instance.ExecuteNonQuery("DELETE FROM declaracao_saude_regra_item WHERE itemdeclaracaoregra_regraDeclaracaoId=" + this._id, pm);
                pm.Remove(this);
                pm.Commit();
            }
            catch(Exception ex)
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

        public static IList<RegraDeclaracaoSaude> Carregar(Object operadoraID)
        {
            String query = "* FROM declaracao_saude_regra WHERE declaracaosauderegra_operadoraId=" + operadoraID + " ORDER BY declaracaosauderegra_descricao";

            return LocatorHelper.Instance.ExecuteQuery
                <RegraDeclaracaoSaude>(query, typeof(RegraDeclaracaoSaude));
        }

        public Boolean ValidaIdade(int idade)
        {
            if (String.IsNullOrEmpty(this._operadorIdade) || _idade == -1) { return true; }

            switch (_operadorIdade)
            {
                case "+":
                {
                    return !(idade > this._idade);
                }
                case "+=":
                {
                    return !(idade >= this._idade);
                }
                case "=":
                {
                    return !(idade == this._idade);
                }
                case "-=":
                {
                    return !(idade <= this._idade);
                }
                case "-":
                {
                    return !(idade < this._idade);
                }
            }

            return true;
        }

        public Boolean ValidaSexo(int sexo)
        {
            if (_sexoId == -1) { return true; }

            return sexo != _sexoId;
        }

        public Boolean Valida(int idade, int sexo)
        {
            Boolean idadeOK = true;
            Boolean sexoOK  = true;
            sexoOK  = this.ValidaSexo(sexo);
            idadeOK = this.ValidaIdade(idade);

            if (this._idade != -1 && this._sexoId != -1) //deve validar os dois
                return idadeOK || sexoOK;
            else if (this._sexoId != -1)                 //deve validar apenas o sexo
                return sexoOK;
            else                                         //deva validar apenas a idade
                return idadeOK;
        }
    }

    /// <summary>
    /// Itens da REGRA da declaração de saúde
    /// </summary>
    [Serializable()]
    [DBTable("declaracao_saude_regra_item")]
    public class ItemRegraDeclaracaoSaude : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _itemDeclaracaoId;
        Object _regraDeclaracaoId;

        #endregion

        #region propriedades

        [DBFieldInfo("itemdeclaracaoregra_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        /// <summary>
        /// ID da regra à qual pertence este item de regra.
        /// </summary>
        [DBFieldInfo("itemdeclaracaoregra_regraDeclaracaoId", FieldType.Single)]
        public Object RegraDeclaracaoID
        {
            get { return _regraDeclaracaoId; }
            set { _regraDeclaracaoId= value; }
        }

        /// <summary>
        /// ID do item da ficha de declaração de saúde.
        /// </summary>
        [DBFieldInfo("itemdeclaracaoregra_itemDeclaracaoId", FieldType.Single)]
        public Object ItemDeclaracaoID
        {
            get { return _itemDeclaracaoId; }
            set { _itemDeclaracaoId= value; }
        }

        #endregion

        public ItemRegraDeclaracaoSaude() { }

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

        public static IList<ItemRegraDeclaracaoSaude> Carregar(Object regraId)
        {
            String query = "* FROM declaracao_saude_regra_item WHERE itemdeclaracaoregra_regraDeclaracaoId=" + regraId;

            return LocatorHelper.Instance.ExecuteQuery
                <ItemRegraDeclaracaoSaude>(query, typeof(ItemRegraDeclaracaoSaude));
        }

        public static ItemRegraDeclaracaoSaude Carregar(Object regraId, Object itemFichaId)
        {
            String query = "* FROM declaracao_saude_regra_item WHERE itemdeclaracaoregra_regraDeclaracaoId=" + regraId + " AND itemdeclaracaoregra_itemDeclaracaoId=" + itemFichaId;

            IList<ItemRegraDeclaracaoSaude> lista = LocatorHelper.Instance.ExecuteQuery
                <ItemRegraDeclaracaoSaude>(query, typeof(ItemRegraDeclaracaoSaude));

            if (lista == null)
                return null;
            else
                return lista[0];
        }
    }

    /// <summary>
    /// Itens da declaração de saúde PREENCHIDA.
    /// </summary>
    [Serializable()]
    [DBTable("declaracao_saude_item_instancia")]
    public class ItemDeclaracaoSaudeINSTANCIA : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _beneficiarioId;
        Boolean _sim;
        Object _itemDeclaracaoId; //ID do item da declaração de saúde.
        DateTime _data;
        String _descricao;

        String _cidInicial;
        String _cidFinal;
        Boolean _aprovadoPeloMedico;
        String _obsMedico;
        DateTime _dataAprovadoMedico;

        Boolean _aprovadoPeloDeptoTecnico;
        DateTime _dataAprovadoPeloDeptoTecnico;
        String _obsDeptoTecnico;

        String _itemDeclaracaoTexto; //Texto do item da declaração de saúde.
        String _itemDeclaracaoCodigo;

        #endregion

        #region propriedades 

        [DBFieldInfo("itemdeclaracaosaudeinstancia_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("itemdeclaracaosaudeinstancia_beneficiarioId", FieldType.Single)]
        public Object BeneficiarioID
        {
            get { return _beneficiarioId; }
            set { _beneficiarioId= value; }
        }

        [DBFieldInfo("itemdeclaracaosaudeinstancia_sim", FieldType.Single)]
        public Boolean Sim
        {
            get { return _sim; }
            set { _sim= value; }
        }

        [DBFieldInfo("itemDeclaracaoSaudeInstancia_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        public String strData
        {
            get
            {
                if (_data == DateTime.MinValue)
                    return "";
                else
                    return _data.ToString("dd/MM/yyyy");
            }
        }

        [DBFieldInfo("itemDeclaracaoSaudeInstancia_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("itemDeclaracaoSaudeInstancia_cidInicial", FieldType.Single)]
        public String CIDInicial
        {
            get { return _cidInicial; }
            set { _cidInicial= value; }
        }

        [DBFieldInfo("itemDeclaracaoSaudeInstancia_cidFinal", FieldType.Single)]
        public String CIDFinal
        {
            get { return _cidFinal; }
            set { _cidFinal= value; }
        }

        [DBFieldInfo("itemDeclaracaoSaudeInstancia_obs", FieldType.Single)]
        public String ObsMedico
        {
            get { return _obsMedico; }
            set { _obsMedico= value; }
        }

        [DBFieldInfo("itemDeclaracaoSaudeInstancia_aprovadoMedico", FieldType.Single)]
        public Boolean AprovadoPeloMedico
        {
            get { return _aprovadoPeloMedico; }
            set { _aprovadoPeloMedico= value; }
        }

        [DBFieldInfo("itemDeclaracaoSaudeInstancia_dataAprovadoMedico", FieldType.Single)]
        public DateTime DataAprovadoPeloMedico
        {
            get { return _dataAprovadoMedico; }
            set { _dataAprovadoMedico= value; }
        }

        [DBFieldInfo("itemDeclaracaoSaudeInstancia_obsDeptoTecnico", FieldType.Single)]
        public String ObsDeptoTecnico
        {
            get { return _obsDeptoTecnico; }
            set { _obsDeptoTecnico= value; }
        }

        [DBFieldInfo("itemDeclaracaoSaudeInstancia_parecerDeptoTecnico", FieldType.Single)]
        public Boolean AprovadoPeloDeptoTecnico
        {
            get { return _aprovadoPeloDeptoTecnico; }
            set { _aprovadoPeloDeptoTecnico= value; }
        }

        [DBFieldInfo("itemDeclaracaoSaudeInstancia_dataParecerDeptoTecnico", FieldType.Single)]
        public DateTime DataAprovadoPeloDeptoTecnico
        {
            get { return _dataAprovadoPeloDeptoTecnico; }
            set { _dataAprovadoPeloDeptoTecnico= value; }
        }

        [Joinned("itemdeclaracaosaude_id")]
        [DBFieldInfo("itemdeclaracaosaudeinstancia_itemdeclaracaoid", FieldType.Single)]
        public Object ItemDeclaracaoID
        {
            get { return _itemDeclaracaoId; }
            set { _itemDeclaracaoId= value; }
        }

        [Joinned("itemdeclaracaosaude_texto")]
        public String ItemDeclaracaoTexto
        {
            get { return _itemDeclaracaoTexto; }
            set { _itemDeclaracaoTexto= value; }
        }

        [Joinned("itemdeclaracaosaude_codigo")]
        public String ItemDeclaracaoCodigo
        {
            get { return _itemDeclaracaoCodigo; }
            set { _itemDeclaracaoCodigo= value; }
        }

        #endregion

        public ItemDeclaracaoSaudeINSTANCIA() { _sim = false; _data = DateTime.Now; _aprovadoPeloDeptoTecnico = false; _aprovadoPeloMedico = false; }

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

        public static IList<ItemDeclaracaoSaudeINSTANCIA> Carregar(Object operadoraId)
        {
            String query = String.Concat(
                "itemdeclaracaosaude_id, itemdeclaracaosaude_texto ",
                " FROM declaracao_saude_item ",
                " WHERE itemdeclaracaosaude_operadoraid=", operadoraId,
                " ORDER BY itemdeclaracaosaude_ordem");

            return LocatorHelper.Instance.ExecuteQuery
                <ItemDeclaracaoSaudeINSTANCIA>(query, typeof(ItemDeclaracaoSaudeINSTANCIA));
        }

        public static Object CarregarID(Object beneficiarioId, Object itemDeclaracaoSaudeId, PersistenceManager pm)
        {
            String qry = String.Concat("itemdeclaracaosaudeinstancia_id FROM declaracao_saude_item_instancia WHERE itemDeclaracaoSaudeInstancia_itemdeclaracaoid=", itemDeclaracaoSaudeId, " AND itemdeclaracaosaudeinstancia_beneficiarioId=", beneficiarioId);

            IList<ItemDeclaracaoSaudeINSTANCIA> list = LocatorHelper.Instance.ExecuteQuery<ItemDeclaracaoSaudeINSTANCIA>(qry, typeof(ItemDeclaracaoSaudeINSTANCIA), pm);

            if (list == null || list.Count == 0)
                return null;
            else
                return list[0].ID;
        }

        public static IList<ItemDeclaracaoSaudeINSTANCIA> Carregar(Object beneficiarioId, Object operadoraId)
        {
            return Carregar(beneficiarioId, operadoraId, null);
        }

        public static IList<ItemDeclaracaoSaudeINSTANCIA> Carregar(Object beneficiarioId, Object operadoraId, PersistenceManager pm)
        {
            String query = String.Concat(
                "itemdeclaracaosaude_id, itemdeclaracaosaude_texto,itemdeclaracaosaude_codigo,declaracao_saude_item_instancia.*",
                " FROM declaracao_saude_item ",
                "   LEFT JOIN declaracao_saude_item_instancia ON (itemDeclaracaoSaude_id=itemDeclaracaoSaudeInstancia_itemdeclaracaoid AND itemDeclaracaoSaudeInstancia_beneficiarioId=", beneficiarioId, ") ",
                " WHERE itemdeclaracaosaude_operadoraid=", operadoraId,
                " ORDER BY itemdeclaracaosaude_ordem asc,itemdeclaracaosaudeinstancia_sim desc");

            return LocatorHelper.Instance.ExecuteQuery
                <ItemDeclaracaoSaudeINSTANCIA>(query, typeof(ItemDeclaracaoSaudeINSTANCIA), pm);
        }
    }
}