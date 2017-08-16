namespace MedProj.www.financeiro.config_emails
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

    public partial class textos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregarTextos();
            }
        }

        void carregarTextos()
        {
            var lista = ConfigEmailFacade.Instancia.CarregarTextos();

            grid.DataSource = lista;
            grid.DataBind();

            if (lista == null || lista.Count == 0)
                litAviso.Text = "Nenhum registro encontrado";
            else
                litAviso.Text = "";
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("texto.aspx");
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    //Util.Geral.grid_AdicionaToolTip<LinkButton>(e, 2, 0, "Editar");
            //    //Util.Geral.grid_AdicionaToolTip<LinkButton>(e, 3, 0, "Arquivo de log");

            //    ConfigEmailAviso obj = e.Row.DataItem as ConfigEmailAviso;
            //    if (obj != null && !obj.Ativo) e.Row.ForeColor = System.Drawing.Color.Red;

            //    Geral.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente excluir?");
            //}
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                string id = Util.Geral.ObterDataKeyValDoGrid<string>(grid, e, 0);

                Response.Redirect(string.Format("texto.aspx?{0}={1}", Util.Keys.IdKey, id));
            }
            else if (e.CommandName.Equals("Log"))
            {
            }
        }

        protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }

        protected void lnkVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("config_emails_lista.aspx");
        }

        /*******************************************************************************/
    }
}