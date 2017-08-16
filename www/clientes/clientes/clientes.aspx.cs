using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LC.Web.PadraoSeguros.Entity;

namespace MedProj.www.clientes.clientes
{
    public partial class clientes : Page
    {
        protected const String IDKey = "_idkey";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Util.Geral.ExibirEstipulantes(cboAssociadoPJ, true, true);
            }
        }

        //protected void ExibirEstipulantes(DropDownList combo, Boolean itemSELECIONE, Boolean apenasAtivos)
        //{
        //    combo.Items.Clear();
        //    combo.DataValueField = "ID";
        //    combo.DataTextField = "Descricao";
        //    combo.DataSource = Estipulante.Carregar(apenasAtivos);
        //    combo.DataBind();

        //    if (itemSELECIONE)
        //    {
        //        combo.Items.Insert(0, new ListItem("Selecione", "-1"));
        //    }
        //}

        void CarregaContratos()
        {
            DataTable lista = null;

            long assocId = 0;
            if (cboAssociadoPJ.SelectedIndex > 0) assocId = Convert.ToInt64(cboAssociadoPJ.SelectedValue);

            //if(txtCartao.Text != "" || txtNome.Text != "")
                lista = Contrato.DTCarregarPorParametros(txtCartao.Text, txtNome.Text, "", assocId);

            gridContratos.DataSource = lista;
            gridContratos.DataBind();
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("cliente.aspx");
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.CarregaContratos();
            if (gridContratos.Rows.Count == 0) { litAviso.Text = "Nenhum registro localizado"; }
        }

        protected void gridContratos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
                Session[IDKey] = gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)][0];
                String uri = "cliente.aspx?" + IDKey + "=" + Session[IDKey];
                //if (txtProtocolo.Text.Trim() != "")
                //    uri += "&prot=" + txtProtocolo.Text;

                Response.Redirect(uri);
            }
        }

        protected void gridContratos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Boolean rascunho = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][1]);
                ((Image)e.Row.Cells[4].Controls[1]).Visible = rascunho;

                Boolean cancelado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][2]);
                Boolean inativado = Convert.ToBoolean(gridContratos.DataKeys[e.Row.RowIndex][3]);

                //UIHelper.AuthWebCtrl((LinkButton)e.Row.Cells[5].Controls[0], new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.PropostaBeneficiarioIDKey });
                //UIHelper.AuthCtrl((LinkButton)e.Row.Cells[7].Controls[0], new String[] { Perfil.AdministradorIDKey });
                //UIHelper.AuthWebCtrl((LinkButton)e.Row.Cells[7].Controls[0], new String[] { Perfil.AdministradorIDKey });
                grid_RowDataBound_Confirmacao(sender, e, 7, "Deseja realmente prosseguir com a exclusão?\\nEssa operação não poderá ser desfeita.");

                if (Usuario.Autenticado.PerfilID != Perfil.AdministradorIDKey) { gridContratos.Columns[7].Visible = false; }

                if (cancelado || inativado)
                {
                    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='../../images/unactive.png' title='inativo' alt='inativo' border='0'>";
                    //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente ativar o contrato?");
                }
                else
                {
                    //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente cancelar o contrato?");
                    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='../../images/active.png' title='ativo' alt='ativo' border='0'>";
                }
            }
        }

        protected void gridContratos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridContratos.PageIndex = e.NewPageIndex;
            this.CarregaContratos();
        }

        protected void grid_RowDataBound_Confirmacao(Object sender, GridViewRowEventArgs e, int indiceControle, String Msg)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[indiceControle].Attributes.Add("onClick", "return confirm('" + Msg + "');");
            }
        }
    }
}