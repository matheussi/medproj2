namespace MedProj.www.financeiro.config_adicional_boleto
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj.www.Util;
    using MedProj.Entidades;
    using MedProj.Entidades.Enuns;
    using LC.Web.PadraoSeguros.Facade;
    using Entity = LC.Web.PadraoSeguros.Entity;
    using System.Configuration;

    public partial class config_adicional_lista : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregaAssociadosPJ();
                this.carregaContratoADM();
                //this.carregarConfiguracoes();
            }
        }

        void carregaAssociadosPJ()
        {
            cboAssociadoPJ.Items.Clear();
            var lista = AssociadoPJFacade.Instance.Carregar(string.Empty);
            cboAssociadoPJ.Items.Add(new ListItem("todos", "0"));

            foreach (var a in lista)
            {
                if (!a.Ativo) continue;
                cboAssociadoPJ.Items.Add(new ListItem(a.Nome, a.ID.ToString()));
            }
        }

        void carregaContratoADM()
        {
            cboContratoADM.Items.Clear();

            List<ContratoADM> lista = null;

            if (cboAssociadoPJ.SelectedIndex <= 0)
                lista = ContratoAdmFacade.Instance.Carregar(string.Empty);
            else
                lista = ContratoAdmFacade.Instance.CarregarTodos(CTipos.CToLong(cboAssociadoPJ.SelectedValue));

            cboContratoADM.Items.Add(new ListItem("todos", "0"));

            foreach (var a in lista)
            {
                if (!a.Ativo) continue;
                cboContratoADM.Items.Add(new ListItem(a.Descricao, a.ID.ToString()));
            }
        }

        void carregaContratos()
        {
            DataTable dt = null;
            cboContrato.Items.Clear();

            cboContrato.Items.Add(new ListItem("todos", "0"));

            if (cboContratoADM.SelectedIndex > 0)
            {
                dt = ContratoFacade.Instance.CarregaPorContratoAdmId(cboContratoADM.SelectedValue);
            }
            else if (cboAssociadoPJ.SelectedIndex > 0)
            {
                dt = ContratoFacade.Instance.CarregaPorEstipulanteId(cboAssociadoPJ.SelectedValue);
            }

            if (dt != null && dt.Rows != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (CTipos.CToInt(row["Inativo"]) == 1 || CTipos.CToInt(row["Cancelado"]) == 1) continue;

                    cboContrato.Items.Add(
                        new ListItem(
                            Util.CTipos.CToString(row["BeneficiarioNome"]),
                            Util.CTipos.CToString(row["ContratoID"])));
                }
            }
        }

        void carregarConfiguracoes()
        {
            List<ConfigAdicional> lista = null;

            if (cboAssociadoPJ.SelectedIndex <= 0 && cboContratoADM.SelectedIndex <= 0) //if (cboAssociadoPJ.SelectedIndex < 0 && cboContratoADM.SelectedIndex <= 0)
                lista = ConfigAdicionalFacade.Instancia.Carregar();
            else if (cboAssociadoPJ.SelectedIndex > 0 && cboContratoADM.SelectedIndex <= 0)//else if (cboAssociadoPJ.SelectedIndex >= 0 && cboContratoADM.SelectedIndex <= 0)
                lista = ConfigAdicionalFacade.Instancia.CarregarPorAssoc(CTipos.CToLong(cboAssociadoPJ.SelectedValue));
            else if (cboAssociadoPJ.SelectedIndex > 0 && cboContratoADM.SelectedIndex > 0 && cboContrato.SelectedIndex <= 0)//else if (cboAssociadoPJ.SelectedIndex >= 0 && cboContratoADM.SelectedIndex > 0 && cboContrato.SelectedIndex <= 0)
                lista = ConfigAdicionalFacade.Instancia.CarregarPorAssoc(CTipos.CToLong(cboAssociadoPJ.SelectedValue), CTipos.CToLong(cboContratoADM.SelectedValue));
            else if (cboContratoADM.SelectedIndex <= 0 && cboContrato.SelectedIndex == 0) //selecionou um contrato adm, mas todos os contratos
                lista = ConfigAdicionalFacade.Instancia.CarregarPor(CTipos.CToLong(cboContratoADM.SelectedValue));
            else if (cboContratoADM.SelectedIndex > 0 && cboContrato.SelectedIndex > 0)
                lista = ConfigAdicionalFacade.Instancia.CarregarPor(CTipos.CToLong(cboContratoADM.SelectedValue), CTipos.CToLong(cboContrato.SelectedValue));

            grid.DataSource = lista;
            grid.DataBind();

            if (lista == null || lista.Count == 0)
                litAviso.Text = "Nenhum registro encontrado";
            else
                litAviso.Text = "";
        }

        protected void lnkNovo_Click(object sender, EventArgs e)
        {
            Response.Redirect("config_adicional.aspx");
        }

        /*******************************************************************************/

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Util.Geral.grid_AdicionaToolTip<LinkButton>(e, 2, 0, "Editar");
                //Util.Geral.grid_AdicionaToolTip<LinkButton>(e, 3, 0, "Arquivo de log");

                ConfigAdicional obj = e.Row.DataItem as ConfigAdicional;
                if (obj != null && !obj.Ativo) e.Row.ForeColor = System.Drawing.Color.Red;

                Geral.grid_RowDataBound_Confirmacao(sender, e, 2, "Deseja realmente excluir?");
            }
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Editar"))
            {
                string id = Util.Geral.ObterDataKeyValDoGrid<string>(grid, e, 0);

                Response.Redirect(string.Format("config_adicional.aspx?{0}={1}", Util.Keys.IdKey, id));
            }
            else if (e.CommandName.Equals("Log"))
            {
            }
        }

        protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

        }

        /*******************************************************************************/

        protected void cboAssociadoPJ_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregaContratoADM();
            this.carregaContratos();
        }

        protected void cboContratoADM_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregaContratos();
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.carregarConfiguracoes();
        }
    }
}