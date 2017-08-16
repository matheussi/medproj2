namespace MedProj.www
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

    using MedProj.Entidades;
    using LC.Web.PadraoSeguros.Facade;

    public partial class meusdados : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Util.UsuarioLogado.ID)) Response.Redirect("~/login.aspx");

                this.carregarDados();
            }
        }

        void carregarDados()
        {
            Usuario usuario = UsuarioFacade.Instance.Carregar(Util.CTipos.CTipo<long>(Util.UsuarioLogado.ID));

            txtLogin.Text = usuario.Login;
            //txtEmail.Text = usuario.Email;
            txtNome.Text = usuario.Nome;
            txtSenha.Attributes.Add("value", usuario.Senha);
            txtSenha2.Attributes.Add("value", usuario.Senha);

            //if (usuario.Tipo == Entidades.Enuns.TipoUsuario.ContratoDePrestador)
            //{
            //    usuario.Unidade.Owner
            //}
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            #region validacoes 

            if (string.IsNullOrEmpty(Util.UsuarioLogado.ID)) Response.Redirect("~/login.aspx");

            if (string.IsNullOrEmpty(txtNome.Text))
            {
                Util.Geral.Alerta(this, "Informe seu nome.");
                return;
            }
            if (txtSenha.Text.Trim() == "" || txtSenha2.Text.Trim() == "")
            {
                Util.Geral.Alerta(this, "Informe sua senha.");
                return;
            }
            if (txtSenha.Text != txtSenha2.Text)
            {
                Util.Geral.Alerta(this, "As senhas informadas não conferem.");
                return;
            }

            #endregion

            Usuario usuario = UsuarioFacade.Instance.Carregar(Util.CTipos.CTipo<long>(Util.UsuarioLogado.ID));

            usuario.Nome = txtNome.Text;
            usuario.Senha = txtSenha.Text;

            UsuarioFacade.Instance.Salvar(usuario);

            txtSenha.Attributes.Add("value", usuario.Senha);
            txtSenha2.Attributes.Add("value", usuario.Senha);

            Util.Geral.Alerta(this, "Dados salvos.");
        }
    }
}