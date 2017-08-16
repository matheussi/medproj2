namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("almox_produto")]
    public class AlmoxProduto : EntityBase, IPersisteableEntity
    {
        public enum CampoChave : int
        {
            Descricao,
            Codigo
        }

        #region campos 

        Object _id;
        Object _filialId;
        Object _operadoraId;
        String _descricao;
        Object _tipoId;
        //String _codigo;
        Object _usuarioId;
        int _qtd;
        int _qtdMin;
        int _qtdMax;
        DateTime _data;
        Boolean _ativo;

        String _filialNome;
        String _tipoProdutoDescricao;

        #endregion

        #region propriedades 

        [DBFieldInfo("almox_produto_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("almox_produto_filialId", FieldType.Single)]
        public Object FilialID
        {
            get { return _filialId; }
            set { _filialId= value; }
        }

        [DBFieldInfo("almox_produto_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("almox_produto_descricao", FieldType.Single)]
        public String Descricao
        {
            get { return _descricao; }
            set { _descricao= value; }
        }

        [DBFieldInfo("almox_produto_tipoId", FieldType.Single)]
        public Object TipoProdutoID
        {
            get { return _tipoId; }
            set { _tipoId= value; }
        }

        //[DBFieldInfo("almox_produto_codigo", FieldType.Single)]
        //public String Codigo
        //{
        //    get { return _codigo; }
        //    set { _codigo= value; }
        //}

        [DBFieldInfo("almox_produto_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId= value; }
        }

        [DBFieldInfo("almox_produto_qtd", FieldType.Single)]
        public int QTD
        {
            get { return _qtd; }
            set { _qtd= value; }
        }

        [DBFieldInfo("almox_produto_qtdMin", FieldType.Single)]
        public int QTDMin
        {
            get { return _qtdMin; }
            set { _qtdMin= value; }
        }

        [DBFieldInfo("almox_produto_qtdMax", FieldType.Single)]
        public int QTDMax
        {
            get { return _qtdMax; }
            set { _qtdMax= value; }
        }

        [DBFieldInfo("almox_produto_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [DBFieldInfo("almox_produto_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return _ativo; }
            set { _ativo= value; }
        }

        [Joinned("almox_tipoproduto_descricao")]
        public String TipoProdutoDescricao
        {
            get { return _tipoProdutoDescricao; }
            set { _tipoProdutoDescricao= value; }
        }

        [Joinned("filial_nome")]
        public String FilialNome
        {
            get { return _filialNome; }
            set { _filialNome= value; }
        }

        #endregion

        public AlmoxTipoProduto AlmoxTipoProduto
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public AlmoxProduto() { _data = DateTime.Now; _qtdMin = 0; _qtdMax = 0; _ativo = true; }
        public AlmoxProduto(Object id) : this() { _id = id; }

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

        public static IList<AlmoxProduto> CarregarTodos()
        {
            return CarregarTodos(null, false);
        }

        public static AlmoxProduto Carregar(Object tipoId, Object operadoraId)
        {
            String query = "";

            if (operadoraId != null)
            {
                query = String.Concat("almox_produto.*, almox_tipoproduto_descricao FROM almox_produto ",
                     "INNER JOIN almox_tipo_produto ON almox_produto_tipoId=almox_tipoproduto_id ",
                     "INNER JOIN operadora ON operadora_id=almox_produto_operadoraId ",
                     "WHERE almox_produto_operadoraId=", operadoraId, " AND almox_produto_tipoId=", tipoId, " ORDER BY operadora_nome");
            }
            else
            {
                query = String.Concat("almox_produto.*, almox_tipoproduto_descricao FROM almox_produto ",
                     "INNER JOIN almox_tipo_produto ON almox_produto_tipoId=almox_tipoproduto_id ",
                     "WHERE almox_produto_operadoraId IS NULL AND almox_produto_tipoId=", tipoId);
            }

            IList<AlmoxProduto> lista = LocatorHelper.Instance.ExecuteQuery<AlmoxProduto>(query, typeof(AlmoxProduto));

            if (lista == null)
                return null;
            else
                return lista[0];
        }

        public static IList<AlmoxProduto> CarregarTodosSemOperadora()
        {
            String query = String.Concat("almox_produto.*, almox_tipoproduto_descricao, filial_nome FROM almox_produto ",
                "INNER JOIN almox_tipo_produto ON almox_produto_tipoId=almox_tipoproduto_id ",
                "INNER JOIN filial ON filial_id=almox_produto_filialId ",
                "WHERE almox_produto_operadoraId IS NULL ORDER BY almox_tipoproduto_descricao");

            return LocatorHelper.Instance.ExecuteQuery<AlmoxProduto>(query, typeof(AlmoxProduto));
        }

        public static IList<AlmoxProduto> CarregarTodosPorOperadora(Object operadoraId)
        {
            String query = String.Concat("almox_produto.*, almox_tipoproduto_descricao, filial_nome FROM almox_produto ",
                "INNER JOIN almox_tipo_produto ON almox_produto_tipoId=almox_tipoproduto_id ",
                "INNER JOIN operadora ON operadora_id=almox_produto_operadoraId ",
                "INNER JOIN filial ON filial_id=almox_produto_filialId ",
                "WHERE almox_produto_operadoraId=", operadoraId, " ORDER BY operadora_nome");

            return LocatorHelper.Instance.ExecuteQuery<AlmoxProduto>(query, typeof(AlmoxProduto));
        }

        public static IList<AlmoxProduto> CarregarTodosPorOperadora(Object operadoraId, Object tipoId)
        {
            String query = String.Concat("almox_produto.*, almox_tipoproduto_descricao FROM almox_produto ",
                "INNER JOIN almox_tipo_produto ON almox_produto_tipoId=almox_tipoproduto_id ",
                "INNER JOIN operadora ON operadora_id=almox_produto_operadoraId ",
                "WHERE almox_produto_operadoraId=", operadoraId, " AND almox_produto_tipoId=", tipoId, " ORDER BY operadora_nome");

            return LocatorHelper.Instance.ExecuteQuery<AlmoxProduto>(query, typeof(AlmoxProduto));
        }

        public static IList<AlmoxProduto> CarregarTodos(Object tipoID)
        {
            return CarregarTodos(tipoID, false);
        }

        public static IList<AlmoxProduto> CarregarTodos(Boolean apenasAtivos)
        {
            return CarregarTodos(null, apenasAtivos);
        }

        public static IList<AlmoxProduto> CarregarTodos(Object tipoID, Boolean apenasAtivos)
        {
            String where = "";

            if (tipoID != null)
                where = " WHERE almox_produto_tipoId=" + tipoID;

            if (apenasAtivos)
            {
                if (where.Length > 0) { where += " AND "; }
                else { where = " WHERE "; }

                where += " almox_produto_ativo=1 ";
            }

            String query = String.Concat("almox_produto.*, almox_tipoproduto_descricao FROM almox_produto ",
                "INNER JOIN almox_tipo_produto ON almox_produto_tipoId=almox_tipoproduto_id ",
                where,
                " ORDER BY almox_tipoproduto_descricao");

            return LocatorHelper.Instance.ExecuteQuery<AlmoxProduto>(query, typeof(AlmoxProduto));
        }

        public static Boolean ExisteProdutoParaEsteTipoOperadoraFilial(Object tipoId, Object operadoraId, Object filialId, Object prodId)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT almox_produto_id FROM almox_produto WHERE almox_produto_tipoId=");
            sql.Append(tipoId);

            if(filialId != null)
            {
                sql.Append(" AND almox_produto_filialId=");
                sql.Append(filialId);
            }

            if(operadoraId != null)
            {
                sql.Append(" AND almox_produto_operadoraId=");
                sql.Append(operadoraId);
            }

            if (prodId != null)
            {
                sql.Append(" AND almox_produto_id <> "); 
                sql.Append(prodId);
            }

            Object ret = LocatorHelper.Instance.ExecuteScalar(sql.ToString(), null, null);
            sql.Remove(0, sql.Length);
            return (ret != null);
        }

        public static IList<AlmoxProduto> CarregarPorParametros(Object tipoID, String valor, CampoChave campoOrdem, CampoChave campoFiltro)
        {
            String campo = "almox_produto_descricao";
            if (campoOrdem == CampoChave.Codigo) { campo = "almox_produto_codigo"; }

            String where = "";
            String _campoFiltro = "almox_produto_descricao";
            if (!String.IsNullOrEmpty(valor))
            {
                if (campoFiltro == CampoChave.Codigo) { _campoFiltro = "almox_produto_codigo"; }
                where = " AND " + _campoFiltro + "=@Valor ";
            }

            String query = String.Concat("almox_produto.*, almox_tipoproduto_descricao FROM almox_produto ",
                "INNER JOIN almox_tipo_produto ON almox_produto_tipoId=almox_tipoproduto_id ",
                "WHERE almox_produto_tipoId=", tipoID, where, " ORDER BY ", campo);

            if (where.Length > 0)
            {
                String[] param = new String[] { "@Valor" };
                String[] pvalor = new String[] { valor };

                return LocatorHelper.Instance.ExecuteParametrizedQuery
                    <AlmoxProduto>(query, param, pvalor, typeof(AlmoxProduto));
            }
            else
            {
                return LocatorHelper.Instance.ExecuteQuery<AlmoxProduto>(query, typeof(AlmoxProduto));
            }
        }

        public static void AlterarStatus(Object ProdutoID, Boolean ativo)
        {
            String command = "UPDATE almox_produto SET almox_produto_ativo=" + Convert.ToInt32(ativo) + " WHERE almox_produto_id=" + ProdutoID;
            NonQueryHelper.Instance.ExecuteNonQuery(command, null);
        }

        internal static void AlteraQTD(ref PersistenceManager pm, AlmoxMovimentacao.TipoMovimentacao tipoMovimentacao, Object produtoId, int qtd)
        {
            StringBuilder query = new StringBuilder();
            query.Append("UPDATE almox_produto SET almox_produto_qtd=(almox_produto_qtd");

            if (tipoMovimentacao == AlmoxMovimentacao.TipoMovimentacao.Entrada)
                query.Append(" + ");
            else
                query.Append(" - ");

            query.Append(qtd);
            query.Append(") WHERE almox_produto_id = ");
            query.Append(produtoId);

            NonQueryHelper.Instance.ExecuteNonQuery(query.ToString(), pm);
        }
    }
}