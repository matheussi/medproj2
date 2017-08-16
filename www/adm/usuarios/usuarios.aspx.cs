namespace MedProj.www.adm.usuarios
{
    using System;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Web.Security;
    using System.Data.OleDb;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj.www.Util;
    using MedProj.Entidades;
    using MedProj.Entidades.Enuns;
    using LC.Web.PadraoSeguros.Facade;

    public partial class usuarios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        void carregarUsuarios()
        {
            grid.DataSource = UsuarioFacade.Instance.Carregar(
                (TipoUsuario)Enum.Parse(typeof(TipoUsuario), cboTipoUsuario.SelectedValue), txtNome.Text);

            grid.DataBind();
        }


        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("usuario.aspx");
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.carregarUsuarios();
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if(e.CommandName.Equals("Editar"))
            {
                long id = Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);
                Response.Redirect("usuario.aspx?" + Keys.IdKey + "=" + id);
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Usuario u = e.Row.DataItem as Usuario;
                if (u == null) return;

                if (u.Tipo == TipoUsuario.ContratoDePrestador)
                    e.Row.Cells[0].Text = "Prestador";

                if (!u.Ativo)
                {
                    e.Row.Cells[0].ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[1].ForeColor = System.Drawing.Color.Red;
                    e.Row.Cells[2].ForeColor = System.Drawing.Color.Red;
                    e.Row.ToolTip = "usuário inativo";
                }
            }
        }
    }
}