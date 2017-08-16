namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [DBTable("almox_movimentacao")]
    public class AlmoxMovimentacao : EntityBase, IPersisteableEntity
    {
        public enum TipoMovimentacao : int
        {
            Entrada,
            Saida,
            Indefinido
        }

        public enum SubTipoMovimentacao : int
        {
            Normal,
            Perda,
            Devolucao,
            Outros
        }

        #region campos 

        Object _id;
        Object _movimentacaoId;
        Object _produtoId;
        String _obs;
        DateTime _data;
        int _qtd;
        int _qtdFlutuante;
        int _tipo;
        int _subTipo;
        int _numDe;
        int _numAte;
        int _numDeFlutuante;
        int _numAteFlutuante;
        Object _usuarioId;
        Object _usuarioRetirada;

        String _letra;
        Int32 _qtdZerosAEsquerda;

        //joinned
        int _produtoQtdAtual;
        String _produtoDescricao;
        String _tipoProdutoDescricao;
        String _operadoraNome;

        #endregion

        #region propriedades 

        [DBFieldInfo("almox_movimentacao_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        /// <summary>
        /// Caso seja uma movimentação de saida, retorna/seta o ID da movimentação de entrada
        /// fonte desta movimentação.
        /// </summary>
        [DBFieldInfo("almox_movimentacao_movimentacaoId", FieldType.Single)]
        public Object MovimentacaoID
        {
            get { return _movimentacaoId; }
            set { _movimentacaoId= value; }
        }

        [DBFieldInfo("almox_movimentacao_produtoId", FieldType.Single)]
        public Object ProdutoID
        {
            get { return _produtoId; }
            set { _produtoId= value; }
        }

        [DBFieldInfo("almox_movimentacao_obs", FieldType.Single)]
        public String Obs
        {
            get { return _obs; }
            set { _obs= value; }
        }

        [DBFieldInfo("almox_movimentacao_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [DBFieldInfo("almox_movimentacao_qtd", FieldType.Single)]
        public int QTD
        {
            get { return _qtd; }
            set { _qtd= value; }
        }

        [DBFieldInfo("almox_movimentacao_qtdFlutuante", FieldType.Single)]
        public int QTDFlutuante
        {
            get { return _qtdFlutuante; }
            set { _qtdFlutuante = value; }
        }

        [DBFieldInfo("almox_movimentacao_tipo", FieldType.Single)]
        public int TipoDeMovimentacao
        {
            get { return _tipo; }
            set { _tipo= value; }
        }

        [DBFieldInfo("almox_movimentacao_subTipo", FieldType.Single)]
        public int SubTipoDeMovimentacao
        {
            get { return _subTipo; }
            set { _subTipo= value; }
        }

        [DBFieldInfo("almox_movimentacao_numDe", FieldType.Single)]
        public int NumeracaoInicial
        {
            get { return _numDe; }
            set { _numDe= value; }
        }

        [DBFieldInfo("almox_movimentacao_numAte", FieldType.Single)]
        public int NumeracaoFinal
        {
            get { return _numAte; }
            set { _numAte= value; }
        }

        [DBFieldInfo("almox_movimentacao_numDeFlutuante", FieldType.Single)]
        public int NumeracaoInicialFlutuante
        {
            get { return _numDeFlutuante; }
            set { _numDeFlutuante= value; }
        }

        [DBFieldInfo("almox_movimentacao_numAteFlutuante", FieldType.Single)]
        public int NumeracaoFinalFlutuante
        {
            get { return _numAteFlutuante; }
            set { _numAteFlutuante= value; }
        }

        public String NumeracaoResumo
        {
            get
            {
                if (_numDe == 0 || _numAte == 0) { return String.Empty; }

                //return String.Concat(_numDe.ToString(), " a ", _numAte.ToString());

                String mascara = new String('0', _qtdZerosAEsquerda);
                String _de = String.Format("{0:" + mascara + "}", _numDe);
                String _ate = String.Format("{0:" + mascara + "}", _numAte);

                if (!String.IsNullOrEmpty(_letra))
                {
                    _de = Letra + _de;
                    _ate = Letra + _ate;
                }

                return String.Concat(_de, " a ", _ate);
            }
        }

        public String NumeracaoFlutuanteResumo
        {
            get
            {
                if (_numDeFlutuante == 0 || _numAteFlutuante == 0) { return String.Empty; }
                if (_numDeFlutuante == _numAteFlutuante) { return String.Empty; }

                String mascara = new String('0', _qtdZerosAEsquerda);
                String _de = String.Format("{0:" + mascara + "}", _numDeFlutuante);
                String _ate = String.Format("{0:" + mascara + "}", _numAteFlutuante);

                if (!String.IsNullOrEmpty(_letra))
                {
                    _de = Letra + _de;
                    _ate = Letra + _ate;
                }

                return String.Concat(_de, " a ", _ate);
            }
        }

        [DBFieldInfo("almox_movimentacao_usuarioId", FieldType.Single)]
        public Object UsuarioID
        {
            get { return _usuarioId; }
            set { _usuarioId= value; }
        }

        [DBFieldInfo("almox_movimentacao_usuarioRetiradaId", FieldType.Single)]
        public Object UsuarioRetiradaID
        {
            get { return _usuarioRetirada; }
            set { _usuarioRetirada= value; }
        }

        [DBFieldInfo("almox_movimentacao_letra", FieldType.Single)]
        public String Letra
        {
            get { return _letra; }
            set { _letra= value; }
        }

        [DBFieldInfo("almox_movimentacao_zerosAEsquerda", FieldType.Single)]
        public Int32 QtdZerosAEsquerda
        {
            get { return _qtdZerosAEsquerda; }
            set { _qtdZerosAEsquerda= value; }
        }

        [Joinned("almox_produto_descricao")]
        public String ProdutoDescricao
        {
            get { return _produtoDescricao; }
            set { _produtoDescricao= value; }
        }

        [Joinned("almox_produto_qtd")]
        public int ProdutoQTDAtual
        {
            get { return _produtoQtdAtual; }
            set { _produtoQtdAtual= value; }
        }

        [Joinned("almox_tipoproduto_descricao")]
        public String TipoProdutoDescricao
        {
            get { return _tipoProdutoDescricao; }
            set { _tipoProdutoDescricao= value; }
        }

        [Joinned("operadora_nome")]
        public String OperadoraNome
        {
            get { return _operadoraNome; }
            set { _operadoraNome= value; }
        }

        #endregion

        public AlmoxProduto AlmoxProduto
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public AlmoxMovimentacao() { _data = DateTime.Now; _subTipo = 0; }

        #region métodos EntityBase 

        public void Salvar()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.TransactionContext();

            try
            {
                pm.Save(this);
                AlmoxProduto.AlteraQTD(ref pm, (TipoMovimentacao)this._tipo, this._produtoId, this._qtd);

                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
        }

        public void Remover()
        {
            //try
            //{
            //    base.Remover(this);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            throw new ApplicationException("Not implemented method.");
        }

        public void Carregar()
        {
            base.Carregar(this);
        }
        #endregion

        public static IList<AlmoxMovimentacao> CarregarTodos(TipoMovimentacao tipo)
        {
            return CarregarTodos(tipo, null, null);
        }

        public static IList<AlmoxMovimentacao> CarregarTodasEntradas(String operadoraId, String filialId, String tipoProdutoId)
        {
            String conditions = "";

            if (operadoraId != null && operadoraId != "-1" && operadoraId != "0")
                conditions = " AND almox_produto_operadoraId=" + operadoraId;
            else if (operadoraId != null && operadoraId == "-1")
                conditions = " AND almox_produto_operadoraId IS NULL";

            if (filialId != null && filialId != "-1")
            {
                //if (conditions.Length > 0) { conditions += " AND "; }
                conditions += " AND almox_produto_filialId=" + filialId;
            }

            if (tipoProdutoId != null && tipoProdutoId != "-1")
            {
                //if (conditions.Length > 0) { conditions += " AND "; }
                conditions += " AND almox_produto_tipoId=" + tipoProdutoId;
            }

            String query = String.Concat("almox_movimentacao.*, almox_produto_qtd, almox_produto_descricao, almox_tipoproduto_descricao, operadora_nome FROM almox_movimentacao ",
                "INNER JOIN almox_produto ON almox_movimentacao_produtoId=almox_produto_id ",
                "INNER JOIN almox_tipo_produto ON almox_produto_tipoId=almox_tipoproduto_id ",
                "LEFT JOIN operadora ON almox_produto_operadoraId=operadora_id ",
                "WHERE almox_movimentacao_tipo=", Convert.ToInt32(TipoMovimentacao.Entrada), conditions, 
                " ORDER BY almox_movimentacao_data DESC");

            return LocatorHelper.Instance.ExecuteQuery<AlmoxMovimentacao>(query, typeof(AlmoxMovimentacao));
        }

        public static IList<AlmoxMovimentacao> CarregarTodos(TipoMovimentacao tipoMovimentacao, Object tipoProduto, Object produtoId)
        {
            String where = "";
            if (tipoMovimentacao != TipoMovimentacao.Indefinido)
            {
                where = "WHERE almox_movimentacao_tipo=" + Convert.ToInt32(tipoMovimentacao);
            }

            if (tipoProduto != null)
            {
                if (where.Length > 0) { where += " AND "; }
                else { where = "WHERE "; }

                where += "almox_produto_tipoId=" + tipoProduto;
            }

            if (produtoId != null)
            {
                if (where.Length > 0) { where += " AND "; }
                else { where = "WHERE "; }

                where += "almox_movimentacao_produtoId=" + produtoId;
            }

            String query = String.Concat("almox_movimentacao.*, almox_produto_qtd, almox_produto_descricao, almox_tipoproduto_descricao FROM almox_movimentacao ",
                "INNER JOIN almox_produto ON almox_movimentacao_produtoId=almox_produto_id ",
                "INNER JOIN almox_tipo_produto ON almox_produto_tipoId=almox_tipoproduto_id ",
                where,
                " ORDER BY almox_movimentacao_data DESC");

            return LocatorHelper.Instance.ExecuteQuery<AlmoxMovimentacao>(query, typeof(AlmoxMovimentacao));
        }

        public static IList<AlmoxMovimentacao> CarregaOwners(Object movimentacaoFilhaId, TipoMovimentacao tipo)
        {
            String where = " WHERE almox_movimentacao_movimentacaoId=" + movimentacaoFilhaId;
            if (tipo != TipoMovimentacao.Indefinido)
            {
                where = " AND almox_movimentacao_tipo=" + Convert.ToInt32(tipo);
            }

            String query = String.Concat("almox_movimentacao.*, almox_produto_qtd, almox_produto_descricao, almox_tipoproduto_descricao FROM almox_movimentacao ",
                "INNER JOIN almox_produto ON almox_movimentacao_produtoId=almox_produto_id ",
                "INNER JOIN almox_tipo_produto ON almox_produto_tipoId=almox_tipoproduto_id ",
                where,
                " ORDER BY almox_movimentacao_data DESC");

            return LocatorHelper.Instance.ExecuteQuery<AlmoxMovimentacao>(query, typeof(AlmoxMovimentacao));
        }

        /// <summary>
        /// Checa se houve uma entrada em estoque para o intervalo.
        /// </summary>
        public static Boolean ExisteIntervalo(int numeroInicio, int numeroFinal, Object operadoraId, Int32 qtdZerosEsquerda, String letraTematica)
        {
            String query = "SELECT almox_movimentacao_id FROM  almox_movimentacao INNER JOIN almox_produto ON almox_movimentacao_produtoId = almox_produto_id WHERE almox_movimentacao_tipo=0 AND almox_movimentacao_numDe <= " + numeroInicio.ToString() + " AND almox_movimentacao_numAte >= " + numeroFinal.ToString();
            
            if(operadoraId != null)
                query += " AND almox_produto_operadoraId=" + operadoraId;

            if (!String.IsNullOrEmpty(letraTematica))
                query += " AND almox_movimentacao_letra='" + letraTematica + "'";
            if(qtdZerosEsquerda > 0)
                query += " AND almox_movimentacao_zerosAEsquerda=" + qtdZerosEsquerda;

            System.Data.DataTable dt = LocatorHelper.Instance.ExecuteQuery(query, "resultset").Tables[0];

            Boolean result = dt != null && dt.Rows.Count > 0;
            if (dt != null) { dt.Dispose(); }

            return result;
        }

        /// <summary>
        /// Checa se houve uma entrada em estoque para o intervalo.
        /// </summary>
        public static Boolean ExisteIntervalo(int numeroInicio, int numeroFinal)
        {
            String query = "SELECT almox_movimentacao_id FROM  almox_movimentacao WHERE almox_movimentacao_tipo=0 AND almox_movimentacao_numDe <= " + numeroInicio.ToString() + " AND almox_movimentacao_numAte >= " + numeroFinal.ToString();

            System.Data.DataTable dt = LocatorHelper.Instance.ExecuteQuery(query, "resultset").Tables[0];

            Boolean result = dt != null && dt.Rows.Count > 0;
            if (dt != null) { dt.Dispose(); }

            return result;
        }

        /// <summary>
        /// Checa se um determinado intervado de produtos numerados foi retirado do estoque.
        /// True = INTERVALO NÃO ESTÁ NO ESTOQUE, FOI RETIRADO
        /// </summary>
        public static Boolean ExisteIntervaloRetirado(int numeroInicio, int numeroFinal)
        {
            String query = "SELECT almox_movimentacao_id, almox_movimentacao_data FROM  almox_movimentacao WHERE almox_movimentacao_tipo=1  and almox_movimentacao_numDe <= " + numeroInicio.ToString() + " AND almox_movimentacao_numAte >= " + numeroFinal.ToString() + " ORDER BY almox_movimentacao_data DESC";
            System.Data.DataTable dtSaida = LocatorHelper.Instance.ExecuteQuery(query, "resultset").Tables[0];

            if (dtSaida == null || dtSaida.Rows.Count == 0) { return false; }//nao consta SAÍDA deste intervalo no estoque

            query = "SELECT almox_movimentacao_id, almox_movimentacao_data FROM  almox_movimentacao WHERE almox_movimentacao_tipo=0  and almox_movimentacao_numDe <= " + numeroInicio.ToString() + " AND almox_movimentacao_numAte >= " + numeroFinal.ToString() + " ORDER BY almox_movimentacao_data DESC";
            System.Data.DataTable dtEntrada = LocatorHelper.Instance.ExecuteQuery(query, "resultset2").Tables[0];

            if (dtEntrada == null || dtEntrada.Rows.Count == 0) { return false; } //nao consta ENTRADA deste intervalo no estoque

            //para estar fora do estoque, a data da última saída deve ser superior à data da última entrada
            DateTime dataSaida = Convert.ToDateTime(dtSaida.Rows[0]["almox_movimentacao_data"]);
            DateTime dataEntrada = Convert.ToDateTime(dtEntrada.Rows[0]["almox_movimentacao_data"]);

            Boolean result = dataSaida > dataEntrada;
            dtSaida.Dispose();
            dtEntrada.Dispose();

            return result;
        }

        public static Boolean ExisteIntervaloRetirado(int numeroInicio, int numeroFinal, Object produtoId, Int32 qtdZerosEsquerda, String letraTematica)
        {
            String numeroDe = "", numeroAte = "";

            //if (qtdZerosEsquerda > 0)
            //{
            //    String mascara = new String('0', qtdZerosEsquerda);
            //    numeroDe = String.Format("{0:" + mascara + "}", numeroInicio);
            //    numeroAte = String.Format("{0:" + mascara + "}", numeroFinal);
            //}
            //else
            //{
                numeroDe  = numeroInicio.ToString();
                numeroAte = numeroFinal.ToString();
            //}

            //if (!String.IsNullOrEmpty(letraTematica))
            //{
            //    numeroDe = letraTematica + numeroDe;
            //    numeroAte = letraTematica + numeroAte;
            //}

            String query = String.Concat("SELECT COUNT(almox_contratoimp_id) FROM  almox_contrato_impresso WHERE almox_contratoimp_produtorId IS NOT NULL AND almox_contratoimp_produtoId = " , produtoId , "  AND almox_contratoimp_numero BETWEEN " , numeroDe , " AND " , numeroAte);
            Object ret = LocatorHelper.Instance.ExecuteScalar(query, null, null);

            if (ret != null && ret != DBNull.Value && Convert.ToInt32(ret) > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checa se um intervalo de contratos foi usando em estoque.
        /// TODO: Acrescentar na checagem a qtd de zeros e a letra temática
        /// </summary>
        public static Boolean UsadoEmContrato(int numeroInicio, int numeroFinal)
        {
            String query = "SELECT contrato_id FROM  contrato WHERE contrato_numero IN(";
            String inclausule = "";

            for (int i = numeroInicio; i <= numeroFinal; i++)
            {
                if (inclausule.Length > 0) { inclausule += ","; }
                inclausule += "'" + i.ToString() + "'";
            }

            query += inclausule + ")";

            System.Data.DataTable dt = LocatorHelper.Instance.ExecuteQuery(query, "resultset").Tables[0];

            Boolean result = dt != null && dt.Rows.Count > 0;
            if (dt != null) { dt.Dispose(); }

            return result;
        }
    }

    [DBTable("almox_produto_agente")]
    public sealed class ProdutoCorretor : EntityBase, IPersisteableEntity
    {
        Object _id;
        Object _entradaId;
        Object _produtoId;
        int _produtoNumero;
        int _produtoQtd;
        Object _agenteId;
        DateTime _data;

        Object _operadoraId;
        String _operadoraNome;
        String _agenteNome;
        Object _contratoId;

        #region propriedades 

        [DBFieldInfo("produtocorretor_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("produtocorretor_entradaid", FieldType.Single)]
        public Object EntradaID
        {
            get { return _entradaId; }
            set { _entradaId= value; }
        }
          

        [DBFieldInfo("produtocorretor_produtoid", FieldType.Single)]
        public Object ProdutoID
        {
            get { return _produtoId; }
            set { _produtoId= value; }
        }

        [DBFieldInfo("produtocorretor_produtoNumero", FieldType.Single)]
        public int ProdutoNumero
        {
            get { return _produtoNumero; }
            set { _produtoNumero = value; }
        }

        [DBFieldInfo("produtocorretor_produtoQTD", FieldType.Single)]
        public int ProdutoQTD
        {
            get { return _produtoQtd; }
            set { _produtoQtd= value; }
        }

        [DBFieldInfo("produtocorretor_agenteid", FieldType.Single)]
        public Object AgenteID
        {
            get { return _agenteId; }
            set { _agenteId= value; }
        }

        [DBFieldInfo("produtocorretor_data", FieldType.Single)]
        public DateTime Data
        {
            get { return _data; }
            set { _data= value; }
        }

        [Joinned("usuario_nome")]
        public String AgenteNome
        {
            get { return _agenteNome; }
            set { _agenteNome = value; }
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

        [Joinned("contrato_id")]
        public Object ContratoID
        {
            get { return _contratoId; }
            set { _contratoId= value; }
        }

        public String NumOuQtd
        {
            get
            {
                if (_produtoNumero > 0)
                    return _produtoNumero.ToString();
                else
                    return _produtoQtd.ToString();
            }
        }

        [Obsolete("Em desuso", false)]
        public Boolean Rasurado
        {
            get { return false; }
            set { }
        }

        #endregion

        public ProdutoCorretor() { _data = DateTime.Now; }

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

        public static IList<ProdutoCorretor> Carregar(Object movimentacaoId)
        {
            String query = "almox_produto_agente.*, usuario_nome FROM almox_produto_agente INNER JOIN usuario ON produtocorretor_agenteid=usuario_id WHERE produtocorretor_entradaid=" + movimentacaoId + " ORDER BY produtocorretor_produtoNumero";

            return LocatorHelper.Instance.ExecuteQuery<ProdutoCorretor>(query, typeof(ProdutoCorretor));
        }

        public static IList<ProdutoCorretor> CarregarPorProdutoNumero(Int32 produtoNumero)
        {
            String query = String.Concat("SELECT contrato_id, almox_produto_agente.*, almox_produto_operadoraId, operadora_nome",
                " FROM almox_produto_agente",
                " INNER JOIN almox_produto ON produtocorretor_produtoId=almox_produto_id",
                " INNER JOIN operadora ON almox_produto_operadoraId=operadora_id",
                " LEFT JOIN contrato ON contrato_operadoraId=operadora_id AND contrato_numero=produtocorretor_produtoNumero",
                " WHERE produtocorretor_produtonumero=", produtoNumero);

            IList<ProdutoCorretor> returned = LocatorHelper.Instance.ExecuteQuery<ProdutoCorretor>(query, typeof(ProdutoCorretor));

            if (returned == null) { return null; }
            List<ProdutoCorretor> lista = new List<ProdutoCorretor>();

            foreach (ProdutoCorretor pc in returned)
            {
                if (pc.ContratoID == null) { lista.Add(pc); }
            }

            return lista;
        }

        public static Boolean Remove(Object agenteId, Object produtoId, int numero, int qtd, PersistenceManager pm)
        {
            String cmd = "";

            if (qtd > 0)
            {
                String sql = "SELECT TOP 1 produtocorretor_id, produtocorretor_produtoQTD FROM almox_produto_agente WHERE produtocorretor_produtoQTD >= " + qtd.ToString() + " AND produtocorretor_agenteid=" + agenteId + " AND produtocorretor_produtoid=" + produtoId;
                System.Data.DataTable dt = LocatorHelper.Instance.ExecuteQuery(sql, "resultset").Tables[0];

                if (dt != null && dt.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dt.Rows[0]["produtocorretor_produtoQTD"]) == qtd)
                    {
                        //remove
                        cmd = "DELETE FROM almox_produto_agente WHERE produtocorretor_id=" + dt.Rows[0]["produtocorretor_id"];
                        return NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm) > 0;
                    }
                    else
                    {
                        //atualiza
                        cmd = "UPDATE almox_produto_agente SET produtocorretor_produtoQTD = (produtocorretor_produtoQTD-" + qtd.ToString() + ") WHERE produtocorretor_id=" + dt.Rows[0]["produtocorretor_id"];
                        return NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm) > 0;
                    }
                }
                else
                    return false;
            }
            else
            {

                cmd = "DELETE FROM almox_produto_agente WHERE produtocorretor_agenteid=" + agenteId;
                cmd += " AND produtocorretor_produtoNumero=" + numero.ToString();
                cmd += " AND produtocorretor_produtoid=" + produtoId;

                return NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm) > 0;
            }
        }
    }
}