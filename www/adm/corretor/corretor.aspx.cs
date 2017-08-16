namespace MedProj.www.adm.corretor
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj.www.Util;
    using MedProj.Entidades;
    using LC.Web.PadraoSeguros.Facade;

    public partial class corretor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //this.carregarTabelasDeComissao();

                if (Geral.IdEnviado(this.Context, Keys.IdKey) != 0)
                {
                    this.carregarCorretor();
                }
            }
        }

        void carregarCorretor()
        {
            long id = Geral.IdEnviado(this.Context, Keys.IdKey);
            Corretor obj = CorretorFacade.Instancia.Carregar(id);

            txtNome.Text = obj.Nome;

            //if (obj.TabelaComissao != null && obj.TabelaComissao.TemId)
            //{
            //    cboTabela.SelectedValue = obj.TabelaComissao.ID.ToString();
            //}

            //chkStatus.Checked = obj.Ativo;
        }

        //void carregarTabelasDeComissao()
        //{
        //    cboTabela.Items.Clear();
        //    cboTabela.Items.Add(new ListItem("selecione", "-1"));

        //    var tabelas = RegraComissaoFacade.Instance.Carregar(String.Empty);

        //    if (tabelas != null)
        //    {
        //        tabelas.ForEach
        //        (
        //            t => cboTabela.Items.Add(new ListItem(t.Nome, t.ID.ToString()))
        //        );
        //    }
        //}

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("corretores.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (txtNome.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome do corretor.");
                txtNome.Focus();
                return;
            }

            Corretor obj = new Corretor();

            long id = Geral.IdEnviado(this.Context, Keys.IdKey);
            if (id > 0) obj.ID = id;

            obj.Nome = txtNome.Text;
            //obj.Ativo = chkStatus.Checked;
            //obj.Detalhamento = chkDetalhamento.Checked;

            //if (cboTabela.SelectedIndex > 0)
            //    obj.TabelaComissao = RegraComissaoFacade.Instance.Carregar(CTipos.CToLong(cboTabela.SelectedValue));

            CorretorFacade.Instancia.Salvar(obj);

            Response.Redirect("corretores.aspx");
        }
    }
}