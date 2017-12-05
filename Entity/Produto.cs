namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("produto")]
    public class Produto : EntityBase, IPersisteableEntity
    {
        #region properties 

        [DBFieldInfo("produto_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        { 
            get; set; 
        }

        [DBFieldInfo("produto_operadoraId",FieldType.Single)]
        public object OperadoraId
        {
            get;
            set;
        }
        
        /// <summary>
        /// Estipulante Id
        /// </summary>
        [DBFieldInfo("produto_associadoId",FieldType.Single)]
        public object AssociadoID
        {
            get;
            set;
        }
        
        [DBFieldInfo("produto_contatoAdmId",FieldType.Single)]
        public object ContratoAdmID
        {
            get;
            set;
        }
        
        [DBFieldInfo("produto_nome",FieldType.Single)]
        public string Nome
        {
            get;
            set;
        }
        
        [DBFieldInfo("produto_data",FieldType.Single)]
        public DateTime Data
        {
            get;
            set;
        }

        #endregion

        public static IList<ProdutoItem> CarregarItensVigentes(object contratoAdmId, PersistenceManager pm)
        {
            string sql = string.Concat(
                "select produto_item.* ",
                "   from produto_item ",
                "       inner join produto on produto_id = produtoitem_produtoid ",
                "   where ",
                "       produto_contratoAdmId=", contratoAdmId,
                "       and getdate() >= produtoitem_vigencia ",
                "   order by ",
                "       produtoitem_nome, produtoitem_id, produtoitem_vigencia desc ");

            var lista = LocatorHelper.Instance.ExecuteQuery<ProdutoItem>(sql, typeof(ProdutoItem), pm);

            if (lista == null || lista.Count == 0) return null;

            IList<ProdutoItem> tratados = new List<ProdutoItem>();

            foreach (var item in lista)
            {
                if (!colecaoContemNome(tratados, item.Nome)) tratados.Add(item);
            }

            return tratados;
        }

        static bool colecaoContemNome(IList<ProdutoItem> lista, string nome)
        {
            if (lista == null) return false;

            foreach (var item in lista)
            {
                if (item.Nome.ToLower() == nome.ToLower()) return true;
            }

            return false;
        }
    }

    [DBTable("produto_item")]
    public class ProdutoItem : EntityBase, IPersisteableEntity
    {
        #region properties 

        [DBFieldInfo("produtoitem_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get;
            set;
        }

        [DBFieldInfo("produtoitem_produtoId", FieldType.Single)]
        public object ProdutoID
        {
            get;
            set;
        }

        [DBFieldInfo("produtoitem_nome", FieldType.Single)]
        public string Nome
        {
            get;
            set;
        }

        [DBFieldInfo("produtoitem_vigencia", FieldType.Single)]
        public DateTime Vigencia
        {
            get;
            set;
        }

        [DBFieldInfo("produtoitem_valor", FieldType.Single)]
        public decimal Valor
        {
            get;
            set;
        }

        [DBFieldInfo("produtoitem_valorNet", FieldType.Single)]
        public decimal ValorNet
        {
            get;
            set;
        }

        [DBFieldInfo("produtoitem_data", FieldType.Single)]
        public DateTime Data
        {
            get;
            set;
        }

        #endregion
    }

    [DBTable("produto_item_cobranca")]
    public class ProdutoITEM_Cobranca : EntityBase, IPersisteableEntity
    {
        [DBFieldInfo("produtoitemcobranca_id", FieldType.PrimaryKeyAndIdentity)]
        public object ID
        {
            get;
            set;
        }

        [DBFieldInfo("produtoitemcobranca_cobrancaid", FieldType.Single)]
        public object CobrancaID
        {
            get;
            set;
        }

        [DBFieldInfo("produtoitemcobranca_produtoitemid", FieldType.Single)]
        public object ProdutoItemID
        {
            get;
            set;
        }

        [DBFieldInfo("produtoitemcobranca_produtoitemvalor", FieldType.Single)]
        public decimal ProdutoItemValor
        {
            get;
            set;
        }

        [DBFieldInfo("produtoitemcobranca_qtvidas", FieldType.Single)]
        public decimal ProdutoItemQTDVidas
        {
            get;
            set;
        }

        public static void SalvarRelacionamento(object cobrancaId, IList<ProdutoItem> itens, int qtdVidas, PersistenceManager pm)
        {
            if (itens == null || cobrancaId == null) return;
            bool newPm = false;

            if (pm == null)
            {
                pm = new PersistenceManager();
                pm.UseSingleCommandInstance();
                newPm = true;
            }

            foreach (var i in itens)
            {
                var relac = new ProdutoITEM_Cobranca 
                { 
                    CobrancaID = cobrancaId, 
                    ProdutoItemID = i.ID, 
                    ProdutoItemValor = i.Valor,
                    ProdutoItemQTDVidas = qtdVidas
                };

                pm.Save(relac);
            }

            if (newPm)
            {
                pm.CloseSingleCommandInstance();
                pm.Dispose();
            }
        }
    }
}
