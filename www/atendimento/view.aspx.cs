using LC.Web.PadraoSeguros.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ent = MedProj.Entidades;
using Enty = LC.Web.PadraoSeguros.Entity;

namespace MedProj.www.atendimento
{
    public partial class view : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (! string.IsNullOrEmpty(Request[Util.Keys.IdKey]))
                {
                    Response.Write(documentoAtendimento(Convert.ToInt64(Request[Util.Keys.IdKey])));
                }
            }
        }

        string documentoAtendimento(long atendimentoId)
        {
            return Util.Geral.ComprovacaoAtendimentoDoc(atendimentoId, 0, null);

            //Ent.Atendimento atendimento = AtendimentoCredFacade.Instance.Carregar(atendimentoId);

            //StringBuilder sb = new StringBuilder();

            //sb.Append("<html>");
            //sb.Append("<body leftmargin='15' topmargin='5' style='font-family:arial'>");
            //sb.Append("<table cellpadding='10px' width='850px'>");
            //sb.Append("<tr>");
            //sb.Append("<td align='center'><img src='"); sb.Append(System.Configuration.ConfigurationManager.AppSettings["logoUrl"]); sb.Append("'/></td>");
            //sb.Append("</tr>");
            //sb.Append("</table>");
            //sb.Append("<table cellpadding='10px' style='border: solid 1px gray;background-color:gainsboro' width='850px'>");
            //sb.Append("<tr>");
            //sb.Append("<td align='center'>");
            //sb.Append("<b>Comprovante de atendimento</b>");
            //sb.Append("</td>");
            //sb.Append("</tr>");
            //sb.Append("</table>");
            //sb.Append("<br>");

            ////HEADER 

            //sb.Append("<table width='850px' cellspacing='0' cellpadding='4'>");
            //sb.Append("<tr><td style='border-bottom:solid 1px gray'><b>Credenciado</b></td></tr>");
            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td>");
            //sb.Append(atendimento.Unidade.Nome); //sb.Append(Util.UsuarioLogado.NomeUnidade);
            //sb.Append("</td>");
            //sb.Append("</tr>");
            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td>");
            //sb.Append(string.Concat(atendimento.Unidade.Endereco, ", ", atendimento.Unidade.Numero, " - ", atendimento.Unidade.Bairro, " - ", atendimento.Unidade.Cidade, " - ", atendimento.Unidade.UF)); //sb.Append(Util.UsuarioLogado.EnderecoUnidade);
            //sb.Append("</td>");
            //sb.Append("</tr>");

            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td>");

            //if (!string.IsNullOrEmpty(atendimento.Unidade.Telefone))
            //    sb.Append(string.Concat("<b>Telefone:</b> ", atendimento.Unidade.Telefone));

            //if (!string.IsNullOrEmpty(atendimento.Unidade.Email))
            //{
            //    if (!string.IsNullOrEmpty(atendimento.Unidade.Telefone)) sb.Append("&nbsp;&nbsp;");

            //    sb.Append(string.Concat("<b>E-mail:</b> ", atendimento.Unidade.Email));
            //}

            //sb.Append("</td>");
            //sb.Append("</tr>");

            //sb.Append("</table>");

            //sb.Append("<br />");

            ////DETAIL

            //Enty.ContratoBeneficiario titular = Enty.ContratoBeneficiario.CarregarTitular(atendimento.Contrato.ID, null);

            //sb.Append("<table width='850px' cellspacing='0' cellpadding='4'>");
            //sb.Append("<tr><td colspan='2' style='border-bottom:solid 1px gray'><b>Beneficiário</b></td></tr>");
            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td width='110px'>Nome:</td>");
            //sb.Append("<td>"); sb.Append(titular.BeneficiarioNome); sb.Append("</td>");
            //sb.Append("</tr>");
            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td width='110px'>Data:</td>");
            //sb.Append("<td>"); sb.Append(atendimento.Data.ToString("dd/MM/yyyy")); sb.Append("</td>");
            //sb.Append("</tr>");

            //sb.Append("</table>");
            //sb.Append("<table width='850px' cellspacing='0' cellpadding='4'>");
            //sb.Append("<tr style='background-color:whitesmoke'><td>Especialidade</td><td colspan='2'>Procedimento</td></tr>");

            //List<Entidades.AtendimentoProcedimento> ordernados = AtendimentoCredFacade.Instance.CarregarProcedimentosDoAtendimento(atendimento.ID).OrderBy(p => p.Procedimento.Nome).ToList();
            //foreach (var proc in ordernados)
            //{
            //    sb.Append("<tr style='background-color:whitesmoke'>");

            //    sb.Append("<td>");
            //    sb.Append(proc.Especialidade.Nome);
            //    sb.Append("</td>");

            //    sb.Append("<td>");

            //    if (proc.Cancelado) sb.Append("<strike>");
            //    sb.Append(proc.Procedimento.Codigo);
            //    sb.Append("-");
            //    sb.Append(proc.Procedimento.Nome);
            //    if (proc.Cancelado) sb.Append("</strike>");
            //    sb.Append("</td>");
            //    sb.Append("<td align='right'>R$ ");
            //    if (proc.Cancelado) sb.Append("0,00");
            //    else                sb.Append(proc.Valor.ToString("N2"));
            //    sb.Append("</td>");
            //    sb.Append("</tr>");
            //}

            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td colspan='3' align='right'>Total: R$ ");
            //sb.Append(atendimento.Procedimentos.Where(p => p.Cancelado == false).Sum(p => p.Valor).ToString("N2"));
            //sb.Append("</tr>");

            //sb.Append("</table>");


            //sb.Append("<br><br>");
            //sb.Append("<table width='850px' cellspacing='0'>");
            //sb.Append("<tr>");
            //sb.Append("<td>Senha de autenticidade: JL98709U9Y09BHBB54DGSDAWQW</td>");
            //sb.Append("</tr>");
            //sb.Append("</table>");

            //sb.Append("</body></html>");

            //return sb.ToString();
        }
    }
}