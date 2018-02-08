namespace MedProj.www.adm.cobertura
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using MedProj.Entidades;
    using MedProj.www.Util;
    using LC.Web.PadraoSeguros.Facade;

    using Entity = LC.Web.PadraoSeguros.Entity;
     
    public partial class cobertura : System.Web.UI.Page
    {
        long IdItemCobertura
        {
            set { ViewState["idproc"] = value; }
            get { return CTipos.CToLong(ViewState["idproc"]); }
        }

        long IdVigenciaCobertura
        {
            set { ViewState["idVig"] = value; }
            get { return CTipos.CToLong(ViewState["idVig"]); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtValorItem.Attributes.Add("onkeyup", "mascara('" + txtValorItem.ClientID + "')");
            //txtValorPorVida.Attributes.Add("onkeyup", "mascara('" + txtValorPorVida.ClientID + "')");
            txtValorVig.Attributes.Add("onkeyup", "mascara('" + txtValorVig.ClientID + "')");
            txtValorNetVig.Attributes.Add("onkeyup", "mascara('" + txtValorNetVig.ClientID + "')");

            if (!IsPostBack)
            {
                this.ExibirOperadoras(cboOperadora, false, true);
                this.ExibirEstipulantes(cboEstipulante, false, true);
                this.CarregaContratoADM();

                if (Request[Keys.IdKey] != null)
                {
                    this.carregarTabela();
                    pnlProcedimentos.Visible = true;
                    this.carregarItems(null);
                    this.carregarVigencias();

                    if (!string.IsNullOrEmpty(Request["msg"]))
                    {
                        Geral.Alerta(this, "Tabela salva com sucesso.");
                    }
                }
            }
        }

        void carregarTabela()
        {
            long id = Convert.ToInt64(Request[Keys.IdKey]);

            TabelaCobertura tabela = TabelaCoberturaFacade.Instancia.Carregar(id);

            txtTabela.Text = tabela.Nome;
            //txtValorPorVida.Text = tabela.ValorPorVida.ToString("N2");

            cboOperadora.SelectedIndex = 0; //TODO: parametrizar também. Mapenar no objeto estipulante e contratoadm
            cboOperadora_SelectedIndexChanged(null, null);
            cboEstipulante.SelectedValue = tabela.AssociadoPj.ID.ToString();
            cboEstipulante_SelectedIndexChanged(null, null);
            cboContratoADM.SelectedValue = tabela.ContratoAdm.ID.ToString();
        }

        /****************************************************************************************/

        void ExibirOperadoras(DropDownList combo, Boolean itemSELECIONE, Boolean somenteAtivas)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Nome";
            combo.DataSource = Entity.Operadora.CarregarTodas(somenteAtivas);
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        void ExibirEstipulantes(DropDownList combo, Boolean itemSELECIONE, Boolean apenasAtivos)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = Entity.Estipulante.Carregar(apenasAtivos);
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        void CarregaContratoADM()
        {
            cboContratoADM.Items.Clear();
            if (!HaItemSelecionado(cboEstipulante) || !HaItemSelecionado(cboOperadora))
            {
                cboContratoADM.Items.Clear();
                return;
            }

            cboContratoADM.DataValueField = "ID";
            cboContratoADM.DataTextField = "DescricaoCodigoSaudeDental";

            IList<Entity.ContratoADM> lista = null;
            lista = Entity.ContratoADM.Carregar(cboEstipulante.SelectedValue, cboOperadora.SelectedValue, true);

            cboContratoADM.DataSource = lista;
            cboContratoADM.DataBind();
            //cboContratoADM.Items.Insert(0, new ListItem("selecione", "-1"));
        }

        protected void cboEstipulante_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CarregaContratoADM();
        }

        protected void cboOperadora_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ExibirEstipulantes(cboEstipulante, false, true);
            this.CarregaContratoADM();
        }

        /***************************************************************************************/

        void carregarItems(string nome)
        {
            long id = Convert.ToInt64(Request[Keys.IdKey]);
            List<ItemCobertura> itens = TabelaCoberturaFacade.Instancia.CarregarItens(id, nome);

            grid.DataSource = itens;
            grid.DataBind();
        }

        void carregarVigencias()
        {
            long id = Convert.ToInt64(Request[Keys.IdKey]);
            List<VigenciaCobertura> itens = TabelaCoberturaFacade.Instancia.CarregarVigencias(id);

            gridVig.DataSource = itens;
            gridVig.DataBind();
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("coberturas.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (txtTabela.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome da tabela.");
                txtTabela.Focus();
                return;
            }

            if (cboContratoADM.Items.Count == 0)
            {
                Geral.Alerta(this, "Informe o contrato adm da tabela.");
                cboContratoADM.Focus();
                return;
            }

            

            long id = Geral.IdEnviado(this.Context, Keys.IdKey);

            TabelaCobertura tabela = null;

            tabela = new TabelaCobertura();
            if (id != 0)
            {
                var test = TabelaCoberturaFacade.Instancia.CarregarPorContratoADM(id, CTipos.CTipo<long>(cboContratoADM.SelectedValue));
                if (test != null)
                {
                    Geral.Alerta(this, "Já existe uma tabela de cobertura para este contrato adm: " + test.Nome);
                    return;
                }

                tabela = TabelaCoberturaFacade.Instancia.Carregar(id);
            }
            else
            {
                var test = TabelaCoberturaFacade.Instancia.CarregarPorContratoADM(null, CTipos.CTipo<long>(cboContratoADM.SelectedValue));
                if (test != null)
                {
                    Geral.Alerta(this, "Já existe uma tabela de cobertura para este contrato adm: " + test.Nome);
                    return;
                }
            }

            tabela.Nome = txtTabela.Text;
            //tabela.ValorPorVida = Util.CTipos.ToDecimal(txtValorPorVida.Text);

            tabela.AssociadoPj = new AssociadoPJ();
            tabela.AssociadoPj.ID = Convert.ToInt64(cboEstipulante.SelectedValue);

            tabela.ContratoAdm = new ContratoADM();
            tabela.ContratoAdm.ID = Convert.ToInt64(cboContratoADM.SelectedValue);

            TabelaCoberturaFacade.Instancia.Salvar(tabela);

            if (id == 0) Response.Redirect("cobertura.aspx?msg=1&" + Keys.IdKey + "=" + tabela.ID.ToString());
            else Geral.Alerta(this, "Tabela salva com sucesso.");
        }

        //----------------------------------------------------------------------------------------

        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            #region validacao

            if (txtDescricao.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome do item de cobertura.");
                txtDescricao.Focus();
                return;
            }

            decimal valorItem = CTipos.ToDecimal(txtValorItem.Text);

            #endregion

            long idTabela = Geral.IdEnviado(this.Context, Keys.IdKey);

            ItemCobertura obj = new ItemCobertura();

            obj.Valor = valorItem;
            obj.Descricao = txtDescricao.Text;

            if (cboStatusItem.SelectedIndex == 0)
                obj.Status = cboStatusItem.SelectedValue;
            else
                obj.Status = null;

            obj.Tabela = TabelaCoberturaFacade.Instancia.Carregar(idTabela);

            if (this.IdItemCobertura > 0) obj.ID = this.IdItemCobertura;

            TabelaCoberturaFacade.Instancia.SalvarItem(obj);

            pnlProcedimentos.Visible = true;
            this.carregarItems(null);

            Geral.Alerta(this, "Item salvo com sucesso.");

            txtDescricao.Text = "";
            txtValorItem.Text = "0,00";
            this.IdItemCobertura = 0;
            cboStatusItem.SelectedIndex = 0;
        }

        protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.carregarItems(null); //txtLocalizar.Text
        }

        protected void cmdLocalizar_Click(object sender, EventArgs e)
        {
            //this.carregarItems(txtLocalizar.Text);
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                this.IdItemCobertura = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                ItemCobertura item = TabelaCoberturaFacade.Instancia.CarregarItem(this.IdItemCobertura);

                txtDescricao.Text = item.Descricao;

                if (item.Valor.HasValue) txtValorItem.Text  = item.Valor.Value.ToString("N2");
                else txtValorItem.Text                      = "0,00";

                if (!string.IsNullOrEmpty(item.Status)) cboStatusItem.SelectedValue = item.Status;
                else                                    cboStatusItem.SelectedIndex = 1;
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                try
                {
                    TabelaCoberturaFacade.Instancia.ExcluirItem(id);

                    this.carregarItems(null); //txtLocalizar.Text
                }
                catch
                {
                    Util.Geral.Alerta(null, this, "_err", "Não foi possível remover a cobertura. Talvez ela esteja em uso.");
                }
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja excluir a cobertura?");
        }

        Boolean HaItemSelecionado(DropDownList combo)
        {
            if (combo.Items.Count == 0) { return false; }

            return combo.SelectedValue != "0" &&
                   combo.SelectedValue != "-1" &&
                   combo.SelectedValue != "";
        }

        //----------------------------------------------------------------------------------------

        protected void cmdAddVig_Click(object sender, EventArgs e)
        {
            #region validacao

            DateTime inicio = Util.CTipos.CStringToDateTime(txtInicio.Text);
            if (inicio == DateTime.MinValue)
            {
                Geral.Alerta(this, "Data de início inválida.");
                return;
            }

            decimal valor = CTipos.ToDecimal(txtValorVig.Text);
            if (valor == 0)
            {
                Geral.Alerta(this, "Valor inválido.");
                return;
            }

            decimal valorNet = CTipos.ToDecimal(txtValorNetVig.Text);
            if (valorNet == 0)
            {
                Geral.Alerta(this, "Valor Net inválido.");
                return;
            }

            #endregion

            long idTabela = Geral.IdEnviado(this.Context, Keys.IdKey);

            VigenciaCobertura obj = new VigenciaCobertura();

            obj.Valor       = valor;
            obj.ValorNet    = valorNet;
            obj.Inicio      = inicio;
            obj.Tabela      = TabelaCoberturaFacade.Instancia.Carregar(idTabela);

            if (this.IdVigenciaCobertura > 0) obj.ID = this.IdVigenciaCobertura;

            TabelaCoberturaFacade.Instancia.SalvarVigencia(obj);

            pnlProcedimentos.Visible = true;
            this.carregarVigencias();

            Geral.Alerta(this, "Vigência salva com sucesso.");

            txtInicio.Text = "";
            txtValorVig.Text = "0,00";
            txtValorNetVig.Text = "0,00";
            this.IdVigenciaCobertura = 0;
        }

        protected void gridVig_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridVig.PageIndex = e.NewPageIndex;
            this.carregarVigencias();
        }

        protected void gridVig_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                this.IdVigenciaCobertura = Util.Geral.ObterDataKeyValDoGrid<long>(gridVig, e, 0);

                VigenciaCobertura c = TabelaCoberturaFacade.Instancia.CarregarVigencia(this.IdVigenciaCobertura);

                txtInicio.Text = c.Inicio.ToString("dd/MM/yyyy");
                txtValorVig.Text = c.Valor.ToString("n2");
                txtValorNetVig.Text = c.ValorNet.ToString("n2");
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(gridVig, e, 0);

                try
                {
                    TabelaCoberturaFacade.Instancia.ExcluirVigencia(id);

                    this.carregarVigencias();
                }
                catch
                {
                    Util.Geral.Alerta(null, this, "_err", "Não foi possível remover a vigência. Talvez ela esteja em uso.");
                }
            }
        }

        protected void gridVig_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 3, "Deseja excluir a vigência?");
        }
    }
}