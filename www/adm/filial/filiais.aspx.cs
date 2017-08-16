namespace MedProj.www.adm.filial
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj.Entidades;
    using LC.Web.PadraoSeguros.Facade;

    public partial class filiais : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregarFiliais();
            }
        }

        void carregarFiliais()
        {
            grid.DataSource = FilialFacade.Instancia.CarregarTodos(txtNome.Text.Trim());
            grid.DataBind();
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("filial.aspx");
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.carregarFiliais();
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                Response.Redirect("filial.aspx?" + Util.Keys.IdKey + "=" + id.ToString());
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                try
                {
                    //FilialFacade.Instancia.Excluir(id);
                    this.carregarFiliais();
                }
                catch
                {
                    Util.Geral.Alerta(null, this, "_err", "Não foi possível remover a filial. Talvez ela esteja em uso.");
                }
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja excluir a filial?");
        }
    }
}