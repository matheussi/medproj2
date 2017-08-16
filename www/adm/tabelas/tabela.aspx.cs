using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using MedProj.www.Util;
using MedProj.Entidades;
using LC.Web.PadraoSeguros.Facade;

namespace MedProj.www.adm.tabelas
{
    public partial class tabela : System.Web.UI.Page
    {
        long idVigencia
        {
            get { return CTipos.CToLong(ViewState[Keys.IDKey2]); }
            set { ViewState[Keys.IDKey2] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtCH.Attributes.Add("onkeyup", "mascara('" + txtCH.ClientID + "')");

            if (!IsPostBack)
            {
                if (Geral.IdEnviado(this.Context, Keys.IdKey) > 0) this.carregarTabela();

                if (!string.IsNullOrEmpty(Request["msg"])) Geral.Alerta(this, "Tabela de preço salva com sucesso.");
            }
        }

        void carregarTabela()
        {
            pnlVigencias.Visible = true;

            long id = Geral.IdEnviado(this.Context, Keys.IdKey);

            TabelaPreco tabela = TabelaPrecoFacade.Instancia.Carregar(id);

            txtNome.Text = tabela.Nome;

            this.carregarVigencias(tabela.Vigencias);

            //using (Contexto contexto = new Contexto())
            //{
            //    TabelaPreco tabela = contexto
            //        .TabelasPreco
            //            .Include("Vigencias")
            //            .Where(t => t.ID == id)
            //            .Single();

            //    txtNome.Text = tabela.Nome;

            //    this.carregarVigencias(contexto, id);
            //}
        }

        //void carregarVigencias(Contexto contexto, long tabelaId)
        void carregarVigencias(IList<TabelaPrecoVigencia> vigencias)
        {
            //List<TabelaPrecoVigencia> vigencias = null;

            //if (contexto != null)
            //{
            //    vigencias = contexto.TabelasVigencia
            //        .Where(v => v.Tabela.ID == tabelaId)
            //        .OrderByDescending(v => v.DataInicio)
            //        .ToList();
            //}
            //else
            //{
            //    using (Contexto _contexto = new Contexto())
            //    {
            //        vigencias = _contexto.TabelasVigencia
            //            .Where(v => v.Tabela.ID == tabelaId)
            //            .OrderByDescending(v => v.DataInicio)
            //            .ToList();
            //    }
            //}

            gridVigencias.DataSource = vigencias;
            gridVigencias.DataBind();
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("tabelas.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            #region validacoes

            if (txtNome.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome da tabela de preço.");
                txtNome.Focus();
                return;
            }

            //if (inicio == DateTime.MinValue)
            //{
            //    Geral.Alerta(this, "Informe a data de início da vigência.");
            //    txtDataDe.Focus();
            //    return;
            //}

            //if (ate == DateTime.MinValue)
            //{
            //    Geral.Alerta(this, "Informe a data final da vigência.");
            //    txtDataAte.Focus();
            //    return;
            //}
            //if (de > ate)
            //{
            //    Geral.Alerta(this, "A data inicial da vigência não pode ser maior que a data final.");
            //    txtDataDe.Focus();
            //    return;
            //}
            //if (fatorCh == decimal.Zero)
            //{
            //    Geral.Alerta(this, "Informe o fator de multiplicação de CH.");
            //    txtCH.Focus();
            //    return;
            //}

            #endregion

            long id = Geral.IdEnviado(this.Context, Keys.IdKey);

            TabelaPreco tabela = new TabelaPreco();

            if (id > 0)
            {
                tabela = TabelaPrecoFacade.Instancia.Carregar(id);
            }

            tabela.Nome = txtNome.Text;

            TabelaPrecoFacade.Instancia.Salvar(tabela);

            //TabelaPreco tabela = null;

            //using (Contexto contexto = new Contexto())
            //{
            //    if (id > 0)
            //    {
            //        tabela = contexto.TabelasPreco.Where(t => t.ID == id).Single();
            //    }
            //    else
            //    {
            //        tabela = new TabelaPreco();
            //    }

            //    tabela.Nome = txtNome.Text;

            //    if (id == 0) contexto.TabelasPreco.Add(tabela);

            //    contexto.SaveChanges();
            //}

            if (id == 0) Response.Redirect("tabela.aspx?msg=1&" + Keys.IdKey + "=" + tabela.ID.ToString());
            else Geral.Alerta(this, "Tabela de preço salva com sucesso.");
        }

        #region Vigencias 

        protected void gridVigencias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                long id = Geral.ObterDataKeyValDoGrid<long>(gridVigencias, e, 0);

                TabelaPrecoVigencia vigencia = TabelaPrecoVigenciaFacade.Instancia.Carregar(id);

                this.idVigencia = vigencia.ID;
                txtCH.Text      = vigencia.ValorReal.ToString("N2");
                txtDataDe.Text  = vigencia.DataInicio.ToString("dd/MM/yyyy");

                //using (Contexto contexto = new Contexto())
                //{
                //    TabelaPrecoVigencia vigencia = contexto.TabelasVigencia
                //        .Where(v => v.ID == id)
                //        .Single();

                //    this.idVigencia = vigencia.ID;
                //    txtCH.Text = vigencia.ValorReal.ToString("N2");
                //    txtDataDe.Text = vigencia.DataInicio.ToString("dd/MM/yyyy");
                //}
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long id = Geral.IdEnviado(this.Context, Keys.IdKey);
                long idVig = Geral.ObterDataKeyValDoGrid<long>(gridVigencias, e, 0);

                TabelaPrecoVigenciaFacade.Instancia.Excluir(idVig);

                TabelaPreco tabela = TabelaPrecoFacade.Instancia.Carregar(id);

                this.carregarVigencias(tabela.Vigencias);

                //using (Contexto contexto = new Contexto())
                //{
                //    TabelaPrecoVigencia vigencia = contexto.TabelasVigencia
                //        .Where(v => v.ID == idVig)
                //        .Single();

                //    vigencia.Tabela = contexto.TabelasPreco.Where(t => t.ID == id).Single();

                //    contexto.TabelasVigencia.Remove(vigencia);
                //    contexto.SaveChanges();

                //    this.carregarVigencias(contexto, Geral.IdEnviado(this.Context, Keys.IdKey));
                //}
            }
        }

        protected void gridVigencias_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Geral.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente excluir a vigência?");
        }

        void limpaCamposVigencia()
        {
            txtCH.Text = "0";
            txtDataDe.Text = "";
        }

        protected void lnkSalvarVigencia_Click(object sender, EventArgs e)
        {
            decimal valor   = CTipos.ToDecimal(txtCH.Text);
            DateTime inicio = CTipos.CStringToDateTime(txtDataDe.Text);

            #region validacoes 

            if (valor == 0)
            {
                Geral.Alerta(this, "Informe o valor.");
                txtCH.Focus();
                return;
            }

            if (inicio == DateTime.MinValue)
            {
                Geral.Alerta(this, "Informe a data de início da vigência.");
                txtDataDe.Focus();
                return;
            }

            #endregion

            TabelaPrecoVigencia vigencia = new TabelaPrecoVigencia();
            vigencia.Tabela = new TabelaPreco();

            long tabelaPrecoId = Geral.IdEnviado(this.Context, Keys.IdKey);

            if (this.idVigencia > 0) vigencia.ID = this.idVigencia;

            vigencia.Ativa = true;
            vigencia.DataInicio = inicio;
            vigencia.ValorReal = valor;

            vigencia.Tabela.ID = tabelaPrecoId;

            TabelaPrecoVigenciaFacade.Instancia.Salvar(vigencia);

            TabelaPreco tabela = TabelaPrecoFacade.Instancia.Carregar(tabelaPrecoId);

            this.carregarVigencias(tabela.Vigencias);
            this.idVigencia = 0;
            this.limpaCamposVigencia();


        //    TabelaPrecoVigencia vigencia = null;
        //    long id = Geral.IdEnviado(this.Context, Keys.IdKey);

        //    using (var contexto = new Contexto())
        //    {
        //        if (this.idVigencia == 0) vigencia = new TabelaPrecoVigencia();
        //        else
        //        {
        //            vigencia = contexto.TabelasVigencia
        //                .Where(v => v.ID == this.idVigencia)
        //                .Single();
        //        }

        //        vigencia.Ativa = true;
        //        vigencia.DataInicio = inicio;
        //        vigencia.ValorReal = valor;

        //        vigencia.Tabela = contexto.TabelasPreco.Where(t => t.ID == id).Single();

        //        if (this.idVigencia == 0) contexto.TabelasVigencia.Add(vigencia);

        //        contexto.SaveChanges();

        //        this.carregarVigencias(contexto, vigencia.Tabela.ID);
        //    }

        //    this.idVigencia = 0;
        //    this.limpaCamposVigencia();
        }

        #endregion
    }
}