namespace MedProj.www.adm.usuarios
{
    using System;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Data.OleDb;
    using System.Web.Security;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj.www.Util;
    using MedProj.Entidades;
    using MedProj.Entidades.Enuns;
    using LC.Web.PadraoSeguros.Facade;
    using System.Net.Mail;

    public partial class usuario : System.Web.UI.Page
    {
        long usuarioId
        {
            get
            {
                if (!string.IsNullOrEmpty(Request[Keys.IdKey]))
                    return Convert.ToInt64(Request[Keys.IdKey]);
                else
                    return 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregarPrestadores();

                if (this.usuarioId > 0) this.carregarUsuario();
            }
        }

        void carregarPrestadores()
        {
            cboPrestador.Items.Clear();
            cboPrestador.DataValueField = "ID";
            cboPrestador.DataTextField  = "Nome";
            cboPrestador.DataSource     = PrestadorFacade.Instancia.CarregarTodos();
            cboPrestador.DataBind();
        }

        void carregarUnidades()
        {
            cboUnidade.Items.Clear();
            if (cboPrestador.Items.Count == 0) return;

            var lista = PrestadorUnidadeFacade.Instancia.CarregaPorPrestadorId(Convert.ToInt64(cboPrestador.SelectedValue));

            foreach (PrestadorUnidade unidade in lista)
            {
                cboUnidade.Items.Add(new ListItem(unidade.Nome,unidade.ID.ToString()));
            }
        }

        void carregarUsuario()
        {
            Usuario u = UsuarioFacade.Instance.Carregar(this.usuarioId);

            txtLogin.Text = u.Login;
            txtNome.Text  = u.Nome;
            txtSenha.Attributes.Add("value", u.Senha);
            cboTipoUsuario.SelectedIndex = (int)u.Tipo;

            chkAtivo.Checked = u.Ativo;

            litDataCadastro.Text = u.DataCadastro.ToString("dd/MM/yyyy HH:mm");

            divDataCadastro.Visible = true;

            if (u.Tipo == TipoUsuario.ContratoDePrestador)
            {
                pnlPrestador.Visible = true;

                cboPrestador.SelectedValue = u.Unidade.Owner.ID.ToString();
                this.carregarUnidades();

                cboUnidade.SelectedValue = u.Unidade.ID.ToString();
                
            }
        }

        protected void cboTipoUsuario_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTipoUsuario.SelectedValue == "2")
                pnlPrestador.Visible = true;
            else
                pnlPrestador.Visible = false;
        }

        protected void cboPrestador_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregarUnidades();
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("usuarios.aspx");
        }

        public new bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                m = null;

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            #region validacoes 

            if (txtNome.Text.Trim() == "")
            {
                Util.Geral.Alerta(this, "Informe o nome.");
                txtNome.Focus();
                return;
            }

            if (txtLogin.Text.Trim() == "")
            {
                Util.Geral.Alerta(this, "Informe o login.");
                txtLogin.Focus();
                return;
            }
            else
            {
                if (!IsValid(txtLogin.Text))
                {
                    Util.Geral.Alerta(this, "O login deve ser um e-mail válido.");
                    txtLogin.Focus();
                    return;
                }

                //valida login ja em uso
                long? id = null;
                if (this.usuarioId > 0) id = this.usuarioId;

                bool testeLogin = UsuarioFacade.Instance.VerificarLogin(id, txtLogin.Text);

                if (testeLogin == false)
                {
                    Util.Geral.Alerta(this, "Login ja utilizado.");
                    txtLogin.Focus();
                    return;
                }
            }

            if (txtSenha.Text.Trim().Length < 5)
            {
                Util.Geral.Alerta(this, "Informe a senha.");
                txtSenha.Focus();
                return;
            }

            if (cboTipoUsuario.SelectedIndex == 2 && cboUnidade.Items.Count == 0)
            {
                Util.Geral.Alerta(this, "Informe a unidade.");
                cboTipoUsuario.Focus();
                return;
            }

            #endregion

            try
            {
                Usuario u = null;

                if (this.usuarioId == 0)
                    u = new Usuario();
                else
                    u = UsuarioFacade.Instance.Carregar(this.usuarioId);

                u.Login = txtLogin.Text;
                u.Nome = txtNome.Text;
                u.Senha = txtSenha.Text;
                u.Tipo = (TipoUsuario)Enum.Parse(typeof(TipoUsuario), cboTipoUsuario.SelectedValue);

                u.Ativo = chkAtivo.Checked;

                if (u.Tipo == TipoUsuario.ContratoDePrestador)
                    u.Unidade = PrestadorUnidadeFacade.Instancia.CarregaPorId(Convert.ToInt64(cboUnidade.SelectedValue));
                else
                    u.Unidade = null;

                UsuarioFacade.Instance.Salvar(u);
                Response.Redirect("usuarios.aspx");
            }
            catch
            {
                Util.Geral.Alerta(this, "Erro inesperado. Por favor, tente novamente.");
            }
        }
    }
}