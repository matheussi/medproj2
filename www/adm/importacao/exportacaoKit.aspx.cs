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

    public partial class exportacaoKit : System.Web.UI.Page
    {
        long? idAgenda
        {
            get
            {
                return Geral.IdEnviadoNull(HttpContext.Current, Keys.IdKey);
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
            AgendaExportacaoKit agenda = AgendaExportacaoKitFacade.Instancia.Carregar(this.idAgenda.Value);

            txtDescricao.Text = agenda.Descricao;
            txtExecutarEm.Text = agenda.DataProcessamento.ToString("dd/MM/yyyy");
            cboFilial.SelectedValue = agenda.Filial.ID.ToString();
            cboEstipulante.SelectedValue = agenda.AssociadoPj.ID.ToString();
            cboOperadora.SelectedValue = agenda.Operadora.ID.ToString();
            cboOperadora_SelectedIndexChanged(null, null);
            cboContrato.SelectedValue = agenda.Contrato.ID.ToString();

            if (agenda.DataConclusao.HasValue) cmdSalvar.Visible = false;

            pnlDownload.Visible = true;

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
            try
            {
                HttpExtensions.ForceDownload(HttpContext.Current.Response, @"~/files/exportKit/" + this.idAgenda + ".csv", "dados.csv");
            }
            catch
            {
                Util.Geral.Alerta(this, "Erro inesperado.");
            }
        }

        protected void lnkArquivoLog_Click(object sender, EventArgs e)
        {
            List<AgendaExportacaoKitItemLog> log = AgendaExportacaoKitFacade.Instancia.CarregarLog(this.idAgenda.Value);

            if (log == null) return;

            HttpResponse response = HttpContext.Current.Response;
            response.Clear();
            response.Charset = "";

            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", "attachment;filename=\"log.xls\"");

            DataTable dt = new DataTable();
            dt.Columns.Add("NumeroCartao");
            dt.Columns.Add("Titular");
            dt.Columns.Add("CPF");

            foreach (AgendaExportacaoKitItemLog item in log)
            {
                DataRow nova = dt.NewRow();

                if (item.Titular != null && item.Titular.Contrato != null)
                    nova["NumeroCartao"] = string.Concat("'", item.Titular.Contrato.Numero);

                nova["Titular"] = item.Titular.Beneficiario.Nome;
                nova["CPF"] = string.Concat("'", item.Titular.Beneficiario.CPF);

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
            Response.Redirect("exportacoesKit.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            #region validacao

            if (txtDescricao.Text.Trim().Length < 5)
            {
                Util.Geral.Alerta(up, this, "A descrição deve ser informada com no mínimo 5 caractéres.");
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
                AgendaExportacaoKit agenda = null;

                if (!this.idAgenda.HasValue)
                {
                    agenda = new AgendaExportacaoKit();
                    agenda.InicializarInstancias();
                    agenda.Autor.ID = Convert.ToInt64(Util.UsuarioLogado.ID);
                    agenda.DataCriacao = DateTime.Now;
                }
                else
                {
                    agenda = AgendaExportacaoKitFacade.Instancia.Carregar(this.idAgenda.Value);
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

                AgendaExportacaoKitFacade.Instancia.Salvar(agenda);

                Response.Redirect("exportacoesKit.aspx");
            }
            catch
            {
                Util.Geral.Alerta(this, "Erro inesperado. Por favor, tente novamente.");
            }
        }
    }
}