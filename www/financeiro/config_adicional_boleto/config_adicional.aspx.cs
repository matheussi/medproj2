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

    //using Excel;
    using MedProj.www.Util;
    using MedProj.Entidades;
    using MedProj.Entidades.Enuns;
    using LC.Web.PadraoSeguros.Facade;
    using Entity = LC.Web.PadraoSeguros.Entity;
    using System.Configuration;

    public partial class config_adicional : System.Web.UI.Page
    {
        string idConfig
        {
            get
            {
                return Request[Util.Keys.IdKey];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtValor.Attributes.Add("onKeyUp", "mascara('" + txtValor.ClientID + "')");

            if (!IsPostBack)
            {
                this.carregaAssociadosPJ();
                this.carregaContratosADM();
                this.carregaContratos();

                if (!string.IsNullOrEmpty(this.idConfig)) { this.carregaConfig(); }
            }
        }

        void carregaConfig()
        {
            var config = ConfigAdicionalFacade.Instancia.Carregar(CTipos.CToLong(this.idConfig));

            txtDescricao.Text = config.Descricao;

            txtValor.Text = config.Valor.ToString("N2");
            txtTextoNoBoleto.Text = config.TextoNoBoleto;

            if (config.AssociadoPj != null)
            {
                cboEstipulante.SelectedValue = config.AssociadoPj.ID.ToString();
                this.carregaContratosADM();

                if (config.ContratoAdm != null)
                {
                    cboContratoADM.SelectedValue = config.ContratoAdm.ID.ToString();
                }
            }

            this.carregaContratos();

            if (config.Contratos != null && config.Contratos.Count > 0)
            {
                foreach (var c in config.Contratos)
                {
                    foreach (ListItem i in lstContratos.Items)
                    {
                        if (i.Value == c.ID.ToString()) i.Selected = true;
                    }
                }
            }

            chkAtivo.Checked = config.Ativo;
            chkContratosTodos.Checked = config.TodosContratos;
            if (config.TodosContratos) lstContratos.Visible = false;
        }

        void carregaAssociadosPJ()
        {
            List<AssociadoPJ> lista = AssociadoPJFacade.Instance.Carregar(string.Empty);
            cboEstipulante.Items.Clear();

            if (lista != null)
            {
                //lista.ForEach(p => cboEstipulante.Items.Add(new ListItem(p.Nome, p.ID.ToString())));

                lista.ForEach(delegate(AssociadoPJ p)
                {
                    if (p.Ativo) cboEstipulante.Items.Add(new ListItem(p.Nome, p.ID.ToString()));
                });
            }

            cboEstipulante.Items.Insert(0, new ListItem("Todos", "0"));

            if (cboEstipulante.Items.Count > 0) cboEstipulante.SelectedIndex = 0;
        }

        void carregaContratosADM()
        {
            cboContratoADM.Items.Clear();

            if (cboEstipulante.SelectedIndex > -1)
            {
                List<ContratoADM> lista = null;

                if (cboEstipulante.SelectedIndex > 0)
                    lista = ContratoAdmFacade.Instance.CarregarTodos(Convert.ToInt64(cboEstipulante.SelectedValue));
                else
                    lista = ContratoAdmFacade.Instance.Carregar(string.Empty);

                if (lista != null)
                {
                    //lista.ForEach(p => cboContratoADM.Items.Add(new ListItem(p.Descricao, p.ID.ToString())));

                    lista.ForEach(delegate(ContratoADM p)
                    {
                        if (p.Ativo) cboContratoADM.Items.Add(new ListItem(p.Descricao, p.ID.ToString()));
                    });
                }
            }

            cboContratoADM.Items.Insert(0, new ListItem("Todos", "0"));
            cboContratoADM.SelectedIndex = 0;
        }

        void carregaContratos()
        {
            DataTable dt = null;
            lstContratos.Items.Clear();

            if (cboContratoADM.SelectedIndex > 0)
            {
                dt = ContratoFacade.Instance.CarregaPorContratoAdmId(cboContratoADM.SelectedValue);
            }
            else if (cboEstipulante.SelectedIndex > 0)
            {
                dt = ContratoFacade.Instance.CarregaPorEstipulanteId(cboEstipulante.SelectedValue);
            }
            else
            {
                dt = ContratoFacade.Instance.CarregaSomentePjs();
            }

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (CTipos.CToInt(row["Inativo"]) == 1 || CTipos.CToInt(row["Cancelado"]) == 1) continue;

                    lstContratos.Items.Add(
                        new ListItem(
                            Util.CTipos.CToString(row["BeneficiarioNome"]),
                            Util.CTipos.CToString(row["ContratoID"])));
                }
            }
        }

        /******************************************************************************************/

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("config_adicional_lista.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            #region validacao

            if (txtDescricao.Text.Trim().Length <= 2)
            {
                Geral.Alerta(this, "Informe por favor uma descrição.");
                txtDescricao.Focus();
                return;
            }

            bool selecionouContrato = false;
            foreach (ListItem item in lstContratos.Items)
            {
                if (item.Selected)
                {
                    selecionouContrato = true;
                    break;
                }
            }

            if (!chkContratosTodos.Checked && !selecionouContrato)
            {
                Geral.Alerta(this, "Você deve selecionar um contrato ou marcar a opção (todos).");
                return;
            }

            #endregion

            ConfigAdicional config = null;

            if (string.IsNullOrEmpty(this.idConfig))
                config = new ConfigAdicional();
            else
                config = ConfigAdicionalFacade.Instancia.Carregar(CTipos.CToLong(this.idConfig));

            if (cboEstipulante.SelectedIndex > 0)
                config.AssociadoPj = AssociadoPJFacade.Instance.Carregar(Convert.ToInt64(cboEstipulante.SelectedValue));
            else
                config.AssociadoPj = null;

            if (cboContratoADM.SelectedIndex > 0)
                config.ContratoAdm = ContratoAdmFacade.Instance.Carregar(Convert.ToInt64(cboContratoADM.SelectedValue));
            else
                config.ContratoAdm = null;

            if (!chkContratosTodos.Checked)
            {
                #region gerencia contratos adicionados

                //adiciona novos contratos
                foreach (ListItem item in lstContratos.Items)
                {
                    if (item.Selected)
                    {
                        if (config.Contratos == null) config.Contratos = new List<Contrato>();

                        if (!config.Contratos.Contains(new Contrato { ID = Convert.ToInt64(item.Value) }))
                            config.Contratos.Add(new Contrato { ID = Convert.ToInt64(item.Value) });
                    }
                }

                List<string> aRemover = new List<string>();
                bool emUso = false;
                for (int i = 0; i < config.Contratos.Count; i++)
                {
                    emUso = false;
                    foreach (ListItem item in lstContratos.Items)
                    {
                        if (item.Value == config.Contratos[i].ID.ToString() && item.Selected)
                        {
                            emUso = true;
                            break;
                        }
                    }

                    if (!emUso)
                    {
                        aRemover.Add(config.Contratos[i].ID.ToString());
                    }
                }
                if (aRemover.Count > 0)
                {
                    foreach (string id in aRemover)
                    {
                        for (int i = 0; i < config.Contratos.Count; i++)
                        {
                            if (id == config.Contratos[i].ID.ToString())
                            {
                                config.Contratos.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
                #endregion
            }
            else if (config.Contratos != null)
            {
                config.Contratos.Clear();
            }

            config.TodosContratos = chkContratosTodos.Checked;
            config.Ativo = chkAtivo.Checked;
            config.Descricao = txtDescricao.Text;
            config.TextoNoBoleto = txtTextoNoBoleto.Text;
            config.Valor = CTipos.ToDecimal(txtValor.Text);

            ConfigAdicionalFacade.Instancia.Salvar(config);

            Response.Redirect("config_adicional_lista.aspx");
        }

        /******************************************************************************************/

        protected void cboEstipulante_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregaContratosADM();
            this.carregaContratos();
            this.selecionaContratos();
        }

        protected void cboContratoADM_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.carregaContratos();

            //seleciona contratos
            this.selecionaContratos();
        }

        void selecionaContratos()
        {
            if (!string.IsNullOrEmpty(this.idConfig) && lstContratos.Items.Count > 0)//if (cboContratoADM.SelectedIndex > 0)
            {
                var config = ConfigAdicionalFacade.Instancia.Carregar(CTipos.CToLong(this.idConfig));

                if (config.Contratos != null && config.Contratos.Count > 0 && cboContratoADM.SelectedValue == config.ContratoAdm.ID.ToString())
                {
                    foreach (var c in config.Contratos)
                    {
                        foreach (ListItem i in lstContratos.Items)
                        {
                            if (i.Value == c.ID.ToString()) i.Selected = true;
                        }
                    }
                }
            }
        }

        protected void chkContratosTodos_CheckedChanged(object sender, EventArgs e)
        {
            lstContratos.Visible = !chkContratosTodos.Checked;
        }
    }
}