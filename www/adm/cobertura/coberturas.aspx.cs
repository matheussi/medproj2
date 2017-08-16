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
    using LinqKit;
    using LC.Web.PadraoSeguros.Facade;

    public partial class coberturas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregarTabelas();
            }
        }

        void carregarTabelas()
        {
            //int? codigo = CTipos.CToIntNullable(txtCodigo.Text);
            //string nome = txtProcedimento.Text;

            grid.DataSource = TabelaCoberturaFacade.Instancia.Carregar();
            grid.DataBind();

            if (grid.Rows.Count > 0)
                litMensagem.Text = "";
            else
                litMensagem.Text = "Nenhuma tabela localizada";
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("cobertura.aspx");
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.carregarTabelas();
        }

        protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.carregarTabelas();
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                string id = Geral.ObterDataKeyValDoGrid<string>(grid, e, 0);

                Response.Redirect("cobertura.aspx?" + Keys.IdKey + "=" + id);
            }
        }
    }
}