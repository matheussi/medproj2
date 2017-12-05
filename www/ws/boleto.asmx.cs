namespace MedProj.www.ws
{
    using System;
    using System.Web;
    using System.Data;
    using System.Text;
    using System.Linq;
    using System.Web.Services;
    using System.Globalization;
    using System.Configuration;
    using System.Collections.Generic;

    using PdfSharp;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.IO;
    using PdfSharp.Drawing;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity;
    using System.IO;
    using PdfSharp.Drawing.Layout;
    using System.Collections;
    using NHibernate;
    using NHibernate.Linq;
    using FluentNHibernate.Cfg.Db;
    using FluentNHibernate.Cfg;
    using NHibernate.Dialect;
    using FluentNHibernate.Conventions.Helpers;

    //using iugu01;

    /// <summary>
    /// 
    /// </summary>
    [WebService(Namespace = "http://sispag.clubeazul.org.br/servicos/", Description = "Serviço Web que expõe métodos para operações com cobrança e boletos.")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class boleto : System.Web.Services.WebService
    {
        string TokenGuid
        {
            get { return "233478a4-d2a3-4514-b9c2-6c70f5c2e63d"; }
        }

        [WebMethod(Description = "Dados os parâmetros, altera a cobrança e cria uma pendência CNAB.")]
        public string AlterarBoleto(string token, string idCobranca, string qtdVidas, string novoVencimento)
        {
            #region validacoes 

            if (token != this.TokenGuid) return retorno("erro", "Erro de autenticacao");

            int _vidas = 0;

            if (!string.IsNullOrEmpty(qtdVidas))
            {
                Int32.TryParse(qtdVidas, out _vidas);
                if (_vidas == 0)
                {
                    return retorno("erro", "Quantidade de vidas informada inválida: " + qtdVidas);
                }
            }

            DateTime dataVencimento = DateTime.MinValue;

            if (!string.IsNullOrEmpty(novoVencimento))
            {
                string[] arr = novoVencimento.Split('/');
                if (arr.Length != 3)
                {
                    return retorno("erro", "Data de vencimento não estava em um formato válido: dd/MM/yyyy");
                }

                try
                {
                    dataVencimento = new DateTime(
                        Convert.ToInt32(arr[2]), Convert.ToInt32(arr[1]), Convert.ToInt32(arr[0]), 23, 59, 59, 900);
                }
                catch
                {
                    return retorno("erro", "Data de vencimento não estava em um formato válido: dd/MM/yyyy");
                }
            }

            #endregion

            DateTime dataVencimentoAntiga = DateTime.MinValue;
            decimal valorAntigo = 0;
            bool alteracaoDeValor = false, alteracaoDeVencimento = false;

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    Cobranca cobranca = new Cobranca(idCobranca);
                    pm.Load(cobranca);
                    if(cobranca.Cancelada)
                    {
                        pm.Rollback();
                        return retorno("erro", "Cobrança " + idCobranca + " está cancelada");
                    }

                    Contrato contrato = new Contrato(cobranca.PropostaID);
                    pm.Load(contrato);
                    if(contrato.Inativo || contrato.Cancelado)
                    {
                        pm.Rollback();
                        return retorno("erro", "Contrato está cancelado");
                    }

                    valorAntigo = cobranca.Valor;
                    if (cobranca.QtdVidas != _vidas && _vidas != 0) alteracaoDeValor = true;

                    if (dataVencimento != DateTime.MinValue)
                    {
                        if (cobranca.DataVencimento.Day   != dataVencimento.Day ||
                            cobranca.DataVencimento.Month != dataVencimento.Month ||
                            cobranca.DataVencimento.Year  != dataVencimento.Year) alteracaoDeVencimento = true;
                    }

                    dataVencimentoAntiga = cobranca.DataVencimento;
                    if (alteracaoDeVencimento) cobranca.DataVencimento = dataVencimento;

                    if (alteracaoDeValor)
                    {
                        cobranca.QtdVidas = _vidas;

                        string erro = "";
                        decimal valorPorVida = this.calulaValorPorVida(pm, contrato, novoVencimento, out erro);
                        if (valorPorVida == decimal.Zero)
                        {
                            pm.Rollback();
                            return retorno("erro", erro);
                        }

                        cobranca.Valor = Convert.ToDecimal(_vidas) * valorPorVida;

                        decimal acrescimoDeContrato = calculaAcrescimoDeContrato(pm, contrato, cobranca);
                        cobranca.Valor += acrescimoDeContrato;

                        this.calculaConfiguracaoValorAdicional(pm, contrato, ref cobranca);
                    }

                    if (alteracaoDeValor || alteracaoDeVencimento)
                    {
                        pm.Save(cobranca);

                        CobrancaLog.CobrancaCriadaLog log = new CobrancaLog.CobrancaCriadaLog();
                        log.CobrancaValor = 0;
                        log.PropostaID = contrato.ID;
                        log.DataEnviada = novoVencimento;
                        log.Vidas = _vidas;
                        log.Msg = "Alteracao de valor ou vencto";
                        log.Origem = (int)CobrancaLog.Fonte.WebService;
                        log.CobrancaValor = valorAntigo;
                        log.CobrancaVencimento = dataVencimentoAntiga;
                        pm.Save(log);

                        //se ja estava registrada, então tem que gerar movimentacao de alteracao
                        if (cobranca.ArquivoIDUltimoEnvio != null)
                        {
                            CobrancaLog.PendenciaCNAB pendencia = CobrancaLog.PendenciaCNAB.CarregaPendente(cobranca.ID, pm);
                            if (pendencia == null) pendencia = new CobrancaLog.PendenciaCNAB();

                            pendencia.AlteracaoValor = alteracaoDeValor;
                            pendencia.AlteracaoVencimento = alteracaoDeVencimento;
                            pendencia.CobrancaID = cobranca.ID;
                            pendencia.Processado = false;

                            pm.Save(pendencia);
                        }
                    }

                    pm.Commit();
                }
                catch(Exception ex)
                {
                    pm.Rollback();

                    if (ex.InnerException == null)
                        return retorno("erro", ex.Message);
                    else
                        return retorno("erro", ex.InnerException.Message);
                }
                finally
                {
                    pm.Dispose();
                }
            }

            return retorno("ok", "Alterado com sucesso");
        }

        decimal calulaValorPorVida(PersistenceManager pm, Contrato contrato, string vencimento, out string erro)
        {
            erro = "";

            object aux = LocatorHelper.Instance.ExecuteScalar(
                string.Concat("select tabela_id from tabela_cobertura where tabela_contratoAdmId=", contrato.ContratoADMID),
                null, null, pm);

            if (aux == null || aux == DBNull.Value || Convert.ToString(aux).Trim() == "")
            {
                erro = "Não foi possível localizar uma tabela de cobertura para o contrato adm " + Convert.ToString(contrato.ContratoADMID);
                return decimal.Zero;
            }

            var data = LocatorHelper.Instance.ExecuteQuery(
                string.Concat("select top 1 * from tabela_cobertura_vigencia where vigcobertura_inicio <= '", DateTime.Now.ToString("yyyy-MM-dd"), "' and vigcobertura_tabelaId=", aux, " order by vigcobertura_inicio desc"), //string.Concat("select top 1 * from tabela_cobertura_vigencia where vigcobertura_inicio <= '", arr[2], "-", arr[1], "-", arr[0], "' and vigcobertura_tabelaId=", aux, " order by vigcobertura_inicio desc"),
                "result", pm).Tables[0];

            if (data.Rows.Count < 1)
            {
                data.Dispose();
                erro = "Nenhuma vigência localizada para a cobertura " + aux + " no vencimento " + vencimento;
                return decimal.Zero;
            }

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            decimal valorPorVida = Convert.ToDecimal(data.Rows[0]["vigcobertura_valor"], cinfo);
            data.Dispose();

            return valorPorVida;
        }
        decimal calculaAcrescimoDeContrato(PersistenceManager pm, Contrato contrato, Cobranca cobranca)
        {
            decimal acrescimoOuDesconto = 0;

            if (cobranca != null && cobranca.AcrescimoDeContrato != null && cobranca.AcrescimoDeContrato != decimal.Zero)
            {
                if (cobranca.AcrescimoDeContratoTipo == 1)
                    acrescimoOuDesconto = cobranca.AcrescimoDeContrato;
                else if (cobranca.AcrescimoDeContratoTipo == 2)
                    acrescimoOuDesconto = (-1 * cobranca.AcrescimoDeContrato);
            }
            else
            {
                if (contrato.DescontoAcrescimoTipo != 0)
                {
                    if (contrato.DescontoAcrescimoData == DateTime.MinValue ||
                        contrato.DescontoAcrescimoData >= DateTime.Now)
                    {
                        if (contrato.DescontoAcrescimoTipo == 1)
                            acrescimoOuDesconto = contrato.DescontoAcrescimoValor;
                        else
                            acrescimoOuDesconto = (-1 * contrato.DescontoAcrescimoValor);
                    }

                    if (contrato.DescontoAcrescimoData == DateTime.MinValue)
                    {
                        contrato.DescontoAcrescimoTipo = 0;
                        pm.Save(contrato);//////////////////////////////////
                    }
                }
            }

            return acrescimoOuDesconto;
        }
        public void calculaConfiguracaoValorAdicional(PersistenceManager pm, Contrato contrato, ref Cobranca cobranca)
        {
            DataTable dt = LC.Web.PadraoSeguros.Facade.CobrancaFacade.Instancia.CarregaAdicionais(contrato, pm);
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            if (dt != null)
            {
                if (toDecimal(dt.Rows[0]["Valor"], cinfo) > decimal.Zero)
                {
                    cobranca.AdicionalID = toInt(dt.Rows[0]["ID"]);
                    cobranca.Valor += toDecimal(dt.Rows[0]["Valor"], cinfo);
                    cobranca.InstrucaoAdicional = string.Concat(toString(dt.Rows[0]["Texto"]), " ", toDecimal(dt.Rows[0]["Valor"], cinfo).ToString("C"));
                }

                dt.Dispose();
            }
        }

        /***************************************************************************************************************************/

        [WebMethod(Description = "Dados os parâmetros, cria a cobrança e retorna a url para exibição do boleto - Versão 02.")]
        public string GerarBoletoV2(string token, string idContrato, string qtdVidas, string vencimento, string competencia)
        {
            if (token != this.TokenGuid) return retorno("erro", "Erro de autenticacao");

            string[] arr = vencimento.Split('/');
            if (arr.Length != 3)
            {
                return retorno("erro", "Data de vencimento não estava em um formato válido: dd/MM/yyyy");
            }

            if (string.IsNullOrEmpty(competencia) || competencia.Length != 7)
            {
                return retorno("erro", "Competencia não estava em um formato válido: MM/yyyy");
            }

            try
            {
                DateTime dtComp = new DateTime(toInt(competencia.Split('/')[1]), toInt(competencia.Split('/')[0]), 1);
            }
            catch
            {
                return retorno("erro", "Competencia não estava em um formato válido: MM/yyyy");
            }
            
            DateTime dataVencimento = new DateTime(
                Convert.ToInt32(arr[2]), Convert.ToInt32(arr[1]), Convert.ToInt32(arr[0]), 23, 59, 59, 900);

            string boletoUrl = "", status = "", instrucoes = "";
            bool calculaJuro = false;

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    var contrato = new Contrato(idContrato);
                    pm.Load(contrato);

                    if (contrato.Cancelado || contrato.Inativo)
                    {
                        CobrancaLog.CobrancaCriadaLog logErr01 = new CobrancaLog.CobrancaCriadaLog();
                        logErr01.CobrancaValor = 0;
                        logErr01.PropostaID = idContrato;
                        logErr01.DataEnviada = vencimento;
                        logErr01.Vidas = toInt(qtdVidas);
                        logErr01.Msg = "Contrato inativo: " + contrato.ID;
                        logErr01.Origem = (int)CobrancaLog.Fonte.WebService;
                        pm.Save(logErr01);

                        pm.Commit();
                        return retorno("erro", "Contrato inativo: " + contrato.ID);
                    }

                    #region Verifica se o ja há uma cobrança paga para a competencia

                    object aux = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select cobranca_id from cobranca where cobranca_pago=1 and cobranca_propostaId=", contrato.ID, " and (cobranca_cancelada is null or cobranca_cancelada=0) and cobranca_competencia='", competencia, "'"),
                        null, null, pm);

                    if (aux != null && aux != DBNull.Value && Convert.ToString(aux).Trim() != "")
                    {
                        CobrancaLog.CobrancaCriadaLog logErr01 = new CobrancaLog.CobrancaCriadaLog();
                        logErr01.CobrancaValor = 0;
                        logErr01.PropostaID = idContrato;
                        logErr01.DataEnviada = vencimento;
                        logErr01.Vidas = toInt(qtdVidas);
                        logErr01.Msg = "Competencia ja liquidada: " + competencia;
                        logErr01.Origem = (int)CobrancaLog.Fonte.WebService;
                        pm.Save(logErr01);

                        pm.Commit();
                        return retorno("erro", "Competencia ja liquidada: " + competencia);
                    }
                    #endregion

                    string erro = "";
                    decimal valorPorVida = this.calulaValorPorVida(pm, contrato, vencimento, out erro);
                    if (valorPorVida == 0)
                    {
                        pm.Rollback();
                        return retorno("erro", erro);
                    }

                    int diaVenctoProjeto = this.toInt(LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select contratoADM_DTVC from contratoadm where contratoadm_id=", contrato.ContratoADMID),
                        null, null, pm));

                    #region IUGU - Customer =====================================

                    var titular = ContratoBeneficiario.CarregarTitular(contrato.ID, pm);

                    if (string.IsNullOrEmpty(contrato.IuguCustumerId) && contrato.IuguHabilitado)
                    {
                        //TODO: Denis COMENTAr linha abaixo
                        //titular.BeneficiarioEmail = "matheussi@gmail.com";

                        //using (iugu_srv_test.iugu_interop proxy = new iugu_srv_test.iugu_interop())
                        using (iugu_srv.iugu_interop proxy = new iugu_srv.iugu_interop())
                        {
                            contrato.IuguCustumerId = proxy.ObterCustomer(
                                null, Convert.ToString(contrato.ID), titular.BeneficiarioEmail, titular.BeneficiarioNome, token);

                            contrato.AtualizarIuguCustomerId(pm);
                        }
                    }

                    #endregion ====================================================

                    #region IUGU - Subscription =====================================

                    if (string.IsNullOrEmpty(contrato.IuguSubscriptionId) && contrato.IuguHabilitado)
                    {
                        //using (iugu_srv_test.iugu_interop proxy = new iugu_srv_test.iugu_interop())
                        using (iugu_srv.iugu_interop proxy = new iugu_srv.iugu_interop())
                        {
                            string msg = "";
                            bool ok = proxy.ObterSubscription(contrato.IuguCustumerId, token, out msg);

                            if (ok)
                            {
                                contrato.IuguSubscriptionId = msg;
                                contrato.AtualizarIuguSubscriptonId(pm);
                            }
                            else
                            {
                                pm.Rollback();
                                return retorno("erro", msg);
                            }
                        }
                    }

                    #endregion ====================================================

                    //TODO: denis, voltar o que estava antes?
                    var cobrancas = Cobranca.CarregarTodas(idContrato, true, pm); //var cobrancas = Cobranca.CarregarTodas(idContrato, false, pm);
                    int ultimaParcela = 1;

                    if (cobrancas != null && cobrancas.Count > 0) ultimaParcela = cobrancas.Max(c => c.Parcela) + 1;

                    decimal acrescimoOuDesconto = 0;
                    int acrescimoOuDescontoTipo = -1;

                    #region calcula acréscimos ou descontos de contrato

                    if (contrato.DescontoAcrescimoTipo != 0)
                    {
                        if (contrato.DescontoAcrescimoData == DateTime.MinValue ||
                            contrato.DescontoAcrescimoData >= DateTime.Now)
                        {
                            if (contrato.DescontoAcrescimoTipo == 1)
                            {
                                acrescimoOuDescontoTipo = 1;
                                acrescimoOuDesconto = contrato.DescontoAcrescimoValor;
                            }
                            else
                            {
                                acrescimoOuDescontoTipo = 2;
                                acrescimoOuDesconto = (-1 * contrato.DescontoAcrescimoValor);
                            }
                        }

                        if (contrato.DescontoAcrescimoData == DateTime.MinValue)
                        {
                            contrato.DescontoAcrescimoTipo = 0;
                            pm.Save(contrato);//////////////////////////////////
                        }
                    }
                    #endregion


                    //salva a cobranca
                    Cobranca cobranca = new Cobranca();
                    cobranca.Parcela = ultimaParcela;
                    cobranca.DataVencimento = dataVencimento;
                    if (acrescimoOuDesconto > decimal.Zero) cobranca.AcrescimoDeContrato = acrescimoOuDesconto;
                    if (acrescimoOuDescontoTipo > -1) cobranca.AcrescimoDeContratoTipo = acrescimoOuDescontoTipo;

                    if (cobranca.Parcela <= 1)
                    {
                        #region comentado
                        //aux = LocatorHelper.Instance.ExecuteScalar(
                        //    string.Concat("select estipulante_dataVencimento from estipulante where estipulante_id=", contrato.EstipulanteID),
                        //    null, null, pm);

                        //if (aux != null && aux != DBNull.Value)
                        //{
                        //    if (dataVencimento.Month == DateTime.Now.Month &&
                        //        dataVencimento.Day > Convert.ToInt32(aux))
                        //    {
                        //        cobranca.DataVencimento = DateTime.Now.AddDays(2);
                        //        cobranca.DataVencimento = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, cobranca.DataVencimento.Day, 23, 59, 59, 990);
                        //    }
                        //}
                        //else
                        //{
                        //    if (dataVencimento < DateTime.Now)
                        //    {
                        //        cobranca.DataVencimento = DateTime.Now.AddDays(2);
                        //        cobranca.DataVencimento = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, cobranca.DataVencimento.Day, 23, 59, 59, 990);
                        //    }
                        //}
                        #endregion

                        cobranca.Parcela = 1;

                        cobranca.DataVencimento = calcula1oVencimento(dataVencimento, diaVenctoProjeto);

                        #region comentado 2 
                        //regras dos dias 28,29,30 e 31
                        #endregion
                    }
                    else
                    {
                        string qry = "";
                        //Regra 2: não pode gerar uma cobrança para um mês em que ja há cobrança
                        try
                        {
                            qry = string.Concat(
                                "select cobranca_id from ",
                                "   cobranca where ",
                                "       (cobranca_cancelada is null or cobranca_cancelada=0) and cobranca_propostaid=", contrato.ID,
                                "       and cobranca_competencia='", competencia, "'");

                            aux = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
                            if (aux != null && aux != DBNull.Value && Convert.ToString(aux).Trim() != "")
                            {
                                    CobrancaLog.CobrancaCriadaLog logErr01 = new CobrancaLog.CobrancaCriadaLog();
                                    logErr01.CobrancaValor = 0;
                                    logErr01.PropostaID = idContrato;
                                    logErr01.DataEnviada = vencimento;
                                    logErr01.Vidas = toInt(qtdVidas);
                                    logErr01.Msg = "Ja existe uma cobranca gerada para a competencia " + competencia;
                                    logErr01.Origem = (int)CobrancaLog.Fonte.WebService;
                                    pm.Save(logErr01);

                                    pm.Commit();

                                    return retorno("erro", "Ja existe uma cobranca gerada para a competencia " + competencia);
                            }
                        }
                        catch (Exception ex)
                        {
                            pm.Rollback();

                            return retorno("erro", ex.Message);
                        }

                        #region Regra 1: não pode gerar uma cobrança se não houver cobrança gerada no mês anterior
                        try
                        {
                            DateTime refe = new DateTime(
                                toInt(competencia.Split('/')[1]), toInt(competencia.Split('/')[0]), 1).AddMonths(-1);

                            qry = string.Concat(
                                "select cobranca_id from ",
                                "   cobranca where ",
                                "       cobranca_propostaid=", contrato.ID, //cobranca_cancelada=0 and 
                                "       and cobranca_competencia='", refe.ToString("MM/yyyy"), "'"); //"       and year(cobranca_dataVencimento)=", refe.Year);

                            aux = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
                            if ((aux == null || aux == DBNull.Value || Convert.ToString(aux).Trim() == "") && DateTime.Now > new DateTime(2017, 12, 1))
                            {
                                CobrancaLog.CobrancaCriadaLog logErr01 = new CobrancaLog.CobrancaCriadaLog();
                                logErr01.CobrancaValor = 0;
                                logErr01.PropostaID = idContrato;
                                logErr01.DataEnviada = vencimento;
                                logErr01.Vidas = toInt(qtdVidas);
                                logErr01.Msg = "Sem cobranca na comp. anterior: " + refe.ToString("MM/yyyy");
                                logErr01.Origem = (int)CobrancaLog.Fonte.WebService;
                                pm.Save(logErr01);

                                pm.Commit();

                                return retorno("erro", "Nao identificamos cobranca gerada na competencia anterior: " + refe.ToString("MM/yyyy"));
                            }
                        }
                        catch (Exception ex)
                        {
                            pm.Rollback();

                            return retorno("erro", ex.Message);
                        }
                        #endregion

                        // Parcelas 2 em diante: só podem ser geradas entre o dia 1 do mes de vencimento e 
                        // 15 dias após a data de vencimento

                        DateTime vencimentoPROJETO = new DateTime(dataVencimento.Year, dataVencimento.Month, diaVenctoProjeto, 23, 59, 59, 995);

                        // Intervarlo permitido para geração de cobranças
                        DateTime inicioPeriodo = new DateTime(dataVencimento.Year, dataVencimento.Month, 1);
                        DateTime fimPeriodo = new DateTime(vencimentoPROJETO.AddDays(15).Year, vencimentoPROJETO.AddDays(15).Month, vencimentoPROJETO.AddDays(15).Day, 23, 59, 59, 990); //new DateTime(dataVencimento.AddDays(15).Year, dataVencimento.AddDays(15).Month, dataVencimento.AddDays(15).Day, 23, 59, 59, 990);

                        if (DateTime.Now < inicioPeriodo || DateTime.Now > fimPeriodo)
                        {
                            //Emissão fora do periodo permitido
                            pm.Rollback();
                            return retorno("erro", "Emissão fora do periodo permitido que é de " + inicioPeriodo.ToString("dd/MM/yyyy") + " a " + fimPeriodo.ToString("dd/MM/yyyy"));
                        }

                        // Se a cobrança for emitida após o vencimento original, mas dentro do período permitido 
                        if (DateTime.Now.Day > vencimentoPROJETO.Day && DateTime.Now.Day <= fimPeriodo.Day) //if (DateTime.Now.Day > dataVencimento.Day && DateTime.Now.Day <= fimPeriodo.Day)
                        {
                            DateTime novoVencimento = new DateTime(
                                DateTime.Now.AddDays(2).Year,
                                DateTime.Now.AddDays(2).Month,
                                DateTime.Now.AddDays(2).Day, 23, 59, 59, 900);

                            cobranca.DataVencimento = novoVencimento;

                            instrucoes = "1";
                            calculaJuro = true;
                        }
                    }

                    //Devido à margem para registro no banco ...
                    DateTime validadePagto = LC.Web.PadraoSeguros.Facade.CobrancaFacade.Instancia.calculaValidadeBoleto(cobranca.DataCriacao);
                    if (cobranca.DataVencimento < validadePagto)
                    {
                        cobranca.DataVencimento = new DateTime(
                            validadePagto.AddDays(1).Year,
                            validadePagto.AddDays(1).Month,
                            validadePagto.AddDays(1).Day, 23, 59, 59, 900);
                    }

                    cobranca.Valor = (valorPorVida * Convert.ToDecimal(qtdVidas));

                    if (calculaJuro)
                    {
                        cobranca.CalculaJurosMulta(dataVencimento);
                    }

                    cobranca.Valor += acrescimoOuDesconto;

                    cobranca.Tipo = Convert.ToInt32(Cobranca.eTipo.Normal);
                    cobranca.CobrancaRefID = null;
                    cobranca.DataPgto = DateTime.MinValue;
                    cobranca.ValorPgto = Decimal.Zero;
                    cobranca.Pago = false;
                    cobranca.PropostaID = idContrato;
                    cobranca.Cancelada = false;
                    cobranca.QtdVidas = Convert.ToInt32(qtdVidas);
                    cobranca.Competencia = competencia;

                    #region Verifica se tem configuração de adicional para o boleto

                    if (contrato.IuguHabilitado == false) //somente para quem não é iugu
                    {
                        DataTable dt = LC.Web.PadraoSeguros.Facade.CobrancaFacade.Instancia.CarregaAdicionais(contrato, pm);
                        System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                        if (dt != null)
                        {
                            if (toDecimal(dt.Rows[0]["Valor"], cinfo) > decimal.Zero)
                            {
                                cobranca.AdicionalID = toInt(dt.Rows[0]["ID"]);
                                cobranca.Valor += toDecimal(dt.Rows[0]["Valor"], cinfo);
                                cobranca.InstrucaoAdicional = string.Concat(toString(dt.Rows[0]["Texto"]), " ", toDecimal(dt.Rows[0]["Valor"], cinfo).ToString("C"));
                            }
                        }
                    }

                    #endregion

                    pm.Save(cobranca);

                    #region IUGU - Boleto =========================================

                    if (contrato.IuguHabilitado && !string.IsNullOrEmpty(contrato.IuguCustumerId))
                    {
                        var ends = Endereco.CarregarPorDono(titular.BeneficiarioID, Endereco.TipoDono.Beneficiario, pm);

                        if (ends == null || ends.Count == 0)
                        {
                            pm.Rollback();
                            return retorno("erro", "Não foi possível localizar o endereço do beneficiário");
                        }

                        iugu_srv.PagadorVO vo = new iugu_srv.PagadorVO();
                        vo.bairro = ends[0].Bairro;
                        vo.cep = ends[0].CEP;
                        vo.cidade = ends[0].Cidade;
                        vo.cpfOuCnpj = titular.BeneficiarioCPF;
                        vo.email = titular.BeneficiarioEmail;
                        vo.estado = ends[0].UF;
                        vo.nome = titular.BeneficiarioNome;
                        vo.numero = ends[0].Numero;
                        vo.pais = "Brasil";
                        vo.rua = ends[0].Logradouro;

                        #region IUGU - Itens ====================================================================

                        //string[][] itens = null; // new string[][] { new string[] { "Clube Azul", cobranca.Valor.ToString("N2").Replace(".", "").Replace(",", "") } };

                        //if (contrato.IuguHabilitado)
                        //{
                        //    var itensProduto = Produto.CarregarItensVigentes(contrato.ContratoADMID, pm);
                        //    if (itensProduto != null)
                        //    {
                        //        itens = new string[1 + itensProduto.Count][];
                        //        itens[0] = new string[] { "Clube Azul", cobranca.Valor.ToString("N2").Replace(".", "").Replace(",", "") };

                        //        for (int k = 0; k < itensProduto.Count; k++) //foreach (var itemProd in itensProduto)
                        //        {
                        //            itens[k + 1] = new string[] { itensProduto[k].Nome, itensProduto[k].Valor.ToString("N2").Replace(".", "").Replace(",", "") };

                        //            cobranca.Valor += itensProduto[k].Valor;
                        //        }

                        //        ProdutoITEM_Cobranca.SalvarRelacionamento(cobranca.ID, itensProduto, pm);
                        //    }
                        //    else
                        //    {
                        //        itens = new string[][] { new string[] { "Clube Azul", cobranca.Valor.ToString("N2").Replace(".", "").Replace(",", "") } };
                        //    }
                        //}
                        #endregion

                        #region IUGU - Itens ====================================================================

                        string[][] itens = null; 

                        if (contrato.IuguHabilitado)
                        {
                            var itensProduto = Produto.CarregarItensVigentes(contrato.ContratoADMID, pm);
                            if (itensProduto != null)
                            {
                                for (int k = 0; k < itensProduto.Count; k++) //foreach (var itemProd in itensProduto)
                                {
                                    cobranca.Valor += (itensProduto[k].Valor * Convert.ToDecimal(cobranca.QtdVidas));
                                }

                                ProdutoITEM_Cobranca.SalvarRelacionamento(cobranca.ID, itensProduto, cobranca.QtdVidas, pm); //TODO:gravar qtd de vidas tb
                            }

                            //itens = new string[][] 
                            //{
                            //    new string[] { "Clube Azul", cobranca.Valor.ToString("N2").Replace(".", "").Replace(",", "") } 
                            //};
                            itens = new string[][] 
                            {
                                new string[] { "Clube Azul", cobranca.Valor.ToString("N2").Replace(".", "").Replace(",", "") } ,
                                new string[] { "Custo de cobrança", System.Configuration.ConfigurationManager.AppSettings["iugu_taxa"] } 
                            };
                        }
                        #endregion

                        //using (iugu_srv_test.iugu_interop proxy = new iugu_srv_test.iugu_interop())
                        using (iugu_srv.iugu_interop proxy = new iugu_srv.iugu_interop())
                        {
                            string msg = "";
                            string boletoIdCrypto = new Util.Crypto.SecureQueryString().Encrypt(Convert.ToString(cobranca.ID));
                            bool iuguOk = proxy.NovoBoletoAsync(contrato.IuguCustumerId, contrato.IuguSubscriptionId, cobranca.DataVencimento, itens, vo, boletoIdCrypto, out msg);

                            if (iuguOk)
                            {
                                cobranca.Iugu_Id = msg.Split(new string[] { "___" }, StringSplitOptions.None)[1];
                                cobranca.Iugu_Url = msg.Split(new string[] { "___" }, StringSplitOptions.None)[0];
                                cobranca.ArquivoIDUltimoEnvio = -10;
                                pm.Save(cobranca);
                            }
                            else
                            {
                                pm.Rollback();
                                return retorno("erro", msg);
                            }
                        }
                    }
                    #endregion

                    //atualiza a qtd de vidas na proposta
                    string sql = string.Concat("update contrato set contrato_qtdVidas=", qtdVidas, " where contrato_id=", idContrato);
                    NonQueryHelper.Instance.ExecuteNonQuery(sql, pm);

                    ContratoBeneficiario cb = ContratoBeneficiario.CarregarTitular(idContrato, pm);

                    CobrancaLog.CobrancaCriadaLog log = new CobrancaLog.CobrancaCriadaLog();
                    log.CobrancaID = cobranca.ID;
                    log.CobrancaValor = cobranca.Valor;
                    log.PropostaID = idContrato;
                    log.CobrancaVencimento = cobranca.DataVencimento;
                    log.DataEnviada = vencimento;
                    log.Vidas = toInt(qtdVidas);
                    log.Origem = (int)CobrancaLog.Fonte.WebService;

                    try
                    {
                        pm.Save(log);
                    }
                    catch
                    {
                    }

                    pm.Commit();

                    if (contrato.IuguHabilitado && !string.IsNullOrEmpty(cobranca.Iugu_Url))
                    {
                        status = "ok";
                        boletoUrl = cobranca.Iugu_Url;
                    }
                    else
                        boletoUrl = this.BoletoURL(cobranca, cb, out status, instrucoes);
                }
                catch (Exception ex)
                {
                    pm.Rollback();
                    return retorno("erro", ex.Message);
                }

                

                return retorno(status, boletoUrl);
            }
        }


        /***************************************************************************************************************************/

        [WebMethod(Description = "Dados os parâmetros, cria a cobrança e retorna a url para exibição do boleto.")]
        public string GerarBoleto(string token, string idContrato, string qtdVidas, string vencimento)
        {
            if (token != this.TokenGuid) return retorno("erro", "Erro de autorizacao");

            string[] arr = vencimento.Split('/');
            if (arr.Length != 3)
            {
                return retorno("erro", "Data de vencimento não estava em um formato válido: dd/MM/yyyy");
            }

            DateTime dataVencimento = new DateTime(
                Convert.ToInt32(arr[2]), Convert.ToInt32(arr[1]), Convert.ToInt32(arr[0]), 23, 59, 59, 900);

            string boletoUrl = "", status = "", instrucoes = "";
            bool calculaJuro = false;

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    var contrato = new Contrato(idContrato);
                    pm.Load(contrato);

                    #region Verifica se o ja há uma cobrança paga para o vencimento

                    object aux = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select cobranca_id from cobranca where cobranca_pago=1 and cobranca_propostaId=", contrato.ID, " and (cobranca_cancelada is null or cobranca_cancelada=0) and month(cobranca_dataCriacao)=", arr[1], " and year(cobranca_dataCriacao)=", arr[2]),
                        null, null, pm);

                    if (aux != null && aux != DBNull.Value && Convert.ToString(aux).Trim() != "")
                    {
                        CobrancaLog.CobrancaCriadaLog logErr01 = new CobrancaLog.CobrancaCriadaLog();
                        logErr01.CobrancaValor = 0;
                        logErr01.PropostaID = idContrato;
                        logErr01.DataEnviada = vencimento;
                        logErr01.Vidas = toInt(qtdVidas);
                        logErr01.Msg = "Vencimento ja pago";
                        logErr01.Origem = (int)CobrancaLog.Fonte.WebService;
                        pm.Save(logErr01);

                        pm.Commit();
                        return retorno("erro", "Vencimento ja pago.");
                    }
                    #endregion

                    #region comentado
                    //aux = LocatorHelper.Instance.ExecuteScalar(
                    //    string.Concat("select tabela_id from tabela_cobertura where tabela_contratoAdmId=", contrato.ContratoADMID),
                    //    null, null, pm);

                    //if (aux == null || aux == DBNull.Value || Convert.ToString(aux).Trim() == "")
                    //{
                    //    pm.Rollback();
                    //    return retorno("erro", "Não foi possível localizar uma tabela de cobertura para o contrato adm " + Convert.ToString(contrato.ContratoADMID));
                    //}

                    //var data = LocatorHelper.Instance.ExecuteQuery(
                    //    string.Concat("select top 1 * from tabela_cobertura_vigencia where vigcobertura_inicio <= '", DateTime.Now.ToString("yyyy-MM-dd"), "' and vigcobertura_tabelaId=", aux, " order by vigcobertura_inicio desc"), //string.Concat("select top 1 * from tabela_cobertura_vigencia where vigcobertura_inicio <= '", arr[2], "-", arr[1], "-", arr[0], "' and vigcobertura_tabelaId=", aux, " order by vigcobertura_inicio desc"),
                    //    "result", pm).Tables[0];

                    //if (data.Rows.Count < 1)
                    //{
                    //    data.Dispose();
                    //    pm.Rollback();
                    //    return retorno("erro", "Nenhuma vigência localizada para a cobertura " + aux + " no vencimento " + vencimento);
                    //}

                    //System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                    //decimal valorPorVida = Convert.ToDecimal(data.Rows[0]["vigcobertura_valor"], cinfo);
                    #endregion

                    string erro = "";
                    decimal valorPorVida = this.calulaValorPorVida(pm, contrato, vencimento, out erro);
                    if (valorPorVida == 0)
                    {
                        pm.Rollback();
                        return retorno("erro", erro);
                    }

                    int diaVenctoProjeto = this.toInt(LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select contratoADM_DTVC from contratoadm where contratoadm_id=", contrato.ContratoADMID),
                        null, null, pm));

                    //TODO: denis, voltar o que estava antes?
                    var cobrancas = Cobranca.CarregarTodas(idContrato, true, pm); //var cobrancas = Cobranca.CarregarTodas(idContrato, false, pm);
                    int ultimaParcela = 1;

                    if (cobrancas != null && cobrancas.Count > 0) ultimaParcela = cobrancas.Max(c => c.Parcela) + 1;

                    decimal acrescimoOuDesconto = 0;
                    int acrescimoOuDescontoTipo = -1;

                    #region calcula acréscimos ou descontos de contrato

                    if (contrato.DescontoAcrescimoTipo != 0)
                    {
                        if (contrato.DescontoAcrescimoData == DateTime.MinValue ||
                            contrato.DescontoAcrescimoData >= DateTime.Now)
                        {
                            if (contrato.DescontoAcrescimoTipo == 1)
                            {
                                acrescimoOuDescontoTipo = 1;
                                acrescimoOuDesconto = contrato.DescontoAcrescimoValor;
                            }
                            else
                            {
                                acrescimoOuDescontoTipo = 2;
                                acrescimoOuDesconto = (-1 * contrato.DescontoAcrescimoValor);
                            }
                        }

                        if (contrato.DescontoAcrescimoData == DateTime.MinValue)
                        {
                            contrato.DescontoAcrescimoTipo = 0;
                            pm.Save(contrato);//////////////////////////////////
                        }
                    }
                    #endregion


                    //salva a cobranca
                    Cobranca cobranca = new Cobranca();
                    cobranca.Parcela = ultimaParcela;
                    cobranca.DataVencimento = dataVencimento;
                    if (acrescimoOuDesconto > decimal.Zero) cobranca.AcrescimoDeContrato = acrescimoOuDesconto;
                    if (acrescimoOuDescontoTipo > -1) cobranca.AcrescimoDeContratoTipo = acrescimoOuDescontoTipo;

                    if (cobranca.Parcela <= 1)
                    {
                        #region comentado
                        //aux = LocatorHelper.Instance.ExecuteScalar(
                        //    string.Concat("select estipulante_dataVencimento from estipulante where estipulante_id=", contrato.EstipulanteID),
                        //    null, null, pm);

                        //if (aux != null && aux != DBNull.Value)
                        //{
                        //    if (dataVencimento.Month == DateTime.Now.Month &&
                        //        dataVencimento.Day > Convert.ToInt32(aux))
                        //    {
                        //        cobranca.DataVencimento = DateTime.Now.AddDays(2);
                        //        cobranca.DataVencimento = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, cobranca.DataVencimento.Day, 23, 59, 59, 990);
                        //    }
                        //}
                        //else
                        //{
                        //    if (dataVencimento < DateTime.Now)
                        //    {
                        //        cobranca.DataVencimento = DateTime.Now.AddDays(2);
                        //        cobranca.DataVencimento = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, cobranca.DataVencimento.Day, 23, 59, 59, 990);
                        //    }
                        //}
                        #endregion

                        cobranca.Parcela = 1;

                        cobranca.DataVencimento = calcula1oVencimento(dataVencimento, diaVenctoProjeto);

                        #region comentado 2

                        //int diaHoje   = DateTime.Now.Day;
                        //int diaUltimo = ultimoDiaDoMes(DateTime.Now);

                        //if (diaHoje == diaUltimo)
                        //{
                        //    if (diaHoje == 28 || diaHoje == 29)
                        //    {
                        //        DateTime novoVencimento = dataVencimento.AddMonths(1);
                        //        novoVencimento = new DateTime(novoVencimento.Year, novoVencimento.Month, 1, 23, 59, 59, 900);
                        //        cobranca.DataVencimento = novoVencimento;
                        //    }
                        //    else if (diaHoje == 30 || diaHoje == 31)
                        //    {
                        //        DateTime novoVencimento = dataVencimento.AddMonths(1);
                        //        novoVencimento = new DateTime(novoVencimento.Year, novoVencimento.Month, 2, 23, 59, 59, 900);
                        //        cobranca.DataVencimento = novoVencimento;
                        //    }
                        //}
                        //else
                        //{
                        //    if (diaVenctoProjeto == 0) //nao pôde ler a data de vencto do projeto
                        //    {
                        //        if (dataVencimento < DateTime.Now) //PRIMEIRA PARCELA - ANTES DO FIM DO     
                        //        {
                        //            cobranca.DataVencimento = DateTime.Now.AddDays(2);
                        //            cobranca.DataVencimento = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, cobranca.DataVencimento.Day, 23, 59, 59, 990);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (DateTime.Now.Day < diaVenctoProjeto)
                        //        {
                        //            cobranca.DataVencimento = new DateTime(
                        //                cobranca.DataVencimento.Year,
                        //                cobranca.DataVencimento.Month,
                        //                diaVenctoProjeto, 23, 59, 59, 990);
                        //        }
                        //        else
                        //        {
                        //            cobranca.DataVencimento = DateTime.Now.AddDays(2);
                        //            cobranca.DataVencimento = new DateTime(
                        //                cobranca.DataVencimento.Year, 
                        //                cobranca.DataVencimento.Month, 
                        //                cobranca.DataVencimento.Day, 23, 59, 59, 990);
                        //        }
                        //    }
                        //}
                        #endregion
                    }
                    else
                    {
                        string qry = "";
                        //Regra 2: não pode gerar uma cobrança para um mês em que ja há cobrança
                        try
                        {
                            qry = string.Concat(
                                "select cobranca_id from ",
                                "   cobranca where ",
                                //"       cobranca_cobrancaRefId is null and ",
                                "       cobranca_cancelada=0 and cobranca_propostaid=", contrato.ID,
                                "       and month(cobranca_dataVencimento)=", cobranca.DataVencimento.Month,
                                "       and year(cobranca_dataVencimento)=", cobranca.DataVencimento.Year);

                            aux = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
                            if (aux != null && aux != DBNull.Value && Convert.ToString(aux).Trim() != "")
                            {
                                //achou, agora precisa verificar a competencia, pois se for outra competencia, não pode bloquear
                                DateTime dataref = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, 1).AddMonths(-1);
                                qry = string.Concat(
                                "select cobranca_id from ",
                                "   cobranca where ",
                                "       cobranca_cancelada=0 and cobranca_propostaid=", contrato.ID,
                                "       and month(cobranca_dataVencimento)=", dataref.Month,
                                "       and year(cobranca_dataVencimento)=", dataref.Year);

                                aux = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
                                if (aux != null && aux != DBNull.Value && Convert.ToString(aux).Trim() != "")
                                {
                                    CobrancaLog.CobrancaCriadaLog logErr01 = new CobrancaLog.CobrancaCriadaLog();
                                    logErr01.CobrancaValor = 0;
                                    logErr01.PropostaID = idContrato;
                                    logErr01.DataEnviada = vencimento;
                                    logErr01.Vidas = toInt(qtdVidas);
                                    logErr01.Msg = "Ja existe uma cobranca gerada para o periodo";
                                    logErr01.Origem = (int)CobrancaLog.Fonte.WebService;
                                    pm.Save(logErr01);

                                    pm.Commit();

                                    return retorno("erro", "Ja existe uma cobranca gerada para o periodo");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            pm.Rollback();

                            return retorno("erro", ex.Message);
                        }

                        #region Regra 1: não pode gerar uma cobrança se não houver cobrança gerada no mês anterior
                        try
                        {
                            DateTime refe = dataVencimento.AddMonths(-1);
                            qry = string.Concat(
                                "select cobranca_id from ",
                                "   cobranca where ",
                                "       cobranca_propostaid=", contrato.ID, //cobranca_cancelada=0 and 
                                "       and month(cobranca_dataVencimento)=", refe.Month, //"       and month(cobranca_dataVencimento)=", refe.Month,
                                "       and year(cobranca_dataVencimento)=", refe.Year); //"       and year(cobranca_dataVencimento)=", refe.Year);

                            aux = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
                            if (aux == null || aux == DBNull.Value || Convert.ToString(aux).Trim() == "")
                            {
                                CobrancaLog.CobrancaCriadaLog logErr01 = new CobrancaLog.CobrancaCriadaLog();
                                logErr01.CobrancaValor = 0;
                                logErr01.PropostaID = idContrato;
                                logErr01.DataEnviada = vencimento;
                                logErr01.Vidas = toInt(qtdVidas);
                                logErr01.Msg = "Sem cobranca na comp. anterior";
                                logErr01.Origem = (int)CobrancaLog.Fonte.WebService;
                                pm.Save(logErr01);

                                pm.Commit();

                                return retorno("erro", "Nao identificamos cobranca gerada na competencia anterior");
                            }
                        }
                        catch (Exception ex)
                        {
                            pm.Rollback();

                            return retorno("erro", ex.Message);
                        }
                        #endregion

                        // Parcelas 2 em diante: só podem ser geradas entre o dia 1 do mes de vencimento e 
                        // 15 dias após a data de vencimento

                        // Intervarlo permitido para geração de cobranças
                        DateTime inicioPeriodo = new DateTime(dataVencimento.Year, dataVencimento.Month, 1);
                        DateTime fimPeriodo = new DateTime(dataVencimento.AddDays(15).Year, dataVencimento.AddDays(15).Month, dataVencimento.AddDays(15).Day, 23, 59, 59, 990);

                        if (DateTime.Now < inicioPeriodo || DateTime.Now > fimPeriodo)
                        {
                            //Emissão fora do periodo permitido
                            pm.Rollback();
                            return retorno("erro", "Emissão fora do periodo permitido que é de " + inicioPeriodo.ToString("dd/MM/yyyy") + " a " + fimPeriodo.ToString("dd/MM/yyyy"));
                        }

                        // Se a cobrança for emitida após o vencimento original, mas dentro do período permitido 
                        if (DateTime.Now.Day >= dataVencimento.Day && DateTime.Now.Day <= fimPeriodo.Day)
                        {
                            DateTime novoVencimento = new DateTime(
                                DateTime.Now.AddDays(2).Year,
                                DateTime.Now.AddDays(2).Month,
                                DateTime.Now.AddDays(2).Day, 23, 59, 59, 900);

                            cobranca.DataVencimento = novoVencimento;

                            instrucoes = "1";
                            calculaJuro = true;
                        }
                    }

                    //Devido à margem para registro no banco ...
                    DateTime validadePagto = LC.Web.PadraoSeguros.Facade.CobrancaFacade.Instancia.calculaValidadeBoleto(cobranca.DataCriacao);
                    if (cobranca.DataVencimento < validadePagto)
                    {
                        cobranca.DataVencimento = new DateTime(
                            validadePagto.AddDays(1).Year,
                            validadePagto.AddDays(1).Month,
                            validadePagto.AddDays(1).Day, 23, 59, 59, 900);
                    }

                    cobranca.Valor = (valorPorVida * Convert.ToDecimal(qtdVidas));

                    if (calculaJuro)
                    {
                        cobranca.CalculaJurosMulta(dataVencimento);
                    }

                    cobranca.Valor += acrescimoOuDesconto;

                    cobranca.Tipo = Convert.ToInt32(Cobranca.eTipo.Normal);
                    cobranca.CobrancaRefID = null;
                    cobranca.DataPgto = DateTime.MinValue;
                    cobranca.ValorPgto = Decimal.Zero;
                    cobranca.Pago = false;
                    cobranca.PropostaID = idContrato;
                    cobranca.Cancelada = false;
                    cobranca.QtdVidas = Convert.ToInt32(qtdVidas);

                    #region Verifica se tem configuração de adicional para o boleto

                    DataTable dt = LC.Web.PadraoSeguros.Facade.CobrancaFacade.Instancia.CarregaAdicionais(contrato, pm);
                    System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                    if (dt != null)
                    {
                        if (toDecimal(dt.Rows[0]["Valor"], cinfo) > decimal.Zero)
                        {
                            cobranca.AdicionalID = toInt(dt.Rows[0]["ID"]);
                            cobranca.Valor += toDecimal(dt.Rows[0]["Valor"], cinfo);
                            cobranca.InstrucaoAdicional = string.Concat(toString(dt.Rows[0]["Texto"]), " ", toDecimal(dt.Rows[0]["Valor"], cinfo).ToString("C"));
                        }
                    }

                    #endregion

                    pm.Save(cobranca);

                    //atualiza a qtd de vidas na proposta
                    string sql = string.Concat("update contrato set contrato_qtdVidas=", qtdVidas, " where contrato_id=", idContrato);
                    NonQueryHelper.Instance.ExecuteNonQuery(sql, pm);

                    ContratoBeneficiario cb = ContratoBeneficiario.CarregarTitular(idContrato, pm);

                    CobrancaLog.CobrancaCriadaLog log = new CobrancaLog.CobrancaCriadaLog();
                    log.CobrancaID = cobranca.ID;
                    log.CobrancaValor = cobranca.Valor;
                    log.PropostaID = idContrato;
                    log.CobrancaVencimento = cobranca.DataVencimento;
                    log.DataEnviada = vencimento;
                    log.Vidas = toInt(qtdVidas);
                    log.Origem = (int)CobrancaLog.Fonte.WebService;

                    try
                    {
                        pm.Save(log);
                    }
                    catch
                    {
                    }

                    pm.Commit();

                    boletoUrl = this.BoletoURL(cobranca, cb, out status, instrucoes);
                }
                catch (Exception ex)
                {
                    pm.Rollback();
                    return retorno("erro", ex.Message);
                }

                return retorno(status, boletoUrl);
            }
        }

        [WebMethod(Description = "Dados os parâmetros, retorna a url para exibição do boleto.")]
        public string ObterBoletoUrl(string token, string idCobranca)
        {
            if (token != this.TokenGuid) return retorno("erro", "Erro de autorizacao");

            string url = "", status = "";

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                Cobranca cobranca = new Cobranca(idCobranca);

                try
                {
                    pm.Load(cobranca);
                }
                catch
                {
                    pm.CloseSingleCommandInstance();
                    return retorno("erro", "Cobranca nao localizada para o id + " + idCobranca);
                }

                if (cobranca.Cancelada)
                {
                    pm.CloseSingleCommandInstance();
                    return retorno("erro", "Cobranca ja cancelada");
                }

                //TODO: necessário verificar se contrato é pj?
                if (cobranca.DataVencimento < DateTime.Now)
                {
                    if (Cobranca.VencidoHa5DiasUteis(cobranca.DataVencimento))
                    {
                        pm.CloseSingleCommandInstance();
                        return retorno("erro", "Cobranca vencida ha mais de 5 dias uteis");
                    }
                    else
                    {
                        //deve-se cancelar a cobranca atual e gerar uma nova:
                        
                        //Denis
                        //cobranca.Cancelada = true;
                        //pm.Save(cobranca);

                        //cobranca.CobrancaRefID = Convert.ToInt64(cobranca.ID);
                        //cobranca.ID = null;
                        //cobranca.ArquivoIDUltimoEnvio = null;
                        //cobranca.DataCriacao = DateTime.Now;
                        //cobranca.Cancelada = false;
                        ////////////////////////////////////////////

                        //Denis
                        //cobranca.CalculaJurosMulta();
                        //cobranca.DataVencimento = DateTime.Now.AddDays(1);
                        //cobranca.DataVencimento = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, cobranca.DataVencimento.Day, 23, 59, 59, 990);
                        ////////////////////////////////////////////

                        #region COMENTADO Regra: não pode gerar uma cobrança para um mês em que ja há cobrança
                        //Denis - bloco region
                        //try
                        //{
                        //    string qry = string.Concat(
                        //        "select cobranca_id from ",
                        //        "   cobranca where ",
                        //        "       cobranca_cobrancaRefId is null and cobranca_cancelada=0 and cobranca_propostaid=", cobranca.PropostaID,
                        //        "       and month(cobranca_dataVencimento)=", cobranca.DataVencimento.Month,
                        //        "       and cobranca_id <> ", cobranca.CobrancaRefID,
                        //        "       and year(cobranca_dataVencimento)=", cobranca.DataVencimento.Year);

                        //    object aux = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
                        //    if (aux != null && aux != DBNull.Value && Convert.ToString(aux).Trim() != "")
                        //    {
                        //        //CobrancaLog.CobrancaCriadaLog logErr01 = new CobrancaLog.CobrancaCriadaLog();
                        //        //logErr01.CobrancaValor = 0;
                        //        //logErr01.PropostaID = idContrato;
                        //        //logErr01.DataEnviada = vencimento;
                        //        //logErr01.Vidas = toInt(qtdVidas);
                        //        //logErr01.Msg = "Ja existe uma cobranca gerada para o periodo";
                        //        //logErr01.Origem = (int)CobrancaLog.Fonte.WebService;
                        //        //pm.Save(logErr01);

                        //        pm.CloseSingleCommandInstance();
                        //        return retorno("erro", "Ja existe uma cobranca gerada para o periodo");
                        //    }
                        //}
                        //catch
                        //{
                        //}
                        #endregion

                        //Denis
                        //pm.Save(cobranca);

                        //CobrancaLog.CobrancaCriadaLog log = new CobrancaLog.CobrancaCriadaLog();
                        //log.CobrancaID = cobranca.ID;
                        //log.CobrancaValor = cobranca.Valor;
                        //log.PropostaID = cobranca.PropostaID;
                        //log.CobrancaVencimento = cobranca.DataVencimento;
                        //log.DataEnviada = cobranca.DataVencimento.ToString("dd/MM/yyyy");
                        //log.Vidas = cobranca.QtdVidas;
                        //log.Msg = "Gerada a partir da cobranca " + Convert.ToString(cobranca.CobrancaRefID);
                        //log.Origem = (int)CobrancaLog.Fonte.WebService;

                        //pm.Save(log);
                        ////////////////////////////////////////////
                    }
                }

                ContratoBeneficiario cb = ContratoBeneficiario.CarregarTitular(cobranca.PropostaID, pm);

                pm.CloseSingleCommandInstance();


                url = this.BoletoURL(cobranca, cb, out status);
            }

            return retorno(status, url);
        }

        string BoletoURL(Cobranca cobranca, ContratoBeneficiario titular, out string status, string instrucaoInfomada = "")
        {
            Contrato contrato = new Contrato(cobranca.PropostaID);
            contrato.Carregar();
            cobranca.ContratoCodCobranca = contrato.CodCobranca.ToString();

            status = "ok";

            string nossoNumero = "", nome = "", email = "";

            if (!cobranca.Pago) // se NÃO está pago //////////////////////
            {
                nossoNumero = cobranca.GeraNossoNumero();

                nome = titular.BeneficiarioNome;
            }
            else
            {
                status = "erro";
                return "Cobranca ja paga";
            }

            //string naoReceber = "";
            //int dia = cobranca.DataVencimento.Day;
            //int mes = cobranca.DataVencimento.Month;
            //int ano = cobranca.DataVencimento.Year;

            String uri = "";
            String instrucoes = "";

            if (string.IsNullOrEmpty(instrucaoInfomada))
            {
                if (cobranca.Parcela == 1 || cobranca.CobrancaRefID == null)
                    instrucoes = "0"; //AS COBERTURAS DO SEGURO E DEMAIS BENEFICIOS ESTAO CONDICIONADAS AO PAGAMENTO DESTE DOCUMENTO.quebraSR CAIXA, APOS O VENCIMENTO MULTA DE 10% E JUROS DE 1% A.D.quebraNAO RECEBER APOS 05 DIAS DO VENCIMENTO.
                else
                    instrucoes = "1"; // "AS COBERTURAS DO SEGURO E DEMAIS BENEFICIOS ESTAO CONDICIONADAS AO PAGAMENTO DESTE DOCUMENTO.<br/>SR CAIXA, NAO RECEBER APOS O VENCIMENTO.";
            }
            else
                instrucoes = instrucaoInfomada;

            decimal Valor = cobranca.Valor;
            string end1 = "", end2 = "";
            //instrucoes = "";

            ////////nossoNumero = "00037208";

            IList<Endereco> enderecos = Endereco.CarregarPorDono(titular.BeneficiarioID, Endereco.TipoDono.Beneficiario);
            //IList<Endereco> enderecos = Endereco.CarregarPorDono(beneficiario.ID, Endereco.TipoDono.Beneficiario);
            if (enderecos != null && enderecos.Count > 0)
            {
                string compl = ""; if (!string.IsNullOrEmpty(enderecos[0].Complemento)) { compl = " - " + enderecos[0].Complemento; }

                end1 = string.Concat(enderecos[0].Logradouro, ", ", enderecos[0].Numero, compl);
                end2 = string.Concat(enderecos[0].CEP, " - ", enderecos[0].Bairro, " - ", enderecos[0].Cidade, " - ", enderecos[0].UF);
            }

            uri = string.Concat("bid=", titular.BeneficiarioID,
                "&contid=", contrato.ID, "&cobid=", cobranca.ID, "&instru=", instrucoes);

            string encript = new Util.Crypto.SecureQueryString().Encrypt(uri);

            String finalUrl = string.Concat(
                "http://sispag.clubeazul.org.br/boleto/boleto_itau.aspx?param=",
                encript); //Util.Geral.EncryptBetweenPHP(uri));

            //string temp = new Util.Crypto.SecureQueryString().Decrypt(encript);
                
            status = "ok";
            return finalUrl;
        }

        string ____BoletoURL(Cobranca cobranca, ContratoBeneficiario titular, out string status)
        {
            Contrato contrato = new Contrato(cobranca.PropostaID);
            contrato.Carregar();
            cobranca.ContratoCodCobranca = contrato.CodCobranca.ToString();

            status = "ok";

            string nossoNumero = "", nome = "", email = "";

            if (!cobranca.Pago) // se NÃO está pago //////////////////////
            {
                nossoNumero = cobranca.GeraNossoNumero();

                #region comentado

                //if (!Cobranca.NossoNumeroITAU)
                //    nossoNumero = nossoNumero.Substring(0, nossoNumero.Length - 1); //tira o DV

                //DateTime DataVenct = vencto;

                //if (cobranca.DataVencimento < vencto) //loga alteraçao de vencto para isencao de juros
                //{
                //    cobranca.DataVencimentoISENCAOJURO = vencto;
                //    Cobranca.LogaNovaDataDeVencimentoParaEmissao(
                //        cobranca.ID, cobranca.DataVencimento, vencto, Usuario.Autenticado.ID, null);
                //}

                //DateTime vigencia, vencimento;
                //Int32 diaDataSemJuros = -1;
                //Object valorDataLimite = null;
                //CalendarioVencimento rcv = null;

                //DateTime dataSemJuros = DateTime.MinValue;

                //try
                //{
                //    if (diaDataSemJuros == -1 || diaDataSemJuros == 0) { diaDataSemJuros = cobranca.DataVencimento.Day; }
                //    dataSemJuros = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, diaDataSemJuros, 23, 59, 59);
                //}
                //catch { }

                //if (DataVenct > DateTime.Now)
                //{
                //    dia = DataVenct.Day;
                //    mes = DataVenct.Month;
                //    ano = DataVenct.Year;
                //}

                //Boolean calculaJuros = true;
                //if (DataVenct <= new DateTime(2012, 7, 13, 23, 59, 59, 990) && DataVenct.Day == 10 && DataVenct.Month == 7 && DataVenct.Year == 2012)
                //{ calculaJuros = false; }

                //if (DataVenct.Year == 2014 && DataVenct.Month == 3 &&
                //    (DataVenct.Day == 10 || DataVenct.Day == 25))
                //{
                //    calculaJuros = false;
                //}

                ///////////////////////////////////////////////////

                #endregion

                nome = titular.BeneficiarioNome;

                #region comentado

                //calculaJuros = false; // acrescentado recentemente


                //if (calculaJuros && dataSemJuros != DateTime.MinValue && dataSemJuros < DateTime.Now && cobranca.DataVencimentoISENCAOJURO < DateTime.Now)
                //{
                //    DateTime database = new DateTime(ano, mes, dia, 23, 59, 59, 500);
                //    //CALCULA OS JUROS
                //    TimeSpan tempoAtraso = database.Subtract(dataSemJuros);

                //    Decimal atraso = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["jurosAtraso"]);
                //    Decimal atrasoDia = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["jurosDia"]);

                //    Valor += Valor * atraso;

                //    if (tempoAtraso.Days > 1)
                //    {
                //        Valor += Valor * (atrasoDia * (tempoAtraso.Days));
                //    }
                //}

                #endregion
            }
            else
            {
                status = "erro";
                return "Cobranca ja paga";
            }

            //string naoReceber = "";
            int dia = cobranca.DataVencimento.Day;
            int mes = cobranca.DataVencimento.Month;
            int ano = cobranca.DataVencimento.Year;

            String uri = "";
            String instrucoes = "";
            
            if(cobranca.CobrancaRefID == null)
                instrucoes = "AS COBERTURAS DO SEGURO E DEMAIS BENEFICIOS ESTAO CONDICIONADAS AO PAGAMENTO DESTE DOCUMENTO.<br/>SR CAIXA, APOS O VENCIMENTO MULTA DE 10% E JUROS DE 1% A.D.<br/><br/>NAO RECEBER APOS 05 DIAS DO VENCIMENTO."; 
            else
                instrucoes = "AS COBERTURAS DO SEGURO E DEMAIS BENEFICIOS ESTAO CONDICIONADAS AO PAGAMENTO DESTE DOCUMENTO.<br/>SR CAIXA, NAO RECEBER APOS O VENCIMENTO.";

            decimal Valor = cobranca.Valor;
            string end1 = "", end2 = "";
            //instrucoes = "";

            ////////nossoNumero = "00037208";

            IList<Endereco> enderecos = Endereco.CarregarPorDono(titular.BeneficiarioID, Endereco.TipoDono.Beneficiario);
            //IList<Endereco> enderecos = Endereco.CarregarPorDono(beneficiario.ID, Endereco.TipoDono.Beneficiario);
            if (enderecos != null && enderecos.Count > 0)
            {
                string compl = ""; if (!string.IsNullOrEmpty(enderecos[0].Complemento)) { compl = " - " + enderecos[0].Complemento; }

                end1 = string.Concat(enderecos[0].Logradouro, ", ", enderecos[0].Numero, compl);
                end2 = string.Concat(enderecos[0].CEP, " - ", enderecos[0].Bairro, " - ", enderecos[0].Cidade, " - ", enderecos[0].UF);
            }


            uri = EntityBase.RetiraAcentos(String.Concat("?nossonum=", nossoNumero, "&valor=", Valor.ToString("N2"), "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dia, "&v_mes=", mes, "&v_ano=", ano, "&numdoc2=", contrato.Numero.PadLeft(5, '0'), "&nome=", nome, "&cod_cli=", cobranca.ID, "&end1=", end1, "&end2=", end2, "&mailto=", email, "&instr=", instrucoes));//, ".<br><br>" + naoReceber));

            String finalUrl = "";

            finalUrl = string.Concat(
                 ConfigurationManager.AppSettings["boleto2Url"], "?param=",                                          //"http://localhost/phpBoleto/boleto/boleto_itau.php?param=",
                 Util.Geral.EncryptBetweenPHP(uri));

            status = "ok";
            return finalUrl;
        }

        string retorno(string status, string mensagem, bool semCDATA = false)  
        {
            string abreCDATA = "<![CDATA[", fechaCDATA = "]]>";

            if (semCDATA)
            {
                abreCDATA = ""; fechaCDATA = "";
            }

            return string.Concat("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>",
                "<retorno>",
                "<status>", status, "</status>",
                "<resposta>", abreCDATA, mensagem, fechaCDATA,"</resposta>",
                "</retorno>");
        }

        string retornoPDF(string status, string linkPdf, string linkCartao, string mensagem = "", bool semCDATA = false)
        {
            string abreCDATA = "<![CDATA[", fechaCDATA = "]]>";

            if (semCDATA)
            {
                abreCDATA = ""; fechaCDATA = "";
            }

            return string.Concat("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>",
                "<retorno>",
                "<status>", status, "</status>",
                "<pdf>", abreCDATA, linkPdf, fechaCDATA, "</pdf>",
                "<cartao>", abreCDATA, linkCartao, fechaCDATA, "</cartao>",
                "<msg>", abreCDATA, mensagem, fechaCDATA, "</msg>",
                "</retorno>");
        }

        [WebMethod(Description = "Dado um id de contrato, gera o número do contrato e o atribui. Retorna o número gerado.")]
        public string SetaNumeroDeContrato(string token, string contratoId)
        {
            if (token != this.TokenGuid) return retorno("erro", "Erro de autorizacao");

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                Contrato contrato = null;

                try
                {
                    contrato = new Contrato(contratoId);
                    pm.Load(contrato);

                    if (contrato.NumeroID != null && contrato.NumeroID != DBNull.Value && Convert.ToString(contrato.NumeroID).Trim() != "")
                    {
                        pm.Rollback();
                        return retorno("erro", string.Concat("Contrato id ", contratoId, " ja possui o número de id ", contrato.NumeroID));
                    }

                    string qry = "SELECT MAX(numerocontrato_numero) FROM numero_contrato";
                    object aux = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);

                    List<NumeroCartao> numero = new List<NumeroCartao>();
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

                    contrato.Numero = numero[0].NumeroCompletoSemCV;

                    numero[0].Ativo = true;
                    numero[0].Data = DateTime.Now;

                    pm.Save(numero[0]);

                    contrato.NumeroID = Convert.ToInt64(numero[0].ID);

                    pm.Save(contrato);

                    pm.Commit();

                    return retorno("ok", contrato.Numero);
                }
                catch(Exception ex)
                {
                    pm.Rollback();
                    return retorno("erro", ex.Message);
                }
            }
        }

        int ultimoDiaDoMes(DateTime data)
        {
            var pdata = new DateTime(data.Year, data.Month, 1);

            return pdata.AddMonths(1).AddDays(-1).Day;
        }

        decimal toDecimal(object param, System.Globalization.CultureInfo cinfo)
        {
            if (param == null || param == DBNull.Value)
                return decimal.Zero;
            else
                return Convert.ToDecimal(param, cinfo);
        }

        string toString(object param)
        {
            if (param == null || param == DBNull.Value)
                return null;
            else
                return Convert.ToString(param);
        }

        DateTime toDate(object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return DateTime.MinValue;
            else
            {
                try
                {
                    string strdata = Convert.ToString(param).Substring(0, 10);
                    String[] arr = strdata.Split('/');
                    if (arr.Length != 3) { return DateTime.MinValue; }

                    DateTime resultado = DateTime.MinValue;


                    return new DateTime(Int32.Parse(arr[2].Replace(" 00:00:00", "")), Int32.Parse(arr[1]), Int32.Parse(arr[0]));
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }
        }

        int toInt(object param)
        {
            if (param == null || param == DBNull.Value)
                return 0;
            else
                return Convert.ToInt32(param);
        }
        
        //[WebMethod()]
        DateTime calcula1oVencimento(DateTime vencimentoReferencia, int diaVencimentoProjeto)  //(string strvencimentoReferencia, string strAgora)
        {
            //string[] arr = strvencimentoReferencia.Split('/');
            //DateTime vencimentoReferencia = new DateTime(
            //    Convert.ToInt32(arr[2]), Convert.ToInt32(arr[1]), Convert.ToInt32(arr[0]), 23, 59, 59, 900);

            //string[] arrAgora = strAgora.Split('/');
            //DateTime agora = new DateTime(
            //    Convert.ToInt32(arrAgora[2]), Convert.ToInt32(arrAgora[1]), Convert.ToInt32(arrAgora[0]), DateTime.Now.Hour, DateTime.Now.Minute, 59, 900);

            DateTime agora = DateTime.Now;

            DateTime limiteAte = new DateTime(agora.Year, agora.Month, 8, 23, 59, 59, 995);
            DateTime limiteDe = new DateTime(agora.AddMonths(-1).Year, agora.AddMonths(-1).Month, 28);

            if (agora >= limiteDe && agora <= limiteAte)
            {
                return new DateTime(agora.Year, agora.Month, diaVencimentoProjeto, 23, 59, 59, 995);
            }
            else if (agora.Day >= 9 && agora.Day <= 27)
            {
                return new DateTime(agora.AddDays(2).Year, agora.AddDays(2).Month, agora.AddDays(2).Day, 23, 59, 59, 995);
            }
            else
            {
                return new DateTime(agora.AddMonths(1).Year, agora.AddMonths(1).Month, diaVencimentoProjeto, 23, 59, 59, 995);
            }
/*
             
Denis, bom dia !!

   Para fechar esse assunto do 1º boleto o que adotaremos 2 condições:

Condição A)  1º boleto sendo gerado entre os dias 28 do mês anterior e 08 do mês seguinte o vencimento sempre será o vencimento original do projeto, que em sua maioria é dia 10 de cada mês.  Ex: 1º boleto gerado no dia 28/03/2017 , vencimento será dia 10/04/2017 ou 1º boleto no dia 04/04/2017 o vencimento será no dia 10/04/2017.
Condição B)  1º  boleto gerado entre 09 e 27 do mês corrente o vencimento deste 1º boleto  será sempre em 48 horas da emissão, e a partir do segundo boleto o vencimento original do projeto. Ex: 1º boleto gerado em 09/04/2017 vencimento será dia 11/04/2017, e o segundo boleto vencimento padrão do projeto todo dia 10, portanto em 10/05/2017 e assim por diante. Ou 1º boleto gerado em 27/03/2017 vencimento em 29/03/2017 e o segundo  boleto vencimento padrão do projeto todo dia 10, portanto  em 10/04/2017 e assim por diante.

Desta forma ajustamos o fluxo e evitamos falhas. Lembrando que esse 1º boleto não pode ter juros nem multa inseridos se forem gerados conforme acima explicado, pois se trata de 1º boleto.

Abraços
Dornelas 
             
             */


            //DateTime dtRef = new DateTime(vencimentoReferencia.Year, vencimentoReferencia.Month, vencimentoReferencia.Day);

            //if (agora < dtRef.AddDays(-2))
            //{
            //    return vencimentoReferencia;
            //}
            //else
            //{
            //    dtRef = agora.AddDays(2);
            //    dtRef = new DateTime(dtRef.Year, dtRef.Month, dtRef.Day, 23, 59, 59, 900);
            //    return dtRef;
            //}
        }

        #region Geracao de Cartao 

        string DadosParaCartao___(string codigoContrato, string matriculaBeneficiario, string token)
        {
            if (token != this.TokenGuid) return retorno("erro", "Erro de autorizacao");

            string qry = "", retornar = "";
            StringBuilder sb = new StringBuilder();
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                if (!string.IsNullOrEmpty(codigoContrato))
                {
                    qry = string.Concat(
                        "select * from beneficiario ",
                        "   inner join contrato_beneficiario on contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 and contratobeneficiario_beneficiarioId = beneficiario_id ",
                        "   inner join contrato on contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 and contratobeneficiario_contratoId = contrato_id ",
                        "   inner join contratoadm on contratoadm_id = contrato_contratoAdmId ",
                        "   inner join operadora on contrato_operadoraId=operadora_id ",
                        "   inner join estipulante on estipulante_id = contrato_estipulanteId",
                        "   left join endereco on endereco_donoId=beneficiario_id and endereco_donoTipo=0",
                        " where ",
                        "   contrato_cancelado <> 1 and contrato_inativo <> 1 ",
                        "   and contratoadm_id=", codigoContrato,
                        " order by beneficiario_nome ");
                }
                else
                {
                    qry = string.Concat(
                        "select * from beneficiario ",
                        "   inner join contrato_beneficiario on contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 and contratobeneficiario_beneficiarioId = beneficiario_id ",
                        "   inner join contrato on contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 and contratobeneficiario_contratoId = contrato_id ",
                        "   inner join contratoadm on contratoadm_id = contrato_contratoAdmId ",
                        "   inner join operadora on contrato_operadoraId=operadora_id ",
                        "   inner join estipulante on estipulante_id = contrato_estipulanteId",
                        "   left join endereco on endereco_donoId=beneficiario_id and endereco_donoTipo=0",
                        " where ",
                        "   contrato_cancelado <> 1 and contrato_inativo <> 1 ",
                        "   and contrato_numeroMatricula='", matriculaBeneficiario,"'",
                        " order by beneficiario_nome ");
                }

                DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result", pm).Tables[0];
                pm.CloseSingleCommandInstance();

                if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
                {
                    return retorno("erro", "Nenhum registro localizado");
                }

                List<string> idsprocessados = new List<string>();

                foreach (DataRow row in dt.Rows)
                {
                    if(idsprocessados.Contains(Convert.ToString(row["beneficiario_id"]))) continue;
                    idsprocessados.Add(Convert.ToString(row["beneficiario_id"]));

                    sb.Append("<beneficiario>");
                    sb.Append("<nome>"); sb.Append(row["beneficiario_nome"]); sb.Append("</nome>");
                    sb.Append("<documento>"); sb.Append(row["beneficiario_cpf"]); sb.Append("</documento>");
                    sb.Append("<nomeMae>"); sb.Append(row["beneficiario_nomeMae"]); sb.Append("</nomeMae>");
                    sb.Append("<email>"); sb.Append(row["beneficiario_email"]); sb.Append("</email>");

                    sb.Append("<contrato>");
                    sb.Append("<numero>"); sb.Append(row["contrato_numero"]); sb.Append("</numero>");
                    sb.Append("<dataCadastro>"); sb.Append(Convert.ToDateTime(row["contrato_data"], cinfo).ToString("dd/MM/yyyy")); sb.Append("</dataCadastro>");
                    sb.Append("<dataAdmissao>"); sb.Append(Convert.ToDateTime(row["contrato_admissao"], cinfo).ToString("dd/MM/yyyy")); sb.Append("</dataAdmissao>");
                    sb.Append("<dataValidade>"); 
                    if(row["contrato_validade"] != null && row["contrato_validade"] != DBNull.Value && Convert.ToString(row["contrato_validade"]).Trim() != "")
                        sb.Append(Convert.ToDateTime(row["contrato_validade"], cinfo).ToString("dd/MM/yyyy")); 
                    sb.Append("</dataValidade>");
                    sb.Append("<apolice>"); sb.Append(row["contrato_numeroApolice"]); sb.Append("</apolice>");
                    sb.Append("</contrato>");

                    sb.Append("<contratoAdm>");
                    sb.Append("<nome>"); sb.Append(row["contratoadm_descricao"]); sb.Append("</nome>");
                    sb.Append("<diaVencimento>"); sb.Append(row["contratoADM_DTVC"]); sb.Append("</diaVencimento>");
                    sb.Append("</contratoAdm>");

                    sb.Append("<operadora>");
                    sb.Append("<nome>"); sb.Append(row["operadora_nome"]); sb.Append("</nome>");
                    sb.Append("<cnpj>"); sb.Append(row["operadora_cnpj"]); sb.Append("</cnpj>");
                    sb.Append("<telefone>"); sb.Append(row["operadora_fone"]); sb.Append("</telefone>");
                    sb.Append("</operadora>");

                    sb.Append("<associadopj>");
                    sb.Append("<nome>"); sb.Append(row["estipulante_descricao"]); sb.Append("</nome>");
                    sb.Append("<diaVencimento>"); sb.Append(row["estipulante_dataVencimento"]); sb.Append("</diaVencimento>");
                    sb.Append("</associadopj>");

                    sb.Append("<endereco>");
                    sb.Append("<logradouro>"); sb.Append(row["endereco_logradouro"]); sb.Append("</logradouro>");
                    sb.Append("<numero>"); sb.Append(row["endereco_numero"]); sb.Append("</numero>");
                    sb.Append("<complemento>"); sb.Append(row["endereco_complemento"]); sb.Append("</complemento>");
                    sb.Append("<bairro>"); sb.Append(row["endereco_bairro"]); sb.Append("</bairro>");
                    sb.Append("<cidade>"); sb.Append(row["endereco_cidade"]); sb.Append("</cidade>");
                    sb.Append("<uf>"); sb.Append(row["endereco_uf"]); sb.Append("</uf>");
                    sb.Append("<cep>"); sb.Append(row["endereco_cep"]); sb.Append("</cep>");
                    sb.Append("</endereco>");

                    sb.Append("</beneficiario>");
                }

                retornar = sb.ToString();
                idsprocessados.Clear();
                sb.Remove(0, sb.Length);
            }

            return retorno("ok", retornar, true);
        }
        string geraPdf(string modelo, DataRow rowDadosBasicos, DataRowCollection rowDadosCobertura)
        {
            #region text

            //Estrategia para negrito: deixar com espaços em branco os textos em negrito, e depois posicionar o texto
            //negritado "na mao"

            string text = string.Concat("Agora você é um(a) associado(a) Clube Azul Vida saudável, um cartão de benefícios na área da saúde e bem estar.",
                "Você terá vantagens e serviços diversos, tais como ",
                "rede conveniada a baixo custo, descontos em farmácias, seguros diversos, entre ",
                "outros. A relação de benefícios varia conforme a sua contratação.\n\n",
                "Acima você encontra o número do seu cartão e sua senha de acesso, que pode ser ",
                "trocada através da internet. Ela é sua assinatura eletrônica para autorizar o débito dos ",
                "procedimentos em cada prestador, portanto, guarde-a sob sigilo. Atenção, a inserção ",
                "incorreta no sistema por três vezes consecutivas bloqueará o seu cartão, ",
                "inviabilizando a sua utilização.\n\n",
                "Para utilizar a nossa rede afiliada de médicos, dentistas e laboratórios, é necessário ",
                "ter créditos no seu cartão para pagamento dos procedimentos. Você poderá carregar ",
                "créditos no seu cartão através de pagamento de boletos de recarga.\n\n",
                "Para obter mais informações a respeito dos serviços que estão disponíveis no seu ",
                "cartão, e sobre os processos e prazos de disponibilidade dos créditos acesse o site ",
                "www.clubeazul.org.br ou entre em contato com nosso Call Center 3916-7277 (Rio ",
                "de Janeiro) ou 4020-1610 (outras localidades) de segunda a sexta das 09:00h às ",
                "17:00h.");

            #endregion

            string nome             = toString(rowDadosBasicos["beneficiario_nome"]);
            string cpf              = toString(rowDadosBasicos["beneficiario_cpf"]);

            if (!string.IsNullOrWhiteSpace(cpf))
                cpf = Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");

            string nascimento = toDate(rowDadosBasicos["beneficiario_dataNascimento"]).ToString("dd/MM/yyyy");

            string senha            = toString(rowDadosBasicos["contrato_senha"]);
            string ramo             = toString(rowDadosBasicos["contrato_ramo"]);
            string apolice          = toString(rowDadosBasicos["contrato_numeroApolice"]);
            string certificado      = toString(rowDadosBasicos["contrato_numeroMatricula"]);
            string vigencia         = toDate(rowDadosBasicos["contrato_vigencia"]).ToString("dd/MM/yyyy"); //inicio do risco
            string vigenciaFim      = toDate(rowDadosBasicos["contrato_vigencia"]).AddYears(1).ToString("dd/MM/yyyy"); //Fim Vigência
            string estipulanteNome  = toString(rowDadosBasicos["estipulante_descricao"]);
            string emissao          = toDate(rowDadosBasicos["beneficiario_data"]).ToString("dd/MM/yyyy");
            string contratoId       = toString(rowDadosBasicos["contrato_id"]);
            string produto          = toString(rowDadosBasicos["contrato_produto"]);

            string numero           = toString(rowDadosBasicos["contrato_numero"]);
            numero                  = Convert.ToUInt64(numero).ToString(@"0000\.0000\.0000\.0000");

            string caminhoPdfs      = ConfigurationManager.AppSettings["appPdFCarteiraCaminhoFisico"];
            string pdfOriginal      = "";

            int indicePagina;
            if(modelo.ToLower() == "capemisa")
                indicePagina = 0; //pdfOriginal = caminhoPdfs + "mod-capemisa.pdf";
            else
                indicePagina = 1; // pdfOriginal = caminhoPdfs + "mod-generalli.pdf";

            pdfOriginal = caminhoPdfs + "mod.pdf";

            string pdfNome          = string.Concat(new Guid(Convert.ToInt32(contratoId), 1, 2, 3, 4, 5, 6, 7, 8, 9, 0), ".pdf");
            string pdfNovo          = string.Concat(caminhoPdfs, pdfNome);

            PdfSharp.Pdf.PdfDocument PDFDoc    = PdfSharp.Pdf.IO.PdfReader.Open(pdfOriginal, PdfDocumentOpenMode.Import);
            PdfSharp.Pdf.PdfDocument PDFNewDoc = new PdfSharp.Pdf.PdfDocument();
            //for (int Pg = 0; Pg < PDFDoc.Pages.Count; Pg++)
            //{
            //    PDFNewDoc.AddPage(PDFDoc.Pages[Pg]);
            //}

            PDFNewDoc.AddPage(PDFDoc.Pages[indicePagina]);

            //Atualizando
            XFont font = new XFont("Arial Rounded MT Bold", 7, XFontStyle.Regular);
            XFont fontTexto = new XFont("DIN-Regular", 10, XFontStyle.Regular);
            XFont fCartaoNome = new XFont("Calibri", 9, XFontStyle.Bold);
            XFont fArial7 = new XFont("Arial", 7, XFontStyle.Regular);
            XFont fArialBold7 = new XFont("Arial", 7, XFontStyle.Bold);

            PdfPage page = PDFNewDoc.Pages[0];
            XGraphics gfx = XGraphics.FromPdfPage(page);

            CultureInfo cinfo = new CultureInfo("pt-Br");

            if (modelo == "capemisa")
            {
                #region capemisa

                //Cabecalho
                gfx.DrawString(nome.Split(' ')[0], font, XBrushes.Black, 208, 43);
                gfx.DrawString(numero, font, XBrushes.Black, 336, 43);
                gfx.DrawString(senha, font, XBrushes.Black, 485, 43);

                //Texto
                XRect rect = new XRect(42, 65, 505, 200);
                gfx.DrawRectangle(XPens.Transparent, XBrushes.Transparent, rect);
                XTextFormatter tf = new XTextFormatter(gfx);
                tf.Alignment = XParagraphAlignment.Justify;
                tf.DrawString(text, fontTexto, XBrushes.Black, rect, XStringFormats.TopLeft);

                gfx.DrawString(certificado, font, XBrushes.Black, 371, 297);
                gfx.DrawString(ramo, font, XBrushes.Black, 316, 321);
                gfx.DrawString(apolice, font, XBrushes.Black, 374, 321);

                gfx.DrawString(nome, font, XBrushes.Black, 42, 345);
                gfx.DrawString(estipulanteNome, font, XBrushes.Black, 376, 345);

                gfx.DrawString(cpf, font, XBrushes.Black, 42, 371);
                gfx.DrawString(nascimento, font, XBrushes.Black, 175, 371);
                gfx.DrawString(vigencia, font, XBrushes.Black, 286, 371);
                gfx.DrawString(vigenciaFim, font, XBrushes.Black, 400, 371);
                gfx.DrawString(emissao, font, XBrushes.Black, 490, 371);

                for (int i = 0; i < rowDadosCobertura.Count; i++)
                {
                    if (toString(rowDadosCobertura[i][0]).Length <= 45)
                        gfx.DrawString(toString(rowDadosCobertura[i][0]), font, XBrushes.Black, 42, 395 + (i * 10)); //horizontal - vertical
                    else
                        gfx.DrawString(toString(rowDadosCobertura[i][0]).Substring(0, 45), font, XBrushes.Black, 42, 395 + (i * 10));

                    gfx.DrawString(toDecimal(rowDadosCobertura[i][1], cinfo).ToString("N2"), font, XBrushes.Black, 177, 395 + (i * 10));

                    #region comentado 

                    //if (toString(rowDadosCobertura[i][0]).Length <= 45)
                    //    gfx.DrawString(toString(rowDadosCobertura[i][0]), font, XBrushes.Black, 82, 424 + (i * 10)); //horizontal - vertical
                    //else
                    //    gfx.DrawString(toString(rowDadosCobertura[i][0]).Substring(0, 45), font, XBrushes.Black, 82, 424 + (i * 10));

                    //gfx.DrawString(toDecimal(rowDadosCobertura[i][1], cinfo).ToString("N2"), font, XBrushes.Black, 219, 424 + (i * 10));
                    #endregion
                }

                //CARTAO
                string via = numero.Substring(14, 1).PadLeft(3, '0');
                gfx.DrawString(numero, fCartaoNome, XBrushes.Black, 306, 715);
                gfx.DrawString(nome.ToUpper(), fCartaoNome, XBrushes.Black, 306, 730);
                gfx.DrawString(produto.ToUpper(), fArialBold7, XBrushes.Black, 306, 760);
                gfx.DrawString("Via " + via, fArial7, XBrushes.Black, 385, 760);
                gfx.DrawString("Validade consulte nosso site", fArial7, XBrushes.Black, 306, 780);

                #endregion
            }
            else
            {
                #region generali 

                gfx.DrawString(nome.Split(' ')[0], font, XBrushes.Black, 208, 43);
                gfx.DrawString(numero, font, XBrushes.Black, 336, 43);
                gfx.DrawString(senha, font, XBrushes.Black, 485, 43);

                //texto
                XRect rect = new XRect(42, 65, 505, 200);
                gfx.DrawRectangle(XPens.Transparent, XBrushes.Transparent, rect);
                XTextFormatter tf = new XTextFormatter(gfx);
                tf.Alignment = XParagraphAlignment.Justify;
                tf.DrawString(text, fontTexto, XBrushes.Black, rect, XStringFormats.TopLeft);

                gfx.DrawString(certificado, font, XBrushes.Black, 371, 286);
                gfx.DrawString(ramo, font, XBrushes.Black, 396, 315);
                gfx.DrawString(apolice, font, XBrushes.Black, 479, 315);

                gfx.DrawString(nome.ToUpper(), font, XBrushes.Black, 42, 339);
                gfx.DrawString(estipulanteNome, font, XBrushes.Black, 396, 339);

                gfx.DrawString(cpf, font, XBrushes.Black, 42, 363);
                gfx.DrawString(nascimento, font, XBrushes.Black, 175, 363);
                gfx.DrawString(vigencia, font, XBrushes.Black, 286, 363);
                gfx.DrawString(vigenciaFim, font, XBrushes.Black, 400, 363);
                gfx.DrawString(emissao, font, XBrushes.Black, 490, 363);

                //dados de cobertura
                for (int i = 0; i < rowDadosCobertura.Count; i++)
                {
                    if (toString(rowDadosCobertura[i][0]).Length <= 62)
                        gfx.DrawString(toString(rowDadosCobertura[i][0]), font, XBrushes.Black, 42, 395 + (i * 10)); //horizontal - vertical
                    else
                        gfx.DrawString(toString(rowDadosCobertura[i][0]).Substring(0, 62), font, XBrushes.Black, 42, 395 + (i * 10));

                    gfx.DrawString(toDecimal(rowDadosCobertura[i][1], cinfo).ToString("N2"), font, XBrushes.Black, 193, 395 + (i * 10));
                }

                //cartao
                string via = numero.Substring(14, 1).PadLeft(3, '0');
                gfx.DrawString(numero, fCartaoNome, XBrushes.Black, 306, 715);
                gfx.DrawString(nome.ToUpper(), fCartaoNome, XBrushes.Black, 306, 730);
                gfx.DrawString(produto.ToUpper(), fArialBold7, XBrushes.Black, 306, 760);
                gfx.DrawString("Via " + via, fArial7, XBrushes.Black, 385, 760);
                gfx.DrawString("Validade consulte nosso site", fArial7, XBrushes.Black, 306, 780);

                #region comentado 

                //gfx.DrawString(certificado, font, XBrushes.Black, 415, 325);
                //gfx.DrawString(ramo, font, XBrushes.Black, 452, 354);
                //gfx.DrawString(apolice, fontMenor, XBrushes.Black, 517, 354);

                //gfx.DrawString(nome, font, XBrushes.Black, 85, 377);
                //gfx.DrawString(estipulanteNome, font, XBrushes.Black, 436, 377);

                //gfx.DrawString(cpf, font, XBrushes.Black, 83, 401);
                //gfx.DrawString(nascimento, font, XBrushes.Black, 213, 401);
                //gfx.DrawString(vigencia, font, XBrushes.Black, 313, 401);
                //gfx.DrawString(vigenciaFim, font, XBrushes.Black, 430, 401);
                //gfx.DrawString(emissao, font, XBrushes.Black, 530, 401);

                //for (int i = 0; i < rowDadosCobertura.Count; i++)
                //{
                //    if (toString(rowDadosCobertura[i][0]).Length <= 62)
                //        gfx.DrawString(toString(rowDadosCobertura[i][0]), fontMenor, XBrushes.Black, 83, 433 + (i * 10)); //horizontal - vertical
                //    else
                //        gfx.DrawString(toString(rowDadosCobertura[i][0]).Substring(0, 62), fontMenor, XBrushes.Black, 83, 433 + (i * 10));

                //    gfx.DrawString(toDecimal(rowDadosCobertura[i][1], cinfo).ToString("N2"), fontMenor, XBrushes.Black, 234, 433 + (i * 10));
                //}
                #endregion

                #endregion
            }

            if (File.Exists(pdfNovo)) File.Delete(pdfNovo);

            PDFNewDoc.Save(pdfNovo);

            return string.Concat(ConfigurationManager.AppSettings["appUrl"], "files/pdfcarteira/", pdfNome);
        }

        [Obsolete()]
        string geraCartao(string contratoId, string numero, string nome, string estipulnate)
        {
            string via = numero.Substring(14, 1).PadLeft(3, '0');

            if (!string.IsNullOrWhiteSpace(numero))
                numero = Convert.ToUInt64(numero).ToString(@"0000\.0000\.0000\.0000");

            string caminhoPdfs = ConfigurationManager.AppSettings["appPdFCarteiraCaminhoFisico"];
            string pdfOriginal = ConfigurationManager.AppSettings["appPdFCarteiraCaminhoFisico"] + "mod-cartao.pdf";

            string pdfNome = string.Concat(new Guid(Convert.ToInt32(contratoId), 7, 753, 1, 2, 3, 4, 5, 6, 8, 9), ".pdf");
            string pdfNovo = string.Concat(caminhoPdfs, pdfNome);

            PdfSharp.Pdf.PdfDocument PDFDoc = PdfSharp.Pdf.IO.PdfReader.Open(pdfOriginal, PdfDocumentOpenMode.Import);
            PdfSharp.Pdf.PdfDocument PDFNewDoc = new PdfSharp.Pdf.PdfDocument();

            for (int Pg = 0; Pg < PDFDoc.Pages.Count; Pg++)
            {
                PDFNewDoc.AddPage(PDFDoc.Pages[Pg]);
            }

            //Atualizando
            XFont font = new XFont("Verdana", 6, XFontStyle.Bold);
            XFont fontMenor = new XFont("Verdana", 5, XFontStyle.Bold);

            PdfPage page = PDFNewDoc.Pages[0];
            XGraphics gfx = XGraphics.FromPdfPage(page);

            CultureInfo cinfo = new CultureInfo("pt-Br");

            #region
            gfx.DrawString(numero, font, XBrushes.Black, 18, 74);

            gfx.DrawString(nome, font, XBrushes.Black, 18, 87);

            gfx.DrawString(estipulnate, fontMenor, XBrushes.Black, 18, 119);
            gfx.DrawString("Via " + via, fontMenor, XBrushes.Black, 110, 119);

            gfx.DrawString("Validade consulte nosso site", fontMenor, XBrushes.Black, 18, 140);
            #endregion

            if (File.Exists(pdfNovo)) File.Delete(pdfNovo);

            PDFNewDoc.Save(pdfNovo);

            return string.Concat(ConfigurationManager.AppSettings["appUrl"], "files/pdfcarteira/", pdfNome);
        }

        [WebMethod()]
        public string GerarCartao(string idContrato, string modelo, string token, CartaoDTO dto)
        {
            #region validacoes 

            if (token != this.TokenGuid) return retorno("erro", "Erro de autorizacao");

            if (string.IsNullOrEmpty(modelo) || (modelo.ToLower() != "capemisa" && modelo.ToLower() != "generalli"))
            {
                return retorno("erro", "Parametro modelo dever igual a 'capemisa' ou 'generalli'.");
            }

            if (string.IsNullOrEmpty(idContrato))
            {
                if (dto == null)
                {
                    return retorno("erro", "Parâmetros idContato e dto não enviados");
                }
                else
                {
                    string msg = "";
                    bool ok = dto.Validar(out msg);

                    if (!ok)
                    {
                        return retorno("erro", msg);
                    }

                    idContrato = Importar(dto, out msg).ToString();

                    if (idContrato == "0")
                    {
                        return retorno("erro", msg);
                    }
                }
            }

            #endregion

            string qry = "", pdfGerado = "", cartaoGerado = "";
            StringBuilder sb = new StringBuilder();
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                //Dados básicos
                qry = string.Concat(
                    "select beneficiario_nome,contrato_numero,beneficiario_cpf,beneficiario_dataNascimento,contrato_ramo,contrato_numeroApolice,contrato_numeroMatricula,contrato_vigencia,contrato_vigencia,estipulante_descricao,beneficiario_data,contrato_id,contratoadm_id, contrato_senha, contrato_produto from beneficiario ",
                    "   inner join contrato_beneficiario on contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 and contratobeneficiario_beneficiarioId = beneficiario_id ",
                    "   inner join contrato on contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 and contratobeneficiario_contratoId = contrato_id ",
                    "   inner join contratoadm on contratoadm_id = contrato_contratoAdmId ",
                    "   inner join operadora on contrato_operadoraId=operadora_id ",
                    "   inner join estipulante on estipulante_id = contrato_estipulanteId",
                    "   left join endereco on endereco_donoId=beneficiario_id and endereco_donoTipo=0",
                    " where ",
//                  "   contrato_cancelado <> 1 and contrato_inativo <> 1 and ",
                    "   contrato_id=", idContrato,
                    " order by beneficiario_nome ");

                DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "result", pm).Tables[0];

                if (dt == null || dt.Rows == null || dt.Rows.Count == 0)
                {
                    pm.CloseSingleCommandInstance();
                    return retornoPDF("erro", "", "", "Nenhum registro localizado.", true);
                }

                qry = string.Concat("select itemcobertura_descricao, itemcobertura_valor, status_",
                    " from tabela_cobertura_item ",
                    "   inner join tabela_cobertura on tabela_cobertura_item.itemcobertura_tabelaId = tabela_cobertura.tabela_id ",
                    " where status_='Contratado' and tabela_contratoAdmId = ", dt.Rows[0]["contratoadm_id"]);

                DataTable dtCobertura = LocatorHelper.Instance.ExecuteQuery(qry, "result", pm).Tables[0];
                pm.CloseSingleCommandInstance();

                if (dtCobertura == null || dtCobertura.Rows == null || dtCobertura.Rows.Count == 0)
                {
                    return retornoPDF("erro", "", "", "Nenhuma cobertura localizada.", true);
                }

                pdfGerado = geraPdf(modelo, dt.Rows[0], dtCobertura.Rows);

                cartaoGerado = ""; // geraCartao(toString(dt.Rows[0]["contrato_id"]), toString(dt.Rows[0]["contrato_numero"]), toString(dt.Rows[0]["beneficiario_nome"]), toString(dt.Rows[0]["estipulante_descricao"]));

                #region comentado 

                //List<string> idsprocessados = new List<string>();

                //foreach (DataRow row in dt.Rows)
                //{
                //    if (idsprocessados.Contains(Convert.ToString(row["beneficiario_id"]))) continue;
                //    idsprocessados.Add(Convert.ToString(row["beneficiario_id"]));

                //    sb.Append("<beneficiario>");
                //    sb.Append("<nome>"); sb.Append(row["beneficiario_nome"]); sb.Append("</nome>");
                //    sb.Append("<documento>"); sb.Append(row["beneficiario_cpf"]); sb.Append("</documento>");
                //    sb.Append("<nomeMae>"); sb.Append(row["beneficiario_nomeMae"]); sb.Append("</nomeMae>");
                //    sb.Append("<email>"); sb.Append(row["beneficiario_email"]); sb.Append("</email>");

                //    sb.Append("<contrato>");
                //    sb.Append("<numero>"); sb.Append(row["contrato_numero"]); sb.Append("</numero>");
                //    sb.Append("<dataCadastro>"); sb.Append(Convert.ToDateTime(row["contrato_data"], cinfo).ToString("dd/MM/yyyy")); sb.Append("</dataCadastro>");
                //    sb.Append("<dataAdmissao>"); sb.Append(Convert.ToDateTime(row["contrato_admissao"], cinfo).ToString("dd/MM/yyyy")); sb.Append("</dataAdmissao>");
                //    sb.Append("<dataValidade>");

                //    if (row["contrato_validade"] != null && row["contrato_validade"] != DBNull.Value && Convert.ToString(row["contrato_validade"]).Trim() != "")
                //        sb.Append(Convert.ToDateTime(row["contrato_validade"], cinfo).ToString("dd/MM/yyyy"));

                //    sb.Append("</dataValidade>");
                //    sb.Append("<apolice>"); sb.Append(row["contrato_numeroApolice"]); sb.Append("</apolice>");
                //    sb.Append("</contrato>");

                //    sb.Append("<contratoAdm>");
                //    sb.Append("<nome>"); sb.Append(row["contratoadm_descricao"]); sb.Append("</nome>");
                //    sb.Append("<diaVencimento>"); sb.Append(row["contratoADM_DTVC"]); sb.Append("</diaVencimento>");
                //    sb.Append("</contratoAdm>");

                //    sb.Append("<operadora>");
                //    sb.Append("<nome>"); sb.Append(row["operadora_nome"]); sb.Append("</nome>");
                //    sb.Append("<cnpj>"); sb.Append(row["operadora_cnpj"]); sb.Append("</cnpj>");
                //    sb.Append("<telefone>"); sb.Append(row["operadora_fone"]); sb.Append("</telefone>");
                //    sb.Append("</operadora>");

                //    sb.Append("<associadopj>");
                //    sb.Append("<nome>"); sb.Append(row["estipulante_descricao"]); sb.Append("</nome>");
                //    sb.Append("<diaVencimento>"); sb.Append(row["estipulante_dataVencimento"]); sb.Append("</diaVencimento>");
                //    sb.Append("</associadopj>");

                //    sb.Append("<endereco>");
                //    sb.Append("<logradouro>"); sb.Append(row["endereco_logradouro"]); sb.Append("</logradouro>");
                //    sb.Append("<numero>"); sb.Append(row["endereco_numero"]); sb.Append("</numero>");
                //    sb.Append("<complemento>"); sb.Append(row["endereco_complemento"]); sb.Append("</complemento>");
                //    sb.Append("<bairro>"); sb.Append(row["endereco_bairro"]); sb.Append("</bairro>");
                //    sb.Append("<cidade>"); sb.Append(row["endereco_cidade"]); sb.Append("</cidade>");
                //    sb.Append("<uf>"); sb.Append(row["endereco_uf"]); sb.Append("</uf>");
                //    sb.Append("<cep>"); sb.Append(row["endereco_cep"]); sb.Append("</cep>");
                //    sb.Append("</endereco>");

                //    sb.Append("</beneficiario>");
                //}

                //retornar = sb.ToString();
                //idsprocessados.Clear();
                //sb.Remove(0, sb.Length);

                #endregion
            }

            return retornoPDF("ok", pdfGerado, cartaoGerado, "", true);
        }

        long Importar(CartaoDTO dto, out string mensagem)
        {
            mensagem = "";

            using (var sessao = ObterSessao())
            {
                //List<AssociadoPJ_X_Beneficiario> lista = new List<AssociadoPJ_X_Beneficiario>();
                using (var trans = sessao.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    IDbCommand cmdAux = sessao.Connection.CreateCommand();
                    trans.Enlist(cmdAux);

                    try
                    {
                        object aux = null;
                        string cpf = "", contratoNumero = "";

                        Entidades.Beneficiario beneficiario;

                        //bool cepErrado = false;
                        //AssociadoPJ_X_Beneficiario itemAssociacao = null;

                        //Beneficiário
                        cpf = dto.CPF_TITULAR;

                        #region validacoes e salva beneficiario

                        try
                        {
                            if (cpf != "99999999999")
                            {
                                cpf = cpf.Replace(".", "").Replace("-", "").Replace(@"/", "").PadLeft(11, '0');
                                beneficiario = sessao.Query<Entidades.Beneficiario>().Where(b => b.CPF.Replace(".", "").Replace("-", "") == cpf).SingleOrDefault();
                            }
                            else
                            {
                                beneficiario = null;
                            }
                        }
                        catch
                        {
                            mensagem = "Erro inesperado 001";
                            trans.Rollback();
                            return 0;
                        }

                        if (beneficiario != null)
                        {
                            mensagem = "Beneficiário já cadastrado";
                            trans.Rollback();
                            return 0;
                        }

                        #region salva beneficiario

                        beneficiario = new Entidades.Beneficiario();

                        beneficiario.Celular = dddFone(dto.DDD_CEL, dto.FONE_CEL);
                        beneficiario.CPF = cpf;
                        beneficiario.Data = DateTime.Now;
                        beneficiario.DataNascimento = dto.DT_NASCIMENTO;
                        beneficiario.Email = dto.EMAIL;
                        beneficiario.EstadoCivilId = 0;
                        beneficiario.Nome = dto.NOME_BENEFICIARIO;
                        //beneficiario.NomeMae = dto.NOME_MAE;
                        beneficiario.Peso = 0;
                        beneficiario.Ramal = dto.RAMAL1;
                        //beneficiario.Ramal2 = stoString(row["RAMAL2"]);
                        //beneficiario.RG = dto.RG;
                        //beneficiario.RGOrgaoExp = dto.RG_ORGAO_EXP;
                        //beneficiario.RgUF = dto.RG_UF;
                        beneficiario.SexoId = traduzSexo(dto.SEXO).ToString();
                        beneficiario.Telefone = dddFone(dto.DDD1, dto.FONE1);
                        //beneficiario.Telefone2 = dddFone(dto.DDD2, dto.FONE2);

                        sessao.Save(beneficiario);

                        #endregion


                        #endregion

                        #region salva endereço

                        //ATENCAO: nao se deve alterar enderecos pre-existentes, pois isso alterará tb os contratos antigos, caso existam
                        Entidades.Endereco endereco = new Entidades.Endereco();

                        endereco.Bairro = dto.BAIRRO;
                        endereco.CEP = dto.CEP.Replace("-", "");
                        endereco.Cidade = dto.CIDADE;
                        endereco.Complemento = dto.COMPLEMENTO;
                        endereco.DonoId = beneficiario.ID;
                        endereco.Logradouro = dto.LOGRADOURO;
                        endereco.Numero = dto.NUMERO;
                        endereco.Tipo = 0; //stoString(row["TIPO"]) == "1" ? 1 : 0;
                        endereco.UF = dto.UF.Trim();

                        sessao.Save(endereco);

                        #endregion

                        Entidades.NumeroCartao numero = null;

                        numero = new Entidades.NumeroCartao();

                        // Gera um número válido de contrato
                        cmdAux.CommandText = "SELECT MAX(numerocontrato_numero) FROM numero_contrato";
                        aux = cmdAux.ExecuteScalar();

                        if (aux != null && aux != DBNull.Value)
                        {
                            contratoNumero = (Convert.ToInt64(aux) + 1).ToString();
                            numero.Numero = contratoNumero;
                            numero.GerarDigitoVerificador();
                        }
                        else
                        {
                            numero.GerarNumeroInicial();
                        }

                        numero.Ativo = true;
                        numero.Contrato = null;
                        numero.Data = DateTime.Now;

                        Entidades.Contrato contrato = new Entidades.Contrato();

                        #region preenche contrato

                        contrato.Importado = true;
                        contrato.Cancelado = false;
                        contrato.ContratoADMID = Convert.ToInt64(dto.ContratoAdmID);
                        contrato.DataAdmissao = dto.DT_ADMISSAO;
                        contrato.DataVigencia = dto.DT_VIGENCIA;
                        contrato.DonoID = -1;
                        contrato.EmailCobranca = beneficiario.Email;
                        contrato.EnderecoCobrancaID = endereco.ID;
                        contrato.EnderecoReferenciaID = endereco.ID;
                        contrato.EstipulanteID = Convert.ToInt64(dto.AssociadoPjID); //Estipulante
                        contrato.FilialID = Convert.ToInt64(dto.FilialID); //Filial
                        contrato.Inativo = false;
                        contrato.Numero = numero.NumeroCompletoSemCV;
                        contrato.NumeroID = 0; ////////////////////////

                        contrato.OperadoraID = Convert.ToInt64(dto.OperadoraID);
                        contrato.PlanoID = Convert.ToInt64(dto.PlanoID);

                        contrato.Matricula = dto.MATRICULA;

                        //contrato.ResponsavelCPF = stoString(row["CPF_RESP_LEGAL"]);
                        //contrato.ResponsavelDataNascimento = stoDateTimeNull(row["DT_NASC_RESP_LEGAL"]); ;
                        //contrato.ResponsavelNome = stoString(row["NOME_RESP_LEGAL"]);
                        //contrato.ResponsavelRG = stoString(row["RG_RESP_LEGAL"]);
                        //contrato.ResponsavelSexo = stoString(row["SEXO_RESP_LEGAL"]).Trim() == "1" ? "Masculino" : "Feminino";

                        contrato.Senha = ""; /////////////
                        contrato.TipoAcomodacao = 0;
                        contrato.TipoContratoID = 1; //straduzTipoContrato(row["TIPO_PROPOSTA"]);
                        contrato.NumeroApolice = dto.APOLICE;
                        contrato.Ramo = dto.RAMO;
                        contrato.UsuarioID = 1;
                        contrato.KitSolicitado = false;
                        contrato.CartaoSolicitado = false;
                        //contrato.CaminhoArquivo = dto.TIPO;

                        contrato.Produto = dto.PRODUTOR;

                        Entidades.AssociadoPJ assocpj = sessao.Get<Entidades.AssociadoPJ>(Convert.ToInt64(dto.AssociadoPjID));

                        if (assocpj.TipoDataValidade == Entidades.Enuns.TipoDataValidade.DataFixa)
                        {
                            contrato.DataValidade = assocpj.DataValidadeFixa.Value;
                        }
                        else
                        {
                            contrato.DataValidade = contrato.DataVigencia.AddMonths(assocpj.MesesAPartirDaVigencia.Value);
                        }

                        #endregion

                        try
                        {
                            contrato.Importado = false;
                            sessao.Save(contrato);

                            numero.Contrato = contrato;
                            sessao.Save(numero);

                            contrato.NumeroID = numero.ID;

                            contrato.GerarSenha();

                            sessao.Update(contrato);
                        }
                        catch
                        {
                            mensagem = "Erro insperado 002";
                            trans.Rollback();
                            return 0;
                        }

                        Entidades.ContratoBeneficiario cb = new Entidades.ContratoBeneficiario();
                        cb.Ativo = true;
                        cb.Beneficiario = beneficiario;
                        cb.Contrato = contrato;
                        cb.Tipo = 0;
                        cb.Data = contrato.DataAdmissao;
                        cb.Vigencia = contrato.DataVigencia;

                        sessao.Save(cb);

                        #region comentado

                        //lista.Add(
                        //    new AssociadoPJ_X_Beneficiario
                        //    {
                        //        AssociadoPjId = contrato.EstipulanteID.ToString(),
                        //        BeneficiarioId = Convert.ToString(beneficiario.ID)
                        //    });

                        //agenda.DataConclusao = DateTime.Now;
                        //sessao.Update(agenda);

                        //log.ForEach(l => sessao.Save(l));

                        ////salva a associação beneficiário x associado pj, sobrescrevendo a anterior
                        //if (lista != null && lista.Count > 0)
                        //{
                        //    cmdAux.CommandText = "delete from associadopj_beneficiario where assocbenef_associadopjId=" + lista[0].AssociadoPjId;
                        //    cmdAux.ExecuteNonQuery();

                        //    foreach (var assoc in lista)
                        //    {
                        //        cmdAux.CommandText = string.Concat(
                        //            "insert into associadopj_beneficiario (assocbenef_associadopjId,assocbenef_beneficiarioId) values (", assoc.AssociadoPjId, ",", assoc.BeneficiarioId, ")");
                        //        cmdAux.ExecuteNonQuery();
                        //    }
                        //}
                        #endregion

                        trans.Commit();

                        return contrato.ID;
                    }
                    catch
                    {
                        trans.Rollback();
                        mensagem = "Erro insperado 003";
                        return 0;
                    }
                }
            }
        }

        private static NHibernate.Cfg.Configuration _config = null;
        private static Hashtable _allFactories = new Hashtable();

        protected static ISession ObterSessao()
        {
            string chave = "1";
            ISessionFactory _sessionFactory = null;

            if (!_allFactories.ContainsKey(chave))
            {
                _sessionFactory = CriarSessaoNHibernate();
                _allFactories.Add(chave, _sessionFactory);
            }
            else
            {
                _sessionFactory = (ISessionFactory)_allFactories[chave];
            }

            ISession sessao = _sessionFactory.OpenSession();
            sessao.FlushMode = FlushMode.Commit;

            return sessao;
        }

        static ISessionFactory CriarSessaoNHibernate()
        {
            string connString = ConfigurationManager.ConnectionStrings["Contexto"].ConnectionString;

            var f = Fluently.Configure().Database((MsSqlConfiguration.MsSql2008.Dialect<MsSql2008Dialect>().ConnectionString(connString).ShowSql()))
                    .ExposeConfiguration(p => p.Properties["command_timeout"] = "1000") //timeout
                    .ExposeConfiguration(p => p.Properties["hibernate.cache.use_query_cache"] = "false") //sem cache 
                    .ExposeConfiguration(x => x.SetInterceptor(new SqlStatementInterceptor()))
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<MedProj.Entidades.Map.NHibernate.UsuarioMap>().Conventions.Setup(x => x.Add(AutoImport.Never())));

            _config = f.BuildConfiguration();
            return f.BuildSessionFactory();
        }

        string dddFone(string ddd, string fone)
        {
            if (string.IsNullOrWhiteSpace(fone))
                return string.Empty;
            else if (!string.IsNullOrWhiteSpace(ddd))
                return string.Concat("(", ddd, ") ", fone);
            else if (fone.IndexOf('(') > -1)
                return fone;
            else
                return string.Concat("(00) ", fone);
        }

        int traduzSexo(Object param)
        {
            if (param == null || param == DBNull.Value)
                return 0;

            if (Convert.ToString(param).ToUpper() == "M")
                return 1;
            else
                return 0;
        }

        #endregion

        //[WebMethod()]
        public string ObterBoletoUrl_TESTE(string token, string idCobranca)
        {
            token = "233478a4-d2a3-4514-b9c2-6c70f5c2e63d";
            idCobranca = "2070";

            if (token != this.TokenGuid) return retorno("erro", "Erro de autorizacao");

            string url = "", status = "";

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.UseSingleCommandInstance();

                Cobranca cobranca = new Cobranca(idCobranca);

                try
                {
                    pm.Load(cobranca);
                }
                catch
                {
                    pm.CloseSingleCommandInstance();
                    return retorno("erro", "Cobranca nao localizada para o id + " + idCobranca);
                }

                if (cobranca.Cancelada)
                {
                    pm.CloseSingleCommandInstance();
                    return retorno("erro", "Cobranca ja cancelada");
                }

                //TODO: necessário verificar se contrato é pj?
                if (cobranca.DataVencimento < DateTime.Now)
                {
                    if (Cobranca.VencidoHa5DiasUteis(cobranca.DataVencimento))
                    {
                        pm.CloseSingleCommandInstance();
                        return retorno("erro", "Cobranca vencida ha mais de 5 dias uteis");
                    }
                    else
                    {
                        //deve-se cancelar a cobranca atual e gerar uma nova:
                        cobranca.Cancelada = true;
                        //pm.Save(cobranca);

                        cobranca.CobrancaRefID = Convert.ToInt64(cobranca.ID);
                        cobranca.ID = null;
                        cobranca.ArquivoIDUltimoEnvio = null;
                        cobranca.DataCriacao = DateTime.Now;
                        cobranca.Cancelada = false;

                        cobranca.CalculaJurosMulta();
                        cobranca.DataVencimento = DateTime.Now.AddDays(1);
                        cobranca.DataVencimento = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, cobranca.DataVencimento.Day, 23, 59, 59, 990);

                        #region Regra: não pode gerar uma cobrança para um mês em que ja há cobrança
                        try
                        {
                            string qry = string.Concat(
                                "select cobranca_id from ",
                                "   cobranca where ",
                                "       cobranca_cancelada=0 and cobranca_propostaid=", cobranca.PropostaID,
                                "       and month(cobranca_dataVencimento)=", cobranca.DataVencimento.Month,
                                "       and cobranca_id <> ", cobranca.CobrancaRefID,
                                "       and year(cobranca_dataVencimento)=", cobranca.DataVencimento.Year);

                            object aux = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
                            if (aux != null && aux != DBNull.Value && Convert.ToString(aux).Trim() != "")
                            {
                                //CobrancaLog.CobrancaCriadaLog logErr01 = new CobrancaLog.CobrancaCriadaLog();
                                //logErr01.CobrancaValor = 0;
                                //logErr01.PropostaID = idContrato;
                                //logErr01.DataEnviada = vencimento;
                                //logErr01.Vidas = toInt(qtdVidas);
                                //logErr01.Msg = "Ja existe uma cobranca gerada para o periodo";
                                //logErr01.Origem = (int)CobrancaLog.Fonte.WebService;
                                //pm.Save(logErr01);

                                pm.CloseSingleCommandInstance();
                                return retorno("erro", "Ja existe uma cobranca gerada para o periodo");
                            }
                        }
                        catch
                        {
                        }
                        #endregion

                        //pm.Save(cobranca);

                        CobrancaLog.CobrancaCriadaLog log = new CobrancaLog.CobrancaCriadaLog();
                        log.CobrancaID = cobranca.ID;
                        log.CobrancaValor = cobranca.Valor;
                        log.PropostaID = cobranca.PropostaID;
                        log.CobrancaVencimento = cobranca.DataVencimento;
                        log.DataEnviada = cobranca.DataVencimento.ToString("dd/MM/yyyy");
                        log.Vidas = cobranca.QtdVidas;
                        log.Msg = "Gerada a partir da cobranca " + Convert.ToString(cobranca.CobrancaRefID);
                        log.Origem = (int)CobrancaLog.Fonte.WebService;

                        //pm.Save(log);
                    }
                }

                ContratoBeneficiario cb = ContratoBeneficiario.CarregarTitular(cobranca.PropostaID, pm);

                pm.CloseSingleCommandInstance();


                url = this.BoletoURL(cobranca, cb, out status);
            }

            return retorno(status, url);
        }

        //[WebMethod()]
        public string GerarBoleto_TESTE(string token, string idContrato, string qtdVidas, string vencimento)
        {
            token = this.TokenGuid;

            idContrato = "215947";
            vencimento = "10/09/2017";
            //idContrato = "203701";
            //vencimento = "20/09/2017";

            qtdVidas = "3";

            if (token != this.TokenGuid) return retorno("erro", "Erro de autorizacao");

            string[] arr = vencimento.Split('/');
            if (arr.Length != 3)
            {
                return retorno("erro", "Data de vencimento não estava em um formato válido: dd/MM/yyyy");
            }

            DateTime dataVencimento = new DateTime(
                Convert.ToInt32(arr[2]), Convert.ToInt32(arr[1]), Convert.ToInt32(arr[0]), 23, 59, 59, 900);

            string boletoUrl = "", status = "", instrucoes = "";
            bool calculaJuro = false;

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    var contrato = new Contrato(idContrato);
                    pm.Load(contrato);

                    #region DEScomentado devido a situacoes que podem ocorrer

                    object aux = LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select cobranca_id from cobranca where cobranca_pago=1 and cobranca_propostaId=", contrato.ID, " and (cobranca_cancelada is null or cobranca_cancelada=0) and month(cobranca_dataCriacao)=", arr[1], " and year(cobranca_dataCriacao)=", arr[2]),
                        null, null, pm);

                    if (aux != null && aux != DBNull.Value && Convert.ToString(aux).Trim() != "")
                    {
                        CobrancaLog.CobrancaCriadaLog logErr01 = new CobrancaLog.CobrancaCriadaLog();
                        logErr01.CobrancaValor = 0;
                        logErr01.PropostaID = idContrato;
                        logErr01.DataEnviada = vencimento;
                        logErr01.Vidas = toInt(qtdVidas);
                        logErr01.Msg = "Vencimento ja pago";
                        logErr01.Origem = (int)CobrancaLog.Fonte.WebService;
                        //pm.Save(logErr01);

                        pm.Rollback();
                        return retorno("erro", "Vencimento ja pago.");
                    }
                    #endregion

                    #region comentado
                    //aux = LocatorHelper.Instance.ExecuteScalar(
                    //    string.Concat("select tabela_id from tabela_cobertura where tabela_contratoAdmId=", contrato.ContratoADMID),
                    //    null, null, pm);

                    //if (aux == null || aux == DBNull.Value || Convert.ToString(aux).Trim() == "")
                    //{
                    //    pm.Rollback();
                    //    return retorno("erro", "Não foi possível localizar uma tabela de cobertura para o contrato adm " + Convert.ToString(contrato.ContratoADMID));
                    //}

                    //var data = LocatorHelper.Instance.ExecuteQuery(
                    //    string.Concat("select top 1 * from tabela_cobertura_vigencia where vigcobertura_inicio <= '", DateTime.Now.ToString("yyyy-MM-dd"), "' and vigcobertura_tabelaId=", aux, " order by vigcobertura_inicio desc"), //string.Concat("select top 1 * from tabela_cobertura_vigencia where vigcobertura_inicio <= '", arr[2], "-", arr[1], "-", arr[0], "' and vigcobertura_tabelaId=", aux, " order by vigcobertura_inicio desc"),
                    //    "result", pm).Tables[0];

                    //if (data.Rows.Count < 1)
                    //{
                    //    data.Dispose();
                    //    pm.Rollback();
                    //    return retorno("erro", "Nenhuma vigência localizada para a cobertura " + aux + " no vencimento " + vencimento);
                    //}

                    //System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                    //decimal valorPorVida = Convert.ToDecimal(data.Rows[0]["vigcobertura_valor"], cinfo);
                    #endregion

                    string erro = "";
                    decimal valorPorVida = this.calulaValorPorVida(pm, contrato, vencimento, out erro);
                    if (valorPorVida == 0)
                    {
                        pm.Rollback();
                        return retorno("erro", erro);
                    }

                    int diaVenctoProjeto = this.toInt(LocatorHelper.Instance.ExecuteScalar(
                        string.Concat("select contratoADM_DTVC from contratoadm where contratoadm_id=", contrato.ContratoADMID),
                        null, null, pm));

                    //TODO: denis, voltar o que estava antes?
                    var cobrancas = Cobranca.CarregarTodas(idContrato, true, pm); //var cobrancas = Cobranca.CarregarTodas(idContrato, false, pm);
                    int ultimaParcela = 1;

                    if (cobrancas != null && cobrancas.Count > 0) ultimaParcela = cobrancas.Max(c => c.Parcela) + 1;

                    decimal acrescimoOuDesconto = 0;
                    int acrescimoOuDescontoTipo = -1;

                    if (contrato.DescontoAcrescimoTipo != 0)
                    {
                        if (contrato.DescontoAcrescimoData == DateTime.MinValue ||
                            contrato.DescontoAcrescimoData >= DateTime.Now)
                        {
                            if (contrato.DescontoAcrescimoTipo == 1)
                            {
                                acrescimoOuDescontoTipo = 1;
                                acrescimoOuDesconto = contrato.DescontoAcrescimoValor;
                            }
                            else
                            {
                                acrescimoOuDescontoTipo = 2;
                                acrescimoOuDesconto = (-1 * contrato.DescontoAcrescimoValor);
                            }
                        }

                        if (contrato.DescontoAcrescimoData == DateTime.MinValue)
                        {
                            contrato.DescontoAcrescimoTipo = 0;
                            //pm.Save(contrato);//////////////////////////////////
                        }
                    }

                    //salva a cobranca
                    Cobranca cobranca = new Cobranca();
                    cobranca.Parcela = ultimaParcela;
                    cobranca.DataVencimento = dataVencimento;
                    if (acrescimoOuDesconto > decimal.Zero) cobranca.AcrescimoDeContrato = acrescimoOuDesconto;
                    if (acrescimoOuDescontoTipo > -1) cobranca.AcrescimoDeContratoTipo = acrescimoOuDescontoTipo;

                    if (cobranca.Parcela <= 1)
                    {
                        #region comentado
                        //aux = LocatorHelper.Instance.ExecuteScalar(
                        //    string.Concat("select estipulante_dataVencimento from estipulante where estipulante_id=", contrato.EstipulanteID),
                        //    null, null, pm);

                        //if (aux != null && aux != DBNull.Value)
                        //{
                        //    if (dataVencimento.Month == DateTime.Now.Month &&
                        //        dataVencimento.Day > Convert.ToInt32(aux))
                        //    {
                        //        cobranca.DataVencimento = DateTime.Now.AddDays(2);
                        //        cobranca.DataVencimento = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, cobranca.DataVencimento.Day, 23, 59, 59, 990);
                        //    }
                        //}
                        //else
                        //{
                        //    if (dataVencimento < DateTime.Now)
                        //    {
                        //        cobranca.DataVencimento = DateTime.Now.AddDays(2);
                        //        cobranca.DataVencimento = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, cobranca.DataVencimento.Day, 23, 59, 59, 990);
                        //    }
                        //}
                        #endregion

                        cobranca.Parcela = 1;

                        cobranca.DataVencimento = calcula1oVencimento(dataVencimento, diaVenctoProjeto);

                        #region comentado 2

                        //int diaHoje   = DateTime.Now.Day;
                        //int diaUltimo = ultimoDiaDoMes(DateTime.Now);

                        //if (diaHoje == diaUltimo)
                        //{
                        //    if (diaHoje == 28 || diaHoje == 29)
                        //    {
                        //        DateTime novoVencimento = dataVencimento.AddMonths(1);
                        //        novoVencimento = new DateTime(novoVencimento.Year, novoVencimento.Month, 1, 23, 59, 59, 900);
                        //        cobranca.DataVencimento = novoVencimento;
                        //    }
                        //    else if (diaHoje == 30 || diaHoje == 31)
                        //    {
                        //        DateTime novoVencimento = dataVencimento.AddMonths(1);
                        //        novoVencimento = new DateTime(novoVencimento.Year, novoVencimento.Month, 2, 23, 59, 59, 900);
                        //        cobranca.DataVencimento = novoVencimento;
                        //    }
                        //}
                        //else
                        //{
                        //    if (diaVenctoProjeto == 0) //nao pôde ler a data de vencto do projeto
                        //    {
                        //        if (dataVencimento < DateTime.Now) //PRIMEIRA PARCELA - ANTES DO FIM DO     
                        //        {
                        //            cobranca.DataVencimento = DateTime.Now.AddDays(2);
                        //            cobranca.DataVencimento = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, cobranca.DataVencimento.Day, 23, 59, 59, 990);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (DateTime.Now.Day < diaVenctoProjeto)
                        //        {
                        //            cobranca.DataVencimento = new DateTime(
                        //                cobranca.DataVencimento.Year,
                        //                cobranca.DataVencimento.Month,
                        //                diaVenctoProjeto, 23, 59, 59, 990);
                        //        }
                        //        else
                        //        {
                        //            cobranca.DataVencimento = DateTime.Now.AddDays(2);
                        //            cobranca.DataVencimento = new DateTime(
                        //                cobranca.DataVencimento.Year, 
                        //                cobranca.DataVencimento.Month, 
                        //                cobranca.DataVencimento.Day, 23, 59, 59, 990);
                        //        }
                        //    }
                        //}
                        #endregion
                    }
                    else
                    {
                        string qry = "";
                        //bool verficiarCompetenciaAnterior = true;

                        //Regra 2: não pode gerar uma cobrança para um mês em que ja há cobrança
                        try
                        {
                            qry = string.Concat(
                                "select cobranca_id from ",
                                "   cobranca where ",
                                //"       cobranca_cobrancaRefId is null and ",
                                "       cobranca_cancelada=0 and cobranca_propostaid=", contrato.ID,
                                "       and month(cobranca_dataVencimento)=", cobranca.DataVencimento.Month,
                                "       and year(cobranca_dataVencimento)=", cobranca.DataVencimento.Year);

                            aux = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
                            if (aux != null && aux != DBNull.Value && Convert.ToString(aux).Trim() != "")
                            {
                                //achou, agora precisa verificar a competencia, pois se for outra competencia, não pode bloquear
                                DateTime dataref = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, 1).AddMonths(-1);
                                qry = string.Concat(
                                "select cobranca_id from ",
                                "   cobranca where ",
                                "       cobranca_cancelada=0 and cobranca_propostaid=", contrato.ID,
                                "       and month(cobranca_dataVencimento)=", dataref.Month,
                                "       and year(cobranca_dataVencimento)=", dataref.Year);

                                aux = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);

                                if (aux != null && aux != DBNull.Value && Convert.ToString(aux).Trim() != "")
                                {
                                    CobrancaLog.CobrancaCriadaLog logErr01 = new CobrancaLog.CobrancaCriadaLog();
                                    logErr01.CobrancaValor = 0;
                                    logErr01.PropostaID = idContrato;
                                    logErr01.DataEnviada = vencimento;
                                    logErr01.Vidas = toInt(qtdVidas);
                                    logErr01.Msg = "Ja existe uma cobranca gerada para o periodo";
                                    logErr01.Origem = (int)CobrancaLog.Fonte.WebService;
                                    //pm.Save(logErr01);

                                    //pm.Rollback();
                                    //return retorno("erro", "Ja existe uma cobranca gerada para o periodo");
                                }
                                //else
                                //    verficiarCompetenciaAnterior = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            pm.Rollback();

                            return retorno("erro", ex.Message);
                        }

                        //Regra 1: não pode gerar uma cobrança se não houver cobrança gerada no mês anterior
                        try
                        {
                            //if (verficiarCompetenciaAnterior)
                            //{
                                DateTime refe = dataVencimento.AddMonths(-1);
                                qry = string.Concat(
                                    "select cobranca_id from ",
                                    "   cobranca where ",
                                    "       cobranca_propostaid=", contrato.ID, //cobranca_cancelada=0 and 
                                    "       and month(cobranca_dataVencimento)=", refe.Month,
                                    "       and year(cobranca_dataVencimento)=", refe.Year);

                                aux = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
                                if (aux == null || aux == DBNull.Value || Convert.ToString(aux).Trim() == "")
                                {
                                    CobrancaLog.CobrancaCriadaLog logErr01 = new CobrancaLog.CobrancaCriadaLog();
                                    logErr01.CobrancaValor = 0;
                                    logErr01.PropostaID = idContrato;
                                    logErr01.DataEnviada = vencimento;
                                    logErr01.Vidas = toInt(qtdVidas);
                                    logErr01.Msg = "Sem cobranca na comp. anterior";
                                    logErr01.Origem = (int)CobrancaLog.Fonte.WebService;
                                    //pm.Save(logErr01);

                                    pm.Rollback();

                                    return retorno("erro", "Nao identificamos cobranca gerada na competencia anterior");
                                }
                            //}
                        }
                        catch (Exception ex)
                        {
                            pm.Rollback();

                            return retorno("erro", ex.Message);
                        }

                        // Parcelas 2 em diante: só podem ser geradas entre o dia 1 do mes de vencimento e 
                        // 15 dias após a data de vencimento

                        // intervarlo permitido para geração de cobranças
                        DateTime inicioPeriodo = new DateTime(dataVencimento.Year, dataVencimento.Month, 1);
                        DateTime fimPeriodo = new DateTime(dataVencimento.AddDays(15).Year, dataVencimento.AddDays(15).Month, dataVencimento.AddDays(15).Day, 23, 59, 59, 990);

                        if (DateTime.Now < inicioPeriodo || DateTime.Now > fimPeriodo)
                        {
                            //Emissão fora do periodo permitido
                            pm.Rollback();
                            return retorno("erro", "Emissão fora do periodo permitido que é de " + inicioPeriodo.ToString("dd/MM/yyyy") + " a " + fimPeriodo.ToString("dd/MM/yyyy"));
                        }

                        // Se a cobrança for emitida após o vencimento original, mas dentro do período permitido 
                        if (DateTime.Now.Day >= dataVencimento.Day && DateTime.Now.Day <= fimPeriodo.Day)
                        {
                            //TODO: calcular juro e multa ?

                            DateTime novoVencimento = new DateTime(
                                DateTime.Now.AddDays(2).Year,
                                DateTime.Now.AddDays(2).Month,
                                DateTime.Now.AddDays(2).Day, 23, 59, 59, 900);

                            cobranca.DataVencimento = novoVencimento;

                            instrucoes = "1";
                            calculaJuro = true;
                        }
                    }

                    //Devido à margem para registro no banco ...
                    DateTime validadePagto = LC.Web.PadraoSeguros.Facade.CobrancaFacade.Instancia.calculaValidadeBoleto(cobranca.DataCriacao);
                    if (cobranca.DataVencimento < validadePagto)
                    {
                        cobranca.DataVencimento = new DateTime(
                            validadePagto.AddDays(1).Year,
                            validadePagto.AddDays(1).Month,
                            validadePagto.AddDays(1).Day, 23, 59, 59, 900);
                    }

                    cobranca.Valor = (valorPorVida * Convert.ToDecimal(qtdVidas));

                    if (calculaJuro)
                    {
                        cobranca.CalculaJurosMulta(dataVencimento);
                    }

                    cobranca.Valor += acrescimoOuDesconto;

                    cobranca.Tipo = Convert.ToInt32(Cobranca.eTipo.Normal);
                    cobranca.CobrancaRefID = null;
                    cobranca.DataPgto = DateTime.MinValue;
                    cobranca.ValorPgto = Decimal.Zero;
                    cobranca.Pago = false;
                    cobranca.PropostaID = idContrato;
                    cobranca.Cancelada = false;
                    cobranca.QtdVidas = Convert.ToInt32(qtdVidas);

                    #region Verifica se tem configuração de adicional para o boleto

                    DataTable dt = LC.Web.PadraoSeguros.Facade.CobrancaFacade.Instancia.CarregaAdicionais(contrato, pm);
                    System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                    if (dt != null)
                    {
                        if (toDecimal(dt.Rows[0]["Valor"], cinfo) > decimal.Zero)
                        {
                            cobranca.AdicionalID = toInt(dt.Rows[0]["ID"]);
                            cobranca.Valor += toDecimal(dt.Rows[0]["Valor"], cinfo);
                            cobranca.InstrucaoAdicional = string.Concat(toString(dt.Rows[0]["Texto"]), " ", toDecimal(dt.Rows[0]["Valor"], cinfo).ToString("C"));
                        }
                    }

                    #endregion

                    //pm.Save(cobranca);

                    //atualiza a qtd de vidas na proposta
                    string sql = string.Concat("update contrato set contrato_qtdVidas=", qtdVidas, " where contrato_id=", idContrato);
                    //NonQueryHelper.Instance.ExecuteNonQuery(sql, pm);

                    ContratoBeneficiario cb = ContratoBeneficiario.CarregarTitular(idContrato, pm);

                    CobrancaLog.CobrancaCriadaLog log = new CobrancaLog.CobrancaCriadaLog();
                    log.CobrancaID = cobranca.ID;
                    log.CobrancaValor = cobranca.Valor;
                    log.PropostaID = idContrato;
                    log.CobrancaVencimento = cobranca.DataVencimento;
                    log.DataEnviada = vencimento;
                    log.Vidas = toInt(qtdVidas);
                    log.Origem = (int)CobrancaLog.Fonte.WebService;

                    try
                    {
                        //pm.Save(log);
                    }
                    catch
                    {
                    }

                    cobranca.ID = 3655;
                    pm.Load(cobranca);
                    pm.Rollback();

                    boletoUrl = this.BoletoURL(cobranca, cb, out status, instrucoes);
                }
                catch (Exception ex)
                {
                    pm.Rollback();
                    return retorno("erro", ex.Message);
                }

                return retorno(status, boletoUrl);
            }
        }

        [Serializable]
        public class CartaoDTO
        {
            public string   PRODUTOR { get; set; }
            public string   MATRICULA{ get; set; }
            public string   RAMO { get; set; }
            public string   APOLICE { get; set; }
            public DateTime DT_ADMISSAO { get; set; }
            public DateTime DT_VIGENCIA { get; set; }
            public string   CPF_TITULAR { get; set; }
            public string   NOME_BENEFICIARIO { get; set; }
            public DateTime DT_NASCIMENTO { get; set; }
            public char     SEXO { get; set; }
            public string   DDD1 { get; set; }
            public string   FONE1 { get; set; }
            public string   RAMAL1 { get; set; }
            public string   DDD_CEL { get; set; }
            public string   FONE_CEL { get; set; }
            public string   EMAIL { get; set; }
            public string   CEP { get; set; }
            public string   LOGRADOURO { get; set; }
            public string   NUMERO { get; set; }
            public string   COMPLEMENTO { get; set; }
            public string   BAIRRO { get; set; }
            public string   CIDADE { get; set; }
            public string   UF { get; set; }

            public string   ContratoAdmID { get; set; }
            public string   AssociadoPjID { get; set; }
            public string   OperadoraID { get; set; }
            public string   FilialID { get; set; }
            public string   PlanoID { get; set; }

            public bool Validar(out string erro)
            {
                erro = "";

                if (string.IsNullOrEmpty(ContratoAdmID))
                {
                    erro = "ContratoAdmID não informado";
                    return false;
                }

                if (string.IsNullOrEmpty(AssociadoPjID))
                {
                    erro = "AssociadoPjID não informado";
                    return false;
                }

                if (string.IsNullOrEmpty(OperadoraID))
                {
                    erro = "OperadoraID não informado";
                    return false;
                }

                if (string.IsNullOrEmpty(FilialID))
                {
                    erro = "FilialID não informado";
                    return false;
                }

                if (string.IsNullOrEmpty(PlanoID))
                {
                    erro = "ContratoAdmID não PlanoID";
                    return false;
                }

                if (string.IsNullOrEmpty(PRODUTOR))
                {
                    erro = "PRODUTOR não informado";
                    return false;
                }

                if (string.IsNullOrEmpty(MATRICULA))
                {
                    erro = "MATRICULA não informada";
                    return false;
                }

                if (DT_ADMISSAO == DateTime.MinValue)
                {
                    erro = "DT_ADMISSAO não informada";
                    return false;
                }

                if (DT_VIGENCIA == DateTime.MinValue)
                {
                    erro = "DT_VIGENCIA não informada";
                    return false;
                }

                if (string.IsNullOrEmpty(CPF_TITULAR)) //todo: validar cpf
                {
                    erro = "CPF_TITULAR não informado";
                    return false;
                }

                if (string.IsNullOrEmpty(NOME_BENEFICIARIO))
                {
                    erro = "NOME_BENEFICIARIO não informado";
                    return false;
                }

                if (DT_NASCIMENTO == DateTime.MinValue)
                {
                    erro = "DT_NASCIMENTO não informada";
                    return false;
                }

                if (string.IsNullOrEmpty(CEP))
                {
                    erro = "CEP não informado";
                    return false;
                }

                if (string.IsNullOrEmpty(LOGRADOURO))
                {
                    erro = "LOGRADOURO não informado";
                    return false;
                }

                return true;
            }
        }
    }
}