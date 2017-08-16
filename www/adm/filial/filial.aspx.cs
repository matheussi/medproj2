namespace MedProj.www.adm.filial
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj.www.Util;
    using MedProj.Entidades;
    using LC.Web.PadraoSeguros.Facade;

    public partial class filial : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Geral.IdEnviado(this.Context, Keys.IdKey) != 0)
                {
                    this.carregarFilial();
                }
            }
        }

        void carregarFilial()
        {
            long id = Geral.IdEnviado(this.Context, Keys.IdKey);
            Filial obj = FilialFacade.Instancia.Carregar(id);

            txtNome.Text = obj.Nome;
            chkStatus.Checked = obj.Ativa;
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("filiais.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (txtNome.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome da filial.");
                txtNome.Focus();
                return;
            }

            Filial obj = new Filial();

            long id = Geral.IdEnviado(this.Context, Keys.IdKey);
            if (id > 0) obj.ID = id;

            obj.Nome = txtNome.Text;
            obj.Ativa = chkStatus.Checked;

            FilialFacade.Instancia.Salvar(obj);

            Response.Redirect("filiais.aspx");
        }
    }
}