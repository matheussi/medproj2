using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using MedProj.www.Util;
using MedProj.Entidades;
using LC.Web.PadraoSeguros.Facade;

namespace MedProj.www.adm.segmento
{
    public partial class segmento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Geral.IdEnviado(this.Context, Keys.IdKey) != 0)
                {
                    this.carregarSegmento();
                }
            }
        }

        void carregarSegmento()
        {
            long id = Geral.IdEnviado(this.Context, Keys.IdKey);
            Segmento obj = SegmentoFacade.Instancia.Carregar(id);

            txtSegmento.Text = obj.Nome;
            chkStatus.Checked = obj.Ativo;
            chkDetalhamento.Checked = obj.Detalhamento;
        }

        protected void cmdVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("segmentos.aspx");
        }

        protected void cmdSalvar_Click(object sender, EventArgs e)
        {
            if (txtSegmento.Text.Trim() == "")
            {
                Geral.Alerta(this, "Informe o nome do segmento.");
                txtSegmento.Focus();
                return;
            }

            Segmento obj = new Segmento();

            long id = Geral.IdEnviado(this.Context, Keys.IdKey);
            if (id > 0) obj.ID = id;

            obj.Nome            = txtSegmento.Text;
            obj.Ativo           = chkStatus.Checked;
            obj.Detalhamento    = chkDetalhamento.Checked;

            SegmentoFacade.Instancia.Salvar(obj);

            Response.Redirect("segmentos.aspx");
        }
    }
}