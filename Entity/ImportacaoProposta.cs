namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.IO;
    using System.Web;
    using System.Xml;
    using System.Data;
    using System.Text;
    using System.Xml.XPath;
    using System.Configuration;
    using System.Collections.Generic;

    using LC.Framework.Phantom;
    using LC.Framework.DataUtil;

    public class ImportacaoProposta
    {
        public void ImportarMapeamento()
        {
            String file = String.Concat(HttpContext.Current.Server.MapPath("/"), ImportacaoProposta.BaseFileTargetPath, "mapp.txt");

            String[] lines = File.ReadAllLines(file);

            if (lines == null || lines.Length == 0) { return; }

            String[] aux = null;

            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0) { continue; } //ignora o cabeçalho 

                try
                {
                    aux = lines[i].Split('#');

                    ItemMapeamento item = new ItemMapeamento();

                    #region preenche instância

                    item.NomeProdutor = aux[0].Split('-')[1];
                    item.CNPJ = aux[2];
                    item.CPF  = aux[3];

                    try
                    {
                        item.Celular = aux[18];
                        item.DataExclusao = aux[14];
                        item.DataInclusao = aux[13];
                        item.Email = aux[17];
                        item.EnviaEmail = aux[15];
                        item.EnviaSMS = aux[16];
                        item.Filial = aux[5];
                        item.Idade = aux[12];
                        item.InscricaoMunicipal = aux[4];
                        item.NomeGuerra = aux[1];
                        item.Sexo = aux[11];
                        item.Situacao = aux[10];
                        item.SituacaoEspecial = aux[8];
                        item.TipoPessoa = aux[9];
                        item.TipoProdutor = aux[7];
                        item.UnidadeNegocio = aux[6];
                    }
                    catch
                    {
                    }

                    #endregion

                    item.SaveOrUpdate(pm);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            pm.CloseSingleCommandInstance();
            pm.Dispose();
        }

        public void AtualizaCorretoresDePropostasImportadas()
        {
            PersistenceManager pm = new PersistenceManager();
            pm.UseSingleCommandInstance();

            DataTable dt = LocatorHelper.Instance.ExecuteQuery("SELECT contrato_id,contrato_corretorTerceiroCPF FROM contrato WHERE contrato_obs LIKE '%importado%' AND (contrato_corretorTerceiroNome IS NULL OR contrato_corretorTerceiroNome='') AND contrato_corretorTerceiroCPF <> '' AND contrato_corretorTerceiroCPF IS NOT NULL", "resultset", pm).Tables[0];
            String nomeCorretor = "";

            foreach (DataRow row in dt.Rows)
            {
                nomeCorretor = ObtemNomeParaCorretor(
                    Convert.ToString(row["contrato_corretorTerceiroCPF"]), pm); //ConvertHelper.ConvertToString(LocatorHelper.Instance.ExecuteScalar("SELECT mapp_produtor FROM importMapping WHERE mapp_cpf='" + row["contrato_corretorTerceiroCPF"] + "'", null, null, pm));

                if (!String.IsNullOrEmpty(nomeCorretor))
                {
                    NonQueryHelper.Instance.ExecuteNonQuery("UPDATE contrato SET contrato_corretorTerceiroNome='" + nomeCorretor + "' WHERE contrato_id=" + row["contrato_id"], pm);
                }
            }

            pm.CloseSingleCommandInstance();
            pm.Dispose();
        }

        internal String ObtemNomeParaCorretor(String corretorDoc, PersistenceManager pm)
        {
            Object ret;

            ret = LocatorHelper.Instance.ExecuteScalar("SELECT mapp_produtor FROM importMapping WHERE mapp_cpf='" + corretorDoc + "'", null, null, pm);

            if (ret == null)
                return String.Empty;
            else
                return Convert.ToString(ret);
        }

        public static readonly String BaseFileTargetPath = ConfigurationManager.AppSettings["importproposal_file"];

        public static IList<ItemAgendamento> Carregar(Boolean processados, DateTime de, DateTime ate)
        {
            String statusCond = "";
            if (processados)
                statusCond = " WHERE ica_processado=1 ";
            else
                statusCond = " WHERE ica_processado=0 ";

            String qry = String.Concat("* FROM importContratoAgendamento ", statusCond,
                " AND ica_processarEm BETWEEN '", de.ToString("yyyy-MM-dd"), "' AND '",
                ate.ToString("yyyy-MM-dd 23:59:59.998"), "' ORDER BY ica_processarEm DESC");

            return LocatorHelper.Instance.ExecuteQuery<ItemAgendamento>(qry, typeof(ItemAgendamento));
        }

        /// <summary>
        /// Carrega agendamentos pendentes e sem erros, para efetivar importação.
        /// </summary>
        public static IList<ItemAgendamento> CarregarPendentes()
        {
            String qry = "TOP 1 * FROM importContratoAgendamento WHERE (ica_erro is null or ica_erro = '') AND ica_processado=0 AND ica_processarEm <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";

            return LocatorHelper.Instance.ExecuteQuery<ItemAgendamento>(qry, typeof(ItemAgendamento));
        }

        public static IList<ItemFalhaImportacao> CarregarCritica(Object agendaId)
        {
            String qry = "* FROM importFalha WHERE if_itemagendamentoId=" + agendaId + " ORDER BY if_movito";

            return LocatorHelper.Instance.ExecuteQuery<ItemFalhaImportacao>(qry, typeof(ItemFalhaImportacao));
        }

        String TraduzOrgaoEmissor(String cod)
        {
            switch (cod)
            {
                case "1":
                {
                    return "SSP - SECRETARIA DE SEGURANÇA PÚBLICA";
                }
                case "2":
                {
                    return "MINISTÉRIO DA AERONAUTICA";
                }
                case "3":
                {
                    return "MINISTÉRIO DO EXÉRCITO";
                }
                case "4":
                {
                    return "MINISTÉRIO DA MARINHA";
                }
                case "5":
                {
                    return "POLICIA FEDERAL";
                }
                case "6":
                {
                    return "CARTEIRA DE IDENTIDADE CLASSISTA";
                }
                case "7":
                {
                    return "CONSELHO REGIONAL DE ADMINISTRAÇÃO";
                }
                case "8":
                {
                    return "CONSELHO REGIONAL DE ASSISTENTES SOCIAIS";
                }
                case "9":
                {
                    return "CONSELHO REGIONAL DE BIBLIOTECONOMIA";
                }
                case "10":
                {
                    return "CONSELHO REGIONAL DE CONTABILIDADE";
                }
                case "11":
                {
                    return "CONSELHO REGIONAL CORRETORES IMÓVEIS";
                }
                case "12":
                {
                    return "CONSELHO REGIONAL ENFERMAGEM";
                }
                case "13":
                {
                    return "CONSELHO REGIONAL ENG.ARQ. E AGRONOMIA";
                }
                case "14":
                {
                    return "CONSELHO REGIONAL DE ESTATISTICA";
                }
                case "15":
                {
                    return "CONSELHO REGIONAL DE FARMÁCIA";
                }
                case "16":
                {
                    return "CONSELHO REGIONAL FISIOT.TERAPIA OCUPACIONAL";
                }
                case "17":
                {
                    return "CONSELHO REGIONAL DE MEDICINA";
                }
                case "18":
                {
                    return "CONSELHO REGIONAL MEDICINA VETERINÁRIA";
                }
                case "19":
                {
                    return "CONSELHO REGIONAL DE NUTRIÇÃO";
                }
                case "20":
                {
                    return "CONSELHO REGIONAL DE ODONTOLOGIA";
                }
                case "21":
                {
                    return "CONSELHO REGIONAL PROF.RELAÇÕES PÚBLICAS";
                }
                case "22":
                {
                    return "CONSELHO REGIONAL DE PSICOLOGIA";
                }
                case "23":
                {
                    return "CONSELHO REGIONAL DE QUÍMICA";
                }
                case "24":
                {
                    return "CONSELHO REGIONAL REPRES.COMERCIAIS";
                }
                case "25":
                {
                    return "ORDEM DOS MÚSICOS DO BRASIL";
                }
                case "26":
                {
                    return "ORDEM DOS ADVOGADOS DO BRASIL";
                }
                case "27":
                {
                    return "OUTROS EMISSORES";
                }
                case "28":
                {
                    return "DOCUMENTOS ESTRANGEIROS";
                }
                default:
                {
                    return String.Empty;
                }
            }
        }
        String TraduzSexo(String cod)
        {
            if (cod == "F")
                return "2";
            else
                return "1";
        }
        String TraduzOperadoraOrigem(String cod)
        {
            return String.Empty;
        }
        Object TraduzEstadoCivil(String cod, Object operadoraId, PersistenceManager pm)
        {
            String descr = "";

            #region switch 

            switch (cod)
            {
                case "1":
                {
                    descr = "Solteiro";
                    break;
                }
                case "2":
                {
                    descr = "Casado";
                    break;
                }
                case "3":
                {
                    descr = "Viuvo";
                    break;
                }
                case "4":
                {
                    descr = "Separado";
                    break;
                }
                case "5":
                {
                    descr = "Divorciado";
                    break;
                }
                default:
                {
                    descr = "outros";
                    break;
                }
            }
            #endregion

            Object ret = LocatorHelper.Instance.ExecuteScalar(String.Concat("SELECT TOP 1 estadocivil_id FROM estado_civil WHERE estadocivil_operadoraId=", operadoraId, " AND estadocivil_descricao LIKE '%", descr, "%'"), null, null, pm);

            return ret;
        }
        Object TraduzParentesco(String cod, Object contratoAdmId, PersistenceManager pm)
        {
            String descr = "";

            #region switch

            switch (cod)
            {
                case "1":
                {
                    descr = "Pai";
                    break;
                }
                case "2":
                {
                    descr = "Conjuge";
                    break;
                }
                case "3":
                {
                    descr = "Filho";
                    break;
                }
                default:
                {
                    descr = "outros";
                    break;
                }
            }
            #endregion

            Object ret = LocatorHelper.Instance.ExecuteScalar(String.Concat("SELECT TOP 1 contratoAdmparentescoagregado_id FROM contratoADM_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=", contratoAdmId, " AND contratoAdmparentescoagregado_parentescoDescricao LIKE '%", descr, "%'"), null, null, pm);

            return ret;
        }

        DateTime toDateTime(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return DateTime.MinValue;
            else
            {
                String[] strparam = Convert.ToString(param).Split('/');

                return new DateTime(Int32.Parse(strparam[2]), 
                    Int32.Parse(strparam[1]), Int32.Parse(strparam[0]));
            }
        }
        Decimal  toDecimal(Object param)
        {
            if (param == null || param == DBNull.Value || Convert.ToString(param).Trim() == "")
                return 0;
            else
            {
                if (Convert.ToString(param).StartsWith(".")) { param = "0" + Convert.ToString(param); }
                param = Convert.ToString(param).Replace(".", ",");
                return Convert.ToDecimal(param);
            }
        }

        public void Importar(ItemAgendamento itemAgenda)
        {
            XmlDocument xdoc = new XmlDocument();

            try
            {
                xdoc.Load(String.Concat(ImportacaoProposta.BaseFileTargetPath, @itemAgenda.ID, ".xml"));
            }
            catch (Exception ex)
            {
                itemAgenda.Erro = "Erro no arquivo: " + ex.Message;
                itemAgenda.Salvar();
                throw ex;
            }

            XmlNodeList xPropostaNodeList = xdoc.SelectNodes("/EXPORTACAO/PROPOSTA");

            if (xPropostaNodeList == null || xPropostaNodeList.Count == 0) { return; }

            #region variaveis 

            XmlNode auxNode = null;
            XmlNodeList auxNodeList = null;

            String auxString = null;
            Contrato contrato = null;
            ContratoADM contratoAdm = null;
            DateTime auxData = DateTime.MinValue;
            Beneficiario beneficiario = null;
            ContratoBeneficiario cb = null;
            Endereco endereco = null; IList<Endereco> auxEnderecoList = null;
            Plano plano = null;Object adicionalId = null;AdicionalBeneficiario adicionalBeneficiario = null;

            String adicionalCod = "";
            DateTime vigencia = DateTime.MinValue, vencimento = DateTime.MinValue, admissao = DateTime.MinValue;
            Int32 diaDataSemJuros = -1, dependenteNumSeq = 0; Object valorDataLimite = null;
            CalendarioVencimento rcv = null;

            #endregion

            PersistenceManager pm = null;
            Boolean falhou = false;
            ImportacaoProposta ip = new ImportacaoProposta();
            Boolean naoImplantada = false;

            foreach (XmlNode xPropostaNode in xPropostaNodeList)
            {
                falhou = false;
                naoImplantada = false;

                pm = new PersistenceManager();
                pm.BeginTransactionContext();
                adicionalId = null;

                if (itemAgenda == null) { itemAgenda = new ItemAgendamento(); itemAgenda.ID = 2; }

                try
                {
                    //auxNode = xPropostaNode.SelectSingleNode("NUM_PROPOSTA");
                    //if (auxNode.InnerText.Trim() != "110052817") { continue; }

                    auxNode = xPropostaNode.SelectSingleNode("NUM_CONTRATO_ADESAO_MEDICO");
                    contratoAdm = ContratoADM.Carregar(auxNode.InnerText.Substring(0,6), pm);
                    if (contratoAdm == null)
                    {
                        pm.Rollback();
                        auxString = auxNode.InnerText;
                        auxNode = xPropostaNode.SelectSingleNode("NUM_PROPOSTA");
                        LogAcao(itemAgenda.ID, auxNode.InnerText, "Contrato ADM não localizado: " + auxString, null);
                        continue;
                    }

                    auxNode = xPropostaNode.SelectSingleNode("NUM_PROPOSTA");

                    contrato = Contrato.CarregarParcial(auxNode.InnerText, contratoAdm.OperadoraID, pm);

                    auxNode = xPropostaNode.SelectSingleNode("TXT_SITUACAO");
                    if (auxNode != null && auxNode.InnerText != null && auxNode.InnerText.ToUpper() != "IMPLANTADA")
                    {
                        naoImplantada = true;
                    }

                    if (contrato != null)
                    {
                        if (!naoImplantada && contrato.Status == (int)Contrato.eStatus.NaoImplantadoNaImportacao)
                        {
                            contrato.Carregar();
                            contrato.Inativo = false;
                            contrato.Status = (int)Contrato.eStatus.Normal;
                            contrato.Obs += Environment.NewLine + "Proposta implantada em + " + DateTime.Now.ToString("dd/MM/yyyy hh:mm");
                            contrato.Alteracao = DateTime.Now;
                            contrato.Salvar();
                        }

                        pm.Rollback();
                        LogAcao(itemAgenda.ID, auxNode.InnerText, "Contrato ja cadastrado.", null);
                        continue;
                    }
                    auxNode = xPropostaNode.SelectSingleNode("COD_CORRETORA");

                    contrato = new Contrato();
                    contrato.DonoID = 3678;            //????

                    auxNode = xPropostaNode.SelectSingleNode("NUM_PROPOSTA");
                    contrato.Numero = auxNode.InnerText;
                    contrato.OperadoraID = contratoAdm.OperadoraID;
                    contrato.ContratoADMID = contratoAdm.ID;
                    contrato.EstipulanteID = contratoAdm.EstipulanteID;
                    contrato.Adimplente = true;

                    //auxNode = xPropostaNode.SelectSingleNode("TXT_SITUACAO");
                    //if (auxNode != null && auxNode.InnerText != null && auxNode.InnerText.ToUpper() != "IMPLANTADA")
                    //{
                    //    naoImplantada = true;
                    //}

                    auxNode = xPropostaNode.SelectSingleNode("DT_PROTOCOLO"); //auxNode = xPropostaNode.SelectSingleNode("DT_VENDA");
                    contrato.Admissao = toDateTime(auxNode.InnerText);

                    CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(contrato.ContratoADMID,
                        contrato.Admissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, pm);

                    auxNode = xPropostaNode.SelectSingleNode("DT_VIGENCIA");
                    if (vigencia.ToString("dd/MM/yyy") != auxNode.InnerText)
                    {
                        ////////////bloco tirardo por solicitacao do marcio em 14/03/2012 às 11:00
                        ///////////recolocado devido a bugs
                        auxData = toDateTime(auxNode.InnerText).AddDays(-29);

                        CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(contrato.ContratoADMID,
                            auxData, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, pm);

                        if (vigencia.ToString("dd/MM/yyy") != auxNode.InnerText)
                        {
                            pm.Rollback();
                            LogAcao(itemAgenda.ID, contrato.Numero, String.Concat("Data de vigência informada no arquivo não corresponde à data de vigência calculada. (Contrato: ", contrato.Numero, " - Contrato ADM: ", contratoAdm.Numero, ")"), null);
                            continue;
                        }

                        contrato.Admissao = auxData;
                        contrato.Obs = String.Concat("Data de admissão ajustada de ", auxNode.InnerText, " para ", auxData.ToString("dd/MM/yyyy"), Environment.NewLine);
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////

                        ////solicitado pelo marcio: usar a data que vem no arquivo
                        //vigencia = new DateTime(
                        //    Convert.ToInt32(auxNode.InnerText.Split('/')[2]),
                        //    Convert.ToInt32(auxNode.InnerText.Split('/')[1]),
                        //    Convert.ToInt32(auxNode.InnerText.Split('/')[0]));

                        //contrato.Obs = String.Concat("Data de admissão ajustada.", Environment.NewLine);
                    }

                    contrato.Vigencia = vigencia;

                    auxNode = xPropostaNode.SelectSingleNode("COD_PLANO_MEDICO"); //PLANO
                    if (auxNode == null || String.IsNullOrEmpty(auxNode.InnerText))
                    {
                        pm.Rollback();
                        LogAcao(itemAgenda.ID, contrato.Numero, String.Concat("Código de plano não informado para o contrato ", contrato.Numero, " (tag COD_PLANO_MEDICO)."), null);
                        continue;
                    }

                    plano = Plano.Carregar(contrato.ContratoADMID, auxNode.InnerText, "", pm);
                    if (plano == null)
                    {
                        pm.Rollback();
                        LogAcao(itemAgenda.ID, contrato.Numero, String.Concat("Plano não localizado. Código: ", auxNode.InnerText, " para o contrato adm ", contratoAdm.Numero), null);
                        continue;
                    }
                    contrato.PlanoID = plano.ID;

                    contrato.TipoAcomodacao = (plano.Codigo == auxNode.InnerText) ? 0 : 1;

                    contrato.Cancelado = false;
                    auxNode = xPropostaNode.SelectSingleNode("COD_CORRETOR");
                    contrato.CorretorTerceiroCPF = auxNode.InnerText;
                    contrato.CorretorTerceiroNome = ip.ObtemNomeParaCorretor(contrato.CorretorTerceiroCPF, pm);
                    //auxNode = xPropostaNode.SelectSingleNode("COD_SUPERVISOR");
                    //contrato.SuperiorTerceiroCPF = auxNode.InnerText;

                    contrato.TipoContratoID = (((int)TipoContrato.TipoComissionamentoProdutorOuOperadora.Normal) + 1);
                    contrato.Vencimento = vencimento;
                    contrato.Pendente = false;
                    contrato.CobrarTaxaAssociativa = true;
                    auxNode = xPropostaNode.SelectSingleNode("NUM_CPF_RESP");
                    contrato.ResponsavelCPF = auxNode.InnerText;
                    auxNode = xPropostaNode.SelectSingleNode("NOM_RESP");
                    contrato.ResponsavelNome = auxNode.InnerText;
                    auxNode = xPropostaNode.SelectSingleNode("DATA_NASCIMENTO_RESP");
                    contrato.ResponsavelDataNascimento = toDateTime(auxNode.InnerText);
                    auxNode = xPropostaNode.SelectSingleNode("IND_SEXO_RESP");
                    contrato.ResponsavelSexo = TraduzSexo(auxNode.InnerText);

                    #region TITULAR 
                    beneficiario = new Beneficiario();

                    auxNode = xPropostaNode.SelectSingleNode("DDD_CELULAR");
                    if (!String.IsNullOrEmpty(auxNode.InnerText))
                    {
                        beneficiario.Celular = String.Concat("(", auxNode.InnerText, ")");
                        auxNode = xPropostaNode.SelectSingleNode("NUM_DDD_CELULAR");
                        beneficiario.Celular = String.Concat(beneficiario.Celular, " ", auxNode.InnerText);
                    }

                    auxNode = xPropostaNode.SelectSingleNode("NUM_CPF");
                    beneficiario.CPF = auxNode.InnerText;
                    auxNode = xPropostaNode.SelectSingleNode("DATA_NASCIMENTO");
                    beneficiario.DataNascimento = toDateTime(auxNode.InnerText);
                    auxNode = xPropostaNode.SelectSingleNode("END_EMAIL");
                    beneficiario.Email = auxNode.InnerText;
                    auxNode = xPropostaNode.SelectSingleNode("NOME_TITULAR");
                    beneficiario.Nome = auxNode.InnerText;
                    auxNode = xPropostaNode.SelectSingleNode("NOME_MAE");
                    beneficiario.NomeMae = auxNode.InnerText;
                    auxNode = xPropostaNode.SelectSingleNode("NUM_IDENTIDADE");
                    beneficiario.RG = auxNode.InnerText;
                    auxNode = xPropostaNode.SelectSingleNode("COD_ORGAO_EMISSOR");
                    beneficiario.RgOrgaoExp = TraduzOrgaoEmissor(auxNode.InnerText);
                    beneficiario.RgUF = ""; //????
                    auxNode = xPropostaNode.SelectSingleNode("IND_SEXO");
                    beneficiario.Sexo = TraduzSexo(auxNode.InnerText);

                    auxNode = xPropostaNode.SelectSingleNode("DDD_TELEFONE_1");
                    if (!String.IsNullOrEmpty(auxNode.InnerText))
                    {
                        beneficiario.Telefone = String.Concat("(", auxNode.InnerText, ")");
                        auxNode = xPropostaNode.SelectSingleNode("NUM_TELEFONE_1");
                        beneficiario.Telefone = String.Concat(beneficiario.Telefone, " ", auxNode.InnerText);
                    }

                    auxNode = xPropostaNode.SelectSingleNode("DDD_TELEFONE_2");
                    if (!String.IsNullOrEmpty(auxNode.InnerText))
                    {
                        beneficiario.Telefone2 = String.Concat("(", auxNode.InnerText, ")");
                        auxNode = xPropostaNode.SelectSingleNode("NUM_TELEFONE_2");
                        beneficiario.Telefone2 = String.Concat(beneficiario.Telefone2, " ", auxNode.InnerText);
                    }

                    //tenta localizar o beneficiario.
                    beneficiario.ID = Beneficiario.CarregarPorParametro(beneficiario.Nome, beneficiario.NomeMae, pm, beneficiario.DataNascimento, beneficiario.CPF);
                    //if (beneficiario.ID != null) { beneficiarioExistente = true; }

                    pm.Save(beneficiario);
                    #endregion

                    #region ENDERECO TITULAR 

                    // Só grava um novo endereço se ele nao existia anteriormente na base, 
                    // do contrário, usa o existente

                    auxEnderecoList = Endereco.CarregarPorDono(beneficiario.ID, Endereco.TipoDono.Beneficiario, pm);

                    if (auxEnderecoList == null || auxEnderecoList.Count == 0)
                    {
                        endereco = new Endereco();
                        auxNode = xPropostaNode.SelectSingleNode("NOM_BAIRRO");
                        endereco.Bairro = auxNode.InnerText;
                        auxNode = xPropostaNode.SelectSingleNode("NUM_CEP");
                        endereco.CEP = auxNode.InnerText;
                        auxNode = xPropostaNode.SelectSingleNode("NOM_MUNICIPIO");
                        endereco.Cidade = auxNode.InnerText;
                        auxNode = xPropostaNode.SelectSingleNode("TXT_COMPLEMENTO");
                        endereco.Complemento = auxNode.InnerText;
                        endereco.DonoId = beneficiario.ID;
                        endereco.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                        auxNode = xPropostaNode.SelectSingleNode("NOM_LOGRADOURO");
                        endereco.Logradouro = auxNode.InnerText;
                        auxNode = xPropostaNode.SelectSingleNode("NUM_ENDERECO");
                        endereco.Numero = auxNode.InnerText;
                        endereco.Tipo = (int)Endereco.TipoEndereco.Residencial;
                        auxNode = xPropostaNode.SelectSingleNode("SGL_UF");
                        endereco.UF = auxNode.InnerText;
                        pm.Save(endereco);
                    }
                    else
                        endereco = auxEnderecoList[0];

                    #endregion

                    contrato.EnderecoCobrancaID   = endereco.ID;
                    contrato.EnderecoReferenciaID = endereco.ID;

                    //calcula o codigo de cobranca para o contrato.
                    contrato.CodCobranca = Convert.ToInt32(LocatorHelper.Instance.ExecuteScalar("SELECT MAX(contrato_codcobranca) FROM contrato", null, null, pm)) + 1;

                    contrato.Data = DateTime.Now;
                    contrato.Obs += "IMPORTADO EM " + contrato.Data.ToString("dd/MM/yyyy HH:mm");
                    if (naoImplantada)
                    {
                        contrato.Obs = String.Concat(contrato.Obs, "\n", "NÃO IMPLANTADO");
                        contrato.Inativo = true;
                        contrato.Status  = (int)Contrato.eStatus.NaoImplantadoNaImportacao;
                    }
                    pm.Save(contrato);

                    #region ADICIONAIS - TITULAR 

                    auxNode = xPropostaNode.SelectSingleNode("COD_PLANO_DENTAL");
                    if (!String.IsNullOrEmpty(auxNode.InnerText))
                    {
                        adicionalCod = auxNode.InnerText;
                        adicionalId = Adicional.CarregarIDPorCodigoTitular(auxNode.InnerText, contrato.OperadoraID, pm);
                        if (adicionalId == null)
                        {
                            pm.Rollback();
                            LogAcao(itemAgenda.ID, contrato.Numero, "Adicional não localizado: " + adicionalCod, null);
                            continue;
                        }

                        //Checa se o adicional está relacionado ao plano
                        if (!ContratoADMPlanoAdicional.ExisteRelacionamento(contrato.ContratoADMID, contrato.PlanoID, adicionalId, pm))
                        {
                            pm.Rollback();
                            LogAcao(itemAgenda.ID, contrato.Numero, "Adicional não relacionado ao plano. Adicional: " + adicionalCod, null);
                            continue;
                        }

                        adicionalBeneficiario = new AdicionalBeneficiario();
                        adicionalBeneficiario.AdicionalID = adicionalId;
                        adicionalBeneficiario.BeneficiarioID = beneficiario.ID;
                        adicionalBeneficiario.PropostaID = contrato.ID;
                        pm.Save(adicionalBeneficiario);
                    }

                    #endregion

                    #region CONTRATO_BENEFICIARIO - TITULAR

                    cb = new ContratoBeneficiario();
                    auxNode = xPropostaNode.SelectSingleNode("ALTURA");
                    cb.Altura = toDecimal(auxNode.InnerText);
                    cb.Ativo = true;
                    cb.BeneficiarioID = beneficiario.ID;
                    auxNode = xPropostaNode.SelectSingleNode("COD_PRC");
                    cb.CarenciaCodigo = auxNode.InnerText;
                    cb.CarenciaContratoTempo = 0;    //????
                    cb.CarenciaMatriculaNumero = ""; //????
                    auxNode = xPropostaNode.SelectSingleNode("COD_OPERADORA_ORIGEM");
                    cb.CarenciaOperadoraDescricao = TraduzOperadoraOrigem(auxNode.InnerText);
                    cb.ContratoID = contrato.ID;
                    cb.Data = contrato.Admissao;
                    auxNode = xPropostaNode.SelectSingleNode("IND_ESTADO_CIVIL");
                    cb.EstadoCivilID = TraduzEstadoCivil(auxNode.InnerText, contrato.OperadoraID, pm);
                    cb.NumeroSequencial = 0;
                    cb.ParentescoID = null;
                    auxNode = xPropostaNode.SelectSingleNode("PESO");
                    cb.Peso = toDecimal(auxNode.InnerText);
                    cb.Portabilidade = "";
                    cb.Status = (int)ContratoBeneficiario.eStatus.Incluido;
                    cb.Tipo = (int)ContratoBeneficiario.TipoRelacao.Titular;
                    //auxNode = xPropostaNode.SelectSingleNode("VAL_MENSALIDADE_TIT");
                    cb.Valor = TabelaValor.CalculaValor(cb.BeneficiarioID, Beneficiario.CalculaIdade(beneficiario.DataNascimento, contrato.Admissao), contrato.ContratoADMID, contrato.PlanoID, (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao,pm, contrato.Admissao, null);

                    if (cb.Valor == 0)
                    {
                        pm.Rollback();
                        LogAcao(itemAgenda.ID, contrato.Numero, String.Concat("Tabela de valor não encontrada para esse período de venda. Contrato Adm: ", contratoAdm.Numero, " - Contrato: ", contrato.Numero), null);
                        continue;
                    }

                    cb.Vigencia = contrato.Vigencia;
                    pm.Save(cb);

                    #endregion

                    //// DECLARACAO_SAUDE - TITULAR ////
                    this.ImportarFichaDeSaude(xPropostaNode, contrato.OperadoraID, beneficiario.ID, auxNode, pm);

                    //// DEPENDENTE ////
                    auxNodeList = xPropostaNode.SelectNodes("DEPENDENTE");
                    dependenteNumSeq = 0;
                    if (auxNodeList != null && auxNodeList.Count > 0)
                    {
                        foreach(XmlNode xDependenteNode in auxNodeList)
                        {
                            if (xDependenteNode.InnerXml == "") { continue; }
                            dependenteNumSeq++;

                            #region DEPENDENTE 

                            beneficiario = new Beneficiario();

                            auxNode = xDependenteNode.SelectSingleNode("NUM_CPF");
                            beneficiario.CPF = auxNode.InnerText;
                            auxNode = xDependenteNode.SelectSingleNode("DATA_NASCIMENTO");
                            beneficiario.DataNascimento = toDateTime(auxNode.InnerText);
                            auxNode = xDependenteNode.SelectSingleNode("NOME_DEPENDENTE");
                            beneficiario.Nome = auxNode.InnerText;
                            auxNode = xDependenteNode.SelectSingleNode("NOME_MAE");
                            beneficiario.NomeMae = auxNode.InnerText;

                            auxNode = xDependenteNode.SelectSingleNode("IND_SEXO");
                            beneficiario.Sexo = TraduzSexo(auxNode.InnerText);

                            beneficiario.ID = Beneficiario.CarregarPorParametro(beneficiario.Nome, beneficiario.NomeMae, pm, beneficiario.DataNascimento, beneficiario.CPF);

                            pm.Save(beneficiario);
                            #endregion

                            #region OPCIONAIS - DEPENDENTE 

                            auxNode = xDependenteNode.SelectSingleNode("IND_OPTOU_DENTAL");
                            if (!String.IsNullOrEmpty(auxNode.InnerText) && auxNode.InnerText.ToUpper() != "N")
                            {
                                if (adicionalId == null)
                                {
                                    pm.Rollback();
                                    LogAcao(itemAgenda.ID, contrato.Numero, String.Concat("Titular não possui adicional, mas dependente sim. Contrato: ", contrato.Numero), null);
                                    falhou = true;
                                    break; ;
                                }

                                adicionalBeneficiario = new AdicionalBeneficiario();
                                adicionalBeneficiario.AdicionalID = adicionalId;
                                adicionalBeneficiario.BeneficiarioID = beneficiario.ID;
                                adicionalBeneficiario.PropostaID = contrato.ID;
                                pm.Save(adicionalBeneficiario);
                            }

                            #endregion

                            #region CONTRATO_BENEFICIARIO - DEPENDENTE 

                            cb = new ContratoBeneficiario();
                            auxNode = xDependenteNode.SelectSingleNode("ALTURA");
                            cb.Altura = toDecimal(auxNode.InnerText);
                            cb.Ativo = true;
                            cb.BeneficiarioID = beneficiario.ID;
                            cb.CarenciaContratoTempo = 0;    //????
                            cb.CarenciaMatriculaNumero = ""; //????
                            cb.ContratoID = contrato.ID;
                            cb.Data = contrato.Admissao;
                            auxNode = xDependenteNode.SelectSingleNode("COD_DEPENDENCIA");
                            cb.ParentescoID = TraduzParentesco(auxNode.InnerText, contrato.ContratoADMID, pm);
                            cb.NumeroSequencial = dependenteNumSeq;
                            auxNode = xDependenteNode.SelectSingleNode("PESO");
                            cb.Peso = toDecimal(auxNode.InnerText);
                            cb.Portabilidade = "";
                            cb.Status = (int)ContratoBeneficiario.eStatus.Incluido;
                            cb.Tipo = (int)ContratoBeneficiario.TipoRelacao.Dependente;
                            //auxNode = xDependenteNode.SelectSingleNode("VAL_MENSALIDADE_DEP");
                            cb.Valor = TabelaValor.CalculaValor(cb.BeneficiarioID, Beneficiario.CalculaIdade(beneficiario.DataNascimento, contrato.Admissao), contrato.ContratoADMID, contrato.PlanoID, (Contrato.eTipoAcomodacao)contrato.TipoAcomodacao, pm ,contrato.Admissao, null); // ConvertHelper.ConvertToDecimal(auxNode.InnerText);//devo somar VAL_MENSALIDADE_DEP com VAL_MENSALIDADE_DENTAL_DEP ???
                            cb.Vigencia = contrato.Vigencia;
                            pm.Save(cb);

                            #endregion

                            //// DECLARACAO_SAUDE ////
                            this.ImportarFichaDeSaude(xDependenteNode, contrato.OperadoraID, beneficiario.ID, auxNode, pm);
                        }
                    }

                    if (falhou) { continue; }

                    #region PRIMEIRA COBRANCA PAGA 

                    Cobranca cobranca = new Cobranca();
                    cobranca.Cancelada = false;
                    cobranca.ComissaoPaga = true;
                    cobranca.ContratoCodCobranca = Convert.ToString(contrato.CodCobranca);
                    cobranca.DataCriacao = DateTime.Now;
                    cobranca.DataPgto = contrato.Admissao;
                    cobranca.DataVencimento = contrato.Admissao;
                    cobranca.Pago = true;
                    cobranca.Parcela = 1;
                    cobranca.PropostaID = contrato.ID;
                    cobranca.Tipo = (int)Cobranca.eTipo.Normal;

                    List<CobrancaComposite> composite = null;
                    cobranca.Valor = Contrato.CalculaValorDaProposta(contrato.ID, contrato.Admissao, pm, false, true, ref composite);

                    cobranca.ValorPgto = cobranca.Valor;

                    if (cobranca.Valor <= 0)
                    {
                        pm.Rollback();
                        LogAcao(itemAgenda.ID, contrato.Numero, "Impossível calcular valor da proposta.", null);
                        continue;
                    }

                    pm.Save(cobranca);
                    CobrancaComposite.Salvar(cobranca.ID, composite, pm);
                    cobranca = null;
                    #endregion

                    if(!String.IsNullOrEmpty(contrato.CorretorTerceiroNome))
                        LogAcao(itemAgenda.ID, contrato.Numero, "Importado com sucesso.", pm);
                    else
                        LogAcao(itemAgenda.ID, contrato.Numero, "Importado com sucesso. ATENCAO: Corretor importado sem nome.", pm);

                    pm.Commit();
                }
                catch (Exception ex)
                {
                    pm.Rollback();
                    LogAcao(itemAgenda.ID, contrato.Numero, ex.Message, null);
                    continue;
                }
                finally
                {
                    pm.Dispose();
                }
            }

            // solução paliativa até a definitiva (aguardando definição da padrao)
            NonQueryHelper.Instance.ExecuteNonQuery("update contrato set contrato_donoId=3718 where contrato_donoId=3678 and contrato_operadoraId=19; update contrato set contrato_donoId=3717 where contrato_donoId=3678 and contrato_operadoraId=18; update contrato set contrato_donoId=3716 where contrato_donoId=3678 and contrato_operadoraId=17;update contrato set contrato_donoId=3719 where contrato_donoId=3678 and contrato_operadoraId=23 ", null);
        }

        /// <summary>
        /// Grava a ficha de saúde do beneficiário.
        /// </summary>
        void ImportarFichaDeSaude(XmlNode xBeneficiarioNode, Object operadoraId, Object beneficiarioId, XmlNode auxNode, PersistenceManager pm)
        {
            auxNode = xBeneficiarioNode.SelectSingleNode("DECLARACAO_SAUDE");
            XmlNodeList nodelist = auxNode.SelectNodes("ITEM");

            if (nodelist == null || nodelist.Count == 0) { return; }

            ItemDeclaracaoSaudeINSTANCIA item = null;
            ItemDeclaracaoSaude def = null;
            foreach (XmlNode node in nodelist)
            {
                auxNode = node.SelectSingleNode("NUM_PERGUNTA");

                def = ItemDeclaracaoSaude.Carregar(operadoraId, auxNode.InnerText, pm);
                if (def == null) { continue; }

                item = new ItemDeclaracaoSaudeINSTANCIA();

                item.ItemDeclaracaoID = def.ID;
                item.BeneficiarioID = beneficiarioId;

                //tenta localizar se o item ja existe
                item.ID = ItemDeclaracaoSaudeINSTANCIA.CarregarID(beneficiarioId, def.ID, pm);

                if (item.ID == null)
                {
                    item.AprovadoPeloDeptoTecnico = true;
                    item.AprovadoPeloMedico = true;
                    item.CIDFinal = "";
                    item.CIDInicial = "";
                }
                else
                    pm.Load(item);
                
                auxNode = node.SelectSingleNode("RESPOSTA");

                auxNode = node.SelectSingleNode("RESPOSTA");
                if (auxNode.InnerText != null && auxNode.InnerText.ToUpper() != "N")
                {
                    auxNode = node.SelectSingleNode("TXT_RESPOSTA");
                    item.Descricao = auxNode.InnerText;
                    item.Sim = true;
                    auxNode = node.SelectSingleNode("ANO_EVENTO");

                    if (!String.IsNullOrEmpty(auxNode.InnerText))
                        item.Data = new DateTime(Int32.Parse(auxNode.InnerText), DateTime.Now.Month, 1);
                    else
                        item.Data = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                }
                else
                    item.Sim = false;

                pm.Save(item);
            }
        }

        void LogAcao(Object itemAgendamentoId, String contratoNumero, String motivo, PersistenceManager pm)
        {
            ItemFalhaImportacao ifi = new ItemFalhaImportacao();
            ifi.ItemAgendamentoID = itemAgendamentoId;
            ifi.Motivo = motivo;
            ifi.NumeroProposta = contratoNumero;
            if (pm == null)
                ifi.Salvar();
            else
                pm.Save(ifi);
            ifi = null;
        }

        [DBTable("importContratoAgendamento")]
        public class ItemAgendamento : EntityBase, IPersisteableEntity 
        {
            #region Fields

            Object _id;
            String _descricao;
            String _arquivo;
            DateTime _inicio;
            Boolean _processado;
            DateTime _processadoData;
            String _erro;

            #endregion

            #region properties

            [DBFieldInfo("ica_id", FieldType.PrimaryKeyAndIdentity)]
            public Object ID
            {
                get { return _id; }
                set { _id = value; }
            }

            [DBFieldInfo("ica_descricao", FieldType.Single)]
            public String Descricao
            {
                get { return _descricao; }
                set { _descricao = value; }
            }

            [DBFieldInfo("ica_arquivo", FieldType.Single)]
            public String Arquivo
            {
                get { return _arquivo; }
                set { _arquivo = value; }
            }

            [DBFieldInfo("ica_processarEm", FieldType.Single)]
            public DateTime ProcessarEm
            {
                get { return _inicio; }
                set { _inicio = value; }
            }

            [DBFieldInfo("ica_processado", FieldType.Single)]
            public Boolean Processado
            {
                get { return _processado; }
                set { _processado = value; }
            }

            [DBFieldInfo("ica_processadoData", FieldType.Single)]
            public DateTime ProcessadoData
            {
                get { return _processadoData; }
                set { _processadoData = value; }
            }

            [DBFieldInfo("ica_erro", FieldType.Single)]
            public String Erro
            {
                get { return _erro; }
                set { _erro= value; }
            }

            #endregion

            public ItemAgendamento(Object id) : this() { _id = id; }
            public ItemAgendamento() { _processado = false; _processadoData = DateTime.MinValue; }

            #region base methods

            public void Salvar()
            {
                base.Salvar(this);
            }

            public void Remover()
            {
                try
                {
                    File.Delete(String.Concat(HttpContext.Current.Server.MapPath("/"), ImportacaoProposta.BaseFileTargetPath.Replace("/", "\\"), this._id, ".xml"));
                }
                catch { }
                base.Remover(this);
            }

            public void Carregar()
            {
                base.Carregar(this);
            }

            #endregion
        }

        [DBTable("importFalha")]
        public class ItemFalhaImportacao : EntityBase, IPersisteableEntity 
        {
            #region Fields

            Object _id;
            Object _itemAgendamentoId;
            String _motivo;
            String _numeroProposta;
            DateTime _data;

            #endregion

            #region Properties

            [DBFieldInfo("if_id", FieldType.PrimaryKeyAndIdentity)]
            public Object ID
            {
                get { return _id; }
                set { _id = value; }
            }

            [DBFieldInfo("if_itemAgendamentoId", FieldType.Single)]
            public Object ItemAgendamentoID
            {
                get { return _itemAgendamentoId; }
                set { _itemAgendamentoId = value; }
            }

            [DBFieldInfo("if_movito", FieldType.Single)]
            public String Motivo
            {
                get { return _motivo; }
                set { _motivo = value; }
            }

            [DBFieldInfo("if_numeroProposta", FieldType.Single)]
            public String NumeroProposta
            {
                get { return _numeroProposta; }
                set { _numeroProposta = value; }
            }

            [DBFieldInfo("if_data", FieldType.Single)]
            public DateTime Data
            {
                get { return _data; }
                set { _data = value; }
            }

            #endregion

            public ItemFalhaImportacao() { _data = DateTime.Now; }
            public ItemFalhaImportacao(Object id) : this() { _id = id; }

            #region Base methods

            public void Salvar()
            {
                base.Salvar(this);
            }

            public void Remover()
            {
                base.Remover(this);
            }

            public void Carregar()
            {
                base.Carregar(this);
            }

            #endregion
        }

        [DBTable("importMapping")]
        public class ItemMapeamento : EntityBase, IPersisteableEntity
        {
            #region Fields

            Object _id;
            String _cpf;
            String _cnpj;
            String _produtor;
            String _nomeGuerra;
            String _inscricaoMunicipal;
            String _filial;
            String _unidadeNegocio;
            String _tipoProdutor;
            String _situacaoEspecial;
            String _tipoPessoa;
            String _situacao;
            String _sexo;
            String _idade;
            String _dataInclusao;
            String _dataExclusao;
            String _enviaEmail;
            String _enviaSMS;
            String _email;
            String _celular;
            String _dependencia;

            #endregion

            #region properties 

            [DBFieldInfo("mapp_id", FieldType.PrimaryKeyAndIdentity)]
            public Object ID
            {
                get { return _id; }
                set { _id = value; }
            }

            [DBFieldInfo("mapp_cpf", FieldType.Single)]
            public String CPF
            {
                get { return _cpf; }
                set { _cpf = value; }
            }

            [DBFieldInfo("mapp_cnpj", FieldType.Single)]
            public String CNPJ
            {
                get { return _cnpj; }
                set { _cnpj= value; }
            }

            [DBFieldInfo("mapp_produtor", FieldType.Single)]
            public String NomeProdutor
            {
                get { return _produtor; }
                set { _produtor= value; }
            }

            [DBFieldInfo("mapp_nomeGuerra", FieldType.Single)]
            public String NomeGuerra
            {
                get { return _nomeGuerra; }
                set { _nomeGuerra= value; }
            }

            [DBFieldInfo("mapp_inscMunicipal", FieldType.Single)]
            public String InscricaoMunicipal
            {
                get { return _inscricaoMunicipal; }
                set { _inscricaoMunicipal= value; }
            }

            [DBFieldInfo("mapp_filial", FieldType.Single)]
            public String Filial
            {
                get { return _filial; }
                set { _filial= value; }
            }

            [DBFieldInfo("mapp_unidadeNegocio", FieldType.Single)]
            public String UnidadeNegocio
            {
                get { return _unidadeNegocio; }
                set { _unidadeNegocio= value; }
            }

            [DBFieldInfo("mapp_tipoProd", FieldType.Single)]
            public String TipoProdutor
            {
                get { return _tipoProdutor; }
                set { _tipoProdutor= value; }
            }

            [DBFieldInfo("mapp_situacaoEspecial", FieldType.Single)]
            public String SituacaoEspecial
            {
                get { return _situacaoEspecial; }
                set { _situacaoEspecial= value; }
            }

            [DBFieldInfo("mapp_tipoPessoa", FieldType.Single)]
            public String TipoPessoa
            {
                get { return _tipoPessoa; }
                set { _tipoPessoa= value; }
            }

            [DBFieldInfo("mapp_situacao", FieldType.Single)]
            public String Situacao
            {
                get { return _situacao; }
                set { _situacao= value; }
            }

            [DBFieldInfo("mapp_sexo", FieldType.Single)]
            public String Sexo
            {
                get { return _sexo; }
                set { _sexo= value; }
            }

            [DBFieldInfo("mapp_idade", FieldType.Single)]
            public String Idade
            {
                get { return _idade; }
                set { _idade= value; }
            }

            [DBFieldInfo("mapp_dataInclusao", FieldType.Single)]
            public String DataInclusao
            {
                get { return _dataInclusao; }
                set { _dataInclusao= value; }
            }

            [DBFieldInfo("mapp_dataExclusao", FieldType.Single)]
            public String DataExclusao
            {
                get { return _dataExclusao; }
                set { _dataExclusao = value; }
            }

            [DBFieldInfo("mapp_enviaEmail", FieldType.Single)]
            public String EnviaEmail
            {
                get { return _enviaEmail; }
                set { _enviaEmail= value; }
            }

            [DBFieldInfo("mapp_enviaSMS", FieldType.Single)]
            public String EnviaSMS
            {
                get { return _enviaSMS; }
                set { _enviaSMS= value; }
            }

            [DBFieldInfo("mapp_email", FieldType.Single)]
            public String Email
            {
                get { return _email; }
                set { _email= value; }
            }

            [DBFieldInfo("mapp_celular", FieldType.Single)]
            public String Celular
            {
                get { return _celular; }
                set { _celular= value; }
            }

            #endregion

            #region Base methods 

            public void Salvar()
            {
                base.Salvar(this);
            }

            public void Remover()
            {
                base.Remover(this);
            }

            public void Carregar()
            {
                base.Carregar(this);
            }

            #endregion

            /// <summary>
            /// Checa se o documento ja existe. Se sim, atualiza. Do contrário, insere.
            /// </summary>
            internal void SaveOrUpdate(PersistenceManager pm)
            {
                if (!String.IsNullOrEmpty(this._cpf))
                {
                    this._id = LocatorHelper.Instance.ExecuteScalar(
                        String.Concat("SELECT mapp_id FROM importMapping WHERE mapp_cpf='", this._cpf, "'"), null, null, pm);
                    pm.Save(this);
                }
                else if(!String.IsNullOrEmpty(this._cnpj))
                {
                    this._id = LocatorHelper.Instance.ExecuteScalar(
                        String.Concat("SELECT mapp_id FROM importMapping WHERE mapp_cnpj='", this._cnpj, "'"), null, null, pm);
                    pm.Save(this);
                }
            }
        }
    }
}