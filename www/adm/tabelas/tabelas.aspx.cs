using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using LinqKit;
using MedProj.Entidades;
using MedProj.www.Util;
using LC.Web.PadraoSeguros.Facade;

namespace MedProj.www.adm.tabelas
{
    public partial class tabelas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregarTabelas();
            }
        }

        void carregarTabelas()
        {
            //DateTime de  = CTipos.CStringToDateTime(txtDataDe.Text);
            //DateTime ate = CTipos.CStringToDateTime(txtDataAte.Text);

            //var predicado = PredicateBuilder.True<TabelaPrecoVigencia>();
            //long tabelaId = Util.CTipos.CTipo<long>(cboTabela.SelectedValue);

            //predicado = predicado.And(t => t.Tabela.ID == tabelaId);

            //if (de != DateTime.MinValue) predicado = predicado.And(t => t.DataInicio.Day == de.Day && t.DataInicio.Month == de.Month && t.DataInicio.Year == de.Year);
            //if (ate != DateTime.MinValue) predicado = predicado.And(t => t.DataFim.Value.Day == ate.Day && t.DataFim.Value.Month == ate.Month && t.DataFim.Value.Year == ate.Year);

            grid.DataSource = TabelaPrecoFacade.Instancia.Carregar(txtTabela.Text);
            grid.DataBind();

            //using (Contexto contexto = new Contexto())
            //{
            //    grid.DataSource = contexto.TabelasPreco
            //        .Where(t => t.Nome.Contains(txtTabela.Text))
            //        .OrderBy(t => t.Nome)
            //        .ToList();

            //    grid.DataBind();
            //}

            #region comentado
            //List<TabelaPrecoVigencia> lista = new List<TabelaPrecoVigencia>();

            //if (cboTabela.SelectedIndex == 0)
            //{
            //    lista.Add(new TabelaPrecoVigencia { DataFim = DateTime.Now.AddMonths(-3), DataInicio = DateTime.Now.AddMonths(-4), FatorCH = 1.2M });
            //    lista.Add(new TabelaPrecoVigencia { DataFim = DateTime.Now.AddMonths(-1), DataInicio = DateTime.Now.AddMonths(-2), FatorCH = 1.5M });
            //    lista.Add(new TabelaPrecoVigencia { DataFim = DateTime.Now.AddMonths(1), DataInicio = DateTime.Now, FatorCH = 1.5M });

            //    lista.ForEach(t => t.Tabela.Nome = "LOW");
            //}
            //else if (cboTabela.SelectedIndex == 1)
            //{
            //    lista.Add(new TabelaPrecoVigencia { DataFim = DateTime.Now.AddMonths(-3), DataInicio = DateTime.Now.AddMonths(-4), FatorCH = 1.9M });
            //    lista.Add(new TabelaPrecoVigencia { DataFim = DateTime.Now.AddMonths(-1), DataInicio = DateTime.Now.AddMonths(-2), FatorCH = 2M });
            //    lista.Add(new TabelaPrecoVigencia { DataFim = DateTime.Now.AddMonths(1), DataInicio = DateTime.Now, FatorCH = 2.2M });

            //    lista.ForEach(t => t.Tabela.Nome = "MEDIUM");
            //}
            //else if (cboTabela.SelectedIndex == 2)
            //{
            //    lista.Add(new TabelaPrecoVigencia { DataFim = DateTime.Now.AddMonths(-3), DataInicio = DateTime.Now.AddMonths(-4), FatorCH = 2.5M });
            //    lista.Add(new TabelaPrecoVigencia { DataFim = DateTime.Now.AddMonths(-1), DataInicio = DateTime.Now.AddMonths(-2), FatorCH = 2.8M });
            //    lista.Add(new TabelaPrecoVigencia { DataFim = DateTime.Now.AddMonths(1), DataInicio = DateTime.Now, FatorCH = 3.2M });

            //    lista.ForEach(t => t.Tabela.Nome = "TOP");
            //}

            //grid.DataSource = lista;
            //grid.DataBind();
            #endregion
        }

        void carregarTabelasDeProcedimentos()
        {
            //cboTabela.Items.Clear();

            //cboTabela.DataValueField = "ID";
            //cboTabela.DataTextField = "Nome";

            //using (Contexto contexto = new Contexto())
            //{
            //    cboTabela.DataSource = contexto.TabelasProcedimento
            //        .Where(t => t.Ativa == true)
            //        .OrderBy(t => t.Nome)
            //        .ToList();

            //    cboTabela.DataBind();
            //}
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.carregarTabelas();
        }

        protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grid.PageIndex = e.NewPageIndex;
            this.carregarTabelas();
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("tabela.aspx");
        }

        protected void cboTabela_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregarTabelas();
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                long id = Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);
                Response.Redirect("tabela.aspx?" + Keys.IdKey + "=" + id);
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long id = Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                try
                {
                    TabelaPrecoFacade.Instancia.Excluir(id);
                    this.carregarTabelas();
                }
                catch
                {
                    Util.Geral.Alerta(null, this, "_err", "Não foi possível remover a tabela.\\nCaso ela possua vigências, estas devem ser removidas.");
                }

                //using (Contexto contexto = new Contexto())
                //{
                //    TabelaPreco tab = contexto.TabelasPreco
                //        .Where(p => p.ID == id)
                //        .Single();

                //    try
                //    {
                //        contexto.TabelasPreco.Remove(tab);
                //        contexto.SaveChanges();

                //        this.carregarTabelas();
                //    }
                //    catch
                //    {
                //        Util.Geral.Alerta(null, this, "_err", "Não foi possível remover a tabela.\\nCaso ela possua vigências, estas devem ser removidas.");
                //    }
                //}
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            Geral.grid_RowDataBound_Confirmacao(sender, e, 1, "Deseja realmente excluir a tabela?");
        }
    }
}