using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using MedProj.www.Util;
using LC.Web.PadraoSeguros.Entity;
using Ent = MedProj.Entidades;

namespace MedProj.www.clientes.pessoas
{
    public partial class pessoas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //using (Ent.Contexto contexto = new Ent.Contexto())
            //{
            //    gridBeneficiarios.DataSource = contexto.Beneficiarios.OrderBy(b => b.Nome).ToList();
            //    gridBeneficiarios.DataBind();
            //}
        }

        protected void cmdConsultar_Click(Object sender, EventArgs e)
        {
            IList<Beneficiario> lista = null;
            lista = Beneficiario.CarregarPorParametro(txtNome.Text, txtCPF.Text,
                txtRG.Text, PegaTipoDeBusca(optQualquer, optInicio, optInteiro));

            gridBeneficiarios.DataSource = lista;
            gridBeneficiarios.DataBind();
        }

        SearchMatchType PegaTipoDeBusca(RadioButton optQualquer, RadioButton optInicio, RadioButton optInteiro)
        {
            if (optQualquer.Checked) { return SearchMatchType.QualquerParteDoCampo; }
            if (optInicio.Checked) { return SearchMatchType.InicioDoCampo; }
            else { return SearchMatchType.CampoInteiro; }
        }

        protected void cmdNovo_Click(Object sender, EventArgs e)
        {
            Response.Redirect("pessoa.aspx");
        }

        protected void gridBeneficiarios_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Session[Keys.IDKey] = gridBeneficiarios.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("pessoa.aspx?", Keys.IDKey, "=", Session[Keys.IDKey]));
            }
            else if (e.CommandName.Equals("contratos"))
            {
                gridContratos.DataSource = Contrato.CarregarPorBeneficiário(gridBeneficiarios.DataKeys[Convert.ToInt32(e.CommandArgument)][0]);
                gridContratos.DataBind();
                pnlContratos.Visible = gridContratos.Rows.Count > 0;
                lblSuperior.InnerText = "Contratos do beneficiário " + gridBeneficiarios.Rows[Convert.ToInt32(e.CommandArgument)].Cells[0].Text;
                if (gridContratos.Rows.Count == 0)
                {
                    Geral.Alerta(null, this.Page, "_nenhum", "Nenhuma proposta para esse beneficiário.");
                }
            }
        }

        protected void gridContratos_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("editar"))
            {
                Session[Keys.IDKey] = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                Response.Redirect(String.Concat("../clientes/cliente.aspx?", Keys.IDKey, "=", Session[Keys.IDKey]));
            }
        }

        protected void gridContratos_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[4].Text == "0")
                    e.Row.Cells[4].Text = "SIM";
                else
                    e.Row.Cells[4].Text = "NÃO";
            }
        }

        protected void gridBeneficiarios_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Object idEnriq = gridBeneficiarios.DataKeys[e.Row.RowIndex][1];

                if (idEnriq != null)
                {
                    e.Row.ForeColor = System.Drawing.Color.Blue;
                    e.Row.ToolTip = "enriquecimento pendente";
                }
            }
        }
    }
}