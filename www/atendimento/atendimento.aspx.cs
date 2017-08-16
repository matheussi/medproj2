namespace MedProj.www.atendimento
{
    using System;
    using System.Web;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using Ent = MedProj.Entidades;
    using LC.Web.PadraoSeguros.Facade;
    using Enty = LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Entity;
    using System.Text;
    using MedProj.www.Util;

    public partial class atendimento : System.Web.UI.Page
    {
        long IdAtendimento
        {
            get
            {
                if (Session["idAt"] == null) return 0;
                return (long)Session["idAt"];
            }

            set { Session["idAt"] = value; }
        }

        long IdContrato
        {
            get
            {
                if (ViewState["id"] == null) return 0;
                return (long)ViewState["id"];
            }

            set { ViewState["id"] = value; }
        }

        long IdVigencia
        {
            get
            {
                if (ViewState["idV"] == null) return 0;
                return (long)ViewState["idV"];
            }

            set { ViewState["idV"] = value; }
        }

        decimal Saldo
        {
            get
            {
                if (ViewState["saldo"] == null) return 0;
                return (decimal)ViewState["saldo"];
            }

            set { ViewState["saldo"] = value; }
        }

        decimal ValorBase
        {
            get
            {
                if (ViewState["valor"] == null) return 0;
                return (decimal)ViewState["valor"];
            }

            set { ViewState["valor"] = value; }
        }

        List<ProcedimentoVO> procedimentos
        {
            get
            {
                if (ViewState["proc"] == null) return null;

                return ViewState["proc"] as List<ProcedimentoVO>;
            }
            set { ViewState["proc"] = value; }
        }

        List<Ent.UnidadeProcedimento> procedimentosDaUnidade
        {
            get
            {
                if (Session["procU"] == null) return null;

                return Session["procU"] as List<Ent.UnidadeProcedimento>;
            }
            set { Session["procU"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (MedProj.www.Util.UsuarioLogado.TipoUsuario != MedProj.www.Util.UsuarioLogado.Tipo.ContratoDePrestador &&
                MedProj.www.Util.UsuarioLogado.IDUnidade == 0)
            {
                Response.Redirect("~/atendimento/selecionaPrestador.aspx");
            }

            base.OnLoad(e);
            this.Config();
            //divConfirm.Attributes.Add("style", "display:none");


            if (!IsPostBack)
            {
                Util.Geral.JSScript(this, "showModal()");/////////todo: retirar

                this.carregaDados();
            }
        }

        void Config()
        {
            litPrestador.Text = Util.UsuarioLogado.Nome;
            litNomeUnidade.Text = Util.UsuarioLogado.NomeUnidade;
            litEnderecoUnidade.Text = Util.UsuarioLogado.EnderecoUnidade;

            if (!string.IsNullOrEmpty(Util.UsuarioLogado.FoneUnidade))
                litContatoUnidade.Text = string.Concat("<b>Telefone:</b> ", Util.UsuarioLogado.FoneUnidade);

            if (!string.IsNullOrEmpty(Util.UsuarioLogado.EmailUnidade))
            {
                if (!string.IsNullOrEmpty(litContatoUnidade.Text)) litContatoUnidade.Text += "&nbsp;&nbsp;";

                litContatoUnidade.Text = string.Concat(litContatoUnidade.Text, "<b>E-mail:</b> ", Util.UsuarioLogado.EmailUnidade);
            }
        }

        void carregaDados()
        {
            cboProcedimentos.Items.Clear();
            cboEspecialidade.Items.Clear();

            Ent.PrestadorUnidade unidade = PrestadorUnidadeFacade.Instancia.CarregaPorId(Util.UsuarioLogado.IDUnidade);

            List<Ent.UnidadeProcedimento> procedimentos = ProcedimentoFacade.Instancia.CarregarProcedimentosDaUnidade(unidade.ID);
            this.procedimentosDaUnidade = procedimentos.OrderBy(p => p.Procedimento.Nome).ToList();
            gridTodosProcedimentos.DataSource = this.procedimentosDaUnidade;
            gridTodosProcedimentos.DataBind();

            List<string> especialidades = new List<string>(); 
            //List<Ent.UnidadeEspecialidade> especialidades = EspecialidadeFacade.Instance.CarregarProcedimentosDaUnidade(unidade.ID);

            this.ValorBase = 0;
            Ent.TabelaPrecoVigencia vigencia = unidade.TabelaPreco.Vigencias.Where(tp => tp.Ativa == true).OrderByDescending(tp => tp.DataInicio).FirstOrDefault();

            if (vigencia != null)
            {
                this.IdVigencia = vigencia.ID;
                this.ValorBase = vigencia.ValorReal;
                litTopoResumo.Text = "";// string.Concat("<br/>Tabela praticada: ", unidade.TabelaPreco.Nome, " - Vigência: ", vigencia.DataInicio.ToString("dd/MM/yyyy"));
            }
            else
            {
                litTopoResumo.Text = "";// string.Concat("<br/>Tabela praticada: ", unidade.TabelaPreco.Nome);
                imgAdd.Enabled = false;
            }

            if (procedimentos != null)
            {
                List<Ent.UnidadeProcedimento> ordenados = procedimentos.OrderBy(up => up.Procedimento.Especialidade).ToList();
                foreach (Ent.UnidadeProcedimento proc in ordenados)
                {
                    if (especialidades.Contains(proc.Procedimento.Especialidade)) continue;
                    especialidades.Add(proc.Procedimento.Especialidade);
                }
            }
            else
                imgAdd.Enabled = false;

            if (especialidades != null)
            {
                foreach(string esp in especialidades) //foreach (Ent.UnidadeEspecialidade ue in especialidades)
                {
                    cboEspecialidade.Items.Add(new ListItem(esp, esp)); //cboEspecialidade.Items.Add(new ListItem(ue.Especialidade.Nome, ue.Especialidade.ID.ToString()));
                }
            }
            else
                imgAdd.Enabled = false;

            imgAdd.Enabled = true; ///////////////////////////adicionado devido ao autocomplete
        }

        protected void cmdValidarNumero_Click(object sender, ImageClickEventArgs e)
        {
            litResultadoValidacao.Text = "";
            this.IdContrato = 0; this.Saldo = 0; this.IdAtendimento = 0;

            if (string.IsNullOrWhiteSpace(txtNumeroCartao.Text)) return;

            IList<Enty.Contrato> lista = Enty.Contrato.CarregarPorNumero(txtNumeroCartao.Text, null);
            this.iniciaAtendimento(lista);
        }

        void iniciaAtendimento(IList<Enty.Contrato> lista)
        {
            if (lista == null || lista.Count != 1)
            {
                Util.Geral.Alerta(this, "Número de cartão inválido.");
                return;
            }
            else if (lista[0].Inativo)
            {
                Util.Geral.Alerta(this, "Este cartão está inativo.");
                return;
            }

            if (txtNumeroCartao.Text.Trim() == "") txtNumeroCartao.Text = lista[0].Numero;

            Enty.ContratoBeneficiario titular = Enty.ContratoBeneficiario.CarregarTitular(lista[0].ID, null);

            if (!string.IsNullOrEmpty(titular.BeneficiarioCPF))
            {
                litResultadoValidacao.Text = string.Concat("<div class=\"alert alert-info\" role=\"alert\">",
                    titular.BeneficiarioNome, " - CPF: ", Convert.ToUInt64(titular.BeneficiarioCPF).ToString(@"000\.000\.000\-00"), "</div>");
            }
            else
            {
                litResultadoValidacao.Text = string.Concat("<div class=\"alert alert-info\" role=\"alert\">", titular.BeneficiarioNome, "</div>");
            }

            this.IdContrato = Convert.ToInt64(lista[0].ID);

            if (txtCPFTitularCartao.Text.Trim() == "") txtCPFTitularCartao.Text = titular.BeneficiarioCPF;

            Ent.Saldo objsaldo = ContratoFacade.Instance.CarregaSaldo(this.IdContrato);

            if (objsaldo == null) this.Saldo = 0;
            else this.Saldo = objsaldo.Atual;

            grid.DataSource = null;
            grid.DataBind();
            if (this.procedimentos != null) this.procedimentos.Clear();

            pnlGravar.Visible = false;
            cmdCancelarAtendimento.Visible = true;
            cmdCancelarAtendimento2.Visible = true;
        }

        protected void cmdValidarCPFTitular_Click(object sender, ImageClickEventArgs e)
        {
            litResultadoValidacao.Text = "";
            this.IdContrato = 0; this.Saldo = 0; this.IdAtendimento = 0;

            if (string.IsNullOrWhiteSpace(txtCPFTitularCartao.Text)) return;

            IList<Enty.Contrato> lista = Enty.Contrato.CarregarPorCPFTitular(txtCPFTitularCartao.Text, null);

            if (lista.Count != 1)
            {
                //Localizou mais de um contrato, deve-se selecionar o contrato correto
                pnlMaisDeUmContrato.Visible = true;
                gridMaisDeUmContrato.DataSource = lista;
                gridMaisDeUmContrato.DataBind();
                return;
            }

            this.iniciaAtendimento(lista);
        }

        protected void gridMaisDeUmContrato_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Selecionar"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(gridMaisDeUmContrato, e, 0);
                pnlMaisDeUmContrato.Visible = false;
                gridMaisDeUmContrato.DataSource = null;
                gridMaisDeUmContrato.DataBind();

                IList<Enty.Contrato> lista = Enty.Contrato.CarregarPorIDContrato(id, null);
                this.iniciaAtendimento(lista);
            }
        }

        protected void imgAdd_Click(object sender, EventArgs e)
        {
            if (this.IdContrato == 0)
            {
                Util.Geral.Alerta(this, "Cartão não selecionado.");
                return;
            }
            if (txtProcedimentoId.Value.Trim() == "" || txtProcedimentos.Text.Trim() == "") //if (cboProcedimentos.Items.Count == 0)
            {
                txtProcedimentoId.Value = "";
                Util.Geral.Alerta(this, "Procedimento não selecionado.");
                return;
            }

            //if (cboEspecialidade.Items.Count == 0)
            //{
            //    Util.Geral.Alerta(this, "Especialidade não selecionada.");
            //    return;
            //}

            if (this.procedimentos != null)
            {
                ProcedimentoVO _vo = this.procedimentos.Where(p => p.Id.ToString() == txtProcedimentoId.Value).FirstOrDefault();

                if (_vo != null)
                {
                    _vo = null;
                    pnlConfirmProc.Visible = true;
                    return;
                }
            }

            //Ent.UnidadeProcedimento up = ProcedimentoFacade.Instancia.
            //    CarregarUnidadeProcedimento(Convert.ToInt64(cboProcedimentos.SelectedValue), Util.UsuarioLogado.IDUnidade);
            Ent.UnidadeProcedimento up = ProcedimentoFacade.Instancia.
                CarregarUnidadeProcedimento(Convert.ToInt64(txtProcedimentoId.Value), Util.UsuarioLogado.IDUnidade);

            decimal valorCalculado = up.ValorCalculado;

            #region valida saldo - comentado 
            //essa validacao foi para o botao de finalizar o atendimento

            //if (grid.Rows.Count == 0)
            //{
            //    if (valorCalculado > this.Saldo)
            //    {
            //        Util.Geral.Alerta(this, "Saldo insuficiente.");
            //        return;
            //    }
            //}
            //else
            //{
            //    decimal total = 0;
            //    for (int i = 0; i < grid.Rows.Count; i++)
            //    {
            //        total += Util.CTipos.CTipo<decimal>(grid.Rows[i].Cells[3].Text);
            //    }

            //    total += valorCalculado;

            //    if (total > this.Saldo)
            //    {
            //        Util.Geral.Alerta(this, "Saldo insuficiente.");
            //        return;
            //    }
            //}
            #endregion

            if (!pnlGravar.Visible) pnlGravar.Visible = true;
            if (this.procedimentos == null) this.procedimentos = new List<ProcedimentoVO>();

            ProcedimentoVO vo = new ProcedimentoVO
            {
                Id          = up.Procedimento.ID,
                Nome        = up.Procedimento.Nome,
                Codigo      = up.Procedimento.Codigo.ToString(),
                Valor       = valorCalculado,
                EspecId     = up.Procedimento.Especialidade, //cboEspecialidade.SelectedValue, //Convert.ToInt64(cboEspecialidade.SelectedValue),
                EspecNome   = up.Procedimento.Especialidade, //cboEspecialidade.SelectedItem.Text
                Duplicado   = false
            };

            this.procedimentos.Add(vo);

            grid.DataSource = this.procedimentos;
            grid.DataBind();

            txtProcedimentos.Text = "";
            txtProcedimentoId.Value = "";
        }

        protected void cmdCancelarAtendimento_Click(object sender, ImageClickEventArgs e)
        {
            this.limparForm();
        }

        void limparForm()
        {
            this.IdContrato = 0;
            this.IdAtendimento = 0;

            litResultadoValidacao.Text = "";
            this.procedimentos = null;
            grid.DataSource = null;
            grid.DataBind();

            pnlGravar.Visible = false;
            pnlMaisDeUmContrato.Visible = false;

            gridMaisDeUmContrato.DataSource = null;
            gridMaisDeUmContrato.DataBind();

            cmdCancelarAtendimento.Visible = false;
            cmdCancelarAtendimento2.Visible = false;
        }

        protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 4, "Remover procedimento?");
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("excluir"))
            {
                long id   = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);
                string idEs = Util.Geral.ObterDataKeyValDoGrid<string>(grid, e, 1);

                this.procedimentos.RemoveAt(this.procedimentos.FindIndex(p => p.Id == id && p.EspecId == idEs));

                grid.DataSource = this.procedimentos;
                grid.DataBind();

                if (this.procedimentos.Count == 0) pnlGravar.Visible = false;
            }
        }

        

        protected void cmdGravar_Click(object sender, EventArgs e)
        {
            //divConfirm.Attributes.Add("style", "display:inline");

            lnkPrint.Visible = false;
            lnkEmail.Visible = false;
            cmdPrint.Visible = false;
            cmdEmail.Visible = false;

            cmdValidar.Enabled = true;
            litAtendimentoEfetivado.Text = "";

            Util.Geral.JSScript(this, "showModal()");

            pnlSenha.Visible      = true;
            pnlCampoEmail.Visible = false;

            ContratoBeneficiario tit = ContratoBeneficiario.CarregarTitular(this.IdContrato, null);
            if (tit != null) txtEmail.Text = tit.BeneficiarioEmail;
            else             txtEmail.Text = "";

            litComprovante.Text = documentoAtendimento(0);
        }

        //SALVA o atendimento
        protected void cmdValidar_Click(object sender, EventArgs e)
        {
            #region validacao de saldo 

            if (optCartao.Checked)
            {
                decimal valorAtendimento = 0;

                System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
                for (int i = 0; i < grid.Rows.Count; i++)
                {
                    valorAtendimento += Convert.ToDecimal(grid.Rows[i].Cells[3].Text, cinfo);
                }

                if (valorAtendimento > this.Saldo)
                {
                    Util.Geral.JSScript(this, "showModal();alert('Saldo insuficiente.');");
                    litComprovante.Text = documentoAtendimento(0);
                    return;
                }
            }

            #endregion

            lnkPrint.Visible = false;
            lnkEmail.Visible = false;
            cmdPrint.Visible = false;
            cmdEmail.Visible = false;

            //System.Threading.Thread.Sleep(3000);

            if (txtSenhaContrato.Text.Trim().Length <= 0)
            {
                Util.Geral.JSScript(this, "showModal();alert('Senha inválida.');");
                litComprovante.Text = documentoAtendimento(0);
                txtSenhaContrato.Focus();
                return;
            }
            else
            {
                bool ret = false;

                if (txtSenhaContrato.Text == System.Configuration.ConfigurationManager.AppSettings["senhaMaster"])
                {
                    ret = true;
                }
                else if (MedProj.www.Util.UsuarioLogado.TipoUsuario == MedProj.www.Util.UsuarioLogado.Tipo.ContratoDePrestador)
                {
                    ret = ContratoFacade.Instance.ValidarSenha(txtNumeroCartao.Text, 0, txtSenhaContrato.Text);
                }
                else if (MedProj.www.Util.UsuarioLogado.TipoUsuario == MedProj.www.Util.UsuarioLogado.Tipo.Administrador)
                {
                    Ent.Usuario u = UsuarioFacade.Instance.Carregar(MedProj.www.Util.CTipos.CToLong(MedProj.www.Util.UsuarioLogado.ID));
                    if (u.Senha == txtSenhaContrato.Text) ret = true;
                }

                if (!ret)
                {
                    MedProj.www.Util.Geral.JSScript(this, "showModal();alert('Senha inválida.');");
                    litComprovante.Text = documentoAtendimento(0);
                    txtSenhaContrato.Focus();
                    return;
                }
            }

            #region salva atendimento 

            Contrato contrato = new Contrato(this.IdContrato);
            contrato.Carregar();
            if (contrato.Inativo || contrato.Cancelado)
            {
                MedProj.www.Util.Geral.JSScript(this, "showModal();alert('Cartão inativo ou cancelado.');");
                litComprovante.Text = documentoAtendimento(0);
                txtSenhaContrato.Focus();
                return;
            }

            Ent.Atendimento atend = new Ent.Atendimento();
            atend.Contrato = new Ent.Contrato();
            atend.Contrato.ID = this.IdContrato;
            atend.NumeroCartao = txtNumeroCartao.Text;
            atend.Unidade = new Ent.PrestadorUnidade();
            atend.Unidade.ID = Util.UsuarioLogado.IDUnidade;
            atend.ValorBase = this.ValorBase;
            atend.Vigencia = new Ent.TabelaPrecoVigencia();
            atend.Vigencia.ID = this.IdVigencia;

            if (optCartao.Checked) atend.FormaPagto = Ent.Enuns.FormaPagtoAtendimento.Cartao;
            else                   atend.FormaPagto = Ent.Enuns.FormaPagtoAtendimento.Dinheiro;

            if (MedProj.www.Util.UsuarioLogado.TipoUsuario == MedProj.www.Util.UsuarioLogado.Tipo.Administrador)
            {
                atend.UsuarioMaster = new Ent.Usuario();
                atend.UsuarioMaster.ID = Util.CTipos.CToLong(MedProj.www.Util.UsuarioLogado.ID);
            }

            List<Ent.AtendimentoProcedimento> procedimentos = new List<Ent.AtendimentoProcedimento>();

            foreach (ProcedimentoVO vo in this.procedimentos)
            {
                Ent.AtendimentoProcedimento proc = new Ent.AtendimentoProcedimento();
                proc.Atendimento = new Ent.Atendimento();
                proc.Procedimento = new Ent.Procedimento();
                proc.Procedimento.ID = vo.Id;
                proc.Procedimento.Nome = vo.Nome;
                //proc.Especialidade = new Ent.Especialidade();
                //proc.Especialidade.ID = vo.EspecId;
                //proc.Especialidade.Nome = vo.EspecNome;
                proc.Valor = vo.Valor;
                proc.Duplicado = vo.Duplicado;

                //TODO: acrescentar propriedades para guardar o id da tabela de preço

                procedimentos.Add(proc);
            }

            long f_ret = AtendimentoCredFacade.Instance.Salvar(atend, procedimentos);
            if (f_ret == 0)
            {
                MedProj.www.Util.Geral.JSScript(this, "showModal();alert('Erro ao criar atendimento.');");
                litComprovante.Text = documentoAtendimento(0);
                return;
            }
            else if (f_ret == -1)
            {
                MedProj.www.Util.Geral.JSScript(this, "showModal();alert('Saldo insuficiente.');");
                litComprovante.Text = documentoAtendimento(0);
                return;
            }

            #endregion

            this.limparForm();                      //todo: denis descomentar e testar

            this.IdAtendimento = atend.ID;          //todo: denis descomentar e testar
            this.IdContrato = atend.Contrato.ID;    //todo: denis descomentar e testar

            pnlGravar.Visible  = false;
            cmdValidar.Enabled = false;

            lnkPrint.Visible = true;
            lnkEmail.Visible = true;
            cmdPrint.Visible = true;
            cmdEmail.Visible = true;

            this.Saldo -= procedimentos.Sum(p => p.Valor);

            Util.Geral.JSScript(this, "showModal()");

            lnkPrint.Attributes.Remove("onclick");
            lnkPrint.Attributes.Add("onclick", "if(confirm('Deseja abrir o comprovante para impressão?')) { window.open('view.aspx?" + Util.Keys.IdKey + "=" + this.IdAtendimento.ToString() + "'); return false; } else { return false; }");

            cmdPrint.Attributes.Remove("onclick");
            cmdPrint.Attributes.Add("onclick", "if(confirm('Deseja abrir o comprovante para impressão?')) { window.open('view.aspx?" + Util.Keys.IdKey + "=" + this.IdAtendimento.ToString() + "'); return false; } else { return false; }");

            litAtendimentoEfetivado.Text = "<div class='alert alert-success' role='alert'><h3>Comprovante do atendimento</h3>";
            litAtendimentoEfetivadoFechaDiv.Text = "</div>";

            pnlSenha.Visible = false;
            pnlCampoEmail.Visible = true;
        }

        protected void lnkEmail_Click(object sender, EventArgs e)
        {
            if (txtEmail.Text.Trim() == "")
            {
                Util.Geral.JSScript(this, "showModal();alert('Nenhum e-mail informado.');");
                return;
            }

            string err = "";
            bool ret = Util.Geral.Mail.Enviar("Atendimento", documentoAtendimento(this.IdAtendimento), txtEmail.Text, true, out err);

            if (ret)
            {
                Util.Geral.JSScript(this, "showModal();alert('E-mail enviado para " + txtEmail.Text + ".');");
            }
            else
            {
                Util.Geral.JSScript(this, "showModal();alert('Não foi possível envar para " + txtEmail.Text + ".');");
            }
        }

        protected void lnkPrint_Click(object sender, EventArgs e)
        {
        }


        string documentoAtendimento(long atendimentoId)
        {
            return Util.Geral.ComprovacaoAtendimentoDoc(atendimentoId, this.IdContrato, this.procedimentos, txtNumeroCartao.Text);

            //Ent.Atendimento atendimento = null;

            //if (atendimentoId > 0) atendimento = AtendimentoCredFacade.Instance.Carregar(atendimentoId);
            //else
            //{
            //    atendimento = new Ent.Atendimento();
            //    atendimento.Data = DateTime.Now;
            //    atendimento.Contrato = new Ent.Contrato();
            //    atendimento.Contrato.ID = this.IdContrato;
            //    atendimento.Unidade = PrestadorUnidadeFacade.Instancia.CarregaPorId(Util.UsuarioLogado.IDUnidade);

            //    //Ent.Procedimento _procedimento = null;
            //    atendimento.Procedimentos = new List<Ent.AtendimentoProcedimento>();

            //    if (this.procedimentos != null)
            //    {
            //        foreach (var proc in this.procedimentos)
            //        {
            //            Ent.AtendimentoProcedimento ap = new Ent.AtendimentoProcedimento();
            //            ap.Procedimento = new Ent.Procedimento();
            //            ap.Procedimento.Nome = proc.Nome;
            //            ap.Valor = proc.Valor;

            //            atendimento.Procedimentos.Add(ap);
            //        }
            //    }
            //}

            //StringBuilder sb = new StringBuilder();

            //sb.Append("<html>");
            //sb.Append("<body leftmargin='15' style='font-family:arial'>");
            //sb.Append("<table cellpadding='10px' width='100%'>");
            //sb.Append("<tr>");
            //sb.Append("<td align='center'><img src='"); sb.Append(System.Configuration.ConfigurationManager.AppSettings["logoUrl"]); sb.Append("'/></td>");
            //sb.Append("</tr>");
            //sb.Append("</table>");
            //sb.Append("<table cellpadding='10px' style='border: solid 1px gray;background-color:gainsboro' width='100%'>");
            //sb.Append("<tr>");
            //sb.Append("<td align='center'>");
            //sb.Append("<b>Comprovante de atendimento</b>");
            //sb.Append("</td>");
            //sb.Append("</tr>");
            //sb.Append("</table>");
            //sb.Append("<br>");

            ////HEADER 

            //sb.Append("<table width='100%' cellspacing='0' cellpadding='4'>");
            //sb.Append("<tr><td style='border-bottom:solid 1px gray'><b>Credenciado</b></td></tr>");
            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td>");
            //sb.Append(atendimento.Unidade.Nome);
            //sb.Append("</td>");
            //sb.Append("</tr>");
            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td>");
            //sb.Append(string.Concat(atendimento.Unidade.Endereco, ", ", atendimento.Unidade.Numero, " - ", atendimento.Unidade.Bairro, " - ", atendimento.Unidade.Cidade, " - ", atendimento.Unidade.UF)); //sb.Append(Util.UsuarioLogado.EnderecoUnidade);
            //sb.Append("</td>");
            //sb.Append("</tr>");

            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td>");

            //if (!string.IsNullOrEmpty(atendimento.Unidade.Telefone))
            //    sb.Append(string.Concat("<b>Telefone:</b> ", atendimento.Unidade.Telefone));

            //if (!string.IsNullOrEmpty(atendimento.Unidade.Email))
            //{
            //    if (!string.IsNullOrEmpty(atendimento.Unidade.Telefone)) sb.Append("&nbsp;&nbsp;");

            //    sb.Append(string.Concat("<b>E-mail:</b> ", atendimento.Unidade.Email));
            //}

            //sb.Append("</td>");
            //sb.Append("</tr>");

            //sb.Append("</table>");

            //sb.Append("<br />");

            ////DETAIL

            //ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(atendimento.Contrato.ID, null);

            //sb.Append("<table width='100%' cellspacing='0' cellpadding='4'>");
            //sb.Append("<tr><td colspan='2' style='border-bottom:solid 1px gray'><b>Beneficiário</b></td></tr>");
            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td width='130px'>Nome:</td>");
            //sb.Append("<td>"); sb.Append(titular.BeneficiarioNome); sb.Append("</td>");
            //sb.Append("</tr>");
            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td width='130px'>Data:</td>");
            //sb.Append("<td>"); sb.Append(atendimento.Data.ToString("dd/MM/yyyy")); sb.Append("</td>");
            //sb.Append("</tr>");

            //sb.Append("</table>");
            //sb.Append("<table width='100%' cellspacing='0' cellpadding='4'>");
            //sb.Append("<tr style='background-color:whitesmoke'><td colspan='2'>Procedimentos</td></tr>");

            //foreach (var proc in atendimento.Procedimentos)
            //{
            //    sb.Append("<tr style='background-color:whitesmoke'>");
            //    sb.Append("<td>");
            //    sb.Append(proc.Procedimento.Nome);
            //    sb.Append("</td>");
            //    sb.Append("<td align='right'>R$ ");
            //    sb.Append(proc.Valor.ToString("N2"));
            //    sb.Append("</td>");
            //    sb.Append("</tr>");
            //}

            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td colspan='2' align='right'>Total: R$ ");
            //sb.Append(atendimento.Procedimentos.Sum(p => p.Valor).ToString("N2"));
            //sb.Append("</tr>");

            //sb.Append("</table>");


            //sb.Append("<br><br>");
            //sb.Append("<table width='100%' cellspacing='0'>");
            //sb.Append("<tr>");
            //sb.Append("<td>Senha de autenticidade: JL98709U9Y09BHBB54DGSDAWQW</td>");
            //sb.Append("</tr>");
            //sb.Append("</table>");

            //sb.Append("</body></html>");

            //return sb.ToString();
        }

        //--//
        protected void cmdNao_Click(object sender, EventArgs e)
        {
            pnlConfirmProc.Visible = false;
        }

        protected void cmdSim_Click(object sender, EventArgs e)
        {
            //TODO: centralizar

            Ent.UnidadeProcedimento up = ProcedimentoFacade.Instancia.
                CarregarUnidadeProcedimento(Convert.ToInt64(txtProcedimentoId.Value), Util.UsuarioLogado.IDUnidade);

            decimal valorCalculado = up.ValorCalculado;

            #region valida saldo - comentado 
            //if (grid.Rows.Count == 0)
            //{
            //    if (valorCalculado > this.Saldo)
            //    {
            //        Util.Geral.Alerta(this, "Saldo insuficiente.");
            //        return;
            //    }
            //}
            //else
            //{
            //    decimal total = 0;
            //    for (int i = 0; i < grid.Rows.Count; i++)
            //    {
            //        total += Util.CTipos.CTipo<decimal>(grid.Rows[i].Cells[3].Text);
            //    }

            //    total += valorCalculado;

            //    if (total > this.Saldo)
            //    {
            //        Util.Geral.Alerta(this, "Saldo insuficiente.");
            //        return;
            //    }
            //}
            #endregion

            if (!pnlGravar.Visible) pnlGravar.Visible = true;
            if (this.procedimentos == null) this.procedimentos = new List<ProcedimentoVO>();

            ProcedimentoVO vo = new ProcedimentoVO
            {
                Id          = up.Procedimento.ID,
                Nome        = up.Procedimento.Nome,
                Codigo      = up.Procedimento.Codigo.ToString(),
                Valor       = valorCalculado,
                EspecNome   = up.Procedimento.Especialidade,
                EspecId     = up.Procedimento.Especialidade,
                Duplicado   = true
                //EspecId     = cboEspecialidade.SelectedValue, //Convert.ToInt64(cboEspecialidade.SelectedValue),
                //EspecNome   = cboEspecialidade.SelectedItem.Text
            };

            this.procedimentos.Add(vo);

            grid.DataSource = this.procedimentos;
            grid.DataBind();

            txtProcedimentos.Text = "";
            txtProcedimentoId.Value = "";

            pnlConfirmProc.Visible = false;
        }
        //--//

        protected void lnkTodosProcedimentos_Click(object sender, EventArgs e)
        {
            pnlTodosProcedimentos.Visible = true;
        }

        protected void gridTodosProcedimentos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Incluir"))
            {
                int index = Convert.ToInt32(e.CommandArgument);
                string id = ((Literal)gridTodosProcedimentos.Rows[index].Cells[0].Controls[1]).Text;
                string nome = ((Literal)gridTodosProcedimentos.Rows[index].Cells[3].Controls[1]).Text;
                txtProcedimentoId.Value = id;
                txtProcedimentos.Text = nome;
                pnlTodosProcedimentos.Visible = false;
                this.imgAdd_Click(null, null);
            }
        }

        protected void gridTodosProcedimentos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridTodosProcedimentos.DataSource = this.procedimentosDaUnidade;
            gridTodosProcedimentos.PageIndex = e.NewPageIndex;
            gridTodosProcedimentos.DataBind();
        }

        protected void cmdFecharShadowProcedimentos_Click(object sender, EventArgs e)
        {
            pnlTodosProcedimentos.Visible = false;
        }

        protected void gridTodosProcedimentos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Geral.grid_AdicionaToolTip<LinkButton>(e, 1, 0, "adicionar...");
            }
        }

        protected void optFormaPagto_CheckedChanged(object sender, EventArgs e)
        {
            if (optCartao.Checked)
            {
                cmdValidar.Visible = true;
                cmdValidar2.Visible = false;
            }
            else
            {
                cmdValidar.Visible = false;
                cmdValidar2.Visible = true;
            }

            Util.Geral.JSScript(this, "showModal()");
        }
    }
}