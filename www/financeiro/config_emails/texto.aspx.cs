namespace MedProj.www.financeiro.config_emails
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    //using Excel;
    using MedProj.www.Util;
    using MedProj.Entidades;
    using MedProj.Entidades.Enuns;
    using LC.Web.PadraoSeguros.Facade;
    using Entity = LC.Web.PadraoSeguros.Entity;
    using System.Configuration;

    public partial class texto : System.Web.UI.Page
    {
        string idTexto
        {
            get
            {
                return Request[Util.Keys.IdKey];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(this.idTexto)) { this.carregaTexto(); }
            }
        }

        void carregaTexto()
        {
            var config = ConfigEmailFacade.Instancia.CarregarTexto(CTipos.CToLong(this.idTexto));

            txtDescricao.Text = config.Descricao;
            txtEmail.Text = config.Texto;
            chkAtivo.Checked = config.Ativo;
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("textos.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            #region validacao 

            if (txtDescricao.Text.Trim().Length <= 2)
            {
                Geral.Alerta(this, "Informe por favor uma descrição.");
                txtDescricao.Focus();
                return;
            }

            if (txtEmail.Text.Trim().Length <= 10)
            {
                Geral.Alerta(this, "Informe por favor o texto do e-mail.");
                txtEmail.Focus();
                return;
            }

            #endregion

            ConfigEmailTexto config = null;

            if (string.IsNullOrEmpty(this.idTexto))
                config = new ConfigEmailTexto();
            else
                config = ConfigEmailFacade.Instancia.CarregarTexto(CTipos.CToLong(this.idTexto));

            config.Ativo = chkAtivo.Checked;
            config.Descricao = txtDescricao.Text;
            config.Texto = txtEmail.Text;

            ConfigEmailFacade.Instancia.SalvarTexto(config);

            Response.Redirect("textos.aspx");
        }
    }
}