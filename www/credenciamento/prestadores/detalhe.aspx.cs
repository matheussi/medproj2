namespace MedProj.www.credenciamento.prestadores
{
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
    using System.Text;

    public partial class detalhe : System.Web.UI.Page
    {
        long? prestadorId
        {
            get
            {
                if (string.IsNullOrEmpty(Request["id"]))
                    return null;
                else
                    return Util.CTipos.CToLongNullable(Request["id"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (this.prestadorId != null) { this.escreveRelatorio(); }
            }
        }

        void escreveRelatorio()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table width='98%' cellpadding='1' align='center' cellspacing='0' border='0'>");
            sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>");

            sb.Append("<tr><td>");

            Prestador prestador = PrestadorFacade.Instancia.CarregarPorId(this.prestadorId.Value);

            sb.Append("<h2><b>"); sb.Append(prestador.Nome); sb.Append("</b></h2>");


            sb.Append("</td><td><img src='../../Images/CLUBEAZUL.png'></td>");

            sb.Append("<tr><td colspan='2'>&nbsp;</td></tr>");

            List<UnidadeProcedimento> procedimentos = null;
            List<PrestadorUnidade> unidades = PrestadorUnidadeFacade.Instancia.CarregaPorPrestadorId(prestador.ID);

            if (unidades != null && unidades.Count > 0)
            {
                sb.Append("<tr><td colspan='2'>");

                foreach (PrestadorUnidade unidade in unidades)
                {
                    sb.Append("<table width='100%' cellpadding='3' cellspacing='0' border='1'>");

                    sb.Append("<tr><td>");
                    sb.Append("<h2>"); sb.Append(unidade.Nome); sb.Append("</h2>");
                    sb.Append("</td></tr>");

                    sb.Append("<tr><td>");
                    sb.Append("<h3>Endereço</h3>");
                    sb.Append("</td></tr>");

                    sb.Append("<tr><td>");

                    sb.Append("<table width='100%' cellpadding='0' cellspacing='0' border='0'>");
                    sb.Append("<tr><td>");
                    sb.Append(unidade.Endereco);

                    if (!string.IsNullOrEmpty(unidade.Numero))
                    {
                        sb.Append(", "); sb.Append(unidade.Numero);
                    }

                    if (!string.IsNullOrEmpty(unidade.Complemento))
                    {
                        sb.Append(" "); sb.Append(unidade.Complemento);
                    }

                    if (!string.IsNullOrEmpty(unidade.Bairro))
                    {
                        sb.Append(" - "); sb.Append(unidade.Bairro);
                    }

                    if (!string.IsNullOrEmpty(unidade.Cidade))
                    {
                        sb.Append(" - "); sb.Append(unidade.Cidade);
                    }

                    if (!string.IsNullOrEmpty(unidade.UF))
                    {
                        sb.Append(" - "); sb.Append(unidade.UF);
                    }

                    if (!string.IsNullOrEmpty(unidade.CEP))
                    {
                        sb.Append(" - CEP "); sb.Append(unidade.CEP);
                    }

                    sb.Append("<br/>");

                    if (!string.IsNullOrEmpty(unidade.Telefone))
                    {
                        sb.Append("Telefone: "); sb.Append(unidade.Telefone);
                    }
                    if (!string.IsNullOrEmpty(unidade.Celular))
                    {
                        sb.Append("&nbsp;Celular: "); sb.Append(unidade.Celular);
                    }
                    if (!string.IsNullOrEmpty(unidade.Email))
                    {
                        sb.Append("&nbsp;E-mail: "); sb.Append(unidade.Email);
                    }

                    sb.Append("</td></tr>");
                    sb.Append("</table>");

                    procedimentos = ProcedimentoFacade.Instancia.CarregarProcedimentosDaUnidade(unidade.ID);

                    if (procedimentos != null && procedimentos.Count > 0)
                    {
                        sb.Append("<tr><td>");
                        sb.Append("<h3>Procedimentos</h3>");
                        sb.Append("</td></tr>");

                        sb.Append("<tr><td>");

                        sb.Append("<table width='100%' cellpadding='0' cellspacing='0' border='0'>");
                        sb.Append("<tr><td><b>Código</b></td><td width='35%'><b>Procedimento</b></td><td><b>Especialidade</b></td><td><b>Preço</b></td></tr>");

                        foreach (var p in procedimentos)
                        {
                            sb.Append("<tr>");

                            sb.Append("<td>"); sb.Append(p.Procedimento.Codigo); sb.Append("</td>");
                            sb.Append("<td>"); sb.Append(p.Procedimento.Nome); sb.Append("</td>");
                            sb.Append("<td>"); sb.Append(p.Procedimento.Especialidade); sb.Append("</td>");
                            sb.Append("<td>"); sb.Append(p.ValorSobrescrito.Value.ToString("N2")); sb.Append("</td>");

                            sb.Append("</tr>");
                        }

                        sb.Append("</table>");

                        sb.Append("</td></tr>");
                    }

                    sb.Append("</table><br/>");
                }

                sb.Append("</td></tr>");
            }

            sb.Append("</table>");

            litCorpo.Text = sb.ToString();
        }
    }
}