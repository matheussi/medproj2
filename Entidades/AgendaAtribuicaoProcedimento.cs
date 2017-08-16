namespace MedProj.Entidades
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Data;
    using System.Data.OleDb;
    using System.Configuration;
    using System.Collections.Generic;

    public class AgendaAtribuicaoProcedimento : EntidadeBase
    {
        public AgendaAtribuicaoProcedimento()
        {
            this.Ativa = true;
            this.Processado = false;
            this.DataCriacao = DateTime.Now;
            this.TabelaDePrecoViaPlanilha = false;
        }

        //public virtual long ID { get; set; }
        public virtual string Descricao { get; set; }
        public virtual DateTime DataCriacao { get; set; }

        /// <summary>
        /// Data em que deverá ocorrer o processamento
        /// </summary>
        public virtual DateTime DataProcessamento { get; set; }
        /// <summary>
        /// Data em que o processamento foi concluído
        /// </summary>
        public virtual DateTime? DataConclusao { get; set; }
        public virtual bool Processado { get; set; }
        public virtual string Arquivo { get; set; }

        public virtual TabelaPreco Tabela { get; set; }
        public virtual bool Ativa { get; set; }

        public virtual IList<PrestadorUnidade> Contratos { get; set; }
        public virtual IList<AgendaAtribProcedRESULTADO> Log { get; set; }

        public virtual string Erro { get; set; }

        /// <summary>
        /// Quando TRUE, a rotina de importação vai procurar pela coluna "tabela" na planilha de dados para obter a tabela de preço. Padrão: false
        /// </summary>
        public virtual bool TabelaDePrecoViaPlanilha { get; set; }

        public virtual DataTable ObterDados()
        {
            if (string.IsNullOrEmpty(this.Arquivo)) return null;

            string connExcel = "";

            string arquivoFonte = string.Concat(ConfigurationManager.AppSettings["appImportProcCaminhoFisico"], //ConfigurationManager.AppSettings["appUrl"], 
                this.ID, Path.GetExtension(this.Arquivo));

            if (arquivoFonte.ToUpper().IndexOf("XLSX") > -1)
                connExcel = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=", arquivoFonte, ";Extended Properties='Excel 8.0;HDR=Yes;'");
            else if (arquivoFonte.ToUpper().IndexOf("XLS") > -1)
                connExcel = string.Concat(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=", arquivoFonte, ";Extended Properties='Excel 8.0;HDR=Yes;'");
            else
                return null;

            DataTable dt = new DataTable();
            using (OleDbConnection connection = new OleDbConnection(connExcel))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand("select * from [PROCEDIMENTO$]", connection);
                OleDbDataAdapter adp = new OleDbDataAdapter(command);
                adp.Fill(dt);
            }

            return dt;
            #region comentado
        //    if (string.IsNullOrWhiteSpace(this.Arquivo)) return null;

        //    string nomeArquivo = string.Concat(this.ID, Path.GetExtension(this.Arquivo));

        //    string caminho = string.Concat(@ConfigurationManager.AppSettings["appImportCaminhoFisico"], nomeArquivo);

        //    if (!File.Exists(caminho)) return null;

        //    string connExcel = "";
        //    DataTable dt = new DataTable();
        //    bool excel = true;

        //    if (this.Arquivo.ToUpper().IndexOf("XLSX") > -1)
        //        connExcel = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=", caminho, ";Extended Properties='Excel 8.0;HDR=Yes;'");
        //    else if (this.Arquivo.ToUpper().IndexOf("XLS") > -1)
        //        connExcel = string.Concat(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=", caminho, ";Extended Properties='Excel 8.0;HDR=Yes;'");
        //    else
        //    {
        //        excel = false;
        //        string[] linhas = File.ReadAllLines(caminho, Encoding.GetEncoding("iso-8859-1"));

        //        if (linhas == null || linhas.Length <= 1) { dt.Dispose(); return null; }

        //        #region cria as colunas do DataTable

        //        dt.Columns.Add("CARTAO");
        //        dt.Columns.Add("PRODUTOR");
        //        dt.Columns.Add("MATRICULA");
        //        dt.Columns.Add("TIPO_PROPOSTA");
        //        dt.Columns.Add("DT_ADMISSAO");
        //        dt.Columns.Add("DT_VIGENCIA");
        //        dt.Columns.Add("CPF_TITULAR");
        //        dt.Columns.Add("NOME_BENEFICIARIO");
        //        dt.Columns.Add("DT_NASCIMENTO");
        //        dt.Columns.Add("RG");
        //        dt.Columns.Add("RG_ORGAO_EXP");
        //        dt.Columns.Add("RG_UF");
        //        dt.Columns.Add("SEXO");
        //        dt.Columns.Add("NOME_MAE");
        //        dt.Columns.Add("DDD1");
        //        dt.Columns.Add("FONE1");
        //        dt.Columns.Add("RAMAL1");
        //        dt.Columns.Add("DDD2");
        //        dt.Columns.Add("FONE2");
        //        dt.Columns.Add("RAMAL2");
        //        dt.Columns.Add("DDD_CEL");
        //        dt.Columns.Add("FONE_CEL");
        //        dt.Columns.Add("EMAIL");
        //        dt.Columns.Add("NOME_RESP_LEGAL");
        //        dt.Columns.Add("CPF_RESP_LEGAL");
        //        dt.Columns.Add("RG_RESP_LEGAL");
        //        dt.Columns.Add("DT_NASC_RESP_LEGAL");
        //        dt.Columns.Add("SEXO_RESP_LEGAL");
        //        dt.Columns.Add("PARENTESCO_RESP_LEGAL");
        //        dt.Columns.Add("CEP");
        //        dt.Columns.Add("LOGRADOURO");
        //        dt.Columns.Add("NUMERO");
        //        dt.Columns.Add("COMPLEMENTO");
        //        dt.Columns.Add("BAIRRO");
        //        dt.Columns.Add("CIDADE");
        //        dt.Columns.Add("UF");
        //        dt.Columns.Add("TIPO");

        //        #endregion

        //        string linha = "";
        //        string[] colunas = null;

        //        for (int i = 1; i < linhas.Length; i++)
        //        {
        //            linha = linhas[i];

        //            colunas = linha.Split(';');

        //            DataRow row = dt.NewRow();

        //            #region preenche linha do DataTable

        //            row["CARTAO"] = colunas[0];
        //            row["PRODUTOR"] = colunas[1];
        //            row["MATRICULA"] = colunas[2];
        //            row["TIPO_PROPOSTA"] = colunas[3];
        //            row["DT_ADMISSAO"] = colunas[4];
        //            row["DT_VIGENCIA"] = colunas[5];
        //            row["CPF_TITULAR"] = colunas[6];
        //            row["NOME_BENEFICIARIO"] = colunas[7];
        //            row["DT_NASCIMENTO"] = colunas[8];
        //            row["RG"] = colunas[9];
        //            row["RG_ORGAO_EXP"] = colunas[10];
        //            row["RG_UF"] = colunas[11];
        //            row["SEXO"] = colunas[12];
        //            row["NOME_MAE"] = colunas[13];
        //            row["DDD1"] = colunas[14];
        //            row["FONE1"] = colunas[15];
        //            row["RAMAL1"] = colunas[16];
        //            row["DDD2"] = colunas[17];
        //            row["FONE2"] = colunas[18];
        //            row["RAMAL2"] = colunas[19];
        //            row["DDD_CEL"] = colunas[20];
        //            row["FONE_CEL"] = colunas[21];
        //            row["EMAIL"] = colunas[22];
        //            row["NOME_RESP_LEGAL"] = colunas[23];
        //            row["CPF_RESP_LEGAL"] = colunas[24];
        //            row["RG_RESP_LEGAL"] = colunas[25];
        //            row["DT_NASC_RESP_LEGAL"] = colunas[26];
        //            row["SEXO_RESP_LEGAL"] = colunas[27];
        //            row["PARENTESCO_RESP_LEGAL"] = colunas[28];
        //            row["CEP"] = colunas[29];
        //            row["LOGRADOURO"] = colunas[30];
        //            row["NUMERO"] = colunas[31];
        //            row["COMPLEMENTO"] = colunas[32];
        //            row["BAIRRO"] = colunas[33];
        //            row["CIDADE"] = colunas[34];
        //            row["UF"] = colunas[35];
        //            row["TIPO"] = colunas[36];

        //            #endregion

        //            dt.Rows.Add(row);
        //        }
        //    }

        //    if (excel)
        //    {
        //        using (OleDbConnection connection = new OleDbConnection(connExcel))
        //        {
        //            connection.Open();
        //            OleDbCommand command = new OleDbCommand("select * from [CONTRATO$]", connection);
        //            OleDbDataAdapter adp = new OleDbDataAdapter(command);
        //            adp.Fill(dt);
        //        }
        //    }

            //    return dt;
        #endregion
        }
    }

    public class AgendaAtribProcedRESULTADO : EntidadeBase
    {
        public AgendaAtribProcedRESULTADO() { }

        public AgendaAtribProcedRESULTADO(AgendaAtribuicaoProcedimento agenda)
        {
            Agenda = agenda;
        }

        public virtual AgendaAtribuicaoProcedimento Agenda { get; set; }
        public virtual Procedimento Procedimento { get; set; }
        public virtual PrestadorUnidade ContratoDePrestador { get; set; }
        public virtual string Mensagem { get; set; }
        public virtual bool Ok { get; set; }
    }
}
