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

namespace MedProj.www.adm.importacao
{
    public partial class importacoes : System.Web.UI.Page
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
            DateTime de  = Util.CTipos.CStringToDateTime(txtDataDe.Text, 0,0,0,500);
            DateTime ate = Util.CTipos.CStringToDateTime(txtDataAte.Text, 23,59,59,990);

            if (de == DateTime.MinValue || ate == DateTime.MinValue || de > ate)
            {
                Util.Geral.Alerta(this, "Período de data inválido.");
                return;
            }

            grid.DataSource = AgendaImportacaoFacade.Instancia.Carregar(de, ate, AgendaStatus.Indefinido);
            grid.DataBind();
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("importacao.aspx");
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

                Response.Redirect(string.Format("importacao.aspx?{0}={1}", Util.Keys.IdKey, id));
            }
            else if (e.CommandName.Equals("Log"))
            {
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Util.Geral.grid_AdicionaToolTip<LinkButton>(e, 7, 0, "Editar");
                Util.Geral.grid_AdicionaToolTip<ImageButton>(e, 8, 0, "Arquivo de log");

                AgendaImportacao agenda = e.Row.DataItem as AgendaImportacao;
                if (agenda != null && !agenda.Ativa) e.Row.ForeColor = System.Drawing.Color.Red;

                //if (agenda != null && !agenda.DataConclusao.HasValue)
                //{
                //    ((ImageButton)e.Row.Cells[8].Controls[0]).Visible = false;
                //}
                //else
                //{
                //    ((ImageButton)e.Row.Cells[8].Controls[0]).Visible = false; //true;
                //    ((ImageButton)e.Row.Cells[8].Controls[0]).CommandArgument = agenda.ID.ToString();
                //    ScriptManager.GetCurrent(this).RegisterPostBackControl(((ImageButton)e.Row.Cells[8].Controls[0]));
                //}
            }
        }
    }
}