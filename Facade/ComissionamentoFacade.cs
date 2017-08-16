namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.Data;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity;

    public sealed class ComissionamentoFacade
    {
        #region Singleton 

        static ComissionamentoFacade _instance;
        public static ComissionamentoFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new ComissionamentoFacade(); }
                return _instance;
            }
        }
        #endregion

        ComissionamentoFacade() {}

        class CobrancaCorretorChefiaVO
        {
            String _cobrancaId;
            ArrayList _produtorIds;

            public String CobrancaID
            {
                get { return _cobrancaId; }
                set { _cobrancaId= value; }
            }
            public ArrayList ProdutorIDs
            {
                get { if (_produtorIds == null) { _produtorIds = new ArrayList(); } return _produtorIds; }
                set { _produtorIds = value; }
            }

            public CobrancaCorretorChefiaVO() { }
        }

        /// <summary>
        /// Adiciona um produtor que seja COMISSIONAVEL e que esteja no grafo da equipe para uma cobranca.
        /// </summary>
        void adicionaGrafoDeProdutoresParaCobranca(ref List<CobrancaCorretorChefiaVO> vos, Object cobrancaId, Object produtorId, Boolean procurar)
        {
            if (procurar) //checa se ja existe antes adicionar
            {
                foreach (CobrancaCorretorChefiaVO vo in vos)
                {
                    if (vo.CobrancaID == Convert.ToString(cobrancaId)) //primeira chave é o ID da cobrança
                    {
                        foreach (Object id in vo.ProdutorIDs) //para a cobrança corrente, checa os produtores adicionados
                        {
                            if (Convert.ToString(id) == Convert.ToString(produtorId)) { return; } //já está adicionado. nao faz nada
                        }

                        vo.ProdutorIDs.Add(Convert.ToString(produtorId));
                        return;
                    }
                }
            }

            CobrancaCorretorChefiaVO _vo = new CobrancaCorretorChefiaVO();
            _vo.CobrancaID = Convert.ToString(cobrancaId);
            _vo.ProdutorIDs.Add(produtorId);
            vos.Add(_vo);
        }

        /// <summary>
        /// TODO: refatorar para otimizar.
        /// </summary>
        public DataTable CarregaRelacaoEmAberto(String nomeListagem, String mensagemListagem, String[] filialIDs, String[] operadoraIDs, String[] perfilIDs, DateTime? dataCorte, Object produtorId)
        {
            #region variáveis 

            Object grupoId = null;
            List<CobrancaCorretorChefiaVO> vos = new List<CobrancaCorretorChefiaVO>();
            DataTable dt = null, resultado = new DataTable();
            Usuario corretor = null;
            ComissionamentoUsuario comU = null;
            DateTime? vigencia = null, admissao = null; 
            IList<ComissionamentoItem> itensComissionamento = null;
            ComissionamentoVitaliciedade itemVitaliciedade = null;
            IList<SuperiorSubordinado> superiores = null;
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            Int32 tipoContrato = -1, parcela = -1;
            Decimal valorComissao = 0, creditos = 0, percentualComissao = 0, valorPago = 0;
            List<MovimentacaoContaCorrente> itensCC = null;
            MovimentacaoContaCorrente itemCC = null;
            TipoContrato.TipoComissionamentoProdutorOuOperadora tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal;
            UsuarioFilial usuarioFilial = null;
            Comissionamento comissionamentoModelo = null;
            Contrato contrato = null;
            IList<TabelaValor> tabelasValor = null;
            Taxa taxa = null;

            #endregion

            #region configura DataTable resultado 

            resultado.Columns.Add("CobrancaID");
            resultado.Columns.Add("CobrancaVencimento");
            resultado.Columns.Add("ContratoNumero");
            resultado.Columns.Add("ContratoID");
            resultado.Columns.Add("ContratoVigencia");
            resultado.Columns.Add("OperadoraID");
            resultado.Columns.Add("OperadoraNome");
            resultado.Columns.Add("CobrancaValorPago");
            resultado.Columns.Add("CobrancaDataPago");
            resultado.Columns.Add("CobrancaParcela");
            resultado.Columns.Add("ProdutorID");
            resultado.Columns.Add("ProdutorNome");
            resultado.Columns.Add("ProdutorValor");
            resultado.Columns.Add("ProdutorPercentualComissao");
            resultado.Columns.Add("ProdutorCredito");
            resultado.Columns.Add("ProdutorPerfilID");
            resultado.Columns.Add("ProdutorApelido");
            resultado.Columns.Add("SuperiorApelido");
            resultado.Columns.Add("NomeTitular");
            resultado.Columns.Add("ContratoAdmissao");
            resultado.Columns.Add("FecharComissao");

            resultado.Columns.Add("ProdutorBanco");
            resultado.Columns.Add("ProdutorAgencia");
            resultado.Columns.Add("ProdutorConta");

            #endregion

            #region configura parâmetros da query 

            String filiaisParam = "", operadorasParam = "", perfisParam = "", dataParam = "";
            if (filialIDs != null && filialIDs.Length > 0)
            {
                filiaisParam = String.Concat(" AND usuariofilial_filialId IN (", String.Join(",", filialIDs), ") "); //String.Concat(" AND almox_produto_filialId IN (", String.Join(",", filialIDs), ") "); //filiaisParam = String.Concat(" AND usuariofilial_data <= contrato_admissao AND usuario_filialId IN (", String.Join(",", filialIDs), ") ");
            }

            if (operadoraIDs != null && operadoraIDs.Length > 0)
                operadorasParam = String.Concat(" AND contrato_operadoraId IN (", String.Join(",", operadoraIDs), ") ");

            if (perfilIDs != null && perfilIDs.Length > 0)
                perfisParam = String.Join(",", perfilIDs);

            if (dataCorte != null)
                dataParam = String.Concat(" AND cobranca_dataPagto <= '", dataCorte.Value.ToString("yyyy-MM-dd 23:59:59.700"), "' ");

            #endregion

            //faz join com a tabela de usuários para poder usar o filtro por filiais!
            String qry = String.Concat("cobranca.*, beneficiario_nome, contrato_cobrarTaxaAssociativa, contrato_donoId, contrato_numero, contrato_contratoAdmId, contrato_admissao, contrato_vigencia, contrato_tipoContratoId, operadora_id, operadora_nome ",
                "   FROM cobranca ",
                "       INNER JOIN contrato                ON cobranca_propostaId=contrato_id ",
                //"       INNER JOIN almox_contrato_impresso ON almox_contratoimp_id=contrato_numeroId ",
                //"       INNER JOIN almox_produto           ON almox_produto_id=almox_contratoimp_produtoId ",
                "       INNER JOIN operadora               ON operadora_id=contrato_operadoraId ",
                "       INNER JOIN contrato_beneficiario   ON contratobeneficiario_contratoId=contrato_id AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN beneficiario            ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_tipo=", Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular),
                "       INNER JOIN usuario                 ON usuario_id=contrato_donoId ",
                "       INNER JOIN usuario_filial          ON usuario_id=usuariofilial_usuarioId ",
                "   WHERE ", 
                "   (cobranca_comissaoPaga=0 OR cobranca_comissaoPaga IS NULL) AND cobranca_pago=1 AND cobranca_parcela > 1 ",
                filiaisParam, operadorasParam, dataParam);

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];
                if (dt.Rows.Count == 0) { pm.Commit(); dt.Dispose(); return null; }

                //ARMAZENA todos os corretores em uma colecao de ids de corretor, sem duplicar.
                System.Collections.ArrayList array = new System.Collections.ArrayList();
                foreach (DataRow row in dt.Rows)
                {
                    if (!array.Contains(Convert.ToString(row["contrato_donoId"])))
                    {
                        array.Add(Convert.ToString(row["contrato_donoId"]));
                    }
                }

                #region Salva a listagem ------------------------------------------------------------

                Listagem listagem = new Listagem();
                listagem.Nome = nomeListagem;
                listagem.Mensagem = mensagemListagem;
                if (dataCorte != null) { listagem.DataCorte = dataCorte.Value; }
                pm.Save(listagem);

                #region filiais

                if (filialIDs != null && filialIDs.Length > 0)
                {
                    Listagem.Filial lf;
                    foreach (String filialID in filialIDs)
                    {
                        lf = new Listagem.Filial();
                        lf.ListagemID = listagem.ID;
                        lf.FilialID = filialID;
                        pm.Save(lf);
                    }
                }
                #endregion

                #region operadoras

                if (operadoraIDs != null && operadoraIDs.Length > 0)
                {
                    Listagem.Operadora lo;
                    foreach (String operadoraID in operadoraIDs)
                    {
                        lo = new Listagem.Operadora();
                        lo.ListagemID = listagem.ID;
                        lo.OperadoraID = operadoraID;
                        pm.Save(lo);
                    }
                }
                #endregion

                #region perfis

                if (perfilIDs != null && perfilIDs.Length > 0)
                {
                    Listagem.Perfil lp;
                    foreach (String perfilID in perfilIDs)
                    {
                        lp = new Listagem.Perfil();
                        lp.ListagemID = listagem.ID;
                        lp.PerfilID = perfilID;
                        pm.Save(lp);
                    }
                }
                #endregion

                #endregion

                //passeia por todos os corretores
                DataRow[] rows;
                DataRow resultadoLinha;
                foreach (Object corretorId in array)
                {
                    //percorre os contratos do corretor corrente
                    rows = dt.Select("contrato_donoId=" + corretorId);
                    foreach (DataRow row in rows)
                    {
                        valorComissao = 0;
                        vigencia = Convert.ToDateTime(row["contrato_vigencia"], cinfo);
                        admissao = Convert.ToDateTime(row["contrato_admissao"], cinfo);

                        usuarioFilial = UsuarioFilial.CarregarVigente(row["contrato_donoId"], admissao.Value, pm);
                        if (usuarioFilial == null || !contemValor(filialIDs, usuarioFilial.FilialID))
                        {
                            continue;
                        }

                        //Corretor
                        corretor = new Usuario(row["contrato_donoId"]);
                        pm.Load(corretor);

                        //comissao do corretor - leva em consideracao a data de admissao do contrato
                        comU = ComissionamentoUsuario.CarregarVigente(corretor.ID, vigencia, pm);

                        if (comU != null)
                        {
                            resultadoLinha = resultado.NewRow();

                            #region preenche a linha 

                            resultadoLinha["CobrancaID"]        = row["cobranca_id"];
                            resultadoLinha["CobrancaVencimento"]= Convert.ToDateTime(row["cobranca_dataVencimento"], cinfo).ToString("dd/MM/yyyy");

                            valorPago = Convert.ToDecimal(row["cobranca_valor"], cinfo);
                            //if (cToString(row["contrato_cobrarTaxaAssociativa"]) == "1")
                            //{
                            //    contrato = Contrato.CarregarParcial(row["cobranca_propostaId"], pm);
                            //    valorPago -= Contrato.CalculaValorDaTaxaAssociativa(contrato, -1, pm);
                            //}

                            //tabelasValor = TabelaValor.CarregarPorContratoID_Parcial(row["contrato_contratoAdmId"], pm);
                            //if (tabelasValor != null)
                            //{
                            //    if (tabelasValor.Count == 1)
                            //    {
                            //        taxa = Taxa.CarregarPorTabela(tabelasValor[0].ID, pm);
                            //        if (taxa != null && !taxa.Embutido && taxa.ValorEmbutido > 0)
                            //        {
                            //            valorPago -= taxa.ValorEmbutido;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        //o que fazer...
                            //    }
                            //}

                            resultadoLinha["CobrancaValorPago"] = valorPago.ToString("N2");
                            resultadoLinha["CobrancaDataPago"]  = Convert.ToDateTime(row["cobranca_dataPagto"], cinfo).ToString("dd/MM/yyyy");
                            resultadoLinha["ContratoNumero"]    = row["contrato_numero"];
                            resultadoLinha["ContratoID"]        = row["cobranca_propostaId"];
                            resultadoLinha["ContratoVigencia"]  = Convert.ToDateTime(row["contrato_vigencia"], cinfo).ToString("dd/MM/yyyy");
                            resultadoLinha["CobrancaParcela"]   = row["cobranca_parcela"];
                            resultadoLinha["OperadoraID"]       = row["operadora_id"];
                            resultadoLinha["OperadoraNome"]     = row["operadora_nome"];
                            resultadoLinha["ProdutorID"]        = row["contrato_donoId"];
                            resultadoLinha["ProdutorPerfilID"]  = corretor.PerfilID;
                            resultadoLinha["ProdutorApelido"]   = corretor.Apelido;
                            resultadoLinha["NomeTitular"]       = row["beneficiario_nome"];
                            resultadoLinha["ContratoAdmissao"]  = Convert.ToDateTime(row["contrato_admissao"], cinfo).ToString("dd/MM/yyyy");

                            resultadoLinha["ProdutorBanco"]     = corretor.Banco;
                            resultadoLinha["ProdutorAgencia"]   = corretor.Agencia;
                            resultadoLinha["ProdutorConta"]     = corretor.Conta;

                            resultadoLinha["FecharComissao"]    = "0";
                            #endregion

                            #region Comissionamento 

                            percentualComissao = 0;
                            comissionamentoModelo = new Comissionamento(comU.TabelaComissionamentoID);
                            pm.Load(comissionamentoModelo);

                            grupoId = ComissionamentoGrupo.ObterID(comissionamentoModelo.ID, row["contrato_contratoAdmId"], pm);
                            if (grupoId != null)
                            {
                                itensComissionamento = ComissionamentoItem.Carregar(grupoId, pm);
                            }
                            else
                                continue;

                            if (itensComissionamento != null)
                            {
                                parcela = Convert.ToInt32(row["cobranca_parcela"]);
                                tipoContrato = Convert.ToInt32(row["contrato_tipoContratoId"]);
                                foreach (ComissionamentoItem item in itensComissionamento)
                                {
                                    if (parcela == item.Parcela)
                                    {
                                        if (tipoContrato == 1) //normal
                                        {
                                            tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal;
                                            valorComissao = valorPago * (item.Percentual / 100);
                                            percentualComissao = item.Percentual;
                                            break;
                                        }
                                        else if (tipoContrato == 4) //carencia
                                        {
                                            tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia;
                                            valorComissao = valorPago * (item.PercentualCompraCarencia / 100);
                                            percentualComissao = item.PercentualCompraCarencia;
                                            break;
                                        }
                                        else if (tipoContrato == 3) //migracao
                                        {
                                            tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao;
                                            valorComissao = valorPago * (item.PercentualMigracao / 100);
                                            percentualComissao = item.PercentualMigracao;
                                            break;
                                        }
                                        else if (tipoContrato == 2) //adm
                                        {
                                            tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Administrativa;
                                            valorComissao = valorPago * (item.PercentualADM / 100);
                                            percentualComissao = item.PercentualADM;
                                            break;
                                        }
                                        else if (tipoContrato == 5) //especial
                                        {
                                            tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial;
                                            valorComissao = valorPago * (item.PercentualEspecial / 100);
                                            percentualComissao = item.PercentualEspecial;
                                            break;
                                        }
                                        else if (tipoContrato == 6) //idade
                                        {
                                            tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Idade;
                                            valorComissao = valorPago * (item.Idade / 100);
                                            percentualComissao = item.Idade;
                                            break;
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region Vitaliciedade - COMO EXIBIR NO RELATORIO ESTE PERCENTUAL DE COMISSAO ??? 

                            //itemVitaliciedade = ComissionamentoVitaliciedade.Carregar(comU.TabelaComissionamentoID, row["contrato_contratoAdmId"], tipoComissionamento, pm);
                            itemVitaliciedade = ComissionamentoVitaliciedade.Carregar(grupoId, tipoComissionamento, pm);
                            if (itemVitaliciedade != null && itemVitaliciedade.ParcelaInicio >= parcela)
                            {
                                valorComissao += (valorPago * (itemVitaliciedade.Percentual / 100));
                            }
                            #endregion

                            resultadoLinha["ProdutorNome"]               = corretor.Nome;
                            resultadoLinha["ProdutorValor"]              = valorComissao.ToString("N2");
                            resultadoLinha["ProdutorPercentualComissao"] = percentualComissao.ToString("N2");

                            //se nao foi informado um produtor especifico ou 
                            //foi informado e é o mesmo que o dono do contrato (corretor que vendeu)
                            if (produtorId == null ||
                                Convert.ToString(produtorId) == Convert.ToString(corretor.ID))
                            {
                                //checa também se o perfil está incluso na relacao de perfis enviada por parametro
                                if (perfilIDs == null || contemValor(perfilIDs, corretor.PerfilID))
                                {
                                    resultadoLinha["FecharComissao"] = "1";

                                    #region itens abertos na conta corrente 

                                    creditos = 0;
                                    itensCC = (List<MovimentacaoContaCorrente>)MovimentacaoContaCorrente.CarregarEmAberto(corretor.ID, dataCorte.Value, pm);
                                    if (itensCC == null) { itensCC = new List<MovimentacaoContaCorrente>(); }

                                    //só salva o credito se ele ainda nao foi feito.
                                    if (valorComissao > 0 && !MovimentacaoContaCorrente.CreditoJaFeitoPara(corretor.ID, row["cobranca_id"], pm)) //itemCC != null && itemCC.Valor > 0)
                                    {
                                        itemCC = new MovimentacaoContaCorrente();
                                        itemCC.CategoriaID = CategoriaContaCorrente.SysPremiacaoCategoriaID;
                                        itemCC.CobrancaID = row["cobranca_id"];
                                        itemCC.Data = DateTime.Now;
                                        itemCC.LisagemFechamentoID = listagem.ID;
                                        itemCC.ProdutorID = corretor.ID;
                                        itemCC.Valor = valorComissao;
                                        pm.Save(itemCC);
                                    }

                                    foreach (MovimentacaoContaCorrente _item in itensCC)
                                    {
                                        if (CategoriaContaCorrente.eTipo.Credito == (CategoriaContaCorrente.eTipo)_item.CategoriaTipo)
                                            creditos += _item.Valor;
                                        else
                                            creditos -= _item.Valor;

                                        _item.LisagemFechamentoID = listagem.ID;
                                        pm.Save(_item);
                                    }
                                    #endregion
                                }
                            }

                            resultadoLinha["ProdutorCredito"] = creditos.ToString("N2");
                            resultado.Rows.Add(resultadoLinha);
                            adicionaGrafoDeProdutoresParaCobranca(ref vos, row["cobranca_id"], corretor.ID, false);
                            //salvarGrafoEquipe(corretor.ID, corretor.PerfilID, listagem.ID, pm);
                        }

                        //Superiores
                        superiores = SuperiorSubordinado.CarregarSuperiores(corretor.ID, admissao, pm);
                        while (superiores != null && superiores.Count > 0)
                        {
                            valorComissao  = 0;
                            comU = ComissionamentoUsuario.CarregarVigente(superiores[0].SuperiorID, vigencia, pm);

                            if (superiores[0].SubordinadoID.ToString() == corretor.ID.ToString())
                            {
                                salvarGrafoEquipeCorretor(corretor.ID, corretor.PerfilID, listagem.ID, pm);
                            }
                            //salvarGrafoEquipe(superiores[0], listagem.ID, pm);
                            if (comU == null) { superiores = SuperiorSubordinado.CarregarSuperiores(superiores[0].SuperiorID, admissao, pm); continue; }
                            comissionamentoModelo = new Comissionamento(comU.TabelaComissionamentoID);
                            pm.Load(comissionamentoModelo);

                            salvarGrafoEquipe(superiores[0], listagem.ID, pm);

                            if (comU != null)
                            {
                                resultadoLinha = resultado.NewRow();

                                #region preenche a linha 

                                resultadoLinha["CobrancaID"]        = row["cobranca_id"];
                                resultadoLinha["CobrancaVencimento"]= Convert.ToDateTime(row["cobranca_dataVencimento"], cinfo).ToString("dd/MM/yyyy");
                                resultadoLinha["CobrancaValorPago"] = valorPago; //Convert.ToDecimal(row["cobranca_valorPagto"], cinfo).ToString("N2");
                                resultadoLinha["CobrancaDataPago"]  = Convert.ToDateTime(row["cobranca_dataPagto"], cinfo).ToString("dd/MM/yyyy");
                                resultadoLinha["CobrancaParcela"]   = row["cobranca_parcela"];
                                resultadoLinha["ContratoNumero"]    = row["contrato_numero"];
                                resultadoLinha["ContratoID"]        = row["cobranca_propostaId"];
                                resultadoLinha["ContratoVigencia"]  = Convert.ToDateTime(row["contrato_vigencia"], cinfo).ToString("dd/MM/yyyy");
                                resultadoLinha["OperadoraID"]       = row["operadora_id"];
                                resultadoLinha["OperadoraNome"]     = row["operadora_nome"];
                                resultadoLinha["ProdutorID"]        = superiores[0].SuperiorID;
                                resultadoLinha["ProdutorPerfilID"]  = superiores[0].SuperiorPerfilID;
                                resultadoLinha["ProdutorApelido"]   = superiores[0].SuperiorApelido;
                                resultadoLinha["NomeTitular"]       = row["beneficiario_nome"];
                                resultadoLinha["ContratoAdmissao"]  = Convert.ToDateTime(row["contrato_admissao"], cinfo).ToString("dd/MM/yyyy");
                                resultadoLinha["FecharComissao"]    = "0";

                                resultadoLinha["ProdutorBanco"]     = superiores[0].SuperiorBanco;
                                resultadoLinha["ProdutorAgencia"]   = superiores[0].SuperiorAgencia;
                                resultadoLinha["ProdutorConta"]     = superiores[0].SuperiorConta;

                                #endregion

                                #region Comissionamento 

                                comissionamentoModelo = new Comissionamento(comU.TabelaComissionamentoID);
                                pm.Load(comissionamentoModelo);

                                itensComissionamento  = ComissionamentoItem.Carregar(comissionamentoModelo.GrupoID, pm);
                                valorComissao = 0; percentualComissao = 0;

                                if (itensComissionamento != null)
                                {
                                    parcela = Convert.ToInt32(row["cobranca_parcela"]);
                                    tipoContrato = Convert.ToInt32(row["contrato_tipoContratoId"]);
                                    foreach (ComissionamentoItem item in itensComissionamento)
                                    {
                                        if (parcela == item.Parcela)
                                        {
                                            if (tipoContrato == 1) //normal
                                            {
                                                tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal;
                                                valorComissao = valorPago * (item.Percentual / 100);
                                                percentualComissao = item.Percentual;
                                                break;
                                            }
                                            else if (tipoContrato == 4) //carencia
                                            {
                                                tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Carencia;
                                                valorComissao = valorPago * (item.PercentualCompraCarencia / 100);
                                                percentualComissao = item.PercentualCompraCarencia;
                                                break;
                                            }
                                            else if (tipoContrato == 3) //migracao
                                            {
                                                tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Migracao;
                                                valorComissao = valorPago * (item.PercentualMigracao / 100);
                                                percentualComissao = item.PercentualMigracao;
                                                break;
                                            }
                                            else if (tipoContrato == 2) //adm
                                            {
                                                tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Administrativa;
                                                valorComissao = valorPago * (item.PercentualADM / 100);
                                                percentualComissao = item.PercentualADM;
                                                break;
                                            }
                                            else if (tipoContrato == 5) //especial
                                            {
                                                tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Especial;
                                                valorComissao = valorPago * (item.PercentualEspecial / 100);
                                                percentualComissao = item.PercentualEspecial;
                                                break;
                                            }
                                            else if (tipoContrato == 6) //idade
                                            {
                                                tipoComissionamento = TipoContrato.TipoComissionamentoProdutorOuOperadora.Idade;
                                                valorComissao = valorPago * (item.Idade / 100);
                                                percentualComissao = item.Idade;
                                                break;
                                            }
                                        }
                                    }
                                }
                                #endregion

                                #region Vitaliciedade COMO EXIBIR NO RELATORIO ESTE PERCENTUAL DE COMISSAO ???

                                itemVitaliciedade = ComissionamentoVitaliciedade.Carregar(grupoId, tipoComissionamento, pm); //ComissionamentoVitaliciedade.Carregar(comissionamentoModelo.GrupoID, tipoComissionamento, pm);
                                if (itemVitaliciedade != null && itemVitaliciedade.ParcelaInicio >= parcela)
                                {
                                    valorComissao += (valorPago * (itemVitaliciedade.Percentual / 100));
                                }
                                #endregion

                                atualizaApelidoSuperior(ref resultado, superiores[0].SubordinadoID, superiores[0].SuperiorApelido);

                                resultadoLinha["ProdutorNome"]               = superiores[0].SuperiorNome;
                                resultadoLinha["ProdutorValor"]              = valorComissao.ToString("N2");
                                resultadoLinha["ProdutorPercentualComissao"] = percentualComissao.ToString("N2");

                                //se nao foi informado um produtor especifico ou 
                                //foi informado e é o mesmo que o produtor corrente
                                if (produtorId == null ||
                                    Convert.ToString(produtorId) == Convert.ToString(superiores[0].SuperiorID))
                                {
                                    //checa também se o perfil está incluso na relacao de perfis enviada por parametro
                                    if (perfilIDs == null || contemValor(perfilIDs, superiores[0].SuperiorPerfilID))
                                    {
                                        resultadoLinha["FecharComissao"] = "1";

                                        #region itens abertos na conta corrente

                                        creditos = 0;
                                        itensCC = (List<MovimentacaoContaCorrente>)MovimentacaoContaCorrente.CarregarEmAberto(superiores[0].SuperiorID, dataCorte.Value, pm);
                                        if (itensCC == null) { itensCC = new List<MovimentacaoContaCorrente>(); }

                                        //só salva o credito se ele ainda nao foi feito.
                                        if (valorComissao > 0 && !MovimentacaoContaCorrente.CreditoJaFeitoPara(superiores[0].SuperiorID, row["cobranca_id"], pm))
                                        {
                                            itemCC = new MovimentacaoContaCorrente();
                                            itemCC.CategoriaID = CategoriaContaCorrente.SysPremiacaoCategoriaID;
                                            itemCC.CobrancaID = row["cobranca_id"];
                                            itemCC.Data = DateTime.Now;
                                            itemCC.LisagemFechamentoID = listagem.ID;
                                            itemCC.ProdutorID = superiores[0].SuperiorID;
                                            itemCC.Valor = valorComissao;
                                            pm.Save(itemCC);
                                        }

                                        foreach (MovimentacaoContaCorrente _item in itensCC)
                                        {
                                            if (CategoriaContaCorrente.eTipo.Credito == (CategoriaContaCorrente.eTipo)_item.CategoriaTipo)
                                                creditos += _item.Valor;
                                            else
                                                creditos -= _item.Valor;
                                            _item.LisagemFechamentoID = listagem.ID;
                                            pm.Save(_item);
                                        }
                                        #endregion
                                    }
                                }

                                resultadoLinha["ProdutorCredito"] = creditos.ToString("N2");
                                resultado.Rows.Add(resultadoLinha);
                                adicionaGrafoDeProdutoresParaCobranca(ref vos, row["cobranca_id"], superiores[0].SuperiorID, true);
                            }

                            superiores = SuperiorSubordinado.CarregarSuperiores(superiores[0].SuperiorID, admissao, pm);
                        }
                    }
                }

                //salva a relacao de pagamento de comissao
                salvarRelacao(resultado, listagem.ID, pm);

                //baixa as cobrancas que devem ser baixadas
                //baixarCobrancas(vos, pm); 

                pm.Commit();
            }
            catch(Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                dt.Dispose();
                pm = null;
            }

            return resultado;
        }

        void salvarGrafoEquipe(SuperiorSubordinado relacao, Object listagemId, PersistenceManager pm)
        {
            String cmd = String.Concat("if(NOT EXISTS(SELECT NULL FROM listagem_relacao_grafo WHERE listagemrelacaografo_listagemId=", listagemId, " AND listagemrelacaografo_superiorId=", relacao.SuperiorID, " AND listagemrelacaografo_imediatoId=", relacao.SubordinadoID, " AND listagemrelacaografo_superiorPerfilId=", relacao.SuperiorPerfilID, "AND listagemrelacaografo_imediatoPerfilId=", relacao.SubordinadoPerfilID, ")) ",
                " BEGIN ",
                "   INSERT INTO listagem_relacao_grafo (listagemrelacaografo_listagemId,listagemrelacaografo_superiorId,listagemrelacaografo_superiorPerfilId,listagemrelacaografo_imediatoId,listagemrelacaografo_imediatoPerfilId) VALUES (", listagemId, ", ", relacao.SuperiorID, ", ", relacao.SuperiorPerfilID, ", ", relacao.SubordinadoID, ", ", relacao.SubordinadoPerfilID, ")",
                " END ");

            NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm);
        }

        void salvarGrafoEquipeCorretor(Object corretorId, Object corretorPerfilId, Object listagemId, PersistenceManager pm)
        {
            String cmd = String.Concat("if(NOT EXISTS(SELECT NULL FROM listagem_relacao_grafo WHERE listagemrelacaografo_listagemId=", listagemId, " AND listagemrelacaografo_superiorId=", corretorId, " AND listagemrelacaografo_imediatoId IS NULL AND listagemrelacaografo_superiorPerfilId=", corretorPerfilId, " AND listagemrelacaografo_imediatoPerfilId IS NULL)) ",
                " BEGIN ",
                "   INSERT INTO listagem_relacao_grafo (listagemrelacaografo_listagemId,listagemrelacaografo_superiorId,listagemrelacaografo_superiorPerfilId,listagemrelacaografo_imediatoId,listagemrelacaografo_imediatoPerfilId) VALUES (", listagemId, ", ", corretorId, ",", corretorPerfilId, ", NULL, NULL)",
                " END ");

            NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm);
        }

        /// <summary>
        /// Otimizar rotina.
        /// </summary>
        void salvarRelacao(DataTable dt, Object listagemId, PersistenceManager pm)
        {
            StringBuilder sb = new StringBuilder();
            String[] pValues;
            String[] pNames = new String[] { "@OperadoraNome", "@CobrancaValorPago", "@ProdutorNome", "@ProdutorValor", "@ProdutorApelido", "@SuperiorApelido", "@NomeTitular", "@ProdutorBanco", "@ProdutorAgencia", "@ProdutorConta" };
            foreach (DataRow row in dt.Rows)
            {
                if (cToString(row["ProdutorPercentualComissao"]) == "0.00" || cToString(row["ProdutorPercentualComissao"]) == "0,00")
                {
                    continue;
                }
                pValues = new String[] { cToString(row["OperadoraNome"]), cToString(row["CobrancaValorPago"]),
                    cToString(row["ProdutorNome"]), cToString(row["ProdutorValor"]), cToString(row["ProdutorApelido"]), 
                    cToString(row["SuperiorApelido"]), cToString(row["NomeTitular"]), cToString(row["ProdutorBanco"]), cToString(row["ProdutorAgencia"]), cToString(row["ProdutorConta"]) };

                sb.Append("INSERT INTO listagem_relacao (listagemrelacao_listagemId, listagemrelacao_contratoNumero,listagemrelacao_contratoId,listagemrelacao_operadoraId,listagemrelacao_operadoraNome,listagemrelacao_cobrancaId,listagemrelacao_cobrancaValorPago,listagemrelacao_cobrancaDataPago,listagemrelacao_percentualComissao,listagemrelacao_cobrancaParcela,listagemrelacao_produtorId,listagemrelacao_produtorNome,listagemrelacao_produtorValor,listagemrelacao_produtorCredito,listagemrelacao_produtorPerfilId,listagemrelacao_produtorApelido,listagemrelacao_superiorApelido,listagemrelacao_contratoNomeTitular,listagemrelacao_produtorBanco,listagemrelacao_produtorAgencia,listagemrelacao_produtorConta, listagemrelacao_cobrancaDataVencto,listagemrelacao_contratoVigencia,listagemrelacao_fechado,listagemrelacao_contratoAdmissao) VALUES (");
                sb.Append(listagemId); sb.Append(",'");
                sb.Append(row["ContratoNumero"]); sb.Append("',");
                sb.Append(row["ContratoID"]); sb.Append(",");
                sb.Append(row["OperadoraID"]); sb.Append(",");
                sb.Append("@OperadoraNome"); sb.Append(",");
                sb.Append(row["CobrancaID"]); sb.Append(",");
                sb.Append("@CobrancaValorPago"); sb.Append(",'");
                sb.Append(row["CobrancaDataPago"]); sb.Append("','");
                sb.Append(row["ProdutorPercentualComissao"]); sb.Append("',");
                sb.Append(row["CobrancaParcela"]); sb.Append(",");
                sb.Append(row["ProdutorID"]); sb.Append(",");
                sb.Append("@ProdutorNome"); sb.Append(",");
                sb.Append("@ProdutorValor"); sb.Append(",'");
                sb.Append(row["ProdutorCredito"]); sb.Append("',");
                sb.Append(row["ProdutorPerfilID"]); sb.Append(",");
                sb.Append("@ProdutorApelido"); sb.Append(",");
                sb.Append("@SuperiorApelido"); sb.Append(",");
                sb.Append("@NomeTitular"); sb.Append(",");

                sb.Append("@ProdutorBanco"); sb.Append(",");
                sb.Append("@ProdutorAgencia"); sb.Append(",");
                sb.Append("@ProdutorConta"); sb.Append(",'");

                sb.Append(row["CobrancaVencimento"]); sb.Append("','");
                sb.Append(row["ContratoVigencia"]); sb.Append("',");
                sb.Append(row["FecharComissao"]); sb.Append(",'");
                sb.Append(row["ContratoAdmissao"]); sb.Append("'); ");

                NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pNames, pValues, pm);
                sb.Remove(0, sb.Length);
            }
        }

        void baixarCobrancas(List<CobrancaCorretorChefiaVO> vos, PersistenceManager pm)
        {
            if (vos == null || vos.Count == 0) { return; }
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (CobrancaCorretorChefiaVO vo in vos)
            {
                if (sb.Length > 0) { sb.Append(" ; "); }
                i = 0;

                sb.Append("IF (");

                foreach (Object id in vo.ProdutorIDs)
                {
                    if (i > 0) { sb.Append(" AND "); }
                    sb.Append("EXISTS(SELECT listagemrelacao_cobrancaId FROM listagem_relacao WHERE listagemrelacao_fechado=1 AND listagemrelacao_cobrancaId="); sb.Append(vo.CobrancaID); sb.Append(" AND listagemrelacao_produtorId="); sb.Append(id); sb.Append(")");
                    i++;
                }

                sb.Append(") BEGIN UPDATE cobranca SET cobranca_comissaoPaga=1 WHERE cobranca_id=");
                sb.Append(vo.CobrancaID); sb.Append(" END ");
            }

            NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
        }

        void atualizaApelidoSuperior(ref DataTable dt, Object produtorId, String apelidoSuperior)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToString(row["ProdutorID"]) == Convert.ToString(produtorId))
                {
                    row["SuperiorApelido"] = apelidoSuperior;
                    break;
                }
            }
        }

        String cToString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return "";
            else
                return Convert.ToString(param);
        }

        /// <summary>
        /// Checa se um valor existe no array.
        /// </summary>
        Boolean contemValor(String[] array, Object valor)
        {
            if (array == null) { return false; }

            String _valor = Convert.ToString(valor);
            foreach (String item in array)
            {
                if (item == _valor) { return true; }
            }

            return false;
        }
    }
}