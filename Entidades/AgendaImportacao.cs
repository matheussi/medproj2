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

    public class AgendaImportacao : EntidadeBase
    {
        public AgendaImportacao()
        {
            this.Ativa = true;
            this.Processado = false;
            this.NaoCriticarCPF = false;
        }

        public virtual void InicializarInstancias()
        {
            Autor = new Usuario();

            Filial = new Filial();
            AssociadoPj = new AssociadoPJ();
            Operadora = new Operadora();
            Contrato = new ContratoADM();
            Plano = new Plano();
        }

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

        public virtual string Arquivo     { get; set; }

        public virtual Usuario Autor { get; set; }

        public virtual Filial Filial { get; set; }
        public virtual AssociadoPJ AssociadoPj { get; set; }
        public virtual Operadora Operadora { get; set; }
        public virtual ContratoADM Contrato { get; set; }
        public virtual Plano Plano { get; set; }

        public virtual bool Ativa { get; set; }

        public virtual string Erro { get; set; }

        public virtual bool NaoCriticarCPF { get; set; }

        public virtual DataTable ObterDados()
        {
            if(string.IsNullOrWhiteSpace(this.Arquivo)) return null;

            string nomeArquivo = string.Concat(this.ID, Path.GetExtension(this.Arquivo));

            string caminho = string.Concat(@ConfigurationManager.AppSettings["appImportCaminhoFisico"], nomeArquivo);

            if (!File.Exists(caminho)) return null;

            string connExcel = "";
            DataTable dt = new DataTable();
            bool excel = true;

            if (this.Arquivo.ToUpper().IndexOf("XLSX") > -1)
                connExcel = string.Concat(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=", caminho, ";Extended Properties='Excel 8.0;HDR=Yes;'");
            else if (this.Arquivo.ToUpper().IndexOf("XLS") > -1)
                connExcel = string.Concat(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=", caminho, ";Extended Properties='Excel 8.0;HDR=Yes;'");
            else
            {
                excel = false;
                string[] linhas = File.ReadAllLines(caminho, Encoding.GetEncoding("iso-8859-1"));

                if (linhas == null || linhas.Length <= 1) { dt.Dispose(); return null; }

                #region cria as colunas do DataTable 
                
                dt.Columns.Add("CARTAO");
                dt.Columns.Add("PRODUTOR");
                dt.Columns.Add("MATRICULA");
                dt.Columns.Add("RAMO");
                dt.Columns.Add("APOLICE");

                dt.Columns.Add("DT_ADMISSAO");
                dt.Columns.Add("DT_VIGENCIA");
                dt.Columns.Add("CPF_TITULAR");
                dt.Columns.Add("NOME_BENEFICIARIO");
                dt.Columns.Add("DT_NASCIMENTO");
                dt.Columns.Add("RG");
                dt.Columns.Add("RG_ORGAO_EXP");
                dt.Columns.Add("RG_UF");
                dt.Columns.Add("SEXO");
                dt.Columns.Add("NOME_MAE");
                dt.Columns.Add("DDD1");
                dt.Columns.Add("FONE1");
                dt.Columns.Add("RAMAL1");
                dt.Columns.Add("DDD2");
                dt.Columns.Add("FONE2");
                dt.Columns.Add("RAMAL2");
                dt.Columns.Add("DDD_CEL");
                dt.Columns.Add("FONE_CEL");
                dt.Columns.Add("EMAIL");
                dt.Columns.Add("NOME_RESP_LEGAL");
                dt.Columns.Add("CPF_RESP_LEGAL");
                dt.Columns.Add("RG_RESP_LEGAL");
                dt.Columns.Add("DT_NASC_RESP_LEGAL");
                dt.Columns.Add("SEXO_RESP_LEGAL");
                dt.Columns.Add("PARENTESCO_RESP_LEGAL");
                dt.Columns.Add("CEP");
                dt.Columns.Add("LOGRADOURO");
                dt.Columns.Add("NUMERO");
                dt.Columns.Add("COMPLEMENTO");
                dt.Columns.Add("BAIRRO");
                dt.Columns.Add("CIDADE");
                dt.Columns.Add("UF");
                dt.Columns.Add("TIPO");

                #endregion

                string linha = "";
                string[] colunas = null;

                for (int i = 1; i < linhas.Length; i++)
                {
                    linha = linhas[i];

                    colunas = linha.Split(';');

                    DataRow row = dt.NewRow();

                    #region preenche linha do DataTable 

                    row["CARTAO"]                   = colunas[0];
                    row["PRODUTOR"]                 = colunas[1];
                    row["MATRICULA"]                = colunas[2];
                    row["RAMO"]                     = colunas[3];
                    row["APOLICE"]                  = colunas[4];
                    row["DT_ADMISSAO"]              = colunas[5];
                    row["DT_VIGENCIA"]              = colunas[6];
                    row["CPF_TITULAR"]              = colunas[7];
                    row["NOME_BENEFICIARIO"]        = colunas[8];
                    row["DT_NASCIMENTO"]            = colunas[9];
                    row["RG"]                       = colunas[10];
                    row["RG_ORGAO_EXP"]             = colunas[11];
                    row["RG_UF"]                    = colunas[12];
                    row["SEXO"]                     = colunas[13];
                    row["NOME_MAE"]                 = colunas[14];
                    row["DDD1"]                     = colunas[15];
                    row["FONE1"]                    = colunas[16];
                    row["RAMAL1"]                   = colunas[17];
                    row["DDD2"]                     = colunas[18];
                    row["FONE2"]                    = colunas[19];
                    row["RAMAL2"]                   = colunas[20];
                    row["DDD_CEL"]                  = colunas[21];
                    row["FONE_CEL"]                 = colunas[22];
                    row["EMAIL"]                    = colunas[23];
                    row["NOME_RESP_LEGAL"]          = colunas[24];
                    row["CPF_RESP_LEGAL"]           = colunas[25];
                    row["RG_RESP_LEGAL"]            = colunas[26];
                    row["DT_NASC_RESP_LEGAL"]       = colunas[27];
                    row["SEXO_RESP_LEGAL"]          = colunas[28];
                    row["PARENTESCO_RESP_LEGAL"]    = colunas[29];
                    row["CEP"]                      = colunas[30];
                    row["LOGRADOURO"]               = colunas[31];
                    row["NUMERO"]                   = colunas[32];
                    row["COMPLEMENTO"]              = colunas[33];
                    row["BAIRRO"]                   = colunas[34];
                    row["CIDADE"]                   = colunas[35];
                    row["UF"]                       = colunas[36];
                    row["TIPO"]                     = colunas[37];

                    #endregion

                    dt.Rows.Add(row);
                }
            }

            if (excel)
            {
                using (OleDbConnection connection = new OleDbConnection(connExcel))
                {
                    connection.Open();
                    OleDbCommand command = new OleDbCommand("select * from [CONTRATO$]", connection);
                    OleDbDataAdapter adp = new OleDbDataAdapter(command);
                    adp.Fill(dt);
                }
            }

            return dt;
        }
    }
}