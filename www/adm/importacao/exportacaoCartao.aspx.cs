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

    public partial class exportacaoCartao : System.Web.UI.Page
    {
        long? idAgenda
        {
            get
            {
                if (string.IsNullOrEmpty(Request[Util.Keys.IdKey]))
                    return null;
                else
                    return Convert.ToInt64(Request[Util.Keys.IdKey]);
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

                if (this.idAgenda.HasValue) { this.carregarAgenda(); }
            }
        }

        void carregarAgenda()
        {
            AgendaExportacaoCartao agenda = AgendaExportacaoCartaoFacade.Instancia.Carregar(this.idAgenda.Value);

            txtDescricao.Text = agenda.Descricao;
            txtExecutarEm.Text = agenda.DataProcessamento.ToString("dd/MM/yyyy");
            cboFilial.SelectedValue = agenda.Filial.ID.ToString();
            cboEstipulante.SelectedValue = agenda.AssociadoPj.ID.ToString();
            cboOperadora.SelectedValue = agenda.Operadora.ID.ToString();
            cboOperadora_SelectedIndexChanged(null, null);
            cboContrato.SelectedValue = agenda.Contrato.ID.ToString();

            if (agenda.DataConclusao.HasValue) cmdSalvar.Visible = false;

            pnlDownload.Visible = true;

            //string importpath = string.Concat(ConfigurationManager.AppSettings["appUrl"], @"files/export/");

            chkInativo.Checked = !agenda.Ativa;

            if (!agenda.DataConclusao.HasValue) { lnkArquivoLog.Visible = false; lnkArquivoDados.Visible = false; }
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

        protected void cboEstipulante_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CarregaContratoADM();
        }

        protected void cboOperadora_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CarregaContratoADM();
        }

        /********************************************************************************************************/

        protected void lnkArquivoDados_Click(object sender, EventArgs e)
        {
            HttpExtensions.ForceDownload(HttpContext.Current.Response, @"~/files/export/" + this.idAgenda + ".txt", "dados.txt");
        }

        protected void lnkArquivoLog_Click(object sender, EventArgs e)
        {
            List<AgendaExportacaoCartaoItem> log = AgendaExportacaoCartaoFacade.Instancia.CarregarLog(this.idAgenda.Value);

            if (log == null) return;

            log = log.OrderBy(i => i.Titular.Beneficiario.Nome).ToList();

            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.Charset = "";

            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", "attachment;filename=\"log.xls\"");

            DataTable dt = new DataTable();
            dt.Columns.Add("CARTAO");
            dt.Columns.Add("ABREVIADO");
            dt.Columns.Add("RG"); //////////////////////

            dt.Columns.Add("PRODUTO");
            dt.Columns.Add("VIA");
            dt.Columns.Add("CVV");
            dt.Columns.Add("VALIDADE");
            dt.Columns.Add("SENHA");


            dt.Columns.Add("NOME_BENEFICIARIO");

            dt.Columns.Add("CONTRATOPJ");//////////////


            dt.Columns.Add("CPF_TITULAR");
            dt.Columns.Add("DT_NASCIMENTO");

            dt.Columns.Add("RAMO");
            dt.Columns.Add("APOLICE");

            dt.Columns.Add("INICIO_DO_RISCO");///////////////
            dt.Columns.Add("FIM_DA_VIGENCIA");//////////////
            dt.Columns.Add("DATA_DE_EMISSAO");//////////////

            dt.Columns.Add("LOGRADOURO");
            dt.Columns.Add("NUMERO");
            dt.Columns.Add("COMPLEMENTO");
            dt.Columns.Add("BAIRRO");
            dt.Columns.Add("CEP");
            dt.Columns.Add("CIDADE");
            dt.Columns.Add("UF");
            dt.Columns.Add("MATRICULA");

            dt.Columns.Add("PATH");

            ContratoADM contratoadm = null;
            NumeroCartao cartao = null;
            foreach (AgendaExportacaoCartaoItem item in log)
            {
                DataRow nova = dt.NewRow();

                contratoadm = ContratoAdmFacade.Instance.Carregar(item.Titular.Contrato.ContratoADMID);
                cartao = NumeroCartaoFacade.Instancia.Carregar(item.Titular.Contrato.NumeroID);

                //if (item.Titular != null && item.Titular.Contrato != null)
                //{
                //    if(item.Titular.Contrato.Numero.StartsWith("0"))
                        nova["CARTAO"] = string.Concat("'", item.Titular.Contrato.Numero);
                    //else
                    //    nova["CARTAO"] = item.Titular.Contrato.Numero;
                //}

                nova["CPF_TITULAR"] = string.Concat("'", item.Titular.Beneficiario.CPF);

                if (item.Titular != null && item.Titular.Contrato != null)
                {
                    if (item.Titular.Contrato.Senha.StartsWith("0"))
                        nova["SENHA"] = string.Concat("'", item.Titular.Contrato.Senha);
                    else
                        nova["SENHA"] = item.Titular.Contrato.Senha;

                    if (!string.IsNullOrEmpty(item.Titular.Contrato.Ramo))
                        nova["RAMO"] = item.Titular.Contrato.Ramo;

                    if (!string.IsNullOrEmpty(item.Titular.Contrato.NumeroApolice))
                        nova["APOLICE"] = item.Titular.Contrato.NumeroApolice;
                }

                if (item.Titular != null && item.Titular.Beneficiario != null)
                {
                    nova["DT_NASCIMENTO"]       = item.Titular.Beneficiario.DataNascimento.ToString("dd/MM/yyyy"); 
                    nova["NOME_BENEFICIARIO"]   = item.Titular.Beneficiario.Nome;
                    nova["ABREVIADO"]           = item.Abreviar2(item.Titular.Beneficiario.Nome);
                    nova["RG"]                  = item.Titular.Beneficiario.RG;////////////////
                }

                nova["CONTRATOPJ"]  = contratoadm.Descricao;
                nova["PRODUTO"]     = item.Titular.Contrato.Produto;

                if (item.Titular != null && item.Titular.Contrato != null && item.Titular.Contrato.EnderecoReferencia != null && item.Titular.Contrato.EnderecoReferencia.GetType().ToString() != "EnderecoProxy")
                {
                    nova["LOGRADOURO"]  = item.Titular.Contrato.EnderecoReferencia.Logradouro;
                    nova["NUMERO"]      = item.Titular.Contrato.EnderecoReferencia.Numero;
                    nova["COMPLEMENTO"] = item.Titular.Contrato.EnderecoReferencia.Complemento;
                    nova["BAIRRO"]      = item.Titular.Contrato.EnderecoReferencia.Bairro;
                    nova["CEP"]         = item.Titular.Contrato.EnderecoReferencia.CEP;
                    nova["CIDADE"]      = item.Titular.Contrato.EnderecoReferencia.Cidade;
                    nova["UF"]          = item.Titular.Contrato.EnderecoReferencia.UF;
                }

                if (item.Titular != null && item.Titular.Contrato != null)
                    nova["MATRICULA"] = item.Titular.Contrato.Matricula;

                //nova["PRODUTO"] = item.Titular.Contrato.EstipulanteID

                nova["VIA"] = cartao.Via;
                nova["CVV"] = cartao.CV;
                nova["VALIDADE"] = "CONSULTE NOSSO SITE";

                if (item.Titular.Contrato.DataVigencia != DateTime.MinValue)
                {
                    nova["INICIO_DO_RISCO"] = item.Titular.Contrato.DataVigencia.ToString("dd/MM/yyyy");
                    nova["FIM_DA_VIGENCIA"] = item.Titular.Contrato.DataVigencia.AddYears(1).AddDays(-1).ToString("dd/MM/yyyy"); //item.Titular.Contrato.DataValidade.ToString("dd/MM/yyyy"); ???
                }

                if(item.Titular.Contrato.DataAdmissao != DateTime.MinValue)
                    nova["DATA_DE_EMISSAO"] = item.Titular.Contrato.DataAdmissao.ToString("dd/MM/yyyy");

                nova["PATH"] = item.Titular.Contrato.CaminhoArquivo;

                dt.Rows.Add(nova);
            }

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

        /********************************************************************************************************/

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("exportacoesCartao.aspx");
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

            #endregion

            try
            {
                AgendaExportacaoCartao agenda = null;

                if (!this.idAgenda.HasValue)
                {
                    agenda = new AgendaExportacaoCartao();
                    agenda.InicializarInstancias();
                    agenda.Autor.ID = Convert.ToInt64(Util.UsuarioLogado.ID);
                    agenda.DataCriacao = DateTime.Now;
                }
                else
                {
                    agenda = AgendaExportacaoCartaoFacade.Instancia.Carregar(this.idAgenda.Value);
                    agenda.Ativa = !chkInativo.Checked;

                    long autorId = agenda.Autor.ID;
                    agenda.InicializarInstancias();
                    agenda.Autor.ID = autorId;
                }

                agenda.Descricao = txtDescricao.Text;
                agenda.DataProcessamento = data;

                agenda.AssociadoPj.ID = Convert.ToInt64(cboEstipulante.SelectedValue);
                agenda.Contrato.ID = Convert.ToInt64(cboContrato.SelectedValue);
                agenda.Filial.ID = Convert.ToInt64(cboFilial.SelectedValue);
                agenda.Operadora.ID = Convert.ToInt64(cboOperadora.SelectedValue);

                AgendaExportacaoCartaoFacade.Instancia.Salvar(agenda);

                Response.Redirect("exportacoesCartao.aspx");
            }
            catch
            {
                Util.Geral.Alerta(this, "Erro inesperado. Por favor, tente novamente.");
            }
        }
    }
}