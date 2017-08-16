namespace MedProj.www.financeiro.cnab
{
    using System;
    using System.IO;
    using System.Web;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj.Entidades;
    using MedProj.Entidades.Enuns;
    using LC.Web.PadraoSeguros.Facade;
    using System.Configuration;

    public partial class cnab : System.Web.UI.Page
    {
        List<Cobranca> m_cobrancas
        {
            get
            {
                return Session["m_cob"] as List<Cobranca>;
            }
            set
            {
                if (value == null)
                    Session.Remove("m_cob");
                else
                    Session["m_cob"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlPendencias.Visible = true;
                pnlArquivosCNAB.Visible = false;

                txtDataAtePendentes.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtDataDePendentes.Text = DateTime.Now.AddDays(-5).ToString("dd/MM/yyyy");

                txtDataAte.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtDataDe.Text = DateTime.Now.AddDays(-8).ToString("dd/MM/yyyy");
            }
        }

        protected void cmdPesquisarPendentes_click(object sender, EventArgs e)
        {
            this.pesquisar();
        }

        void pesquisar()
        {
            TipoFiltroPendenciaCNAB tipo = TipoFiltroPendenciaCNAB.Hoje;
            if (cboTipoFiltroPendencias.SelectedValue == "1") tipo = TipoFiltroPendenciaCNAB.Ontem;
            else if (cboTipoFiltroPendencias.SelectedValue == "2") tipo = TipoFiltroPendenciaCNAB.Periodo;

            DateTime de = Util.CTipos.CStringToDateTime(txtDataDePendentes.Text);
            DateTime ate = Util.CTipos.CStringToDateTime(txtDataAtePendentes.Text, 23, 59, 59, 995);

            if (tipo == TipoFiltroPendenciaCNAB.Periodo && (de == DateTime.MinValue || ate == DateTime.MinValue || de > ate))
            {
                Util.Geral.Alerta(this, "Período de data inválido.");
                return;
            }

            List<Cobranca> lista = null;

            if(optNovos.Checked)
                lista = CobrancaFacade.Instancia.CarregarPendenciasCNAB(tipo, de, ate);
            else
                lista = CobrancaFacade.Instancia.CarregarPendenciasCNAB_Alteracoes(tipo, de, ate);

            litPendenciasMsg.Text = "";
            cmdGerarCnab.Visible = true;

            if (lista == null || lista.Count == 0)
            {
                this.m_cobrancas = null;
                cmdGerarCnab.Visible = false;
                litPendenciasMsg.Text = "Nenhuma cobrança pendente";
            }
            else
                this.m_cobrancas = lista;

            grid.DataSource = this.m_cobrancas;
            grid.DataBind();
        }

        protected void opt_CheckedChanged(object sender, EventArgs e)
        {
            pnlPendencias.Visible = optPendencias.Checked == true;
            pnlArquivosCNAB.Visible = optArquivosCNAB.Checked == true;
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            }
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

            if (e.CommandName.Equals("Baixar"))
            {
            }
            else if (e.CommandName.Equals("Ver"))
            {
            }
        }

        protected void cmdGerarCnab_Click(object sender, EventArgs e)
        {
            if (this.m_cobrancas == null || this.m_cobrancas.Count == 0) return;

            int i = -1; CheckBox ochk = null;
            List<string> ids = new List<string>();

            foreach (var c in this.m_cobrancas)
            {
                i++;

                ochk = grid.Rows[i].Cells[0].Controls[1] as CheckBox;
                if (ochk.Checked == false) continue;

                ids.Add(Convert.ToString(grid.DataKeys[i][0]));
            }

            if (ids.Count > 0)
            {
                if (optNovos.Checked)
                {
                    CobrancaFacade.Instancia.GerarArquivoCNAB(ids.ToArray());
                }
                else
                {
                    CobrancaFacade.Instancia.GerarArquivoCNAB(ids.ToArray(), true);

                    //alerta temporario
                    //Util.Geral.Alerta(this, "A implementação CNAB de alteração está pendente, é necessário alterar os títulos no sistema bancário.");
                }

                this.pesquisar();
            }
            else
            {
                Util.Geral.Alerta(this, "Nenhum item selecionado.");
            }
        }
        
        //<!-------------------------------------------------------------------------------->//

        protected void gridCnabs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Baixar"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(gridCnabs, e, 0);
                var remessa = CobrancaFacade.Instancia.CarregarRemessa(id);

                //string url = string.Concat(
                //    ConfigurationManager.AppSettings["appUrl"],
                //    "files/remessa/",
                //    Path.GetFileName(remessa.Arquivo));

                byte[] Content = File.ReadAllBytes(remessa.Arquivo);
                Response.ContentType = "text";
                Response.AddHeader("content-disposition", "attachment; filename=" + Path.GetFileName(remessa.Arquivo));
                Response.BufferOutput = true;;
                Response.OutputStream.Write(Content, 0, Content.Length);
                Response.End();

                this.consultarCNABs();
            }
            else if (e.CommandName.Equals("Ver"))
            {
            }
        }

        protected void gridCnabs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //LinkButton lb = e.Row.Cells[0].Controls[0] as LinkButton;
                ////ScriptManager.GetCurrent(this).RegisterAsyncPostBackControl(lb);
                //ScriptManager.GetCurrent(this).RegisterPostBackControl(lb);

                LinkButton buttonEditarCadastro = (LinkButton)e.Row.FindControl("btnEditaCadastro");
                buttonEditarCadastro.CommandArgument = e.Row.RowIndex.ToString();
                ScriptManager.GetCurrent(this).RegisterPostBackControl(buttonEditarCadastro);
            }
        }


        protected void cmdVisualizar_Click(object sender, EventArgs e)
        {
            this.consultarCNABs();
        }

        void consultarCNABs()
        {
            DateTime de = Util.CTipos.CStringToDateTime(txtDataDe.Text);
            DateTime ate = Util.CTipos.CStringToDateTime(txtDataAte.Text, 23, 59, 59, 995);

            if (de == DateTime.MinValue || ate == DateTime.MinValue || de > ate)
            {
                Util.Geral.Alerta(this, "Período de data inválido.");
                return;
            }

            TipoRemessaCnab tipo = TipoRemessaCnab.Novo;
            if (optEdicao.Checked) tipo = TipoRemessaCnab.Alteracao;

            List<Remessa> remessa = CobrancaFacade.Instancia.CarregarRemessa(de, ate, tipo);

            gridCnabs.DataSource = remessa;
            gridCnabs.DataBind();
        }
    }
}