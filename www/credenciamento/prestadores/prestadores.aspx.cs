using System;
using LinqKit;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using MedProj.www.Util;
using MedProj.Entidades;
using LC.Web.PadraoSeguros.Facade;

namespace MedProj.www.credenciamento.prestadores
{
    public partial class prestadores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregaRegioes();
                this.carregaSegmentos();
                this.carregaEspecialidades();
            }
        }

        void carregaSegmentos()
        {
            cboSegmento.Items.Clear();
            List<Segmento> lista = SegmentoFacade.Instancia.CarregarTodos(string.Empty);

            if (lista != null)
            {
                foreach (Segmento e in lista)
                {
                    ListItem item = new ListItem(e.Nome, e.ID.ToString());
                    cboSegmento.Items.Add(item);
                }
            }

            if (cboSegmento.Items.Count > 9) cboSegmento.SelectedIndex = 9;
        }

        void carregaEspecialidades()
        {
            //cboEspecialidade.Items.Clear();
            //cboEspecialidade.Items.Add(new ListItem("selecione", "-1"));
            //List<Especialidade> lista = EspecialidadeFacade.Instance.Carregar("");

            //if (lista != null)
            //{
            //    foreach (Especialidade e in lista)
            //    {
            //        ListItem item = new ListItem(e.Nome, e.ID.ToString());
            //        cboEspecialidade.Items.Add(item);
            //    }
            //}
        }

        void carregaRegioes()
        {
            //using (Contexto contexto = new Contexto())
            //{
            //    cboRegiao.Items.Clear();
            //    cboRegiao.Items.Add(new ListItem("selecione", "0"));

            //    List<Regiao> lista = contexto.Regioes
            //        .OrderBy(t => t.Nome)
            //        .ToList<Regiao>();

            //    if (lista != null)
            //    {
            //        foreach (Regiao r in lista)
            //        {
            //            ListItem item = new ListItem(r.Nome, r.ID.ToString());
            //            cboRegiao.Items.Add(item);
            //        }
            //    }
            //}
        }

        void carregarPrestadores()
        {
            long? especialidadeId = null;
            //if (cboEspecialidade.SelectedIndex > 0) especialidadeId = CTipos.CTipo<long>(cboEspecialidade.SelectedValue);

            if (especialidadeId.HasValue)
            {
                grid.Columns[1].Visible = true;
                grid.Columns[2].Visible = true;

                grid.DataSource = PrestadorFacade.Instancia.CarregarPorParametros(especialidadeId, txtNomeEmpresaCliente.Text, false);
            }
            else if(cboSegmento.Items.Count > 0)
            {
                grid.Columns[1].Visible = false;
                grid.Columns[2].Visible = false;

                //grid.DataSource = PrestadorFacade.Instancia.CarregarPorNome(
                //    txtNomeEmpresaCliente.Text, Util.CTipos.CTipo<long>(cboSegmento.SelectedValue),  false);

                grid.DataSource = PrestadorFacade.Instancia.CarregarPorNome(
                    txtNomeEmpresaCliente.Text, Util.CTipos.CTipo<long>(cboSegmento.SelectedValue));
            }
            grid.DataBind();

            if (grid.Rows.Count == 0)
                litMensagem.Text = "Nenhum prestador encontrato";
            else
                litMensagem.Text = "";

            #region 
            //using (var contexto = new Contexto())
            //{
            //    if (cboEspecialidade.SelectedIndex <= 0)
            //    {
            //        var lista =
            //        (
            //            from     p  in contexto.Prestadores
            //                join s  in contexto.Segmentos on p.Segmento.ID equals s.ID
            //                join pu in contexto.PrestadorUnidades on p.ID equals pu.Owner.ID
            //                join ue in contexto.UnidadeEspecialidades on pu.ID equals ue.Unidade.ID
            //                join e  in contexto.Especialidades on ue.Especialidade.ID equals e.ID
            //            where (p.Nome.Contains(txtNomeEmpresaCliente.Text) || pu.Nome.Contains(txtNomeEmpresaCliente.Text))
            //            orderby p.Nome,e.Nome

            //            select new
            //            {
            //                ID = p.ID,
            //                UID = pu.ID,
            //                Nome = p.Nome,
            //                EspecialidadeNome = e.Nome,
            //                EspecialidadeEnde = string.Concat(pu.Endereco, " ,", pu.Numero, " - ", pu.Bairro, " - ", pu.Cidade, " - ", pu.UF) 
            //            }
            //        )
            //        .ToList();

            //        grid.DataSource = lista;
            //        grid.DataBind();
            //    }
            //    else
            //    {
            //        long idEspecialidade = CTipos.CToLong(cboEspecialidade.SelectedValue);

            //        var lista =
            //        (
            //            from     p  in contexto.Prestadores
            //                join s  in contexto.Segmentos on p.Segmento.ID equals s.ID
            //                join pu in contexto.PrestadorUnidades on p.ID equals pu.Owner.ID
            //                join ue in contexto.UnidadeEspecialidades on pu.ID equals ue.Unidade.ID
            //                join e  in contexto.Especialidades on ue.Especialidade.ID equals e.ID
            //            where (p.Nome.Contains(txtNomeEmpresaCliente.Text) || pu.Nome.Contains(txtNomeEmpresaCliente.Text)) && ue.Especialidade.ID == idEspecialidade
            //            orderby p.Nome,e.Nome

            //            select new
            //            {
            //                ID                = p.ID,
            //                UID               = pu.ID,
            //                Nome              = p.Nome,
            //                EspecialidadeNome = e.Nome,
            //                EspecialidadeEnde = string.Concat(pu.Endereco, ", ", pu.Numero, " - ", pu.Bairro, " - ", pu.Cidade, " - ", pu.UF)
            //            }
            //        )
            //        .ToList();

            //        grid.DataSource = lista;
            //        grid.DataBind();
            //    }
            //}


            //var predicado = PredicateBuilder.True<Prestador>();

            //predicado = predicado.And(p => p.ID > 0);

            //if (txtNomeEmpresaCliente.Text.Trim() != "")
            //    predicado = predicado.And(p => p.Nome.Contains(txtNomeEmpresaCliente.Text));

            //using (Contexto contexto = new Contexto())
            //{
            //    grid.DataSource = contexto.Prestadores
            //        .Include("Segmento")
            //        .AsExpandable()
            //        .Where(predicado)
            //        .OrderBy(p => p.Nome)
            //        .ToList();
            //}

            //grid.DataBind();
            #endregion
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("prestador.aspx");
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.carregarPrestadores();
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Geral.grid_AdicionaToolTip<LinkButton>(e, 4, 0, "relatório");

                LinkButton lnk = (LinkButton)e.Row.Cells[4].Controls[0];
                lnk.OnClientClick = string.Concat("window.open('detalhe.aspx?id=", grid.DataKeys[e.Row.RowIndex].Value, "', 'MsgWindow', 'menubar=0,scrollbars=1,resizable=1,width=810, height=680');"); 
                //string.Concat("alert('ID: ", grid.DataKeys[e.Row.RowIndex].Value, "');return false;");

                Geral.grid_RowDataBound_Confirmacao(sender, e, 5, "Deseja realmente excluir o prestador?");
                Geral.grid_AdicionaToolTip<LinkButton>(e, 5, 0, "Excluir");
            }
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                string id = Geral.ObterDataKeyValDoGrid<string>(grid, e, 0);
                Response.Redirect("prestador.aspx?" + Keys.IdKey + "=" + id);
            }
            else if (e.CommandName.Equals("Excluir"))
            {
                long id = Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);

                Prestador p = PrestadorFacade.Instancia.CarregarPorId(id);
                p.Deletado = true;

                PrestadorFacade.Instancia.Salvar(p);

                this.carregarPrestadores();
            }
        }
    }
}