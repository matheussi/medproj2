using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using LC.Web.PadraoSeguros.Entity;
using System.Configuration;

namespace MedProj.www
{
    public partial class boleto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(Request["key"])) return;

                Cobranca cobranca = null;

                try
                {
                    cobranca = new Cobranca(Request["key"]);
                    cobranca.Carregar();
                }
                catch
                {
                    return;
                }

                Contrato contrato = new Contrato(cobranca.PropostaID);
                contrato.Carregar();

                ContratoBeneficiario titular = ContratoBeneficiario.CarregarTitular(contrato.ID, null);
                Beneficiario beneficiario = new Beneficiario(titular.BeneficiarioID);
                beneficiario.Carregar();

                String uri = "";

                decimal Valor = cobranca.Valor;
                string end1 = "", end2 = "";
                String instrucoes = "Valor mínimo de recarga: R$ 30,00<br>Para utilizar seu cartão, suas taxas administrativas devem estar em dia.<br>Para regularização e novos boletos - ligue 21 3916-7277<br>Todas as informações deste boleto são de responsabilidade do cedente";

                IList<Endereco> enderecos = Endereco.CarregarPorDono(beneficiario.ID, Endereco.TipoDono.Beneficiario);
                if (enderecos != null && enderecos.Count > 0)
                {
                    string compl = ""; if (!string.IsNullOrEmpty(enderecos[0].Complemento)) { compl = " - " + enderecos[0].Complemento; }

                    end1 = string.Concat(enderecos[0].Logradouro, ", ", enderecos[0].Numero, compl);
                    end2 = string.Concat(enderecos[0].CEP, " - ", enderecos[0].Bairro, " - ", enderecos[0].Cidade, " - ", enderecos[0].UF);
                }

                String nome = beneficiario.Nome;

                string nossoNumero = cobranca.GeraNossoNumero();

                int dia = cobranca.DataVencimento.Day;
                int mes = cobranca.DataVencimento.Month;
                int ano = cobranca.DataVencimento.Year;

                String naoReceber = "Não receber após o vencimento.";

                uri = EntityBase.RetiraAcentos(String.Concat("?mailto=&nossonum=", nossoNumero, "&valor=", Valor.ToString("N2"), "&d_dia=", DateTime.Now.Day, "&d_mes=", DateTime.Now.Month, "&d_ano=", DateTime.Now.Year, "&p_dia=", DateTime.Now.Day, "&p_mes=", DateTime.Now.Month, "&p_ano=", DateTime.Now.Year, 
                    "&v_dia=", dia, "&v_mes=", mes, "&v_ano=", ano, 
                    "&numdoc2=", contrato.Numero.PadLeft(5, '0'), "&nome=", nome, 
                    "&cod_cli=", cobranca.ID, "&end1=", end1, "&end2=", end2, 
                    "&instr=", instrucoes, ".<br><br>" + naoReceber));

                string finalUrl = string.Concat(
                     ConfigurationManager.AppSettings["boletoUrl"], "?param=",
                     Util.Geral.EncryptBetweenPHP(uri));

                Response.Redirect(finalUrl);
            }
        }
    }
}