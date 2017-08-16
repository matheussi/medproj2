namespace MedProj.www.adm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Facade;
    using LC.Web.PadraoSeguros.Entity;
    using System.Collections;
    using System.Data;
    using System.Net.Mail;
    using System.Configuration;
    using System.Text;
    using System.Security.Cryptography;
    using System.IO;

    public partial class atendRelatorio : System.Web.UI.Page
    {
        DataTable dataCache
        {
            get { return Session["dataCache"] as DataTable; }
            set
            {
                if (value == null) { Session.Remove("dataCache"); }
                else { Session["dataCache"] = value; }
            }
        }

        void carregarOperadoras()
        {
            this.ExibirOperadoras(cboOperadora);
        }

        void ExibirOperadoras(ListBox list)
        {
            list.Items.Clear();
            list.DataValueField = "ID";
            list.DataTextField = "Nome";
            list.DataSource = Operadora.CarregarTodas(true);
            list.DataBind();
        }

        void carregarCategorias()
        {
            cboCategoria.DataValueField = "ID";
            cboCategoria.DataTextField = "Descricao";
            cboCategoria.DataSource = AtendimentoTipo.CarregarTodos();
            cboCategoria.DataBind();

            cboCategoria.Items.Insert(0, new ListItem("Todas", "-1"));
        }

        String[] PegaIDsSelecionados(ListBox lst)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (ListItem item in lst.Items)
            {
                if (item.Selected && item.Value != "-1")
                {
                    if (sb.Length > 0) { sb.Append(","); }
                    sb.Append(item.Value);
                }
            }

            if (sb.Length == 0)
                return null;
            else
                return sb.ToString().Split(',');
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dataCache = null;
                this.carregarCategorias();
                this.carregarOperadoras();
                AtendimentoTemp.UI.FillCombo(cboSubCategoria);
                cboSubCategoria.Items.Insert(0, new ListItem("Todas", "-1"));
            }
        }

        protected void cmdLocalizar_Click(Object sender, EventArgs e)
        {
            DateTime de = Util.CTipos.CStringToDateTime(txtDe.Text);
            DateTime ate = Util.CTipos.CStringToDateTime(txtAte.Text);

            if (de == DateTime.MinValue || ate == DateTime.MinValue || ate < de)
            {
                Util.Geral.Alerta(null, this, "_err", "Informe o intervalo de data.");
                return;
            }

            Object categoriaId = null;
            Object subcategoriaId = null;
            String abertoPor = null;

            if (cboCategoria.SelectedIndex > 0)
                categoriaId = cboCategoria.SelectedValue;

            if (cboSubCategoria.SelectedIndex > 0)
                subcategoriaId = cboSubCategoria.SelectedValue;

            if (txtAbertoPor.Text.Trim() != "")
                abertoPor = txtAbertoPor.Text;

            String[] operadoraIds = this.PegaIDsSelecionados(cboOperadora);
            if (operadoraIds == null || operadoraIds.Length == 0)
            {
                int total = cboOperadora.Items.Count;
                operadoraIds = new String[total];
                for (int i = 0; i < total; i++) { operadoraIds[i] = cboOperadora.Items[i].Value; }
            }

            dataCache = AtendimentoTemp.CarregarDataTable(
                categoriaId, subcategoriaId,
                operadoraIds, abertoPor,
                de, ate, Convert.ToInt32(cboStatuc.SelectedValue));

            grid.DataSource = dataCache;
            grid.DataBind();

            if (grid.Rows.Count == 0)
            {
                cmdToExcel.Visible = false;
                litAviso.Text = "nenhum atendimento encontrado.";
            }
            else
            {
                cmdToExcel.Visible = false; // true;
                litAviso.Text = "";
            }
        }

        protected void grid_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.cmdLocalizar_Click(null, null);
        }
        protected void grid_RowCommand(Object sender, GridViewCommandEventArgs e)
        {
        }
        protected void grid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //AtendimentoTemp obj = e.Row.DataItem as AtendimentoTemp;
                //if (obj != null)
                //{
                //    if (String.IsNullOrEmpty(obj.ResolvidoPor) && obj.DataPrevisao < DateTime.Now)
                //    {
                //        e.Row.ForeColor = System.Drawing.Color.Red;
                //        e.Row.ToolTip = "atrasado";
                //    }
                //}

                DataRowView obj = e.Row.DataItem as DataRowView;
                if (obj != null)
                {
                    if (Util.CTipos.CToString(obj["ResolvidoPor"]) == "" && obj["DataPrevisao"] != DBNull.Value && Convert.ToDateTime(obj["DataPrevisao"]) < DateTime.Now)
                    {
                        e.Row.ForeColor = System.Drawing.Color.Red;
                        e.Row.ToolTip = "atrasado";
                    }

                    if (obj["strDataFim"] == DBNull.Value || Convert.ToDateTime(obj["strDataFim"]) == DateTime.MinValue)
                        e.Row.Cells[7].Text = "";

                    if (obj["DataCancelamento"] == DBNull.Value || Convert.ToDateTime(obj["DataCancelamento"]) == DateTime.MinValue)
                        e.Row.Cells[3].Text = "";
                }
            }
        }

        protected void cmdToExcel_Click(Object sende, EventArgs e)
        {
            //DataTable dt = new DataTable();
            //dt.Columns.Add("Nome");
            //DataRow row = dt.NewRow();
            //dt.Rows.Add(row);

            //grid.DataSource = dt; //filtraValueObjects(true);
            //grid.DataBind();

            if (dataCache != null)
            {
                foreach (DataRow row in dataCache.Rows)
                {
                    if (Convert.ToString(row["ContratoNumero"]).StartsWith("0"))
                    {
                        row["ContratoNumero"] = String.Concat("'", Convert.ToString(row["ContratoNumero"]));
                    }

                    if (row["Saude"] != DBNull.Value && Convert.ToString(row["Saude"]).Trim() != "")
                    {
                        row["Saude"] = String.Concat("'", Convert.ToString(row["Saude"]));
                    }
                }

                grid.DataSource = dataCache;
                grid.DataBind();
            }

            String attachment = "attachment; filename=file.xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }
    }
}