namespace MedProj.www.proxy
{
    using System;
    using System.Web;
    using System.Text;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using Ent = MedProj.Entidades;
    using LC.Web.PadraoSeguros.Facade;
    using LC.Web.PadraoSeguros.Entity;
    using Enty = LC.Web.PadraoSeguros.Entity;

    public partial class proxyCarregaProcedimentosCredenciado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.processaRequisicao(Request["name_startsWith"]);
            }
        }

        void processaRequisicao(string param)
        {
            if (Util.UsuarioLogado.IDUnidade == 0) return;

            List<Ent.UnidadeProcedimento> procedimentos = 
                ProcedimentoFacade.Instancia.CarregarProcedimentosDaUnidade(Util.UsuarioLogado.IDUnidade, param);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.Append("{ \"Procs\" : [");

            if (procedimentos != null)
            {
                int i = 0;
                decimal valor = 0;
                foreach (Ent.UnidadeProcedimento proc in procedimentos)
                {
                    valor = proc.ValorCalculado;

                    //solicitacao
                    //if (valor == decimal.Zero) continue;

                    if (i > 0) sb.Append(" , ");

                    sb.Append(" { \"ID\" : \""); sb.Append(proc.Procedimento.ID); sb.Append("\", ");
                    sb.Append("\"Valor\" : \""); sb.Append(valor.ToString("N2")); sb.Append("\", ");
                    sb.Append("\"Espec\" : \""); sb.Append(EntityBase.RetiraAcentos(proc.Procedimento.Especialidade)); sb.Append("\", ");
                    sb.Append("\"Nome\" : \""); sb.Append(proc.Procedimento.Codigo); sb.Append("-"); sb.Append(EntityBase.RetiraAcentos(proc.Procedimento.Nome)); sb.Append("\" } ");

                    i++;
                }
            }

            sb.Append(" ] } ");

            Response.Clear();
            Response.ContentEncoding = System.Text.Encoding.UTF8; //.GetEncoding("ISO-8859-1");
            Response.ContentType = "application/json";
            Response.Write(sb.ToString());
            Response.End();
        }
    }
}