using System;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using MedProj.www.Util;
using MedProj.Entidades;
using LC.Web.PadraoSeguros.Facade;

namespace MedProj.www.adm.associadoPJ
{
    public partial class associado : System.Web.UI.Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                this.carregarBeneficiariosPJ();

                if (Geral.IdEnviado(this.Context, Keys.IdKey) != 0)
                {
                    this.carregar();
                }
            }
        }

        void carregarBeneficiariosPJ()
        {
            cboBeneficiario.Items.Clear();

            var lista = BeneficiarioFacade.Instancia.CarregarPJ();

            if (lista != null)
            {
                lista.ForEach(b => cboBeneficiario.Items.Add(new ListItem(string.Concat(b.Nome, " (", b.CPF, ")"), b.ID.ToString())));
            }

            cboBeneficiario.Items.Insert(0, new ListItem("selecione", "-1"));
        }

        void carregar()
        {
            long id = Geral.IdEnviado(this.Context, Keys.IdKey);
            AssociadoPJ obj = AssociadoPJFacade.Instance.Carregar(id);

            txtDescricao.Text = obj.Nome;

            if (obj.MesesAPartirDaVigencia.HasValue)
            {
                cboDataFixa.Checked = false;
                cboQtdMeses.Checked = true;

                txtMesesAPartirDaVigencia.Text = obj.MesesAPartirDaVigencia.Value.ToString();
            }
            else
            {
                cboDataFixa.Checked = true;
                cboQtdMeses.Checked = false;

                if(obj.DataValidadeFixa.HasValue)
                    txtDataFixa.Text = obj.DataValidadeFixa.Value.ToString("dd/MM/yyyy");
            }

            if (obj.BeneficiarioID.HasValue)
                cboBeneficiario.SelectedValue = obj.BeneficiarioID.Value.ToString();
            else
                cboBeneficiario.SelectedIndex = 0;

            cbo_CheckedChanged(null, null);
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("associados.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            #region validacoes 

            DateTime? data = null;
            int? meses = null;

            if (txtDescricao.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome do associado.");
                txtDescricao.Focus();
                return;
            }

            if (cboDataFixa.Checked)
            {
                data = Util.CTipos.ToDateTime(txtDataFixa.Text, 23, 59, 59, 995);
                if (data == null || data.Value == DateTime.MinValue)
                {
                    Util.Geral.Alerta(this, "A data informada é inválida.");
                    txtDataFixa.Focus();
                    return;
                }
            }
            else
            {
                meses = Util.CTipos.CToIntNullable(txtMesesAPartirDaVigencia.Text);
                if (meses == null)
                {
                    Util.Geral.Alerta(this, "A quantidade de meses informada é inválida.");
                    txtMesesAPartirDaVigencia.Focus();
                    return;
                }
            }

            #endregion

            AssociadoPJ obj = new AssociadoPJ();

            long id = Geral.IdEnviado(this.Context, Keys.IdKey);
            if (id != 0)
            {
                obj.ID = id;
            }

            obj.Nome = txtDescricao.Text;
            obj.DataValidadeFixa = data;
            obj.MesesAPartirDaVigencia = meses;
            obj.Ativo = true;

            if (cboBeneficiario.SelectedIndex <= 0) obj.BeneficiarioID = null;
            else                                    obj.BeneficiarioID = Convert.ToInt64(cboBeneficiario.SelectedValue);

            AssociadoPJFacade.Instance.Salvar(obj);

            Response.Redirect("associados.aspx");
        }

        protected void cbo_CheckedChanged(object sender, EventArgs e)
        {
            if (cboDataFixa.Checked)
            {
                pnlMeses.Visible = false;
                pnlDataFixa.Visible = true;
                txtDataFixa.Focus();
            }
            else
            {
                pnlMeses.Visible = true;
                pnlDataFixa.Visible = false;
                txtMesesAPartirDaVigencia.Focus();
            }
        }
    }
}