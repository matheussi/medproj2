namespace MedProj.www.adm.produto
{
    using System;
    using LinqKit;
    using System.Web;
    using System.Linq;
    using System.Web.UI;
    using MedProj.www.Util;
    using MedProj.Entidades;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using LC.Web.PadraoSeguros.Facade;

    using Entity = LC.Web.PadraoSeguros.Entity;

    public partial class produtos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.ExibirOperadoras(cboOperadora, false, true);
                this.ExibirEstipulantes(cboAssociadoPj, false, true);
                this.CarregaContratoADM();

                this.carregarProdutos();
            }
        }

        void carregarProdutos()
        {
            long? operadoraId = CTipos.CToLongNullable(cboOperadora.SelectedValue);
            long? associadoPj = CTipos.CToLongNullable(cboAssociadoPj.SelectedValue);
            long? contratoAdmId = CTipos.CToLongNullable(cboContratoAdm.SelectedValue);


            if (IsPostBack)
            {
                if (contratoAdmId.HasValue == false)
                {
                    Geral.Alerta(this, "Nenhum contrato adm selecionado.");
                    return;
                }

                grid.DataSource = ProdutoFacade.Instancia.Carregar(operadoraId, associadoPj, contratoAdmId);
            }
            else
            {
                grid.DataSource = ProdutoFacade.Instancia.Carregar();
            }

            grid.DataBind();
        }

        /*-------------------------------------------------------------------------------------------------------------*/

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
            cboContratoAdm.Items.Clear();
            if (!HaItemSelecionado(cboAssociadoPj) || !HaItemSelecionado(cboOperadora))
            {
                cboContratoAdm.Items.Clear();
                return;
            }

            cboContratoAdm.DataValueField = "ID";
            cboContratoAdm.DataTextField = "DescricaoCodigoSaudeDental";

            IList<Entity.ContratoADM> lista = null;
            lista = Entity.ContratoADM.Carregar(cboAssociadoPj.SelectedValue, cboOperadora.SelectedValue, true);

            cboContratoAdm.DataSource = lista;
            cboContratoAdm.DataBind();
        }



        protected void cboOperadora_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ExibirEstipulantes(cboAssociadoPj, false, true);
            this.CarregaContratoADM();
        }

        protected void cboAssociadoPj_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CarregaContratoADM();
        }

        /*-------------------------------------------------------------------------------------------------------------*/

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("produto.aspx");
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.carregarProdutos();
        }

        protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.carregarProdutos();
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                string id = Geral.ObterDataKeyValDoGrid<string>(grid, e, 0);

                Response.Redirect("produto.aspx?" + Keys.IdKey + "=" + id);
            }
        }
    }
}