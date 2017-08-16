using System;
using System.Web;
using System.Linq;
using System.Web.Services;
using System.Collections.Generic;

using LC.Web.PadraoSeguros.Entity;
using LC.Framework.Phantom;
using System.Configuration;
using System.Data;
using System.Text;

namespace MedProj.www.ws
{
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

            string boletoUrl = "", status = "";

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
                        catch(Exception ex)
                        {
                            pm.Rollback();

                            return retorno("erro", ex.Message);
                        }

                        //Regra 1: não pode gerar uma cobrança se não houver cobrança gerada no mês anterior
                        try
                        {
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
                                pm.Save(logErr01);

                                pm.Commit();

                                return retorno("erro", "Nao identificamos cobranca gerada na competencia anterior");
                            }
                        }
                        catch(Exception ex)
                        {
                            pm.Rollback();

                            return retorno("erro", ex.Message);
                        }

                        // Parcelas 2 em diante: só podem ser geradas entre o dia 1 do mes de vencimento e 
                        // 5 dias após a data de vencimento

                        // intervarlo permitido para geração de cobranças
                        DateTime inicioPeriodo = new DateTime(dataVencimento.Year, dataVencimento.Month, 1);
                        DateTime fimPeriodo = new DateTime(dataVencimento.AddDays(5).Year, dataVencimento.AddDays(5).Month, dataVencimento.AddDays(5).Day, 23, 59, 59, 990);

                        if (DateTime.Now < inicioPeriodo || DateTime.Now > fimPeriodo)
                        {
                            //Emissão fora do periodo permitido
                            //TODO: deve-se checar adimplencia?
                            //data.Dispose();
                            pm.Rollback();
                            return retorno("erro", "Emissao fora do periodo permitido que e de " + inicioPeriodo.ToString("dd/MM/yyyy") + " a " + fimPeriodo.ToString("dd/MM/yyyy"));
                        }

                        // Se a cobrança for emitida após o vencimento original, mas dentro do período permitido 
                        if (DateTime.Now.Day > dataVencimento.Day && DateTime.Now.Day <= fimPeriodo.Day)
                        {
                            //TODO: calcular juro e multa ?

                            DateTime novoVencimento = new DateTime(
                                DateTime.Now.AddDays(2).Year, 
                                DateTime.Now.AddDays(2).Month, 
                                DateTime.Now.AddDays(2).Day, 23, 59, 59, 900);

                            cobranca.DataVencimento = novoVencimento;
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

                    cobranca.Valor = (valorPorVida * Convert.ToDecimal(qtdVidas)) + acrescimoOuDesconto;
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
                    //
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

                    boletoUrl = this.BoletoURL(cobranca, cb, out status);
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

        string BoletoURL(Cobranca cobranca, ContratoBeneficiario titular, out string status)
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

            if (cobranca.Parcela == 1 || cobranca.CobrancaRefID == null)
                instrucoes = "0"; //AS COBERTURAS DO SEGURO E DEMAIS BENEFICIOS ESTAO CONDICIONADAS AO PAGAMENTO DESTE DOCUMENTO.quebraSR CAIXA, APOS O VENCIMENTO MULTA DE 10% E JUROS DE 1% A.D.quebraNAO RECEBER APOS 05 DIAS DO VENCIMENTO.
            else
                instrucoes = "1"; // "AS COBERTURAS DO SEGURO E DEMAIS BENEFICIOS ESTAO CONDICIONADAS AO PAGAMENTO DESTE DOCUMENTO.<br/>SR CAIXA, NAO RECEBER APOS O VENCIMENTO.";

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

        [WebMethod()]
        public string GerarBoleto_TESTE(string token, string idContrato, string qtdVidas, string vencimento)
        {
            token = "233478a4-d2a3-4514-b9c2-6c70f5c2e63d";
            idContrato = "206836";
            vencimento = "10/08/2017";
            qtdVidas = "6";

            if (token != this.TokenGuid) return retorno("erro", "Erro de autorizacao");

            string[] arr = vencimento.Split('/');
            if (arr.Length != 3)
            {
                return retorno("erro", "Data de vencimento não estava em um formato válido: dd/MM/yyyy");
            }

            DateTime dataVencimento = new DateTime(
                Convert.ToInt32(arr[2]), Convert.ToInt32(arr[1]), Convert.ToInt32(arr[0]), 23, 59, 59, 900);

            string boletoUrl = "", status = "";

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
                        //CobrancaLog.CobrancaCriadaLog logErr01 = new CobrancaLog.CobrancaCriadaLog();
                        //logErr01.CobrancaValor = 0;
                        //logErr01.PropostaID = idContrato;
                        //logErr01.DataEnviada = vencimento;
                        //logErr01.Vidas = toInt(qtdVidas);
                        //logErr01.Msg = "Vencimento ja pago";
                        //logErr01.Origem = (int)CobrancaLog.Fonte.WebService;
                        //pm.Save(logErr01);

                        //pm.Rollback();
                        //return retorno("erro", "Vencimento ja pago.");
                    }
                    #endregion

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

                    //string[] arr = vencimento.Split('/');

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
                            pm.Save(contrato);//////////////////////////////////
                        }
                    }



                    //salva a cobranca
                    Cobranca cobranca = new Cobranca();
                    cobranca.Parcela = ultimaParcela;
                    cobranca.DataVencimento = dataVencimento;
                    if (acrescimoOuDesconto > decimal.Zero) cobranca.AcrescimoDeContrato = acrescimoOuDesconto;
                    if (acrescimoOuDescontoTipo > -1) cobranca.AcrescimoDeContratoTipo = acrescimoOuDescontoTipo;

                    //TODO: denis COMENTAR linha abaixo!
                    //cobranca.Parcela = 1;////////////////////////

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
                                "       cobranca_cobrancaRefId is null and ",
                                "       cobranca_cancelada=0 and cobranca_propostaid=", contrato.ID,
                                "       and month(cobranca_dataVencimento)=", cobranca.DataVencimento.Month,
                                "       and year(cobranca_dataVencimento)=", cobranca.DataVencimento.Year);

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

                                pm.Rollback();

                                return retorno("erro", "Ja existe uma cobranca gerada para o periodo");
                            }
                        }
                        catch
                        {
                        }

                        //Regra 1: não pode gerar uma cobrança se não houver cobrança gerada no mês anterior
                        try
                        {
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
                                pm.Save(logErr01);

                                pm.Rollback();

                                return retorno("erro", "Nao identificamos cobranca gerada na competencia anterior");
                            }
                        }
                        catch
                        {
                        }

                        // Parcelas 2 em diante: só podem ser geradas entre o dia 1 do mes de vencimento e 
                        // 5 dias após a data de vencimento

                        // intervarlo permitido para geração de cobranças
                        DateTime inicioPeriodo = new DateTime(dataVencimento.Year, dataVencimento.Month, 1);
                        DateTime fimPeriodo = new DateTime(dataVencimento.AddDays(5).Year, dataVencimento.AddDays(5).Month, dataVencimento.AddDays(5).Day, 23, 59, 59, 990);

                        if (DateTime.Now < inicioPeriodo || DateTime.Now > fimPeriodo)
                        {
                            //Emissão fora do periodo permitido
                            //TODO: deve-se checar adimplencia?
                            //data.Dispose();
                            pm.Rollback();
                            return retorno("erro", "Emissao fora do periodo permitido que e de " + inicioPeriodo.ToString("dd/MM/yyyy") + " a " + fimPeriodo.ToString("dd/MM/yyyy"));
                        }

                        // Se a cobrança for emitida após o vencimento original, mas dentro do período permitido 
                        if (DateTime.Now.Day > dataVencimento.Day && DateTime.Now.Day <= fimPeriodo.Day)
                        {
                            //TODO: calcular juro e multa ?

                            DateTime novoVencimento = new DateTime(
                                DateTime.Now.AddDays(2).Year,
                                DateTime.Now.AddDays(2).Month,
                                DateTime.Now.AddDays(2).Day, 23, 59, 59, 900);

                            cobranca.DataVencimento = novoVencimento;
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

                    cobranca.Valor = (valorPorVida * Convert.ToDecimal(qtdVidas)) + acrescimoOuDesconto;
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

                    //
                    pm.Save(cobranca); //cobranca = new Cobranca(125); pm.Load(cobranca);//////////////////////////////////

                    //atualiza a qtd de vidas na proposta
                    string sql = string.Concat("update contrato set contrato_qtdVidas=", qtdVidas, " where contrato_id=", idContrato);
                    //
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
                        //
                        pm.Save(log);//////////////////////////////////
                    }
                    catch
                    {
                    }

                    pm.Rollback();

                    boletoUrl = this.BoletoURL(cobranca, cb, out status);
                }
                catch (Exception ex)
                {
                    pm.Rollback();
                    return retorno("erro", ex.Message);
                }

                return retorno(status, boletoUrl);
            }
        }

        [WebMethod()]
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


        [WebMethod()]
        public string DadosParaCartao(string codigoContrato, string matriculaBeneficiario, string token)
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
    }
}