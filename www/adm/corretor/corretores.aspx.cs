namespace MedProj.www.adm.corretor
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj.Entidades;
    using LC.Web.PadraoSeguros.Facade;

    public partial class corretores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregarCorretores();
            }
        }

        void carregarCorretores()
        {
            grid.DataSource = CorretorFacade.Instancia.CarregarTodos(txtNome.Text.Trim());
            grid.DataBind();
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("corretor.aspx");
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.carregarCorretores();
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                Response.Redirect("corretor.aspx?" + Util.Keys.IdKey + "=" + id.ToString());
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                try
                {
                    SegmentoFacade.Instancia.Excluir(id);
                    this.carregarCorretores();
                }
                catch
                {
                    Util.Geral.Alerta(null, this, "_err", "Não foi possível remover o corretor. Talvez ele esteja em uso.");
                }
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja excluir o corretor?");
        }
    }
}