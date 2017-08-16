using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;

using LC.Framework.Phantom;
using LC.Framework.BusinessLayer;

namespace LC.Web.PadraoSeguros.Entity
{
    /// <summary>
    /// Representa um Arquivo Transacional da Unimed.
    /// </summary>
    public class ArqTransacionalUnimed
    {
        #region Private Fields

        /// <summary>
        /// Seção U01 - IDENTIFICADOR DE EMPRESA.
        /// </summary>
        private const String U01 = "U01";

        /// <summary>
        /// Seção U02 - IDENTIFICADOR DE BENEFICIÁRIO.
        /// </summary>
        private const String U02 = "U02";

        /// <summary>
        /// Seção U03 - PLANOS DO BENEFICIÁRIO.
        /// </summary>
        private const String U03 = "U03";

        /// <summary>
        /// Seção U05 - COMPLEMENTO DE BENEFICIÁRIO.
        /// </summary>
        private const String U05 = "U05";

        /// <summary>
        /// Seção U07 - DECLARAÇÃO DE SAÚDE.
        /// </summary>
        private const String U07 = "U07";

        /// <summary>
        /// Seção U08 - CÓDIGO INTERNACIONAL DE DOENÇAS (C.I.D).
        /// </summary>
        private const String U08 = "U08";

        /// <summary>
        /// Seção U99 - TRAILLER DE ARQUIVO.
        /// </summary>
        private const String U99 = "U09";

        /// <summary>
        /// Caminho relativo do repositório de arquivos transacionais.
        /// </summary>
        private readonly String ArqTransacionalRelativePath = ConfigurationManager.AppSettings["transactFilePath"];

        /// <summary>
        /// Caminho físico e absoluto da raiz aplicação.
        /// </summary>
        private readonly String ArqTransacionalRootPath = String.Empty;

        #endregion

        #region Private Members

        /// <summary>
        /// Path para os Arquivos Transacionais.
        /// </summary>
        private String ArqTransacionalFilePath
        {
            get
            {
                return String.Concat(this.ArqTransacionalRootPath, ArqTransacionalRelativePath.Replace("/", "\\"));
            }
        }

        #endregion

        #region Private Methods

        #region SalvarArqTransacional

        /// <summary>
        /// Salva o Arquivo Transacional no disco.
        /// </summary>
        /// <param name="SB">Buffer com o texto do Arquivo.</param>
        /// <param name="FileName">Nome do Arquivo.</param>
        /// <returns>True se conseguiu salvar e False se deu algum problema</returns>
        private Boolean SalvarArqTransacional(StringBuilder SB, String FileName)
        {
            if (SB != null && SB.Length > 0 && !String.IsNullOrEmpty(FileName))
            {
                if (!Directory.Exists(this.ArqTransacionalFilePath))
                {
                    try
                    {
                        Directory.CreateDirectory(this.ArqTransacionalFilePath);
                    }
                    catch (Exception) { throw; }
                }

                try
                {
                    String conteudo = SB.ToString();
                    conteudo = conteudo.Replace("º", "o");
                    conteudo = conteudo.Replace("ª", "a");
                    File.WriteAllText(String.Concat(this.ArqTransacionalFilePath, FileName), conteudo);//, System.Text.Encoding.GetEncoding("iso-8859-1")
                }
                catch (Exception) { throw; }

                return true;
            }

            return false;
        }

        #endregion

        #region GerarArquivo

        public Boolean GerarArquivoUNIMED_Exclusao_temp(ref String ArquivoNome)
        {
            Boolean criacaoOK = false;

            Object OperadoraID = Operadora.UnimedID;

            ArqTransacionalUnimedConf arqTransUnimed = new ArqTransacionalUnimedConf();
            arqTransUnimed.CarregarPorOperadora(OperadoraID);

            #region traduz tipo movimentacao

            String TipoMov = "";
            String cbTipo0Cond = "";
            String Mov = Movimentacao.CancelamentoContrato;
            //switch (Mov)
            //{
            //    case Movimentacao.AlteracaoBeneficiario:
            //    case Movimentacao.MudancaDePlano:
            //        {
            //            TipoMov = TipoMovimentacao.Alteracao;
            //            break;
            //        }
            //    case Movimentacao.ExclusaoBeneficiario:
            //        {
            //            TipoMov = TipoMovimentacao.Exclusao;
            //            break;
            //        }
            //    case Movimentacao.CancelamentoContrato:
            //        {
                        TipoMov = TipoMovimentacao.Exclusao;
                        cbTipo0Cond = " and cben.contratobeneficiario_tipo=0 ";
            //            break;
            //        }
            //    case Movimentacao.InclusaoBeneficiario:
            //        {
            //            TipoMov = TipoMovimentacao.Inclusao;
            //            break;
            //        }
            //    case Movimentacao.SegundaViaCartaoBeneficiario:
            //        {
            //            TipoMov = TipoMovimentacao.EmissaoSegundaVia;
            //            break;
            //        }
            //}
            #endregion

            if (arqTransUnimed.ID != null)
            {
                Int32 intArqNumeroSequencia = 1;
                Int32 intQtdeBeneficiarioInclusao = 0;
                Int32 intQtdeBeneficiarioExclusao = 0;
                Int32 intQtdeBeneficiarioAlteracao = 0;
                Int32 intQtdeRegistroU02 = 0;
                Int32 intQtdeRegistroU03 = 0;
                Int32 intQtdeRegistroU07U08 = 0;

                ArqTransacionalLote lote = new ArqTransacionalLote();
                StringBuilder sbFileBuffer = new StringBuilder();

                //StringBuilder condition = new StringBuilder();
                //foreach (ItemAgendaArquivoUnimed item in itens)
                //{
                //    if (condition.Length > 0) { condition.Append(" OR "); }
                //    condition.Append("(contratobeneficiario_ativo=1 AND contratobeneficiario_beneficiarioId="); condition.Append(item.BeneficiarioID);
                //    condition.Append(" AND contratobeneficiario_contratoId="); condition.Append(item.PropostaID);
                //    condition.Append(")");
                //}
                //condition.Append("contrato_numero in () and contrato_operadoraId=3");

                PersistenceManager PM = new PersistenceManager();
                PM = new PersistenceManager();
                PM.BeginTransactionContext();

                try
                {
                    using (DataTable dtBeneficiarios = LocatorHelper.Instance.ExecuteQuery("select contrato_admissao, contrato_numeroMatricula, contratobeneficiario_carenciaCodigo, contrato_vigencia, contratoadm_numero, contratoadm_id, c.contrato_numero, cBen.contratobeneficiario_id, cBen.contratobeneficiario_vigencia, cBen.contratobeneficiario_contratoId, cBen.contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo, contratobeneficiario_numeroSequencia, cBen.contratobeneficiario_portabilidade, cBen.contratobeneficiario_data, cBen.contratobeneficiario_estadoCivilId, estadocivil_codigo, beneficiario_nome, beneficiario_sexo,beneficiario_cpf, beneficiario_rg, beneficiario_rgUF, beneficiario_rgOrgaoExp, beneficiario_declaracaoNascimentoVivo, beneficiario_cns, cBen.contratobeneficiario_dataCasamento, beneficiario_nomeMae, beneficiario_dataNascimento,beneficiario_telefone, beneficiario_ramal, beneficiario_telefone2, beneficiario_celular, beneficiario_email, endereco_logradouro, endereco_numero, endereco_complemento,endereco_bairro, endereco_cidade, endereco_uf, endereco_cep FROM contrato_beneficiario cBen INNER JOIN contrato c ON cBen.contratobeneficiario_contratoId = c.contrato_id and cBen.contratobeneficiario_tipo=0 INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id left JOIN endereco ende ON c.contrato_enderecoReferenciaId = ende.endereco_id inner JOIN contratoADM on contratoadm_id=contrato_contratoAdmId inner JOIN estado_civil ON estadocivil_id=cBen.contratobeneficiario_estadocivilId LEFT JOIN contratoAdm_parentesco_agregado ON contratoAdmparentescoagregado_id=cBen.contratobeneficiario_parentescoId WHERE contrato_numero in ('2091166','2097159','2101067','2105199','2115360','2077547','2078429','2081983','2108104','21116491','2072990','2126853','2051302','2132093','2089400','2119020','2095584','2127269','2125382','2128837','2079094','2084108','2130055','2086089','2082664','2131427','2092733','2090573','2117470','2112283','2118293','2120377','2124463','2119238','2102798','2122786','2135362','2083311','2117088','2076993','2131727','2040681','2132094','2092976','2116903','2116781','2097214','2106310','2123566','2128580','2077304','2108129','2111510','2074690','2075861','2076718','2079176','2070514','2111135','2039023','2093546','2115100','2100995','2101218','2100873','2122836','2125734','2081253','2110137','2114650','2130270','2136686','2129428','2067978','2122700','2061274','2061177','2087678','2092775','2092166','2093331','2095143','2084319','2120842','2122991','2123972','2108647','2129551','2082720','2110118','2111605','2130063','2092481','2084831','2113461','2096463','2114630','2107222','2125229','2108796','2109165','2114867','2108965','2110022','2129871','2057580','2055005','2057172','2045985','2083467','2115683','2103610','2101770','2128553','2075752','2083174','2088200','2111704','2112706','2114125','2135766','2074056','2075631U','2127031','2132480','2153096','2123343','2141965','2125624','2155886','2168396','2171744','2138647','2170311','2040368','2050747','2071545','2073335','2035835','2059006','2055223','2048397','2040056','2042106','2039738','2090508','2115757','2087619','2091260','2089805','2108331','2082027','2078975','2081044','2108670','2114907','2113426','2090305','2116201','2116148','2105707','2138696','2140287','2160138','2145803','2162411','2167141','2171529','2146296','2163169','2057742','2071549','2069836','2051476','2048467','2072696','2072600','2101313','2040202','2050491','2060202','2061715','2061127','2065809','2084830','2092613','2087633','2094652','2084285','2089860','2095757','2117872','2106097','2081557','2085395','2084317','2086027','2110374','2109403','2134462','2069478','2072783','2073459','2061200','2075587','2075545','2101263','2134875','2141238','2164542','2161232','2130726','2128316','2140523','2063930','2057412','2061038','2095223','2126829','2085229','2106899','2128102','2129538','2113715','2117579','2119870','2073459','2069478','2072783','2127253','2140766','2167219','2167244','2151459','2152751','2174906','2160256','2154941','2065804','2052142','2037850','2036928','2047525','2086846','2089425','2094207','2124300','2120658','2102743','2128554','2075345','2077508','2136006','2122998','2131230','2131345','2135663','2094174','2072544','2078514','2075031','2102147','2125687','2082983','2110335','2132474','2107358','2137257','2137611','2140616','2148117''','2143427','2059448','2058103','2051113','2048264','2048409','2064962','2084429','2117719','2108211','2125241','2080961','2082734','2082090','2107038','2106489','2123280','2131180','2130400','2094175','2072773','2071276','2070083','2073130','2076435','2126779','2177980','2136579','2131596','2151620','2139685','2156726','2156728','2154862','2173713','2140857','2155723','2150990','216943', '2067675','2035744','2035354','2040589','2052995','2055890','2043768','2063943','2063290','2084691','2090436','2083229','2091081','2089305','2123732','2102441','2127634','2128028','2081110','2110278','2108170','2133846','2137256','2071509','2073130','2072773','2074339','2075015','2077792','2076985','2081228','2096405','2128365','2166189','2178004','2178151','2141976','2130029','2164334','2105274','2145946','2174181','2069238','2060989','2066718','2040600','2054708','2055793','2043809','2132425','2094206','2090006','2090803','2096741','2108724','2122769','2125178','2130151','2081549','2105687','2129481','2136280','2116083','2072477','2073814','2077677','2085935','2148425','2168808','2144086','2164417','2066585','2044192','2060726','2063064','2091750','2088897','2084634','2092769','2087632','2112852','2101624','2090701','2114156','2126385','2121391','2127840','2081224','2081989','2078307','2130291','2090253','2070083','2072544','2071509','2078092','2075146U','2077149','2096405','2107362','2128939','2111549','2111442','2146038','2146260','2161139','2169781','2171108','2167374','2154818','2142108','2171018','2160341','2153409','2144150','2148134','2137705','2140186','2157212','2160287','2165496','2156340','2160271','2135745','2159613','2165620','2177455','2153453','2167831','2154562','2171934','2173030','2139595','2151262','2174013','2161980','2157746','2145863','2168726','2163337','2170697','2171275','2144862','2164648','2151937','2151131','2128314','2154240','2154792','2144790','2141706','2165093','2163312','2175599','2144225','2162814','2164150','2155833','2175542','2136299','2157130','2169474','2139005','2141243','2163999','2150502','2147681','2154981','2137828','2164024','2168943','2165767') and contrato_operadoraId=3 and contrato_contratoadmid=340", "resultset").Tables[0])
                    {
                        if (dtBeneficiarios != null && dtBeneficiarios.Rows != null && dtBeneficiarios.Rows.Count > 0)
                        {
                            #region variaveis

                            ArqTransacionalLoteItem itemLote = null;
                            Contrato contrato = new Contrato();
                            Plano plano = new Plano();
                            Object contratoID, beneficiarioID, beneficiarioEstadoCivilID;
                            String beneficiarioParentescoCod, strBeneficiarioNome, strBeneficiarioSexo, strBeneficiarioCPF, strBeneficiarioTitularCPF, strCodigoCarencia,
                                   strBeneficiarioRG, strBeneficiarioEndereco, strBeneficiarioEnderecoNum, strBeneficiarioEnderecoCompl,
                                   strBeneficiarioBairro, strBeneficiarioCEP, strBeneficiarioCidade, strBeneficiarioUF, strBeneficiarioTelefone,
                                   strBeneficiarioTelefoneRamal, strBeneficiarioNomeMae, strBneficiarioPisPasep, strCodigoPlano, strTipoMovimentacaoAux;
                            Int16 intBeneficiarioSequencia, intBeneficiarioTipo;
                            DateTime dtBeneficiarioDataNascimento, dtBeneficiarioDataCasamento, dtBeneficiarioVigencia, dtBeneficiarioCadastro, vigenciaProposta;

                            lote.OperadoraID = OperadoraID;
                            lote.Movimentacao = Mov;
                            lote.TipoMovimentacao = TipoMov;

                            #endregion

                            #region Seção U01

                            if (dtBeneficiarios.Rows.Count > 0)
                            {
                                arqTransUnimed.OperadoraNaUnimed = Convert.ToString(dtBeneficiarios.Rows[0]["contratoadm_numero"]).Trim().PadLeft(8, '0');
                            }

                            //NR_SEQ
                            sbFileBuffer.Append(intArqNumeroSequencia.ToString().PadLeft(6, '0'));
                            //TP_REG
                            sbFileBuffer.Append(U01);
                            //CD_UNI
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCodSingular))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCodSingular, 4);
                            //CD_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                            //NOME_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNome))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNome, 40);
                            //CNPJ_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCNPJ))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCNPJ, 14);
                            //TP_IDENTIFICA
                            if (!String.IsNullOrEmpty(arqTransUnimed.TipoIdentificacao.ToString()))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.TipoIdentificacao, 1);
                            //VERSAO
                            if (!String.IsNullOrEmpty("4.0")) //arqTransUnimed.ArqVersao
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.ArqVersao, 3);
                            //DT_GERACAO
                            sbFileBuffer.Append(DateTime.Now.ToString("ddMMyyyy"));
                            //CEI_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCEI))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCEI, 13);

                            sbFileBuffer.AppendLine();

                            #endregion

                            foreach (DataRow row in dtBeneficiarios.Rows)
                            {
                                //switch (Mov)
                                //{
                                //    case Movimentacao.InclusaoBeneficiario:
                                //        intQtdeBeneficiarioInclusao++;
                                //        break;
                                //    case Movimentacao.ExclusaoBeneficiario:
                                        intQtdeBeneficiarioExclusao++;
                                //        break;
                                //    case Movimentacao.AlteracaoBeneficiario:
                                //        intQtdeBeneficiarioAlteracao++;
                                //        break;
                                //    case Movimentacao.MudancaDePlano:
                                //        intQtdeBeneficiarioAlteracao++;
                                //        break;
                                //}

                                contratoID = row["contratobeneficiario_contratoId"];
                                contrato.ID = contratoID;

                                if (contrato.ID != null)
                                {
                                    contrato.Carregar();
                                    if (contrato.PlanoID != null)
                                    {
                                        plano.ID = contrato.PlanoID;
                                        plano.Carregar();
                                    }
                                }

                                #region preenche variáveis

                                beneficiarioID = row["contratobeneficiario_beneficiarioId"];
                                beneficiarioParentescoCod = (row["contratoAdmparentescoagregado_parentescoCodigo"] == null || row["contratoAdmparentescoagregado_parentescoCodigo"] is DBNull) ? "00" : Convert.ToString(row["contratoAdmparentescoagregado_parentescoCodigo"]);
                                beneficiarioEstadoCivilID = (row["contratobeneficiario_estadoCivilId"] == null || row["contratobeneficiario_estadoCivilId"] is DBNull) ? "0" : Convert.ToString(row["estadocivil_codigo"]);
                                strBeneficiarioNome = (row["beneficiario_nome"] == null || row["beneficiario_nome"] is DBNull) ? null : Convert.ToString(row["beneficiario_nome"]);
                                strBeneficiarioSexo = (row["beneficiario_sexo"] == null || row["beneficiario_sexo"] is DBNull) ? null : Convert.ToString(row["beneficiario_sexo"]);
                                strBeneficiarioCPF = (row["beneficiario_cpf"] == null || row["beneficiario_cpf"] is DBNull) ? "" : Convert.ToString(row["beneficiario_cpf"]);
                                strBeneficiarioCPF = strBeneficiarioCPF.Replace("99999999999", "00000000000");

                                strBeneficiarioTitularCPF = ContratoBeneficiario.GetCPFTitular(contratoID, PM);
                                strBeneficiarioRG = (row["beneficiario_rg"] == null || row["beneficiario_rg"] is DBNull) ? String.Empty : Convert.ToString(row["beneficiario_rg"]);
                                intBeneficiarioTipo = (row["contratobeneficiario_tipo"] == null || row["contratobeneficiario_tipo"] is DBNull) ? Convert.ToInt16(-1) : Convert.ToInt16(row["contratobeneficiario_tipo"]);
                                intBeneficiarioSequencia = (row["contratobeneficiario_numeroSequencia"] == null || row["contratobeneficiario_numeroSequencia"] is DBNull) ? Convert.ToInt16(0) : Convert.ToInt16(row["contratobeneficiario_numeroSequencia"]);
                                dtBeneficiarioDataNascimento = (row["beneficiario_dataNascimento"] == null || row["beneficiario_dataNascimento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["beneficiario_dataNascimento"]);
                                dtBeneficiarioVigencia = (row["contratobeneficiario_vigencia"] == null || row["contratobeneficiario_vigencia"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_vigencia"]);
                                dtBeneficiarioDataCasamento = (row["contratobeneficiario_dataCasamento"] == null || row["contratobeneficiario_dataCasamento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_dataCasamento"]);
                                dtBeneficiarioCadastro = (row["contratobeneficiario_data"] == null || row["contratobeneficiario_data"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_data"]);
                                strBeneficiarioEndereco = (row["endereco_logradouro"] == null || row["endereco_logradouro"] is DBNull) ? null : Convert.ToString(row["endereco_logradouro"]);
                                strBeneficiarioEnderecoNum = (row["endereco_numero"] == null || row["endereco_numero"] is DBNull) ? null : Convert.ToString(row["endereco_numero"]);
                                strBeneficiarioEnderecoCompl = (row["endereco_complemento"] == null || row["endereco_complemento"] is DBNull) ? null : Convert.ToString(row["endereco_complemento"]);
                                strBeneficiarioBairro = (row["endereco_bairro"] == null || row["endereco_bairro"] is DBNull) ? null : Convert.ToString(row["endereco_bairro"]);
                                strBeneficiarioCEP = (row["endereco_cep"] == null || row["endereco_cep"] is DBNull) ? null : Convert.ToString(row["endereco_cep"]);
                                strBeneficiarioCidade = (row["endereco_cidade"] == null || row["endereco_cidade"] is DBNull) ? null : Convert.ToString(row["endereco_cidade"]);
                                strBeneficiarioUF = (row["endereco_uf"] == null || row["endereco_uf"] is DBNull) ? null : Convert.ToString(row["endereco_uf"]);
                                strBeneficiarioTelefone = (row["beneficiario_telefone"] == null || row["beneficiario_telefone"] is DBNull) ? null : Convert.ToString(row["beneficiario_telefone"]);
                                strBeneficiarioTelefoneRamal = (row["beneficiario_ramal"] == null || row["beneficiario_ramal"] is DBNull) ? null : Convert.ToString(row["beneficiario_ramal"]);
                                strBeneficiarioNomeMae = (row["beneficiario_nomeMae"] == null || row["beneficiario_nomeMae"] is DBNull) ? null : Convert.ToString(row["beneficiario_nomeMae"]);
                                strCodigoCarencia = (row["contratobeneficiario_carenciaCodigo"] == null || row["contratobeneficiario_carenciaCodigo"] is DBNull) ? "" : Convert.ToString(row["contratobeneficiario_carenciaCodigo"]).PadLeft(2, '0');
                                strBneficiarioPisPasep = String.Empty;
                                strCodigoPlano = String.Empty;
                                arqTransUnimed.OperadoraNaUnimed = Convert.ToString(row["contratoadm_numero"]).Trim().PadLeft(8, '0');
                                #endregion

                                if (strBeneficiarioTelefone != null)
                                {
                                    strBeneficiarioTelefone = strBeneficiarioTelefone.Replace("-", "");
                                    if (strBeneficiarioTelefone.Length > 8)
                                    {
                                        strBeneficiarioTelefone = strBeneficiarioTelefone.Substring(4).Trim();
                                    }
                                }

                                if (contrato.TipoAcomodacao > -1)
                                    if (contrato.TipoAcomodacao == 0)
                                        strCodigoPlano = (String.IsNullOrEmpty(plano.Codigo)) ? String.Empty : plano.Codigo;
                                    else
                                        strCodigoPlano = (String.IsNullOrEmpty(plano.CodigoParticular)) ? String.Empty : plano.CodigoParticular;

                                // CASO SEJA UMA MUDANÇA DE PLANO, SINALIZA A SEÇÃO PARA UMA ALTERAÇÃO
                                if (Mov.Equals(Movimentacao.MudancaDePlano) && TipoMov.Equals(TipoMovimentacao.Exclusao))
                                    strTipoMovimentacaoAux = TipoMovimentacao.Alteracao;
                                else
                                    strTipoMovimentacaoAux = TipoMov;

                                #region Seção U02

                                intQtdeRegistroU02++;

                                //NR_SEQ
                                sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                //TP_REG
                                sbFileBuffer.Append(U02);
                                //TP_MOV
                                EntityBase.AppendPreparedField(ref sbFileBuffer, strTipoMovimentacaoAux, 1);
                                //CD_EMP
                                if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                //CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF, 11);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                //CPF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (!String.IsNullOrEmpty(strBeneficiarioCPF))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCPF, 11);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                //RG_TRAB : TODO ? Tratar a exceção de alguma maneira
                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                //CD_USU : TODO ? Confirmar se é 0 para mandar, está igual arq exemplo
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                //SQ_USU
                                EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                //DV_USU
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                //NOME_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (!String.IsNullOrEmpty(strBeneficiarioNome))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNome, 70);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
                                //TP_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (intBeneficiarioTipo > 0)
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioTipo.ToString(), 1);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                //CD_SEXO
                                if (!String.IsNullOrEmpty(strBeneficiarioSexo))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, (strBeneficiarioSexo.Equals("1") ? "M" : "F"), 1);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 1);
                                //CD_PARENT
                                EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioParentescoCod.PadLeft(2, '0'), 2);
                                //DT_NASC
                                EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataNascimento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataNascimento.ToString("ddMMyyyy")), 8);
                                //CD_EST_CIVIL
                                if (beneficiarioEstadoCivilID != null)
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioEstadoCivilID.ToString(), 1);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                //DT_CASAMENTO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataCasamento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataCasamento.ToString("ddMMyyyy")), 8);
                                //IDENTIDADE
                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioRG, 20);
                                //EMISSOR
                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 10);

                                ////DECL_NASC_VIVO
                                //if (row["beneficiario_declaracaoNascimentoVivo"] == null || row["beneficiario_declaracaoNascimentoVivo"] == DBNull.Value || Convert.ToString(row["beneficiario_declaracaoNascimentoVivo"]).Trim() == "")
                                //    sbFileBuffer.Append("0".PadLeft(11, '0'));
                                //else
                                //    sbFileBuffer.Append(Convert.ToString(row["beneficiario_declaracaoNascimentoVivo"]).PadLeft(11, '0'));
                                ////CNS
                                //if (row["beneficiario_cns"] == null || row["beneficiario_cns"] == DBNull.Value || Convert.ToString(row["beneficiario_cns"]).Trim() == "")
                                //    sbFileBuffer.Append("0".PadLeft(15, '0'));
                                //else
                                //    sbFileBuffer.Append(Convert.ToString(row["beneficiario_cns"]).PadLeft(15, '0'));

                                //CD_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                //NOME_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                //CD_SUB_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                //CD_CARGO : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                //DT_ADMISSAO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                //DATA EFETIVACAO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                //DT_CADASTRO (VIGENCIA) : TODO ? Tratar exceção em caso de FALSE. (OBRIGATÓRIO)
                                if (dtBeneficiarioCadastro.CompareTo(DateTime.MinValue) > 0)
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                //DT_EXCLUSAO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                ////PORTABILIDADE
                                //sbFileBuffer.Append("N");

                                sbFileBuffer.AppendLine();

                                #endregion

                                if (Mov.Equals(Movimentacao.InclusaoBeneficiario) ||
                                    Mov.Equals(Movimentacao.AlteracaoBeneficiario))
                                {
                                    #region Seção U05

                                    //NR_SEQ
                                    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                    //TP_REG
                                    sbFileBuffer.Append(U05);
                                    //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                    //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
                                    //SQ_USU
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                    //ENDERECO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioEndereco))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEndereco, 45);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 45);

                                    //NR_ENDER : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioEnderecoNum))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEnderecoNum.Trim().PadLeft(5, '0'), 5);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);

                                    //COMPL_ENDER : TODO ? Limitar a 15 o complemento da UNIMED no cadastro de endereço
                                    if (!String.IsNullOrEmpty(strBeneficiarioEnderecoCompl))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEnderecoCompl, 15);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 15);
                                    //BAIRRO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioBairro))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioBairro, 20);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                    //CEP : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioCEP))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCEP, 8);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                    //CIDADE : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioCidade))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCidade, 30);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 30);
                                    //UF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioUF))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioUF, 2);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 2);
                                    //TELEFONE
                                    if (!String.IsNullOrEmpty(strBeneficiarioTelefone))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTelefone, 9);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 9);
                                    //RAMAL
                                    if (!String.IsNullOrEmpty(strBeneficiarioTelefoneRamal))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTelefoneRamal, 6);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 6);
                                    //NOME_MAE : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioNomeMae))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNomeMae, 70);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
                                    //PIS_PASEP : TODO ? Não tem no Cadastro, ele não é enviado nem com caracter em branco mesmo estando com espaço em branco
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBneficiarioPisPasep, 11);
                                    ////CIDADE_RESIDENCIA
                                    //EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCidade, 30);

                                    sbFileBuffer.AppendLine();

                                    #endregion
                                }

                                if (Mov.Equals(Movimentacao.InclusaoBeneficiario) || Mov.Equals(Movimentacao.MudancaDePlano))
                                {
                                    #region Seção U03

                                    Boolean bolHasSubPlano = false;
                                    Boolean bolRepeatOnce = false;

                                    #region Exclusão de Plano Antigo

                                    if (Mov.Equals(Movimentacao.MudancaDePlano)) //&& TipoMov.Equals(TipoMovimentacao.Exclusao))
                                    {
                                        ContratoPlano ContratoPlanoAntigo = ContratoPlano.CarregarPenultimo(contrato.ID, PM);

                                        if (ContratoPlanoAntigo != null)
                                        {
                                            // Carregar o plano antigo, código do plano antigo, acomodação, etc.
                                            if (ContratoPlanoAntigo != null && ContratoPlanoAntigo.ID != null)
                                            {
                                                Plano planoAntigo = new Plano(ContratoPlanoAntigo.PlanoID);
                                                planoAntigo.Carregar();

                                                bolHasSubPlano = !String.IsNullOrEmpty(planoAntigo.SubPlano) || !String.IsNullOrEmpty(planoAntigo.SubPlanoParticular);
                                                bolRepeatOnce = false;

                                                String strCodigoPlanoAntigo = null;

                                                if (ContratoPlanoAntigo.TipoAcomodacao > -1)
                                                    if (ContratoPlanoAntigo.TipoAcomodacao == 0)
                                                        strCodigoPlanoAntigo = (String.IsNullOrEmpty(planoAntigo.Codigo)) ? String.Empty : planoAntigo.Codigo;
                                                    else
                                                        strCodigoPlanoAntigo = (String.IsNullOrEmpty(planoAntigo.CodigoParticular)) ? String.Empty : planoAntigo.CodigoParticular;

                                                do
                                                {
                                                    intQtdeRegistroU03++;

                                                    //NR_SEQ
                                                    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                                    //TP_REG
                                                    sbFileBuffer.Append(U03);
                                                    //TP_MOV
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.Exclusao, 1);
                                                    //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);


                                                    //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);

                                                    //SQ_USU
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                                    //CD_PLANO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(strCodigoPlanoAntigo))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoPlanoAntigo.PadLeft(10, '0'), 10);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                                    //DT_INICIO
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, ContratoPlanoAntigo.Data.ToString("ddMMyyyy"), 8);//EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
                                                    //DT_FIM
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, DateTime.Now.ToString("ddMMyyyy"), 8);
                                                    //ITEM_REDUCAO : TODO ? Saber da onde vem essa informação
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoCarencia, 2);

                                                    sbFileBuffer.AppendLine();

                                                    if (bolHasSubPlano)
                                                    {
                                                        if (ContratoPlanoAntigo.TipoAcomodacao == 0)
                                                            strCodigoPlanoAntigo = planoAntigo.SubPlano;
                                                        else
                                                            strCodigoPlanoAntigo = planoAntigo.SubPlanoParticular;

                                                        bolRepeatOnce = !bolRepeatOnce;
                                                    }

                                                } while (bolHasSubPlano && bolRepeatOnce);
                                            }
                                        }

                                        ContratoPlanoAntigo = null;
                                    }

                                    #endregion

                                    #region Inclusão de Plano Novo

                                    bolHasSubPlano = !String.IsNullOrEmpty(plano.SubPlano) || !String.IsNullOrEmpty(plano.SubPlanoParticular);
                                    bolRepeatOnce = false;

                                    ContratoPlano ContratoPlanoAtual = ContratoPlano.CarregarAtual(contrato.ID, PM);
                                    if (ContratoPlanoAtual == null) { ContratoPlanoAtual = new ContratoPlano(); }

                                    do
                                    {
                                        intQtdeRegistroU03++;

                                        //NR_SEQ
                                        sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                        //TP_REG
                                        sbFileBuffer.Append(U03);
                                        //TP_MOV
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.Inclusao, 1);
                                        //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                        //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);

                                        //SQ_USU
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                        //CD_PLANO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(strCodigoPlano))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoPlano.PadLeft(10, '0'), 10);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                        //DT_INICIO
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, ContratoPlanoAtual.Data.ToString("ddMMyyyy"), 8); //EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
                                        //DT_FIM
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                        //ITEM_REDUCAO : TODO ? Saber de onde vem essa informação
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoCarencia, 2);

                                        sbFileBuffer.AppendLine();

                                        if (bolHasSubPlano)
                                        {
                                            if (contrato.TipoAcomodacao == 0)
                                                strCodigoPlano = plano.SubPlano;
                                            else
                                                strCodigoPlano = plano.SubPlanoParticular;

                                            bolRepeatOnce = !bolRepeatOnce;
                                        }

                                    } while (bolHasSubPlano && bolRepeatOnce);

                                    #endregion

                                    #endregion

                                    #region Caso seja uma MUDANÇA DE PLANO, devemos EMITIR uma segunda via de Cartão.
                                    if (Mov.Equals(Movimentacao.MudancaDePlano))
                                        //{
                                        //    #region Seção U02 Alteração de Cartão

                                        //    intQtdeRegistroU02++;

                                        //    //NR_SEQ
                                        //    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                        //    //TP_REG
                                        //    sbFileBuffer.Append(U02);
                                        //    //TP_MOV
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.EmissaoSegundaVia, 1);
                                        //    //CD_EMP
                                        //    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                        //    //CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        //    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF, 11);
                                        //    else
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                        //    //CPF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        //    if (!String.IsNullOrEmpty(strBeneficiarioCPF))
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCPF, 11);
                                        //    else
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                        //    //RG_TRAB : TODO ? Tratar a exceção de alguma maneira
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                        //    //CD_USU : TODO ? Confirmar se é 0 para mandar, está igual arq exemplo
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                        //    //SQ_USU
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                        //    //DV_USU
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                        //    //NOME_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        //    if (!String.IsNullOrEmpty(strBeneficiarioNome))
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNome, 70);
                                        //    else
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
                                        //    //TP_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        //    if (intBeneficiarioTipo > 0)
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioTipo.ToString(), 1);
                                        //    else
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                        //    //CD_SEXO
                                        //    if (!String.IsNullOrEmpty(strBeneficiarioSexo))
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, (strBeneficiarioSexo.Equals("1") ? "M" : "F"), 1);
                                        //    else
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 1);
                                        //    //CD_PARENT
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioParentescoCod.PadLeft(2, '0'), 2);
                                        //    //DT_NASC
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataNascimento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataNascimento.ToString("ddMMyyyy")), 8);
                                        //    //CD_EST_CIVIL
                                        //    if (beneficiarioEstadoCivilID != null)
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioEstadoCivilID.ToString(), 1);
                                        //    else
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                        //    //DT_CASAMENTO
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataCasamento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataCasamento.ToString("ddMMyyyy")), 8);
                                        //    //IDENTIDADE
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioRG, 20);
                                        //    //EMISSOR
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 10);
                                        //    //CD_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                        //    //NOME_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                        //    //CD_SUB_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                        //    //CD_CARGO : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                        //    //DT_ADMISSAO
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                        //    //DT_CADASTRO : TODO ? Tratar exceção em caso de FALSE. (OBRIGATÓRIO)
                                        //    if (dtBeneficiarioCadastro.CompareTo(DateTime.MinValue) > 0)
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioCadastro.ToString("ddMMyyyy"), 8);
                                        //    else
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                        //    //DT_EXCLUSAO
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                        //    sbFileBuffer.AppendLine();

                                        //    #endregion
                                        //}
                                    #endregion

                                        if (Mov.Equals(Movimentacao.InclusaoBeneficiario))
                                        {
                                            #region Seção U07 / U08

                                            IList<ItemDeclaracaoSaudeINSTANCIA> lstDeclaracaoSaude = ItemDeclaracaoSaudeINSTANCIA.Carregar(beneficiarioID, OperadoraID);

                                            if (lstDeclaracaoSaude != null && lstDeclaracaoSaude.Count > 0)
                                            {
                                                ItemDeclaracaoSaude itemDeclaracao = new ItemDeclaracaoSaude();

                                                foreach (ItemDeclaracaoSaudeINSTANCIA itemDeclaracaoInstancia in lstDeclaracaoSaude)
                                                {
                                                    if (itemDeclaracaoInstancia.Sim)
                                                    {
                                                        intQtdeRegistroU07U08++;

                                                        itemDeclaracao.ID = itemDeclaracaoInstancia.ItemDeclaracaoID;
                                                        itemDeclaracao.Carregar();

                                                        #region Seção U07

                                                        //NR_SEQ
                                                        sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                                        //TP_REG
                                                        sbFileBuffer.Append(U07);
                                                        //TP_MOV
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMov, 1);
                                                        //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                                        if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                                        //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                        if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
                                                        //SQ_USU
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                                        //CD_QUESTAO
                                                        if (!String.IsNullOrEmpty(itemDeclaracao.Codigo))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracao.Codigo.PadLeft(3, '0'), 3);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "000", 3);
                                                        //RESPOSTA
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "S", 1);
                                                        //DT_EVENTO
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.Data.ToString("ddMMyyyy"), 8);
                                                        //ESPECIFICACAO
                                                        if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.Descricao))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.Descricao, 400);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 400);

                                                        sbFileBuffer.AppendLine();

                                                        #endregion

                                                        #region Seção U08

                                                        //NR_SEQ
                                                        sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                                        //TP_REG
                                                        sbFileBuffer.Append(U08);
                                                        //TP_MOV
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMov, 1);
                                                        //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                                        if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                                        //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                        if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
                                                        //SQ_USU
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                                        //CD_CID_INICIAL
                                                        if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.CIDInicial))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.CIDInicial, 4);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 4);
                                                        //CD_CID_FINAL
                                                        if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.CIDFinal))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.CIDFinal, 4);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 4);

                                                        sbFileBuffer.AppendLine();

                                                        #endregion
                                                    }
                                                }

                                                itemDeclaracao = null;
                                            }

                                            lstDeclaracaoSaude = null;

                                            #endregion
                                        }
                                }

                                #region Build Lote

                                itemLote = new ArqTransacionalLoteItem();

                                itemLote.ContratoID = contrato.ID;
                                itemLote.BeneficiarioID = beneficiarioID;
                                itemLote.BeneficiarioSequencia = intBeneficiarioSequencia;
                                itemLote.Ativo = true;

                                lote.Itens.Add(itemLote);

                                #endregion
                            }

                            #region Seção U99

                            //NR_SEQ
                            sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                            //TP_REG
                            sbFileBuffer.Append(U99);
                            //QD_U02
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU02.ToString().PadLeft(6, '0'), 6);
                            //QD_U03
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU03.ToString().PadLeft(6, '0'), 6);
                            //QD_U04
                            EntityBase.AppendPreparedField(ref sbFileBuffer, "000000", 6);
                            //QD_U07
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU07U08.ToString().PadLeft(6, '0'), 6);
                            //QD_U08
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU07U08.ToString().PadLeft(6, '0'), 6);
                            ////QD_U10
                            //sbFileBuffer.Append("000000");
                            //QD_INC
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioInclusao.ToString().PadLeft(6, '0'), 6);
                            //QD_EXC
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioExclusao.ToString().PadLeft(6, '0'), 6);
                            //QD_USU
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioAlteracao.ToString().PadLeft(6, '0'), 6);

                            #endregion

                            if (intQtdeBeneficiarioInclusao > 0)
                                lote.Quantidade = intQtdeBeneficiarioInclusao;
                            else if (intQtdeBeneficiarioAlteracao > 0)
                                lote.Quantidade = intQtdeBeneficiarioAlteracao;
                            else if (intQtdeBeneficiarioExclusao > 0)
                                lote.Quantidade = intQtdeBeneficiarioExclusao;

                            if (lote.Itens != null && lote.Itens.Count > 0)
                            {
                                try
                                {
                                    lote.Salvar(true, PM);
                                }
                                catch (Exception) { throw; }

                                String strArquivoNome = lote.Arquivo;

                                try
                                {
                                    this.SalvarArqTransacional(sbFileBuffer, strArquivoNome);
                                }
                                catch (Exception) { throw; }

                                criacaoOK = true;
                                ArquivoNome = strArquivoNome;

                                PM.Rollback(); //PM.Commit();
                            }
                            else
                            {
                                PM.Rollback();
                            }
                        }
                        else
                        {
                            PM.Rollback();
                        }
                    }
                }
                catch (Exception)
                {
                    PM.Rollback();
                    throw;
                }
                finally
                {
                    PM.Dispose();
                    PM = null;
                }

                sbFileBuffer = null;
            }

            arqTransUnimed = null;

            return criacaoOK;
        }

        public Boolean GerarArquivoUNIMED_DemaisMovimentacoes(ref String ArquivoNome, IList<ItemAgendaArquivoUnimed> itens, String Mov)
        {
            Boolean criacaoOK = false;

            Object OperadoraID = Operadora.UnimedID;

            ArqTransacionalUnimedConf arqTransUnimed = new ArqTransacionalUnimedConf();
            arqTransUnimed.CarregarPorOperadora(OperadoraID);

            #region traduz tipo movimentacao 

            String TipoMov = "";
            String cbTipo0Cond = "";

            switch (Mov)
            {
                case Movimentacao.AlteracaoBeneficiario:
                case Movimentacao.MudancaDePlano:
                {
                    TipoMov = TipoMovimentacao.Alteracao;
                    break;
                }
                case Movimentacao.ExclusaoBeneficiario:
                {
                    TipoMov = TipoMovimentacao.Exclusao;
                    break;
                }
                case Movimentacao.CancelamentoContrato:
                {
                    TipoMov = TipoMovimentacao.Exclusao;
                    cbTipo0Cond = " and cben.contratobeneficiario_tipo=0 ";
                    break;
                }
                case Movimentacao.InclusaoBeneficiario:
                {
                    TipoMov = TipoMovimentacao.Inclusao;
                    break;
                }
                case Movimentacao.SegundaViaCartaoBeneficiario:
                {
                    TipoMov = TipoMovimentacao.EmissaoSegundaVia;
                    break;
                }
            }
            #endregion

            if (arqTransUnimed.ID != null)
            {
                Int32 intArqNumeroSequencia = 1;
                Int32 intQtdeBeneficiarioInclusao = 0;
                Int32 intQtdeBeneficiarioExclusao = 0;
                Int32 intQtdeBeneficiarioAlteracao = 0;
                Int32 intQtdeRegistroU02 = 0;
                Int32 intQtdeRegistroU03 = 0;
                Int32 intQtdeRegistroU07U08 = 0;

                ArqTransacionalLote lote = new ArqTransacionalLote();
                StringBuilder sbFileBuffer = new StringBuilder();

                StringBuilder condition = new StringBuilder();
                foreach (ItemAgendaArquivoUnimed item in itens)
                {
                    if (condition.Length > 0) { condition.Append(" OR "); }
                    condition.Append("(contratobeneficiario_ativo=1 AND contratobeneficiario_beneficiarioId="); condition.Append(item.BeneficiarioID);
                    condition.Append(" AND contratobeneficiario_contratoId="); condition.Append(item.PropostaID);
                    condition.Append(")");
                }

                PersistenceManager PM = new PersistenceManager();
                PM = new PersistenceManager();
                PM.BeginTransactionContext();

                try
                {
                    //using (DataTable dtBeneficiarios = LocatorHelper.Instance.ExecuteQuery("select contrato_admissao, contrato_numeroMatricula, contratobeneficiario_carenciaCodigo, contrato_vigencia, contratoadm_numero, contratoadm_id, item_lote_id, lote_data_criacao, c.contrato_numero, cBen.contratobeneficiario_id, cBen.contratobeneficiario_vigencia, cBen.contratobeneficiario_contratoId, cBen.contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo, contratobeneficiario_numeroSequencia, cBen.contratobeneficiario_portabilidade, cBen.contratobeneficiario_data, cBen.contratobeneficiario_estadoCivilId, estadocivil_codigo, beneficiario_nome, beneficiario_sexo,beneficiario_cpf, beneficiario_rg, beneficiario_rgUF, beneficiario_rgOrgaoExp, beneficiario_declaracaoNascimentoVivo, beneficiario_cns, cBen.contratobeneficiario_dataCasamento, beneficiario_nomeMae, beneficiario_dataNascimento,beneficiario_telefone, beneficiario_ramal, beneficiario_telefone2, beneficiario_celular, beneficiario_email, endereco_logradouro, endereco_numero, endereco_complemento,endereco_bairro, endereco_cidade, endereco_uf, endereco_cep FROM contrato_beneficiario cBen INNER JOIN contrato c ON cBen.contratobeneficiario_contratoId = c.contrato_id and cben.contratobeneficiario_tipo=0 INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id and cben.contratobeneficiario_tipo=0 INNER JOIN endereco ende ON c.contrato_enderecoReferenciaId = ende.endereco_id INNER JOIN contratoADM on contratoadm_id=contrato_contratoAdmId INNER JOIN estado_civil ON estadocivil_id=cBen.contratobeneficiario_estadocivilId LEFT JOIN contratoAdm_parentesco_agregado ON contratoAdmparentescoagregado_id=cBen.contratobeneficiario_parentescoId LEFT JOIN arquivo_transacional_lote_item ON item_contrato_id=c.contrato_id AND item_beneficiario_id=Ben.beneficiario_id AND item_ativo=1 LEFT JOIN arquivo_transacional_lote ON lote_id=item_lote_id AND lote_exportacao <> 1  WHERE " + condition.ToString() + " order by beneficiario_nome", "resultset").Tables[0])
                    //using (DataTable dtBeneficiarios = LocatorHelper.Instance.ExecuteQuery("select contrato_admissao, contrato_numeroMatricula, contratobeneficiario_carenciaCodigo, contrato_vigencia, contratoadm_numero, contratoadm_id, c.contrato_numero, cBen.contratobeneficiario_id, cBen.contratobeneficiario_vigencia, cBen.contratobeneficiario_contratoId, cBen.contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo, contratobeneficiario_numeroSequencia, cBen.contratobeneficiario_portabilidade, cBen.contratobeneficiario_data, cBen.contratobeneficiario_estadoCivilId, estadocivil_codigo, beneficiario_nome, beneficiario_sexo,beneficiario_cpf, beneficiario_rg, beneficiario_rgUF, beneficiario_rgOrgaoExp, beneficiario_declaracaoNascimentoVivo, beneficiario_cns, cBen.contratobeneficiario_dataCasamento, beneficiario_nomeMae, beneficiario_dataNascimento,beneficiario_telefone, beneficiario_ramal, beneficiario_telefone2, beneficiario_celular, beneficiario_email, endereco_logradouro, endereco_numero, endereco_complemento,endereco_bairro, endereco_cidade, endereco_uf, endereco_cep FROM contrato_beneficiario cBen INNER JOIN contrato c ON cBen.contratobeneficiario_contratoId = c.contrato_id and cben.contratobeneficiario_tipo=0 INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id and cben.contratobeneficiario_tipo=0 INNER JOIN endereco ende ON c.contrato_enderecoReferenciaId = ende.endereco_id INNER JOIN contratoADM on contratoadm_id=contrato_contratoAdmId INNER JOIN estado_civil ON estadocivil_id=cBen.contratobeneficiario_estadocivilId LEFT JOIN contratoAdm_parentesco_agregado ON contratoAdmparentescoagregado_id=cBen.contratobeneficiario_parentescoId WHERE " + condition.ToString() + " order by beneficiario_nome", "resultset").Tables[0])
                    using(DataTable dtBeneficiarios = LocatorHelper.Instance.ExecuteQuery("select contrato_admissao, contrato_numeroMatricula, contratobeneficiario_carenciaCodigo, contrato_vigencia, contratoadm_numero, contratoadm_id, c.contrato_numero, cBen.contratobeneficiario_id, cBen.contratobeneficiario_vigencia, cBen.contratobeneficiario_contratoId, cBen.contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo, contratobeneficiario_numeroSequencia, cBen.contratobeneficiario_portabilidade, cBen.contratobeneficiario_data, cBen.contratobeneficiario_estadoCivilId, estadocivil_codigo, beneficiario_nome, beneficiario_sexo,beneficiario_cpf, beneficiario_rg, beneficiario_rgUF, beneficiario_rgOrgaoExp, beneficiario_declaracaoNascimentoVivo, beneficiario_cns, cBen.contratobeneficiario_dataCasamento, beneficiario_nomeMae, beneficiario_dataNascimento,beneficiario_telefone, beneficiario_ramal, beneficiario_telefone2, beneficiario_celular, beneficiario_email, endereco_logradouro, endereco_numero, endereco_complemento,endereco_bairro, endereco_cidade, endereco_uf, endereco_cep FROM contrato_beneficiario cBen INNER JOIN contrato c ON cBen.contratobeneficiario_contratoId = c.contrato_id INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id LEFT JOIN endereco ende ON c.contrato_enderecoReferenciaId = ende.endereco_id INNER JOIN contratoADM on contratoadm_id=contrato_contratoAdmId INNER JOIN estado_civil ON estadocivil_id=cBen.contratobeneficiario_estadocivilId LEFT JOIN contratoAdm_parentesco_agregado ON contratoAdmparentescoagregado_id=cBen.contratobeneficiario_parentescoId WHERE " + condition.ToString() + " --order by beneficiario_nome", "resultset").Tables[0])
                    {
                        if (dtBeneficiarios != null && dtBeneficiarios.Rows != null && dtBeneficiarios.Rows.Count > 0)
                        {
                            #region variaveis 

                            ArqTransacionalLoteItem itemLote = null;
                            Contrato contrato = new Contrato();
                            Plano plano = new Plano();
                            Object contratoID, beneficiarioID, beneficiarioEstadoCivilID;
                            String beneficiarioParentescoCod, strBeneficiarioNome, strBeneficiarioSexo, strBeneficiarioCPF, strBeneficiarioTitularCPF, strCodigoCarencia,
                                   strBeneficiarioRG, strBeneficiarioEndereco, strBeneficiarioEnderecoNum, strBeneficiarioEnderecoCompl,
                                   strBeneficiarioBairro, strBeneficiarioCEP, strBeneficiarioCidade, strBeneficiarioUF, strBeneficiarioTelefone,
                                   strBeneficiarioTelefoneRamal, strBeneficiarioNomeMae, strBneficiarioPisPasep, strCodigoPlano, strTipoMovimentacaoAux;
                            Int16 intBeneficiarioSequencia, intBeneficiarioTipo;
                            DateTime dtBeneficiarioDataNascimento, dtBeneficiarioDataCasamento, dtBeneficiarioVigencia, dtBeneficiarioCadastro, vigenciaProposta;

                            lote.OperadoraID = OperadoraID;
                            lote.Movimentacao = Mov;
                            lote.TipoMovimentacao = TipoMov;

                            #endregion

                            #region Seção U01

                            if (dtBeneficiarios.Rows.Count > 0)
                            {
                                arqTransUnimed.OperadoraNaUnimed = Convert.ToString(dtBeneficiarios.Rows[0]["contratoadm_numero"]).Trim().PadLeft(8, '0');
                            }

                            //NR_SEQ
                            sbFileBuffer.Append(intArqNumeroSequencia.ToString().PadLeft(6, '0'));
                            //TP_REG
                            sbFileBuffer.Append(U01);
                            //CD_UNI
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCodSingular))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCodSingular, 4);
                            //CD_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                            //NOME_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNome))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNome, 40);
                            //CNPJ_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCNPJ))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCNPJ, 14);
                            //TP_IDENTIFICA
                            if (!String.IsNullOrEmpty(arqTransUnimed.TipoIdentificacao.ToString()))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.TipoIdentificacao, 1);
                            //VERSAO
                            if (!String.IsNullOrEmpty("4.0")) //arqTransUnimed.ArqVersao
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.ArqVersao, 3);
                            //DT_GERACAO
                            sbFileBuffer.Append(DateTime.Now.ToString("ddMMyyyy"));
                            //CEI_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCEI))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCEI, 13);

                            sbFileBuffer.AppendLine();

                            #endregion

                            foreach (DataRow row in dtBeneficiarios.Rows)
                            {
                                switch (Mov)
                                {
                                    case Movimentacao.InclusaoBeneficiario:
                                        intQtdeBeneficiarioInclusao++;
                                        break;
                                    case Movimentacao.ExclusaoBeneficiario:
                                        intQtdeBeneficiarioExclusao++;
                                        break;
                                    case Movimentacao.AlteracaoBeneficiario:
                                        intQtdeBeneficiarioAlteracao++;
                                        break;
                                    case Movimentacao.MudancaDePlano:
                                        intQtdeBeneficiarioAlteracao++;
                                        break;
                                }

                                contratoID = row["contratobeneficiario_contratoId"];
                                contrato.ID = contratoID;

                                if (contrato.ID != null)
                                {
                                    contrato.Carregar();
                                    if (contrato.PlanoID != null)
                                    {
                                        plano.ID = contrato.PlanoID;
                                        plano.Carregar();
                                    }
                                }

                                #region preenche variáveis

                                beneficiarioID = row["contratobeneficiario_beneficiarioId"];
                                beneficiarioParentescoCod = (row["contratoAdmparentescoagregado_parentescoCodigo"] == null || row["contratoAdmparentescoagregado_parentescoCodigo"] is DBNull) ? "00" : Convert.ToString(row["contratoAdmparentescoagregado_parentescoCodigo"]);
                                beneficiarioEstadoCivilID = (row["contratobeneficiario_estadoCivilId"] == null || row["contratobeneficiario_estadoCivilId"] is DBNull) ? "0" : Convert.ToString(row["estadocivil_codigo"]);
                                strBeneficiarioNome = (row["beneficiario_nome"] == null || row["beneficiario_nome"] is DBNull) ? null : Convert.ToString(row["beneficiario_nome"]);
                                strBeneficiarioSexo = (row["beneficiario_sexo"] == null || row["beneficiario_sexo"] is DBNull) ? null : Convert.ToString(row["beneficiario_sexo"]);
                                strBeneficiarioCPF = (row["beneficiario_cpf"] == null || row["beneficiario_cpf"] is DBNull) ? "" : Convert.ToString(row["beneficiario_cpf"]);
                                strBeneficiarioCPF = strBeneficiarioCPF.Replace("99999999999", "00000000000");

                                strBeneficiarioTitularCPF = ContratoBeneficiario.GetCPFTitular(contratoID, PM);
                                strBeneficiarioRG = (row["beneficiario_rg"] == null || row["beneficiario_rg"] is DBNull) ? String.Empty : Convert.ToString(row["beneficiario_rg"]);
                                intBeneficiarioTipo = (row["contratobeneficiario_tipo"] == null || row["contratobeneficiario_tipo"] is DBNull) ? Convert.ToInt16(-1) : Convert.ToInt16(row["contratobeneficiario_tipo"]);
                                intBeneficiarioSequencia = (row["contratobeneficiario_numeroSequencia"] == null || row["contratobeneficiario_numeroSequencia"] is DBNull) ? Convert.ToInt16(0) : Convert.ToInt16(row["contratobeneficiario_numeroSequencia"]);
                                dtBeneficiarioDataNascimento = (row["beneficiario_dataNascimento"] == null || row["beneficiario_dataNascimento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["beneficiario_dataNascimento"]);
                                dtBeneficiarioVigencia = (row["contratobeneficiario_vigencia"] == null || row["contratobeneficiario_vigencia"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_vigencia"]);
                                dtBeneficiarioDataCasamento = (row["contratobeneficiario_dataCasamento"] == null || row["contratobeneficiario_dataCasamento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_dataCasamento"]);
                                dtBeneficiarioCadastro = (row["contratobeneficiario_data"] == null || row["contratobeneficiario_data"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_data"]);
                                strBeneficiarioEndereco = (row["endereco_logradouro"] == null || row["endereco_logradouro"] is DBNull) ? null : Convert.ToString(row["endereco_logradouro"]);
                                strBeneficiarioEnderecoNum = (row["endereco_numero"] == null || row["endereco_numero"] is DBNull) ? null : Convert.ToString(row["endereco_numero"]);
                                strBeneficiarioEnderecoCompl = (row["endereco_complemento"] == null || row["endereco_complemento"] is DBNull) ? null : Convert.ToString(row["endereco_complemento"]);
                                strBeneficiarioBairro = (row["endereco_bairro"] == null || row["endereco_bairro"] is DBNull) ? null : Convert.ToString(row["endereco_bairro"]);
                                strBeneficiarioCEP = (row["endereco_cep"] == null || row["endereco_cep"] is DBNull) ? null : Convert.ToString(row["endereco_cep"]);
                                strBeneficiarioCidade = (row["endereco_cidade"] == null || row["endereco_cidade"] is DBNull) ? null : Convert.ToString(row["endereco_cidade"]);
                                strBeneficiarioUF = (row["endereco_uf"] == null || row["endereco_uf"] is DBNull) ? null : Convert.ToString(row["endereco_uf"]);
                                strBeneficiarioTelefone = (row["beneficiario_telefone"] == null || row["beneficiario_telefone"] is DBNull) ? null : Convert.ToString(row["beneficiario_telefone"]);
                                strBeneficiarioTelefoneRamal = (row["beneficiario_ramal"] == null || row["beneficiario_ramal"] is DBNull) ? null : Convert.ToString(row["beneficiario_ramal"]);
                                strBeneficiarioNomeMae = (row["beneficiario_nomeMae"] == null || row["beneficiario_nomeMae"] is DBNull) ? null : Convert.ToString(row["beneficiario_nomeMae"]);
                                strCodigoCarencia = (row["contratobeneficiario_carenciaCodigo"] == null || row["contratobeneficiario_carenciaCodigo"] is DBNull) ? "" : Convert.ToString(row["contratobeneficiario_carenciaCodigo"]).PadLeft(2, '0');
                                strBneficiarioPisPasep = String.Empty;
                                strCodigoPlano = String.Empty;
                                arqTransUnimed.OperadoraNaUnimed = Convert.ToString(row["contratoadm_numero"]).Trim().PadLeft(8, '0');
                                #endregion

                                if (strBeneficiarioTelefone != null)
                                {
                                    strBeneficiarioTelefone = strBeneficiarioTelefone.Replace("-", "");
                                    if (strBeneficiarioTelefone.Length > 8)
                                    {
                                        strBeneficiarioTelefone = strBeneficiarioTelefone.Substring(4).Trim();
                                    }
                                }

                                if (contrato.TipoAcomodacao > -1)
                                    if (contrato.TipoAcomodacao == 0)
                                        strCodigoPlano = (String.IsNullOrEmpty(plano.Codigo)) ? String.Empty : plano.Codigo;
                                    else
                                        strCodigoPlano = (String.IsNullOrEmpty(plano.CodigoParticular)) ? String.Empty : plano.CodigoParticular;

                                // CASO SEJA UMA MUDANÇA DE PLANO, SINALIZA A SEÇÃO PARA UMA ALTERAÇÃO
                                if (Mov.Equals(Movimentacao.MudancaDePlano) && TipoMov.Equals(TipoMovimentacao.Exclusao))
                                    strTipoMovimentacaoAux = TipoMovimentacao.Alteracao;
                                else
                                    strTipoMovimentacaoAux = TipoMov;

                                #region Seção U02

                                intQtdeRegistroU02++;

                                //NR_SEQ
                                sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                //TP_REG
                                sbFileBuffer.Append(U02);
                                //TP_MOV
                                EntityBase.AppendPreparedField(ref sbFileBuffer, strTipoMovimentacaoAux, 1);
                                //CD_EMP
                                if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                //CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF, 11);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                //CPF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (!String.IsNullOrEmpty(strBeneficiarioCPF))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCPF, 11);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                //RG_TRAB : TODO ? Tratar a exceção de alguma maneira
                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                //CD_USU : TODO ? Confirmar se é 0 para mandar, está igual arq exemplo
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                //SQ_USU
                                EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                //DV_USU
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                //NOME_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (!String.IsNullOrEmpty(strBeneficiarioNome))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNome, 70);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
                                //TP_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (intBeneficiarioTipo > 0)
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioTipo.ToString(), 1);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                //CD_SEXO
                                if (!String.IsNullOrEmpty(strBeneficiarioSexo))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, (strBeneficiarioSexo.Equals("1") ? "M" : "F"), 1);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 1);
                                //CD_PARENT
                                EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioParentescoCod.PadLeft(2, '0'), 2);
                                //DT_NASC
                                EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataNascimento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataNascimento.ToString("ddMMyyyy")), 8);
                                //CD_EST_CIVIL
                                if (beneficiarioEstadoCivilID != null)
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioEstadoCivilID.ToString(), 1);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                //DT_CASAMENTO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataCasamento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataCasamento.ToString("ddMMyyyy")), 8);
                                //IDENTIDADE
                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioRG, 20);
                                //EMISSOR
                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 10);

                                ////DECL_NASC_VIVO
                                //if (row["beneficiario_declaracaoNascimentoVivo"] == null || row["beneficiario_declaracaoNascimentoVivo"] == DBNull.Value || Convert.ToString(row["beneficiario_declaracaoNascimentoVivo"]).Trim() == "")
                                //    sbFileBuffer.Append("0".PadLeft(11, '0'));
                                //else
                                //    sbFileBuffer.Append(Convert.ToString(row["beneficiario_declaracaoNascimentoVivo"]).PadLeft(11, '0'));
                                ////CNS
                                //if (row["beneficiario_cns"] == null || row["beneficiario_cns"] == DBNull.Value || Convert.ToString(row["beneficiario_cns"]).Trim() == "")
                                //    sbFileBuffer.Append("0".PadLeft(15, '0'));
                                //else
                                //    sbFileBuffer.Append(Convert.ToString(row["beneficiario_cns"]).PadLeft(15, '0'));

                                //CD_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                //NOME_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                //CD_SUB_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                //CD_CARGO : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                //DT_ADMISSAO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                //DATA EFETIVACAO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                //DT_CADASTRO (VIGENCIA) : TODO ? Tratar exceção em caso de FALSE. (OBRIGATÓRIO)
                                if (dtBeneficiarioCadastro.CompareTo(DateTime.MinValue) > 0)
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                //DT_EXCLUSAO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                ////PORTABILIDADE
                                //sbFileBuffer.Append("N");

                                sbFileBuffer.AppendLine();

                                #endregion

                                if (Mov.Equals(Movimentacao.InclusaoBeneficiario) ||
                                    Mov.Equals(Movimentacao.AlteracaoBeneficiario))
                                {
                                    #region Seção U05

                                    //NR_SEQ
                                    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                    //TP_REG
                                    sbFileBuffer.Append(U05);
                                    //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                    //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
                                    //SQ_USU
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                    //ENDERECO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioEndereco))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEndereco, 45);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 45);

                                    //NR_ENDER : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioEnderecoNum))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEnderecoNum.Trim().PadLeft(5, '0'), 5);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);

                                    //COMPL_ENDER : TODO ? Limitar a 15 o complemento da UNIMED no cadastro de endereço
                                    if (!String.IsNullOrEmpty(strBeneficiarioEnderecoCompl))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEnderecoCompl, 15);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 15);
                                    //BAIRRO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioBairro))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioBairro, 20);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                    //CEP : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioCEP))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCEP, 8);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                    //CIDADE : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioCidade))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCidade, 30);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 30);
                                    //UF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioUF))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioUF, 2);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 2);
                                    //TELEFONE
                                    if (!String.IsNullOrEmpty(strBeneficiarioTelefone))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTelefone, 9);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 9);
                                    //RAMAL
                                    if (!String.IsNullOrEmpty(strBeneficiarioTelefoneRamal))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTelefoneRamal, 6);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 6);
                                    //NOME_MAE : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioNomeMae))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNomeMae, 70);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
                                    //PIS_PASEP : TODO ? Não tem no Cadastro, ele não é enviado nem com caracter em branco mesmo estando com espaço em branco
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBneficiarioPisPasep, 11);
                                    ////CIDADE_RESIDENCIA
                                    //EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCidade, 30);

                                    sbFileBuffer.AppendLine();

                                    #endregion
                                }

                                if (Mov.Equals(Movimentacao.InclusaoBeneficiario) || Mov.Equals(Movimentacao.MudancaDePlano))
                                {
                                    #region Seção U03

                                    Boolean bolHasSubPlano = false;
                                    Boolean bolRepeatOnce = false;

                                    #region Exclusão de Plano Antigo

                                    if (Mov.Equals(Movimentacao.MudancaDePlano)) //&& TipoMov.Equals(TipoMovimentacao.Exclusao))
                                    {
                                        ContratoPlano ContratoPlanoAntigo = ContratoPlano.CarregarPenultimo(contrato.ID, PM);

                                        if (ContratoPlanoAntigo != null)
                                        {
                                            // Carregar o plano antigo, código do plano antigo, acomodação, etc.
                                            if (ContratoPlanoAntigo != null && ContratoPlanoAntigo.ID != null)
                                            {
                                                Plano planoAntigo = new Plano(ContratoPlanoAntigo.PlanoID);
                                                planoAntigo.Carregar();

                                                bolHasSubPlano = !String.IsNullOrEmpty(planoAntigo.SubPlano) || !String.IsNullOrEmpty(planoAntigo.SubPlanoParticular);
                                                bolRepeatOnce = false;

                                                String strCodigoPlanoAntigo = null;

                                                if (ContratoPlanoAntigo.TipoAcomodacao > -1)
                                                    if (ContratoPlanoAntigo.TipoAcomodacao == 0)
                                                        strCodigoPlanoAntigo = (String.IsNullOrEmpty(planoAntigo.Codigo)) ? String.Empty : planoAntigo.Codigo;
                                                    else
                                                        strCodigoPlanoAntigo = (String.IsNullOrEmpty(planoAntigo.CodigoParticular)) ? String.Empty : planoAntigo.CodigoParticular;

                                                do
                                                {
                                                    intQtdeRegistroU03++;

                                                    //NR_SEQ
                                                    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                                    //TP_REG
                                                    sbFileBuffer.Append(U03);
                                                    //TP_MOV
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.Exclusao, 1);
                                                    //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);


                                                    //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);

                                                    //SQ_USU
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                                    //CD_PLANO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(strCodigoPlanoAntigo))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoPlanoAntigo.PadLeft(10, '0'), 10);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                                    //DT_INICIO
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, ContratoPlanoAntigo.Data.ToString("ddMMyyyy"), 8);//EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
                                                    //DT_FIM
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, DateTime.Now.ToString("ddMMyyyy"), 8);
                                                    //ITEM_REDUCAO : TODO ? Saber da onde vem essa informação
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoCarencia, 2);

                                                    sbFileBuffer.AppendLine();

                                                    if (bolHasSubPlano)
                                                    {
                                                        if (ContratoPlanoAntigo.TipoAcomodacao == 0)
                                                            strCodigoPlanoAntigo = planoAntigo.SubPlano;
                                                        else
                                                            strCodigoPlanoAntigo = planoAntigo.SubPlanoParticular;

                                                        bolRepeatOnce = !bolRepeatOnce;
                                                    }

                                                } while (bolHasSubPlano && bolRepeatOnce);
                                            }
                                        }

                                        ContratoPlanoAntigo = null;
                                    }

                                    #endregion

                                    #region Inclusão de Plano Novo

                                    bolHasSubPlano = !String.IsNullOrEmpty(plano.SubPlano) || !String.IsNullOrEmpty(plano.SubPlanoParticular);
                                    bolRepeatOnce = false;

                                    ContratoPlano ContratoPlanoAtual = ContratoPlano.CarregarAtual(contrato.ID, PM);
                                    if (ContratoPlanoAtual == null) { ContratoPlanoAtual = new ContratoPlano(); }

                                    do
                                    {
                                        intQtdeRegistroU03++;

                                        //NR_SEQ
                                        sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                        //TP_REG
                                        sbFileBuffer.Append(U03);
                                        //TP_MOV
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.Inclusao, 1);
                                        //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                        //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);

                                        //SQ_USU
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                        //CD_PLANO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(strCodigoPlano))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoPlano.PadLeft(10, '0'), 10);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                        //DT_INICIO
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, ContratoPlanoAtual.Data.ToString("ddMMyyyy"), 8); //EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
                                        //DT_FIM
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                        //ITEM_REDUCAO : TODO ? Saber de onde vem essa informação
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoCarencia, 2);

                                        sbFileBuffer.AppendLine();

                                        if (bolHasSubPlano)
                                        {
                                            if (contrato.TipoAcomodacao == 0)
                                                strCodigoPlano = plano.SubPlano;
                                            else
                                                strCodigoPlano = plano.SubPlanoParticular;

                                            bolRepeatOnce = !bolRepeatOnce;
                                        }

                                    } while (bolHasSubPlano && bolRepeatOnce);

                                    #endregion

                                    #endregion

                                    #region Caso seja uma MUDANÇA DE PLANO, devemos EMITIR uma segunda via de Cartão.
                                    if (Mov.Equals(Movimentacao.MudancaDePlano))
                                    //{
                                    //    #region Seção U02 Alteração de Cartão

                                    //    intQtdeRegistroU02++;

                                    //    //NR_SEQ
                                    //    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                    //    //TP_REG
                                    //    sbFileBuffer.Append(U02);
                                    //    //TP_MOV
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.EmissaoSegundaVia, 1);
                                    //    //CD_EMP
                                    //    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                    //    //CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    //    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF, 11);
                                    //    else
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                    //    //CPF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    //    if (!String.IsNullOrEmpty(strBeneficiarioCPF))
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCPF, 11);
                                    //    else
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                    //    //RG_TRAB : TODO ? Tratar a exceção de alguma maneira
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                    //    //CD_USU : TODO ? Confirmar se é 0 para mandar, está igual arq exemplo
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                    //    //SQ_USU
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                    //    //DV_USU
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                    //    //NOME_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    //    if (!String.IsNullOrEmpty(strBeneficiarioNome))
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNome, 70);
                                    //    else
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
                                    //    //TP_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    //    if (intBeneficiarioTipo > 0)
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioTipo.ToString(), 1);
                                    //    else
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                    //    //CD_SEXO
                                    //    if (!String.IsNullOrEmpty(strBeneficiarioSexo))
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, (strBeneficiarioSexo.Equals("1") ? "M" : "F"), 1);
                                    //    else
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 1);
                                    //    //CD_PARENT
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioParentescoCod.PadLeft(2, '0'), 2);
                                    //    //DT_NASC
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataNascimento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataNascimento.ToString("ddMMyyyy")), 8);
                                    //    //CD_EST_CIVIL
                                    //    if (beneficiarioEstadoCivilID != null)
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioEstadoCivilID.ToString(), 1);
                                    //    else
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                    //    //DT_CASAMENTO
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataCasamento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataCasamento.ToString("ddMMyyyy")), 8);
                                    //    //IDENTIDADE
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioRG, 20);
                                    //    //EMISSOR
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 10);
                                    //    //CD_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                    //    //NOME_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                    //    //CD_SUB_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                    //    //CD_CARGO : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                    //    //DT_ADMISSAO
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                    //    //DT_CADASTRO : TODO ? Tratar exceção em caso de FALSE. (OBRIGATÓRIO)
                                    //    if (dtBeneficiarioCadastro.CompareTo(DateTime.MinValue) > 0)
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioCadastro.ToString("ddMMyyyy"), 8);
                                    //    else
                                    //        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                    //    //DT_EXCLUSAO
                                    //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                    //    sbFileBuffer.AppendLine();

                                    //    #endregion
                                    //}
                                    #endregion

                                    if (Mov.Equals(Movimentacao.InclusaoBeneficiario))
                                    {
                                        #region Seção U07 / U08

                                        IList<ItemDeclaracaoSaudeINSTANCIA> lstDeclaracaoSaude = ItemDeclaracaoSaudeINSTANCIA.Carregar(beneficiarioID, OperadoraID);

                                        if (lstDeclaracaoSaude != null && lstDeclaracaoSaude.Count > 0)
                                        {
                                            ItemDeclaracaoSaude itemDeclaracao = new ItemDeclaracaoSaude();

                                            foreach (ItemDeclaracaoSaudeINSTANCIA itemDeclaracaoInstancia in lstDeclaracaoSaude)
                                            {
                                                if (itemDeclaracaoInstancia.Sim)
                                                {
                                                    intQtdeRegistroU07U08++;

                                                    itemDeclaracao.ID = itemDeclaracaoInstancia.ItemDeclaracaoID;
                                                    itemDeclaracao.Carregar();

                                                    #region Seção U07

                                                    //NR_SEQ
                                                    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                                    //TP_REG
                                                    sbFileBuffer.Append(U07);
                                                    //TP_MOV
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMov, 1);
                                                    //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                                    //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
                                                    //SQ_USU
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                                    //CD_QUESTAO
                                                    if (!String.IsNullOrEmpty(itemDeclaracao.Codigo))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracao.Codigo.PadLeft(3, '0'), 3);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "000", 3);
                                                    //RESPOSTA
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "S", 1);
                                                    //DT_EVENTO
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.Data.ToString("ddMMyyyy"), 8);
                                                    //ESPECIFICACAO
                                                    if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.Descricao))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.Descricao, 400);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 400);

                                                    sbFileBuffer.AppendLine();

                                                    #endregion

                                                    #region Seção U08

                                                    //NR_SEQ
                                                    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                                    //TP_REG
                                                    sbFileBuffer.Append(U08);
                                                    //TP_MOV
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMov, 1);
                                                    //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                                    //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
                                                    //SQ_USU
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                                    //CD_CID_INICIAL
                                                    if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.CIDInicial))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.CIDInicial, 4);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 4);
                                                    //CD_CID_FINAL
                                                    if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.CIDFinal))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.CIDFinal, 4);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 4);

                                                    sbFileBuffer.AppendLine();

                                                    #endregion
                                                }
                                            }

                                            itemDeclaracao = null;
                                        }

                                        lstDeclaracaoSaude = null;

                                        #endregion
                                    }
                                }

                                #region Build Lote

                                itemLote = new ArqTransacionalLoteItem();

                                itemLote.ContratoID = contrato.ID;
                                itemLote.BeneficiarioID = beneficiarioID;
                                itemLote.BeneficiarioSequencia = intBeneficiarioSequencia;
                                itemLote.Ativo = true;

                                lote.Itens.Add(itemLote);

                                #endregion
                            }

                            #region Seção U99

                            //NR_SEQ
                            sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                            //TP_REG
                            sbFileBuffer.Append(U99);
                            //QD_U02
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU02.ToString().PadLeft(6, '0'), 6);
                            //QD_U03
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU03.ToString().PadLeft(6, '0'), 6);
                            //QD_U04
                            EntityBase.AppendPreparedField(ref sbFileBuffer, "000000", 6);
                            //QD_U07
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU07U08.ToString().PadLeft(6, '0'), 6);
                            //QD_U08
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU07U08.ToString().PadLeft(6, '0'), 6);
                            ////QD_U10
                            //sbFileBuffer.Append("000000");
                            //QD_INC
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioInclusao.ToString().PadLeft(6, '0'), 6);
                            //QD_EXC
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioExclusao.ToString().PadLeft(6, '0'), 6);
                            //QD_USU
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioAlteracao.ToString().PadLeft(6, '0'), 6);

                            #endregion

                            if (intQtdeBeneficiarioInclusao > 0)
                                lote.Quantidade = intQtdeBeneficiarioInclusao;
                            else if (intQtdeBeneficiarioAlteracao > 0)
                                lote.Quantidade = intQtdeBeneficiarioAlteracao;
                            else if (intQtdeBeneficiarioExclusao > 0)
                                lote.Quantidade = intQtdeBeneficiarioExclusao;

                            if (lote.Itens != null && lote.Itens.Count > 0)
                            {
                                try
                                {
                                    lote.Salvar(true, PM);
                                }
                                catch (Exception) { throw; }

                                String strArquivoNome = lote.Arquivo;

                                try
                                {
                                    this.SalvarArqTransacional(sbFileBuffer, strArquivoNome);
                                }
                                catch (Exception) { throw; }

                                criacaoOK = true;
                                ArquivoNome = strArquivoNome;

                                PM.Commit();
                            }
                            else
                            {
                                PM.Rollback();
                            }
                        }
                        else
                        {
                            PM.Rollback();
                        }
                    }
                }
                catch (Exception)
                {
                    PM.Rollback();
                    throw;
                }
                finally
                {
                    PM.Dispose();
                    PM = null;
                }

                sbFileBuffer = null;
            }

            arqTransUnimed = null;

            return criacaoOK;
        }

        public Boolean GerarArquivoUNIMED_DemaisMovimentacoes_VariosContratosADM(ref String ArquivoNome, IList<ItemAgendaArquivoUnimed> itens, String Mov)
        {
            Boolean criacaoOK = false;

            Object OperadoraID = Operadora.UnimedID;

            ArqTransacionalUnimedConf arqTransUnimed = new ArqTransacionalUnimedConf();
            arqTransUnimed.CarregarPorOperadora(OperadoraID);

            #region traduz tipo movimentacao

            String TipoMov = "";
            String cbTipo0Cond = "";

            switch (Mov)
            {
                case Movimentacao.AlteracaoBeneficiario:
                case Movimentacao.MudancaDePlano:
                    {
                        TipoMov = TipoMovimentacao.Alteracao;
                        break;
                    }
                case Movimentacao.ExclusaoBeneficiario:
                    {
                        TipoMov = TipoMovimentacao.Exclusao;
                        break;
                    }
                case Movimentacao.CancelamentoContrato:
                    {
                        TipoMov = TipoMovimentacao.Exclusao;
                        cbTipo0Cond = " and cben.contratobeneficiario_tipo=0 ";
                        break;
                    }
                case Movimentacao.InclusaoBeneficiario:
                    {
                        TipoMov = TipoMovimentacao.Inclusao;
                        break;
                    }
                case Movimentacao.SegundaViaCartaoBeneficiario:
                    {
                        TipoMov = TipoMovimentacao.EmissaoSegundaVia;
                        break;
                    }
            }
            #endregion

            if (arqTransUnimed.ID != null)
            {
                Int32 intArqNumeroSequencia = 1;
                Int32 intQtdeBeneficiarioInclusao = 0;
                Int32 intQtdeBeneficiarioExclusao = 0;
                Int32 intQtdeBeneficiarioAlteracao = 0;
                Int32 intQtdeRegistroU02 = 0;
                Int32 intQtdeRegistroU03 = 0;
                Int32 intQtdeRegistroU07U08 = 0;

                ArqTransacionalLote lote = new ArqTransacionalLote();
                StringBuilder sbFileBuffer = new StringBuilder();

                StringBuilder condition = new StringBuilder();
                foreach (ItemAgendaArquivoUnimed item in itens)
                {
                    if (condition.Length > 0) { condition.Append(" OR "); }
                    condition.Append("(contratobeneficiario_ativo=1 AND contratobeneficiario_beneficiarioId="); condition.Append(item.BeneficiarioID);
                    condition.Append(" AND contratobeneficiario_contratoId="); condition.Append(item.PropostaID);
                    condition.Append(")");
                }

                PersistenceManager PM = new PersistenceManager();
                PM = new PersistenceManager();
                PM.BeginTransactionContext();

                try
                {
                    using (DataTable dtBeneficiarios = LocatorHelper.Instance.ExecuteQuery("select contrato_admissao, contrato_numeroMatricula, contratobeneficiario_carenciaCodigo, contrato_vigencia, contratoadm_numero, contratoadm_id, contratoadm_descricao, c.contrato_numero, cBen.contratobeneficiario_id, cBen.contratobeneficiario_vigencia, cBen.contratobeneficiario_contratoId, cBen.contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo, contratobeneficiario_numeroSequencia, cBen.contratobeneficiario_portabilidade, cBen.contratobeneficiario_data, cBen.contratobeneficiario_estadoCivilId, estadocivil_codigo, beneficiario_nome, beneficiario_sexo,beneficiario_cpf, beneficiario_rg, beneficiario_rgUF, beneficiario_rgOrgaoExp, beneficiario_declaracaoNascimentoVivo, beneficiario_cns, cBen.contratobeneficiario_dataCasamento, beneficiario_nomeMae, beneficiario_dataNascimento,beneficiario_telefone, beneficiario_ramal, beneficiario_telefone2, beneficiario_celular, beneficiario_email, endereco_logradouro, endereco_numero, endereco_complemento,endereco_bairro, endereco_cidade, endereco_uf, endereco_cep FROM contrato_beneficiario cBen INNER JOIN contrato c ON cBen.contratobeneficiario_contratoId = c.contrato_id INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id LEFT JOIN endereco ende ON c.contrato_enderecoReferenciaId = ende.endereco_id INNER JOIN contratoADM on contratoadm_id=contrato_contratoAdmId INNER JOIN estado_civil ON estadocivil_id=cBen.contratobeneficiario_estadocivilId LEFT JOIN contratoAdm_parentesco_agregado ON contratoAdmparentescoagregado_id=cBen.contratobeneficiario_parentescoId WHERE " + condition.ToString() + " order by contratoadm_descricao", "resultset").Tables[0])
                    {
                        if (dtBeneficiarios != null && dtBeneficiarios.Rows != null && dtBeneficiarios.Rows.Count > 0)
                        {
                            #region variaveis

                            ArqTransacionalLoteItem itemLote = null;
                            Contrato contrato = new Contrato();
                            Plano plano = new Plano();
                            Object contratoID, beneficiarioID, beneficiarioEstadoCivilID;
                            String beneficiarioParentescoCod, strBeneficiarioNome, strBeneficiarioSexo, strBeneficiarioCPF, strBeneficiarioTitularCPF, strCodigoCarencia,
                                   strBeneficiarioRG, strBeneficiarioEndereco, strBeneficiarioEnderecoNum, strBeneficiarioEnderecoCompl,
                                   strBeneficiarioBairro, strBeneficiarioCEP, strBeneficiarioCidade, strBeneficiarioUF, strBeneficiarioTelefone,
                                   strBeneficiarioTelefoneRamal, strBeneficiarioNomeMae, strBneficiarioPisPasep, strCodigoPlano, strTipoMovimentacaoAux;
                            Int16 intBeneficiarioSequencia, intBeneficiarioTipo;
                            DateTime dtBeneficiarioDataNascimento, dtBeneficiarioDataCasamento, dtBeneficiarioVigencia, dtBeneficiarioCadastro, vigenciaProposta;

                            lote.OperadoraID = OperadoraID;
                            lote.Movimentacao = Mov;
                            lote.TipoMovimentacao = TipoMov;

                            #endregion

                            #region Seção U01

                            if (dtBeneficiarios.Rows.Count > 0)
                            {
                                arqTransUnimed.OperadoraNaUnimed = Convert.ToString(dtBeneficiarios.Rows[0]["contratoadm_numero"]).Trim().PadLeft(8, '0');
                            }

                            //NR_SEQ
                            sbFileBuffer.Append(intArqNumeroSequencia.ToString().PadLeft(6, '0'));
                            //TP_REG
                            sbFileBuffer.Append(U01);
                            //CD_UNI
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCodSingular))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCodSingular, 4);
                            //CD_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                            //NOME_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNome))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNome, 40);
                            //CNPJ_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCNPJ))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCNPJ, 14);
                            //TP_IDENTIFICA
                            if (!String.IsNullOrEmpty(arqTransUnimed.TipoIdentificacao.ToString()))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.TipoIdentificacao, 1);
                            //VERSAO
                            if (!String.IsNullOrEmpty("4.0")) //arqTransUnimed.ArqVersao
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.ArqVersao, 3);
                            //DT_GERACAO
                            sbFileBuffer.Append(DateTime.Now.ToString("ddMMyyyy"));
                            //CEI_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCEI))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCEI, 13);

                            sbFileBuffer.AppendLine();

                            #endregion

                            String empresaAtual = Convert.ToString(dtBeneficiarios.Rows[0]["contratoadm_descricao"]);

                            foreach (DataRow row in dtBeneficiarios.Rows)
                            {
                                switch (Mov)
                                {
                                    case Movimentacao.InclusaoBeneficiario:
                                        intQtdeBeneficiarioInclusao++;
                                        break;
                                    case Movimentacao.ExclusaoBeneficiario:
                                        intQtdeBeneficiarioExclusao++;
                                        break;
                                    case Movimentacao.AlteracaoBeneficiario:
                                        intQtdeBeneficiarioAlteracao++;
                                        break;
                                    case Movimentacao.MudancaDePlano:
                                        intQtdeBeneficiarioAlteracao++;
                                        break;
                                }

                                contratoID = row["contratobeneficiario_contratoId"];
                                contrato.ID = contratoID;

                                if (contrato.ID != null)
                                {
                                    contrato.Carregar();
                                    if (contrato.PlanoID != null)
                                    {
                                        plano.ID = contrato.PlanoID;
                                        plano.Carregar();
                                    }
                                }

                                #region preenche variáveis

                                beneficiarioID = row["contratobeneficiario_beneficiarioId"];
                                beneficiarioParentescoCod = (row["contratoAdmparentescoagregado_parentescoCodigo"] == null || row["contratoAdmparentescoagregado_parentescoCodigo"] is DBNull) ? "00" : Convert.ToString(row["contratoAdmparentescoagregado_parentescoCodigo"]);
                                beneficiarioEstadoCivilID = (row["contratobeneficiario_estadoCivilId"] == null || row["contratobeneficiario_estadoCivilId"] is DBNull) ? "0" : Convert.ToString(row["estadocivil_codigo"]);
                                strBeneficiarioNome = (row["beneficiario_nome"] == null || row["beneficiario_nome"] is DBNull) ? null : Convert.ToString(row["beneficiario_nome"]);
                                strBeneficiarioSexo = (row["beneficiario_sexo"] == null || row["beneficiario_sexo"] is DBNull) ? null : Convert.ToString(row["beneficiario_sexo"]);
                                strBeneficiarioCPF = (row["beneficiario_cpf"] == null || row["beneficiario_cpf"] is DBNull) ? "" : Convert.ToString(row["beneficiario_cpf"]);
                                strBeneficiarioCPF = strBeneficiarioCPF.Replace("99999999999", "00000000000");

                                strBeneficiarioTitularCPF = ContratoBeneficiario.GetCPFTitular(contratoID, PM);
                                strBeneficiarioRG = (row["beneficiario_rg"] == null || row["beneficiario_rg"] is DBNull) ? String.Empty : Convert.ToString(row["beneficiario_rg"]);
                                intBeneficiarioTipo = (row["contratobeneficiario_tipo"] == null || row["contratobeneficiario_tipo"] is DBNull) ? Convert.ToInt16(-1) : Convert.ToInt16(row["contratobeneficiario_tipo"]);
                                intBeneficiarioSequencia = (row["contratobeneficiario_numeroSequencia"] == null || row["contratobeneficiario_numeroSequencia"] is DBNull) ? Convert.ToInt16(0) : Convert.ToInt16(row["contratobeneficiario_numeroSequencia"]);
                                dtBeneficiarioDataNascimento = (row["beneficiario_dataNascimento"] == null || row["beneficiario_dataNascimento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["beneficiario_dataNascimento"]);
                                dtBeneficiarioVigencia = (row["contratobeneficiario_vigencia"] == null || row["contratobeneficiario_vigencia"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_vigencia"]);
                                dtBeneficiarioDataCasamento = (row["contratobeneficiario_dataCasamento"] == null || row["contratobeneficiario_dataCasamento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_dataCasamento"]);
                                dtBeneficiarioCadastro = (row["contratobeneficiario_data"] == null || row["contratobeneficiario_data"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_data"]);
                                strBeneficiarioEndereco = (row["endereco_logradouro"] == null || row["endereco_logradouro"] is DBNull) ? null : Convert.ToString(row["endereco_logradouro"]);
                                strBeneficiarioEnderecoNum = (row["endereco_numero"] == null || row["endereco_numero"] is DBNull) ? null : Convert.ToString(row["endereco_numero"]);
                                strBeneficiarioEnderecoCompl = (row["endereco_complemento"] == null || row["endereco_complemento"] is DBNull) ? null : Convert.ToString(row["endereco_complemento"]);
                                strBeneficiarioBairro = (row["endereco_bairro"] == null || row["endereco_bairro"] is DBNull) ? null : Convert.ToString(row["endereco_bairro"]);
                                strBeneficiarioCEP = (row["endereco_cep"] == null || row["endereco_cep"] is DBNull) ? null : Convert.ToString(row["endereco_cep"]);
                                strBeneficiarioCidade = (row["endereco_cidade"] == null || row["endereco_cidade"] is DBNull) ? null : Convert.ToString(row["endereco_cidade"]);
                                strBeneficiarioUF = (row["endereco_uf"] == null || row["endereco_uf"] is DBNull) ? null : Convert.ToString(row["endereco_uf"]);
                                strBeneficiarioTelefone = (row["beneficiario_telefone"] == null || row["beneficiario_telefone"] is DBNull) ? null : Convert.ToString(row["beneficiario_telefone"]);
                                strBeneficiarioTelefoneRamal = (row["beneficiario_ramal"] == null || row["beneficiario_ramal"] is DBNull) ? null : Convert.ToString(row["beneficiario_ramal"]);
                                strBeneficiarioNomeMae = (row["beneficiario_nomeMae"] == null || row["beneficiario_nomeMae"] is DBNull) ? null : Convert.ToString(row["beneficiario_nomeMae"]);
                                strCodigoCarencia = (row["contratobeneficiario_carenciaCodigo"] == null || row["contratobeneficiario_carenciaCodigo"] is DBNull) ? "" : Convert.ToString(row["contratobeneficiario_carenciaCodigo"]).PadLeft(2, '0');
                                strBneficiarioPisPasep = String.Empty;
                                strCodigoPlano = String.Empty;
                                arqTransUnimed.OperadoraNaUnimed = Convert.ToString(row["contratoadm_numero"]).Trim().PadLeft(8, '0');
                                #endregion

                                if (strBeneficiarioTelefone != null)
                                {
                                    strBeneficiarioTelefone = strBeneficiarioTelefone.Replace("-", "");
                                    if (strBeneficiarioTelefone.Length > 8)
                                    {
                                        strBeneficiarioTelefone = strBeneficiarioTelefone.Substring(4).Trim();
                                    }
                                }

                                if (contrato.TipoAcomodacao > -1)
                                    if (contrato.TipoAcomodacao == 0)
                                        strCodigoPlano = (String.IsNullOrEmpty(plano.Codigo)) ? String.Empty : plano.Codigo;
                                    else
                                        strCodigoPlano = (String.IsNullOrEmpty(plano.CodigoParticular)) ? String.Empty : plano.CodigoParticular;

                                // CASO SEJA UMA MUDANÇA DE PLANO, SINALIZA A SEÇÃO PARA UMA ALTERAÇÃO
                                if (Mov.Equals(Movimentacao.MudancaDePlano) && TipoMov.Equals(TipoMovimentacao.Exclusao))
                                    strTipoMovimentacaoAux = TipoMovimentacao.Alteracao;
                                else
                                    strTipoMovimentacaoAux = TipoMov;

                                if (Convert.ToString(row["contratoadm_descricao"]) != empresaAtual)
                                {
                                    empresaAtual = Convert.ToString(row["contratoadm_descricao"]);
                                    arqTransUnimed.OperadoraNaUnimed = Convert.ToString(row["contratoadm_numero"]);

                                    #region Seção U01

                                    arqTransUnimed.OperadoraNaUnimed = Convert.ToString(row["contratoadm_numero"]).Trim().PadLeft(8, '0');

                                    //NR_SEQ
                                    if (intArqNumeroSequencia != 1) { intArqNumeroSequencia++; }
                                    sbFileBuffer.Append(intArqNumeroSequencia.ToString().PadLeft(6, '0'));
                                    //TP_REG
                                    sbFileBuffer.Append(U01);
                                    //CD_UNI
                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCodSingular))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCodSingular, 4);
                                    //CD_EMP
                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                    //NOME_EMP
                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNome))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, empresaAtual/*arqTransUnimed.OperadoraNome*/, 40);
                                    //CNPJ_EMP
                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCNPJ))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCNPJ, 14);
                                    //TP_IDENTIFICA
                                    if (!String.IsNullOrEmpty(arqTransUnimed.TipoIdentificacao.ToString()))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.TipoIdentificacao, 1);
                                    //VERSAO
                                    if (!String.IsNullOrEmpty("4.0")) //arqTransUnimed.ArqVersao
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.ArqVersao, 3);
                                    //DT_GERACAO
                                    sbFileBuffer.Append(DateTime.Now.ToString("ddMMyyyy"));
                                    //CEI_EMP
                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCEI))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCEI, 13);

                                    sbFileBuffer.AppendLine();

                                    #endregion
                                }

                                #region Seção U02

                                intQtdeRegistroU02++;

                                //NR_SEQ
                                sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                //TP_REG
                                sbFileBuffer.Append(U02);
                                //TP_MOV
                                EntityBase.AppendPreparedField(ref sbFileBuffer, strTipoMovimentacaoAux, 1);
                                //CD_EMP
                                if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                //CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF, 11);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                //CPF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (!String.IsNullOrEmpty(strBeneficiarioCPF))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCPF, 11);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                //RG_TRAB : TODO ? Tratar a exceção de alguma maneira
                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                //CD_USU : TODO ? Confirmar se é 0 para mandar, está igual arq exemplo
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                //SQ_USU
                                EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                //DV_USU
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                //NOME_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (!String.IsNullOrEmpty(strBeneficiarioNome))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNome, 70);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
                                //TP_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (intBeneficiarioTipo > 0)
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioTipo.ToString(), 1);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                //CD_SEXO
                                if (!String.IsNullOrEmpty(strBeneficiarioSexo))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, (strBeneficiarioSexo.Equals("1") ? "M" : "F"), 1);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 1);
                                //CD_PARENT
                                EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioParentescoCod.PadLeft(2, '0'), 2);
                                //DT_NASC
                                EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataNascimento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataNascimento.ToString("ddMMyyyy")), 8);
                                //CD_EST_CIVIL
                                if (beneficiarioEstadoCivilID != null)
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioEstadoCivilID.ToString(), 1);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                //DT_CASAMENTO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataCasamento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataCasamento.ToString("ddMMyyyy")), 8);
                                //IDENTIDADE
                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioRG, 20);
                                //EMISSOR
                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 10);

                                ////DECL_NASC_VIVO
                                //if (row["beneficiario_declaracaoNascimentoVivo"] == null || row["beneficiario_declaracaoNascimentoVivo"] == DBNull.Value || Convert.ToString(row["beneficiario_declaracaoNascimentoVivo"]).Trim() == "")
                                //    sbFileBuffer.Append("0".PadLeft(11, '0'));
                                //else
                                //    sbFileBuffer.Append(Convert.ToString(row["beneficiario_declaracaoNascimentoVivo"]).PadLeft(11, '0'));
                                ////CNS
                                //if (row["beneficiario_cns"] == null || row["beneficiario_cns"] == DBNull.Value || Convert.ToString(row["beneficiario_cns"]).Trim() == "")
                                //    sbFileBuffer.Append("0".PadLeft(15, '0'));
                                //else
                                //    sbFileBuffer.Append(Convert.ToString(row["beneficiario_cns"]).PadLeft(15, '0'));

                                //CD_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                //NOME_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                //CD_SUB_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                //CD_CARGO : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                //DT_ADMISSAO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                //DATA EFETIVACAO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                //DT_CADASTRO (VIGENCIA) : TODO ? Tratar exceção em caso de FALSE. (OBRIGATÓRIO)
                                if (dtBeneficiarioCadastro.CompareTo(DateTime.MinValue) > 0)
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                //DT_EXCLUSAO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                ////PORTABILIDADE
                                //sbFileBuffer.Append("N");

                                sbFileBuffer.AppendLine();

                                #endregion

                                if (Mov.Equals(Movimentacao.InclusaoBeneficiario) ||
                                    Mov.Equals(Movimentacao.AlteracaoBeneficiario))
                                {
                                    #region Seção U05

                                    //NR_SEQ
                                    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                    //TP_REG
                                    sbFileBuffer.Append(U05);
                                    //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                    //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
                                    //SQ_USU
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                    //ENDERECO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioEndereco))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEndereco, 45);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 45);

                                    //NR_ENDER : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioEnderecoNum))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEnderecoNum.Trim().PadLeft(5, '0'), 5);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);

                                    //COMPL_ENDER : TODO ? Limitar a 15 o complemento da UNIMED no cadastro de endereço
                                    if (!String.IsNullOrEmpty(strBeneficiarioEnderecoCompl))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEnderecoCompl, 15);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 15);
                                    //BAIRRO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioBairro))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioBairro, 20);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                    //CEP : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioCEP))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCEP, 8);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                    //CIDADE : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioCidade))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCidade, 30);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 30);
                                    //UF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioUF))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioUF, 2);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 2);
                                    //TELEFONE
                                    if (!String.IsNullOrEmpty(strBeneficiarioTelefone))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTelefone, 9);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 9);
                                    //RAMAL
                                    if (!String.IsNullOrEmpty(strBeneficiarioTelefoneRamal))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTelefoneRamal, 6);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 6);
                                    //NOME_MAE : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioNomeMae))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNomeMae, 70);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
                                    //PIS_PASEP : TODO ? Não tem no Cadastro, ele não é enviado nem com caracter em branco mesmo estando com espaço em branco
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBneficiarioPisPasep, 11);
                                    ////CIDADE_RESIDENCIA
                                    //EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCidade, 30);

                                    sbFileBuffer.AppendLine();

                                    #endregion
                                }

                                if (Mov.Equals(Movimentacao.InclusaoBeneficiario) || Mov.Equals(Movimentacao.MudancaDePlano))
                                {
                                    #region Seção U03

                                    Boolean bolHasSubPlano = false;
                                    Boolean bolRepeatOnce = false;

                                    #region Exclusão de Plano Antigo

                                    if (Mov.Equals(Movimentacao.MudancaDePlano)) //&& TipoMov.Equals(TipoMovimentacao.Exclusao))
                                    {
                                        ContratoPlano ContratoPlanoAntigo = ContratoPlano.CarregarPenultimo(contrato.ID, PM);

                                        if (ContratoPlanoAntigo != null)
                                        {
                                            // Carregar o plano antigo, código do plano antigo, acomodação, etc.
                                            if (ContratoPlanoAntigo != null && ContratoPlanoAntigo.ID != null)
                                            {
                                                Plano planoAntigo = new Plano(ContratoPlanoAntigo.PlanoID);
                                                planoAntigo.Carregar();

                                                bolHasSubPlano = !String.IsNullOrEmpty(planoAntigo.SubPlano) || !String.IsNullOrEmpty(planoAntigo.SubPlanoParticular);
                                                bolRepeatOnce = false;

                                                String strCodigoPlanoAntigo = null;

                                                if (ContratoPlanoAntigo.TipoAcomodacao > -1)
                                                    if (ContratoPlanoAntigo.TipoAcomodacao == 0)
                                                        strCodigoPlanoAntigo = (String.IsNullOrEmpty(planoAntigo.Codigo)) ? String.Empty : planoAntigo.Codigo;
                                                    else
                                                        strCodigoPlanoAntigo = (String.IsNullOrEmpty(planoAntigo.CodigoParticular)) ? String.Empty : planoAntigo.CodigoParticular;

                                                do
                                                {
                                                    intQtdeRegistroU03++;

                                                    //NR_SEQ
                                                    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                                    //TP_REG
                                                    sbFileBuffer.Append(U03);
                                                    //TP_MOV
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.Exclusao, 1);
                                                    //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);


                                                    //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);

                                                    //SQ_USU
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                                    //CD_PLANO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(strCodigoPlanoAntigo))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoPlanoAntigo.PadLeft(10, '0'), 10);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                                    //DT_INICIO
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, ContratoPlanoAntigo.Data.ToString("ddMMyyyy"), 8);//EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
                                                    //DT_FIM
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, DateTime.Now.ToString("ddMMyyyy"), 8);
                                                    //ITEM_REDUCAO : TODO ? Saber da onde vem essa informação
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoCarencia, 2);

                                                    sbFileBuffer.AppendLine();

                                                    if (bolHasSubPlano)
                                                    {
                                                        if (ContratoPlanoAntigo.TipoAcomodacao == 0)
                                                            strCodigoPlanoAntigo = planoAntigo.SubPlano;
                                                        else
                                                            strCodigoPlanoAntigo = planoAntigo.SubPlanoParticular;

                                                        bolRepeatOnce = !bolRepeatOnce;
                                                    }

                                                } while (bolHasSubPlano && bolRepeatOnce);
                                            }
                                        }

                                        ContratoPlanoAntigo = null;
                                    }

                                    #endregion

                                    #region Inclusão de Plano Novo

                                    bolHasSubPlano = !String.IsNullOrEmpty(plano.SubPlano) || !String.IsNullOrEmpty(plano.SubPlanoParticular);
                                    bolRepeatOnce = false;

                                    ContratoPlano ContratoPlanoAtual = ContratoPlano.CarregarAtual(contrato.ID, PM);
                                    if (ContratoPlanoAtual == null) { ContratoPlanoAtual = new ContratoPlano(); }

                                    do
                                    {
                                        intQtdeRegistroU03++;

                                        //NR_SEQ
                                        sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                        //TP_REG
                                        sbFileBuffer.Append(U03);
                                        //TP_MOV
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.Inclusao, 1);
                                        //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                        //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);

                                        //SQ_USU
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                        //CD_PLANO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(strCodigoPlano))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoPlano.PadLeft(10, '0'), 10);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                        //DT_INICIO
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, ContratoPlanoAtual.Data.ToString("ddMMyyyy"), 8); //EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
                                        //DT_FIM
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                        //ITEM_REDUCAO : TODO ? Saber de onde vem essa informação
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoCarencia, 2);

                                        sbFileBuffer.AppendLine();

                                        if (bolHasSubPlano)
                                        {
                                            if (contrato.TipoAcomodacao == 0)
                                                strCodigoPlano = plano.SubPlano;
                                            else
                                                strCodigoPlano = plano.SubPlanoParticular;

                                            bolRepeatOnce = !bolRepeatOnce;
                                        }

                                    } while (bolHasSubPlano && bolRepeatOnce);

                                    #endregion

                                    #endregion

                                    #region Caso seja uma MUDANÇA DE PLANO, devemos EMITIR uma segunda via de Cartão.
                                    if (Mov.Equals(Movimentacao.MudancaDePlano))
                                        //{
                                        //    #region Seção U02 Alteração de Cartão

                                        //    intQtdeRegistroU02++;

                                        //    //NR_SEQ
                                        //    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                        //    //TP_REG
                                        //    sbFileBuffer.Append(U02);
                                        //    //TP_MOV
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.EmissaoSegundaVia, 1);
                                        //    //CD_EMP
                                        //    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                        //    //CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        //    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF, 11);
                                        //    else
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                        //    //CPF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        //    if (!String.IsNullOrEmpty(strBeneficiarioCPF))
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCPF, 11);
                                        //    else
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                        //    //RG_TRAB : TODO ? Tratar a exceção de alguma maneira
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                        //    //CD_USU : TODO ? Confirmar se é 0 para mandar, está igual arq exemplo
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                        //    //SQ_USU
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                        //    //DV_USU
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                        //    //NOME_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        //    if (!String.IsNullOrEmpty(strBeneficiarioNome))
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNome, 70);
                                        //    else
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
                                        //    //TP_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        //    if (intBeneficiarioTipo > 0)
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioTipo.ToString(), 1);
                                        //    else
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                        //    //CD_SEXO
                                        //    if (!String.IsNullOrEmpty(strBeneficiarioSexo))
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, (strBeneficiarioSexo.Equals("1") ? "M" : "F"), 1);
                                        //    else
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 1);
                                        //    //CD_PARENT
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioParentescoCod.PadLeft(2, '0'), 2);
                                        //    //DT_NASC
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataNascimento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataNascimento.ToString("ddMMyyyy")), 8);
                                        //    //CD_EST_CIVIL
                                        //    if (beneficiarioEstadoCivilID != null)
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioEstadoCivilID.ToString(), 1);
                                        //    else
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                        //    //DT_CASAMENTO
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataCasamento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataCasamento.ToString("ddMMyyyy")), 8);
                                        //    //IDENTIDADE
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioRG, 20);
                                        //    //EMISSOR
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 10);
                                        //    //CD_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                        //    //NOME_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                        //    //CD_SUB_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                        //    //CD_CARGO : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                        //    //DT_ADMISSAO
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                        //    //DT_CADASTRO : TODO ? Tratar exceção em caso de FALSE. (OBRIGATÓRIO)
                                        //    if (dtBeneficiarioCadastro.CompareTo(DateTime.MinValue) > 0)
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioCadastro.ToString("ddMMyyyy"), 8);
                                        //    else
                                        //        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                        //    //DT_EXCLUSAO
                                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                        //    sbFileBuffer.AppendLine();

                                        //    #endregion
                                        //}
                                    #endregion

                                        if (Mov.Equals(Movimentacao.InclusaoBeneficiario))
                                        {
                                            #region Seção U07 / U08

                                            IList<ItemDeclaracaoSaudeINSTANCIA> lstDeclaracaoSaude = ItemDeclaracaoSaudeINSTANCIA.Carregar(beneficiarioID, OperadoraID);

                                            if (lstDeclaracaoSaude != null && lstDeclaracaoSaude.Count > 0)
                                            {
                                                ItemDeclaracaoSaude itemDeclaracao = new ItemDeclaracaoSaude();

                                                foreach (ItemDeclaracaoSaudeINSTANCIA itemDeclaracaoInstancia in lstDeclaracaoSaude)
                                                {
                                                    if (itemDeclaracaoInstancia.Sim)
                                                    {
                                                        intQtdeRegistroU07U08++;

                                                        itemDeclaracao.ID = itemDeclaracaoInstancia.ItemDeclaracaoID;
                                                        itemDeclaracao.Carregar();

                                                        #region Seção U07

                                                        //NR_SEQ
                                                        sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                                        //TP_REG
                                                        sbFileBuffer.Append(U07);
                                                        //TP_MOV
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMov, 1);
                                                        //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                                        if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                                        //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                        if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
                                                        //SQ_USU
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                                        //CD_QUESTAO
                                                        if (!String.IsNullOrEmpty(itemDeclaracao.Codigo))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracao.Codigo.PadLeft(3, '0'), 3);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "000", 3);
                                                        //RESPOSTA
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "S", 1);
                                                        //DT_EVENTO
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.Data.ToString("ddMMyyyy"), 8);
                                                        //ESPECIFICACAO
                                                        if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.Descricao))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.Descricao, 400);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 400);

                                                        sbFileBuffer.AppendLine();

                                                        #endregion

                                                        #region Seção U08

                                                        //NR_SEQ
                                                        sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                                        //TP_REG
                                                        sbFileBuffer.Append(U08);
                                                        //TP_MOV
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMov, 1);
                                                        //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                                        if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                                        //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                        if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
                                                        //SQ_USU
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                                        //CD_CID_INICIAL
                                                        if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.CIDInicial))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.CIDInicial, 4);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 4);
                                                        //CD_CID_FINAL
                                                        if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.CIDFinal))
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.CIDFinal, 4);
                                                        else
                                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 4);

                                                        sbFileBuffer.AppendLine();

                                                        #endregion
                                                    }
                                                }

                                                itemDeclaracao = null;
                                            }

                                            lstDeclaracaoSaude = null;

                                            #endregion
                                        }
                                }

                                #region Build Lote

                                itemLote = new ArqTransacionalLoteItem();

                                itemLote.ContratoID = contrato.ID;
                                itemLote.BeneficiarioID = beneficiarioID;
                                itemLote.BeneficiarioSequencia = intBeneficiarioSequencia;
                                itemLote.Ativo = true;

                                lote.Itens.Add(itemLote);

                                #endregion
                            }

                            #region Seção U99

                            //NR_SEQ
                            sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                            //TP_REG
                            sbFileBuffer.Append(U99);
                            //QD_U02
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU02.ToString().PadLeft(6, '0'), 6);
                            //QD_U03
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU03.ToString().PadLeft(6, '0'), 6);
                            //QD_U04
                            EntityBase.AppendPreparedField(ref sbFileBuffer, "000000", 6);
                            //QD_U07
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU07U08.ToString().PadLeft(6, '0'), 6);
                            //QD_U08
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU07U08.ToString().PadLeft(6, '0'), 6);
                            ////QD_U10
                            //sbFileBuffer.Append("000000");
                            //QD_INC
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioInclusao.ToString().PadLeft(6, '0'), 6);
                            //QD_EXC
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioExclusao.ToString().PadLeft(6, '0'), 6);
                            //QD_USU
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioAlteracao.ToString().PadLeft(6, '0'), 6);

                            #endregion

                            if (intQtdeBeneficiarioInclusao > 0)
                                lote.Quantidade = intQtdeBeneficiarioInclusao;
                            else if (intQtdeBeneficiarioAlteracao > 0)
                                lote.Quantidade = intQtdeBeneficiarioAlteracao;
                            else if (intQtdeBeneficiarioExclusao > 0)
                                lote.Quantidade = intQtdeBeneficiarioExclusao;

                            if (lote.Itens != null && lote.Itens.Count > 0)
                            {
                                try
                                {
                                    lote.Salvar(true, PM);
                                }
                                catch (Exception) { throw; }

                                String strArquivoNome = lote.Arquivo;

                                try
                                {
                                    this.SalvarArqTransacional(sbFileBuffer, strArquivoNome);
                                }
                                catch (Exception) { throw; }

                                criacaoOK = true;
                                ArquivoNome = strArquivoNome;

                                PM.Commit();
                            }
                            else
                            {
                                PM.Rollback();
                            }
                        }
                        else
                        {
                            PM.Rollback();
                        }
                    }
                }
                catch (Exception)
                {
                    PM.Rollback();
                    throw;
                }
                finally
                {
                    PM.Dispose();
                    PM = null;
                }

                sbFileBuffer = null;
            }

            arqTransUnimed = null;

            return criacaoOK;
        }

        /// <summary>
        /// Método para Gerar Arquivos
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        /// <param name="Mov">Inclusão de Beneficiário, Mudança de Plano, etc.</param>
        /// <param name="TipoMovimentacao">Inclusão, Alteração, Exclusão, etc.</param>
        /// <returns>True se gerou sem problemas, False se encontrou algum problema.</returns>
        private Boolean GerarArquivo(Object OperadoraID, ref String ArquivoNome, ContratoBeneficiario.eStatus Status, Object[] ContratoID, Object[] BeneficiarioID, String Mov, String TipoMov, DateTime vigencia)
        {
            Boolean criacaoOK = false;

            ArqTransacionalUnimedConf arqTransUnimed = new ArqTransacionalUnimedConf();
            arqTransUnimed.CarregarPorOperadora(OperadoraID);

            if (arqTransUnimed.ID != null)
            {
                Int32 intArqNumeroSequencia        = 1;
                Int32 intQtdeBeneficiarioInclusao  = 0;
                Int32 intQtdeBeneficiarioExclusao  = 0;
                Int32 intQtdeBeneficiarioAlteracao = 0;
                Int32 intQtdeRegistroU02           = 0;
                Int32 intQtdeRegistroU03           = 0;
                Int32 intQtdeRegistroU07U08        = 0;

                ArqTransacionalLote lote   = new ArqTransacionalLote();
                StringBuilder sbFileBuffer = new StringBuilder();

                PersistenceManager PM = new PersistenceManager();
                PM = new PersistenceManager();
                PM.BeginTransactionContext();

                try
                {

                    if ((ContratoID != null && ContratoID.Length > 0) &&
                        (BeneficiarioID != null && BeneficiarioID.Length > 0) &&
                        (ContratoID.Length == BeneficiarioID.Length))
                    {
                        for (Int32 i = 0; i < ContratoID.Length; i++)
                        {
                            try
                            {
                                ContratoBeneficiario.AlteraStatusBeneficiario(ContratoID[i], BeneficiarioID[i], Status, PM);
                            }
                            catch (Exception) { throw; }
                        }
                    }

                    //using (DataTable dtBeneficiarios = this.GetBeneficiarioPorStatus(OperadoraID, Status, ContratoID, BeneficiarioID, PM, vigencia, ContratoID[0]))
                    //using (DataTable dtBeneficiarios = LocatorHelper.Instance.ExecuteQuery("select contrato_admissao, contrato_numeroMatricula, contratobeneficiario_carenciaCodigo, contrato_vigencia, contratoadm_numero, contratoadm_id, item_lote_id, lote_data_criacao, c.contrato_numero, cBen.contratobeneficiario_id, cBen.contratobeneficiario_vigencia, cBen.contratobeneficiario_contratoId, cBen.contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo, contratobeneficiario_numeroSequencia, cBen.contratobeneficiario_portabilidade, cBen.contratobeneficiario_data, cBen.contratobeneficiario_estadoCivilId, estadocivil_codigo, beneficiario_nome, beneficiario_sexo,beneficiario_cpf, beneficiario_rg, beneficiario_rgUF, beneficiario_rgOrgaoExp, beneficiario_declaracaoNascimentoVivo, beneficiario_cns, cBen.contratobeneficiario_dataCasamento, beneficiario_nomeMae, beneficiario_dataNascimento,beneficiario_telefone, beneficiario_ramal, beneficiario_telefone2, beneficiario_celular, beneficiario_email, endereco_logradouro, endereco_numero, endereco_complemento,endereco_bairro, endereco_cidade, endereco_uf, endereco_cep FROM contrato_beneficiario cBen INNER JOIN contrato c ON cBen.contratobeneficiario_contratoId = c.contrato_id and cben.contratobeneficiario_tipo=0 INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id and cben.contratobeneficiario_tipo=0 INNER JOIN endereco ende ON c.contrato_enderecoReferenciaId = ende.endereco_id INNER JOIN contratoADM on contratoadm_id=contrato_contratoAdmId INNER JOIN estado_civil ON estadocivil_id=cBen.contratobeneficiario_estadocivilId LEFT JOIN contratoAdm_parentesco_agregado ON contratoAdmparentescoagregado_id=cBen.contratobeneficiario_parentescoId LEFT JOIN arquivo_transacional_lote_item ON item_contrato_id=c.contrato_id AND item_beneficiario_id=Ben.beneficiario_id AND item_ativo=1 LEFT JOIN arquivo_transacional_lote ON lote_id=item_lote_id AND lote_exportacao <> 1  where contrato_id in (select contrato_id from __excluir inner join contrato on contrato_numero=número and contrato_operadoraId=3) and contrato_contratoadmid=" + ContratoID[0] + " order by beneficiario_nome", "resultset").Tables[0])
                    //using (DataTable dtBeneficiarios = LocatorHelper.Instance.ExecuteQuery("select contrato_admissao, contrato_numeroMatricula, contratobeneficiario_carenciaCodigo, contrato_vigencia, contratoadm_numero, contratoadm_id, c.contrato_numero, cBen.contratobeneficiario_id, cBen.contratobeneficiario_vigencia, cBen.contratobeneficiario_contratoId, cBen.contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo, contratobeneficiario_numeroSequencia, cBen.contratobeneficiario_portabilidade, cBen.contratobeneficiario_data, cBen.contratobeneficiario_estadoCivilId, estadocivil_codigo, beneficiario_nome, beneficiario_sexo,beneficiario_cpf, beneficiario_rg, beneficiario_rgUF, beneficiario_rgOrgaoExp, beneficiario_declaracaoNascimentoVivo, beneficiario_cns, cBen.contratobeneficiario_dataCasamento, beneficiario_nomeMae, beneficiario_dataNascimento,beneficiario_telefone, beneficiario_ramal, beneficiario_telefone2, beneficiario_celular, beneficiario_email, endereco_logradouro, endereco_numero, endereco_complemento,endereco_bairro, endereco_cidade, endereco_uf, endereco_cep FROM contrato_beneficiario cBen INNER JOIN contrato c ON cBen.contratobeneficiario_contratoId = c.contrato_id and cben.contratobeneficiario_tipo=0 INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id and cben.contratobeneficiario_tipo=0 INNER JOIN endereco ende ON c.contrato_enderecoReferenciaId = ende.endereco_id INNER JOIN contratoADM on contratoadm_id=contrato_contratoAdmId INNER JOIN estado_civil ON estadocivil_id=cBen.contratobeneficiario_estadocivilId LEFT JOIN contratoAdm_parentesco_agregado ON contratoAdmparentescoagregado_id=cBen.contratobeneficiario_parentescoId where beneficiario_nome='TAYNAH BRUNO FANTI' order by beneficiario_nome", "resultset").Tables[0])
                    using (DataTable dtBeneficiarios = this.GetBeneficiarioPorStatus(OperadoraID, Status, ContratoID, BeneficiarioID, PM, vigencia, ContratoID[0]))
                    {
                        if (dtBeneficiarios != null && dtBeneficiarios.Rows != null && dtBeneficiarios.Rows.Count > 0)
                        {
                            ArqTransacionalLoteItem itemLote = null;
                            Contrato contrato = new Contrato();
                            Plano plano = new Plano();
                            Object contratoID, beneficiarioID, beneficiarioEstadoCivilID;
                            String beneficiarioParentescoCod, strBeneficiarioNome, strBeneficiarioSexo, strBeneficiarioCPF, strBeneficiarioTitularCPF, strCodigoCarencia,
                                   strBeneficiarioRG, strBeneficiarioEndereco, strBeneficiarioEnderecoNum, strBeneficiarioEnderecoCompl,
                                   strBeneficiarioBairro, strBeneficiarioCEP, strBeneficiarioCidade, strBeneficiarioUF, strBeneficiarioTelefone,
                                   strBeneficiarioTelefoneRamal, strBeneficiarioNomeMae, strBneficiarioPisPasep, strCodigoPlano, strTipoMovimentacaoAux;
                            Int16 intBeneficiarioSequencia, intBeneficiarioTipo;
                            DateTime dtBeneficiarioDataNascimento, dtBeneficiarioDataCasamento, dtBeneficiarioVigencia, dtBeneficiarioCadastro, vigenciaProposta;

                            lote.OperadoraID = OperadoraID;
                            lote.Movimentacao = Mov;
                            lote.TipoMovimentacao = TipoMov;

                            #region Seção U01

                            if (dtBeneficiarios.Rows.Count > 0)
                            {
                                arqTransUnimed.OperadoraNaUnimed = Convert.ToString(dtBeneficiarios.Rows[0]["contratoadm_numero"]).Trim().PadLeft(8, '0');
                            }

                            //NR_SEQ
                            sbFileBuffer.Append(intArqNumeroSequencia.ToString().PadLeft(6, '0'));
                            //TP_REG
                            sbFileBuffer.Append(U01);
                            //CD_UNI
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCodSingular))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCodSingular, 4);
                            //CD_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                            //NOME_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNome))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNome, 40);
                            //CNPJ_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCNPJ))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCNPJ, 14);
                            //TP_IDENTIFICA
                            if (!String.IsNullOrEmpty(arqTransUnimed.TipoIdentificacao.ToString()))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.TipoIdentificacao, 1);
                            //VERSAO
                            if (!String.IsNullOrEmpty("4.0")) //arqTransUnimed.ArqVersao
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.ArqVersao, 3);
                            //DT_GERACAO
                            sbFileBuffer.Append(DateTime.Now.ToString("ddMMyyyy"));
                            //CEI_EMP
                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCEI))
                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCEI, 13);

                            sbFileBuffer.AppendLine();

                            #endregion

                            foreach (DataRow row in dtBeneficiarios.Rows)
                            {
                                switch (Mov)
                                {
                                    case Movimentacao.InclusaoBeneficiario:
                                        intQtdeBeneficiarioInclusao++;
                                        break;
                                    case Movimentacao.ExclusaoBeneficiario:
                                        intQtdeBeneficiarioExclusao++;
                                        break;
                                    case Movimentacao.AlteracaoBeneficiario:
                                        intQtdeBeneficiarioAlteracao++;
                                        break;
                                    case Movimentacao.MudancaDePlano:
                                        intQtdeBeneficiarioAlteracao++;
                                        break;
                                }

                                contratoID = row["contratobeneficiario_contratoId"];
                                contrato.ID = contratoID;

                                if (contrato.ID != null)
                                {
                                    contrato.Carregar();
                                    if (contrato.PlanoID != null)
                                    {
                                        plano.ID = contrato.PlanoID;
                                        plano.Carregar();
                                    }
                                }

                                #region preenche variáveis 

                                beneficiarioID = row["contratobeneficiario_beneficiarioId"];
                                beneficiarioParentescoCod = (row["contratoAdmparentescoagregado_parentescoCodigo"] == null || row["contratoAdmparentescoagregado_parentescoCodigo"] is DBNull) ? "00" : Convert.ToString(row["contratoAdmparentescoagregado_parentescoCodigo"]);
                                beneficiarioEstadoCivilID = (row["contratobeneficiario_estadoCivilId"] == null || row["contratobeneficiario_estadoCivilId"] is DBNull) ? "0" : Convert.ToString(row["estadocivil_codigo"]);
                                strBeneficiarioNome = (row["beneficiario_nome"] == null || row["beneficiario_nome"] is DBNull) ? null : Convert.ToString(row["beneficiario_nome"]);
                                strBeneficiarioSexo = (row["beneficiario_sexo"] == null || row["beneficiario_sexo"] is DBNull) ? null : Convert.ToString(row["beneficiario_sexo"]);
                                strBeneficiarioCPF = (row["beneficiario_cpf"] == null || row["beneficiario_cpf"] is DBNull) ? "" : Convert.ToString(row["beneficiario_cpf"]);
                                strBeneficiarioCPF = strBeneficiarioCPF.Replace("99999999999", "00000000000");

                                strBeneficiarioTitularCPF = ContratoBeneficiario.GetCPFTitular(contratoID, PM);
                                strBeneficiarioRG = (row["beneficiario_rg"] == null || row["beneficiario_rg"] is DBNull) ? String.Empty : Convert.ToString(row["beneficiario_rg"]);
                                intBeneficiarioTipo = (row["contratobeneficiario_tipo"] == null || row["contratobeneficiario_tipo"] is DBNull) ? Convert.ToInt16(-1) : Convert.ToInt16(row["contratobeneficiario_tipo"]);
                                intBeneficiarioSequencia = (row["contratobeneficiario_numeroSequencia"] == null || row["contratobeneficiario_numeroSequencia"] is DBNull) ? Convert.ToInt16(0) : Convert.ToInt16(row["contratobeneficiario_numeroSequencia"]);
                                dtBeneficiarioDataNascimento = (row["beneficiario_dataNascimento"] == null || row["beneficiario_dataNascimento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["beneficiario_dataNascimento"]);
                                dtBeneficiarioVigencia = (row["contratobeneficiario_vigencia"] == null || row["contratobeneficiario_vigencia"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_vigencia"]);
                                dtBeneficiarioDataCasamento = (row["contratobeneficiario_dataCasamento"] == null || row["contratobeneficiario_dataCasamento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_dataCasamento"]);
                                dtBeneficiarioCadastro = (row["contratobeneficiario_data"] == null || row["contratobeneficiario_data"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_data"]);
                                strBeneficiarioEndereco = (row["endereco_logradouro"] == null || row["endereco_logradouro"] is DBNull) ? null : Convert.ToString(row["endereco_logradouro"]);
                                strBeneficiarioEnderecoNum = (row["endereco_numero"] == null || row["endereco_numero"] is DBNull) ? null : Convert.ToString(row["endereco_numero"]);
                                strBeneficiarioEnderecoCompl = (row["endereco_complemento"] == null || row["endereco_complemento"] is DBNull) ? null : Convert.ToString(row["endereco_complemento"]);
                                strBeneficiarioBairro = (row["endereco_bairro"] == null || row["endereco_bairro"] is DBNull) ? null : Convert.ToString(row["endereco_bairro"]);
                                strBeneficiarioCEP = (row["endereco_cep"] == null || row["endereco_cep"] is DBNull) ? null : Convert.ToString(row["endereco_cep"]);
                                strBeneficiarioCidade = (row["endereco_cidade"] == null || row["endereco_cidade"] is DBNull) ? null : Convert.ToString(row["endereco_cidade"]);
                                strBeneficiarioUF = (row["endereco_uf"] == null || row["endereco_uf"] is DBNull) ? null : Convert.ToString(row["endereco_uf"]);
                                strBeneficiarioTelefone = (row["beneficiario_telefone"] == null || row["beneficiario_telefone"] is DBNull) ? null : Convert.ToString(row["beneficiario_telefone"]);
                                strBeneficiarioTelefoneRamal = (row["beneficiario_ramal"] == null || row["beneficiario_ramal"] is DBNull) ? null : Convert.ToString(row["beneficiario_ramal"]);
                                strBeneficiarioNomeMae = (row["beneficiario_nomeMae"] == null || row["beneficiario_nomeMae"] is DBNull) ? null : Convert.ToString(row["beneficiario_nomeMae"]);
                                strCodigoCarencia = (row["contratobeneficiario_carenciaCodigo"] == null || row["contratobeneficiario_carenciaCodigo"] is DBNull) ? "" : Convert.ToString(row["contratobeneficiario_carenciaCodigo"]).PadLeft(2, '0'); 
                                strBneficiarioPisPasep = String.Empty;
                                strCodigoPlano = String.Empty;
                                arqTransUnimed.OperadoraNaUnimed = Convert.ToString(row["contratoadm_numero"]).Trim().PadLeft(8, '0');
                                #endregion

                                if (strBeneficiarioTelefone != null)
                                {
                                    strBeneficiarioTelefone = strBeneficiarioTelefone.Replace("-", "");
                                    if (strBeneficiarioTelefone.Length > 8)
                                    {
                                        strBeneficiarioTelefone = strBeneficiarioTelefone.Substring(4).Trim();
                                    }
                                }

                                if (contrato.TipoAcomodacao > -1)
                                    if (contrato.TipoAcomodacao == 0)
                                        strCodigoPlano = (String.IsNullOrEmpty(plano.Codigo)) ? String.Empty : plano.Codigo;
                                    else
                                        strCodigoPlano = (String.IsNullOrEmpty(plano.CodigoParticular)) ? String.Empty : plano.CodigoParticular;

                                // CASO SEJA UMA MUDANÇA DE PLANO, SINALIZA A SEÇÃO PARA UMA ALTERAÇÃO
                                if (Mov.Equals(Movimentacao.MudancaDePlano) && TipoMov.Equals(TipoMovimentacao.Exclusao))
                                    strTipoMovimentacaoAux = TipoMovimentacao.Alteracao;
                                else
                                    strTipoMovimentacaoAux = TipoMov;

                                #region Seção U02

                                intQtdeRegistroU02++;

                                //NR_SEQ
                                sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                //TP_REG
                                sbFileBuffer.Append(U02);
                                //TP_MOV
                                EntityBase.AppendPreparedField(ref sbFileBuffer, strTipoMovimentacaoAux, 1);
                                //CD_EMP
                                if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                //CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF, 11);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                //CPF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (!String.IsNullOrEmpty(strBeneficiarioCPF))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCPF, 11);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                //RG_TRAB : TODO ? Tratar a exceção de alguma maneira
                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                //CD_USU : TODO ? Confirmar se é 0 para mandar, está igual arq exemplo
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                //SQ_USU
                                EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                //DV_USU
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                //NOME_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (!String.IsNullOrEmpty(strBeneficiarioNome))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNome, 70);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
                                //TP_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                if (intBeneficiarioTipo > 0)
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioTipo.ToString(), 1);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                //CD_SEXO
                                if (!String.IsNullOrEmpty(strBeneficiarioSexo))
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, (strBeneficiarioSexo.Equals("1") ? "M" : "F"), 1);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 1);
                                //CD_PARENT
                                EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioParentescoCod.PadLeft(2, '0'), 2);
                                //DT_NASC
                                EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataNascimento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataNascimento.ToString("ddMMyyyy")), 8);
                                //CD_EST_CIVIL
                                if (beneficiarioEstadoCivilID != null)
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioEstadoCivilID.ToString(), 1);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                //DT_CASAMENTO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataCasamento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataCasamento.ToString("ddMMyyyy")), 8);
                                //IDENTIDADE
                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioRG, 20);
                                //EMISSOR
                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 10);

                                ////DECL_NASC_VIVO
                                //if (row["beneficiario_declaracaoNascimentoVivo"] == null || row["beneficiario_declaracaoNascimentoVivo"] == DBNull.Value || Convert.ToString(row["beneficiario_declaracaoNascimentoVivo"]).Trim() == "")
                                //    sbFileBuffer.Append("0".PadLeft(11, '0'));
                                //else
                                //    sbFileBuffer.Append(Convert.ToString(row["beneficiario_declaracaoNascimentoVivo"]).PadLeft(11, '0'));
                                ////CNS
                                //if (row["beneficiario_cns"] == null || row["beneficiario_cns"] == DBNull.Value || Convert.ToString(row["beneficiario_cns"]).Trim() == "")
                                //    sbFileBuffer.Append("0".PadLeft(15, '0'));
                                //else
                                //    sbFileBuffer.Append(Convert.ToString(row["beneficiario_cns"]).PadLeft(15, '0'));

                                //CD_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                //NOME_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                //CD_SUB_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                //CD_CARGO : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                //DT_ADMISSAO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                //DATA EFETIVACAO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                //DT_CADASTRO (VIGENCIA) : TODO ? Tratar exceção em caso de FALSE. (OBRIGATÓRIO)
                                if (dtBeneficiarioCadastro.CompareTo(DateTime.MinValue) > 0)
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
                                else
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                //DT_EXCLUSAO
                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                ////PORTABILIDADE
                                //sbFileBuffer.Append("N");

                                sbFileBuffer.AppendLine();

                                #endregion

                                if (Mov.Equals(Movimentacao.InclusaoBeneficiario) ||
                                    Mov.Equals(Movimentacao.AlteracaoBeneficiario))
                                {
                                    #region Seção U05

                                    //NR_SEQ
                                    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                    //TP_REG
                                    sbFileBuffer.Append(U05);
                                    //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                    //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
                                    //SQ_USU
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                    //ENDERECO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioEndereco))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEndereco, 45);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 45);

                                    //NR_ENDER : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioEnderecoNum))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEnderecoNum.Trim().PadLeft(5, '0'), 5);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);

                                    //COMPL_ENDER : TODO ? Limitar a 15 o complemento da UNIMED no cadastro de endereço
                                    if (!String.IsNullOrEmpty(strBeneficiarioEnderecoCompl))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEnderecoCompl, 15);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 15);
                                    //BAIRRO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioBairro))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioBairro, 20);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                    //CEP : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioCEP))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCEP, 8);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                    //CIDADE : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioCidade))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCidade, 30);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 30);
                                    //UF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioUF))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioUF, 2);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 2);
                                    //TELEFONE
                                    if (!String.IsNullOrEmpty(strBeneficiarioTelefone))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTelefone, 9);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 9);
                                    //RAMAL
                                    if (!String.IsNullOrEmpty(strBeneficiarioTelefoneRamal))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTelefoneRamal, 6);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 6);
                                    //NOME_MAE : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                    if (!String.IsNullOrEmpty(strBeneficiarioNomeMae))
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNomeMae, 70);
                                    else
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
                                    //PIS_PASEP : TODO ? Não tem no Cadastro, ele não é enviado nem com caracter em branco mesmo estando com espaço em branco
                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBneficiarioPisPasep, 11);
                                    ////CIDADE_RESIDENCIA
                                    //EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCidade, 30);

                                    sbFileBuffer.AppendLine();

                                    #endregion
                                }

                                if (Mov.Equals(Movimentacao.InclusaoBeneficiario) || Mov.Equals(Movimentacao.MudancaDePlano))
                                {
                                    #region Seção U03

                                    Boolean bolHasSubPlano = false;
                                    Boolean bolRepeatOnce = false;

                                    #region Exclusão de Plano Antigo

                                    if (Mov.Equals(Movimentacao.MudancaDePlano) && TipoMov.Equals(TipoMovimentacao.Exclusao))
                                    {
                                        ContratoPlano ContratoPlanoAntigo = ContratoPlano.CarregarUltimo(contrato.ID);

                                        if (ContratoPlanoAntigo != null)
                                        {
                                            // Carregar o plano antigo, código do plano antigo, acomodação, etc.
                                            if (ContratoPlanoAntigo != null && ContratoPlanoAntigo.ID != null)
                                            {
                                                Plano planoAntigo = new Plano(ContratoPlanoAntigo.PlanoID);
                                                planoAntigo.Carregar();

                                                bolHasSubPlano = !String.IsNullOrEmpty(planoAntigo.SubPlano) || !String.IsNullOrEmpty(planoAntigo.SubPlanoParticular);
                                                bolRepeatOnce = false;

                                                String strCodigoPlanoAntigo = null;

                                                if (ContratoPlanoAntigo.TipoAcomodacao > -1)
                                                    if (ContratoPlanoAntigo.TipoAcomodacao == 0)
                                                        strCodigoPlanoAntigo = (String.IsNullOrEmpty(planoAntigo.Codigo)) ? String.Empty : planoAntigo.Codigo;
                                                    else
                                                        strCodigoPlanoAntigo = (String.IsNullOrEmpty(planoAntigo.CodigoParticular)) ? String.Empty : planoAntigo.CodigoParticular;

                                                do
                                                {
                                                    intQtdeRegistroU03++;

                                                    //NR_SEQ
                                                    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                                    //TP_REG
                                                    sbFileBuffer.Append(U03);
                                                    //TP_MOV
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.Exclusao, 1);
                                                    //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);


                                                    //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);

                                                    //SQ_USU
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                                    //CD_PLANO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(strCodigoPlanoAntigo))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoPlanoAntigo.PadLeft(10, '0'), 10);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                                    //DT_INICIO
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
                                                    //DT_FIM
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, DateTime.Now.ToString("ddMMyyyy"), 8);
                                                    //ITEM_REDUCAO : TODO ? Saber da onde vem essa informação
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoCarencia, 2);

                                                    sbFileBuffer.AppendLine();

                                                    if (bolHasSubPlano)
                                                    {
                                                        if (ContratoPlanoAntigo.TipoAcomodacao == 0)
                                                            strCodigoPlanoAntigo = planoAntigo.SubPlano;
                                                        else
                                                            strCodigoPlanoAntigo = planoAntigo.SubPlanoParticular;

                                                        bolRepeatOnce = !bolRepeatOnce;
                                                    }

                                                } while (bolHasSubPlano && bolRepeatOnce);
                                            }
                                        }

                                        ContratoPlanoAntigo = null;
                                    }

                                    #endregion

                                    #region Inclusão de Plano Novo

                                    bolHasSubPlano = !String.IsNullOrEmpty(plano.SubPlano) || !String.IsNullOrEmpty(plano.SubPlanoParticular);
                                    bolRepeatOnce = false;

                                    do
                                    {
                                        intQtdeRegistroU03++;

                                        //NR_SEQ
                                        sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                        //TP_REG
                                        sbFileBuffer.Append(U03);
                                        //TP_MOV
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.Inclusao, 1);
                                        //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                        //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);

                                        //SQ_USU
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                        //CD_PLANO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(strCodigoPlano))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoPlano.PadLeft(10, '0'), 10);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                        //DT_INICIO
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
                                        //DT_FIM
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                        //ITEM_REDUCAO : TODO ? Saber da onde vem essa informação
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoCarencia, 2);

                                        sbFileBuffer.AppendLine();

                                        if (bolHasSubPlano)
                                        {
                                            if (contrato.TipoAcomodacao == 0)
                                                strCodigoPlano = plano.SubPlano;
                                            else
                                                strCodigoPlano = plano.SubPlanoParticular;

                                            bolRepeatOnce = !bolRepeatOnce;
                                        }

                                    } while (bolHasSubPlano && bolRepeatOnce);

                                    #endregion

                                    #endregion

                                    // Caso seja uma MUDANÇA DE PLANO, devemos EMITIR uma segunda via de Cartão.
                                    if (Mov.Equals(Movimentacao.MudancaDePlano))
                                    {
                                        #region Seção U02 Alteração de Cartão

                                        intQtdeRegistroU02++;

                                        //NR_SEQ
                                        sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                        //TP_REG
                                        sbFileBuffer.Append(U02);
                                        //TP_MOV
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.EmissaoSegundaVia, 1);
                                        //CD_EMP
                                        if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                        //CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF, 11);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                        //CPF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(strBeneficiarioCPF))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCPF, 11);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
                                        //RG_TRAB : TODO ? Tratar a exceção de alguma maneira
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                        //CD_USU : TODO ? Confirmar se é 0 para mandar, está igual arq exemplo
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
                                        //SQ_USU
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                        //DV_USU
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                        //NOME_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        if (!String.IsNullOrEmpty(strBeneficiarioNome))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNome, 70);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
                                        //TP_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                        if (intBeneficiarioTipo > 0)
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioTipo.ToString(), 1);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                        //CD_SEXO
                                        if (!String.IsNullOrEmpty(strBeneficiarioSexo))
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, (strBeneficiarioSexo.Equals("1") ? "M" : "F"), 1);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 1);
                                        //CD_PARENT
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioParentescoCod.PadLeft(2, '0'), 2);
                                        //DT_NASC
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataNascimento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataNascimento.ToString("ddMMyyyy")), 8);
                                        //CD_EST_CIVIL
                                        if (beneficiarioEstadoCivilID != null)
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioEstadoCivilID.ToString(), 1);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                        //DT_CASAMENTO
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataCasamento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataCasamento.ToString("ddMMyyyy")), 8);
                                        //IDENTIDADE
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioRG, 20);
                                        //EMISSOR
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 10);
                                        //CD_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                        //NOME_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                        //CD_SUB_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                        //CD_CARGO : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                        //DT_ADMISSAO
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                        //DT_CADASTRO : TODO ? Tratar exceção em caso de FALSE. (OBRIGATÓRIO)
                                        if (dtBeneficiarioCadastro.CompareTo(DateTime.MinValue) > 0)
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioCadastro.ToString("ddMMyyyy"), 8);
                                        else
                                            EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                        //DT_EXCLUSAO
                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                        sbFileBuffer.AppendLine();

                                        #endregion
                                    }

                                    if (Mov.Equals(Movimentacao.InclusaoBeneficiario))
                                    {
                                        #region Seção U07 / U08

                                        IList<ItemDeclaracaoSaudeINSTANCIA> lstDeclaracaoSaude = ItemDeclaracaoSaudeINSTANCIA.Carregar(beneficiarioID, OperadoraID);

                                        if (lstDeclaracaoSaude != null && lstDeclaracaoSaude.Count > 0)
                                        {
                                            ItemDeclaracaoSaude itemDeclaracao = new ItemDeclaracaoSaude();

                                            foreach (ItemDeclaracaoSaudeINSTANCIA itemDeclaracaoInstancia in lstDeclaracaoSaude)
                                            {
                                                if (itemDeclaracaoInstancia.Sim)
                                                {
                                                    intQtdeRegistroU07U08++;

                                                    itemDeclaracao.ID = itemDeclaracaoInstancia.ItemDeclaracaoID;
                                                    itemDeclaracao.Carregar();

                                                    #region Seção U07

                                                    //NR_SEQ
                                                    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                                    //TP_REG
                                                    sbFileBuffer.Append(U07);
                                                    //TP_MOV
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMov, 1);
                                                    //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                                    //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
                                                    //SQ_USU
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                                    //CD_QUESTAO
                                                    if (!String.IsNullOrEmpty(itemDeclaracao.Codigo))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracao.Codigo.PadLeft(3, '0'), 3);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "000", 3);
                                                    //RESPOSTA
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "S", 1);
                                                    //DT_EVENTO
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.Data.ToString("ddMMyyyy"), 8);
                                                    //ESPECIFICACAO
                                                    if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.Descricao))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.Descricao, 400);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 400);

                                                    sbFileBuffer.AppendLine();

                                                    #endregion

                                                    #region Seção U08

                                                    //NR_SEQ
                                                    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                                                    //TP_REG
                                                    sbFileBuffer.Append(U08);
                                                    //TP_MOV
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMov, 1);
                                                    //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                                    //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
                                                    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
                                                    //SQ_USU
                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
                                                    //CD_CID_INICIAL
                                                    if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.CIDInicial))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.CIDInicial, 4);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 4);
                                                    //CD_CID_FINAL
                                                    if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.CIDFinal))
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.CIDFinal, 4);
                                                    else
                                                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 4);

                                                    sbFileBuffer.AppendLine();

                                                    #endregion
                                                }
                                            }

                                            itemDeclaracao = null;
                                        }

                                        lstDeclaracaoSaude = null;

                                        #endregion
                                    }
                                }

                                #region Build Lote

                                itemLote = new ArqTransacionalLoteItem();

                                itemLote.ContratoID = contrato.ID;
                                itemLote.BeneficiarioID = beneficiarioID;
                                itemLote.BeneficiarioSequencia = intBeneficiarioSequencia;
                                itemLote.Ativo = true;

                                lote.Itens.Add(itemLote);

                                #endregion
                            }

                            #region Seção U99

                            //NR_SEQ
                            sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
                            //TP_REG
                            sbFileBuffer.Append(U99);
                            //QD_U02
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU02.ToString().PadLeft(6, '0'), 6);
                            //QD_U03
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU03.ToString().PadLeft(6, '0'), 6);
                            //QD_U04
                            EntityBase.AppendPreparedField(ref sbFileBuffer, "000000", 6);
                            //QD_U07
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU07U08.ToString().PadLeft(6, '0'), 6);
                            //QD_U08
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU07U08.ToString().PadLeft(6, '0'), 6);
                            ////QD_U10
                            //sbFileBuffer.Append("000000");
                            //QD_INC
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioInclusao.ToString().PadLeft(6, '0'), 6);
                            //QD_EXC
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioExclusao.ToString().PadLeft(6, '0'), 6);
                            //QD_USU
                            EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioAlteracao.ToString().PadLeft(6, '0'), 6);

                            #endregion

                            if (intQtdeBeneficiarioInclusao > 0)
                                lote.Quantidade = intQtdeBeneficiarioInclusao;
                            else if (intQtdeBeneficiarioAlteracao > 0)
                                lote.Quantidade = intQtdeBeneficiarioAlteracao;
                            else if (intQtdeBeneficiarioExclusao > 0)
                                lote.Quantidade = intQtdeBeneficiarioExclusao;

                            if (lote.Itens != null && lote.Itens.Count > 0)
                            {
                                try
                                {
                                    lote.Salvar(true, PM);
                                }
                                catch (Exception) { throw; }

                                String strArquivoNome = lote.Arquivo;

                                try
                                {
                                    this.SalvarArqTransacional(sbFileBuffer, strArquivoNome);
                                }
                                catch (Exception) { throw; }

                                criacaoOK = true;
                                ArquivoNome = strArquivoNome;

                                PM.Commit();
                            }
                            else
                            {
                                PM.Rollback();
                            }
                        }
                        else
                        {
                            PM.Rollback();
                        }
                    }
                }
                catch (Exception)
                {
                    PM.Rollback();
                    throw;
                }
                finally
                {
                    PM.Dispose();
                    PM = null;
                }

                sbFileBuffer = null;
            }

            arqTransUnimed = null;

            return criacaoOK;
        }

        private Boolean GerarArquivo_Fortaleza(Object OperadoraID, ref String ArquivoNome, ContratoBeneficiario.eStatus Status, Object[] ContratoID, Object[] BeneficiarioID, String Mov, String TipoMov, DateTime vigencia)
        {
            Boolean criacaoOK = false;

            ArqTransacionalUnimedConf arqTransUnimed = new ArqTransacionalUnimedConf();
            arqTransUnimed.CarregarPorOperadora(OperadoraID);

            //if (arqTransUnimed.ID != null)
            //{
                Int32 intArqNumeroSequencia = 1;
                Int32 intQtdeBeneficiarioInclusao = 0;
                Int32 intQtdeBeneficiarioExclusao = 0;
                Int32 intQtdeBeneficiarioAlteracao = 0;
                Int32 intQtdeRegistroU02 = 0;
                Int32 intQtdeRegistroU03 = 0;
                Int32 intQtdeRegistroU07U08 = 0;

                ArqTransacionalLote lote = new ArqTransacionalLote();
                StringBuilder sbFileBuffer = new StringBuilder();

                PersistenceManager PM = new PersistenceManager();
                PM = new PersistenceManager();
                PM.BeginTransactionContext();

                try
                {

                    if ((ContratoID != null && ContratoID.Length > 0) &&
                        (BeneficiarioID != null && BeneficiarioID.Length > 0) &&
                        (ContratoID.Length == BeneficiarioID.Length))
                    {
                        for (Int32 i = 0; i < ContratoID.Length; i++)
                        {
                            try
                            {
                                ContratoBeneficiario.AlteraStatusBeneficiario(ContratoID[i], BeneficiarioID[i], Status, PM);
                            }
                            catch (Exception) { throw; }
                        }
                    }

                    using (DataTable dtBeneficiarios = this.GetBeneficiarioPorStatus(OperadoraID, Status, ContratoID, BeneficiarioID, PM, vigencia, String.Join(",", (String[])ContratoID)))
                    {
                        if (dtBeneficiarios != null && dtBeneficiarios.Rows != null && dtBeneficiarios.Rows.Count > 0)
                        {
                            ArqTransacionalLoteItem itemLote = null;
                            Contrato contrato = new Contrato();
                            Plano plano = new Plano();
                            Object contratoID, beneficiarioID, beneficiarioEstadoCivilID;
                            String beneficiarioParentescoCod, strBeneficiarioNome, strBeneficiarioSexo, strBeneficiarioCPF, strBeneficiarioTitularCPF, strCodigoCarencia,
                                   strBeneficiarioRG, strBeneficiarioRgUF, strBeneficiarioRgOrgaoExp, strBeneficiarioEndereco, strBeneficiarioEnderecoNum, strBeneficiarioEnderecoCompl,
                                   strBeneficiarioBairro, strBeneficiarioCEP, strBeneficiarioCidade, strBeneficiarioUF, strBeneficiarioTelefone,
                                   strBeneficiarioTelefoneRamal, strBeneficiarioNomeMae, strBneficiarioPisPasep, strCodigoPlano, strTipoMovimentacaoAux;
                            Int16 intBeneficiarioSequencia, intBeneficiarioTipo;
                            DateTime dtBeneficiarioDataNascimento, dtBeneficiarioDataCasamento, dtBeneficiarioVigencia, dtBeneficiarioCadastro, vigenciaProposta;

                            lote.OperadoraID = OperadoraID;
                            lote.Movimentacao = Mov;
                            lote.TipoMovimentacao = TipoMov;

                            int count = 1;

                            #region Seção 801 

                            if (dtBeneficiarios.Rows.Count > 0)
                            {
                                arqTransUnimed.OperadoraNaUnimed = Convert.ToString(dtBeneficiarios.Rows[0]["contratoadm_numero"]).Trim().PadLeft(8, '0');
                            }

                            //NR_SEQ
                            sbFileBuffer.Append(count.ToString().PadLeft(8, '0'));
                            //TIPO_REG
                            sbFileBuffer.Append("801");
                            //DATA GERACAO
                            sbFileBuffer.Append(DateTime.Now.ToString("yyyyMMdd"));
                            //NUM_VERSAO_ARQ
                            sbFileBuffer.Append("13");
                            //UNIMED_PRODUCAO 
                            sbFileBuffer.Append("063");

                            #endregion

                            
                            foreach (DataRow row in dtBeneficiarios.Rows)
                            {
                                #region switch 

                                switch (Mov)
                                {
                                    case Movimentacao.InclusaoBeneficiario:
                                        intQtdeBeneficiarioInclusao++;
                                        break;
                                    case Movimentacao.ExclusaoBeneficiario:
                                        intQtdeBeneficiarioExclusao++;
                                        break;
                                    case Movimentacao.AlteracaoBeneficiario:
                                        intQtdeBeneficiarioAlteracao++;
                                        break;
                                    case Movimentacao.MudancaDePlano:
                                        intQtdeBeneficiarioAlteracao++;
                                        break;
                                }
                                #endregion

                                contratoID = row["contratobeneficiario_contratoId"];
                                contrato.ID = contratoID;

                                if (contrato.ID != null)
                                {
                                    contrato.Carregar();
                                    if (contrato.PlanoID != null)
                                    {
                                        plano.ID = contrato.PlanoID;
                                        plano.Carregar();
                                    }
                                }

                                #region variavies 

                                beneficiarioID = row["contratobeneficiario_beneficiarioId"];
                                beneficiarioParentescoCod = (row["contratoAdmparentescoagregado_parentescoCodigo"] == null || row["contratoAdmparentescoagregado_parentescoCodigo"] is DBNull) ? "00" : Convert.ToString(row["contratoAdmparentescoagregado_parentescoCodigo"]);
                                beneficiarioEstadoCivilID = (row["contratobeneficiario_estadoCivilId"] == null || row["contratobeneficiario_estadoCivilId"] is DBNull) ? "0" : Convert.ToString(row["estadocivil_codigo"]);
                                strBeneficiarioNome = (row["beneficiario_nome"] == null || row["beneficiario_nome"] is DBNull) ? "" : Convert.ToString(row["beneficiario_nome"]);
                                if (strBeneficiarioNome.Length > 120) { strBeneficiarioNome.Substring(0, 119); }
                                else { strBeneficiarioNome = strBeneficiarioNome.PadRight(120, ' '); }
                                strBeneficiarioNome = EntityBase.RetiraAcentos(strBeneficiarioNome);

                                strBeneficiarioSexo = (row["beneficiario_sexo"] == null || row["beneficiario_sexo"] is DBNull) ? null : Convert.ToString(row["beneficiario_sexo"]);
                                strBeneficiarioCPF = (row["beneficiario_cpf"] == null || row["beneficiario_cpf"] is DBNull) ? "" : Convert.ToString(row["beneficiario_cpf"]);
                                strBeneficiarioCPF = strBeneficiarioCPF.Replace("99999999999", "00000000000");

                                strBeneficiarioTitularCPF = ContratoBeneficiario.GetCPFTitular(contratoID, PM);
                                strBeneficiarioRG = (row["beneficiario_rg"] == null || row["beneficiario_rg"] is DBNull) ? String.Empty : Convert.ToString(row["beneficiario_rg"]);
                                strBeneficiarioRgUF = (row["beneficiario_rgUF"] == null || row["beneficiario_rgUF"] is DBNull) ? String.Empty : Convert.ToString(row["beneficiario_rgUF"]);
                                strBeneficiarioRgOrgaoExp = (row["beneficiario_rgOrgaoExp"] == null || row["beneficiario_rgOrgaoExp"] is DBNull) ? String.Empty : Convert.ToString(row["beneficiario_rgOrgaoExp"]);

                                intBeneficiarioTipo = (row["contratobeneficiario_tipo"] == null || row["contratobeneficiario_tipo"] is DBNull) ? Convert.ToInt16(-1) : Convert.ToInt16(row["contratobeneficiario_tipo"]);
                                intBeneficiarioSequencia = (row["contratobeneficiario_numeroSequencia"] == null || row["contratobeneficiario_numeroSequencia"] is DBNull) ? Convert.ToInt16(0) : Convert.ToInt16(row["contratobeneficiario_numeroSequencia"]);
                                dtBeneficiarioDataNascimento = (row["beneficiario_dataNascimento"] == null || row["beneficiario_dataNascimento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["beneficiario_dataNascimento"]);
                                dtBeneficiarioVigencia = (row["contratobeneficiario_vigencia"] == null || row["contratobeneficiario_vigencia"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_vigencia"]);
                                dtBeneficiarioDataCasamento = (row["contratobeneficiario_dataCasamento"] == null || row["contratobeneficiario_dataCasamento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_dataCasamento"]);
                                dtBeneficiarioCadastro = (row["contratobeneficiario_data"] == null || row["contratobeneficiario_data"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_data"]);
                                strBeneficiarioEndereco = (row["endereco_logradouro"] == null || row["endereco_logradouro"] is DBNull) ? null : Convert.ToString(row["endereco_logradouro"]);
                                strBeneficiarioEnderecoNum = (row["endereco_numero"] == null || row["endereco_numero"] is DBNull) ? null : Convert.ToString(row["endereco_numero"]);
                                strBeneficiarioEnderecoCompl = (row["endereco_complemento"] == null || row["endereco_complemento"] is DBNull) ? null : Convert.ToString(row["endereco_complemento"]);
                                strBeneficiarioBairro = (row["endereco_bairro"] == null || row["endereco_bairro"] is DBNull) ? null : Convert.ToString(row["endereco_bairro"]);
                                strBeneficiarioCEP = (row["endereco_cep"] == null || row["endereco_cep"] is DBNull) ? null : Convert.ToString(row["endereco_cep"]);
                                strBeneficiarioCidade = (row["endereco_cidade"] == null || row["endereco_cidade"] is DBNull) ? null : Convert.ToString(row["endereco_cidade"]);
                                strBeneficiarioUF = (row["endereco_uf"] == null || row["endereco_uf"] is DBNull) ? null : Convert.ToString(row["endereco_uf"]);
                                strBeneficiarioTelefone = (row["beneficiario_telefone"] == null || row["beneficiario_telefone"] is DBNull) ? null : Convert.ToString(row["beneficiario_telefone"]);
                                strBeneficiarioTelefoneRamal = (row["beneficiario_ramal"] == null || row["beneficiario_ramal"] is DBNull) ? null : Convert.ToString(row["beneficiario_ramal"]);
                                strBeneficiarioNomeMae = (row["beneficiario_nomeMae"] == null || row["beneficiario_nomeMae"] is DBNull) ? "" : Convert.ToString(row["beneficiario_nomeMae"]);
                                if (strBeneficiarioNomeMae.Length > 120)
                                    strBeneficiarioNomeMae = strBeneficiarioNomeMae.Substring(0, 119);
                                strBeneficiarioNomeMae = EntityBase.RetiraAcentos(strBeneficiarioNomeMae);

                                strCodigoCarencia = (row["contratobeneficiario_carenciaCodigo"] == null || row["contratobeneficiario_carenciaCodigo"] is DBNull) ? "" : Convert.ToString(row["contratobeneficiario_carenciaCodigo"]).PadLeft(2, '0');
                                strBneficiarioPisPasep = String.Empty;
                                strCodigoPlano = String.Empty;
                                arqTransUnimed.OperadoraNaUnimed = Convert.ToString(row["contratoadm_numero"]).Trim().PadLeft(8, '0');

                                #endregion

                                if (strBeneficiarioTelefone != null)
                                {
                                    strBeneficiarioTelefone = strBeneficiarioTelefone.Replace("-", "");
                                    if (strBeneficiarioTelefone.Length > 8)
                                    {
                                        strBeneficiarioTelefone = strBeneficiarioTelefone.Substring(4).Trim();
                                    }
                                }

                                if (contrato.TipoAcomodacao > -1)
                                    if (contrato.TipoAcomodacao == 0)
                                        strCodigoPlano = (String.IsNullOrEmpty(plano.Codigo)) ? String.Empty : plano.Codigo;
                                    else
                                        strCodigoPlano = (String.IsNullOrEmpty(plano.CodigoParticular)) ? String.Empty : plano.CodigoParticular;

                                //// CASO SEJA UMA MUDANÇA DE PLANO, SINALIZA A SEÇÃO PARA UMA ALTERAÇÃO
                                //if (Mov.Equals(Movimentacao.MudancaDePlano) && TipoMov.Equals(TipoMovimentacao.Exclusao))
                                //    strTipoMovimentacaoAux = TipoMovimentacao.Alteracao;
                                //else
                                //    strTipoMovimentacaoAux = TipoMov;

                                sbFileBuffer.Append(Environment.NewLine);
                                count++;

                                #region Seção 802

                                intQtdeRegistroU02++;

                                //NR_SEQ
                                sbFileBuffer.Append(count.ToString().PadLeft(8, '0'));
                                //TP_REG
                                sbFileBuffer.Append("802");

                                //COD_UNIMED_CARTEIRA 
                                sbFileBuffer.Append("    ");
                                //COD_CARTEIRA 
                                sbFileBuffer.Append("            ");
                                //DV_CARTEIRA 
                                sbFileBuffer.Append(" ");
                                //COD_BENEFICIARIO 
                                sbFileBuffer.Append("        ");
                                //COD EMPRESA ??????????????
                                sbFileBuffer.Append("5407");
                                //COD_FAMILIA
                                sbFileBuffer.Append("      ");

                                //COD_CONTRATO ??????????????
                                sbFileBuffer.Append("00005407");
                                //COD_UNIMED_EMPRESA 
                                sbFileBuffer.Append("0063");
                                //DATA CONTRATO ??????????????
                                sbFileBuffer.Append(Convert.ToDateTime(row["contrato_vigencia"]).ToString("yyyyMMdd"));
                                //COD DEPENDENCIA
                                if (Convert.ToInt32(row["contratobeneficiario_tipo"]) == Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular))
                                    sbFileBuffer.Append("T");
                                else
                                {
                                    if (Convert.ToInt32(row["contratoAdmparentescoagregado_parentescoTipo"]) == Convert.ToInt32(Parentesco.eTipo.Agregado))
                                        sbFileBuffer.Append("A");
                                    else
                                        sbFileBuffer.Append("D");
                                }
                                //COD PARENTESCO
                                sbFileBuffer.Append(Convert.ToString(row["contratoAdmparentescoagregado_parentescoCodigo"]).PadLeft(2, '0'));
                                //COD LOTACAO ????????????
                                sbFileBuffer.Append("00000000");
                                //DATA_PRE_ADMISSIONAL 15
                                sbFileBuffer.Append("        ");
                                //NOME BENEFICIARIO SABIUS 16
                                sbFileBuffer.Append("                         ");

                                //COD_PLANO
                                if (contrato.TipoAcomodacao == 0)
                                    sbFileBuffer.Append(plano.Codigo.PadRight(3,' '));
                                else
                                    sbFileBuffer.Append(plano.CodigoParticular.PadRight(3, ' '));

                                //DIAS A DEDUZIR 18
                                sbFileBuffer.Append("0000");

                                //DATA NASCIMENTO 19
                                sbFileBuffer.Append(dtBeneficiarioDataNascimento.ToString("yyyyMMdd"));

                                //SEXO
                                if (Convert.ToInt32(row["beneficiario_sexo"]) == 1)
                                    sbFileBuffer.Append("M");
                                else
                                    sbFileBuffer.Append("F");

                                //CPF
                                sbFileBuffer.Append(row["beneficiario_cpf"]);
                                //IDENTIDADE
                                sbFileBuffer.Append(strBeneficiarioRG.PadLeft(15, '0'));
                                //COD_UF_RG
                                sbFileBuffer.Append(strBeneficiarioRgUF.PadRight(2, ' '));
                                //ORG_EXP_RG 24
                                sbFileBuffer.Append(strBeneficiarioRgOrgaoExp.PadRight(4, ' '));
                                //COD_PAIS_RG 
                                sbFileBuffer.Append("076");
                                //COD_ESTADO_CIVIL 26
                                sbFileBuffer.Append(row["estadocivil_codigo"]);
                                //UNIMED_DESTINO
                                sbFileBuffer.Append("    ");
                                //TIPO_REPASSE
                                sbFileBuffer.Append(" ");
                                //DATA_INI_REPASSE
                                sbFileBuffer.Append("        ");
                                //DATA_FIM_REPASSE
                                sbFileBuffer.Append("        ");
                                //DATA_INC_PLANO_REPAS 31
                                sbFileBuffer.Append("        ");
                                //NOME_CONTRATANTE
                                sbFileBuffer.Append("".PadLeft(120, ' '));
                                //CPF_CONTRATANTE
                                sbFileBuffer.Append("".PadLeft(11, ' '));
                                //DT_NASC_CONTRATANTE 34
                                sbFileBuffer.Append("".PadLeft(8, ' '));
                                //DATA_VENCIMENTO
                                sbFileBuffer.Append("".PadLeft(2, ' '));
                                //VL_INSCR_REC
                                sbFileBuffer.Append("".PadLeft(10, ' '));
                                //VL_MENS_REC
                                sbFileBuffer.Append("".PadLeft(10, ' '));
                                //MATRICULA 37 ????
                                sbFileBuffer.Append(Convert.ToString(row["beneficiario_cpf"]).Substring(0, 6).PadLeft(20, '0'));
                                //COD_EQUIPE
                                sbFileBuffer.Append("     ");
                                //COD_AGENTE
                                sbFileBuffer.Append("      ");
                                //COD_PROPOSTA
                                sbFileBuffer.Append("          ");
                                //COD_ACAO
                                sbFileBuffer.Append("001");
                                //DATA_ACAO 41 ???? qual data do contrato
                                sbFileBuffer.Append(Convert.ToDateTime(row["contrato_vigencia"]).ToString("yyyyMMdd"));
                                //COD_MOTIVO_ACAO
                                sbFileBuffer.Append("0001");
                                //NOME_MAE
                                sbFileBuffer.Append(strBeneficiarioNomeMae.PadRight(120, ' '));
                                //PF_PJ
                                sbFileBuffer.Append("J");
                                //FORMA_PAGTO
                                sbFileBuffer.Append(" ");
                                //EMPRESA_TRANSF
                                sbFileBuffer.Append("    ");
                                //CONTRATO_TRANS
                                sbFileBuffer.Append("        ");
                                //MESMA_FAMILIA
                                sbFileBuffer.Append(" ");
                                //COD_PADRAO_PROPOSTA
                                sbFileBuffer.Append("          ");
                                //NOME_BENEFICIARIO_ANS
                                sbFileBuffer.Append(strBeneficiarioNome);
                                //COD_CARENCIA ????
                                if (row["contratobeneficiario_carenciaCodigo"] != null && row["contratobeneficiario_carenciaCodigo"] != DBNull.Value && Convert.ToString(row["contratobeneficiario_carenciaCodigo"]).Trim() != "" && Convert.ToString(row["contratobeneficiario_carenciaCodigo"]).Trim() != "0")
                                    sbFileBuffer.Append(Convert.ToString(row["contratobeneficiario_carenciaCodigo"]).Substring(0,2));
                                else
                                    sbFileBuffer.Append("00");

                                //NUM_DEP_PROPOSTA
                                sbFileBuffer.Append((dtBeneficiarios.Select("contratobeneficiario_contratoId=" + row["contratobeneficiario_contratoId"]).Length - 1).ToString().PadLeft(2, '0'));
                                //COD_PROPOSTA_ORIGEM
                                sbFileBuffer.Append("          ");
                                //PAGTO_NA_PROPOSTA ????? nao houve???
                                sbFileBuffer.Append("N");
                                //MATRICULA_TRANSF
                                sbFileBuffer.Append("".PadLeft(20, ' '));
                                //PROPOSTA_TRANSF
                                sbFileBuffer.Append("".PadLeft(10, ' '));
                                //PESO
                                sbFileBuffer.Append("".PadLeft(6, ' '));
                                //ALTURA 59
                                sbFileBuffer.Append("".PadLeft(3, ' '));
                                //RESIDE_BR
                                sbFileBuffer.Append("0");

                                #region comentado 
                                ////DT_NASC
                                //EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataNascimento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataNascimento.ToString("ddMMyyyy")), 8);
                                ////CD_EST_CIVIL
                                //if (beneficiarioEstadoCivilID != null)
                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioEstadoCivilID.ToString(), 1);
                                //else
                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
                                ////DT_CASAMENTO
                                //EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataCasamento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataCasamento.ToString("ddMMyyyy")), 8);
                                ////IDENTIDADE
                                //EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioRG, 20);
                                ////EMISSOR
                                //EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 10);
                                ////CD_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                //EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
                                ////NOME_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                //EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
                                ////CD_SUB_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                //EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);

                                ////CD_CARGO : TODO ? Verificar o pq de não serem enviados no arq exemplo.
                                //EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

                                ////DT_ADMISSAO
                                //EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                ////DATA EFETIVACAO
                                //EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                ////DT_CADASTRO (VIGENCIA) : TODO ? Tratar exceção em caso de FALSE. (OBRIGATÓRIO)
                                //if (dtBeneficiarioCadastro.CompareTo(DateTime.MinValue) > 0)
                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
                                //else
                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                ////DT_EXCLUSAO
                                //EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
                                #endregion

                                #endregion

                                sbFileBuffer.Append(Environment.NewLine);
                                count++;

                                #region Seção 803 - Endereço 

                                //NR_SEQ
                                sbFileBuffer.Append(count.ToString().PadLeft(8, '0'));
                                //TIPO
                                sbFileBuffer.Append("803");
                                //COD_TIPO_ENDEREÇO
                                sbFileBuffer.Append("B");
                                //CEP
                                sbFileBuffer.Append(strBeneficiarioCEP.Replace("-", ""));

                                //COD_TIPO_LOGRADOURO
                                sbFileBuffer.Append(TraduzTipoEndereco(strBeneficiarioEndereco));

                                //NOME_LOGRADOURO
                                if (strBeneficiarioEndereco.Length > 40) { strBeneficiarioEndereco = strBeneficiarioEndereco.Substring(0, 39); }
                                strBeneficiarioEndereco = strBeneficiarioEndereco.PadRight(40, ' ');
                                sbFileBuffer.Append(EntityBase.RetiraAcentos(strBeneficiarioEndereco));

                                //NUM_ENDERECO
                                sbFileBuffer.Append(strBeneficiarioEnderecoNum.PadRight(8, ' '));
                                //COMPL_ENDERECO
                                sbFileBuffer.Append(EntityBase.RetiraAcentos(strBeneficiarioEnderecoCompl.PadRight(45, ' ')));
                                //NOME_BAIRRO
                                if (strBeneficiarioBairro.Length > 30) { strBeneficiarioBairro = strBeneficiarioBairro.Substring(0, 29); }
                                strBeneficiarioBairro = strBeneficiarioBairro.PadRight(30, ' ');
                                sbFileBuffer.Append(EntityBase.RetiraAcentos(strBeneficiarioBairro));
                                //NOME_CIDADE
                                if (strBeneficiarioCidade.Length > 30) { strBeneficiarioCidade = strBeneficiarioCidade.Substring(0, 29); }
                                strBeneficiarioCidade = strBeneficiarioCidade.PadRight(30, ' ');
                                sbFileBuffer.Append(EntityBase.RetiraAcentos(strBeneficiarioCidade));
                                //UF
                                sbFileBuffer.Append(strBeneficiarioUF);
                                //COD_CIDADE
                                sbFileBuffer.Append("      ");
                                //COD_LOGRADOURO
                                sbFileBuffer.Append("     ");
                                //COD_BAIRRO
                                sbFileBuffer.Append("    ");
                                //COD_ENDERECO
                                sbFileBuffer.Append("      ");

                                #endregion

                                sbFileBuffer.Append(Environment.NewLine);
                                count++;

                                #region Seção 804 contato 

                                //NR_SEQ
                                sbFileBuffer.Append(count.ToString().PadLeft(8, '0'));
                                //TIPO_REGISTRO
                                sbFileBuffer.Append("804");
                                //COD_TIPO_ENDEREÇO
                                sbFileBuffer.Append("B");
                                //COD_TIPO_MEIO_CONTATO
                                if (row["beneficiario_celular"] != null && row["beneficiario_celular"] != DBNull.Value && Convert.ToString(row["beneficiario_celular"]).Replace("(", "").Replace(")","").Trim() != "")
                                {
                                    sbFileBuffer.Append("05");
                                    sbFileBuffer.Append(Convert.ToString(row["beneficiario_celular"]).Replace("(", "").Replace(")", "").Replace(" ", "").PadRight(50, ' '));
                                }
                                else if (row["beneficiario_telefone"] != null && row["beneficiario_telefone"] != DBNull.Value && Convert.ToString(row["beneficiario_telefone"]).Replace("(", "").Replace(")", "").Trim() != "")
                                {
                                    sbFileBuffer.Append("01");
                                    sbFileBuffer.Append(Convert.ToString(row["beneficiario_telefone"]).Replace("(", "").Replace(")", "").Replace(" ", "").PadRight(50, ' '));
                                }
                                else if (row["beneficiario_telefone2"] != null && row["beneficiario_telefone2"] != DBNull.Value && Convert.ToString(row["beneficiario_telefone2"]).Replace("(", "").Replace(")", "").Trim() != "")
                                {
                                    sbFileBuffer.Append("01");
                                    sbFileBuffer.Append(Convert.ToString(row["beneficiario_telefone2"]).Replace("(", "").Replace(")", "").Replace(" ","").PadRight(50, ' '));
                                }
                                else if (row["beneficiario_email"] != null && row["beneficiario_email"] != DBNull.Value && Convert.ToString(row["beneficiario_email"]).Trim() != "")
                                {
                                    sbFileBuffer.Append("09");
                                    sbFileBuffer.Append(Convert.ToString(row["beneficiario_email"]).PadRight(50, ' '));
                                }
                                else
                                {
                                    sbFileBuffer.Append("00");
                                    sbFileBuffer.Append("".PadRight(50, ' '));
                                }

                                #endregion

                                sbFileBuffer.Append(Environment.NewLine);
                                count++;

                                #region Seção 805 plano, opcionais 

                                //NR_SEQ
                                sbFileBuffer.Append(count.ToString().PadLeft(8, '0'));
                                //TIPO_REGISTRO
                                sbFileBuffer.Append("805");


                                //PRODUTO_UNIMED 
                                //sbFileBuffer.Append("P000002557"); //0000000063
                                if (contrato.TipoAcomodacao == 0)
                                    sbFileBuffer.Append(plano.SubPlano.PadRight(10, ' '));
                                else
                                    sbFileBuffer.Append(plano.SubPlanoParticular.PadRight(10, ' '));

                                //ITEM_PROD_UNIMED
                                if (contrato.TipoAcomodacao == 0)
                                {
                                    //if(!String.IsNullOrEmpty(plano.SubPlano))
                                    //    sbFileBuffer.Append(plano.SubPlano.PadLeft(2, '0'));
                                    //else
                                        sbFileBuffer.Append("01");
                                }
                                else
                                {
                                    //if (!String.IsNullOrEmpty(plano.SubPlanoParticular))
                                    //    sbFileBuffer.Append(plano.SubPlanoParticular.PadLeft(2, '0'));
                                    //else
                                        sbFileBuffer.Append("01");
                                }
                                //ITEM_PROD_TRANSF
                                sbFileBuffer.Append("  ");

                                #endregion

                                #region Build Lote

                                itemLote = new ArqTransacionalLoteItem();

                                itemLote.ContratoID = contrato.ID;
                                itemLote.BeneficiarioID = beneficiarioID;
                                itemLote.BeneficiarioSequencia = intBeneficiarioSequencia;
                                itemLote.Ativo = true;

                                lote.Itens.Add(itemLote);

                                #endregion
                            }

                            sbFileBuffer.Append(Environment.NewLine);
                            count++;

                            #region Seção 809 Trailler 

                            //NR_SEQ
                            sbFileBuffer.Append(count.ToString().PadLeft(8, '0'));
                            //TP_REG
                            sbFileBuffer.Append("809");
                            //QTDE_TOT_R802
                            sbFileBuffer.Append(intQtdeBeneficiarioInclusao.ToString().PadLeft(4, '0'));
                            //QTDE_TOT_R803
                            sbFileBuffer.Append(intQtdeBeneficiarioInclusao.ToString().PadLeft(4, '0'));
                            //QTDE_TOT_R804
                            sbFileBuffer.Append(intQtdeBeneficiarioInclusao.ToString().PadLeft(4, '0'));
                            //QTDE_TOT_R805
                            sbFileBuffer.Append(intQtdeBeneficiarioInclusao.ToString().PadLeft(4, '0'));
                            //QTDE_TOT_R806
                            sbFileBuffer.Append("0000");
                            //QTDE_TOT_R807
                            sbFileBuffer.Append("0000");
                            //QTDE_TOT_R808
                            sbFileBuffer.Append("0000");

                            #endregion

                            if (intQtdeBeneficiarioInclusao > 0)
                                lote.Quantidade = intQtdeBeneficiarioInclusao;
                            else if (intQtdeBeneficiarioAlteracao > 0)
                                lote.Quantidade = intQtdeBeneficiarioAlteracao;
                            else if (intQtdeBeneficiarioExclusao > 0)
                                lote.Quantidade = intQtdeBeneficiarioExclusao;

                            if (lote.Itens != null && lote.Itens.Count > 0)
                            {
                                try
                                {
                                    lote.Salvar(true, PM);
                                }
                                catch (Exception) { throw; }

                                String strArquivoNome = lote.Arquivo;

                                try
                                {
                                    this.SalvarArqTransacional(sbFileBuffer, strArquivoNome);
                                }
                                catch (Exception) { throw; }

                                criacaoOK = true;
                                ArquivoNome = strArquivoNome;

                                PM.Commit(); 
                            }
                            else
                            {
                                PM.Rollback();
                            }
                        }
                        else
                        {
                            PM.Rollback();
                        }
                    }
                }
                catch (Exception)
                {
                    PM.Rollback();
                    throw;
                }
                finally
                {
                    PM.Dispose();
                    PM = null;
                }

                sbFileBuffer = null;
            //}

            arqTransUnimed = null;

            return criacaoOK;
        }

        String TraduzTipoEndereco(String endereco)
        {
            String tipoEnd = endereco.Split(' ')[0].Trim().ToUpper();

            if (tipoEnd == "RUA")
                return "R  ";
            else if (tipoEnd == "ALAMEDA")
                return "AL ";
            else if (tipoEnd == "ACESSO")
                return "AC ";
            else if (tipoEnd == "AVENIDA" || tipoEnd == "AV")
                return "AV ";
            else if (tipoEnd == "BECO")
                return "BC ";
            else if (tipoEnd == "CHÁCARA" || tipoEnd == "CHACARA")
                return "CH ";
            else if (tipoEnd == "CONJUNTO")
                return "CJ ";
            else if (tipoEnd == "ESTRADA")
                return "EST";
            else if (tipoEnd == "FAVELA")
                return "FAV";
            else if (tipoEnd == "FAZENDA")
                return "FAZ";
            else if (tipoEnd == "GALERIA")
                return "GAL";
            else if (tipoEnd == "LARGO")
                return "LG ";
            else if (tipoEnd == "LOTEAMENTO")
                return "LOT";
            else if (tipoEnd == "PRAÇA" || tipoEnd == "PRACA")
                return "PC ";
            else if (tipoEnd == "QUADRA")
                return "Q  ";
            else if (tipoEnd == "RODOVIA")
                return "ROD";
            else if (tipoEnd == "SERRA")
                return "SER";
            else if (tipoEnd == "SERRA")
                return "SER";
            else if (tipoEnd == "SÍTIO" || tipoEnd == "SITIO")
                return "SIT";
            else if (tipoEnd == "SETOR")
                return "ST ";
            else if (tipoEnd == "TRAVESSA")
                return "TV ";
            else if (tipoEnd == "VILA")
                return "VL ";

            //if (tipoEnd.Length > 3) { tipoEnd = tipoEnd.Substring(0, 2); }
            return "R  ";  //tipoEnd;
        }

        #endregion

        #endregion

        #region Private Structs

        /// <summary>
        /// Tipo de Movimentação.
        /// </summary>
        private struct TipoMovimentacao
        {
            /// <summary>
            /// Inclusão.
            /// </summary>
            public static readonly String Inclusao = "I";

            /// <summary>
            /// Alteração.
            /// </summary>
            public static readonly String Alteracao = "A";

            /// <summary>
            /// Exclusão.
            /// </summary>
            public static readonly String Exclusao = "E";

            /// <summary>
            /// Emissão de 2ª.
            /// </summary>
            public static readonly String EmissaoSegundaVia = "C";
        }

        #endregion

        #region Public Structs

        /// <summary>
        /// Movimentação (Inclusao de Beneficiario, Mudança de Plano, etc).
        /// </summary>
        public struct Movimentacao
        {
            /// <summary>
            /// Inclusão de Beneficiário.
            /// </summary>
            public const String InclusaoBeneficiario = "IB";

            /// <summary>
            /// Alteração de Beneficiário.
            /// </summary>
            public const String AlteracaoBeneficiario = "AB";

            /// <summary>
            /// Exclusão de Beneficiário.
            /// </summary>
            public const String ExclusaoBeneficiario = "EB";

            /// <summary>
            /// Segunda via de Cartão do Benficiário.
            /// </summary>
            public const String SegundaViaCartaoBeneficiario = "SVCB";

            /// <summary>
            /// Mudança de Plano.
            /// </summary>
            public const String MudancaDePlano = "MDP";

            /// <summary>
            /// Cancelamento de Contrato.
            /// </summary>
            public const String CancelamentoContrato = "CC";
        }

        #endregion

        #region Private Constructors

        /// <summary>
        /// Construtor Privado.
        /// </summary>
        private ArqTransacionalUnimed() { }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Contrutor para setar o Root Path.
        /// </summary>
        /// <param name="RootPath"></param>
        public ArqTransacionalUnimed(String RootPath)
        {
            if (!String.IsNullOrEmpty(RootPath))
                this.ArqTransacionalRootPath = RootPath;
        }

        #endregion

        #region Public Methods

        #region GetBeneficiarioPorStatus

        /// <summary>
        /// Método para pegar os Beneficiario pelo o Status.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="Status">Status do Beneficiario.</param>
        /// <returns></returns>
        private DataTable GetBeneficiarioPorStatus(Object OperadoraID, ContratoBeneficiario.eStatus Status)
        {
            return null;// this.GetBeneficiarioPorStatus(OperadoraID, Status, null, null, null);
        }

        /// <summary>
        /// Método para pegar os Beneficiario pelo o Status.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="Status">Status do Beneficiario.</param>
        /// <param name="ContratoID">Array com ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array com ID de Beneficiario.</param>
        /// <returns></returns>
        public DataTable GetBeneficiarioPorStatus(Object OperadoraID, ContratoBeneficiario.eStatus Status, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, DateTime vigencia, Object contratoAdmId)
        {
            return this.GetBeneficiarioPorStatus(OperadoraID, Status, ContratoID, BeneficiarioID, loteId, null, vigencia, contratoAdmId);
        }

        public DataTable GetBeneficiarioPorStatus(Object OperadoraID, ContratoBeneficiario.eStatus Status, Object[] ContratoID, Object[] BeneficiarioID, PersistenceManager PM, DateTime vigencia, Object contratoAdmId)
        {
            return GetBeneficiarioPorStatus(OperadoraID, Status, ContratoID, BeneficiarioID, null, null, vigencia, contratoAdmId);
        }
        /// <summary>
        /// Método para pegar os Beneficiario pelo o Status.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="Status">Status do Beneficiario.</param>
        /// <param name="ContratoID">Array com ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array com ID de Beneficiario.</param>
        /// <returns></returns>
        public DataTable GetBeneficiarioPorStatus(Object OperadoraID, ContratoBeneficiario.eStatus Status, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia, Object contratoAdmId)
        {
            switch (Status)
            {
                case ContratoBeneficiario.eStatus.Novo:
                    return Untyped.UntypedProcesses.GetBeneficiarioInclusao(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.PendenteNaOperadora:
                    return Untyped.UntypedProcesses.GetBeneficiarioPendente(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.Devolvido:
                    return Untyped.UntypedProcesses.GetBeneficiarioDevolvido(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.AlteracaoCadastroPendente:
                    return Untyped.UntypedProcesses.GetBeneficiarioAlteracaoCadastroPendente(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.AlteracaoCadastroPendenteNaOperadora:
                    return Untyped.UntypedProcesses.GetBeneficiarioAlteracaoCadastroPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.AlteracaoCadastroDevolvido:
                    return Untyped.UntypedProcesses.GetBeneficiarioAlteracaoCadastroPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.SegundaViaCartaoPendente:
                    return Untyped.UntypedProcesses.GetBeneficiarioSegundaViaCartaoPendente(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.SegundaViaCartaoPendenteNaOperadora:
                    return Untyped.UntypedProcesses.GetBeneficiarioSegundaViaCartaoPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.SegundaViaCartaoDevolvido:
                    return Untyped.UntypedProcesses.GetBeneficiarioSegundaViaCartaoPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.ExclusaoPendente:
                    return Untyped.UntypedProcesses.GetBeneficiarioExclusaoPendente(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora:
                    return Untyped.UntypedProcesses.GetBeneficiarioExclusaoPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.ExclusaoDevolvido:
                    return Untyped.UntypedProcesses.GetBeneficiarioExclusaoPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.MudancaPlanoPendente:
                    return Untyped.UntypedProcesses.GetBeneficiarioMudancaPlanoPendente(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.MudancaPlanoPendenteNaOperadora:
                    return Untyped.UntypedProcesses.GetBeneficiarioMudancaPlanoPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.MudancaDePlanoDevolvido:
                    return Untyped.UntypedProcesses.GetBeneficiarioMudancaPlanoPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.CancelamentoPendente:
                    return Untyped.UntypedProcesses.GetBeneficiarioCancelamentoContratoPendente(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora:
                    return Untyped.UntypedProcesses.GetBeneficiarioCancelamentoContratoPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.CancelamentoDevolvido:
                    return Untyped.UntypedProcesses.GetBeneficiarioCancelamentoContratoPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.Desconhecido:
                    return null;
                default:
                    return null;
            }
        }

        #endregion

        #region GerarArquivoInclusao

        /// <summary>
        /// Método para Gerar um Arquivo de Inclusao.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <returns></returns>
        public Boolean GerarArquivoInclusao(Object OperadoraID, ref String ArquivoNome)
        {
            return this.GerarArquivoInclusao(OperadoraID, ref ArquivoNome, null, null, DateTime.MinValue);
        }

        /// <summary>
        /// Método para Gerar um Arquivo de Inclusao.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public Boolean GerarArquivoInclusao(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia)
        {
            if(Operadora.IsUnimed(OperadoraID))
                return this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.Novo, ContratoID, BeneficiarioID, Movimentacao.InclusaoBeneficiario, TipoMovimentacao.Inclusao,vigencia);
            else // fortaleza
                return this.GerarArquivo_Fortaleza(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.Novo, ContratoID, BeneficiarioID, Movimentacao.InclusaoBeneficiario, TipoMovimentacao.Inclusao, vigencia);
        }

        #endregion

        #region GerarArquivoInclusaoDevolvido

        /// <summary>
        /// Método para Gerar um Arquivo de Inclusao para os que foram Devolvidos.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <returns></returns>
        public Boolean GerarArquivoInclusaoDevolvido(Object OperadoraID, ref String ArquivoNome)
        {
            return this.GerarArquivoInclusaoDevolvido(OperadoraID, ref ArquivoNome, null, null);
        }

        /// <summary>
        /// Método para Gerar um Arquivo de Inclusao para os que foram Devolvidos.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public Boolean GerarArquivoInclusaoDevolvido(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        {
            return false; // this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.Devolvido, ContratoID, BeneficiarioID, Movimentacao.InclusaoBeneficiario, TipoMovimentacao.Inclusao);
        }

        #endregion

        #region GerarArquivoAlteracao

        /// <summary>
        /// Método para Gerar um Arquivo de Alteração dos Dados Cadastrais.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <returns></returns>
        public Boolean GerarArquivoAlteracao(Object OperadoraID, ref String ArquivoNome)
        {
            return this.GerarArquivoAlteracao(OperadoraID, ref ArquivoNome, null, null);
        }

        /// <summary>
        /// Método para Gerar um Arquivo de Alteração dos Dados Cadastrais.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public Boolean GerarArquivoAlteracao(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        {
            return false; // this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.AlteracaoCadastroPendente, ContratoID, BeneficiarioID, Movimentacao.AlteracaoBeneficiario, TipoMovimentacao.Alteracao);
        }

        #endregion

        #region GerarArquivoAlteracaoDevolvido

        /// <summary>
        /// Método para Gerar um Arquivo de Alteração dos Dados Cadastrais para os que foram Devolvidos.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <returns></returns>
        public Boolean GerarArquivoAlteracaoDevolvido(Object OperadoraID, ref String ArquivoNome)
        {
            return this.GerarArquivoAlteracaoDevolvido(OperadoraID, ref ArquivoNome, null, null);
        }

        /// <summary>
        /// Método para Gerar um Arquivo de Alteração dos Dados Cadastrais para os que foram Devolvidos.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public Boolean GerarArquivoAlteracaoDevolvido(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        {
            return false; // this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.AlteracaoCadastroDevolvido, ContratoID, BeneficiarioID, Movimentacao.AlteracaoBeneficiario, TipoMovimentacao.Alteracao);
        }

        #endregion

        #region GerarArquivoExclusao

        /// <summary>
        /// Método para Gerar um Arquivo de Exclusão de Beneficiário.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <returns></returns>
        public Boolean GerarArquivoExclusao(Object OperadoraID, ref String ArquivoNome)
        {
            return this.GerarArquivoExclusao(OperadoraID, ref ArquivoNome, null, null);
        }

        /// <summary>
        /// Método para Gerar um Arquivo de Exclusão de Beneficiário.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public Boolean GerarArquivoExclusao(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        {
            return false; // this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.ExclusaoPendente, ContratoID, BeneficiarioID, Movimentacao.ExclusaoBeneficiario, TipoMovimentacao.Exclusao);
        }

        #endregion

        #region GerarArquivoExclusaoDevolvido

        /// <summary>
        /// Método para Gerar um Arquivo de Exclusão de Beneficiário.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <returns></returns>
        public Boolean GerarArquivoExclusaoDevolvido(Object OperadoraID, ref String ArquivoNome)
        {
            return this.GerarArquivoExclusaoDevolvido(OperadoraID, ref ArquivoNome, null, null);
        }

        /// <summary>
        /// Método para Gerar um Arquivo de Exclusão de Beneficiário.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public Boolean GerarArquivoExclusaoDevolvido(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        {
            return false; // this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.ExclusaoDevolvido, ContratoID, BeneficiarioID, Movimentacao.ExclusaoBeneficiario, TipoMovimentacao.Exclusao);
        }

        #endregion

        #region GerarArquivoSegundaViaCartao

        /// <summary>
        /// Método para Gerar um Arquivo de Exclusão de Beneficiário.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <returns></returns>
        public Boolean GerarArquivoSegundaViaCartao(Object OperadoraID, ref String ArquivoNome)
        {
            return this.GerarArquivoSegundaViaCartao(OperadoraID, ref ArquivoNome, null, null);
        }

        /// <summary>
        /// Método para Gerar um Arquivo de Exclusão de Beneficiário.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public Boolean GerarArquivoSegundaViaCartao(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        {
            return false; // this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.SegundaViaCartaoPendente, ContratoID, BeneficiarioID, Movimentacao.SegundaViaCartaoBeneficiario, TipoMovimentacao.EmissaoSegundaVia);
        }

        #endregion

        #region GerarArquivoSegundaViaCartaoDevolvido

        /// <summary>
        /// Método para Gerar um Arquivo de Segunda Via de Cartão dos Beneficiários que foram devolvidos.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <returns></returns>
        public Boolean GerarArquivoSegundaViaCartaoDevolvido(Object OperadoraID, ref String ArquivoNome)
        {
            return this.GerarArquivoSegundaViaCartaoDevolvido(OperadoraID, ref ArquivoNome, null, null);
        }

        /// <summary>
        /// Método para Gerar um Arquivo de Segunda Via de Cartão dos Beneficiários que foram devolvidos.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public Boolean GerarArquivoSegundaViaCartaoDevolvido(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        {
            return false; // this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.SegundaViaCartaoDevolvido, ContratoID, BeneficiarioID, Movimentacao.SegundaViaCartaoBeneficiario, TipoMovimentacao.EmissaoSegundaVia);
        }

        #endregion

        #region GerarArquivoMudancaPlano

        /// <summary>
        /// Método para Gerar um Arquivo de Mudança de Plano do Beneficiário.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <returns></returns>
        public Boolean GerarArquivoMudancaPlano(Object OperadoraID, ref String ArquivoNome)
        {
            return this.GerarArquivoMudancaPlano(OperadoraID, ref ArquivoNome, null, null);
        }

        /// <summary>
        /// Método para Gerar um Arquivo de Mudança de Plano do Beneficiário.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public Boolean GerarArquivoMudancaPlano(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        {
            return false; // this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.MudancaPlanoPendente, ContratoID, BeneficiarioID, Movimentacao.MudancaDePlano, TipoMovimentacao.Exclusao);
        }

        #endregion

        #region GerarArquivoMudancaPlanoDevolvido

        /// <summary>
        /// Método para Gerar um Arquivo de Mudança de Plano do Beneficiário que foi devolvido pela OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <returns></returns>
        public Boolean GerarArquivoMudancaPlanoDevolvido(Object OperadoraID, ref String ArquivoNome)
        {
            return this.GerarArquivoMudancaPlanoDevolvido(OperadoraID, ref ArquivoNome, null, null);
        }

        /// <summary>
        /// Método para Gerar um Arquivo de Mudança de Plano do Beneficiário que foi devolvido pela OPERADORA.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public Boolean GerarArquivoMudancaPlanoDevolvido(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        {
            return false; // this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.MudancaDePlanoDevolvido, ContratoID, BeneficiarioID, Movimentacao.MudancaDePlano, TipoMovimentacao.Exclusao);
        }

        #endregion

        #region GerarArquivoCancelamentoContrato

        /// <summary>
        /// Método para Gerar um Arquivo de Cancelamento de Contrato do Beneficiário.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <returns></returns>
        public Boolean GerarArquivoCancelamentoContrato(Object OperadoraID, ref String ArquivoNome)
        {
            return this.GerarArquivoCancelamentoContrato(OperadoraID, ref ArquivoNome, null, null);
        }

        /// <summary>
        /// Método para Gerar um Arquivo de Cancelamento de Contrato do Beneficiário.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public Boolean GerarArquivoCancelamentoContrato(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        {
            return this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.CancelamentoPendente, ContratoID, BeneficiarioID, Movimentacao.CancelamentoContrato, TipoMovimentacao.Exclusao, DateTime.MinValue);
        }

        #endregion

        #region GerarArquivoCancelamentoContratoDevolvido

        /// <summary>
        /// Método para Gerar um Arquivo de Cancelamento de Contrato para os que foram devolvidos.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <returns></returns>
        public Boolean GerarArquivoCancelamentoContratoDevolvido(Object OperadoraID, ref String ArquivoNome)
        {
            return this.GerarArquivoCancelamentoContratoDevolvido(OperadoraID, ref ArquivoNome, null, null);
        }

        /// <summary>
        /// Método para Gerar um Arquivo de Cancelamento de Contrato do Beneficiário.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public Boolean GerarArquivoCancelamentoContratoDevolvido(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        {
            return false; // this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.CancelamentoDevolvido, ContratoID, BeneficiarioID, Movimentacao.CancelamentoContrato, TipoMovimentacao.Exclusao);
        }

        #endregion

        #region GerarArquivoPorStatus

        /// <summary>
        /// Método para Gerar um Arquivo de acordo com o Status do Beneficiario no Contrato.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public void GerarArquivoPorStatus(Object OperadoraID, ref String ArquivoNome, ContratoBeneficiario.eStatus Status, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia)
        {
            switch (Status)
            {
                case ContratoBeneficiario.eStatus.Novo:
                    this.GerarArquivoInclusao(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID, vigencia);
                    break;
                case ContratoBeneficiario.eStatus.Devolvido:
                    //this.GerarArquivoInclusaoDevolvido(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.AlteracaoCadastroPendente:
                    this.GerarArquivoAlteracao(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.AlteracaoCadastroDevolvido:
                    this.GerarArquivoAlteracaoDevolvido(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.SegundaViaCartaoPendente:
                    this.GerarArquivoSegundaViaCartao(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.SegundaViaCartaoDevolvido:
                    this.GerarArquivoSegundaViaCartaoDevolvido(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.ExclusaoPendente:
                    this.GerarArquivoExclusao(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.ExclusaoDevolvido:
                    this.GerarArquivoExclusaoDevolvido(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.MudancaPlanoPendente:
                    this.GerarArquivoMudancaPlano(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.MudancaDePlanoDevolvido:
                    this.GerarArquivoMudancaPlanoDevolvido(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.CancelamentoPendente:
                    this.GerarArquivoCancelamentoContrato(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.CancelamentoDevolvido:
                    this.GerarArquivoCancelamentoContratoDevolvido(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
            }
        }

        #endregion

        #endregion
    }

    public class ArqTransacionalAmil
    {
        #region Private Fields 

        /// <summary>
        /// Caminho relativo do repositório de arquivos transacionais.
        /// </summary>
        private readonly String ArqTransacionalRelativePath = ConfigurationManager.AppSettings["transactFilePath"];

        /// <summary>
        /// Caminho físico e absoluto da raiz aplicação.
        /// </summary>
        private readonly String ArqTransacionalRootPath = String.Empty;

        #endregion

        #region Private Members

        /// <summary>
        /// Path para os Arquivos Transacionais.
        /// </summary>
        private String ArqTransacionalFilePath
        {
            get
            {
                return String.Concat(this.ArqTransacionalRootPath, ArqTransacionalRelativePath.Replace("/", "\\"));
            }
        }

        #endregion

        #region Private Methods 

        #region SalvarArqTransacional

        /// <summary>
        /// Salva o Arquivo Transacional no disco.
        /// </summary>
        /// <param name="SB">Buffer com o texto do Arquivo.</param>
        /// <param name="FileName">Nome do Arquivo.</param>
        /// <returns>True se conseguiu salvar e False se deu algum problema</returns>
        private Boolean SalvarArqTransacional(StringBuilder SB, String FileName)
        {
            if (SB != null && SB.Length > 0 && !String.IsNullOrEmpty(FileName))
            {
                if (!Directory.Exists(this.ArqTransacionalFilePath))
                {
                    try
                    {
                        Directory.CreateDirectory(this.ArqTransacionalFilePath);
                    }
                    catch (Exception) { throw; }
                }

                try
                {
                    String conteudo = SB.ToString();
                    conteudo = conteudo.Replace("º", "o");
                    conteudo = conteudo.Replace("ª", "a");
                    File.WriteAllText(String.Concat(this.ArqTransacionalFilePath, FileName), conteudo);//, System.Text.Encoding.GetEncoding("iso-8859-1")
                }
                catch (Exception) { throw; }

                return true;
            }

            return false;
        }

        #endregion

        #region GerarArquivo 

        //public Boolean GerarArquivoUNIMED_DemaisMovimentacoes(ref String ArquivoNome, IList<ItemAgendaArquivoUnimed> itens, String Mov)
        //{
        //    Boolean criacaoOK = false;

        //    Object OperadoraID = Operadora.UnimedID;

        //    ArqTransacionalUnimedConf arqTransUnimed = new ArqTransacionalUnimedConf();
        //    arqTransUnimed.CarregarPorOperadora(OperadoraID);

        //    #region traduz tipo movimentacao

        //    String TipoMov = "";
        //    String cbTipo0Cond = "";

        //    switch (Mov)
        //    {
        //        case Movimentacao.AlteracaoBeneficiario:
        //        case Movimentacao.MudancaDePlano:
        //            {
        //                TipoMov = TipoMovimentacao.Alteracao;
        //                break;
        //            }
        //        case Movimentacao.ExclusaoBeneficiario:
        //            {
        //                TipoMov = TipoMovimentacao.Exclusao;
        //                break;
        //            }
        //        case Movimentacao.CancelamentoContrato:
        //            {
        //                TipoMov = TipoMovimentacao.Exclusao;
        //                cbTipo0Cond = " and cben.contratobeneficiario_tipo=0 ";
        //                break;
        //            }
        //        case Movimentacao.InclusaoBeneficiario:
        //            {
        //                TipoMov = TipoMovimentacao.Inclusao;
        //                break;
        //            }
        //        case Movimentacao.SegundaViaCartaoBeneficiario:
        //            {
        //                TipoMov = TipoMovimentacao.EmissaoSegundaVia;
        //                break;
        //            }
        //    }
        //    #endregion

        //    if (arqTransUnimed.ID != null)
        //    {
        //        Int32 intArqNumeroSequencia = 1;
        //        Int32 intQtdeBeneficiarioInclusao = 0;
        //        Int32 intQtdeBeneficiarioExclusao = 0;
        //        Int32 intQtdeBeneficiarioAlteracao = 0;
        //        Int32 intQtdeRegistroU02 = 0;
        //        Int32 intQtdeRegistroU03 = 0;
        //        Int32 intQtdeRegistroU07U08 = 0;

        //        ArqTransacionalLote lote = new ArqTransacionalLote();
        //        StringBuilder sbFileBuffer = new StringBuilder();

        //        StringBuilder condition = new StringBuilder();
        //        foreach (ItemAgendaArquivoUnimed item in itens)
        //        {
        //            if (condition.Length > 0) { condition.Append(" OR "); }
        //            condition.Append("(contratobeneficiario_ativo=1 AND contratobeneficiario_beneficiarioId="); condition.Append(item.BeneficiarioID);
        //            condition.Append(" AND contratobeneficiario_contratoId="); condition.Append(item.PropostaID);
        //            condition.Append(")");
        //        }

        //        PersistenceManager PM = new PersistenceManager();
        //        PM = new PersistenceManager();
        //        PM.BeginTransactionContext();

        //        try
        //        {
        //            //using (DataTable dtBeneficiarios = LocatorHelper.Instance.ExecuteQuery("select contrato_admissao, contrato_numeroMatricula, contratobeneficiario_carenciaCodigo, contrato_vigencia, contratoadm_numero, contratoadm_id, item_lote_id, lote_data_criacao, c.contrato_numero, cBen.contratobeneficiario_id, cBen.contratobeneficiario_vigencia, cBen.contratobeneficiario_contratoId, cBen.contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo, contratobeneficiario_numeroSequencia, cBen.contratobeneficiario_portabilidade, cBen.contratobeneficiario_data, cBen.contratobeneficiario_estadoCivilId, estadocivil_codigo, beneficiario_nome, beneficiario_sexo,beneficiario_cpf, beneficiario_rg, beneficiario_rgUF, beneficiario_rgOrgaoExp, beneficiario_declaracaoNascimentoVivo, beneficiario_cns, cBen.contratobeneficiario_dataCasamento, beneficiario_nomeMae, beneficiario_dataNascimento,beneficiario_telefone, beneficiario_ramal, beneficiario_telefone2, beneficiario_celular, beneficiario_email, endereco_logradouro, endereco_numero, endereco_complemento,endereco_bairro, endereco_cidade, endereco_uf, endereco_cep FROM contrato_beneficiario cBen INNER JOIN contrato c ON cBen.contratobeneficiario_contratoId = c.contrato_id and cben.contratobeneficiario_tipo=0 INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id and cben.contratobeneficiario_tipo=0 INNER JOIN endereco ende ON c.contrato_enderecoReferenciaId = ende.endereco_id INNER JOIN contratoADM on contratoadm_id=contrato_contratoAdmId INNER JOIN estado_civil ON estadocivil_id=cBen.contratobeneficiario_estadocivilId LEFT JOIN contratoAdm_parentesco_agregado ON contratoAdmparentescoagregado_id=cBen.contratobeneficiario_parentescoId LEFT JOIN arquivo_transacional_lote_item ON item_contrato_id=c.contrato_id AND item_beneficiario_id=Ben.beneficiario_id AND item_ativo=1 LEFT JOIN arquivo_transacional_lote ON lote_id=item_lote_id AND lote_exportacao <> 1  WHERE " + condition.ToString() + " order by beneficiario_nome", "resultset").Tables[0])
        //            //using (DataTable dtBeneficiarios = LocatorHelper.Instance.ExecuteQuery("select contrato_admissao, contrato_numeroMatricula, contratobeneficiario_carenciaCodigo, contrato_vigencia, contratoadm_numero, contratoadm_id, c.contrato_numero, cBen.contratobeneficiario_id, cBen.contratobeneficiario_vigencia, cBen.contratobeneficiario_contratoId, cBen.contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo, contratobeneficiario_numeroSequencia, cBen.contratobeneficiario_portabilidade, cBen.contratobeneficiario_data, cBen.contratobeneficiario_estadoCivilId, estadocivil_codigo, beneficiario_nome, beneficiario_sexo,beneficiario_cpf, beneficiario_rg, beneficiario_rgUF, beneficiario_rgOrgaoExp, beneficiario_declaracaoNascimentoVivo, beneficiario_cns, cBen.contratobeneficiario_dataCasamento, beneficiario_nomeMae, beneficiario_dataNascimento,beneficiario_telefone, beneficiario_ramal, beneficiario_telefone2, beneficiario_celular, beneficiario_email, endereco_logradouro, endereco_numero, endereco_complemento,endereco_bairro, endereco_cidade, endereco_uf, endereco_cep FROM contrato_beneficiario cBen INNER JOIN contrato c ON cBen.contratobeneficiario_contratoId = c.contrato_id and cben.contratobeneficiario_tipo=0 INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id and cben.contratobeneficiario_tipo=0 INNER JOIN endereco ende ON c.contrato_enderecoReferenciaId = ende.endereco_id INNER JOIN contratoADM on contratoadm_id=contrato_contratoAdmId INNER JOIN estado_civil ON estadocivil_id=cBen.contratobeneficiario_estadocivilId LEFT JOIN contratoAdm_parentesco_agregado ON contratoAdmparentescoagregado_id=cBen.contratobeneficiario_parentescoId WHERE " + condition.ToString() + " order by beneficiario_nome", "resultset").Tables[0])
        //            using (DataTable dtBeneficiarios = LocatorHelper.Instance.ExecuteQuery("select contrato_admissao, contrato_numeroMatricula, contratobeneficiario_carenciaCodigo, contrato_vigencia, contratoadm_numero, contratoadm_id, c.contrato_numero, cBen.contratobeneficiario_id, cBen.contratobeneficiario_vigencia, cBen.contratobeneficiario_contratoId, cBen.contratobeneficiario_beneficiarioId, contratobeneficiario_tipo, contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo, contratobeneficiario_numeroSequencia, cBen.contratobeneficiario_portabilidade, cBen.contratobeneficiario_data, cBen.contratobeneficiario_estadoCivilId, estadocivil_codigo, beneficiario_nome, beneficiario_sexo,beneficiario_cpf, beneficiario_rg, beneficiario_rgUF, beneficiario_rgOrgaoExp, beneficiario_declaracaoNascimentoVivo, beneficiario_cns, cBen.contratobeneficiario_dataCasamento, beneficiario_nomeMae, beneficiario_dataNascimento,beneficiario_telefone, beneficiario_ramal, beneficiario_telefone2, beneficiario_celular, beneficiario_email, endereco_logradouro, endereco_numero, endereco_complemento,endereco_bairro, endereco_cidade, endereco_uf, endereco_cep FROM contrato_beneficiario cBen INNER JOIN contrato c ON cBen.contratobeneficiario_contratoId = c.contrato_id INNER JOIN beneficiario Ben ON cBen.contratobeneficiario_beneficiarioId = Ben.beneficiario_id INNER JOIN endereco ende ON c.contrato_enderecoReferenciaId = ende.endereco_id INNER JOIN contratoADM on contratoadm_id=contrato_contratoAdmId INNER JOIN estado_civil ON estadocivil_id=cBen.contratobeneficiario_estadocivilId LEFT JOIN contratoAdm_parentesco_agregado ON contratoAdmparentescoagregado_id=cBen.contratobeneficiario_parentescoId WHERE " + condition.ToString() + " --order by beneficiario_nome", "resultset").Tables[0])
        //            {
        //                if (dtBeneficiarios != null && dtBeneficiarios.Rows != null && dtBeneficiarios.Rows.Count > 0)
        //                {
        //                    #region variaveis

        //                    ArqTransacionalLoteItem itemLote = null;
        //                    Contrato contrato = new Contrato();
        //                    Plano plano = new Plano();
        //                    Object contratoID, beneficiarioID, beneficiarioEstadoCivilID;
        //                    String beneficiarioParentescoCod, strBeneficiarioNome, strBeneficiarioSexo, strBeneficiarioCPF, strBeneficiarioTitularCPF, strCodigoCarencia,
        //                           strBeneficiarioRG, strBeneficiarioEndereco, strBeneficiarioEnderecoNum, strBeneficiarioEnderecoCompl,
        //                           strBeneficiarioBairro, strBeneficiarioCEP, strBeneficiarioCidade, strBeneficiarioUF, strBeneficiarioTelefone,
        //                           strBeneficiarioTelefoneRamal, strBeneficiarioNomeMae, strBneficiarioPisPasep, strCodigoPlano, strTipoMovimentacaoAux;
        //                    Int16 intBeneficiarioSequencia, intBeneficiarioTipo;
        //                    DateTime dtBeneficiarioDataNascimento, dtBeneficiarioDataCasamento, dtBeneficiarioVigencia, dtBeneficiarioCadastro, vigenciaProposta;

        //                    lote.OperadoraID = OperadoraID;
        //                    lote.Movimentacao = Mov;
        //                    lote.TipoMovimentacao = TipoMov;

        //                    #endregion

        //                    #region Seção U01

        //                    if (dtBeneficiarios.Rows.Count > 0)
        //                    {
        //                        arqTransUnimed.OperadoraNaUnimed = Convert.ToString(dtBeneficiarios.Rows[0]["contratoadm_numero"]).Trim().PadLeft(8, '0');
        //                    }

        //                    //NR_SEQ
        //                    sbFileBuffer.Append(intArqNumeroSequencia.ToString().PadLeft(6, '0'));
        //                    //TP_REG
        //                    sbFileBuffer.Append(U01);
        //                    //CD_UNI
        //                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCodSingular))
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCodSingular, 4);
        //                    //CD_EMP
        //                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
        //                    //NOME_EMP
        //                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNome))
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNome, 40);
        //                    //CNPJ_EMP
        //                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCNPJ))
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCNPJ, 14);
        //                    //TP_IDENTIFICA
        //                    if (!String.IsNullOrEmpty(arqTransUnimed.TipoIdentificacao.ToString()))
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.TipoIdentificacao, 1);
        //                    //VERSAO
        //                    if (!String.IsNullOrEmpty("4.0")) //arqTransUnimed.ArqVersao 
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.ArqVersao, 3);
        //                    //DT_GERACAO
        //                    sbFileBuffer.Append(DateTime.Now.ToString("ddMMyyyy"));
        //                    //CEI_EMP
        //                    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCEI))
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCEI, 13);

        //                    sbFileBuffer.AppendLine();

        //                    #endregion

        //                    foreach (DataRow row in dtBeneficiarios.Rows)
        //                    {
        //                        switch (Mov)
        //                        {
        //                            case Movimentacao.InclusaoBeneficiario:
        //                                intQtdeBeneficiarioInclusao++;
        //                                break;
        //                            case Movimentacao.ExclusaoBeneficiario:
        //                                intQtdeBeneficiarioExclusao++;
        //                                break;
        //                            case Movimentacao.AlteracaoBeneficiario:
        //                                intQtdeBeneficiarioAlteracao++;
        //                                break;
        //                            case Movimentacao.MudancaDePlano:
        //                                intQtdeBeneficiarioAlteracao++;
        //                                break;
        //                        }

        //                        contratoID = row["contratobeneficiario_contratoId"];
        //                        contrato.ID = contratoID;

        //                        if (contrato.ID != null)
        //                        {
        //                            contrato.Carregar();
        //                            if (contrato.PlanoID != null)
        //                            {
        //                                plano.ID = contrato.PlanoID;
        //                                plano.Carregar();
        //                            }
        //                        }

        //                        #region preenche variáveis

        //                        beneficiarioID = row["contratobeneficiario_beneficiarioId"];
        //                        beneficiarioParentescoCod = (row["contratoAdmparentescoagregado_parentescoCodigo"] == null || row["contratoAdmparentescoagregado_parentescoCodigo"] is DBNull) ? "00" : Convert.ToString(row["contratoAdmparentescoagregado_parentescoCodigo"]);
        //                        beneficiarioEstadoCivilID = (row["contratobeneficiario_estadoCivilId"] == null || row["contratobeneficiario_estadoCivilId"] is DBNull) ? "0" : Convert.ToString(row["estadocivil_codigo"]);
        //                        strBeneficiarioNome = (row["beneficiario_nome"] == null || row["beneficiario_nome"] is DBNull) ? null : Convert.ToString(row["beneficiario_nome"]);
        //                        strBeneficiarioSexo = (row["beneficiario_sexo"] == null || row["beneficiario_sexo"] is DBNull) ? null : Convert.ToString(row["beneficiario_sexo"]);
        //                        strBeneficiarioCPF = (row["beneficiario_cpf"] == null || row["beneficiario_cpf"] is DBNull) ? "" : Convert.ToString(row["beneficiario_cpf"]);
        //                        strBeneficiarioCPF = strBeneficiarioCPF.Replace("99999999999", "00000000000");

        //                        strBeneficiarioTitularCPF = ContratoBeneficiario.GetCPFTitular(contratoID, PM);
        //                        strBeneficiarioRG = (row["beneficiario_rg"] == null || row["beneficiario_rg"] is DBNull) ? String.Empty : Convert.ToString(row["beneficiario_rg"]);
        //                        intBeneficiarioTipo = (row["contratobeneficiario_tipo"] == null || row["contratobeneficiario_tipo"] is DBNull) ? Convert.ToInt16(-1) : Convert.ToInt16(row["contratobeneficiario_tipo"]);
        //                        intBeneficiarioSequencia = (row["contratobeneficiario_numeroSequencia"] == null || row["contratobeneficiario_numeroSequencia"] is DBNull) ? Convert.ToInt16(0) : Convert.ToInt16(row["contratobeneficiario_numeroSequencia"]);
        //                        dtBeneficiarioDataNascimento = (row["beneficiario_dataNascimento"] == null || row["beneficiario_dataNascimento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["beneficiario_dataNascimento"]);
        //                        dtBeneficiarioVigencia = (row["contratobeneficiario_vigencia"] == null || row["contratobeneficiario_vigencia"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_vigencia"]);
        //                        dtBeneficiarioDataCasamento = (row["contratobeneficiario_dataCasamento"] == null || row["contratobeneficiario_dataCasamento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_dataCasamento"]);
        //                        dtBeneficiarioCadastro = (row["contratobeneficiario_data"] == null || row["contratobeneficiario_data"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_data"]);
        //                        strBeneficiarioEndereco = (row["endereco_logradouro"] == null || row["endereco_logradouro"] is DBNull) ? null : Convert.ToString(row["endereco_logradouro"]);
        //                        strBeneficiarioEnderecoNum = (row["endereco_numero"] == null || row["endereco_numero"] is DBNull) ? null : Convert.ToString(row["endereco_numero"]);
        //                        strBeneficiarioEnderecoCompl = (row["endereco_complemento"] == null || row["endereco_complemento"] is DBNull) ? null : Convert.ToString(row["endereco_complemento"]);
        //                        strBeneficiarioBairro = (row["endereco_bairro"] == null || row["endereco_bairro"] is DBNull) ? null : Convert.ToString(row["endereco_bairro"]);
        //                        strBeneficiarioCEP = (row["endereco_cep"] == null || row["endereco_cep"] is DBNull) ? null : Convert.ToString(row["endereco_cep"]);
        //                        strBeneficiarioCidade = (row["endereco_cidade"] == null || row["endereco_cidade"] is DBNull) ? null : Convert.ToString(row["endereco_cidade"]);
        //                        strBeneficiarioUF = (row["endereco_uf"] == null || row["endereco_uf"] is DBNull) ? null : Convert.ToString(row["endereco_uf"]);
        //                        strBeneficiarioTelefone = (row["beneficiario_telefone"] == null || row["beneficiario_telefone"] is DBNull) ? null : Convert.ToString(row["beneficiario_telefone"]);
        //                        strBeneficiarioTelefoneRamal = (row["beneficiario_ramal"] == null || row["beneficiario_ramal"] is DBNull) ? null : Convert.ToString(row["beneficiario_ramal"]);
        //                        strBeneficiarioNomeMae = (row["beneficiario_nomeMae"] == null || row["beneficiario_nomeMae"] is DBNull) ? null : Convert.ToString(row["beneficiario_nomeMae"]);
        //                        strCodigoCarencia = (row["contratobeneficiario_carenciaCodigo"] == null || row["contratobeneficiario_carenciaCodigo"] is DBNull) ? "" : Convert.ToString(row["contratobeneficiario_carenciaCodigo"]).PadLeft(2, '0');
        //                        strBneficiarioPisPasep = String.Empty;
        //                        strCodigoPlano = String.Empty;
        //                        arqTransUnimed.OperadoraNaUnimed = Convert.ToString(row["contratoadm_numero"]).Trim().PadLeft(8, '0');
        //                        #endregion

        //                        if (strBeneficiarioTelefone != null)
        //                        {
        //                            strBeneficiarioTelefone = strBeneficiarioTelefone.Replace("-", "");
        //                            if (strBeneficiarioTelefone.Length > 8)
        //                            {
        //                                strBeneficiarioTelefone = strBeneficiarioTelefone.Substring(4).Trim();
        //                            }
        //                        }

        //                        if (contrato.TipoAcomodacao > -1)
        //                            if (contrato.TipoAcomodacao == 0)
        //                                strCodigoPlano = (String.IsNullOrEmpty(plano.Codigo)) ? String.Empty : plano.Codigo;
        //                            else
        //                                strCodigoPlano = (String.IsNullOrEmpty(plano.CodigoParticular)) ? String.Empty : plano.CodigoParticular;

        //                        // CASO SEJA UMA MUDANÇA DE PLANO, SINALIZA A SEÇÃO PARA UMA ALTERAÇÃO
        //                        if (Mov.Equals(Movimentacao.MudancaDePlano) && TipoMov.Equals(TipoMovimentacao.Exclusao))
        //                            strTipoMovimentacaoAux = TipoMovimentacao.Alteracao;
        //                        else
        //                            strTipoMovimentacaoAux = TipoMov;

        //                        #region Seção U02

        //                        intQtdeRegistroU02++;

        //                        //NR_SEQ
        //                        sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
        //                        //TP_REG
        //                        sbFileBuffer.Append(U02);
        //                        //TP_MOV
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, strTipoMovimentacaoAux, 1);
        //                        //CD_EMP
        //                        if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
        //                        //CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                        if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF, 11);
        //                        else
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
        //                        //CPF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                        if (!String.IsNullOrEmpty(strBeneficiarioCPF))
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCPF, 11);
        //                        else
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
        //                        //RG_TRAB : TODO ? Tratar a exceção de alguma maneira
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
        //                        //CD_USU : TODO ? Confirmar se é 0 para mandar, está igual arq exemplo
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
        //                        //SQ_USU
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
        //                        //DV_USU
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
        //                        //NOME_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                        if (!String.IsNullOrEmpty(strBeneficiarioNome))
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNome, 70);
        //                        else
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
        //                        //TP_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                        if (intBeneficiarioTipo > 0)
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioTipo.ToString(), 1);
        //                        else
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
        //                        //CD_SEXO
        //                        if (!String.IsNullOrEmpty(strBeneficiarioSexo))
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, (strBeneficiarioSexo.Equals("1") ? "M" : "F"), 1);
        //                        else
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 1);
        //                        //CD_PARENT
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioParentescoCod.PadLeft(2, '0'), 2);
        //                        //DT_NASC
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataNascimento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataNascimento.ToString("ddMMyyyy")), 8);
        //                        //CD_EST_CIVIL
        //                        if (beneficiarioEstadoCivilID != null)
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioEstadoCivilID.ToString(), 1);
        //                        else
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
        //                        //DT_CASAMENTO
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataCasamento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataCasamento.ToString("ddMMyyyy")), 8);
        //                        //IDENTIDADE
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioRG, 20);
        //                        //EMISSOR
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 10);

        //                        ////DECL_NASC_VIVO
        //                        //if (row["beneficiario_declaracaoNascimentoVivo"] == null || row["beneficiario_declaracaoNascimentoVivo"] == DBNull.Value || Convert.ToString(row["beneficiario_declaracaoNascimentoVivo"]).Trim() == "")
        //                        //    sbFileBuffer.Append("0".PadLeft(11, '0'));
        //                        //else
        //                        //    sbFileBuffer.Append(Convert.ToString(row["beneficiario_declaracaoNascimentoVivo"]).PadLeft(11, '0'));
        //                        ////CNS
        //                        //if (row["beneficiario_cns"] == null || row["beneficiario_cns"] == DBNull.Value || Convert.ToString(row["beneficiario_cns"]).Trim() == "")
        //                        //    sbFileBuffer.Append("0".PadLeft(15, '0'));
        //                        //else
        //                        //    sbFileBuffer.Append(Convert.ToString(row["beneficiario_cns"]).PadLeft(15, '0'));

        //                        //CD_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
        //                        //NOME_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
        //                        //CD_SUB_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
        //                        //CD_CARGO : TODO ? Verificar o pq de não serem enviados no arq exemplo.
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
        //                        //DT_ADMISSAO
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

        //                        //DATA EFETIVACAO
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

        //                        //DT_CADASTRO (VIGENCIA) : TODO ? Tratar exceção em caso de FALSE. (OBRIGATÓRIO)
        //                        if (dtBeneficiarioCadastro.CompareTo(DateTime.MinValue) > 0)
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
        //                        else
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
        //                        //DT_EXCLUSAO
        //                        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
        //                        ////PORTABILIDADE
        //                        //sbFileBuffer.Append("N");

        //                        sbFileBuffer.AppendLine();

        //                        #endregion

        //                        if (Mov.Equals(Movimentacao.InclusaoBeneficiario) ||
        //                            Mov.Equals(Movimentacao.AlteracaoBeneficiario))
        //                        {
        //                            #region Seção U05

        //                            //NR_SEQ
        //                            sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
        //                            //TP_REG
        //                            sbFileBuffer.Append(U05);
        //                            //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
        //                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
        //                            else
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
        //                            //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                            if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
        //                            else
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
        //                            //SQ_USU
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
        //                            //ENDERECO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                            if (!String.IsNullOrEmpty(strBeneficiarioEndereco))
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEndereco, 45);
        //                            else
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 45);

        //                            //NR_ENDER : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                            if (!String.IsNullOrEmpty(strBeneficiarioEnderecoNum))
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEnderecoNum.Trim().PadLeft(5, '0'), 5);
        //                            else
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);

        //                            //COMPL_ENDER : TODO ? Limitar a 15 o complemento da UNIMED no cadastro de endereço
        //                            if (!String.IsNullOrEmpty(strBeneficiarioEnderecoCompl))
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioEnderecoCompl, 15);
        //                            else
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 15);
        //                            //BAIRRO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                            if (!String.IsNullOrEmpty(strBeneficiarioBairro))
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioBairro, 20);
        //                            else
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
        //                            //CEP : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                            if (!String.IsNullOrEmpty(strBeneficiarioCEP))
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCEP, 8);
        //                            else
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
        //                            //CIDADE : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                            if (!String.IsNullOrEmpty(strBeneficiarioCidade))
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCidade, 30);
        //                            else
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 30);
        //                            //UF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                            if (!String.IsNullOrEmpty(strBeneficiarioUF))
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioUF, 2);
        //                            else
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 2);
        //                            //TELEFONE
        //                            if (!String.IsNullOrEmpty(strBeneficiarioTelefone))
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTelefone, 9);
        //                            else
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 9);
        //                            //RAMAL
        //                            if (!String.IsNullOrEmpty(strBeneficiarioTelefoneRamal))
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTelefoneRamal, 6);
        //                            else
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 6);
        //                            //NOME_MAE : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                            if (!String.IsNullOrEmpty(strBeneficiarioNomeMae))
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNomeMae, 70);
        //                            else
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
        //                            //PIS_PASEP : TODO ? Não tem no Cadastro, ele não é enviado nem com caracter em branco mesmo estando com espaço em branco
        //                            EntityBase.AppendPreparedField(ref sbFileBuffer, strBneficiarioPisPasep, 11);
        //                            ////CIDADE_RESIDENCIA
        //                            //EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCidade, 30);

        //                            sbFileBuffer.AppendLine();

        //                            #endregion
        //                        }

        //                        if (Mov.Equals(Movimentacao.InclusaoBeneficiario) || Mov.Equals(Movimentacao.MudancaDePlano))
        //                        {
        //                            #region Seção U03

        //                            Boolean bolHasSubPlano = false;
        //                            Boolean bolRepeatOnce = false;

        //                            #region Exclusão de Plano Antigo

        //                            if (Mov.Equals(Movimentacao.MudancaDePlano)) //&& TipoMov.Equals(TipoMovimentacao.Exclusao))
        //                            {
        //                                ContratoPlano ContratoPlanoAntigo = ContratoPlano.CarregarPenultimo(contrato.ID, PM);

        //                                if (ContratoPlanoAntigo != null)
        //                                {
        //                                    // Carregar o plano antigo, código do plano antigo, acomodação, etc.
        //                                    if (ContratoPlanoAntigo != null && ContratoPlanoAntigo.ID != null)
        //                                    {
        //                                        Plano planoAntigo = new Plano(ContratoPlanoAntigo.PlanoID);
        //                                        planoAntigo.Carregar();

        //                                        bolHasSubPlano = !String.IsNullOrEmpty(planoAntigo.SubPlano) || !String.IsNullOrEmpty(planoAntigo.SubPlanoParticular);
        //                                        bolRepeatOnce = false;

        //                                        String strCodigoPlanoAntigo = null;

        //                                        if (ContratoPlanoAntigo.TipoAcomodacao > -1)
        //                                            if (ContratoPlanoAntigo.TipoAcomodacao == 0)
        //                                                strCodigoPlanoAntigo = (String.IsNullOrEmpty(planoAntigo.Codigo)) ? String.Empty : planoAntigo.Codigo;
        //                                            else
        //                                                strCodigoPlanoAntigo = (String.IsNullOrEmpty(planoAntigo.CodigoParticular)) ? String.Empty : planoAntigo.CodigoParticular;

        //                                        do
        //                                        {
        //                                            intQtdeRegistroU03++;

        //                                            //NR_SEQ
        //                                            sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
        //                                            //TP_REG
        //                                            sbFileBuffer.Append(U03);
        //                                            //TP_MOV
        //                                            EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.Exclusao, 1);
        //                                            //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
        //                                            if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
        //                                                EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
        //                                            else
        //                                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);


        //                                            //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                                            if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
        //                                                EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
        //                                            else
        //                                                EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);

        //                                            //SQ_USU
        //                                            EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
        //                                            //CD_PLANO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                                            if (!String.IsNullOrEmpty(strCodigoPlanoAntigo))
        //                                                EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoPlanoAntigo.PadLeft(10, '0'), 10);
        //                                            else
        //                                                EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
        //                                            //DT_INICIO
        //                                            EntityBase.AppendPreparedField(ref sbFileBuffer, ContratoPlanoAntigo.Data.ToString("ddMMyyyy"), 8);//EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
        //                                            //DT_FIM
        //                                            EntityBase.AppendPreparedField(ref sbFileBuffer, DateTime.Now.ToString("ddMMyyyy"), 8);
        //                                            //ITEM_REDUCAO : TODO ? Saber da onde vem essa informação
        //                                            EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoCarencia, 2);

        //                                            sbFileBuffer.AppendLine();

        //                                            if (bolHasSubPlano)
        //                                            {
        //                                                if (ContratoPlanoAntigo.TipoAcomodacao == 0)
        //                                                    strCodigoPlanoAntigo = planoAntigo.SubPlano;
        //                                                else
        //                                                    strCodigoPlanoAntigo = planoAntigo.SubPlanoParticular;

        //                                                bolRepeatOnce = !bolRepeatOnce;
        //                                            }

        //                                        } while (bolHasSubPlano && bolRepeatOnce);
        //                                    }
        //                                }

        //                                ContratoPlanoAntigo = null;
        //                            }

        //                            #endregion

        //                            #region Inclusão de Plano Novo

        //                            bolHasSubPlano = !String.IsNullOrEmpty(plano.SubPlano) || !String.IsNullOrEmpty(plano.SubPlanoParticular);
        //                            bolRepeatOnce = false;

        //                            ContratoPlano ContratoPlanoAtual = ContratoPlano.CarregarAtual(contrato.ID, PM);
        //                            if (ContratoPlanoAtual == null) { ContratoPlanoAtual = new ContratoPlano(); }

        //                            do
        //                            {
        //                                intQtdeRegistroU03++;

        //                                //NR_SEQ
        //                                sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
        //                                //TP_REG
        //                                sbFileBuffer.Append(U03);
        //                                //TP_MOV
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.Inclusao, 1);
        //                                //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
        //                                if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
        //                                    EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
        //                                else
        //                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

        //                                //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                                if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
        //                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
        //                                else
        //                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);

        //                                //SQ_USU
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
        //                                //CD_PLANO : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                                if (!String.IsNullOrEmpty(strCodigoPlano))
        //                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoPlano.PadLeft(10, '0'), 10);
        //                                else
        //                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
        //                                //DT_INICIO
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, ContratoPlanoAtual.Data.ToString("ddMMyyyy"), 8); //EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioVigencia.ToString("ddMMyyyy"), 8);
        //                                //DT_FIM
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
        //                                //ITEM_REDUCAO : TODO ? Saber de onde vem essa informação
        //                                EntityBase.AppendPreparedField(ref sbFileBuffer, strCodigoCarencia, 2);

        //                                sbFileBuffer.AppendLine();

        //                                if (bolHasSubPlano)
        //                                {
        //                                    if (contrato.TipoAcomodacao == 0)
        //                                        strCodigoPlano = plano.SubPlano;
        //                                    else
        //                                        strCodigoPlano = plano.SubPlanoParticular;

        //                                    bolRepeatOnce = !bolRepeatOnce;
        //                                }

        //                            } while (bolHasSubPlano && bolRepeatOnce);

        //                            #endregion

        //                            #endregion

        //                            #region Caso seja uma MUDANÇA DE PLANO, devemos EMITIR uma segunda via de Cartão.
        //                            if (Mov.Equals(Movimentacao.MudancaDePlano))
        //                                //{
        //                                //    #region Seção U02 Alteração de Cartão

        //                                //    intQtdeRegistroU02++;

        //                                //    //NR_SEQ
        //                                //    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
        //                                //    //TP_REG
        //                                //    sbFileBuffer.Append(U02);
        //                                //    //TP_MOV
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMovimentacao.EmissaoSegundaVia, 1);
        //                                //    //CD_EMP
        //                                //    if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
        //                                //    //CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                                //    if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF, 11);
        //                                //    else
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
        //                                //    //CPF : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                                //    if (!String.IsNullOrEmpty(strBeneficiarioCPF))
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioCPF, 11);
        //                                //    else
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 11);
        //                                //    //RG_TRAB : TODO ? Tratar a exceção de alguma maneira
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
        //                                //    //CD_USU : TODO ? Confirmar se é 0 para mandar, está igual arq exemplo
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, "0000000000", 10);
        //                                //    //SQ_USU
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
        //                                //    //DV_USU
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
        //                                //    //NOME_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                                //    if (!String.IsNullOrEmpty(strBeneficiarioNome))
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioNome, 70);
        //                                //    else
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 70);
        //                                //    //TP_USU : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                                //    if (intBeneficiarioTipo > 0)
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioTipo.ToString(), 1);
        //                                //    else
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
        //                                //    //CD_SEXO
        //                                //    if (!String.IsNullOrEmpty(strBeneficiarioSexo))
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, (strBeneficiarioSexo.Equals("1") ? "M" : "F"), 1);
        //                                //    else
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 1);
        //                                //    //CD_PARENT
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioParentescoCod.PadLeft(2, '0'), 2);
        //                                //    //DT_NASC
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataNascimento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataNascimento.ToString("ddMMyyyy")), 8);
        //                                //    //CD_EST_CIVIL
        //                                //    if (beneficiarioEstadoCivilID != null)
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, beneficiarioEstadoCivilID.ToString(), 1);
        //                                //    else
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, "0", 1);
        //                                //    //DT_CASAMENTO
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, ((dtBeneficiarioDataCasamento.CompareTo(DateTime.MinValue) == 0) ? "0".PadLeft(8, '0') : dtBeneficiarioDataCasamento.ToString("ddMMyyyy")), 8);
        //                                //    //IDENTIDADE
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioRG, 20);
        //                                //    //EMISSOR
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 10);
        //                                //    //CD_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
        //                                //    //NOME_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 20);
        //                                //    //CD_SUB_LOCAL : TODO ? Verificar o pq de não serem enviados no arq exemplo.
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000", 5);
        //                                //    //CD_CARGO : TODO ? Verificar o pq de não serem enviados no arq exemplo.
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
        //                                //    //DT_ADMISSAO
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
        //                                //    //DT_CADASTRO : TODO ? Tratar exceção em caso de FALSE. (OBRIGATÓRIO)
        //                                //    if (dtBeneficiarioCadastro.CompareTo(DateTime.MinValue) > 0)
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, dtBeneficiarioCadastro.ToString("ddMMyyyy"), 8);
        //                                //    else
        //                                //        EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
        //                                //    //DT_EXCLUSAO
        //                                //    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);

        //                                //    sbFileBuffer.AppendLine();

        //                                //    #endregion
        //                                //}
        //                            #endregion

        //                                if (Mov.Equals(Movimentacao.InclusaoBeneficiario))
        //                                {
        //                                    #region Seção U07 / U08

        //                                    IList<ItemDeclaracaoSaudeINSTANCIA> lstDeclaracaoSaude = ItemDeclaracaoSaudeINSTANCIA.Carregar(beneficiarioID, OperadoraID);

        //                                    if (lstDeclaracaoSaude != null && lstDeclaracaoSaude.Count > 0)
        //                                    {
        //                                        ItemDeclaracaoSaude itemDeclaracao = new ItemDeclaracaoSaude();

        //                                        foreach (ItemDeclaracaoSaudeINSTANCIA itemDeclaracaoInstancia in lstDeclaracaoSaude)
        //                                        {
        //                                            if (itemDeclaracaoInstancia.Sim)
        //                                            {
        //                                                intQtdeRegistroU07U08++;

        //                                                itemDeclaracao.ID = itemDeclaracaoInstancia.ItemDeclaracaoID;
        //                                                itemDeclaracao.Carregar();

        //                                                #region Seção U07

        //                                                //NR_SEQ
        //                                                sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
        //                                                //TP_REG
        //                                                sbFileBuffer.Append(U07);
        //                                                //TP_MOV
        //                                                EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMov, 1);
        //                                                //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
        //                                                if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
        //                                                else
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
        //                                                //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                                                if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
        //                                                else
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
        //                                                //SQ_USU
        //                                                EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
        //                                                //CD_QUESTAO
        //                                                if (!String.IsNullOrEmpty(itemDeclaracao.Codigo))
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracao.Codigo.PadLeft(3, '0'), 3);
        //                                                else
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "000", 3);
        //                                                //RESPOSTA
        //                                                EntityBase.AppendPreparedField(ref sbFileBuffer, "S", 1);
        //                                                //DT_EVENTO
        //                                                EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.Data.ToString("ddMMyyyy"), 8);
        //                                                //ESPECIFICACAO
        //                                                if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.Descricao))
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.Descricao, 400);
        //                                                else
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 400);

        //                                                sbFileBuffer.AppendLine();

        //                                                #endregion

        //                                                #region Seção U08

        //                                                //NR_SEQ
        //                                                sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
        //                                                //TP_REG
        //                                                sbFileBuffer.Append(U08);
        //                                                //TP_MOV
        //                                                EntityBase.AppendPreparedField(ref sbFileBuffer, TipoMov, 1);
        //                                                //CD_EMP : TODO ? Tratar execeção quando FALSE (OBRIGATÓRIO)
        //                                                if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
        //                                                else
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, "00000000", 8);
        //                                                //CD_IDENTIFICA ou -> CPF_TIT : TODO ? Tratar a exceção para FALSE (OBRIGATÓRIO)
        //                                                if (!String.IsNullOrEmpty(strBeneficiarioTitularCPF))
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, strBeneficiarioTitularCPF.Trim().PadLeft(14, '0'), 14);
        //                                                else
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 14);
        //                                                //SQ_USU
        //                                                EntityBase.AppendPreparedField(ref sbFileBuffer, intBeneficiarioSequencia.ToString().PadLeft(2, '0'), 2);
        //                                                //CD_CID_INICIAL
        //                                                if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.CIDInicial))
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.CIDInicial, 4);
        //                                                else
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 4);
        //                                                //CD_CID_FINAL
        //                                                if (!String.IsNullOrEmpty(itemDeclaracaoInstancia.CIDFinal))
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, itemDeclaracaoInstancia.CIDFinal, 4);
        //                                                else
        //                                                    EntityBase.AppendPreparedField(ref sbFileBuffer, String.Empty, 4);

        //                                                sbFileBuffer.AppendLine();

        //                                                #endregion
        //                                            }
        //                                        }

        //                                        itemDeclaracao = null;
        //                                    }

        //                                    lstDeclaracaoSaude = null;

        //                                    #endregion
        //                                }
        //                        }

        //                        #region Build Lote

        //                        itemLote = new ArqTransacionalLoteItem();

        //                        itemLote.ContratoID = contrato.ID;
        //                        itemLote.BeneficiarioID = beneficiarioID;
        //                        itemLote.BeneficiarioSequencia = intBeneficiarioSequencia;
        //                        itemLote.Ativo = true;

        //                        lote.Itens.Add(itemLote);

        //                        #endregion
        //                    }

        //                    #region Seção U99

        //                    //NR_SEQ
        //                    sbFileBuffer.Append((++intArqNumeroSequencia).ToString().PadLeft(6, '0'));
        //                    //TP_REG
        //                    sbFileBuffer.Append(U99);
        //                    //QD_U02
        //                    EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU02.ToString().PadLeft(6, '0'), 6);
        //                    //QD_U03
        //                    EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU03.ToString().PadLeft(6, '0'), 6);
        //                    //QD_U04
        //                    EntityBase.AppendPreparedField(ref sbFileBuffer, "000000", 6);
        //                    //QD_U07
        //                    EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU07U08.ToString().PadLeft(6, '0'), 6);
        //                    //QD_U08
        //                    EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeRegistroU07U08.ToString().PadLeft(6, '0'), 6);
        //                    ////QD_U10
        //                    //sbFileBuffer.Append("000000");
        //                    //QD_INC
        //                    EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioInclusao.ToString().PadLeft(6, '0'), 6);
        //                    //QD_EXC
        //                    EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioExclusao.ToString().PadLeft(6, '0'), 6);
        //                    //QD_USU
        //                    EntityBase.AppendPreparedField(ref sbFileBuffer, intQtdeBeneficiarioAlteracao.ToString().PadLeft(6, '0'), 6);

        //                    #endregion

        //                    if (intQtdeBeneficiarioInclusao > 0)
        //                        lote.Quantidade = intQtdeBeneficiarioInclusao;
        //                    else if (intQtdeBeneficiarioAlteracao > 0)
        //                        lote.Quantidade = intQtdeBeneficiarioAlteracao;
        //                    else if (intQtdeBeneficiarioExclusao > 0)
        //                        lote.Quantidade = intQtdeBeneficiarioExclusao;

        //                    if (lote.Itens != null && lote.Itens.Count > 0)
        //                    {
        //                        try
        //                        {
        //                            lote.Salvar(true, PM);
        //                        }
        //                        catch (Exception) { throw; }

        //                        String strArquivoNome = lote.Arquivo;

        //                        try
        //                        {
        //                            this.SalvarArqTransacional(sbFileBuffer, strArquivoNome);
        //                        }
        //                        catch (Exception) { throw; }

        //                        criacaoOK = true;
        //                        ArquivoNome = strArquivoNome;

        //                        PM.Commit();
        //                    }
        //                    else
        //                    {
        //                        PM.Rollback();
        //                    }
        //                }
        //                else
        //                {
        //                    PM.Rollback();
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            PM.Rollback();
        //            throw;
        //        }
        //        finally
        //        {
        //            PM.Dispose();
        //            PM = null;
        //        }

        //        sbFileBuffer = null;
        //    }

        //    arqTransUnimed = null;

        //    return criacaoOK;
        //}

        String pegaTelefone(String fone)
        {
            if (String.IsNullOrEmpty(fone)) { return String.Empty; }

            String[] aux = fone.Split(')');
            if (aux.Length == 1) { return fone; }

            return aux[1].Trim();
        }

        String pegaDDD(String fone)
        {
            if (String.IsNullOrEmpty(fone)) { return String.Empty; }

            String[] aux = fone.Split(')');
            return aux[0].Replace("(", "").Trim();
        }

        /// <summary>
        /// Método para Gerar Arquivos
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        /// <param name="Mov">Inclusão de Beneficiário, Mudança de Plano, etc.</param>
        /// <param name="TipoMovimentacao">Inclusão, Alteração, Exclusão, etc.</param>
        /// <returns>True se gerou sem problemas, False se encontrou algum problema.</returns>
        private Boolean GerarArquivo(Object OperadoraID, ref String ArquivoNome, ContratoBeneficiario.eStatus Status, Object[] ContratoID, Object[] BeneficiarioID, String Mov, String TipoMov, DateTime vigencia)
        {
            Boolean criacaoOK = false;

            Int32 intQtdeBeneficiarioInclusao = 0;
            Int32 intQtdeBeneficiarioExclusao = 0;
            Int32 intQtdeBeneficiarioAlteracao = 0;

            ArqTransacionalLote lote = new ArqTransacionalLote();
            StringBuilder sbFileBuffer = new StringBuilder();

            PersistenceManager PM = new PersistenceManager();
            PM = new PersistenceManager();
            PM.BeginTransactionContext();

            Contrato contrato = null;

            //try
            //{
                if ((ContratoID != null && ContratoID.Length > 0) &&
                    (BeneficiarioID != null && BeneficiarioID.Length > 0) &&
                    (ContratoID.Length == BeneficiarioID.Length))
                {
                    for (Int32 i = 0; i < ContratoID.Length; i++)
                    {
                        try
                        {
                            //todo: descomentar linha abaixo e apagar esta. AGUARDAR OK do Marcio
                            //ContratoBeneficiario.AlteraStatusBeneficiario(ContratoID[i], BeneficiarioID[i], Status, PM);
                        }
                        catch (Exception) { throw; }
                    }
                }

                using (DataTable dtBeneficiarios = this.GetBeneficiarioPorStatus(OperadoraID, Status, ContratoID, BeneficiarioID, PM, vigencia, ContratoID[0]))
                {
                    if (dtBeneficiarios != null && dtBeneficiarios.Rows != null && dtBeneficiarios.Rows.Count > 0)
                    {
                        #region variaveis 

                        String celular = "", ddd = "";

                        AdicionalFaixa faixa = null;

                        ArqTransacionalLoteItem itemLote = null;
                        contrato = new Contrato();
                        ContratoADM contratoadm = new ContratoADM();
                        Plano plano = new Plano();
                        Object contratoID, beneficiarioID, beneficiarioEstadoCivilID;
                        String beneficiarioParentescoCod, strBeneficiarioNome, strBeneficiarioSexo, strBeneficiarioCPF, strBeneficiarioTitularCPF, strCodigoCarencia,
                               strBeneficiarioRG, strBeneficiarioEndereco, strBeneficiarioEnderecoNum, strBeneficiarioEnderecoCompl,
                               strBeneficiarioBairro, strBeneficiarioCEP, strBeneficiarioCidade, strBeneficiarioUF, strBeneficiarioTelefone,
                               strBeneficiarioTelefoneRamal, strBeneficiarioNomeMae, strBneficiarioPisPasep, strCodigoPlano; //strTipoMovimentacaoAux;
                        Int16 intBeneficiarioSequencia, intBeneficiarioTipo;
                        DateTime dtBeneficiarioDataNascimento, dtBeneficiarioDataCasamento, dtBeneficiarioVigencia, dtBeneficiarioCadastro; //vigenciaProposta;
                        IList<AdicionalBeneficiario> adicionais = null; //usado para carregar os adicionais de um beneficiario
                        List<AdicionalBeneficiario> adicionaisDaProposta = null; //usado para guardar todos os adicionais da proposta
                        Decimal totalDental = 0, totalOpcionais = 0;
                        ContratoBeneficiario titular = null;
                        Beneficiario beneficiario = null;
                        Endereco endereco = null;
                        IList<ItemDeclaracaoSaudeINSTANCIA> declaracaoSaude = null;
                        IList<ContratoBeneficiario> dependentes = null;
                        List<CobrancaComposite> composite = new List<CobrancaComposite>();
                        Estipulante estipulante = null;
                        Decimal decAux = 0;
                        List<String> contratoIds = new List<String>();
                        EstipulanteTaxa taxaAdesao = null;
                        IList<AdicionalFaixa> faixasAdicional = null;
                        List<String> controle = new List<String>();

                        #endregion

                        lote.OperadoraID = OperadoraID;
                        lote.Movimentacao = Mov;
                        lote.TipoMovimentacao = TipoMov;

                        #region Seção U01 - comentada 

                        //if (dtBeneficiarios.Rows.Count > 0)
                        //{
                        //    arqTransUnimed.OperadoraNaUnimed = Convert.ToString(dtBeneficiarios.Rows[0]["contratoadm_numero"]).Trim().PadLeft(8, '0');
                        //}

                        ////NR_SEQ
                        //sbFileBuffer.Append(intArqNumeroSequencia.ToString().PadLeft(6, '0'));
                        ////TP_REG
                        //sbFileBuffer.Append(U01);
                        ////CD_UNI
                        //if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCodSingular))
                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCodSingular, 4);
                        ////CD_EMP
                        //if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNaUnimed))
                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNaUnimed, 8);
                        ////NOME_EMP
                        //if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraNome))
                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraNome, 40);
                        ////CNPJ_EMP
                        //if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCNPJ))
                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCNPJ, 14);
                        ////TP_IDENTIFICA
                        //if (!String.IsNullOrEmpty(arqTransUnimed.TipoIdentificacao.ToString()))
                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.TipoIdentificacao, 1);
                        ////VERSAO
                        //if (!String.IsNullOrEmpty("4.0")) //arqTransUnimed.ArqVersao
                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.ArqVersao, 3);
                        ////DT_GERACAO
                        //sbFileBuffer.Append(DateTime.Now.ToString("ddMMyyyy"));
                        ////CEI_EMP
                        //if (!String.IsNullOrEmpty(arqTransUnimed.OperadoraCEI))
                        //    EntityBase.AppendPreparedField(ref sbFileBuffer, arqTransUnimed.OperadoraCEI, 13);

                        //sbFileBuffer.AppendLine();

                        #endregion

                        StringBuilder sb = new StringBuilder();
                        System.Collections.Hashtable htOperadoras = new System.Collections.Hashtable();

                        sb.AppendLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>");
                        sb.AppendLine("<IMPORTACAO>");

                        Boolean temDental = false;

                        foreach (DataRow row in dtBeneficiarios.Rows)
                        {
                            if (contratoIds.Contains(Convert.ToString(row["contrato_id"]))) { continue; }

                            contratoIds.Add(Convert.ToString(row["contrato_id"]));
                            adicionaisDaProposta = new List<AdicionalBeneficiario>();
                            totalDental = 0; totalOpcionais = 0;

                            #region incrementa contador de tipo de transacao 

                            switch (Mov)
                            {
                                case Movimentacao.InclusaoBeneficiario:
                                    intQtdeBeneficiarioInclusao++;
                                    break;
                                case Movimentacao.ExclusaoBeneficiario:
                                    intQtdeBeneficiarioExclusao++;
                                    break;
                                case Movimentacao.AlteracaoBeneficiario:
                                    intQtdeBeneficiarioAlteracao++;
                                    break;
                                case Movimentacao.MudancaDePlano:
                                    intQtdeBeneficiarioAlteracao++;
                                    break;
                            }
                            #endregion

                            contratoID = row["contratobeneficiario_contratoId"];
                            contrato.ID = contratoID;

                            #region carrega instâncias 

                            if (contrato.ID != null)
                            {
                                contrato.Carregar();
                                if (contrato.PlanoID != null)
                                {
                                    plano.ID = contrato.PlanoID;
                                    PM.Load(plano);
                                }

                                contratoadm.ID = contrato.ContratoADMID;
                                PM.Load(contratoadm);

                                estipulante = new Estipulante(contrato.EstipulanteID);
                                PM.Load(estipulante);

                                titular = ContratoBeneficiario.CarregarTitular(contrato.ID, PM);

                                adicionais = AdicionalBeneficiario.Carregar(contrato.ID, titular.BeneficiarioID, PM);
                                if (adicionais != null)
                                {
                                    adicionaisDaProposta.AddRange(adicionais);
                                }

                                beneficiario = new Beneficiario(titular.BeneficiarioID);
                                PM.Load(beneficiario);
                                endereco = new Endereco(contrato.EnderecoReferenciaID);
                                PM.Load(endereco);

                                declaracaoSaude = ItemDeclaracaoSaudeINSTANCIA.
                                    Carregar(beneficiario.ID, contrato.OperadoraID, PM);

                                dependentes = ContratoBeneficiario.CarregarPorContratoID(contrato.ID, true, true, PM);
                            }
                            #endregion

                            #region preenche variáveis 

                            beneficiarioID = row["contratobeneficiario_beneficiarioId"];
                            beneficiarioParentescoCod = (row["contratoAdmparentescoagregado_parentescoCodigo"] == null || row["contratoAdmparentescoagregado_parentescoCodigo"] is DBNull) ? "00" : Convert.ToString(row["contratoAdmparentescoagregado_parentescoCodigo"]);
                            beneficiarioEstadoCivilID = (row["contratobeneficiario_estadoCivilId"] == null || row["contratobeneficiario_estadoCivilId"] is DBNull) ? "0" : Convert.ToString(row["estadocivil_codigo"]);
                            strBeneficiarioNome = (row["beneficiario_nome"] == null || row["beneficiario_nome"] is DBNull) ? null : Convert.ToString(row["beneficiario_nome"]);
                            strBeneficiarioSexo = (row["beneficiario_sexo"] == null || row["beneficiario_sexo"] is DBNull) ? null : Convert.ToString(row["beneficiario_sexo"]);
                            strBeneficiarioCPF = (row["beneficiario_cpf"] == null || row["beneficiario_cpf"] is DBNull) ? "" : Convert.ToString(row["beneficiario_cpf"]);
                            strBeneficiarioCPF = strBeneficiarioCPF.Replace("99999999999", "00000000000");

                            strBeneficiarioTitularCPF = ContratoBeneficiario.GetCPFTitular(contratoID, PM);
                            strBeneficiarioRG = (row["beneficiario_rg"] == null || row["beneficiario_rg"] is DBNull) ? String.Empty : Convert.ToString(row["beneficiario_rg"]);
                            intBeneficiarioTipo = (row["contratobeneficiario_tipo"] == null || row["contratobeneficiario_tipo"] is DBNull) ? Convert.ToInt16(-1) : Convert.ToInt16(row["contratobeneficiario_tipo"]);
                            intBeneficiarioSequencia = (row["contratobeneficiario_numeroSequencia"] == null || row["contratobeneficiario_numeroSequencia"] is DBNull) ? Convert.ToInt16(0) : Convert.ToInt16(row["contratobeneficiario_numeroSequencia"]);
                            dtBeneficiarioDataNascimento = (row["beneficiario_dataNascimento"] == null || row["beneficiario_dataNascimento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["beneficiario_dataNascimento"]);
                            dtBeneficiarioVigencia = (row["contratobeneficiario_vigencia"] == null || row["contratobeneficiario_vigencia"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_vigencia"]);
                            dtBeneficiarioDataCasamento = (row["contratobeneficiario_dataCasamento"] == null || row["contratobeneficiario_dataCasamento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_dataCasamento"]);
                            dtBeneficiarioCadastro = (row["contratobeneficiario_data"] == null || row["contratobeneficiario_data"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_data"]);
                            strBeneficiarioEndereco = (row["endereco_logradouro"] == null || row["endereco_logradouro"] is DBNull) ? null : Convert.ToString(row["endereco_logradouro"]);
                            strBeneficiarioEnderecoNum = (row["endereco_numero"] == null || row["endereco_numero"] is DBNull) ? null : Convert.ToString(row["endereco_numero"]);
                            strBeneficiarioEnderecoCompl = (row["endereco_complemento"] == null || row["endereco_complemento"] is DBNull) ? null : Convert.ToString(row["endereco_complemento"]);
                            strBeneficiarioBairro = (row["endereco_bairro"] == null || row["endereco_bairro"] is DBNull) ? null : Convert.ToString(row["endereco_bairro"]);
                            strBeneficiarioCEP = (row["endereco_cep"] == null || row["endereco_cep"] is DBNull) ? null : Convert.ToString(row["endereco_cep"]);
                            strBeneficiarioCidade = (row["endereco_cidade"] == null || row["endereco_cidade"] is DBNull) ? null : Convert.ToString(row["endereco_cidade"]);
                            strBeneficiarioUF = (row["endereco_uf"] == null || row["endereco_uf"] is DBNull) ? null : Convert.ToString(row["endereco_uf"]);
                            strBeneficiarioTelefone = (row["beneficiario_telefone"] == null || row["beneficiario_telefone"] is DBNull) ? null : Convert.ToString(row["beneficiario_telefone"]);
                            strBeneficiarioTelefoneRamal = (row["beneficiario_ramal"] == null || row["beneficiario_ramal"] is DBNull) ? null : Convert.ToString(row["beneficiario_ramal"]);
                            strBeneficiarioNomeMae = (row["beneficiario_nomeMae"] == null || row["beneficiario_nomeMae"] is DBNull) ? null : Convert.ToString(row["beneficiario_nomeMae"]);
                            strCodigoCarencia = (row["contratobeneficiario_carenciaCodigo"] == null || row["contratobeneficiario_carenciaCodigo"] is DBNull) ? "" : Convert.ToString(row["contratobeneficiario_carenciaCodigo"]).PadLeft(2, '0');
                            strBneficiarioPisPasep = String.Empty;
                            strCodigoPlano = String.Empty;
                            //arqTransUnimed.OperadoraNaUnimed = Convert.ToString(row["contratoadm_numero"]).Trim().PadLeft(8, '0');
                            #endregion

                            sb.AppendLine("<PROPOSTA>");

                            sb.Append("<COD_ADMINISTRADORA>");
                            sb.Append(row["contratoadm_codAdministradora"]);
                            sb.AppendLine("</COD_ADMINISTRADORA>");

                            sb.Append("<COD_CORRETORA>");
                            sb.Append(EntityBase.CToString(row["usuario_codigo"]).PadLeft(6, '0'));
                            sb.AppendLine("</COD_CORRETORA>");

                            sb.Append("<COD_FILIAL>");
                            sb.Append(row["contratoadm_codFilial"]);
                            sb.AppendLine("</COD_FILIAL>");

                            sb.Append("<COD_UNIDADE>");
                            sb.Append(row["contratoadm_codUnidade"]);
                            sb.AppendLine("</COD_UNIDADE>");

                            sb.Append("<NOME_ENTIDADE>");
                            sb.Append(EntityBase.RetiraAcentos(estipulante.Descricao));
                            sb.AppendLine("</NOME_ENTIDADE>");

                            sb.Append("<NUM_CONTRATO>");
                            sb.Append(EntityBase.CToString(contratoadm.ContratoSaude).PadRight(9, '0'));
                            sb.AppendLine("</NUM_CONTRATO>");

                            temDental = false;
                            if (adicionais != null)
                            {
                                foreach (AdicionalBeneficiario ab in adicionais)
                                {
                                    if (AdicionalBeneficiario.EDental(ab))
                                    {
                                        temDental = true;
                                        break;
                                    }
                                }
                            }

                            if (!temDental)
                                sb.AppendLine("<NUM_CONTRATO_DENTAL />");
                            else
                            {
                                sb.Append("<NUM_CONTRATO_DENTAL>");
                                sb.Append(EntityBase.CToString(contratoadm.ContratoDental).PadRight(9, '0'));
                                sb.AppendLine("</NUM_CONTRATO_DENTAL>");
                            }

                            sb.Append("<NUM_PROPOSTA>");
                            sb.Append(contrato.Numero);
                            sb.AppendLine("</NUM_PROPOSTA>");

                            if (!String.IsNullOrEmpty(contrato.SuperiorTerceiroCPF))
                            {
                                sb.Append("<COD_SUPERVISOR>");
                                sb.Append(contrato.SuperiorTerceiroCPF);
                                sb.AppendLine("</COD_SUPERVISOR>");
                            }
                            else
                                sb.AppendLine("<COD_SUPERVISOR/>");

                            sb.Append("<COD_CORRETOR>");
                            sb.Append(contrato.CorretorTerceiroCPF);
                            sb.AppendLine("</COD_CORRETOR>");

                            sb.AppendLine("<NUM_DDD_TELEFONE_CORRETOR />");
                            sb.AppendLine("<NUM_TELEFONE_CORRETOR />");
                            sb.AppendLine("<END_EMAIL_CORRETOR />");

                            sb.Append("<COD_OPERADORA>");
                            if (EntityBase.CToString(row["contratoadm_descricao"]).ToLower().IndexOf("dix") > -1)
                                sb.Append("2");
                            else
                                sb.Append("1");
                            sb.AppendLine("</COD_OPERADORA>");

                            sb.Append("<DT_VENDA>");
                            sb.Append(contrato.Admissao.ToString("dd/MM/yyyy"));
                            sb.AppendLine("</DT_VENDA>");

                            sb.Append("<DT_INICIO_VIGENCIA>");
                            sb.Append(contrato.Vigencia.ToString("dd/MM/yyyy"));
                            sb.AppendLine("</DT_INICIO_VIGENCIA>");

                            //plano_codigo ou plano_codigoParticular
                            sb.Append("<COD_PLANO_MEDICO>");
                            if ((Contrato.eTipoAcomodacao)contrato.TipoAcomodacao == Contrato.eTipoAcomodacao.quartoComun)
                            { sb.Append(plano.Codigo); }
                            else { sb.Append(plano.CodigoParticular); }
                            sb.AppendLine("</COD_PLANO_MEDICO>");

                            //codigo titular do adicional 
                            sb.Append("<COD_PLANO_DENTAL>");
                            if (adicionais != null && adicionais.Count > 0)
                            {
                                sb.Append(adicionais[0].AdicionalCodTitular);
                            }
                            sb.AppendLine("</COD_PLANO_DENTAL>");

                            decAux = Contrato.CalculaValorDaPropostaSemTaxaAssocSemAdicional
                                (contrato, titular, contrato.Vencimento, PM);

                            sb.Append("<VAL_MENSALIDADE_TIT>");
                            sb.Append(decAux.ToString("N2").Replace(".", ","));
                            sb.AppendLine("</VAL_MENSALIDADE_TIT>");

                            if (!temDental)
                                sb.AppendLine("<VAL_MENSALIDADE_DENTAL_TIT />");
                            else
                            {
                                sb.Append("<VAL_MENSALIDADE_DENTAL_TIT>");
                                foreach (AdicionalBeneficiario ab in adicionais)
                                {
                                    if (AdicionalBeneficiario.EDental(ab))
                                    {
                                        faixasAdicional = AdicionalFaixa.CarregarPorTabela(ab.AdicionalID, null, PM);
                                        if (faixasAdicional != null && faixasAdicional.Count > 0)
                                        {
                                            sb.Append(faixasAdicional[0].Valor.ToString("N2").Replace(".", ","));
                                        }
                                        break;
                                    }
                                }
                                sb.AppendLine("</VAL_MENSALIDADE_DENTAL_TIT>");
                                
                            }

                            sb.AppendLine("<OPCIONAIS>");

                            if (adicionais != null && adicionais.Count > 0 && !temDental)
                            {
                                sb.AppendLine("<COD_OPCIONAL>");
                                sb.Append(adicionais[0].AdicionalCodTitular);
                                sb.Append("</COD_OPCIONAL>");
                                sb.Append("<VAL_OPCIONAL />");
                            }
                            else
                            {
                                sb.AppendLine("<COD_OPCIONAL />");
                                sb.Append("<VAL_OPCIONAL />");
                            }
                            
                            sb.AppendLine("</OPCIONAIS>");

                            if (EntityBase.CToString(titular.BeneficiarioCPF) != "" && titular.BeneficiarioCPF != "99999999999")
                            {
                                sb.Append("<NUM_CPF>");
                                sb.Append(titular.BeneficiarioCPF);
                                sb.AppendLine("</NUM_CPF>");
                            }
                            else
                                sb.Append("<NUM_CPF />");

                            sb.Append("<IND_ESTADO_CIVIL>");
                            sb.Append(titular.EstadoCivilCodigo);
                            sb.AppendLine("</IND_ESTADO_CIVIL>");

                            sb.Append("<NOME_TITULAR>");
                            sb.Append(EntityBase.RetiraAcentos(titular.BeneficiarioNome));
                            sb.AppendLine("</NOME_TITULAR>");

                            sb.Append("<NOME_MAE>");
                            sb.Append(EntityBase.RetiraAcentos(beneficiario.NomeMae));
                            sb.AppendLine("</NOME_MAE>");

                            sb.Append("<DATA_NASCIMENTO>");
                            sb.Append(beneficiario.DataNascimento.ToString("dd/MM/yyyy"));
                            sb.AppendLine("</DATA_NASCIMENTO>");

                            sb.Append("<IND_SEXO>");
                            if(beneficiario.Sexo == "1")
                                sb.Append("M");
                            else
                                sb.Append("F");
                            sb.AppendLine("</IND_SEXO>");

                            sb.Append("<NUM_IDENTIDADE>");
                            sb.Append(beneficiario.RG);
                            sb.AppendLine("</NUM_IDENTIDADE>");


                            sb.AppendLine("<COD_ORGAC_EMISSOR />");
                            sb.AppendLine("<COD_PAIS_EMISSOR />");

                            sb.Append("<NUM_CEP>");
                            sb.Append(endereco.CEP);
                            sb.AppendLine("</NUM_CEP>");

                            sb.Append("<NOM_LOGRADOURO>");
                            sb.Append(EntityBase.RetiraAcentos(endereco.Logradouro));
                            sb.AppendLine("</NOM_LOGRADOURO>");

                            sb.Append("<NUM_ENDERECO>");
                            sb.Append(EntityBase.RetiraAcentos(endereco.Numero));
                            sb.AppendLine("</NUM_ENDERECO>");

                            sb.Append("<TXT_COMPLEMENTO>");
                            sb.Append(EntityBase.RetiraAcentos(endereco.Complemento));
                            sb.AppendLine("</TXT_COMPLEMENTO>");

                            sb.Append("<COD_MUNICIPIO>");
                            sb.AppendLine("</COD_MUNICIPIO>");

                            sb.Append("<NOM_MUNICIPIO>");
                            sb.Append(EntityBase.RetiraAcentos(endereco.Cidade));
                            sb.AppendLine("</NOM_MUNICIPIO>");

                            sb.Append("<COD_BAIRRO>");
                            sb.AppendLine("</COD_BAIRRO>");

                            sb.Append("<NOM_BAIRRO>");
                            sb.Append(EntityBase.RetiraAcentos(endereco.Bairro));
                            sb.AppendLine("</NOM_BAIRRO>");

                            sb.Append("<SGL_UF>");
                            sb.Append(EntityBase.RetiraAcentos(endereco.UF));
                            sb.AppendLine("</SGL_UF>");

                            sb.Append("<DDD_TELEFONE_1>");
                            sb.Append(pegaDDD(beneficiario.Telefone));
                            sb.AppendLine("</DDD_TELEFONE_1>");

                            sb.Append("<NUM_TELEFONE_1>");
                            sb.Append(pegaTelefone(beneficiario.Telefone));
                            sb.AppendLine("</NUM_TELEFONE_1>");

                            ddd = pegaDDD(beneficiario.Celular);
                            sb.Append("<DDD_CELULAR>");
                            sb.Append(ddd);
                            sb.AppendLine("</DDD_CELULAR>");

                            celular = pegaTelefone(beneficiario.FCelular).Replace("-", "");
                            if (celular.Length == 8 && ddd == "11") { celular = String.Concat("9", celular); }
                            sb.Append("<NUM_CELULAR>");
                            sb.Append(celular);
                            sb.AppendLine("</NUM_CELULAR>");

                            sb.Append("<DDD_TELEFONE_2>");
                            sb.Append(pegaDDD(beneficiario.Telefone2));
                            sb.AppendLine("</DDD_TELEFONE_2>");

                            sb.Append("<NUM_TELEFONE_2>");
                            sb.Append(pegaTelefone(beneficiario.Telefone2));
                            sb.AppendLine("</NUM_TELEFONE_2>");

                            sb.AppendLine("<NUM_RAMAL_2 />");
                            sb.AppendLine("<DDD_TELEFONE_3 />");
                            sb.AppendLine("<NUM_TELEFONE_3 />");

                            sb.Append("<END_EMAIL>");
                            sb.Append(EntityBase.RetiraAcentos(beneficiario.Email));
                            sb.AppendLine("</END_EMAIL>");

                            sb.Append("<NOM_RESP>");
                            sb.Append(EntityBase.RetiraAcentos(contrato.ResponsavelNome));
                            sb.AppendLine("</NOM_RESP>");

                            sb.Append("<NUM_CPF_RESP>");
                            sb.Append(contrato.ResponsavelCPF);
                            sb.AppendLine("</NUM_CPF_RESP>");

                            sb.Append("<DATA_NASCIMENTO_RESP>");
                            if (contrato.ResponsavelDataNascimento != DateTime.MinValue)
                                sb.Append(contrato.ResponsavelDataNascimento.ToString("dd/MM/yyyy"));
                            sb.AppendLine("</DATA_NASCIMENTO_RESP>");

                            sb.AppendLine("<IND_SEXO_RESP />");

                            if (EntityBase.CToString(row["operadoraorigem_codigo"]) == String.Empty)
                                sb.AppendLine("<COD_OPERADORA_ORIGEM />");
                            else
                            {
                                sb.Append("<COD_OPERADORA_ORIGEM>");
                                sb.Append(row["operadoraorigem_codigo"]);
                                sb.AppendLine("</COD_OPERADORA_ORIGEM>");
                            }

                            if (String.IsNullOrEmpty(titular.CarenciaCodigo))
                            {
                                sb.AppendLine("<NUM_PRODUTO_OPER_ORIGEM />");
                                sb.AppendLine("<NUM_CONTRATO_ORIGEM />");
                                sb.AppendLine("<NUM_REGISTRO_ANS_ORIGEM />");
                            }
                            else
                            {
                                sb.AppendLine("<NUM_PRODUTO_OPER_ORIGEM>1</NUM_PRODUTO_OPER_ORIGEM>");
                                sb.AppendLine("<NUM_CONTRATO_ORIGEM>1</NUM_CONTRATO_ORIGEM>");
                                sb.AppendLine("<NUM_REGISTRO_ANS_ORIGEM>1</NUM_REGISTRO_ANS_ORIGEM>");
                            }

                            if (row["contratobeneficiario_carenciaContratoDataDe"] != DBNull.Value)
                            {
                                sb.Append("<DT_INI_VIG_ORIGEM>");
                                sb.Append(Convert.ToDateTime(row["contratobeneficiario_carenciaContratoDataDe"]).ToString("dd/MM/yyyy"));
                                sb.AppendLine("</DT_INI_VIG_ORIGEM>");
                            }
                            else
                                sb.AppendLine("<DT_INI_VIG_ORIGEM />");

                            if (row["contratobeneficiario_carenciaContratoDataAte"] != DBNull.Value)
                            {
                                sb.Append("<DT_ULTIMO_PGTO>");
                                sb.Append(Convert.ToDateTime(row["contratobeneficiario_carenciaContratoDataAte"]).ToString("dd/MM/yyyy"));
                                sb.AppendLine("</DT_ULTIMO_PGTO>");
                            }
                            else
                                sb.AppendLine("<DT_ULTIMO_PGTO />");

                            if (String.IsNullOrEmpty(titular.CarenciaCodigo))
                                sb.AppendLine("<COD_PRC />");
                            else
                            {
                                sb.Append("<COD_PRC>");

                                if (Operadora.IsAmil_SemCodPrcJR_NaExportacao(OperadoraID))
                                    sb.Append(titular.CarenciaCodigo.ToUpper().Replace("JR", ""));
                                else
                                    sb.Append(titular.CarenciaCodigo);

                                sb.AppendLine("</COD_PRC>");
                            }

                            sb.AppendLine("<COD_OPERADORA_ORIGEM_DENTAL />");
                            sb.AppendLine("<NUM_PROD_OPER_ORIGEM_DENTAL />");
                            sb.AppendLine("<NUM_CONTRATO_ORIGEM_DENTAL />");
                            sb.AppendLine("<NUM_REGISTRO_ANS_ORIGEM_DENTAL />");
                            sb.AppendLine("<DT_INI_VIG_ORIGEM_DENTAL />");
                            sb.AppendLine("<DT_ULTIMO_PGTO_DENTAL />");
                            if (!AdicionalBeneficiario.TemDental(adicionais))
                            {
                                sb.AppendLine("<COD_PRC_ODONTO />");
                            }
                            else
                            {
                                sb.Append("<COD_PRC_ODONTO>");
                                sb.Append(ConfigurationManager.AppSettings["amil_prc_odonto"]);
                                sb.AppendLine("</COD_PRC_ODONTO>");
                            }

                            sb.Append("<PESO>");
                            sb.Append(titular.Peso.ToString("N0"));
                            sb.AppendLine("</PESO>");

                            sb.Append("<ALTURA>");
                            sb.Append(titular.Altura.ToString("N2").Replace(",", "."));
                            sb.AppendLine("</ALTURA>");

                            sb.AppendLine("<ENTREVISTA />");

                            if (declaracaoSaude != null)
                            {
                                controle.Clear();
                                foreach (ItemDeclaracaoSaudeINSTANCIA item in declaracaoSaude)
                                {
                                    if (controle.Contains(Convert.ToString(item.ItemDeclaracaoID))) { continue; }
                                    controle.Add(Convert.ToString(item.ItemDeclaracaoID));

                                    sb.AppendLine("<DECLARACAO_SAUDE>");

                                    sb.Append("<NUM_PERGUNTA>");
                                    sb.Append(EntityBase.RetiraAcentos(item.ItemDeclaracaoCodigo));
                                    sb.AppendLine("</NUM_PERGUNTA>");

                                    if (item.Sim)
                                    {
                                        sb.AppendLine("<RESPOSTA>S</RESPOSTA>");
                                        sb.Append("<ANO_EVENTO>");
                                        sb.Append(item.Data.Year);
                                        sb.AppendLine("</ANO_EVENTO>");
                                        sb.Append("<TXT_RESPOSTA>");
                                        sb.Append(EntityBase.RetiraAcentos(item.Descricao));
                                        sb.AppendLine("</TXT_RESPOSTA>");
                                    }
                                    else
                                    {
                                        sb.AppendLine("<RESPOSTA>N</RESPOSTA>");
                                        sb.AppendLine("<ANO_EVENTO />");
                                        sb.AppendLine("<TXT_RESPOSTA />");
                                    }
                                    
                                    sb.AppendLine("</DECLARACAO_SAUDE>");
                                }
                            }
                            else
                                sb.AppendLine("<DECLARACAO_SAUDE />");

                            #region dependentes 

                            if (dependentes != null)
                            {
                                foreach (ContratoBeneficiario dependente in dependentes)
                                {
                                    sb.AppendLine("<DEPENDENTE>");

                                    sb.Append("<COD_DEPENDENCIA>");
                                    sb.Append(dependente.ParentescoCodigo);
                                    sb.AppendLine("</COD_DEPENDENCIA>");

                                    sb.AppendLine("<ORDEM_DEPENDENTE />");

                                    sb.Append("<NOME_DEPENDENTE>");
                                    sb.Append(EntityBase.RetiraAcentos(dependente.BeneficiarioNome));
                                    sb.AppendLine("</NOME_DEPENDENTE>");


                                    if (EntityBase.CToString(dependente.BeneficiarioCPF) != "" && dependente.BeneficiarioCPF != "99999999999")
                                    {
                                        sb.Append("<NUM_CPF>");
                                        sb.Append(dependente.BeneficiarioCPF);
                                        sb.AppendLine("</NUM_CPF>");
                                    }
                                    else
                                        sb.Append("<NUM_CPF />");

                                    sb.Append("<IND_ESTADO_CIVIL>");
                                    sb.Append(dependente.EstadoCivilCodigo);
                                    sb.AppendLine("</IND_ESTADO_CIVIL>");

                                    sb.Append("<DATA_NASCIMENTO>");
                                    sb.Append(dependente.BeneficiarioDataNascimento.ToString("dd/MM/yyyy"));
                                    sb.AppendLine("</DATA_NASCIMENTO>");

                                    sb.Append("<IND_SEXO>");
                                    if (dependente.BeneficiarioSexo == "1")
                                        sb.Append("M");
                                    else
                                        sb.Append("F");
                                    sb.AppendLine("</IND_SEXO>");

                                    sb.Append("<NOME_MAE>");
                                    sb.Append(EntityBase.RetiraAcentos(dependente.BeneficiarioNomeMae));
                                    sb.AppendLine("</NOME_MAE>");

                                    adicionais = AdicionalBeneficiario.Carregar(contrato.ID, titular.BeneficiarioID, PM);
                                    if (adicionais != null)
                                    {
                                        adicionaisDaProposta.AddRange(adicionais);
                                    }

                                    sb.Append("<IND_OPTOU_DENTAL>");
                                    if (!AdicionalBeneficiario.TemDental(adicionais))
                                        sb.Append("N");
                                    else
                                        sb.Append("S");
                                    sb.AppendLine("</IND_OPTOU_DENTAL>");

                                    decAux = Contrato.CalculaValorDaPropostaSemTaxaAssocSemAdicional(contrato, dependente, contrato.Vencimento, PM);

                                    sb.AppendLine("<VAL_MENSALIDADE_DEP>");
                                    sb.Append(decAux.ToString("N2").Replace(".", ","));
                                    sb.AppendLine("</VAL_MENSALIDADE_DEP>");

                                    if (!temDental)
                                        sb.AppendLine("<VAL_MENSALIDADE_DENTAL_DEP />");
                                    else
                                    {
                                        sb.Append("<VAL_MENSALIDADE_DENTAL_DEP>");
                                        foreach (AdicionalBeneficiario ab in adicionais)
                                        {
                                            if (AdicionalBeneficiario.EDental(ab))
                                            {
                                                if(faixasAdicional == null || faixasAdicional.Count == 0)
                                                    faixasAdicional = AdicionalFaixa.CarregarPorTabela(ab.AdicionalID, null, PM);

                                                if (faixasAdicional != null && faixasAdicional.Count > 0)
                                                {
                                                    sb.Append(faixasAdicional[0].Valor.ToString("N2").Replace(".", ","));
                                                }
                                                break;
                                            }
                                        }
                                        sb.AppendLine("</VAL_MENSALIDADE_DENTAL_DEP>");
                                    }

                                    if (String.IsNullOrEmpty(dependente.CarenciaCodigo))// || (dependente.CarenciaCodigo != "JR398" && dependente.CarenciaCodigo != "JR129"))
                                        sb.AppendLine("<COD_PRC_DEP />");
                                    else
                                    {
                                        sb.Append("<COD_PRC_DEP>");

                                        //sb.Append(dependente.CarenciaCodigo); //.ToUpper().Replace("JR", "")
                                        if (Operadora.IsAmil_SemCodPrcJR_NaExportacao(OperadoraID))
                                            sb.Append(dependente.CarenciaCodigo.ToUpper().Replace("JR", ""));
                                        else
                                            sb.Append(dependente.CarenciaCodigo);
                                        sb.AppendLine("</COD_PRC_DEP>");
                                    }
                                    sb.AppendLine("<COD_PRC_ODONTO_DEP />");

                                    if (adicionais == null)
                                        sb.Append("<OPCIONAIS />");
                                    else
                                    {
                                        foreach (AdicionalBeneficiario ab in adicionais)
                                        {
                                            sb.Append("<OPCIONAIS>");

                                            if (!temDental)
                                            {
                                                sb.Append("<COD_OPCIONAL>");
                                                sb.Append(ab.AdicionalCodTitular);
                                                sb.Append("</COD_OPCIONAL>");
                                            }
                                            else
                                                sb.Append("<COD_OPCIONAL />");

                                            sb.Append("<VAL_OPCIONAL />");
                                            sb.AppendLine("</OPCIONAIS>");
                                            break; //////////////envio soh o primeiro? 
                                        }
                                    }
                                    adicionais = null;

                                    sb.Append("<PESO>");
                                    sb.Append(dependente.Peso.ToString("N0"));
                                    sb.AppendLine("</PESO>");

                                    sb.Append("<ALTURA>");
                                    sb.Append(dependente.Altura.ToString("N2").Replace(",", "."));
                                    sb.AppendLine("</ALTURA>");

                                    sb.AppendLine("<ENTREVISTA />");

                                    declaracaoSaude = ItemDeclaracaoSaudeINSTANCIA.
                                        Carregar(dependente.BeneficiarioID, contrato.OperadoraID, PM);

                                    if (declaracaoSaude != null)
                                    {
                                        controle.Clear();
                                        foreach (ItemDeclaracaoSaudeINSTANCIA item in declaracaoSaude)
                                        {
                                            //if (!item.Sim) { continue; }
                                            if (controle.Contains(Convert.ToString(item.ItemDeclaracaoID))) { continue; }
                                            controle.Add(Convert.ToString(item.ItemDeclaracaoID));

                                            sb.AppendLine("<DECLARACAO_SAUDE>");

                                            sb.Append("<NUM_PERGUNTA>");
                                            sb.Append(EntityBase.RetiraAcentos(item.ItemDeclaracaoCodigo));
                                            sb.AppendLine("</NUM_PERGUNTA>");

                                            if (item.Sim)
                                            {
                                                sb.AppendLine("<RESPOSTA>S</RESPOSTA>");
                                                sb.Append("<ANO_EVENTO>");
                                                sb.Append(item.Data.Year);
                                                sb.AppendLine("</ANO_EVENTO>");
                                                sb.Append("<TXT_RESPOSTA>");
                                                sb.Append(EntityBase.RetiraAcentos(item.Descricao));
                                                sb.AppendLine("</TXT_RESPOSTA>");
                                            }
                                            else
                                            {
                                                sb.AppendLine("<RESPOSTA>N</RESPOSTA>");
                                                sb.AppendLine("<ANO_EVENTO />");
                                                sb.AppendLine("<TXT_RESPOSTA />");
                                            }

                                            sb.AppendLine("</DECLARACAO_SAUDE>");
                                        }
                                    }
                                    else
                                        sb.AppendLine("<DECLARACAO_SAUDE />");

                                    sb.AppendLine("</DEPENDENTE>");
                                }
                            }
                            else
                                sb.AppendLine("<DEPENDENTE />");

                            #endregion

                            sb.Append("<VAL_TAXA_ADESAO>");
                            if (contrato.CobrarTaxaAssociativa) //sempre zero
                            {
                                sb.Append("0");
                            }
                            else
                                sb.Append("0");
                            sb.AppendLine("</VAL_TAXA_ADESAO>");

                            if (composite != null) { composite.Clear(); }
                            decAux = Contrato.CalculaValorDaProposta(contrato.ID, contrato.Vencimento, PM, true, false, ref composite); //sem a taxa associativa

                            totalDental = 0; totalOpcionais = 0;
                            foreach (AdicionalBeneficiario ab in adicionaisDaProposta)
                            {
                                faixasAdicional = AdicionalFaixa.CarregarPorTabela(ab.AdicionalID, null, PM);
                                if (faixasAdicional == null) { continue; }

                                if (AdicionalBeneficiario.EDental(ab))
                                    totalDental += faixasAdicional[0].Valor;
                                else
                                    totalOpcionais += faixasAdicional[0].Valor;
                            }

                            sb.Append("<VAL_TOTAL_MENSALIDADE>");
                            sb.Append((decAux - totalDental - totalOpcionais).ToString("N2").Replace(".", ""));
                            sb.AppendLine("</VAL_TOTAL_MENSALIDADE>");

                            sb.Append("<VAL_TOTAL_MENSALIDADE_DENTAL>");
                            sb.Append(totalDental.ToString("N2").Replace(".", ""));
                            sb.AppendLine("</VAL_TOTAL_MENSALIDADE_DENTAL>");

                            sb.Append("<VAL_TOTAL_OPCIONAIS>");
                            sb.Append(totalOpcionais.ToString("N2").Replace(".", ""));
                            sb.AppendLine("</VAL_TOTAL_OPCIONAIS>");

                            sb.Append("<VAL_TOTAL_PROPOSTA>");
                            sb.Append(decAux.ToString("N2").Replace(".", ""));
                            sb.AppendLine("</VAL_TOTAL_PROPOSTA>");

                            sb.AppendLine("</PROPOSTA>");

                            #region Build Lote 

                            itemLote = new ArqTransacionalLoteItem();

                            itemLote.ContratoID = contrato.ID;
                            itemLote.BeneficiarioID = beneficiarioID;
                            itemLote.BeneficiarioSequencia = intBeneficiarioSequencia;
                            itemLote.Ativo = true;

                            lote.Itens.Add(itemLote); 

                            #endregion

                        } //fim do loop principal

                        sb.Append("</IMPORTACAO>");

                        if (intQtdeBeneficiarioInclusao > 0)
                            lote.Quantidade = intQtdeBeneficiarioInclusao;
                        else if (intQtdeBeneficiarioAlteracao > 0)
                            lote.Quantidade = intQtdeBeneficiarioAlteracao;
                        else if (intQtdeBeneficiarioExclusao > 0)
                            lote.Quantidade = intQtdeBeneficiarioExclusao;

                        if (lote.Itens != null && lote.Itens.Count > 0)
                        {
                            try
                            {
                                lote.Salvar("AM", false, PM);
                                lote.Arquivo += ".xml";
                            }
                            catch (Exception) { throw; }

                            String strArquivoNome = lote.Arquivo;

                            try
                            {
                                this.SalvarArqTransacional(sb, strArquivoNome); //sbFileBuffer
                            }
                            catch (Exception) { throw; }

                            criacaoOK = true;
                            ArquivoNome = strArquivoNome;

                            PM.Rollback(); //voltar o commit. Aguardar ok do Marcio
                        }
                        else
                        {
                            PM.Rollback();
                        }
                    }
                    else
                    {
                        PM.Rollback();
                    }
                }
            //}
            //catch (Exception)
            //{
            //    PM.Rollback();
            //    throw;
            //}
            //finally
            //{
            //    PM.Dispose();
            //    PM = null;
            //}

            sbFileBuffer = null;

            return criacaoOK;
        }

        private Boolean GerarArquivoDeInclusaoPorContratoIDs(ref String ArquivoNome, String[] contratoIDs, String Mov, String TipoMov)
        {
            Boolean criacaoOK = false;

            Int32 intQtdeBeneficiarioInclusao = 0;
            Int32 intQtdeBeneficiarioExclusao = 0;
            Int32 intQtdeBeneficiarioAlteracao = 0;

            ArqTransacionalLote lote = new ArqTransacionalLote();
            StringBuilder sbFileBuffer = new StringBuilder();

            PersistenceManager PM = new PersistenceManager();
            PM = new PersistenceManager();
            PM.BeginTransactionContext();

            Contrato contrato = null;

            //try
            //{
            if (contratoIDs != null && contratoIDs.Length > 0)
            {
                for (Int32 i = 0; i < contratoIDs.Length; i++)
                {
                    try
                    {
                        //todo: descomentar linha abaixo e apagar esta. Aguardar ok do Marcio
                        //ContratoBeneficiario.AlteraStatusBeneficiario(ContratoID[i], BeneficiarioID[i], Status, PM);
                    }
                    catch (Exception) { throw; }
                }
            }

            using (DataTable dtBeneficiarios = Untyped.UntypedProcesses.GetBeneficiarios(contratoIDs, null))
            {
                if (dtBeneficiarios != null && dtBeneficiarios.Rows != null && dtBeneficiarios.Rows.Count > 0)
                {
                    #region variaveis

                    String celular = "", ddd = "";

                    AdicionalFaixa faixa = null;

                    ArqTransacionalLoteItem itemLote = null;
                    contrato = new Contrato();
                    ContratoADM contratoadm = new ContratoADM();
                    Plano plano = new Plano();
                    Object contratoID, beneficiarioID, beneficiarioEstadoCivilID;
                    String beneficiarioParentescoCod, strBeneficiarioNome, strBeneficiarioSexo, strBeneficiarioCPF, strBeneficiarioTitularCPF, strCodigoCarencia,
                           strBeneficiarioRG, strBeneficiarioEndereco, strBeneficiarioEnderecoNum, strBeneficiarioEnderecoCompl,
                           strBeneficiarioBairro, strBeneficiarioCEP, strBeneficiarioCidade, strBeneficiarioUF, strBeneficiarioTelefone,
                           strBeneficiarioTelefoneRamal, strBeneficiarioNomeMae, strBneficiarioPisPasep, strCodigoPlano; //strTipoMovimentacaoAux;
                    Int16 intBeneficiarioSequencia, intBeneficiarioTipo;
                    DateTime dtBeneficiarioDataNascimento, dtBeneficiarioDataCasamento, dtBeneficiarioVigencia, dtBeneficiarioCadastro; //vigenciaProposta;
                    IList<AdicionalBeneficiario> adicionais = null; //usado para carregar os adicionais de um beneficiario
                    List<AdicionalBeneficiario> adicionaisDaProposta = null; //usado para guardar todos os adicionais da proposta
                    Decimal totalDental = 0, totalOpcionais = 0;
                    ContratoBeneficiario titular = null;
                    Beneficiario beneficiario = null;
                    Endereco endereco = null;
                    IList<ItemDeclaracaoSaudeINSTANCIA> declaracaoSaude = null;
                    IList<ContratoBeneficiario> dependentes = null;
                    List<CobrancaComposite> composite = new List<CobrancaComposite>();
                    Estipulante estipulante = null;
                    Decimal decAux = 0;
                    List<String> contratoIds = new List<String>();
                    EstipulanteTaxa taxaAdesao = null;
                    IList<AdicionalFaixa> faixasAdicional = null;
                    List<String> controle = new List<String>();

                    #endregion

                    lote.OperadoraID = dtBeneficiarios.Rows[0]["operadora_id"];
                    lote.Movimentacao = Mov;
                    lote.TipoMovimentacao = TipoMov;

                    StringBuilder sb = new StringBuilder();
                    System.Collections.Hashtable htOperadoras = new System.Collections.Hashtable();

                    sb.AppendLine("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>");
                    sb.AppendLine("<IMPORTACAO>");

                    Boolean temDental = false;

                    foreach (DataRow row in dtBeneficiarios.Rows)
                    {
                        if (contratoIds.Contains(Convert.ToString(row["contrato_id"]))) { continue; }

                        contratoIds.Add(Convert.ToString(row["contrato_id"]));
                        adicionaisDaProposta = new List<AdicionalBeneficiario>();
                        totalDental = 0; totalOpcionais = 0;

                        #region incrementa contador de tipo de transacao

                        switch (Mov)
                        {
                            case Movimentacao.InclusaoBeneficiario:
                                intQtdeBeneficiarioInclusao++;
                                break;
                            case Movimentacao.ExclusaoBeneficiario:
                                intQtdeBeneficiarioExclusao++;
                                break;
                            case Movimentacao.AlteracaoBeneficiario:
                                intQtdeBeneficiarioAlteracao++;
                                break;
                            case Movimentacao.MudancaDePlano:
                                intQtdeBeneficiarioAlteracao++;
                                break;
                        }
                        #endregion

                        contratoID = row["contratobeneficiario_contratoId"];
                        contrato.ID = contratoID;

                        #region carrega instâncias

                        if (contrato.ID != null)
                        {
                            contrato.Carregar();
                            if (contrato.PlanoID != null)
                            {
                                plano.ID = contrato.PlanoID;
                                PM.Load(plano);
                            }

                            contratoadm.ID = contrato.ContratoADMID;
                            PM.Load(contratoadm);

                            estipulante = new Estipulante(contrato.EstipulanteID);
                            PM.Load(estipulante);

                            titular = ContratoBeneficiario.CarregarTitular(contrato.ID, PM);

                            adicionais = AdicionalBeneficiario.Carregar(contrato.ID, titular.BeneficiarioID, PM);
                            if (adicionais != null)
                            {
                                adicionaisDaProposta.AddRange(adicionais);
                            }

                            beneficiario = new Beneficiario(titular.BeneficiarioID);
                            PM.Load(beneficiario);
                            endereco = new Endereco(contrato.EnderecoReferenciaID);
                            PM.Load(endereco);

                            declaracaoSaude = ItemDeclaracaoSaudeINSTANCIA.
                                Carregar(beneficiario.ID, contrato.OperadoraID, PM);

                            dependentes = ContratoBeneficiario.CarregarPorContratoID(contrato.ID, true, true, PM);
                        }
                        #endregion

                        #region preenche variáveis

                        beneficiarioID = row["contratobeneficiario_beneficiarioId"];
                        beneficiarioParentescoCod = (row["contratoAdmparentescoagregado_parentescoCodigo"] == null || row["contratoAdmparentescoagregado_parentescoCodigo"] is DBNull) ? "00" : Convert.ToString(row["contratoAdmparentescoagregado_parentescoCodigo"]);
                        beneficiarioEstadoCivilID = (row["contratobeneficiario_estadoCivilId"] == null || row["contratobeneficiario_estadoCivilId"] is DBNull) ? "0" : Convert.ToString(row["estadocivil_codigo"]);
                        strBeneficiarioNome = (row["beneficiario_nome"] == null || row["beneficiario_nome"] is DBNull) ? null : Convert.ToString(row["beneficiario_nome"]);
                        strBeneficiarioSexo = (row["beneficiario_sexo"] == null || row["beneficiario_sexo"] is DBNull) ? null : Convert.ToString(row["beneficiario_sexo"]);
                        strBeneficiarioCPF = (row["beneficiario_cpf"] == null || row["beneficiario_cpf"] is DBNull) ? "" : Convert.ToString(row["beneficiario_cpf"]);
                        strBeneficiarioCPF = strBeneficiarioCPF.Replace("99999999999", "00000000000");

                        strBeneficiarioTitularCPF = ContratoBeneficiario.GetCPFTitular(contratoID, PM);
                        strBeneficiarioRG = (row["beneficiario_rg"] == null || row["beneficiario_rg"] is DBNull) ? String.Empty : Convert.ToString(row["beneficiario_rg"]);
                        intBeneficiarioTipo = (row["contratobeneficiario_tipo"] == null || row["contratobeneficiario_tipo"] is DBNull) ? Convert.ToInt16(-1) : Convert.ToInt16(row["contratobeneficiario_tipo"]);
                        intBeneficiarioSequencia = (row["contratobeneficiario_numeroSequencia"] == null || row["contratobeneficiario_numeroSequencia"] is DBNull) ? Convert.ToInt16(0) : Convert.ToInt16(row["contratobeneficiario_numeroSequencia"]);
                        dtBeneficiarioDataNascimento = (row["beneficiario_dataNascimento"] == null || row["beneficiario_dataNascimento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["beneficiario_dataNascimento"]);
                        dtBeneficiarioVigencia = (row["contratobeneficiario_vigencia"] == null || row["contratobeneficiario_vigencia"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_vigencia"]);
                        dtBeneficiarioDataCasamento = (row["contratobeneficiario_dataCasamento"] == null || row["contratobeneficiario_dataCasamento"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_dataCasamento"]);
                        dtBeneficiarioCadastro = (row["contratobeneficiario_data"] == null || row["contratobeneficiario_data"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(row["contratobeneficiario_data"]);
                        strBeneficiarioEndereco = (row["endereco_logradouro"] == null || row["endereco_logradouro"] is DBNull) ? null : Convert.ToString(row["endereco_logradouro"]);
                        strBeneficiarioEnderecoNum = (row["endereco_numero"] == null || row["endereco_numero"] is DBNull) ? null : Convert.ToString(row["endereco_numero"]);
                        strBeneficiarioEnderecoCompl = (row["endereco_complemento"] == null || row["endereco_complemento"] is DBNull) ? null : Convert.ToString(row["endereco_complemento"]);
                        strBeneficiarioBairro = (row["endereco_bairro"] == null || row["endereco_bairro"] is DBNull) ? null : Convert.ToString(row["endereco_bairro"]);
                        strBeneficiarioCEP = (row["endereco_cep"] == null || row["endereco_cep"] is DBNull) ? null : Convert.ToString(row["endereco_cep"]);
                        strBeneficiarioCidade = (row["endereco_cidade"] == null || row["endereco_cidade"] is DBNull) ? null : Convert.ToString(row["endereco_cidade"]);
                        strBeneficiarioUF = (row["endereco_uf"] == null || row["endereco_uf"] is DBNull) ? null : Convert.ToString(row["endereco_uf"]);
                        strBeneficiarioTelefone = (row["beneficiario_telefone"] == null || row["beneficiario_telefone"] is DBNull) ? null : Convert.ToString(row["beneficiario_telefone"]);
                        strBeneficiarioTelefoneRamal = (row["beneficiario_ramal"] == null || row["beneficiario_ramal"] is DBNull) ? null : Convert.ToString(row["beneficiario_ramal"]);
                        strBeneficiarioNomeMae = (row["beneficiario_nomeMae"] == null || row["beneficiario_nomeMae"] is DBNull) ? null : Convert.ToString(row["beneficiario_nomeMae"]);
                        strCodigoCarencia = (row["contratobeneficiario_carenciaCodigo"] == null || row["contratobeneficiario_carenciaCodigo"] is DBNull) ? "" : Convert.ToString(row["contratobeneficiario_carenciaCodigo"]).PadLeft(2, '0');
                        strBneficiarioPisPasep = String.Empty;
                        strCodigoPlano = String.Empty;
                        //arqTransUnimed.OperadoraNaUnimed = Convert.ToString(row["contratoadm_numero"]).Trim().PadLeft(8, '0');
                        #endregion

                        sb.AppendLine("<PROPOSTA>");

                        sb.Append("<COD_ADMINISTRADORA>");
                        sb.Append(row["contratoadm_codAdministradora"]);
                        sb.AppendLine("</COD_ADMINISTRADORA>");

                        sb.Append("<COD_CORRETORA>");
                        sb.Append(EntityBase.CToString(row["usuario_codigo"]).PadLeft(6, '0'));
                        sb.AppendLine("</COD_CORRETORA>");

                        sb.Append("<COD_FILIAL>");
                        sb.Append(row["contratoadm_codFilial"]);
                        sb.AppendLine("</COD_FILIAL>");

                        sb.Append("<COD_UNIDADE>");
                        sb.Append(row["contratoadm_codUnidade"]);
                        sb.AppendLine("</COD_UNIDADE>");

                        sb.Append("<NOME_ENTIDADE>");
                        sb.Append(EntityBase.RetiraAcentos(estipulante.Descricao));
                        sb.AppendLine("</NOME_ENTIDADE>");

                        sb.Append("<NUM_CONTRATO>");
                        sb.Append(EntityBase.CToString(contratoadm.ContratoSaude).PadRight(9, '0'));
                        sb.AppendLine("</NUM_CONTRATO>");

                        temDental = false;
                        if (adicionais != null)
                        {
                            foreach (AdicionalBeneficiario ab in adicionais)
                            {
                                if (AdicionalBeneficiario.EDental(ab))
                                {
                                    temDental = true;
                                    break;
                                }
                            }
                        }

                        if (!temDental)
                            sb.AppendLine("<NUM_CONTRATO_DENTAL />");
                        else
                        {
                            sb.Append("<NUM_CONTRATO_DENTAL>");
                            sb.Append(EntityBase.CToString(contratoadm.ContratoDental).PadRight(9, '0'));
                            sb.AppendLine("</NUM_CONTRATO_DENTAL>");
                        }

                        sb.Append("<NUM_PROPOSTA>");
                        sb.Append(contrato.Numero);
                        sb.AppendLine("</NUM_PROPOSTA>");

                        if (!String.IsNullOrEmpty(contrato.SuperiorTerceiroCPF))
                        {
                            sb.Append("<COD_SUPERVISOR>");
                            sb.Append(contrato.SuperiorTerceiroCPF);
                            sb.AppendLine("</COD_SUPERVISOR>");
                        }
                        else
                            sb.AppendLine("<COD_SUPERVISOR/>");

                        sb.Append("<COD_CORRETOR>");
                        sb.Append(contrato.CorretorTerceiroCPF);
                        sb.AppendLine("</COD_CORRETOR>");

                        sb.AppendLine("<NUM_DDD_TELEFONE_CORRETOR />");
                        sb.AppendLine("<NUM_TELEFONE_CORRETOR />");
                        sb.AppendLine("<END_EMAIL_CORRETOR />");

                        sb.Append("<COD_OPERADORA>");
                        if (EntityBase.CToString(row["contratoadm_descricao"]).ToLower().IndexOf("dix") > -1)
                            sb.Append("2");
                        else
                            sb.Append("1");
                        sb.AppendLine("</COD_OPERADORA>");

                        sb.Append("<DT_VENDA>");
                        sb.Append(contrato.Admissao.ToString("dd/MM/yyyy"));
                        sb.AppendLine("</DT_VENDA>");

                        sb.Append("<DT_INICIO_VIGENCIA>");
                        sb.Append(contrato.Vigencia.ToString("dd/MM/yyyy"));
                        sb.AppendLine("</DT_INICIO_VIGENCIA>");

                        //plano_codigo ou plano_codigoParticular
                        sb.Append("<COD_PLANO_MEDICO>");
                        if ((Contrato.eTipoAcomodacao)contrato.TipoAcomodacao == Contrato.eTipoAcomodacao.quartoComun)
                        { sb.Append(plano.Codigo); }
                        else { sb.Append(plano.CodigoParticular); }
                        sb.AppendLine("</COD_PLANO_MEDICO>");

                        //codigo titular do adicional 
                        sb.Append("<COD_PLANO_DENTAL>");
                        if (adicionais != null && adicionais.Count > 0)
                        {
                            sb.Append(adicionais[0].AdicionalCodTitular);
                        }
                        sb.AppendLine("</COD_PLANO_DENTAL>");

                        decAux = Contrato.CalculaValorDaPropostaSemTaxaAssocSemAdicional
                            (contrato, titular, contrato.Vencimento, PM);

                        sb.Append("<VAL_MENSALIDADE_TIT>");
                        sb.Append(decAux.ToString("N2").Replace(".", ","));
                        sb.AppendLine("</VAL_MENSALIDADE_TIT>");

                        if (!temDental)
                            sb.AppendLine("<VAL_MENSALIDADE_DENTAL_TIT />");
                        else
                        {
                            sb.Append("<VAL_MENSALIDADE_DENTAL_TIT>");
                            foreach (AdicionalBeneficiario ab in adicionais)
                            {
                                if (AdicionalBeneficiario.EDental(ab))
                                {
                                    faixasAdicional = AdicionalFaixa.CarregarPorTabela(ab.AdicionalID, null, PM);
                                    if (faixasAdicional != null && faixasAdicional.Count > 0)
                                    {
                                        sb.Append(faixasAdicional[0].Valor.ToString("N2").Replace(".", ","));
                                    }
                                    break;
                                }
                            }
                            sb.AppendLine("</VAL_MENSALIDADE_DENTAL_TIT>");

                        }

                        sb.AppendLine("<OPCIONAIS>");

                        if (adicionais != null && adicionais.Count > 0 && !temDental)
                        {
                            sb.AppendLine("<COD_OPCIONAL>");
                            sb.Append(adicionais[0].AdicionalCodTitular);
                            sb.Append("</COD_OPCIONAL>");
                            sb.Append("<VAL_OPCIONAL />");
                        }
                        else
                        {
                            sb.AppendLine("<COD_OPCIONAL />");
                            sb.Append("<VAL_OPCIONAL />");
                        }

                        sb.AppendLine("</OPCIONAIS>");

                        if (EntityBase.CToString(titular.BeneficiarioCPF) != "" && titular.BeneficiarioCPF != "99999999999")
                        {
                            sb.Append("<NUM_CPF>");
                            sb.Append(titular.BeneficiarioCPF);
                            sb.AppendLine("</NUM_CPF>");
                        }
                        else
                            sb.Append("<NUM_CPF />");

                        sb.Append("<IND_ESTADO_CIVIL>");
                        sb.Append(titular.EstadoCivilCodigo);
                        sb.AppendLine("</IND_ESTADO_CIVIL>");

                        sb.Append("<NOME_TITULAR>");
                        sb.Append(EntityBase.RetiraAcentos(titular.BeneficiarioNome));
                        sb.AppendLine("</NOME_TITULAR>");

                        sb.Append("<NOME_MAE>");
                        sb.Append(EntityBase.RetiraAcentos(beneficiario.NomeMae));
                        sb.AppendLine("</NOME_MAE>");

                        sb.Append("<DATA_NASCIMENTO>");
                        sb.Append(beneficiario.DataNascimento.ToString("dd/MM/yyyy"));
                        sb.AppendLine("</DATA_NASCIMENTO>");

                        sb.Append("<IND_SEXO>");
                        if (beneficiario.Sexo == "1")
                            sb.Append("M");
                        else
                            sb.Append("F");
                        sb.AppendLine("</IND_SEXO>");

                        sb.Append("<NUM_IDENTIDADE>");
                        sb.Append(beneficiario.RG);
                        sb.AppendLine("</NUM_IDENTIDADE>");


                        sb.AppendLine("<COD_ORGAC_EMISSOR />");
                        sb.AppendLine("<COD_PAIS_EMISSOR />");

                        sb.Append("<NUM_CEP>");
                        sb.Append(endereco.CEP);
                        sb.AppendLine("</NUM_CEP>");

                        sb.Append("<NOM_LOGRADOURO>");
                        sb.Append(EntityBase.RetiraAcentos(endereco.Logradouro));
                        sb.AppendLine("</NOM_LOGRADOURO>");

                        sb.Append("<NUM_ENDERECO>");
                        sb.Append(EntityBase.RetiraAcentos(endereco.Numero));
                        sb.AppendLine("</NUM_ENDERECO>");

                        sb.Append("<TXT_COMPLEMENTO>");
                        sb.Append(EntityBase.RetiraAcentos(endereco.Complemento));
                        sb.AppendLine("</TXT_COMPLEMENTO>");

                        sb.Append("<COD_MUNICIPIO>");
                        sb.AppendLine("</COD_MUNICIPIO>");

                        sb.Append("<NOM_MUNICIPIO>");
                        sb.Append(EntityBase.RetiraAcentos(endereco.Cidade));
                        sb.AppendLine("</NOM_MUNICIPIO>");

                        sb.Append("<COD_BAIRRO>");
                        sb.AppendLine("</COD_BAIRRO>");

                        sb.Append("<NOM_BAIRRO>");
                        sb.Append(EntityBase.RetiraAcentos(endereco.Bairro));
                        sb.AppendLine("</NOM_BAIRRO>");

                        sb.Append("<SGL_UF>");
                        sb.Append(EntityBase.RetiraAcentos(endereco.UF));
                        sb.AppendLine("</SGL_UF>");

                        sb.Append("<DDD_TELEFONE_1>");
                        sb.Append(pegaDDD(beneficiario.Telefone));
                        sb.AppendLine("</DDD_TELEFONE_1>");

                        sb.Append("<NUM_TELEFONE_1>");
                        sb.Append(pegaTelefone(beneficiario.Telefone));
                        sb.AppendLine("</NUM_TELEFONE_1>");

                        ddd = pegaDDD(beneficiario.Celular);
                        sb.Append("<DDD_CELULAR>");
                        sb.Append(ddd);
                        sb.AppendLine("</DDD_CELULAR>");

                        celular = pegaTelefone(beneficiario.FCelular).Replace("-", "");
                        if (celular.Length == 8 && ddd == "11") { celular = String.Concat("9", celular); }
                        sb.Append("<NUM_CELULAR>");
                        sb.Append(celular);
                        sb.AppendLine("</NUM_CELULAR>");

                        sb.Append("<DDD_TELEFONE_2>");
                        sb.Append(pegaDDD(beneficiario.Telefone2));
                        sb.AppendLine("</DDD_TELEFONE_2>");

                        sb.Append("<NUM_TELEFONE_2>");
                        sb.Append(pegaTelefone(beneficiario.Telefone2));
                        sb.AppendLine("</NUM_TELEFONE_2>");

                        sb.AppendLine("<NUM_RAMAL_2 />");
                        sb.AppendLine("<DDD_TELEFONE_3 />");
                        sb.AppendLine("<NUM_TELEFONE_3 />");

                        sb.Append("<END_EMAIL>");
                        sb.Append(EntityBase.RetiraAcentos(beneficiario.Email));
                        sb.AppendLine("</END_EMAIL>");

                        sb.Append("<NOM_RESP>");
                        sb.Append(EntityBase.RetiraAcentos(contrato.ResponsavelNome));
                        sb.AppendLine("</NOM_RESP>");

                        sb.Append("<NUM_CPF_RESP>");
                        sb.Append(contrato.ResponsavelCPF);
                        sb.AppendLine("</NUM_CPF_RESP>");

                        sb.Append("<DATA_NASCIMENTO_RESP>");
                        if (contrato.ResponsavelDataNascimento != DateTime.MinValue)
                            sb.Append(contrato.ResponsavelDataNascimento.ToString("dd/MM/yyyy"));
                        sb.AppendLine("</DATA_NASCIMENTO_RESP>");

                        sb.AppendLine("<IND_SEXO_RESP />");

                        if (EntityBase.CToString(row["operadoraorigem_codigo"]) == String.Empty)
                            sb.AppendLine("<COD_OPERADORA_ORIGEM />");
                        else
                        {
                            sb.Append("<COD_OPERADORA_ORIGEM>");
                            sb.Append(row["operadoraorigem_codigo"]);
                            sb.AppendLine("</COD_OPERADORA_ORIGEM>");
                        }

                        if (String.IsNullOrEmpty(titular.CarenciaCodigo))
                        {
                            sb.AppendLine("<NUM_PRODUTO_OPER_ORIGEM />");
                            sb.AppendLine("<NUM_CONTRATO_ORIGEM />");
                            sb.AppendLine("<NUM_REGISTRO_ANS_ORIGEM />");
                        }
                        else
                        {
                            sb.AppendLine("<NUM_PRODUTO_OPER_ORIGEM>1</NUM_PRODUTO_OPER_ORIGEM>");
                            sb.AppendLine("<NUM_CONTRATO_ORIGEM>1</NUM_CONTRATO_ORIGEM>");
                            sb.AppendLine("<NUM_REGISTRO_ANS_ORIGEM>1</NUM_REGISTRO_ANS_ORIGEM>");
                        }

                        if (row["contratobeneficiario_carenciaContratoDataDe"] != DBNull.Value)
                        {
                            sb.Append("<DT_INI_VIG_ORIGEM>");
                            sb.Append(Convert.ToDateTime(row["contratobeneficiario_carenciaContratoDataDe"]).ToString("dd/MM/yyyy"));
                            sb.AppendLine("</DT_INI_VIG_ORIGEM>");
                        }
                        else
                            sb.AppendLine("<DT_INI_VIG_ORIGEM />");

                        if (row["contratobeneficiario_carenciaContratoDataAte"] != DBNull.Value)
                        {
                            sb.Append("<DT_ULTIMO_PGTO>");
                            sb.Append(Convert.ToDateTime(row["contratobeneficiario_carenciaContratoDataAte"]).ToString("dd/MM/yyyy"));
                            sb.AppendLine("</DT_ULTIMO_PGTO>");
                        }
                        else
                            sb.AppendLine("<DT_ULTIMO_PGTO />");

                        if (String.IsNullOrEmpty(titular.CarenciaCodigo)) // || (titular.CarenciaCodigo != "JR398" && titular.CarenciaCodigo != "JR129"))
                            sb.AppendLine("<COD_PRC />");
                        else
                        {
                            sb.Append("<COD_PRC>");
                            //sb.Append(titular.CarenciaCodigo); //.ToUpper().Replace("JR", "")
                            if (Operadora.IsAmil_SemCodPrcJR_NaExportacao(lote.OperadoraID))
                                sb.Append(titular.CarenciaCodigo.ToUpper().Replace("JR", ""));
                            else
                                sb.Append(titular.CarenciaCodigo);

                            sb.AppendLine("</COD_PRC>");
                        }

                        sb.AppendLine("<COD_OPERADORA_ORIGEM_DENTAL />");
                        sb.AppendLine("<NUM_PROD_OPER_ORIGEM_DENTAL />");
                        sb.AppendLine("<NUM_CONTRATO_ORIGEM_DENTAL />");
                        sb.AppendLine("<NUM_REGISTRO_ANS_ORIGEM_DENTAL />");
                        sb.AppendLine("<DT_INI_VIG_ORIGEM_DENTAL />");
                        sb.AppendLine("<DT_ULTIMO_PGTO_DENTAL />");

                        if (!AdicionalBeneficiario.TemDental(adicionais))
                        {
                            sb.AppendLine("<COD_PRC_ODONTO />");
                        }
                        else
                        {
                            sb.Append("<COD_PRC_ODONTO>");
                            sb.Append(ConfigurationManager.AppSettings["amil_prc_odonto"]);
                            sb.AppendLine("</COD_PRC_ODONTO>");
                        }

                        sb.Append("<PESO>");
                        sb.Append(titular.Peso.ToString("N0"));
                        sb.AppendLine("</PESO>");

                        sb.Append("<ALTURA>");
                        sb.Append(titular.Altura.ToString("N2").Replace(",", "."));
                        sb.AppendLine("</ALTURA>");

                        sb.AppendLine("<ENTREVISTA />");

                        if (declaracaoSaude != null)
                        {
                            controle.Clear();
                            foreach (ItemDeclaracaoSaudeINSTANCIA item in declaracaoSaude)
                            {
                                //if (!item.Sim) { continue; }
                                if (controle.Contains(Convert.ToString(item.ItemDeclaracaoID))) { continue; }
                                controle.Add(Convert.ToString(item.ItemDeclaracaoID));

                                sb.AppendLine("<DECLARACAO_SAUDE>");

                                sb.Append("<NUM_PERGUNTA>");
                                sb.Append(EntityBase.RetiraAcentos(item.ItemDeclaracaoCodigo));
                                sb.AppendLine("</NUM_PERGUNTA>");

                                if (item.Sim)
                                {
                                    sb.AppendLine("<RESPOSTA>S</RESPOSTA>");
                                    sb.Append("<ANO_EVENTO>");
                                    sb.Append(item.Data.Year);
                                    sb.AppendLine("</ANO_EVENTO>");
                                    sb.Append("<TXT_RESPOSTA>");
                                    sb.Append(EntityBase.RetiraAcentos(item.Descricao));
                                    sb.AppendLine("</TXT_RESPOSTA>");
                                }
                                else
                                {
                                    sb.AppendLine("<RESPOSTA>N</RESPOSTA>");
                                    sb.AppendLine("<ANO_EVENTO />");
                                    sb.AppendLine("<TXT_RESPOSTA />");
                                }

                                sb.AppendLine("</DECLARACAO_SAUDE>");
                            }
                        }
                        else
                            sb.AppendLine("<DECLARACAO_SAUDE />");

                        #region dependentes

                        if (dependentes != null)
                        {
                            foreach (ContratoBeneficiario dependente in dependentes)
                            {
                                sb.AppendLine("<DEPENDENTE>");

                                sb.Append("<COD_DEPENDENCIA>");
                                sb.Append(dependente.ParentescoCodigo);
                                sb.AppendLine("</COD_DEPENDENCIA>");

                                sb.AppendLine("<ORDEM_DEPENDENTE />");

                                sb.Append("<NOME_DEPENDENTE>");
                                sb.Append(EntityBase.RetiraAcentos(dependente.BeneficiarioNome));
                                sb.AppendLine("</NOME_DEPENDENTE>");


                                if (EntityBase.CToString(dependente.BeneficiarioCPF) != "" && dependente.BeneficiarioCPF != "99999999999")
                                {
                                    sb.Append("<NUM_CPF>");
                                    sb.Append(dependente.BeneficiarioCPF);
                                    sb.AppendLine("</NUM_CPF>");
                                }
                                else
                                    sb.Append("<NUM_CPF />");

                                sb.Append("<IND_ESTADO_CIVIL>");
                                sb.Append(dependente.EstadoCivilCodigo);
                                sb.AppendLine("</IND_ESTADO_CIVIL>");

                                sb.Append("<DATA_NASCIMENTO>");
                                sb.Append(dependente.BeneficiarioDataNascimento.ToString("dd/MM/yyyy"));
                                sb.AppendLine("</DATA_NASCIMENTO>");

                                sb.Append("<IND_SEXO>");
                                if (dependente.BeneficiarioSexo == "1")
                                    sb.Append("M");
                                else
                                    sb.Append("F");
                                sb.AppendLine("</IND_SEXO>");

                                sb.Append("<NOME_MAE>");
                                sb.Append(EntityBase.RetiraAcentos(dependente.BeneficiarioNomeMae));
                                sb.AppendLine("</NOME_MAE>");

                                adicionais = AdicionalBeneficiario.Carregar(contrato.ID, titular.BeneficiarioID, PM);
                                if (adicionais != null)
                                {
                                    adicionaisDaProposta.AddRange(adicionais);
                                }

                                sb.Append("<IND_OPTOU_DENTAL>");
                                if (!AdicionalBeneficiario.TemDental(adicionais))
                                    sb.Append("N");
                                else
                                    sb.Append("S");
                                sb.AppendLine("</IND_OPTOU_DENTAL>");

                                decAux = Contrato.CalculaValorDaPropostaSemTaxaAssocSemAdicional(contrato, dependente, contrato.Vencimento, PM);

                                sb.AppendLine("<VAL_MENSALIDADE_DEP>");
                                sb.Append(decAux.ToString("N2").Replace(".", ","));
                                sb.AppendLine("</VAL_MENSALIDADE_DEP>");

                                if (!temDental)
                                    sb.AppendLine("<VAL_MENSALIDADE_DENTAL_DEP />");
                                else
                                {
                                    sb.Append("<VAL_MENSALIDADE_DENTAL_DEP>");
                                    foreach (AdicionalBeneficiario ab in adicionais)
                                    {
                                        if (AdicionalBeneficiario.EDental(ab))
                                        {
                                            if (faixasAdicional == null || faixasAdicional.Count == 0)
                                                faixasAdicional = AdicionalFaixa.CarregarPorTabela(ab.AdicionalID, null, PM);

                                            if (faixasAdicional != null && faixasAdicional.Count > 0)
                                            {
                                                sb.Append(faixasAdicional[0].Valor.ToString("N2").Replace(".", ","));
                                            }
                                            break;
                                        }
                                    }
                                    sb.AppendLine("</VAL_MENSALIDADE_DENTAL_DEP>");
                                }

                                if (String.IsNullOrEmpty(dependente.CarenciaCodigo))// || (dependente.CarenciaCodigo != "JR398" && dependente.CarenciaCodigo != "JR129"))
                                    sb.AppendLine("<COD_PRC_DEP />");
                                else
                                {
                                    sb.Append("<COD_PRC_DEP>");

                                    //sb.Append(dependente.CarenciaCodigo); //.ToUpper().Replace("JR", "")
                                    if (Operadora.IsAmil_SemCodPrcJR_NaExportacao(lote.OperadoraID))
                                        sb.Append(dependente.CarenciaCodigo.ToUpper().Replace("JR", ""));
                                    else
                                        sb.Append(dependente.CarenciaCodigo);

                                    sb.AppendLine("</COD_PRC_DEP>");
                                }
                                sb.AppendLine("<COD_PRC_ODONTO_DEP />");

                                if (adicionais == null)
                                    sb.Append("<OPCIONAIS />");
                                else
                                {
                                    foreach (AdicionalBeneficiario ab in adicionais)
                                    {
                                        sb.Append("<OPCIONAIS>");

                                        if (!temDental)
                                        {
                                            sb.Append("<COD_OPCIONAL>");
                                            sb.Append(ab.AdicionalCodTitular);
                                            sb.Append("</COD_OPCIONAL>");
                                        }
                                        else
                                            sb.Append("<COD_OPCIONAL />");

                                        sb.Append("<VAL_OPCIONAL />");
                                        sb.AppendLine("</OPCIONAIS>");
                                        break; //////////////envio soh o primeiro? 
                                    }
                                }
                                adicionais = null;

                                sb.Append("<PESO>");
                                sb.Append(dependente.Peso.ToString("N0"));
                                sb.AppendLine("</PESO>");

                                sb.Append("<ALTURA>");
                                sb.Append(dependente.Altura.ToString("N2").Replace(",", "."));
                                sb.AppendLine("</ALTURA>");

                                sb.AppendLine("<ENTREVISTA />");

                                declaracaoSaude = ItemDeclaracaoSaudeINSTANCIA.
                                    Carregar(dependente.BeneficiarioID, contrato.OperadoraID, PM);

                                if (declaracaoSaude != null)
                                {
                                    controle.Clear();
                                    foreach (ItemDeclaracaoSaudeINSTANCIA item in declaracaoSaude)
                                    {
                                        //if (!item.Sim) { continue; }
                                        if (controle.Contains(Convert.ToString(item.ItemDeclaracaoID))) { continue; }
                                        controle.Add(Convert.ToString(item.ItemDeclaracaoID));

                                        sb.AppendLine("<DECLARACAO_SAUDE>");

                                        sb.Append("<NUM_PERGUNTA>");
                                        sb.Append(EntityBase.RetiraAcentos(item.ItemDeclaracaoCodigo));
                                        sb.AppendLine("</NUM_PERGUNTA>");

                                        if (item.Sim)
                                        {
                                            sb.AppendLine("<RESPOSTA>S</RESPOSTA>");
                                            sb.Append("<ANO_EVENTO>");
                                            sb.Append(item.Data.Year);
                                            sb.AppendLine("</ANO_EVENTO>");
                                            sb.Append("<TXT_RESPOSTA>");
                                            sb.Append(EntityBase.RetiraAcentos(item.Descricao));
                                            sb.AppendLine("</TXT_RESPOSTA>");
                                        }
                                        else
                                        {
                                            sb.AppendLine("<RESPOSTA>N</RESPOSTA>");
                                            sb.AppendLine("<ANO_EVENTO />");
                                            sb.AppendLine("<TXT_RESPOSTA />");
                                        }

                                        sb.AppendLine("</DECLARACAO_SAUDE>");
                                    }
                                }
                                else
                                    sb.AppendLine("<DECLARACAO_SAUDE />");

                                sb.AppendLine("</DEPENDENTE>");
                            }
                        }
                        else
                            sb.AppendLine("<DEPENDENTE />");

                        #endregion

                        sb.Append("<VAL_TAXA_ADESAO>");
                        if (contrato.CobrarTaxaAssociativa) //sempre zero
                        {
                            //taxaAdesao = EstipulanteTaxa.CarregarVigente(contrato.EstipulanteID, PM);
                            //if (taxaAdesao != null && taxaAdesao.Valor > Decimal.Zero)
                            //    sb.Append(taxaAdesao.Valor.ToString("N2"));
                            //else
                            sb.Append("0");
                        }
                        else
                            sb.Append("0");
                        sb.AppendLine("</VAL_TAXA_ADESAO>");

                        if (composite != null) { composite.Clear(); }
                        decAux = Contrato.CalculaValorDaProposta(contrato.ID, contrato.Vencimento, PM, true, false, ref composite); //sem a taxa associativa

                        totalDental = 0; totalOpcionais = 0;
                        foreach (AdicionalBeneficiario ab in adicionaisDaProposta)
                        {
                            faixasAdicional = AdicionalFaixa.CarregarPorTabela(ab.AdicionalID, null, PM);
                            if (faixasAdicional == null) { continue; }

                            if (AdicionalBeneficiario.EDental(ab))
                                totalDental += faixasAdicional[0].Valor;
                            else
                                totalOpcionais += faixasAdicional[0].Valor;
                        }

                        sb.Append("<VAL_TOTAL_MENSALIDADE>");
                        sb.Append((decAux - totalDental - totalOpcionais).ToString("N2").Replace(".", ""));
                        sb.AppendLine("</VAL_TOTAL_MENSALIDADE>");

                        sb.Append("<VAL_TOTAL_MENSALIDADE_DENTAL>");
                        sb.Append(totalDental.ToString("N2").Replace(".", ""));
                        sb.AppendLine("</VAL_TOTAL_MENSALIDADE_DENTAL>");

                        sb.Append("<VAL_TOTAL_OPCIONAIS>");
                        sb.Append(totalOpcionais.ToString("N2").Replace(".", ""));
                        sb.AppendLine("</VAL_TOTAL_OPCIONAIS>");

                        sb.Append("<VAL_TOTAL_PROPOSTA>");
                        sb.Append(decAux.ToString("N2").Replace(".", ""));
                        sb.AppendLine("</VAL_TOTAL_PROPOSTA>");

                        sb.AppendLine("</PROPOSTA>");

                        #region Build Lote

                        itemLote = new ArqTransacionalLoteItem();

                        itemLote.ContratoID = contrato.ID;
                        itemLote.BeneficiarioID = beneficiarioID;
                        itemLote.BeneficiarioSequencia = intBeneficiarioSequencia;
                        itemLote.Ativo = true;

                        lote.Itens.Add(itemLote);

                        #endregion

                    } //fim do loop principal

                    sb.Append("</IMPORTACAO>");

                    if (intQtdeBeneficiarioInclusao > 0)
                        lote.Quantidade = intQtdeBeneficiarioInclusao;
                    else if (intQtdeBeneficiarioAlteracao > 0)
                        lote.Quantidade = intQtdeBeneficiarioAlteracao;
                    else if (intQtdeBeneficiarioExclusao > 0)
                        lote.Quantidade = intQtdeBeneficiarioExclusao;

                    if (lote.Itens != null && lote.Itens.Count > 0)
                    {
                        try
                        {
                            lote.Salvar("AM", false, PM);
                            lote.Arquivo += ".xml";
                        }
                        catch (Exception) { throw; }

                        String strArquivoNome = lote.Arquivo;

                        try
                        {
                            this.SalvarArqTransacional(sb, strArquivoNome); 
                        }
                        catch (Exception) { throw; }

                        criacaoOK = true;
                        ArquivoNome = strArquivoNome;

                        PM.Rollback(); //PM.Commit(); voltar o commit - Aguardar ok do Marcio
                    }
                    else
                    {
                        PM.Rollback();
                    }
                }
                else
                {
                    PM.Rollback();
                }
            }
            //}
            //catch (Exception)
            //{
            //    PM.Rollback();
            //    throw;
            //}
            //finally
            //{
            //    PM.Dispose();
            //    PM = null;
            //}

            sbFileBuffer = null;

            return criacaoOK;
        }

        String TraduzTipoEndereco(String endereco)
        {
            String tipoEnd = endereco.Split(' ')[0].Trim().ToUpper();

            if (tipoEnd == "RUA")
                return "R  ";
            else if (tipoEnd == "ALAMEDA")
                return "AL ";
            else if (tipoEnd == "ACESSO")
                return "AC ";
            else if (tipoEnd == "AVENIDA" || tipoEnd == "AV")
                return "AV ";
            else if (tipoEnd == "BECO")
                return "BC ";
            else if (tipoEnd == "CHÁCARA" || tipoEnd == "CHACARA")
                return "CH ";
            else if (tipoEnd == "CONJUNTO")
                return "CJ ";
            else if (tipoEnd == "ESTRADA")
                return "EST";
            else if (tipoEnd == "FAVELA")
                return "FAV";
            else if (tipoEnd == "FAZENDA")
                return "FAZ";
            else if (tipoEnd == "GALERIA")
                return "GAL";
            else if (tipoEnd == "LARGO")
                return "LG ";
            else if (tipoEnd == "LOTEAMENTO")
                return "LOT";
            else if (tipoEnd == "PRAÇA" || tipoEnd == "PRACA")
                return "PC ";
            else if (tipoEnd == "QUADRA")
                return "Q  ";
            else if (tipoEnd == "RODOVIA")
                return "ROD";
            else if (tipoEnd == "SERRA")
                return "SER";
            else if (tipoEnd == "SERRA")
                return "SER";
            else if (tipoEnd == "SÍTIO" || tipoEnd == "SITIO")
                return "SIT";
            else if (tipoEnd == "SETOR")
                return "ST ";
            else if (tipoEnd == "TRAVESSA")
                return "TV ";
            else if (tipoEnd == "VILA")
                return "VL ";

            //if (tipoEnd.Length > 3) { tipoEnd = tipoEnd.Substring(0, 2); }
            return "R  ";  //tipoEnd;
        }

        #endregion

        #endregion

        #region Private Structs

        /// <summary>
        /// Tipo de Movimentação.
        /// </summary>
        private struct TipoMovimentacao
        {
            /// <summary>
            /// Inclusão.
            /// </summary>
            public static readonly String Inclusao = "I";

            /// <summary>
            /// Alteração.
            /// </summary>
            public static readonly String Alteracao = "A";

            /// <summary>
            /// Exclusão.
            /// </summary>
            public static readonly String Exclusao = "E";

            /// <summary>
            /// Emissão de 2ª.
            /// </summary>
            public static readonly String EmissaoSegundaVia = "C";
        }

        #endregion

        #region Public Structs

        /// <summary>
        /// Movimentação (Inclusao de Beneficiario, Mudança de Plano, etc).
        /// </summary>
        public struct Movimentacao
        {
            /// <summary>
            /// Inclusão de Beneficiário.
            /// </summary>
            public const String InclusaoBeneficiario = "IB";

            /// <summary>
            /// Alteração de Beneficiário.
            /// </summary>
            public const String AlteracaoBeneficiario = "AB";

            /// <summary>
            /// Exclusão de Beneficiário.
            /// </summary>
            public const String ExclusaoBeneficiario = "EB";

            /// <summary>
            /// Segunda via de Cartão do Benficiário.
            /// </summary>
            public const String SegundaViaCartaoBeneficiario = "SVCB";

            /// <summary>
            /// Mudança de Plano.
            /// </summary>
            public const String MudancaDePlano = "MDP";

            /// <summary>
            /// Cancelamento de Contrato.
            /// </summary>
            public const String CancelamentoContrato = "CC";
        }

        #endregion

        #region Private Constructors

        /// <summary>
        /// Construtor Privado.
        /// </summary>
        private ArqTransacionalAmil() { }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Contrutor para setar o Root Path.
        /// </summary>
        /// <param name="RootPath"></param>
        public ArqTransacionalAmil(String RootPath)
        {
            if (!String.IsNullOrEmpty(RootPath))
                this.ArqTransacionalRootPath = RootPath;
        }

        #endregion

        #region Public Methods

        #region GetBeneficiarioPorStatus

        /// <summary>
        /// Método para pegar os Beneficiario pelo o Status.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="Status">Status do Beneficiario.</param>
        /// <param name="ContratoID">Array com ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array com ID de Beneficiario.</param>
        /// <returns></returns>
        public DataTable GetBeneficiarioPorStatus(Object OperadoraID, ContratoBeneficiario.eStatus Status, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, DateTime vigencia, Object contratoAdmId)
        {
            return this.GetBeneficiarioPorStatus(OperadoraID, Status, ContratoID, BeneficiarioID, loteId, null, vigencia, contratoAdmId);
        }

        public DataTable GetBeneficiarioPorStatus(Object OperadoraID, ContratoBeneficiario.eStatus Status, Object[] ContratoID, Object[] BeneficiarioID, PersistenceManager PM, DateTime vigencia, Object contratoAdmId)
        {
            return GetBeneficiarioPorStatus(OperadoraID, Status, ContratoID, BeneficiarioID, null, null, vigencia, contratoAdmId);
        }

        /// <summary>
        /// Método para pegar os Beneficiario pelo o Status.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="Status">Status do Beneficiario.</param>
        /// <param name="ContratoID">Array com ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array com ID de Beneficiario.</param>
        /// <returns></returns>
        public DataTable GetBeneficiarioPorStatus(Object OperadoraID, ContratoBeneficiario.eStatus Status, Object[] ContratoID, Object[] BeneficiarioID, Object loteId, PersistenceManager PM, DateTime vigencia, Object contratoAdmId)
        {
            switch (Status)
            {
                case ContratoBeneficiario.eStatus.Novo:
                    return Untyped.UntypedProcesses.GetBeneficiarioInclusao(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia);
                case ContratoBeneficiario.eStatus.PendenteNaOperadora:
                    return Untyped.UntypedProcesses.GetBeneficiarioPendente(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.Devolvido:
                    return Untyped.UntypedProcesses.GetBeneficiarioDevolvido(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.AlteracaoCadastroPendente:
                    return Untyped.UntypedProcesses.GetBeneficiarioAlteracaoCadastroPendente(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.AlteracaoCadastroPendenteNaOperadora:
                    return Untyped.UntypedProcesses.GetBeneficiarioAlteracaoCadastroPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.AlteracaoCadastroDevolvido:
                    return Untyped.UntypedProcesses.GetBeneficiarioAlteracaoCadastroPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.SegundaViaCartaoPendente:
                    return Untyped.UntypedProcesses.GetBeneficiarioSegundaViaCartaoPendente(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.SegundaViaCartaoPendenteNaOperadora:
                    return Untyped.UntypedProcesses.GetBeneficiarioSegundaViaCartaoPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.SegundaViaCartaoDevolvido:
                    return Untyped.UntypedProcesses.GetBeneficiarioSegundaViaCartaoPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.ExclusaoPendente:
                    return Untyped.UntypedProcesses.GetBeneficiarioExclusaoPendente(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.ExclusaoPendenteNaOperadora:
                    return Untyped.UntypedProcesses.GetBeneficiarioExclusaoPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.ExclusaoDevolvido:
                    return Untyped.UntypedProcesses.GetBeneficiarioExclusaoPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.MudancaPlanoPendente:
                    return Untyped.UntypedProcesses.GetBeneficiarioMudancaPlanoPendente(OperadoraID, ContratoID, BeneficiarioID, loteId, PM);
                case ContratoBeneficiario.eStatus.MudancaPlanoPendenteNaOperadora:
                    return Untyped.UntypedProcesses.GetBeneficiarioMudancaPlanoPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.MudancaDePlanoDevolvido:
                    return Untyped.UntypedProcesses.GetBeneficiarioMudancaPlanoPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.CancelamentoPendente:
                    return Untyped.UntypedProcesses.GetBeneficiarioCancelamentoContratoPendente(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.CancelamentoPendenteNaOperadora:
                    return Untyped.UntypedProcesses.GetBeneficiarioCancelamentoContratoPendenteOperadora(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.CancelamentoDevolvido:
                    return Untyped.UntypedProcesses.GetBeneficiarioCancelamentoContratoPendenteDevolvido(OperadoraID, ContratoID, BeneficiarioID, loteId, PM, vigencia, contratoAdmId);
                case ContratoBeneficiario.eStatus.Desconhecido:
                    return null;
                default:
                    return null;
            }
        }

        #endregion

        #region GerarArquivoInclusao

        /// <summary>
        /// Método para Gerar um Arquivo de Inclusao.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <returns></returns>
        public Boolean GerarArquivoInclusao(Object OperadoraID, ref String ArquivoNome)
        {
            return this.GerarArquivoInclusao(OperadoraID, ref ArquivoNome, null, null, DateTime.MinValue); 
        }

        /// <summary>
        /// Método para Gerar um Arquivo de Inclusao.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public Boolean GerarArquivoInclusao(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia)
        {
            return this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.Novo, ContratoID, BeneficiarioID, Movimentacao.InclusaoBeneficiario, TipoMovimentacao.Inclusao, vigencia);
        }

        #endregion

        #region GerarArquivoInclusaoDevolvido

        ///// <summary>
        ///// Método para Gerar um Arquivo de Inclusao para os que foram Devolvidos.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora.</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <returns></returns>
        //public Boolean GerarArquivoInclusaoDevolvido(Object OperadoraID, ref String ArquivoNome)
        //{
        //    return this.GerarArquivoInclusaoDevolvido(OperadoraID, ref ArquivoNome, null, null);
        //}

        ///// <summary>
        ///// Método para Gerar um Arquivo de Inclusao para os que foram Devolvidos.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <param name="Status">Status do Beneficiario no Contrato.</param>
        ///// <param name="ContratoID">Array de ID de Contrato.</param>
        ///// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        //public Boolean GerarArquivoInclusaoDevolvido(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        //{
        //    return false; //this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.Devolvido, ContratoID, BeneficiarioID, Movimentacao.InclusaoBeneficiario, TipoMovimentacao.Inclusao);
        //}

        #endregion

        #region GerarArquivoAlteracao

        ///// <summary>
        ///// Método para Gerar um Arquivo de Alteração dos Dados Cadastrais.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora.</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <returns></returns>
        //public Boolean GerarArquivoAlteracao(Object OperadoraID, ref String ArquivoNome)
        //{
        //    return this.GerarArquivoAlteracao(OperadoraID, ref ArquivoNome, null, null);
        //}

        ///// <summary>
        ///// Método para Gerar um Arquivo de Alteração dos Dados Cadastrais.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <param name="Status">Status do Beneficiario no Contrato.</param>
        ///// <param name="ContratoID">Array de ID de Contrato.</param>
        ///// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        //public Boolean GerarArquivoAlteracao(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        //{
        //    return false; //this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.AlteracaoCadastroPendente, ContratoID, BeneficiarioID, Movimentacao.AlteracaoBeneficiario, TipoMovimentacao.Alteracao);
        //}

        #endregion

        #region GerarArquivoAlteracaoDevolvido

        ///// <summary>
        ///// Método para Gerar um Arquivo de Alteração dos Dados Cadastrais para os que foram Devolvidos.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora.</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <returns></returns>
        //public Boolean GerarArquivoAlteracaoDevolvido(Object OperadoraID, ref String ArquivoNome)
        //{
        //    return this.GerarArquivoAlteracaoDevolvido(OperadoraID, ref ArquivoNome, null, null);
        //}

        ///// <summary>
        ///// Método para Gerar um Arquivo de Alteração dos Dados Cadastrais para os que foram Devolvidos.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <param name="Status">Status do Beneficiario no Contrato.</param>
        ///// <param name="ContratoID">Array de ID de Contrato.</param>
        ///// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        //public Boolean GerarArquivoAlteracaoDevolvido(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        //{
        //    return false; //this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.AlteracaoCadastroDevolvido, ContratoID, BeneficiarioID, Movimentacao.AlteracaoBeneficiario, TipoMovimentacao.Alteracao);
        //}

        #endregion

        #region GerarArquivoExclusao

        ///// <summary>
        ///// Método para Gerar um Arquivo de Exclusão de Beneficiário.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora.</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <returns></returns>
        //public Boolean GerarArquivoExclusao(Object OperadoraID, ref String ArquivoNome)
        //{
        //    return this.GerarArquivoExclusao(OperadoraID, ref ArquivoNome, null, null);
        //}

        ///// <summary>
        ///// Método para Gerar um Arquivo de Exclusão de Beneficiário.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <param name="Status">Status do Beneficiario no Contrato.</param>
        ///// <param name="ContratoID">Array de ID de Contrato.</param>
        ///// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        //public Boolean GerarArquivoExclusao(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        //{
        //    return false; //this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.ExclusaoPendente, ContratoID, BeneficiarioID, Movimentacao.ExclusaoBeneficiario, TipoMovimentacao.Exclusao);
        //}

        #endregion

        #region GerarArquivoExclusaoDevolvido

        ///// <summary>
        ///// Método para Gerar um Arquivo de Exclusão de Beneficiário.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora.</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <returns></returns>
        //public Boolean GerarArquivoExclusaoDevolvido(Object OperadoraID, ref String ArquivoNome)
        //{
        //    return this.GerarArquivoExclusaoDevolvido(OperadoraID, ref ArquivoNome, null, null);
        //}

        ///// <summary>
        ///// Método para Gerar um Arquivo de Exclusão de Beneficiário.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <param name="Status">Status do Beneficiario no Contrato.</param>
        ///// <param name="ContratoID">Array de ID de Contrato.</param>
        ///// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        //public Boolean GerarArquivoExclusaoDevolvido(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        //{
        //    return false; //this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.ExclusaoDevolvido, ContratoID, BeneficiarioID, Movimentacao.ExclusaoBeneficiario, TipoMovimentacao.Exclusao);
        //}

        #endregion

        #region GerarArquivoSegundaViaCartao

        ///// <summary>
        ///// Método para Gerar um Arquivo de Exclusão de Beneficiário.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora.</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <returns></returns>
        //public Boolean GerarArquivoSegundaViaCartao(Object OperadoraID, ref String ArquivoNome)
        //{
        //    return this.GerarArquivoSegundaViaCartao(OperadoraID, ref ArquivoNome, null, null);
        //}

        ///// <summary>
        ///// Método para Gerar um Arquivo de Exclusão de Beneficiário.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <param name="Status">Status do Beneficiario no Contrato.</param>
        ///// <param name="ContratoID">Array de ID de Contrato.</param>
        ///// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        //public Boolean GerarArquivoSegundaViaCartao(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        //{
        //    return false; //this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.SegundaViaCartaoPendente, ContratoID, BeneficiarioID, Movimentacao.SegundaViaCartaoBeneficiario, TipoMovimentacao.EmissaoSegundaVia);
        //}

        #endregion

        #region GerarArquivoSegundaViaCartaoDevolvido

        ///// <summary>
        ///// Método para Gerar um Arquivo de Segunda Via de Cartão dos Beneficiários que foram devolvidos.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora.</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <returns></returns>
        //public Boolean GerarArquivoSegundaViaCartaoDevolvido(Object OperadoraID, ref String ArquivoNome)
        //{
        //    return this.GerarArquivoSegundaViaCartaoDevolvido(OperadoraID, ref ArquivoNome, null, null);
        //}

        ///// <summary>
        ///// Método para Gerar um Arquivo de Segunda Via de Cartão dos Beneficiários que foram devolvidos.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <param name="Status">Status do Beneficiario no Contrato.</param>
        ///// <param name="ContratoID">Array de ID de Contrato.</param>
        ///// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        //public Boolean GerarArquivoSegundaViaCartaoDevolvido(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        //{
        //    return false; //this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.SegundaViaCartaoDevolvido, ContratoID, BeneficiarioID, Movimentacao.SegundaViaCartaoBeneficiario, TipoMovimentacao.EmissaoSegundaVia);
        //}

        #endregion

        #region GerarArquivoMudancaPlano

        ///// <summary>
        ///// Método para Gerar um Arquivo de Mudança de Plano do Beneficiário.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora.</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <returns></returns>
        //public Boolean GerarArquivoMudancaPlano(Object OperadoraID, ref String ArquivoNome)
        //{
        //    return this.GerarArquivoMudancaPlano(OperadoraID, ref ArquivoNome, null, null);
        //}

        ///// <summary>
        ///// Método para Gerar um Arquivo de Mudança de Plano do Beneficiário.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <param name="Status">Status do Beneficiario no Contrato.</param>
        ///// <param name="ContratoID">Array de ID de Contrato.</param>
        ///// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        //public Boolean GerarArquivoMudancaPlano(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        //{
        //    return false; //this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.MudancaPlanoPendente, ContratoID, BeneficiarioID, Movimentacao.MudancaDePlano, TipoMovimentacao.Exclusao);
        //}

        #endregion

        #region GerarArquivoMudancaPlanoDevolvido

        ///// <summary>
        ///// Método para Gerar um Arquivo de Mudança de Plano do Beneficiário que foi devolvido pela OPERADORA.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora.</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <returns></returns>
        //public Boolean GerarArquivoMudancaPlanoDevolvido(Object OperadoraID, ref String ArquivoNome)
        //{
        //    return this.GerarArquivoMudancaPlanoDevolvido(OperadoraID, ref ArquivoNome, null, null);
        //}

        ///// <summary>
        ///// Método para Gerar um Arquivo de Mudança de Plano do Beneficiário que foi devolvido pela OPERADORA.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <param name="Status">Status do Beneficiario no Contrato.</param>
        ///// <param name="ContratoID">Array de ID de Contrato.</param>
        ///// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        //public Boolean GerarArquivoMudancaPlanoDevolvido(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        //{
        //    return false; //this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.MudancaDePlanoDevolvido, ContratoID, BeneficiarioID, Movimentacao.MudancaDePlano, TipoMovimentacao.Exclusao);
        //}

        #endregion

        #region GerarArquivoCancelamentoContrato

        ///// <summary>
        ///// Método para Gerar um Arquivo de Cancelamento de Contrato do Beneficiário.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora.</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <returns></returns>
        //public Boolean GerarArquivoCancelamentoContrato(Object OperadoraID, ref String ArquivoNome)
        //{
        //    return this.GerarArquivoCancelamentoContrato(OperadoraID, ref ArquivoNome, null, null);
        //}

        ///// <summary>
        ///// Método para Gerar um Arquivo de Cancelamento de Contrato do Beneficiário.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <param name="Status">Status do Beneficiario no Contrato.</param>
        ///// <param name="ContratoID">Array de ID de Contrato.</param>
        ///// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        //public Boolean GerarArquivoCancelamentoContrato(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        //{
        //    return this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.CancelamentoPendente, ContratoID, BeneficiarioID, Movimentacao.CancelamentoContrato, TipoMovimentacao.Exclusao, DateTime.MinValue);
        //}

        #endregion

        #region GerarArquivoCancelamentoContratoDevolvido

        ///// <summary>
        ///// Método para Gerar um Arquivo de Cancelamento de Contrato para os que foram devolvidos.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora.</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <returns></returns>
        //public Boolean GerarArquivoCancelamentoContratoDevolvido(Object OperadoraID, ref String ArquivoNome)
        //{
        //    return this.GerarArquivoCancelamentoContratoDevolvido(OperadoraID, ref ArquivoNome, null, null);
        //}

        ///// <summary>
        ///// Método para Gerar um Arquivo de Cancelamento de Contrato do Beneficiário.
        ///// </summary>
        ///// <param name="OperadoraID">ID da Operadora</param>
        ///// <param name="ArquivoNome">Nome do Arquivo.</param>
        ///// <param name="Status">Status do Beneficiario no Contrato.</param>
        ///// <param name="ContratoID">Array de ID de Contrato.</param>
        ///// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        //public Boolean GerarArquivoCancelamentoContratoDevolvido(Object OperadoraID, ref String ArquivoNome, Object[] ContratoID, Object[] BeneficiarioID)
        //{
        //    return false; //this.GerarArquivo(OperadoraID, ref ArquivoNome, ContratoBeneficiario.eStatus.CancelamentoDevolvido, ContratoID, BeneficiarioID, Movimentacao.CancelamentoContrato, TipoMovimentacao.Exclusao);
        //}

        #endregion

        #region GerarArquivoPorStatus

        /// <summary>
        /// Método para Gerar um Arquivo de acordo com o Status do Beneficiario no Contrato.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora</param>
        /// <param name="ArquivoNome">Nome do Arquivo.</param>
        /// <param name="Status">Status do Beneficiario no Contrato.</param>
        /// <param name="ContratoID">Array de ID de Contrato.</param>
        /// <param name="BeneficiarioID">Array de ID de Beneficiário.</param>
        public void GerarArquivoPorStatus(Object OperadoraID, ref String ArquivoNome, ContratoBeneficiario.eStatus Status, Object[] ContratoID, Object[] BeneficiarioID, DateTime vigencia)
        {
            switch (Status)
            {
                case ContratoBeneficiario.eStatus.Novo:
                    this.GerarArquivoInclusao(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID, vigencia);
                    break;
                case ContratoBeneficiario.eStatus.Devolvido:
                    ////this.GerarArquivoInclusaoDevolvido(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.AlteracaoCadastroPendente:
                    //this.GerarArquivoAlteracao(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.AlteracaoCadastroDevolvido:
                    //this.GerarArquivoAlteracaoDevolvido(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.SegundaViaCartaoPendente:
                    //this.GerarArquivoSegundaViaCartao(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.SegundaViaCartaoDevolvido:
                    //this.GerarArquivoSegundaViaCartaoDevolvido(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.ExclusaoPendente:
                    //this.GerarArquivoExclusao(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.ExclusaoDevolvido:
                    //this.GerarArquivoExclusaoDevolvido(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.MudancaPlanoPendente:
                    //this.GerarArquivoMudancaPlano(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.MudancaDePlanoDevolvido:
                    //this.GerarArquivoMudancaPlanoDevolvido(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.CancelamentoPendente:
                    //this.GerarArquivoCancelamentoContrato(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
                case ContratoBeneficiario.eStatus.CancelamentoDevolvido:
                    //this.GerarArquivoCancelamentoContratoDevolvido(OperadoraID, ref ArquivoNome, ContratoID, BeneficiarioID);
                    break;
            }
        }

        /// <summary>
        /// Gera arquivos de inclusão Amil segundo um array de ids de proposta. 
        /// Não respeita status de inclusão de beneficiários
        /// </summary>
        public void GerarArquivoDeInclusaoPorContratoIDs(ref String arquivoNome, String[] propostaIDs)
        {
            this.GerarArquivoDeInclusaoPorContratoIDs(
                ref arquivoNome, propostaIDs, Movimentacao.InclusaoBeneficiario, TipoMovimentacao.Inclusao);
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Representa um Lote de Arquivo Transacional.
    /// </summary>
    [DBTable("arquivo_transacional_lote")]
    public class ArqTransacionalLote : EntityBase, IPersisteableEntity
    {
        #region Private Fields

        private Object _lote_id;
        private Object _lote_operadora_id;
        private Int32 _lote_quantidade;
        private Int32 _lote_numeracao;
        private DateTime _lote_data_criacao;
        private DateTime _lote_data_vigencia;
        private String _lote_movimentacao;
        private String _lote_tipo_movimentacao;
        private String _lote_arq_nome;
        private Object _layout_customizado_id;
        /// <summary>
        /// Se for um arquivo de exportação, não deve gerar numeração nem alterar status de beneficiários.
        /// </summary>
        private Boolean _exportacao;
        private IList<ArqTransacionalLoteItem> _itens;

        #endregion

        #region Public Members

        /// <summary>
        /// ID do Lote.
        /// </summary>
        [DBFieldInfo("lote_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return this._lote_id; }
            set { this._lote_id = value; }
        }

        /// <summary>
        /// ID da Operadora.
        /// </summary>
        [DBFieldInfo("lote_operadora_id", FieldType.Single)]
        public Object OperadoraID
        {
            get { return this._lote_operadora_id; }
            set { this._lote_operadora_id = value; }
        }

        /// <summary>
        /// Quantidade de Itens do Lote.
        /// </summary>
        [DBFieldInfo("lote_quantidade", FieldType.Single)]
        public Int32 Quantidade
        {
            get { return this._lote_quantidade; }
            set { this._lote_quantidade = value; }
        }

        /// <summary>
        /// Numeração do Lote. Controle diário.
        /// </summary>
        [DBFieldInfo("lote_numeracao", FieldType.Single)]
        public Int32 Numeracao
        {
            get { return this._lote_numeracao; }
            set { this._lote_numeracao = value; }
        }

        /// <summary>
        /// Data de Criação do Lote.
        /// </summary>
        [DBFieldInfo("lote_data_criacao", FieldType.Single)]
        public DateTime DataCriacao
        {
            get { return this._lote_data_criacao; }
            set { this._lote_data_criacao = value; }
        }

        /// <summary>
        /// Data de vigência dos itens do Lote.
        /// </summary>
        [DBFieldInfo("lote_data_vigencia", FieldType.Single)]
        public DateTime DataVigencia
        {
            get { return this._lote_data_vigencia; }
            set { this._lote_data_vigencia= value; }
        }

        /// <summary>
        /// Movimentação. Inclusão de Beneficiário, Alteração dos Dados Cadastrais, Exclusão de Beneficiário
        /// Mudança de Plano e etc.
        /// </summary>
        [DBFieldInfo("lote_movimentacao", FieldType.Single)]
        public String Movimentacao
        {
            get { return this._lote_movimentacao; }
            set { this._lote_movimentacao = value; }
        }

        /// <summary>
        /// Tipo de Movimentação. Inclusão, Alteração, Exclusão e etc.
        /// </summary>
        [DBFieldInfo("lote_tipo_movimentacao", FieldType.Single)]
        public String TipoMovimentacao
        {
            get { return this._lote_tipo_movimentacao; }
            set { this._lote_tipo_movimentacao = value; }
        }

        /// <summary>
        /// Nome do Arquivo.
        /// </summary>
        [DBFieldInfo("lote_arq_nome", FieldType.Single)]
        public String Arquivo
        {
            get { return this._lote_arq_nome; }
            set { this._lote_arq_nome = value; }
        }

        /// <summary>
        /// Um arquivo transacional pode usar um layout customizado.
        /// Esta propriedade guardará o id do layout.
        /// </summary>
        [DBFieldInfo("lote_layout_customizado_id", FieldType.Single)]
        public Object LayoutCustomizadoID
        {
            get { return this._layout_customizado_id; }
            set { this._layout_customizado_id = value; }
        }

        /// <summary>
        /// Se for um arquivo de exportação, não deve gerar numeração nem alterar status de beneficiários.
        /// </summary>
        [DBFieldInfo("lote_exportacao", FieldType.Single)]
        public Boolean Exportacao
        {
            get { return this._exportacao; }
            set { this._exportacao= value; }
        }

        /// <summary>
        /// Itens do Lote.
        /// </summary>
        public IList<ArqTransacionalLoteItem> Itens
        {
            get { return this._itens; }
            set { this._itens = value; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Método para Gerar a Numeração Diária por Operadora.
        /// </summary>
        private void GerarNumeracaoDiaria()
        {
            if (this._lote_operadora_id != null)
            {
                String[] strParam = new String[3];
                String[] strValue = new String[3];

                strParam[0] = "@operadora_id";
                strParam[1] = "@data_de";
                strParam[2] = "@data_ate";

                strValue[0] = this._lote_operadora_id.ToString();
                strValue[1] = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                strValue[2] = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");

                String strSQL = String.Concat("SELECT ",
                                              "      MAX(lote_numeracao) ",
                                              "  FROM arquivo_transacional_lote ",
                                              "  WHERE lote_exportacao <> 1 AND (lote_operadora_id = @operadora_id AND (lote_data_criacao BETWEEN @data_de AND @data_ate));");

                Object retVal = null;

                try
                {
                    retVal = LocatorHelper.Instance.ExecuteScalar(strSQL, strParam, strValue);
                }
                catch (Exception) { throw; }

                if (retVal == null || retVal is DBNull)
                    this._lote_numeracao = 1;
                else
                    this._lote_numeracao = Convert.ToInt32(retVal) + 1;
            }
            else
                throw new ArgumentNullException("ID de Operadora é nulo.");
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Inicializa um lote genérico e vazio.
        /// </summary>
        public ArqTransacionalLote()
        {
            this._lote_id = null;
            this._lote_operadora_id = null;
            this._lote_quantidade = 0;
            this._lote_numeracao = 1;
            this._lote_data_criacao = DateTime.Now;
            this._itens = new List<ArqTransacionalLoteItem>();
            this._exportacao = false;
        }

        #endregion

        #region Entity Base Methods

        public void Carregar()
        {
            this.Carregar(this);
        }

        #region Salvar

        /// <summary>
        /// Método para Salvar sem a Geração Automática da Numeração Diária.
        /// </summary>
        public void Salvar()
        {
            this.Salvar(false);
        }

        /// <summary>
        /// Método para Salvar com a possibilidade de Gerar a Numeração Diária automaticamente.
        /// </summary>
        public void Salvar(Boolean GerarNumeracaoDiaria, PersistenceManager PM)
        {
            Salvar("UP", GerarNumeracaoDiaria, PM);
        }

        internal void Salvar(String fileNamePrefix, Boolean GerarNumeracaoDiaria, PersistenceManager PM)
        {
            Boolean closePMInstance = false;
            if (PM == null) { PM = new PersistenceManager(); PM.UseSingleCommandInstance(); closePMInstance = true; }

            if (GerarNumeracaoDiaria && !_exportacao)
            {
                try
                {
                    this.GerarNumeracaoDiaria();
                }
                catch (Exception) { throw; }
            }

            if (this._lote_id == null && GerarNumeracaoDiaria && !_exportacao)
            {
                ArqTransacionalUnimedConf arqTransUnimed = new ArqTransacionalUnimedConf();
                arqTransUnimed.CarregarPorOperadora(this._lote_operadora_id, PM);

                this._lote_arq_nome = String.Concat(fileNamePrefix, arqTransUnimed.OperadoraCodSingular, DateTime.Now.ToString("ddMMyyyy"), ".", this._lote_numeracao.ToString().PadLeft(3, '0'));

                arqTransUnimed = null;
            }
            else
                this._lote_arq_nome = String.Concat(fileNamePrefix,"_",DateTime.Now.ToString("ddMMyyyyHHmmss"));

            PM.Save(this);

            if (_exportacao)
            {
                if (closePMInstance) { PM.CloseSingleCommandInstance(); PM.Dispose(); PM = null; }
                return;
            }

            ContratoBeneficiario.eStatus proximoStatus;

            if (!String.IsNullOrEmpty(this._lote_movimentacao))
                proximoStatus = ContratoBeneficiario.ProximoStatusPorMovimentacao(this._lote_movimentacao);
            else
            {
                if (closePMInstance) { PM.CloseSingleCommandInstance(); PM.Dispose(); PM = null; }
                throw new ArgumentNullException("A Movimentação não foi informada.");
            }

            if (proximoStatus == ContratoBeneficiario.eStatus.Desconhecido)
            {
                if (closePMInstance) { PM.CloseSingleCommandInstance(); PM.Dispose(); PM = null; }
                throw new Exception("O Status da Movimentação não possui uma sequência no Workflow de Arquivos Transacionais.");
            }

            foreach (ArqTransacionalLoteItem loteItem in this._itens)
            {
                loteItem.LoteID = this._lote_id;
                PM.Save(loteItem);

                ContratoBeneficiario.AlteraStatusBeneficiario(loteItem.ContratoID, loteItem.BeneficiarioID, proximoStatus, PM);
            }

            if (closePMInstance) { PM.CloseSingleCommandInstance(); PM.Dispose(); PM = null; }
        }

        /// <summary>
        /// Método para Salvar com a possibilidade de Gerar a Numeração Diária automaticamente.
        /// </summary>
        /// <param name="GerarNumeracaoDiaria">True para Gerar a Numeração Diária e False para não gerar.</param>
        public void Salvar(Boolean GerarNumeracaoDiaria)
        {
            Salvar(GerarNumeracaoDiaria, null);
        }

        #endregion

        public void Remover()
        {
            this.Remover(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Método para Carregar o Lote por Contrato e Beneficiario.
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        /// <param name="BeneficiarioID">ID do Beneficiario.</param>
        public void CarregarPorContratoBeneficiario(Object ContratoID, Object BeneficiarioID)
        {
            this.CarregarPorContratoBeneficiario(ContratoID, BeneficiarioID, null);
        }

        /// <summary>
        /// Método para Carregar o Lote por Contrato e Beneficiario.
        /// </summary>
        /// <param name="ContratoID">ID do Contrato.</param>
        /// <param name="BeneficiarioID">ID do Beneficiario.</param>
        /// <param name="pm">Objeto PersistenceManager para o contexto transacional.</param>
        public void CarregarPorContratoBeneficiario(Object ContratoID, Object BeneficiarioID, PersistenceManager pm)
        {
            if (ContratoID != null && BeneficiarioID != null)
            {
                String[] strParam = new String[2];
                String[] strValue = new String[2];

                strParam[0] = "@contratoId";
                strParam[1] = "@beneficiarioId";

                strValue[0] = ContratoID.ToString();
                strValue[1] = BeneficiarioID.ToString();

                String strSQL = String.Concat("SELECT ",
                                                "    TOP 1 l.*  ",
                                                "FROM arquivo_transacional_lote l  ",
                                                "INNER JOIN arquivo_transacional_lote_item lI ON  l.lote_id = lI.item_lote_id  ",
                                                "WHERE l.lote_exportacao <> 1 AND item_contrato_id = @contratoId AND item_beneficiario_id = @beneficiarioId  AND item_ativo = 1 ",
                                                "ORDER BY lote_data_criacao DESC;");

                IList<ArqTransacionalLote> lstLote = null;

                try
                {
                    lstLote = LocatorHelper.Instance.ExecuteParametrizedQuery<ArqTransacionalLote>(strSQL, strParam, strValue, typeof(ArqTransacionalLote), pm);
                }
                catch (Exception) { throw; }

                if (lstLote != null && lstLote.Count > 0)
                {
                    this._lote_id = lstLote[0]._lote_id;
                    this._lote_operadora_id = lstLote[0]._lote_operadora_id;
                    this._lote_quantidade = lstLote[0]._lote_quantidade;
                    this._lote_numeracao = lstLote[0]._lote_numeracao;
                    this._lote_data_criacao = lstLote[0]._lote_data_criacao;
                    this._lote_movimentacao = lstLote[0]._lote_movimentacao;
                    this._lote_tipo_movimentacao = lstLote[0]._lote_tipo_movimentacao;
                    this._lote_arq_nome = lstLote[0]._lote_arq_nome;
                }

            }
            else
                throw new ArgumentNullException("ID do Contrato ou do Beneficiario não foi informado.");
        }

        /// <summary>
        /// Método para Carregar os Lotes de uma Operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <returns>Retorna uma lista com Lotes preenchidos.</returns>
        public static IList<ArqTransacionalLote> CarregarPorOperadora(Object OperadoraID, Boolean sobrescreveHHmmss)
        {
            return CarregarPorOperadora(OperadoraID, DateTime.Now, DateTime.Now, DateTime.MinValue, null, false, sobrescreveHHmmss);
        }

        /// <summary>
        /// Método para Carregar os Lotes de uma Operadora.
        /// </summary>
        /// <param name="OperadoraID">ID da Operadora.</param>
        /// <param name="Diario">True para trazer somente os arquivos diários e False para todos.</param>
        /// <returns>Retorna uma lista com Lotes preenchidos.</returns>
        public static IList<ArqTransacionalLote> CarregarPorOperadora(Object OperadoraID, DateTime Inicial, DateTime Final, DateTime Vigencia, List<Object> TipoMovimentacao, Boolean exportacao, Boolean sobrescreveHHmmss)
        {
            if (OperadoraID != null)
            {
                String[] strParam = null;
                String[] strValue = null;
                String strDateWhere = String.Empty;
                String strTipoMovWhere = String.Empty;
                String strDateVigenciaWhere = String.Empty;
                String strDateVigenciaInnerJoin = String.Empty;
                String strDateVigenciaDistinct = String.Empty;

                Int32 intQtdeParam = 0;
                Int32 intQtdeParamTipoMov = 0;

                if (TipoMovimentacao != null) intQtdeParamTipoMov = TipoMovimentacao.Count;

                intQtdeParam = (Vigencia.CompareTo(DateTime.MinValue) == 0) ? 3 : 5;

                strParam = new String[intQtdeParam + intQtdeParamTipoMov];
                strValue = new String[intQtdeParam + intQtdeParamTipoMov];

                strParam[0] = "@operadora_id";
                strParam[1] = "@data_de";
                strParam[2] = "@data_ate";

                strValue[0] = OperadoraID.ToString();

                if (!sobrescreveHHmmss)
                {
                    strValue[1] = Inicial.ToString("yyyy-MM-dd 00:00:00");
                    strValue[2] = Final.ToString("yyyy-MM-dd 23:59:59");
                }
                else
                {
                    strValue[1] = Inicial.ToString("yyyy-MM-dd HH:mm:ss:000");
                    strValue[2] = Final.ToString("yyyy-MM-dd HH:mm:ss:999");
                }

                if (intQtdeParam > 3)
                {
                    strParam[3] = "@data_vigencia_de";
                    strParam[4] = "@data_vigencia_ate";

                    strValue[3] = Vigencia.ToString("yyyy-MM-dd 00:00:00");
                    strValue[4] = Vigencia.ToString("yyyy-MM-dd 23:59:59");

                    strDateVigenciaDistinct = " DISTINCT ";
                    strDateVigenciaInnerJoin = String.Concat(" INNER JOIN arquivo_transacional_lote_item lI ON  l.lote_id = lI.item_lote_id ",
                                                             " INNER JOIN contrato c ON lI.item_contrato_id = c.contrato_id ");
                    strDateVigenciaWhere = " AND (contrato_vigencia BETWEEN @data_vigencia_de AND @data_vigencia_ate) ";
                }

                strDateWhere = " AND (lote_data_criacao BETWEEN @data_de AND @data_ate)";

                if (intQtdeParamTipoMov > 0)
                    for (Int32 i = 0; i < TipoMovimentacao.Count; i++)
                    {
                        strParam[intQtdeParam + i] = String.Concat("@lote_mov_", i.ToString());
                        strValue[intQtdeParam + i] = TipoMovimentacao[i].ToString();

                        strTipoMovWhere += String.Concat(" AND lote_movimentacao = ", strParam[intQtdeParam + i]);
                    }

                String exportacaoCond = "0";
                if (exportacao) { exportacaoCond = "1"; }

                String strSQL = String.Concat("SELECT ", strDateVigenciaDistinct,
                                              "      l.* ",
                                              "  FROM arquivo_transacional_lote l ", strDateVigenciaInnerJoin,
                                              "  WHERE l.lote_exportacao =", exportacaoCond, " AND (lote_operadora_id = @operadora_id ", strTipoMovWhere, strDateWhere, strDateVigenciaWhere, ")",
                                              "  ORDER BY lote_numeracao DESC");

                try
                {
                    return LocatorHelper.Instance.ExecuteParametrizedQuery<ArqTransacionalLote>(strSQL, strParam, strValue, typeof(ArqTransacionalLote));
                }
                catch (Exception) { throw; }
            }
            else
                throw new ArgumentNullException("ID de Operadora é nulo.");
        }

        public static void DesfazerLote(String[] contratoBeneficiarioIds)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();
            DataTable dt = null;

            try
            {
                String qry = String.Concat("lote_id, item_id, lote_layout_customizado_id, operadora_nome, beneficiario_nome, contratobeneficiario_id, contratobeneficiario_status, contratobeneficiario_tipo ", 
                    "   FROM contrato_beneficiario ",
                    "       INNER JOIN contrato ON contrato_id=contratobeneficiario_contratoId",
                    "       INNER JOIN operadora ON contrato_operadoraid=operadora_id",
                    "       INNER JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId",
                    "       INNER JOIN arquivo_transacional_lote_item ON item_contrato_id=contratobeneficiario_contratoId AND item_beneficiario_id=contratobeneficiario_beneficiarioId AND item_ativo=1",
                    "       INNER JOIN arquivo_transacional_lote ON lote_id=item_lote_id",
                    "   WHERE lote_exportacao <> 1 AND contratobeneficiario_id IN (", String.Join(",", contratoBeneficiarioIds), ")");

                dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];

                ContratoBeneficiario.eStatus novoStatus = ContratoBeneficiario.eStatus.Desconhecido;
                foreach (DataRow row in dt.Rows)
                {
                    //Altera o status do beneficiário para pendente no sistema
                    novoStatus = ContratoBeneficiario.StatusAntesDeDesfazerEnvio(((ContratoBeneficiario.eStatus)Convert.ToInt32(row["contratobeneficiario_status"])));
                    ContratoBeneficiario.AlteraStatusBeneficiario(row["contratobeneficiario_id"], novoStatus, pm);

                    //inativa a entrada do beneficiário no lote desfeito
                    ArqTransacionalLoteItem.Inativar(row["item_id"], pm);
                }

                pm.Commit();
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
                dt.Dispose();
            }
        }

        #endregion
    }

    /// <summary>
    /// Representa um Lote de Arquivo Transacional.
    /// </summary>
    [DBTable("arquivo_transacional_lote_item")]
    public class ArqTransacionalLoteItem : EntityBase, IPersisteableEntity
    {
        #region Private Fields

        private Object _item_id;
        private Object _item_lote_id;
        private Object _item_contrato_id;
        private Object _item_beneficiario_id;
        private Int32 _item_beneficiario_sequencia;
        private Boolean _item_ativo;

        #endregion

        #region Public Members

        /// <summary>
        /// ID do Lote.
        /// </summary>
        [DBFieldInfo("item_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return this._item_id; }
            set { this._item_id = value; }
        }

        /// <summary>
        /// ID do Lote.
        /// </summary>
        [DBFieldInfo("item_lote_id", FieldType.Single)]
        public Object LoteID
        {
            get { return this._item_lote_id; }
            set { this._item_lote_id = value; }
        }

        /// <summary>
        /// ID do Contrato.
        /// </summary>
        [DBFieldInfo("item_contrato_id", FieldType.Single)]
        public Object ContratoID
        {
            get { return this._item_contrato_id; }
            set { this._item_contrato_id = value; }
        }

        /// <summary>
        /// ID do Beneficiario.
        /// </summary>
        [DBFieldInfo("item_beneficiario_id", FieldType.Single)]
        public Object BeneficiarioID
        {
            get { return this._item_beneficiario_id; }
            set { this._item_beneficiario_id = value; }
        }

        /// <summary>
        /// Sequencia do Beneficiario na familia.
        /// </summary>
        [DBFieldInfo("item_beneficiario_sequencia", FieldType.Single)]
        public Int32 BeneficiarioSequencia
        {
            get { return this._item_beneficiario_sequencia; }
            set { this._item_beneficiario_sequencia = value; }
        }

        /// <summary>
        /// Status do Item.
        /// </summary>
        [DBFieldInfo("item_ativo", FieldType.Single)]
        public Boolean Ativo
        {
            get { return this._item_ativo; }
            set { this._item_ativo = value; }
        }

        #endregion

        #region Entity Base Methods

        public void Carregar()
        {
            this.Carregar(this);
        }

        public void Salvar()
        {
            this.Salvar(this);
        }

        public void Remover()
        {
            this.Remover(this);
        }

        #endregion

        /// <summary>
        /// Inativa o item do lote
        /// </summary>
        internal static void Inativar(Object itemId, PersistenceManager pm)
        {
            String cmd = String.Concat("UPDATE arquivo_transacional_lote_item SET item_ativo=0 WHERE item_id=", itemId);
            NonQueryHelper.Instance.ExecuteNonQuery(cmd, pm);
        }
    }

    [Serializable]
    [DBTable("arquivo_transacional_unimed_agenda")]
    public class ItemAgendaArquivoUnimed : EntityBase, IPersisteableEntity
    {
        #region fields 

        Object _id;
        Object _propostaId;
        Object _beneficiarioId;
        Int32  _tipo;
        String _tipoDescricao;

        String _propostaNumero;
        String _beneficiarioNome;
        String _beneficiarioCpf;

        #endregion

        #region properties 

        [DBFieldInfo("atua_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID 
        {
            get { return this._id; }
            set { this._id= value; }
        }

        [DBFieldInfo("atua_propostaId", FieldType.Single)]
        public Object PropostaID 
        {
            get { return this._propostaId; }
            set { this._propostaId= value; }
        }

        [DBFieldInfo("atua_beneficiarioId", FieldType.Single)]
        public Object BeneficiarioID
        {
            get { return this._beneficiarioId; }
            set { this._beneficiarioId= value; }
        }

        [DBFieldInfo("atua_tipo", FieldType.Single)]
        public Int32 Tipo 
        {
            get { return this._tipo; }
            set { this._tipo= value; }
        }

        [DBFieldInfo("atua_tipoDescricao", FieldType.Single)]
        public String TipoDescricao 
        {
            get { return this._tipoDescricao; }
            set { this._tipoDescricao= value; }
        }

        [Joinned("contrato_numero")]
        public String PropostaNumero
        {
            get { return this._propostaNumero; }
            set { this._propostaNumero= value; }
        }

        [Joinned("beneficiario_nome")]
        public String BeneficiarioNome
        {
            get { return this._beneficiarioNome; }
            set { this._beneficiarioNome= value; }
        }

        [Joinned("beneficiario_cpf")]
        public String BeneficiarioCPF
        {
            get { return this._beneficiarioCpf; }
            set { this._beneficiarioCpf= value; }
        }

        #endregion

        public ItemAgendaArquivoUnimed() { }
        public ItemAgendaArquivoUnimed(Object id) { _id = id; }

        #region Entity Base Methods 

        public void Carregar()
        {
            base.Carregar(this);
        }

        public void Salvar()
        {
            if (!ExisteItem(_propostaId, _beneficiarioId, _tipo, null))
            {
                base.Salvar(this);
            }
        }

        public void Remover()
        {
            base.Remover(this);
        }

        #endregion

        public static IList<ItemAgendaArquivoUnimed> CarregarTodos()
        {
            return LocatorHelper.Instance.ExecuteQuery<ItemAgendaArquivoUnimed>(
                String.Concat("arquivo_transacional_unimed_agenda.*, contrato_numero, beneficiario_nome, beneficiario_cpf ",
                "   FROM arquivo_transacional_unimed_agenda ",
                "       INNER JOIN beneficiario ON beneficiario_id=atua_beneficiarioId ",
                "       INNER JOIN contrato ON contrato_id=atua_propostaId ",
                "   ORDER BY atua_tipo"), 
                typeof(ItemAgendaArquivoUnimed));
        }

        public static Boolean ExisteItem(Object propostaId, Object beneficiarioId, Int32 tipo, PersistenceManager pm)
        {
            Object ret = LocatorHelper.Instance.ExecuteScalar("SELECT TOP 1 atua_id FROM arquivo_transacional_unimed_agenda WHERE atua_propostaId=" + propostaId + " AND atua_beneficiarioId=" + beneficiarioId + " AND atua_tipo=" + tipo, null, null, pm);

            if (ret == null || ret == DBNull.Value)
                return false;
            else
                return true;
        }

        public static void Clear()
        {
            NonQueryHelper.Instance.ExecuteNonQuery("TRUNCATE TABLE arquivo_transacional_unimed_agenda", null);
        }
    }
}