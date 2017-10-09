namespace MedProj.www.adm.produto
{
    using System;
    using System.Web;
    using System.Linq;
    using System.Web.UI;
    using MedProj.www.Util;
    using MedProj.Entidades;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using LC.Web.PadraoSeguros.Facade;

    using Entity = LC.Web.PadraoSeguros.Entity;

    public partial class produto : System.Web.UI.Page
    {
        long IdItem
        {
            set { ViewState["idi"] = value; }
            get { return CTipos.CToLong(ViewState["idi"]); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtValor.Attributes.Add("onkeyup", "mascara('" + txtValor.ClientID + "')");
            txtValorNet.Attributes.Add("onkeyup", "mascara('" + txtValorNet.ClientID + "')");

            if (!IsPostBack)
            {
                this.ExibirOperadoras(cboOperadora, false, true);
                this.ExibirEstipulantes(cboEstipulante, false, true);
                this.CarregaContratoADM();

                long? id = Geral.IdEnviadoNull(this.Context, Keys.IdKey);
                if (id.HasValue)
                {
                    this.carregarProduto(id.Value);
                }
            }
        }

        void carregarProduto(long id)
        {
            var prod = ProdutoFacade.Instancia.Carregar(id);

            txtNome.Text = prod.Nome;
            cboOperadora.SelectedValue = prod.Operadora.ID.ToString();
            cboEstipulante.SelectedValue = prod.AssociadoPj.ID.ToString();
            cboEstipulante_SelectedIndexChanged(null, null);
            cboContratoADM.SelectedValue = prod.ContratoAdm.ID.ToString();

            pnlItens.Visible = true;

            this.carregarItens();
        }

        void carregarItens()
        {
            grid.DataSource = ProdutoFacade.Instancia.CarregarItens(Geral.IdEnviadoNull(this.Context, Keys.IdKey).Value);
            grid.DataBind();
        }

        /****************************************************************************************/

        Boolean HaItemSelecionado(DropDownList combo)
        {
            if (combo.Items.Count == 0) { return false; }

            return combo.SelectedValue != "0" &&
                   combo.SelectedValue != "-1" &&
                   combo.SelectedValue != "";
        }

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

        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            #region validacao 

            if (txtProduto.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome do produto.");
                return;
            }

            decimal valor     = CTipos.ToDecimal(txtValor.Text);
            decimal valorNet  = CTipos.ToDecimal(txtValorNet.Text);
            DateTime vigencia = CTipos.CStringToDateTime(txtVigencia.Text);

            if (vigencia == DateTime.MinValue)
            {
                Geral.Alerta(this, "Informe a vigência.");
                return;
            }

            #endregion

            ProdutoItem item = null;

            if (this.IdItem > 0)
            {
                item = ProdutoFacade.Instancia.CarregarItem(this.IdItem);
                item.DataAlteracao = DateTime.Now;
            }
            else
                item = new ProdutoItem();

            item.Nome       = txtProduto.Text;
            item.ProdutoId  = Geral.IdEnviadoNull(this.Context, Keys.IdKey).Value;
            item.UsuarioId  = Util.CTipos.CToLong(Util.UsuarioLogado.ID);
            item.Valor      = valor;
            item.ValorNet   = valorNet;
            item.Vigencia   = vigencia;

            ProdutoFacade.Instancia.SalvarItem(item);

            txtProduto.Text = "";
            txtValor.Text = "0,00";
            txtValorNet.Text = "0,00";

            this.IdItem = 0;

            this.carregarItens();

            //Geral.Alerta(this, "Item salvo com sucesso.");
        }

        protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.carregarItens();
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                this.IdItem = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                ProdutoItem i = ProdutoFacade.Instancia.CarregarItem(this.IdItem);

                txtProduto.Text = i.Nome;
                txtVigencia.Text = i.Vigencia.ToString("dd/MM/yyyy");
                txtValor.Text = i.Valor.ToString("N2");
                txtValorNet.Text = i.ValorNet.ToString("N2");
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                try
                {
                    ProdutoFacade.Instancia.ExcluirItem(id);
                    this.carregarItens();

                    Util.Geral.Alerta(null, this, "_o", "Item excluído com sucesso.");
                }
                catch
                {
                    Util.Geral.Alerta(null, this, "_err", "Não foi possível remover o item. Talvez ela esteja em uso.");
                }
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja excluir o item?");
        }

        /***************************************************************************************/

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("produtos.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (txtNome.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome do produto.");
                txtNome.Focus();
                return;
            }

            if (cboContratoADM.Items.Count == 0)
            {
                Geral.Alerta(this, "Informe o contrato administrativo.");
                cboContratoADM.Focus();
                return;
            }

            Produto prod = null;

            long? id = Geral.IdEnviadoNull(this.Context, Keys.IdKey);

            if (id.HasValue)
            {
                prod = ProdutoFacade.Instancia.Carregar(id.Value);
                prod.DataAlteracao = DateTime.Now;
            }
            else
            {
                prod = new Produto();
                prod.Operadora = new Operadora();
                prod.AssociadoPj = new AssociadoPJ();
                prod.ContratoAdm = new ContratoADM();
            }

            prod.AssociadoPj.ID = Util.CTipos.CToLong(cboEstipulante.SelectedValue);
            prod.ContratoAdm.ID = Util.CTipos.CToLong(cboContratoADM.SelectedValue);
            prod.Nome = txtNome.Text;
            prod.Operadora.ID = Util.CTipos.CToLong(cboOperadora.SelectedValue);
            prod.UsuarioId = Util.CTipos.CToLong(Util.UsuarioLogado.ID);

            ProdutoFacade.Instancia.Salvar(prod);

            Response.Redirect("produto.aspx?msg=1&" + Keys.IdKey + "=" + prod.ID.ToString());

            //long id = Geral.IdEnviado(this.Context, Keys.IdKey);

            //TabelaCobertura tabela = null;

            //tabela = new TabelaCobertura();
            //if (id != 0)
            //{
            //    var test = TabelaCoberturaFacade.Instancia.CarregarPorContratoADM(id, CTipos.CTipo<long>(cboContratoADM.SelectedValue));
            //    if (test != null)
            //    {
            //        Geral.Alerta(this, "Já existe uma tabela de cobertura para este contrato adm: " + test.Nome);
            //        return;
            //    }

            //    tabela = TabelaCoberturaFacade.Instancia.Carregar(id);
            //}
            //else
            //{
            //    var test = TabelaCoberturaFacade.Instancia.CarregarPorContratoADM(null, CTipos.CTipo<long>(cboContratoADM.SelectedValue));
            //    if (test != null)
            //    {
            //        Geral.Alerta(this, "Já existe uma tabela de cobertura para este contrato adm: " + test.Nome);
            //        return;
            //    }
            //}

            //tabela.Nome = txtTabela.Text;
            ////tabela.ValorPorVida = Util.CTipos.ToDecimal(txtValorPorVida.Text);

            //tabela.AssociadoPj = new AssociadoPJ();
            //tabela.AssociadoPj.ID = Convert.ToInt64(cboEstipulante.SelectedValue);

            //tabela.ContratoAdm = new ContratoADM();
            //tabela.ContratoAdm.ID = Convert.ToInt64(cboContratoADM.SelectedValue);

            //TabelaCoberturaFacade.Instancia.Salvar(tabela);

            //if (id == 0) Response.Redirect("cobertura.aspx?msg=1&" + Keys.IdKey + "=" + tabela.ID.ToString());
            //else Geral.Alerta(this, "Tabela salva com sucesso.");
        }
    }
}