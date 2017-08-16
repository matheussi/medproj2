namespace MedProj.www.adm.comissao
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    //using Excel;
    using MedProj.www.Util;
    using MedProj.Entidades;
    using MedProj.Entidades.Enuns;
    using LC.Web.PadraoSeguros.Facade;
    using Entity = LC.Web.PadraoSeguros.Entity;
    using System.Configuration;

    public partial class regra : System.Web.UI.Page
    {
        string idRegra
        {
            get
            {
                return Request[Util.Keys.IdKey];
            }
        }

        long contratoId
        {
            get
            {
                return CTipos.CToLong(Session["contrId"]);
            }
            set
            {
                Session["contrId"] = value;
            }
        }

        long itemId
        {
            get
            {
                return CTipos.CToLong(Session["itemId"]);
            }
            set
            {
                Session["itemId"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtPercentual.Attributes.Add("onkeyup", "mascara('" + txtPercentual.ClientID + "')");
            txtPercentualExcecao.Attributes.Add("onkeyup", "mascara('" + txtPercentualExcecao.ClientID + "')");

            if (!IsPostBack)
            {
                this.carregarCorretores();
                this.carregaAssociadosPJ();

                if (!string.IsNullOrEmpty(this.idRegra))
                {
                    this.carregaRegra();
                    this.carregaItens();
                    this.carregaContratosDaTabela();
                    this.carregaCorretoresDaTabela();
                }
            }
        }

        void carregaRegra()
        {
            var regra = RegraComissaoFacade.Instance.Carregar(CTipos.CToLong(this.idRegra));

            txtNome.Text = regra.Nome;
            cboAssociadoPJ.SelectedValue = regra.Estipulante.ID.ToString();

            //this.setupTipo();

            //txtDiasAntecedencia.Text = config.DiasAntesVencimento.ToString();
            //txtEmail.Text = config.Email;
            //txtFrequencia.Text = config.Frequencia.ToString();

            chkAtivo.Checked = regra.Ativa;

            cboAssociadoPJ.Enabled = false;
        }

        void carregaItens()
        {
            var itens = RegraComissaoFacade.Instance.CarregarItens(CTipos.CToLong(this.idRegra));
            gridItens.DataSource = itens;
            gridItens.DataBind();

            this.carregarCorretoresExcecao();
        }

        void carregarCorretores()
        {
            List<Corretor> lista = CorretorFacade.Instancia.CarregarTodos(string.Empty);
            cboItemCorretores.Items.Clear();
            cboCorretorExcecao.Items.Clear();
            //cboCorretor.Items.Add(new ListItem("selecione", "-1"));

            if (lista != null)
            {
                foreach (Corretor c in lista)
                {
                    cboItemCorretores.Items.Add(new ListItem(c.Nome, c.ID.ToString()));
                    cboCorretorExcecao.Items.Add(new ListItem(c.Nome, c.ID.ToString()));
                }
            }

            //this.corretores = lista;
        }

        void carregarCorretoresExcecao()
        {
            var itens = RegraComissaoFacade.Instance.CarregarItens(CTipos.CToLong(this.idRegra));
            cboCorretorExcecao.Items.Clear();

            if (itens != null)
            {
                foreach (var c in itens)
                {
                    if (cboCorretorExcecao.Items.FindByValue(c.Corretor.ID.ToString()) != null) continue;

                    cboCorretorExcecao.Items.Add(new ListItem(c.Corretor.Nome, c.Corretor.ID.ToString()));
                }
            }
        }

        void carregaAssociadosPJ()
        {
            List<AssociadoPJ> lista = AssociadoPJFacade.Instance.Carregar(string.Empty);
            cboAssociadoPJ.Items.Clear();

            if (lista != null)
            {
                lista.ForEach(p => cboAssociadoPJ.Items.Add(new ListItem(p.Nome, p.ID.ToString())));
            }
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("regras.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            #region validacao

            if (txtNome.Text.Trim().Length <= 2)
            {
                Geral.Alerta(this, "Informe por favor uma descrição.");
                txtNome.Focus();
                return;
            }

            if (cboAssociadoPJ.Items.Count == 0)
            {
                Geral.Alerta(this, "Informe por favor o associado PJ.");
                cboAssociadoPJ.Focus();
                return;
            }

            //if (cboTipo.SelectedValue == "1" && CTipos.CToInt(txtFrequencia.Text) == 1)
            //{
            //    Geral.Alerta(this, "Informe por favor os dias decorridos do vencimento.");
            //    txtFrequencia.Focus();
            //    return;
            //}

            //if (txtEmail.Text.Trim().Length <= 10)
            //{
            //    Geral.Alerta(this, "Informe por favor o texto do e-mail.");
            //    txtEmail.Focus();
            //    return;
            //}

            #endregion

            RegraComissao regra = null;

            if (string.IsNullOrEmpty(this.idRegra))
            {
                regra = new RegraComissao();
                regra.Estipulante = AssociadoPJFacade.Instance.Carregar(CTipos.CToLong(cboAssociadoPJ.SelectedValue));
            }
            else
            {
                regra = RegraComissaoFacade.Instance.Carregar(CTipos.CToLong(this.idRegra));
                regra.Estipulante = AssociadoPJFacade.Instance.Carregar(regra.Estipulante.ID);
            }

            regra.Ativa = chkAtivo.Checked;
            regra.Nome  = txtNome.Text;

            RegraComissaoFacade.Instance.Salvar(regra);

            Response.Redirect("regras.aspx");
        }

        protected void cboTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.setupTipo();
        }

        void setupTipo()
        {
            //pnlFrequencia.Visible = cboTipo.SelectedValue == "2";
            //pnlDiasAntecedencia.Visible = cboTipo.SelectedValue == "1";
        }

        // Itens ////////////////////

        protected void cmdAddVig_Click(object sender, EventArgs e)
        {
            #region validacao 

            if (string.IsNullOrEmpty(this.idRegra))
            {
                Util.Geral.Alerta(this, "Você deve salvar a tabela primeiramente.");
                return;
            }

            long regraId = Convert.ToInt64(idRegra);

            int? parcela = CTipos.CToIntNullable(txtParcela.Text);
            if (!parcela.HasValue)
            {
                Util.Geral.Alerta(this, "Você deve informar a parcela.");
                return;
            }

            decimal percentual = CTipos.ToDecimal(txtPercentual.Text);
            if (!parcela.HasValue)
            {
                Util.Geral.Alerta(this, "Você deve informar o percentual.");
                return;
            }

            if (cboItemCorretores.Items.Count == 0)
            {
                Util.Geral.Alerta(this, "Nenhum corretor selecionado.");
                return;
            }

            long corretorId = Convert.ToInt64(cboItemCorretores.SelectedValue);

            if (this.itemId == 0)
            {
                bool jaAdicionado = RegraComissaoFacade.Instance.CorretorJaAdicionado(regraId, corretorId, parcela.Value);
                if (jaAdicionado)
                {
                    Util.Geral.Alerta(this, "Corretor ja possui comissionamento para a parcela " + parcela.Value.ToString() + ".");
                    return;
                }
            }
            else
            {
                bool jaAdicionado = RegraComissaoFacade.Instance.CorretorJaAdicionado(regraId, corretorId, parcela.Value, this.itemId);
                if (jaAdicionado)
                {
                    Util.Geral.Alerta(this, "Corretor ja possui comissionamento para a parcela " + parcela.Value.ToString() + ".");
                    return;
                }
            }
            #endregion

            RegraComissaoItem item = null;
            
            if(this.itemId == 0) item = new RegraComissaoItem();
            else item = RegraComissaoFacade.Instance.CarregarItem(this.itemId);

            item.Parcela = parcela;
            item.Percentual = percentual;
            item.RegraID = CTipos.CToLong(this.idRegra);
            item.Vitalicio = chkVitalicio.Checked;
            item.Corretor = CorretorFacade.Instancia.Carregar(Convert.ToInt64(cboItemCorretores.SelectedValue));

            RegraComissaoFacade.Instance.SalvarItem(item);

            this.itemId = 0;
            this.carregaItens();
            chkVitalicio.Checked = false;

            Util.Geral.Alerta(this, "Dados salvos com sucesso.");
        }

        protected void gridVig_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(gridItens, e, 0);
                var item = RegraComissaoFacade.Instance.CarregarItem(id);

                txtParcela.Text         = item.Parcela.ToString();
                txtPercentual.Text      = item.Percentual.ToString("N2");
                item.RegraID            = CTipos.CToLong(this.idRegra);
                chkVitalicio.Checked    = item.Vitalicio;
                cboItemCorretores.SelectedValue = item.Corretor.ID.ToString();

                this.itemId = item.ID;
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(gridItens, e, 0);

                try
                {
                    RegraComissaoFacade.Instance.ExcluirItem(id);

                    this.carregaItens();
                }
                catch
                {
                    Util.Geral.Alerta(null, this, "_err", "Não foi possível remover o item.");
                }
            }
        }

        protected void gridVig_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja excluir o item?\\nEssa operação não poderá ser desfeita.");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                RegraComissaoItem item = e.Row.DataItem as RegraComissaoItem;
                if (item != null)
                {
                    if (item.Vitalicio)
                        ((Literal)e.Row.Cells[3].Controls[1]).Text = "Sim";
                    else
                        ((Literal)e.Row.Cells[3].Controls[1]).Text = "Não";
                }
            }
        }

        #region Corretores /////////////

        void carregaCorretoresDaTabela()
        {
            //gridCorretor.DataSource = RegraComissaoFacade.Instance.CarregarCorretores(Convert.ToInt64(this.idRegra));
            //gridCorretor.DataBind();
        }

        protected void cmdAddCorretor_Click(object sender, EventArgs e)
        {
            //#region validacao 

            //if (string.IsNullOrEmpty(this.idRegra))
            //{
            //    Util.Geral.Alerta(this, "Você deve salvar a tabela primeiramente.");
            //    return;
            //}

            //if (cboCorretor.Items.Count == 0)
            //{
            //    Util.Geral.Alerta(this, "Você deve salvar a tabela primeiramente.");
            //    return;
            //}

            //long regraId = Convert.ToInt64(this.idRegra);
            //long correId = Convert.ToInt64(cboCorretor.SelectedValue);

            //if (RegraComissaoFacade.Instance.ExisteCorretorParaRegra(regraId, correId))
            //{
            //    Util.Geral.Alerta(this, "Corretor ja adicionado à tabela.");
            //    return;
            //}


            //#endregion

            //RegracomCorretor cor = new RegracomCorretor();
            //cor.Corretor         = CorretorFacade.Instancia.Carregar(correId);
            //cor.Regra            = RegraComissaoFacade.Instance.Carregar(regraId);
            //cor.UsuarioId        = Convert.ToInt64(Util.UsuarioLogado.ID);

            //RegraComissaoFacade.Instance.SalvarCorretor(cor);

            //this.carregaCorretoresDaTabela();
        }

        protected void gridCorretor_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            //if (e.CommandName.Equals("Editar"))
            //{
            //}
            //else if (e.CommandName.Equals("Excluir"))
            //{
            //    long id = Util.Geral.ObterDataKeyValDoGrid<long>(gridCorretor, e, 0);

            //    try
            //    {
            //        RegraComissaoFacade.Instance.ExcluirCorretor(id);

            //        this.carregaCorretoresDaTabela();
            //    }
            //    catch
            //    {
            //        Util.Geral.Alerta(null, this, "_err", "Não foi possível remover o corretor.");
            //    }
            //}
        }

        protected void gridCorretor_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja excluir o corretor?");
        }

        #endregion

        #region Contratos /////////////

        void CarregaContratos()
        {
            DataTable lista = null;

            long assocId = 0;
            if (cboAssociadoPJ.SelectedIndex > -1) assocId = Convert.ToInt64(cboAssociadoPJ.SelectedValue);

            //if(txtCartao.Text != "" || txtNome.Text != "")
            lista = Entity.Contrato.DTCarregarPorParametros(txtCartao.Text, txtNomeBeneficiario.Text, "", assocId);

            gridContrato.DataSource = lista;
            gridContrato.DataBind();
        }

        void carregaContratosDaTabela()
        {
            if (string.IsNullOrEmpty(this.idRegra)) return;

            gridContratoAdicionado.DataSource = Entity.Contrato.DTCarregarPorRegraComissionamento(this.idRegra);
            gridContratoAdicionado.DataBind();
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.CarregaContratos();
            if (gridContrato.Rows.Count == 0) 
            { 
                Util.Geral.Alerta(this, "Nenhum contrato localizado. Você selecionou o Associado PJ correto?");
            }
        }

        protected void cmdInserirTodos_Click(object sender, EventArgs e)
        {
            long projeid = Convert.ToInt64(cboAssociadoPJ.SelectedValue);

            List<Contrato> contratos = RegraComissaoFacade.Instance.CarregarContratosDoAssociadoPj(projeid);
            if (contratos == null || contratos.Count == 0)
            {
                Geral.Alerta(this, "Projeto não possui nenhum contrato.");
                return;
            }

            long regraid = Convert.ToInt64(this.idRegra);

            var existentes = RegraComissaoFacade.Instance.CarregarContratos(regraid);
            if (existentes == null) existentes = new List<RegracomContrato>();

            List<RegracomContrato> lote = new List<RegracomContrato>();

            foreach (Contrato contrato in contratos)
            {
                if (!existeNaColecao(existentes, contrato.ID))
                {
                    RegracomContrato novo = new RegracomContrato();
                    novo.Contrato = new Contrato();
                    novo.Contrato.ID = contrato.ID;
                    novo.Regra = new RegraComissao();
                    novo.Regra.ID = regraid;
                    novo.UsuarioId = Convert.ToInt64(Util.UsuarioLogado.ID);

                    lote.Add(novo);
                }
            }

            if (lote.Count > 0)
            {
                RegraComissaoFacade.Instance.SalvarContratoLOTE(lote);
                this.carregaContratosDaTabela();
                Geral.Alerta(this, "Contratos adicionados com sucesso.");
            }
        }

        bool existeNaColecao(List<RegracomContrato> colecao, long contratoId)
        {
            foreach (var item in colecao)
            {
                if (item.Contrato.ID == contratoId) return true;
            }

            return false;
        }

        protected void gridContrato_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Adicionar"))
            {
                #region validacao 

                if (string.IsNullOrEmpty(this.idRegra))
                {
                    Util.Geral.Alerta(this, "Você deve salvar a tabela primeiramente.");
                    return;
                }

                long regraId = Util.CTipos.CToLong(this.idRegra);
                long contrId = Util.Geral.ObterDataKeyValDoGrid<long>(gridContrato, e, 0);

                if (RegraComissaoFacade.Instance.ExisteContratoParaRegra(regraId, contrId))
                {
                    Util.Geral.Alerta(this, "Contrato ja adicionado à tabela.");
                    return;
                }

                #endregion

                RegracomContrato rc = new RegracomContrato();
                rc.Contrato = RegraComissaoFacade.Instance.CarregarContrato(contrId);
                rc.Regra = RegraComissaoFacade.Instance.Carregar(regraId);
                rc.UsuarioId = Convert.ToInt64(Util.UsuarioLogado.ID);

                RegraComissaoFacade.Instance.SalvarContrato(rc);

                this.carregaContratosDaTabela();

            }
            else if (e.CommandName.Equals("Editar"))
            {
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                //long id = Util.Geral.ObterDataKeyValDoGrid<long>(gridCorretor, e, 0);

                //try
                //{
                //    RegraComissaoFacade.Instance.ExcluirCorretor(id);

                //    this.carregaCorretoresDaTabela();
                //}
                //catch
                //{
                //    Util.Geral.Alerta(null, this, "_err", "Não foi possível remover o corretor.");
                //}
            }
        }

        protected void gridContrato_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja adicionar o contrato?");
        }

        //

        protected void gridContratoAdicionado_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Excluir"))
            {
                long contratoid = Util.Geral.ObterDataKeyValDoGrid<long>(gridContratoAdicionado, e, 0);

                try
                {
                    long regraid = Convert.ToInt64(this.idRegra);
                    RegraComissaoFacade.Instance.ExcluirContrato(regraid, contratoid);

                    this.carregaContratosDaTabela();
                }
                catch
                {
                    Util.Geral.Alerta(null, this, "_err", "Não foi possível remover o contrato.");
                }

                this.carregaContratosDaTabela();
            }
            else if (e.CommandName.Equals("Excecao"))
            {
                this.contratoId = Convert.ToInt64(e.CommandArgument);
                Entity.Contrato.DTCarregarPorRegraComissionamento(this.idRegra);
                var ret = Entity.ContratoBeneficiario.CarregarTitular(this.contratoId, null);

                litContratoExcecao.Text = string.Concat(ret.BeneficiarioNome, "<br>");
                pnlExcecao.Visible = true;
                this.carregarItensDeExcecao();
            }
        }

        protected void gridContratoAdicionado_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja excluir o contrato?");
        }

        #endregion

        // Itens Excecao ////////////////////

        void carregarItensDeExcecao()
        {
            gridItensExcecao.DataSource = RegraComissaoFacade.Instance.
                CarregarItensExcecao(Convert.ToInt64(this.idRegra), this.contratoId);
            gridItensExcecao.DataBind();
        }

        protected void cmdAddParcelaExcecao_Click(object sender, EventArgs e)
        {
            #region validacao

            int? parcela = null;
            decimal percentual = 0;

            if (string.IsNullOrEmpty(this.idRegra))
            {
                Util.Geral.Alerta(this, "Você deve salvar a tabela primeiramente.");
                return;
            }

            long regraId = Convert.ToInt64(idRegra);

            if (!optNaoComissionadoExcecao.Checked)
            {
                parcela = CTipos.CToIntNullable(txtParcelaExcecao.Text);
                if (!parcela.HasValue || parcela.Value < 1)
                {
                    Util.Geral.Alerta(this, "Você deve informar a parcela.");
                    return;
                }

                percentual = CTipos.ToDecimal(txtPercentualExcecao.Text);
                if (!parcela.HasValue || percentual <= 0)
                {
                    Util.Geral.Alerta(this, "Você deve informar o percentual.");
                    return;
                }
            }
            else
                parcela = 0;

            if (this.contratoId == 0)
            {
                Util.Geral.Alerta(this, "Nenhum contrato selecionado.");
                return;
            }

            if (cboCorretorExcecao.Items.Count == 0)
            {
                Util.Geral.Alerta(this, "Nenhum corretor selecionado.");
                return;
            }

            long corretorId = Convert.ToInt64(cboCorretorExcecao.SelectedValue);

            //if (!optNaoComissionadoExcecao.Checked)
            //{
                bool jaAdicionado = RegraComissaoFacade.Instance.ParcelaJaAdicionada(regraId, this.contratoId, corretorId, parcela.Value);
                if (jaAdicionado)
                {
                    Util.Geral.Alerta(this, "A parcela " + parcela.Value.ToString() + " ja foi adicionada para esse corretor.");
                    return;
                }
            //}
            //else
            //{
                //verifica se ja nao tem uma instrucao para nao haver comissionamento
                jaAdicionado = RegraComissaoFacade.Instance.NaoComissionamentoJaAdicionado(regraId, this.contratoId, corretorId);
                if (jaAdicionado)
                {
                    Util.Geral.Alerta(this, "Já existe instrução para não comissionar esse corretor.");
                    return;
                }
            //}
            #endregion

            RegraComissaoItemExcecao item = new RegraComissaoItemExcecao();
            item.Parcela = parcela.Value;
            item.Percentual = percentual;
            item.RegraID = CTipos.CToLong(this.idRegra);
            item.Vitalicio = chkVitalicioExcecao.Checked;
            item.ContratoID = this.contratoId;
            item.Corretor = CorretorFacade.Instancia.Carregar(corretorId);
            item.NaoComissionado = optNaoComissionadoExcecao.Checked; //chkNaoComissionadoExcecao.Checked;

            RegraComissaoFacade.Instance.SalvarItemExcecao(item);

            this.carregarItensDeExcecao();
            chkVitalicioExcecao.Checked = false;

            optComissionadoExcecao.Checked = true;
            optNaoComissionadoExcecao.Checked = false;
            opt_changed(null, null);
        }

        protected void opt_changed(object sender, EventArgs e)
        {
            if (optNaoComissionadoExcecao.Checked)
                pnlComissionavel.Visible = false;
            else
                pnlComissionavel.Visible = true;
        }

        protected void gridItemExcecao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(gridItensExcecao, e, 0);

                try
                {
                    RegraComissaoFacade.Instance.ExcluirItemExcecao(id);

                    this.carregarItensDeExcecao();
                }
                catch
                {
                    Util.Geral.Alerta(null, this, "_err", "Não foi possível remover o item.");
                }
            }
        }

        protected void gridItemExcecao_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja excluir o item?\\nEssa operação não poderá ser desfeita.");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                RegraComissaoItemExcecao item = e.Row.DataItem as RegraComissaoItemExcecao;
                if (item != null)
                {
                    if (item.Vitalicio)
                        ((Literal)e.Row.Cells[3].Controls[1]).Text = "Sim";
                    else
                        ((Literal)e.Row.Cells[3].Controls[1]).Text = "Não";

                    if (item.NaoComissionado)
                    {
                        e.Row.ForeColor = System.Drawing.Color.Red;
                        e.Row.ToolTip = "não comissionado";
                    }
                }
            }
        }

        protected void cmdSairItensExecao_Click(object sender, EventArgs e)
        {
            pnlExcecao.Visible = false;
            this.contratoId = 0;
        }
    }
}