namespace MedProj.www.financeiro.reports
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;

    public partial class adimplenciaStatus : Page
    {
        //List<RelatorioFacade.AdimplenciaVO> dataCache
        //{
        //    get { return ViewState["dataCache"] as List<RelatorioFacade.AdimplenciaVO>; }
        //    set
        //    {
        //        if (value == null) { Session.Remove("dataCache"); }
        //        else { ViewState["dataCache"] = value; }
        //    }
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Util.Geral.ExibirEstipulantes(cboAssociadoPJ, true, true);
                this.carregarContratosIUGU();
            }
        }

        void carregarContratosIUGU()
        {
            var lista = Contrato.CarregarContratoIUGU();
            cboContrato.DataValueField = "Id";
            cboContrato.DataTextField = "BeneficiarioTitularNome";
            cboContrato.DataSource = lista;
            cboContrato.DataBind();

            cboContrato.Items.Insert(0, new ListItem("Selecione", "-1"));
        }

        void montarRelatorio()
        {
            DateTime? de  = Util.CTipos.CStringToDateTimeG(txtDe.Text);
            DateTime? ate = Util.CTipos.CStringToDateTimeG(txtAte.Text);

            List<RelatorioFacade.AdimplenciaVO> vos = null;

            string asspjid = "0";

            if (!chkIugu.Checked)
            {
                gridContratos.Columns[5].Visible = true;
                gridContratos.Columns[7].Visible = true;
                gridContratos.Columns[8].Visible = true;
                gridContratos.Columns[9].Visible = true;
                gridContratos.Columns[10].Visible = true;

                if (cboAssociadoPJ.SelectedIndex > 0) asspjid = cboAssociadoPJ.SelectedValue;

                if (cboTipo.SelectedIndex == 0)
                    vos = RelatorioFacade.Instancia.RelatorioAdimplentes(asspjid, de, ate);
                else if (cboTipo.SelectedIndex == 1)
                    vos = RelatorioFacade.Instancia.RelatorioInadimplentes(asspjid, de, ate);
                else
                {
                    if (!de.HasValue || !ate.HasValue)
                    {
                        Util.Geral.Alerta(this, "Os campos de data são obrigatórios para este relatório.");
                        return;
                    }

                    gridContratos.Columns[5].Visible = false;
                    gridContratos.Columns[7].Visible = false;
                    gridContratos.Columns[8].Visible = false;
                    gridContratos.Columns[9].Visible = false;
                    gridContratos.Columns[10].Visible = false;

                    vos = RelatorioFacade.Instancia.cobrancasNaoGeradas(asspjid, de, ate);
                }

                gridContratos.DataSource = vos;
                gridContratos.DataBind();
                if (gridContratos.Rows.Count == 0)  { litAviso.Text = "Nenhum registro localizado"; }
                else                                { litAviso.Text = ""; }
            }
            else ///////////////////////// IUGU ///////////////////////////////////
            {
                string contratoId = "";
                if (cboContrato.SelectedIndex > 0) contratoId = cboContrato.SelectedValue;

                vos = RelatorioFacade.Instancia.RelatorioIUGU(contratoId, de, ate);

                gridIUGU.DataSource = vos;
                gridIUGU.DataBind();

                if (gridIUGU.Rows.Count == 0) { litAviso.Text = "Nenhum registro localizado"; }
                else
                {
                    litAviso.Text = string.Concat(
                        "<b>Total Cobertura:</b> ", vos.Sum(v => v.TotalCoberturaValor).ToString("C"), " - ",
                        "<b>Total Produtos: </b>", vos.Sum(v => v.TotalProdutoValor).ToString("C"));
                }
            }

            if (vos != null && vos.Count > 0)
            {
                lnkToExcel.Visible  = true;
                lnkToExcelT.Visible = true;
            }
            else
            {
                lnkToExcel.Visible  = false;
                lnkToExcelT.Visible = false;
            }
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.montarRelatorio();
        }

        /****/

        protected void gridContratos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
            }
        }
        protected void gridContratos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Boolean rascunho = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][1]);
                //((Image)e.Row.Cells[4].Controls[1]).Visible = rascunho;

                //Boolean cancelado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][2]);
                //Boolean inativado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][3]);

                ////UIHelper.AuthWebCtrl((LinkButton)e.Row.Cells[5].Controls[0], new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.PropostaBeneficiarioIDKey });
                ////UIHelper.AuthCtrl((LinkButton)e.Row.Cells[7].Controls[0], new String[] { Perfil.AdministradorIDKey });
                ////UIHelper.AuthWebCtrl((LinkButton)e.Row.Cells[7].Controls[0], new String[] { Perfil.AdministradorIDKey });
                //grid_RowDataBound_Confirmacao(sender, e, 7, "Deseja realmente prosseguir com a exclusão?\\nEssa operação não poderá ser desfeita.");

                //if (Usuario.Autenticado.PerfilID != Perfil.AdministradorIDKey) { gridContratos.Columns[7].Visible = false; }

                //if (cancelado || inativado)
                //{
                //    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                //    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='../../images/unactive.png' title='inativo' alt='inativo' border='0'>";
                //    //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente ativar o contrato?");
                //}
                //else
                //{
                //    //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente cancelar o contrato?");
                //    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='../../images/active.png' title='ativo' alt='ativo' border='0'>";
                //}
            }
        }
        protected void gridContratos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridContratos.PageIndex = e.NewPageIndex;
            this.montarRelatorio();
        }
        protected void grid_RowDataBound_Confirmacao(Object sender, GridViewRowEventArgs e, int indiceControle, String Msg)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[indiceControle].Attributes.Add("onClick", "return confirm('" + Msg + "');");
            }
        }


        protected void gridIUGU_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
            }
        }
        protected void gridIUGU_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Boolean rascunho = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][1]);
                //((Image)e.Row.Cells[4].Controls[1]).Visible = rascunho;

                //Boolean cancelado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][2]);
                //Boolean inativado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][3]);

                ////UIHelper.AuthWebCtrl((LinkButton)e.Row.Cells[5].Controls[0], new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.PropostaBeneficiarioIDKey });
                ////UIHelper.AuthCtrl((LinkButton)e.Row.Cells[7].Controls[0], new String[] { Perfil.AdministradorIDKey });
                ////UIHelper.AuthWebCtrl((LinkButton)e.Row.Cells[7].Controls[0], new String[] { Perfil.AdministradorIDKey });
                //grid_RowDataBound_Confirmacao(sender, e, 7, "Deseja realmente prosseguir com a exclusão?\\nEssa operação não poderá ser desfeita.");

                //if (Usuario.Autenticado.PerfilID != Perfil.AdministradorIDKey) { gridContratos.Columns[7].Visible = false; }

                //if (cancelado || inativado)
                //{
                //    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                //    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='../../images/unactive.png' title='inativo' alt='inativo' border='0'>";
                //    //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente ativar o contrato?");
                //}
                //else
                //{
                //    //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente cancelar o contrato?");
                //    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='../../images/active.png' title='ativo' alt='ativo' border='0'>";
                //}
            }
        }
        protected void gridIUGU_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridIUGU.PageIndex = e.NewPageIndex;
            this.montarRelatorio();
        }
        protected void gridIUGU_RowDataBound_Confirmacao(Object sender, GridViewRowEventArgs e, int indiceControle, String Msg)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    e.Row.Cells[indiceControle].Attributes.Add("onClick", "return confirm('" + Msg + "');");
            //}
        }

        /****/

        protected void cmdToExcel_Click(Object sende, EventArgs e)
        {
            List<RelatorioFacade.AdimplenciaVO> vos = null;
            DateTime? de  = Util.CTipos.CStringToDateTimeG(txtDe.Text);
            DateTime? ate = Util.CTipos.CStringToDateTimeG(txtAte.Text);

            DataTable dt = new DataTable();

            if (!chkIugu.Checked)
            {
                string asspjid = "0";
                if (cboAssociadoPJ.SelectedIndex > 0) asspjid = cboAssociadoPJ.SelectedValue;

                if (cboTipo.SelectedIndex == 0)
                    vos = RelatorioFacade.Instancia.RelatorioAdimplentes(asspjid, de, ate);
                else
                    vos = RelatorioFacade.Instancia.RelatorioInadimplentes(asspjid, de, ate);

                if (vos == null || vos.Count == 0) return;

                //HttpResponse response = HttpContext.Current.Response;
                //response.Clear();
                //response.Charset = "";

                //response.ContentType = "application/vnd.ms-excel";
                //response.AddHeader("Content-Disposition", "attachment;filename=\"relatorio_financeiro.xls\"");

                dt.Columns.Add("NumeroCartao");
                dt.Columns.Add("CNPJ");
                dt.Columns.Add("Titular");
                dt.Columns.Add("AssociadoPJ");
                dt.Columns.Add("ContratoADM");

                dt.Columns.Add("Vencimento");
                dt.Columns.Add("Parcela");
                dt.Columns.Add("Vidas");

                if (cboTipo.SelectedIndex == 0) //adimplentes
                {
                    dt.Columns.Add("ValorPago");
                    dt.Columns.Add("DataPagamento");
                }
                else
                {
                    dt.Columns.Add("ValorPendente");
                }

                //dt.Columns.Add("AssociadoPJ");

                foreach (var vo in vos)
                {
                    DataRow nova = dt.NewRow();

                    nova["NumeroCartao"] = string.Concat("'", vo.ContratoNumero);
                    nova["CNPJ"] = string.Concat("'", vo.BeneficiarioDocumento);
                    nova["Titular"] = vo.BeneficiarioNome;
                    nova["AssociadoPJ"] = vo.AssociadoPJ;
                    nova["ContratoADM"] = vo.ContratoADM;

                    nova["Parcela"] = vo.Parcela;

                    nova["Vidas"] = vo.CobrancaVidas;
                    nova["Vencimento"] = vo.CobrancaVencimento.ToString("dd/MM/yyyy");

                    if (cboTipo.SelectedIndex == 0) //adimplentes
                    {
                        nova["ValorPago"] = vo.CobrancaValorPago.ToString("C");
                        nova["DataPagamento"] = vo.CobrancaDataPago.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        nova["ValorPendente"] = vo.CobrancaValorPendente.ToString("C");
                    }

                    nova["AssociadoPJ"] = vo.EstipulanteNome;

                    dt.Rows.Add(nova);
                }
            }
            else
            {
                string contratoId = "";
                if (cboContrato.SelectedIndex > 0) contratoId = cboContrato.SelectedValue;

                vos = RelatorioFacade.Instancia.RelatorioIUGU(contratoId, de, ate);

                if (vos == null || vos.Count == 0) return;

                dt.Columns.Add("NumeroCartao");
                dt.Columns.Add("Titular");

                dt.Columns.Add("Vencimento");
                dt.Columns.Add("Pagamento");
                dt.Columns.Add("Cobertura");
                dt.Columns.Add("Produto");

                //dt.Columns.Add("AssociadoPJ");

                foreach (var vo in vos)
                {
                    DataRow nova = dt.NewRow();

                    nova["NumeroCartao"] = string.Concat("'", vo.ContratoNumero);
                    nova["Titular"] = vo.BeneficiarioNome;
                    nova["Vencimento"] = vo.CobrancaVencimento.ToString("dd/MM/yyyy");
                    nova["Pagamento"] = vo.CobrancaDataPago.ToString("dd/MM/yyyy");

                    nova["Cobertura"] = vo.TotalCoberturaValor.ToString("C");
                    nova["Produto"] = vo.TotalProdutoValor.ToString("C");

                    dt.Rows.Add(nova);
                }
            }

            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.Charset = "";

            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", "attachment;filename=\"relatorio_financeiro.xls\"");

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    DataGrid dg = new DataGrid();

                    dg.DataSource = dt;
                    dg.DataBind();
                    dg.RenderControl(htw);
                    response.Write(sw.ToString());
                    response.End();
                }
            }
        }

        protected void cboTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTipo.SelectedIndex == 0) //adimplentes
            {
                gridContratos.Columns[4].Visible = true;    // vencimento
                gridContratos.Columns[5].Visible = true;    // vidas
                gridContratos.Columns[6].Visible = true;    // valor pago
                gridContratos.Columns[7].Visible = true;    // data pago
                gridContratos.Columns[8].Visible = false;   // valor pendente
            }
            else
            {
                gridContratos.Columns[4].Visible = true;    // vencimento
                gridContratos.Columns[5].Visible = true;    // vidas
                gridContratos.Columns[6].Visible = false;   // valor pago
                gridContratos.Columns[7].Visible = false;   // data pago
                gridContratos.Columns[8].Visible = true;    // valor pendente
            }

            lnkToExcel.Visible = false;
            lnkToExcelT.Visible = false;
            gridContratos.DataSource = null;
            gridContratos.DataBind();
        }

        protected void chkIugu_CheckedChanged(object sender, EventArgs e)
        {
            pnlTipo.Visible = !chkIugu.Checked;
            pnlContrato.Visible = chkIugu.Checked;
            pnlAssociadoPJ.Visible = !chkIugu.Checked;

            pnl_Label_Contrato.Visible = chkIugu.Checked;
            pnl_Label_Tipo_e_AssociadoPj.Visible = !chkIugu.Checked;

            pnlResultadoIUGU.Visible = chkIugu.Checked;
            pnlResultadoPadrao.Visible = !chkIugu.Checked;

            if (chkIugu.Checked)
            {
                ////litAssociadoPJ_Contrato.Text = "Contrato:";
                //cboTipo.SelectedIndex = 0;
                //cboTipo.Enabled = false;
                if (gridIUGU.Rows.Count > 0)
                {
                    lnkToExcel.Visible = true;
                    lnkToExcelT.Visible = true;
                }
                else
                {
                    lnkToExcel.Visible = false;
                    lnkToExcelT.Visible = false;
                }
            }
            else
            {
                ////litAssociadoPJ_Contrato.Text = "Associado PJ:";
                //cboTipo.Enabled = true;

                if (gridContratos.Rows.Count > 0)
                {
                    lnkToExcel.Visible = true;
                    lnkToExcelT.Visible = true;
                }
                else
                {
                    lnkToExcel.Visible = false;
                    lnkToExcelT.Visible = false;
                }
            }
        }
    }
}