namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.Linq;
    using System.Data;
    using System.Text;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity;

    using NHibernate;
    using NHibernate.Linq;
    using MedProj.Entidades.Enuns;
    using Ent = MedProj.Entidades;
    using System.Data.OleDb;

    public sealed class ContratoFacade : FacadeBase
    {
        #region Singleton 

        static ContratoFacade _instance;
        public static ContratoFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new ContratoFacade(); }
                return _instance;
            }
        }
        #endregion

        private ContratoFacade() { }

        /// <summary>
        /// 
        /// </summary>
        public bool Salvar(Contrato contrato, ContratoBeneficiario titular, IList<ContratoBeneficiario> dependentes, Object[] fichas, Object usuarioLiberadorID, IList<AdicionalBeneficiario> adicionalBeneficiario, Conferencia conferencia, Decimal valorTotal, out string msg)
        {
            Decimal valorTotalContrato = 0; msg = "";
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                Boolean novoContrato = contrato.ID == null;

                IList<NumeroCartao> numero = null;

                if (string.IsNullOrEmpty(contrato.Senha))
                {
                    contrato.GerarSenha();
                }

                if (novoContrato)
                {
                    Ent.AssociadoPJ pj = AssociadoPJFacade.Instance.Carregar(Convert.ToInt64(contrato.EstipulanteID));
                    if (pj.TipoDataValidade == TipoDataValidade.Indefinido)
                    {
                        pm.Rollback();
                        msg = "Nenhuma configuração de validade do cartão encontrada.";
                        return false;
                    }
                    else
                    {
                        if (pj.TipoDataValidade == TipoDataValidade.DataFixa)
                        {
                            contrato.DataValidade = pj.DataValidadeFixa.Value;
                        }
                        else
                        {
                            contrato.DataValidade = contrato.Vigencia.AddMonths(pj.MesesAPartirDaVigencia.Value);
                        }
                    }

                    //calcula o codigo de cobranca para o contrato.
                    String qry = ""; // "SELECT MAX(contrato_codcobranca) FROM contrato";
                    contrato.CodCobranca = 0; //;Convert.ToInt32(LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm)) + 1;

                    //validacoes do numero do contrato
                    if (string.IsNullOrEmpty(contrato.Numero))
                    {
                        #region logica nao utilizada - comentado 
                        ////não informou um numero, entao pega um disponivel
                        //numero = LocatorHelper.Instance.ExecuteQuery<NumeroCartao>("select top 1 * from numero_contrato where numerocontrato_ativo=1 and numerocontrato_contratoId is null ", typeof(NumeroCartao), pm);

                        //if (numero == null || numero.Count == 0)
                        //{
                        //    msg = "Número de cartão indisponível.";
                        //    pm.Rollback();
                        //    return false;
                        //}
                        //else
                        //{
                        //    contrato.NumeroID = numero[0].ID;
                        //    //será salvo mais abaixo
                        //}
                        #endregion

                        qry = "SELECT MAX(numerocontrato_numero) FROM numero_contrato";
                        object aux = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
                        //string str = "";

                        numero = new List<NumeroCartao>();
                        numero.Add(new NumeroCartao());

                        if (aux != null && aux != DBNull.Value)
                        {
                            numero[0].Numero = (Convert.ToInt64(aux) + 1).ToString();
                            numero[0].GerarDigitoVerificador();
                        }
                        else
                        {
                            numero[0].GerarNumeroInicial();
                        }

                        #region antigo - comentado 
                        //qry = "SELECT MAX(numerocontrato_id) FROM numero_contrato";
                        //object aux = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
                        //string str = "";
                        
                        //numero = new List<NumeroCartao>();
                        //numero.Add(new NumeroCartao());

                        //if (aux != null && aux != DBNull.Value)
                        //{
                        //    Int64 ret = Convert.ToInt64(aux);
                        //    qry = "SELECT numerocontrato_numero FROM numero_contrato where numerocontrato_id=" + ret.ToString();
                        //    str = Convert.ToString(LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm));
                        //    numero[0].Numero = (Convert.ToInt64(str) + 1).ToString();
                        //    numero[0].GerarDigitoVerificador();
                        //}
                        //else
                        //{
                        //    numero[0].GerarNumeroInicial();
                        //}
                        #endregion

                        contrato.Numero = numero[0].NumeroCompletoSemCV;

                        numero[0].Ativo = true;
                        numero[0].Data = DateTime.Now;

                        pm.Save(numero[0]);

                        contrato.NumeroID = Convert.ToInt64(numero[0].ID);
                    }
                    else
                    {
                        numero = LocatorHelper.Instance.ExecuteQuery<NumeroCartao>("select top 1 * from numero_contrato where numerocontrato_ativo=1 and numerocontrato_contratoId is null and numerocontrato_numero='" + contrato.Numero.Substring(0,13) + "'", typeof(NumeroCartao), pm);

                        if (numero == null || numero.Count == 0)
                        {
                            msg = "Número de cartão indisponível.";
                            pm.Rollback();
                            return false;
                        }
                        else
                        {
                            contrato.NumeroID = numero[0].ID;
                            //será salvo mais abaixo
                        }
                    }
                }
                else
                {
                    #region editando ... 

                    if (contrato.DataValidade == DateTime.MinValue)
                    {
                        msg = "Data de validade não informada.";
                        pm.Rollback();
                        return false;
                    }

                    string numeroSalvo = Convert.ToString(LocatorHelper.Instance.ExecuteScalar("select contrato_numero from contrato where contrato_id=" + contrato.ID, null, null, pm));
                    string numeroSalvoId = Convert.ToString(LocatorHelper.Instance.ExecuteScalar("select contrato_numeroId from contrato where contrato_id=" + contrato.ID, null, null, pm));

                    if ((numeroSalvoId != Convert.ToString(contrato.NumeroID)) || (contrato.Numero != numeroSalvo))
                    {
                        numero = LocatorHelper.Instance.ExecuteQuery<NumeroCartao>("select top 1 * from numero_contrato where numerocontrato_ativo=1 and numerocontrato_contratoId is null and numerocontrato_numero='" + contrato.Numero.Substring(0,13) + "'", typeof(NumeroCartao), pm);

                        if (numero == null || numero.Count == 0)
                        {
                            pm.Rollback();
                            msg = "Número de cartão atribuído indisponível.";
                            return false;
                        }

                        NumeroCartao original = new NumeroCartao();
                        original.ID = numeroSalvo;
                        pm.Load(original);
                        original.Ativo = false;
                        pm.Save(original);

                        //salva o novo
                        numero[0].ContratoId = contrato.ID;
                        numero[0].Data = DateTime.Now;
                        pm.Save(numero[0]);

                        contrato.ID = numero[0].ID;
                    }
                    #endregion
                }

                //Salva o contrato.
                pm.Save(contrato);

                if (novoContrato && numero != null)
                {
                    numero[0].ContratoId = contrato.ID;
                    numero[0].Data = DateTime.Now;
                    pm.Save(numero[0]);
                }

                #region gera primeria cobranca 
                //if (novoContrato)
                //{
                //    //gera a primeira cobranca ja paga
                //    Cobranca cobranca = new Cobranca();
                //    cobranca.Cancelada = false;
                //    cobranca.ComissaoPaga = true;
                //    cobranca.ContratoCodCobranca = Convert.ToString(contrato.CodCobranca);
                //    cobranca.DataCriacao = DateTime.Now;
                //    cobranca.DataPgto = contrato.Admissao;
                //    cobranca.DataVencimento = contrato.Admissao;
                //    cobranca.Pago = true;
                //    cobranca.Parcela = 1;
                //    cobranca.PropostaID = contrato.ID;
                //    cobranca.Tipo = (int)Cobranca.eTipo.Normal;
                //    cobranca.Valor = valorTotal;
                //    cobranca.ValorPgto = cobranca.Valor;
                //    pm.Save(cobranca);

                //    List<CobrancaComposite> composite = new List<CobrancaComposite>();
                //    Contrato.CalculaValorDaProposta(cobranca.PropostaID, cobranca.DataVencimento, pm, false, true, ref composite);
                //    CobrancaComposite.Salvar(cobranca.ID, composite, pm);
                //    composite = null;
                //}
                #endregion

                //if (usuarioLiberadorID != null)
                //    Contrato.SetaUsuarioLiberador(contrato.ID, usuarioLiberadorID, pm);

                //Salva o titular
                titular.ContratoID = contrato.ID;
                titular.NumeroSequencial = 0;
                titular.Vigencia = contrato.Vigencia;

                if (titular.BeneficiarioID == null)
                {
                    titular.BeneficiarioID = ContratoBeneficiario.CarregaTitularID(contrato.ID, pm);
                }
                if (titular.ID == null)
                {
                    titular.ID = ContratoBeneficiario.CarregaID_ParaTitular(contrato.ID, pm);
                }
                //if (titular.ID != null) { pm.Load(titular); } nao pode carregar, pois sobrescreve dados preenchidos na tela
                else 
                {
                    titular.Tipo = Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular);
                }

                //if (titular.ID == null) { titular.Status = (Int32)ContratoBeneficiario.eStatus.Novo; }
                /*else*/if (titular.Status == Convert.ToInt32(ContratoBeneficiario.eStatus.Incluido)) { valorTotalContrato += titular.Valor; }

                if (novoContrato) { titular.Status = (Int32)ContratoBeneficiario.eStatus.Novo; titular.Data = contrato.Admissao; titular.Vigencia = contrato.Vigencia; }
                pm.Save(titular);

                #region Salva os dependentes 

                if (dependentes != null)
                {
                    CalendarioVencimento rcv = null;
                    DateTime vigencia = DateTime.MinValue, vencimento = DateTime.MinValue; Int32 diasDataSemJuros = 0; Object valorDataLimite = null;
                    foreach (ContratoBeneficiario dependente in dependentes)
                    {
                        if (dependente.NumeroSequencial < 0)
                        {
                            dependente.NumeroSequencial = ContratoBeneficiario.ProximoNumeroSequencial(contrato.ID, dependente.BeneficiarioID, pm);
                        }

                        dependente.ContratoID = contrato.ID;

                        if (dependente.ID == null)
                        {
                            dependente.Status = (Int32)ContratoBeneficiario.eStatus.Novo;

                            if (novoContrato)
                                dependente.Vigencia = contrato.Vigencia;
                            else
                            {
                                CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(contrato.ContratoADMID,
                                    dependente.Data, out vigencia, out vencimento, out diasDataSemJuros, out valorDataLimite, out rcv, pm);
                                dependente.Vigencia = vigencia;
                            }
                        }
                        else if (dependente.Status == (Int32)ContratoBeneficiario.eStatus.Incluido)
                        {
                            valorTotalContrato += dependente.Valor;
                        }

                        pm.Save(dependente);
                    }
                }
                #endregion

                #region comentado 

                //Salva as fichas de saúde dos beneficiários
                //Boolean aprovadoPeloDepto = true;
                //if (fichas != null && fichas.Length > 0) //&& Convert.ToInt32(contrato.ID) != 152685)
                //{
                //    foreach (IList<ItemDeclaracaoSaudeINSTANCIA> lista in fichas)
                //    {
                //        if (lista == null) { continue; }
                //        foreach (ItemDeclaracaoSaudeINSTANCIA item in lista)
                //        {
                //            //Se id==null, tenta achar pelo id do beneficiario e do item de ficha de saude
                //            if (item.ID == null)
                //            {
                //                item.ID = ItemDeclaracaoSaudeINSTANCIA.CarregarID(
                //                    item.BeneficiarioID, item.ItemDeclaracaoID, pm);
                //            }

                //            if (item.ID != null)
                //                pm.Save(item);
                //            else if (item.Sim)
                //                pm.Save(item);

                //            //se tem positivacao sem aprovacao do técnico, a proposta fica pendente
                //            if (item.Sim && !item.AprovadoPeloDeptoTecnico && aprovadoPeloDepto == true) { aprovadoPeloDepto = false; }
                //        }
                //    }
                //}

                //if (novoContrato && !aprovadoPeloDepto) //se o contrato é novo e há positivacoes nas fichas de saude SEM a aprovacao do Depto. Tecnico:
                //{
                //    contrato.Pendente = true;
                //    pm.Save(contrato);
                //}

                ////salva os produtos adicionais contratados
                //if (adicionalBeneficiario != null)
                //{
                //    foreach (AdicionalBeneficiario ad in adicionalBeneficiario)
                //    {
                //        ad.PropostaID = contrato.ID;
                //        pm.Save(ad);
                //    }
                //}

                //////checa historico de planos e atualiza a última entrada se necessário.
                ////ContratoPlano obj = ContratoPlano.CarregarAtual(contrato.ID, pm);
                ////if (obj != null)
                ////{
                ////    if (Convert.ToString(obj.PlanoID) != Convert.ToString(contrato.PlanoID))
                ////    {
                ////        obj.PlanoID = contrato.PlanoID;
                ////        pm.Save(obj);
                ////    }

                ////    obj = null;
                ////}

                ////checa almoxarifado - contrato impresso
                //String letra = "";
                //if (PrimeiraPosicaoELetra(contrato.Numero))
                //    letra = contrato.Numero.Substring(0, 1);

                //AlmoxContratoImpresso aci = null;
                ////if (!String.IsNullOrEmpty(letra))
                ////    aci = AlmoxContratoImpresso.Carregar(contrato.OperadoraID, contrato.Numero.Replace(letra, ""), letra, -1, pm);
                ////else
                ////    aci = AlmoxContratoImpresso.Carregar(contrato.OperadoraID, contrato.Numero, letra, -1, pm);

                //if (aci != null && aci.AgenteID == null)
                //{
                //    aci.AgenteID = contrato.DonoID;
                //    aci.Data = contrato.Data;
                //    pm.Save(aci);
                //}
                //else if (aci == null)
                //{
                //    #region IMPOSSIVEL
                //    //aci = new AlmoxContratoImpresso();
                //    //aci.AgenteID = contrato.DonoID;
                //    //aci.Data = contrato.Data;
                //    //aci.MovID = null; //?????
                //    //aci.Numero = contrato.Numero;
                //    //aci.OperadoraID = contrato.OperadoraID;
                //    //aci.ProdutoID = null; //?????
                //    ////pm.Save(aci);
                //    #endregion
                //}

                ////Altera status da proposta em conferencia / cadastro
                //if (conferencia != null)
                //{
                //    conferencia.Carregar();
                //    conferencia.Departamento = 6; //TODO: corrigir (Int32)ContratoStatusHistorico.eStatus.Cadastrado;
                //    pm.Save(conferencia);

                //    ContratoStatusHistorico.Salvar(contrato.Numero, contrato.OperadoraID, ContratoStatusHistorico.eStatus.Cadastrado, pm);
                //}
                //else if (novoContrato)
                //{
                //    ContratoStatusHistorico.Salvar(contrato.Numero, contrato.OperadoraID, ContratoStatusHistorico.eStatus.Cadastrado, pm);
                //}

                ////Checa se é necessário gravar o valor total do contrato
                //if (valorTotalContrato > 0)
                //{
                //    ContratoValor.InsereNovoValorSeNecessario(contrato.ID, valorTotalContrato, pm);
                //}
                #endregion

                pm.Commit();

                return true;
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

        public bool ValidaCartao(object contratoId, string numeroCartao, out string  msg)
        {
            msg = "";
            string qry = "select top 1 * from numero_contrato where (convert(varchar,numerocontrato_numero)+convert(varchar,numerocontrato_via)+convert(varchar,numerocontrato_dv))='" + numeroCartao + "'";
            IList<NumeroCartao> numero = LocatorHelper.Instance.ExecuteQuery<NumeroCartao>(qry, typeof(NumeroCartao));

            if (numero == null || numero.Count == 0)
            {
                msg = "Número não encontrado.";
                return false;
            }

            if (contratoId != null)
            {
                if (numero[0].ContratoId != null)
                {
                    if (Convert.ToString(contratoId) == Convert.ToString(numero[0].ContratoId))
                        return true;
                    else if (Convert.ToString(contratoId) != Convert.ToString(numero[0].ContratoId))
                    {
                        msg = "Número ja em uso.";
                        return false;
                    }
                }
                else
                {
                    if (!numero[0].Ativo)
                    {
                        msg = "Número inativado.";
                        return false;
                    }
                }
            }
            else
            {
                if (numero[0].ContratoId != null)
                {
                    msg = "Número ja em uso.";
                    return false;
                }

                if (!numero[0].Ativo)
                {
                    msg = "Número inativado.";
                    return false;
                }
            }

            return true;
        }

        public Ent.NumeroCartao GerarNovaViaCartao(object contratoId)
        {
            using (var sessao = ObterSessao())
            {
                using (var tran = sessao.BeginTransaction())
                {
                    Ent.Contrato contrato   = sessao.Get<Ent.Contrato>(Convert.ToInt64(contratoId));
                    Ent.NumeroCartao numero = sessao.Get<Ent.NumeroCartao>(contrato.NumeroID);

                    numero.Ativo = false;
                    sessao.Update(numero);

                    IDbCommand cmd = sessao.Connection.CreateCommand();
                    tran.Enlist(cmd);

                    cmd.CommandText = "SELECT MAX(numerocontrato_numero) FROM numero_contrato";
                    string strNumero = Convert.ToString(cmd.ExecuteScalar());

                    //cmd.CommandText = "SELECT MAX(numerocontrato_id) FROM numero_contrato";
                    //long numeroId = Convert.ToInt64(cmd.ExecuteScalar());

                    //cmd.CommandText = "select numerocontrato_numero FROM numero_contrato where numerocontrato_id=" + numeroId.ToString();
                    //string strNumero = Convert.ToString(cmd.ExecuteScalar());

                    Ent.NumeroCartao novo = new Ent.NumeroCartao();
                    novo.Numero = (Convert.ToInt64(strNumero) + 1).ToString();
                    novo.Via    = numero.Via + 1;
                    novo.GerarDigitoVerificador();
                    novo.Contrato = contrato;

                    sessao.Save(novo);

                    contrato.Numero = novo.NumeroCompletoSemCV;
                    contrato.NumeroID = novo.ID;
                    contrato.CartaoSolicitado = false;

                    if (contrato.DataVigencia == DateTime.MinValue) contrato.DataVigencia = contrato.DataAdmissao;

                    sessao.Update(contrato);

                    tran.Commit();

                    return novo;
                }
            }
        }

        public Contrato Carregar(string numeroCartao)
        {
            string qry = "select top 1 * from contrato where contrato_numero=@contrato_numero";
            IList<Contrato> contrato = LocatorHelper.Instance.ExecuteParametrizedQuery<Contrato>(qry, new string[] { "@contrato_numero" }, new string[] { numeroCartao }, typeof(Contrato));

            if (contrato == null || contrato.Count == 0)
            {
                return null;
            }
            else
            {
                return contrato[0];
            }
        }

        //As cobrancas não sao geradas em lotes mais
        //public void GerarOuAtualizarCobrancas(List<String> contratoIDs)
        //{
        //    String inClausule = String.Join(",", contratoIDs.ToArray());
        //    PersistenceManager pm = new PersistenceManager();
        //    pm.BeginTransactionContext();

        //    try
        //    {
        //        String qry = String.Concat("SELECT contrato_id as ContratoID, contrato_contratoAdmId as ContratoAdmID, contrato_vencimento as ContratoVencimento, SUM(contratobeneficiario_valor) AS Total ", 
        //        "   FROM contrato ",
        //        "       INNER JOIN contrato_beneficiario ON contratobeneficiario_contratoId = contrato_id ",
        //        "   WHERE contratobeneficiario_status=2 AND contrato_id IN (", inClausule, ") ",
        //        "   GROUP BY contrato_id, contrato_contratoAdmId, contrato_vencimento HAVING SUM(contratobeneficiario_valor) > 0");

        //        DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset", pm).Tables[0];

        //        DateTime vigenciaBeneficiario, vencimentoBeneficiario;
        //        Int32 diaDataSemJuros; Object valorDataLimite;

        //        if (dt.Rows.Count > 0)
        //        {
        //            IList<Cobranca> cobrancasExistentes = null;
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                cobrancasExistentes = Cobranca.CarregarTodas(row["ContratoID"], pm);

        //                if (cobrancasExistentes == null) //nao tem cobranças geradas. deve-se somente gerá-las
        //                {
        //                    Cobranca.Gerar(row["ContratoID"],
        //                        Convert.ToDateTime(row["ContratoVencimento"], new System.Globalization.CultureInfo("pt-Br")), 12, pm);

        //                    ContratoValor.InsereNovoValorSeNecessario(row["ContratoID"], Convert.ToDecimal(row["Total"]), pm);
        //                }
        //                else //Tem cobranças geradas. Precisa ver qual delas deverá ser atualizada.
        //                {
        //                    //Recalcula o valor total do contrato somando todos os beneficiarios ativos e com vencimento compativel às cobrancas
        //                    IList<ContratoBeneficiario> beneficiarios = ContratoBeneficiario.CarregarPorContratoID(row["ContratoID"], true, false, pm);
        //                    Contrato contrato = new Contrato(row["ContratoID"]);
        //                    pm.Load(contrato);

        //                    Decimal novoTotalContrato;

        //                    Int32 i = 0;
        //                    foreach (Cobranca cobrancaExistente in cobrancasExistentes)
        //                    {
        //                        novoTotalContrato = 0;
        //                        Int32 qtdBeneficiarioParaEssaCobranca = 0;
        //                        foreach (ContratoBeneficiario beneficiario in beneficiarios)
        //                        {
        //                            CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(
        //                                row["ContratoAdmID"], beneficiario.Data, out vigenciaBeneficiario, out vencimentoBeneficiario, out diaDataSemJuros, out valorDataLimite, pm);

        //                            //se o beneficiario ja entra na cobranca, incrementa seu valor
        //                            if (cobrancaExistente.DataVencimento >= vencimentoBeneficiario)
        //                            {
        //                                qtdBeneficiarioParaEssaCobranca++;
        //                                novoTotalContrato += Contrato.CalculaValorDaPropostaSemTaxaAssociativa(beneficiario.ContratoID, beneficiario, cobrancaExistente.DataVencimento, pm); //beneficiario.Valor;
        //                            }
        //                        }

        //                        novoTotalContrato += Contrato.CalculaValorDaTaxaAssociativa(contrato, qtdBeneficiarioParaEssaCobranca, pm);
        //                        cobrancaExistente.Valor = novoTotalContrato;

        //                        i++;
        //                        pm.Save(cobrancaExistente);

        //                        if (i == cobrancasExistentes.Count) //atualiza o novo valor do contrato
        //                        {
        //                            ContratoValor.InsereNovoValorSeNecessario(row["ContratoID"], novoTotalContrato, pm);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        pm.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        pm.Rollback();
        //        throw ex;
        //    }
        //    finally
        //    {
        //        pm = null;
        //    }
        //}

        /// <summary>
        /// Atribui o status de Adimplente ou Inadimplente (proprieade Adimplente do objeto Contrato) 
        /// segundo cobranças em aberto de cada contrato.
        /// </summary>
        /// <param name="pm">Objeto PersistenceManager participante de uma transação.</param>
        public void AtribuiStatusAdimplenteOuInadimplente(PersistenceManager pm)
        {
            PersistenceManager _pm = null;

            IList<Contrato> inadimplentes = Contrato.BuscarECarregarInadimplentes_PORCOBRANCA(pm);

            if (pm == null)
            {
                _pm = new PersistenceManager();
                _pm.BeginTransactionContext();
            }
            else
                _pm = pm;

            try
            {
                if (inadimplentes != null && inadimplentes.Count > 0)
                {
                    String[] ids = new String[inadimplentes.Count];
                    for (Int32 i = 0; i < inadimplentes.Count; i++)
                    {
                        ids[i] = Convert.ToString(inadimplentes[i].ID);
                    }

                    String inClausule = String.Join(",", ids);
                    String command = "UPDATE contrato SET contrato_adimplente=0 WHERE contrato_adimplente=1 AND contrato_id IN (" + inClausule + ")";
                    NonQueryHelper.Instance.ExecuteNonQuery(command, _pm);

                    //restaura contratos ok para ADIMPLENTE
                    command = "UPDATE contrato SET contrato_adimplente=1 WHERE contrato_adimplente=0 AND contrato_cancelado=0 AND contrato_rascunho=0 AND contrato_inativo <> 1 AND contrato_id NOT IN (" + inClausule + ")";
                    NonQueryHelper.Instance.ExecuteNonQuery(command, _pm);
                }
                else
                {
                    //todos os contratos estão ADIMPLENTES
                    String command = "UPDATE contrato SET contrato_adimplente=1 WHERE contrato_adimplente=0 AND contrato_cancelado=0 AND contrato_rascunho=0 AND contrato_inativo <> 1 ";
                    NonQueryHelper.Instance.ExecuteNonQuery(command, _pm);
                }

                if (pm == null) { _pm.Commit(); }
            }
            catch
            {
                if (pm == null) { _pm.Rollback(); }
                throw;
            }
            finally
            {
                if (pm == null) { _pm = null; }
            }
        }

        static Boolean PrimeiraPosicaoELetra(String param)
        {
            if (String.IsNullOrEmpty(param)) { return false; }

            String pos1 = param.Substring(0, 1);

            if (pos1 != "0" && pos1 != "1" && pos1 != "2" && pos1 != "3" && pos1 != "4" &&
                pos1 != "5" && pos1 != "6" && pos1 != "7" && pos1 != "8" && pos1 != "9")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetaDataAlteracaoContratos(Object beneficiarioId)
        {
            IList<Contrato> contratos = Contrato.CarregarPorBeneficiário(beneficiarioId);
            if (contratos == null || contratos.Count == 0) { return; }

            StringBuilder sb = new StringBuilder();
            DateTime data = DateTime.Now;

            foreach (Contrato contrato in contratos)
            {
                if (sb.Length > 0) { sb.Append(";"); }

                sb.Append("UPDATE contrato SET contrato_alteracao='");
                sb.Append(data.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.Append("' WHERE contrato_id="); sb.Append(contrato.ID);
            }

            NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), null);
        }

        public System.Collections.Hashtable InativaContratos(String[] numerosDasPropostas, Object operadoraId, DateTime dataInativacao, DateTime? faltaDePagamentoEm, Object statusId, String statusObs, Object usuarioId)
        {
            if (operadoraId == null || numerosDasPropostas == null || numerosDasPropostas.Length == 0) { return null; }

            Object oret = null;
            Int32 iret = 0;
            String obs = "";

            if(faltaDePagamentoEm != null)
                obs = String.Concat("Inativado em ", dataInativacao.ToString("dd/MM/yyyy"), " por falta de pagamento em ", faltaDePagamentoEm.Value.ToString("dd/MM/yyyy"), ".");

            StringBuilder sb = new StringBuilder();

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            System.Collections.Hashtable ht = new System.Collections.Hashtable();

            try
            {
                ContratoStatusInstancia csi = null;
                foreach (String numero in numerosDasPropostas)
                {
                    if (numero == null || numero.Trim() == "") { continue; }

                    //Pega as observacoes do contrato
                    if (!String.IsNullOrEmpty(obs))
                    {
                        oret = LocatorHelper.Instance.ExecuteScalar(String.Concat("SELECT contrato_obs FROM contrato WHERE contrato_numero='", numero.Trim(), "' AND contrato_operadoraId=", operadoraId), null, null, pm);
                        if (oret == null || oret == DBNull.Value)
                            oret = obs;
                        else
                            oret = String.Concat(Convert.ToString(oret), Environment.NewLine, obs);
                    }
                    else
                        oret = null;

                    sb.Append("UPDATE contrato SET contrato_inativo=1, contrato_dataCancelamento='");
                    sb.Append(dataInativacao.ToString("yyyy-MM-dd"));

                    if (oret != null)
                    {
                        sb.Append("', contrato_obs='");
                        sb.Append(Convert.ToString(oret).Replace("'", "´"));
                    }

                    sb.Append("', contrato_alteracao='");
                    sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sb.Append("' WHERE contrato_numero='");
                    sb.Append(numero.Trim()); sb.Append("' AND contrato_operadoraId=");
                    sb.Append(operadoraId);

                    iret = NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
                    sb.Remove(0, sb.Length);

                    //pega o id do contrato
                    oret = LocatorHelper.Instance.ExecuteScalar(String.Concat("SELECT contrato_id FROM contrato WHERE contrato_numero='", numero.Trim(), "' AND contrato_operadoraId=", operadoraId), null, null, pm);

                    if (!ht.ContainsKey(numero.Trim()))
                    {
                        if (iret > 0)
                        {
                            ht.Add(numero.Trim(), "ok");
                            csi = new ContratoStatusInstancia();
                            csi.ContratoID = oret;
                            csi.Data = dataInativacao;
                            csi.DataSistema = DateTime.Now;
                            csi.StatusID = statusId;
                            csi.StatusTipo = (int)ContratoStatus.eTipo.Inativacao;
                            csi.UsuarioID = usuarioId;
                            csi.OBS = statusObs;
                            pm.Save(csi);
                        }
                        else
                            ht.Add(numero.Trim(), "falhou");
                    }

                    //TODO: marcar os beneficiarios para geracao de arquivos
                }

                pm.Commit();
                return ht;
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }
        }

        public System.Collections.Hashtable ReativaContratos(String[] numerosDasPropostas, Object operadoraId, DateTime dataReativacao, Object statusId, String statusObs, Object usuarioId)
        {
            if (operadoraId == null || numerosDasPropostas == null || numerosDasPropostas.Length == 0) { return null; }

            Object oret = null;
            Int32 iret = 0;

            StringBuilder sb = new StringBuilder();

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            System.Collections.Hashtable ht = new System.Collections.Hashtable();

            try
            {
                ContratoStatusInstancia csi = null;
                foreach (String numero in numerosDasPropostas)
                {
                    if (numero == null || numero.Trim() == "") { continue; }

                    //Pega as observacoes do contrato
                    oret = LocatorHelper.Instance.ExecuteScalar(String.Concat("SELECT contrato_obs FROM contrato WHERE contrato_numero='", numero.Trim(), "' AND contrato_operadoraId=", operadoraId), null, null, pm);
                    if (oret == null || oret == DBNull.Value || Convert.ToString(oret) == "")
                        oret = statusObs;
                    else
                        oret = String.Concat(Convert.ToString(oret), Environment.NewLine, statusObs);

                    sb.Append("UPDATE contrato SET contrato_inativo=0, contrato_dataCancelamento=NULL");

                    sb.Append(", contrato_obs='");
                    sb.Append(Convert.ToString(oret).Replace("'", "´"));

                    sb.Append("', contrato_alteracao='");
                    sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sb.Append("' WHERE contrato_numero='");
                    sb.Append(numero.Trim()); sb.Append("' AND contrato_operadoraId=");
                    sb.Append(operadoraId);

                    iret = NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
                    sb.Remove(0, sb.Length);

                    //pega o id do contrato
                    oret = LocatorHelper.Instance.ExecuteScalar(String.Concat("SELECT contrato_id FROM contrato WHERE contrato_numero='", numero.Trim(), "' AND contrato_operadoraId=", operadoraId), null, null, pm);

                    if (!ht.ContainsKey(numero.Trim()))
                    {
                        if (iret > 0)
                        {
                            ht.Add(numero.Trim(), "ok");
                            csi = new ContratoStatusInstancia();
                            csi.ContratoID = oret;
                            csi.Data = dataReativacao;
                            csi.DataSistema = DateTime.Now;
                            csi.StatusID = statusId;
                            csi.StatusTipo = (int)ContratoStatus.eTipo.Reativacao;
                            csi.UsuarioID = usuarioId;
                            csi.OBS = statusObs;
                            pm.Save(csi);
                        }
                        else
                            ht.Add(numero.Trim(), "falhou");
                    }

                    //marcar os beneficiarios para geracao de arquivos
                    NonQueryHelper.Instance.ExecuteNonQuery(String.Concat(
                        "update contrato_beneficiario set contratobeneficiario_status=", (int)ContratoBeneficiario.eStatus.Novo, " where contratobeneficiario_ativo=1 and contratobeneficiario_contratoid=", oret), 
                        pm);
                }

                pm.Commit();
                return ht;
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }
        }

        public DataTable RelatorioConferenciaDigitacao(String[] contratoAdmIds, DateTime vigencia)
        {
            String qry = String.Concat("SELECT contratoadm_descricao as ContratoAdmDescricao, contrato_numero as PropostaNumero, beneficiario_nome as TitularNome, (SELECT COUNT(contratobeneficiario_id) FROM contrato_beneficiario WHERE contratobeneficiario_contratoId=contrato_id) as QtdVidas ",
                "   FROM contrato ",
                "       INNER JOIN contratoADM ON contratoadm_id = contrato_contratoAdmId ",
                "       INNER JOIN contrato_beneficiario ON contratobeneficiario_contratoId=contrato_id ",
                "       INNER JOIN beneficiario ON contratobeneficiario_beneficiarioId=beneficiario_id AND contratobeneficiario_tipo=0 ",
                "   WHERE contrato_rascunho<>1 AND contrato_cancelado<>1 AND contrato_inativo<>1 ",
                "       AND contrato_vigencia='", vigencia.ToString("yyyy-MM-dd 00:00:00"), "'",
                "       AND contrato_contratoAdmId IN (", String.Join(",", contratoAdmIds),") ",
                "   ORDER BY contratoadm_descricao, contrato_numero");

            return LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];
        }

        public void AtualizaValorDeCobrancas(Object contratoId, out String msg)
        {
            msg = "";

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            Contrato contrato = new Contrato(contratoId);
            pm.Load(contrato);

            try
            {
                IList<Cobranca> cobrancas = Cobranca.CarregarTodas(contratoId, true, Cobranca.eTipo.Normal, pm);

                if (cobrancas != null)
                {
                    List<CobrancaComposite> composite = null;
                    foreach (Cobranca cobranca in cobrancas)
                    {
                        //if (cobranca.Pago) { continue; }
                        if (cobranca.Parcela == 1) { continue; } //nao calcula a parcela 1 pq ela nao está na vigência do beneficiário
                        if (cobranca.Tipo != Convert.ToInt32(Cobranca.eTipo.Normal)) { continue; }

                        if (cobranca.DataVencimento.Year >= DateTime.Now.Year) //atualiza somente parcelas atuais
                        {
                            cobranca.Valor = Contrato.CalculaValorDaProposta(contratoId, cobranca.DataVencimento, pm, false, true, ref composite, true);

                            if (cobranca.Valor > 0)
                            {
                                if (!cobranca.Pago) { pm.Save(cobranca); }
                            }
                            else if (String.IsNullOrEmpty(msg))
                            {
                                if (!cobranca.Pago)
                                {
                                    msg = String.Concat("Data de vencimento ",
                                        cobranca.DataVencimento.ToString("dd/MM/yyyy"), " descoberta. Cobrança com valor incorreto.");
                                    continue;
                                }
                            }

                            if (composite != null)
                            {
                                CobrancaComposite.Remover(cobranca.ID, pm);
                                foreach (CobrancaComposite comp in composite)
                                {
                                    comp.CobrancaID = cobranca.ID;
                                    pm.Save(comp);
                                }
                            }
                        }
                    }
                }

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                //throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }
        }

        public Ent.Saldo CarregaSaldo(long contratoId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<Ent.Saldo>().Where(s => s.Contrato.ID == contratoId).FirstOrDefault();
            }
        }

        public DataTable CarregaPorContratoAdmId(string contratoAdmId)
        {
            string qry = string.Concat(
                "select contrato_id as ContratoID, beneficiario_nome as BeneficiarioNome, contrato_cancelado as Cancelado, contrato_inativo as Inativo ",
                "   from contrato ",
                "       inner join contrato_beneficiario on contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 and contratobeneficiario_contratoId=contrato_id ",
                "       inner join beneficiario on contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 and contratobeneficiario_beneficiarioId=beneficiario_id ",
                "   where ",
                "       contrato_contratoAdmId = ", contratoAdmId, //contrato_cancelado=0 and contrato_inativo=0 and 
                "   order by beneficiario_nome");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];

            return dt;
        }

        public DataTable CarregaPorEstipulanteId(string estipulanteId)
        {
            string qry = string.Concat(
                "select contrato_id as ContratoID, beneficiario_nome as BeneficiarioNome, contrato_cancelado as Cancelado, contrato_inativo as Inativo ",
                "   from contrato ",
                "       inner join contrato_beneficiario on contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 and contratobeneficiario_contratoId=contrato_id ",
                "       inner join beneficiario on contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 and contratobeneficiario_beneficiarioId=beneficiario_id ",
                "   where ",
                "       contrato_estipulanteId = ", estipulanteId, //contrato_cancelado=0 and contrato_inativo=0 and 
                "   order by beneficiario_nome");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];

            return dt;
        }

        public DataTable CarregaSomentePjs()
        {
            string qry = string.Concat(
                "select contrato_id as ContratoID, beneficiario_nome as BeneficiarioNome, contrato_cancelado as Cancelado, contrato_inativo as Inativo ",
                "   from contrato ",
                "       inner join contrato_beneficiario on contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 and contratobeneficiario_contratoId=contrato_id ",
                "       inner join beneficiario on contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 and contratobeneficiario_beneficiarioId=beneficiario_id ",
                "   where ",
                "       contrato_tipoPessoa = 1",
                "   order by beneficiario_nome");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result").Tables[0];

            return dt;
        }

        public List<Ent.SaldoMovimentacaoHistorico> CarregarHistoricoMov(long contratoId, DateTime de, DateTime ate)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<Ent.SaldoMovimentacaoHistorico>()
                    .Where(m => m.Contrato.ID == contratoId && m.Data >= de && m.Data  <= ate)
                    .OrderByDescending(m => m.Data)
                    .ToList();
            }
        }

        public bool ValidarSenha(string numero, long contratoId, string senha)
        {
            bool ret = false;

            using (var sessao = ObterSessao())
            {
                Ent.Contrato contrato = sessao.Query<Ent.Contrato>()
                    .Where(c => c.Numero == numero && c.Senha == senha)
                    .SingleOrDefault();

                ret = contrato != null;
            }

            return ret;
        }

        public void ___Importar()
        {
            int contratoAdmId = 10004, estipulanteId = 10011;
            DateTime agora  = DateTime.Now;

            string connExcel = string.Concat(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\import_massa\dados.xls;Extended Properties='Excel 8.0;HDR=Yes;'");
            DataTable dt = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(connExcel))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [dados$]", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                object aux = null;
                string aux2 = null;
                string qry = null;
                Contrato contrato = null;
                Beneficiario beneficiario = null;
                ContratoBeneficiario contratoBeneficiario = null;
                Endereco endereco = null;
                NumeroCartao numero = null;
                bool existeBeneficiario = false;

                foreach (DataRow row in dt.Rows)
                {
                    existeBeneficiario = false;
                    aux2 = Convert.ToString(row["CPF_TITULAR"]).Replace("-", "").Replace(".", "").PadLeft(11, '0');
                    aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select beneficiario_id from beneficiario where beneficiario_cpf='", aux2, "'"), null, null, pm);

                    if (aux == null && aux == DBNull.Value)
                    {
                        beneficiario = new Beneficiario();
                        beneficiario.Celular = null;
                        beneficiario.CelularOperadora = null;
                        beneficiario.CNS = null;
                        beneficiario.ContratoNumero = null;
                        beneficiario.CPF = aux2;
                    }
                    else
                    {
                        beneficiario = new Beneficiario(aux);
                        pm.Load(beneficiario);
                        existeBeneficiario = true;
                    }

                    beneficiario.DataNascimento = CToDateTime10(Convert.ToString(row["DT_NASCIMENTO"]).Substring(0, 10).Replace("/", ""), 0, 5, 0, true);
                    beneficiario.Nome = Convert.ToString(row["NOME_COMPLETO"]);
                    beneficiario.Sexo = Convert.ToString(row["SEXO"]) == "F" ? "0" : "1";

                    pm.Save(beneficiario);

                    endereco = new Endereco();

                    endereco.Bairro = "Centro";
                    endereco.CEP = "20010120";
                    endereco.DonoId = beneficiario.ID;
                    endereco.DonoTipo = 0;
                    endereco.Logradouro = "Rua do Mercado";
                    endereco.Numero = "11";
                    endereco.Tipo = 0;
                    endereco.UF = "RJ";

                    pm.Save(endereco);


                    contrato = new Contrato();
                    contrato.Adimplente = true;
                    contrato.Admissao = CToDateTime10(Convert.ToString(row["DATA_DE_CADATRO"]).Substring(0,10).Replace("/",""), 0, 5, 0, true);
                    contrato.Cancelado = Convert.ToString(row["STATUS_DO_CARTAO"]) == "HABILITADO" ? false : true;
                    contrato.CobrarTaxaAssociativa = false;
                    contrato.CodCobranca = -5;
                    contrato.ContratoADMID = contratoAdmId;
                    contrato.CorretorTerceiroCPF = "";
                    contrato.CorretorTerceiroNome = "";
                    contrato.Data = agora;
                    contrato.DataValidade = agora.AddYears(1);
                    contrato.Desconto = 0;
                    contrato.DonoID = 1;
                    contrato.EmailCobranca = "";
                    contrato.EnderecoCobrancaID = endereco.ID; //??
                    contrato.EnderecoReferenciaID = endereco.ID; //??
                    contrato.EstipulanteID = estipulanteId;
                    contrato.FilialID = 4; //??
                    contrato.GerarSenha();
                    contrato.Importado = true;
                    contrato.Inativo = contrato.Cancelado;
                    contrato.Numero = ""; //?
                    contrato.NumeroID = null; //??
                    contrato.NumeroMatricula = Convert.ToString(row["IDENTICADOR_EXTERNO"]);
                    contrato.Obs = null;
                    contrato.OperadoraID = 3;
                    contrato.Pendente = false;
                    contrato.PlanoID = 1210; //??
                    contrato.Rascunho = true;
                    contrato.Status = 0;
                    contrato.TipoContratoID = 1;
                    contrato.UsuarioID = 1;
                    contrato.Vencimento = agora;
                    contrato.Vigencia = CToDateTime10(Convert.ToString(row["DT_VIGENCIA"]).Substring(0, 10).Replace("/", ""), 0, 5, 0, true);

                    #region numero do contrato 

                    numero = new NumeroCartao();

                    aux = Convert.ToString(row["CARTAO"]).Replace(".", "");

                    numero.Ativo = true;
                    numero.Data = DateTime.Now;
                    numero.Via = Convert.ToInt32(Convert.ToString(aux).Substring(14, 1));
                    numero.SetaDv(Convert.ToInt32(Convert.ToString(aux).Substring(15, 1)));
                    numero.Numero = Convert.ToString(aux).Substring(0, 14);
                    numero.GerarCV();

                    contrato.Numero = numero.NumeroCompletoSemCV;

                    pm.Save(numero);

                    contrato.NumeroID = Convert.ToInt64(numero.ID);

                    #endregion

                    pm.Save(contrato);

                    numero.ContratoId = contrato.ID;
                    pm.Save(numero);

                    contratoBeneficiario = new ContratoBeneficiario();

                    //contratoBeneficiario.Altura = 0;
                    contratoBeneficiario.Ativo = true;
                    contratoBeneficiario.BeneficiarioID = beneficiario.ID;
                    contratoBeneficiario.CarenciaCodigo = null;
                    contratoBeneficiario.ContratoID = contrato.ID;
                    contratoBeneficiario.Data = agora;
                    contratoBeneficiario.NumeroSequencial = 0;
                    //contratoBeneficiario.Peso = 0;
                    contratoBeneficiario.Status = 0;
                    contratoBeneficiario.Tipo = 0;
                    contratoBeneficiario.Valor = 0;
                    contratoBeneficiario.Vigencia = contrato.Vigencia;

                    pm.Save(contratoBeneficiario);
                }
                

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }

            /* 
 
            begin tran 
            --ENDERECO
            delete from endereco where endereco_id in 
            (select contrato_enderecoReferenciaId from contrato where contrato_rascunho=1)

            --BENEFICIARIO
            delete from beneficiario where beneficiario_id in 
            (select contratobeneficiario_beneficiarioid from contrato_beneficiario where contratobeneficiario_contratoid in 
            (select contrato_id from contrato where contrato_rascunho=1))

            --CONTRATO X BENEFICIARIO
            delete from contrato_beneficiario where contratobeneficiario_contratoid in 
            (select contrato_id from contrato where contrato_rascunho=1)

            --NUMERO DE CONTRATO
            delete from numero_contrato where numerocontrato_id in 
            (select contrato_numeroid from contrato where contrato_rascunho=1)

            --CONTRATO
            delete from contrato where contrato_rascunho=1

            commit tran

            */
        }

        string toString(object param)
        {
            if (param == null || param == DBNull.Value)
                return "";
            else
                return Convert.ToString(param);
        }

        decimal toDecimal(object param, System.Globalization.CultureInfo cinfo)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return decimal.Zero;
            else
                return Convert.ToDecimal(param);
        }

        DateTime toDateTime(object param, System.Globalization.CultureInfo cinfo)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return DateTime.MinValue;
            else
                return Convert.ToDateTime(param, cinfo);
        }

        public void ___ImportarHabilitados()
        {
            int estipulanteId = 10019, contratoAdmId = 10046, planoId = 1251, filialId = 4;
            DateTime agora = DateTime.Now;
            Beneficiario beneficiario = null;

            //string connExcel = string.Concat(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\import_massa\MIGRACAO_OAB.xlsx;Extended Properties='Excel 8.0;HDR=Yes;'");
            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\import_massa\MIGRACAO_OAB.accdb;Persist Security Info=False");
            DataTable dt = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [dados]", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                #region variaveis 

                object aux = null;
                string aux2 = null;
                string qry = null;
                Contrato contrato = null;
                ContratoBeneficiario contratoBeneficiario = null;
                Endereco endereco = null;
                NumeroCartao numero = null;
                bool existeBeneficiario = false;

                #endregion

                foreach (DataRow row in dt.Rows)
                {
                    if (toString(row["STATUS_DO_CARTAO"]).ToLower() != "habilitado") continue;

                    existeBeneficiario = false;
                    aux2 = Convert.ToString(row["CPF_TITULAR"]).Replace("-", "").Replace(".", "").PadLeft(11, '0');
                    aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select beneficiario_id from beneficiario where beneficiario_cpf='", aux2, "'"), null, null, pm);

                    if (aux == null || aux == DBNull.Value)
                    {
                        beneficiario = new Beneficiario();
                        beneficiario.Celular = null;
                        beneficiario.CelularOperadora = null;
                        beneficiario.CNS = null;
                        beneficiario.ContratoNumero = null;
                        beneficiario.CPF = aux2;
                    }
                    else
                    {
                        beneficiario = new Beneficiario(aux);
                        pm.Load(beneficiario);
                        existeBeneficiario = true;
                    }

                    beneficiario.DataNascimento = CToDateTime10(Convert.ToString(row["DT_NASCIMENTO"]).Substring(0, 10).Replace("/", ""), 0, 5, 0, true);
                    beneficiario.Nome = Convert.ToString(row["NOME_COMPLETO"]);
                    beneficiario.Sexo = Convert.ToString(row["SEXO"]) == "F" ? "0" : "1";

                    pm.Save(beneficiario);

                    if (beneficiario.Nome == "ALOYSIO MULLER DE OLIVEIRA")
                    {
                        int para = 0;
                    }

                    #region endereco 

                    if (!existeBeneficiario)
                    {
                        endereco = new Endereco();
                        endereco.Bairro = toString(row["BAIRRO"]);
                        endereco.CEP = toString(row["CEP"]).Replace("-", "");
                        endereco.DonoId = beneficiario.ID;
                        endereco.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                        endereco.Logradouro = toString(row["LOGRADOURO"]);
                        endereco.Numero = toString(row["NUMERO"]);
                        endereco.Cidade = toString(row["CIDADE"]);
                        endereco.Tipo = 0;
                        endereco.UF = toString(row["UF"]);

                        pm.Save(endereco);
                    }
                    else
                    {
                        aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select endereco_id from endereco where endereco_donoTipo=0 and endereco_cep='", toString(row["CEP"]).Replace("-", ""), "' and endereco_numero='", row["NUMERO"], "' and endereco_donoId=", beneficiario.ID), null, null, pm);
                        if (aux != null && aux != DBNull.Value)
                        {
                            endereco = new Endereco(aux);
                            pm.Load(endereco);
                        }
                        else
                        {
                            endereco = new Endereco();
                            endereco.Bairro = toString(row["BAIRRO"]);
                            endereco.CEP = toString(row["CEP"]).Replace("-", "");
                            endereco.DonoId = beneficiario.ID;
                            endereco.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                            endereco.Logradouro = toString(row["LOGRADOURO"]);
                            endereco.Numero = toString(row["NUMERO"]);
                            endereco.Cidade = toString(row["CIDADE"]);
                            endereco.Tipo = 0;
                            endereco.UF = toString(row["UF"]);

                            pm.Save(endereco);
                        }
                    }
                    #endregion


                    contrato = new Contrato();
                    contrato.Adimplente = true;
                    contrato.Admissao = CToDateTime10(Convert.ToString(row["DATA_DE_CADATRO"]).Substring(0, 10).Replace("/", ""), 0, 5, 0, true);
                    contrato.Cancelado = Convert.ToString(row["STATUS_DO_CARTAO"]).ToUpper() == "HABILITADO" ? false : true;
                    contrato.CobrarTaxaAssociativa = false;
                    contrato.CodCobranca = -5;
                    contrato.ContratoADMID = contratoAdmId;
                    contrato.CorretorTerceiroCPF = "";
                    contrato.CorretorTerceiroNome = "";
                    contrato.Data = agora;
                    contrato.DataValidade = agora.AddYears(1);
                    contrato.Desconto = 0;
                    contrato.DonoID = 1;
                    contrato.EmailCobranca = toString(row["EMAIL"]);
                    contrato.EnderecoCobrancaID = endereco.ID; 
                    contrato.EnderecoReferenciaID = endereco.ID; 
                    contrato.EstipulanteID = estipulanteId;
                    contrato.FilialID = filialId;
                    contrato.GerarSenha();
                    contrato.Importado = true;
                    contrato.Inativo = contrato.Cancelado;
                    contrato.Numero = ""; //?
                    contrato.NumeroID = null; //??
                    contrato.NumeroMatricula = Convert.ToString(row["IDENTICADOR_EXTERNO"]);
                    contrato.Obs = null;
                    contrato.OperadoraID = 3;
                    contrato.Pendente = false;
                    contrato.PlanoID = planoId; 
                    contrato.Rascunho = false;
                    contrato.Status = 0;
                    contrato.TipoContratoID = 1;
                    contrato.UsuarioID = 1;
                    contrato.Vencimento = agora;

                    if (toString(row["DT_VIGENCIA"]).Trim() == "")
                        contrato.Vigencia = agora;
                    else
                        contrato.Vigencia = CToDateTime10(Convert.ToString(row["DT_VIGENCIA"]).Substring(0, 10).Replace("/", ""), 0, 5, 0, true);

                    #region numero do contrato

                    numero = new NumeroCartao();

                    aux = Convert.ToString(row["CARTAO"]).Replace(".", "");

                    numero.Ativo = true;
                    numero.Data = DateTime.Now;
                    numero.Via = Convert.ToInt32(Convert.ToString(aux).Substring(14, 1));
                    numero.SetaDv(Convert.ToInt32(Convert.ToString(aux).Substring(15, 1)));
                    numero.Numero = Convert.ToString(aux).Substring(0, 14);
                    numero.GerarCV();

                    contrato.Numero = numero.NumeroCompletoSemCV;

                    pm.Save(numero);

                    contrato.NumeroID = Convert.ToInt64(numero.ID);

                    #endregion

                    pm.Save(contrato);

                    numero.ContratoId = contrato.ID;
                    pm.Save(numero);

                    contratoBeneficiario = new ContratoBeneficiario();

                    //contratoBeneficiario.Altura = 0;
                    contratoBeneficiario.Ativo = true;
                    contratoBeneficiario.BeneficiarioID = beneficiario.ID;
                    contratoBeneficiario.CarenciaCodigo = null;
                    contratoBeneficiario.ContratoID = contrato.ID;
                    contratoBeneficiario.Data = agora;
                    contratoBeneficiario.NumeroSequencial = 0;
                    //contratoBeneficiario.Peso = 0;
                    contratoBeneficiario.Status = 0;
                    contratoBeneficiario.Tipo = 0;
                    contratoBeneficiario.Valor = 0;
                    contratoBeneficiario.Vigencia = contrato.Vigencia;

                    pm.Save(contratoBeneficiario);
                }


                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }

            /* 
 
            begin tran 
            --ENDERECO
            delete from endereco where endereco_id in 
            (select contrato_enderecoReferenciaId from contrato where contrato_rascunho=1)

            --BENEFICIARIO
            delete from beneficiario where beneficiario_id in 
            (select contratobeneficiario_beneficiarioid from contrato_beneficiario where contratobeneficiario_contratoid in 
            (select contrato_id from contrato where contrato_rascunho=1))

            --CONTRATO X BENEFICIARIO
            delete from contrato_beneficiario where contratobeneficiario_contratoid in 
            (select contrato_id from contrato where contrato_rascunho=1)

            --NUMERO DE CONTRATO
            delete from numero_contrato where numerocontrato_id in 
            (select contrato_numeroid from contrato where contrato_rascunho=1)

            --CONTRATO
            delete from contrato where contrato_rascunho=1

            commit tran

            */
        }

        public void ___ImportarCancelados()
        {
            int estipulanteId = 10019, contratoAdmId = 10046, planoId = 1251, filialId = 4;
            DateTime agora = DateTime.Now;
            Beneficiario beneficiario = null;

            //string connExcel = string.Concat(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\import_massa\MIGRACAO_OAB.xlsx;Extended Properties='Excel 8.0;HDR=Yes;'");
            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\import_massa\MIGRACAO_OAB.accdb;Persist Security Info=False");
            DataTable dt = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [dados]", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();
            int i = 0;

            try
            {
                #region variaveis

                object aux = null;
                string aux2 = null;
                string qry = null;
                Contrato contrato = null;
                ContratoBeneficiario contratoBeneficiario = null;
                Endereco endereco = null;
                NumeroCartao numero = null;
                bool existeBeneficiario = false;
                bool statusCanceladoOuAtivo = false;

                #endregion

                foreach (DataRow row in dt.Rows)
                {
                    i++;
                    if (toString(row["STATUS_DO_CARTAO"]).ToLower() != "cancelado") continue;

                    if (i == 822)
                    {
                        int parar = 0;
                    }

                    existeBeneficiario = false;
                    aux2 = Convert.ToString(row["CPF_TITULAR"]).Replace("-", "").Replace(".", "").PadLeft(11, '0');

                    aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select contrato_id from contrato inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id inner join beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId where contrato_cancelado=0 and contrato_inativo=0 and beneficiario_cpf='", aux2, "'"), null, null, pm);
                    if (aux == null || aux == DBNull.Value)
                        statusCanceladoOuAtivo = false; // se nao achou, ativo
                    else
                        statusCanceladoOuAtivo = true; // se achou ativo, mantem cancelado

                    aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select beneficiario_id from beneficiario where beneficiario_cpf='", aux2, "'"), null, null, pm);

                    if (aux == null || aux == DBNull.Value)
                    {
                        beneficiario = new Beneficiario();
                        beneficiario.Celular = null;
                        beneficiario.CelularOperadora = null;
                        beneficiario.CNS = null;
                        beneficiario.ContratoNumero = null;
                        beneficiario.CPF = aux2;
                    }
                    else
                    {
                        beneficiario = new Beneficiario(aux);
                        pm.Load(beneficiario);
                        existeBeneficiario = true;
                    }

                    try
                    {
                        beneficiario.DataNascimento = CToDateTime10(Convert.ToString(row["DT_NASCIMENTO"]).Substring(0, 10).Replace("/", ""), 0, 5, 0, true);
                    }
                    catch
                    {
                        beneficiario.DataNascimento = DateTime.Now;
                    }
                    beneficiario.Nome = Convert.ToString(row["NOME_COMPLETO"]);
                    beneficiario.Sexo = Convert.ToString(row["SEXO"]) == "F" ? "0" : "1";

                    pm.Save(beneficiario);

                    if (beneficiario.Nome == "ALOYSIO MULLER DE OLIVEIRA")
                    {
                        int para = 0;
                    }

                    #region endereco

                    if (!existeBeneficiario)
                    {
                        endereco = new Endereco();
                        endereco.Bairro = toString(row["BAIRRO"]);
                        endereco.CEP = toString(row["CEP"]).Replace("-", "");
                        endereco.DonoId = beneficiario.ID;
                        endereco.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                        endereco.Logradouro = toString(row["LOGRADOURO"]);
                        endereco.Numero = toString(row["NUMERO"]);
                        endereco.Cidade = toString(row["CIDADE"]);
                        endereco.Tipo = 0;
                        endereco.UF = toString(row["UF"]);

                        pm.Save(endereco);
                    }
                    else
                    {
                        aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select endereco_id from endereco where endereco_donoTipo=0 and endereco_cep='", toString(row["CEP"]).Replace("-", ""), "' and endereco_numero='", row["NUMERO"], "' and endereco_donoId=", beneficiario.ID), null, null, pm);
                        if (aux != null && aux != DBNull.Value)
                        {
                            endereco = new Endereco(aux);
                            pm.Load(endereco);
                        }
                        else
                        {
                            endereco = new Endereco();
                            endereco.Bairro = toString(row["BAIRRO"]);
                            endereco.CEP = toString(row["CEP"]).Replace("-", "");
                            endereco.DonoId = beneficiario.ID;
                            endereco.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                            endereco.Logradouro = toString(row["LOGRADOURO"]);
                            endereco.Numero = toString(row["NUMERO"]);
                            endereco.Cidade = toString(row["CIDADE"]);
                            endereco.Tipo = 0;
                            endereco.UF = toString(row["UF"]);

                            pm.Save(endereco);
                        }
                    }
                    #endregion


                    contrato = new Contrato();
                    contrato.Adimplente = true;
                    contrato.Admissao = CToDateTime10(Convert.ToString(row["DATA_DE_CADATRO"]).Substring(0, 10).Replace("/", ""), 0, 5, 0, true);
                    contrato.Cancelado = statusCanceladoOuAtivo;
                    contrato.CobrarTaxaAssociativa = false;
                    contrato.CodCobranca = -5;
                    contrato.ContratoADMID = contratoAdmId;
                    contrato.CorretorTerceiroCPF = "";
                    contrato.CorretorTerceiroNome = "";
                    contrato.Data = agora;
                    contrato.DataValidade = agora.AddYears(1);
                    contrato.Desconto = 0;
                    contrato.DonoID = 1;
                    contrato.EmailCobranca = toString(row["EMAIL"]);
                    contrato.EnderecoCobrancaID = endereco.ID;
                    contrato.EnderecoReferenciaID = endereco.ID;
                    contrato.EstipulanteID = estipulanteId;
                    contrato.FilialID = filialId;
                    contrato.GerarSenha();
                    contrato.Importado = true;
                    contrato.Inativo = contrato.Cancelado;
                    contrato.Numero = ""; //?
                    contrato.NumeroID = null; //??
                    contrato.NumeroMatricula = Convert.ToString(row["IDENTICADOR_EXTERNO"]);
                    contrato.Obs = null;
                    contrato.OperadoraID = 3;
                    contrato.Pendente = false;
                    contrato.PlanoID = planoId;
                    contrato.Rascunho = false;
                    contrato.Status = 0;
                    contrato.TipoContratoID = 1;
                    contrato.UsuarioID = 1;
                    contrato.Vencimento = agora;

                    if (toString(row["DT_VIGENCIA"]).Trim() == "")
                        contrato.Vigencia = agora;
                    else
                        contrato.Vigencia = CToDateTime10(Convert.ToString(row["DT_VIGENCIA"]).Substring(0, 10).Replace("/", ""), 0, 5, 0, true);

                    #region numero do contrato

                    numero = new NumeroCartao();

                    aux = Convert.ToString(row["CARTAO"]).Replace(".", "");

                    numero.Ativo = true;
                    numero.Data = DateTime.Now;
                    numero.Via = Convert.ToInt32(Convert.ToString(aux).Substring(14, 1));
                    numero.SetaDv(Convert.ToInt32(Convert.ToString(aux).Substring(15, 1)));
                    numero.Numero = Convert.ToString(aux).Substring(0, 14);
                    numero.GerarCV();

                    contrato.Numero = numero.NumeroCompletoSemCV;

                    pm.Save(numero);

                    contrato.NumeroID = Convert.ToInt64(numero.ID);

                    #endregion

                    pm.Save(contrato);

                    numero.ContratoId = contrato.ID;
                    pm.Save(numero);

                    contratoBeneficiario = new ContratoBeneficiario();

                    //contratoBeneficiario.Altura = 0;
                    contratoBeneficiario.Ativo = true;
                    contratoBeneficiario.BeneficiarioID = beneficiario.ID;
                    contratoBeneficiario.CarenciaCodigo = null;
                    contratoBeneficiario.ContratoID = contrato.ID;
                    contratoBeneficiario.Data = agora;
                    contratoBeneficiario.NumeroSequencial = 0;
                    //contratoBeneficiario.Peso = 0;
                    contratoBeneficiario.Status = 0;
                    contratoBeneficiario.Tipo = 0;
                    contratoBeneficiario.Valor = 0;
                    contratoBeneficiario.Vigencia = contrato.Vigencia;

                    pm.Save(contratoBeneficiario);
                }


                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }

            /* 
 
            begin tran 
            --ENDERECO
            delete from endereco where endereco_id in 
            (select contrato_enderecoReferenciaId from contrato where contrato_rascunho=1)

            --BENEFICIARIO
            delete from beneficiario where beneficiario_id in 
            (select contratobeneficiario_beneficiarioid from contrato_beneficiario where contratobeneficiario_contratoid in 
            (select contrato_id from contrato where contrato_rascunho=1))

            --CONTRATO X BENEFICIARIO
            delete from contrato_beneficiario where contratobeneficiario_contratoid in 
            (select contrato_id from contrato where contrato_rascunho=1)

            --NUMERO DE CONTRATO
            delete from numero_contrato where numerocontrato_id in 
            (select contrato_numeroid from contrato where contrato_rascunho=1)

            --CONTRATO
            delete from contrato where contrato_rascunho=1

            commit tran

            */
        }

        public void ___ImportarFUNC()
        {
            bool jhm = false;
            //int estipulanteId = 8, contratoAdmId = 10043, planoId = 1248, filialId = 4;
            //int estipulanteId = 8, contratoAdmId = 5, planoId = 514, filialId = 4; //pata negra -15
            //int estipulanteId = 10022, contratoAdmId = 0, planoId = 0, filialId = 4; //sindirepa -20 ==================
            //int estipulanteId = 111, contratoAdmId = 0, planoId = 0, filialId = 4; //jhm (lapa) -25 ==================
            //int estipulanteId = 110, contratoAdmId = 268, planoId = 1197, filialId = 4; //fisica  -30
            int estipulanteId = 10016, contratoAdmId = 10333, planoId = 1538, filialId = 4; //importacao final  -35 
            DateTime agora = DateTime.Now;
            Beneficiario beneficiario = null;

            //string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\import_massa\lapa.accdb;Persist Security Info=False");
            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\import_massa\final.accdb;Persist Security Info=False");
            DataTable dt = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [CartoesFinais]", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();
            int i = 0;
            StringBuilder sb = new StringBuilder();
            List<string> lista = new List<string>();

            try
            {
                #region variaveis

                object aux = null;
                string aux2 = null;
                string qry = null;
                Contrato contrato = null;
                ContratoBeneficiario contratoBeneficiario = null;
                Endereco endereco = null;
                NumeroCartao numero = null;
                bool existeBeneficiario = false;
                List<string> numeros = new List<string>();

                #endregion

                foreach (DataRow row in dt.Rows)
                {
                    i++;

                    #region verfifica se ja existe

                    //aux = LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato where contrato_numero='" + Convert.ToString(row["NumeroCartao"]).Replace(".", "") + "'", null, null, pm);
                    //if (aux == null || aux == DBNull.Value)
                    //{
                    //    numeros.Add(Convert.ToString(row["NumeroCartao"]));
                    //}

                    //continue;

                    #endregion

                    #region só deixa importar quem NAO existe

                    aux = LocatorHelper.Instance.ExecuteScalar("select contrato_id from contrato where contrato_numero='" + Convert.ToString(row["CARTAO"]).Replace(".", "") + "'", null, null, pm);
                    if (aux != null && aux != DBNull.Value)
                    {
                        continue;
                    }

                    #endregion

                    #region verifica duplicidade antes de importar
                    //aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select numerocontrato_id from numero_contrato where numerocontrato_numero='", Convert.ToString(row["CARTAO"]).Replace(".", "").Replace("'", "").Substring(0, 14), "'"), null, null, pm);

                    //if (aux != null && aux != DBNull.Value)
                    //{
                    //    int j = 0;
                    //}

                    //continue;
                    #endregion

                    #region Serve para SINDIREPA e JHM 
                    ////Contrato ADM
                    //aux = LocatorHelper.Instance.ExecuteScalar("select contratoadm_id from contratoadm where contratoadm_descricao='" + Convert.ToString(row["nomeAssociada"]).Replace("'", "") + "' and contratoadm_estipulanteId=" + estipulanteId, null, null, pm);
                    //if (aux == null || aux == DBNull.Value)
                    //{
                    //    aux = LocatorHelper.Instance.ExecuteScalar("select contratoadm_id from contratoadm where contratoadm_doc='" + Convert.ToString(row["cnpjAssociada"]).Replace(".", "") + "' and contratoadm_estipulanteId=" + estipulanteId, null, null, pm);
                    //    if (aux == null || aux == DBNull.Value)
                    //    {
                    //        #region TRAVA
                    //        pm.Rollback();
                    //        return;
                    //        #endregion

                    //        #region LOGA
                    //        //if (lista.Contains(Convert.ToString(row["nomeAssociada"]))) continue;

                    //        //if (sb.Length > 0) sb.Append(", ");
                    //        //sb.Append(row["nomeAssociada"]);
                    //        //lista.Add(Convert.ToString(row["nomeAssociada"]));
                    //        #endregion

                    //        #region CRIA
                    //        //NonQueryHelper.Instance.ExecuteNonQuery("INSERT INTO contratoADM ([contratoadm_descricao],[contratoadm_operadoraId],[contratoadm_estipulanteId],[contratoadm_ativo],[contratoadm_data]) VALUES ('" + Convert.ToString(row["nomeAssociada"]).Replace("'", "") + "',3," + estipulanteId + ",1,getdate())", pm);
                    //        //aux = LocatorHelper.Instance.ExecuteScalar("select contratoadm_id from contratoadm where contratoadm_descricao='" + Convert.ToString(row["nomeAssociada"]).Replace("'", "") + "' and contratoadm_estipulanteId=" + estipulanteId, null, null, pm);

                    //        //NonQueryHelper.Instance.ExecuteNonQuery("INSERT INTO plano(plano_contratoId,plano_descricao,[plano_ativo],[plano_data],[plano_quartoComum],[plano_quartoParticular])VALUES(" + aux + ",'Padrão',1,getdate(),1,0)", pm);
                    //        #endregion
                    //    }
                    //}
                    //contratoAdmId = Convert.ToInt32(aux);
                    ////continue;

                    ////Plano
                    //aux = LocatorHelper.Instance.ExecuteScalar("select plano_id from plano where plano_contratoid=" + contratoAdmId, null, null, pm);
                    //if (aux == null || aux == DBNull.Value)
                    //{
                    //    pm.Rollback();
                    //    return;
                    //}
                    //planoId = Convert.ToInt32(aux);
                    #endregion

                    if (i == 822)
                    {
                        int parar = 0;
                    }

                    existeBeneficiario = false;
                    aux2 = Convert.ToString(row["CPF_TITULAR"]).Replace("-", "").Replace(".", "").PadLeft(11, '0');

                    aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select beneficiario_id from beneficiario where beneficiario_cpf='", aux2, "'"), null, null, pm);

                    if (aux == null || aux == DBNull.Value)
                    {
                        beneficiario = new Beneficiario();
                        beneficiario.Celular = null;
                        beneficiario.CelularOperadora = null;
                        beneficiario.CNS = null;
                        beneficiario.ContratoNumero = null;
                        beneficiario.CPF = aux2;
                    }
                    else
                    {
                        beneficiario = new Beneficiario(aux);
                        pm.Load(beneficiario);
                        existeBeneficiario = true;
                    }

                    try
                    {
                        beneficiario.DataNascimento = CToDateTime10(Convert.ToString(row["DT_NASCIMENTO"]).Substring(0, 10).Replace("/", ""), 0, 5, 0, true);
                    }
                    catch
                    {
                        beneficiario.DataNascimento = DateTime.Now;
                    }
                    beneficiario.Nome = Convert.ToString(row["NOME_COMPLETO"]);
                    beneficiario.Sexo = Convert.ToString(row["SEXO"]) == "F" ? "0" : "1";

                    pm.Save(beneficiario);

                    //if (beneficiario.Nome == "ALOYSIO MULLER DE OLIVEIRA")
                    //{
                    //    int para = 0;
                    //}

                    #region endereco

                    if (!jhm)
                    {
                        if (!existeBeneficiario)
                        {
                            endereco = new Endereco();

                            if (toString(row["LOGRADOURO"]).Trim() != "")
                            {
                                endereco.Bairro = toString(row["BAIRRO"]);
                                endereco.CEP = toString(row["CEP"]).Replace("-", "");
                                endereco.DonoId = beneficiario.ID;
                                endereco.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                                endereco.Logradouro = toString(row["LOGRADOURO"]);
                                endereco.Numero = toString(row["NUMERO"]);
                                endereco.Cidade = toString(row["CIDADE"]);
                                endereco.Tipo = 0;
                                endereco.UF = toString(row["UF"]);
                            }
                            else
                            {
                                endereco.Bairro = "CENTRO";
                                endereco.CEP = "20010120";
                                endereco.Complemento = "22o ANDAR";
                                endereco.DonoId = beneficiario.ID;
                                endereco.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                                endereco.Logradouro = "RUA DO MERCADO";
                                endereco.Numero = "11";
                                endereco.Cidade = "RIO DE JANEIRO";
                                endereco.Tipo = 0;
                                endereco.UF = "RJ";
                            }

                            pm.Save(endereco);
                        }
                        else
                        {
                            aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select endereco_id from endereco where endereco_donoTipo=0 and endereco_cep='", toString(row["CEP"]).Replace("-", ""), "' and endereco_numero='", row["NUMERO"], "' and endereco_donoId=", beneficiario.ID), null, null, pm);
                            if (aux != null && aux != DBNull.Value)
                            {
                                endereco = new Endereco(aux);
                                pm.Load(endereco);
                            }
                            else
                            {
                                endereco = new Endereco();
                                if (toString(row["LOGRADOURO"]).Trim() != "")
                                {
                                    endereco.Bairro = toString(row["BAIRRO"]);
                                    endereco.CEP = toString(row["CEP"]).Replace("-", "");
                                    endereco.DonoId = beneficiario.ID;
                                    endereco.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                                    endereco.Logradouro = toString(row["LOGRADOURO"]);
                                    endereco.Numero = toString(row["NUMERO"]);
                                    endereco.Cidade = toString(row["CIDADE"]);
                                    endereco.Tipo = 0;
                                    endereco.UF = toString(row["UF"]);
                                }
                                else
                                {
                                    endereco.Bairro = "CENTRO";
                                    endereco.CEP = "20010120";
                                    endereco.Complemento = "22o ANDAR";
                                    endereco.DonoId = beneficiario.ID;
                                    endereco.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                                    endereco.Logradouro = "RUA DO MERCADO";
                                    endereco.Numero = "11";
                                    endereco.Cidade = "RIO DE JANEIRO";
                                    endereco.Tipo = 0;
                                    endereco.UF = "RJ";
                                }

                                pm.Save(endereco);
                            }
                        }
                    }
                    else
                    {
                        endereco = new Endereco();
                        endereco.Bairro = "CENTRO";
                        endereco.CEP = "20010120";
                        endereco.Complemento = "22o ANDAR";
                        endereco.DonoId = beneficiario.ID;
                        endereco.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                        endereco.Logradouro = "RUA DO MERCADO";
                        endereco.Numero = "11";
                        endereco.Cidade = "RIO DE JANEIRO";
                        endereco.Tipo = 0;
                        endereco.UF = "RJ";

                        pm.Save(endereco);
                    }
                    #endregion

                    contrato = new Contrato();
                    contrato.Adimplente = true;
                    try
                    {
                        contrato.Admissao = CToDateTime10(Convert.ToString(row["DATA_DE_CADATRO"]).Substring(0, 10).Replace("/", ""), 0, 5, 0, true);
                    }
                    catch
                    {
                        contrato.Admissao = CToDateTime10(Convert.ToString(row["DT_VIGENCIA"]).Substring(0, 10).Replace("/", ""), 0, 5, 0, true);
                    }

                    contrato.Cancelado = Convert.ToString(row["STATUS_DO_CARTAO"]).ToUpper() == "CANCELADO" ? true : false;
                    contrato.CobrarTaxaAssociativa = false;
                    contrato.CodCobranca = -35;
                    contrato.ContratoADMID = contratoAdmId;
                    contrato.CorretorTerceiroCPF = "";
                    contrato.CorretorTerceiroNome = "";
                    contrato.Data = agora;
                    contrato.DataValidade = agora.AddYears(1);
                    contrato.Desconto = 0;
                    contrato.DonoID = 1;
                    contrato.EmailCobranca = toString(row["EMAIL"]);
                    contrato.EnderecoCobrancaID = endereco.ID;
                    contrato.EnderecoReferenciaID = endereco.ID;
                    contrato.EstipulanteID = estipulanteId;
                    contrato.FilialID = filialId;
                    contrato.GerarSenha();
                    contrato.Importado = true;
                    contrato.Inativo = contrato.Cancelado;
                    contrato.Numero = ""; //?
                    contrato.NumeroID = null; //??
                    contrato.NumeroMatricula = Convert.ToString(row["IDENTICADOR_EXTERNO"]);
                    contrato.Obs = null;
                    contrato.OperadoraID = 3;
                    contrato.Pendente = false;
                    contrato.PlanoID = planoId;
                    contrato.Rascunho = false;
                    contrato.Status = 0;
                    contrato.TipoContratoID = 1;
                    contrato.UsuarioID = 1;
                    contrato.Vencimento = agora;

                    if (toString(row["DT_VIGENCIA"]).Trim() == "")
                        contrato.Vigencia = agora;
                    else
                        contrato.Vigencia = CToDateTime10(Convert.ToString(row["DT_VIGENCIA"]).Substring(0, 10).Replace("/", ""), 0, 5, 0, true);

                    #region numero do contrato

                    numero = new NumeroCartao();

                    aux = Convert.ToString(row["CARTAO"]).Replace(".", "").Replace("'", "");

                    numero.Ativo = true;
                    numero.Data = DateTime.Now;
                    numero.Via = Convert.ToInt32(Convert.ToString(aux).Substring(14, 1));
                    numero.SetaDv(Convert.ToInt32(Convert.ToString(aux).Substring(15, 1)));
                    numero.Numero = Convert.ToString(aux).Substring(0, 14);
                    numero.GerarCV();

                    contrato.Numero = numero.NumeroCompletoSemCV;

                    pm.Save(numero);

                    contrato.NumeroID = Convert.ToInt64(numero.ID);

                    #endregion

                    pm.Save(contrato);

                    numero.ContratoId = contrato.ID;
                    pm.Save(numero);

                    contratoBeneficiario = new ContratoBeneficiario();

                    //contratoBeneficiario.Altura = 0;
                    contratoBeneficiario.Ativo = true;
                    contratoBeneficiario.BeneficiarioID = beneficiario.ID;
                    contratoBeneficiario.CarenciaCodigo = null;
                    contratoBeneficiario.ContratoID = contrato.ID;
                    contratoBeneficiario.Data = agora;
                    contratoBeneficiario.NumeroSequencial = 0;
                    //contratoBeneficiario.Peso = 0;
                    contratoBeneficiario.Status = 0;
                    contratoBeneficiario.Tipo = 0;
                    contratoBeneficiario.Valor = 0;
                    contratoBeneficiario.Vigencia = contrato.Vigencia;

                    pm.Save(contratoBeneficiario);
                }

                //foreach (string __numero in numeros)
                //{
                //    qry = string.Concat("insert into _log_nao_localizados_2 (cartao) values ('",
                //        __numero, "')");

                //    NonQueryHelper.Instance.ExecuteNonQuery(qry, pm);
                //}


                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }

            /* 
 
            begin tran 
            --ENDERECO
            delete from endereco where endereco_id in 
            (select contrato_enderecoReferenciaId from contrato where contrato_rascunho=1)

            --BENEFICIARIO
            delete from beneficiario where beneficiario_id in 
            (select contratobeneficiario_beneficiarioid from contrato_beneficiario where contratobeneficiario_contratoid in 
            (select contrato_id from contrato where contrato_rascunho=1))

            --CONTRATO X BENEFICIARIO
            delete from contrato_beneficiario where contratobeneficiario_contratoid in 
            (select contrato_id from contrato where contrato_rascunho=1)

            --NUMERO DE CONTRATO
            delete from numero_contrato where numerocontrato_id in 
            (select contrato_numeroid from contrato where contrato_rascunho=1)

            --CONTRATO
            delete from contrato where contrato_rascunho=1

            commit tran

            */
        }

        public void ___importarAssociadoPJ_e_ContratosADM_ArrumaDOC(DataTable dt)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();
            int i = 0;

            StringBuilder sb = new StringBuilder();

            List<string> lista = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                ////if (Convert.ToString(row["CLIENTE"]) != "AUTO SERVIÇOS SAO MARCOS LTDA") continue;

                if (lista.Contains(Convert.ToString(row["CLIENTE"]))) continue;

                lista.Add(Convert.ToString(row["CLIENTE"]));

                object aux = LocatorHelper.Instance.ExecuteScalar("select contratoadm_id from contratoadm where contratoadm_descricao='" + Convert.ToString(row["CLIENTE"]).Replace("'","") + "'", null, null, pm);
                if (aux == null || aux == DBNull.Value) continue;

                NonQueryHelper.Instance.ExecuteNonQuery("update contratoadm set contratoadm_doc='" + Convert.ToString(row["CPF_CNPJ"]).Replace("-","").Replace(".","").Replace("/","").Trim() + "' where contratoadm_id=" + Convert.ToString(aux), pm);

                sb.Append("update contratoadm set contratoadm_doc='" + Convert.ToString(row["CPF_CNPJ"]).Replace("-", "").Replace(".", "").Replace("/", "").Trim() + "' where contratoadm_id=" + Convert.ToString(aux));
                sb.Append(";");
            }

            int j = 0;

            pm.CloseSingleCommandInstance();
            pm.Dispose();
        }

        public void ___atualizaSenhas()
        {
            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\senhas.accdb;Persist Security Info=False");
            DataTable dt = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [dados]", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();
            int i = 0;
            StringBuilder sb = new StringBuilder();
            List<string> lista = new List<string>();

            try
            {
                #region variaveis
                int ret = 0, para = 0; i = 0;
                object aux = null;
                #endregion

                foreach (DataRow row in dt.Rows)
                {
                    i++;

                    aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select contrato_id from contrato ",
                        " where contrato_numero='",
                        toString(row["NumeroCartao"]).Replace(".", "").Replace("'", "").Trim(), "'"), null, null, pm);

                    if (aux == null || aux == DBNull.Value)
                    {
                        //NonQueryHelper.Instance.ExecuteNonQuery(
                        //    string.Concat("insert into _log_nao_localizados values ('", toString(row["NumeroCartao"]).Replace(".", "").Replace("'", "").Trim(), "', '", toString(row["Nome"]).Replace(".", "").Replace("'", "").Trim(), "')"),
                        //    pm);

                        continue;
                    }


                    NonQueryHelper.Instance.ExecuteNonQuery(
                        string.Concat("update contrato set contrato_senha='", row["SenhaInicial"], "' where contrato_numero='", toString(row["NumeroCartao"]).Replace(".", "").Replace("'", "").Trim(), "'"), 
                        pm);
                }

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }
        }

        public void ___atualizaSenhas2()
        {

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();
            int i = 0;
            StringBuilder sb = new StringBuilder();
            List<string> lista = new List<string>();

            try
            {
                #region variaveis
                int ret = 0, para = 0; i = 0;
                object aux = null;
                #endregion

                DataTable dt = LocatorHelper.Instance.ExecuteQuery("select contrato_id from contrato (nolock) where len(contrato_senha) > 6", "result", pm).Tables[0];
                DataTable dtFonte = new DataTable();

                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(@"Server=localhost;database=dados_791;Trusted_Connection=True"))
                {
                    conn.Open();
                    System.Data.SqlClient.SqlDataAdapter adp = new System.Data.SqlClient.SqlDataAdapter("select contrato_id,contrato_senha from contrato",conn);
                    adp.Fill(dtFonte);
                    adp.Dispose();
                    conn.Close();
                }

                Contrato contrato = null;
                DataRow[] rows = null;
                foreach (DataRow row in dt.Rows)
                {
                    i++;

                    contrato = new Contrato();

                    //NonQueryHelper.Instance.ExecuteNonQuery(
                    //    string.Concat("update contrato set contrato_senha='", contrato.GerarSenha(), "' where contrato_id=", row[0]),
                    //    pm);

                    rows = dtFonte.Select("contrato_id=" + row["contrato_id"]);

                    if (rows != null && rows.Length == 1)
                    {
                        NonQueryHelper.Instance.ExecuteNonQuery(
                        string.Concat("update contrato set contrato_senha='", rows[0]["contrato_senha"], "' where contrato_id=", row[0]),
                        pm);
                    }
                    else
                    {
                        int droga = 0;
                    }
                }

            }
            catch
            {
                throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }
        }

        public void ___atualizaSaldo()
        {
            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\transacoes\transacoes.accdb;Persist Security Info=False");
            DataTable dt = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [dados] ", connection); //where NumeroCartao='.6370870008353311'
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();
            int i = 0;
            StringBuilder sb = new StringBuilder();
            List<string> lista = new List<string>();

            try
            {
                #region variaveis
                int ret = 0, para = 0; i = 0;
                object aux = null;
                #endregion

                //List<string> numeros2 = new List<string>();
                //foreach (DataRow row in dt.Rows)
                //{
                //    i++;

                //    if (numeros2.Contains(toString(row["NumeroCartao"]))) continue;

                //    aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select contrato_id from contrato ",
                //        " where contrato_numero='",
                //        toString(row["NumeroCartao"]).Replace(".", "").Replace("'", "").Trim(), "'"), null, null, pm);

                //    if (aux == null || aux == DBNull.Value)
                //    {
                //        NonQueryHelper.Instance.ExecuteNonQuery(
                //            string.Concat("insert into _log_nao_localizados2 values ('", toString(row["NumeroCartao"]).Replace(".", "").Replace("'", "").Trim(), "', '')"),
                //            pm);
                //    }

                //    numeros2.Add(toString(row["NumeroCartao"]));
                //}

                //pm.Commit();
                //return;

                List<string> numeros = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    if (numeros.Contains(toString(row["NumeroCartao"]).Replace(".", "").Replace("'", "").Trim()))
                        continue;

                    numeros.Add(toString(row["NumeroCartao"]).Replace(".", "").Replace("'", "").Trim());
                }

                System.Globalization.CultureInfo cinfo= new System.Globalization.CultureInfo("pt-Br");
                DataRow[] rows = null; string qry = "";
                foreach (string numero in numeros)
                {
                    rows = dt.Select(string.Concat("NumeroCartao='.", numero, "'"), "DataTransacao ASC");

                    aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select contrato_id from contrato ",
                        " where contrato_numero='",
                        numero, "'"), null, null, pm);

                    if (aux == null) continue;

                    //SALDO
                    qry = string.Concat("insert into contrato_saldo (saldo_atual,saldo_creditoTotal,saldo_dataUltimaMovimentacao,saldo_debitoTotal,saldo_contratoId) values ('",
                            toString(rows[rows.Length - 1]["SaldoAposTransacao"]).Replace(".", "").Replace(",", "."), "',",
                            "0,",
                            "'", toDateTime(rows[rows.Length - 1]["DataTransacao"], cinfo).ToString("yyyy-MM-dd HH:mm:ss"), "',",
                            "0,",
                            aux, ")");

                    NonQueryHelper.Instance.ExecuteNonQuery(qry, pm);

                    //Historico
                    foreach (DataRow row in rows)
                    {
                        qry = string.Concat("insert into contrato_saldo_historico (saldohist_contratoId,saldohist_descricao,saldohist_data) values (",
                            aux, ",",
                            "'", toString(row["TipoTransacao"]).Replace(".", "").Replace(",", "."), " (", toString(row["NomePrestador"]).Replace("'",""), "): R$", toDecimal(row["ValorTransacao"],cinfo).ToString("N2"), "',",
                            "'", toDateTime(row["DataTransacao"], cinfo).ToString("yyyy-MM-dd HH:mm:ss"), "')");

                        NonQueryHelper.Instance.ExecuteNonQuery(qry, pm);
                    }
                }

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }
        }

        public void ___atualizaSaldo_FINAL()
        {
            string connAccess = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\transacoes\transacoes_final.accdb;Persist Security Info=False");
            DataTable dt = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(connAccess))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [dados] ", connection); //where NumeroCartao='.6370870008353311'
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                #region variaveis
                int i = 0;
                string qry = "";
                object aux = null, saldoId = null;
                #endregion

                foreach (DataRow row in dt.Rows)
                {
                    i++;
                    aux = null; saldoId = null;

                    aux = LocatorHelper.Instance.ExecuteScalar(string.Concat("select contrato_id from contrato ",
                        " where contrato_numero='",
                        toString(row["NumeroCartao"]).Replace(".", "").Replace(",", "."), "'"), null, null, pm);

                    if (aux == null) continue;

                    saldoId = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select saldo_id from contrato_saldo where saldo_contratoId=", aux),
                        null, null, pm);

                    if (saldoId != null)
                    {
                        qry = string.Concat("update contrato_saldo set saldo_atual='",
                            toString(row["saldo"]).Replace(".", "").Replace(",", "."), "',",
                            "saldo_creditoTotal='0', saldo_dataUltimaMovimentacao=getdate(),",
                            "saldo_debitoTotal='0' where saldo_id=", saldoId);
                    }
                    else
                    {
                        qry = string.Concat("insert into contrato_saldo (saldo_atual,saldo_creditoTotal,saldo_dataUltimaMovimentacao,saldo_debitoTotal,saldo_contratoId) values ('",
                            toString(row["saldo"]).Replace(".", "").Replace(",", "."), "',",
                            "0,",
                            "'", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "',",
                            "0,",
                            aux, ")");
                    }

                    NonQueryHelper.Instance.ExecuteNonQuery(qry, pm);
                }

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }
        }
    }
}