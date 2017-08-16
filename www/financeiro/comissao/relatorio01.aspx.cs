namespace MedProj.www.financeiro.comissao
{
    using System;
    using System.IO;
    using System.Web;
    using System.Data;
    using System.Text;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Collections.Generic;
    using LC.Web.PadraoSeguros.Entity;
    using LC.Web.PadraoSeguros.Facade;
    using LC.Framework.Phantom;
    using MedProj.www.Util;
    using MedProj.Entidades;

    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using iTextSharp.text.html;
    using iTextSharp.text.html.simpleparser;

    public partial class relatorio01 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.carregarCorretores();
                this.carregarAssociadosPJ();
                this.carregarTabelasDeComissao();

                this.txtAte.Text = DateTime.Now.ToString("dd/MM/yyyy");
                txtDe.Text = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy");
            }
        }

        void carregarCorretores()
        {
            cboCorretor.Items.Clear();
            cboCorretor.Items.Add(new System.Web.UI.WebControls.ListItem("selecione", "-1"));
            var ret = CorretorFacade.Instancia.CarregarTodos(string.Empty);
            if (ret != null)
            {
                foreach (Corretor c in ret)
                {
                    cboCorretor.Items.Add(new System.Web.UI.WebControls.ListItem(c.Nome, c.ID.ToString()));
                }
            }
        }

        void carregarAssociadosPJ()
        {
            cboAssociadoPJ.Items.Clear();
            cboAssociadoPJ.Items.Add(new System.Web.UI.WebControls.ListItem("selecione", "-1"));
            var ret = AssociadoPJFacade.Instance.Carregar(string.Empty);
            if (ret != null)
            {
                foreach (AssociadoPJ c in ret)
                {
                    cboAssociadoPJ.Items.Add(new System.Web.UI.WebControls.ListItem(c.Nome, c.ID.ToString()));
                }
            }
        }

        void carregarTabelasDeComissao()
        {
            cboTabelaComissao.Items.Clear();
            cboTabelaComissao.Items.Add(new System.Web.UI.WebControls.ListItem("selecione", "-1"));

            List<RegraComissao> regras = RegraComissaoFacade.Instance.Carregar(string.Empty);

            if (regras != null && regras.Count > 0)
            {
                regras.ForEach(r =>
                    cboTabelaComissao.Items.Add(
                    new System.Web.UI.WebControls.ListItem(r.Nome, r.ID.ToString()))
                );
            }
        }

        List<ItemRelatorio> montarRelatorio()
        {
            litTotal.Text = "";
            string cond = "", condEx = "";

            if (cboTabelaComissao.SelectedIndex > 0)
            {
                cond = " and regracom_id=" + cboTabelaComissao.SelectedValue;
                condEx = " and regracom_id=" + cboTabelaComissao.SelectedValue;
            }

            if (cboCorretor.SelectedIndex > 0)
            {
                cond = " and corretor_id=" + cboCorretor.SelectedValue;
                condEx = " and regracomissaoexcecao_corretorId=" + cboCorretor.SelectedValue;
            }

            if (cboAssociadoPJ.SelectedIndex > 0)
            {
                cond += " and estipulante_id=" + cboAssociadoPJ.SelectedValue;
                condEx = " and contrato_estipulanteId=" + cboAssociadoPJ.SelectedValue;
            }

            DateTime? de = Util.CTipos.CStringToDateTimeG(txtDe.Text);
            if (de.HasValue)
            {
                cond += " and cobranca_dataPagto >= '" + de.Value.ToString("yyyy-MM-dd") + "'";
            }

            DateTime? ate = Util.CTipos.CStringToDateTimeG(txtAte.Text);
            if (ate.HasValue)
            {
                cond += " and cobranca_dataPagto <= '" + ate.Value.ToString("yyyy-MM-dd 23:59:59") + "'";
            }

            #region query comentada 
            //string sql = string.Concat(
            //    "select contrato_id, estipulante_id,cobranca_id, corretor_nome, estipulante_descricao, beneficiario_nome,cobranca_valor,cobranca_valorPagto,cobranca_dataPagto,cobranca_parcela, regracomitem_percentual,regracomitem_parcela ",
            //    "   from cobranca ",
            //    "       inner join contrato on contrato_id = cobranca_propostaId ",
            //    "       inner join corretor on corretor_id = contrato_corretorComissionadoId ",
            //    "       inner join regraComissao on regracom_id = corretor_tabelaComissaoId ",
            //    "       inner join estipulante on estipulante_id = contrato_estipulanteId ",
            //    "       inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 ",
            //    "       inner join beneficiario on contratobeneficiario_beneficiarioId = beneficiario_id and contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 ",
            //    "       inner join regraComissaoItem on regracomitem_regraId = regracom_id ",
            //    "   where ",
            //    "       cobranca_pago=1 ", cond,
            //    "       and contrato_corretorComissionadoId is not null",
            //    "   order by corretor_nome, contrato_id,cobranca_parcela,regracomitem_parcela");
            #endregion

            string sql = string.Concat(
                "select contrato_id, contrato_numero, contratoadm_descricao,estipulante_id,cobranca_id, regracomitem_corretorId,corretor_id, corretor_nome, estipulante_descricao, beneficiario_nome,cobranca_valor,cobranca_valorPagto,cobranca_dataPagto,cobranca_parcela, cobranca_qtdVidas, regracomitem_percentual,regracomitem_parcela,regracomitem_vitalicio,ca_valor ",
                "   from cobranca ",
                "       inner join contrato on contrato_id = cobranca_propostaId ",
                "       inner join contratoadm on contratoadm_id = contrato_contratoAdmId ",
                "       inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 ",
                "       inner join beneficiario on contratobeneficiario_beneficiarioId = beneficiario_id and contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 ",
                "       inner join regraComissao_contrato on contrato_id = regracomissaocontrato_contratoId ",
                "       inner join regraComissao on regracom_id = regracomissaocontrato_regraId ",
                "       inner join regraComissaoItem on regracomitem_regraId = regracom_id ",
                "       inner join corretor on corretor_id = regracomitem_corretorId ",
                "       inner join estipulante on estipulante_id = regracom_estipulanteId ",
                "       left join config_adicional on cobranca_adicionalId = ca_id ",
                "   where ",
                "       cobranca_pago=1 and cobranca_cancelada=0 ", cond, //and contrato_id in(143274,144222)
                "   order by corretor_nome, contrato_id,cobranca_parcela,regracomitem_parcela");

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(sql, "result").Tables[0];

            // Carrega excecao
            sql = string.Concat(
                "select regraComissaoExcecao.* ",
                "   from regraComissaoExcecao ",
                "       inner join contrato on contrato_id = regracomissaoexcecao_contratoId ",
                "       inner join regraComissao on regracom_id = regracomissaoexcecao_regraid ",
                "   where ",
                "       regracom_id > 0", condEx,
                "   order by regracom_id, contrato_id, regracomissaoexcecao_corretorId, regracomissaoexcecao_parcela");

            DataTable dtExc = LocatorHelper.Instance.ExecuteQuery(sql, "result").Tables[0];

            decimal percentualDeComissao = 0;
            List<ItemRelatorio> relatorio = new List<ItemRelatorio>();
            System.Globalization.CultureInfo usCinfo = new System.Globalization.CultureInfo("en-US");

            int totalVidas = 0;
            decimal totalPago = 0, totalComissao = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (existeCobranca(CTipos.CToLong(row["cobranca_id"]), CTipos.CToLong(row["corretor_id"]), relatorio)) continue;

                percentualDeComissao = obtemPercentualDeComissao(CTipos.CToLong(row["estipulante_id"]), CTipos.CToLong(row["contrato_id"]), CTipos.CToLong(row["cobranca_id"]), CTipos.CToInt(row["corretor_id"]), CTipos.CToInt(row["regracomitem_parcela"]), CTipos.CToInt(row["cobranca_parcela"]), dt, dtExc);
                if (percentualDeComissao == 0) continue;

                ItemRelatorio i = new ItemRelatorio();

                i.Beneficiario = CTipos.CToString(row["beneficiario_nome"]);
                i.BeneficiarioCartao = CTipos.CToString(row["contrato_numero"]);
                i.ContratoADM = CTipos.CToString(row["contratoadm_descricao"]);
                i.CobrancaValor = CTipos.ToDecimal(row["cobranca_valor"]) - CTipos.ToDecimal(row["ca_valor"]);
                i.Calculado = i.CobrancaValor * (percentualDeComissao / Convert.ToDecimal(100));
                i.CobrancaDataPago = CTipos.CObjectToDateTime(row["cobranca_dataPagto"], usCinfo);
                i.CobrancaId = CTipos.CToLong(row["cobranca_id"]);
                i.CobrancaParcela = CTipos.CToInt(row["cobranca_parcela"]);
                i.CobrancaValorPago = CTipos.ToDecimal(row["cobranca_valorPagto"]) - CTipos.ToDecimal(row["ca_valor"]);
                i.CobrancaQtdVidas = CTipos.CToInt(row["cobranca_qtdVidas"]);
                i.ContratoId = CTipos.CToLong(row["contrato_id"]);
                i.Corretor = CTipos.CToString(row["corretor_nome"]);
                i.CorretorId = CTipos.CToLong(row["corretor_id"]);
                i.Estipulante = CTipos.CToString(row["estipulante_descricao"]);
                i.ParcelaComissao = CTipos.CToInt(row["regracomitem_parcela"]);
                i.PercentualComissao = percentualDeComissao;
                relatorio.Add(i);

                totalPago       += i.CobrancaValor;
                totalComissao   += i.Calculado;
                totalVidas      += i.CobrancaQtdVidas;
            }

            gridCobrancas.DataSource = relatorio;
            gridCobrancas.DataBind();

            if (relatorio != null && relatorio.Count > 0)
            {
                litTotal.Text = string.Concat("<strong>Totais - </strong>Pago: ",
                    totalPago.ToString("C"), " - Vidas: ", totalVidas, " - Comissão: ",
                    totalComissao.ToString("C"));
            }

            return relatorio;
        }

        bool existeCobranca(long cobrancaId, long corretorId, List<ItemRelatorio> relatorio)
        {
            bool existe = false;

            if (relatorio != null)
            {
                foreach (var i in relatorio)
                {
                    if (i.CobrancaId == cobrancaId && i.CorretorId == corretorId)
                    {
                        existe = true;
                        break;
                    }
                }
            }

            return existe;
        }

        decimal obtemPercentualDeComissao(long estipulanteId, long contratoId, long cobrancaId, int corretorId, int regraParcela, int cobrancaParcela, DataTable dt, DataTable dtExc)
        {
            decimal percentual = 0;

            //INICIO: Checa Excecoes
            DataRow[] exRows = dtExc.Select(string.Concat("regracomissaoexcecao_corretorId=", corretorId, " and regracomissaoexcecao_contratoId=", contratoId), "regracomissaoexcecao_regraid, regracomissaoexcecao_parcela asc");
            if (exRows.Length > 0)
            {
                //checa se tem instrução de NAO comissionar
                foreach (DataRow row in exRows)
                {
                    if (CTipos.CToInt(row["regracomissaoexcecao_naocomissionado"]) == 1) return 0;
                }

                foreach (DataRow row in exRows)
                {
                    if (CTipos.CToInt(row["regracomissaoexcecao_parcela"]) == cobrancaParcela)
                    {
                        return CTipos.ToDecimal(row["regracomissaoexcecao_percentual"]);
                    }
                }

                if (Convert.ToInt32(exRows[exRows.Length - 1]["regracomissaoexcecao_parcela"]) < cobrancaParcela &&
                    Convert.ToInt32(exRows[exRows.Length - 1]["regracomissaoexcecao_vitalicio"]) == 1)
                {
                    return CTipos.ToDecimal(exRows[exRows.Length - 1]["regracomissaoexcecao_percentual"]);
                }

                return 0; //se tem exeção, não pode computar a regra padrão, mesmo não achando valor
            }
            // FIM: Checa Excecoes

            foreach (DataRow row in dt.Rows)
            {
                if (CTipos.CToLong(row["cobranca_id"]) == cobrancaId &&
                    CTipos.CToLong(row["estipulante_id"]) == estipulanteId &&
                    CTipos.CToInt(row["corretor_id"]) == corretorId &&
                    CTipos.CToInt(row["cobranca_parcela"]) == regraParcela &&
                    CTipos.CToInt(row["regracomitem_parcela"]) == regraParcela)
                {
                    percentual = CTipos.ToDecimal(row["regracomitem_percentual"]);
                    break;
                }
            }

            if (percentual == 0) //não achou, verifica se tem vitaliciedade
            {
                //seleciona as parcelas comissionaveis do corretor para esta cobranca
                DataRow[] rows = dt.Select("corretor_id=" + corretorId + " and contrato_id=" + contratoId, "regracomitem_parcela asc");

                if (Convert.ToInt32(rows[rows.Length - 1]["regracomitem_parcela"]) < cobrancaParcela &&
                    Convert.ToInt32(rows[rows.Length - 1]["regracomitem_vitalicio"]) == 1)
                {
                    percentual = CTipos.ToDecimal(rows[rows.Length - 1]["regracomitem_percentual"]);
                }
            }

            return percentual;
        }

        protected void cmdProcurar_Click(object sender, EventArgs e)
        {
            this.montarRelatorio();
            if (gridCobrancas.Rows.Count == 0) { litAviso.Text = "Nenhum registro localizado"; }
        }

        protected void cmdExportar_Click(object sender, EventArgs e)
        {
            List<ItemRelatorio> relatorio = this.montarRelatorio();

            if (relatorio == null || relatorio.Count == 0) return;

            #region separa os dados 

            List<string> projetos = new List<string>();
            List<string> corretores = new List<string>();
            List<string> contratosAdm = new List<string>();

            foreach (var item in relatorio)
            {
                if (!corretores.Contains(item.Corretor)) corretores.Add(item.Corretor);
                if (!projetos.Contains(item.Estipulante)) projetos.Add(item.Estipulante);
                if (!contratosAdm.Contains(item.ContratoADM)) contratosAdm.Add(item.ContratoADM);
            }

            #endregion

            #region PDF

            //https://www.google.com.br/webhp?sourceid=chrome-instant&ion=1&espv=2&ie=UTF-8#q=asp.net+itextsharp+pdf
            //http://www.4guysfromrolla.com/articles/030911-1.aspx

            Font font = FontFactory.GetFont("Arial", 10, BaseColor.BLACK);
            Font titulo = FontFactory.GetFont("Arial", 14, BaseColor.BLACK);
            Font fontB = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);

            var document = new Document(PageSize.A4.Rotate(), 25, 25, 25, 40);

            var output = new MemoryStream();// FileStream(Server.MapPath("rel_comissao.pdf"), FileMode.Create);
            var writer = PdfWriter.GetInstance(document, output);

            PdfPageEventPageNo pageEventHndl = new PdfPageEventPageNo();
            writer.PageEvent = pageEventHndl;

            document.Open();

            //var p1 = new Paragraph("Relatório de comissionamento - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), titulo);
            //document.Add(p1);

            int totalVidas = 0;
            decimal totalPagto = 0, totalComissao = 0;

            bool primeiraPagina = false;
            foreach (string proj in projetos)
            {
                foreach (string contratoadm in contratosAdm)
                {
                    foreach (string corretor in corretores)
                    {
                        List<ItemRelatorio> res = relatorio
                            .Where(r => r.Estipulante == proj && r.ContratoADM == contratoadm && r.Corretor == corretor)
                            .ToList();

                        if (res.Count == 0) continue;

                        if (!primeiraPagina) primeiraPagina = true;
                        else
                        {
                            //insere nova página
                            document.NewPage();
                        }

                        var p1 = new Paragraph("PROJETO: " + proj, titulo);
                        document.Add(p1);
                        p1 = new Paragraph("CORRETOR: " + corretor, titulo);
                        document.Add(p1);
                        p1 = new Paragraph("CONTRATO ADM: " + contratoadm, titulo);
                        document.Add(p1);
                        p1 = new Paragraph(string.Concat("PERIODO: ", txtDe.Text, " a ", txtAte.Text), titulo);
                        document.Add(p1);

                        //monta o relatorio
                        var tabela = new PdfPTable(7);
                        tabela.WidthPercentage = 100;
                        tabela.HorizontalAlignment = 0;
                        tabela.SpacingBefore = 10;
                        tabela.SpacingAfter = 10;
                        tabela.DefaultCell.Border = 1;

                        int[] widths = new int[] { 225, 75, 75, 60, 55, 80, 80 };
                        tabela.SetWidths(widths);

                        tabela.SpacingBefore = 20f;

                        //tabela.AddCell(new Phrase("Corretor", font));
                        //tabela.AddCell(new Phrase("Projeto", font));
                        tabela.AddCell(new Phrase("Beneficiario", font));
                        tabela.AddCell(new Phrase("Parcela", font));
                        tabela.AddCell(new Phrase("Data Pagto", font));
                        tabela.AddCell(new Phrase("Premio", font));
                        tabela.AddCell(new Phrase("Vidas", font));
                        tabela.AddCell(new Phrase("Percentual", font));
                        tabela.AddCell(new Phrase("Valor", font));

                        totalVidas = 0; totalPagto = 0; totalComissao = 0;

                        foreach (var item in res)
                        {
                            totalPagto += item.CobrancaValor;
                            totalVidas += item.CobrancaQtdVidas;
                            totalComissao += item.Calculado;

                            //tabela.AddCell(new Phrase(item.Corretor, font));
                            //tabela.AddCell(new Phrase(item.Estipulante, font));
                            tabela.AddCell(new Phrase(item.Beneficiario, font));
                            tabela.AddCell(new Phrase(item.CobrancaParcela.ToString(), font));
                            tabela.AddCell(new Phrase(item.CobrancaDataPago.ToString("dd/MM/yyyy"), font));
                            tabela.AddCell(new Phrase(item.CobrancaValor.ToString("N2"), font));
                            tabela.AddCell(new Phrase(item.CobrancaQtdVidas.ToString(), font));
                            tabela.AddCell(new Phrase(item.PercentualComissao.ToString("N2"), font));
                            tabela.AddCell(new Phrase(item.Calculado.ToString("N2"), font));
                        }

                        //tabela.AddCell(new Phrase("TOTAIS", font));
                        //tabela.AddCell(new Phrase(" ", font));
                        tabela.AddCell(new Phrase("Total " + res.Count.ToString(), font));
                        tabela.AddCell(new Phrase(" ", font));
                        tabela.AddCell(new Phrase(" ", fontB));
                        tabela.AddCell(new Phrase(totalPagto.ToString("N2"), font));
                        tabela.AddCell(new Phrase(totalVidas.ToString(), font));
                        tabela.AddCell(new Phrase(" ", font));
                        tabela.AddCell(new Phrase(totalComissao.ToString("N2"), font));

                        document.Add(tabela);
                    }
                }
            }

            

            document.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment;filename=rel_comissao.pdf");
            Response.BinaryWrite(output.ToArray());

            #endregion
        }

        protected void cmdExportar_Click_BKP(object sender, EventArgs e)
        {
            var relatorio = this.montarRelatorio();

            if (relatorio == null || relatorio.Count == 0) return;

            #region PDF

            //https://www.google.com.br/webhp?sourceid=chrome-instant&ion=1&espv=2&ie=UTF-8#q=asp.net+itextsharp+pdf
            //http://www.4guysfromrolla.com/articles/030911-1.aspx

            Font font = FontFactory.GetFont("Arial", 10, BaseColor.BLACK);
            Font titulo = FontFactory.GetFont("Arial", 14, BaseColor.BLACK);
            Font fontB = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);

            var document = new Document(PageSize.A4.Rotate(), 25, 25, 25, 25);

            var output = new MemoryStream();// FileStream(Server.MapPath("rel_comissao.pdf"), FileMode.Create);
            var writer = PdfWriter.GetInstance(document, output);

            PdfPageEventPageNo pageEventHndl = new PdfPageEventPageNo();
            writer.PageEvent = pageEventHndl;

            document.Open();

            var p1 = new Paragraph("Relatório de comissionamento - " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), titulo);
            document.Add(p1);

            var tabela = new PdfPTable(9);
            //tabela.DefaultCell.Border = 1;
            //tabela.DefaultCell.BorderColor = BaseColor.BLACK;
            tabela.WidthPercentage = 100;
            //orderInfoTable.TotalWidth = ???
            tabela.HorizontalAlignment = 0;
            tabela.SpacingBefore = 10;
            tabela.SpacingAfter = 10;
            tabela.DefaultCell.Border = 1;
            //tabela.SetWidthPercentage(widths, PageSize.A4));
            int[] widths = new int[] { 225, 200, 225, 75, 75, 60, 55, 80, 80 };
            tabela.SetWidths(widths);

            tabela.SpacingBefore = 20f;

            tabela.AddCell(new Phrase("Corretor", font));
            tabela.AddCell(new Phrase("Projeto", font));
            tabela.AddCell(new Phrase("Beneficiario", font));
            tabela.AddCell(new Phrase("Parcela", font));
            tabela.AddCell(new Phrase("Pagto", font));
            tabela.AddCell(new Phrase("Valor", font));
            tabela.AddCell(new Phrase("Vidas", font));
            tabela.AddCell(new Phrase("Percentual", font));
            tabela.AddCell(new Phrase("Comissão", font));

            int totalVidas = 0;
            decimal totalPagto = 0, totalComissao = 0;

            foreach (var item in relatorio)
            {
                totalPagto += item.CobrancaValor;
                totalVidas += item.CobrancaQtdVidas;
                totalComissao += item.Calculado;

                tabela.AddCell(new Phrase(item.Corretor, font));
                tabela.AddCell(new Phrase(item.Estipulante, font));
                tabela.AddCell(new Phrase(item.Beneficiario, font));
                tabela.AddCell(new Phrase(item.CobrancaParcela.ToString(), font));
                tabela.AddCell(new Phrase(item.CobrancaDataPago.ToString("dd/MM/yyyy"), font));
                tabela.AddCell(new Phrase(item.CobrancaValor.ToString("N2"), font));
                tabela.AddCell(new Phrase(item.CobrancaQtdVidas.ToString(), font));
                tabela.AddCell(new Phrase(item.PercentualComissao.ToString("N2"), font));
                tabela.AddCell(new Phrase(item.Calculado.ToString("N2"), font));
            }

            tabela.AddCell(new Phrase("TOTAIS", font));
            tabela.AddCell(new Phrase(" ", font));
            tabela.AddCell(new Phrase(relatorio.Count.ToString(), font));
            tabela.AddCell(new Phrase(" ", font));
            tabela.AddCell(new Phrase(" ", fontB));
            tabela.AddCell(new Phrase(totalPagto.ToString("N2"), font));
            tabela.AddCell(new Phrase(totalVidas.ToString(), font));
            tabela.AddCell(new Phrase(" ", font));
            tabela.AddCell(new Phrase(totalComissao.ToString("N2"), font));

            document.Add(tabela);

            document.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "attachment;filename=rel_comissao.pdf");
            Response.BinaryWrite(output.ToArray());

            #endregion
        }

        protected void cmdExportarCSV_Click(object sender, EventArgs e)
        {
            var relatorio = this.montarRelatorio();

            if (relatorio == null || relatorio.Count == 0) return;

            #region CSV 

            var csvExp = new CsvExport();

            int totalVidas = 0;
            decimal totalPagto = 0, totalComissao = 0;

            foreach (ItemRelatorio item in relatorio)
            {
                totalPagto += item.CobrancaValor;
                totalVidas += item.CobrancaQtdVidas;
                totalComissao += item.Calculado;

                csvExp.AddRow();
                csvExp["Corretor"]      = item.Corretor;
                csvExp["Projeto"]       = item.Estipulante;
                csvExp["ContratoADM"]   = item.ContratoADM;
                csvExp["Beneficiario"]  = item.Beneficiario;
                csvExp["Cartao"]        = string.Concat("'", item.BeneficiarioCartao);
                csvExp["Parcela"]       = item.CobrancaParcela;
                csvExp["DataPagto"]     = item.CobrancaDataPago.ToString("dd/MM/yyyy");
                csvExp["ValorPagto"]    = item.CobrancaValor.ToString("N2");
                csvExp["Vidas"]         = item.CobrancaQtdVidas.ToString();
                csvExp["Percentual"]    = item.PercentualComissao.ToString("N2");
                csvExp["Valor"]         = item.Calculado.ToString("N2");
            }

            csvExp.AddRow();
            csvExp["Corretor"]      = "TOTAIS";
            csvExp["Projeto"]       = "------";
            csvExp["ContratoADM"]   = "------";
            csvExp["Beneficiario"]  = relatorio.Count.ToString();
            csvExp["Cartao"]        = "------";
            csvExp["Parcela"]       = "------";
            csvExp["DataPagto"]     = "------";
            csvExp["ValorPagto"]    = totalPagto.ToString("N2");
            csvExp["Vidas"]         = totalVidas.ToString();
            csvExp["Percentual"]    = "------";
            csvExp["Valor"]         = totalComissao.ToString("N2");

            string conteudo = csvExp.Export();
            string arquivo  = "rel_comissao.csv";

            Response.Clear();
            Response.ContentType = "application/CSV";
            Response.AddHeader("content-disposition", "attachment; filename=\"" + arquivo + "\"");
            Response.Write(conteudo);
            Response.End();
            #endregion
        }

        protected void gridCobrancas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "editar")
            {
            }
        }

        protected void gridCobrancas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Boolean rascunho = Convert.ToBoolean(gridCobrancas.DataKeys[e.Row.RowIndex][1]);
                //((Image)e.Row.Cells[4].Controls[1]).Visible = rascunho;

                //Boolean cancelado = Convert.ToBoolean(gridCobrancas.DataKeys[e.Row.RowIndex][2]);
                //Boolean inativado = Convert.ToBoolean(gridCobrancas.DataKeys[e.Row.RowIndex][3]);

                ////UIHelper.AuthWebCtrl((LinkButton)e.Row.Cells[5].Controls[0], new String[] { Perfil.CadastroIDKey, Perfil.ConferenciaIDKey, Perfil.OperadorIDKey, Perfil.OperadorLiberBoletoIDKey, Perfil.PropostaBeneficiarioIDKey });
                ////UIHelper.AuthCtrl((LinkButton)e.Row.Cells[7].Controls[0], new String[] { Perfil.AdministradorIDKey });
                ////UIHelper.AuthWebCtrl((LinkButton)e.Row.Cells[7].Controls[0], new String[] { Perfil.AdministradorIDKey });
                //grid_RowDataBound_Confirmacao(sender, e, 7, "Deseja realmente prosseguir com a exclusão?\\nEssa operação não poderá ser desfeita.");

                //if (Usuario.Autenticado.PerfilID != Perfil.AdministradorIDKey) { gridCobrancas.Columns[7].Visible = false; }

                //if (cancelado || inativado)
                //{
                //    e.Row.Cells[0].ForeColor = System.Drawing.Color.FromName("#CC0000");
                //    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='../../images/unactive.png' title='inativo' alt='inativo' border='0'>";
                //    //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente ativar o contrato?");
                //}
                //else
                //{
                //    //base.grid_RowDataBound_Confirmacao(sender, e, 4, "Deseja realmente cancelar o contrato?");
                //    ((LinkButton)e.Row.Cells[5].Controls[0]).Text = "<img src='../../images/active.png' title='ativo' alt='ativo' border='0'>";
                //}
            }
        }

        protected void gridCobrancas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridCobrancas.PageIndex = e.NewPageIndex;
            this.montarRelatorio();
        }

        protected void grid_RowDataBound_Confirmacao(Object sender, GridViewRowEventArgs e, int indiceControle, String Msg)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[indiceControle].Attributes.Add("onClick", "return confirm('" + Msg + "');");
            }
        }
    }

    [Serializable]
    class ItemRelatorio
    {
        public long ContratoId { get; set; }
        public long CobrancaId { get; set; }
        public int CobrancaQtdVidas { get; set; }
        public string Corretor { get; set; }
        public long CorretorId { get; set; }
        public string Estipulante { get; set; }
        public string ContratoADM { get; set; }
        public string Beneficiario { get; set; }
        public string BeneficiarioCartao { get; set; }
        public decimal CobrancaValor { get; set; }
        public decimal CobrancaValorPago { get; set; }
        public DateTime CobrancaDataPago { get; set; }
        public int CobrancaParcela { get; set; }
        public decimal PercentualComissao { get; set; }
        public decimal Calculado { get; set; }
        public int ParcelaComissao { get; set; }
    }

    public class PdfPageEventPageNo__ : iTextSharp.text.pdf.PdfPageEventHelper
    {
        protected PdfTemplate total;
        protected BaseFont helv;
        private bool settingFont = false;

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            total = writer.DirectContent.CreateTemplate(100, 100);
            total.BoundingBox = new Rectangle(-20, -20, 100, 100);

            helv = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            PdfContentByte cb = writer.DirectContent;
            cb.SaveState();
            string text = writer.PageNumber.ToString(); // +" de ";
            float textBase = document.Bottom - 20;
            float textSize = 12; //helv.GetWidthPoint(text, 12);
            cb.BeginText();
            cb.SetFontAndSize(helv, 12);
            if ((writer.PageNumber % 2) == 1)
            {
                cb.SetTextMatrix(document.Left, textBase);
                cb.ShowText(text);
                cb.EndText();
                cb.AddTemplate(total, document.Left + textSize, textBase);
            }
            else
            {
                float adjust = helv.GetWidthPoint("0", 12);
                cb.SetTextMatrix(document.Right - textSize - adjust, textBase);
                //cb.SetTextMatrix(50f, textBase);
                cb.ShowText(text);
                cb.EndText();
                cb.AddTemplate(total, document.Right - adjust, textBase);
            }
            cb.RestoreState();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            total.BeginText();
            total.SetFontAndSize(helv, 12);
            total.SetTextMatrix(0, 0);
            int pageNumber = writer.PageNumber - 1;
            total.ShowText(Convert.ToString(pageNumber));
            total.EndText();
        }
    }

    public class PdfPageEventPageNo : PdfPageEventHelper
    {
        PdfContentByte cb;
        PdfTemplate template;
        protected BaseFont helv;


        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            cb = writer.DirectContent;
            template = cb.CreateTemplate(150, 50);

            helv = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            int pageN = writer.PageNumber;
            //String text = "Page " + pageN.ToString() + " of ";
            String text = pageN.ToString();
            float len = 12f; // this.RunDateFont.BaseFont.GetWidthPoint(text, this.RunDateFont.Size);

            iTextSharp.text.Rectangle pageSize = document.PageSize;

            cb.SetRGBColorFill(100, 100, 100);

            cb.BeginText();
            cb.SetFontAndSize(helv, 12f);
            //cb.SetTextMatrix(document.LeftMargin, pageSize.GetBottom(document.BottomMargin));
            cb.SetTextMatrix(document.LeftMargin, document.BottomMargin - 10);
            cb.ShowText(text);

            cb.EndText();

            cb.AddTemplate(template, document.LeftMargin + len, pageSize.GetBottom(document.BottomMargin));
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            template.BeginText();
            template.SetFontAndSize(helv, 12f);
            template.SetTextMatrix(50, 0); //
            //template.ShowText("" + (writer.PageNumber - 1));
            template.EndText();
        }
    }
}