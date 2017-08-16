namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("almox_contrato_impresso")]
    public class AlmoxContratoImpresso : EntityBase, IPersisteableEntity
    {
        #region campos 

        Object _id;
        Object _movimentacaoId;
        Object _produtoId;
        Object _produtorId;
        Int32 _numero;
        String _letra;
        Int32 _qtdZerosEsquerda;
        DateTime _dataRetirada;
        Boolean _rasurado;

        String _agenteNome;
        Object _operadoraId;
        String _operadoraNome;

        String _emprestaA;

        #endregion

        #region propriedades 

        [DBFieldInfo("almox_contratoimp_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("almox_contratoimp_movId", FieldType.Single)]
        public Object MovID
        {
            get { return _movimentacaoId; }
            set { _movimentacaoId= value; }
        }

        [DBFieldInfo("almox_contratoimp_produtoId", FieldType.Single)]
        public Object ProdutoID
        {
            get { return _produtoId; }
            set { _produtoId= value; }
        }

        [DBFieldInfo("almox_contratoimp_produtorId", FieldType.Single)]
        public Object AgenteID
        {
            get { return _produtorId; }
            set { _produtorId= value; }
        }

        [DBFieldInfo("almox_contratoimp_numero", FieldType.Single)]
        public Int32 Numero
        {
            get { return _numero; }
            set { _numero= value; }
        }

        [DBFieldInfo("almox_contratoimp_letra", FieldType.Single)]
        public String Letra
        {
            get { return _letra; }
            set { _letra= value; }
        }

        [DBFieldInfo("almox_contratoimp_zerosAEsquerda", FieldType.Single)]
        public Int32 QtdZerosAEsquerda
        {
            get { return _qtdZerosEsquerda; }
            set { _qtdZerosEsquerda= value; }
        }

        [DBFieldInfo("almox_contratoimp_dataRetirada", FieldType.Single)]
        public DateTime Data
        {
            get { return _dataRetirada; }
            set { _dataRetirada= value; }
        }

        [DBFieldInfo("almox_contratoimp_rasurado", FieldType.Single)]
        public Boolean Rasurado
        {
            get { return _rasurado; }
            set { _rasurado= value; }
        }

        [Joinned("usuario_nome")]
        public String AgenteNome
        {
            get { return _agenteNome; }
            set { _agenteNome= value; }
        }

        [Joinned("almox_produto_operadoraId")]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [Joinned("operadora_nome")]
        public String OperadoraNome
        {
            get { return _operadoraNome; }
            set { _operadoraNome= value; }
        }

        /// <summary>
        /// Caso o corretor que tenha efetuado a retirada empreste o impresso a um outro corretor, e esse outro corretor faça 
        /// a venda, seu nome estará nesta propriedade.
        /// </summary>
        [Joinned("EMPRESTADO")]
        public String EmprestadoA
        {
            get
            {
                if (String.IsNullOrEmpty(_emprestaA) || _emprestaA == _agenteNome)
                    return "--------------";
                else
                    return _emprestaA;
            }
            set { _emprestaA= value; }
        }

        public String NumOuQtd
        {
            get { return _numero.ToString(); }
        }

        /// <summary>
        /// Gera o alfanumérico que é o número do impresso.
        /// </summary>
        public String NumeroDoImpresso
        {
            get { return EntityBase.GeraNumeroDeContrato(_numero, _qtdZerosEsquerda, _letra); }
        }

        #endregion

        #region persistence methods 

        public void Salvar()
        {
            base.Salvar(this);
        }

        public void Carregar()
        {
            base.Carregar(this);
        }

        //public void Remover()
        //{
        //    base.Remover(this);
        //}

        public void Salvar(IList<AlmoxContratoImpresso> lista)
        {
            AlmoxContratoImpresso.SalvarLista(lista);
        }

        public static void SalvarLista(IList<AlmoxContratoImpresso> lista)
        {
            if (lista == null || lista.Count == 0) { return; }

            PersistenceManager pm = new PersistenceManager();
            pm.TransactionContext();

            try
            {
                foreach (AlmoxContratoImpresso aci in lista)
                    pm.Save(aci);

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

        public AlmoxContratoImpresso() { _dataRetirada = DateTime.MinValue; }
        public AlmoxContratoImpresso(Object id) : this() { _id = id; }

        public static IList<AlmoxContratoImpresso> CarregarPorNumeroProduto(Int32 numeroContrato, String letra, int qtdZerosEsquerda)
        {
            String qry = "almox_contrato_impresso.*, almox_produto_operadoraId, operadora_nome FROM almox_contrato_impresso INNER JOIN almox_produto ON almox_contratoimp_produtoId=almox_produto_id INNER JOIN operadora ON operadora_id=almox_produto_operadoraId WHERE (almox_contratoimp_rasurado=0 OR almox_contratoimp_rasurado IS NULL) AND almox_contratoimp_numero=@NUM"; //(almox_contratoimp_rasurado=0 OR almox_contratoimp_rasurado IS NULL) AND -> para impedir que propostas rasuradas sejam retornadas.

            String[] paramNm = new String[] { "@NUM" };
            String[] paramVl = new String[] { numeroContrato.ToString() };

            if (qtdZerosEsquerda > 0)
                qry += " AND almox_contratoimp_zerosAEsquerda=" + qtdZerosEsquerda.ToString();

            if (!String.IsNullOrEmpty(letra))
                qry += " AND almox_contratoimp_letra='" + letra + "'";
            else
                qry += " AND (almox_contratoimp_letra='' OR almox_contratoimp_letra IS NULL)";

            return LocatorHelper.Instance.ExecuteParametrizedQuery
                <AlmoxContratoImpresso>(qry, paramNm, paramVl, typeof(AlmoxContratoImpresso));
        }

        public static AlmoxContratoImpresso CarregarPorNumeroProduto(Int32 numeroContrato, Object operadoraId, String letra, int qtdZerosEsquerda)
        {
            String qry = "almox_contrato_impresso.*, almox_produto_operadoraId, operadora_nome FROM almox_contrato_impresso INNER JOIN almox_produto ON almox_contratoimp_produtoId=almox_produto_id INNER JOIN operadora ON operadora_id=almox_produto_operadoraId WHERE almox_produto_operadoraId=" + operadoraId + " AND almox_contratoimp_numero=@NUM";
            String[] paramNm = new String[] { "@NUM" };
            String[] paramVl = new String[] { numeroContrato.ToString() };

            if (qtdZerosEsquerda > 0)
                qry += " AND almox_contratoimp_zerosAEsquerda=" + qtdZerosEsquerda.ToString();

            if (!String.IsNullOrEmpty(letra))
                qry += " AND almox_contratoimp_letra='" + letra + "'";

            IList<AlmoxContratoImpresso> lista = LocatorHelper.Instance.ExecuteParametrizedQuery
                <AlmoxContratoImpresso>(qry, paramNm, paramVl, typeof(AlmoxContratoImpresso));

            if (lista == null)
                return null;
            else
                return lista[0];
        }

        public static void RetiraDeProdutor(Object produtoId, Int32 numero, String letra, Int32 qtdZerosEsquerda, PersistenceManager pm)
        {
            String command = "UPDATE almox_contrato_impresso SET almox_contratoimp_produtorId=NULL WHERE almox_contratoimp_numero=@Numero AND almox_contratoimp_produtoId=" +produtoId;
            String[] paramNm = new String[] { "@Numero" };
            String[] paramVl = new String[] { numero.ToString() };

            if (qtdZerosEsquerda > 0)
                command += " AND almox_contratoimp_zerosAEsquerda=" + qtdZerosEsquerda.ToString();

            if (!String.IsNullOrEmpty(letra))
                command += " AND almox_contratoimp_letra='" + letra + "'";

            NonQueryHelper.Instance.ExecuteNonQuery(command, paramNm, paramVl, pm);
        }

        public static void SetaProdutor(Object agenteId, Object produtoId, String numero, String letra, Int32 qtdZerosEsquerda, PersistenceManager pm)
        {
            String command = "UPDATE almox_contrato_impresso SET almox_contratoimp_produtorId=" + agenteId + ", almox_contratoimp_dataRetirada='" + DateTime.Now.ToString("yyyy/MM/dd") + "' WHERE almox_contratoimp_numero=@Numero AND almox_contratoimp_produtoId=" + produtoId;
            String[] paramNm = new String[] { "@Numero" };
            String[] paramVl = new String[] { numero };

            if (qtdZerosEsquerda > 0)
                command += " AND almox_contratoimp_zerosAEsquerda=" + qtdZerosEsquerda.ToString();

            if (!String.IsNullOrEmpty(letra))
                command += " AND almox_contratoimp_letra='" + letra + "'";

            NonQueryHelper.Instance.ExecuteNonQuery(command, paramNm, paramVl, pm);
        }

        public static Boolean ExisteContrato(Int32 numeroContrato, String letra, Int32 qtdZerosEsquerda)
        {
            String command = "SELECT COUNT(*) FROM almox_contrato_impresso WHERE almox_contratoimp_numero=@NUM";
            String[] paramNm = new String[] { "@NUM" };
            String[] paramVl = new String[] { numeroContrato.ToString() };

            if (qtdZerosEsquerda > 0)
                command += " AND almox_contratoimp_zerosAEsquerda=" + qtdZerosEsquerda.ToString();

            if (!String.IsNullOrEmpty(letra))
                command += " AND almox_contratoimp_letra='" + letra + "'";

            Object ret = LocatorHelper.Instance.ExecuteScalar(command, paramNm, paramVl);

            if (ret == null || ret == DBNull.Value || Convert.ToInt32(ret) == 0)
                return false;
            else
                return true;
        }

        public static Boolean ExisteIntervaloRetirado(Int32 numDe, Int32 numAte, Int32 qtdZeros, String letra, Object operadoraId, PersistenceManager pm)
        {
            String query = "SELECT almox_contratoimp_id FROM almox_contrato_impresso INNER JOIN almox_produto ON almox_contratoimp_produtoId=almox_produto_id WHERE almox_contratoimp_produtorId IS NOT NULL AND almox_produto_operadoraId=" + operadoraId + " AND almox_contratoimp_numero IN (";
            String clausule = "";

            //String numero = "", mascara = "";

            for (Int32 i = numDe; i <= numAte; i++)
            {
                if (clausule.Length > 0) { clausule += ", "; }
                //inClausule += "'";

                //if (qtdZeros > 0)
                //{
                //    mascara = new String('0', qtdZeros);
                //    numero  = String.Format("{0:" + mascara + "}", i);
                //}

                //if (!String.IsNullOrEmpty(letra))
                //{
                //    numero = letra + numero;
                //}

                clausule += i.ToString(); //numero +"'";
            }

            clausule += ")";

            if (qtdZeros > 0)
                clausule += " AND almox_contratoimp_zerosAEsquerda=" + qtdZeros.ToString();

            if (!String.IsNullOrEmpty(letra))
                clausule += " AND almox_contratoimp_letra='" + letra + "'";

            DataSet ds = LocatorHelper.Instance.ExecuteQuery(query + clausule, "resultset");
            return ds != null && ds.Tables[0].Rows.Count > 0;
        }

        public static Boolean ContratoFoiRetiradoDoEstoque(Int32 numeroContrato, String letra, Int32 qtdZeros, Object operadoraId)
        {
            String command = "SELECT almox_contratoimp_produtorId FROM almox_contrato_impresso INNER JOIN almox_produto ON almox_contratoimp_produtoId=almox_produto_id WHERE almox_contratoimp_numero=@NUM AND almox_produto_operadoraId=" + operadoraId;
            String[] paramNm = new String[] { "@NUM" };
            String[] paramVl = new String[] { numeroContrato.ToString() };

            if (qtdZeros > 0)
                command += " AND almox_contratoimp_zerosAEsquerda=" + qtdZeros.ToString();

            if (!String.IsNullOrEmpty(letra))
                command += " AND almox_contratoimp_letra='" + letra + "'";

            Object ret = LocatorHelper.Instance.ExecuteScalar(command, paramNm, paramVl);

            if (ret == null || ret == DBNull.Value)
                return false;
            else
                return true;
        }

        public static IList<AlmoxContratoImpresso> CarregarRetiradas(Object movimentacaoId)
        {
            String qry = String.Concat("almox_contrato_impresso.*, almox_produto_operadoraId, operadora_nome, u.usuario_nome, u2.usuario_nome AS EMPRESTADO ",
                "FROM almox_contrato_impresso ",
                "   INNER JOIN almox_produto ON almox_contratoimp_produtoId=almox_produto_id ",
                "   INNER JOIN operadora ON operadora_id=almox_produto_operadoraId ",
                "   INNER JOIN usuario u ON almox_contratoimp_produtorId=u.usuario_id ",
                "   LEFT JOIN contrato ON contrato_operadoraId=operadora_id AND contrato_numeroId=almox_contratoimp_id ",
                "   LEFT JOIN usuario u2 ON contrato_donoid=u2.usuario_id ",
                "WHERE almox_contratoimp_movId IS NULL OR almox_contratoimp_movId=", movimentacaoId);

            return LocatorHelper.Instance.ExecuteQuery<AlmoxContratoImpresso>(qry, typeof(AlmoxContratoImpresso));
        }

        public static DataTable SumarioRetiradas(Object corretorId, Object produtoId, Boolean agrupado)
        {
            String qry = "";

            if (agrupado)
            {
                qry = String.Concat("SELECT almox_contratoimp_produtoId AS PRODID, almox_contratoimp_produtorId AS CORRETORID, u1.usuario_nome AS CORRETORNOME, almox_produto_descricao AS PRODUTONOME, operadora_nome AS OPERADORANOME, COUNT(almox_contratoimp_produtoId) as QTD ",
                    "FROM almox_contrato_impresso ",
                    "   INNER JOIN usuario u1 ON u1.usuario_id=almox_contratoimp_produtorId ",
                    "   INNER JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                    "   INNER JOIN operadora ON operadora_id=almox_produto_operadoraId ",
                    "   LEFT JOIN contrato ON contrato_numeroId=almox_contratoimp_id AND contrato_operadoraId=operadora_id ",
                    "WHERE (almox_contratoimp_rasurado = 0 OR almox_contratoimp_rasurado IS NULL) AND contrato_id IS NULL AND almox_contratoimp_produtorId=", corretorId,
                    " GROUP BY almox_contratoimp_produtoId, almox_contratoimp_produtorId, u1.usuario_nome, almox_produto_descricao, operadora_nome ",
                    " ORDER BY OPERADORANOME, PRODUTONOME ");
            }
            else
            {
                qry = String.Concat("SELECT almox_contratoimp_id AS ID, almox_contratoimp_produtoId AS PRODID, almox_contratoimp_numero AS PRODNUM, almox_contratoimp_produtorId AS CORRETORID, almox_contratoimp_letra AS PRODNUMLETRA, almox_contratoimp_zerosAEsquerda AS PRODNUMZEROS, u1.usuario_nome AS CORRETORNOME, almox_produto_descricao AS PRODUTONOME, operadora_nome AS OPERADORANOME, almox_contratoimp_dataRetirada AS DTRETIRADA, almox_contratoimp_rasurado AS RASURADO ",
                    "FROM almox_contrato_impresso ",
                    "   INNER JOIN usuario u1 ON u1.usuario_id=almox_contratoimp_produtorId ",
                    "   INNER JOIN almox_produto ON almox_produto_id=almox_contratoimp_produtoId ",
                    "   INNER JOIN operadora ON operadora_id=almox_produto_operadoraId ",
                    "   LEFT JOIN contrato ON contrato_numeroId=almox_contratoimp_id AND contrato_operadoraId=operadora_id ",
                    "WHERE (almox_contratoimp_rasurado = 0 OR almox_contratoimp_rasurado IS NULL) AND contrato_id IS NULL AND almox_contratoimp_produtorId=", corretorId, " AND almox_contratoimp_produtoId=", produtoId,
                    " ORDER BY OPERADORANOME, PRODUTONOME ");
            }

            return LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];
        }

        public static void RasuraProposta(Object impressoId)
        {
            String command = "UPDATE almox_contrato_impresso SET almox_contratoimp_rasurado=1 WHERE almox_contratoimp_id=" + impressoId;
            NonQueryHelper.Instance.ExecuteNonQuery(command, null);
        }

        public static void DESrasuraProposta(Object impressoId)
        {
            String command = "UPDATE almox_contrato_impresso SET almox_contratoimp_rasurado=0 WHERE almox_contratoimp_id=" + impressoId;
            NonQueryHelper.Instance.ExecuteNonQuery(command, null);
        }

        public static AlmoxContratoImpresso Carregar(Object operadoraId, String numero, String letra, Int32 qtdZerosEsquerda, PersistenceManager pm)
        {
            String qry = String.Concat("SELECT almox_contrato_impresso.* FROM almox_contrato_impresso INNER JOIN almox_produto ON almox_contratoimp_produtoId=almox_produto_id WHERE almox_produto_operadoraId=", operadoraId, " AND almox_contratoimp_numero='", numero, "'");

            if (qtdZerosEsquerda > 0)
                qry += " AND almox_contratoimp_zerosAEsquerda=" + qtdZerosEsquerda.ToString();

            if (!String.IsNullOrEmpty(letra))
                qry += " AND almox_contratoimp_letra='" + letra + "'";

            IList<AlmoxContratoImpresso> lista = LocatorHelper.Instance.ExecuteQuery<AlmoxContratoImpresso>(qry, typeof(AlmoxContratoImpresso), pm);

            if (lista == null)
                return null;
            else
                return lista[0];
        }
    }
}