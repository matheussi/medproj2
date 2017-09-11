namespace Servicos
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Drawing;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Collections.Generic;

    using MedProj.Entidades;
    using LC.Web.PadraoSeguros.Facade;
    using PSE = LC.Web.PadraoSeguros.Entity;
    using System.Data.OleDb;
    using System.Configuration;

    using NHibernate;
    using NHibernate.Linq;
    using NHibernate.Dialect;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using FluentNHibernate.Conventions.Helpers;
    using MedProj.Entidades.Map.NHibernate;
    using System.Data.SqlClient;
    using LC.Framework.Phantom;
    using System.Net.Mail;

    using LC.Framework.DataUtil;
    using LC.Framework.BusinessLayer;
    using System.Security.Cryptography;
using System.IO;

    public partial class Form1 : Form
    {
        private static NHibernate.Cfg.Configuration _config = null;

        bool processando = false; ////
        bool processandoKit = false;
        bool processandoMail = false;

        public Form1()
        {
            //Cryptography crypt = new Cryptography(CryptProvider.DES);
            //string encriptado = crypt.Encrypt("51");
            //string decri = crypt.Decrypt(encriptado);

            InitializeComponent();

            //string ra = "10218136065000124910800096604                                 00037208            175000372080             I06180714          00037208            000000000000000020010431070  000000000024000000000000000000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000   21071400000000000000000000000                                                                    B3000002";
            //string valor = ra.Substring(153, 12);
            //string nossonu = ra.Substring(62, 8);
            //string outrovalor = ra.Substring(254, 12);
            //string dataPgto = ra.Substring(110, 6);

            timerExportCartaoAgend.Enabled = true;
            timerImportAgend.Enabled = true;
            timerExportKitAgend.Enabled = true;

            timerImportAgend.Interval = 50000;
            timerExportKitAgend.Interval = 120000;
            timerExportCartaoAgend.Interval = 80000;

            button1.Visible = false;/////////////

            string num = ".6070870002616213".Replace(".", "");
            string via = num.Substring(14, 1);
            string dv = num.Substring(15, 1);
            string numero = num.Substring(0, 14);

            int i = 0;

            //bool ok = ArquivoBaixaFacade.Instance.ProcessarBaixa(10);

        }

        private void cmdImportAgendIniciar_Click(object sender, EventArgs e)
        {
            if (processando) return;

            this.runImportacaoAgendada();
        }

        private void timerImportAgend_Tick(object sender, EventArgs e)
        {
            if (processando) return;

            this.runImportacaoAgendada();
        }

        void runImportacaoAgendada()
        {
            AgendaImportacao agenda=null;
            try
            {
                processando = true;

                agenda = AgendaImportacaoFacade.Instancia.CarregarPendenteParaProcessamento(DateTime.Now);

                if (agenda == null)
                {
                    processando = false;
                    lblImportAgendStatus.Text = "Parado...";
                    return;
                }

                lblImportAgendStatus.Text = "Processando. Aguarde...";
                Application.DoEvents();

                ImportFacade facade = new ImportFacade();

                facade.Importar(agenda);

                lblImportAgendStatus.Text = "Parado...";
            }
            catch (Exception ex)
            {
                lblImportAgendStatus.Text = ex.Message;

                if (agenda != null)
                {
                    agenda.Ativa = false;
                    agenda.Erro = ex.Message;
                    AgendaImportacaoFacade.Instancia.Salvar(agenda);
                }
            }
            finally
            {
                processando = false;
            }
        }

        /******************************************************************************************************************/

        private void cmdExportAgendCartaoIniciar_Click(object sender, EventArgs e)
        {
            if (processando) return;

            this.runExportacaoCartaoAgendada();
        }

        void runExportacaoCartaoAgendada()
        {
            try
            {
                processando = true;

                AgendaExportacaoCartao agenda = AgendaExportacaoCartaoFacade.Instancia.CarregarPendenteParaProcessamento(DateTime.Now);

                if (agenda == null)
                {
                    processando = false;
                    lblExportAgendCartaoStatus.Text = "Parado...";
                    return;
                }

                lblExportAgendCartaoStatus.Text = "Processando. Aguarde...";
                Application.DoEvents();

                ImportFacade facade = new ImportFacade();

                facade.ExportarParaCartao(agenda);

                lblExportAgendCartaoStatus.Text = "Parado...";
            }
            catch (Exception ex)
            {
                lblExportAgendCartaoStatus.Text = ex.Message;
            }
            finally
            {
                processando = false;
            }
        }

        private void timerExportCartaoAgend_Tick(object sender, EventArgs e)
        {
            if (processando) return;

            this.runExportacaoCartaoAgendada();
        }

        /******************************************************************************************************************/

        private void cmdExportAgendKitIniciar_Click(object sender, EventArgs e)
        {
            if (processando) return;

            this.runExportacaoKitAgendada();
        }

        void runExportacaoKitAgendada()
        {
            try
            {
                processando = true;

                AgendaExportacaoKit agenda = AgendaExportacaoKitFacade.Instancia.CarregarPendenteParaProcessamento(DateTime.Now);

                if (agenda == null)
                {
                    processando = false; //processandoKit = false;
                    lblExportAgendKitStatus.Text = "Parado...";
                    return;
                }

                lblExportAgendKitStatus.Text = "Processando. Aguarde...";
                Application.DoEvents();

                ImportFacade facade = new ImportFacade();

                facade.ExportarParaKit(agenda);

                lblExportAgendKitStatus.Text = "Parado...";
            }
            catch (Exception ex)
            {
                lblExportAgendKitStatus.Text = ex.Message;
            }
            finally
            {
                processando = false;
            }
        }

        private void timerExportKitAgend_Tick(object sender, EventArgs e)
        {
            if (processando) return;

            this.runExportacaoKitAgendada();
        }

        /******************************************************************************************************************/

        private void tComunicacaoEmail_Tick(object sender, EventArgs e)
        {
            this.runComunicacaoEmail();
        }

        private void cmdComunicacaoEmail_Click(object sender, EventArgs e)
        {
            this.runComunicacaoEmail();
        }

        void runComunicacaoEmail()
        {
            if (processandoMail) return;

            try
            {
                processandoMail = true;
                DateTime agora = DateTime.Now;
                lblAvisosPorEmail.Text = "Processando";
                Application.DoEvents();

                using (var sessaoF = CriarSessaoNHibernate())
                {
                    using (var sessao = sessaoF.OpenSession())
                    {
                        #region aviso de pagamento recebido

                        using (ITransaction tran = sessao.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {
                            try
                            {
                                var configsPagto = sessao.Query<ConfigEmailAviso>()
                                    .Fetch(co => co.Texto)
                                    .FetchMany(c => c.Contratos)
                                    .Where(c => c.Ativo == true) 
                                    //.Where(c => c.Ativo == true && c.ID == 17) //TODO: denis comentar e descomentar acima     //-------------//.Where(c => c.Tipo == MedProj.Entidades.Enuns.TipoConfig.AvisoDePagamento && c.Ativo == true)
                                    .ToList();

                                int confgs = 0;
                                int registros = 0;

                                if (configsPagto != null && configsPagto.Count > 0)
                                {
                                    foreach (var config in configsPagto)
                                    {
                                        confgs++;

                                        if (config.Texto == null || string.IsNullOrEmpty(config.Texto.Texto)) continue;

                                        lblAvisosPorEmail.Text = "Configs: " + confgs.ToString() + " de " + configsPagto.Count.ToString();
                                        Application.DoEvents();

                                        lblAvisosPorEmail.Text = "Configs: " + configsPagto.Count.ToString();
                                        Application.DoEvents();

                                        agora = DateTime.Now;
                                        var lista = ConfigEmailFacade.Instancia.
                                            CarregarParaProcessamento(config, sessao, tran);

                                        if (lista != null && lista.Count > 0)
                                        {
                                            foreach (var vo in lista)
                                            {
                                                registros++;

                                                lblAvisosPorEmail.Text = "Configs: " + confgs.ToString() + " de " + configsPagto.Count.ToString() + "|| Registros: " + registros.ToString() + " de " + lista.Count.ToString();
                                                Application.DoEvents();

                                                ConfigEmailAvisoINSTANCIA inst = new ConfigEmailAvisoINSTANCIA();

                                                inst.CobrancaID = vo.CobrancaID;
                                                inst.ConfigID = config.ID;
                                                inst.Tipo = config.Tipo;
                                                inst.Data = agora;
                                                inst.Email = vo.BeneficiarioMAIL;
                                                if (string.IsNullOrWhiteSpace(inst.Email)) inst.MSG = "ERRO: nenhum e-mail fornecido.";
                                                else if (!string.IsNullOrWhiteSpace(vo.ERRO)) inst.MSG = vo.ERRO;

                                                inst.Tipo = config.Tipo;// MedProj.Entidades.Enuns.TipoConfig.AvisoDePagamento;

                                                sessao.Save(inst); 

                                                this.enviaEmailDeAviso(config, inst, vo); 
                                                System.Threading.Thread.Sleep(1000);
                                            }
                                        }
                                    }
                                }

                                tran.Commit();
                            }
                            catch
                            {
                                tran.Rollback();
                                throw;
                            }
                            finally
                            {
                            }
                        }

                        #endregion
                    }
                }

                processandoMail = false;
                lblAvisosPorEmail.Text = "Concluído: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                Application.DoEvents();
            }
            finally
            {
                processando = false;
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        void enviaEmailDeAviso(ConfigEmailAviso config, ConfigEmailAvisoINSTANCIA inst, InstanciaSrcVO vo)
        {
            if (!string.IsNullOrWhiteSpace(vo.BeneficiarioMAIL) && string.IsNullOrEmpty(vo.ERRO))
            {
                try
                {
                    MailMessage msg = new MailMessage(
                                new MailAddress(ConfigurationManager.AppSettings["mailFrom"], 
                                    ConfigurationManager.AppSettings["mailFromName"]),
                                new MailAddress("temp@temp.com"));

                    msg.To.Clear();

                    if (vo.BeneficiarioMAIL.IndexOf(';') > -1)
                    {
                        string[] arr = vo.BeneficiarioMAIL.Split(';');
                        foreach (string e in arr) { if (!string.IsNullOrEmpty(e)) { msg.To.Add(e); } }
                    }
                    else
                        msg.To.Add(vo.BeneficiarioMAIL);

                    //msg.To.Clear();
                    //msg.Bcc.Add("denis@wedigi.com.br");

                    msg.ReplyToList.Add("contato@clubeazul.org.br");

                    msg.Subject = "Clube Azul - Informação"; //TODO: parametrizar

                    msg.IsBodyHtml = true;
                    msg.Body = config.Texto.Texto  //config.Email
                        .Replace("[#NOME]", vo.BeneficiarioNM)
                        .Replace("[#VNCT]", vo.CobrancaDtVenct.ToString("dd/MM/yyyy"))
                        .Replace("[#VLOR]", vo.CobrancaValor.ToString("C"))
                        .Replace("[#CMPT]", vo.CobrancaDtVenct.AddMonths(-1).ToString("MM/yyyy"))
                        .Replace("[#QTDV]", vo.QtdVidas.ToString());

                    string linkBoleto = "";
                    if (msg.Body.IndexOf("[#LINK]") > -1)
                    {
                        string status = "";
                        linkBoleto = boletoURL(vo.PropostaID.ToString(), vo.CobrancaID.ToString(), out status);
                        msg.Body = msg.Body.Replace("[#LINK]", "<a target='_blank' href='" + linkBoleto + "'>").Replace("[#LINK/]", "</a>");
                    }

                    msg.Body = msg.Body.Replace("[#ELINK]", linkBoleto);

                    #region comentado
                    /*
                     string.Concat(
                        "<body style='font-family:calibri'>",
                        "<table>",
                        "<tr><td align='center' style='background-color:#E1E1E1'><img src='http://sispag.clubeazul.org.br/Images/clubeazul.png' alt='Clube Azul' title='Clube Azul' /></td></tr>",
                        "<tr><td>Olá, [#NOME].<br/><br/>Informamos que recebemos o pagamento referente o boleto com vencimento em [#VNCT], no valor de [#VLOR].<br/><br/>Atenciosamente,<br/>Clube Azul<br/><br/></td></tr>",
                        "<tr><td><font size='1'>Este é um e-mail automático, por favor não o responda</font></td></tr>",
                        "</table>",
                        "</body>")
                     */
                    #endregion


                    SmtpClient client = new SmtpClient();
                    client.Send(msg);
                    msg.Dispose();
                    client = null;
                }
                catch { }
            }
        }

        string boletoURL(string contratoId, string cobrancaId, out string status)
        {
            PSE.Contrato contrato = new PSE.Contrato(contratoId);
            contrato.Carregar();

            PSE.Cobranca cobranca = new PSE.Cobranca(cobrancaId);
            cobranca.Carregar();

            PSE.ContratoBeneficiario titular = PSE.ContratoBeneficiario.CarregarTitular(contratoId, null);

            cobranca.ContratoCodCobranca = contrato.CodCobranca.ToString();

            status = "ok";

            string nossoNumero = "", nome = ""; //, email = "";

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
            int dia = cobranca.DataVencimento.Day;
            int mes = cobranca.DataVencimento.Month;
            int ano = cobranca.DataVencimento.Year;

            String uri = "";
            String instrucoes = "";

            if (cobranca.Parcela == 1 || cobranca.CobrancaRefID == null)
                instrucoes = "0"; //AS COBERTURAS DO SEGURO E DEMAIS BENEFICIOS ESTAO CONDICIONADAS AO PAGAMENTO DESTE DOCUMENTO.quebraSR CAIXA, APOS O VENCIMENTO MULTA DE 10% E JUROS DE 1% A.D.quebraNAO RECEBER APOS 05 DIAS DO VENCIMENTO.
            else
                instrucoes = "1"; //AS COBERTURAS DO SEGURO E DEMAIS BENEFICIOS ESTAO CONDICIONADAS AO PAGAMENTO DESTE DOCUMENTO.<br/>SR CAIXA, NAO RECEBER APOS O VENCIMENTO.

            decimal Valor = cobranca.Valor;
            string end1 = "", end2 = "";

            IList<PSE.Endereco> enderecos = PSE.Endereco.CarregarPorDono(titular.BeneficiarioID, PSE.Endereco.TipoDono.Beneficiario);
            //IList<Endereco> enderecos = Endereco.CarregarPorDono(beneficiario.ID, Endereco.TipoDono.Beneficiario);
            if (enderecos != null && enderecos.Count > 0)
            {
                string compl = ""; if (!string.IsNullOrEmpty(enderecos[0].Complemento)) { compl = " - " + enderecos[0].Complemento; }

                end1 = string.Concat(enderecos[0].Logradouro, ", ", enderecos[0].Numero, compl);
                end2 = string.Concat(enderecos[0].CEP, " - ", enderecos[0].Bairro, " - ", enderecos[0].Cidade, " - ", enderecos[0].UF);
            }


            //uri = retiraAcentos(String.Concat("?nossonum=", nossoNumero, "&valor=", Valor.ToString("N2"), "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dia, "&v_mes=", mes, "&v_ano=", ano, "&numdoc2=", contrato.Numero.PadLeft(5, '0'), "&nome=", nome, "&cod_cli=", cobranca.ID, "&end1=", end1, "&end2=", end2, "&mailto=", email, "&instr=", instrucoes));//, ".<br><br>" + naoReceber));
            uri = string.Concat("bid=", titular.BeneficiarioID,
                "&contid=", contrato.ID, "&cobid=", cobranca.ID, "&instru=", instrucoes);

            string encripted = this.encrypt(uri);

            String finalUrl = "";

            if (contrato.Tipo == (int)PSE.eTipoPessoa.Juridica)
            {
                finalUrl = string.Concat(
                     ConfigurationManager.AppSettings["boleto2Url"], "?param=",                                          //"http://localhost/phpBoleto/boleto/boleto_itau.php?param=",
                     encripted);
            }
            else
            {
                finalUrl = string.Concat(
                     ConfigurationManager.AppSettings["boletoUrl"], "?param=",                                          //"http://localhost/phpBoleto/boleto/boleto_itau.php?param=",
                     encripted);
            }

            status = "ok";
            return finalUrl;
        }

        string retiraAcentos(String Texto)
        {
            if (String.IsNullOrEmpty(Texto)) { return Texto; }
            String comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            String semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
                Texto = Texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());

            return Texto.Replace("'", "");
        }
        static string encryptBetweenPHP(string param)
        {
            byte[] key = Encoding.UTF8.GetBytes("passwordDR0wSS@P6660juht");
            byte[] iv = Encoding.UTF8.GetBytes("password");
            byte[] data = Encoding.UTF8.GetBytes(param);
            byte[] enc = new byte[0];
            TripleDES tdes = TripleDES.Create();
            tdes.IV = iv;
            tdes.Key = key;
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.Zeros;
            ICryptoTransform ict = tdes.CreateEncryptor();
            enc = ict.TransformFinalBlock(data, 0, data.Length);
            return bin2Hex(enc);
        }
        static string bin2Hex(byte[] bin)
        {
            StringBuilder sb = new StringBuilder(bin.Length * 2);
            foreach (byte b in bin)
            {
                sb.Append(b.ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        string encrypt(string stringToEncrypt)
        {
            byte[] key = { };
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xab, 0xcd, 0xef };
            string sEncryptionKey = "#R3@2o16";

            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(sEncryptionKey);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms,
                  des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /******************************************************************************************************************/
        /******************************************************************************************************************/
        /******************************************************************************************************************/

        private void cmdProcsXPrestadores_Click(object sender, EventArgs e)
        {
            this.runAtribuicaoProcedimentosPrestadores();
        }

        void runAtribuicaoProcedimentosPrestadores()
        {
            if (processando) return;

            processando = true;

            List<AgendaAtribuicaoProcedimento> agenda = null;

            try
            {
                agenda = AgendaAtribuicaoProcedimentoFacade.Instancia.Carregar(DateTime.Now.AddDays(-30), DateTime.Now, MedProj.Entidades.Enuns.AgendaStatus.Pendente);

                //TODO: denis comentar 2 linhas abaixo
                //agenda = new List<AgendaAtribuicaoProcedimento>();
                //agenda.Add(AgendaAtribuicaoProcedimentoFacade.Instancia.Carregar(438));
                /////////////////////////////////////////////////

                if (agenda == null || agenda.Count == 0) return;
                ImportFacade.Instance.ImportarProcedicedimentosParaUnidades(agenda[0]);
            }
            catch (Exception ex)
            {
                string erro = ex.Message;
                if (ex.InnerException != null) erro = ex.InnerException.Message;

                if (agenda != null && agenda.Count > 0)
                {
                    agenda[0].Erro = erro;
                    agenda[0].Ativa = false;
                    if (agenda[0].DataConclusao.HasValue == false) agenda[0].DataConclusao = DateTime.Now;
                    AgendaAtribuicaoProcedimentoFacade.Instancia.Salvar(agenda[0]);
                }
            }
            finally
            {
                processando = false;
            }
        }

        private void tProcsPrestadores_Tick(object sender, EventArgs e)
        {
            this.runAtribuicaoProcedimentosPrestadores();
        }

        /******************************************************************************************************************/
        /******************************************************************************************************************/
        /******************************************************************************************************************/


        private void button1_Click(object sender, EventArgs e)
        {
            processando = true;

            //////Para importar prodecimentos 
            ////Int64[] ids = new Int64[] { 120 }; //new Int64[] { 23, 24, 25, 26, 27, 28, 29 };
            ////ImportFacade.Instance.ImportarProcedicedimentosParaUnidades(ids);
            //var agenda = AgendaAtribuicaoProcedimentoFacade.Instancia.Carregar(DateTime.Now.AddDays(-30), DateTime.Now, MedProj.Entidades.Enuns.AgendaStatus.Pendente);
            //if (agenda == null) return;
            //ImportFacade.Instance.ImportarProcedicedimentosParaUnidades(agenda[0]);

            //////Para importar procedimentos ODNTO - caso excepcional
            ////importaProcedimentosODONTO();

            ////cancelarImportacao();

            //this.arrumaProcedimentosOdonto();

            //this.importarMASSA();
            //importarAssociadoPJ_e_ContratosADM_ArrumaDOC();

            //importarAssociadoPJ_e_ContratosADM();

            //this.arrumarNumerosDuplicados();

            //arrumaIdsDeRelacionamentoUnidadeProcedimento();

            //arrumaCoordenadas();

            //exportacaoFormatoLogCartao();

            //arrumaFones();
            //geraLogsDeCobranca();

            //desfazImportacao();


            processando = false;
        }

        void arrumarNumerosDuplicados()
        {
            long maiorNumero = 0;
            string ids = "", aux = "";

            NumeroCartao cartao = null;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection("Server=SQL5.IPHOTEL.COM.BR,9104;database=dados_791;user id=clubeazul;pwd=d05be078;Pooling=False"))
            {
                conn.Open();

                using (System.Data.SqlClient.SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (System.Data.SqlClient.SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = trans;
                            //cmd.CommandText = "select contrato_id,contrato_numero,contrato_numeroid,contrato_data,contrato_importado,contrato_estipulanteid,contrato_contratoadmid,contrato_cancelado,estipulante_descricao from contrato  inner join estipulante on estipulante_id=contrato_estipulanteid where contrato_numero in ('6370870001216211','6370870001269510','6370870010463310','6370870001217110','6370870001169710','6370870001225210','6370870003601716','6370870001266812','6370870001268610','637087000115119','6370870008426815','6370870010454419','6370870008426114','6370870001201818','6370870010473316','6370870001252218','6370870001172117','6370870003641017','6370870001152310','637087008426513','6370870001221614','6370870010471518','6370870001207212','6370870001204515','6370870010452610','6370870002534918','6370870001212615','6370870001178611','6370870008426610','630870001163010','6370870001201710','6370870001244118','6370870001147910','6370870001857411','6370870005963917','6370870006214713','6370870001196610','6370870001546610','6370870001223510','6370870003966218','6370870003276815','6370870001214413','6370870001232411','6370870001150610','6370870001198418','6370870001222513','6370870003639012','6370870005967319','637087000129917','6370870001313110','6370870008425819','6370870006186310','6370870001241410','6370870001239815','6370870001162219','6370870001223412','6370870001194811','6370870002535019','637087001179510','6370870001142510','6370870010472417','6370870001781512','6370870004389311','6370870001170319','6370870001206313','6370870001195710','6370870001163118','6370870010464317','670870002551413','6370870001181116','6370870001242310','6370870001151411','6370870001209010','6370870008425517','6370870010463418','6370870008427412','6370870001148917','637087000119831','6370870001166915','6370870008426319','6370870001227019','6370870008427110','6370870001211716','6370870003474218','6370870001231610','6370870001213514','6370870001157916','637087000314015','6370870001171110','6370870001240511','6370870003398112','6370870004616911','6370870010462519','6370870008425215','6370870001168713','6370870001143311','6370870001203616','637080002108510','6370870010453510','6370870001164017','6370870005960616','6370870008425410','6370870010199915','6370870001235119','6370870001210817','6370870001218010','6370870001160410','370870001256914','6370870001228910','6370870008425118','6370870001182015','6370870001271115','6370870001149816','6370870001145110','6370870004902817','6370870001267711','637087000126127','6370870005485411','6370870001245017','6370870001243219','6370870008427218','6370870010586919','6370870001173016','6370870001260318','6370870005486612','6370870001262116','6370870001230613','637870001150512','6370870008427013','6370870001257813','6370870001258712','6370870001153210','6370870001266910','6370870008427315','6370870001258810','6370870001251319','6370870003192913','6370870008426718','6370870001169612','6370870008425614','6370870001159714','6370870001175914','6370870008425916','6370870001238916','6370870001180217','6370870001146019','637087000124410','6370870002421617','6370870005418412','6370870001270216','6370870010451711','6370870001141718','6370870001193912','6370870001220715','6370870001142412','6370870001167814','637087000144210','6370870001189419','6370870001253117','6370870008426912','6370870010458015','6370870001158815','6370870008425711','6370870002005117','6370870001231512','6370870002553017','637080008426211','6370870001236018','6370870000935010','6370870001191014','6370870004933810','6370870005291811','6370870001259611','6370870001190115','6370870001234210','6370870010461610','670870001197519','6370870001254016','6370870003803912','6370870010471410','6370870001177810','6370870001233310','6370870001272014','6370870003768319','6370870005944211','6370870001171218','6370870001224311','6370870001155018','6370870001205414','6370870001263015','6370870003443819','6370870001265913','6370870008426416','6370870001252110','6370870001250410','637087000124713','6370870001215410','6370870001208111','6370870001161310','6370870010457116','6370870001199317','6370870008425312','6370870008426017','6370870001200919','6370870003406310','637087003213210','6370870001177712','6370870000783619','6370870001176813','6370870001260210','6370870001247915','6370870008444511','6370870001202717','6370870001215312','6370870001248814','6370870001207310','637087004607319','6370870001226110','6370870001976917','6370870008427510','6370870003722513') and contrato_cancelado=0 and contrato_importado=0 order by contrato_numero, contrato_data asc ";
                            cmd.CommandText = "select contrato_id,contrato_numero,contrato_numeroid,contrato_data,contrato_importado,contrato_estipulanteid,contrato_contratoadmid,contrato_cancelado,estipulante_descricao from contrato  inner join estipulante on estipulante_id=contrato_estipulanteid where contrato_numero in ('6370870001249713','6370870001163010','6370870001244010','6370870001144210','6370870001197519','6370870001179510','6370870001229917','6370870001150512','6370870001154119','6370870001198310','6370870001261217','6370870001256914') and contrato_cancelado=0 and contrato_importado=0 order by contrato_numero, contrato_data asc ";
                            System.Data.SqlClient.SqlDataAdapter adp = new System.Data.SqlClient.SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adp.Fill(dt);

                            foreach (DataRow row in dt.Rows)
                            {
                                cmd.CommandText = "SELECT MAX(numerocontrato_numero) FROM numero_contrato";
                                maiorNumero = Convert.ToInt64(cmd.ExecuteScalar()) + 1;

                                //arruma o numero de cartao
                                cmd.CommandText = "select * from numero_contrato where numerocontrato_id=" + row["contrato_numeroId"];
                                using (System.Data.SqlClient.SqlDataReader dr = cmd.ExecuteReader())
                                {
                                    if (dr.Read())
                                    {
                                        cartao = new NumeroCartao();
                                        //////cartao.CV = Convert.ToString(dr["numerocontrato_cv"]);
                                        //////cartao.DV = Convert.ToInt32(dr["numerocontrato_dv"]);
                                        cartao.ID = Convert.ToInt64(dr["numerocontrato_id"]);
                                        cartao.Numero = Convert.ToString(dr["numerocontrato_numero"]);
                                        cartao.Via = Convert.ToInt32(dr["numerocontrato_via"]);
                                        dr.Close();
                                    }
                                    else
                                    {
                                        throw new ApplicationException();
                                    }
                                }

                                cartao.Numero = maiorNumero.ToString();
                                cartao.GerarDigitoVerificador();

                                cmd.CommandText = "update numero_contrato set numerocontrato_numero='" + cartao.Numero + "', numerocontrato_dv=" + cartao.DV.ToString() + " where numerocontrato_id=" + cartao.ID.ToString();
                                cmd.ExecuteNonQuery();

                                aux = cartao.Numero + cartao.Via.ToString() + cartao.DV.ToString();
                                cmd.CommandText = "update contrato set contrato_numero='" + aux + "' where contrato_id=" + row["contrato_id"];
                                cmd.ExecuteNonQuery();

                                if (ids == "") ids = Convert.ToString(row["contrato_id"]);
                                else ids += "," + Convert.ToString(row["contrato_id"]);
                            }
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        void cancelarImportacao()
        {
            long importacaoId = 10;

            long contratoId = 0, beneficiarioId = 0, enderecoId = 0, numero_contratoId = 0;

            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection("Server=SQL5.IPHOTEL.COM.BR,9104;database=dados_791;user id=clubeazul;pwd=d05be078;Pooling=False"))
            {
                conn.Open();

                using (System.Data.SqlClient.SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        using (System.Data.SqlClient.SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = trans;
                            cmd.CommandText = "select * from importacao_log where importacaolog_status=0 and importacaolog_agendaId = " + importacaoId.ToString();
                            System.Data.SqlClient.SqlDataAdapter adp = new System.Data.SqlClient.SqlDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adp.Fill(dt);

                            foreach (DataRow row in dt.Rows)
                            {
                                cmd.CommandText = "select contratobeneficiario_contratoId from contrato_beneficiario where contratobeneficiario_id=" + row["importacaolog_titularId"];
                                contratoId = Convert.ToInt64(cmd.ExecuteScalar());

                                cmd.CommandText = "select contratobeneficiario_beneficiarioId from contrato_beneficiario where contratobeneficiario_id=" + row["importacaolog_titularId"];
                                beneficiarioId = Convert.ToInt64(cmd.ExecuteScalar());

                                cmd.CommandText = "select contrato_enderecoReferenciaId from contrato where contrato_id=" + contratoId.ToString();
                                enderecoId = Convert.ToInt64(cmd.ExecuteScalar());

                                cmd.CommandText = "select contrato_numeroId from contrato where contrato_id=" + contratoId.ToString();
                                numero_contratoId = Convert.ToInt64(cmd.ExecuteScalar());

                                //deleta contrato_beneficiario, contrato, endereco, beneficiario, numero_contrato

                                cmd.CommandText = "delete from contrato_beneficiario where contratobeneficiario_id = " + row["importacaolog_titularId"];
                                cmd.ExecuteNonQuery();

                                cmd.CommandText = "delete from contrato where contrato_id = " + contratoId;
                                cmd.ExecuteNonQuery();

                                cmd.CommandText = "delete from endereco where endereco_id = " + enderecoId;
                                cmd.ExecuteNonQuery();

                                cmd.CommandText = "delete from beneficiario where beneficiario_id = " + beneficiarioId;
                                cmd.ExecuteNonQuery();

                                cmd.CommandText = "delete from numero_contrato where numerocontrato_id = " + numero_contratoId;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        void importaProcedimentos()
        {
            DataTable dt = new DataTable();

            string accTabela = "PorEspecialidade"; //  PorEspecialidade odonto;
            long tabelaProcId = 3; //3 7 ;

            string accConn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\procedimentos.accdb;Persist Security Info=False;";
            using (OleDbConnection conn = new OleDbConnection(accConn))
            {
                OleDbDataAdapter adp = new OleDbDataAdapter("select * from " + accTabela, conn);
                adp.Fill(dt);
                conn.Close();
                adp.Dispose();
            }

            var _sessionFactory = CriarSessaoNHibernate();
            using (ISession sessao = _sessionFactory.OpenSession())
            {
                sessao.FlushMode = FlushMode.Commit;

                string categoria = "";

                foreach (DataRow row in dt.Rows)
                {
                    if (row["Categoria"] != DBNull.Value && row["Categoria"] != null && Convert.ToString(row["Categoria"]).Trim() != "")
                    {
                        categoria = Convert.ToString(row["Categoria"]).Trim();
                    }

                    Procedimento proc = new Procedimento();
                    proc.CH = Convert.ToDecimal(row["valor"]);
                    proc.Codigo = Convert.ToString(row["codigo"]);
                    proc.Nome = Convert.ToString(row["procedimento"]);
                    proc.Porte = "";
                    proc.Tabela = new TabelaProcedimento();
                    proc.Tabela.ID = tabelaProcId;
                    proc.Categoria = categoria;
                    proc.Especialidade = Convert.ToString(row["Especialidade"]).Trim();

                    sessao.Save(proc);
                }
            }

            dt.Dispose();

            MessageBox.Show("OK");
        }

        void importaProcedimentosODONTO()
        {
            DataTable dt = new DataTable();

            string accTabela = "odonto2"; //  PorEspecialidade odonto;
            long tabelaProcId = 7; //3 7 ;

            string accConn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\procedimentos2.accdb;Persist Security Info=False;";
            using (OleDbConnection conn = new OleDbConnection(accConn))
            {
                OleDbDataAdapter adp = new OleDbDataAdapter("select * from " + accTabela, conn);
                adp.Fill(dt);
                conn.Close();
                adp.Dispose();
            }

            var _sessionFactory = CriarSessaoNHibernate();
            using (ISession sessao = _sessionFactory.OpenSession())
            {
                sessao.FlushMode = FlushMode.Commit;

                string categoria = "ODONTOLOGIA", especialidade = "ODONTOLOGIA";

                foreach (DataRow row in dt.Rows)
                {
                    Procedimento proc = new Procedimento();
                    proc.CH = Convert.ToDecimal(row["valor_01"]);
                    proc.Codigo = Convert.ToString(row["codigo"]);
                    proc.Nome = Convert.ToString(row["procedimento"]);
                    proc.Porte = "";
                    proc.Tabela = new TabelaProcedimento();
                    proc.Tabela.ID = tabelaProcId;
                    proc.Categoria = categoria;
                    proc.Especialidade = especialidade;

                    sessao.Save(proc);
                }
            }

            dt.Dispose();

            MessageBox.Show("OK");
        }

        void importarMASSA()
        {
            ContratoFacade.Instance.___atualizaSenhas2(); //ContratoFacade.Instance.___atualizaSaldo_FINAL(); ; //ContratoFacade.Instance.___ImportarFUNC();
        }

        void importarAssociadoPJ_e_ContratosADM()
        {
            processando = true;

            DataTable dtAssociadoPJ = new DataTable();
            DataTable dtContratoADM = new DataTable();

            string accTabela = "dados"; //  PorEspecialidade odonto;

            string accConn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\associadospj_contratosadm2.accdb;Persist Security Info=False;";
            using (OleDbConnection conn = new OleDbConnection(accConn))
            {
                OleDbDataAdapter adp = new OleDbDataAdapter("select distinct(ASSOCIADO_PJ) from " + accTabela, conn);
                adp.Fill(dtAssociadoPJ);

                adp.SelectCommand.CommandText = "select * from " + accTabela;
                adp.Fill(dtContratoADM);

                conn.Close();
                adp.Dispose();
            }

            

            var _sessionFactory = CriarSessaoNHibernate();
            using (ISession sessao = _sessionFactory.OpenSession())
            {
                sessao.FlushMode = FlushMode.Commit;

                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        int i = 0;
                        foreach (DataRow row in dtAssociadoPJ.Rows)
                        {
                            i++;

                            AssociadoPJ pj = sessao.Query<AssociadoPJ>().
                                Where(a => a.Nome == Convert.ToString(row[0])).FirstOrDefault();

                            if (pj == null)
                            {
                                pj = new AssociadoPJ();
                                pj.Nome = Convert.ToString(row[0]);
                                pj.DataValidadeFixa = DateTime.Now.AddYears(5);
                            }

                            DataRow[] ret = dtContratoADM.Select("ASSOCIADO_PJ = '" + pj.Nome + "'");
                            pj.Radical = Convert.ToString(ret[0]["Radical"]);

                            //if (pj.ID >= 10016)
                            //{
                            //    if (pj.TipoDataValidade == MedProj.Entidades.Enuns.TipoDataValidade.Indefinido)
                            //    {
                            //        pj.DataValidadeFixa = DateTime.Now.AddYears(5);
                            //    }
                            //}

                            //sessao.SaveOrUpdate(pj);

                            foreach (DataRow contratoadmRow in ret)
                            {
                                #region contrato adm

                                ContratoADM contrato = sessao.Query<ContratoADM>()
                                    .Where(a => a.Descricao == Convert.ToString(contratoadmRow["CLIENTE"]) && a.AssociadoPJ.ID == pj.ID)
                                    .FirstOrDefault();

                                Plano plano = null;

                                if (contrato == null)
                                {
                                    contrato = new ContratoADM();
                                    contrato.AssociadoPJ = pj;
                                    contrato.Ativo = true;
                                    contrato.Descricao = Convert.ToString(contratoadmRow["CLIENTE"]);
                                    contrato.Operadora = new Operadora();
                                    contrato.Operadora.ID = 3;
                                    sessao.Save(contrato);

                                    plano = new Plano();
                                    plano.ContratoAdm = contrato;
                                    plano.Data = DateTime.Now;
                                    plano.Descricao = "Padrão";
                                    plano.QuartoComum = true;

                                    sessao.Save(plano);
                                }
                                else
                                {
                                    plano = sessao.Query<Plano>()
                                        .Fetch(p => p.ContratoAdm)
                                        .Where(p => p.ContratoAdm.ID == contrato.ID).FirstOrDefault();

                                    if (plano == null)
                                    {
                                        plano = new Plano();
                                        plano.ContratoAdm = contrato;
                                        plano.Data = DateTime.Now;
                                        plano.Descricao = "Padrão";
                                        plano.QuartoComum = true;

                                        sessao.Save(plano);
                                    }
                                }

                                #endregion
                            }
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            dtAssociadoPJ.Dispose();
            dtContratoADM.Dispose();

            MessageBox.Show("OK");
        }

        void importarAssociadoPJ_e_ContratosADM_ArrumaDOC()
        {
            processando = true;

            DataTable dtAssociadoPJ = new DataTable();
            DataTable dtContratoADM = new DataTable();

            string accTabela = "dados"; //  PorEspecialidade odonto;

            string accConn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\associadospj_contratosadm.accdb;Persist Security Info=False;";
            using (OleDbConnection conn = new OleDbConnection(accConn))
            {
                OleDbDataAdapter adp = new OleDbDataAdapter("select distinct(ASSOCIADO_PJ) from " + accTabela, conn);
                adp.Fill(dtAssociadoPJ);

                adp.SelectCommand.CommandText = "select * from " + accTabela;
                adp.Fill(dtContratoADM);

                conn.Close();
                adp.Dispose();
            }

            ContratoFacade.Instance.___importarAssociadoPJ_e_ContratosADM_ArrumaDOC(dtContratoADM);


            dtAssociadoPJ.Dispose();
            dtContratoADM.Dispose();

            MessageBox.Show("OK");
        }

        /// <summary>
        /// para setar o id do procedimento da tabela de ODONTO, removendo os da tabela Saude
        /// </summary>
        public void arrumaProcedimentosOdonto()
        {
            processando = true;

            using (var sessaoF = CriarSessaoNHibernate())
            {
                using (var sessao = sessaoF.OpenSession())
                {
                    List<Procedimento> lista = sessao.Query<Procedimento>()
                        .Where(p => p.Especialidade == "Odontologia" && p.Tabela.ID == 3)
                        .ToList();

                    //sessao.Connection.Open();
                    using (IDbCommand cmd = sessao.Connection.CreateCommand())
                    {
                        long idProcTabelaCerta = 0;

                        foreach (var proc in lista)
                        {
                            cmd.CommandText = string.Concat("select ID from procedimento where tabela_id=7 and codigo='", proc.Codigo, "'");
                            idProcTabelaCerta = toLong(cmd.ExecuteScalar());

                            if (idProcTabelaCerta == 0) continue;

                            //atualiza as unidades que utilizam o procedimento da tabela errada para a certa
                            cmd.CommandText = string.Concat("update unidade_procedimento set Procedimento_ID=",
                                idProcTabelaCerta, " where Procedimento_ID=", proc.ID);
                            cmd.ExecuteNonQuery();

                            //atualiza os atendimentos que utilizam o procedimento da tabela errada para a certa
                            cmd.CommandText = string.Concat("update atendimentoCred_procedimento set atendimentocredproc_procedimentoId=",
                                idProcTabelaCerta, " where atendimentocredproc_procedimentoId=", proc.ID);
                            cmd.ExecuteNonQuery();

                            //deleta o procedimento que está na tabela errada
                            cmd.CommandText = string.Concat("delete from procedimento where ID=", proc.ID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    sessao.Connection.Close();
                    sessao.Connection.Dispose();
                }
            }
        }

        long toLong(object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return 0;
            else
                return Convert.ToInt64(param);
        }

        static ISessionFactory CriarSessaoNHibernate()
        {
            string connString = ConfigurationManager.ConnectionStrings["Contexto"].ConnectionString;

            var f = Fluently.Configure().Database((MsSqlConfiguration.MsSql2008.Dialect<MsSql2008Dialect>().ConnectionString(connString).ShowSql()))
                    .ExposeConfiguration(p => p.Properties["command_timeout"] = "1000") //timeout
                    .ExposeConfiguration(p => p.Properties["hibernate.cache.use_query_cache"] = "false") //sem cache 
                    .ExposeConfiguration(x => x.SetInterceptor(new SqlStatementInterceptor()))
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<UsuarioMap>().Conventions.Setup(x => x.Add(AutoImport.Never())));

            _config = f.BuildConfiguration();
            return f.BuildSessionFactory();
        }


        /******************************************************************************************************************/
        /******************************************************************************************************************/
        /******************************************************************************************************************/

        private void cmdArquivosDePagamento_Click(object sender, EventArgs e)
        {
            this.runArquivosDePagamentos();
        }

        void runArquivosDePagamentos()
        {
            if (processando) return;

            processando = true;

            try
            {
                List<AgendaPagamento> agendas = AgendaPagamentoFacade.Instancia.CarregarPendentes();

                if (agendas != null && agendas.Count > 0)
                    AgendaPagamentoFacade.Instancia.Processar(agendas[0]);
            }
            catch
            {
            }
            finally
            {
                processando = false;
            }
        }

        private void tArquivoPagamento_Tick(object sender, EventArgs e)
        {
            this.runArquivosDePagamentos();
        }



        void arrumaIdsDeRelacionamentoUnidadeProcedimento()
        {
            object aux = null, aux2 = null; int ret = 0;
            using (SqlConnection conn = new SqlConnection("Server=SQL5.IPHOTEL.COM.BR,9104;database=dados_791;user id=clubeazul;pwd=d05be078"))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                SqlDataAdapter adp = new SqlDataAdapter("select * from unidade_procedimento2 where unidade_id is not null", conn);
                DataTable dt = new DataTable();
                adp.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    cmd.CommandText = "select id from unidade_procedimento where unidade_id is null and ID=" + row["ID"];
                    aux = cmd.ExecuteScalar();

                    if (aux != null && aux != DBNull.Value)
                    {
                        //achou unidade. agora, verifica se essa unidade ja nao tem outra linha com esse procedimento
                        cmd.CommandText = string.Concat("select id from unidade_procedimento where procedimento_id=",
                            row["Procedimento_ID"], " and Unidade_ID=", row["Unidade_ID"]);

                        aux2 = cmd.ExecuteScalar();

                        if (aux2 == null || aux2 == DBNull.Value)
                        {
                            //não achou outra linha para essa unidade, com esse mesmo procedimento, entao atualiza
                            cmd.CommandText = string.Concat("update unidade_procedimento set Unidade_ID=",
                                row["Unidade_ID"], " where ID = ", row["ID"]);

                            //ret = cmd.ExecuteNonQuery();
                        }
                    }
                }

            //object aux = null, aux2 = null;
            //using (SqlConnection conn = new SqlConnection("Server=SQL5.IPHOTEL.COM.BR,9104;database=dados_791;user id=clubeazul;pwd=d05be078"))
            //{
            //    conn.Open();
            //    SqlCommand cmd = conn.CreateCommand();
            //    SqlDataAdapter adp = new SqlDataAdapter("select * from unidade_procedimento where unidade_id is null", conn);
            //    DataTable dt = new DataTable();
            //    adp.Fill(dt);

            //    foreach (DataRow row in dt.Rows)
            //    {
            //        cmd.CommandText = "select unidade_id from unidade_procedimento__3 where ID=" + row["ID"];
            //        aux = cmd.ExecuteScalar();

            //        if (aux != null && aux != DBNull.Value)
            //        {
            //            //achou unidade. agora, verifica se essa unidade ja nao tem outra linha com esse procedimento
            //            cmd.CommandText = string.Concat("select id from unidade_procedimento where procedimento_id=",
            //                row["Procedimento_ID"], " and Unidade_ID=", aux);

            //            aux2 = cmd.ExecuteScalar();

            //            if (aux2 == null || aux2 == DBNull.Value)
            //            {
            //                //não achou outra linha para essa unidade, com esse mesmo procedimento, entao atualiza
            //                cmd.CommandText = string.Concat("update unidade_procedimento set Unidade_ID=", 
            //                    aux, " where ID = ", row["ID"]);
            //            }
            //        }
            //    }
            }

            //List<string> ids = new List<string>();
            //using (var sessaoF = CriarSessaoNHibernate())
            //{
            //    using (var sessao = sessaoF.OpenSession())
            //    {
            //        using (IDbCommand cmd = sessao.Connection.CreateCommand())
            //        {
            //            cmd.CommandText = "select * from unidade_procedimento where unidade_id is null";

            //            using (IDataReader dr = cmd.ExecuteReader())
            //            {
            //                while (dr.Read())
            //                {
            //                    ids.Add(Convert.ToString(dr[0]));
            //                }
            //                dr.Close();
            //            }
            //        }
            //    }
            //}
        }

        public void arrumaCoordenadas()
        {
            using (var sessaoF = CriarSessaoNHibernate())
            {
                using (var sessao = sessaoF.OpenSession())
                {
                    var lista = sessao.Query<PrestadorUnidade>()
                        //.Where(p => p.Longitude == 0)
                        .ToList();

                    if(lista != null)
                    {
                        using (ITransaction tran = sessao.BeginTransaction())
                        {
                            using (IDbCommand cmd = sessao.Connection.CreateCommand())
                            {
                                tran.Enlist(cmd);

                                foreach (PrestadorUnidade pu in lista)
                                {
                                    if (pu.Latitude != 0) continue;

                                    pu.SetaCoordenadas();

                                    cmd.CommandText = "update prestador_unidade set latitude='" + pu.Latitude.ToString().Replace(",", ".")
                                        + "', Longitude='" + pu.Longitude.ToString().Replace(",",".") + "' where ID=" + pu.ID.ToString();

                                    cmd.ExecuteNonQuery();
                                }
                            }

                            tran.Commit();
                        }
                    }
                }
            }
        }

        public void exportacaoFormatoLogCartao()
        {
            long associadoPjID = 10020; //estipulante 

            StringBuilder sb = new StringBuilder();

            using (var sessaoF = CriarSessaoNHibernate())
            {
                using (var sessao = sessaoF.OpenSession())
                {
                    var contratos = sessao.Query<Contrato>()
                        .Where(c => c.EstipulanteID == associadoPjID)
                        .ToList();

                    if (contratos == null) return;

                    Endereco end = null;
                    Beneficiario b = null;
                    ContratoADM adm = null;
                    NumeroCartao cartao = null;
                    ContratoBeneficiario cb = null;

                    #region datatable 

                    sb.Append("<table>");
                    sb.Append("<tr>");


                    DataTable dt = new DataTable();
                    dt.Columns.Add("CARTAO"); sb.Append("<td>CARTAO</td>");
                    dt.Columns.Add("ABREVIADO"); sb.Append("<td>ABREVIADO</td>");
                    dt.Columns.Add("RG"); sb.Append("<td>RG</td>"); //////////////////////

                    dt.Columns.Add("PRODUTO"); sb.Append("<td>PRODUTO</td>");
                    dt.Columns.Add("VIA"); sb.Append("<td>VIA</td>");
                    dt.Columns.Add("CVV"); sb.Append("<td>CVV</td>");
                    dt.Columns.Add("VALIDADE"); sb.Append("<td>VALIDADE</td>");
                    dt.Columns.Add("SENHA"); sb.Append("<td>SENHA</td>");


                    dt.Columns.Add("NOME_BENEFICIARIO"); sb.Append("<td>NOME_BENEFICIARIO</td>");

                    dt.Columns.Add("CONTRATOPJ"); sb.Append("<td>CONTRATOPJ</td>"); //////////////


                    dt.Columns.Add("CPF_TITULAR"); sb.Append("<td>CPF_TITULAR</td>");
                    dt.Columns.Add("DT_NASCIMENTO"); sb.Append("<td>DT_NASCIMENTO</td>");

                    dt.Columns.Add("RAMO"); sb.Append("<td>RAMO</td>");
                    dt.Columns.Add("APOLICE"); sb.Append("<td>APOLICE</td>");

                    dt.Columns.Add("INICIO_DO_RISCO"); sb.Append("<td>INICIO_DO_RISCO</td>");///////////////
                    dt.Columns.Add("FIM_DA_VIGENCIA"); sb.Append("<td>FIM_DA_VIGENCIA</td>");//////////////
                    dt.Columns.Add("DATA_DE_EMISSAO"); sb.Append("<td>DATA_DE_EMISSAO</td>");//////////////

                    dt.Columns.Add("LOGRADOURO"); sb.Append("<td>LOGRADOURO</td>");
                    dt.Columns.Add("NUMERO"); sb.Append("<td>NUMERO</td>");
                    dt.Columns.Add("COMPLEMENTO"); sb.Append("<td>COMPLEMENTO</td>");
                    dt.Columns.Add("BAIRRO"); sb.Append("<td>BAIRRO</td>");
                    dt.Columns.Add("CEP"); sb.Append("<td>CEP</td>");
                    dt.Columns.Add("CIDADE"); sb.Append("<td>CIDADE</td>");
                    dt.Columns.Add("UF"); sb.Append("<td>UF</td>");
                    dt.Columns.Add("MATRICULA"); sb.Append("<td>MATRICULA</td>");

                    dt.Columns.Add("PATH"); sb.Append("<td>PATH</td>");

                    sb.Append("</tr>");

                    #endregion

                    int i = 0;
                    foreach (var contrato in contratos)
                    {
                        i++;

                        cb = sessao.Query<ContratoBeneficiario>()
                            .Where(_cb => _cb.Contrato.ID == contrato.ID && _cb.Tipo == 0 && _cb.Sequencia == 0)
                            .SingleOrDefault();

                        if (cb == null) continue;

                        b = sessao.Query<Beneficiario>()
                            .Where(_b => _b.ID == cb.Beneficiario.ID)
                            .SingleOrDefault();

                        if (b == null) continue;

                        #region info 

                        DataRow nova = dt.NewRow();

                        sb.Append("<tr>");

                        adm = sessao.Get<ContratoADM>(contrato.ContratoADMID);// ContratoAdmFacade.Instance.Carregar(item.Titular.Contrato.ContratoADMID);
                        cartao = sessao.Get<NumeroCartao>(contrato.NumeroID); //NumeroCartaoFacade.Instancia.Carregar(item.Titular.Contrato.NumeroID);

                        nova["CARTAO"] = string.Concat("'", contrato.Numero);
                        sb.Append("<td>"); sb.Append(nova["CARTAO"]); sb.Append("</td>");

                        nova["CPF_TITULAR"] = string.Concat("'", b.CPF);

                        if (b != null && contrato != null)
                        {
                            if (contrato.Senha.StartsWith("0"))
                                nova["SENHA"] = string.Concat("'", contrato.Senha);
                            else
                                nova["SENHA"] = contrato.Senha;

                            if (!string.IsNullOrEmpty(contrato.Ramo))
                                nova["RAMO"] = contrato.Ramo;

                            if (!string.IsNullOrEmpty(contrato.NumeroApolice))
                                nova["APOLICE"] = contrato.NumeroApolice;
                        }


                        if (cb != null && cb.Beneficiario != null)
                        {
                            nova["DT_NASCIMENTO"]       = cb.Beneficiario.DataNascimento.ToString("dd/MM/yyyy");
                            nova["NOME_BENEFICIARIO"]   = cb.Beneficiario.Nome;
                            nova["ABREVIADO"]           = cb.Abreviar2(cb.Beneficiario.Nome);
                            nova["RG"]                  = cb.Beneficiario.RG;////////////////
                        }

                        nova["CONTRATOPJ"] = adm.Descricao;
                        nova["PRODUTO"] = contrato.Produto;

                        if (cb != null && cb.Contrato != null && cb.Contrato.EnderecoReferencia != null && cb.Contrato.EnderecoReferencia.GetType().ToString() != "EnderecoProxy")
                        {
                            nova["LOGRADOURO"] = cb.Contrato.EnderecoReferencia.Logradouro;
                            nova["NUMERO"] = cb.Contrato.EnderecoReferencia.Numero;
                            nova["COMPLEMENTO"] = cb.Contrato.EnderecoReferencia.Complemento;
                            nova["BAIRRO"] = cb.Contrato.EnderecoReferencia.Bairro;
                            nova["CEP"] = cb.Contrato.EnderecoReferencia.CEP;
                            nova["CIDADE"] = cb.Contrato.EnderecoReferencia.Cidade;
                            nova["UF"] = cb.Contrato.EnderecoReferencia.UF;
                        }
                        else
                        {
                            end = sessao.Query<Endereco>()
                                .Where(e => e.DonoId == b.ID && e.DonoTipo == 0)
                                .FirstOrDefault();

                            if (end != null)
                            {
                                nova["LOGRADOURO"] = end.Logradouro;
                                nova["NUMERO"] = end.Numero;
                                nova["COMPLEMENTO"] = end.Complemento;
                                nova["BAIRRO"] = end.Bairro;
                                nova["CEP"] = end.CEP;
                                nova["CIDADE"] = end.Cidade;
                                nova["UF"] = end.UF;
                            }
                        }

                        if (cb != null && cb.Contrato != null)
                            nova["MATRICULA"] = cb.Contrato.Matricula;

                        nova["VIA"] = cartao.Via;
                        nova["CVV"] = cartao.CV;
                        nova["VALIDADE"] = "CONSULTE NOSSO SITE";

                        if (cb.Contrato.DataVigencia != DateTime.MinValue)
                        {
                            nova["INICIO_DO_RISCO"] = cb.Contrato.DataVigencia.ToString("dd/MM/yyyy");
                            nova["FIM_DA_VIGENCIA"] = cb.Contrato.DataVigencia.AddYears(1).AddDays(-1).ToString("dd/MM/yyyy"); //item.Titular.Contrato.DataValidade.ToString("dd/MM/yyyy"); ???
                        }


                        if (cb.Contrato.DataAdmissao != DateTime.MinValue)
                            nova["DATA_DE_EMISSAO"] = cb.Contrato.DataAdmissao.ToString("dd/MM/yyyy");

                        nova["PATH"] = cb.Contrato.CaminhoArquivo;

                        sb.Append("<td>"); sb.Append(nova["ABREVIADO"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["RG"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["PRODUTO"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["VIA"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["CVV"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["VALIDADE"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["SENHA"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["NOME_BENEFICIARIO"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["CONTRATOPJ"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["CPF_TITULAR"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["DT_NASCIMENTO"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["RAMO"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["APOLICE"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["INICIO_DO_RISCO"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["FIM_DA_VIGENCIA"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["DATA_DE_EMISSAO"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["LOGRADOURO"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["NUMERO"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["COMPLEMENTO"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["BAIRRO"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["CEP"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["CIDADE"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["UF"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["MATRICULA"]); sb.Append("</td>");
                        sb.Append("<td>"); sb.Append(nova["PATH"]); sb.Append("</td>");


                        sb.Append("</tr>");

                        dt.Rows.Add(nova);

                        #endregion

                        //if (i == 3) break;
                    }

                    sb.Append("</table>");

                    string html = sb.ToString();
                }
            }
        }

        public void arrumaFones()
        {
            DataTable bh = new DataTable();
            DataTable uberaba = new DataTable();
            string accConn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\ACER E1 572 6830\Desktop\sispag\___outros\dados.accdb;Persist Security Info=False;";
            using (OleDbConnection conn = new OleDbConnection(accConn))
            {
                OleDbDataAdapter adp = new OleDbDataAdapter("select * from bh", conn);
                adp.Fill(bh);

                adp.SelectCommand.CommandText = "select * from uberaba";
                adp.Fill(uberaba);

                conn.Close();
                adp.Dispose();
            }

            string sql = "";
            object aux = null;

            using (var sessaoF = CriarSessaoNHibernate())
            {
                using (var sessao = sessaoF.OpenSession())
                {

                    //sessao.Connection.Open();
                    using (IDbCommand cmd = sessao.Connection.CreateCommand())
                    {
                        int total = bh.Rows.Count;
                        int i = 0;

                        #region bh 

                        foreach (DataRow row in bh.Rows)
                        {
                            continue;
                            i++;

                            sql = string.Concat("select contrato_id from contrato where contrato_numero='", Convert.ToString(row["Cartao"]).Replace("!", ""), "'");
                            cmd.CommandText = sql;
                            aux = cmd.ExecuteScalar();

                            if (aux == null || aux == DBNull.Value) continue;

                            sql = string.Concat("select contratobeneficiario_beneficiarioid from contrato_beneficiario where contratobeneficiario_tipo=0 and contratobeneficiario_contratoId=", aux);
                            cmd.CommandText = sql;
                            aux = cmd.ExecuteScalar();

                            if (aux == null || aux == DBNull.Value) continue;

                            if (row["TELEFONE"] != null && row["TELEFONE"] != DBNull.Value && Convert.ToString(row["TELEFONE"]).Trim() != "")
                            {
                                if (row["DDD"] != null && row["DDD"] != DBNull.Value && Convert.ToString(row["DDD"]).Trim() != "")
                                {
                                    sql = string.Concat("update beneficiario set beneficiario_telefone='(", Convert.ToString(row["ddd"]).Trim(), ") ", Convert.ToString(row["TELEFONE"]).Trim(), "' where beneficiario_id=", aux);
                                }
                                else
                                {
                                    sql = string.Concat("update beneficiario set beneficiario_telefone='(00) ", Convert.ToString(row["TELEFONE"]).Trim(), "' where beneficiario_id=", aux);
                                }

                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }

                            /****/

                            if (row["TELEFONE2"] != null && row["TELEFONE2"] != DBNull.Value && Convert.ToString(row["TELEFONE2"]).Trim() != "")
                            {
                                if (row["DDD2"] != null && row["DDD2"] != DBNull.Value && Convert.ToString(row["DDD2"]).Trim() != "")
                                {
                                    sql = string.Concat("update beneficiario set beneficiario_telefone2='(", Convert.ToString(row["ddd2"]).Trim(), ") ", Convert.ToString(row["TELEFONE2"]).Trim(), "' where beneficiario_id=", aux);
                                }
                                else
                                {
                                    sql = string.Concat("update beneficiario set beneficiario_telefone2='(00) ", Convert.ToString(row["TELEFONE2"]).Trim(), "' where beneficiario_id=", aux);
                                }

                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }

                            /****/

                            if (row["TELEFONE3"] != null && row["TELEFONE3"] != DBNull.Value && Convert.ToString(row["TELEFONE3"]).Trim() != "")
                            {
                                if (row["DDD3"] != null && row["DDD3"] != DBNull.Value && Convert.ToString(row["DDD3"]).Trim() != "")
                                {
                                    sql = string.Concat("update beneficiario set beneficiario_celular='(", Convert.ToString(row["ddd3"]).Trim(), ") ", Convert.ToString(row["TELEFONE3"]).Trim(), "' where beneficiario_id=", aux);
                                }
                                else
                                {
                                    sql = string.Concat("update beneficiario set beneficiario_celular='(00) ", Convert.ToString(row["TELEFONE3"]).Trim(), "' where beneficiario_id=", aux);
                                }

                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        #endregion

                        #region uberaba 

                        total = uberaba.Rows.Count;
                        i = 0;

                        foreach (DataRow row in uberaba.Rows)
                        {
                            i++;

                            sql = string.Concat("select contrato_id from contrato where contrato_numero='", Convert.ToString(row["Cartao"]).Replace("!", ""), "'");
                            cmd.CommandText = sql;
                            aux = cmd.ExecuteScalar();

                            if (aux == null || aux == DBNull.Value) continue;

                            sql = string.Concat("select contratobeneficiario_beneficiarioid from contrato_beneficiario where contratobeneficiario_tipo=0 and contratobeneficiario_contratoId=", aux);
                            cmd.CommandText = sql;
                            aux = cmd.ExecuteScalar();

                            if (aux == null || aux == DBNull.Value) continue;

                            if (row["TELEFONE"] != null && row["TELEFONE"] != DBNull.Value && Convert.ToString(row["TELEFONE"]).Trim() != "")
                            {
                                if (row["DDD"] != null && row["DDD"] != DBNull.Value && Convert.ToString(row["DDD"]).Trim() != "")
                                {
                                    sql = string.Concat("update beneficiario set beneficiario_telefone='(", Convert.ToString(row["ddd"]).Trim(), ") ", Convert.ToString(row["TELEFONE"]).Trim(), "' where beneficiario_id=", aux);
                                }
                                else
                                {
                                    sql = string.Concat("update beneficiario set beneficiario_telefone='(00) ", Convert.ToString(row["TELEFONE"]).Trim(), "' where beneficiario_id=", aux);
                                }

                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }

                            /****/

                            if (row["TELEFONE2"] != null && row["TELEFONE2"] != DBNull.Value && Convert.ToString(row["TELEFONE2"]).Trim() != "")
                            {
                                if (row["DDD2"] != null && row["DDD2"] != DBNull.Value && Convert.ToString(row["DDD2"]).Trim() != "")
                                {
                                    sql = string.Concat("update beneficiario set beneficiario_telefone2='(", Convert.ToString(row["ddd2"]).Trim(), ") ", Convert.ToString(row["TELEFONE2"]).Trim(), "' where beneficiario_id=", aux);
                                }
                                else
                                {
                                    sql = string.Concat("update beneficiario set beneficiario_telefone2='(00) ", Convert.ToString(row["TELEFONE2"]).Trim(), "' where beneficiario_id=", aux);
                                }

                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }

                            /****/

                            if (row["TELEFONE3"] != null && row["TELEFONE3"] != DBNull.Value && Convert.ToString(row["TELEFONE3"]).Trim() != "")
                            {
                                if (row["DDD3"] != null && row["DDD3"] != DBNull.Value && Convert.ToString(row["DDD3"]).Trim() != "")
                                {
                                    sql = string.Concat("update beneficiario set beneficiario_celular='(", Convert.ToString(row["ddd3"]).Trim(), ") ", Convert.ToString(row["TELEFONE3"]).Trim(), "' where beneficiario_id=", aux);
                                }
                                else
                                {
                                    sql = string.Concat("update beneficiario set beneficiario_celular='(00) ", Convert.ToString(row["TELEFONE3"]).Trim(), "' where beneficiario_id=", aux);
                                }

                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        #endregion
                    }
                }
            }
        }

        /// <summary>
        /// Carga inicial para a tabela log_cobrancagerada
        /// </summary>
        public void geraLogsDeCobranca()
        {
            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    var cobrancas = LocatorHelper.Instance.ExecuteQuery<PSE.Cobranca>("* from cobranca order by cobranca_id", typeof(PSE.Cobranca), pm);

                    foreach (var cob in cobrancas)
                    {
                        PSE.CobrancaLog.CobrancaCriadaLog log = new PSE.CobrancaLog.CobrancaCriadaLog();
                        log.CobrancaID = cob.ID;
                        log.CobrancaValor = cob.Valor;
                        log.CobrancaVencimento = cob.DataVencimento;
                        log.Data = cob.DataCriacao;
                        log.Origem = (int)PSE.CobrancaLog.Fonte.Sistema;
                        log.PropostaID = cob.PropostaID;

                        pm.Save(log);
                    }

                    pm.Commit();
                }
                catch
                {
                    pm.Rollback();
                }
                finally
                {
                    pm.Dispose();
                }
            }
        }

        /*************************************************************************/

        void desfazImportacao()
        {
            processando = true;

            #region numeros

            List<string> nums = new List<string>();

            nums.Add("6370870040937612");
            nums.Add("6370870040940019");
            nums.Add("6370870040942416");
            nums.Add("6370870040944813");
            nums.Add("6370870040947216");
            nums.Add("6370870040949613");
            nums.Add("6370870040975918");
            nums.Add("6370870040978310");
            nums.Add("6370870040980711");
            nums.Add("6370870040983114");
            nums.Add("6370870040985511");
            nums.Add("6370870040987919");
            nums.Add("6370870040937710");
            nums.Add("6370870040940117");
            nums.Add("6370870040942514");
            nums.Add("6370870040944911");
            nums.Add("6370870040947314");
            nums.Add("6370870040949711");
            nums.Add("6370870040974616");
            nums.Add("6370870040977019");
            nums.Add("6370870040979416");
            nums.Add("6370870040981817");
            nums.Add("6370870040984210");
            nums.Add("6370870040986617");
            nums.Add("6370870040974714");
            nums.Add("6370870040977117");
            nums.Add("6370870040979514");
            nums.Add("6370870040981915");
            nums.Add("6370870040984318");
            nums.Add("6370870040986715");
            nums.Add("6370870040974017");
            nums.Add("6370870040976414");
            nums.Add("6370870040978811");
            nums.Add("6370870040981218");
            nums.Add("6370870040983615");
            nums.Add("6370870040986018");
            nums.Add("6370870040974812");
            nums.Add("6370870040977215");
            nums.Add("6370870040979612");
            nums.Add("6370870040982019");
            nums.Add("6370870040984416");
            nums.Add("6370870040986813");
            nums.Add("6370870040937013");
            nums.Add("6370870040939410");
            nums.Add("6370870040941811");
            nums.Add("6370870040944214");
            nums.Add("6370870040946611");
            nums.Add("6370870040949014");
            nums.Add("6370870040936517");
            nums.Add("6370870040938914");
            nums.Add("6370870040941310");
            nums.Add("6370870040943718");
            nums.Add("6370870040946110");
            nums.Add("6370870040948518");
            nums.Add("6370870040936419");
            nums.Add("6370870040938816");
            nums.Add("6370870040941212");
            nums.Add("6370870040943610");
            nums.Add("6370870040946012");
            nums.Add("6370870040948410");
            nums.Add("6370870040974213");
            nums.Add("6370870040976610");
            nums.Add("6370870040979013");
            nums.Add("6370870040981414");
            nums.Add("6370870040983811");
            nums.Add("6370870040986214");
            nums.Add("6370870040974311");
            nums.Add("6370870040976719");
            nums.Add("6370870040979111");
            nums.Add("6370870040981512");
            nums.Add("6370870040983910");
            nums.Add("6370870040986312");
            nums.Add("6370870040975319");
            nums.Add("6370870040977716");
            nums.Add("6370870040980112");
            nums.Add("6370870040982510");
            nums.Add("6370870040984917");
            nums.Add("6370870040987310");
            nums.Add("6370870040974518");
            nums.Add("6370870040976915");
            nums.Add("6370870040979318");
            nums.Add("6370870040981719");
            nums.Add("6370870040984111");
            nums.Add("6370870040986519");
            nums.Add("6370870040976011");
            nums.Add("6370870040978419");
            nums.Add("6370870040980810");
            nums.Add("6370870040983212");
            nums.Add("6370870040985610");
            nums.Add("6370870040988012");
            nums.Add("6370870040936713");
            nums.Add("6370870040939116");
            nums.Add("6370870040941517");
            nums.Add("6370870040943914");
            nums.Add("6370870040946317");
            nums.Add("6370870040948714");
            nums.Add("6370870040974410");
            nums.Add("6370870040976817");
            nums.Add("6370870040979210");
            nums.Add("6370870040981610");
            nums.Add("6370870040984013");
            nums.Add("6370870040986410");
            nums.Add("6370870040975417");
            nums.Add("6370870040977814");
            nums.Add("6370870040980210");
            nums.Add("6370870040982618");
            nums.Add("6370870040985010");
            nums.Add("6370870040987418");
            nums.Add("6370870040975810");
            nums.Add("6370870040978212");
            nums.Add("6370870040980613");
            nums.Add("6370870040983016");
            nums.Add("6370870040985413");
            nums.Add("6370870040987810");
            nums.Add("6370870040975711");
            nums.Add("6370870040978114");
            nums.Add("6370870040980515");
            nums.Add("6370870040982912");
            nums.Add("6370870040985315");
            nums.Add("6370870040987712");
            nums.Add("6370870040976316");
            nums.Add("6370870040978713");
            nums.Add("6370870040981110");
            nums.Add("6370870040983517");
            nums.Add("6370870040985914");
            nums.Add("6370870040988317");
            nums.Add("6370870040936615");
            nums.Add("6370870040939018");
            nums.Add("6370870040941419");
            nums.Add("6370870040943816");
            nums.Add("6370870040946219");
            nums.Add("6370870040948616");
            nums.Add("6370870040952010");
            nums.Add("6370870040954417");
            nums.Add("6370870040956814");
            nums.Add("6370870040959217");
            nums.Add("6370870040961618");
            nums.Add("6370870040964010");
            nums.Add("6370870040989010");
            nums.Add("6370870040938511");
            nums.Add("6370870040940912");
            nums.Add("6370870040943315");
            nums.Add("6370870040945712");
            nums.Add("6370870040948115");
            nums.Add("6370870040951415");
            nums.Add("6370870040953812");
            nums.Add("6370870040956215");
            nums.Add("6370870040958612");
            nums.Add("6370870040961019");
            nums.Add("6370870040963416");
            nums.Add("6370870040989118");
            nums.Add("6370870040938610");
            nums.Add("6370870040941016");
            nums.Add("6370870040943413");
            nums.Add("6370870040945810");
            nums.Add("6370870040948213");
            nums.Add("6370870040952118");
            nums.Add("6370870040954515");
            nums.Add("6370870040956912");
            nums.Add("6370870040959315");
            nums.Add("6370870040961716");
            nums.Add("6370870040964119");
            nums.Add("6370870040950810");
            nums.Add("6370870040953213");
            nums.Add("6370870040955610");
            nums.Add("6370870040958013");
            nums.Add("6370870040960414");
            nums.Add("6370870040962811");
            nums.Add("6370870040988710");
            nums.Add("6370870040938217");
            nums.Add("6370870040940618");
            nums.Add("6370870040943010");
            nums.Add("6370870040945418");
            nums.Add("6370870040947815");
            nums.Add("6370870040988916");
            nums.Add("6370870040938413");
            nums.Add("6370870040940814");
            nums.Add("6370870040943217");
            nums.Add("6370870040945614");
            nums.Add("6370870040948017");
            nums.Add("6370870040988415");
            nums.Add("6370870040937917");
            nums.Add("6370870040940313");
            nums.Add("6370870040942710");
            nums.Add("6370870040945113");
            nums.Add("6370870040947510");
            nums.Add("6370870040950919");
            nums.Add("6370870040953311");
            nums.Add("6370870040955719");
            nums.Add("6370870040958111");
            nums.Add("6370870040960512");
            nums.Add("6370870040962910");
            nums.Add("6370870040989216");
            nums.Add("6370870040936310");
            nums.Add("6370870040938718");
            nums.Add("6370870040941114");
            nums.Add("6370870040943511");
            nums.Add("6370870040945919");
            nums.Add("6370870040951110");
            nums.Add("6370870040953518");
            nums.Add("6370870040955915");
            nums.Add("6370870040958318");
            nums.Add("6370870040960719");
            nums.Add("6370870040963111");
            nums.Add("6370870040989717");
            nums.Add("6370870040936811");
            nums.Add("6370870040939214");
            nums.Add("6370870040941615");
            nums.Add("6370870040944018");
            nums.Add("6370870040946415");
            nums.Add("6370870040975613");
            nums.Add("6370870040978016");
            nums.Add("6370870040980417");
            nums.Add("6370870040982814");
            nums.Add("6370870040985217");
            nums.Add("6370870040987614");
            nums.Add("6370870040988611");
            nums.Add("6370870040938119");
            nums.Add("6370870040940510");
            nums.Add("6370870040942917");
            nums.Add("6370870040945310");
            nums.Add("6370870040947717");
            nums.Add("6370870040937416");
            nums.Add("6370870040939813");
            nums.Add("6370870040942210");
            nums.Add("6370870040944617");
            nums.Add("6370870040947010");
            nums.Add("6370870040949417");
            nums.Add("6370870040974115");
            nums.Add("6370870040976512");
            nums.Add("6370870040978910");
            nums.Add("6370870040981316");
            nums.Add("6370870040983713");
            nums.Add("6370870040986116");
            nums.Add("6370870040937514");
            nums.Add("6370870040939911");
            nums.Add("6370870040942318");
            nums.Add("6370870040944715");
            nums.Add("6370870040947118");
            nums.Add("6370870040949515");
            nums.Add("6370870040988818");
            nums.Add("6370870040938315");
            nums.Add("6370870040940716");
            nums.Add("6370870040943119");
            nums.Add("6370870040945516");
            nums.Add("6370870040947913");
            nums.Add("6370870040937318");
            nums.Add("6370870040939715");
            nums.Add("6370870040942111");
            nums.Add("6370870040944519");
            nums.Add("6370870040946916");
            nums.Add("6370870040949319");
            nums.Add("6370870040989815");
            nums.Add("6370870040936910");
            nums.Add("6370870040939312");
            nums.Add("6370870040941713");
            nums.Add("6370870040944116");
            nums.Add("6370870040946513");
            nums.Add("6370870040937819");
            nums.Add("6370870040940215");
            nums.Add("6370870040942612");
            nums.Add("6370870040945015");
            nums.Add("6370870040947412");
            nums.Add("6370870040949810");
            nums.Add("6370870040951012");
            nums.Add("6370870040953410");
            nums.Add("6370870040955817");
            nums.Add("6370870040958210");
            nums.Add("6370870040960610");
            nums.Add("6370870040963013");
            nums.Add("6370870040966418");
            nums.Add("6370870040968815");
            nums.Add("6370870040971211");
            nums.Add("6370870040973619");
            nums.Add("6370870040976110");
            nums.Add("6370870040978517");
            nums.Add("6370870040950614");
            nums.Add("6370870040953017");
            nums.Add("6370870040955414");
            nums.Add("6370870040957811");
            nums.Add("6370870040960218");
            nums.Add("6370870040962615");
            nums.Add("6370870040950516");
            nums.Add("6370870040952913");
            nums.Add("6370870040955316");
            nums.Add("6370870040957713");
            nums.Add("6370870040960110");
            nums.Add("6370870040962517");
            nums.Add("6370870040965813");
            nums.Add("6370870040968216");
            nums.Add("6370870040970617");
            nums.Add("6370870040973010");
            nums.Add("6370870040975515");
            nums.Add("6370870040977912");
            nums.Add("6370870040965214");
            nums.Add("6370870040967611");
            nums.Add("6370870040970018");
            nums.Add("6370870040972415");
            nums.Add("6370870040974910");
            nums.Add("6370870040977313");
            nums.Add("6370870040949918");
            nums.Add("6370870040952314");
            nums.Add("6370870040954711");
            nums.Add("6370870040957114");
            nums.Add("6370870040959511");
            nums.Add("6370870040961912");
            nums.Add("6370870040950418");
            nums.Add("6370870040952815");
            nums.Add("6370870040955218");
            nums.Add("6370870040957615");
            nums.Add("6370870040960011");
            nums.Add("6370870040962419");
            nums.Add("6370870040966516");
            nums.Add("6370870040968913");
            nums.Add("6370870040971310");
            nums.Add("6370870040973717");
            nums.Add("6370870040976218");
            nums.Add("6370870040978615");
            nums.Add("6370870040965312");
            nums.Add("6370870040967710");
            nums.Add("6370870040970116");
            nums.Add("6370870040972513");
            nums.Add("6370870040975014");
            nums.Add("6370870040977411");
            nums.Add("6370870040948311");
            nums.Add("6370870040950712");
            nums.Add("6370870040953115");
            nums.Add("6370870040955512");
            nums.Add("6370870040957910");
            nums.Add("6370870040960316");
            nums.Add("6370870040965519");
            nums.Add("6370870040967916");
            nums.Add("6370870040970312");
            nums.Add("6370870040972710");
            nums.Add("6370870040975210");
            nums.Add("6370870040977618");
            nums.Add("6370870040950211");
            nums.Add("6370870040952619");
            nums.Add("6370870040955011");
            nums.Add("6370870040957419");
            nums.Add("6370870040959816");
            nums.Add("6370870040962212");
            nums.Add("6370870040990010");
            nums.Add("6370870040937111");
            nums.Add("6370870040939519");
            nums.Add("6370870040941910");
            nums.Add("6370870040944312");
            nums.Add("6370870040946710");
            nums.Add("6370870040948812");
            nums.Add("6370870040951219");
            nums.Add("6370870040953616");
            nums.Add("6370870040956019");
            nums.Add("6370870040958416");
            nums.Add("6370870040960817");
            nums.Add("6370870040951818");
            nums.Add("6370870040954210");
            nums.Add("6370870040956618");
            nums.Add("6370870040959010");
            nums.Add("6370870040961411");
            nums.Add("6370870040963819");
            nums.Add("6370870040990119");
            nums.Add("6370870040937210");
            nums.Add("6370870040939617");
            nums.Add("6370870040942013");
            nums.Add("6370870040944410");
            nums.Add("6370870040946818");
            nums.Add("6370870040950310");
            nums.Add("6370870040952717");
            nums.Add("6370870040955110");
            nums.Add("6370870040957517");
            nums.Add("6370870040959914");
            nums.Add("6370870040962310");
            nums.Add("6370870040951916");
            nums.Add("6370870040954319");
            nums.Add("6370870040956716");
            nums.Add("6370870040959119");
            nums.Add("6370870040961510");
            nums.Add("6370870040963917");
            nums.Add("6370870040980918");
            nums.Add("6370870040983310");
            nums.Add("6370870040985718");
            nums.Add("6370870040988110");
            nums.Add("6370870040988513");
            nums.Add("6370870040938010");
            nums.Add("6370870040940411");
            nums.Add("6370870040942819");
            nums.Add("6370870040945211");
            nums.Add("6370870040947619");
            nums.Add("6370870040951710");
            nums.Add("6370870040954112");
            nums.Add("6370870040956510");
            nums.Add("6370870040958917");
            nums.Add("6370870040961313");
            nums.Add("6370870040963710");
            nums.Add("6370870040964914");
            nums.Add("6370870040967317");
            nums.Add("6370870040969714");
            nums.Add("6370870040972110");
            nums.Add("6370870040950113");
            nums.Add("6370870040952510");
            nums.Add("6370870040954918");
            nums.Add("6370870040957310");
            nums.Add("6370870040959718");
            nums.Add("6370870040962114");
            nums.Add("6370870040948910");
            nums.Add("6370870040951317");
            nums.Add("6370870040953714");
            nums.Add("6370870040956117");
            nums.Add("6370870040958514");
            nums.Add("6370870040960915");
            nums.Add("6370870040952216");
            nums.Add("6370870040954613");
            nums.Add("6370870040957016");
            nums.Add("6370870040959413");
            nums.Add("6370870040961814");
            nums.Add("6370870040964217");
            nums.Add("6370870040964816");
            nums.Add("6370870040967219");
            nums.Add("6370870040969616");
            nums.Add("6370870040972012");
            nums.Add("6370870040981011");
            nums.Add("6370870040983419");
            nums.Add("6370870040985816");
            nums.Add("6370870040988219");
            nums.Add("6370870040980319");
            nums.Add("6370870040982716");
            nums.Add("6370870040985119");
            nums.Add("6370870040987516");
            nums.Add("6370870040989913");
            nums.Add("6370870040965410");
            nums.Add("6370870040967818");
            nums.Add("6370870040970214");
            nums.Add("6370870040972611");
            nums.Add("6370870040975112");
            nums.Add("6370870040977510");
            nums.Add("6370870040979710");
            nums.Add("6370870040982117");
            nums.Add("6370870040984514");
            nums.Add("6370870040986911");
            nums.Add("6370870040989314");
            nums.Add("6370870040964610");
            nums.Add("6370870040967012");
            nums.Add("6370870040969410");
            nums.Add("6370870040971810");
            nums.Add("6370870040964315");
            nums.Add("6370870040966712");
            nums.Add("6370870040969115");
            nums.Add("6370870040971516");
            nums.Add("6370870040973913");
            nums.Add("6370870040979819");
            nums.Add("6370870040982215");
            nums.Add("6370870040984612");
            nums.Add("6370870040987015");
            nums.Add("6370870040989412");
            nums.Add("6370870040966211");
            nums.Add("6370870040968619");
            nums.Add("6370870040971015");
            nums.Add("6370870040973412");
            nums.Add("6370870040980014");
            nums.Add("6370870040982411");
            nums.Add("6370870040984819");
            nums.Add("6370870040987211");
            nums.Add("6370870040989619");
            nums.Add("6370870040962713");
            nums.Add("6370870040965116");
            nums.Add("6370870040967513");
            nums.Add("6370870040969910");
            nums.Add("6370870040972317");
            nums.Add("6370870040965018");
            nums.Add("6370870040967415");
            nums.Add("6370870040969812");
            nums.Add("6370870040972219");
            nums.Add("6370870040963210");
            nums.Add("6370870040965617");
            nums.Add("6370870040968010");
            nums.Add("6370870040970410");
            nums.Add("6370870040972818");
            nums.Add("6370870040964718");
            nums.Add("6370870040967110");
            nums.Add("6370870040969518");
            nums.Add("6370870040971919");
            nums.Add("6370870040966310");
            nums.Add("6370870040968717");
            nums.Add("6370870040971113");
            nums.Add("6370870040973510");
            nums.Add("6370870040964511");
            nums.Add("6370870040966919");
            nums.Add("6370870040969311");
            nums.Add("6370870040971712");
            nums.Add("6370870040949210");
            nums.Add("6370870040951611");
            nums.Add("6370870040954014");
            nums.Add("6370870040956411");
            nums.Add("6370870040958819");
            nums.Add("6370870040961215");
            nums.Add("6370870040966113");
            nums.Add("6370870040968510");
            nums.Add("6370870040970911");
            nums.Add("6370870040973314");
            nums.Add("6370870040949112");
            nums.Add("6370870040951513");
            nums.Add("6370870040953910");
            nums.Add("6370870040956313");
            nums.Add("6370870040958710");
            nums.Add("6370870040961117");
            nums.Add("6370870040966614");
            nums.Add("6370870040969017");
            nums.Add("6370870040971418");
            nums.Add("6370870040973815");
            nums.Add("6370870040979917");
            nums.Add("6370870040982313");
            nums.Add("6370870040984710");
            nums.Add("6370870040987113");
            nums.Add("6370870040989510");
            nums.Add("6370870040950015");
            nums.Add("6370870040952412");
            nums.Add("6370870040954810");
            nums.Add("6370870040957212");
            nums.Add("6370870040959610");
            nums.Add("6370870040962016");
            nums.Add("6370870040963318");
            nums.Add("6370870040965715");
            nums.Add("6370870040968118");
            nums.Add("6370870040970519");
            nums.Add("6370870040972916");
            nums.Add("6370870040963612");
            nums.Add("6370870040966015");
            nums.Add("6370870040968412");
            nums.Add("6370870040970813");
            nums.Add("6370870040973216");
            nums.Add("6370870040964413");
            nums.Add("6370870040966810");
            nums.Add("6370870040969213");
            nums.Add("6370870040971614");
            nums.Add("6370870040963514");
            nums.Add("6370870040965911");
            nums.Add("6370870040968314");
            nums.Add("6370870040970715");
            nums.Add("6370870040973118");

            nums.Add("6370870040925818");
            nums.Add("6370870040928210");
            nums.Add("6370870040930611");
            nums.Add("6370870040933014");
            nums.Add("6370870040935411");
            nums.Add("6370870040920811");
            nums.Add("6370870040923214");
            nums.Add("6370870040925611");
            nums.Add("6370870040928014");
            nums.Add("6370870040930415");
            nums.Add("6370870040932812");
            nums.Add("6370870040935215");
            nums.Add("6370870040919810");
            nums.Add("6370870040922217");
            nums.Add("6370870040924614");
            nums.Add("6370870040927017");
            nums.Add("6370870040929414");
            nums.Add("6370870040931815");
            nums.Add("6370870040934218");
            nums.Add("6370870040920016");
            nums.Add("6370870040922413");
            nums.Add("6370870040924810");
            nums.Add("6370870040927213");
            nums.Add("6370870040929610");
            nums.Add("6370870040932017");
            nums.Add("6370870040934414");
            nums.Add("6370870040920615");
            nums.Add("6370870040923018");
            nums.Add("6370870040925415");
            nums.Add("6370870040927812");
            nums.Add("6370870040930219");
            nums.Add("6370870040932616");
            nums.Add("6370870040935019");
            nums.Add("6370870040920517");
            nums.Add("6370870040922914");
            nums.Add("6370870040925317");
            nums.Add("6370870040927714");
            nums.Add("6370870040930110");
            nums.Add("6370870040932518");
            nums.Add("6370870040934915");
            nums.Add("6370870040920713");
            nums.Add("6370870040923116");
            nums.Add("6370870040925513");
            nums.Add("6370870040927910");
            nums.Add("6370870040930317");
            nums.Add("6370870040932714");
            nums.Add("6370870040935117");
            nums.Add("6370870040920419");
            nums.Add("6370870040922816");
            nums.Add("6370870040925219");
            nums.Add("6370870040927616");
            nums.Add("6370870040930012");
            nums.Add("6370870040932410");
            nums.Add("6370870040934817");
            nums.Add("6370870040919919");
            nums.Add("6370870040922315");
            nums.Add("6370870040924712");
            nums.Add("6370870040927115");
            nums.Add("6370870040929512");
            nums.Add("6370870040931913");
            nums.Add("6370870040934316");
            nums.Add("6370870040921318");
            nums.Add("6370870040923715");
            nums.Add("6370870040926118");
            nums.Add("6370870040928515");
            nums.Add("6370870040930916");
            nums.Add("6370870040933319");
            nums.Add("6370870040935716");
            nums.Add("6370870040921710");
            nums.Add("6370870040924113");
            nums.Add("6370870040926510");
            nums.Add("6370870040928918");
            nums.Add("6370870040931314");
            nums.Add("6370870040933711");
            nums.Add("6370870040936114");
            nums.Add("6370870040920910");
            nums.Add("6370870040923312");
            nums.Add("6370870040925710");
            nums.Add("6370870040928112");
            nums.Add("6370870040930513");
            nums.Add("6370870040932910");
            nums.Add("6370870040935313");
            nums.Add("6370870040921514");
            nums.Add("6370870040923911");
            nums.Add("6370870040926314");
            nums.Add("6370870040928711");
            nums.Add("6370870040931118");
            nums.Add("6370870040933515");
            nums.Add("6370870040935912");
            nums.Add("6370870040921210");
            nums.Add("6370870040923617");
            nums.Add("6370870040926010");
            nums.Add("6370870040928417");
            nums.Add("6370870040930818");
            nums.Add("6370870040933210");
            nums.Add("6370870040935618");
            nums.Add("6370870040920212");
            nums.Add("6370870040922610");
            nums.Add("6370870040925012");
            nums.Add("6370870040927410");
            nums.Add("6370870040929817");
            nums.Add("6370870040932213");
            nums.Add("6370870040934610");
            nums.Add("6370870040921111");
            nums.Add("6370870040923519");
            nums.Add("6370870040925916");
            nums.Add("6370870040928319");
            nums.Add("6370870040930710");
            nums.Add("6370870040933112");
            nums.Add("6370870040935510");
            nums.Add("6370870040921917");
            nums.Add("6370870040924310");
            nums.Add("6370870040926717");
            nums.Add("6370870040929110");
            nums.Add("6370870040931510");
            nums.Add("6370870040933918");
            nums.Add("6370870040919614");
            nums.Add("6370870040922010");
            nums.Add("6370870040924418");
            nums.Add("6370870040926815");
            nums.Add("6370870040929218");
            nums.Add("6370870040931619");
            nums.Add("6370870040934011");
            nums.Add("6370870040921612");
            nums.Add("6370870040924015");
            nums.Add("6370870040926412");
            nums.Add("6370870040928810");
            nums.Add("6370870040931216");
            nums.Add("6370870040933613");
            nums.Add("6370870040936016");
            nums.Add("6370870040919712");
            nums.Add("6370870040922119");
            nums.Add("6370870040924516");
            nums.Add("6370870040926913");
            nums.Add("6370870040929316");
            nums.Add("6370870040931717");
            nums.Add("6370870040934110");
            nums.Add("6370870040921416");
            nums.Add("6370870040923813");
            nums.Add("6370870040926216");
            nums.Add("6370870040928613");
            nums.Add("6370870040931010");
            nums.Add("6370870040933417");
            nums.Add("6370870040935814");
            nums.Add("6370870040921819");
            nums.Add("6370870040924211");
            nums.Add("6370870040926619");
            nums.Add("6370870040929011");
            nums.Add("6370870040931412");
            nums.Add("6370870040933810");
            nums.Add("6370870040936212");
            nums.Add("6370870040920114");
            nums.Add("6370870040922511");
            nums.Add("6370870040924919");
            nums.Add("6370870040927311");
            nums.Add("6370870040929719");
            nums.Add("6370870040932115");
            nums.Add("6370870040934512");
            nums.Add("6370870040920310");
            nums.Add("6370870040922718");
            nums.Add("6370870040925110");
            nums.Add("6370870040927518");
            nums.Add("6370870040929915");
            nums.Add("6370870040932311");
            nums.Add("6370870040934719");


            nums.Add("6370870040916917");
            nums.Add("6370870040919310");
            nums.Add("6370870040916612");
            nums.Add("6370870040919015");
            nums.Add("6370870040914814");
            nums.Add("6370870040917217");
            nums.Add("6370870040916819");
            nums.Add("6370870040919211");
            nums.Add("6370870040916514");
            nums.Add("6370870040918911");
            nums.Add("6370870040916318");
            nums.Add("6370870040918715");
            nums.Add("6370870040915910");
            nums.Add("6370870040918312");
            nums.Add("6370870040915713");
            nums.Add("6370870040918116");
            nums.Add("6370870040916210");
            nums.Add("6370870040918617");
            nums.Add("6370870040916013");
            nums.Add("6370870040918410");
            nums.Add("6370870040915419");
            nums.Add("6370870040917816");
            nums.Add("6370870040915811");
            nums.Add("6370870040918214");
            nums.Add("6370870040916416");
            nums.Add("6370870040918813");
            nums.Add("6370870040915310");
            nums.Add("6370870040917718");
            nums.Add("6370870040915615");
            nums.Add("6370870040918018");
            nums.Add("6370870040915517");
            nums.Add("6370870040917914");
            nums.Add("6370870040915114");
            nums.Add("6370870040917511");
            nums.Add("6370870040916111");
            nums.Add("6370870040918519");
            nums.Add("6370870040916710");
            nums.Add("6370870040919113");
            nums.Add("6370870040914618");
            nums.Add("6370870040917010");
            nums.Add("6370870040919418");
            nums.Add("6370870040915016");
            nums.Add("6370870040917413");
            nums.Add("6370870040914716");
            nums.Add("6370870040917119");
            nums.Add("6370870040919516");
            nums.Add("6370870040915212");
            nums.Add("6370870040917610");
            nums.Add("6370870040914912");
            nums.Add("6370870040917315");



            #endregion

            using (PersistenceManager pm = new PersistenceManager())
            {
                pm.BeginTransactionContext();

                try
                {
                    int contratoqtd = 0;
                    int contratoId = 0;
                    int numeroid = 0, ret = 0;
                    foreach (string num in nums)
                    {
                        contratoqtd = Convert.ToInt32(
                            LocatorHelper.Instance.ExecuteScalar(
                                string.Concat("select count(contrato_id) from contrato where contrato_numero='", num, "'"),
                                null, null, pm));

                        if (contratoqtd != 1) continue;

                        contratoId = Convert.ToInt32(
                            LocatorHelper.Instance.ExecuteScalar(
                                string.Concat("select contrato_id from contrato where contrato_numero='", num, "' and contrato_data >= '2016-12-22'"), 
                                null, null, pm));

                        numeroid = ConvertHelper.ConvertToInt(
                            LocatorHelper.Instance.ExecuteScalar(
                                string.Concat("select contrato_numeroid from contrato where contrato_numero='", num, "' and contrato_id=",contratoId," and contrato_data >= '2016-12-22'"),
                                null, null, pm));

                        if (numeroid == 0) continue;

                        // DELETES 

                        ret = NonQueryHelper.Instance.ExecuteNonQuery(
                            string.Concat("delete from contrato_beneficiario where contratobeneficiario_contratoid=", contratoId), pm);

                        if (ret > 1)
                        {
                            pm.Rollback();
                            return;
                        }

                        NonQueryHelper.Instance.ExecuteNonQuery(
                            string.Concat("delete from contrato where contrato_id=", contratoId), pm);

                        NonQueryHelper.Instance.ExecuteNonQuery(
                            string.Concat("delete from numero_contrato where numerocontrato_id=", numeroid), pm);
                    }

                    pm.Commit();
                }
                catch
                {
                    pm.Rollback();
                }
                finally
                {
                    pm.Dispose();
                }
            }
        }
    }

    internal class SqlStatementInterceptor : EmptyInterceptor
    {
        public static bool LogSQLOn;
        public static string LogSQL;

        public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
        {
            if (SqlStatementInterceptor.LogSQLOn)
                SqlStatementInterceptor.LogSQL += "\n" + sql;

            return sql;
        }
    }
}