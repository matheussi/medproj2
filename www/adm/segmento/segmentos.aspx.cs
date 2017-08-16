using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using MedProj.Entidades;
using LC.Web.PadraoSeguros.Facade;

namespace MedProj.www.adm.segmento
{
    public partial class segmentos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregarSegmentos();
            }
        }

        void carregarSegmentos()
        {
            grid.DataSource = SegmentoFacade.Instancia.CarregarTodos(txtNome.Text.Trim());
            grid.DataBind();

            //using (Contexto contexto = new Contexto())
            //{
            //    if (txtNome.Text.Trim() != "")
            //    {
            //        grid.DataSource = contexto.Segmentos
            //            .Where(e => e.Nome.Contains(txtNome.Text))
            //            .OrderBy(e => e.Nome)
            //            .ToList();
            //    }
            //    else
            //    {
            //        grid.DataSource = contexto.Segmentos
            //            .OrderBy(e => e.Nome)
            //            .ToList();
            //    }

            //    grid.DataBind();
            //}
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("segmento.aspx");
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.carregarSegmentos();
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                Response.Redirect("segmento.aspx?" + Util.Keys.IdKey + "=" + id.ToString());
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                try
                {
                    SegmentoFacade.Instancia.Excluir(id);
                    this.carregarSegmentos();
                }
                catch
                {
                    Util.Geral.Alerta(null, this, "_err", "Não foi possível remover o segmento. Talvez ele esteja em uso.");
                }

                //using (Contexto contexto = new Contexto())
                //{
                //    Segmento seg = contexto.Segmentos.Find(id);

                //    try
                //    {
                //        contexto.Segmentos.Remove(seg);
                //        contexto.SaveChanges();

                //        this.carregarSegmentos();
                //    }
                //    catch
                //    {
                //        Util.Geral.Alerta(null, this, "_err", "Não foi possível remover o segmento. Talvez ele esteja em uso.");
                //    }
                //}
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja excluir o segmento?");
        }
    }
}