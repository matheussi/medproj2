namespace MedProj.www.financeiro.config_emails
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

    public partial class config_emails : System.Web.UI.Page
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
            if (!IsPostBack)
            {
                this.configTipos();
                this.carregaTextos();
                this.carregaAssociadosPJ();
                this.carregaContratosADM();
                this.carregaContratos();
                this.setupTipo();

                if (!string.IsNullOrEmpty(this.idConfig)) { this.carregaConfig(); }
            }
        }

        void carregaConfig()
        {
            var config = ConfigEmailFacade.Instancia.Carregar(CTipos.CToLong(this.idConfig));

            txtDescricao.Text = config.Descricao;
            cboTipo.SelectedValue = Convert.ToInt32(config.Tipo).ToString();

            this.setupTipo();

            txtDiasAntecedencia.Text = config.DiasAntesVencimento.ToString();
            //txtEmail.Text = config.Email;
            txtFrequencia.Text = config.Frequencia.ToString();

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

            if (config.Texto != null)
            {
                cboTexto.SelectedValue = config.Texto.ID.ToString();
            }

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

        void carregaTextos()
        {
            List<ConfigEmailTexto> lista = ConfigEmailFacade.Instancia.CarregarTextos();
            cboTexto.Items.Clear();

            if (lista != null)
            {
                //lista.ForEach(p => cboEstipulante.Items.Add(new ListItem(p.Nome, p.ID.ToString())));
                lista.ForEach(delegate(ConfigEmailTexto t)
                {
                    cboTexto.Items.Add(new ListItem(t.Descricao, t.ID.ToString()));
                });
            }

            if (cboTexto.Items.Count > 0) cboTexto.SelectedIndex = 0;
        }

        void configTipos()
        {
            cboTipo.Items.Clear();
            cboTipo.Items.Add(new ListItem("Aviso De Pagamento", "0"));
            cboTipo.Items.Add(new ListItem("Aviso De Vencimento Próximo", "1"));
            cboTipo.Items.Add(new ListItem("Aviso De Vencimento Passado", "2"));
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

            if(cboEstipulante.Items.Count > 0) cboEstipulante.SelectedIndex = 0;
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

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("config_emails_lista.aspx");
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

            if (cboTipo.SelectedValue == "1" && CTipos.CToInt(txtDiasAntecedencia.Text) == 0)
            {
                Geral.Alerta(this, "Informe por favor os dias de antecedência.");
                txtDiasAntecedencia.Focus();
                return;
            }

            if (cboTipo.SelectedValue == "1" && CTipos.CToInt(txtFrequencia.Text) == 1)
            {
                Geral.Alerta(this, "Informe por favor os dias decorridos do vencimento.");
                txtFrequencia.Focus();
                return;
            }

            //if (txtEmail.Text.Trim().Length <= 10)
            //{
            //    Geral.Alerta(this, "Informe por favor o texto do e-mail.");
            //    txtEmail.Focus();
            //    return;
            //}

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

            ConfigEmailAviso config = null;

            if (string.IsNullOrEmpty(this.idConfig))
            {
                config = new ConfigEmailAviso();
            }
            else
                config = ConfigEmailFacade.Instancia.Carregar(CTipos.CToLong(this.idConfig));

            config.Texto = ConfigEmailFacade.Instancia.CarregarTexto(CTipos.CTipo<long>(cboTexto.SelectedValue));

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

                //remove contratos excluídos
                //foreach (ListItem item in lstContratos.Items)
                //{
                //    if (item.Selected) continue; //só verifica os que NÃO estão selecionados no listbox

                //    for (int i = 0; i < config.Contratos.Count; i++)
                //    {
                //        if (config.Contratos[i].ID.ToString() == item.Value)
                //        {
                //            config.Contratos.RemoveAt(i);
                //            break;
                //        }
                //    }
                //}

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
            else if(config.Contratos != null)
            {
                config.Contratos.Clear();
            }

            config.TodosContratos = chkContratosTodos.Checked;
            config.Ativo = chkAtivo.Checked;
            config.Descricao = txtDescricao.Text;
            config.DiasAntesVencimento = CTipos.CToInt(txtDiasAntecedencia.Text);
            //config.Email = txtEmail.Text;
            config.Frequencia = CTipos.CToInt(txtFrequencia.Text);
            config.Tipo = (TipoConfig)Enum.Parse(typeof(TipoConfig), cboTipo.SelectedValue);

            ConfigEmailFacade.Instancia.Salvar(config);

            Response.Redirect("config_emails_lista.aspx");
        }

        protected void cboTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.setupTipo();
        }

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
                var config = ConfigEmailFacade.Instancia.Carregar(CTipos.CToLong(this.idConfig));

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

        void setupTipo()
        {
            pnlFrequencia.Visible = cboTipo.SelectedValue == "2";
            pnlDiasAntecedencia.Visible = cboTipo.SelectedValue == "1";

            string texto = null;

            if (cboTipo.SelectedIndex == 0) //aviso de pagamento
            {
                texto = "<table style='font-family:calibri'><tbody><tr><td align='center' style='background-color:#E1E1E1'><img alt='Clube Azul' src='http://sispag.clubeazul.org.br/Images/clubeazul.png' title='Clube Azul' /></td></tr><tr><td><p>&nbsp;</p><p>Ol&aacute;, [#NOME].<br /><br />Informamos que recebemos o pagamento referente o boleto com vencimento em [#VNCT], &nbsp;referente a compet&ecirc;ncia&nbsp;[#CMPT]&nbsp;no valor de [#VLOR] e com n&uacute;mero de vidas de&nbsp;[#QTDV]&nbsp;.</p><p><br />Atenciosamente,<br />Clube Azul</p><p>&nbsp;</p><p>&nbsp;</p></td></tr><tr><td><font size='1'>Este &eacute; um e-mail autom&aacute;tico, por favor n&atilde;o o responda</font></td></tr></tbody></table><p>&nbsp;</p>";
            }
            else if (cboTipo.SelectedIndex == 1) //aviso de vencimento proximo
            {
                texto = "<table style='font-family:calibri'><tbody><tr><td align='center' style='background-color:#E1E1E1'><img alt='Clube Azul' src='http://sispag.clubeazul.org.br/Images/clubeazul.png' title='Clube Azul' /></td></tr><tr><td><p>Ol&aacute;, [#NOME].</p><p><br />Informamos que se aproxima a data de vencimento de seu boleto, que &eacute; em [#VNCT], e o valor &eacute; de [#VLOR].<br /><br />Para sua comodidade, caso queira visualizar e/ou atualizar seu boleto segue o [#LINK]clique aqui[#LINK/]<span style='font-size: 9pt; font-family: Calibri, sans-serif;'>, ou, se estiver tendo problemas para abrir o link, copie e cole o link a seguir em seu navegador de internet:<br /><br />[#ELINK]</span></p><p>Caso tenha ocorrido altera&ccedil;&atilde;o no quadro de funcion&aacute;rios com base em sua GEFIP atual, lembre-se de enviar essa admiss&atilde;o ou demiss&atilde;o ao e-mail <a href='mailto:operacional@clubeazul.org.br'>operacional@clubeazul.org.br</a> anexando a planilha de movimenta&ccedil;&atilde;o cadastral.</p><p>IMPORTANTE: CASO N&Atilde;O SEJA ENVIADO ESSA ATUALIZA&Ccedil;AO O SEGURO CONTRATADO N&Atilde;O TER&Aacute; COBERTURA.</p><p>Em caso de duvida a disposi&ccedil;&atilde;o no e-mail <a href='mailto:operacional@clubeazul.org.br'>operacional@clubeazul.org.br</a></p><p>Caso j&aacute; tenha efetuado o pagamento, por favor ignore esta mensagem.<br /><br />Atenciosamente,<br />Clube Azul</p><p>&nbsp;</p></td></tr><tr><td><font size='1'>Este &eacute; um e-mail autom&aacute;tico, por favor n&atilde;o o responda</font></td></tr></tbody></table><p>&nbsp;</p>";
            }
            else if (cboTipo.SelectedIndex == 2) //aviso de vencimento passado
            {
                texto = "<table style='font-family:calibri'><tbody><tr><td align='center' style='background-color:#E1E1E1'><img alt='Clube Azul' src='http://sispag.clubeazul.org.br/Images/clubeazul.png' title='Clube Azul' /></td></tr><tr><td><p style='background-image: initial; background-attachment: initial; background-size: initial; background-origin: initial; background-clip: initial; background-position: initial; background-repeat: initial;'><span style='font-size: 9pt; font-family: Calibri, sans-serif;'>Ol&aacute;, [#NOME].<br /><br />Informamos que a cobran&ccedil;a com data de vencimento em [#VNCT]&nbsp;</span>referente a compet&ecirc;ncia&nbsp;[#CMPT]<span style='font-family: Calibri, sans-serif; font-size: 9pt;'>, e cujo valor &eacute; de [#VLOR], est&aacute; vencida.</span></p><p style='background-image: initial; background-attachment: initial; background-size: initial; background-origin: initial; background-clip: initial; background-position: initial; background-repeat: initial;'><strong><u><span style='font-size: 9pt; font-family: Calibri, sans-serif;'>LEMBRAMOS QUE AS COBERTURAS DO SEGURO E DEMAIS BENEFICIOS EST&Atilde;O CONDICIONADAS AO PAGAMENTO DESTE DOCUMENTO.</span></u></strong><br /><br /><span style='font-size: 9pt; font-family: Calibri, sans-serif;'>Para visualizar o boleto, queira por favor [#LINK]clicar aqui[#LINK/], ou, se estiver tendo problemas para abrir o link, copie e cole o link a seguir em seu navegador de internet:<br /><br />[#ELINK]<br /><br /></span></p><p style='background-image: initial; background-attachment: initial; background-size: initial; background-origin: initial; background-clip: initial; background-position: initial; background-repeat: initial;'><span style='font-size: 9pt; font-family: Calibri, sans-serif;'>Caso j&aacute; tenha efetuado o pagamento, por favor ignore esta mensagem.<br /><br />Atenciosamente,<br />Clube Azul</span></p><p>&nbsp;</p></td></tr><tr><td><font size='1'>Este &eacute; um e-mail autom&aacute;tico, por favor n&atilde;o o responda</font></td></tr></tbody></table><p>&nbsp;</p>";
            }

            if (!string.IsNullOrEmpty(texto)) txtEmail.Text = texto;
        }

        protected void chkContratosTodos_CheckedChanged(object sender, EventArgs e)
        {
            lstContratos.Visible = !chkContratosTodos.Checked;
        }
    }
}