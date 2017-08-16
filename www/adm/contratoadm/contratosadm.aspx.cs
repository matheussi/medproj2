namespace MedProj.www.adm.contratoadm
{
    using System;
    using System.Web;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj.Entidades;
    using LC.Web.PadraoSeguros.Facade;

    public partial class contratosadm : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.carregar();
            }
        }

        void carregar()
        {
            grid.DataSource = ContratoAdmFacade.Instance.Carregar(txtNome.Text);
            grid.DataBind();
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.carregar();
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("contratoadm.aspx");
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                Response.Redirect("contratoadm.aspx?" + Util.Keys.IdKey + "=" + id.ToString());
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                try
                {
                    //AssociadoPJFacade.Instance.Excluir(id);
                    this.carregar();
                }
                catch
                {
                    Util.Geral.Alerta(null, this, "_err", "Não foi possível remover o contrato. Talvez ele esteja em uso.");
                }
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja excluir o contrato?");

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var c = e.Row.DataItem as ContratoADM;
                if (!c.Ativo) e.Row.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}