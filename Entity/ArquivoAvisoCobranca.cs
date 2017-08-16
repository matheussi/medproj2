namespace LC.Web.PadraoSeguros.Entity
{
    using System;
    using System.Text;
    using System.Configuration;
    using System.Collections.Generic;

    using LC.Framework.Phantom;

    [Serializable]
    [DBTable("arquivoAvisoCobranca")]
    public class ArquivoAvisoCobranca : EntityBase, IPersisteableEntity
    {
        #region enum 

        public enum eTipoAviso : int
        {
            Voz,
            CartaCancelamento,
            SMS,
            CartaReativacaoBoletoDuplo,
            BoletoViaEmail
        }
        #endregion

        #region fields 

        Object _id;
        Object _operadoraId;
        Int32 _tipoAviso;
        DateTime _dataEmissao;
        Boolean _processado;
        Int32 _mes;
        Int32 _ano;

        #endregion

        #region properties 

        [DBFieldInfo("arquivoaviso_id", FieldType.PrimaryKeyAndIdentity)]
        public Object ID
        {
            get { return _id; }
            set { _id= value; }
        }

        [DBFieldInfo("arquivoaviso_operadoraId", FieldType.Single)]
        public Object OperadoraID
        {
            get { return _operadoraId; }
            set { _operadoraId= value; }
        }

        [DBFieldInfo("arquivoaviso_tipoAviso", FieldType.Single)]
        public Int32 TipoAviso
        {
            get { return _tipoAviso; }
            set { _tipoAviso= value; }
        }

        [DBFieldInfo("arquivoaviso_dataEmissao", FieldType.Single)]
        public DateTime DataEmissao
        {
            get { return _dataEmissao; }
            set { _dataEmissao= value; }
        }

        [DBFieldInfo("arquivoaviso_processado", FieldType.Single)]
        public Boolean Processado
        {
            get { return _processado; }
            set { _processado= value; }
        }

        [DBFieldInfo("arquivoaviso_mes", FieldType.Single)]
        public Int32 Mes
        {
            get { return _mes; }
            set { _mes= value; }
        }

        [DBFieldInfo("arquivoaviso_ano", FieldType.Single)]
        public Int32 Ano
        {
            get { return _ano; }
            set { _ano= value; }
        }

        #endregion

        public ArquivoAvisoCobranca() { _tipoAviso = 0; _dataEmissao = DateTime.Now; _processado = false; }

        #region EntityBase methods 

        public void Salvar()
        {
            base.Salvar(this);
        }

        internal static void Salvar(ArquivoAvisoCobranca arquivo, PersistenceManager pm)
        {
            String qry = String.Concat("SELECT arquivoaviso_id FROM arquivoAvisoCobranca WHERE arquivoaviso_processado=0 AND arquivoaviso_operadoraId=", arquivo.OperadoraID, " AND arquivoaviso_mes=", arquivo.Mes, " AND arquivoaviso_ano=", arquivo.Ano, " AND arquivoaviso_tipoAviso=", arquivo.TipoAviso);
            Object id = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);
            if (id != null && id != DBNull.Value)
                arquivo.ID = id;

            pm.Save(arquivo);
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

        public static void SetarComoProcessado(Object arquivoId)
        {
            String cmd = "UPDATE arquivoAvisoCobranca SET arquivoaviso_processado=1 WHERE arquivoaviso_id=" + arquivoId;
            NonQueryHelper.Instance.ExecuteNonQuery(cmd, null);
        }

        public static Boolean FoiProcessado(Object operadoraId, eTipoAviso tipo, Int32 mes, Int32 ano)
        {
            return FoiProcessado(operadoraId, tipo, mes, ano, null);
        }
        public static Boolean FoiProcessado(Object operadoraId, eTipoAviso tipo, Int32 mes, Int32 ano, PersistenceManager pm)
        {
            String qry = String.Concat("SELECT arquivoaviso_id FROM arquivoAvisoCobranca WHERE arquivoaviso_processado=1 AND arquivoaviso_operadoraId=", operadoraId, " AND arquivoaviso_mes=", mes, " AND arquivoaviso_ano=", ano, " AND arquivoaviso_tipoAviso=", Convert.ToInt32(tipo));
            Object retorno = LocatorHelper.Instance.ExecuteScalar(qry, null, null, pm);

            if (retorno == null || retorno == DBNull.Value)
                return false;
            else
                return true;
        }

        #region Gerar arquivos 

        public static IList<RetornoProcessamentoVO> GeraArquivo(eTipoAviso tipo, String[] operadoraIDs, Int32 mes, Int32 ano)
        {
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                List<RetornoProcessamentoVO> vos = new List<RetornoProcessamentoVO>();
                IList<Cobranca> atrasadas = null;
                System.Data.DataTable dt = null;

                foreach (String operadoraId in operadoraIDs)
                {
                    RetornoProcessamentoVO vo = new RetornoProcessamentoVO();
                    vo.OperadoraNome = Operadora.CarregarNome(operadoraId, pm);

                    if (tipo != eTipoAviso.BoletoViaEmail)
                    {
                        if (FoiProcessado(operadoraId, tipo, mes, ano, pm))
                        {
                            vo.OperadoraNome += " (já enviado)";
                            vo.Processado = true;
                            vo.TipoAviso = Convert.ToInt32(tipo).ToString();
                            vos.Add(vo);
                            continue;
                        }

                        atrasadas = Cobranca.CarregarAtrasadas(operadoraId, mes, ano, pm);

                        if (atrasadas == null || atrasadas.Count == 0)
                        {
                            vo.OperadoraNome += " (nenhuma ocorrência)";
                            vo.Processado = true;
                            vo.TipoAviso = Convert.ToInt32(tipo).ToString();
                            vos.Add(vo);
                            atrasadas = null;
                            continue;
                        }
                    }
                    else
                    {
                        String qry = String.Concat("select cobranca_arquivoUltimoEnvioId, contrato_codcobranca, cobranca_tipo, contrato_contratoAdmId, contrato_admissao, cobranca_id, cobranca_parcela, beneficiario_email,beneficiario_cpf,operadora_nome,beneficiario_nome,cobranca_dataVencimento,cobranca_valor,cobranca_nossoNumero,contrato_numero ",
                            "   from beneficiario ",
                            "       inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_tipo=0 ",
                            "       inner join contrato on contrato_id=contratobeneficiario_contratoId and contrato_cancelado <> 1 ",
                            "       inner join operadora on operadora_id=contrato_operadoraId ",
                            "       inner join cobranca on cobranca_propostaId=contrato_id ",
                            "   where ",
                            Contrato.CondicaoBasicaQuery, " AND ",
                            "       beneficiario_email IS NOT NULL AND beneficiario_email <> '' AND ",
                            "       MONTH(cobranca_dataVencimento)=", mes, " AND ",
                            "       YEAR(cobranca_dataVencimento)=", ano, " AND ",
                            "       cobranca_pago=0 AND operadora_id=", operadoraId,
                            "   ORDER BY cobranca_dataVencimento");

                        dt = LocatorHelper.Instance.ExecuteQuery(qry, "result", pm).Tables[0];

                        if (dt == null || dt.Rows.Count == 0)
                        {
                            vo.OperadoraNome += " (nenhuma ocorrência)";
                            vo.Processado = true;
                            vo.TipoAviso = Convert.ToInt32(tipo).ToString();
                            vos.Add(vo);
                            continue;
                        }
                    }

                    ArquivoAvisoCobranca arquivo = new ArquivoAvisoCobranca();
                    arquivo.DataEmissao = DateTime.Now;
                    arquivo.OperadoraID = operadoraId;
                    arquivo.Processado = false;
                    arquivo.TipoAviso = Convert.ToInt32(tipo);
                    arquivo.Mes = mes;
                    arquivo.Ano = ano;

                    int qtd = 0;
                    if (tipo == eTipoAviso.CartaCancelamento)
                    {
                        vo.ArquivoConteudo = GeraArquivoCARTACancelamento(operadoraId, mes, ano, atrasadas, out qtd, pm);
                    }
                    else if (tipo == eTipoAviso.BoletoViaEmail)
                    {
                        vo.ArquivoConteudo = GeraArquivoBoletoViaEmail(operadoraId, mes, ano, dt, out qtd, pm);
                    }
                    else if (tipo == eTipoAviso.SMS)
                    {
                        vo.ArquivoConteudo = GeraArquivoSMS(operadoraId, mes, ano, atrasadas, out qtd, pm);
                    }
                    else if (tipo == eTipoAviso.Voz)
                    {
                        vo.ArquivoConteudo = GeraArquivoVOZ(operadoraId, mes, ano, atrasadas, out qtd, pm);
                    }
                    else
                    {
                        vo.ArquivoConteudo = GeraArquivoCARTAReativacao(operadoraId, mes, ano, atrasadas, out qtd, pm);
                    }

                    vo.QTD = qtd;
                    if (qtd > 0)
                    {
                        ArquivoAvisoCobranca.Salvar(arquivo, pm);
                        vo.ArquivoAvisoID = Convert.ToString(arquivo.ID);
                        vo.Processado = false;
                    }
                    else
                    {
                        vo.OperadoraNome += " (nenhuma ocorrência)";
                        vo.Processado = true;
                    }

                    vo.OperadoraID = operadoraId;
                    vo.TipoAviso = arquivo.TipoAviso.ToString();

                    vos.Add(vo);
                }

                pm.Commit();
                return vos;
            }
            catch (Exception ex)
            {
                pm.Rollback();
                throw ex;
            }
            finally
            {
                pm = null;
            }
        }

        //TODO: centralizar a lógica que se repete nas três formas de aviso
        static String GeraArquivoVOZ(String operadoraId, Int32 mes, Int32 ano, IList<Cobranca> atrasadas, out Int32 qtd, PersistenceManager pm)
        {
            List<String> contratosProcessados = new List<String>();
            Contrato contrato = null;
            StringBuilder sb = new StringBuilder();
            System.Data.DataTable dt = null;

            DateTime vigencia, vencimento, dataSemJuros, dataLimite = DateTime.MinValue;
            Int32 diaDataSemJuros, aux; Object valorDataLimite;
            String query = null, fone = null, dataPorExtenso = null, mensagem = null;
            qtd = 0;
            CalendarioVencimento rcv = null;

            foreach (Cobranca cobranca in atrasadas)
            {
                //se ja processou a proposta, continua para a próxima
                if (contratosProcessados.Contains(Convert.ToString(cobranca.PropostaID))) { continue; }

                contrato = new Contrato(cobranca.PropostaID);
                pm.Load(contrato);

                if(contrato.Cancelado) { continue; }

                CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(
                    contrato.ContratoADMID, contrato.Admissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, pm);

                dataSemJuros = cobranca.DataVencimento.AddDays(diaDataSemJuros);
                dataSemJuros = new DateTime(dataSemJuros.Year, dataSemJuros.Month, dataSemJuros.Day, 23, 59, 59);

                if (Int32.TryParse(Convert.ToString(valorDataLimite), out aux)) 
                {
                    //aux contém o dia da data limite
                    dataLimite = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, aux, 23, 59, 59);
                }
                else
                {
                    aux = -1; //não há uma data limite em formato legível, mas em texto
                }

                //Se tem dataLimite <> texto e dataSemJuros ja passou e dataLimite ainda nao passou 
                //OU
                //Se dataLimite == texto e dataSemJuros ja passou e MesDataSemJuros <= Mes atual
                if ((aux != -1 && dataSemJuros < DateTime.Now && dataLimite > DateTime.Now) || (dataSemJuros < DateTime.Now && aux > -1 && dataLimite.Month <= DateTime.Now.Month))
                {
                    //Pega o titular do contrato e gera uma linha no arquivo de VOZ
                    query = String.Concat("SELECT contratobeneficiario_id, beneficiario_nome, beneficiario_telefone, beneficiario_telefone2, beneficiario_celular, operadora_mensagemRemessa ",
                        " FROM beneficiario",
                        "   INNER JOIN contrato_beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId ",
                        "   INNER JOIN contrato ON contrato_id=contratobeneficiario_contratoId ",
                        "   INNER JOIN operadora ON contrato_operadoraId = operadora_id ",
                        " WHERE ",
                        "   contratobeneficiario_tipo=0 AND contrato_id=", cobranca.PropostaID);
                    dt = LocatorHelper.Instance.ExecuteQuery(query, "resultset", pm).Tables[0];

                    if (valorCampoFone(dt.Rows[0]["beneficiario_telefone"]) != null)
                    {
                        fone = valorCampoFone(dt.Rows[0]["beneficiario_telefone"]);
                    }
                    else if (valorCampoFone(dt.Rows[0]["beneficiario_telefone2"]) != null)
                    {
                        fone = valorCampoFone(dt.Rows[0]["beneficiario_telefone2"]);
                    }
                    else
                    {
                        continue; //o cliente não tem telefone fixo
                    }

                    mensagem = Convert.ToString(dt.Rows[0]["operadora_mensagemRemessa"]);

                    qtd++; //incrementa a qtd de registros do arquivo

                    if (sb.Length > 0) { sb.Append(Environment.NewLine); }
                    sb.Append(fone);
                    sb.Append(",");
                    sb.Append(dt.Rows[0]["beneficiario_nome"]);
                    sb.Append(",");
                    sb.Append(dt.Rows[0]["contratobeneficiario_id"]);
                    sb.Append(",");
                    sb.Append(dataPorExtenso);
                    sb.Append(",");
                    sb.Append(mensagem);
                    sb.Append(",");
                    sb.Append(cobranca.OperadoraNome.ToUpper());
                    dt.Dispose();

                    contratosProcessados.Add(Convert.ToString(cobranca.PropostaID));//para nao mandar mais que um aviso à mesma pessoa
                }
            }

            return sb.ToString();
        }

        static String GeraArquivoCARTACancelamento(String operadoraId, Int32 mes, Int32 ano, IList<Cobranca> atrasadas, out Int32 qtd, PersistenceManager pm)
        {
            List<String> contratosProcessados = new List<String>();
            Contrato contrato = null;
            StringBuilder sb = new StringBuilder();
            System.Data.DataTable dt = null;
            Endereco endereco = null;

            DateTime vigencia, vencimento, dataSemJuros, dataLimite = DateTime.MinValue;
            Int32 diaDataSemJuros, aux; Object valorDataLimite;
            String query = null, nome = null, logradouro = null, dataLimiteFile = null;
            String bairro = null, cidade = null, uf = null, cep = null;
            CalendarioVencimento rcv = null;
            qtd = 0;

            foreach (Cobranca cobranca in atrasadas)
            {
                //se ja processou a proposta, continua para a próxima
                if (contratosProcessados.Contains(Convert.ToString(cobranca.PropostaID))) { continue; }

                contrato = new Contrato(cobranca.PropostaID);
                pm.Load(contrato);

                if (contrato.Cancelado) { continue; }

                CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(
                    contrato.ContratoADMID, contrato.Admissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, pm);

                dataSemJuros = cobranca.DataVencimento.AddDays(diaDataSemJuros);
                dataSemJuros = new DateTime(dataSemJuros.Year, dataSemJuros.Month, dataSemJuros.Day, 23, 59, 59);

                if (Int32.TryParse(Convert.ToString(valorDataLimite), out aux))
                {
                    //aux contém o dia da data limite
                    dataLimite = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, aux, 23, 59, 59);
                }
                else
                {
                    aux = -1; //não há uma data limite em formato legível, mas em texto
                }

                //Se tem dataLimite <> texto e dataSemJuros ja passou e dataLimite ainda nao passou 
                //OU
                //Se dataLimite == texto e dataSemJuros ja passou e MesDataSemJuros <= Mes atual
                if ((aux != -1 && dataSemJuros < DateTime.Now && dataLimite > DateTime.Now) || (dataSemJuros < DateTime.Now && aux > -1 && dataLimite.Month <= DateTime.Now.Month))
                {
                    //Pega o titular do contrato e gera uma linha no arquivo de VOZ
                    query = String.Concat("SELECT contratobeneficiario_id, beneficiario_id, beneficiario_nome, beneficiario_telefone,beneficiario_telefone2,beneficiario_celular, contrato_enderecoCobrancaId",
                        " FROM beneficiario",
                        "   INNER JOIN contrato_beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId ",
                        "   INNER JOIN contrato ON contrato_id=contratobeneficiario_contratoId ",
                        " WHERE ",
                        "   contratobeneficiario_tipo=0 AND contrato_id=", cobranca.PropostaID);
                    dt = LocatorHelper.Instance.ExecuteQuery(query, "resultset", pm).Tables[0];

                    endereco = new Endereco(dt.Rows[0]["contrato_enderecoCobrancaId"]);
                    pm.Load(endereco);

                    qtd++; //incrementa a qtd de registros do arquivo
                    contratosProcessados.Add(Convert.ToString(cobranca.PropostaID));

                    nome = valorPadRight(dt.Rows[0]["beneficiario_nome"], 40);
                    logradouro = valorPadRight(String.Concat(endereco.Logradouro, ", ", 
                                                endereco.Numero, " ", endereco.Complemento), 65);

                    bairro = valorPadRight(endereco.Bairro, 25);
                    cidade = valorPadRight(endereco.Cidade, 25);
                    uf = valorPadRight(endereco.UF, 2);
                    cep = valorPadRight(endereco.CEP, 8);
                    dataLimiteFile = dataLimite.ToString("ddMMyyyy").Replace("/", "");

                    if (sb.Length > 0) { sb.Append(Environment.NewLine); }

                    //TODO: gera arquivo segundo layout
                    sb.Append(nome);
                    sb.Append(dataLimiteFile);
                    sb.Append(logradouro);
                    sb.Append(bairro);
                    sb.Append(cidade);
                    sb.Append(uf);
                    sb.Append(cep);
                    dt.Dispose();
                }
            }

            return sb.ToString();
        }

        static String GeraArquivoBoletoViaEmail(String operadoraId, Int32 mes, Int32 ano, System.Data.DataTable dt, out Int32 qtd, PersistenceManager pm)
        {
            List<String> contratosProcessados = new List<String>();
            //Contrato contrato = null;
            StringBuilder sb = new StringBuilder();
            //System.Data.DataTable dt = null;

            DateTime vigencia, vencimento, dataLimite = DateTime.MinValue; //dataSemJuros
            Int32 diaDataSemJuros; // aux; 
            Object valorDataLimite;
            //String query = null, fone = null, dataPorExtenso = null, mensagem = null, nome = null, codigo = null;
            qtd = 0;
            Int32 total = 0, result = 0;
            CalendarioVencimento rcv = null;

            String nossoNumero = "";
            Cobranca cobranca = new Cobranca();

            #region cabecalho 

            sb.Append("\t\t\t\tCliente\t\t\t\t\tData vencimento\t\t\tData do documento\t\t\tData do processamento");

            sb.Append(Environment.NewLine);

            sb.Append("Nossonum");
            sb.Append("\t");
            sb.Append("Numdoc");
            sb.Append("\t");
            sb.Append("Valor");
            sb.Append("\t");
            sb.Append("codigo");
            sb.Append("\t");
            sb.Append("Nome");
            sb.Append("\t");
            sb.Append("email");
            sb.Append("\t");
            sb.Append("cod_conf");
            sb.Append("\t");
            sb.Append("instrucoes");
            sb.Append("\t");
            sb.Append("mensagem");
            sb.Append("\t");
            sb.Append("dia"); //vencimento
            sb.Append("\t");
            sb.Append("mês"); //vencimento
            sb.Append("\t");
            sb.Append("ano"); //vencimento
            sb.Append("\t");
            sb.Append("dia"); //data doc
            sb.Append("\t");
            sb.Append("mês"); //data doc
            sb.Append("\t");
            sb.Append("ano"); //data doc
            sb.Append("\t");
            sb.Append("dia"); //data proc
            sb.Append("\t");
            sb.Append("mês"); //data proc
            sb.Append("\t");
            sb.Append("ano"); //data proc

            #endregion

            Int32 totalLista = dt.Rows.Count;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                if (row["beneficiario_email"] == DBNull.Value || Convert.ToString(row["beneficiario_email"]).Trim() == "") { total++; continue; }

                sb.Append(Environment.NewLine);
                qtd++;

                //nosso numero
                if (row["cobranca_nossonumero"] != DBNull.Value)
                {
                    nossoNumero = Convert.ToString(row["cobranca_nossonumero"]).Substring(1);
                }
                else
                {
                    cobranca.Tipo = Convert.ToInt32(row["cobranca_tipo"]);
                    cobranca.ContratoCodCobranca = Convert.ToString(row["contrato_codcobranca"]);
                    cobranca.Parcela = Convert.ToInt32(row["cobranca_parcela"]);

                    nossoNumero = cobranca.GeraNossoNumero().Substring(1);
                }
                sb.Append(nossoNumero.Substring(0, nossoNumero.Length - 1));

                sb.Append("\t");

                //Numdoc
                sb.Append(row["contrato_numero"]);

                sb.Append("\t");

                //Valor
                sb.Append(row["cobranca_valor"]);

                sb.Append("\t");

                //codigo
                sb.Append(row["contrato_codcobranca"]);

                sb.Append("\t");

                //Nome
                sb.Append(row["beneficiario_nome"]);

                sb.Append("\t");

                //email
                sb.Append(Convert.ToString(row["beneficiario_email"]).ToLower().Replace("´", ""));

                sb.Append("\t");

                //cod_conf
                sb.Append("1");

                sb.Append("\t");

                //instrucoes
                CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(row["contrato_contratoAdmId"],
                    Convert.ToDateTime(row["contrato_admissao"]), out vigencia, out vencimento, 
                    out diaDataSemJuros, out valorDataLimite, out rcv, pm);

                if(valorDataLimite == null){ valorDataLimite = ""; }
                if (Int32.TryParse(Convert.ToString(valorDataLimite), out result))
                {
                    //sb.Append("<br>Excepcionalmente neste mes o vencimento foi alterado para dia 21, data limite dia 26 e isento de multa e juros.<br>Nao receber apos 26/09/2011.");
                    //int tempppp = 0;
                }
                else
                {
                    if (EntityBase.RetiraAcentos(Convert.ToString(valorDataLimite)).ToLower().IndexOf("apos") > -1)
                        sb.Append("<br>NAO RECEBER ");
                    else
                        sb.Append("<br>NAO RECEBER APOS ");
                    sb.Append(valorDataLimite);
                }

                sb.Append("<br>EXCEPCIONALMENTE ISENTO DE JUROS E MULTA.<br>");

                sb.Append("\t");

                //mensagem
                sb.Append("");

                sb.Append("\t");

                //dia vencimento 
                sb.Append(Convert.ToDateTime(row["cobranca_dataVencimento"]).Day);

                sb.Append("\t");

                //mes vencimento 
                sb.Append(Convert.ToDateTime(row["cobranca_dataVencimento"]).Month);

                sb.Append("\t");

                //ano vencimento 
                sb.Append(Convert.ToDateTime(row["cobranca_dataVencimento"]).Year);

                sb.Append("\t");

                //dia data doc
                sb.Append(DateTime.Now.Day);

                sb.Append("\t");

                //mes data doc
                sb.Append(DateTime.Now.Month);

                sb.Append("\t");

                //ano data doc
                sb.Append(DateTime.Now.Year);

                sb.Append("\t");

                //dia data proc
                sb.Append(DateTime.Now.Day);

                sb.Append("\t");

                //mes data proc
                sb.Append(DateTime.Now.Month);

                sb.Append("\t");

                //ano data proc
                sb.Append(DateTime.Now.Year);

                #region comentado... 

                //se ja processou a proposta, continua para a próxima
                //if (contratosProcessados.Contains(Convert.ToString(cobranca.PropostaID))) { total++; continue; }

                //contrato = new Contrato(cobranca.PropostaID);
                //pm.Load(contrato);

                //if (contrato.Cancelado || contrato.Inativo) { total++; continue; }

                //CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(
                //    contrato.ContratoADMID, contrato.Admissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, pm);

                //dataSemJuros = cobranca.DataVencimento.AddDays(diaDataSemJuros);
                //dataSemJuros = new DateTime(dataSemJuros.Year, dataSemJuros.Month, dataSemJuros.Day, 23, 59, 59);

                //if (Int32.TryParse(Convert.ToString(valorDataLimite), out aux))
                //{
                //    //aux contém o dia da data limite
                //    dataLimite = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, aux, 23, 59, 59);
                //}
                //else
                //{
                //    aux = -1; //não há uma data limite em formato legível, mas em texto
                //}

                ////Se tem dataLimite <> texto e dataSemJuros ja passou e dataLimite ainda nao passou 
                ////OU
                ////Se dataLimite == texto e dataSemJuros ja passou e MesDataSemJuros <= Mes atual
                //if ((aux != -1 && dataSemJuros < DateTime.Now && dataLimite > DateTime.Now) || (dataSemJuros < DateTime.Now && aux > -1 && dataLimite.Month <= DateTime.Now.Month))
                //{
                //    //TODO: esse método ja existe na ContratoBeneficiario.cs
                //    //Pega o titular do contrato e gera uma linha no arquivo de VOZ
                //    query = String.Concat("SELECT contratobeneficiario_id, beneficiario_nome, beneficiario_celular ",
                //        " FROM beneficiario",
                //        "   INNER JOIN contrato_beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId ",
                //        "   INNER JOIN contrato ON contrato_id=contratobeneficiario_contratoId ",
                //        " WHERE ",
                //        "   contratobeneficiario_tipo=0 AND contrato_id=", cobranca.PropostaID);
                //    dt = LocatorHelper.Instance.ExecuteQuery(query, "resultset", pm).Tables[0];

                //    codigo = valorPadRight(dt.Rows[0]["contratobeneficiario_id"], 30);
                //    nome = valorPadRight(dt.Rows[0]["beneficiario_nome"], 30);
                //    fone = valorCampoFone(dt.Rows[0]["beneficiario_celular"]);
                //    if (fone == null)
                //    {
                //        total++;
                //        continue; //o cliente não tem celular
                //    }

                //    qtd++; //incrementa a qtd de registros do arquivo
                //    contratosProcessados.Add(Convert.ToString(cobranca.PropostaID));

                //    if (sb.Length > 0) { sb.Append(Environment.NewLine); }
                //    //TODO: Escrever arquivo segundo layout

                //    if (qtd == 1)
                //    {
                //        sb.Append("1");
                //        sb.Append(DateTime.Now.ToString("yyyyMMdd").Replace("/", ""));
                //        sb.Append(valorPadRight(ConfigurationManager.AppSettings["smsNomeEmpresa"], 25));
                //        sb.Append(valorPadRight(ConfigurationManager.AppSettings["smsVersaoLayout"], 10));
                //        sb.AppendLine("");
                //        sb.Append("2");
                //        sb.Append(valorPadRight(ConfigurationManager.AppSettings["smsMensagemCobranca"], 135));
                //        sb.AppendLine("");
                //    }

                //    sb.Append("3");
                //    sb.Append(fone);
                //    sb.Append(nome);
                //    sb.Append(codigo);
                //    sb.Append(valorPadRight(ConfigurationManager.AppSettings["smsTelefoneReceptivo"], 20));

                //    total++;

                //    if (total == totalLista)
                //    {
                //        sb.AppendLine("");
                //        sb.Append("9");
                //        sb.Append(qtd.ToString().PadLeft(8, Convert.ToChar("0")));
                //    }
                //    dt.Dispose();
                //}
                #endregion
            }

            return sb.ToString();
        }

        static String GeraArquivoSMS(String operadoraId, Int32 mes, Int32 ano, IList<Cobranca> atrasadas, out Int32 qtd, PersistenceManager pm)
        {
            List<String> contratosProcessados = new List<String>();
            Contrato contrato = null;
            StringBuilder sb = new StringBuilder();
            System.Data.DataTable dt = null;

            DateTime vigencia, vencimento, dataSemJuros, dataLimite = DateTime.MinValue;
            Int32 diaDataSemJuros, aux; Object valorDataLimite;
            String query = null, fone = null, dataPorExtenso = null, mensagem = null, nome = null, codigo = null;
            qtd = 0;
            Int32 total = 0, diaVencto = 0;
            CalendarioVencimento rcv = null;

            Int32 totalLista = atrasadas.Count;
            foreach (Cobranca cobranca in atrasadas)
            {
                if (qtd == 0) { diaVencto = cobranca.DataVencimento.Day; }

                if (diaVencto != cobranca.DataVencimento.Day)
                {
                    sb.AppendLine("");
                    sb.Append("-----------------------------------------------------------------------------------------------------------");
                    diaVencto = cobranca.DataVencimento.Day;
                }

                //se ja processou a proposta, continua para a próxima
                if (contratosProcessados.Contains(Convert.ToString(cobranca.PropostaID))) { total++;  continue; }

                contrato = new Contrato(cobranca.PropostaID);
                pm.Load(contrato);

                if (contrato.Cancelado || contrato.Inativo || cobranca.Pago) { total++; continue; }

                //Pega o titular do contrato e gera uma linha no arquivo de VOZ
                query = String.Concat("SELECT contratobeneficiario_id, beneficiario_nome, beneficiario_celular ",
                    " FROM beneficiario",
                    "   INNER JOIN contrato_beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId ",
                    "   INNER JOIN contrato ON contrato_id=contratobeneficiario_contratoId ",
                    " WHERE ",
                    "   contratobeneficiario_tipo=0 AND contrato_id=", cobranca.PropostaID);
                dt = LocatorHelper.Instance.ExecuteQuery(query, "resultset", pm).Tables[0];

                codigo = Convert.ToString(dt.Rows[0]["contratobeneficiario_id"]).PadRight(30, ' ');
                nome = Convert.ToString(dt.Rows[0]["beneficiario_nome"]).PadRight(30, ' ');
                if (nome.Length > 30) { nome = nome.Substring(0, 30); }
                fone = valorCampoFone(dt.Rows[0]["beneficiario_celular"]);
                if (fone == null)
                {
                    total++;
                    if (total == totalLista)
                    {
                        sb.AppendLine("");
                        sb.Append("9");
                        sb.Append(qtd.ToString().PadLeft(8, Convert.ToChar("0")));
                    }
                    continue; //o cliente não tem celular
                }
                else
                    fone = fone.PadRight(10, ' ');

                qtd++; //incrementa a qtd de registros do arquivo
                contratosProcessados.Add(Convert.ToString(cobranca.PropostaID));

                if (sb.Length > 0) { sb.Append(Environment.NewLine); }
                //TODO: Escrever arquivo segundo layout

                if (qtd == 1)
                {
                    sb.Append("1");
                    sb.Append(DateTime.Now.ToString("yyyyMMdd").Replace("/", ""));
                    sb.Append(valorPadRight(ConfigurationManager.AppSettings["smsNomeEmpresa"], 25));
                    sb.Append(valorPadRight(ConfigurationManager.AppSettings["smsVersaoLayout"], 10));
                    sb.AppendLine("");
                    sb.Append("2");
                    sb.Append(valorPadRight(ConfigurationManager.AppSettings["smsMensagemCobranca"], 135));
                    sb.AppendLine("");
                }

                sb.Append("3");
                sb.Append(fone);
                sb.Append(nome);
                sb.Append(codigo);
                sb.Append(valorPadRight(ConfigurationManager.AppSettings["smsTelefoneReceptivo"], 20).PadRight(20, ' '));

                total++;

                if (total == totalLista)
                {
                    sb.AppendLine("");
                    sb.Append("9");
                    sb.Append(qtd.ToString().PadLeft(8, Convert.ToChar("0")));
                }
                dt.Dispose();
            }

            return sb.ToString();
        }

        static String GeraArquivoCARTAReativacao(String operadoraId, Int32 mes, Int32 ano, IList<Cobranca> atrasadas, out Int32 qtd, PersistenceManager pm)
        {
            List<String> contratosProcessados = new List<String>();
            Contrato contrato = null;
            StringBuilder sb = new StringBuilder();
            System.Data.DataTable dt = null;

            DateTime vigencia, vencimento, dataSemJuros, dataLimite = DateTime.MinValue;
            Int32 diaDataSemJuros, aux; Object valorDataLimite;
            String query = null, fone = null, dataPorExtenso = null, mensagem = null;
            qtd = 0;
            CalendarioVencimento rcv = null;

            foreach (Cobranca cobranca in atrasadas)
            {
                //se ja processou a proposta, continua para a próxima
                if (contratosProcessados.Contains(Convert.ToString(cobranca.PropostaID))) { continue; }

                contrato = new Contrato(cobranca.PropostaID);
                pm.Load(contrato);

                if (contrato.Cancelado) { continue; }

                CalendarioAdmissaoVigencia.CalculaDatasDeVigenciaEVencimento(
                    contrato.ContratoADMID, contrato.Admissao, out vigencia, out vencimento, out diaDataSemJuros, out valorDataLimite, out rcv, pm);

                dataSemJuros = cobranca.DataVencimento.AddDays(diaDataSemJuros);
                dataSemJuros = new DateTime(dataSemJuros.Year, dataSemJuros.Month, dataSemJuros.Day, 23, 59, 59);

                if (Int32.TryParse(Convert.ToString(valorDataLimite), out aux))
                {
                    //aux contém o dia da data limite
                    dataLimite = new DateTime(cobranca.DataVencimento.Year, cobranca.DataVencimento.Month, aux, 23, 59, 59);
                }
                else
                {
                    aux = -1; //não há uma data limite em formato legível, mas em texto
                }

                //Se tem dataLimite <> texto e dataSemJuros ja passou e dataLimite ainda nao passou 
                //OU
                //Se dataLimite == texto e dataSemJuros ja passou e MesDataSemJuros <= Mes atual
                if ((aux != -1 && dataSemJuros < DateTime.Now && dataLimite > DateTime.Now) || (dataSemJuros < DateTime.Now && aux > -1 && dataLimite.Month <= DateTime.Now.Month))
                {
                    //TODO: Escrever arquivo segundo layout

                    //sb.Append(fone);
                    //sb.Append(",");
                    //sb.Append(dt.Rows[0]["beneficiario_nome"]);
                    //sb.Append(",");
                    //sb.Append(dt.Rows[0]["contratobeneficiario_id"]);
                    //sb.Append(",");
                    //sb.Append(dataPorExtenso);
                    //sb.Append(",");
                    //sb.Append(mensagem);
                    //sb.Append(",");
                    //sb.Append(cobranca.OperadoraNome.ToUpper());
                    //dt.Dispose();
                }
            }

            return sb.ToString();
        }

        static String valorCampoFone(Object campo)
        {
            if (campo == null || campo == DBNull.Value)
                return null;
            else if (Convert.ToString(campo).Trim().Replace("(", "").Replace(")", "").Replace(" ", "").Length != 10)
                return null;
            else
                return Convert.ToString(campo).Trim().Replace("(", "").Replace(")", "").Replace(" ", "");
        }

        static String valorPadRight(Object campo, int limit)
        {
            if (campo == null || campo == DBNull.Value)
                return null;
            else if (campo.ToString().Length > limit)
                return campo.ToString().Substring(0, limit - 1);
            else
                return campo.ToString().PadRight(limit);
        }

        #endregion

        [Serializable]
        public class RetornoProcessamentoVO
        {
            #region fields 

            String _arquivoAvisoId;
            String _arquivoConteudo;
            String _operadoraId;
            String _operadoraNome;
            String _tipoAviso;
            DateTime _dataEmissao;
            Boolean _processado;
            Int32 _qtd;

            #endregion

            #region properties 

            public String ArquivoAvisoID
            {
                get { return _arquivoAvisoId; }
                set { _arquivoAvisoId= value; }
            }

            public String ArquivoConteudo
            {
                get { return _arquivoConteudo; }
                set { _arquivoConteudo= value; }
            }

            public String OperadoraID
            {
                get { return _operadoraId; }
                set { _operadoraId= value; }
            }

            public String OperadoraNome
            {
                get { return _operadoraNome; }
                set { _operadoraNome= value; }
            }

            public String TipoAviso
            {
                get { return _tipoAviso; }
                set { _tipoAviso= value; }
            }

            public DateTime DataEmissao
            {
                get { return _dataEmissao; }
                set { _dataEmissao= value; }
            }

            public Boolean Processado
            {
                get { return _processado; }
                set { _processado= value; }
            }

            public Int32 QTD
            {
                get { return _qtd; }
                set { _qtd= value; }
            }

            #endregion

            public RetornoProcessamentoVO() { }
        }
    }
}