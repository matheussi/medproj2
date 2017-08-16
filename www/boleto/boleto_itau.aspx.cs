namespace MedProj.www.boletoReg
{
    using System;
    using System.Web;
    using System.Data;
    using System.Web.UI;
    //using Impactro.Cobranca;
    using System.Collections;
    using System.Web.Security;
    using System.Configuration;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls.WebParts;
    using Ent = LC.Web.PadraoSeguros.Entity;
    using BoletoNet;
    using System.IO;
    using LC.Web.PadraoSeguros.Facade;

    public partial class boleto_itau : System.Web.UI.Page
    {
        int DigitoM10(string strintNumero)
        {
            long intNumero = Convert.ToInt64(strintNumero);

            int[] intPesos = { 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1 };
            string strText = intNumero.ToString();

            if (strText.Length > 16)
                throw new Exception("Número não suportado pela função!");

            int intSoma = 0;
            int intIdx = 0;
            for (int intPos = strText.Length - 1; intPos >= 0; intPos--)
            {
                intSoma += Convert.ToInt32(strText[intPos].ToString()) * intPesos[intIdx];
                intIdx++;
            }

            intSoma = intSoma % 10;
            intSoma = 10 - intSoma;
            if (intSoma == 10)
            {
                intSoma = 0;
            }

            return intSoma;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Entity 

            string bid = "", contid = "", cobid = "", instru = "";

            string[] instrucoes = new string[] { 
                "AS COBERTURAS DO SEGURO E DEMAIS BENEFICIOS ESTAO CONDICIONADAS AO PAGAMENTO DESTE DOCUMENTO.<br/>SR CAIXA, APOS O VENCIMENTO MULTA DE 10% E JUROS DE 1% A.M PRO RATA DIE.<br/>NAO RECEBER APOS 05 DIAS DO VENCIMENTO.", 
                "AS COBERTURAS DO SEGURO E DEMAIS BENEFICIOS ESTAO CONDICIONADAS AO PAGAMENTO DESTE DOCUMENTO.<br/>SR CAIXA, NAO RECEBER APOS O VENCIMENTO." };

            if (string.IsNullOrEmpty(Request["param"]))
            {
                bid     = Request["bid"];
                cobid   = Request["cobid"];
                contid  = Request["contid"];
                instru  = Request["instru"];

                if (string.IsNullOrEmpty(cobid)) return;
            }
            else
            {
                string[] param = new Util.Crypto.SecureQueryString().Decrypt(Request.Url.ToString().Split(new string[] { "param=" }, StringSplitOptions.None)[1]).Split('&');

                bid     = param[0].Split('=')[1];
                contid  = param[1].Split('=')[1];
                cobid   = param[2].Split('=')[1];
                instru  = param[3].Split('=')[1];
            }

            Ent.Beneficiario b = new Ent.Beneficiario(bid);
            b.Carregar();

            Ent.Contrato contrato = new Ent.Contrato(contid);
            contrato.Carregar();

            Ent.Cobranca cobr = new Ent.Cobranca(cobid);
            cobr.Carregar();

            if (contrato.EnderecoCobrancaID == null && contrato.EnderecoReferenciaID == null)
            {
                var _end = Ent.Endereco.CarregarPorDono(b.ID, Ent.Endereco.TipoDono.Beneficiario);
                if (_end != null && _end.Count > 0)
                {
                    contrato.EnderecoCobrancaID = _end[0].ID;
                }
                else
                {
                    throw new ApplicationException("Beneficiario " + b.ID + " sem endereco.");
                }
            }

            Ent.Endereco en = null;
            if (contrato.EnderecoCobrancaID != null) en = new Ent.Endereco(contrato.EnderecoCobrancaID);
            else en = new Ent.Endereco(contrato.EnderecoReferenciaID);

            try
            {
                en.Carregar();
            }
            catch
            {
                var ends = Ent.Endereco.CarregarPorDono(b.ID, Ent.Endereco.TipoDono.Beneficiario);
                if (ends != null && ends.Count > 0)
                {
                    en = new Ent.Endereco(ends[0].ID);
                    en.Carregar();
                }
            }

            string logradouro = en.Logradouro;
            if (!string.IsNullOrEmpty(en.Numero)) logradouro += string.Concat(", ", en.Numero);
            if (!string.IsNullOrEmpty(en.Complemento)) logradouro += string.Concat(" - ", en.Complemento);
            #endregion

            DateTime vencimento = cobr.DataVencimento;
            string carteira = ConfigurationManager.AppSettings["cedenteCARTEIRA"];

            #region parametros cedente 

            string[] dadosCedente = new string[] { 
                ConfigurationManager.AppSettings["cedenteCNPJ"],
                ConfigurationManager.AppSettings["cedenteNOME"],
                ConfigurationManager.AppSettings["cedenteAG"],
                ConfigurationManager.AppSettings["cedenteAGDV"],
                ConfigurationManager.AppSettings["cedenteCONTA"],
                ConfigurationManager.AppSettings["cedenteCONTADV"]
            };
            #endregion

            Cedente c = new Cedente(dadosCedente[0], dadosCedente[1], dadosCedente[2], dadosCedente[3], dadosCedente[4], dadosCedente[5]); //Cedente c = new Cedente("18.386.464/0001-43", "Clube Azul Vida saudavel de Beneficios", "9108", "0", "09882", "4");

            //Endereço do Cedente
            c.Endereco = new Endereco();
            c.Endereco.End = "SQN 416 Bloco M Ap 132";
            c.Endereco.Bairro = "Asa Norte";
            c.Endereco.CEP = "70879110";
            c.Endereco.Cidade = "Brasília";
            c.Endereco.UF = "DF"; 

            #region boleto 

            Boleto b4 = new Boleto(vencimento, cobr.Valor, carteira, Convert.ToString(cobr.ID), c);
            //b4.ValorCobrado = 4;
            b4.EspecieDocumento = new EspecieDocumento_Itau("99");
            b4.NumeroDocumento = Convert.ToString(cobr.ID).PadLeft(7, '0'); //"0000001";
            //b4.Moeda = 9;

            b4.Sacado = new Sacado(b.CPF, b.Nome);
            b4.Sacado.Endereco.End = en.Logradouro;
            b4.Sacado.Endereco.Bairro = en.Bairro;
            b4.Sacado.Endereco.Cidade = en.Cidade;
            b4.Sacado.Endereco.CEP = en.CEP;
            b4.Sacado.Endereco.UF = en.CEP;
            b4.Sacado.Endereco.Numero = en.Numero;
            b4.LocalPagamento = "Preferencialmente no banco Itaú";

            //DateTime validadeAPartirDe = CobrancaFacade.Instancia.calculaValidadeBoleto(cobr.DataCriacao);
            //Instrucao_Itau item3 = new Instrucao_Itau();
            //item3.Descricao = "IMPORTANTE: Aceitar pagamento somente a partir de " + validadeAPartirDe.ToString("dd/MM/yyyy");
            //b4.Instrucoes.Add(item3);


            //todo: remover o campo cobr.InstrucaoAdicional do obj e do banco de dados, carregar o objeto adicional se necessario
            if (!string.IsNullOrEmpty(cobr.InstrucaoAdicional))
            {
                Instrucao_Itau item5 = new Instrucao_Itau();
                item5.Descricao = cobr.InstrucaoAdicional;
                b4.Instrucoes.Add(item5);
            }

            if (!string.IsNullOrEmpty(instru))
            {
                Instrucao_Itau item4 = new Instrucao_Itau();
                item4.Descricao = instrucoes[Convert.ToInt32(instru)];  //"Não receber após " + vencimento.AddMonths(3).ToString("dd/MM/yyyy");
                b4.Instrucoes.Add(item4);
            }

            //b4.DigitoNossoNumero = "1";
            //b4.VariacaoCarteira = "019";
            b4.Banco = new Banco(341);

            #endregion

            BoletoBancario bb = new BoletoBancario();

            bb.CodigoBanco = 341;
            bb.Boleto = b4;
            bb.Cedente.Carteira = carteira;
            bb.Boleto.Valida();
            bb.MostrarCodigoCarteira = true;
            bb.MostrarComprovanteEntrega = false;
            bb.MostrarEnderecoCedente = false;
            bb.FormatoCarne = false;

            string html = bb.MontaHtmlEmbedded(false, false);
            Response.Write(html
                .Replace("Boleto.Net", "Boleto")
                .Replace("Impressão", "Impressão <a href='#' onclick='print();'><img valign='middle' src='http://sispag.clubeazul.org.br/Images/print_16x16.gif' alt='imprimir' title='imprimir' border='0' /></a>"));

            #region comentado 

            //BoletoBancario bb;

            //short _codigoBanco = 341;
            //string carteira = "109";

            //string caminhoFisico = @"C:\inetpub\wwwroot\Ubrasp\www\var\boleto_file\";
            //string caminhoVirtual = "http://ubrasp.iphotel.info/var/boleto_file/";

            //bb = new BoletoBancario();
            //bb.CodigoBanco = _codigoBanco;

            //Cedente ced = new Cedente("49.938.327.0001-06", "UBRASP UNIÃO BRASILEIRA", "0001", "130147652");

            //ced.Codigo = "1201344";  //Convert.ToInt32(dtoced.conta.ToString());
            //ced.DigitoCedente = 2;
            //ced.ContaBancaria = new ContaBancaria("0001", "13014765");

            //BoletoNet.Boleto bol = new BoletoNet.Boleto(cobr.DataVencimento,
            //    cobr.Valor, carteira, Convert.ToString(cobr.ID).PadLeft(12, '0'), ced);
            //bol.NumeroDocumento = c.Numero.PadLeft(11, '0');

            //bol.Sacado = new Sacado(b.CPF.PadLeft(11, '0'), Ent.EntityBase.RetiraAcentos(b.Nome));
            //bol.Sacado.Endereco.End = Ent.EntityBase.RetiraAcentos(logradouro);
            //bol.Sacado.Endereco.Bairro = Ent.EntityBase.RetiraAcentos(en.Bairro);
            //bol.Sacado.Endereco.Cidade = Ent.EntityBase.RetiraAcentos(en.Cidade);
            //bol.Sacado.Endereco.CEP = en.CEP;
            //bol.Sacado.Endereco.UF = en.UF;

            //Instrucao_Itau instr = new Instrucao_Itau();
            //instr.Descricao = "Não receber após " + cobr.DataVencimento.AddDays(45).ToString("dd/MM/yyyy") + ".";
            //bol.Instrucoes.Add(instr);

            //bb.Boleto = bol;
            //bb.Boleto.Valida();

            ////string dvBarras = bol.CodigoBarra.Codigo.Substring(4, 1);
            ////string linhaDig = bol.CodigoBarra.LinhaDigitavel;
            ////bol.CodigoBarra.LinhaDigitavel = linhaDig.Substring(0, 38) + dvBarras + linhaDig.Substring(39, 15);

            ////System.Drawing.Image img = bb.GeraImagemCodigoBarras(bol);
            ////String arquivoCodigoBarra = caminhoFisico + Convert.ToString(cobr.ID) + ".gif";
            ////if (File.Exists(arquivoCodigoBarra)) File.Delete(arquivoCodigoBarra);
            ////img.Save(arquivoCodigoBarra);

            //string corpoBoleto = bb.MontaHtml(); //bb.MontaHtml(caminhoFisico, Convert.ToString(cobr.ID)).Replace(@"C:\Users\ACER E1 572 6830\AppData\Local\Temp\", "http://ubrasp.iphotel.info/images/boleto/");
            //Response.Write(corpoBoleto);

            #endregion
        }

        void geraArquivoCNAB_Remessa()
        {
            Cedente c = new Cedente("18.386.464/0001-43", "Clube Azul Vida saudavel de Beneficios", "9108", "0", "09882", "4");

            DateTime vencimento = new DateTime(2016, 12, 31);

            Boletos boletos = new Boletos();

            #region boleto 1 

            Boleto b1 = new Boleto(vencimento, 1m, "109", "1", c, new EspecieDocumento(341, "1"));
            b1.NumeroDocumento = "00000001";

            b1.Sacado = new Sacado("302.789.608-39", "Nome Cliente");
            b1.Sacado.Endereco.End = "Rua Pe Adelino";
            b1.Sacado.Endereco.Bairro = "Belem";
            b1.Sacado.Endereco.Cidade = "Sao Paulo";
            b1.Sacado.Endereco.CEP = "03303000";
            b1.Sacado.Endereco.UF = "SP";
            b1.Sacado.Endereco.Numero = "1047";
            b1.LocalPagamento = "Preferencialmente no banco Itaú";

            Instrucao_Itau item1 = new Instrucao_Itau(5); //5=instrucoes do titulo ; 10= nao protestar
            item1.Descricao = "Não receber após " + vencimento.AddMonths(3).ToString("dd/MM/yyyy");
            b1.Instrucoes.Add(item1);

            b1.Banco = new Banco(341);

            boletos.Add(b1);

            #endregion

            #region boleto 2 

            Boleto b2 = new Boleto(vencimento, 2m, "109", "2", c, new EspecieDocumento(341, "1"));
            b2.NumeroDocumento = "00000002";

            b2.Sacado = new Sacado("302.789.608-39", "Nome Cliente 2");
            b2.Sacado.Endereco.End = "Rua Pe Adelino";
            b2.Sacado.Endereco.Bairro = "Belem";
            b2.Sacado.Endereco.Cidade = "Sao Paulo";
            b2.Sacado.Endereco.CEP = "03303000";
            b2.Sacado.Endereco.UF = "SP";
            b2.Sacado.Endereco.Numero = "1047";
            b2.LocalPagamento = "Preferencialmente no banco Itaú";

            Instrucao_Itau item2 = new Instrucao_Itau(5);
            b2.Instrucoes.Add(item2);

            b2.Banco = new Banco(341);

            boletos.Add(b2);

            #endregion

            #region boleto 3 

            Boleto b3 = new Boleto(vencimento, 3m, "109", "3", c, new EspecieDocumento(341, "1"));
            b3.NumeroDocumento = "00000003";

            b3.Sacado = new Sacado("302.789.608-39", "Nome Cliente 3");
            b3.Sacado.Endereco.End = "Rua Pe Adelino";
            b3.Sacado.Endereco.Bairro = "Belem";
            b3.Sacado.Endereco.Cidade = "Sao Paulo";
            b3.Sacado.Endereco.CEP = "03303000";
            b3.Sacado.Endereco.UF = "SP";
            b3.Sacado.Endereco.Numero = "1047";
            b3.LocalPagamento = "Preferencialmente no banco Itaú";

            Instrucao_Itau item3 = new Instrucao_Itau(5);
            b3.Instrucoes.Add(item3);

            b3.Banco = new Banco(341);

            boletos.Add(b3);

            #endregion

            #region boleto 4 

            Boleto b4 = new Boleto(vencimento, 4m, "109", "4", c, new EspecieDocumento(341, "1"));
            b4.NumeroDocumento = "00000004";

            b4.Sacado = new Sacado("302.789.608-39", "Nome Cliente 4");
            b4.Sacado.Endereco.End = "Rua Pe Adelino";
            b4.Sacado.Endereco.Bairro = "Belem";
            b4.Sacado.Endereco.Cidade = "Sao Paulo";
            b4.Sacado.Endereco.CEP = "03303000";
            b4.Sacado.Endereco.UF = "SP";
            b4.Sacado.Endereco.Numero = "1047";
            b4.LocalPagamento = "Preferencialmente no banco Itaú";

            Instrucao_Itau item4 = new Instrucao_Itau(5);
            b4.Instrucoes.Add(item4);

            b4.Banco = new Banco(341);

            boletos.Add(b4);

            #endregion

            #region boleto 5 

            Boleto b5 = new Boleto(vencimento, 5m, "109", "5", c, new EspecieDocumento(341, "1"));
            b5.NumeroDocumento = "00000005";

            b5.Sacado = new Sacado("302.789.608-39", "Nome Cliente 5");
            b5.Sacado.Endereco.End = "Rua Pe Adelino";
            b5.Sacado.Endereco.Bairro = "Belem";
            b5.Sacado.Endereco.Cidade = "Sao Paulo";
            b5.Sacado.Endereco.CEP = "03303000";
            b5.Sacado.Endereco.UF = "SP";
            b5.Sacado.Endereco.Numero = "1047";
            b5.LocalPagamento = "Preferencialmente no banco Itaú";

            Instrucao_Itau item5 = new Instrucao_Itau(5);
            b5.Instrucoes.Add(item5);

            b5.Banco = new Banco(341);

            boletos.Add(b5);

            #endregion

            Banco banco = new Banco(341);

            ArquivoRemessa ar = new ArquivoRemessa(TipoArquivo.CNAB400);
            string msg = "";
            bool ok = ar.ValidarArquivoRemessa("", banco, c, boletos, 1, out msg);

            FileStream stream = new FileStream(@"C:\Users\ACER E1 572 6830\teste_remessa03.txt", FileMode.Create);
            ar.GerarArquivoRemessa("", banco, c, boletos, stream, 1);
            stream.Close(); 
            stream.Dispose();
        }

        void geraBoletoTeste()
        {
            //geraArquivoCNAB_Remessa(); return;
            #region Entity

            //Ent.Beneficiario b = new Ent.Beneficiario(Request["bid"]);
            //b.Carregar();

            //Ent.Contrato c = new Ent.Contrato(Request["contid"]);
            //c.Carregar();

            //Ent.Cobranca cobr = new Ent.Cobranca(Request["cobid"]);
            //cobr.Carregar();

            //if (c.EnderecoCobrancaID == null && c.EnderecoReferenciaID == null)
            //{
            //    throw new ApplicationException("Beneficiario " + b.ID + " sem endereco.");
            //}

            //Ent.Endereco en = null;
            //if (c.EnderecoCobrancaID != null) en = new Ent.Endereco(c.EnderecoCobrancaID);
            //else en = new Ent.Endereco(c.EnderecoReferenciaID);

            //try
            //{
            //    en.Carregar();
            //}
            //catch
            //{
            //    var ends = Ent.Endereco.CarregarPorDono(b.ID, Ent.Endereco.TipoDono.Beneficiario);
            //    if (ends != null && ends.Count > 0)
            //    {
            //        en = new Ent.Endereco(ends[0].ID);
            //        en.Carregar();
            //    }
            //}

            //string logradouro = en.Logradouro;
            //if (!string.IsNullOrEmpty(en.Numero)) logradouro += string.Concat(", ", en.Numero);
            //if (!string.IsNullOrEmpty(en.Complemento)) logradouro += string.Concat(" - ", en.Complemento);
            #endregion

            DateTime vencimento = new DateTime(2016, 12, 31, 23, 59, 59, 995);
            //Cedente c = new Cedente("18.386.464/0001-43", "Clube Azul Vida saudavel de Beneficios", "9108-0", "09882-4");
            Cedente c = new Cedente("18.386.464/0001-43", "Clube Azul Vida saudavel de Beneficios", "9108", "0", "09882", "4");

            //Na carteira 198 o código do Cedente é a conta bancária
            //c.Codigo = "1535547";
            //c.Convenio = 52;
            //c.ContaBancaria.Agencia = "9108";

            //Mostrar endereço do Cedente
            c.Endereco = new Endereco();
            c.Endereco.End = "SQN 416 Bloco M Ap 132";
            c.Endereco.Bairro = "Asa Norte";
            c.Endereco.CEP = "70879110";
            c.Endereco.Cidade = "Brasília";
            c.Endereco.UF = "DF";

            #region boleto 1

            Boleto b4 = new Boleto(vencimento, 1m, "109", "1", c);
            //b4.ValorCobrado = 4;
            b4.EspecieDocumento = new EspecieDocumento_Itau("99");
            b4.NumeroDocumento = "0000001";
            //b4.Moeda = 9;

            b4.Sacado = new Sacado("302.789.608-39", "Nome Cliente 1");
            b4.Sacado.Endereco.End = "Rua Pe Adelino";
            b4.Sacado.Endereco.Bairro = "Belem";
            b4.Sacado.Endereco.Cidade = "Sao Paulo";
            b4.Sacado.Endereco.CEP = "03303000";
            b4.Sacado.Endereco.UF = "SP";
            b4.Sacado.Endereco.Numero = "1047";
            b4.LocalPagamento = "Preferencialmente no banco Itaú";

            Instrucao_Itau item4 = new Instrucao_Itau();
            item4.Descricao = "Não receber após " + vencimento.AddMonths(3).ToString("dd/MM/yyyy");
            b4.Instrucoes.Add(item4);

            //b4.DigitoNossoNumero = "1";
            //b4.VariacaoCarteira = "019";
            b4.Banco = new Banco(341);

            #endregion

            BoletoBancario bb = new BoletoBancario();

            bb.CodigoBanco = 341;
            bb.Boleto = b4;
            bb.Cedente.Carteira = "109";
            bb.Boleto.Valida();
            bb.MostrarCodigoCarteira = true;
            bb.MostrarComprovanteEntrega = false;
            bb.MostrarEnderecoCedente = false;
            bb.FormatoCarne = false;

            string html = bb.MontaHtmlEmbedded(false, false);
            Response.Write(html);

            #region comentado

            //BoletoBancario bb;

            //short _codigoBanco = 341;
            //string carteira = "109";

            //string caminhoFisico = @"C:\inetpub\wwwroot\Ubrasp\www\var\boleto_file\";
            //string caminhoVirtual = "http://ubrasp.iphotel.info/var/boleto_file/";

            //bb = new BoletoBancario();
            //bb.CodigoBanco = _codigoBanco;

            //Cedente ced = new Cedente("49.938.327.0001-06", "UBRASP UNIÃO BRASILEIRA", "0001", "130147652");

            //ced.Codigo = "1201344";  //Convert.ToInt32(dtoced.conta.ToString());
            //ced.DigitoCedente = 2;
            //ced.ContaBancaria = new ContaBancaria("0001", "13014765");

            //BoletoNet.Boleto bol = new BoletoNet.Boleto(cobr.DataVencimento,
            //    cobr.Valor, carteira, Convert.ToString(cobr.ID).PadLeft(12, '0'), ced);
            //bol.NumeroDocumento = c.Numero.PadLeft(11, '0');

            //bol.Sacado = new Sacado(b.CPF.PadLeft(11, '0'), Ent.EntityBase.RetiraAcentos(b.Nome));
            //bol.Sacado.Endereco.End = Ent.EntityBase.RetiraAcentos(logradouro);
            //bol.Sacado.Endereco.Bairro = Ent.EntityBase.RetiraAcentos(en.Bairro);
            //bol.Sacado.Endereco.Cidade = Ent.EntityBase.RetiraAcentos(en.Cidade);
            //bol.Sacado.Endereco.CEP = en.CEP;
            //bol.Sacado.Endereco.UF = en.UF;

            //Instrucao_Itau instr = new Instrucao_Itau();
            //instr.Descricao = "Não receber após " + cobr.DataVencimento.AddDays(45).ToString("dd/MM/yyyy") + ".";
            //bol.Instrucoes.Add(instr);

            //bb.Boleto = bol;
            //bb.Boleto.Valida();

            ////string dvBarras = bol.CodigoBarra.Codigo.Substring(4, 1);
            ////string linhaDig = bol.CodigoBarra.LinhaDigitavel;
            ////bol.CodigoBarra.LinhaDigitavel = linhaDig.Substring(0, 38) + dvBarras + linhaDig.Substring(39, 15);

            ////System.Drawing.Image img = bb.GeraImagemCodigoBarras(bol);
            ////String arquivoCodigoBarra = caminhoFisico + Convert.ToString(cobr.ID) + ".gif";
            ////if (File.Exists(arquivoCodigoBarra)) File.Delete(arquivoCodigoBarra);
            ////img.Save(arquivoCodigoBarra);

            //string corpoBoleto = bb.MontaHtml(); //bb.MontaHtml(caminhoFisico, Convert.ToString(cobr.ID)).Replace(@"C:\Users\ACER E1 572 6830\AppData\Local\Temp\", "http://ubrasp.iphotel.info/images/boleto/");
            //Response.Write(corpoBoleto);

            #endregion
        }

        string FormatCode(string text, string with, int length, bool left)
        {
            //Esse método já existe, é PadLeft e PadRight da string
            length -= text.Length;
            if (left)
            {
                for (int i = 0; i < length; ++i)
                {
                    text = with + text;
                }
            }
            else
            {
                for (int i = 0; i < length; ++i)
                {
                    text += with;
                }
            }
            return text;
        }
        string FormatCode(string text, string with, int length)
        {
            return FormatCode(text, with, length, false);
        }
        string FormatCode(string text, int length)
        {
            return text.PadLeft(length, '0');
        }

        //public override void FormataLinhaDigitavel(Boleto boleto)
        //{
        //    int _dacNossoNumero = 1, Codigo = 341;
        //    try
        //    {
        //        string numeroDocumento = FormatCode(boleto.NumeroDocumento.ToString(), 7);
        //        string codigoCedente = FormatCode(boleto.Cedente.Codigo.ToString(), 5);
        //        string agencia = FormatCode(boleto.Cedente.ContaBancaria.Agencia, 4);

        //        string AAA = FormatCode(Codigo.ToString(), 3);
        //        string B = boleto.Moeda.ToString();
        //        string CCC = boleto.Carteira.ToString();
        //        string DD = boleto.NossoNumero.Substring(0, 2);
        //        string X = Mod10(AAA + B + CCC + DD).ToString();
        //        string LD = string.Empty; //Linha Digitável

        //        string DDDDDD = boleto.NossoNumero.Substring(2, 6);

        //        string K = string.Format(" {0} ", _dacBoleto);

        //        string UUUU = FatorVencimento(boleto).ToString();
        //        string VVVVVVVVVV = boleto.ValorBoleto.ToString("f").Replace(",", "").Replace(".", "");

        //        string C1 = string.Empty;
        //        string C2 = string.Empty;
        //        string C3 = string.Empty;
        //        string C5 = string.Empty;

        //        #region AAABC.CCDDX

        //        C1 = string.Format("{0}{1}{2}.", AAA, B, CCC.Substring(0, 1));
        //        C1 += string.Format("{0}{1}{2} ", CCC.Substring(1, 2), DD, X);

        //        #endregion AAABC.CCDDX

        //        #region UUUUVVVVVVVVVV

        //        VVVVVVVVVV = FormatCode(VVVVVVVVVV, 10);
        //        C5 = UUUU + VVVVVVVVVV;

        //        #endregion UUUUVVVVVVVVVV

        //        if (boleto.Carteira == "175" || boleto.Carteira == "176" || boleto.Carteira == "178" || boleto.Carteira == "109" || boleto.Carteira == "121" || boleto.Carteira == "112")//MarcielTorres - adicionado a carteira 112
        //        {
        //            #region Definições
        //            /* AAABC.CCDDX.DDDDD.DEFFFY.FGGGG.GGHHHZ.K.UUUUVVVVVVVVVV
        //      * ------------------------------------------------------
        //      * Campo 1
        //      * AAABC.CCDDX
        //      * AAA - Código do Banco
        //      * B   - Moeda
        //      * CCC - Carteira
        //      * DD  - 2 primeiros números Nosso Número
        //      * X   - DAC Campo 1 (AAABC.CCDD) Mod10
        //      * 
        //      * Campo 2
        //      * DDDDD.DEFFFY
        //      * DDDDD.D - Restante Nosso Número
        //      * E       - DAC (Agência/Conta/Carteira/Nosso Número)
        //      * FFF     - Três primeiros da agência
        //      * Y       - DAC Campo 2 (DDDDD.DEFFF) Mod10
        //      * 
        //      * Campo 3
        //      * FGGGG.GGHHHZ
        //      * F       - Restante da Agência
        //      * GGGG.GG - Número Conta Corrente + DAC
        //      * HHH     - Zeros (Não utilizado)
        //      * Z       - DAC Campo 3
        //      * 
        //      * Campo 4
        //      * K       - DAC Código de Barras
        //      * 
        //      * Campo 5
        //      * UUUUVVVVVVVVVV
        //      * UUUU       - Fator Vencimento
        //      * VVVVVVVVVV - Valor do Título 
        //      */
        //            #endregion Definições

        //            #region DDDDD.DEFFFY

        //            string E = _dacNossoNumero.ToString();
        //            string FFF = agencia.Substring(0, 3);
        //            string Y = Mod10(DDDDDD + E + FFF).ToString();

        //            C2 = string.Format("{0}.", DDDDDD.Substring(0, 5));
        //            C2 += string.Format("{0}{1}{2}{3} ", DDDDDD.Substring(5, 1), E, FFF, Y);

        //            #endregion DDDDD.DEFFFY

        //            #region FGGGG.GGHHHZ

        //            string F = agencia.Substring(3, 1);
        //            string GGGGGG = boleto.Cedente.ContaBancaria.Conta + boleto.Cedente.ContaBancaria.DigitoConta;
        //            string HHH = "000";
        //            string Z = Mod10(F + GGGGGG + HHH).ToString();

        //            C3 = string.Format("{0}{1}.{2}{3}{4}", F, GGGGGG.Substring(0, 4), GGGGGG.Substring(4, 2), HHH, Z);

        //            #endregion FGGGG.GGHHHZ
        //        }
        //        else if (boleto.Carteira == "198" || boleto.Carteira == "107"
        //             || boleto.Carteira == "122" || boleto.Carteira == "142"
        //             || boleto.Carteira == "143" || boleto.Carteira == "196")
        //        {
        //            #region Definições
        //            /* AAABC.CCDDX.DDDDD.DEEEEY.EEEFF.FFFGHZ.K.UUUUVVVVVVVVVV
        //      * ------------------------------------------------------
        //      * Campo 1 - AAABC.CCDDX
        //      * AAA - Código do Banco
        //      * B   - Moeda
        //      * CCC - Carteira
        //      * DD  - 2 primeiros números Nosso Número
        //      * X   - DAC Campo 1 (AAABC.CCDD) Mod10
        //      * 
        //      * Campo 2 - DDDDD.DEEEEY
        //      * DDDDD.D - Restante Nosso Número
        //      * EEEE    - 4 primeiros numeros do número do documento
        //      * Y       - DAC Campo 2 (DDDDD.DEEEEY) Mod10
        //      * 
        //      * Campo 3 - EEEFF.FFFGHZ
        //      * EEE     - Restante do número do documento
        //      * FFFFF   - Código do Cliente
        //      * G       - DAC (Carteira/Nosso Numero(sem DAC)/Numero Documento/Codigo Cliente)
        //      * H       - zero
        //      * Z       - DAC Campo 3
        //      * 
        //      * Campo 4 - K
        //      * K       - DAC Código de Barras
        //      * 
        //      * Campo 5 - UUUUVVVVVVVVVV
        //      * UUUU       - Fator Vencimento
        //      * VVVVVVVVVV - Valor do Título 
        //      */
        //            #endregion Definições

        //            #region DDDDD.DEEEEY

        //            string EEEE = numeroDocumento.Substring(0, 4);
        //            string Y = Mod10(DDDDDD + EEEE).ToString();

        //            C2 = string.Format("{0}.", DDDDDD.Substring(0, 5));
        //            C2 += string.Format("{0}{1}{2} ", DDDDDD.Substring(5, 1), EEEE, Y);

        //            #endregion DDDDD.DEEEEY

        //            #region EEEFF.FFFGHZ

        //            string EEE = numeroDocumento.Substring(4, 3);
        //            string FFFFF = codigoCedente;
        //            string G = Mod10(boleto.Carteira + boleto.NossoNumero + numeroDocumento + codigoCedente).ToString();
        //            string H = "0";
        //            string Z = Mod10(EEE + FFFFF + G + H).ToString();
        //            C3 = string.Format("{0}{1}.{2}{3}{4}{5}", EEE, FFFFF.Substring(0, 2), FFFFF.Substring(2, 3), G, H, Z);

        //            #endregion EEEFF.FFFGHZ
        //        }
        //        else if (boleto.Carteira == "126" || boleto.Carteira == "131" || boleto.Carteira == "146" || boleto.Carteira == "150" || boleto.Carteira == "168")
        //        {
        //            throw new NotImplementedException("Função não implementada.");
        //        }

        //        boleto.CodigoBarra.LinhaDigitavel = C1 + C2 + C3 + K + C5;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Erro ao formatar linha digitável.", ex);
        //    }
        //}
    }
}