using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using LC.Framework.Phantom;
using LC.Web.PadraoSeguros.Facade;
using LC.Web.PadraoSeguros.Entity;
using System.Collections;
using System.Data;
using System.Net.Mail;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;

using Ent = MedProj.Entidades;

namespace MedProj.www.clientes.clientes
{
    class Utilizacao
    {
        public long ID { get; set; }
        public string Prestador { get; set; }
        public string Especialidade { get; set; }
        public string Procedimento { get; set; }
        public string Valor { get; set; }
        public string Data { get; set; }
    }

    public partial class cliente : System.Web.UI.Page
    {
        #region properties

        protected const String IDKey = "_idkey";
        protected const String IDKey2 = "_idkey2";
        protected const String IDKey3 = "_idkey3";
        protected const String IDKey4 = "_idkey4";
        protected const String IDAdicionalKey = "_idAdicK";
        protected const String AlteraPlanoKey = "_altPlan";
        protected const String NovaDataAdmisssaoKey = "_nvDtAdm";
        protected const String PropostaEndReferecia = "_propEndRef";
        protected const String PropostaEndCobranca = "_propEndCob";
        protected const String ConferenciaObjKey = "_confObjKey";
        protected const String ArquivosObjKey = "_arquivosObjKey";
        protected const String FilialIDKey = "_filialIDKey";

        Object ContratoImpressoID
        {
            get { return Cache["___aci_" + Session.SessionID]; }
            set { Cache.Remove("___aci_" + Session.SessionID); if (value != null) { Cache.Insert("___aci_" + Session.SessionID, value, null, DateTime.Now.AddHours(1), TimeSpan.Zero); } }
        }

        Object contratoId
        {
            get
            {
                if (!String.IsNullOrEmpty(Request[IDKey])) { return Request[IDKey]; }
                else { return ViewState[IDKey]; }
            }
            set { ViewState[IDKey] = value; }
        }

        /// <summary>
        /// Guarda o ID da entidade Beneficiario para o titular.
        /// </summary>
        Object TitularID
        {
            get { return ViewState["_titId"]; }
            set
            {
                ViewState["_titId"] = value;
                Session["idBenefTit"] = value;
                Cache.Remove(Session.SessionID);
                if (value != null)
                {
                    Cache.Insert(Session.SessionID, value, null, DateTime.Now.AddMinutes(40), TimeSpan.Zero);
                }
            }
        }

        Object EnderecoTitularID
        {
            get { return ViewState["_titEndId"]; }
            set { ViewState["_titEndId"] = value; }
        }

        /// <summary>
        /// Guarda o ID da entidade ContratoBeneficiario para o titular.
        /// </summary>
        Object TitularID_ContratoBeneficiario
        {
            get { return ViewState["_tit_contr_benef"]; }
            set { ViewState["_tit_contr_benef"] = value; }
        }

        Object DependenteID
        {
            get { return ViewState["_depId"]; }
            set { ViewState["_depId"] = value; }
        }

        IList<ContratoBeneficiario> Dependentes
        {
            get { return ViewState["_depen"] as IList<ContratoBeneficiario>; }
            set { ViewState["_depen"] = value; }
        }

        /// <summary>
        /// Guarada a taxa assiciativa de estipulate, quando esta for para o contrato inteiro, e não por vida.
        /// </summary>
        Decimal ValorTaxaAssociativaContrato
        {
            get { if (ViewState["__valorTaxaConrato"] == null) { return Decimal.Zero; } else { return Convert.ToDecimal(ViewState["__valorTaxaConrato"]); } }
            set { ViewState["__valorTaxaConrato"] = value; }
        }

        Hashtable Valores
        {
            get { return ViewState["__valores"] as Hashtable; }
            set { ViewState["__valores"] = value; }
        }

        Decimal ValorTotalProposta
        {
            get { if (ViewState["__valorTotal"] == null) { return Decimal.Zero; } else { return Convert.ToDecimal(ViewState["__valorTotal"]); } }
            set { ViewState["__valorTotal"] = value; }
        }

        protected Boolean HaItemSelecionado(DropDownList combo)
        {
            if (combo.Items.Count == 0) { return false; }

            return combo.SelectedValue != "0" &&
                   combo.SelectedValue != "-1" &&
                   combo.SelectedValue != "";
        }

        #endregion

        void AdicionaValor(Object idContratoBeneficiario, Decimal valor)
        {
            if (Valores == null) { Valores = new Hashtable(); }
            Valores[Convert.ToString(idContratoBeneficiario)] = valor;
        }
        Decimal PegaValor(Object idContratoBeneficiario)
        {
            if (this.Valores == null) { return Decimal.Zero; }
            return Convert.ToDecimal(this.Valores[Convert.ToString(idContratoBeneficiario)]);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtValorCob.Attributes.Add("onKeyUp", "mascara('" + txtValorCob.ClientID + "')");
            txtValorDecontoAcrescimo.Attributes.Add("onKeyUp", "mascara('" + txtValorDecontoAcrescimo.ClientID + "')");
            txtOpManualValor.Attributes.Add("onKeyUp", "mascara('" + txtOpManualValor.ClientID + "')");
            txtAltCobrancaValor.Attributes.Add("onKeyUp", "mascara('" + txtAltCobrancaValor.ClientID + "')");

            cmdNovoTitular.Attributes.Add("onClick", "win = window.open('clienteP.aspx?et=1', 'janela', 'toolbar=no,scrollbars=1,width=860,height=420'); win.moveTo(100,150); return false;");
            cmdNovoBeneficiario.Attributes.Add("onClick", "win = window.open('clienteP.aspx?et=2&keyid=" + Session.SessionID + "', 'janela', 'toolbar=no,scrollbars=1,width=860,height=420'); win.moveTo(100,150); return false;");

            txtCarenciaTempoContrato.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");
            txtEmpresaAnteriorMeses.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");
            txtCarenciaDependenteTempoContrato.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");

            txtValorTotal.Attributes.Add("onKeyUp", "mascara('" + txtValorTotal.ClientID + "')");
            txtTitPeso.Attributes.Add("onKeyUp", "mascara('" + txtTitPeso.ClientID + "')");
            txtTitAltura.Attributes.Add("onKeyUp", "mascara('" + txtTitAltura.ClientID + "')");
            txtDepPeso.Attributes.Add("onKeyUp", "mascara('" + txtDepPeso.ClientID + "')");
            txtDepAltura.Attributes.Add("onKeyUp", "mascara('" + txtDepAltura.ClientID + "')");

            //cmdSalvarAtendimento.OnClientClick = "alert(document.getElementById('" + cboTipoAtendimento.ClientID + "').value); return false;";
            cmdSalvarAtendimento.OnClientClick = "if(document.getElementById('" + cboTipoAtendimento.ClientID + "').value == '2') { return confirm('Esta ação irá gerar imediatamente uma nova via de carteirinha.\\nDeseja realmente prosseguir?'); } else { return true; }";

            //txtIdCobrancaEmDetalhe.Attributes.Add("style", "display:none");

            this.PreparaLinkParaEditarTitular();

            //UIHelper.AuthCtrl(cmdRecalcularComposicao, null);

            txtSenhaContrato.Attributes.Add("value", txtSenhaContrato.Text);

            if (!IsPostBack)
            {
                litSaldo.Text = "<strong>Saldo:</strong> R$ 0,00";
                txtObs.Visible = false;
                txtDataInicio.Text = DateTime.Now.ToString("dd/MM/yyyy");

                //this.carregarCorretores();

                txtDataDeUtilizacao.Text = DateTime.Now.AddDays(-20).ToString("dd/MM/yyyy");
                txtDataAteUtilizacao.Text = DateTime.Now.ToString("dd/MM/yyyy");

                if(!optCNPJ.Checked)
                    txtVencimentoCob.Text = DateTime.Now.AddYears(1).ToString("dd/MM/yyyy");
                else
                    txtVencimentoCob.Text = DateTime.Now.AddDays(3).ToString("dd/MM/yyyy");

                this.carregaMotivoStatus();

                this.carregaTiposDeAtendimento();
                AtendimentoTemp.UI.FillCombo(cboSubTipoAtendimento);
                cboSubTipoAtendimento.Items.Insert(0, new ListItem("selecione", "0"));

                //txtIdKey.Value = Session.SessionID;
                this.ContratoImpressoID = null;
                //Usuario user = new Usuario();
                //user.ID = Util.UsuarioLogado.ID; //Usuario.Autenticado.ID; 1063
                //user.Carregar();
                //txtValorTotal.ReadOnly = !user.AlteraValorContratos;
                //user = null;
                this.TitularID = null;

                txtDataContrato.Text = DateTime.Now.ToString("dd/MM/yyyy");

                this.CarregaOperadoras();
                if (IDKeyParameterInProcess(ViewState, "_contr"))
                    ExibirEstipulantes(cboEstipulante, true, false);
                else
                    ExibirEstipulantes(cboEstipulante, true, true);

                this.CarregaContratoADM();
                this.CarregaPlanos();
                this.CarregaEstadoCivil();
                ExibirOpcoesDeSexo(cboSexo, false);
                ExibirOpcoesDeSexo(cboSexoDependente, false);
                ExibirOpcoesDeTipoDeContrato(cboTipoProposta, true);
                this.CarregaFiliais();
                if (IDKeyParameterInProcess(ViewState, "_contr"))
                {
                    //this.checaEnriquecimento();
                    Session[ConferenciaObjKey] = null;
                    this.CarregaContrato();
                    this.carregaUtilizacao();
                    this.MontaCombosDeBeneficiarios();
                    this.SetaEstadoDosAdicionais();
                    this.ExibeSumario();
                    lnkOkContrato.Visible = false;

                    //pnlHistoricoPlano.Visible = true;
                    //gridHistoricoPlano.DataSource = ContratoPlano.Carregar(contratoId);
                    //gridHistoricoPlano.DataBind();

                    txtObs.Visible = true;
                    txtObs.ReadOnly = true;
                }
                else if (Session[ConferenciaObjKey] != null)
                {
                    Conferencia conferencia = (Conferencia)Session[ConferenciaObjKey];
                    //this.CarregaContratoAPartirDaConferencia(conferencia);
                }
                else
                {
                    //lnkOkContrato.Visible = true;
                    //cmdSalvar.Enabled = false;
                    cmdAlterarPlano.Visible = false;
                }


                if (pnlAtendimento.Visible) { this.ConfiguraAtendimento(); this.CarregaAtendimentoEmFoco(); }
            }

            if (optCPF.Checked)
                txtCPF.Attributes.Add("onkeypress", "mascara_CPF(this, event);");
            else
                txtCPF.Attributes.Add("onkeypress", "mascara_CNPJ(this, event);");
        }

        //void carregarCorretores()
        //{
        //    List<Ent.Corretor> lista = CorretorFacade.Instancia.CarregarTodos(string.Empty);
        //    cboCorretor.Items.Clear();
        //    cboCorretor.Items.Add(new ListItem("selecione", "-1"));

        //    if (lista != null)
        //    {
        //        foreach (Ent.Corretor c in lista)
        //        {
        //            cboCorretor.Items.Add(new ListItem(c.Nome, c.ID.ToString()));
        //        }
        //    }
        //}

        #region metodos 

        void carregaUtilizacao()
        {
            if (this.contratoId == null) return;

            DateTime de = Util.CTipos.CStringToDateTime(txtDataDeUtilizacao.Text);
            DateTime ate = Util.CTipos.CStringToDateTime(txtDataAteUtilizacao.Text, 23, 59, 59, 990);

            if (de != DateTime.MinValue && ate != DateTime.MinValue)
                gridUtilizacao.DataSource = ContratoFacade.Instance.
                    CarregarHistoricoMov(Util.CTipos.CTipo<long>(this.contratoId), de, ate);
            else
                gridUtilizacao.DataSource = null;

            gridUtilizacao.DataBind();
            //List<Utilizacao> lista = new List<Utilizacao>();

            //lista.Add
            //(  
            //    new Utilizacao
            //    {
            //        Data = "23/04/2014",
            //        Especialidade = "CARDIOLOGIA",
            //        Prestador = "PRESTADOR CADASTRADO",
            //        Procedimento = "ELETROCARDIOGRAMA DE ALTA RESOLUCAO",
            //        Valor = "R$ 180,00"
            //    }
            //);

            //lista.Add
            //(
            //    new Utilizacao
            //    {
            //        Data = "10/01/2014",
            //        Especialidade = "CARDIOLOGIA",
            //        Prestador = "PRESTADOR CADASTRADO",
            //        Procedimento = "CORAÇÃO - MORFOLÓGICO E FUNCIONAL",
            //        Valor = "R$ 80,59"
            //    }
            //);

            //gridUtilizacao.DataSource = lista;
            //gridUtilizacao.DataBind();
        }

        void CarregaDependentes()
        {
            gridDependentes.DataSource = ContratoBeneficiario.CarregarPorContratoID(ViewState[IDKey], true);
            gridDependentes.DataBind();
            spanDependentesCadastrados.Visible = gridDependentes.Rows.Count > 0;
        }

        void CarregaOpcoesParaAgregadosOuDependentes()
        {
            cboParentescoDependente.Items.Clear();
            cboParentescoResponsavel.Items.Clear();

            if (cboContrato.Items.Count == 0) { return; }

            IList<ContratoADMParentescoAgregado> lista =
                ContratoADMParentescoAgregado.Carregar(
                cboContrato.SelectedValue,
                Parentesco.eTipo.Indeterminado);

            cboParentescoDependente.DataValueField = "ID";
            cboParentescoDependente.DataTextField = "ParentescoDescricao";
            cboParentescoResponsavel.DataValueField = "ID";
            cboParentescoResponsavel.DataTextField = "ParentescoDescricao";

            cboParentescoDependente.DataSource = lista;
            cboParentescoDependente.DataBind();
            cboParentescoResponsavel.DataSource = lista;
            cboParentescoResponsavel.DataBind();
        }

        void CarregarAtendimentos()
        {
            if (this.contratoId == null) { pnlAtendimento.Visible = false; return; }

            if (String.IsNullOrEmpty(Usuario.Autenticado.EmpresaCobrancaID))
                gridAtendimento.DataSource = AtendimentoTemp.CarregarPorProposta(this.contratoId);
            else
                gridAtendimento.DataSource = AtendimentoTemp.CarregarPorProposta(this.contratoId, AtendimentoTipo.eTipo.EmpresaCobranca);

            gridAtendimento.DataBind();
        }

        IList<Cobranca> CarregarCobrancas(bool ativas = true)
        {
            if (this.contratoId == null) { return null; }
            //IList<Cobranca> cobrancas = Cobranca.CarregarTodasComParcelamentoInfo_Composite(this.contratoId, true, true, null);
            IList<Cobranca> cobrancas = Cobranca.CarregarTodas(this.contratoId, ativas, true, null);
            gridCobranca.DataSource = cobrancas; //NEGOCIACAO ComParcelamentoInfo
            gridCobranca.DataBind();
            return cobrancas;
        }

        void ConfiguraAtendimento()
        {
            try
            {

                if (this.contratoId == null) { return; }
                this.CarregarAtendimentos();
                IList<Cobranca> cobrancas = this.CarregarCobrancas();
                List<CobrancaComposite> composite = null;

                Contrato contrato = new Contrato(this.contratoId);
                contrato.Carregar();

                if (cobrancas != null && cobrancas.Count > 0)
                {
                    txtParcelaCob.Text = (cobrancas[0].Parcela + 1).ToString(); // (cobrancas[cobrancas.Count - 1].Parcela + 1).ToString();
                    DateTime proxVencto = DateTime.MinValue;

                    proxVencto = cobrancas[0].DataVencimento.AddMonths(1);

                    DateTime vigencia, vencimento, admissao = CStringToDateTime(txtAdmissao.Text);
                    Int32 diaDataSemJuros; Object valorDataLimite;
                    CalendarioVencimento rcv = null;

                    if (!HaItemSelecionado(cboContrato)) { return; } //base.CStringToDateTime(txtAdmissao.Text)
                    CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(cboContrato.SelectedValue,
                       admissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv);

                    if (cobrancas.Count >= 2)
                        proxVencto = new DateTime(proxVencto.Year,
                            proxVencto.Month, contrato.Vencimento.Day, 23, 59, 59, 599);
                    else if (cobrancas.Count == 1)
                        proxVencto = new DateTime(contrato.Vencimento.Year,
                            contrato.Vencimento.Month, contrato.Vencimento.Day, 23, 59, 59, 599);

                    //txtVencimentoCob.Text = proxVencto.ToString("dd/MM/yyyy");
                    //txtValorCob.Text = Contrato.CalculaValorDaProposta(this.contratoId, proxVencto, null, false, true, ref composite, true).ToString("N2");
                }
                else
                {
                    txtParcelaCob.Text = "1";
                    //txtVencimentoCob.Text = contrato.Vencimento.ToString("dd/MM/yyyy");
                    //txtValorCob.Text = Contrato.CalculaValorDaProposta(contrato.ID, contrato.Vencimento, null, false, true, ref composite).ToString("N2");
                }

                //if (Convert.ToDecimal(txtValorCob.Text) == Decimal.Zero)
                //    cmdGerarCobranca.Enabled = false;

                ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(this.contratoId, null);

                if (titular != null)
                {
                    Beneficiario beneficiario = new Beneficiario(titular.BeneficiarioID);
                    beneficiario.Carregar();
                    txtEmailAtendimento.Text = beneficiario.Email;
                }
            }
            catch
            {
            }
        }

        void CarregaAtendimentoEmFoco()
        {
            if (!String.IsNullOrEmpty(Request["prot"]))
            {
                tab.ActiveTabIndex = 7;
                AtendimentoTemp atendimento = new AtendimentoTemp(Request["prot"]);
                atendimento.Carregar();
                this.exibeAtendimento(atendimento);

                for (int i = 0; i < gridAtendimento.Rows.Count; i++)
                {
                    if (Convert.ToString(gridAtendimento.DataKeys[Convert.ToInt32(i)][0]) ==
                        Request["prot"])
                    {
                        gridAtendimento.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        void MontaCombosDeBeneficiarios()
        {
            IList<Beneficiario> lista = new List<Beneficiario>();

            if (this.TitularID != null)
            {
                Beneficiario titular = new Beneficiario();
                titular.ID = Convert.ToString(this.TitularID);
                titular.Nome = txtNome.Text;
                lista.Add(titular);
            }

            if (this.Dependentes != null)
            {
                foreach (ContratoBeneficiario _dependente in this.Dependentes)
                {
                    Beneficiario dependente = new Beneficiario();
                    dependente.ID = _dependente.BeneficiarioID;
                    dependente.Nome = _dependente.BeneficiarioNome;
                    lista.Add(dependente);
                }
            }

            cboBeneficiarioFicha.DataValueField = "ID";
            cboBeneficiarioFicha.DataTextField = "Nome";
            cboBeneficiarioFicha.DataSource = lista;
            cboBeneficiarioFicha.DataBind();
            //this.CarregaFichaDeSaudeDeTodosBeneficiarios();

            cboBeneficiarioAdicional.DataValueField = "ID";
            cboBeneficiarioAdicional.DataTextField = "Nome";
            cboBeneficiarioAdicional.DataSource = lista;
            cboBeneficiarioAdicional.DataBind();
            cboBeneficiarioAdicional_SelectedIndexChanged(null, null);

            this.ExibeSumario();
        }

        void ExibeSumario()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            Decimal total = 0;

            Contrato contrato = new Contrato(this.contratoId);
            if (this.contratoId != null) { contrato.Carregar(); }

            //checa se tem taxa associativa para a proposta inteira (e se deve cobrar)
            if (HaItemSelecionado(cboEstipulante) && chkCobrarTaxa.Visible && chkCobrarTaxa.Checked)
            {
                //checa se há taxa associativa

                EstipulanteTaxa taxa = null;
                if (this.contratoId == null)
                    taxa = EstipulanteTaxa.CarregarVigente(cboEstipulante.SelectedValue);
                else
                    taxa = EstipulanteTaxa.CarregarVigente(cboEstipulante.SelectedValue, contrato.Admissao);

                if (taxa != null && taxa.Valor > 0 && ((EstipulanteTaxa.eTipoTaxa)taxa.TipoTaxa) == EstipulanteTaxa.eTipoTaxa.PorProposta)
                {
                    sb.Append("<table cellpadding='2' cellspacing='0' border='0' width='100%'>");
                    sb.AppendLine("<tr><td class='tdPrincipal1'>TAXA ASSOCIATIVA: ");
                    sb.AppendLine(taxa.Valor.ToString("C"));
                    sb.AppendLine("</td></tr>");
                    sb.AppendLine("</table>");
                    total += taxa.Valor;
                    ValorTaxaAssociativaContrato = taxa.Valor;
                }
            }

            foreach (ListItem itemBeneficiario in cboBeneficiarioAdicional.Items)
            {
                if (sb.Length > 0) { sb.AppendLine("<br>"); }
                sb.AppendLine("<table cellpadding='2' cellspacing='0' border='0' width='100%'>");

                IList<AdicionalBeneficiario> adicionais =
                    (IList<AdicionalBeneficiario>)ViewState["adben_" + itemBeneficiario.Value];

                Beneficiario beneficiario = new Beneficiario();
                beneficiario.ID = itemBeneficiario.Value;
                beneficiario.Carregar();
                int idade = Convert.ToInt32(DateDiff(2, beneficiario.DataNascimento));

                sb.AppendLine("<tr><td class='tdPrincipal1' colspan='2'>");
                sb.Append(beneficiario.Nome);
                sb.Append("</td></tr>");

                //IDADE
                sb.AppendLine("<tr class='tdNormal1'><td width='25%'>Idade</td><td>");
                sb.Append(idade);
                sb.Append("</td></tr>");

                Decimal valor = 0, valorBeneficiario = 0;
                if (HaItemSelecionado(cboPlano))
                {
                    //VALOR DO PLANO
                    if (cboAcomodacao.SelectedIndex > 0)
                    {
                        valor = TabelaValor.CalculaValor(itemBeneficiario.Value, idade, cboContrato.SelectedValue, cboPlano.SelectedValue, ((Contrato.eTipoAcomodacao)Convert.ToInt32(cboAcomodacao.SelectedValue)), CStringToDateTime(txtAdmissao.Text), null);
                        valorBeneficiario += valor;
                    }
                    total += valor;
                    sb.AppendLine("<tr class='tdNormal1'><td>Valor plano</td><td>");
                    sb.Append(valor.ToString("C"));
                    sb.Append("</td></tr>");
                }

                if (HaItemSelecionado(cboEstipulante) && chkCobrarTaxa.Visible && chkCobrarTaxa.Checked)
                {
                    //checa se há taxa associativa
                    EstipulanteTaxa taxa = EstipulanteTaxa.CarregarVigente(cboEstipulante.SelectedValue);
                    if (taxa != null && taxa.Valor > 0 && ((EstipulanteTaxa.eTipoTaxa)taxa.TipoTaxa) == EstipulanteTaxa.eTipoTaxa.PorVida)
                    {
                        sb.AppendLine("<tr class='tdNormal1'><td>");
                        sb.Append("Taxa associativa");
                        sb.Append("</td><td>");
                        total += taxa.Valor;
                        valorBeneficiario += taxa.Valor;
                        sb.Append(taxa.Valor.ToString("C"));
                        sb.Append("</td></tr>");
                    }
                }
                if (adicionais != null)
                {
                    foreach (AdicionalBeneficiario adicional in adicionais)
                    {
                        if (adicional.BeneficiarioID == null) { continue; }

                        Adicional produto = new Adicional();
                        produto.ID = adicional.AdicionalID;
                        produto.Carregar();
                        sb.AppendLine("<tr class='tdNormal1'><td>");
                        sb.Append(produto.Descricao);
                        sb.Append("</td><td>");

                        if (contrato.ID == null)
                            valor = Adicional.CalculaValor(adicional.AdicionalID, adicional.BeneficiarioID, idade);
                        else
                            valor = Adicional.CalculaValor(adicional.AdicionalID, adicional.BeneficiarioID, idade, contrato.Vigencia, null);

                        total += valor;
                        valorBeneficiario += valor;
                        sb.Append(valor.ToString("C"));
                        sb.Append("</td></tr>");
                    }
                }

                sb.AppendLine("</table>");
                this.AdicionaValor(itemBeneficiario.Value, valorBeneficiario);
            }

            litSumario.Text = sb.ToString();
            sb.Remove(0, sb.Length);
            sb = null;
            Decimal desconto = CToDecimal(txtDesconto.Text);
            total = total - desconto;
            //txtValorTotal.Text = total.ToString("N2");
            this.ValorTotalProposta = total;
            upFinalizacao.Update();
        }

        void CarregaFiliais()
        {
            ExibirFiliais(cboFilial, false);
            cboFilial.Items.Insert(0, new ListItem("", "branco"));

            if (Session[FilialIDKey] != null)
            {
                cboFilial.SelectedValue = Convert.ToString(Session[FilialIDKey]);
            }
        }

        void CarregaContratoADM()
        {
            cboContrato.Items.Clear();
            if (!HaItemSelecionado(cboEstipulante) || !HaItemSelecionado(cboOperadora))
            {
                cboContrato.Items.Clear();
                return;
            }

            cboContrato.DataValueField = "ID";
            cboContrato.DataTextField = "DescricaoCodigoSaudeDental";

            IList<ContratoADM> lista = null;
            if (IDKeyParameterInProcess(ViewState, "_contr"))
                lista = ContratoADM.CarregarTodos(
                    cboEstipulante.SelectedValue, cboOperadora.SelectedValue, false);
            else
                lista = ContratoADM.Carregar(
                    cboEstipulante.SelectedValue, cboOperadora.SelectedValue, true);

            cboContrato.DataSource = lista;
            cboContrato.DataBind();
            cboContrato.Items.Insert(0, new ListItem("selecione", "-1"));
        }

        void PreparaLinkParaEditarTitular()
        {
            if (this.TitularID != null)
            {
                cmdAlterarBeneficiarioTitular.Attributes.Add("onClick", "win = window.open('clienteP.aspx?et=1&" + IDKey + "=" + this.TitularID + "', 'janela', 'toolbar=no,scrollbars=1,width=860,height=420'); win.moveTo(100,150); return false;");
            }
        }

        void carregaMotivoStatus()
        {
            cboStatusMotivo.DataValueField = "ID";
            cboStatusMotivo.DataTextField = "Descricao";

            ContratoStatus.eTipo tipo = ContratoStatus.eTipo.Indefinido;
            if (optNormalEdit.Checked) tipo = ContratoStatus.eTipo.Reativacao;
            else if (optInativoEdit.Checked) tipo = ContratoStatus.eTipo.Inativacao;
            else if (optCanceladoEdit.Checked) tipo = ContratoStatus.eTipo.Cancelamento;

            cboStatusMotivo.DataSource = ContratoStatus.Carregar(tipo);
            cboStatusMotivo.DataBind();
        }

        void carregaTiposDeAtendimento()
        {
            cboTipoAtendimento.Items.Clear();
            IList<AtendimentoTipo> tipos = null;

            if (String.IsNullOrEmpty(Usuario.Autenticado.EmpresaCobrancaID))
                tipos = AtendimentoTipo.CarregarTodos();
            else
                tipos = AtendimentoTipo.CarregarTodos(AtendimentoTipo.eTipo.EmpresaCobranca);

            if (tipos != null)
            {
                foreach (AtendimentoTipo tipo in tipos)
                {
                    cboTipoAtendimento.Items.Add(new ListItem(
                        tipo.Descricao, Convert.ToString(tipo.ID)));
                }
            }

            cboTipoAtendimento.Items.Insert(0, new ListItem("selecione", "-1"));
        }

        void CarregaOperadoras()
        {
            if (IDKeyParameterInProcess(ViewState, "_contr"))
                ExibirOperadoras(cboOperadora, true, false);
            else
                ExibirOperadoras(cboOperadora, true, true);
        }

        void CarregaEstadoCivil()
        {
            cboEstadoCivil.Items.Clear();
            cboEstadoCivilDependente.Items.Clear();

            cboEstadoCivil.Items.Add(new ListItem("Casado(a)", "0"));
            cboEstadoCivil.Items.Add(new ListItem("Solteiro(a)", "1"));
            cboEstadoCivil.Items.Add(new ListItem("Divorciado(a)", "2"));
            cboEstadoCivil.Items.Add(new ListItem("Viúvo(a)", "3"));

            //if (!HaItemSelecionado(cboOperadora)) { return; }

            //IList<EstadoCivil> lista = EstadoCivil.CarregarTodos(cboOperadora.SelectedValue);
            //cboEstadoCivil.DataValueField = "ID";
            //cboEstadoCivil.DataTextField = "Descricao";
            //cboEstadoCivil.DataSource = lista;
            //cboEstadoCivil.DataBind();

            //cboEstadoCivilDependente.DataValueField = "ID";
            //cboEstadoCivilDependente.DataTextField = "Descricao";
            //cboEstadoCivilDependente.DataSource = lista;
            //cboEstadoCivilDependente.DataBind();

        }

        void CarregaPlanos()
        {
            this.CarregaAdicionais(true);
            if (!HaItemSelecionado(cboContrato)) { cboPlano.Items.Clear(); cboAcomodacao.Items.Clear(); return; }
            IList<Plano> planos = Plano.CarregarPorContratoID(cboContrato.SelectedValue, true);

            cboPlano.Items.Clear();
            cboPlano.DataValueField = "ID";
            cboPlano.DataTextField = "DescricaoPlanoSubPlano";
            cboPlano.DataSource = planos;
            cboPlano.DataBind();
            cboPlano.Items.Insert(0, new ListItem("Selecione", "-1"));

            //cboPlanoAltera.Items.Clear();
            //cboPlanoAltera.DataValueField = "ID";
            //cboPlanoAltera.DataTextField = "DescricaoPlanoSubPlano";
            //cboPlanoAltera.DataSource = planos;
            //cboPlanoAltera.DataBind();

            this.CarregaAcomodacoes();
        }

        void CarregaAdicionais(Boolean atualizar)
        {
            if (cboBeneficiarioAdicional.SelectedValue == "" || cboOperadora.SelectedValue == "" || cboOperadora.SelectedValue == "-1") { return; }

            if (atualizar)
            {
                foreach (ListItem item in cboBeneficiarioAdicional.Items)
                    ViewState["adben_" + item.Value] = null;
            }

            if (ViewState["adben_" + cboBeneficiarioAdicional.SelectedValue] == null || atualizar)
            {
                foreach (ListItem item in cboBeneficiarioAdicional.Items)
                {
                    if (cboPlano.SelectedIndex > 0)
                    {
                        IList<AdicionalBeneficiario> lista = AdicionalBeneficiario.
                            Carregar(cboContrato.SelectedValue, cboPlano.SelectedValue, ViewState[IDKey], item.Value);

                        if (item.Value == cboBeneficiarioAdicional.SelectedValue)
                        {
                            gridAdicional.DataSource = lista;
                        }
                        ViewState["adben_" + item.Value] = lista;
                    }
                }
            }
            else
            {
                gridAdicional.DataSource = ViewState["adben_" + cboBeneficiarioAdicional.SelectedValue];
            }

            gridAdicional.DataBind();
        }

        void CarregaAcomodacoes()
        {
            if (cboPlano.SelectedIndex > 0)
            {
                Plano plano = new Plano(cboPlano.SelectedValue);
                plano.Carregar();

                ExibirTiposDeAcomodacao(cboAcomodacao, plano.QuartoComum, plano.QuartoParticular, true);
                ///////////////////////////////////////////////////////////////////////////////////////////
                //this.CarregaAcomodacoesAlteracao(plano);
            }
            else
            {
                cboAcomodacao.Items.Clear();
                cboAcomodacao.Items.Add(new ListItem("Selecione", "-1"));
                //cboAcomodacaoAltera.Items.Clear();
            }
        }

        void CarregaContrato()
        {
            ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(ViewState[IDKey], null);
            if (titular != null && titular.Status != (Int32)ContratoBeneficiario.eStatus.Novo)
            {
                cmdCarregaBeneficiarioPorRG.Visible = false;
                cmdNovoTitular.Visible = false;
            }
            titular = null;

            Contrato contrato = new Contrato();
            contrato.ID = ViewState[IDKey];
            contrato.Carregar();

            txtSenhaContrato.Attributes.Add("value", contrato.Senha);

            if (contrato.Tipo == (int)eTipoPessoa.Fisica)
            {
                optCPF.Checked = true;
                optCNPJ.Checked = false;
            }
            else
            {
                optCPF.Checked = false;
                optCNPJ.Checked = true;
            }

            this.configuraTipoPessoa();

            if (contrato.Inativo || contrato.Cancelado)
                cmdGerarCobranca.Enabled = false;

            if (contrato.FilialID != null)
            {
                cboFilial.SelectedValue = Convert.ToString(contrato.FilialID);
            }

            if (contrato.ID == null) { ViewState[IDKey] = null; return; }

            this.ContratoImpressoID = contrato.NumeroID;

            #region Aba1

            //AlmoxContratoImpresso aci = null;//

            //if (contrato.NumeroID != null)
            //{
            //    aci = new AlmoxContratoImpresso(contrato.NumeroID);
            //    aci.Carregar();

            //    txtNumeroContrato.Text = aci.NumeroDoImpresso;
            //    txtNumeroContratoConfirme.Text = txtNumeroContrato.Text;
            //}
            //else
            //{
                txtNumeroContrato.Text = contrato.Numero;
                txtNumeroContratoConfirme.Text = txtNumeroContrato.Text;
            //}

            txtCorretorTerceiroCPF.Text = contrato.CorretorTerceiroCPF;
            txtCorretorTerceiroIdentificacao.Text = contrato.CorretorTerceiroNome;
            txtSuperiorTerceiroCPF.Text = contrato.SuperiorTerceiroCPF;
            txtSuperiorTerceiroIdentificacao.Text = contrato.SuperiorTerceiroNome;

            if (Usuario.Autenticado.PerfilID != Perfil.AdministradorIDKey)
            {
                //txtNumeroContrato.ReadOnly = true;
                //txtNumeroContratoConfirme.ReadOnly = true;
            }

            lnkOkContrato.Visible = false;
            txtNumeroMatricula.Text = contrato.NumeroMatricula;

            if (contrato.Admissao != DateTime.MinValue)
            {
                txtAdmissao.Text = contrato.Admissao.ToString("dd/MM/yyyy");
                txtVigencia.Text = contrato.Vigencia.ToString("dd/MM/yyyy");
                txtVencimento.Text = contrato.Vencimento.ToString("dd/MM/yyyy");
            }

            //if (contrato.CorretorComissionadoId != null)
            //    cboCorretor.SelectedValue = Convert.ToString(contrato.CorretorComissionadoId);
            //else
            //    cboCorretor.SelectedIndex = 0;

            cboEstipulante.SelectedValue = Convert.ToString(contrato.EstipulanteID);
            cboOperadora.SelectedValue = Convert.ToString(contrato.OperadoraID);
            cboOperadora_SelectedIndexChanged(null, null);
            cboContrato.SelectedValue = Convert.ToString(contrato.ContratoADMID);
            cboContrato_SelectedIndexChanged(null, null);
            cboPlano.SelectedValue = Convert.ToString(contrato.PlanoID);
            this.CarregaAcomodacoes();

            cboAcomodacao.SelectedValue = Convert.ToString(contrato.TipoAcomodacao);

            if (cboTipoProposta.Items.FindByValue(Convert.ToString(contrato.TipoContratoID)) != null)
                cboTipoProposta.SelectedValue = Convert.ToString(contrato.TipoContratoID);
            else
                cboTipoProposta.SelectedIndex = 0;

            if (chkCobrarTaxa.Visible && contrato.CobrarTaxaAssociativa)
                chkCobrarTaxa.Checked = true;

            if (!String.IsNullOrEmpty(contrato.EmpresaAnterior))
            {
                pnlInfoAnterior.Visible = true;
                txtEmpresaAnterior.Text = contrato.EmpresaAnterior;
                txtEmpresaAnteriorMeses.Text = Convert.ToString(contrato.EmpresaAnteriorTempo);
                txtEmpresaAnteriorMatricula.Text = contrato.EmpresaAnteriorMatricula;
            }

            this.PreencheCampoCorretor(contrato.DonoID);

            if (contrato.OperadorTmktID != null)
            {
                Usuario operador = Usuario.CarregarParcial(contrato.OperadorTmktID);

                if (operador != null)
                {
                    txtOperadorID.Value = Convert.ToString(operador.ID);
                    txtOperador.Text = String.Concat(operador.Nome, " (", operador.Documento1, ")");
                }
            }

            #endregion

            #region Abas 2, 3 e 4

            txtNomeResponsavel.Text = contrato.ResponsavelNome;
            txtCPFResponsavel.Text = contrato.ResponsavelCPF;
            txtRGResponsavel.Text = contrato.ResponsavelRG;
            if (contrato.ResponsavelDataNascimento != DateTime.MinValue)
                txtDataNascimentoResponsavel.Text = contrato.ResponsavelDataNascimento.ToString("dd/MM/yyyy");
            cboSexoResponsavel.SelectedValue = contrato.ResponsavelSexo;
            if (contrato.ResponsavelParentescoID != null)
                cboParentescoResponsavel.SelectedValue = Convert.ToString(contrato.ResponsavelParentescoID);


            IList<ContratoBeneficiario> beneficiariosContrato =
                ContratoBeneficiario.CarregarPorContratoID(contrato.ID, false);

            if (beneficiariosContrato != null)
            {
                IList<ContratoBeneficiario> dependentes = new List<ContratoBeneficiario>();

                foreach (ContratoBeneficiario _beneficiarioContrato in beneficiariosContrato)
                {
                    if (_beneficiarioContrato.Tipo == Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular))
                    {
                        this.TitularID = _beneficiarioContrato.BeneficiarioID;
                        this.TitularID_ContratoBeneficiario = _beneficiarioContrato.ID;
                        if (_beneficiarioContrato.EstadoCivilID != null)
                            cboEstadoCivil.SelectedValue = Convert.ToString(_beneficiarioContrato.EstadoCivilID);

                        if (_beneficiarioContrato.DataCasamento != DateTime.MinValue)
                            txtTitDataCasamento.Text = _beneficiarioContrato.DataCasamento.ToString("dd/MM/yyyy");

                        txtNumMatriculaSaude.Text = _beneficiarioContrato.NumeroMatriculaSaude;
                        txtNumMatriculaDental.Text = _beneficiarioContrato.NumeroMatriculaDental;

                        Beneficiario beneficiarioTitular = new Beneficiario();
                        beneficiarioTitular.ID = _beneficiarioContrato.BeneficiarioID;
                        beneficiarioTitular.Carregar();

                        this.ExibeBeneficiarioCarregado(beneficiarioTitular, null);

                        cboCarenciaOperadora.Text = CToString(_beneficiarioContrato.CarenciaOperadora);
                        txtCarenciaOperadoraID.Value = CToString(_beneficiarioContrato.CarenciaOperadoraID);
                        txtCarenciaCodigo.Text = _beneficiarioContrato.CarenciaCodigo;
                        txtCarenciaMatricula.Text = _beneficiarioContrato.CarenciaMatriculaNumero;
                        txtCarenciaTempoContrato.Text = _beneficiarioContrato.CarenciaContratoTempo.ToString();
                        if (_beneficiarioContrato.CarenciaContratoDe != DateTime.MinValue)
                            txtTitTempoContratoDe.Text = _beneficiarioContrato.CarenciaContratoDe.ToString("dd/MM/yyyy");
                        if (_beneficiarioContrato.CarenciaContratoAte != DateTime.MinValue)
                            txtTitTempoContratoAte.Text = _beneficiarioContrato.CarenciaContratoAte.ToString("dd/MM/yyyy");

                        if (_beneficiarioContrato.CarenciaContratoDe != DateTime.MinValue && _beneficiarioContrato.CarenciaContratoAte != DateTime.MinValue)
                        {
                            txtCarenciaTempoContrato.Text = GetMonthsBetween(_beneficiarioContrato.CarenciaContratoDe, _beneficiarioContrato.CarenciaContratoAte).ToString();
                        }

                        txtPortabilidade.Text = _beneficiarioContrato.Portabilidade;
                    }
                    else // se NAO é titular, adiciona na colecao de dependentes
                    {
                        dependentes.Add(_beneficiarioContrato);
                    }
                }

                if (dependentes.Count > 0) { this.Dependentes = dependentes; }
                gridDependentes.DataSource = dependentes;
                gridDependentes.DataBind();
            }

            #endregion

            #region Aba5

            txtDataContrato.Text = contrato.Data.ToString("dd/MM/yyyy");
            txtDesconto.Text = contrato.Desconto.ToString("N2");

            Usuario usuarioEmissor = new Usuario();

            if (usuarioEmissor.ID != null && Convert.ToInt32(usuarioEmissor.ID) > 0)
            {
                usuarioEmissor.ID = contrato.DonoID;
                usuarioEmissor.Carregar();
            }

            if (contrato.Inativo)
            {
                optInativo.Checked = true;
                optCancelado.Checked = false;
                optNormal.Checked = false;
            }
            else if (contrato.Cancelado)
            {
                optCancelado.Checked = true;
                optInativo.Checked = false;
                optNormal.Checked = false;
            }
            else
            {
                optNormal.Checked = true;
                optInativo.Checked = false;
                optCancelado.Checked = false;
            }

            #endregion

            ViewState[PropostaEndCobranca] = contrato.EnderecoCobrancaID;
            ViewState[PropostaEndReferecia] = contrato.EnderecoReferenciaID;

            try
            {
                this.ExibeEnderecosDaProposta();
            }
            catch
            {
            }

            txtVidasCobertas.Text = contrato.Vidas.ToString();
            txtQtdVidasCob.Text = contrato.Vidas.ToString();
            txtObs.Text = contrato.Obs;
            lblCodigoCliente.Text = contrato.CodCobranca.ToString();

            if (!optCNPJ.Checked)
                txtVencimentoCob.Text = DateTime.Now.AddYears(1).ToString("dd/MM/yyyy");
            else
                txtVencimentoCob.Text = DateTime.Now.AddDays(3).ToString("dd/MM/yyyy");

            //String msg = "";
            //ContratoFacade.Instance.AtualizaValorDeCobrancas(contrato.ID, out msg);

            //if (!String.IsNullOrEmpty(msg))
            //{
            //    Alerta(null, this, "_msg", "ATENÇÃO!\\n" + msg);
            //}

            this.carregaHistoricoStatus();

            Entidades.Saldo saldo = ContratoFacade.Instance.CarregaSaldo(Convert.ToInt64(this.contratoId));

            if(saldo != null)
                litSaldo.Text = string.Concat("<strong>Saldo:</strong> ", saldo.Atual.ToString("C"));

            //DataTable dados = LocatorHelper.Instance.ExecuteQuery("select * from ir_dados_preprod_sp where UTILIZAR_REGISTRO = 1 and ENVIAR_DMED = 1 and idproposta=" + contratoId, "result", null).Tables[0];
            //if (dados.Rows.Count == 0)
            //{
            //    imgDemonstPagtoMail.OnClientClick = "return false;";
            //    imgDemonstPagtoQualiMail.OnClientClick = "return false;";
            //    dados.Dispose();
            //    return;
            //}
            //dados.Dispose();

            var config = RegraComissaoFacade.Instance.CarregarExcecaoPorContratoId(Convert.ToInt64(contrato.ID));
            if(config != null)
            {
                cboComissaoEstagio.SelectedValue = Convert.ToInt32(config.Tipo).ToString();
                if (config.Data.HasValue)
                    txtcomissaoInicioEm.Text = config.Data.Value.ToString("dd/MM/yyyy");
            }

            
            if (contrato.DescontoAcrescimoTipo > 0)
            {
                cboTipoDescontoAcrescimo.SelectedIndex = contrato.DescontoAcrescimoTipo;
                txtValorDecontoAcrescimo.Text = contrato.DescontoAcrescimoValor.ToString("N2");

                if(contrato.DescontoAcrescimoData > DateTime.MinValue)
                    txtDataDescontoAcrescimo.Text = contrato.DescontoAcrescimoData.ToString("dd/MM/yyyy");
            }
        }

        void SetaEstadoDosAdicionais()
        {
            if (!HaItemSelecionado(cboPlano)) { return; }
            foreach (ListItem item in cboBeneficiarioAdicional.Items)
            {
                IList<AdicionalBeneficiario> lista = AdicionalBeneficiario.
                    Carregar(cboContrato.SelectedValue, cboPlano.SelectedValue, ViewState[IDKey], item.Value);
                ViewState["adben_" + item.Value] = lista;
            }

            gridAdicional.DataSource = ViewState["adben_" + cboBeneficiarioAdicional.SelectedValue];
            gridAdicional.DataBind();
        }

        void PreencheCampoCorretor(Object corretorId)
        {
            Usuario corretor = Usuario.CarregarParcial(corretorId);

            if (corretor != null)
            {
                txtCorretor.Text = String.Concat(corretor.Nome, " (", corretor.Documento1, ")");
                txtCorretorID.Value = Convert.ToString(corretor.ID);
            }
        }

        void SetaEstadoCivil(Object estadoCivilId)
        {
        }

        void carregaHistoricoStatus()
        {
            gridHistoricoStatus.DataSource = null;
            if (this.contratoId == null) { gridHistoricoStatus.DataBind(); return; }

            gridHistoricoStatus.DataSource = ContratoStatusInstancia.Carregar_SemObs(this.contratoId);
            gridHistoricoStatus.DataBind();

            upFinalizacao.Update();
        }

        void LimpaCamposDeDependente()
        {
            this.DependenteID = null;
            txtRGDependente.Text = "";
            txtCPFDependente.Text = "";
            txtNomeDependente.Text = "";
            txtDataNascimentoDependente.Text = "";
            txtDepAltura.Text = "";
            txtDepPeso.Text = "";
            txtDepAdmissao.Text = "";

            cboCarenciaDependenteOperadora.Text = "";
            txtCarenciaDependenteCodigo.Text = "";
            txtCarenciaDependenteMatricula.Text = "";
            txtCarenciaDependenteTempoContrato.Text = "";
            txtDepTempoContratoDe.Text = "";
            txtDepTempoContratoAte.Text = "";
            txtDependentePortabilidade.Text = "";
        }

        #endregion

        protected void lnkOkContrato_Click(object sender, EventArgs e)
        {
            //if (txtNumeroContrato.Text.Trim() == "")
            //{
            //    Util.Geral.Alerta(this, "Informe o número da proposta.");
            //    txtNumeroContrato.Focus();
            //    return;
            //}

            //if (txtNumeroContrato.Text != txtNumeroContratoConfirme.Text)
            //{
            //    Util.Geral.Alerta(this, "Os números de proposta não conferem.");
            //    txtNumeroContrato.Focus();
            //    return;
            //}

            //string msg = "";
            //bool ret = ContratoFacade.Instance.ValidaCartao(this.contratoId, txtNumeroContrato.Text, out msg);

            //if (!ret)
            //{
            //    cmdSalvar.Enabled = false;
            //    Util.Geral.Alerta(this, msg);
            //}
            //else
            //{
            //    imgOk.Visible = true;//Util.Geral.Alerta(this, "Número válido.");
            //    cmdSalvar.Enabled = true;
            //    cboFilial.Focus();
            //    upSalvar.Update();
            //}
        }

        bool validaNumero()
        {
            if (txtNumeroContrato.Text.Trim() == "" && this.contratoId != null)
            {
                Util.Geral.Alerta(this, "Informe o número da proposta.");
                txtNumeroContrato.Focus();
                return false;
            }

            if (txtNumeroContrato.Text.Trim() != "" && txtNumeroContrato.Text != txtNumeroContratoConfirme.Text)
            {
                Util.Geral.Alerta(this, "Os números de proposta não conferem.");
                txtNumeroContrato.Focus();
                return false;
            }

            string msg = "";
            bool ret = true;

            if (txtNumeroContrato.Text.Trim() != "")
                ret = ContratoFacade.Instance.ValidaCartao(this.contratoId, txtNumeroContrato.Text, out msg);

            if (!ret)
            {
                //cmdSalvar.Enabled = false;
                Util.Geral.Alerta(this, msg);
                return false;
            }
            else
            {
                //imgOk.Visible = true;//Util.Geral.Alerta(this, "Número válido.");
                //cmdSalvar.Enabled = true;
                //cboFilial.Focus();
                //upSalvar.Update();
                return true;
            }
        }

        protected void cboTipoProposta_SelectedIndexChanged(object sender, EventArgs e)
        {
            TipoContrato tipo = new TipoContrato();
            tipo.ID = cboTipoProposta.SelectedValue;
            tipo.Carregar();
            pnlInfoAnterior.Visible = tipo.SolicitarInfoAnterior;
            this.ExibeSumario();
        }

        protected void cboEstipulante_SelectedIndexChanged(object sender, EventArgs e)
        {
            EstipulanteTaxa taxa = EstipulanteTaxa.CarregarVigente(cboEstipulante.SelectedValue);
            if (taxa == null)
            {
                chkCobrarTaxa.Checked = false;
                chkCobrarTaxa.Visible = false;
            }
            else
            {
                chkCobrarTaxa.Checked = true;
                chkCobrarTaxa.Visible = true;
            }

            this.CarregaContratoADM();
            this.CarregaPlanos();
            upFinalizacao.Update();
        }

        protected void cboOperadora_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CarregaContratoADM();
            this.CarregaPlanos();
            this.CarregaEstadoCivil();
            //this.CarregaFichaDeSaude(true);
        }

        protected void cboContrato_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CarregaPlanos();
            this.CarregaOpcoesParaAgregadosOuDependentes();
        }

        protected void cboPlano_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CarregaAdicionais(true);
            this.CarregaAcomodacoes();
        }

        protected void cmdAlterarPlano_Click(object sender, EventArgs e)
        {

        }

        protected void cboAcomodacao_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ExibeSumario();
            txtAdmissao.Focus();
        }

        protected void chkHistoricoPlano_CheckedChanged(object sender, EventArgs e)
        {
            trHistoricoPlano.Visible = chkHistoricoPlano.Checked;
        }

        protected void gridHistoricoPlano_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //UIHelper.AuthCtrl(e.Row.Cells[2], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
            grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente remover o item?\\nEssa ação não poderá ser desfeita.");
        }

        protected void gridHistoricoPlano_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                Object id = gridHistoricoPlano.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                ContratoPlano cp = new ContratoPlano();
                cp.ID = id;
                cp.Remover();
                gridHistoricoPlano.DataSource = ContratoPlano.Carregar(contratoId);
                gridHistoricoPlano.DataBind();
            }
        }

        protected void txtAdmissao_TextChanged(object sender, EventArgs e)
        {
            DateTime date = CStringToDateTime(txtAdmissao.Text);

            DateTime hoje = new DateTime(
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (date == DateTime.MinValue || date > hoje)
            {
                txtVigencia.Text = "";
                txtVencimento.Text = "";
                Alerta(null, this, "_err", "Data de admissão inválida.");
            }

            DateTime vigencia, vencimento;
            Int32 diaDataSemJuros; Object valorDataLimite;
            CalendarioVencimento rcv = null;

            if (!HaItemSelecionado(cboContrato)) { return; }
            CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(cboContrato.SelectedValue,
                date, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv);

            if (vigencia == DateTime.MinValue)
                txtVigencia.Text = "";
            else
                txtVigencia.Text = vigencia.ToString("dd/MM/yyyy"); ;

            if (vencimento == DateTime.MinValue)
                txtVencimento.Text = "";
            else
                txtVencimento.Text = vencimento.ToString("dd/MM/yyyy");

            if (vigencia == DateTime.MinValue || vencimento == DateTime.MinValue)
            {
                Alerta(null, this, "_err", "Data de admissão não coberta pelo calendário da operadora.");
                cmdSalvar.Enabled = true;
            }
            else
            {
                //TODO: sabe-se que tem vigencia e vencimento. mas tem que checar se tem RECEBIMENTO.
                cmdSalvar.Enabled = true;
            }

            upSalvar.Update();
        }

        protected void gridSelTitular_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("usar"))
            {
                Object id = gridSelTitular.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Beneficiario benef = new Beneficiario(id);
                benef.Carregar();
                this.ExibeBeneficiarioCarregado(benef, null);
                MontaCombosDeBeneficiarios();
                pnlSelTitular.Visible = false;
            }
        }

        protected void cmdCarregaBeneficiarioPorCPF_Click(object sender, ImageClickEventArgs e)
        {
            if (txtCPF.Text.Trim() == "") { cmdCarregarComboFichaSaudeBeneficiarios.Visible = true; return; }

            //Boolean cpfValido = UIHelper.ValidaCpf(txtCPF.Text);
            //if (!cpfValido)
            //{
            //    base.Alerta(MPE, ref litAlert, "Cpf inválido.", upnlAlerta);
            //    return;
            //}
            //IList<Beneficiario> lista = Beneficiario.CarregarPorParametro(txtCPF.Text, base.CStringToDateTime(txtDataNascimento.Text));denis
            IList<Beneficiario> lista = Beneficiario.CarregarPorParametro(txtCPF.Text, DateTime.MinValue);

            if (lista == null)
                this.ExibeBeneficiarioCarregado(null, null);
            else if (lista.Count > 1)
            {
                gridSelTitular.DataSource = lista;
                gridSelTitular.DataBind();
                pnlSelTitular.Visible = true;
                return;
            }
            else
                this.ExibeBeneficiarioCarregado(lista[0], null);

            MontaCombosDeBeneficiarios();

            if (lista == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, Page.GetType(), "novoTit", "beneficiarioNaoLocalizado('1');", true);
            }
        }

        protected void cmdCarregaBeneficiarioPorRG_Click(object sender, ImageClickEventArgs e)
        {
            if (txtRG.Text.Trim() == "") { return; }
            IList<Beneficiario> lista = Beneficiario.CarregarPorParametro("", "", txtRG.Text, SearchMatchType.QualquerParteDoCampo);

            if (lista == null)
                this.ExibeBeneficiarioCarregado(null, null);
            else
                this.ExibeBeneficiarioCarregado(lista[0], null);

            MontaCombosDeBeneficiarios();

            if (lista == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, Page.GetType(), "novoTit", "beneficiarioNaoLocalizado('1');", true);
            }
        }

        protected void gridEnderecosDisponiveis_Titular_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("usar"))
            {
                Object id = gridEnderecosDisponiveis_Titular.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Endereco endereco = new Endereco();
                endereco.ID = id;
                endereco.Carregar();
                this.ExibeEnderecoDeBeneficiarioCarregado(endereco);

            }
            else if (e.CommandName.Equals("excluir"))
            {
                Object id = gridEnderecosDisponiveis_Titular.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Endereco endereco = new Endereco();
                endereco.ID = id;
                endereco.Remover();
                this.ExibeEnderecosDeBeneficiarioCarregado(this.TitularID, null);
            }
            if (e.CommandName.Equals("cobranca"))
            {
                ViewState[PropostaEndCobranca] = gridEnderecosDisponiveis_Titular.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                this.ExibeEnderecosDaProposta();

            }
            else if (e.CommandName.Equals("referencia"))
            {
                ViewState[PropostaEndReferecia] = gridEnderecosDisponiveis_Titular.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                this.ExibeEnderecosDaProposta();
            }
        }

        protected void gridEnderecosDisponiveis_Titular_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((ImageButton)e.Row.Cells[1].Controls[0]).Attributes.Add("title", "visualizar");
            }
        }

        protected void gridEnderecosSelecionados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //UIHelper.AuthCtrl(e.Row.Cells[2], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
            grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente remover o endereço?");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex == 0)
                {
                    if (Convert.ToString(gridEnderecosSelecionados.DataKeys[e.Row.RowIndex].Value) ==
                        CToString(ViewState[PropostaEndReferecia]))
                    {
                        e.Row.Cells[1].Text = "REFERENCIA";
                    }
                    else if (Convert.ToString(gridEnderecosSelecionados.DataKeys[e.Row.RowIndex].Value) ==
                        CToString(ViewState[PropostaEndCobranca]))
                    {
                        e.Row.Cells[1].Text = "COBRANCA";
                    }
                }
                else
                {
                    if (Convert.ToString(gridEnderecosSelecionados.DataKeys[e.Row.RowIndex].Value) ==
                        CToString(ViewState[PropostaEndCobranca]))
                    {
                        e.Row.Cells[1].Text = "COBRANCA";
                    }
                    else if (Convert.ToString(gridEnderecosSelecionados.DataKeys[e.Row.RowIndex].Value) ==
                        CToString(ViewState[PropostaEndReferecia]))
                    {
                        e.Row.Cells[1].Text = "REFERENCIA";
                    }
                }
            }
        }

        protected void gridEnderecosSelecionados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                if (Convert.ToInt32(e.CommandArgument) == 0)
                {
                    if (
                        Convert.ToString(gridEnderecosSelecionados.DataKeys[Convert.ToInt32(e.CommandArgument)].Value) ==
                        CToString(ViewState[PropostaEndReferecia]))
                    {
                        ViewState.Remove(PropostaEndReferecia);
                        this.ExibeEnderecosDaProposta();
                    }
                    else if (
                        Convert.ToString(gridEnderecosSelecionados.DataKeys[Convert.ToInt32(e.CommandArgument)].Value) ==
                        CToString(ViewState[PropostaEndCobranca]))
                    {
                        ViewState.Remove(PropostaEndCobranca);
                        this.ExibeEnderecosDaProposta();
                    }
                }
                else
                {
                    if (
                        Convert.ToString(gridEnderecosSelecionados.DataKeys[Convert.ToInt32(e.CommandArgument)].Value) ==
                        CToString(ViewState[PropostaEndCobranca]))
                    {
                        ViewState.Remove(PropostaEndCobranca);
                        this.ExibeEnderecosDaProposta();
                    }
                    else if (
                        Convert.ToString(gridEnderecosSelecionados.DataKeys[Convert.ToInt32(e.CommandArgument)].Value) ==
                        CToString(ViewState[PropostaEndReferecia]))
                    {
                        ViewState.Remove(PropostaEndReferecia);
                        this.ExibeEnderecosDaProposta();
                    }
                }
            }
        }

        protected void cmdEnderecoAcoes_Click(object sender, EventArgs e)
        {
            switch (cmdEnderecoAcoes.Text)
            {
                case "Alterar":
                    {
                        Endereco endereco = new Endereco();
                        endereco.ID = gridEnderecosDisponiveis_Titular.DataKeys[gridEnderecosDisponiveis_Titular.SelectedIndex].Value;
                        endereco.Carregar();
                        endereco.Bairro = txtBairro.Text;
                        endereco.CEP = txtCEP.Text;
                        endereco.Cidade = txtCidade.Text;
                        endereco.Complemento = txtComplemento.Text;
                        endereco.Logradouro = txtLogradouro.Text;
                        endereco.Numero = txtNumero.Text;
                        endereco.Tipo = Convert.ToInt32(cboTipoEndereco.SelectedValue);
                        endereco.UF = txtUF.Text;
                        endereco.Salvar();

                        ExibeEnderecosDeBeneficiarioCarregado(endereco.DonoId, null);
                        gridEnderecosDisponiveis_Titular.SelectedIndex = -1;

                        this.LimpaCamposENDERECO_TITULAR();
                        cmdEnderecoAcoes.Text = "Gravar";
                        break;
                    }
                case "Gravar":
                    {
                        Endereco endereco = new Endereco();
                        endereco.DonoId = this.TitularID;
                        endereco.DonoTipo = Convert.ToInt32(Endereco.TipoDono.Beneficiario);
                        endereco.Bairro = txtBairro.Text;
                        endereco.CEP = txtCEP.Text;
                        endereco.Cidade = txtCidade.Text;
                        endereco.Complemento = txtComplemento.Text;
                        endereco.Logradouro = txtLogradouro.Text;
                        endereco.Numero = txtNumero.Text;
                        endereco.Tipo = Convert.ToInt32(cboTipoEndereco.SelectedValue);
                        endereco.UF = txtUF.Text;
                        endereco.Salvar();

                        ExibeEnderecosDeBeneficiarioCarregado(endereco.DonoId, endereco.ID);
                        cmdEnderecoAcoes.Text = "Alterar";
                        break;
                    }
            }
        }

        protected void cmdCarregaBeneficiarioDependentePorCPF_Click(object sender, ImageClickEventArgs e)
        {
            if (txtCPFDependente.Text.Trim().Replace("_", "").Replace(".", "").Replace("-", "").Length == 0) { return; }

            //Boolean cpfValido = UIHelper.ValidaCpf(txtCPFDependente.Text);
            //if (!cpfValido)
            //{
            //    if (Session[ConferenciaObjKey] == null) //só emite o alerta se não for um cadastro vindo da conferencia
            //        base.Alerta(MPE, ref litAlert, "Cpf inválido.", upnlAlerta);
            //    return;
            //}

            IList<Beneficiario> lista = Beneficiario.CarregarPorParametro(txtCPFDependente.Text, CStringToDateTime(txtDataNascimentoDependente.Text));

            if (lista == null)
                this.ExibeBeneficiarioDependenteCarregado(null);
            else if (lista.Count > 1)
            {
                gridSelDependente.DataSource = lista;
                gridSelDependente.DataBind();
                pnlSelDependente.Visible = true;
                return;
            }
            else
                this.ExibeBeneficiarioDependenteCarregado(lista[0]);

            this.MontaCombosDeBeneficiarios();

            if (lista == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, Page.GetType(), "novoDep", "beneficiarioNaoLocalizado('2');", true);
            }
        }

        protected void cmdCarregaBeneficiarioDependentePorRG_Click(object sender, ImageClickEventArgs e)
        {
            if (txtRGDependente.Text.Trim() == "") { return; }
            IList<Beneficiario> lista = Beneficiario.CarregarPorParametro(txtRGDependente.Text, "", "", SearchMatchType.QualquerParteDoCampo);

            if (lista == null)
                this.ExibeBeneficiarioDependenteCarregado(null);
            else if (lista.Count > 1)
            {
                gridSelDependente.DataSource = lista;
                gridSelDependente.DataBind();
                pnlSelDependente.Visible = true;
                return;
            }
            else
                this.ExibeBeneficiarioDependenteCarregado(lista[0]);

            MontaCombosDeBeneficiarios();

            if (lista == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, Page.GetType(), "novoDep", "beneficiarioNaoLocalizado('2');", true);
            }
        }

        protected void cmdAlterarBeneficiarioDependente_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void cmdAddDependente_Click(object sender, ImageClickEventArgs e)
        {
            #region validacao

            if (this.DependenteID == null || cboParentescoDependente.Items.Count == 0)
            {
                Alerta(null, this, "_err", "Atenção!\\nSelecione um parentesco.");
                return;
            }

            //if (!validaAltura(txtDepAltura))
            //{
            //    Alerta(null, this, "_altDep", "Atenção!\\nA altura do dependente deve estar entre 10cm e 2,5m. ");
            //    txtDepAltura.Focus();
            //    return;
            //}

            //if (!validaPeso(txtDepPeso))
            //{
            //    Alerta(null, this, "_err", "Atenção!\\nO peso do dependente deve estar entre 1kg e 300kg. ");
            //    txtDepPeso.Focus();
            //    return;
            //}

            //Decimal imcdep = getIMC(txtDepPeso.Text, txtDepAltura.Text);
            //if (imcdep > 30M)
            //{
            //    Alerta(null, this, "_err", "Atenção!\\nIMC fora da faixa. Encaminhar para área técnica.");
            //    return;
            //}

            //if (txtCarenciaDependenteCodigo.Text.Trim() == "" && this.contratoId == null)
            //{
            //    Alerta(null, this, "_err", "Atenção!\\nPRC do dependente é obrigatório.");
            //    txtCarenciaDependenteCodigo.Focus();
            //    return;
            //}

            #region checa regras para agregados
            //se o contrato ja está salvo, checa as regras de adicao de agregados/dependentes
            if (ViewState[IDKey] != null && HaItemSelecionado(cboContrato))
            {
                ContratoADMParentescoAgregado parentesco = new ContratoADMParentescoAgregado(cboParentescoDependente.SelectedValue);
                parentesco.Carregar();
                IList<AgregadoRegra> regras = AgregadoRegra.Carregar(cboContrato.SelectedValue, (Parentesco.eTipo)parentesco.ParentescoTipo);
                if (regras != null)
                {
                    foreach (AgregadoRegra regra in regras)
                    {
                        if (regra.TipoAgregado == parentesco.ParentescoTipo)
                        {
                            if (((AgregadoRegra.eTipoLimite)regra.TipoLimite) == AgregadoRegra.eTipoLimite.LimiteDeIdade)
                            {
                                Beneficiario benef = new Beneficiario(this.DependenteID);
                                benef.Carregar();
                                int idade = Beneficiario.CalculaIdade(benef.DataNascimento);
                                benef = null;

                                if (idade > regra.ValorLimite)
                                {
                                    Alerta(null, this, "_err", "Não é possível adicionar esse beneficiário. A seguinte regra foi infringida:\\n\\n" + regra.ToString());
                                    return;
                                }
                            }
                            else if (((AgregadoRegra.eTipoLimite)regra.TipoLimite) == AgregadoRegra.eTipoLimite.LimiteDiasDeContrato && contratoId != null)
                            {
                                Contrato contrato = new Contrato(contratoId);
                                contrato.Carregar();
                                String[] tempoContrato = DateDiff(1, contrato.Admissao).Replace("a", "").Replace("m", "").Replace("d", "").Split(' ');

                                int dias = Convert.ToInt32(tempoContrato[2]);

                                if (tempoContrato[0] != "0") //checa se passaram anos
                                {
                                    dias += (Convert.ToInt32(tempoContrato[0]) * 365);
                                }
                                if (tempoContrato[1] != "0") //checa se passaram meses
                                {
                                    dias += (Convert.ToInt32(tempoContrato[1]) * 30);
                                }

                                if (dias > regra.ValorLimite)
                                {
                                    Alerta(null, this, "_err", "Não é possível adicionar esse beneficiário. A seguinte regra foi infringida:\\n\\n" + regra.ToString());
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            DateTime data = new DateTime();
            if (!DateTime.TryParse(txtDepAdmissao.Text, out data))
            {
                Alerta(null, this, "_err", "Atenção!\\nData de admissão inválida.");
                txtDepAdmissao.Focus();
                return;
            }

            IList<ContratoBeneficiario> lista = this.Dependentes;
            if (lista == null) { lista = new List<ContratoBeneficiario>(); }
            else if (gridDependentes.SelectedIndex == -1)
            {
                foreach (ContratoBeneficiario adicionado in this.Dependentes)
                {
                    if (Convert.ToString(adicionado.BeneficiarioID) ==
                        Convert.ToString(this.DependenteID))
                    {
                        Alerta(null, this, "_err", "Atenção!\\nDependente já consta no contrato.");
                        return;
                    }
                }
            }
            #endregion

            ContratoBeneficiario cb = new ContratoBeneficiario();

            if (gridDependentes.SelectedIndex > -1)
            {
                cb.ID = gridDependentes.DataKeys[gridDependentes.SelectedIndex].Value;
                if (cb.ID != null) { cb.Carregar(); }
            }

            cb.BeneficiarioID = this.DependenteID;
            cb.BeneficiarioNome = txtNomeDependente.Text;
            cb.ParentescoID = cboParentescoDependente.SelectedValue;
            cb.ParentescoDescricao = cboParentescoDependente.SelectedItem.Text;
            cb.NumeroMatriculaDental = txtNumMatriculaDentalDep.Text;
            cb.NumeroMatriculaSaude = txtNumMatriculaSaudeDep.Text;
            cb.EstadoCivilID = cboEstadoCivilDependente.SelectedValue;
            cb.EstadoCivilDescricao = cboEstadoCivilDependente.SelectedItem.Text;
            cb.DataCasamento = CStringToDateTime(txtDepDataCasamento.Text);

            cb.Tipo = (int)ContratoBeneficiario.TipoRelacao.Dependente;
            cb.Data = data;
            cb.Altura = CToDecimal(txtDepAltura.Text);
            cb.Peso = CToDecimal(txtDepPeso.Text);
            cb.ContratoID = ViewState[IDKey];

            if (ViewState[IDKey] != null && cb.ID == null) //se o contrato nao é novo, mas o beneficiario é
            {
                DateTime vigencia = DateTime.MinValue, vencimento = DateTime.MinValue;
                Int32 diasDataSemJuros = 0; Object valorDataLimite = null;
                CalendarioVencimento rcv = null;

                CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(cboContrato.SelectedValue,
                    cb.Data, out vigencia, out vencimento, out diasDataSemJuros, out valorDataLimite, out rcv);
                cb.Vigencia = vigencia;
            }

            cb.Portabilidade = txtDependentePortabilidade.Text;
            cb.CarenciaOperadora = cboCarenciaDependenteOperadora.Text;
            cb.CarenciaOperadoraID = CToObject(txtCarenciaDependenteOperadoraID.Value);
            cb.CarenciaCodigo = txtCarenciaDependenteCodigo.Text;

            cb.CarenciaContratoDe = CStringToDateTime(txtDepTempoContratoDe.Text);
            cb.CarenciaContratoAte = CStringToDateTime(txtDepTempoContratoAte.Text);
            cb.CarenciaMatriculaNumero = txtCarenciaDependenteMatricula.Text;

            if (cb.CarenciaContratoDe != DateTime.MinValue && cb.CarenciaContratoAte != DateTime.MinValue)
            {
                txtCarenciaDependenteTempoContrato.Text = GetMonthsBetween(cb.CarenciaContratoDe, cb.CarenciaContratoAte).ToString();
            }

            cb.CarenciaContratoTempo = CToInt(txtCarenciaDependenteTempoContrato.Text);

            if (gridDependentes.SelectedIndex == -1)
                lista.Add(cb);
            else
                lista[gridDependentes.SelectedIndex] = cb;

            this.Dependentes = lista;

            gridDependentes.DataSource = this.Dependentes;
            gridDependentes.SelectedIndex = -1;
            gridDependentes.DataBind();

            spanDependentesCadastrados.Visible = true;
            this.DependenteID = null;
            this.LimpaCamposDeDependente();
            this.MontaCombosDeBeneficiarios();

            if (this.contratoId != null && cboBeneficiarioAdicional.Items.Count > 0 && gridAdicional.Rows.Count > 0)
            {
                //se precisar, o sistema marcará o novo beneficiário com adicional
                CheckBox chk = ((CheckBox)gridAdicional.Rows[0].Cells[1].Controls[1]);
                if (chk != null && chk.Checked) { checkboxGridAdicional_Changed(chk, null); }
            }
        }

        protected void checkboxGridAdicional_Changed2(Object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;

            GridViewRow grow = (GridViewRow)check.NamingContainer;
            int index = grow.RowIndex;

            Adicional ad = new Adicional();
            ad.ID = gridAdicional.DataKeys[index].Value;
            ad.Carregar();

            if (check.Checked && !ad.Ativo)
            {
                check.Checked = false;
                Alerta(null, this, "_err", "Adicional inativo no momento.");
                return;
            }

            IList<AdicionalBeneficiario> itens =
                (IList<AdicionalBeneficiario>)ViewState["adben_" + cboBeneficiarioAdicional.SelectedValue];

            if (itens == null) { itens = new List<AdicionalBeneficiario>(); }

            if (check.Checked) //adiciona na colecao
            {
                for (int i = 0; i < gridAdicional.Rows.Count; i++)
                {
                    Object atualAdicionalId = null;
                    Object atualBeneficiarioId = null;

                    if (gridAdicional.DataKeys[0].Values.Count > 0)
                    {
                        atualAdicionalId = gridAdicional.DataKeys[i][0];
                        atualBeneficiarioId = cboBeneficiarioAdicional.SelectedValue;
                    }

                    AdicionalBeneficiario item = PegaNaColecao(itens, atualAdicionalId, atualBeneficiarioId);

                    Boolean adiciona = false;
                    if (item == null)
                    {
                        item = new AdicionalBeneficiario();
                        adiciona = true;
                    }

                    item.AdicionalID = atualAdicionalId;
                    item.PropostaID = ViewState[IDKey];

                    if (i == index)
                        item.BeneficiarioID = cboBeneficiarioAdicional.SelectedValue;

                    if (adiciona)
                        itens.Add(item);
                }

                //checa se ha alguma regra

                if (ad.ParaTodaProposta)// (ar != null && ar.Tipo == Convert.ToInt32(AdicionalRegra.eTipo.TitularETodosDependentes))
                {
                    foreach (ListItem _item in cboBeneficiarioAdicional.Items)
                    {
                        IList<AdicionalBeneficiario> _itens =
                            (IList<AdicionalBeneficiario>)ViewState["adben_" + _item.Value];
                        if (_itens == null)
                        {
                            _itens = AdicionalBeneficiario.Carregar(cboContrato.SelectedValue, cboPlano.SelectedValue, ViewState[IDKey], _item.Value);
                        }

                        foreach (AdicionalBeneficiario aben in _itens)
                        {
                            if (Convert.ToString(ad.ID) == Convert.ToString(aben.AdicionalID))
                            {
                                aben.AdicionalID = ad.ID;
                                aben.BeneficiarioID = _item.Value;
                                aben.PropostaID = ViewState[IDKey];
                                break;
                            }
                        }

                        ViewState["adben_" + _item.Value] = _itens;
                    }
                }
            }
            else //remove da colecao
            {
                //checa se ha alguma regra
                if (ad.ParaTodaProposta) // (ar != null && ar.Tipo == Convert.ToInt32(AdicionalRegra.eTipo.TitularETodosDependentes))
                {
                    foreach (ListItem _item in cboBeneficiarioAdicional.Items)
                    {
                        IList<AdicionalBeneficiario> _itens =
                            (IList<AdicionalBeneficiario>)ViewState["adben_" + _item.Value];

                        foreach (AdicionalBeneficiario aben in _itens)
                        {
                            if (Convert.ToString(ad.ID) == Convert.ToString(aben.AdicionalID))
                            {
                                aben.BeneficiarioID = null;

                                if (aben.ID != null) { aben.Remover(); }
                                break;
                            }
                        }

                        ViewState["adben_" + _item.Value] = _itens;
                    }
                }
                else
                {
                    if (itens != null)
                    {
                        itens[index].BeneficiarioID = null;

                        if (itens[index].ID != null) { itens[index].Remover(); }
                    }
                }
            }

            ViewState["adben_" + cboBeneficiarioAdicional.SelectedValue] = itens;
            this.ExibeSumario();
        }

        protected void gridDependentes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editarDadosDaProposta"))
            {
                Object id = gridDependentes.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                IList<ContratoBeneficiario> lista = this.Dependentes;
                ContratoBeneficiario cb = lista[Convert.ToInt32(e.CommandArgument)];

                if (cb.ID != null) { cb.Carregar(); }

                this.DependenteID = cb.BeneficiarioID;
                txtNomeDependente.Text = cb.BeneficiarioNome;

                if (cb.ParentescoID != null)
                {
                    if (cboParentescoDependente.Items.FindByValue(Convert.ToString(cb.ParentescoID)) != null)
                        cboParentescoDependente.SelectedValue = Convert.ToString(cb.ParentescoID);
                    else if (cboParentescoDependente.Items.FindByText(Convert.ToString(cb.ParentescoDescricao)) != null)
                        cboParentescoDependente.SelectedValue = cboParentescoDependente.Items.FindByText(Convert.ToString(cb.ParentescoDescricao)).Value;
                }

                txtNumMatriculaDentalDep.Text = cb.NumeroMatriculaDental;
                txtNumMatriculaSaudeDep.Text = cb.NumeroMatriculaSaude;

                if (cb.EstadoCivilID != null)
                    cboEstadoCivilDependente.SelectedValue = Convert.ToString(cb.EstadoCivilID);

                if (cb.DataCasamento != DateTime.MinValue)
                    txtDepDataCasamento.Text = cb.DataCasamento.ToString("dd/MM/yyyy");

                if (cb.Data != DateTime.MinValue)
                    txtDepAdmissao.Text = cb.Data.ToString("dd/MM/yyyy");

                txtDepAltura.Text = cb.Altura.ToString("N2");
                txtDepPeso.Text = cb.Peso.ToString("N2");

                txtDependentePortabilidade.Text = cb.Portabilidade;
                cboCarenciaDependenteOperadora.Text = cb.CarenciaOperadora;
                txtCarenciaDependenteOperadoraID.Value = CToString(cb.CarenciaOperadoraID);
                txtCarenciaDependenteCodigo.Text = cb.CarenciaCodigo;

                if (cb.CarenciaContratoDe != DateTime.MinValue)
                    txtDepTempoContratoDe.Text = cb.CarenciaContratoDe.ToString("dd/MM/yyyy");
                else
                    txtDepTempoContratoDe.Text = "";

                if (cb.CarenciaContratoAte != DateTime.MinValue)
                    txtDepTempoContratoAte.Text = cb.CarenciaContratoAte.ToString("dd/MM/yyyy");
                else
                    txtDepTempoContratoAte.Text = "";

                txtCarenciaDependenteTempoContrato.Text = cb.CarenciaContratoTempo.ToString();
                txtCarenciaDependenteMatricula.Text = cb.CarenciaMatriculaNumero;

                Beneficiario benef = new Beneficiario(cb.BeneficiarioID);
                benef.Carregar();

                txtCPFDependente.Text = benef.CPF;

                if (benef.DataNascimento != DateTime.MinValue)
                    txtDataNascimentoDependente.Text = benef.DataNascimento.ToString("dd/MM/yyyy");

                cboSexoDependente.SelectedValue = benef.Sexo;

                gridDependentes.SelectedIndex = Convert.ToInt32(e.CommandArgument);
            }
            if (e.CommandName.Equals("editar"))
            {
            }
            else if (e.CommandName.Equals("excluir"))
            {
                Object id = gridDependentes.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
                IList<ContratoBeneficiario> lista = this.Dependentes;
                lista.RemoveAt(Convert.ToInt32(e.CommandArgument));
                this.Dependentes = lista;
                gridDependentes.DataSource = lista;
                gridDependentes.DataBind();

                if (id != null)
                {
                    //TODO: se ja incluido, inativa. se nao incluido, pode deletar
                    ContratoBeneficiario item = new ContratoBeneficiario();
                    item.ID = id;
                    item.Carregar();
                    item.Ativo = false;
                    item.DataInativacao = DateTime.Now;
                    item.Salvar();
                    this.CarregaDependentes();
                }

                MontaCombosDeBeneficiarios();
            }
        }

        protected void gridDependentes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //UIHelper.AuthCtrl(e.Row.Cells[6], Perfil.Cadastro_Conferencia_Operador_PropostaclientePerfilArray);
            grid_RowDataBound_Confirmacao(sender, e, 6, "Deseja realmente remover o dependente deste contrato?");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[4].Text = TraduzTipoRelacaoDependenteContrato(Convert.ToInt32(e.Row.Cells[4].Text));

                Object id = gridDependentes.DataKeys[e.Row.RowIndex][1];
                e.Row.Cells[7].Attributes.Add("onClick", "win = window.open('clienteP.aspx?et=2&" + IDKey + "=" + id + "', 'janela', 'toolbar=no,scrollbars=1,width=860,height=420'); win.moveTo(100,150); return false;");
            }
        }

        protected void cboBeneficiarioFicha_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void cmdCarregarComboFichaSaudeBeneficiarios_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void dlFicha_ItemCommand(object source, DataListCommandEventArgs e)
        {

        }

        protected void dlFicha_ItemDataBound(object sender, DataListItemEventArgs e)
        {

        }

        protected void cboBeneficiarioAdicional_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CarregaAdicionais(false);
        }

        protected void gridAdicional_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //UIHelper.AuthWebCtrl((CheckBox)e.Row.Cells[1].Controls[1], Perfil.Cadastro_Conferencia_Operador_PropostaBeneficiarioPerfilArray);
            }
        }

        protected void gridAdicional_RowCommand(object sender, GridViewCommandEventArgs e)
        {

        }

        protected void txtDesconto_TextChanged(object sender, EventArgs e)
        {
            Decimal result = 0;
            if (!Decimal.TryParse(txtDesconto.Text, out result))
                txtDesconto.Text = "0";
            this.ExibeSumario();
        }

        protected void chkCobrarTaxa_CheckedChanged(object sender, EventArgs e)
        {
            this.ExibeSumario();
        }

        protected void cmdLiberar_Click(object sender, EventArgs e)
        {

        }

        protected void lnkAlterarStatus_Click(object sender, EventArgs e)
        {
            mpeAlteraStatus.Show();
        }

        protected void optStatusEdit_Changed(Object sender, EventArgs e)
        {
            carregaMotivoStatus();
            mpeAlteraStatus.Show();
        }

        protected void cmdSalvarHistoricoStatus_Click(Object sender, EventArgs e)
        {
            if (this.contratoId == null) { return; }

            if (cboStatusMotivo.Items.Count == 0)
            {
                Alerta(null, this, "_msg", "Selecione uma ação.");
                mpeAlteraStatus.Show();
                return;
            }

            ContratoStatusInstancia status = ContratoStatusInstancia.CarregarUltima(this.contratoId);
            if (status != null)
            {
                if (cboStatusMotivo.SelectedValue == Convert.ToString(status.StatusID))
                {
                    Alerta(null, this, "_msg", "A proposta ja está com este status.");
                    mpeAlteraStatus.Show();
                    return;
                }
            }

            ContratoStatusInstancia novoStatus = new ContratoStatusInstancia();
            novoStatus.ContratoID = this.contratoId;
            novoStatus.DataSistema = DateTime.Now;
            novoStatus.StatusID = cboStatusMotivo.SelectedValue;
            novoStatus.UsuarioID = Usuario.Autenticado.ID;

            novoStatus.Data = Util.CTipos.CStringToDateTime(txtDataInativacao.Text);
            if (novoStatus.Data == DateTime.MinValue)
            {
                Alerta(null, this, "_msg", "Data de reativação, inativação ou cancelamento inválida.");
                mpeAlteraStatus.Show();
                return;
            }

            novoStatus.Salvar();

            this.carregaHistoricoStatus();

            Contrato contrato = new Contrato(this.contratoId);
            contrato.Carregar();

            optNormal.Checked = optNormalEdit.Checked;
            optInativo.Checked = optInativoEdit.Checked;
            optCancelado.Checked = optCanceladoEdit.Checked;

            if (optInativoEdit.Checked)
            {
                contrato.Inativo = true;
                contrato.Cancelado = false;
            }
            else if (optCanceladoEdit.Checked)
            {
                contrato.Inativo = false;
                contrato.Cancelado = true;
            }
            else
            {
                contrato.Inativo = false;
                contrato.Cancelado = false;
            }

            if (optInativoEdit.Checked || optCanceladoEdit.Checked)
            {
                contrato.DataCancelamento = novoStatus.Data; //base.CStringToDateTime(txtDataInativacao.Text);
            }
            else
            {
                contrato.DataCancelamento = DateTime.MinValue;
            }

            contrato.Obs += Environment.NewLine + Environment.NewLine + txtObsEdit.Text;
            txtObsEdit.Text = "";
            txtObs.Text = contrato.Obs;

            contrato.Salvar();
            mpeAlteraStatus.Hide();
        }

        protected void gridAtendimento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        }

        protected void gridAtendimento_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "detalhe")
            {
                this.LimparCamposAtendimento();
                Object id = gridAtendimento.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                gridAtendimento.SelectedIndex = Convert.ToInt32(e.CommandArgument);
                AtendimentoTemp atendimento = new AtendimentoTemp(id);
                atendimento.Carregar();

                this.exibeAtendimento(atendimento);
            }
        }

        protected void gridAtendimento_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridAtendimento.PageIndex = e.NewPageIndex;
            this.CarregarAtendimentos();
        }

        protected void cboTipoAtendimento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTipoAtendimento.SelectedIndex == 0) { return; }

            AtendimentoTipo obj = new AtendimentoTipo(cboTipoAtendimento.SelectedValue);
            obj.Carregar();

            DateTime inicio = CStringToDateTime(txtDataInicio.Text);
            if (inicio != DateTime.MinValue && litResolvidoPor.Text.Trim() == "")
            {
                DateTime previsao = inicio.AddDays(obj.PrazoConclusao);
                while (previsao.DayOfWeek == DayOfWeek.Saturday || previsao.DayOfWeek == DayOfWeek.Sunday)
                    previsao = previsao.AddDays(1);

                txtDataPrevisao.Text = previsao.ToString("dd/MM/yyyy");
            }
        }

        protected void cmdFecharAtendimento_Click(object sender, EventArgs e)
        {
            this.LimparCamposAtendimento();
            cboTipoAtendimento_Change(null, null);
            txtTexto.ReadOnly = false;
            txtTexto2.Visible = false;
            txtTexto.Focus();
            cmdSalvarAtendimento.Enabled = true;
        }

        protected void cboTipoAtendimento_Change(Object sender, EventArgs e)
        {
            if (cboTipoAtendimento.SelectedIndex == 0) { return; }

            AtendimentoTipo obj = new AtendimentoTipo(cboTipoAtendimento.SelectedValue);
            obj.Carregar();

            DateTime inicio = CStringToDateTime(txtDataInicio.Text);
            if (inicio != DateTime.MinValue && litResolvidoPor.Text.Trim() == "")
            {
                DateTime previsao = inicio.AddDays(obj.PrazoConclusao);
                while (previsao.DayOfWeek == DayOfWeek.Saturday || previsao.DayOfWeek == DayOfWeek.Sunday)
                    previsao = previsao.AddDays(1);

                txtDataPrevisao.Text = previsao.ToString("dd/MM/yyyy");
            }
        }

        protected void cmdSalvarAtendimento_Click(object sender, EventArgs e)
        {
            #region validacoes

            DateTime dataParam = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (txtTexto.Text.Trim() == "")
            {
                Alerta(null, this, "_err1", "Nenhum atendimento informado.");
                txtTexto.Focus();
                return;
            }
            if (txtTexto2.Visible && txtTexto2.Text.Trim() == "")
            {
                Alerta(null, this, "_err1a", "Nenhum atendimento informado.");
                txtTexto2.Focus();
                return;
            }

            //if (txtTitulo.Text.Trim() == "")
            //{
            //    if (txtTexto.Text.Length > 30)
            //        txtTitulo.Text = txtTexto.Text.Substring(0, 29) + " (...)";
            //    else
            //        txtTitulo.Text = txtTexto.Text;
            //}

            if (cboTipoAtendimento.SelectedIndex == 0)
            {
                Alerta(null, this, "_err1", "Você deve informar o tipo de atendimento.");
                cboTipoAtendimento.Focus();
                return;
            }
            if (cboSubTipoAtendimento.SelectedIndex == 0)
            {
                Alerta(null, this, "_err1", "Você deve informar o subtipo de atendimento.");
                cboSubTipoAtendimento.Focus();
                return;
            }

            if (CStringToDateTime(txtDataInicio.Text) == DateTime.MinValue)
            {
                txtDataInicio.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }

            if (CStringToDateTime(txtDataPrevisao.Text) == DateTime.MinValue)
            {
                txtDataPrevisao.Text = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
            }

            if (gridAtendimento.SelectedIndex == -1 && cboTipoAtendimento.SelectedIndex >= 1)
            {
                ////não gerar mais do que uma ocorrência pelo mesmo motivo quando a 
                ////primeira ainda estiver em aberto
                //IList<AtendimentoTemp> atendimentos = AtendimentoTemp.
                //    CarregarPorProposta(this.contratoId, cboTipoAtendimento.SelectedValue);

                //if (atendimentos != null)
                //{
                //    foreach (AtendimentoTemp _atend in atendimentos)
                //    {
                //        if (String.IsNullOrEmpty(_atend.ResolvidoPor))
                //        {
                //            Alerta(null, this, "_err1", "Já há um atendimento em aberto para esse tipo.");
                //            cboTipoAtendimento.Focus();
                //            return;
                //        }
                //    }
                //}
            }
            #endregion

            Ent.NumeroCartao novo = null;
            if (cboTipoAtendimento.SelectedValue == "2" && gridAtendimento.SelectedIndex == -1) 
            {
                //se 2a via de cartao e NAO está editando
                novo = ContratoFacade.Instance.GerarNovaViaCartao(this.contratoId);
            }

            AtendimentoTemp atendimento = new AtendimentoTemp();
            if (gridAtendimento.SelectedIndex != -1)
            {
                atendimento.ID = gridAtendimento.DataKeys[gridAtendimento.SelectedIndex][0];
                atendimento.Carregar();
            }
            else
            {
                atendimento.IniciadoPor = Util.UsuarioLogado.Nome;
            }

            if (chkAtendimentoConcluido.Checked || novo != null) //se marcado como concluido, ou 2a via de boleto
                atendimento.DataFim = DateTime.Now; //base.CStringToDateTime(txtDataConclusao.Text);
            else
                atendimento.DataFim = DateTime.MinValue;

            if (atendimento.DataFim != DateTime.MinValue && String.IsNullOrEmpty(atendimento.ResolvidoPor))
            {
                atendimento.ResolvidoPor = Util.UsuarioLogado.Nome;
            }

            atendimento.DataInicio = CStringToDateTime(txtDataInicio.Text);
            if (atendimento.DataInicio < dataParam && gridAtendimento.SelectedIndex == -1)
            {
                Alerta(null, this, "_err1", "Data não pode ser inferior à data atual.");
                txtDataInicio.Focus();
                return;
            }

            atendimento.DataPrevisao = CStringToDateTime(txtDataPrevisao.Text);
            if (atendimento.DataPrevisao < dataParam && gridAtendimento.SelectedIndex == -1)
            {
                Alerta(null, this, "_err1", "Previsão de conclusão não pode ser inferior à data atual.");
                txtDataPrevisao.Focus();
                return;
            }

            atendimento.PropostaID = this.contratoId;
            atendimento.Texto = txtTexto.Text;
            if (txtTexto2.Visible)
            {
                atendimento.Texto += String.Concat("\n-------\n", txtTexto2.Text, "\n[",
                    Util.UsuarioLogado.Nome, " - ", DateTime.Now.ToString("dd/MM/yyyy HH:mm"), " - ", cboSubTipoAtendimento.SelectedItem.Text + "]");
            }
            else
            {
                atendimento.Texto += String.Concat("\n[",
                    Util.UsuarioLogado.Nome, " - ", DateTime.Now.ToString("dd/MM/yyyy HH:mm"), " - ", cboSubTipoAtendimento.SelectedItem.Text + "]");
            }

            //atendimento.Titulo = txtTitulo.Text;

            atendimento.SubTipoID = cboSubTipoAtendimento.SelectedValue;

            #region envia email de atendimento 

            if (cboTipoAtendimento.SelectedIndex > 0)
            {
                atendimento.TipoID = cboTipoAtendimento.SelectedValue;

                AtendimentoTipo tipo = new AtendimentoTipo(cboTipoAtendimento.SelectedValue);
                tipo.Carregar();
                if (!String.IsNullOrEmpty(tipo.Email))
                {
                    //envia email
                    MailMessage msg = new MailMessage(
                        new MailAddress(ConfigurationManager.AppSettings["mailFrom"], ConfigurationManager.AppSettings["mailFromName"]),
                        new MailAddress(tipo.Email));
                    msg.Subject = "Atendimento";
                    msg.IsBodyHtml = false;

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("Contrato: "); sb.AppendLine(txtNumeroContrato.Text);
                    sb.Append("Tipo de contrato: "); sb.AppendLine(cboTipoProposta.SelectedItem.Text);
                    sb.Append("Estipulante: "); sb.AppendLine(cboEstipulante.SelectedItem.Text);
                    sb.Append("Operadora: "); sb.AppendLine(cboOperadora.SelectedItem.Text);
                    sb.Append("Contrato ADM: "); sb.AppendLine(cboContrato.SelectedItem.Text);
                    sb.Append("Plano: "); sb.AppendLine(cboPlano.SelectedItem.Text);
                    sb.Append("Acomodação: "); sb.AppendLine(cboAcomodacao.SelectedItem.Text);
                    sb.Append("Admissão: "); sb.AppendLine(txtAdmissao.Text);
                    sb.AppendLine(""); sb.AppendLine(""); sb.AppendLine("");
                    sb.Append("Detalhes do atendimento");
                    sb.AppendLine(""); sb.AppendLine("");
                    sb.AppendLine("Tipo"); sb.AppendLine(tipo.Descricao);
                    sb.AppendLine(""); sb.AppendLine("");
                    sb.AppendLine("Título"); sb.AppendLine(atendimento.Titulo);
                    sb.AppendLine(""); sb.AppendLine("");
                    sb.AppendLine("Texto"); sb.AppendLine(atendimento.Texto);
                    sb.AppendLine(""); sb.AppendLine("");
                    sb.AppendLine("Atendente"); sb.AppendLine(atendimento.IniciadoPor);
                    sb.AppendLine(""); sb.AppendLine("");
                    sb.AppendLine("Data"); sb.AppendLine(atendimento.DataInicio.ToString("dd/MM/yyyy"));
                    sb.AppendLine(""); sb.AppendLine("");
                    sb.AppendLine("Previsão"); sb.AppendLine(atendimento.DataPrevisao.ToString("dd/MM/yyyy"));
                    sb.AppendLine(""); sb.AppendLine("");

                    if (atendimento.DataFim != DateTime.MinValue)
                    {
                        sb.AppendLine("Conclusão"); sb.AppendLine(atendimento.DataFim.ToString("dd/MM/yyyy"));
                        sb.AppendLine(""); sb.AppendLine("");
                        sb.AppendLine("Atendente (conclusão)"); sb.AppendLine(atendimento.ResolvidoPor);
                    }

                    msg.Body = sb.ToString();

                    try
                    {
                        SmtpClient client = new SmtpClient();
                        client.Send(msg);
                        msg.Dispose();
                        client = null;
                    }
                    catch { }
                }
            }
            #endregion

            atendimento.Salvar();

            AtendimentoTempItem item = new AtendimentoTempItem();
            item.AtendimentoID = atendimento.ID;
            item.Data = DateTime.Now;
            item.SubTipoID = atendimento.SubTipoID;

            if (!txtTexto2.ReadOnly && txtTexto2.Visible)
                item.Texto = txtTexto2.Text;
            else
                item.Texto = txtTexto.Text;

            if (novo != null)
            {
                txtNumeroContrato.Text = novo.NumeroCompletoSemCV;
                txtNumeroContratoConfirme.Text = novo.NumeroCompletoSemCV;
                item.Texto += "\n\nNovo número de cartão: " + novo.NumeroCompletoSemCV;
            }

            item.TipoID = atendimento.TipoID;
            item.UsuarioID = Usuario.Autenticado.ID;
            item.Salvar();

            txtTexto2.Visible = false;
            txtTexto.ReadOnly = false;
            cmdSalvarAtendimento.Enabled = true;

            Alerta(null, this, "_atendOk", "Atendimento gravado com sucesso!");
            this.LimparCamposAtendimento();
            this.CarregarAtendimentos();
            txtTexto.Focus();
            upAtendimento.Update();
            upDadosComuns.Update();
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
                Panel panel = (Panel)e.Row.Cells[1].FindControl("pnlComposite");
                Literal lit = (Literal)panel.FindControl("litComposite");

                lit.Text = ((Cobranca)e.Row.DataItem).ComposicaoResumo;
                ///////////////////////////////////////////

                if (Server.HtmlDecode(e.Row.Cells[5].Text) == "Não")
                {
                    DateTime vencto = CStringToDateTime(((TextBox)e.Row.Cells[3].Controls[1]).Text);
                    if (vencto < DateTime.Now)
                    {
                        e.Row.Cells[5].ForeColor = System.Drawing.Color.Orange;
                    }

                    if (!((TextBox)e.Row.Cells[3].Controls[1]).Enabled)
                    {
                        DateTime vigencia, vencimento, admissao = CStringToDateTime(txtAdmissao.Text);
                        Int32 diaDataSemJuros = 0, limiteAposVencto = 0; Object valorDataLimite;
                        CalendarioVencimento rcv = null;

                        if (!HaItemSelecionado(cboContrato))
                        {
                            CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(cboContrato.SelectedValue,
                               admissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, out limiteAposVencto, null);
                        }

                        if (limiteAposVencto > 0)
                        {
                            DateTime venctoLimite = DateTime.Now.AddDays(limiteAposVencto);
                            venctoLimite = new DateTime(venctoLimite.Year, venctoLimite.Month, venctoLimite.Day, 23, 59, 59, 999);

                            if (venctoLimite < DateTime.Now)
                            {
                                ((ImageButton)e.Row.Cells[7].Controls[0]).Visible = false;
                                ((ImageButton)e.Row.Cells[9].Controls[0]).Visible = false;
                            }
                        }
                    }
                }
                else
                {
                    Decimal cobrado = 0, pago = 0;
                    pago = Convert.ToDecimal(Server.HtmlDecode(e.Row.Cells[2].Text.Replace("R$ ", "")));
                    cobrado = Convert.ToDecimal(Server.HtmlDecode(((LinkButton)e.Row.Cells[1].Controls[1]).Text.Replace("R$ ", "")));

                    if (pago >= cobrado)
                    {
                        e.Row.Cells[7].Controls[0].Visible = false;
                    }
                    else
                    {
                        ((LinkButton)e.Row.Cells[7].Controls[0]).Attributes.Add("onClick", "if(confirm('Esta ação criará uma cobrança complementar no valor de " + (cobrado - pago).ToString("C") + ".\\nConfirma a operação?')) { return true; } else { return false; }");
                    }
                }

                //((LinkButton)e.Row.Cells[6].Controls[0]).ToolTip = "enviar e-mail";
                ((ImageButton)e.Row.Cells[8].Controls[0]).ToolTip = "recalcular";

                // Regra de mensagem de boleto
                if (optCNPJ.Checked)
                {
                    Cobranca cob = e.Row.DataItem as Cobranca;
                    if (cob.DataVencimento < DateTime.Now) //vencido
                    {
                        bool vencidoHa5DiasOuMais = Cobranca.VencidoHa5DiasUteis(cob.DataVencimento);
                        if (!vencidoHa5DiasOuMais)
                        {
                            ((LinkButton)e.Row.Cells[7].Controls[0]).Attributes.Add("onClick", "if(confirm('Cobrança vencida.\\nEsta ação criará uma nova cobrança.Confirma a operação?')) { return true; } else { return false; }");
                            ((LinkButton)e.Row.Cells[9].Controls[0]).Attributes.Add("onClick", "if(confirm('Cobrança vencida.\\nEsta ação criará uma nova cobrança.Confirma a operação?')) { return true; } else { return false; }");
                        }
                        else
                        {
                            ((LinkButton)e.Row.Cells[7].Controls[0]).Attributes.Add("onClick", "alert('Cobrança vencida há 5 dias ou mais.'); return false;");
                            ((LinkButton)e.Row.Cells[9].Controls[0]).Attributes.Add("onClick", "alert('Cobrança vencida há 5 dias ou mais.'); return false;");
                        }
                    }
                }

                Cobranca _cobranca = (Cobranca)e.Row.DataItem;
                if(_cobranca.Cancelada) //if(Convert.ToBoolean(gridCobranca.DataKeys[e.Row.RowIndex][3]) == false)
                {
                    e.Row.ToolTip = "Cancelado";
                    e.Row.ForeColor = System.Drawing.Color.Red;
                    ((LinkButton)e.Row.Cells[1].FindControl("lnkValor")).ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[5].ForeColor = System.Drawing.Color.Red;
                    //Cobranca cobranca = (Cobranca)e.Row.DataItem;
                }
            }
        }

        protected void gridCobranca_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "recalcular")
            {
                //Alerta(null, this, "_err", "Função indisponível para demonstração.");
                return;///////////////////////////////

                Object id = gridCobranca.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Cobranca cobranca = new Cobranca(id);
                cobranca.Carregar();
                List<CobrancaComposite> composite = null;

                if (cobranca.Tipo == (int)Cobranca.eTipo.Normal)
                {
                    cobranca.Valor = Contrato.CalculaValorDaProposta(this.contratoId, cobranca.DataVencimento, null, true, true, ref composite);
                    cobranca.Salvar();
                    cobranca = null;
                    this.CarregarCobrancas();
                }
            }
            if (e.CommandName == "email" || e.CommandName == "print")
            {
                #region

                Object id = gridCobranca.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Cobranca cobranca = new Cobranca(id);
                cobranca.Carregar();

                if (cobranca.Pago) return;

                if (Cobranca.VencidoHa5DiasUteis(cobranca.DataVencimento) && optCNPJ.Checked) return;

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

                ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(this.contratoId, null);
                Beneficiario beneficiario = new Beneficiario(titular.BeneficiarioID);
                beneficiario.Carregar();
                if (!String.IsNullOrEmpty(beneficiario.Email) && txtEmailAtendimento.Text.Trim() == "") { txtEmailAtendimento.Text = beneficiario.Email; }

                if (txtEmailAtendimento.Text.Trim() == "" && txtEmailAtendimentoCC.Text.Trim() == "" && e.CommandName == "email")
                {
                    Alerta(null, this, "_errNCobMail", "Nenhum endereço de e-mail informado.");
                    return;
                }

                String nome = beneficiario.Nome;

                if (beneficiario.Email != txtEmailAtendimento.Text && txtEmailAtendimento.Text.IndexOf("linkecerebro") == -1 && txtEmailAtendimento.Text.IndexOf("pspadrao") == -1 && txtEmailAtendimento.Text.IndexOf("padraoseguros") == -1 && e.CommandName == "email")
                {
                    beneficiario.Email = txtEmailAtendimento.Text;
                    beneficiario.Salvar();
                }

                if (txtEmailAtendimento.Text.Trim() != "")
                    email = txtEmailAtendimento.Text.Trim();

                if (txtEmailAtendimentoCC.Text.Trim() != "")
                {
                    if (email.Length > 0) { email += ";"; }
                    email += txtEmailAtendimentoCC.Text.Trim();
                }

                String nossoNumero = "";
                Int32 dia = vencto.Day;  //DateTime.Now.AddDays(1).Day;
                Int32 mes = vencto.Month;//DateTime.Now.AddDays(1).Month;
                Int32 ano = vencto.Year; //DateTime.Now.AddDays(1).Year;
                Decimal Valor = 0;

                if (optCNPJ.Checked)
                {
                     dia = DateTime.Now.AddDays(1).Day;
                     mes = DateTime.Now.AddDays(1).Month;
                     ano = DateTime.Now.AddDays(1).Year;
                }

                

                Contrato contrato = new Contrato(this.contratoId);
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
                    if (optCNPJ.Checked && vencto < DateTime.Now) //se é PJ e cobranca vencida
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

                        //Valor = cobranca.Valor;
                        //int diasPassadosDoVencimento = Cobranca.DiferenciaEmDiasUteis(dataBase);

                        //Decimal atraso    = Convert.ToDecimal(10) / Convert.ToDecimal(100); //Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["jurosAtraso"]);
                        //Decimal atrasoDia = 0.0333M; //Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["jurosDia"]);

                        //Valor += (Valor * atraso);
                        //Valor += cobranca.Valor * (atrasoDia * (Convert.ToDecimal(diasPassadosDoVencimento)));

                        //cobranca.Valor = Valor;
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

                //if ((contrato.ContratoADMID != null && Convert.ToInt32(contrato.ContratoADMID) >= Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"])) ||
                //    Convert.ToInt32(cboContrato.SelectedValue) >= Convert.ToInt32(ConfigurationManager.AppSettings["contratoAdmQualicorpIdIncial"]))
                //{
                //    uri = EntityBase.RetiraAcentos(String.Concat("?nossonum=", nossoNumero, "&valor=", Valor.ToString("N2"), "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dia, "&v_mes=", mes, "&v_ano=", ano, "&numdoc2=", contrato.Numero.PadLeft(5, '0'), "&nome=", nome, "&cod_cli=", id, "&end1=", end1, "&end2=", end2, "&mailto=", email, "&instr=", instrucoes, ".<br><br>" + naoReceber));
                //    //uri = EntityBase.RetiraAcentos(String.Concat("?nossonum=", nossoNumero, "&valor=", Valor.ToString("N2"), "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dia, "&v_mes=", mes, "&v_ano=", ano, "&numdoc2=", contrato.Numero.PadLeft(5, '0'), "&nome=", nome, "&cod_cli=", id, "&end1=", end1, "&end2=", end2, "&mailto=", email, "&instr=", instrucoes, ".<br><br>" + naoReceber));
                //}
                //else
                //{
                //    uri = EntityBase.RetiraAcentos(String.Concat("?nossonum=", nossoNumero, "&valor=", Valor.ToString("N2"), "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dia, "&v_mes=", mes, "&v_ano=", ano, "&numdoc2=", contrato.Numero.PadLeft(5, '0'), "&nome=", nome, "&cod_cli=", id, "&end1=", end1, "&end2=", end2, "&mailto=", email, "&instr=", instrucoes, ".<br><br>" + naoReceber));
                //    //uri = EntityBase.RetiraAcentos(String.Concat("?nossonum=", nossoNumero, "&valor=", Valor.ToString("N2"), "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, "&v_dia=", dia, "&v_mes=", mes, "&v_ano=", ano, "&numdoc2=", contrato.Numero.PadLeft(5, '0'), "&nome=", nome, "&cod_cli=", id, "&end1=", end1, "&end2=", end2, "&mailto=", email, "&instr=", instrucoes, ".<br><br>" + naoReceber));
                //}

                //System.Net.WebRequest request = System.Net.WebRequest.Create(uri);
                //System.Net.WebResponse response = request.GetResponse();
                //System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
                //String finalUrl = sr.ReadToEnd();
                //sr.Close();
                //response.Close();

                String finalUrl = "";

#if DEBUG
                //if (contrato.Tipo == (int)eTipoPessoa.Fisica)
                //{
                //    finalUrl = string.Concat(
                //         ConfigurationManager.AppSettings["boletoUrl"], "?param=",                                          //"http://localhost/phpBoleto/boleto/boleto_itau.php?param=",
                //         Util.Geral.EncryptBetweenPHP(uri));
                //}
                //else
                //{
                //    finalUrl = string.Concat(
                //         ConfigurationManager.AppSettings["boleto2Url"], "?param=",                                          //"http://localhost/phpBoleto/boleto/boleto_itau.php?param=",
                //         Util.Geral.EncryptBetweenPHP(uri));
                //}
#else
                //if (contrato.Tipo == (int)eTipoPessoa.Fisica)
                //{
                //    finalUrl = string.Concat(
                //         ConfigurationManager.AppSettings["boletoUrl"], "?param=",                                          //"http://localhost/phpBoleto/boleto/boleto_itau.php?param=",
                //         Util.Geral.EncryptBetweenPHP(uri));
                //}
                //else
                //{
                //    finalUrl = string.Concat(
                //         ConfigurationManager.AppSettings["boleto2Url"], "?param=",                                          //"http://localhost/phpBoleto/boleto/boleto_itau.php?param=",
                //         Util.Geral.EncryptBetweenPHP(uri));
                //}
#endif

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
                        "<a href='", ConfigurationManager.AppSettings["appUrl"], "/boleto.aspx?key=", cobranca.ID,"' target='_blank'>clique aqui</a>.",
                        //"<a href='", finalUrl, "'>clique aqui</a>, ou copie e cole o endereço ",
                        //finalUrl, " na barra de endereços de seu navegador de internet.",
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
                if (!optCNPJ.Checked) return;

                Cobranca cobranca = new Cobranca(gridCobranca.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                cobranca.Carregar();

                txtIdCobrancaEmDetalhe.Text = Convert.ToString(cobranca.ID);
                this.exibeDetalheCobranca(cobranca);
            }
        }

        protected void gridCobranca_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridCobranca.PageIndex = e.NewPageIndex;
            this.CarregarCobrancas();
        }

        protected void cmdGerarCobranca_Click(object sender, EventArgs e)
        {
            #region validacoes

            //if (txtParcelaCob.Text.Trim() == "")
            //{
            //    Alerta(null, this, "__err1", "Informe a parcela da cobrança.");
            //    txtParcelaCob.Focus();
            //    return;
            //}

            if (this.contratoId == null)
            {
                Util.Geral.Alerta(this, "Você deve primeiramente salvar o contrato.");
                return;
            }

            DateTime dataVencimento = DateTime.MinValue;
            if (!DateTime.TryParse(txtVencimentoCob.Text, out dataVencimento))
            {
                Alerta(null, this, "__err2", "Data de vencimento inválida.");
                txtVencimentoCob.Focus();
                return;
            }
            dataVencimento = new DateTime(dataVencimento.Year, dataVencimento.Month, dataVencimento.Day, 23, 59, 59, 0);

            if ((txtValorCob.Text.Trim() == "" || CToDecimal(txtValorCob.Text) < 30) && !optCNPJ.Checked)
            {
                Alerta(null, this, "__err3", "Informe o valor da cobrança (mínimo de R$ 30,00).");
                txtValorCob.Focus();
                return;
            }

            //DateTime dataRef = DateTime.MinValue;
            //if (dataVencimento > DateTime.Now)
            //{
            //    dataRef = new DateTime(
            //        DateTime.Now.AddMonths(2).Year,
            //        DateTime.Now.AddMonths(2).Month, 1);
            //    dataRef = dataRef.AddDays(-1);
            //}


            //if (dataRef != DateTime.MinValue && dataVencimento > dataRef)
            //{
            //    Alerta(null, this, "__err", "Não é possível gerar cobranças com esse vencimento.");
            //    return;
            //}

            int vidas = 0;

            if (optCNPJ.Checked)
            {
                //object aux = LocatorHelper.Instance.ExecuteScalar(
                //            string.Concat("select cobranca_id from cobranca where cobranca_cobrancaRefId is null and cobranca_propostaId=", this.contratoId, " and cobranca_pago=1 and (cobranca_cancelada is null or cobranca_cancelada=0) and month(cobranca_dataVencimento)=", dataVencimento.Month, " and year(cobranca_dataVencimento)=", dataVencimento.Year),
                //            null, null, null);

                //if (aux != null && aux != DBNull.Value || Convert.ToString(aux).Trim() != "")
                //{
                //    Alerta(null, this, "erro", "Vencimento ja pago.");
                //    return;
                //}


                vidas = Util.CTipos.CToInt(txtQtdVidasCob.Text);

                if (vidas == 0)
                {
                    Util.Geral.Alerta(this, "Informe a quantidade de vidas.");
                    return;
                }
            }

            #endregion

            Cobranca cobranca = new Cobranca();

            var cobrancas = Cobranca.CarregarTodas(this.contratoId, false, null);
            int ultimaParcela = 1;

            if (cobrancas != null && cobrancas.Count > 0) ultimaParcela = cobrancas.Max(c => c.Parcela) + 1;

            cobranca.Parcela = ultimaParcela;
            cobranca.DataVencimento = dataVencimento;

            if (optCPF.Checked)
                cobranca.Valor = CToDecimal(txtValorCob.Text);
            else
            {
                Contrato c = new Contrato(this.contratoId);
                c.Carregar();

                string erro = "";

                cobranca.Valor = Convert.ToDecimal(vidas) * Cobranca.calulaValorPorVida(null, c, dataVencimento.ToString("dd/MM/yyyy"), out erro);

                if (cobranca.Valor == decimal.Zero)
                {
                    Util.Geral.Alerta(this, erro);
                    return;
                }

                cobranca.Valor += Cobranca.calculaAcrescimoDeContrato(null, c, null, false);
                Cobranca.calculaConfiguracaoValorAdicional(null, c, ref cobranca);

                cobranca.QtdVidas = vidas;
            }

            cobranca.Tipo = Convert.ToInt32(Cobranca.eTipo.Normal);
            cobranca.CobrancaRefID = null;
            cobranca.DataPgto = DateTime.MinValue;
            cobranca.ValorPgto = Decimal.Zero;
            cobranca.Pago = false;
            cobranca.PropostaID = this.contratoId;
            cobranca.Cancelada = false;
            cobranca.Salvar();

            //List<CobrancaComposite> composite = new List<CobrancaComposite>();

            //Contrato.CalculaValorDaProposta(this.contratoId, cobranca.DataVencimento, null, false, true, ref composite);
            //CobrancaComposite.Salvar(cobranca.ID, composite, null);
            //cobranca = null;

            this.ConfiguraAtendimento();
            Alerta(null, this, "_okNCob", "Cobrança salva com sucesso.");
        }

        protected void btnCalcularValorCob_Click(object sender, ImageClickEventArgs e)
        {
            //txtValorCob.Text = "";
            //DateTime vencto = CStringToDateTime(txtVencimentoCob.Text);
            //if (vencto == DateTime.MinValue) { return; }
            //List<CobrancaComposite> composite = null;

            //txtValorCob.Text = Contrato.CalculaValorDaProposta(this.contratoId, vencto, null, false, true, ref composite).ToString("N2");
        }

        protected void cmdRelacaoCobrancas_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this,
                    this.GetType(),
                    "_abrirRelacao_",
                    String.Concat(" window.open(\"relacaoCobrancas.aspx?", IDKey, "=", this.contratoId, "\", \"janela\", \"toolbar=no,scrollbars=1\"); "), //
                    true);
        }

        protected void cmdTabelaValor_Click(object sender, EventArgs e)
        {
            String param = "?" + IDKey + "=";
            if (cboOperadora.SelectedIndex > 0) { param += cboOperadora.SelectedValue; }
            if (cboContrato.SelectedIndex > 0) { param += "&" + IDKey2 + "=" + cboContrato.SelectedValue; }
            if (cboPlano.SelectedIndex > 0) { param += "&" + IDKey3 + "=" + cboPlano.SelectedValue; }

            ScriptManager.RegisterClientScriptBlock(this,
                    this.GetType(),
                    "_abrirRelacao_",
                    String.Concat(" window.open(\"tabelavalorPoup.aspx", param, "\", \"janela\", \"toolbar=no,scrollbars=1\"); "), //
                    true);
        }

        protected void imgDemonstPagtoPrint_Click(object sender, ImageClickEventArgs e)
        {
            if (this.contratoId == null || this.contratoId != null) { return; }

            ///////////////
            DataTable dados = LocatorHelper.Instance.ExecuteQuery("select * from ir_dados_preprod_sp (nolock) where UTILIZAR_REGISTRO = 1 and ENVIAR_DMED = 1 and idcedente=2 and idproposta=" + this.contratoId, "result", null).Tables[0];
            if (dados.Rows.Count == 0)
            {
                dados.Dispose();
                return;
            }

            //DataRow[] ret = dados.Select("UTILIZAR_REGISTRO = 0 OR ENVIAR_DMED = 0");
            //if (ret.Length > 0)
            //{
            //    dados.Dispose();
            //    return;
            //}
            dados.Dispose();
            ///////////////

            String ano = ConfigurationManager.AppSettings["anoRefDemonstrPagtos"];

            ScriptManager.RegisterClientScriptBlock(this,
                    this.GetType(),
                    "_abrir_",
                    String.Concat(" window.open(\"demonstPagtos.aspx?ano=", ano, "&", IDKey, "=", this.contratoId, "\", \"janela\", \"toolbar=no,scrollbars=1\"); "),
                    true);
        }

        protected void imgDemonstPagtoMail_Click(object sender, ImageClickEventArgs e)
        {
            if (this.contratoId == null || this.contratoId != null) { return; }
            if (String.IsNullOrEmpty(txtEmailAtendimento.Text.Trim())) { return; }
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            String ano = ConfigurationManager.AppSettings["anoRefDemonstrPagtos"];
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            DataTable dados = LocatorHelper.Instance.ExecuteQuery("select * from ir_dados_preprod_sp where UTILIZAR_REGISTRO = 1 AND ENVIAR_DMED = 1 AND idcedente=2 and idproposta=" + contratoId, "result", pm).Tables[0];
            if (dados.Rows.Count == 0)
            {
                dados.Dispose();
                //Alerta(null, this, "err", "Demonstrativo indisponível.");
                pm.CloseSingleCommandInstance();
                pm.Dispose();
                return;
            }

            //DataRow[] ret = dados.Select("UTILIZAR_REGISTRO = 0 OR ENVIAR_DMED = 0");
            //if (ret.Length > 0)
            //{
            //    dados.Dispose();
            //    //Alerta(null, this, "err", "Demonstrativo indisponível.");
            //    pm.CloseSingleCommandInstance();
            //    pm.Dispose();
            //    return;
            //}

            dados.Dispose();

            string titularContratoBeneficiarioId = "";
            List<String> dependContratoBeneficiarioIds = new List<string>();

            foreach (DataRow row in dados.Rows)
            {
                if (CToString(row["SEQUENCIA"]) == "0") { titularContratoBeneficiarioId = CToString(row["IDPROPONENTE"]); continue; }

                dependContratoBeneficiarioIds.Add(CToString(row["IDPROPONENTE"]));
            }

            #region corpo do e-mail

            Contrato contrato = Contrato.CarregarParcial((Object)contratoId, pm);
            Operadora operadora = new Operadora(contrato.OperadoraID);
            pm.Load(operadora);
            ContratoBeneficiario cTitular = ContratoBeneficiario.CarregarPorIDContratoBeneficiario(titularContratoBeneficiarioId, pm);

            //if (cTitular.DMED == false)
            //{
            //    Alerta(null, this, "err", "Titular com pendências DMED.");
            //    pm.CloseSingleCommandInstance();
            //    pm.Dispose();
            //    return;
            //}

            sb.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
            sb.Append("<head><title></title>");
            sb.Append("<style type='text/css'>body, html{ font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:#b8b8b8; font-size:11px; background-color:white; margin:0px; height:100%; }link              { font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:blue; font-size:11px; } table             { font-family: Arial, Trebuchet MS, Verdana, Helvetica; color:black; font-size:11px; }</style>");
            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append("<table align=\"center\" width=\"95%\">");
            sb.Append(" <tr>");
            sb.Append(" <td>");
            sb.Append("<table><tr><td><h2>Demonstrativo de Pagamentos "); sb.Append(ano); sb.Append("</td><td width='35'>&nbsp;</td><td align='left'><img align=\"right\" src='http://www.linkecerebro.com.br/LogoMail.png' /></h2></td></tr></table>");
            sb.Append(" <table style=\"font-size:12px\">");
            sb.Append(" <tr>");
            sb.Append(" <td colspan=\"2\">");
            sb.Append(String.Concat("São Paulo, ", DateTime.Now.Day, " de ", DateTime.Now.ToString("MMMM"), " de ", DateTime.Now.Year, "."));
            sb.Append(" </td>");
            sb.Append(" </tr>");
            sb.Append(" <tr><td height='5px'></td></tr>");

            if (!string.IsNullOrEmpty(contrato.ResponsavelNome) && !string.IsNullOrEmpty(contrato.ResponsavelCPF))
            {
                sb.Append(" <tr><td width=\"140px\"><b>Ilmo(a). Senhor(a)</b></td>"); sb.Append("<td>"); sb.Append(contrato.ResponsavelNome); sb.Append("</td></tr>");
                sb.Append(" <tr><td><b>CPF:</b></td><td>"); sb.Append(contrato.ResponsavelCPF); sb.Append("</td></tr>");
            }
            else
            {
                sb.Append(" <tr><td width=\"140px\"><b>Ilmo(a). Senhor(a)</b></td>"); sb.Append("<td>"); sb.Append(cTitular.BeneficiarioNome); sb.Append("</td></tr>");
                sb.Append(" <tr><td><b>CPF:</b></td><td>"); sb.Append(cTitular.BeneficiarioCPF); sb.Append("</td></tr>");
            }
            sb.Append("</table><br />");

            sb.Append("<table style=\"font-size:12px\"><tr><td><b>Cliente PS Padrão,</b></td></tr><tr><td height='8'></td></tr><tr><td>");
            sb.Append("Abaixo o demonstrativo de pagamentos efetuados, durante o ano calendário de "); sb.Append(ano); sb.Append(", à PS Padrão ");
            sb.Append("Administradora de Benefícios Ltda., inscrita no CNPJ/MF sob o nº 11.273.573/0001-05, e destinados à ");
            sb.Append("manutenção do plano privado de assistência à saúde, coletivo por adesão, por meio de contrato coletivo ");
            sb.Append("firmado com a operadora [operadora].<br />");
            sb.Append("Esse demonstrativo relaciona as despesas médicas que foram pagas pelo(a) Sr(a). e que são dedutíveis em ");
            sb.Append("Declaração de Imposto de Renda.");
            sb.Append("</td></tr></table></td></tr></table><br />");

            #region MESES

            Decimal total = 0, totalJan = 0, totalFev = 0, totalMar = 0, totalAbr = 0, totalMaio = 0, totalJun = 0, totalJul = 0, totalAgo = 0, totalSet = 0;

            totalJan = CToDecimal(dados.Compute("SUM(JAN)", ""));
            totalFev = CToDecimal(dados.Compute("SUM(FEV)", ""));
            totalMar = CToDecimal(dados.Compute("SUM(MAR)", ""));
            totalAbr = CToDecimal(dados.Compute("SUM(ABR)", ""));
            totalMaio = CToDecimal(dados.Compute("SUM(MAI)", ""));
            totalJun = CToDecimal(dados.Compute("SUM(JUN)", ""));
            totalJul = CToDecimal(dados.Compute("SUM(JUL)", ""));
            totalAgo = CToDecimal(dados.Compute("SUM(AGO)", ""));
            totalSet = CToDecimal(dados.Compute("SUM(SETEM)", ""));

            total = totalJan + totalFev + totalMar + totalAbr + totalMaio + totalJun + totalJul + totalAgo + totalSet;

            sb.Append("<table align='center' cellpadding=\"4\" cellspacing=\"0\" style=\"border:solid 1px black;font-size:12px\" width=\"400px\"><tr><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Competência</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>*Valor mensal</b></td></tr>");

            sb.Append("<tr><td>Janeiro</td><td>");
            sb.Append(totalJan.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Fevereiro</td><td>");
            sb.Append(totalFev.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Março</td><td>");
            sb.Append(totalMar.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Abril</td><td>");
            sb.Append(totalAbr.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Maio</td><td>");
            sb.Append(totalMaio.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Junho</td><td>");
            sb.Append(totalJun.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Julho</td><td>");
            sb.Append(totalJul.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Agosto</td><td>");
            sb.Append(totalAgo.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td>Setembro</td><td>");
            sb.Append(totalSet.ToString("N2")); sb.Append("</td></tr>");

            sb.Append("<tr><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>*Valor total</td><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>" + total.ToString("N2") + "</td></tr>");
            sb.Append("<tr><td colspan=\"2\" style=\"border-top:solid 1px black;font-size:11px\"><i>*Valor expresso em reais, sem tarifas bancárias.</i></td></tr></table>");
            sb.Append("<br /><br />");

            #endregion MESES

            sb.Append("<center><div style=\"color:black\"><b>COMPOSIÇÃO DO GRUPO FAMILIAR</b></div></center><table align='center' cellpadding=\"4\" cellspacing=\"0\" style=\"border:solid 1px black;font-size:12px\" width=\"400px\"><tr><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Condição</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>Nome</b></td><td style=\"border-bottom:solid 1px black\" bgcolor='whitesmoke'><b>*Valor por beneficiário(a)</b></td></tr>");

            IList<ContratoBeneficiario> cbeneficiarios = null;
            if (dependContratoBeneficiarioIds != null && dependContratoBeneficiarioIds.Count > 0)
                cbeneficiarios = ContratoBeneficiario.Carregar(dependContratoBeneficiarioIds.ToArray(), pm);

            cTitular.Valor = CToDecimal(dados.Compute(
                "sum(JAN)+sum(FEV)+sum(MAR)+sum(ABR)+sum(MAI)+sum(JUN)+sum(JUL)+sum(AGO)+sum(SETEM)", "IDPROPONENTE=" + cTitular.ID));

            if (cbeneficiarios != null)
            {
                foreach (ContratoBeneficiario cb in cbeneficiarios)
                {
                    cb.Valor = CToDecimal(dados.Compute(
                        "sum(JAN)+sum(FEV)+sum(MAR)+sum(ABR)+sum(MAI)+sum(JUN)+sum(JUL)+sum(AGO)+sum(SETEM)", "IDPROPONENTE=" + cb.ID));
                }
            }
            else
                cbeneficiarios = new List<ContratoBeneficiario>();

            cbeneficiarios.Insert(0, cTitular);

            foreach (ContratoBeneficiario cb in cbeneficiarios)
            {
                if (cb.Valor > 0)
                {
                    sb.Append("<tr>");

                    sb.Append("<td>");
                    if (cb.Tipo == 0)
                        sb.Append("Titular");
                    else
                        sb.Append("Dependente");
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(cb.BeneficiarioNome);
                    sb.Append("</td>");

                    sb.Append("<td>");
                    sb.Append(cb.Valor.ToString("N2"));
                    sb.Append("</td>");

                    sb.Append("</tr>");
                }
            }
            sb.Append("<tr><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke'>*Valor total</td><td style=\"border-top:solid 1px black\" bgcolor='whitesmoke' colspan=\"2\">" + total.ToString("N2") + "</td></tr>");
            sb.Append("<tr><td style=\"border-top:solid 1px black;font-size:11px\" colspan=\"3\"><i>*Valor expresso em reais, sem tarifas bancárias.</i></td></tr></table><br /><br />");

            sb.Append("<table align=\"center\" width=\"95%\"><tr><td>Atenção: Caso este informe seja utilizado para fins de declaração de Imposto de Renda, esclarecemos que somente podem ser deduzidas as parcelas relativas ao contribuinte e aos dependentes devidamente relacionados na própria declaração. As deduções estão sujeitas às regras estabelecidas pela legislação que regulamenta o imposto (Decreto nº 3.000/99).</td></tr><tr><td height='8'></td></tr><tr><td><b>PS Padrão Administradora de Benefícios</b></td></tr></table>");

            sb.Append("<br><br><font size='1' color='red'>Este é um e-mail automático. Por favor, não o responda.</font>");
            sb.Append("</body>");
            sb.Append("</html>");


            String corpo = sb.ToString();
            if (cboOperadora.SelectedItem.Text.IndexOf("-") > -1)
                corpo = corpo.Replace("[operadora]", cboOperadora.SelectedItem.Text.Split('-')[1].Trim());
            else
                corpo = corpo.Replace("[operadora]", cboOperadora.SelectedItem.Text.Trim());

            #endregion corpo do e-mail

            //envia email
            MailMessage msg = new MailMessage(
                new MailAddress(ConfigurationManager.AppSettings["mailFrom"], ConfigurationManager.AppSettings["mailFromName"]),
                new MailAddress(txtEmailAtendimento.Text));
            msg.Subject = "Demonstrativo de Pagamentos " + ano;
            msg.IsBodyHtml = true;
            msg.Body = corpo;

            try
            {
                SmtpClient client = new SmtpClient();
                client.Send(msg);
                msg.Dispose();
                client = null;
                Alerta(null, this, "_ok", "E-mail enviado com sucesso.");
            }
            catch { }
        }

        protected void imgDemonstPagtoQualiPrint_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void imgDemonstPagtoQualiMail_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void cmdCartaDePermanenciaPrint_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void cmdCartaDePermanecia_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void cmdTermoAnualPrint_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void cmdTermoAnual_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void gridSelDependente_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("usar"))
            {
                Object id = gridSelDependente.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Beneficiario benef = new Beneficiario(id);
                benef.Carregar();
                this.ExibeBeneficiarioDependenteCarregado(benef);
                this.MontaCombosDeBeneficiarios();
                pnlSelDependente.Visible = false;
            }
        }

        protected void chkFSim_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void checkboxGridAdicional_Changed(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;

            GridViewRow grow = (GridViewRow)check.NamingContainer;
            int index = grow.RowIndex;

            Adicional ad = new Adicional();
            ad.ID = gridAdicional.DataKeys[index].Value;
            ad.Carregar();

            if (check.Checked && !ad.Ativo)
            {
                check.Checked = false;
                Alerta(null, this, "_err", "Adicional inativo no momento.");
                return;
            }

            IList<AdicionalBeneficiario> itens =
                (IList<AdicionalBeneficiario>)ViewState["adben_" + cboBeneficiarioAdicional.SelectedValue];

            if (itens == null) { itens = new List<AdicionalBeneficiario>(); }

            if (check.Checked) //adiciona na colecao
            {
                for (int i = 0; i < gridAdicional.Rows.Count; i++)
                {
                    Object atualAdicionalId = null;
                    Object atualBeneficiarioId = null;

                    if (gridAdicional.DataKeys[0].Values.Count > 0)
                    {
                        atualAdicionalId = gridAdicional.DataKeys[i][0];
                        atualBeneficiarioId = cboBeneficiarioAdicional.SelectedValue;
                    }

                    AdicionalBeneficiario item = PegaNaColecao(itens, atualAdicionalId, atualBeneficiarioId);

                    Boolean adiciona = false;
                    if (item == null)
                    {
                        item = new AdicionalBeneficiario();
                        adiciona = true;
                    }

                    item.AdicionalID = atualAdicionalId;
                    item.PropostaID = ViewState[IDKey];

                    if (i == index)
                        item.BeneficiarioID = cboBeneficiarioAdicional.SelectedValue;

                    if (adiciona)
                        itens.Add(item);
                }

                //checa se ha alguma regra

                if (ad.ParaTodaProposta)// (ar != null && ar.Tipo == Convert.ToInt32(AdicionalRegra.eTipo.TitularETodosDependentes))
                {
                    foreach (ListItem _item in cboBeneficiarioAdicional.Items)
                    {
                        IList<AdicionalBeneficiario> _itens =
                            (IList<AdicionalBeneficiario>)ViewState["adben_" + _item.Value];
                        if (_itens == null)
                        {
                            _itens = AdicionalBeneficiario.Carregar(cboContrato.SelectedValue, cboPlano.SelectedValue, ViewState[IDKey], _item.Value);
                        }

                        foreach (AdicionalBeneficiario aben in _itens)
                        {
                            if (Convert.ToString(ad.ID) == Convert.ToString(aben.AdicionalID))
                            {
                                aben.AdicionalID = ad.ID;
                                aben.BeneficiarioID = _item.Value;
                                aben.PropostaID = ViewState[IDKey];
                                break;
                            }
                        }

                        ViewState["adben_" + _item.Value] = _itens;
                    }
                }
            }
            else //remove da colecao
            {
                //checa se ha alguma regra
                if (ad.ParaTodaProposta) // (ar != null && ar.Tipo == Convert.ToInt32(AdicionalRegra.eTipo.TitularETodosDependentes))
                {
                    foreach (ListItem _item in cboBeneficiarioAdicional.Items)
                    {
                        IList<AdicionalBeneficiario> _itens =
                            (IList<AdicionalBeneficiario>)ViewState["adben_" + _item.Value];

                        foreach (AdicionalBeneficiario aben in _itens)
                        {
                            if (Convert.ToString(ad.ID) == Convert.ToString(aben.AdicionalID))
                            {
                                aben.BeneficiarioID = null;

                                if (aben.ID != null) { aben.Remover(); }
                                break;
                            }
                        }

                        ViewState["adben_" + _item.Value] = _itens;
                    }
                }
                else
                {
                    if (itens != null)
                    {
                        itens[index].BeneficiarioID = null;

                        if (itens[index].ID != null) { itens[index].Remover(); }
                    }
                }
            }

            ViewState["adben_" + cboBeneficiarioAdicional.SelectedValue] = itens;
            this.ExibeSumario();
        }

        //-----------------------

        protected void cmdVoltar_Click(Object sender, EventArgs e)
        {
            Response.Redirect("clientes.aspx");
        }

        protected void cmdSalvar_Click(Object sender, EventArgs e)
        {
            if (!IsValid) { return; }

            #region validacoes

            #region Tab1

            if (txtCorretorTerceiroIdentificacao.Text.Trim() != "" && txtCorretorTerceiroCPF.Text.Trim() == "")
            {
                Alerta(null, this, "_CorrTerc", "Atenção!\\nVocê deve informar o CPF do corretor.");
                txtCorretorTerceiroCPF.Focus();
                tab.ActiveTabIndex = 0;
                return;
            }

            if (txtCorretorTerceiroCPF.Text.Trim() != "")
            {
                if (!Beneficiario.ValidaCpf(txtCorretorTerceiroCPF.Text))
                {
                    Alerta(null, this, "err", "Atenção!<br>CPF do corretor pessoa física inválido.");
                    tab.ActiveTabIndex = 0;
                    return;
                }
            }

            if (txtSuperiorTerceiroIdentificacao.Text.Trim() != "" && txtSuperiorTerceiroCPF.Text.Trim() == "")
            {
                Alerta(null, this, "_SupeTerc", "Atenção!\\nVocê deve informar o CPF do superior.");
                txtSuperiorTerceiroCPF.Focus();
                tab.ActiveTabIndex = 0;
                return;
            }

            if (txtSuperiorTerceiroCPF.Text.Trim() != "")
            {
                if (!Beneficiario.ValidaCpf(txtSuperiorTerceiroCPF.Text))
                {
                    Alerta(null, this, "_SupeTercCpf", "Atenção!<br>CPF do superior pessoa física inválido.");
                    tab.ActiveTabIndex = 0;
                    txtSuperiorTerceiroCPF.Focus();
                    return;
                }
            }

            //nao pode acontecer de um titular ser tambem um dependente na proposta
            if (this.Dependentes != null)
            {
                foreach (ContratoBeneficiario cb in this.Dependentes)
                {
                    if (Convert.ToString(cb.BeneficiarioID) == Convert.ToString(this.TitularID))
                    {
                        Alerta(null, this, "_err", "Atenção!<br>Titular e dependente são a mesma pessoa.");
                        tab.ActiveTabIndex = 0;
                        return;
                    }
                }
            }

            //if (String.IsNullOrEmpty(txtCorretorID.Value))
            //{
            //    Alerta(null, this, "_err", "Atenção!<br>Selecione um profissional emissor do contrato.");
            //    tab.ActiveTabIndex = 0;
            //    return;
            //}

            //if (txtNumeroContrato.Text.Trim() == String.Empty)
            //{
            //    Alerta(null, this, "_err", "Atenção!\\nInforme o número do contrato.");
            //    txtNumeroContrato.Focus();
            //    tab.ActiveTabIndex = 0;
            //    return;
            //}

            if (!HaItemSelecionado(cboEstipulante))
            {
                Alerta(null, this, "_err", "Atenção!\\nNão há um Associado PJ selecionado.");
                tab.ActiveTabIndex = 0;
                cboEstipulante.Focus();
                return;
            }

            if (!HaItemSelecionado(cboPlano))
            {
                Alerta(null, this, "_err", "Atenção!\\nNão há um plano selecionado.");
                tab.ActiveTabIndex = 0;
                return;
            }

            if (cboTipoProposta.SelectedIndex <= 0)
            {
                Alerta(null, this, "_err", "Atenção!\\nNão há um tipo de proposta selecionado.");
                tab.ActiveTabIndex = 0;
                cboTipoProposta.Focus();
                return;
            }

            String msg = "";
            if (!Contrato.ContratoDisponivel(ViewState[IDKey], cboOperadora.SelectedValue, txtNumeroContrato.Text, ref msg))
            {
                Alerta(null, this, "_err", msg);
                tab.ActiveTabIndex = 0;
                txtNumeroContrato.Focus();
                return;
            }

            //if (cboAcomodacao.SelectedIndex <= 0)
            //{
            //    Alerta(null, this, "_err", "Atenção!<br>Não há um tipo de acomodação selecionado.");
            //    tab.ActiveTabIndex = 0;
            //    cboAcomodacao.Focus();
            //    return;
            //}

            //if (txtVigencia.Text == "" || txtVencimento.Text == "")
            //{
            //    Alerta(null, this, "_err", "Atenção!<br>Informe uma data de admissão valida.");
            //    tab.ActiveTabIndex = 0;
            //    return;
            //}

            //if (pnlInfoAnterior.Visible && (txtEmpresaAnterior.Text.Trim() == "" || txtEmpresaAnteriorMeses.Text.Trim() == "" || txtEmpresaAnteriorMatricula.Text == ""))
            //{
            //    Alerta(null, this, "_err", "Atenção!\\nInforme os dados da empresa anterior.");
            //    tab.ActiveTabIndex = 0;
            //    return;
            //}

            ////checa se o impresso foi rasurado...
            //String letra = String.Empty;
            //if (UIHelper.PrimeiraPosicaoELetra(txtNumeroContrato.Text))
            //    letra = txtNumeroContrato.Text.Substring(0, 1);

            //AlmoxContratoImpresso aci = null;

            //try
            //{
            //    if (!String.IsNullOrEmpty(letra))
            //        aci = AlmoxContratoImpresso.CarregarPorNumeroProduto(Convert.ToInt32(txtNumeroContrato.Text.Replace(letra, "")), cboOperadora.SelectedValue, letra, -1);
            //    else
            //        aci = AlmoxContratoImpresso.CarregarPorNumeroProduto(Convert.ToInt32(txtNumeroContrato.Text), cboOperadora.SelectedValue, letra, -1);

            //    if (aci != null && aci.Rasurado)
            //    {
            //        base.Alerta(MPE, ref litAlert, "Atenção!<br>Essa proposta foi confirmada pelo almoxarifado como rasurada.", upnlAlerta);
            //        return;
            //    }
            //}
            //catch
            //{
            //    aci = null;
            //}

            #endregion

            #region Tab2

            if (String.IsNullOrEmpty(txtNome.Text.Trim()))
            {
                Alerta(null, this, "_err", "Atenção!\\nNão há um titular.");
                tab.ActiveTabIndex = 1;
                return;
            }
            else
            {
                IList<Beneficiario> lista = Beneficiario.CarregarPorParametro("", txtCPF.Text, "", SearchMatchType.QualquerParteDoCampo); //TODO: criar um load parcial. Está carregando o obj inteiro.
                if (lista == null || lista.Count == 0)
                {
                    Alerta(null, this, "_err", "Atenção!\\nCPF do titular não encontrado.");
                    tab.ActiveTabIndex = 1;
                    return;
                }
            }

            if (txtDataNascimentoResponsavel.Text.Trim() != "")
            {
                DateTime data = new DateTime();
                if (!DateTime.TryParse(txtDataNascimentoResponsavel.Text, out data))
                {
                    Alerta(null, this, "_dataNascResp", "Atenção!\\nData de nascimento do responsável legal inválida.");
                    tab.ActiveTabIndex = 1;
                    txtDataNascimentoResponsavel.Focus();
                    return;
                }
            }

            //if (txtCarenciaCodigo.Text.Trim() == "" && this.contratoId == null)
            //{
            //    Alerta(null, this, "_PRCTit", "Atenção!\\nPRC do titular é obrigatório.");
            //    tab.ActiveTabIndex = 1;
            //    txtCarenciaCodigo.Focus();
            //    return;
            //}

            //if (Operadora.IsAmil(cboOperadora.SelectedValue) && txtCarenciaCodigo.Text.Trim() != "")
            //{
            //    if (!Operadora.ValidaPRCAmil(txtCarenciaCodigo.Text))
            //    {
            //        String validos = String.Join(", ", Operadora.AmilPRCs);

            //        if (validos.Trim() != "")
            //        {
            //            Alerta(null, this, "_PRCTit1", "Atenção!\\nPRC do titular é inválido.\\nValores válidos: " + validos);
            //            tab.ActiveTabIndex = 1;
            //            txtCarenciaCodigo.Focus();
            //            return;
            //        }
            //    }
            //}

            DateTime nasc = CStringToDateTime(txtDataNascimento.Text);
            Int32 idade = Beneficiario.CalculaIdade(nasc, DateTime.Now);

            if (idade < 18) // && (txtNomeResponsavel.Text.Trim() == "" || txtCPFResponsavel.Text.Trim() == ""))
            {
                if (this.contratoId == null)
                {
                    if (txtNomeResponsavel.Text.Trim() == "" || txtCPFResponsavel.Text.Trim() == "")
                    {
                        Alerta(null, this, "_RespTit", "Atenção!\\nVocê deve informar o responsável legal do titular (nome e CPF). ");
                        tab.ActiveTabIndex = 1;
                        txtNomeResponsavel.Focus();
                        return;
                    }

                    if (!Beneficiario.ValidaCpf(txtCPFResponsavel.Text))
                    {
                        Alerta(null, this, "_RespCpfTit", "Atenção!\\nO CPF do responsável legal do titular é inválido. ");
                        tab.ActiveTabIndex = 1;
                        txtCPFResponsavel.Focus();
                        return;
                    }
                    else if (txtCPF.Text == txtCPFResponsavel.Text)
                    {
                        Alerta(null, this, "_err", "Atenção!\\nOs CPFs do responsável legal e do titular não podem ser iguais. ");
                        tab.ActiveTabIndex = 1;
                        txtCPFResponsavel.Focus();
                        return;
                    }

                    DateTime data = new DateTime();
                    if (!DateTime.TryParse(txtDataNascimentoResponsavel.Text, out data))
                    {
                        Alerta(null, this, "_dataNascResp", "Atenção!\\nData de nascimento do responsável legal inválida.");
                        tab.ActiveTabIndex = 1;
                        txtDataNascimentoResponsavel.Focus();
                        return;
                    }
                }
            }

            if (txtCPFResponsavel.Text.Trim() != "")
            {
                if (!Beneficiario.ValidaCpf(txtCPFResponsavel.Text))
                {
                    Alerta(null, this, "_RespCpfTit", "Atenção!\\nO CPF do responsável legal do titular é inválido. ");
                    tab.ActiveTabIndex = 1;
                    txtCPFResponsavel.Focus();
                    return;
                }
            }

            //if (!validaAltura(txtTitAltura))
            //{
            //    Alerta(null, this, "_altTit", "Atenção!\\nA altura do titular deve estar entre 10cm e 2,5m. ");
            //    tab.ActiveTabIndex = 1;
            //    txtTitAltura.Focus();
            //    return;
            //}

            //if (!validaPeso(txtTitPeso))
            //{
            //    Alerta(null, this, "_pesTit", "Atenção!\\nO peso do titular deve estar entre 1kg e 300kg. ");
            //    tab.ActiveTabIndex = 1;
            //    txtTitPeso.Focus();
            //    return;
            //}

            //if (!validaIMC_Titular(txtTitPeso, txtTitAltura))
            //{
            //    Alerta(null, this, "_err", "Atenção!\\nIMC fora da faixa. Encaminhar para área técnica. ");
            //}

            #endregion

            Beneficiario titular = new Beneficiario(this.TitularID);
            titular.Carregar();
            int v_idade = Beneficiario.CalculaIdade(titular.DataNascimento, DateTime.Now);// base.CStringToDateTime(txtAdmissao.Text));

            if (this.Dependentes != null)
            {
                int countMae = 0, countConjuge = 0;
                ContratoADMParentescoAgregado parentesco = null;

                foreach (ContratoBeneficiario dependente in this.Dependentes)
                {
                    //if (dependente.ID != null) { dependente.Carregar(); }
                    if (dependente.ParentescoID == null) { continue; }

                    parentesco = new ContratoADMParentescoAgregado(dependente.ParentescoID);
                    parentesco.Carregar();

                    if (parentesco.ParentescoDescricao.ToLower().IndexOf("filh") > -1 && v_idade <= 12)
                    {
                        Alerta(null, this, "_err", "Não é possível cadastrar um(a) filho(a) como dependente para este titular.");
                        tab.ActiveTabIndex = 0;
                        return;
                    }

                    if (parentesco.ParentescoDescricao.ToLower().IndexOf("pai") > -1 ||
                        parentesco.ParentescoDescricao.ToLower().Replace("ã", "a").IndexOf("mae") > -1)
                    {
                        countMae++;
                    }

                    if (parentesco.ParentescoDescricao.ToLower().IndexOf("espos") > -1 ||
                        parentesco.ParentescoDescricao.ToLower().Replace("ô", "o").IndexOf("conjuge") > -1)
                    {
                        countConjuge++;
                    }
                }

                if (countConjuge > 1)
                {
                    Alerta(null, this, "_err", "Apenas um cônjuge como dependente é permitido.");
                    tab.ActiveTabIndex = 0;
                    return;
                }

                if (countMae > 2)
                {
                    Alerta(null, this, "_err", "Apenas um pai e uma mãe serão permitidos como dependentes.");
                    tab.ActiveTabIndex = 0;
                    return;
                }
            }

            if (cboBeneficiarioFicha.Items.Count == 0)
            {
                //base.Alerta(null, this, "_numBenef", "Atenção!\\nNão há beneficiários na proposta.");
                Alerta(null, this, "_err", "Atenção!\\nNão há beneficiários na proposta.");
                return;
            }

            //if (!ChecaNumeroDeContrato())
            //{
            //    tab.ActiveTabIndex = 0;
            //    return;
            //}

            if (ViewState[PropostaEndReferecia] == null)
            {
                Alerta(null, this, "_err", "Atenção!\\nDeve haver um endereço de referência.");
                return;
            }

            if (ViewState[PropostaEndCobranca] == null)
            {
                Alerta(null, this, "_err", "Atenção!\\nDeve haver um endereço de cobrança.");
                return;
            }

            ContratoADM contrato = new ContratoADM(cboContrato.SelectedValue);
            contrato.Carregar();
            DateTime dteContrato = new DateTime(contrato.Data.Year, contrato.Data.Month, contrato.Data.Day, 0, 0, 0);
            DateTime admissao = CStringToDateTime(txtAdmissao.Text);
            if (admissao < dteContrato)
            {
                Alerta(null, this, "_err", "Atenção!\\nA data de admissão não pode ser inferior à data do contrato administrativo.");
                return;
            }

            if (!validaNumero()) return;

            #endregion

            try
            {
                this.Salvar(null, false);
            }
            catch (Exception ex)
            {
                Alerta(null, this, "_errSvInes", ex.Message.Replace("'", "´"));
            }
        }

        void pegaObs(ref Contrato contrato)
        {
            if (!String.IsNullOrEmpty(txtObs.Text.Trim()))
                contrato.Obs = txtObs.Text;

            if (!String.IsNullOrEmpty(txtObsEdit.Text.Trim()))
            {
                if (!String.IsNullOrEmpty(contrato.Obs) && contrato.Obs.Length > 0)
                    contrato.Obs += Environment.NewLine + Environment.NewLine + txtObsEdit.Text;
                else
                    contrato.Obs = txtObsEdit.Text;
            }

            txtObs.Text = contrato.Obs;
            txtObsEdit.Text = "";
            upFinalizacao.Update();
        }

        void Salvar(Object usuarioLiberadorId, Boolean rascunho)
        {
            if (cboFilial.SelectedIndex == 0 && this.contratoId == null) { Alerta(null, this, "_err", "Você deve informar a filial."); return; }
            if (cboEstadoCivil.Items.Count == 0) { Alerta(null, this, "_err", "Não há um estado civil selecionado."); return; }

            Object[] fichas = null; // this.ObtemFichasPreenchidas();

            Contrato contrato = new Contrato();
            contrato.Rascunho = rascunho;
            contrato.ID = ViewState[IDKey];

            if (cboFilial.SelectedIndex > 0)
                contrato.FilialID = cboFilial.SelectedValue;
            else
                contrato.FilialID = null;

            Boolean novo = true;

            if (contrato.ID != null)
            {
                Contrato prova = new Contrato(contrato.ID);
                prova.Carregar();
                contrato.Pendente = prova.Pendente;
                contrato.Data = prova.Data;
                contrato.CodCobranca = prova.CodCobranca;
                contrato.Alteracao = DateTime.Now;
                contrato.UsuarioID = prova.UsuarioID;
                contrato.Inativo = prova.Inativo;
                contrato.Cancelado = prova.Cancelado;
                contrato.DataCancelamento = prova.DataCancelamento;
                contrato.DataValidade = prova.DataValidade;

                ContratoBeneficiario titularAntigo = ContratoBeneficiario.CarregarTitular(prova.ID, null);
                if (titularAntigo != null && this.TitularID_ContratoBeneficiario != null)
                {
                    if (Convert.ToString(titularAntigo.BeneficiarioID) != Convert.ToString(this.TitularID))
                    {
                        //alterou o titular, log a ação
                        LC.Framework.Phantom.NonQueryHelper.Instance.ExecuteNonQuery(
                            String.Concat("insert into CONTRATO_BENEFICIARIO_LOG (log_contratoId,log_beneficiarioId, log_usuarioId, log_data) values (", prova.ID, ",", titularAntigo.BeneficiarioID, ",", Usuario.Autenticado.ID, ", getdate())"),
                            null);
                    }
                }

                contrato.Senha = prova.Senha;
                novo = false;
            }
            else
            {
                contrato.Alteracao = DateTime.MinValue;
                contrato.Data = CStringToDateTime(txtDataContrato.Text);
                contrato.Data = new DateTime(contrato.Data.Year, contrato.Data.Month, contrato.Data.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                contrato.UsuarioID = Usuario.Autenticado.ID;
            }

            contrato.DescontoAcrescimoTipo = cboTipoDescontoAcrescimo.SelectedIndex;
            if (contrato.DescontoAcrescimoTipo > 0)
            {
                contrato.DescontoAcrescimoValor = Util.CTipos.ToDecimal(txtValorDecontoAcrescimo.Text);
                contrato.DescontoAcrescimoData = Util.CTipos.CStringToDateTime(txtDataDescontoAcrescimo.Text, 23, 59, 59, 990);
            }
            else
            {
                contrato.DescontoAcrescimoValor = 0;
                contrato.DescontoAcrescimoData = DateTime.MinValue;
            }

            //if (cboCorretor.SelectedIndex <= 0)
            //    contrato.CorretorComissionadoId = null;
            //else
            //    contrato.CorretorComissionadoId = cboCorretor.SelectedValue;

            contrato.EstipulanteID = cboEstipulante.SelectedValue;
            contrato.OperadoraID = cboOperadora.SelectedValue;
            contrato.ContratoADMID = cboContrato.SelectedValue;
            if (chkCobrarTaxa.Visible && chkCobrarTaxa.Checked)
                contrato.CobrarTaxaAssociativa = true;
            else
                contrato.CobrarTaxaAssociativa = false;

            if (optCPF.Checked)
                contrato.Tipo = (int)eTipoPessoa.Fisica;
            else
                contrato.Tipo = (int)eTipoPessoa.Juridica;

            contrato.Vidas = this.CToInt(txtVidasCobertas.Text);

            contrato.NumeroID = this.ContratoImpressoID;

            if (ViewState[PropostaEndCobranca] != null)
                contrato.EnderecoCobrancaID = ViewState[PropostaEndCobranca];
            else
                contrato.EnderecoCobrancaID = ViewState[PropostaEndReferecia];

            contrato.EnderecoReferenciaID = ViewState[PropostaEndReferecia];

            contrato.Admissao = CStringToDateTime(txtAdmissao.Text);
            contrato.Vigencia = CStringToDateTime(txtVigencia.Text);
            contrato.Vencimento = CStringToDateTime(txtVencimento.Text);
            contrato.Desconto = CToDecimal(txtDesconto.Text);

            //checa se houve participacao de um operador de telemarketing
            if (!String.IsNullOrEmpty(txtOperador.Text))
            {
                contrato.OperadorTmktID = txtOperadorID.Value;
            }

            contrato.DonoID = txtCorretorID.Value;

            contrato.CorretorTerceiroNome = txtCorretorTerceiroIdentificacao.Text;
            contrato.CorretorTerceiroCPF = txtCorretorTerceiroCPF.Text;
            contrato.SuperiorTerceiroNome = txtSuperiorTerceiroIdentificacao.Text;
            contrato.SuperiorTerceiroCPF = txtSuperiorTerceiroCPF.Text;

            contrato.Numero = txtNumeroContrato.Text;
            contrato.NumeroMatricula = txtNumeroMatricula.Text;
            contrato.PlanoID = cboPlano.SelectedValue;
            //contrato.TipoAcomodacao = Convert.ToInt32(cboAcomodacao.SelectedValue);
            contrato.TipoContratoID = cboTipoProposta.SelectedValue;

            contrato.ResponsavelParentescoID = cboParentescoResponsavel.SelectedValue;
            contrato.ResponsavelSexo = cboSexoResponsavel.SelectedValue;
            contrato.ResponsavelRG = txtRGResponsavel.Text;
            contrato.ResponsavelNome = txtNomeResponsavel.Text;
            contrato.ResponsavelCPF = txtCPFResponsavel.Text;
            if (txtDataNascimentoResponsavel.Text.Trim() != "")
            {
                DateTime data = new DateTime();
                if (DateTime.TryParse(txtDataNascimentoResponsavel.Text, out data))
                {
                    contrato.ResponsavelDataNascimento = data;
                }
            }

            ContratoBeneficiario titular = new ContratoBeneficiario();
            titular.ID = this.TitularID_ContratoBeneficiario;
            if (titular.ID != null) { titular.Carregar(); }
            titular.BeneficiarioID = this.TitularID;
            titular.EstadoCivilID = cboEstadoCivil.SelectedValue; //cboEstadoCivilDependente.SelectedValue;
            titular.DataCasamento = CStringToDateTime(txtTitDataCasamento.Text);
            titular.NumeroMatriculaSaude = txtNumMatriculaSaude.Text;
            titular.NumeroMatriculaDental = txtNumMatriculaDental.Text;
            titular.Altura = CToDecimal(txtTitAltura.Text);
            titular.Peso = CToDecimal(txtTitPeso.Text);

            if (this.chkCobrarTaxa.Visible && this.chkCobrarTaxa.Checked)
                titular.Valor = this.PegaValor(titular.BeneficiarioID) + ValorTaxaAssociativaContrato;
            else
                titular.Valor = this.PegaValor(titular.BeneficiarioID);

            titular.Portabilidade = txtPortabilidade.Text;
            titular.CarenciaOperadora = cboCarenciaOperadora.Text;
            titular.CarenciaOperadoraID = CToObject(txtCarenciaOperadoraID.Value);
            titular.CarenciaCodigo = txtCarenciaCodigo.Text;
            titular.CarenciaContratoDe = CStringToDateTime(txtTitTempoContratoDe.Text);
            titular.CarenciaContratoAte = CStringToDateTime(txtTitTempoContratoAte.Text);

            if (txtCarenciaTempoContrato.Text == "") txtCarenciaTempoContrato.Text = "0";
            titular.CarenciaContratoTempo = CToInt(txtCarenciaTempoContrato.Text);
            titular.CarenciaMatriculaNumero = txtCarenciaMatricula.Text;

            //obtem os adicionais contratados
            List<AdicionalBeneficiario> adicionaisContratados = null;
            foreach (ListItem item in cboBeneficiarioAdicional.Items)
            {
                if (ViewState["adben_" + item.Value] != null)
                {
                    if (adicionaisContratados == null) { adicionaisContratados = new List<AdicionalBeneficiario>(); }
                    foreach (AdicionalBeneficiario _ab in ((List<AdicionalBeneficiario>)ViewState["adben_" + item.Value]))
                    {
                        if (_ab.BeneficiarioID != null) { adicionaisContratados.Add(_ab); }
                    }
                }
            }

            this.pegaObs(ref contrato);

            if (contrato.Tipo == (int)eTipoPessoa.Juridica)
            {
                //
            }
            else
            {
                contrato.Senha = txtSenhaContrato.Text;
            }

            if (pnlInfoAnterior.Visible)
            {
                contrato.EmpresaAnterior = txtEmpresaAnterior.Text;
                contrato.EmpresaAnteriorTempo = Convert.ToInt32(txtEmpresaAnteriorMeses.Text);
                contrato.EmpresaAnteriorMatricula = txtEmpresaAnteriorMatricula.Text;
            }
            else
            {
                contrato.EmpresaAnterior = null;
                contrato.EmpresaAnteriorTempo = 0;
                contrato.EmpresaAnteriorMatricula = null;
            }

            if (this.Dependentes != null)
            {
                foreach (ContratoBeneficiario dependente in this.Dependentes)
                {
                    dependente.Valor = this.PegaValor(dependente.BeneficiarioID);
                }
            }

            string msg = "";

            if (ViewState[IDKey] != null)
            {
                ////checa se houve alteracao de plano. se houve, grava historico
                //Contrato contratoAntigo = new Contrato();
                //contratoAntigo.ID = ViewState[IDKey];
                //contratoAntigo.Carregar();

                //if (ViewState[AlteraPlanoKey] != null) //cboPlano.SelectedValue != Convert.ToString(contratoAntigo.PlanoID) &&  : Também há a possibilidade de alterar o tipo de acomodação, mas sem necessariamente trocar de plano!
                //{
                //    DateTime admissao = new DateTime();
                //    DateTime.TryParse(Convert.ToString(ViewState[NovaDataAdmisssaoKey]), out admissao);
                //    ContratoPlano cp = new ContratoPlano();
                //    cp.ContratoID = contratoAntigo.ID;
                //    cp.Data = admissao;
                //    cp.PlanoID = cboPlano.SelectedValue;
                //    cp.TipoAcomodacao = Convert.ToInt32(cboAcomodacao.SelectedValue);
                //    cp.Salvar();

                //    //guarda para gerar arquivo de movimentação
                //    if (Convert.ToString(contrato.OperadoraID) == Convert.ToString(Operadora.UnimedID))
                //    {
                //        ItemAgendaArquivoUnimed item = new ItemAgendaArquivoUnimed();
                //        item.PropostaID = cp.ContratoID;
                //        item.BeneficiarioID = titular.BeneficiarioID;
                //        item.Tipo = 4;
                //        item.TipoDescricao = "MUDANÇA DE PLANO";
                //        item.Salvar();
                //    }
                //}

                Conferencia conferencia = Session[ConferenciaObjKey] as Conferencia;
                if (conferencia != null) { titular.Data = conferencia.PropostaData; }
                contrato.Alteracao = DateTime.Now;

                bool ret = ContratoFacade.Instance.Salvar(contrato, titular, this.Dependentes, fichas, usuarioLiberadorId, adicionaisContratados, conferencia, this.ValorTotalProposta, out msg);

                if (!ret)
                {
                    Alerta(null, this, "_err", msg);
                    return;
                }

                ViewState.Remove(AlteraPlanoKey);
                Session[ConferenciaObjKey] = null;
            }
            else
            {
                Conferencia conferencia = Session[ConferenciaObjKey] as Conferencia;
                if (conferencia != null) { titular.Data = conferencia.PropostaData; }
                bool ret = ContratoFacade.Instance.Salvar(contrato, titular, this.Dependentes, fichas, usuarioLiberadorId, adicionaisContratados, conferencia, this.ValorTotalProposta, out msg);

                if (!ret)
                {
                    Alerta(null, this, "_err", msg);
                    return;
                }

                //ContratoPlano cp = new ContratoPlano();
                //cp.ContratoID = contrato.ID;
                //cp.Data = contrato.Data;
                //cp.PlanoID = contrato.PlanoID;
                //cp.Salvar();
            }

            ViewState[IDKey] = contrato.ID;
            //this.CarregaFichaDeSaude(true);
            //cmdAlterarPlano.Visible = true; denis

            tblMsgRegras.Visible = false;
            Session[ConferenciaObjKey] = null;

            if (novo) 
            { 
                Session[FilialIDKey] = cboFilial.SelectedValue; 
                Response.Redirect(string.Concat("cliente.aspx?", IDKey, "=", contrato.ID));
            }
            else
            {
                msg = "";
                //ContratoFacade.Instance.AtualizaValorDeCobrancas(contrato.ID, out msg);
                this.ConfiguraAtendimento();

                if (String.IsNullOrEmpty(msg))
                    Alerta(null, this, "_salvo", "Contrato salvo com sucesso.");
                else
                    Alerta(null, this, "_salvo", "Contrato salvo com sucesso.\\nATENÇÃO: " + msg);

                upAtendimento.Update();
            }
        }

        #region exibir

        void exibeDetalheCobranca(Cobranca cobranca)
        {
            pnlCobrancaDetalhe.Visible = true;
            litTitulo.Text = String.Concat("<b>Parcela ", cobranca.Parcela,
                " - Vencto.: ", cobranca.DataVencimento.ToString("dd/MM/yyyy"), " - Valor: ", cobranca.Valor.ToString("C"), "</b>");

            //IList<CobrancaComposite> composite = CobrancaComposite.Carregar(cobranca.ID);
            //gridComposicao.DataSource = composite;
            //gridComposicao.DataBind();

            txtAltCobrancaValor.Text = cobranca.Valor.ToString("N2");
            txtAltCobrancaVidas.Text = cobranca.QtdVidas.ToString();
            txtAltCobrancaVencto.Text = cobranca.DataVencimento.ToString("dd/MM/yyyy");
            chkAltCobrancaCancelada.Checked = cobranca.Cancelada;

            cmdSalvarCobrancaDetalhe.Enabled = true;
            chkAltCobrancaValor.Checked = true;
            txtAltCobrancaValor.ReadOnly = false;

            if (cobranca.Pago)
            {
                cmdSalvarCobrancaDetalhe.Enabled = false;

                //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                //CobrancaBaixa baixa = CobrancaBaixa.CarregarUltima(cobranca.ID);
                //sb.Append("<tr height='4'><td height='4'></td></tr><tr><td height='20' align='center' style='background-color:whitesmoke;'><b>Informações da Baixa</b></td></tr>");
                //if (baixa != null)
                //{
                //    CobrancaMotivoBaixa motivo = new CobrancaMotivoBaixa(baixa.MotivoID);
                //    motivo.Carregar();

                //    sb.Append("<tr><td>Data: "); sb.Append(baixa.Data.ToString("dd/MM/yyyy HH:mm")); sb.Append("</td></tr>");
                //    sb.Append("<tr><td>Valor Pago: "); sb.Append(cobranca.ValorPgto.ToString("C")); sb.Append("</td></tr>");
                //    sb.Append("<tr><td>Motivo: "); sb.Append(motivo.Descricao); sb.Append("</td></tr>");
                //    sb.Append("<tr><td>Tipo: "); sb.Append(baixa.strTipo); sb.Append("</td></tr>");
                //    sb.Append("<tr><td>Baixa financeira: "); sb.Append(baixa.BaixaFinanceira ? "Sim" : "Não"); sb.Append("</td></tr>");
                //    sb.Append("<tr><td>Baixa provisória: "); sb.Append(baixa.BaixaProvisoria ? "Sim" : "Não"); sb.Append("</td></tr>");

                //    if (baixa.UsuarioID != null)
                //    {
                //        Usuario u = new Usuario(baixa.UsuarioID);
                //        u.Carregar();
                //        sb.Append("<tr><td>Usuário: "); sb.Append(u.Nome); sb.Append("</td></tr>");
                //    }

                //    if (!String.IsNullOrEmpty(baixa.Obs))
                //    {
                //        sb.Append("<tr height='5'><td height='5'></td></tr>");
                //        sb.Append("<tr><td>"); sb.Append(baixa.Obs); sb.Append("</td></tr>");
                //    }
                //}
                //else
                //{
                //    sb.Append("<tr><td>Data: ");
                //    if (cobranca.Parcela == 1)
                //        sb.Append(cobranca.DataCriacao.ToString("dd/MM/yyyy HH:mm"));
                //    else if (cobranca.DataBaixaAutomatica != DateTime.MinValue)
                //        sb.Append(cobranca.DataBaixaAutomatica.ToString("dd/MM/yyyy HH:mm"));
                //    else
                //        sb.Append(cobranca.DataPgto.ToString("dd/MM/yyyy HH:mm"));
                //    sb.Append("</td></tr>");
                //    sb.Append("<tr><td>Valor Pago: "); sb.Append(cobranca.ValorPgto.ToString("C")); sb.Append("</td></tr>");
                //    sb.Append("<tr><td>Tipo: AUTOMÁTICA</td></tr>");
                //}

                //litBaixa.Text = sb.ToString();
                //sb.Remove(0, sb.Length);
            }
            else
            {
                litBaixa.Text = "";
            }


            #region /////// NEGOCIACAO /////////////////////////////////////////////
            //litNegociacao.Text = "";
            //if (cobranca.Tipo == (int)Cobranca.eTipo.Parcelamento)
            //{
            //    ParcelamentoItem item = ParcelamentoItem.CarregarPorCobrancaId(cobranca.ID);
            //    if (item != null)
            //    {
            //        System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //        sb.Append("<tr height='4'><td height='4'></td></tr><tr><td height='20' align='center' style='background-color:whitesmoke;'><b>Informações de Negociação</b></td></tr>");
            //        sb.Append("<tr><td>");
            //        sb.Append(item.Observacoes);

            //        ParcelamentoHeader header = new ParcelamentoHeader();
            //        header.ID = item.HeaderID;
            //        header.Carregar();

            //        sb.Append("<br><br>Data negociação: "); sb.Append(header.Data.ToString("dd/MM/yyyy"));
            //        sb.Append("<br>Empresa: "); sb.Append(ParcelamentoHeader.GetEmpresaNome(header.EmpresaID));
            //        sb.Append("<br>Parcelas negociadas: ");

            //        IList<Cobranca> negociadas = ParcelamentoCobrancaOriginal.CarregarParcelasNegociadas(header.ID);
            //        if (negociadas != null)
            //        {
            //            for (int i = 0; i < negociadas.Count; i++)
            //            {
            //                if (i > 0 && i < (negociadas.Count - 1)) sb.Append(", ");
            //                else if (i > 0) sb.Append(" e ");

            //                sb.Append(negociadas[i].Parcela);
            //            }
            //        }

            //        if (!String.IsNullOrEmpty(header.OBS))
            //        {
            //            sb.Append("<br>Obs.: "); sb.Append(header.OBS.Replace(Environment.NewLine, "<br>"));
            //        }
            //        sb.Append("</td></tr>");
            //        litNegociacao.Text = sb.ToString();
            //        sb.Remove(0, sb.Length);
            //    }
            //}
            //else
            //{
            //    ParcelamentoCobrancaOriginal pco = ParcelamentoCobrancaOriginal.CarregarPorCobrancaId(cobranca.ID);
            //    if (pco != null)
            //    {
            //        ParcelamentoHeader header = new ParcelamentoHeader();
            //        header.ID = pco.HeaderID;
            //        header.Carregar();

            //        System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //        sb.Append("<tr height='4'><td height='4'></td></tr><tr><td height='20' align='center' style='background-color:whitesmoke;'><b>Informações de Negociação</b></td></tr>");
            //        sb.Append("<tr><td>");
            //        sb.Append("<font color='red'>ESTA COBRANÇA FAZ PARTE DE UMA NEGOCIAÇÃO ATIVA.</font>");

            //        sb.Append("<br><br>Data negociação: "); sb.Append(header.Data.ToString("dd/MM/yyyy"));
            //        sb.Append("<br>Empresa: "); sb.Append(ParcelamentoHeader.GetEmpresaNome(header.EmpresaID));
            //        sb.Append("<br>Parcelas negociadas: ");

            //        IList<Cobranca> negociadas = ParcelamentoCobrancaOriginal.CarregarParcelasNegociadas(header.ID);
            //        if (negociadas != null)
            //        {
            //            for (int i = 0; i < negociadas.Count; i++)
            //            {
            //                if (i > 0 && i < (negociadas.Count - 1)) sb.Append(", ");
            //                else if (i > 0) sb.Append(" e ");

            //                sb.Append(negociadas[i].Parcela);
            //            }
            //        }

            //        if (!String.IsNullOrEmpty(header.OBS))
            //        {
            //            sb.Append("<br>Obs.: "); sb.Append(header.OBS.Replace(Environment.NewLine, "<br>"));
            //        }

            //        sb.Append("</td></tr>");
            //        litNegociacao.Text = sb.ToString();
            //        sb.Remove(0, sb.Length);
            //    }
            //}
            /////////////////////////////////////////////
            #endregion

            txtIdCobrancaEmDetalhe.Text = Convert.ToString(cobranca.ID);
            mpeCobrancaDetalhe.Show();
            upCobrancaDetalhe.Update();
        }

        protected void cmdSalvarCobrancaDetalhe_click(object sender, EventArgs e)
        {
            int vidas = Util.CTipos.CToInt(txtAltCobrancaVidas.Text);
            DateTime vencto = Util.CTipos.CStringToDateTime(txtAltCobrancaVencto.Text, 23, 59, 59, 950);

            #region validacoes 

            if (vidas == 0)
            {
                Util.Geral.Alerta(this, "Informe a quantidade de vidas.");
                pnlCobrancaDetalhe.Visible = true;
                mpeCobrancaDetalhe.Show();
                upCobrancaDetalhe.Update();
                //txtIdCobrancaEmDetalhe.Text = "";
                return;
            }

            if (vencto == DateTime.MinValue || vencto < DateTime.Now)
            {
                Util.Geral.Alerta(this, "Informe uma data de vencimento maior que hoje.");
                pnlCobrancaDetalhe.Visible = true;
                mpeCobrancaDetalhe.Show();
                upCobrancaDetalhe.Update();
                //txtIdCobrancaEmDetalhe.Text = "";
                return;
            }
            #endregion

            bool altVencto = false, altValor = false, altCancel = false;

            Cobranca cob = new Cobranca(txtIdCobrancaEmDetalhe.Text);
            cob.Carregar();
            if (cob.Pago)
            {
                Util.Geral.Alerta(this, "Cobrança ja liquidada.");
                txtIdCobrancaEmDetalhe.Text = "";
                return;
            }

            Contrato contrato = new Contrato(cob.PropostaID);
            contrato.Carregar();

            if (chkAltCobrancaValor.Checked)
            {
                altValor = true;
            }
            else
            {
                decimal novoValor = Util.CTipos.ToDecimal(txtAltCobrancaValor.Text);
                if (novoValor != cob.Valor || Convert.ToInt32(txtAltCobrancaVidas.Text) != cob.QtdVidas)
                {
                    altValor = true;
                }
            }

            //altValor = true;

            if (cob.DataVencimento.Day   != vencto.Day ||
                cob.DataVencimento.Month != vencto.Month ||
                cob.DataVencimento.Year != vencto.Year) altVencto = true;
            if (cob.Cancelada != chkAltCobrancaCancelada.Checked) altCancel = true;

            DateTime dataVencimentoAntiga = cob.DataVencimento;
            decimal valorAntigo = cob.Valor;

            if (altVencto) cob.DataVencimento = vencto;

            if (altValor)
            {
                string erro = "";

                if (!chkAltCobrancaValor.Checked)
                {
                    cob.Valor = Convert.ToDecimal(vidas) * Cobranca.calulaValorPorVida(null, contrato, vencto.ToString("dd/MM/yyyy"), out erro);
                }
                else
                {
                    decimal novoValor = Util.CTipos.ToDecimal(txtAltCobrancaValor.Text);
                    cob.Valor = novoValor;
                }

                if (cob.Valor == decimal.Zero)
                {
                    if (string.IsNullOrEmpty(erro)) erro = "Valor deve ser superior a zero.";
                    Util.Geral.Alerta(this, erro);
                    return;
                }

                cob.Valor += Cobranca.calculaAcrescimoDeContrato(null, contrato, null, false);
                Cobranca.calculaConfiguracaoValorAdicional(null, contrato, ref cob);

                cob.QtdVidas = vidas;
            }

            if (altCancel || altValor || altVencto)
            {
                cob.Cancelada = chkAltCobrancaCancelada.Checked;
                cob.Salvar();

                if ((altValor || altVencto) && !altCancel)
                {
                    CobrancaLog.CobrancaCriadaLog log = new CobrancaLog.CobrancaCriadaLog();
                    log.CobrancaValor = 0;
                    log.PropostaID = cob.PropostaID;
                    log.DataEnviada = vencto.ToString("dd/MM/yyyy");
                    log.Vidas = vidas;
                    log.Msg = "Alteracao de valor ou vencto";
                    log.Origem = (int)CobrancaLog.Fonte.Sistema;
                    log.CobrancaValor = valorAntigo;
                    log.CobrancaVencimento = dataVencimentoAntiga;
                    log.Salvar();

                    //se ja estava registrada, então tem que gerar movimentacao de alteracao
                    if (cob.ArquivoIDUltimoEnvio != null)
                    {
                        CobrancaLog.PendenciaCNAB pendencia = CobrancaLog.PendenciaCNAB.CarregaPendente(cob.ID, null);
                        if (pendencia == null) pendencia = new CobrancaLog.PendenciaCNAB();

                        pendencia.AlteracaoValor = altValor;
                        pendencia.AlteracaoVencimento = altVencto;
                        pendencia.CobrancaID = cob.ID;
                        pendencia.Processado = false;
                        pendencia.Origem = (int)CobrancaLog.Fonte.Sistema;
                        pendencia.Data = DateTime.Now;

                        pendencia.Salvar();
                    }

                    Util.Geral.Alerta(this, "Cobrança alterada com sucesso.");
                }

                this.ConfiguraAtendimento();
            }

            txtIdCobrancaEmDetalhe.Text = "";
        }

        protected void chkAltCobrancaValor_CheckedChanged(object sender, EventArgs e)
        {
            txtAltCobrancaValor.ReadOnly = !chkAltCobrancaValor.Checked;
            pnlCobrancaDetalhe.Visible = true;
            mpeCobrancaDetalhe.Show();
            upCobrancaDetalhe.Update();
        }

        /**********************************************************************************/

        void ExibeBeneficiarioDependenteCarregado(Beneficiario beneficiario)
        {
            if (beneficiario == null)
            {
                this.DependenteID = null;
                txtNomeDependente.Text = "";
                txtDataNascimentoDependente.Text = "";
                txtDepAltura.Text = "";
                txtDepPeso.Text = "";
                txtDepAdmissao.Text = "";
            }
            else
            {
                this.DependenteID = beneficiario.ID;
                txtNomeDependente.Text = beneficiario.Nome.ToUpper();
                txtCPFDependente.Text = beneficiario.CPF;
                txtRGDependente.Text = beneficiario.RG;

                if (!String.IsNullOrEmpty(beneficiario.Sexo))
                    cboSexoDependente.SelectedValue = beneficiario.Sexo;

                if (beneficiario.DataNascimento != DateTime.MinValue)
                    txtDataNascimentoDependente.Text = beneficiario.DataNascimento.ToString("dd/MM/yyyy");

                if (ViewState[IDKey] != null)
                {
                    ContratoBeneficiario cb = ContratoBeneficiario.CarregarPorContratoEBeneficiario(ViewState[IDKey], beneficiario.ID, null);

                    if (cb != null)
                    {
                        txtDepPeso.Text = cb.Peso.ToString("N2"); //beneficiario.Peso.ToString("N2");
                        txtDepAltura.Text = cb.Altura.ToString("N2");//beneficiario.Altura.ToString("N2");
                        cboEstadoCivilDependente.SelectedValue = Convert.ToString(cb.EstadoCivilID);
                    }
                }

                cboParentescoDependente.Focus();
            }
        }

        void exibeAtendimento(AtendimentoTemp atendimento)
        {
            if (atendimento.TipoID != null)
                cboTipoAtendimento.SelectedValue = Convert.ToString(atendimento.TipoID);
            else
                cboTipoAtendimento.SelectedIndex = 0;

            lblAtendimentoProtocolo.Text = Convert.ToString(atendimento.ID);
            //txtTitulo.Text = atendimento.Titulo;
            txtTexto.Text = atendimento.Texto;
            txtTexto.ReadOnly = true;
            txtTexto2.Visible = true;
            chkAtendimentoConcluido.Checked = !String.IsNullOrEmpty(atendimento.ResolvidoPor); //txtDataConclusao.Text = dateToString(atendimento.DataFim);
            txtDataInicio.Text = dateToString(atendimento.DataInicio);
            txtDataPrevisao.Text = dateToString(atendimento.DataPrevisao);

            if (atendimento.IniciadoPor != null && atendimento.IniciadoPor.Length > 32) { atendimento.IniciadoPor = atendimento.IniciadoPor.Substring(0, 31); }
            litCriadoPor.Text = "por: " + atendimento.IniciadoPor;

            if (atendimento.SubTipoID != null)
                cboSubTipoAtendimento.SelectedValue = Convert.ToString(atendimento.SubTipoID);
            else
                cboSubTipoAtendimento.SelectedIndex = 0;

            txtDataInicio.ReadOnly = true;
            imgDataInicio.Visible = false;

            txtTexto2.Focus();

            if (!String.IsNullOrEmpty(atendimento.ResolvidoPor))
            {
                txtDataPrevisao.ReadOnly = true;
                imgDataPrevisao.Visible = false;

                chkAtendimentoConcluido.Enabled = false;

                if (atendimento.ResolvidoPor.Length > 42) { atendimento.ResolvidoPor = atendimento.ResolvidoPor.Substring(0, 41); }
                litResolvidoPor.Text = String.Concat("por ", atendimento.ResolvidoPor, " em ", atendimento.DataFim.ToString("dd/MM/yyyy HH:mm"));
                cmdSalvarAtendimento.Enabled = false;
            }
            else
            {
                txtDataPrevisao.ReadOnly = false;
                imgDataPrevisao.Visible = true;

                chkAtendimentoConcluido.Enabled = true;

                cmdSalvarAtendimento.Enabled = true;
            }
        }

        protected void ExibirEstadosCivis(DropDownList combo, Boolean itemSELECIONE, Object operadoraId)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = EstadoCivil.CarregarTodos(operadoraId);
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirEstadosCivisDeUsuario(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = EstadoCivilUsuario.CarregarTodos();
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirOperadoras(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Nome";
            combo.DataSource = Operadora.CarregarTodas(true);
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirOperadoras(DropDownList combo, Boolean itemSELECIONE, Boolean somenteAtivas)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Nome";
            combo.DataSource = Operadora.CarregarTodas(somenteAtivas);
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirOperadoras(ListBox list)
        {
            list.Items.Clear();
            list.DataValueField = "ID";
            list.DataTextField = "Nome";
            list.DataSource = Operadora.CarregarTodas(true);
            list.DataBind();
        }

        protected void ExibirPlanos(DropDownList combo, Object contratoID, Boolean itemSELECIONE, Boolean apenasAtivos)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = Plano.CarregarPorContratoID(contratoID, apenasAtivos);
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirEstipulantes(DropDownList combo, Boolean itemSELECIONE, Boolean apenasAtivos)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = Estipulante.Carregar(apenasAtivos);
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirOpcoesDeSexo(DropDownList combo, Boolean itemSELECIONE)
        {
            MedProj.www.Util.Geral.ExibirOpcoesDeSexo(combo, itemSELECIONE);
        }

        protected void ExibirOpcoesDeTipoDeContrato(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();
            //combo.Items.Add(new ListItem("NOVA", "1"));
            //combo.Items.Add(new ListItem("COMPRA DE CARÊNCIA", "2"));
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = TipoContrato.Carregar(true);
            combo.DataBind();

            if (itemSELECIONE)
                combo.Items.Insert(0, new ListItem("selecione", "-1"));
        }

        protected void ExibirParentescos(DropDownList combo, Object planoId, Boolean itemSELECIONE, Parentesco.eTipo tipo)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";

            if (planoId == null)
                combo.DataSource = Parentesco.CarregarTodos(tipo);
            else
                combo.DataSource = Parentesco.CarregarNaoUsadosPor(planoId, tipo);

            combo.DataBind();
            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        protected void ExibirCategorias(DropDownList cboCategoriaComissionamento, Boolean apenasAtivos, Boolean itemSelecione)
        {
            cboCategoriaComissionamento.Items.Clear();
            cboCategoriaComissionamento.DataValueField = "ID";
            cboCategoriaComissionamento.DataTextField = "Descricao";
            cboCategoriaComissionamento.DataSource = Categoria.Carregar(apenasAtivos);
            cboCategoriaComissionamento.DataBind();

            if (itemSelecione)
                cboCategoriaComissionamento.Items.Insert(0, new ListItem("selecione", "-1"));
        }

        protected void ExibirFiliais(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();

            combo.DataValueField = "ID";
            combo.DataTextField = "Nome";
            combo.DataSource = Filial.CarregarTodas(true);
            combo.DataBind();

            if (itemSELECIONE) { combo.Items.Insert(0, new ListItem("Selecione", "-1")); }
        }

        protected void ExibirTiposDeProdutos(DropDownList combo, Boolean itemSELECIONE, String textoSELECIONE)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = AlmoxTipoProduto.CarregarTodos();
            combo.DataBind();

            if (itemSELECIONE) { combo.Items.Insert(0, new ListItem(textoSELECIONE, "-1")); }
        }

        protected void ExibirProdutos(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = AlmoxProduto.CarregarTodos(true);
            combo.DataBind();

            if (itemSELECIONE) { combo.Items.Insert(0, new ListItem("Selecione", "-1")); }
        }

        protected void ExibirTiposDeAcomodacao(DropDownList combo, Boolean comum, Boolean particular, Boolean itemSELECIONE)
        {
            combo.Items.Clear();
            if (itemSELECIONE)
                combo.Items.Add(new ListItem("Selecione", "-1"));

            if (comum)
                combo.Items.Add(new ListItem("QUARTO COLETIVO", "0"));

            if (particular)
                combo.Items.Add(new ListItem("QUARTO PARTICULAR", "1"));
        }

        protected void ExibirTipoDeTaxaEstipulante(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();

            if (itemSELECIONE)
                combo.Items.Add(new ListItem("Selecione", "-1"));

            combo.Items.Add(new ListItem("POR VIDA", "0"));
            combo.Items.Add(new ListItem("POR PROPOSTA", "1"));
        }

        void ExibeBeneficiarioCarregado(Beneficiario beneficiario, Object idEnderecoAExibir)
        {
            if (beneficiario == null)
            {
                this.TitularID = null;
                txtNome.Text = "";
                txtDataNascimento.Text = "";
                txtNomeMae.Text = "";
                txtFone1.Text = "";
                txtFone2.Text = "";
                txtFone3.Text = "";
                txtDDD1.Text = "";
                txtDDD2.Text = "";
                txtDDD3.Text = "";
                txtRamal1.Text = "";
                txtRamal2.Text = "";
                txtEmail.Text = "";
                txtTitAltura.Text = "";
                txtTitPeso.Text = "";
                cmdAlterarBeneficiarioTitular.Visible = false;
                ExibeEnderecosDeBeneficiarioCarregado(null, null);
                txtCarenciaCodigo.Text = "";
                txtPortabilidade.Text = "";
                txtCarenciaMatricula.Text = "";
                txtCarenciaTempoContrato.Text = "";
                txtTitTempoContratoDe.Text = "";
                txtTitTempoContratoAte.Text = "";
            }
            else
            {
                this.TitularID = beneficiario.ID;
                txtNome.Text = beneficiario.Nome.ToUpper();
                txtCPF.Text = beneficiario.CPF;
                txtRG.Text = beneficiario.RG;

                if (!String.IsNullOrEmpty(beneficiario.Sexo))
                    cboSexo.SelectedValue = beneficiario.Sexo;

                if (beneficiario.DataNascimento != DateTime.MinValue)
                    txtDataNascimento.Text = beneficiario.DataNascimento.ToString("dd/MM/yyyy");

                txtNomeMae.Text = beneficiario.NomeMae;

                if (ViewState[IDKey] != null)
                {
                    ContratoBeneficiario cb = ContratoBeneficiario.CarregarPorContratoEBeneficiario(ViewState[IDKey], beneficiario.ID, null);

                    if (cb != null)
                    {
                        txtTitPeso.Text = cb.Peso.ToString("N2"); //beneficiario.Peso.ToString("N2");
                        txtTitAltura.Text = cb.Altura.ToString("N2");//beneficiario.Altura.ToString("N2");
                        this.SetaEstadoCivil(cb.EstadoCivilID);
                    }
                }

                txtFone1.Text = PegaTelefone(beneficiario.Telefone);
                txtFone2.Text = PegaTelefone(beneficiario.Telefone2);
                txtFone3.Text = PegaTelefone(beneficiario.Celular);
                txtDDD1.Text = PegaDDD(beneficiario.Telefone);
                txtDDD2.Text = PegaDDD(beneficiario.Telefone2);
                txtDDD3.Text = PegaDDD(beneficiario.Celular);
                txtRamal1.Text = beneficiario.Ramal;
                txtRamal2.Text = beneficiario.Ramal2;
                txtEmail.Text = beneficiario.Email;

                this.PreparaLinkParaEditarTitular();
                cmdAlterarBeneficiarioTitular.Visible = true;
                ExibeEnderecosDeBeneficiarioCarregado(beneficiario.ID, idEnderecoAExibir);

                cboEstadoCivil.Focus();
            }
        }

        void ExibeEnderecosDeBeneficiarioCarregado(Object beneficiarioId, Object idEnderecoAExibir)
        {
            IList<Endereco> lista = null;

            if (beneficiarioId != null)
            {
                lista = Endereco.CarregarPorDono(beneficiarioId, Endereco.TipoDono.Beneficiario);
            }

            if (lista == null)
            {
                ExibeEnderecoDeBeneficiarioCarregado(null);
                gridEnderecosDisponiveis_Titular.DataSource = null;
                gridEnderecosDisponiveis_Titular.DataBind();
                spanEnderecosDisponiveis_Titular.Visible = false;
            }
            else
            {
                gridEnderecosDisponiveis_Titular.DataSource = lista;
                gridEnderecosDisponiveis_Titular.DataBind();
                spanEnderecosDisponiveis_Titular.Visible = true;
            }

            upDadosCadastrais.Update();
        }

        void ExibeEnderecoDeBeneficiarioCarregado(Endereco endereco)
        {
            if (endereco == null)
            {
                txtCEP.Text = "";
                txtLogradouro.Text = "";
                txtNumero.Text = "";
                txtComplemento.Text = "";
                txtBairro.Text = "";
                txtCidade.Text = "";
                txtUF.Text = "";
            }
            else
            {
                txtCEP.Text = endereco.CEP;
                txtLogradouro.Text = endereco.Logradouro;
                txtNumero.Text = endereco.Numero;
                txtComplemento.Text = endereco.Complemento;
                txtBairro.Text = endereco.Bairro;
                txtCidade.Text = endereco.Cidade;
                txtUF.Text = endereco.UF.ToUpper();
                cboTipoEndereco.SelectedValue = Convert.ToString(endereco.Tipo);
                cmdEnderecoAcoes.Text = "Alterar";
            }
        }

        void ExibeEnderecosDaProposta()
        {
            System.Collections.ArrayList arr = new System.Collections.ArrayList();
            List<Endereco> lista = null;

            if (ViewState[PropostaEndCobranca] != null)
            {
                Endereco endereco = new Endereco();
                endereco.ID = ViewState[PropostaEndCobranca];
                endereco.Carregar();
                if (lista == null) { lista = new List<Endereco>(); }
                lista.Add(endereco);
            }
            if (ViewState[PropostaEndReferecia] != null)
            {
                Endereco endereco = new Endereco();
                endereco.ID = ViewState[PropostaEndReferecia];
                endereco.Carregar();
                if (lista == null) { lista = new List<Endereco>(); }
                lista.Add(endereco);
            }

            gridEnderecosSelecionados.DataSource = lista;
            gridEnderecosSelecionados.DataBind();
            upFinalizacao.Update();
        }

        #endregion

        void LimparCamposAtendimento()
        {
            gridAtendimento.SelectedIndex = -1;
            //txtTitulo.Text = "";
            txtTexto.Text = "";
            txtTexto2.Text = "";
            txtDataInicio.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtDataPrevisao.Text = "";
            lblAtendimentoProtocolo.Text = "-------";

            txtDataInicio.ReadOnly = false;
            imgDataInicio.Visible = true;

            txtDataPrevisao.ReadOnly = false;
            imgDataPrevisao.Visible = true;

            chkAtendimentoConcluido.Checked = false; //txtDataConclusao.Text = "";
            chkAtendimentoConcluido.Enabled = true;

            litResolvidoPor.Text = "";
            cboSubTipoAtendimento.SelectedIndex = 0;
        }

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
            if (this.contratoId != null) { return true; }

            Decimal peso = CToDecimal(txtPeso.Text);

            if (peso >= 1M && peso <= 300M)
                return true;
            else
                return false;
        }
        Boolean validaAltura(TextBox txtAlt)
        {
            if (this.contratoId != null) { return true; }

            Decimal altura = CToDecimal(txtAlt.Text);

            if (altura >= 0.1M && altura <= 2.5M)
                return true;
            else
                return false;
        }
        Boolean validaIMC_Titular(TextBox txtpeso, TextBox txtaltura)
        {
            if (this.contratoId != null) { return true; }

            Decimal imc = getIMC(txtpeso.Text, txtaltura.Text);

            if (imc > 30M)
                return false;
            else
                return true;
        }

        Decimal getIMC(String strpeso, String straltura)
        {
            Decimal peso = CToDecimal(strpeso);
            Decimal altura = CToDecimal(straltura);

            return (peso / (altura * altura));
        }

        protected void grid_RowDataBound_Confirmacao(Object sender, GridViewRowEventArgs e, int indiceControle, String Msg)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[indiceControle].Attributes.Add("onClick", "return confirm('" + Msg + "');");
            }
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
            txtBairro.Text = "";
            txtCEP.Text = "";
            txtCidade.Text = "";
            txtComplemento.Text = "";
            txtLogradouro.Text = "";
            txtNumero.Text = "";
            txtUF.Text = "";
        }

        protected void cmdRecalcularComposicao_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void cmdVisualizarUtilizacao_Click(object sender, EventArgs e)
        {
            this.carregaUtilizacao();
        }

        protected void cmdOperacoesManuais_Click(object sender, EventArgs e)
        {
            this.mpeOperacoesManuais.Show();
        }

        protected void cmdSalvarOpManuais_Click(object sender, EventArgs e)
        {
            if (optCNPJ.Checked)
            {
                Alerta(null, this, "_err", "Operação indisponível para contratos PJ.");
                return;
            }

            if (!optDebito.Checked && !optCredito.Checked)
            {
                Alerta(null, this, "_err", "Selecione um tipo de operação.");
                mpeOperacoesManuais.Show();
                return;
            }
            if (txtOpManualObs.Text.Trim() == "")
            {
                Alerta(null, this, "_err", "Você deve informar um comentário.");
                mpeOperacoesManuais.Show();
                return;
            }

            long contrato_id = Convert.ToInt64(this.contratoId);
            Ent.Enuns.TipoMovimentacao tipo = Ent.Enuns.TipoMovimentacao.Credito;
            if (optDebito.Checked) tipo = Ent.Enuns.TipoMovimentacao.Debito;
            decimal valor = CToDecimal(txtOpManualValor.Text);

            ArquivoBaixaFacade.Instance.AtualizaSaldo(contrato_id, tipo, valor, txtOpManualObs.Text, Convert.ToInt64(Util.UsuarioLogado.ID));

            this.CarregaContrato();
            upAdicionais.Update();
            Alerta(null, this, "ok", "Saldo atualizado com sucesso.");
            
        }

        /******************************************************************************************************/
        /******************************************************************************************************/
        /******************************************************************************************************/
        /******************************************************************************************************/

        protected void optTipo_CheckedChanged(object sender, EventArgs e)
        {
            this.configuraTipoPessoa();
        }

        void configuraTipoPessoa()
        {
            if (optCPF.Checked)
            {
                //meeCPF.Mask = "999,999,999-99";
                pnlRGTitular.Visible = true;
                pnlTitular_DataNasc_EstCivil_MatrSau.Visible = true;
                pnlTitular_Sexo_Peso_Altura.Visible = true;
                lblNomeMae_Contato.Text = "Nome da mãe";
                pnlTitular_ResponsavelLegal.Visible = true;
                pnlUtilizacaoPF.Visible = true;
                pnlUtilizacaoPJ.Visible = false;
                pnlTipoCobrancaNormal.Visible = true;
                pnlTipoCobrancaPJ.Visible = false;
            }
            else
            {
                //meeCPF.Mask = "99,999,999/9999-99";
                pnlRGTitular.Visible = false;
                pnlTitular_DataNasc_EstCivil_MatrSau.Visible = false;
                pnlTitular_Sexo_Peso_Altura.Visible = false;
                lblNomeMae_Contato.Text = "Contato";
                pnlTitular_ResponsavelLegal.Visible = false;
                pnlUtilizacaoPF.Visible = false;
                pnlUtilizacaoPJ.Visible = true;
                pnlTipoCobrancaNormal.Visible = false;
                pnlTipoCobrancaPJ.Visible = true;
            }

            upAdicionais.Update();
        }

        protected void cmdSalvarComissaoConf_Click(object sender, EventArgs e)
        {
            if (ViewState[IDKey] == null || Util.CTipos.CToString(ViewState[IDKey]).Trim() == "") return;

            long _contratoId = Convert.ToInt64(ViewState[IDKey]);

            Ent.ComissaoInicioConf conf = RegraComissaoFacade.Instance.CarregarExcecaoPorContratoId(_contratoId);
            if (conf == null) conf = new Ent.ComissaoInicioConf();

            conf.ContratoId = _contratoId;
            conf.Data = Util.CTipos.CStringToDateTimeG(txtcomissaoInicioEm.Text);
            conf.UsuarioId = Util.CTipos.CToInt(Usuario.Autenticado.ID);
            conf.Tipo = (Ent.Enuns.ComissaoInicioConfTipo)Enum.Parse(typeof(Ent.Enuns.ComissaoInicioConfTipo), cboComissaoEstagio.SelectedValue);

            RegraComissaoFacade.Instance.SalvarExcecao(conf);

            Util.Geral.Alerta(this, "Configuração salva com sucesso.");
        }

        protected void cmdExcluirComissaoConf_Click(object sender, EventArgs e)
        {
            if (ViewState[IDKey] == null || Util.CTipos.CToString(ViewState[IDKey]).Trim() == "") return;

            long _contratoId = Convert.ToInt64(ViewState[IDKey]);

            RegraComissaoFacade.Instance.ExcluirExcecao(_contratoId);

            Util.Geral.Alerta(this, "Configuração excluída com sucesso.");
        }

        protected void chkTodasCobrancas_CheckedChanged(object sender, EventArgs e)
        {
            this.CarregarCobrancas(!chkTodasCobrancas.Checked);
        }
    }
}