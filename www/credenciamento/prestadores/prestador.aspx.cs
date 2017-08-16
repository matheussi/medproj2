using System;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using MedProj.Entidades;
using MedProj.www.Util;
using MedProj.Entidades.Enuns;
using LC.Web.PadraoSeguros.Facade;

namespace MedProj.www.credenciamento.prestadores
{
    public partial class prestador : System.Web.UI.Page
    {
        long idUnidadeSel
        {
            get { return CTipos.CToLong(ViewState["_unSel"]); }
            set { ViewState["_unSel"] = value; }
        }

        List<UnidadeProcedimento> ProcedimentosAdicionados
        {
            get
            {
                return Session["procsAd"] as List<UnidadeProcedimento>;
            }
            set
            {
                if (value == null) { Session.Remove("procsAd"); return; }

                Session["procsAd"] = value;
            }
        }

        List<TabelaPreco> TabelasPreco
        {
            get
            {
                return Session["tabe"] as List<TabelaPreco>;
            }
            set
            {
                if (value == null) { Session.Remove("tabe"); return; }

                Session["tabe"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.TabelasPreco = null;
                this.ProcedimentosAdicionados = null;

                this.carregaRegioes();
                this.carregaSegmentos();
                this.carregaTabelasDePreco();
                //this.carregaEspecialidades();

                if (this.TabelasPreco == null || this.TabelasPreco == null)
                {
                    Geral.Alerta(this, "Nenhuma tabela de preço cadastrada no sistema.");
                    cmdContratos.Visible = false;
                    //cmdEspecialidades.Visible = false;
                    cmdDadosBancarios.Visible = false;
                    cmdDadosAcesso.Visible = false;
                }

                if (Geral.IdEnviado(this.Context, Keys.IdKey) > 0)
                {
                    cboSegmento.Enabled = false;

                    this.carregaPrestador();
                    this.carregaTabelas();
                    this.carregaEspecialidadesDaTabela();
                    this.carregaCategoriasDaTabela();
                    this.carregaProcedimentosDaTabela2(null);

                    this.carregarUnidadeEspecialidades();

                    if (!string.IsNullOrEmpty(Request["msg"]))
                    {
                        Geral.Alerta(this, "Prestador salvo com sucesso.");
                    }
                }

                this.carregaEspecialidades();
            }
        }

        void carregaPrestador()
        {
            long id = Geral.IdEnviado(this.Context, Keys.IdKey);

            cboSegmento.Enabled = false;

            Prestador prestador = PrestadorFacade.Instancia.CarregarPorId(id); 

            txtNomePrestador.Text = prestador.Nome;
            txtObs.Text = prestador.Observacoes;

            if (prestador.Segmento != null && prestador.Segmento.ID > 0)
            {
                cboSegmento.SelectedValue = prestador.Segmento.ID.ToString();

                if (!prestador.Segmento.Detalhamento)
                {
                    pnlProcedimentosDetalhamento.Visible = false; //esconde preços e procedimentos para certos prestadores, pois eles não usam
                }
            }


            List<PrestadorUnidade> lista = PrestadorUnidadeFacade.Instancia.CarregaPorPrestadorId(prestador.ID);

            gridContratos.DataSource = lista;
            gridContratos.DataBind();

            this.preencheComboDeContratos(lista);

            pnlBotoes.Visible = true;
        }

        void preencheComboDeContratos(List<PrestadorUnidade> lista)
        {
            cboContrato.Items.Clear();
            cboContrato_Acesso.Items.Clear();
            cboContrato_Bancarios.Items.Clear();

            if (lista == null) return;

            foreach (PrestadorUnidade obj in lista)
            {
                cboContrato.Items.Add(new ListItem(string.Concat(obj.Nome, " (", obj.Endereco, " - ", obj.Cidade, ")"), obj.ID.ToString()));
                cboContrato_Acesso.Items.Add(new ListItem(string.Concat(obj.Nome, " (", obj.Endereco, " - ", obj.Cidade, ")"), obj.ID.ToString()));
                cboContrato_Bancarios.Items.Add(new ListItem(string.Concat(obj.Nome, " (", obj.Endereco, " - ", obj.Cidade, ")"), obj.ID.ToString()));
            }
        }

        void carregaRegioes()
        {
        }

        void carregaTabelasDePreco()
        {
            List<TabelaPreco> tabelas = TabelaPrecoFacade.Instancia.CarregarComVigencia(string.Empty);

            this.TabelasPreco = tabelas;
            cboTabelaPreco.DataSource = tabelas;
            cboTabelaPreco.DataBind();
        }

        void carregaSegmentos()
        {
            long id = Convert.ToInt64(Request[Keys.IdKey]);

            cboSegmento.DataValueField = "ID";
            cboSegmento.DataTextField = "Nome";

            cboSegmento.DataSource = SegmentoFacade.Instancia.CarregarTodos(null);
            cboSegmento.DataBind();
        }

        void carregaTabelas()
        {
            long idSegmento = Convert.ToInt64(cboSegmento.SelectedValue);

            cboTabelaProcedimentos.Items.Clear();

            List<TabelaProcedimento> lista = TabelaProcedimentoFacade.Instancia.Carregar();  //CarregarPorSegmento(idSegmento);

            if (lista != null)
            {
                foreach (TabelaProcedimento t in lista)
                {
                    ListItem item = new ListItem(t.Nome, t.ID.ToString());
                    if (!t.Ativa) item.Attributes.Add("style", "color:red");

                    cboTabelaProcedimentos.Items.Add(item);
                }
            }

            this.carregaProcedimentosDaTabela();
        }

        void carregaProcedimentosDaTabela()
        {
            //long tabelaId = Convert.ToInt64(cboTabelaProcedimentos.SelectedValue);

            //chklProcedimento.DataSource = ProcedimentoFacade.Instancia.CarregarPorTabela(tabelaId, null);

            //chklProcedimento.DataBind();
        }

        void carregaEspecialidades()
        {
            cboEspecialidade.DataTextField = "Nome";
            cboEspecialidade.DataValueField = "ID";
            cboEspecialidade.DataSource = EspecialidadeFacade.Instance.Carregar(null, CTipos.CTipo<long>(cboSegmento.SelectedValue));
            cboEspecialidade.DataBind();
        }

        protected void cboTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTipo.SelectedIndex == 0)
            {
                litNome.Text = "Razão social";
                litDocumento.Text = "CNPJ";
            }
            else
            {
                litNome.Text = "Nome";
                litDocumento.Text = "CPF";
            }
        }

        #region pnlBotoes 

        protected void cmdContratos_Click(object sender, EventArgs e)
        {
            pnlAcesso.Visible = false;
            pnlEspecialidades.Visible = false;
            pnlDadosBancarios.Visible = false;
            pnlContratos.Visible = !pnlContratos.Visible;
        }

        protected void cmdEspecialidades_Click(object sender, EventArgs e)
        {
            pnlAcesso.Visible = false;
            pnlContratos.Visible = false;
            pnlDadosBancarios.Visible = false;
            pnlEspecialidades.Visible = !pnlEspecialidades.Visible;
        }

        protected void cmdDadosBancarios_Click(object sender, EventArgs e)
        {
            pnlAcesso.Visible = false;
            pnlContratos.Visible = false;
            pnlDadosBancarios.Visible = !pnlDadosBancarios.Visible;
            pnlEspecialidades.Visible = false;

            if (pnlDadosBancarios.Visible) cboContrato_Bancarios_SelectedIndexChanged(null, null);
        }

        protected void cmdDadosAcesso_Click(object sender, EventArgs e)
        {
            pnlAcesso.Visible = !pnlAcesso.Visible;
            pnlContratos.Visible = false;
            pnlDadosBancarios.Visible = false;
            pnlEspecialidades.Visible = false;

            if (pnlAcesso.Visible) cboContrato_Acesso_SelectedIndexChanged(null, null);
        }

        #endregion

        #region Salvar e Voltar 

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("prestadores.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (txtNomePrestador.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome do prestador.");
                txtNomePrestador.Focus();
                return;
            }

            if (cboSegmento.Items.Count == 0)
            {
                Geral.Alerta(this, "Informe o segmento da tabela.");
                cboSegmento.Focus();
                return;
            }

            Prestador prestador = null;
            long id = Geral.IdEnviado(this.Context, Keys.IdKey);
            long idSeg = Convert.ToInt64(cboSegmento.SelectedValue);

                if (id != 0)
                {
                    prestador = PrestadorFacade.Instancia.CarregarPorId(id); // contexto.Prestadores.Where(p => p.ID == id).Single();
                }
                else
                {
                    prestador = new Prestador();
                }

                prestador.Nome = txtNomePrestador.Text;
                prestador.Segmento = SegmentoFacade.Instancia.Carregar(idSeg); // contexto.Segmentos.Where(s => s.ID == idSeg).Single();
                prestador.Observacoes = txtObs.Text;

                //if (id == 0) contexto.Prestadores.Add(prestador);
                PrestadorFacade.Instancia.Salvar(prestador);


            if (id == 0) Response.Redirect("prestador.aspx?msg=1&" + Keys.IdKey + "=" + prestador.ID.ToString());
            else Geral.Alerta(this, "Prestador salvo com sucesso.");
        }

        #endregion

        #region contratos 

        void limpaCampos()
        {
            cboTabelaProcedimentos.Enabled = true;

            txtNome.Text = "";
            txtDocumento.Text = "";
            txtEmail.Text = "";
            txtCel.Text = "";
            txtFone.Text = "";

            for (int i = 0; i < chklProcedimento.Items.Count; i++) chklProcedimento.Items[i].Selected = false;

            txtCEP.Text = "";
            txtEndereco.Text = "";
            txtNumero.Text = "";
            txtComplemento.Text = "";
            txtBairro.Text = "";
            txtCidade.Text = "";
            txtEstado.Text = "";
            txtContratoObservacoes.Text = "";

            txtLatitude.Text = "";
            txtLongitude.Text = "";

            this.ProcedimentosAdicionados = null;

            gridProcedimentosAdicionados.DataSource = null;
            gridProcedimentosAdicionados.DataBind();
            pnlProcedimentosAdicionados.Visible = false;
        }

        void carregarUnidades()
        {
            long pid = Geral.IdEnviado(this.Context, Keys.IdKey);

            List<PrestadorUnidade> lista = PrestadorUnidadeFacade.Instancia.CarregaPorPrestadorId(pid);

            gridContratos.DataSource = lista;
            gridContratos.DataBind();

            this.preencheComboDeContratos(lista);
        }

        void carregaEspecialidadesDaTabela()
        {
            cboEspecialidadePro.Items.Clear();
            if (cboTabelaProcedimentos.Items.Count <= 0) { return; }

            long tabelaId = Convert.ToInt64(cboTabelaProcedimentos.SelectedValue);

            string[] especialidades = ProcedimentoFacade.Instancia.CarregarEspecialidades(tabelaId);

            if (especialidades != null)
            {
                foreach (string especialidade in especialidades)
                {
                    cboEspecialidadePro.Items.Add(new ListItem(especialidade, especialidade));
                }
            }
        }
        void carregaCategoriasDaTabela()
        {
            cboCategoria.Items.Clear();
            if (cboTabelaProcedimentos.Items.Count <= 0) { return; }

            long tabelaId = Convert.ToInt64(cboTabelaProcedimentos.SelectedValue);

            string[] categorias = ProcedimentoFacade.Instancia.CarregarCategorias(tabelaId, cboEspecialidadePro.SelectedValue);

            if (categorias != null)
            {
                foreach (string categoria in categorias)
                {
                    cboCategoria.Items.Add(new ListItem(categoria, categoria));
                }
            }
        }
        void carregaProcedimentosDaTabela2(string param)
        {
            gridProcedimentosParaAdd.DataSource = null;

            if (string.IsNullOrWhiteSpace(param) && 
                cboTabelaProcedimentos.Items.Count > 0 && cboEspecialidadePro.Items.Count > 0 && cboCategoria.Items.Count > 0)
            {
                #region 
                long tabelaId = Convert.ToInt64(cboTabelaProcedimentos.SelectedValue);

                List<Procedimento> procedimentos = ProcedimentoFacade.Instancia.
                    CarregarPorTabela(tabelaId, cboEspecialidadePro.SelectedValue, cboCategoria.SelectedValue);

                if (procedimentos != null)
                {
                    TabelaPreco tabela = TabelaPrecoFacade.Instancia.Carregar(Util.CTipos.CTipo<long>(cboTabelaPreco.SelectedValue));
                    TabelaPrecoVigencia vigencia = tabela.Vigencias.Where(tp => tp.Ativa == true).OrderByDescending(tp => tp.DataInicio).FirstOrDefault();

                    procedimentos.ForEach(delegate(Procedimento p)
                    {
                        if (vigencia != null)
                        {
                            p.CH = p.CH * vigencia.ValorReal;
                        }
                        else
                        {
                            p.CH = decimal.Zero;
                        }
                    });
                }

                gridProcedimentosParaAdd.DataSource = procedimentos;
                #endregion
            }
            else if (!string.IsNullOrWhiteSpace(param))
            {
                List<Procedimento> procedimentos = ProcedimentoFacade.Instancia.Carregar(param);

                if (procedimentos != null)
                {
                    TabelaPreco tabela = TabelaPrecoFacade.Instancia.Carregar(Util.CTipos.CTipo<long>(cboTabelaPreco.SelectedValue));
                    TabelaPrecoVigencia vigencia = tabela.Vigencias.Where(tp => tp.Ativa == true).OrderByDescending(tp => tp.DataInicio).FirstOrDefault();

                    procedimentos.ForEach(delegate(Procedimento p)
                    {
                        if (vigencia != null)
                        {
                            p.CH = p.CH * vigencia.ValorReal;
                        }
                        else
                        {
                            p.CH = decimal.Zero;
                        }
                    });
                }

                gridProcedimentosParaAdd.DataSource = procedimentos;
            }

            gridProcedimentosParaAdd.DataBind();
        }

        protected void cmdPesquisarProcedimento_Click(object sender, EventArgs e)
        {
            if (cboTabelaPreco.Items.Count == 0)
            {
                Util.Geral.Alerta(this, "Nenhuma tabela de preço selecionada.");
                return;
            }

            this.carregaProcedimentosDaTabela2(txtPesquisarProcedimento.Text);
        }

        protected void cboTabelaPreco_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregaEspecialidades();
            this.carregaCategoriasDaTabela();
            this.carregaProcedimentosDaTabela2(null);
        }
        protected void cboTabelaProcedimentos_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregaEspecialidadesDaTabela();
            this.carregaCategoriasDaTabela();
            this.carregaProcedimentosDaTabela2(null);
        }
        protected void cboEspecialidadePro_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregaCategoriasDaTabela();
            this.carregaProcedimentosDaTabela2(null);
        }
        protected void cboCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregaProcedimentosDaTabela2(null);
        }

        //--//
        protected void chkChecHeader_CheckedChanged(object sender, EventArgs e)
        {

        }
        //--//

        //- GRID gridProcedimentosParaAdd -//
        protected void gridProcedimentosParaAdd_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList cbo = e.Row.Cells[3].FindControl("cboTabela") as DropDownList;

                cbo.Items.Clear();

                if (this.TabelasPreco == null) return;

                this.TabelasPreco.ForEach(delegate(TabelaPreco t)
                {
                    cbo.Items.Add(new ListItem(t.Nome, t.ID.ToString()));
                });

                cbo.SelectedValue = cboTabelaPreco.SelectedValue;
            }
        }

        protected void gridProcedimentosParaAdd_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                TextBox txt = e.Row.Cells[4].FindControl("txtValor") as TextBox;
                txt.Attributes.Add("onKeyUp", "mascara('" + txt.ClientID + "')");
                txt.Attributes.Add("onkeypress", "return filtro_SoNumeros(event);");
            }
        }

        protected void cboTabela_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList cbo = (DropDownList)sender;

            GridViewRow row = cbo.NamingContainer as GridViewRow;

            TextBox txt = row.Cells[3].FindControl("txtValor") as TextBox;

            long idProc = Util.Geral.ObterDataKeyValDoGrid<long>(gridProcedimentosParaAdd, null, row.RowIndex);
            this.calculaPrecoDaLinha(txt, cbo, cbo.SelectedValue, idProc);
        }

        protected void imgCadeado_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton btn = sender as ImageButton;
            GridViewRow row = btn.NamingContainer as GridViewRow;
            TextBox txt = row.Cells[3].FindControl("txtValor") as TextBox;
            DropDownList cbo = row.Cells[2].FindControl("cboTabela") as DropDownList;

            if (btn.ImageUrl.ToLower().IndexOf("aberto") > -1)
            {
                btn.ImageUrl = "~/Images/cadeado.png";
                txt.ReadOnly = true;
                cbo.Enabled = true;

                long idProc = Util.Geral.ObterDataKeyValDoGrid<long>(gridProcedimentosParaAdd, null, row.RowIndex);
                this.calculaPrecoDaLinha(txt, cbo, cbo.SelectedValue, idProc);
            }
            else
            {
                btn.ImageUrl = "~/Images/cadeado_aberto.png";
                txt.ReadOnly = false;
                cbo.Enabled = false;
            }
        }

        void calculaPrecoDaLinha(TextBox txt, DropDownList cboTabela, string idTabela, long idProcedimento)
        {
            Procedimento procedimento = ProcedimentoFacade.Instancia.Carregar(idProcedimento);

            TabelaPreco tabela = TabelaPrecoFacade.Instancia.Carregar(Util.CTipos.CTipo<long>(cboTabela.SelectedValue));
            TabelaPrecoVigencia vigencia = tabela.Vigencias.Where(tp => tp.Ativa == true).OrderByDescending(tp => tp.DataInicio).FirstOrDefault();

            if (vigencia == null)
                txt.Text = "0,00";
            else
                txt.Text = (vigencia.ValorReal * procedimento.CH).ToString("N2");
        }
        //--///////////////////////////////////////////////////////////////////////////////////////////////////////////

        //- ADICIONAR PROCEDIMENTOS -//
        protected void cmdAddProcedimento_Click(object sender, EventArgs e)
        {
            long id = 0;
            CheckBox chk = null;
            TabelaPreco tabela = null;
            Procedimento procedimento = null;

            TabelaPrecoVigencia vigencia = null;

            TextBox txt = null;
            ImageButton btn = null;
            GridViewRow row = null;
            DropDownList cbo = null;

            bool marcado = false;

            for (int i = 0; i < gridProcedimentosParaAdd.Rows.Count; i++)
            {
                row = gridProcedimentosParaAdd.Rows[i];

                chk = row.Cells[0].FindControl("chkChec") as CheckBox;

                if (chk.Checked)
                {
                    marcado = true;

                    tabela = null;

                    cbo = row.Cells[3].FindControl("cboTabela") as DropDownList;
                    btn = row.Cells[5].FindControl("imgCadeado") as ImageButton;

                    id = Util.Geral.ObterDataKeyValDoGrid<long>(gridProcedimentosParaAdd, null, i);
                    procedimento = ProcedimentoFacade.Instancia.Carregar(id);

                    if (btn.ImageUrl.ToLower().IndexOf("aberto") > -1)
                    {
                        //solicitacao
                        txt = row.Cells[3].FindControl("txtValor") as TextBox;

                        //if (Util.CTipos.CTipo<decimal>(txt.Text) == decimal.Zero)
                        //{
                        //    Geral.Alerta(this, "Há procedimento(s) sem valor atribuído.");
                        //    return;
                        //}
                    }
                    else
                    {
                        tabela = TabelasPreco.Find(t => t.ID == Util.CTipos.CTipo<long>(cbo.SelectedValue));
                    }

                    if (this.ProcedimentosAdicionados == null)
                        this.ProcedimentosAdicionados = new List<UnidadeProcedimento>();
                    else
                    {
                        //checa se ja está adicionado
                        if (this.ProcedimentosAdicionados.FindIndex(pr => pr.Procedimento.ID == id) != -1) continue;
                    }

                    UnidadeProcedimento up = new UnidadeProcedimento();
                    up.Procedimento = procedimento;

                    if (tabela != null)
                    {
                        up.TabelaPreco = tabela;
                        vigencia = tabela.Vigencias.Where(tp => tp.Ativa == true).OrderByDescending(tp => tp.DataInicio).FirstOrDefault();

                        if (vigencia == null || vigencia.ValorReal == decimal.Zero)
                        {
                            Geral.Alerta(this, "Há procedimento(s) sem valor atribuído.");
                            return;
                        }

                        up.ValorSobrescrito = procedimento.CH * vigencia.ValorReal;
                    }
                    else
                    {
                        //solicitacao
                        up.ValorSobrescrito = Util.CTipos.ToDecimal(txt.Text);
                        //if (up.ValorSobrescrito.Value == decimal.Zero)
                        //{
                        //    Geral.Alerta(this, "Há procedimento(s) sem valor atribuído.");
                        //    return;
                        //}
                    }

                    this.ProcedimentosAdicionados.Add(up);
                }
            }

            this.exibirProcedimentosAdicionados();

            if (!marcado)
            {
                Geral.Alerta(this, "Selecione ao menos um procedimento.");
            }
        }

        void exibirProcedimentosAdicionados()
        {
            gridProcedimentosAdicionados.DataSource = this.ProcedimentosAdicionados;
            gridProcedimentosAdicionados.DataBind();

            if (gridProcedimentosAdicionados.Rows.Count == 0)
                pnlProcedimentosAdicionados.Visible = false;
            else
                pnlProcedimentosAdicionados.Visible = true;
        }

        protected void gridProcedimentosAdicionados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 6, "Deseja realmente excluir o procedimento?");
        }

        protected void gridProcedimentosAdicionados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Excluir"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(gridProcedimentosAdicionados, e, 0);
                ProcedimentoFacade.Instancia.ExcluirUnidadeProcedimento(id);

                for (int i = 0; i < this.ProcedimentosAdicionados.Count; i++)
                {
                    if (this.ProcedimentosAdicionados[i].ID == id)
                    {
                        this.ProcedimentosAdicionados.RemoveAt(i);
                        break;
                    }
                }

                this.exibirProcedimentosAdicionados();

                //int index = Util.CTipos.CToInt(e.CommandArgument);
                //UnidadeProcedimento up = this.ProcedimentosAdicionados[index];

                //if (up.ID > 0)
                //{
                //    ProcedimentoFacade.Instancia.ExcluirUnidadeProcedimento(up.ID);
                //}

                //this.ProcedimentosAdicionados.RemoveAt(index);
                //this.exibirProcedimentosAdicionados();

                Geral.Alerta(this, "Procedimento excluído.");
            }
        }
        //--///////////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void cmdCEP_Click(object sender, EventArgs e)
        {
            Geral.ConsultaCEP(txtCEP.Text, txtEndereco, txtBairro, txtCidade, txtEstado);
        }

        protected void chklProcedimento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chklProcedimento.SelectedItem != null)
            {
            }
        }


        protected void cmdLocalizarProcedimentoAdicionado_Click(object sender, EventArgs e)
        {
            if (this.ProcedimentosAdicionados == null) return;

            if (txtLocalizarProcedimentoAdicionado.Text.Trim().Length > 0)
            {
                List<UnidadeProcedimento> res = new List<UnidadeProcedimento>();
                foreach (UnidadeProcedimento up in this.ProcedimentosAdicionados)
                {
                    if (up.Procedimento.Codigo.ToString().Contains(txtLocalizarProcedimentoAdicionado.Text) ||
                        up.Procedimento.Nome.ToLower().Contains(txtLocalizarProcedimentoAdicionado.Text.ToLower()))
                    {
                        res.Add(up);
                    }
                }

                gridProcedimentosAdicionados.DataSource = res;
                gridProcedimentosAdicionados.DataBind();
            }
            else
            {
                this.exibirProcedimentosAdicionados();
            }
        }

        protected void cmdExibirTodosProcedimentoAdicionado_Click(object sender, EventArgs e)
        {
            this.exibirProcedimentosAdicionados();
        }

        //- GRID DE CONTRATOS ADICIONADOS -//
        protected void gridContratos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                #region
                long uid = Geral.ObterDataKeyValDoGrid<long>(gridContratos, e, 0);

                PrestadorUnidade unidade = PrestadorUnidadeFacade.Instancia.CarregaPorId(uid);
                unidade.Procedimentos = ProcedimentoFacade.Instancia.CarregarProcedimentosDaUnidade(uid);

                List<UnidadeProcedimento> procedimentos = new List<UnidadeProcedimento>();
                procedimentos.AddRange(unidade.Procedimentos);

                this.idUnidadeSel = unidade.ID;

                cboTipo.SelectedIndex = unidade.Tipo == TipoPessoa.Fisica ? 1 : 0;

                txtBairro.Text = unidade.Bairro;
                txtCel.Text = unidade.Celular;
                txtCEP.Text = unidade.CEP;
                txtCidade.Text = unidade.Cidade;
                txtComplemento.Text = unidade.Complemento;
                txtDocumento.Text = unidade.Documento;
                txtEmail.Text = unidade.Email;
                txtEndereco.Text = unidade.Endereco;
                txtEstado.Text = unidade.UF;
                txtFone.Text = unidade.Telefone;
                txtNome.Text = unidade.Nome;
                txtNumero.Text = unidade.Numero;

                txtContratoObservacoes.Text = unidade.Observacoes;

                //if (unidade.Longitude != null && unidade.Latitude != null)
                //{
                    txtLatitude.Text = unidade.Latitude.ToString();
                    txtLongitude.Text = unidade.Longitude.ToString();
                //}

                cboPagamento.SelectedValue = Convert.ToInt32(unidade.TipoPagto).ToString();

                if (unidade.TabelaPreco != null && unidade.TabelaPreco.ID > 0)
                    cboTabelaPreco.SelectedValue = unidade.TabelaPreco.ID.ToString();

                cboTabelaProcedimentos.SelectedValue = unidade.Tabela.ID.ToString();
                cboTabelaProcedimentos_SelectedIndexChanged(null, null);
                //cboTabelaProcedimentos.Enabled = false;

                //Procedimentos

                this.ProcedimentosAdicionados = null;

                if (unidade.Procedimentos != null && unidade.Procedimentos.Count > 0)
                {
                    this.ProcedimentosAdicionados = new List<UnidadeProcedimento>();
                    this.ProcedimentosAdicionados.AddRange(unidade.Procedimentos);
                }

                this.exibirProcedimentosAdicionados();

                //this.carregaProcedimentosDaTabela();

                //if (unidade.Procedimentos != null && unidade.Procedimentos.Count > 0) //if (procedimentos != null && procedimentos.Count > 0)
                //{
                //    long aux = 0;
                //    for (int i = 0; i < chklProcedimento.Items.Count; i++)
                //    {
                //        aux = Util.CTipos.CTipo<long>(chklProcedimento.Items[i].Value);

                //        if (procedimentos.FindIndex(p => p.Procedimento.ID == aux) > -1)
                //        {
                //            chklProcedimento.Items[i].Selected = true;
                //        }
                //    }
                //}
                #endregion
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long uid = Geral.ObterDataKeyValDoGrid<long>(gridContratos, e, 0);
                try
                {
                    PrestadorUnidadeFacade.Instancia.Excluir(uid);
                    this.limpaCampos();
                    this.idUnidadeSel = 0;
                    this.carregarUnidades();

                    Util.Geral.Alerta(this, "Contrato removido com sucesso.");
                }
                catch
                {
                    Util.Geral.Alerta(this, "Não foi possível remover o contrato. Talvez ele possua atendimentos.");
                }
            }
        }

        protected void gridContratos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 5, "Deseja realmente excluir o contrato?");
        }
        //--///////////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void cmdAddContratos_Click(object sender, EventArgs e)
        {
            #region validacoes

            if (txtNome.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome.");
                txtNome.Focus();
                return;
            }

            //if (txtEmail.Text.Trim() == "")
            //{
            //    Geral.Alerta(this, "Informe o e-mail.");
            //    txtEmail.Focus();
            //    return;
            //}

            if (cboTabelaPreco.Items.Count == 0)
            {
                Geral.Alerta(this, "Selecione uma tabela de preços.");
                cboTabelaPreco.Focus();
                return;
            }

            //TODO: validar e-mail e se o endereço de email ja existe para outro contrato

            if (cboTabelaProcedimentos.Items.Count == 0)
            {
                Geral.Alerta(this, "Selecione uma tabela de procedimentos.");
                cboTabelaProcedimentos.Focus();
                return;
            }

            #endregion

            PrestadorUnidade unidade = null;
            long idPrestador = Geral.IdEnviado(this.Context, Keys.IdKey);

            if (this.idUnidadeSel == 0) unidade = new PrestadorUnidade();
            else
            {
                unidade = PrestadorUnidadeFacade.Instancia.CarregaPorId(this.idUnidadeSel);
                //unidade.Procedimentos = ProcedimentoFacade.Instancia.CarregarProcedimentosDaUnidade(this.idUnidadeSel);
            }

            unidade.Celular = txtCel.Text;
            unidade.Documento = txtDocumento.Text;
            unidade.Email = txtEmail.Text;
            unidade.Nome = txtNome.Text;

            unidade.CEP = txtCEP.Text;
            unidade.Endereco = txtEndereco.Text;
            unidade.Numero = txtNumero.Text;
            unidade.Complemento = txtComplemento.Text;
            unidade.Bairro = txtBairro.Text;
            unidade.Cidade = txtCidade.Text;
            unidade.UF = txtEstado.Text;

            unidade.Observacoes = txtContratoObservacoes.Text;

            if (this.idUnidadeSel == 0) unidade.Owner = PrestadorFacade.Instancia.CarregarPorId(idPrestador); //contexto.Prestadores.Include("Segmento").Where(p => p.ID == idPrestador).Single();
            //if (idRegiao > 0) unidade.Regiao = contexto.Regioes.Where(r => r.ID == idRegiao).Single();
            unidade.Regiao = null;

            long tabelaId = Util.CTipos.CTipo<long>(cboTabelaProcedimentos.SelectedValue);
            unidade.Tabela = TabelaProcedimentoFacade.Instancia.Carregar(tabelaId); //contexto.TabelasProcedimento.Include("Segmento").Where(t => t.ID == tabelaId).Single();

            tabelaId = Util.CTipos.CTipo<long>(cboTabelaPreco.SelectedValue);
            unidade.TabelaPreco = TabelaPrecoFacade.Instancia.Carregar(tabelaId); // contexto.TabelasPreco.Where(t => t.ID == tabelaId).Single();

            unidade.Telefone = txtFone.Text;
            unidade.Tipo = cboTipo.SelectedIndex == 0 ? TipoPessoa.Juridica : TipoPessoa.Fisica;

            if      (cboPagamento.SelectedValue == "0") unidade.TipoPagto = PeriodicidadePagto.Mensal;
            else if (cboPagamento.SelectedValue == "1") unidade.TipoPagto = PeriodicidadePagto.Quinzenal;
            else                                        unidade.TipoPagto = PeriodicidadePagto.Semanal;

            ////Salva os procedimentos adicionados
            //if (unidade.Procedimentos != null) unidade.Procedimentos.Clear();

            List<UnidadeProcedimento> procedimentos = new List<UnidadeProcedimento>();

            if (this.ProcedimentosAdicionados != null)
            {
                //if (unidade.Procedimentos == null) unidade.Procedimentos = new List<UnidadeProcedimento>();

                //this.ProcedimentosAdicionados.ForEach(up => up.Unidade = unidade);
                //this.ProcedimentosAdicionados.ForEach(up => unidade.Procedimentos.Add(up));

                this.ProcedimentosAdicionados.ForEach(up => procedimentos.Add(up));
            }

            PrestadorUnidadeFacade.Instancia.Salvar(unidade, procedimentos);

            this.limpaCampos();
            this.idUnidadeSel = 0;
            this.carregarUnidades();
        }
        [Obsolete("Obsoleto", false)]
        protected void cmdAddContratos_Click___(object sender, EventArgs e)
        {
            #region validacoes

            if (txtNome.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome.");
                txtNome.Focus();
                return;
            }

            //if (txtEmail.Text.Trim() == "")
            //{
            //    Geral.Alerta(this, "Informe o e-mail.");
            //    txtEmail.Focus();
            //    return;
            //}

            if (cboTabelaPreco.Items.Count == 0)
            {
                Geral.Alerta(this, "Selecione uma tabela de preços.");
                cboTabelaPreco.Focus();
                return;
            }

            //TODO: validar e-mail e se o endereço de email ja existe para outro contrato

            if (cboTabelaProcedimentos.Items.Count == 0)
            {
                Geral.Alerta(this, "Selecione uma tabela de procedimentos.");
                cboTabelaProcedimentos.Focus();
                return;
            }

            #endregion

            PrestadorUnidade unidade = null;
            long idPrestador = Geral.IdEnviado(this.Context, Keys.IdKey);

            if (this.idUnidadeSel == 0) unidade = new PrestadorUnidade();
            else
            {
                unidade = PrestadorUnidadeFacade.Instancia.CarregaPorId(this.idUnidadeSel);
                unidade.Procedimentos = ProcedimentoFacade.Instancia.CarregarProcedimentosDaUnidade(this.idUnidadeSel);
            }

            unidade.Celular = txtCel.Text;
            unidade.Documento = txtDocumento.Text;
            unidade.Email = txtEmail.Text;
            unidade.Nome = txtNome.Text;

            unidade.CEP = txtCEP.Text;
            unidade.Endereco = txtEndereco.Text;
            unidade.Numero = txtNumero.Text;
            unidade.Complemento = txtComplemento.Text;
            unidade.Bairro = txtBairro.Text;
            unidade.Cidade = txtCidade.Text;
            unidade.UF = txtEstado.Text;

            if (this.idUnidadeSel == 0) unidade.Owner = PrestadorFacade.Instancia.CarregarPorId(idPrestador); //contexto.Prestadores.Include("Segmento").Where(p => p.ID == idPrestador).Single();
            //if (idRegiao > 0) unidade.Regiao = contexto.Regioes.Where(r => r.ID == idRegiao).Single();
            unidade.Regiao = null;

            long tabelaId = Util.CTipos.CTipo<long>(cboTabelaProcedimentos.SelectedValue);
            unidade.Tabela = TabelaProcedimentoFacade.Instancia.Carregar(tabelaId); //contexto.TabelasProcedimento.Include("Segmento").Where(t => t.ID == tabelaId).Single();

            tabelaId = Util.CTipos.CTipo<long>(cboTabelaPreco.SelectedValue);
            unidade.TabelaPreco = TabelaPrecoFacade.Instancia.Carregar(tabelaId); // contexto.TabelasPreco.Where(t => t.ID == tabelaId).Single();

            unidade.Telefone = txtFone.Text;
            unidade.Tipo = cboTipo.SelectedIndex == 0 ? TipoPessoa.Juridica : TipoPessoa.Fisica;

            //checa os procedimentos selecionados
            long aux = 0; int index = -1;

            List<UnidadeProcedimento> temp = new List<UnidadeProcedimento>();
            if (unidade.Procedimentos == null) unidade.Procedimentos = new List<UnidadeProcedimento>();
            temp.AddRange(unidade.Procedimentos);

            for (int i = 0; i < chklProcedimento.Items.Count; i++)
            {
                aux = Convert.ToInt64(chklProcedimento.Items[i].Value);

                if (chklProcedimento.Items[i].Selected)
                {
                    //está editando, entao deve-se checar se o procedimento ja está adicionado
                    if (this.idUnidadeSel > 0)
                    {
                        //TODO: checagem
                        index = temp.FindIndex(p => p.Procedimento.ID == aux);

                        if (index > -1) continue;
                    }

                    UnidadeProcedimento up = new UnidadeProcedimento();
                    up.Unidade = unidade;
                    up.Procedimento = ProcedimentoFacade.Instancia.Carregar(aux); //contexto.Procedimentos.Include("Tabela").Where(p => p.ID == aux).Single();

                    if (unidade.Procedimentos == null) unidade.Procedimentos = new List<UnidadeProcedimento>();

                    unidade.Procedimentos.Add(up);
                }
            }

            if (this.idUnidadeSel > 0 && unidade.Procedimentos != null && unidade.Procedimentos.Count > 0)
            {
                //está editando, entao deve-se checar os que foram removidos
                for (int i = 0; i < chklProcedimento.Items.Count; i++)
                {
                    if (!chklProcedimento.Items[i].Selected)
                    {
                        aux = Convert.ToInt64(chklProcedimento.Items[i].Value);

                        index = ((List<UnidadeProcedimento>)unidade.Procedimentos).FindIndex(p => p.Procedimento.ID == aux);

                        if (index > -1)
                        {
                            ProcedimentoFacade.Instancia.ExcluirUnidadeProcedimento(unidade.Procedimentos[index].ID);
                            unidade.Procedimentos.RemoveAt(index);
                        }
                    }
                }
            }

            PrestadorUnidadeFacade.Instancia.Salvar(unidade);

            this.limpaCampos();
            this.idUnidadeSel = 0;
            this.carregarUnidades();
        }

        #endregion 

        #region especialidades 

        void carregarUnidadeEspecialidades()
        {
            long id = Geral.IdEnviado(this.Context, Keys.IdKey);

            //using (var contexto = new Contexto())
            //{
            //    var lista = (from ue in contexto.UnidadeEspecialidades
            //                      join pu in contexto.PrestadorUnidades on ue.Unidade.ID equals pu.ID
            //                      join p  in contexto.Prestadores on pu.Owner.ID equals p.ID
            //                      join e  in contexto.Especialidades on ue.Especialidade.ID equals e.ID
            //                      where p.ID == id
            //                      orderby pu.Nome, e.Nome
            //                      select new
            //                      {
            //                          ID                = ue.ID,
            //                          EspecialidadeNome = e.Nome,
            //                          ContratoNome      = pu.Nome,
            //                          ContratoEndereco  = pu.Endereco,
            //                          ContratoCidade    = pu.Cidade
            //                      }
            //                ).ToList();

                var lista = PrestadorUnidadeFacade.Instancia.CarregarEspecialidadesPorPrestador(id);
                gridEspecialidades.DataSource = lista;
                gridEspecialidades.DataBind();
            //}
        }

        protected void cmdAddEspecialidade_Click(object sender, EventArgs e)
        {
            #region validacao 

            if (cboContrato.Items.Count == 0)
            {
                Geral.Alerta(this, "Selecione um contrato.");
                cboContrato.Focus();
                return;
            }
            if (cboEspecialidade.Items.Count == 0)
            {
                Geral.Alerta(this, "Selecione uma especialidade.");
                cboEspecialidade.Focus();
                return;
            }
            #endregion

            long idUnidade = CTipos.CToLong(cboContrato.SelectedValue);
            long idEspecialidade = CTipos.CToLong(cboEspecialidade.SelectedValue);

            UnidadeEspecialidade obj = new UnidadeEspecialidade();
            obj.Especialidade = EspecialidadeFacade.Instance.Carregar(idEspecialidade);
            obj.Unidade = PrestadorUnidadeFacade.Instancia.CarregaPorId(idUnidade);

            EspecialidadeFacade.Instance.SalvarUnidadeEspecialidade(obj);

            this.carregarUnidadeEspecialidades();
        }

        protected void gridEspecialidades_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Geral.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja remover a especialidade?");
        }

        protected void gridEspecialidades_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                this.carregarUnidadeEspecialidades();
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                //using (Contexto contexto = new Contexto())
                //{
                    long id = Geral.ObterDataKeyValDoGrid<long>(gridEspecialidades, e, 0);

                    //UnidadeEspecialidade obj = contexto.UnidadeEspecialidades
                    //    .Where(ue => ue.ID == id)
                    //    .Single();

                    EspecialidadeFacade.Instance.ExcluirUnidadeEspecialidade(id);

                //    contexto.UnidadeEspecialidades.Remove(obj);

                //    contexto.SaveChanges();
                //}

                this.carregarUnidadeEspecialidades();
            }
        }

        #endregion

        #region dados bancarios

        protected void lnkSalvarDadosBancarios_Click(object sender, EventArgs e)
        {
            long idContrato = CTipos.CToLong(cboContrato_Bancarios.SelectedValue);
            if (idContrato == 0)
            {
                Util.Geral.Alerta(this, "Selecione um contrato.");
                return;
            }

            Banco banco = BancoFacade.Instancia.CarregarPorUnidadeId(idContrato);

            if (banco == null || banco.ID == 0)
            {
                banco = new Banco();
                    
            }

            banco.Nome = txtBanco.Text;
            banco.Codigo = txtBancoNumero.Text;
            banco.Agencia = txtAgencia.Text;
            banco.AgenciaDAC = txtAgenciaDV.Text;

            banco.Conta = txtNumConta.Text;
            banco.ContaDAC = txtNumContaDV.Text;
            banco.Tipo = (TipoConta)Enum.Parse(typeof(TipoConta), cboTipoConta.SelectedValue);
            banco.Unidade = PrestadorUnidadeFacade.Instancia.CarregaPorId(idContrato); 

            BancoFacade.Instancia.Salvar(banco);

            Geral.Alerta(this, "Dados bancários salvos com sucesso.");
        }

        protected void cboContrato_Bancarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            long idContrato = CTipos.CToLong(cboContrato_Bancarios.SelectedValue);

            Banco banco = BancoFacade.Instancia.CarregarPorUnidadeId(idContrato);
            if (banco != null)
            {
                txtAgencia.Text = banco.Agencia;
                txtAgenciaDV.Text = banco.AgenciaDAC;
                txtBanco.Text = banco.Nome;
                txtBancoNumero.Text = banco.Codigo;
                txtNumConta.Text = banco.Conta;
                txtNumContaDV.Text = banco.ContaDAC;
                cboTipoConta.SelectedValue = Convert.ToInt32(banco.Tipo).ToString();
            }
            else
            {
                txtAgencia.Text = "";
                txtAgenciaDV.Text = "";
                txtBanco.Text = "";
                txtNumConta.Text = "";
                txtNumContaDV.Text = "";
                txtBancoNumero.Text = "";
            }
        }

        #endregion

        #region acesso 

        protected void cboContrato_Acesso_SelectedIndexChanged(object sender, EventArgs e)
        {
            long idContrato = CTipos.CToLong(cboContrato_Acesso.SelectedValue);
            Usuario u = UsuarioFacade.Instance.CarregarPorUnidade(idContrato);

            if (u != null)
            {
                txtLogin.Text = u.Login;
                txtSenha.Attributes.Add("value", u.Senha);
            }
            else if(idContrato > 0)
            {
                PrestadorUnidade unidade = PrestadorUnidadeFacade.Instancia.CarregaPorId(idContrato);
                txtLogin.Text =  unidade.Email;
                txtSenha.Attributes.Add("value", "");
            }
        }

        protected void lnkSalvarDadosAcesso_Click(object sender, EventArgs e)
        {
            #region validacoes 
            
            long idContrato = CTipos.CToLong(cboContrato_Acesso.SelectedValue);

            if (idContrato == 0)
            {
                Geral.Alerta(this, "Selecione um contrato.");
                return;
            }

            if (txtLogin.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o login.");
                txtLogin.Focus();
                return;
            }

            if (txtSenha.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe a senha.");
                txtSenha.Focus();
                return;
            }

            //TODO: validar se o login ja existe

            #endregion

            Usuario u = UsuarioFacade.Instance.CarregarPorUnidade(idContrato);

            if (u == null || u.ID == 0)
            {
                u = new Usuario();
                u.Unidade = PrestadorUnidadeFacade.Instancia.CarregaPorId(idContrato);
            }

            bool loginOk = true;
            if (u.ID > 0)
                loginOk = UsuarioFacade.Instance.VerificarLogin(u.ID, txtLogin.Text);
            else
                loginOk = UsuarioFacade.Instance.VerificarLogin(null, txtLogin.Text);

            if (!loginOk)
            {
                Geral.Alerta(this, "O login informado ja está em uso. Por favor, queira informar outro.");
                return;
            }

            //u.Nome = "";
            u.Login = txtLogin.Text;
            u.Senha = txtSenha.Text;
            u.Tipo = TipoUsuario.ContratoDePrestador;

            UsuarioFacade.Instance.Salvar(u);

            Geral.Alerta(this, "Usuário salvo com sucesso.");
            txtSenha.Attributes.Add("value", txtSenha.Text);
        }

        #endregion 

        

    }
}