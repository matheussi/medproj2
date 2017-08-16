namespace MedProj.www.adm.pagamento
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj.www.Util;
    using MedProj.Entidades;
    using LC.Web.PadraoSeguros.Facade;
    using MedProj.Entidades.Enuns;
    using System.IO;
    using System.Data;

    public partial class pagamentos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDataAte.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtDataDe.Text = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            }
        }

        void localizar()
        {
            DateTime de = Util.CTipos.CStringToDateTime(txtDataDe.Text, 0, 0, 0, 500);
            DateTime ate = Util.CTipos.CStringToDateTime(txtDataAte.Text, 23, 59, 59, 990);

            if (de == DateTime.MinValue || ate == DateTime.MinValue || de > ate)
            {
                Util.Geral.Alerta(this, "Período de data inválido.");
                return;
            }

            grid.DataSource = AgendaPagamentoFacade.Instancia.Carregar(de, ate);
            grid.DataBind();
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("pagamento.aspx");
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.localizar();
        }

        protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.localizar();
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                string id = Util.Geral.ObterDataKeyValDoGrid<string>(grid, e, 0);

                Response.Redirect(string.Format("pagamento.aspx?{0}={1}", Util.Keys.IdKey, id));
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Util.Geral.grid_AdicionaToolTip<LinkButton>(e, 6, 0, "Editar");
            }
        }
    }
}