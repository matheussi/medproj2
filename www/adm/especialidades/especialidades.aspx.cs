using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MedProj.Entidades;
using LC.Web.PadraoSeguros.Facade;

namespace MedProj.www.adm.especialidades
{
    public partial class especialidades : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregarEspecialidades();
            }
        }

        void carregarEspecialidades()
        {
            grid.DataSource = EspecialidadeFacade.Instance.Carregar(txtNome.Text);
            grid.DataBind();

            //using (Contexto contexto = new Contexto())
            //{
            //    if (txtNome.Text.Trim() != "")
            //    {
            //        grid.DataSource = contexto.Especialidades
            //            .Where(e => e.Nome.Contains(txtNome.Text))
            //            .OrderBy(e => e.Nome)
            //            .ToList();
            //    }
            //    else
            //    {
            //        grid.DataSource = contexto.Especialidades
            //            .OrderBy(e => e.Nome)
            //            .ToList();
            //    }

            //    grid.DataBind();
            //}
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.carregarEspecialidades();
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("especialidade.aspx");
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                Response.Redirect("especialidade.aspx?" + Util.Keys.IdKey + "=" + id.ToString());
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                try
                {
                    EspecialidadeFacade.Instance.Excluir(id);
                    this.carregarEspecialidades();
                }
                catch
                {
                    Util.Geral.Alerta(null, this, "_err", "Não foi possível remover a especialidade. Talvez ela esteja em uso.");
                }

                //using (Contexto contexto = new Contexto())
                //{
                //    Especialidade espec = contexto.Especialidades.Find(id);

                //    try
                //    {
                //        contexto.Especialidades.Remove(espec);
                //        contexto.SaveChanges();

                //        this.carregarEspecialidades();
                //    }
                //    catch
                //    {
                //        Util.Geral.Alerta(null, this, "_err", "Não foi possível remover a especialidade. Talvez ela esteja em uso.");
                //    }
                //}
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja excluir a especialidade?");
        }
    }
}