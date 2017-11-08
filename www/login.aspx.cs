namespace MedProj.www
{
    using System;
    using System.Web;
    using System.Data;
    using System.Linq;
    using System.Web.UI;
    using System.Web.Security;
    using System.Data.OleDb;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;

    using MedProj.Entidades;

    //using LC.SmartTools.Lead.Entity;
    //using LC.SmartTools.Lead.Facade;
    //using LC.SmartTools.Lead.www.SiteUtil.Seguranca;
    using System.Configuration;
    using System.Globalization;
    using LC.Web.PadraoSeguros.Facade;
    using System.Net.Mail;
    using LC.Framework.Phantom;
    using System.Text;
    //using LC.Framework.Infraestrutura.Comunicacao;
    //using LC.SmartTools.Lead.www.SiteUtil;

    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Remove("logado");
                Util.UsuarioLogado.Encerrar();
                FormsAuthentication.SignOut();

                //string[][] itens = new string[2][];

                //itens[0] = new string[] { "Clube Azul", "1" };
                //itens[1] = new string[] { "Clube Azul", "2" };

                //string or = "10218386464000143910800098824    0000                         000031430000000000000109                     I0100003143  15071700000000334483410000001N010101001000000000000001507170000000000000000000000000000000000000000233069899000158GALVOTECNICA LTDA EPP                   RUA SAO JOAO BATISTA                    BOTAFOGO    22270030RIO DE JANEIRO RJGALVOTECNICA LTDA EPP             15071700 000002";

                //string nw = or.Substring(0, 108) + "31" + or.Substring(110);

                //string linha = "10218386464000143910800098824                                 00003324            109000033246             I0614081700003324  00003324            10081700000000264100333063501000000000044000000000000000000000000000000000000000000000000000000000000000000000000002856700000000025970000000000000   15081700000000000000000000000PARA-CHOQUES ZONA SUL LTDA                                          B3000013";
                //decimal valor = (Convert.ToDecimal(linha.Substring(253, 13)) / Convert.ToDecimal(100)) + (Convert.ToDecimal(linha.Substring(175, 13)) / Convert.ToDecimal(100));

                //Entidades.NumeroCartao numero = null;
                //numero = new Entidades.NumeroCartao();
                //var contratoNumero = (Convert.ToInt64("63708700417315") + 1).ToString();
                //numero.Numero = contratoNumero;
                //numero.GerarDigitoVerificador();
                //numero.Contrato = new Contrato();
                //numero.Contrato.ID = 110698;

                //NumeroCartaoFacade.Instancia.Salvar(numero);

                //NumeroCartaoFacade.Instancia.CancelarNumerosDeCartao(null, null, 0);
            }
        }

        void temp()
        {
            System.Globalization.CultureInfo cinfo = new CultureInfo("pt-Br");
            //6370870002849218
            string linha = "HSY91080966041014072208424991407226370870002849218    00000000000020000000000547    000001AA 06     ";

            string aux = linha.Substring(13, 2); //tipo de registro

            aux = linha.Substring(34, 16); //identificacao

            aux = linha.Substring(15, 6); //data do credito

            aux = linha.Substring(21, 4); //agencia origem

            aux = linha.Substring(25, 3); //num lote

            aux = linha.Substring(28, 6); //data remessa

            aux = string.Concat(linha.Substring(54, 15), ",", linha.Substring(69, 2)); //valor pago

            aux = linha.Substring(54, 17);
            decimal valor = Convert.ToDecimal(aux, cinfo) / 100;

            int i = 0;
            //using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\matheussi\Documents\projmed\dados.mdb"))
            //{
            //    conn.Open();

            //    DataTable dt = new DataTable();
            //    OleDbDataAdapter adp = new OleDbDataAdapter("select * from procedimentos", conn);
            //    adp.Fill(dt);

            //    System.Globalization.CultureInfo cinfo = new CultureInfo("en-US");

            //    using (Contexto contexto = new Contexto())
            //    {
            //        foreach (DataRow row in dt.Rows)
            //        {
            //            Procedimento proc = new Procedimento();

            //            proc.CH = Convert.ToDecimal(row["CH"], cinfo);
            //            proc.Codigo = Convert.ToInt32(row["CODIGO"]);
            //            proc.Nome = Convert.ToString(row["PROCEDIMENTO"]);
            //            proc.Porte = Convert.ToString(row["PORTE"]);

            //            contexto.Procedimentos.Add(proc);
            //        }

            //        contexto.SaveChanges();
            //    }
            //}

            //using (System.Data.OleDb.OleDbConnection conn = new System.Data.OleDb.OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\matheussi\Documents\projmed\dados.mdb"))
            //{
            //    conn.Open();

            //    DataTable dt = new DataTable();
            //    OleDbDataAdapter adp = new OleDbDataAdapter("select * from especialidades", conn);
            //    adp.Fill(dt);

            //    System.Globalization.CultureInfo cinfo = new CultureInfo("en-US");

            //    using (Contexto contexto = new Contexto())
            //    {
            //        foreach (DataRow row in dt.Rows)
            //        {
            //            Especialidade esp = new Especialidade();

            //            esp.Codigo = Convert.ToString(row["CodigoEspec"]);
            //            esp.Descricao = Convert.ToString(row["Descrição"]);
            //            esp.Nome = Convert.ToString(row["Especialidade"]);

            //            contexto.Especialidades.Add(esp);
            //        }

            //        contexto.SaveChanges();
            //    }
            //}
        }

        void temp2()
        {
            //Microsoft.Jet.OLEDB.4.0
            //Microsoft.ACE.OLEDB.12.0
            string con = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\LCProjects\MedProj\www\files\import\3.xls;Extended Properties='Excel 8.0;HDR=Yes;'";
            using(OleDbConnection connection = new OleDbConnection(con))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [CONTRATO$]", connection);
                using(OleDbDataReader dr = command.ExecuteReader())
                {
                     while(dr.Read())
                     {
                         var row1Col0 = dr.GetString(0);
                     }
                }
            }
        }

        void temp3()
        {
            PrestadorUnidadeFacade.Instancia.TempSetaCoord();
        }

        void temp4()
        {
            //string qry = "select cobranca.* FROM cobranca INNER JOIN contrato ON cobranca_propostaId=contrato_id INNER JOIN operadora ON operadora_id = contrato_operadoraId WHERE cobranca_propostaId=206718 AND cobranca_cancelada <> 1  ORDER BY cobranca_parcela DESC ,cobranca_id";
            //IList<LC.Web.PadraoSeguros.Entity.Cobranca> emp = LocatorHelper.Instance.ExecuteQuery<LC.Web.PadraoSeguros.Entity.Cobranca>(qry, typeof(LC.Web.PadraoSeguros.Entity.Cobranca), null);
            //this.salvaProc("Dermatologia");
            //this.salvaProc("Ginecologia");
            //this.salvaProc("Urologia");
            //this.salvaProc("Proctologia");
            //this.salvaProc("Pneumologia");
            //this.salvaProc("Hematologia");
            //this.salvaProc("Nefrologia");
            //this.salvaProc("Ortopedia");
            //this.salvaProc("Pediatria");
            //this.salvaProc("Otorrinolaringologia");
            //this.salvaProc("Mastologia");
            //this.salvaProc("Cirurgia Plástica");
            //this.salvaProc("Hepatologia");
            //this.salvaProc("Gastroenterologia");
        }
        void salvaProc(String especialidade)
        {
            Procedimento proc = new Procedimento();
            proc.Categoria = "Consultas";
            proc.CH = 1;
            proc.Codigo = "10101012";
            proc.Nome = "Consulta em consultorio (no horario normal ou preestabelecido)";
            proc.Tabela = new TabelaProcedimento();
            proc.Tabela.ID = 3;
            proc.Especialidade = especialidade;

            ProcedimentoFacade.Instancia.Salvar(proc, 2, true);
        }

        void temp5_prestadores6meses()
        {
            string dataCorte = "2016-11-01";

            StringBuilder sb = new StringBuilder();
            sb.Append("<table>");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery("select unidade_id,data from unidade_procedimento where data >= '" + dataCorte + "' order by data", "result").Tables[0];

            List<voTemp5> vos = new List<voTemp5>();

            bool localizado = false;
            string aux1 = "", aux2 = "";
            foreach (DataRow row in dt.Rows)
            {
                localizado = false;
                aux1 = Convert.ToString(row[0]);

                foreach (var _vo in vos)
                {
                    if (_vo.id == aux1) { localizado = true; break; }
                }

                if (localizado) continue;

                aux2 = Convert.ToDateTime(row[1]).ToString("dd/MM/yyyy");

                vos.Add(new voTemp5 { id = aux1, data = aux2 });
            }

            object ret = null;
            DataTable dt2 = null;
            foreach (var vo in vos)
            {
                ret = LocatorHelper.Instance.ExecuteScalar("select id from unidade_procedimento where data < '" + dataCorte + "' and unidade_id=" + vo.id, null, null);
                if (ret != null && ret != DBNull.Value) continue;

                dt2 = LocatorHelper.Instance.ExecuteQuery("select id,nome,telefone,email from prestador_unidade where ID=" + vo.id, "result").Tables[0];

                sb.Append("<tr>");

                sb.Append("<td>");
                sb.Append(dt2.Rows[0]["nome"]);
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append(dt2.Rows[0]["telefone"]);
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append(dt2.Rows[0]["email"]);
                sb.Append("</td>");

                sb.Append("<td>");
                sb.Append(vo.data);
                sb.Append("</td>");

                sb.Append("</tr>");
            }

            sb.Append("</table>");

            Response.Write(sb.ToString());
        }
        class voTemp5
        {
            public string id { get; set; }
            public string data { get; set; }
        }

        void temp6_teste_calculo_juros()
        {
            LC.Web.PadraoSeguros.Entity.Cobranca c = new LC.Web.PadraoSeguros.Entity.Cobranca();
            c.Valor = 200;
            c.DataVencimento = new DateTime(2017, 4, 3);

            c.CalculaJurosMulta();

            ////////////////////////////////////////////////

            c.Valor = 200;
            int diasPassados = LC.Web.PadraoSeguros.Entity.Cobranca.DiferenciaEmDiasCorridos(c.DataVencimento, DateTime.Now);

            Decimal atrasoMulta = Convert.ToDecimal(10) / Convert.ToDecimal(100); //Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["jurosAtraso"]);
            //Decimal atrasoJuro = 0.01M; //0.0333M; //Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["jurosDia"]);

            decimal _multa = c.Valor;
            decimal _juro = c.Valor;

            _multa += (c.Valor * atrasoMulta); //multa

            decimal rateJuro = (c.Valor * 0.01M) / 30M;
            _juro = (rateJuro * (Convert.ToDecimal(diasPassados)));

            c.Valor = Math.Round(_multa + _juro, 2);
        }

        protected void cmdEntrar_Click(object sender, EventArgs e)
        {

#if DEBUG
            string[] arr = new string[] { "maden", "maden@@@" };

            //string erro = "";
            //Util.Geral.Mail.Enviar("assunto", "corpo", "denis.goncalves@wedigi.com.br", true, out erro);
#else
            string[] arr = new string[] { txtLogin.Value, txtSenha.Value };
#endif
            Usuario usuario = UsuarioFacade.Instance.LogOn(arr[0], arr[1]);

            if (usuario != null)
            {
                Session["logado"] = 1;
                Util.UsuarioLogado.ID = usuario.ID.ToString();
                Util.UsuarioLogado.Nome = usuario.Nome;

                switch (usuario.Tipo)
                {
                    case Entidades.Enuns.TipoUsuario.Administrador:
                    {
                        Util.UsuarioLogado.TipoUsuario = Util.UsuarioLogado.Tipo.Administrador; break;
                    }
                    case Entidades.Enuns.TipoUsuario.ContratoDePrestador:
                    {
                        Util.UsuarioLogado.TipoUsuario = Util.UsuarioLogado.Tipo.ContratoDePrestador;
                        Util.UsuarioLogado.Nome = usuario.Unidade.Owner.Nome;
                        Util.UsuarioLogado.IDUnidade = usuario.Unidade.ID;
                        Util.UsuarioLogado.NomeUnidade = usuario.Unidade.Nome;
                        Util.UsuarioLogado.EnderecoUnidade = string.Concat(usuario.Unidade.Endereco, ", ", usuario.Unidade.Numero, " - ", usuario.Unidade.Bairro, " - ", usuario.Unidade.Cidade, " - ", usuario.Unidade.UF);
                        Util.UsuarioLogado.FoneUnidade = usuario.Unidade.Telefone;
                        Util.UsuarioLogado.EmailUnidade = usuario.Unidade.Email;
                        break;
                    }
                    case Entidades.Enuns.TipoUsuario.Operador:
                    {
                        Util.UsuarioLogado.TipoUsuario = Util.UsuarioLogado.Tipo.Operador; break;
                    }
                    default:
                    {
                        Util.UsuarioLogado.TipoUsuario = Util.UsuarioLogado.Tipo.Indefinido; break;
                    }
                }

                Response.Redirect("default.aspx");
            }
            else
            {
                Util.Geral.Alerta(this, "Usuário ou senha incorreto(s).");
            }

            #region comentado 

            //Requisicao<string[]> requisicao = new Requisicao<string[]>(arr);
            //RetornoComplex<Usuario, DateTime> retorno = UsuarioFacade.Instancia.LogOn(requisicao);

            //if (retorno == null)
            //    litErro.Text = "Senha ou login incorreto(s)";
            //else
            //{
            //    UsuarioAutenticado.UsuarioNome = retorno.Objeto1.Nome;
            //    UsuarioAutenticado.UsuarioId = retorno.Objeto1.ID;
            //    UsuarioAutenticado.UsuarioEmail = retorno.Objeto1.Email;
            //    UsuarioAutenticado.UsuarioTipo = (int)retorno.Objeto1.Tipo;

            //    if (retorno.Objeto1.UltimoAcesso.HasValue)
            //        UsuarioAutenticado.UsuarioDataUltimoAcesso = retorno.Objeto2.ToString("dd/MM/yyyy HH:mm");
            //    else
            //        UsuarioAutenticado.UsuarioDataUltimoAcesso = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            //    if (retorno.Objeto1.Tipo != Entity.Enuns.TipoUsuario.Master)
            //    {
            //        UsuarioAutenticado.ContratanteId = retorno.Objeto1.Contratante.ID;

            //        if (!string.IsNullOrEmpty(retorno.Objeto1.Contratante.Logo))
            //            UsuarioAutenticado.ContratanteLogo = ConfigurationManager.AppSettings["logoVirtualBasePath"] + retorno.Objeto1.Contratante.Logo;
            //    }

            //    FormsAuthentication.SetAuthCookie(retorno.Objeto1.Email, false);

            //    Response.Redirect("default.aspx");
            //}
            #endregion
        }

        protected void cmdEnviarSenha_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailSenha.Text))
            {
                Util.Geral.Alerta(this, "Informe seu login.");
                return;
            }

            Usuario usuario = UsuarioFacade.Instance.Carregar(EmailSenha.Text);

            if (usuario == null)
            {
                Util.Geral.Alerta(this, "Login não encontrado.");
            }
            else
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                sb.Append("Olá!");
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append("Você está recebendo este e-mail por ter solicitado um lembrete de sua senha no sistema Clube Azul.");
                sb.Append(Environment.NewLine);
                sb.Append("Essa solicitação ocorreu em "); sb.Append(DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("pt-Br")));
                sb.Append(" às ");
                sb.Append(DateTime.Now.ToString("HH:mm", new CultureInfo("pt-Br"))); sb.Append(".");
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append("Seus dados de acesso são:");
                sb.Append(Environment.NewLine);
                sb.Append("Login: "); sb.Append(usuario.Login);
                sb.Append(Environment.NewLine);
                sb.Append("Senha: "); sb.Append(usuario.Senha);
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append("Este é um e-mail automático, por favor não o responda.");


                string err = "";
                //int? porta = null;
                //bool seguro = false;


                //if (ConfigurationManager.AppSettings["appEmailSecure"].ToUpper() == "TRUE") seguro = true;
                //if (ConfigurationManager.AppSettings["appEmailSmtpPort"] != "") porta = Convert.ToInt32(ConfigurationManager.AppSettings["appEmailSmtpPort"]);

                bool ok = Util.Geral.Mail.Enviar("[CLUBE AZUL] - Senha", sb.ToString(), usuario.Login, false, out err);

                if (ok)
                    Util.Geral.Alerta(this, "Lembrete de senha enviado.");
                else
                {
                    Util.Geral.Alerta(this, "Não foi possível enviar o lembrete de senha.");
                }
            }
        }
    }
}