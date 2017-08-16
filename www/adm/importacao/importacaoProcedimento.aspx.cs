namespace MedProj.www.adm.importacao
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

    public partial class importacaoProcedimento : System.Web.UI.Page
    {
        string idAgenda
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
                this.ExibirPrestadores(cboPrestadores);
                this.ExibirTabelasDePreco(cboTabelaPreco, true);
                this.ExibirContratos();

                if (!string.IsNullOrEmpty(this.idAgenda)) this.carregarAgenda();
            }
        }

        void carregarAgenda()
        {
            AgendaAtribuicaoProcedimento agenda = AgendaAtribuicaoProcedimentoFacade.Instancia.Carregar(Convert.ToInt64(this.idAgenda));

            txtDescricao.Text = agenda.Descricao;
            txtExecutarEm.Text = agenda.DataProcessamento.ToString("dd/MM/yyyy");

            if(agenda.Tabela != null) cboTabelaPreco.SelectedValue = agenda.Tabela.ID.ToString();

            if (agenda.DataConclusao.HasValue) cmdSalvar.Visible = false;

            pnlUpload.Visible = false;
            pnlDownload.Visible = true;

            string importpath = string.Concat(ConfigurationManager.AppSettings["appUrl"], @"files/importProc/");

            chkInativo.Checked = !agenda.Ativa;

            litDownload.Text = string.Concat(
                "<a href='", importpath, agenda.ID, Path.GetExtension(agenda.Arquivo),
                "'>Clique para fazer download do arquivo de importação</a>");

            //lnkArquivoLog.Visible = false; //todo: ao fazer o log, remover esta linha

            if (!agenda.DataConclusao.HasValue)
            {
                lnkArquivoLog.Visible = false;
                cboTabelaPreco.Enabled = false;
            }
            else
                cmdSalvar.Visible = false;

            foreach (var contrato in agenda.Contratos)
            {
                lstContratosAdicionados.Items.Add(new ListItem(contrato.Nome, contrato.ID.ToString()));
            }


            if (!string.IsNullOrEmpty(agenda.Erro))
            {
                litErro.Text = string.Concat("&nbsp;<a style='color:red'>(Erro: ", agenda.Erro, ")</a>");
            }
        }

        Boolean HaItemSelecionado(DropDownList combo)
        {
            if (combo.Items.Count == 0) { return false; }

            return combo.SelectedValue != "0" &&
                   combo.SelectedValue != "-1" &&
                   combo.SelectedValue != "";
        }

        void ExibirTabelasDePreco(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();

            combo.DataValueField = "ID";
            combo.DataTextField = "Nome";
            combo.DataSource = TabelaPrecoFacade.Instancia.Carregar(string.Empty);
            combo.DataBind();

            if (itemSELECIONE) { combo.Items.Insert(0, new ListItem("Selecione", "-1")); }
        }

        void ExibirPrestadores(DropDownList combo)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Nome";
            combo.DataSource = PrestadorFacade.Instancia.CarregarPorNome(string.Empty, false);
            combo.DataBind();
        }

        void ExibirContratos()
        {
            cboContrato.Items.Clear();
            cboContrato.DataValueField = "ID";
            cboContrato.DataTextField = "Nome";
            cboContrato.DataSource = PrestadorUnidadeFacade.Instancia.CarregaPorPrestadorId(Convert.ToInt64(cboPrestadores.SelectedValue));
            cboContrato.DataBind();
        }

        protected void cboPrestadores_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ExibirContratos();
        }


        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("importacoesProcedimento.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            #region validacao

            if (txtDescricao.Text.Trim().Length < 10)
            {
                Util.Geral.Alerta(up, this, "A descrição deve ser informada com no mínimo 10 caractéres.");
                txtDescricao.Focus();
                return;
            }

            DateTime data = Util.CTipos.CStringToDateTime(txtExecutarEm.Text);

            if (data == DateTime.MinValue)
            {
                Util.Geral.Alerta(up, this, "Data inválida.");
                txtExecutarEm.Focus();
                return;
            }

            if (lstContratosAdicionados.Items.Count <= 0)
            {
                Util.Geral.Alerta(up, this, "Nenhum contrato adicionado.");
                return;
            }

            //Só envia arquivo durante a criação da agenda, não permite alterar
            if (string.IsNullOrEmpty(this.idAgenda))
            {
                if (listaNovosNomes.Value.Trim() == "")
                {
                    Util.Geral.Alerta(up, this, "Nenhum arquivo informado.");
                    return;
                }
            }

            #endregion

            try
            {
                AgendaAtribuicaoProcedimento agenda = null;

                if (string.IsNullOrEmpty(this.idAgenda))
                {
                    agenda = new AgendaAtribuicaoProcedimento();
                    agenda.Contratos = new List<PrestadorUnidade>();
                    agenda.Processado = false;
                }
                else
                {
                    agenda = AgendaAtribuicaoProcedimentoFacade.Instancia.Carregar(Convert.ToInt64(this.idAgenda));
                    agenda.Ativa = !chkInativo.Checked;
                }

                agenda.Descricao = txtDescricao.Text;
                agenda.DataProcessamento = data;

                foreach (ListItem item in lstContratosAdicionados.Items)
                {
                    // Só adiciona se ainda não está na coleção
                    if (agenda.Contratos.FirstOrDefault<PrestadorUnidade>(p => p.ID.ToString() == item.Value) == null)
                    {
                        agenda.Contratos.Add(
                            PrestadorUnidadeFacade.Instancia.CarregaPorId(Convert.ToInt64(item.Value)));
                    }
                }

                if (cboTabelaPreco.SelectedIndex > 0)
                {
                    agenda.Tabela = TabelaPrecoFacade.Instancia.Carregar(Convert.ToInt64(cboTabelaPreco.SelectedValue));
                }

                if (string.IsNullOrEmpty(this.idAgenda)) agenda.Arquivo = "";
                AgendaAtribuicaoProcedimentoFacade.Instancia.Salvar(agenda);

                //Só envia arquivo durante a criação da agenda, não permite alterar
                if (string.IsNullOrEmpty(this.idAgenda))
                {
                    string temppath = string.Concat(Server.MapPath("~"), @"\files\temp\");
                    string importpath = string.Concat(Server.MapPath("~"), @"\files\importProc\");

                    string arquivoEnviado = Util.Geral.RetiraAcentos(listaNovosNomes.Value);
                    string arquivoASalvar = string.Concat(agenda.ID, Path.GetExtension(arquivoEnviado));

                    agenda.Arquivo = Path.GetFileName(arquivoEnviado);

                    AgendaAtribuicaoProcedimentoFacade.Instancia.Salvar(agenda);

                    File.Move(temppath + arquivoEnviado, importpath + arquivoASalvar);
                }

                #region comentado...

                //FileStream stream = File.Open(importpath + arquivoASalvar, FileMode.Open, FileAccess.Read);

                ////1. Reading from a binary Excel file ('97-2003 format; *.xls)
                ////IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

                ////2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                //IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                ////3. DataSet - The result of each spreadsheet will be created in the result.Tables
                //DataSet result = excelReader.AsDataSet();

                ////4. DataSet - Create column names from first row
                //excelReader.IsFirstRowAsColumnNames = true;
                //DataSet ds = excelReader.AsDataSet();

                //string tblnm = "";
                //for (int i = 0; i < ds.Tables.Count; i++)
                //{
                //    tblnm = ds.Tables[i].TableName;
                //}
                #endregion

                Response.Redirect("importacoesProcedimento.aspx");
            }
            catch
            {
                 Util.Geral.Alerta(this, "Erro inesperado. Por favor, tente novamente.");
                //throw;
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
        }

        protected void lnkArquivoLog_Click(object sender, EventArgs e)
        {
            List<AgendaAtribProcedRESULTADO> log = 
                AgendaAtribuicaoProcedimentoFacade.Instancia.CarregarLog(Convert.ToInt64(this.idAgenda));

            if (log == null) return;

            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.Charset = "";

            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", "attachment;filename=\"file.xls\"");

            DataTable dt = new DataTable();
            dt.Columns.Add("OK");
            dt.Columns.Add("Mensagem");
            dt.Columns.Add("Procedimento");
            dt.Columns.Add("Linha");

            int linha = 1;
            foreach (AgendaAtribProcedRESULTADO item in log)
            {
                linha++;

                DataRow nova = dt.NewRow();

                if (item.Ok) nova["OK"] = "Sim"; else nova["OK"] = "Nao";

                nova["Mensagem"] = item.Mensagem;

                if (item.Procedimento != null)
                    nova["Procedimento"] = item.Procedimento.Nome;

                nova["Linha"] = linha.ToString();

                dt.Rows.Add(nova);
            }

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    DataGrid dg = new DataGrid();
                    //dg.AutoGenerateColumns = false;
                    dg.DataSource = dt;
                    dg.DataBind();
                    dg.RenderControl(htw);
                    response.Write(sw.ToString());
                    response.End();
                }
            }
        }

        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            if (cboPrestadores.Items.Count == 0) return;

            if (lstContratosAdicionados.Items.FindByValue(cboContrato.SelectedValue) != null) return;
            lstContratosAdicionados.Items.Add(new ListItem(cboContrato.SelectedItem.Text, cboContrato.SelectedValue));
        }

        protected void cmdRemove_Click(object sender, EventArgs e)
        {
            if (lstContratosAdicionados.Items.Count == 0) return;

            lstContratosAdicionados.Items.RemoveAt(lstContratosAdicionados.SelectedIndex);
        }

        protected void cmdAddAll_Click(object sender, EventArgs e)
        {
            lstContratosAdicionados.Items.Clear();
            for (int i = 0; i < cboContrato.Items.Count; i++)
            {
                cboContrato.SelectedIndex = i;
                cmdAdd_Click(null, null);
            }
        }
    }
}