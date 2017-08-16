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

namespace MedProj.www.adm.procedimentos
{
    public partial class procedimentos : System.Web.UI.Page
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
            int? codigo = CTipos.CToIntNullable(txtCodigo.Text);
            string nome = txtProcedimento.Text;

            grid.DataSource = TabelaProcedimentoFacade.Instancia.Carregar(codigo, nome);
            grid.DataBind();

            //using (Contexto contexto = new Contexto())
            //{
            //    if (txtCodigo.Text.Trim() != "" || txtProcedimento.Text.Trim() != "")
            //    {
            //        var predicadoOr = PredicateBuilder.False<TabelaProcedimento>();

            //        int codigo = CTipos.CToInt(txtCodigo.Text);
            //        predicadoOr = predicadoOr.Or(t => t.Codigo == codigo);

            //        if (txtProcedimento.Text.Trim() != "")
            //        {
            //            predicadoOr = predicadoOr.Or(t => t.Nome.Contains(txtProcedimento.Text));
            //        }

            //        grid.DataSource = contexto.TabelasProcedimento
            //            .AsExpandable()
            //            .Where(predicadoOr)
            //            .OrderBy(e => e.Nome)
            //            .ToList();
            //    }
            //    else
            //    {
            //        grid.DataSource = contexto.TabelasProcedimento.OrderBy(p => p.Nome).Take(150).ToList();
            //    }

            //    grid.DataBind();
            //}
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("procedimento.aspx");
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

                Response.Redirect("procedimento.aspx?" + Keys.IdKey + "=" + id);
            }
        }
    }
}