namespace MedProj.www.atendimento
{
    using System;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Net.Mail;
    using System.Data.OleDb;
    using System.Web.Security;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj.www.Util;
    using MedProj.Entidades;
    using MedProj.Entidades.Enuns;
    using LC.Web.PadraoSeguros.Facade;

    public partial class selecionaPrestador : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregarPrestadores();

                if (Util.UsuarioLogado.IDUnidade > 0)
                {
                    PrestadorUnidade unidade = PrestadorUnidadeFacade.Instancia.CarregaPorId(Util.UsuarioLogado.IDUnidade);
                    cboPrestador.SelectedValue = unidade.Owner.ID.ToString();
                    cboPrestador_SelectedIndexChanged(null, null);
                    cboUnidade.SelectedValue = unidade.ID.ToString();
                }
            }
        }

        void carregarPrestadores()
        {
            cboPrestador.Items.Clear();
            cboPrestador.DataValueField = "ID";
            cboPrestador.DataTextField  = "Nome";
            cboPrestador.DataSource     = PrestadorFacade.Instancia.CarregarTodos();
            cboPrestador.DataBind();

            cboPrestador.Items.Insert(0, new ListItem("selecione", "0"));
        }

        protected void cboPrestador_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPrestador.SelectedIndex <= 0)
            {
                cboUnidade.Items.Clear();
                return;
            }

            cboUnidade.DataValueField = "ID";
            cboUnidade.DataTextField = "Nome";
            cboUnidade.DataSource = PrestadorUnidadeFacade.Instancia.CarregaPorPrestadorId(Util.CTipos.CTipo<long>(cboPrestador.SelectedValue));
            cboUnidade.DataBind();
        }

        protected void cmdSel_Click(object sender, EventArgs e)
        {
            if (cboPrestador.SelectedIndex <= 0)
            {
                Util.Geral.Alerta(this, "Nenhum prestador selecionada.");
                return;
            }

            if (cboUnidade.Items.Count == 0 || cboUnidade.SelectedIndex < 0)
            {
                Util.Geral.Alerta(this, "Nenhuma unidade selecionada.");
                return;
            }

            Util.UsuarioLogado.IDUnidade = Util.CTipos.CToLong(cboUnidade.SelectedValue);

            Util.Geral.Alerta(this, "A unidade " + cboUnidade.SelectedItem.Text + " foi selecionada.");
        }
    }
}