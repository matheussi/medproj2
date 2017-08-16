namespace MedProj.www.adm.pagamento
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

    public partial class pagamento : System.Web.UI.Page
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
                txtExecutarEm.Text = DateTime.Now.ToString("dd/MM/yyyy");
                this.ExibirPrestadores();
                //if (cboFilial.Items.Count > 0) cboFilial.SelectedIndex = 0;

                //this.ExibirEstipulantes(cboEstipulante, true, true);
                //this.ExibirOperadoras(cboOperadora, true, true);

                if (!string.IsNullOrEmpty(this.idAgenda)) { this.carregarAgenda(); }
            }
        }

        void ExibirPrestadores()
        {
            cboPrestador.Items.Clear();
            cboPrestador.Items.Add(new ListItem("selecione", "-1"));
            List<PrestadorUnidade> lista = PrestadorUnidadeFacade.Instancia.Carrega();

            if (lista == null) return;

            foreach (PrestadorUnidade p in lista)
            {
                cboPrestador.Items.Add(new ListItem(
                    string.Concat(p.Owner.Nome, "(", p.Endereco, ")"), p.ID.ToString()));
            }
        }

        void carregarAgenda()
        {
            AgendaPagamento agenda = AgendaPagamentoFacade.Instancia.Carregar(Convert.ToInt64(this.idAgenda));

            txtDescricao.Text = agenda.Descricao;
            txtExecutarEm.Text = agenda.DataProcessamento.ToString("dd/MM/yyyy");

            if (agenda.DataConclusao.HasValue) cmdSalvar.Visible = false;

            //pnlUpload.Visible = false;
            //pnlDownload.Visible = true;

            txtDe.Text = agenda.PeriodoDe.ToString("dd/MM/yyyy");
            txtAte.Text = agenda.PeriodoAte.ToString("dd/MM/yyyy");

            if      (cboTipo.SelectedValue == "0")  agenda.TipoPagto = PeriodicidadePagto.Mensal;
            else if (cboTipo.SelectedValue == "1")  agenda.TipoPagto = PeriodicidadePagto.Quinzenal;
            else                                    agenda.TipoPagto = PeriodicidadePagto.Semanal;
            //cboTipo.SelectedIndex = agenda.TipoPagto == PeriodicidadePagto.Mensal ? 1 : 2;

            pnlDetalhe.Visible = true;
            chkInativo.Checked = !agenda.Ativa;

            //litDownload.Text = string.Concat(
            //    "<a href='", importpath, agenda.ID, Path.GetExtension(agenda.Arquivo),
            //    "'>Clique para fazer download do arquivo de importação</a>");

            if (!agenda.DataConclusao.HasValue) lnkArquivo.Visible = false;
            else                                cmdSalvar.Visible = false;

            //if (!string.IsNullOrEmpty(agenda.Erro))
            //{
            //    litErro.Text = string.Concat("&nbsp;<a style='color:red'>(Erro: ", agenda.Erro, ")</a>");
            //}
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("pagamentos.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            #region validacao 

            //if (cboPrestador.Items.Count <= 1)
            //{
            //    Util.Geral.Alerta(this, "Nenhum prestador selecionado.");
            //    cboPrestador.Focus();
            //    return;
            //}

            if (cboTipo.SelectedIndex <= 0)
            {
                Util.Geral.Alerta(this, "Selecione o tipo de pagamento.");
                cboTipo.Focus();
                return;
            }

            if (txtDescricao.Text.Trim() == "")
            {
                Util.Geral.Alerta(this, "Informe por favor a descrição.");
                txtDescricao.Focus();
                return;
            }

            DateTime dataExec = Util.CTipos.CStringToDateTime(txtExecutarEm.Text);

            if (dataExec == DateTime.MinValue)
            {
                Util.Geral.Alerta(up, this, "Data de execução inválida.");
                txtExecutarEm.Focus();
                return;
            }

            DateTime de = Util.CTipos.CStringToDateTime(txtDe.Text, 0, 0, 0, 500);
            DateTime ate = Util.CTipos.CStringToDateTime(txtAte.Text, 23, 59, 59, 990);

            if (de == DateTime.MinValue || ate == DateTime.MinValue || de > ate)
            {
                Util.Geral.Alerta(this, "Período de data inválido.");
                txtDe.Focus();
                return;
            }

            #endregion

            AgendaPagamento agenda = null;

            if (string.IsNullOrEmpty(this.idAgenda))
            {
                agenda = new AgendaPagamento();
            }
            else
            {
                agenda = AgendaPagamentoFacade.Instancia.Carregar(Convert.ToInt64(this.idAgenda));
                agenda.Ativa = !chkInativo.Checked;
            }

            agenda.DataProcessamento = dataExec;
            agenda.Descricao         = txtDescricao.Text;
            agenda.TipoPagto = cboTipo.SelectedValue == "0" ? PeriodicidadePagto.Mensal : PeriodicidadePagto.Quinzenal;
            agenda.PeriodoDe         = de;
            agenda.PeriodoAte        = ate;

            AgendaPagamentoFacade.Instancia.Salvar(agenda);

            Response.Redirect("pagamentos.aspx");
        }

        protected void lnkArquivo_Click(object sender, EventArgs e)
        {
            var agenda = AgendaPagamentoFacade.Instancia.CarregarCompleto(Convert.ToInt64(this.idAgenda));

            string arquivo = agenda.GerarArquivo();

            Response.Clear();
            Response.ClearHeaders();

            Response.AddHeader("Content-Length", arquivo.Length.ToString());
            Response.ContentType = "text/plain";
            Response.AppendHeader("content-disposition", "attachment;filename=\"remessa" + agenda.ID.ToString() + " .txt\"");

            Response.Write(arquivo);
            Response.End();
        }
    }
}