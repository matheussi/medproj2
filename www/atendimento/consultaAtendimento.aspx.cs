namespace MedProj.www.atendimento
{
    using System;
    using System.Web;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using Ent = MedProj.Entidades;
    using LC.Web.PadraoSeguros.Facade;
    using Enty = LC.Web.PadraoSeguros.Entity;
    using System.Text;
    using System.Data;

    public partial class consultaAtendimento : System.Web.UI.Page
    {
        long IdAtendimento
        {
            get
            {
                if (ViewState["idAt"] == null) return 0;
                return (long)ViewState["idAt"];
            }

            set { ViewState["idAt"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (MedProj.www.Util.UsuarioLogado.TipoUsuario != MedProj.www.Util.UsuarioLogado.Tipo.ContratoDePrestador &&
                MedProj.www.Util.UsuarioLogado.IDUnidade == 0)
            {
                Response.Redirect("~/atendimento/selecionaPrestador.aspx");
            }

            litPrestador.Text = Util.UsuarioLogado.Nome;

            if (!IsPostBack) 
            {
                txtDataDe.Text = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
                txtDataAte.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        protected void cmdLocalizar_Click(object sender, EventArgs e)
        {
            string numero = txtNumeroCartao.Text;
            DateTime? de  = Util.CTipos.CStringToDateTimeG(txtDataDe.Text);
            DateTime? ate = Util.CTipos.ToDateTime(txtDataAte.Text, 23, 59, 59, 990);

            #region valicacoes 
            if (de == null || ate == null)
            {
                Util.Geral.Alerta(this, "Os parâmetros de data são obrigatórios");
                return;
            }
            if (de != null && ate != null && ate < de)
            {
                Util.Geral.Alerta(this, "Data de início não pode ser superior à data término");
                return;
            }
            //if (ate.Value.Subtract(de.Value).Days > 740)
            //{
            //    Util.Geral.Alerta(this, "Intervalo de data não pode ser superior a dois anos");
            //    return;
            //}
            #endregion

            DataTable dt = AtendimentoCredFacade.Instance.CarregarPorParametros(
                Util.UsuarioLogado.IDUnidade, txtNome.Text, txtCPF.Text, numero, de.Value, ate.Value);

            grid.DataSource = dt;
            grid.DataBind();

            if (dt == null || dt.Rows.Count == 0)
                litResultado.Text = "Nenhum atendimento encontrado";
            else
                litResultado.Text = "";
        }

        protected void grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("detalhe"))
            {
                this.IdAtendimento = Util.Geral.ObterDataKeyValDoGrid<long>(grid, e, 0);
                List<Ent.AtendimentoProcedimento> procedimentos = AtendimentoCredFacade.Instance.CarregarProcedimentosDoAtendimento(this.IdAtendimento);
                gridDetalhe.DataSource = procedimentos;
                gridDetalhe.DataBind();

                Enty.ContratoBeneficiario cb = Enty.ContratoBeneficiario.CarregarTitular(procedimentos[0].Atendimento.Contrato.ID, null);

                litDetalhe.Text = string.Concat("Beneficiário: ", cb.BeneficiarioNome,
                    "<br>Cartão: ", procedimentos[0].Atendimento.Contrato.Numero,
                    "<br>Data: ", procedimentos[0].Atendimento.Data.ToString("dd/MM/yyyy HH:mm"));

                litTotal.Text = string.Concat("Total: ", procedimentos.Where(p => p.Cancelado == false).Sum(p => p.Valor).ToString("C"));

                lnkPrint.Attributes.Remove("onclick");
                lnkPrint.Attributes.Add("onclick", "window.open('view.aspx?" + Util.Keys.IdKey + "=" + this.IdAtendimento.ToString() + "'); return false;");

                Util.Geral.JSScript(this, "showModal()");
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
        }

        protected void grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
        }

        protected void lnkEmail_Click(object sender, EventArgs e)
        {
            //if (txtEmail.Text.Trim() == "")
            //{
            //    Util.Geral.JSScript(this, "showModal();alert('Nenhum e-mail informado.');");
            //    return;
            //}

            Ent.Atendimento atendimento = AtendimentoCredFacade.Instance.Carregar(this.IdAtendimento);
            Enty.ContratoBeneficiario titular = Enty.ContratoBeneficiario.CarregarTitular(atendimento.Contrato.ID, null);

            string err = "";
            bool ret = Util.Geral.Mail.Enviar("Atendimento", documentoAtendimento(this.IdAtendimento), titular.BeneficiarioEmail, true, out err);

            if (ret)
            {
                Util.Geral.JSScript(this, "showModal();alert('E-mail enviado para " + titular.BeneficiarioEmail + ".');");
            }
            else
            {
                Util.Geral.JSScript(this, "showModal();alert('Não foi possível envar para " + titular.BeneficiarioEmail + ".');");
            }
        }

        protected void lnkPrint_Click(object sender, EventArgs e)
        {
        }


        string documentoAtendimento(long atendimentoId)
        {
            return Util.Geral.ComprovacaoAtendimentoDoc(atendimentoId, 0, null);

            //Ent.Atendimento atendimento = AtendimentoCredFacade.Instance.Carregar(atendimentoId);

            //StringBuilder sb = new StringBuilder();

            //sb.Append("<html>");
            //sb.Append("<body leftmargin='15' style='font-family:arial'>");
            //sb.Append("<table cellpadding='10px' width='100%'>");
            //sb.Append("<tr>");
            //sb.Append("<td align='center'><img src='"); sb.Append(System.Configuration.ConfigurationManager.AppSettings["logoUrl"]); sb.Append("'/></td>");
            //sb.Append("</tr>");
            //sb.Append("</table>");
            //sb.Append("<table cellpadding='10px' style='border: solid 1px gray;background-color:gainsboro' width='100%'>");
            //sb.Append("<tr>");
            //sb.Append("<td align='center'>");
            //sb.Append("<b>Comprovante de atendimento</b>");
            //sb.Append("</td>");
            //sb.Append("</tr>");
            //sb.Append("</table>");
            //sb.Append("<br>");

            ////HEADER 

            //sb.Append("<table width='100%' cellspacing='0' cellpadding='4'>");
            //sb.Append("<tr><td style='border-bottom:solid 1px gray'><b>Credenciado</b></td></tr>");
            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td>");
            //sb.Append(atendimento.Unidade.Nome);
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

            //sb.Append("<table width='100%' cellspacing='0' cellpadding='4'>");
            //sb.Append("<tr><td colspan='2' style='border-bottom:solid 1px gray'><b>Beneficiário</b></td></tr>");
            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td width='130px'>Nome:</td>");
            //sb.Append("<td>"); sb.Append(titular.BeneficiarioNome); sb.Append("</td>");
            //sb.Append("</tr>");
            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td width='130px'>Data:</td>");
            //sb.Append("<td>"); sb.Append(atendimento.Data.ToString("dd/MM/yyyy")); sb.Append("</td>");
            //sb.Append("</tr>");

            //sb.Append("</table>");
            //sb.Append("<table width='100%' cellspacing='0' cellpadding='4'>");
            //sb.Append("<tr style='background-color:whitesmoke'><td colspan='2'>Procedimentos</td></tr>");

            //foreach (var proc in atendimento.Procedimentos)
            //{
            //    sb.Append("<tr style='background-color:whitesmoke'>");
            //    sb.Append("<td>");
            //    sb.Append(proc.Procedimento.Nome);
            //    sb.Append("</td>");
            //    sb.Append("<td align='right'>R$ ");
            //    sb.Append(proc.Valor.ToString("N2"));
            //    sb.Append("</td>");
            //    sb.Append("</tr>");
            //}

            //sb.Append("<tr style='background-color:whitesmoke'>");
            //sb.Append("<td colspan='2' align='right'>Total: R$ ");
            //sb.Append(atendimento.Procedimentos.Sum(p => p.Valor).ToString("N2"));
            //sb.Append("</tr>");

            //sb.Append("</table>");


            //sb.Append("<br><br>");
            //sb.Append("<table width='100%' cellspacing='0'>");
            //sb.Append("<tr>");
            //sb.Append("<td>Senha de autenticidade: JL98709U9Y09BHBB54DGSDAWQW</td>");
            //sb.Append("</tr>");
            //sb.Append("</table>");

            //sb.Append("</body></html>");

            //return sb.ToString();
        }

        protected void gridDetalhe_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Ent.AtendimentoProcedimento proc = e.Row.DataItem as Ent.AtendimentoProcedimento;

                if (proc != null && proc.Cancelado)
                    e.Row.ForeColor = System.Drawing.Color.Red;

                //if (proc.Atendimento.FormaPagto == Ent.Enuns.FormaPagtoAtendimento.Cartao)
                //    e.Row.Cells[5].Text = "Cartão";
                //else
                //    e.Row.Cells[5].Text = "Dinheiro";
            }
        }
    }
}