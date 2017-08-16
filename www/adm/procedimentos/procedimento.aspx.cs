using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MedProj.Entidades;
using MedProj.www.Util;
using LC.Web.PadraoSeguros.Facade;

namespace MedProj.www.adm.procedimentos
{
    public partial class procedimento : System.Web.UI.Page
    {
        long IdProcedimento
        {
            set { ViewState["idproc"] = value; }
            get { return CTipos.CToLong(ViewState["idproc"]); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtProcedimentoCH.Attributes.Add("onkeyup", "mascara('" + txtProcedimentoCH.ClientID + "')");

            if (!IsPostBack)
            {
                this.carregarSegmentos();

                if (Request[Keys.IdKey] != null)
                {
                    this.carregarTabela();
                    pnlProcedimentos.Visible = true;
                    this.carregarProcedimentos(null);

                    if (!string.IsNullOrEmpty(Request["msg"]))
                    {
                        Geral.Alerta(this, "Tabela salva com sucesso.");
                    }
                }
            }
        }

        void carregarTabela()
        {
            long id = Convert.ToInt64(Request[Keys.IdKey]);

            TabelaProcedimento tabela = TabelaProcedimentoFacade.Instancia.Carregar(id);

            txtCodigo.Text = tabela.Codigo.ToString();
            txtTabela.Text = tabela.Nome;

            if (tabela.Segmento != null && tabela.Segmento.ID > 0)
                cboSegmento.SelectedValue = tabela.Segmento.ID.ToString();

            //using (Contexto contexto = new Contexto())
            //{
            //    TabelaProcedimento tabela = contexto.TabelasProcedimento
            //        .Include("Segmento")
            //        .Where(t => t.ID == id)
            //        .Single();

            //    txtCodigo.Text = tabela.Codigo.ToString();
            //    txtTabela.Text = tabela.Nome;

            //    if (tabela.Segmento != null && tabela.Segmento.ID > 0)
            //        cboSegmento.SelectedValue = tabela.Segmento.ID.ToString();
            //}
        }

        void carregarSegmentos()
        {
            //long id = Convert.ToInt64(Request[Keys.IdKey]);

            cboSegmento.DataValueField = "ID";
            cboSegmento.DataTextField = "Nome";

            cboSegmento.DataSource = SegmentoFacade.Instancia.CarregarTodos(null);
            cboSegmento.DataBind();

            //using (Contexto contexto = new Contexto())
            //{
            //    cboSegmento.DataSource = contexto.Segmentos.OrderBy(s => s.Nome).ToList();
            //    cboSegmento.DataBind();
            //}
        }

        void carregarProcedimentos(string nome)
        {
            long id = Convert.ToInt64(Request[Keys.IdKey]);
            List<Procedimento> procedimentos = ProcedimentoFacade.Instancia.CarregarPorTabela(id, nome);

            grid.DataSource = procedimentos;
            grid.DataBind();

            //using (Contexto contexto = new Contexto())
            //{
            //    if (string.IsNullOrEmpty(nome))
            //    {
            //        procedimentos = contexto.Procedimentos
            //            .Where(p => p.Tabela.ID == id)
            //            .OrderBy(p => p.Nome)
            //            .ToList();
            //    }
            //    else
            //    {
            //        procedimentos = contexto.Procedimentos
            //            .Where(p => p.Tabela.ID == id && p.Nome.Contains(nome))
            //            .OrderBy(p => p.Nome)
            //            .ToList();
            //    }

            //    grid.DataSource = procedimentos;
            //    grid.DataBind();
            //}
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("procedimentos.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (txtTabela.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome da tabela.");
                txtTabela.Focus();
                return;
            }

            if (cboSegmento.Items.Count == 0)
            {
                Geral.Alerta(this, "Informe o segmento da tabela.");
                cboSegmento.Focus();
                return;
            }

            long id = Geral.IdEnviado(this.Context, Keys.IdKey);

            TabelaProcedimento tabela = null;
            long idSeg = Convert.ToInt64(cboSegmento.SelectedValue);

            tabela = new TabelaProcedimento();
            if (id != 0) tabela.ID = id;
            else         tabela.Data = DateTime.Now;

            tabela.Codigo = CTipos.CToInt(txtCodigo.Text);
            tabela.Nome = txtTabela.Text;

            tabela.Segmento = new Segmento();
            tabela.Segmento.ID = idSeg;

            TabelaProcedimentoFacade.Instancia.Salvar(tabela);

            //using (Contexto contexto = new Contexto())
            //{
            //    if (id != 0)
            //    {
            //        tabela = contexto.TabelasProcedimento.Include("Segmento").Where(t => t.ID == id).Single();

            //        tabela.Codigo = CTipos.CToInt(txtCodigo.Text);
            //        tabela.Nome = txtTabela.Text;
            //        tabela.Segmento = contexto.Segmentos.Where(s => s.ID == idSeg).Single();
            //    }
            //    else
            //    {
            //        tabela = new TabelaProcedimento();

            //        tabela.Codigo = CTipos.CToInt(txtCodigo.Text);
            //        tabela.Data = DateTime.Now;
            //        tabela.Nome = txtTabela.Text;
            //        tabela.Segmento = contexto.Segmentos.Where(s => s.ID == idSeg).Single();

            //        contexto.TabelasProcedimento.Add(tabela);
            //    }

            //    contexto.SaveChanges();
            //}

            if (id == 0) Response.Redirect("procedimento.aspx?msg=1&" + Keys.IdKey + "=" + tabela.ID.ToString());
            else Geral.Alerta(this, "Tabela salva com sucesso.");
        }

        //----------------------------------------------------------------------------------------

        protected void cmdAdd_Click(object sender, EventArgs e)
        {
            #region validacao 

            if (txtProcedimento.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome do procedimento.");
                txtProcedimento.Focus();
                return;
            }

            if (txtEspecTxt.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe a especialidade.");
                txtEspecTxt.Focus();
                return;
            }

            if (txtCategoria.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe a categoria.");
                txtCategoria.Focus();
                return;
            }

            decimal ch = CTipos.ToDecimal(txtProcedimentoCH.Text);
            string codigo = txtProcedimentoCodigo.Text;

            if (ch == 0)
            {
                Geral.Alerta(this, "Informe o valor de CH do procedimento.");
                txtProcedimentoCH.Focus();
                return;
            }
            #endregion

            long idTabela = Geral.IdEnviado(this.Context, Keys.IdKey);

            Procedimento obj = new Procedimento();

            obj.CH        = ch;
            obj.Codigo    = codigo;
            obj.Nome      = txtProcedimento.Text;
            obj.Tabela   = TabelaProcedimentoFacade.Instancia.Carregar(idTabela);
            obj.Categoria = txtCategoria.Text;
            obj.Especialidade = txtEspecTxt.Text;

            if (this.IdProcedimento > 0) obj.ID = this.IdProcedimento;

            ProcedimentoFacade.Instancia.Salvar(obj, CTipos.CTipo<long>(cboSegmento.SelectedValue), true);

            Geral.Alerta(this, "Procedimento salvo com sucesso.");

            this.carregarProcedimentos(txtLocalizar.Text);

            txtProcedimento.Text = "";
            txtProcedimentoCH.Text = "0,00";
            txtProcedimentoCodigo.Text = "0";
            txtCategoria.Text = "";
            txtEspecTxt.Text = "";
            this.IdProcedimento = 0;
        }

        protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.carregarProcedimentos(txtLocalizar.Text);
        }

        protected void cmdLocalizar_Click(object sender, EventArgs e)
        {
            this.carregarProcedimentos(txtLocalizar.Text);
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                this.IdProcedimento = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                Procedimento proc = ProcedimentoFacade.Instancia.Carregar(this.IdProcedimento);

                txtProcedimento.Text = proc.Nome;
                txtProcedimentoCH.Text = proc.CH.ToString("N2");
                txtProcedimentoCodigo.Text = proc.Codigo;

                txtCategoria.Text = proc.Categoria;
                txtEspecTxt.Text = proc.Especialidade;

                //using (Contexto contexto = new Contexto())
                //{
                //    Procedimento proc = contexto.Procedimentos.Find(this.IdProcedimento);

                //    txtProcedimento.Text = proc.Nome;
                //    txtProcedimentoCH.Text = proc.CH.ToString("N2");
                //    txtProcedimentoCodigo.Text = proc.Codigo.ToString();
                //}
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long id = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                try
                {
                    ProcedimentoFacade.Instancia.Excluir(id);

                    this.carregarProcedimentos(txtLocalizar.Text);
                }
                catch
                {
                    Util.Geral.Alerta(null, this, "_err", "Não foi possível remover o procedimento. Talvez ele esteja em uso.");
                }

                //using (Contexto contexto = new Contexto())
                //{
                //    Procedimento proc = contexto.Procedimentos
                //        .Include("Tabela")
                //        .Where(p => p.ID == id)
                //        .Single();

                //    try
                //    {
                //        contexto.Procedimentos.Remove(proc);
                //        contexto.SaveChanges();

                //        this.carregarProcedimentos(txtLocalizar.Text);
                //    }
                //    catch
                //    {
                //        Util.Geral.Alerta(null, this, "_err", "Não foi possível remover o procedimento. Talvez ele esteja em uso.");
                //    }
                //}
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Util.Geral.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja excluir o procedimento?");
        }
    }
}