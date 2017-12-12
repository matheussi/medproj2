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

    public partial class importacao : System.Web.UI.Page
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
                this.ExibirFiliais(cboFilial, false);
                if (cboFilial.Items.Count > 0) cboFilial.SelectedIndex = 0;

                this.ExibirEstipulantes(cboEstipulante, true, true);
                this.ExibirOperadoras(cboOperadora, true, true);

                if (!string.IsNullOrEmpty(this.idAgenda)) { this.carregarAgenda(); }
            }
            else
            {
                Util.Geral.JSScript(this, "montaAnexos();");
            }
        }

        void carregarAgenda()
        {
            AgendaImportacao agenda = AgendaImportacaoFacade.Instancia.Carregar(Convert.ToInt64(this.idAgenda));

            txtDescricao.Text = agenda.Descricao;
            txtExecutarEm.Text = agenda.DataProcessamento.ToString("dd/MM/yyyy");
            cboFilial.SelectedValue = agenda.Filial.ID.ToString();
            cboEstipulante.SelectedValue = agenda.AssociadoPj.ID.ToString();
            cboOperadora.SelectedValue = agenda.Operadora.ID.ToString();
            cboOperadora_SelectedIndexChanged(null, null);
            cboContrato.SelectedValue = agenda.Contrato.ID.ToString();
            cboContrato_SelectedIndexChanged(null, null);
            cboPlano.SelectedValue = agenda.Plano.ID.ToString();
            chkNaoCriticarCpf.Checked = agenda.NaoCriticarCPF;

            if (agenda.DataConclusao.HasValue) cmdSalvar.Visible = false;

            pnlUpload.Visible = false;
            pnlDownload.Visible = true;

            string importpath = string.Concat(ConfigurationManager.AppSettings["appUrl"], @"files/import/");

            chkInativo.Checked = !agenda.Ativa;

            litDownload.Text = string.Concat(
                "<a href='", importpath, agenda.ID, Path.GetExtension(agenda.Arquivo),
                "'>Clique para fazer download do arquivo de importação</a>");

            if(!agenda.DataConclusao.HasValue) lnkArquivoLog.Visible = false;

            if (!string.IsNullOrEmpty(agenda.Erro))
            {
                litErro.Text = string.Concat("&nbsp;<a style='color:red'>(Erro: ", agenda.Erro, ")</a>");
            }

            if (agenda.ContratoPjId > 0)
            {
                txtContratoPJId.Value = agenda.ContratoPjId.ToString();

                var titular = Entity.ContratoBeneficiario.CarregarTitular(agenda.ContratoPjId, null);
                txtContratoPJ.Text = titular.BeneficiarioNome;
            }
        }

        Boolean HaItemSelecionado(DropDownList combo)
        {
            if (combo.Items.Count == 0) { return false; }

            return combo.SelectedValue != "0" &&
                   combo.SelectedValue != "-1" &&
                   combo.SelectedValue != "";
        }

        void ExibirFiliais(DropDownList combo, Boolean itemSELECIONE)
        {
            combo.Items.Clear();

            combo.DataValueField = "ID";
            combo.DataTextField = "Nome";
            combo.DataSource = Entity.Filial.CarregarTodas(true);
            combo.DataBind();

            if (itemSELECIONE) { combo.Items.Insert(0, new ListItem("Selecione", "-1")); }
        }

        void ExibirOperadoras(DropDownList combo, Boolean itemSELECIONE, Boolean somenteAtivas)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Nome";
            combo.DataSource = Entity.Operadora.CarregarTodas(somenteAtivas);
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        void ExibirEstipulantes(DropDownList combo, Boolean itemSELECIONE, Boolean apenasAtivos)
        {
            combo.Items.Clear();
            combo.DataValueField = "ID";
            combo.DataTextField = "Descricao";
            combo.DataSource = Entity.Estipulante.Carregar(apenasAtivos);
            combo.DataBind();

            if (itemSELECIONE)
            {
                combo.Items.Insert(0, new ListItem("Selecione", "-1"));
            }
        }

        void CarregaContratoADM()
        {
            cboContrato.Items.Clear();
            if (!HaItemSelecionado(cboEstipulante) || !HaItemSelecionado(cboOperadora))
            {
                cboContrato.Items.Clear();
                return;
            }

            cboContrato.DataValueField = "ID";
            cboContrato.DataTextField = "DescricaoCodigoSaudeDental";

            IList<Entity.ContratoADM> lista = null;
            lista = Entity.ContratoADM.Carregar(cboEstipulante.SelectedValue, cboOperadora.SelectedValue, true);

            cboContrato.DataSource = lista;
            cboContrato.DataBind();
            cboContrato.Items.Insert(0, new ListItem("selecione", "-1"));
        }

        void CarregaPlanos()
        {
            if (!HaItemSelecionado(cboContrato)) { cboPlano.Items.Clear(); /*cboAcomodacao.Items.Clear();*/ return; }
            IList<Entity.Plano> planos = Entity.Plano.CarregarPorContratoID(cboContrato.SelectedValue, true);

            cboPlano.Items.Clear();
            cboPlano.DataValueField = "ID";
            cboPlano.DataTextField = "DescricaoPlanoSubPlano";
            cboPlano.DataSource = planos;
            cboPlano.DataBind();
            cboPlano.Items.Insert(0, new ListItem("Selecione", "-1"));

            this.CarregaAcomodacoes();
        }

        void CarregaAcomodacoes()
        {
            if (cboPlano.SelectedIndex > 0)
            {
                Entity.Plano plano = new Entity.Plano(cboPlano.SelectedValue);
                plano.Carregar();

                //ExibirTiposDeAcomodacao(cboAcomodacao, plano.QuartoComum, plano.QuartoParticular, true);
            }
            else
            {
                //cboAcomodacao.Items.Clear();
                //cboAcomodacao.Items.Add(new ListItem("Selecione", "-1"));
            }
        }

        void ExibirTiposDeAcomodacao(DropDownList combo, Boolean comum, Boolean particular, Boolean itemSELECIONE)
        {
            combo.Items.Clear();
            if (itemSELECIONE)
                combo.Items.Add(new ListItem("Selecione", "-1"));

            if (comum)
                combo.Items.Add(new ListItem("QUARTO COLETIVO", "0"));

            if (particular)
                combo.Items.Add(new ListItem("QUARTO PARTICULAR", "1"));
        }

        protected void cboEstipulante_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CarregaContratoADM();
            this.CarregaPlanos();
        }

        protected void cboOperadora_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CarregaContratoADM();
            this.CarregaPlanos();
            //this.CarregaEstadoCivil();
        }

        protected void cboContrato_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CarregaPlanos();
            //this.CarregaOpcoesParaAgregadosOuDependentes();
        }

        protected void cboPlano_SelectedIndexChanged(object sender, EventArgs e)
        {
            //this.CarregaAcomodacoes();
        }


        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("importacoes.aspx");
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

            if (!HaItemSelecionado(cboFilial))
            {
                Util.Geral.Alerta(up, this, "A filial deve ser informada.");
                cboFilial.Focus();
                return;
            }

            if (!HaItemSelecionado(cboEstipulante))
            {
                Util.Geral.Alerta(up, this, "O associado pj deve ser informado.");
                cboEstipulante.Focus();
                return;
            }

            if (!HaItemSelecionado(cboOperadora))
            {
                Util.Geral.Alerta(up, this, "A operadora deve ser informada.");
                cboOperadora.Focus();
                return;
            }

            if (!HaItemSelecionado(cboContrato))
            {
                Util.Geral.Alerta(up, this, "O contrato deve ser informado.");
                cboContrato.Focus();
                return;
            }

            if (!HaItemSelecionado(cboPlano))
            {
                Util.Geral.Alerta(up, this, "O plano deve ser informado.");
                cboPlano.Focus();
                return;
            }

            //Só envia arquivo durante a criação da agenda, não permite alterar
            if (string.IsNullOrEmpty(this.idAgenda))
            {
                if (!fuArquivoCSV.HasFile)
                {
                    Util.Geral.Alerta(up, this, "Nenhum arquivo informado.");
                    return;
                }
                //if (listaNovosNomes.Value.Trim() == "")
                //{
                //Util.Geral.Alerta(up, this, "Nenhum arquivo informado.");
                //return;
                //}
            }
                

            #endregion

            try
            {
                AgendaImportacao agenda = null;

                if (string.IsNullOrEmpty(this.idAgenda))
                {
                    agenda = new AgendaImportacao();
                    agenda.InicializarInstancias();
                    agenda.Autor.ID = Convert.ToInt64(Util.UsuarioLogado.ID);
                    agenda.DataCriacao = DateTime.Now;
                }
                else
                {
                    agenda = AgendaImportacaoFacade.Instancia.Carregar(Convert.ToInt64(this.idAgenda));
                    agenda.Ativa = !chkInativo.Checked;

                    long autorId = agenda.Autor.ID;
                    agenda.InicializarInstancias();
                    agenda.Autor.ID = autorId;
                }

                agenda.Descricao            = txtDescricao.Text;
                agenda.DataProcessamento    = data;
                agenda.NaoCriticarCPF       = chkNaoCriticarCpf.Checked;

                agenda.AssociadoPj.ID       = Convert.ToInt64(cboEstipulante.SelectedValue);
                agenda.Contrato.ID          = Convert.ToInt64(cboContrato.SelectedValue);
                agenda.Filial.ID            = Convert.ToInt64(cboFilial.SelectedValue);
                agenda.Operadora.ID         = Convert.ToInt64(cboOperadora.SelectedValue);
                agenda.Plano.ID             = Convert.ToInt64(cboPlano.SelectedValue);

                if (txtContratoPJ.Text.Trim() != "" && txtContratoPJId.Value.Trim() != "")
                {
                    agenda.ContratoPjId = Convert.ToInt64(txtContratoPJId.Value);
                }

                AgendaImportacaoFacade.Instancia.Salvar(agenda);

                //Só envia arquivo durante a criação da agenda, não permite alterar
                if (string.IsNullOrEmpty(this.idAgenda)) 
                {
                    //string temppath = string.Concat(Server.MapPath("~"), @"\files\temp\");
                    string importpath = string.Concat(Server.MapPath("~"), @"\files\import\");

                    string arquivoEnviado = fuArquivoCSV.PostedFile.FileName; //listaNovosNomes.Value;
                    string arquivoASalvar = string.Concat(agenda.ID, Path.GetExtension(arquivoEnviado));

                    fuArquivoCSV.SaveAs(importpath + arquivoASalvar);

                    agenda.Arquivo = Path.GetFileName(arquivoEnviado);

                    AgendaImportacaoFacade.Instancia.Salvar(agenda);

                    //File.Move(temppath + arquivoEnviado, importpath + arquivoASalvar);
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

                Response.Redirect("importacoes.aspx");
            }
            catch
            {
                //Util.Geral.Alerta(this, "Erro inesperado. Por favor, tente novamente.");
                throw;
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
        }

        protected void lnkArquivoLog_Click(object sender, EventArgs e)
        {
            List<AgendaImportacaoItemLog> log = AgendaImportacaoFacade.Instancia.CarregarLog(Convert.ToInt64(this.idAgenda));

            if (log == null) return;

            var csvExp = new CsvExport();

            foreach (AgendaImportacaoItemLog item in log)
            {
                csvExp.AddRow();

                if (item.Titular != null && item.Titular.Contrato != null)
                {
                    csvExp["NumeroCartao"] = string.Concat("'", item.Titular.Contrato.Numero);//todo: denis discutir com marcio se é bom mostrar a senha
                    csvExp["Senha"] = string.Concat("'", item.Titular.Contrato.Senha);
                }
                else
                {
                    csvExp["NumeroCartao"] = "";
                    csvExp["Senha"]        = "";
                }

                csvExp["Linha"]         = (item.Linha + 1).ToString();
                csvExp["Status"]        = item.Status == AgendaImportacaoItemLogStatus.Erro ? "Erro" : "Ok";
                csvExp["Mensagem"]      = item.Mensagem;
            }

            string conteudo = csvExp.Export();
            string arquivo = "log_importacao.csv";

            Response.Clear();
            Response.ContentType = "application/CSV";
            Response.AddHeader("content-disposition", "attachment; filename=\"" + arquivo + "\"");
            Response.Write(conteudo);
            Response.End();

            #region txt tabulado 

            /*

            MemoryStream ms = new MemoryStream();
            TextWriter tw = new StreamWriter(ms);
            tw.Write("NumeroCartao\t\tSenha\t\t\tLinha\t\tStatus\t\tMensagem");

            foreach (AgendaImportacaoItemLog item in log)
            {
                tw.Write(Environment.NewLine);
                tw.Write(string.Concat("'", item.Titular.Contrato.Numero));
                tw.Write("\t");
                tw.Write(string.Concat("'", item.Titular.Contrato.Senha)); 
                tw.Write("\t\t\t");
                tw.Write((item.Linha + 1).ToString());
                tw.Write("\t\t");
                tw.Write(item.Status == AgendaImportacaoItemLogStatus.Erro ? "Erro" : "Ok");
                tw.Write(item.Mensagem);
            }

            tw.Flush();
            byte[] bytes = ms.ToArray();
            ms.Close();
            ms.Dispose();

            Response.Clear();
            Response.ContentType = "application/force-download";
            Response.AddHeader("content-disposition", "attachment;    filename=log_importacao.txt");
            Response.BinaryWrite(bytes);
            Response.End();

            */

            #endregion

            #region excel 
            /*

            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.Charset = "";

            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", "attachment;filename=\"file.xls\"");

            DataTable dt = new DataTable();
            dt.Columns.Add("NumeroCartao");
            dt.Columns.Add("Senha");
            dt.Columns.Add("Linha");
            dt.Columns.Add("Status");
            dt.Columns.Add("Mensagem");

            foreach (AgendaImportacaoItemLog item in log)
            {
                DataRow nova = dt.NewRow();

                if (item.Titular != null && item.Titular.Contrato != null)
                {
                    nova["Senha"] = string.Concat("'", item.Titular.Contrato.Senha); //todo: denis discutir com marcio se é bom mostrar a senha
                    nova["NumeroCartao"] = string.Concat("'", item.Titular.Contrato.Numero);
                }

                nova["Linha"]    = (item.Linha + 1).ToString();
                nova["Status"]   = item.Status == AgendaImportacaoItemLogStatus.Erro ? "Erro" : "Ok";
                nova["Mensagem"] = item.Mensagem;

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

            */
            #endregion
        }
    }
}