namespace MedProj.www.financeiro.cobrancas
{
    using System;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using LC.Web.PadraoSeguros.Entity;
    using System.Net.Mail;
    using System.Configuration;

    public partial class cobrancas : System.Web.UI.Page
    {
        protected const String IDKey = "_idkey";

        string contratoSelecionadoId
        {
            get { return Session["cobr_contr_id"] as string; }
            set { Session["cobr_contr_id"] = value; }
        }

        bool contratoSelecionadoPJ
        {
            get 
            {
                if (Session["cobr_contrpj"] == null) return false;

                return Convert.ToBoolean(Session["cobr_contrpj"]);
            }
            set { Session["cobr_contrpj"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Util.Geral.ExibirEstipulantes(cboAssociadoPJ, true, false);
            }
        }

        void CarregaContratos()
        {
            DataTable lista = null;

            long assocId = 0;
            if (cboAssociadoPJ.SelectedIndex > 0) assocId = Convert.ToInt64(cboAssociadoPJ.SelectedValue);

            //if(txtCartao.Text != "" || txtNome.Text != "")
            lista = Contrato.DTCarregarPorParametros(txtCartao.Text, txtNome.Text, "", assocId);

            gridContratos.DataSource = lista;
            gridContratos.DataBind();
            pnlContratos.Visible = true;
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("cliente.aspx");
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.CarregaContratos();
            if (gridContratos.Rows.Count == 0) { litAviso.Text = "Nenhum registro localizado"; }
        }

        protected void gridContratos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "cobrancas")
            {
                //Session[IDKey] = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                //String uri = "cliente.aspx?" + IDKey + "=" + Session[IDKey];

                //Response.Redirect(uri);

                this.contratoSelecionadoId = e.CommandArgument as string;
                this.exibeCobrancasDoContratoSelecionado();
            }
        }

        void exibeCobrancasDoContratoSelecionado()
        {
            pnlContratos.Visible = false;
            pnlCobrancas.Visible = true;

            Contrato contr = new Contrato(this.contratoSelecionadoId);
            contr.Carregar();

            if (contr.Tipo == (int)eTipoPessoa.Fisica)
                this.contratoSelecionadoPJ = false;
            else
                this.contratoSelecionadoPJ = true;

            this.CarregarCobrancas();
        }

        protected void gridContratos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Boolean rascunho = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][1]);
                ((Image)e.Row.Cells[4].Controls[1]).Visible = rascunho;

                Boolean cancelado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][2]);
                Boolean inativado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][3]);

                //UIHelper.AuthWebCtrl((LinkButton)e.Row.Cells[5].Controls[0], new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.PropostaBeneficiarioIDKey });
                //UIHelper.AuthCtrl((LinkButton)e.Row.Cells[7].Controls[0], new String[] { Perfil.AdministradorIDKey });
                //UIHelper.AuthWebCtrl((LinkButton)e.Row.Cells[7].Controls[0], new String[] { Perfil.AdministradorIDKey });
                grid_RowDataBound_Confirmacao(sender, e, 7, "Deseja realmente prosseguir com a exclusão?\\nEssa operação não poderá ser desfeita.");

                if (Usuario.Autenticado.PerfilID != Perfil.AdministradorIDKey) { gridContratos.Columns[7].Visible = false; }

                if (cancelado || inativado)
                {
                    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='../../images/unactive.png' title='inativo' alt='inativo' border='0'>";
                    //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente ativar o contrato?");
                }
                else
                {
                    //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente cancelar o contrato?");
                    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='../../images/active.png' title='ativo' alt='ativo' border='0'>";
                }
            }
        }

        protected void gridContratos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridContratos.PageIndex = e.NewPageIndex;
            this.CarregaContratos();
        }

        protected void grid_RowDataBound_Confirmacao(Object sender, GridViewRowEventArgs e, int indiceControle, String Msg)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[indiceControle].Attributes.Add("onClick", "return confirm('" + Msg + "');");
            }
        }

        #region cobrancas 

        IList<Cobranca> CarregarCobrancas()
        {
            if (this.contratoSelecionadoId == null) { return null; }
            IList<Cobranca> cobrancas = Cobranca.CarregarTodas(this.contratoSelecionadoId, true, true, null);
            gridCobranca.DataSource = cobrancas; 
            gridCobranca.DataBind();
            return cobrancas;
        }

        protected void gridCobranca_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                /// NEGOCIACAO ///////////////////////////////
                if (gridCobranca.DataKeys[e.Row.RowIndex][1] != null && Convert.ToString(gridCobranca.DataKeys[e.Row.RowIndex][1]).Trim() != "")
                {
                    e.Row.Cells[5].ForeColor = System.Drawing.Color.Green;
                    e.Row.Cells[5].Text = "negociada";
                    e.Row.Cells[6].Text = "";
                    e.Row.ToolTip = "parcela negociada";

                    e.Row.Cells[7].Enabled = false;
                    e.Row.Cells[7].Controls[0].Visible = false;
                    e.Row.Cells[9].Enabled = false;
                    e.Row.Cells[9].Controls[0].Visible = false;
                }

                if (gridCobranca.DataKeys[e.Row.RowIndex][2] != null && Convert.ToString(gridCobranca.DataKeys[e.Row.RowIndex][2]).Trim() != "")
                {
                    e.Row.ToolTip = "Negociação";
                    e.Row.Cells[5].Text += "*";

                    Cobranca cobranca = (Cobranca)e.Row.DataItem;

                    if (!String.IsNullOrEmpty(cobranca.ItemParcelamentoOBS))
                        e.Row.Cells[0].Text = cobranca.ItemParcelamentoOBS.Split(' ')[1];
                }
                /////////////////////////////////////////////////

                //UIHelper.AuthWebCtrl(((TextBox)e.Row.Cells[3].Controls[1]), new String[] { Perfil.Atendimento_Liberacao_Vencimento, Perfil.ConsulPropBenefProdLiberBoletoIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.SupervidorIDKey });

                ///////////////////////////////////////////
                //Panel panel = (Panel)e.Row.Cells[1].FindControl("pnlComposite");
                //Literal lit = (Literal)panel.FindControl("litComposite");

                //lit.Text = ((Cobranca)e.Row.DataItem).ComposicaoResumo;
                ///////////////////////////////////////////

                //if (Server.HtmlDecode(e.Row.Cells[5].Text) == "Não")
                //{
                //    DateTime vencto = CStringToDateTime(((TextBox)e.Row.Cells[3].Controls[1]).Text);
                //    if (vencto < DateTime.Now)
                //    {
                //        e.Row.Cells[5].ForeColor = System.Drawing.Color.Red;
                //    }

                //    if (!((TextBox)e.Row.Cells[3].Controls[1]).Enabled)
                //    {
                //        DateTime vigencia, vencimento, admissao = CStringToDateTime(txtAdmissao.Text);
                //        Int32 diaDataSemJuros = 0, limiteAposVencto = 0; Object valorDataLimite;
                //        CalendarioVencimento rcv = null;

                //        if (!HaItemSelecionado(cboContrato))
                //        {
                //            CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(cboContrato.SelectedValue,
                //               admissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, out limiteAposVencto, null);
                //        }

                //        if (limiteAposVencto > 0)
                //        {
                //            DateTime venctoLimite = DateTime.Now.AddDays(limiteAposVencto);
                //            venctoLimite = new DateTime(venctoLimite.Year, venctoLimite.Month, venctoLimite.Day, 23, 59, 59, 999);

                //            if (venctoLimite < DateTime.Now)
                //            {
                //                ((ImageButton)e.Row.Cells[7].Controls[0]).Visible = false;
                //                ((ImageButton)e.Row.Cells[9].Controls[0]).Visible = false;
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    Decimal cobrado = 0, pago = 0;
                //    pago = Convert.ToDecimal(Server.HtmlDecode(e.Row.Cells[2].Text.Replace("R$ ", "")));
                //    cobrado = Convert.ToDecimal(Server.HtmlDecode(((LinkButton)e.Row.Cells[1].Controls[1]).Text.Replace("R$ ", "")));

                //    if (pago >= cobrado)
                //    {
                //        e.Row.Cells[7].Controls[0].Visible = false;
                //    }
                //    else
                //    {
                //        ((LinkButton)e.Row.Cells[7].Controls[0]).Attributes.Add("onClick", "if(confirm('Esta ação criará uma cobrança complementar no valor de " + (cobrado - pago).ToString("C") + ".\\nConfirma a operação?')) { return true; } else { return false; }");
                //    }
                //}

                ////((LinkButton)e.Row.Cells[6].Controls[0]).ToolTip = "enviar e-mail";
                //((ImageButton)e.Row.Cells[8].Controls[0]).ToolTip = "recalcular";

                //// Regra de mensagem de boleto
                //if (this.contratoSelecionadoPJ)
                //{
                //    Cobranca cob = e.Row.DataItem as Cobranca;
                //    if (cob.DataVencimento < DateTime.Now) //vencido
                //    {
                //        bool vencidoHa5DiasOuMais = Cobranca.VencidoHa5DiasUteis(cob.DataVencimento);
                //        if (!vencidoHa5DiasOuMais)
                //        {
                //            ((LinkButton)e.Row.Cells[7].Controls[0]).Attributes.Add("onClick", "if(confirm('Cobrança vencida.\\nEsta ação criará uma nova cobrança.Confirma a operação?')) { return true; } else { return false; }");
                //            ((LinkButton)e.Row.Cells[9].Controls[0]).Attributes.Add("onClick", "if(confirm('Cobrança vencida.\\nEsta ação criará uma nova cobrança.Confirma a operação?')) { return true; } else { return false; }");
                //        }
                //        else
                //        {
                //            ((LinkButton)e.Row.Cells[7].Controls[0]).Attributes.Add("onClick", "alert('Cobrança vencida há 5 dias ou mais.'); return false;");
                //            ((LinkButton)e.Row.Cells[9].Controls[0]).Attributes.Add("onClick", "alert('Cobrança vencida há 5 dias ou mais.'); return false;");
                //        }
                //    }
                //}
            }
        }

        protected void gridCobranca_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "recalcular")
            {
            }
            if (e.CommandName == "email" || e.CommandName == "print")
            {
                #region

                Object id = gridCobranca.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Cobranca cobranca = new Cobranca(id);
                cobranca.Carregar();

                if (cobranca.Pago) return;

                if (Cobranca.VencidoHa5DiasUteis(cobranca.DataVencimento) && this.contratoSelecionadoPJ) return;

                //List<CobrancaComposite> composite = null;

                String naoReceber = "Não receber após o vencimento.";

                Int32 indice = Convert.ToInt32(e.CommandArgument);
                DateTime vencto = CStringToDateTime(((TextBox)gridCobranca.Rows[indice].Cells[3].Controls[1]).Text);
                if (vencto == DateTime.MinValue)
                {
                    Alerta(null, this, "_errNCobVecto", "Data de vencimento inválida.");
                    return;
                }

                vencto = new DateTime(vencto.Year, vencto.Month, vencto.Day, 23, 59, 59, 500);

                if (vencto < DateTime.Now)
                {
                    vencto = DateTime.Now.AddDays(1);
                    vencto = new DateTime(vencto.Year, vencto.Month, vencto.Day, 23, 59, 59, 950);
                }

                String email = "";

                ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(this.contratoSelecionadoId, null);
                Beneficiario beneficiario = new Beneficiario(titular.BeneficiarioID);
                beneficiario.Carregar();
                //if (!String.IsNullOrEmpty(beneficiario.Email) && txtEmailAtendimento.Text.Trim() == "") { txtEmailAtendimento.Text = beneficiario.Email; }

                //if (txtEmailAtendimento.Text.Trim() == "" && txtEmailAtendimentoCC.Text.Trim() == "" && e.CommandName == "email")
                //{
                //    Alerta(null, this, "_errNCobMail", "Nenhum endereço de e-mail informado.");
                //    return;
                //}

                String nome = beneficiario.Nome;

                //if (beneficiario.Email != txtEmailAtendimento.Text && txtEmailAtendimento.Text.IndexOf("linkecerebro") == -1 && txtEmailAtendimento.Text.IndexOf("pspadrao") == -1 && txtEmailAtendimento.Text.IndexOf("padraoseguros") == -1 && e.CommandName == "email")
                //{
                //    beneficiario.Email = txtEmailAtendimento.Text;
                //    beneficiario.Salvar();
                //}

                //if (txtEmailAtendimento.Text.Trim() != "")
                //    email = txtEmailAtendimento.Text.Trim();

                //if (txtEmailAtendimentoCC.Text.Trim() != "")
                //{
                //    if (email.Length > 0) { email += ";"; }
                //    email += txtEmailAtendimentoCC.Text.Trim();
                //}

                String nossoNumero = "";
                Int32 dia = vencto.Day;  //DateTime.Now.AddDays(1).Day;
                Int32 mes = vencto.Month;//DateTime.Now.AddDays(1).Month;
                Int32 ano = vencto.Year; //DateTime.Now.AddDays(1).Year;
                Decimal Valor = 0;

                if (this.contratoSelecionadoPJ)
                {
                    dia = DateTime.Now.AddDays(1).Day;
                    mes = DateTime.Now.AddDays(1).Month;
                    ano = DateTime.Now.AddDays(1).Year;
                }

                Contrato contrato = new Contrato(this.contratoSelecionadoId);
                contrato.Carregar();
                cobranca.ContratoCodCobranca = contrato.CodCobranca.ToString();

                if (!cobranca.Pago) // se NÃO está pago //////////////////////
                {
                    nossoNumero = cobranca.GeraNossoNumero();

                    if (!Cobranca.NossoNumeroITAU)
                        nossoNumero = nossoNumero.Substring(0, nossoNumero.Length - 1); //tira o DV

                    Int32 diaDataSemJuros = -1;

                    DateTime dataSemJuros = DateTime.MinValue;

                    try
                    {
                        if (diaDataSemJuros == -1 || diaDataSemJuros == 0) { diaDataSemJuros = cobranca.DataVencimento.Day; }
                        dataSemJuros = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, diaDataSemJuros, 23, 59, 59);
                    }
                    catch { }

                    //if (calculaJuros && dataSemJuros != DateTime.MinValue && dataSemJuros < DateTime.Now && cobranca.DataVencimentoISENCAOJURO < DateTime.Now)
                    if (this.contratoSelecionadoPJ && vencto < DateTime.Now) //se é PJ e cobranca vencida
                    {
                        //deve-se cancelar a cobranca atual e gerar uma nova:
                        cobranca.Cancelada = true;
                        cobranca.Salvar();

                        //DateTime dataBase = cobranca.DataVencimento;
                        cobranca.CobrancaRefID = Convert.ToInt64(cobranca.ID);
                        cobranca.ID = null;
                        cobranca.DataCriacao = DateTime.Now;
                        cobranca.Cancelada = false;

                        cobranca.CalculaJurosMulta();
                        cobranca.DataVencimento = vencto; //DateTime.Now.AddDays(1);
                        cobranca.DataVencimento = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, cobranca.DataVencimento.Day, 23, 59, 59, 990);

                        cobranca.Salvar();
                    }
                    else
                    {
                        cobranca.DataVencimento = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, cobranca.DataVencimento.Day, 23, 59, 59, 990);
                    }
                }
                else
                {
                    #region cobrança COMPLEMENTAR ////////////////////////////////////

                    //Cobranca tempCompl = Cobranca.CarregarPor(cobranca.PropostaID, cobranca.Parcela, (int)Cobranca.eTipo.Complementar);
                    //if (tempCompl == null)
                    //{
                    //    cobranca.Tipo = (int)Cobranca.eTipo.Complementar;
                    //    cobranca.DataVencimento = cobranca.DataVencimento; //DateTime.Now.AddDays(7);
                    //    cobranca.Pago = false;
                    //    cobranca.CobrancaRefID = cobranca.ID;
                    //    cobranca.ID = null;
                    //    cobranca.Valor = cobranca.Valor - cobranca.ValorPgto;
                    //    cobranca.ValorPgto = 0;
                    //    cobranca.DataPgto = DateTime.MinValue;
                    //    cobranca.DataCriacao = DateTime.Now;
                    //    cobranca.ContratoCodCobranca = contrato.CodCobranca.ToString();

                    //    if (cobranca.Valor > 0)
                    //    {
                    //        cobranca.Salvar();

                    //        nossoNumero = cobranca.GeraNossoNumero();
                    //        nossoNumero = nossoNumero.Substring(0, nossoNumero.Length - 1); //TIRA o DV
                    //        Valor = cobranca.Valor;
                    //        dia = cobranca.DataVencimento.Day;
                    //        mes = cobranca.DataVencimento.Month;
                    //        ano = cobranca.DataVencimento.Year;
                    //        id = cobranca.ID;
                    //    }
                    //}

                    #endregion
                }

                if (e.CommandName == "print") { email = ""; }

                String uri = "";

                String instrucoes = String.Concat("<br>Este boleto é referente ao período de cobertura de ", cobranca.DataVencimento.Month, "/", cobranca.DataVencimento.Year, ".");

                Valor = cobranca.Valor;
                string end1 = "", end2 = "";

                ////////nossoNumero = "00037208";

                IList<Endereco> enderecos = Endereco.CarregarPorDono(beneficiario.ID, Endereco.TipoDono.Beneficiario);
                if (enderecos != null && enderecos.Count > 0)
                {
                    string compl = ""; if (!string.IsNullOrEmpty(enderecos[0].Complemento)) { compl = " - " + enderecos[0].Complemento; }

                    end1 = string.Concat(enderecos[0].Logradouro, ", ", enderecos[0].Numero, compl);
                    end2 = string.Concat(enderecos[0].CEP, " - ", enderecos[0].Bairro, " - ", enderecos[0].Cidade, " - ", enderecos[0].UF);
                }

                if (contrato.Tipo == (int)eTipoPessoa.Fisica)
                    instrucoes = ""; //"Valor mínimo de recarga: R$ 30,00<br>Para utilizar seu cartão, suas taxas administrativas devem estar em dia.<br>Para regularização e novos boletos - ligue 21 3916-7277<br>Todas as informações deste boleto são de responsabilidade do cedente";
                else
                {
                    if (cobranca.CobrancaRefID == null) //cobranca cujo vencimento não foi alterado
                    {
                        instrucoes = "0"; // "AS COBERTURAS DO SEGURO E DEMAIS BENEFICIOS ESTAO CONDICIONADAS AO PAGAMENTO DESTE DOCUMENTO.<br/>SR CAIXA, APOS O VENCIMENTO MULTA DE 10% E JUROS DE 1% A.D.<br/><br/>NAO RECEBER APOS 05 DIAS DO VENCIMENTO.";
                    }
                    else
                    {
                        instrucoes = "1"; //"AS COBERTURAS DO SEGURO E DEMAIS BENEFICIOS ESTAO CONDICIONADAS AO PAGAMENTO DESTE DOCUMENTO.<br/>SR CAIXA, NAO RECEBER APOS O VENCIMENTO.";
                    }

                    naoReceber = "";
                }

                String finalUrl = "";

                finalUrl = string.Concat("../../boleto/boleto_itau.aspx?bid=", beneficiario.ID, "&contid=", contrato.ID, "&cobid=", cobranca.ID, "&instru=", instrucoes);
                ScriptManager.RegisterClientScriptBlock(this,
                    this.GetType(),
                    "_geraBoleto_" + id,
                    String.Concat(" window.open(\"", finalUrl, "\", \"janela\", \"toolbar=no,scrollbars=1,width=860,height=420\"); "),
                    true);

                this.CarregarCobrancas();
                CobrancaLog log = new CobrancaLog();
                log.CobrancaEnviada(cobranca.ID, Usuario.Autenticado.ID, CobrancaLog.Fonte.Sistema);
                log = null;

                if (!string.IsNullOrWhiteSpace(email))
                {
                    MailMessage msg = new MailMessage(
                        new MailAddress(ConfigurationManager.AppSettings["mailFrom"], ConfigurationManager.AppSettings["mailFromName"]),
                        new MailAddress(email));

                    msg.Subject = "Link para boleto";

                    msg.IsBodyHtml = true;
                    msg.Body = string.Concat(
                        "Olá!<br><br>Para imprimir seu boleto ",
                        "<a href='", ConfigurationManager.AppSettings["appUrl"], "/boleto.aspx?key=", cobranca.ID, "' target='_blank'>clique aqui</a>.",
                        "<br><br>Atenciosamente,<br>Clube Azul");

                    try
                    {
                        SmtpClient client = new SmtpClient();
                        client.Send(msg);
                        msg.Dispose();
                        client = null;
                    }
                    catch { }
                }

                #endregion
            }
            else if (e.CommandName.Equals("detalhe"))
            {
                //Alerta(null, this, "_err", "Função indisponível para demonstração.");
                return;///////////////////////////////

                //Cobranca cobranca = new Cobranca(gridCobranca.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                //cobranca.Carregar();

                //txtIdCobrancaEmDetalhe.Text = Convert.ToString(cobranca.ID);
                //this.exibeDetalheCobranca(cobranca);
            }
        }

        protected void gridCobranca_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridCobranca.PageIndex = e.NewPageIndex;
            this.CarregarCobrancas();
        }

        #endregion

        #region helpers 

        protected String DateDiff(int interval, DateTime data)
        {
            String retorno = "";

            TimeSpan tsDuration;
            tsDuration = DateTime.Now - data;

            Int32 dias = 0;
            Decimal iMeses = 0;
            Int32 meses = 0;
            Decimal iAnos = 0;
            Int32 anos = 0;

            if (interval == 1)
            {
                iAnos = Convert.ToDecimal(tsDuration.Days / 365.25);
                anos = (int)iAnos;
                iMeses = Convert.ToDecimal((iAnos - anos) * 12);
                meses = (int)iMeses;
                dias = (int)((iMeses - meses) * 24);

                TimeSpan tsDurationDia;
                data = data.AddYears(anos);
                data = data.AddMonths(meses);
                tsDurationDia = DateTime.Now - data;

                retorno = Convert.ToString(anos + "a " + meses + "m " + tsDurationDia.Days + "d");
            }
            else if (interval == 2)
            //    retorno = Convert.ToString(Convert.ToInt32(tsDuration.Days / 365));
            //else if (interval == 3)
            {
                iAnos = Convert.ToDecimal(tsDuration.Days / 365.25);
                anos = (int)iAnos;

                TimeSpan tsDurationDia;
                data = data.AddYears(anos);
                data = data.AddMonths(meses);
                tsDurationDia = DateTime.Now - data;

                retorno = anos.ToString();
            }

            return retorno;
        }
        protected int GetMonthsBetween(DateTime from, DateTime to)
        {
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }

            var months = 0;
            while (from <= to) // at least one time
            {
                from = from.AddMonths(1);
                months++;
            }

            return months - 1;
        }

        protected String PegaDDD(String fone)
        {
            if (String.IsNullOrEmpty(fone)) { return String.Empty; }

            String[] aux = fone.Split(')');
            return aux[0].Replace("(", "").Trim();
        }
        protected String PegaTelefone(String fone)
        {
            if (String.IsNullOrEmpty(fone)) { return String.Empty; }

            String[] aux = fone.Split(')');
            if (aux.Length == 1) { return fone; }

            return aux[1].Trim();
        }

        protected Boolean IDKeyParameterInProcess(StateBag viewstate, String keyToUseInCache)
        {
            if (Session[IDKey] != null)
            {
                viewstate[IDKey] = Session[IDKey];
                Session.Remove(IDKey);
                //Cache.Remove(IDKey + keyToUseInCache);
                //Cache.Insert(IDKey + keyToUseInCache, viewstate[IDKey],
                //    null, DateTime.Now.AddMinutes(35), TimeSpan.Zero);
                return true;
            }
            else if (Cache[IDKey + keyToUseInCache] != null)
            {
                viewstate[IDKey] = Cache[IDKey + keyToUseInCache];
                return true;
            }
            else if (Request[IDKey] != null)
            {
                viewstate[IDKey] = Request[IDKey];
                return true;
            }
            else
                return false;
        }

        protected String TraduzTipoRelacaoDependenteContrato(int tipo)
        {
            ContratoBeneficiario.TipoRelacao tipoEnum = (ContratoBeneficiario.TipoRelacao)tipo;

            if (tipoEnum == ContratoBeneficiario.TipoRelacao.Agregado)
                return "Agregado";
            else if (tipoEnum == ContratoBeneficiario.TipoRelacao.Dependente)
                return "Dependente";
            else if (tipoEnum == ContratoBeneficiario.TipoRelacao.Titular)
                return "Titular";

            return String.Empty;
        }

        protected DateTime? CStringToDateTimeG(String strdata)
        {
            String[] arr = strdata.Split('/');
            if (arr.Length != 3) { return null; }

            return CStringToDateTime(strdata);
        }
        protected DateTime CStringToDateTime(String strdata)
        {
            String[] arr = strdata.Split('/');

            if (arr.Length != 3) { return DateTime.MinValue; }

            try
            {
                DateTime data = new DateTime(Int32.Parse(arr[2]), Int32.Parse(arr[1]), Int32.Parse(arr[0]));
                return data;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        protected DateTime CStringToDateTime(String strdata, String strhora)
        {
            String[] arr = strdata.Split('/');
            String[] arrH = strhora.Split(':');

            if (arr.Length != 3) { return DateTime.MinValue; }
            if (arrH.Length != 2) { return DateTime.MinValue; }
            try
            {
                DateTime data = new DateTime(Int32.Parse(arr[2]), Int32.Parse(arr[1]), Int32.Parse(arr[0]), Int32.Parse(arrH[0]), Int32.Parse(arrH[1]), Int32.Parse(DateTime.Now.Second.ToString()));
                return data;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        String dateToString(DateTime date)
        {
            if (date == DateTime.MinValue)
                return String.Empty;
            else
                return date.ToString("dd/MM/yyyy");
        }
        protected DateTime CStringToDateTime(String strdata, String strhora, Int32 segundos)
        {
            String[] arr = strdata.Split('/');
            String[] arrH = strhora.Split(':');

            if (arr.Length != 3) { return DateTime.MinValue; }
            if (arrH.Length != 2) { return DateTime.MinValue; }
            try
            {
                DateTime data = new DateTime(Int32.Parse(arr[2]), Int32.Parse(arr[1]), Int32.Parse(arr[0]), Int32.Parse(arrH[0]), Int32.Parse(arrH[1]), segundos);
                return data;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        protected String CToString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return String.Empty;
            else
                return Convert.ToString(param);
        }
        protected Object CToObject(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return null;
            else
                return param;
        }
        protected int CToInt(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return 0;
            else
                return Convert.ToInt32(param);
        }
        protected Decimal CToDecimal(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return 0;
            else
                return Convert.ToDecimal(param, new System.Globalization.CultureInfo("pt-Br"));
        }
        Boolean validaPeso(TextBox txtPeso)
        {
                return true;
        }
        Boolean validaAltura(TextBox txtAlt)
        {
                return true;
        }
        Boolean validaIMC_Titular(TextBox txtpeso, TextBox txtaltura)
        {
                return true;
        }

        Decimal getIMC(String strpeso, String straltura)
        {
            Decimal peso = CToDecimal(strpeso);
            Decimal altura = CToDecimal(straltura);

            return (peso / (altura * altura));
        }

        protected void Alerta(UpdatePanel uPanel, Page page, String chave, String Mensagem)
        {
            if (uPanel != null)
            {
                ScriptManager.RegisterClientScriptBlock(
                    uPanel, page.GetType(), chave, "alert('" + Mensagem + "');", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(
                    page, page.GetType(), chave, "alert('" + Mensagem + "');", true);
            }
        }

        protected void Alerta(AjaxControlToolkit.ModalPopupExtender MPE, ref Literal lit, String msg, UpdatePanel panel)
        {
            lit.Text = String.Concat("<font face='arial' size='1'>", msg, "</font>");
            MPE.Show();

            if (panel != null) { panel.Update(); }
        }

        AdicionalBeneficiario PegaNaColecao(IList<AdicionalBeneficiario> itens, Object adicionalId, Object beneficiarioId)
        {
            foreach (AdicionalBeneficiario _item in itens)
            {
                if (Convert.ToString(_item.AdicionalID) == Convert.ToString(adicionalId))
                {
                    return _item;
                }
            }

            return null;
        }

        void LimpaCamposENDERECO_TITULAR()
        {
        }

        #endregion
    }
}