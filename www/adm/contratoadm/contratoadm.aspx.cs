namespace MedProj.www.adm.contratoadm
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

    public partial class contratoadm : System.Web.UI.Page
    {
        long contratoAdmId
        {
            get
            {
                if (!string.IsNullOrEmpty(Request[Keys.IdKey]))
                    return Convert.ToInt64(Request[Keys.IdKey]);
                else
                    return 0;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.carregaOperadoras();
                this.carregaAssociadosPj();

                if (this.contratoAdmId > 0) this.carregaContratoAdm();
                else txtPlano.Text = "Padrão";
            }
        }

        void carregaOperadoras()
        {
            List<Operadora> operadoras = OperadoraFacade.Instance.Carregar(null);
            cboOperadora.Items.Add(new ListItem("selecione", "-1"));

            if(operadoras != null && operadoras.Count > 0)
                operadoras.ForEach(o => cboOperadora.Items.Add(new ListItem(o.Nome, o.ID.ToString())));
        }

        void carregaAssociadosPj()
        {
            List<AssociadoPJ> associados = AssociadoPJFacade.Instance.Carregar(null);
            cboAssociadoPJ.Items.Add(new ListItem("selecione", "-1"));

            if (associados != null && associados.Count > 0)
                associados.ForEach(a => cboAssociadoPJ.Items.Add(new ListItem(a.Nome, a.ID.ToString())));
        }

        void carregaContratoAdm()
        {
            var contrato = ContratoAdmFacade.Instance.Carregar(this.contratoAdmId);
            txtDescricao.Text = contrato.Descricao;
            chkAtivo.Checked = contrato.Ativo;
            cboOperadora.SelectedValue = contrato.Operadora.ID.ToString();
            cboAssociadoPJ.SelectedValue = contrato.AssociadoPJ.ID.ToString();

            var plano = PlanoFacade.Instance.CarregarPorContratoAdmId(contrato.ID);

            if (plano != null) txtPlano.Text = plano.Descricao;
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("contratosadm.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            #region validacoes 

            if (txtDescricao.Text.Trim() == "")
            {
                Util.Geral.Alerta(this, "O campo Decrição é obrigatório.");
                return;
            }

            if (txtPlano.Text.Trim() == "")
            {
                Util.Geral.Alerta(this, "O campo Plano é obrigatório.");
                return;
            }

            if (cboOperadora.SelectedIndex <= 0)
            {
                Util.Geral.Alerta(this, "Você deve selecionar uma operadora.");
                return;
            }

            if (cboAssociadoPJ.SelectedIndex <= 0)
            {
                Util.Geral.Alerta(this, "Você deve selecionar um associado.");
                return;
            }

            #endregion


            Plano p = null; 
            ContratoADM c = null;

            if (this.contratoAdmId > 0)
            {
                c = ContratoAdmFacade.Instance.Carregar(this.contratoAdmId);
                p = PlanoFacade.Instance.CarregarPorContratoAdmId(this.contratoAdmId);
            }
            else
            {
                p = new Plano();
                c = new ContratoADM();
            }

            c.AssociadoPJ = AssociadoPJFacade.Instance.Carregar(Util.CTipos.CTipo<long>(cboAssociadoPJ.SelectedValue));

            c.Ativo = chkAtivo.Checked;

            c.Descricao = txtDescricao.Text;

            c.Operadora = OperadoraFacade.Instance.Carregar(Util.CTipos.CTipo<long>(cboOperadora.SelectedValue));

            ContratoAdmFacade.Instance.Salvar(c);

            if (p == null) p = new Plano();
            p.Ativo = true;
            p.ContratoAdm = c;
            p.Descricao = txtPlano.Text;

            PlanoFacade.Instance.Salvar(p);

            Response.Redirect("contratosadm.aspx");
        }
    }
}