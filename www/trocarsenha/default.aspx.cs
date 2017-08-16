namespace MedProj.www.trocarsenha
{
    using System;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Data.OleDb;
    using System.Web.Security;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj = MedProj.Entidades;
    using LC.Web.PadraoSeguros.Facade;
    using Entity = LC.Web.PadraoSeguros.Entity;

    using NHibernate;
    using NHibernate.Dialect;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using FluentNHibernate.Conventions.Helpers;
    using System.Globalization;

    public partial class _default : System.Web.UI.Page
    {
        object idContrato
        {
            get
            {
                return ViewState["idContr"];
            }
            set
            {
                ViewState["idContr"] = value;
            }
        }

        string senhaContrato
        {
            get
            {
                return ViewState["senhaContr"] as string;
            }
            set
            {
                ViewState["senhaContr"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Header.DataBind();

            if (!IsPostBack)
            {
            }
        }

        protected void cmdValidarNumeroContrato_Click(object sender, EventArgs e)
        {
            this.idContrato = null;
            pnlResult.Visible = false;

            if (txtNumeroContrato.Text.Trim().Length != 16)
            {
                Util.Geral.Alerta(this, "Por favor, informe o número do cartão.");
                txtNumeroContrato.Focus();
                return;
            }

            Entity.Contrato contrato = ContratoFacade.Instance.Carregar(txtNumeroContrato.Text);

            if (contrato == null)
            {
                Util.Geral.Alerta(this, "Cartão não localizado.");
                txtNumeroContrato.Focus();
                return;
            }

            Entity.ContratoBeneficiario titular = Entity.ContratoBeneficiario.CarregarTitular(contrato.ID, null);

            if (titular == null)
            {
                Util.Geral.Alerta(this, "Cartão ou titular não localizado.");
                txtNumeroContrato.Focus();
                return;
            }

            pnlResult.Visible = true;
            this.idContrato = contrato.ID;
            this.senhaContrato = contrato.Senha;

            litNome.Text = titular.BeneficiarioNome;
            litCPF.Text = titular.BeneficiarioCPF;

            if (titular.BeneficiarioDataNascimento != DateTime.MinValue)
                litData.Text = titular.BeneficiarioDataNascimento.ToString("dd/MM/yyyy");
            else
                litData.Text = "";

            litEmail.Text = titular.BeneficiarioEmail;
        }

        protected void cmdConfirmar_Click(object sender, EventArgs e)
        {
            #region validacoes 

            if (this.idContrato == null)
            {
                pnlResult.Visible = false;
                return;
            }

            if (txtSenhaAtual.Text.Trim() == "")
            {
                Util.Geral.Alerta(this, "Você deve informar a senha atual.");
                return;
            }
            else if (txtSenhaAtual.Text != this.senhaContrato)
            {
                Util.Geral.Alerta(this, "A senha atual informada não confere.");
                return;
            }

            if (txtNovaSenha1.Text.Trim() == "")
            {
                Util.Geral.Alerta(this, "Por favor, informe a nova senha.");
                return;
            }
            if (txtNovaSenha2.Text.Trim() == "")
            {
                Util.Geral.Alerta(this, "Por favor, confirme a nova senha.");
                return;
            }

            if (txtNovaSenha1.Text != txtNovaSenha2.Text)
            {
                Util.Geral.Alerta(this, "A nova senha informada não confere.");
                return;
            }

            if (txtNovaSenha1.Text.Length != 6)
            {
                Util.Geral.Alerta(this, "A nova senha deve ter 6 dígitos.");
                return;
            }

            if (txtNovaSenha1.Text.StartsWith("0"))
            {
                Util.Geral.Alerta(this, "A nova senha não pode iniciar com zero (0).");
                return;
            }

            Int64 test = 0;
            if (!Int64.TryParse(txtNovaSenha1.Text, out test))
            {
                Util.Geral.Alerta(this, "A nova senha deve ser numérica.");
                return;
            }

            #endregion

            Entity.Contrato contrato = Entity.Contrato.Carregar(this.idContrato)[0];

            contrato.Senha = txtNovaSenha1.Text;
            contrato.Salvar();

            pnlResult.Visible = false;

            Util.Geral.Alerta(this, "Senha alterada com sucesso!");

            this.idContrato = null;
            this.senhaContrato = null;

            #region Email 

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("Olá!");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Você está recebendo este e-mail por ter alterado sua senha no sistema Clube Azul.");
            sb.Append(Environment.NewLine);
            sb.Append("Essa solicitação ocorreu em "); sb.Append(DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("pt-Br")));
            sb.Append(" às ");
            sb.Append(DateTime.Now.ToString("HH:mm", new CultureInfo("pt-Br"))); sb.Append(".");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Sua nova senha é: ");
            sb.Append(txtNovaSenha1.Text);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Este é um e-mail automático, por favor não o responda.");

#if DEBUG
            litEmail.Text = "denis.goncalves@wedigi.com.br";
#endif

            string err = "";
            bool ok = Util.Geral.Mail.Enviar("[CLUBE AZUL] - Senha", sb.ToString(), litEmail.Text, false, out err);

            //if (ok)
            //    Util.Geral.Alerta(this, "Lembrete de senha enviado.");
            //else
            //{
            //    Util.Geral.Alerta(this, "Não foi possível enviar o lembrete de senha.");
            //}

            #endregion
        }
    }
}