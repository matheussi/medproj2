namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.Data;
    using System.Text;
    using System.Linq;
    using System.Collections.Generic;

    using System.Data.SqlClient;
    using System.Configuration;

    using System.Data.OleDb;
    using LC.Framework.Phantom;
    using LC.Web.PadraoSeguros.Entity;

    using NHibernate;
    using NHibernate.Linq;

    using System.Net;

    using Entidades = MedProj.Entidades;
    using System.IO;

    public class ErroSumario
    {
        String _contratoAdmNumero;
        String _codigo;
        String _subPlano;
        String _msg;

        public String ContratoAdmNumero
        {
            get { return _contratoAdmNumero; }
            set { _contratoAdmNumero = value; }
        }

        public String Codigo
        {
            get { return _codigo; }
            set { _codigo = value; }
        }

        public String SubPlano
        {
            get { return _subPlano; }
            set { _subPlano = value; }
        }

        public String MSG
        {
            get { return _msg; }
            set { _msg = value; }
        }
    }

    class AssociadoPJ_X_Beneficiario
    {
        public string AssociadoPjId { get; set; }
        public string BeneficiarioId { get; set; }
    }

    public sealed class ImportFacade : FacadeBase
    {
        #region Singleton 

        static ImportFacade _instance;
        public static ImportFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new ImportFacade(); }
                return _instance;
            }
        }
        #endregion

        enum TipoArquivoDownload : int
        {
            Importacao,
            AtribuicaoProcedimento
        }

        public ImportFacade() { }

        String mdbPath = @"C:\padrao_import\__.accdb";
        String sqlConn = @"Server=187.16.27.102;timeout=1999999999;Database=padrao_producaoDB;USER ID=sa;PWD=!-sql4f34U!65"; //@"Server=MATHEUSSIPC\SQLEXPRESS2008R2;Database=padrao_producaoDB;USER ID=sa;PWD=lcmaster0000;timeout=1999999999";
        Int32 corretorPerilId = 3;

        String   toString(Object param)
        {
            if (param == null || param == DBNull.Value)
                return String.Empty;
            else
                return Convert.ToString(param);
        }
        Int32    toInt32(Object param)
        {
            if (param == null || param == DBNull.Value)
                return 0;
            else
                return Convert.ToInt32(Convert.ToString(param).Replace("'", ""));
        }
        DateTime toDateTime(Object param)
        {
            if (param == null || param == DBNull.Value)
                return DateTime.MinValue;
            else
                return Convert.ToDateTime(param, new System.Globalization.CultureInfo("pt-Br"));
        }
        Decimal  toDecimal(Object param)
        {
            if (param == null || param == DBNull.Value)
                return Decimal.Zero;
            else
            {
                if (Convert.ToString(param).IndexOf("R$") > -1)
                {
                    param = Convert.ToString(param).Replace("R$", "").Trim();
                }
                return Convert.ToDecimal(param, new System.Globalization.CultureInfo("pt-Br"));
            }
        }

        /*****************************************************************/

        /// <summary>
        /// Rotina de importação
        /// </summary>
        public void Importar(Entidades.AgendaImportacao agenda)
        {
           if (agenda == null || agenda.DataConclusao.HasValue) return;

            bool download = this.fazerDownloadSeNecessario(agenda, agenda.Arquivo, TipoArquivoDownload.Importacao);

            List<Entidades.AgendaImportacaoItemLog> log = new List<Entidades.AgendaImportacaoItemLog>();

            using (var sessao = ObterSessao())
            {
                if (!download) 
                {
                    using (ITransaction tran = sessao.BeginTransaction())
                    {
                        agenda.Ativa = false;
                        agenda.Erro = "Não foi possível obter o arquivo.";
                        sessao.Update(agenda);
                        tran.Commit();
                        return;
                    }
                }

                DataTable dados = agenda.ObterDados();

                if (dados == null)
                {
                    using (ITransaction tran = sessao.BeginTransaction())
                    {
                        agenda.Ativa = false;
                        agenda.Erro = "Não foi possível obter os dados do arquivo.";
                        sessao.Update(agenda);
                        tran.Commit();
                        return;
                    }
                }

                int i = 0;

                agenda.Erro = null;
                List<AssociadoPJ_X_Beneficiario> lista = new List<AssociadoPJ_X_Beneficiario>();

                using (var trans = sessao.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    IDbCommand cmdAux = sessao.Connection.CreateCommand();
                    trans.Enlist(cmdAux);

                    try
                    {
                        Int64 ret = 0; object aux = null;
                        string cpf = "", contratoNumero = "";

                        Entidades.Beneficiario beneficiario;

                        bool cepErrado = false;
                        AssociadoPJ_X_Beneficiario itemAssociacao = null;

                        foreach (DataRow row in dados.Rows)
                        {
                            i++;
                            cepErrado = false;

                            //if (i < 1078) continue;

                            //Beneficiário
                            cpf = stoString(row["CPF_TITULAR"]);

                            #region validacoes e salva beneficiario 

                            if  (string.IsNullOrEmpty(cpf) &&
                                (row[1] == DBNull.Value || Convert.ToString(row[1]).Trim() == "") &&
                                (row[2] == DBNull.Value || Convert.ToString(row[2]).Trim() == "") &&
                                (row[3] == DBNull.Value || Convert.ToString(row[3]).Trim() == "") &&
                                (row[4] == DBNull.Value || Convert.ToString(row[4]).Trim() == "") &&
                                (row[5] == DBNull.Value || Convert.ToString(row[5]).Trim() == "") &&
                                (row[6] == DBNull.Value || Convert.ToString(row[6]).Trim() == "") &&
                                (row[8] == DBNull.Value || Convert.ToString(row[8]).Trim() == "") &&
                                (row[9] == DBNull.Value || Convert.ToString(row[9]).Trim() == ""))
                            {
                                continue;
                            }

                            if (string.IsNullOrEmpty(cpf.Trim()))
                            {
                                if (agenda.NaoCriticarCPF == false)
                                {
                                    adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "CPF não informado.");
                                    continue;
                                }
                                else
                                    cpf = "99999999999";
                            }

                            if (string.IsNullOrEmpty(stoString(row["NOME_BENEFICIARIO"]).Trim()))
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Nome não informado.");
                                continue;
                            }

                            if (stoDateTime(row["DT_ADMISSAO"]) == DateTime.MinValue)
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Data Admisão não informada.");
                                continue;
                            }

                            if (stoDateTime(row["DT_VIGENCIA"]) == DateTime.MinValue)
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Data Vigência não informada.");
                                continue;
                            }

                            if (stoDateTime(row["DT_NASCIMENTO"]) == DateTime.MinValue)
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Data Nasc. não informada.");
                                continue;
                            }

                            if (string.IsNullOrEmpty(stoString(row["CEP"]).Trim()))
                            {
                                //adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "CEP não informado.");
                                //continue;
                                cepErrado = true;
                            }

                            if (string.IsNullOrEmpty(stoString(row["LOGRADOURO"]).Trim()))
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Logradouro não informado.");
                                continue;
                            }

                            if (string.IsNullOrEmpty(stoString(row["NUMERO"]).Trim()))
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Número não informado.");
                                continue;
                            }

                            if (string.IsNullOrEmpty(stoString(row["BAIRRO"]).Trim()))
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Bairro não informado.");
                                continue;
                            }

                            if (string.IsNullOrEmpty(stoString(row["CIDADE"]).Trim()))
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Cidade não informada.");
                                continue;
                            }

                            if (string.IsNullOrEmpty(stoString(row["UF"]).Trim()))
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "UF não informada.");
                                continue;
                            }

                            if (stoString(row["BAIRRO"]).Length > 300)
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Bairro invalido.");
                                continue;
                            }
                            if (stoString(row["CEP"]).Replace("-", "").Length > 8)
                            {
                                //adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "CEP invalido.");
                                //continue;
                                cepErrado = true;
                            }
                            if (stoString(row["CIDADE"]).Length > 300)
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Cidade invalida.");
                                continue;
                            }
                            if (stoString(row["COMPLEMENTO"]).Length > 250)
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Complemento invalido.");
                                continue;
                            }
                            if (stoString(row["LOGRADOURO"]).Length > 450)
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Logradouro invalido.");
                                continue;
                            }
                            if (stoString(row["NUMERO"]).Length > 50)
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Numero invalido.");
                                continue;
                            }
                            if (stoString(row["UF"]).Trim().Length > 2)
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "UF invalida.");
                                continue;
                            }

                            if (agenda.AssociadoPj.TipoDataValidade == Entidades.Enuns.TipoDataValidade.Indefinido)
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Impossível determinar validade. Verifique o cadastro do Associado PJ.");
                                continue;
                            }

                            //if (string.IsNullOrEmpty(stoString(row["TIPO"]).Trim()))
                            //{
                            //    adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Tipo de endereço não informado.");
                            //    continue;
                            //}

                            try
                            {
                                if (cpf != "99999999999")
                                {
                                    cpf = cpf.Replace(".", "").Replace("-", "").Replace(@"/", "").PadLeft(11, '0');
                                    beneficiario = sessao.Query<Entidades.Beneficiario>().Where(b => b.CPF.Replace(".", "").Replace("-", "") == cpf).SingleOrDefault();
                                }
                                else
                                {
                                    beneficiario = null;
                                }
                            }
                            catch
                            {
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "CPF com mais de uma ocorrência.");
                                continue;
                            }

                            if (beneficiario == null) //se não existe, cria o titular
                            {
                                #region salva beneficiario 

                                beneficiario = new Entidades.Beneficiario();

                                beneficiario.Celular = dddFone(toString(row["DDD_CEL"]), toString(row["FONE_CEL"])); //string.Concat("(", row["DDD_CEL"], ") ", row["FONE_CEL"]);
                                //beneficiario.CNS = stoString(row["CNS"]);
                                //beneficiario.Altura = 0;
                                beneficiario.CPF = cpf;
                                beneficiario.Data = DateTime.Now;
                                //beneficiario.DataCasamento = DateTime.MinValue;
                                beneficiario.DataNascimento = stoDateTime(row["DT_NASCIMENTO"]);
                                //beneficiario.DeclaracaoNascimentoVivo = stoString(row["DECL_NASC_VIVO"]);
                                beneficiario.Email = stoString(row["EMAIL"]);
                                beneficiario.EstadoCivilId = 0;
                                beneficiario.Nome = stoString(row["NOME_BENEFICIARIO"]);
                                beneficiario.NomeMae = stoString(row["NOME_MAE"]);
                                beneficiario.Peso = 0;
                                beneficiario.Ramal = stoString(row["RAMAL1"]);
                                beneficiario.Ramal2 = stoString(row["RAMAL2"]);
                                beneficiario.RG = stoString(row["RG"]);
                                beneficiario.RGOrgaoExp = stoString(row["RG_ORGAO_EXP"]);
                                beneficiario.RgUF = stoString(row["RG_UF"]);
                                beneficiario.SexoId = traduzSexo(row["SEXO"]).ToString();
                                beneficiario.Telefone = dddFone(toString(row["DDD1"]), toString(row["FONE1"])); //string.Concat("(", row["DDD1"], ") ", row["FONE1"]);
                                beneficiario.Telefone2 = dddFone(toString(row["DDD2"]), toString(row["FONE2"])); //string.Concat("(", row["DDD2"], ") ", row["FONE2"]);

                                sessao.Save(beneficiario);

                                #endregion
                            }
                            else
                            {
                                //deve-se verificar se o contrato atual está cancelado. 
                                //se estiver cancelado, deixa prosseguir
                                bool _continue = false;
                                cmdAux.CommandText = "select contrato_cancelado,contrato_inativo,contrato_estipulanteId,contrato_contratoAdmId from contrato inner join contrato_beneficiario on contrato_id=contratobeneficiario_contratoId where contratobeneficiario_beneficiarioId=" + beneficiario.ID.ToString();
                                using (IDataReader dr = cmdAux.ExecuteReader())
                                {
                                    while (dr.Read())
                                    {
                                        if (Convert.ToInt64(dr[3]) == agenda.Contrato.ID) //if (dr.GetInt64(2) == agenda.AssociadoPj.ID)
                                        {
                                            if (dr.GetBoolean(0) == false && dr.GetBoolean(1) == false)
                                            {
                                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Beneficiario ja cadastrado.");
                                                dr.Close();
                                                _continue = true;
                                                break;
                                            }
                                        }
                                    }

                                    dr.Close();
                                }

                                if (_continue) continue;

                                #region atualiza beneficiario 

                                beneficiario.Celular = dddFone(toString(row["DDD_CEL"]), toString(row["FONE_CEL"])); //string.Concat("(", row["DDD_CEL"], ") ", row["FONE_CEL"]);
                                beneficiario.CPF = cpf;
                                beneficiario.DataNascimento = stoDateTime(row["DT_NASCIMENTO"]);
                                beneficiario.Email = stoString(row["EMAIL"]);
                                beneficiario.Nome = stoString(row["NOME_BENEFICIARIO"]);
                                beneficiario.NomeMae = stoString(row["NOME_MAE"]);
                                beneficiario.Ramal = stoString(row["RAMAL1"]);
                                beneficiario.Ramal2 = stoString(row["RAMAL2"]);
                                beneficiario.RG = stoString(row["RG"]);
                                beneficiario.RGOrgaoExp = stoString(row["RG_ORGAO_EXP"]);
                                beneficiario.RgUF = stoString(row["RG_UF"]);
                                beneficiario.SexoId = traduzSexo(row["SEXO"]).ToString();
                                beneficiario.Telefone = dddFone(toString(row["DDD1"]), toString(row["FONE1"])); //string.Concat("(", row["DDD1"], ") ", row["FONE1"]);
                                beneficiario.Telefone2 = dddFone(toString(row["DDD2"]), toString(row["FONE2"])); //string.Concat("(", row["DDD2"], ") ", row["FONE2"]);

                                sessao.Update(beneficiario);

                                #endregion
                            }

                            #endregion

                            #region salva endereço 

                            //ATENCAO: nao se deve alterar enderecos pre-existentes, pois isso alterará tb os contratos antigos, caso existam
                            Entidades.Endereco endereco = new Entidades.Endereco();

                            endereco.Bairro      = stoString(row["BAIRRO"]);
                            endereco.CEP         = stoString(row["CEP"]).Replace("-", "");
                            endereco.Cidade      = stoString(row["CIDADE"]);
                            endereco.Complemento = stoString(row["COMPLEMENTO"]);
                            endereco.DonoId      = beneficiario.ID;
                            endereco.Logradouro  = stoString(row["LOGRADOURO"]);
                            endereco.Numero      = stoString(row["NUMERO"]);
                            endereco.Tipo        = stoString(row["TIPO"]) == "1" ? 1 : 0;
                            endereco.UF          = stoString(row["UF"]).Trim();

                            sessao.Save(endereco);

                            #endregion

                            Entidades.NumeroCartao numero = null;

                            if (!string.IsNullOrWhiteSpace(stoString(row["CARTAO"])))
                            {
                                //TODO: foi fornecido um número de contrato. VALIDAR
                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, "Número de cartão não pode ser fornecido");
                                continue;
                            }
                            else
                            {
                                numero = new Entidades.NumeroCartao();

                                // Gera um número válido de contrato
                                cmdAux.CommandText = "SELECT MAX(numerocontrato_numero) FROM numero_contrato";
                                aux = cmdAux.ExecuteScalar();

                                if (aux != null && aux != DBNull.Value)
                                {
                                    contratoNumero = (Convert.ToInt64(aux) + 1).ToString();
                                    numero.Numero = contratoNumero;
                                    numero.GerarDigitoVerificador();
                                }
                                else
                                {
                                    numero.GerarNumeroInicial();
                                }

                                numero.Ativo = true;
                                numero.Contrato = null;
                                numero.Data = DateTime.Now;

                                #region antigo
                                //cmdAux.CommandText = "SELECT MAX(numerocontrato_id) FROM numero_contrato";
                                //aux = cmdAux.ExecuteScalar();

                                //if (aux != null && aux != DBNull.Value)
                                //{
                                //    ret = Convert.ToInt64(aux);
                                //    cmdAux.CommandText = "SELECT numerocontrato_numero FROM numero_contrato where numerocontrato_id=" + ret.ToString();
                                //    contratoNumero = (Convert.ToInt64(cmdAux.ExecuteScalar()) + 1).ToString();
                                //    numero.Numero  = contratoNumero;
                                //    numero.GerarDigitoVerificador();
                                //}
                                //else
                                //{
                                //    numero.GerarNumeroInicial();
                                //}

                                //numero.Ativo    = true;
                                //numero.Contrato = null;
                                //numero.Data     = DateTime.Now;
                                #endregion
                            }

                            Entidades.Contrato contrato = new Entidades.Contrato();

                            #region preenche contrato 

                            contrato.Importado = true;
                            contrato.Cancelado = false;
                            contrato.ContratoADMID = agenda.Contrato.ID;
                            contrato.DataAdmissao = stoDateTime(row["DT_ADMISSAO"]);
                            contrato.DataVigencia = stoDateTime(row["DT_VIGENCIA"]);
                            contrato.DonoID = -1;
                            contrato.EmailCobranca = beneficiario.Email;
                            contrato.EnderecoCobrancaID = endereco.ID;
                            contrato.EnderecoReferenciaID = endereco.ID;
                            contrato.EstipulanteID = agenda.AssociadoPj.ID;
                            contrato.FilialID = agenda.Filial.ID;
                            contrato.Inativo = false;
                            contrato.Numero = numero.NumeroCompletoSemCV;
                            contrato.NumeroID = 0; ////////////////////////

                            contrato.OperadoraID = agenda.Operadora.ID;
                            contrato.PlanoID = agenda.Plano.ID;

                            contrato.Matricula = stoString(row["MATRICULA"]);

                            contrato.ResponsavelCPF = stoString(row["CPF_RESP_LEGAL"]);
                            contrato.ResponsavelDataNascimento = stoDateTimeNull(row["DT_NASC_RESP_LEGAL"]); ;
                            contrato.ResponsavelNome = stoString(row["NOME_RESP_LEGAL"]);
                            contrato.ResponsavelRG = stoString(row["RG_RESP_LEGAL"]);
                            contrato.ResponsavelSexo = stoString(row["SEXO_RESP_LEGAL"]).Trim() == "1" ? "Masculino" : "Feminino";

                            contrato.Senha            = ""; /////////////
                            contrato.TipoAcomodacao   = 0;
                            contrato.TipoContratoID   = 1; //straduzTipoContrato(row["TIPO_PROPOSTA"]);
                            contrato.NumeroApolice    = stoStringNULL(row["APOLICE"]);
                            contrato.Ramo             = stoStringNULL(row["RAMO"]);
                            contrato.UsuarioID        = 1;
                            contrato.KitSolicitado    = false;
                            contrato.CartaoSolicitado = false;
                            contrato.CaminhoArquivo   = stoStringNULL(row["TIPO"]);

                            contrato.Produto          = stoStringNULL(row["PRODUTOR"]);

                            if (agenda.AssociadoPj.TipoDataValidade == Entidades.Enuns.TipoDataValidade.DataFixa)
                            {
                                contrato.DataValidade = agenda.AssociadoPj.DataValidadeFixa.Value;
                            }
                            else
                            {
                                contrato.DataValidade = contrato.DataVigencia.AddMonths(agenda.AssociadoPj.MesesAPartirDaVigencia.Value);
                            }

                            #endregion

                            try
                            {
                                sessao.Save(contrato);

                                numero.Contrato = contrato;
                                sessao.Save(numero);

                                contrato.NumeroID = numero.ID;

                                contrato.GerarSenha();

                                sessao.Update(contrato);
                            }
                            catch (Exception ex)
                            {
                                string err = "";
                                if (ex.InnerException == null)
                                    err = ex.Message;
                                else
                                    err = ex.InnerException.Message;

                                adicionaItemLog(agenda, ref log, null, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Erro, err);
                                continue;
                            }

                            Entidades.ContratoBeneficiario cb = new Entidades.ContratoBeneficiario();
                            cb.Ativo        = true;
                            cb.Beneficiario = beneficiario;
                            cb.Contrato     = contrato;
                            cb.Tipo         = 0;
                            cb.Data         = contrato.DataAdmissao;
                            cb.Vigencia     = contrato.DataVigencia;

                            sessao.Save(cb);

                            if(!cepErrado)
                                adicionaItemLog(agenda, ref log, cb, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Ok, null);
                            else
                                adicionaItemLog(agenda, ref log, cb, i, string.Format("CPF: {0}", cpf), Entidades.Enuns.AgendaImportacaoItemLogStatus.Alerta, "CEP invalido");

                            if (agenda.ContratoPjId > 0)
                            {
                                lista.Add
                                (
                                    new AssociadoPJ_X_Beneficiario
                                    {
                                        AssociadoPjId = agenda.ContratoPjId.ToString(), //contrato.EstipulanteID.ToString(),
                                        BeneficiarioId = Convert.ToString(beneficiario.ID)
                                    });
                            }
                        }

                        agenda.DataConclusao = DateTime.Now;
                        sessao.Update(agenda);

                        log.ForEach(l => sessao.Save(l));

                        //SALVA a associação beneficiário x associado pj, sobrescrevendo a anterior
                        //cmdAux.CommandText = string.Concat("select contrato_id from contrato where contrato_estipulanteid=", agenda.AssociadoPj.ID, " and contrato_contratoadmid=", agenda.Contrato.ID);
                        //object id = cmdAux.ExecuteScalar();

                        if(agenda.ContratoPjId > 0)
                        {
                            long id = agenda.ContratoPjId;
                            if (lista != null && lista.Count > 0)
                            {
                                cmdAux.CommandText = "delete from associadopj_beneficiario where assocbenef_associadopjId=" + id;
                                cmdAux.ExecuteNonQuery();

                                foreach (var assoc in lista)
                                {
                                    cmdAux.CommandText = string.Concat(
                                        "insert into associadopj_beneficiario (assocbenef_associadopjId,assocbenef_beneficiarioId) values (", id, ",", assoc.BeneficiarioId, ")");
                                    cmdAux.ExecuteNonQuery();
                                }
                            }
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        string dddFone(string ddd, string fone)
        {
            if (string.IsNullOrWhiteSpace(fone))
                return string.Empty;
            else if (!string.IsNullOrWhiteSpace(ddd))
                return string.Concat("(", ddd, ") ", fone);
            else if(fone.IndexOf('(') > -1)
                return fone;
            else
                return string.Concat("(00) ", fone);
        }

        /// <summary>
        /// Rotina de exportação de dados para confecção de cartão
        /// </summary>
        public string ExportarParaCartao(Entidades.AgendaExportacaoCartao agenda)
        {
            StringBuilder sb = new StringBuilder();

            using (var sessao = ObterSessao())
            {
                int i = 0;
                using (var trans = sessao.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        List<Entidades.Contrato> contratos = sessao.Query<Entidades.Contrato>()
                            .Where(c => c.FilialID == agenda.Filial.ID && c.OperadoraID == agenda.Operadora.ID && c.EstipulanteID == agenda.AssociadoPj.ID && c.ContratoADMID == agenda.Contrato.ID && c.CartaoSolicitado == false && c.Inativo == false && c.Cancelado == false)
                            .OrderBy(c => c.Numero)
                            .ToList();

                        if (contratos == null) { trans.Rollback(); return string.Empty; }

                        string aux = "";
                        Entidades.NumeroCartao numero = null;
                        Entidades.ContratoBeneficiario titular = null;
                        List<Entidades.AgendaExportacaoCartaoItem> log = new List<Entidades.AgendaExportacaoCartaoItem>();

                        List<Entidades.ContratoBeneficiario> titulares = new List<Entidades.ContratoBeneficiario>();
                        foreach (Entidades.Contrato _contrato in contratos)
                        {
                            Entidades.ContratoBeneficiario _titular = sessao.Query<Entidades.ContratoBeneficiario>()
                                .Fetch(cb => cb.Beneficiario)
                                .Where(cb => cb.Contrato.ID == _contrato.ID)
                                .FirstOrDefault();

                            if (_titular == null) continue;

                            _titular.Contrato = _contrato;

                            titulares.Add(_titular);

                        }

                        var ordenados = titulares.OrderBy(t => t.Beneficiario.Nome).ToList();

                        Entidades.Contrato contrato = null;
                        foreach(var t in ordenados) //foreach (Entidades.Contrato contrato in contratos)
                        {
                            i++;

                            contrato = t.Contrato;
                            titular  = t;

                            //titular = sessao.Query<Entidades.ContratoBeneficiario>()
                            //    .Fetch(cb => cb.Beneficiario)
                            //    .Where(cb => cb.Contrato.ID == contrato.ID)
                            //    .FirstOrDefault();

                            //if (titular == null) continue;

                            numero = sessao.Query<Entidades.NumeroCartao>()
                                .Where(nc => nc.Contrato.ID == contrato.ID && nc.Ativo)
                                .FirstOrDefault();

                            if (numero == null) continue;

                            #region conteudo do arquivo

                            if (sb.Length > 0) { sb.Append(Environment.NewLine); }

                            sb.Append("#DCC##EMB#");

                            sb.Append(contrato.Numero.Substring(0, 4));
                            sb.Append(" ");
                            sb.Append(contrato.Numero.Substring(4, 4));
                            sb.Append(" ");
                            sb.Append(contrato.Numero.Substring(8, 4));
                            sb.Append(" ");
                            sb.Append(contrato.Numero.Substring(12, 4));
                            sb.Append(numero.CV);

                            sb.Append("      "); //6 espacos
                            sb.Append("\"");
                            sb.Append("      "); //6 espacos

                            aux = titular.Abreviar2(titular.RetiraAcentos(titular.Beneficiario.Nome.Trim())).ToUpper();

                            if (aux.Length <= 20)
                                sb.Append(aux.PadRight(20, ' '));
                            else
                                sb.Append(aux.Substring(0, 20).PadRight(20, ' '));

                            sb.Append("\"");

                            sb.Append("      "); //6 espacos

                            sb.Append(contrato.Numero.Substring(0, 6)); //primeiro bloco do numero do cartao  + 2dig do segundo bloco

                            sb.Append("      "); //6 espacos

                            sb.Append(contrato.Numero.Substring(0, 4)); //primeiro bloco do numero do cartao

                            sb.Append("\"");

                            sb.Append("      "); //6 espacos

                            sb.Append(contrato.DataValidade.ToString("dd/MM/yyyy")); //TODO: esse campo deve estar em NumeroCartao


                            sb.Append("      "); //6 espacos

                            sb.Append("001"); //TODO: via do cartao

                            sb.Append("#ENC#");

                            if (aux.Length <= 20)
                                sb.Append(aux.PadRight(21, ' '));
                            else
                                sb.Append(aux.Substring(0, 20).PadRight(21, ' '));

                            sb.Append(";");

                            sb.Append(numero.NumeroCompletoSemCV);
                            sb.Append("=");
                            sb.Append(numero.Via.ToString().PadLeft(3, '0')); //sb.Append("001"); //TODO: via do cartao

                            sb.Append(contrato.DataValidade.ToString("MMyy"));                                                              //sb.Append("0142"); //TODO: o que é esse dado?

                            sb.Append("=");

                            sb.Append(contrato.Numero.Substring(0, 6)); //primeiro bloco do numero do cartao  + 2dig do segundo bloco

                            sb.Append(contrato.Numero.Substring(0, 4)); //primeiro bloco do numero do cartao

                            sb.Append(contrato.Numero.Substring(6, 2)); //digitos 1 e 2 do SEGUNDO bloco (?)

                            sb.Append("#END#@@@@@@");

                            #endregion

                            if (titular.Vigencia == DateTime.MinValue) titular.Vigencia = DateTime.Now;
                            log.Add(new Entidades.AgendaExportacaoCartaoItem { Agenda = agenda, Titular = titular });

                            if (contrato.DataVigencia == DateTime.MinValue) contrato.DataVigencia = DateTime.Now;

                            contrato.CartaoSolicitado = true;
                            sessao.Update(contrato);
                        }

                        log.ForEach(l => sessao.Save(l));

                        agenda.DataConclusao = DateTime.Now;
                        sessao.Update(agenda);

                        /****** Gera o arquivo *******/

                        string caminhoBase = ConfigurationManager.AppSettings["appExportCartaoCaminhoFisico"];

                        string arquivo = string.Concat(caminhoBase, agenda.ID, ".txt");
                        System.IO.File.WriteAllText(arquivo, sb.ToString());

                        string ftpBase = ConfigurationManager.AppSettings["ftp"] + agenda.ID.ToString() + ".txt";

                        FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpBase);
                        ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                        ftpRequest.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ftpLogin"], ConfigurationManager.AppSettings["ftpSenha"]);

                        StreamReader sourceStream = new StreamReader(arquivo);
                        byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd()); //Encoding.GetEncoding("iso-8859-1").GetBytes(sourceStream.ReadToEnd());
                        sourceStream.Close();
                        ftpRequest.ContentLength = fileContents.Length;

                        Stream requestStream = ftpRequest.GetRequestStream();
                        requestStream.Write(fileContents, 0, fileContents.Length);
                        requestStream.Close();

                        FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                        ftpResponse.Close();

                        /****************************/

                        trans.Commit();

                        //File.Delete(arquivo);
                    }
                    catch(Exception ex)
                    {
                        trans.Rollback();

                        if (ex.InnerException == null)
                            return ex.Message;
                        else
                            return ex.InnerException.Message;
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Rotina de exportação de dados para confecção do Kit
        /// </summary>
        public string ExportarParaKit(Entidades.AgendaExportacaoKit agenda)
        {
            StringBuilder sb = new StringBuilder();

            //string caminhoBase = ConfigurationManager.AppSettings["appExportKitCaminhoFisico"];

            //string arquivo = string.Concat(caminhoBase, agenda.ID, ".csv");
            //CsvFileWriter writer = new CsvFileWriter(arquivo);

            using (var sessao = ObterSessao())
            {
                using (var trans = sessao.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        List<Entidades.Contrato> contratos = sessao.Query<Entidades.Contrato>()
                            .Where(c => c.FilialID == agenda.Filial.ID && c.OperadoraID == agenda.Operadora.ID && c.EstipulanteID == agenda.AssociadoPj.ID && c.ContratoADMID == agenda.Contrato.ID && c.KitSolicitado == false && c.Inativo == false && c.Cancelado == false)
                            .OrderBy(c => c.Numero)
                            .ToList();

                        if (contratos == null) { trans.Rollback(); return string.Empty; }

                        if (contratos.Count > 100)
                        {
                            int z = 0;
                        }

                        //string aux = "";
                        Entidades.Endereco endereco = null;
                        Entidades.NumeroCartao numero = null;
                        Entidades.ContratoBeneficiario titular = null;
                        List<Entidades.AgendaExportacaoKitItemLog> log = new List<Entidades.AgendaExportacaoKitItemLog>();

                        int i = 0;
                        foreach (Entidades.Contrato contrato in contratos)
                        {
                            i++;

                            if (contrato.EnderecoCobrancaID == 0 && contrato.EnderecoReferenciaID == 0)
                            {
                                continue; //todo: log (tambem na solicitacao de cartao)
                            }

                            titular = sessao.Query<Entidades.ContratoBeneficiario>()
                                .Fetch(cb => cb.Beneficiario)
                                .Where(cb => cb.Contrato.ID == contrato.ID)
                                .FirstOrDefault();

                            if (titular == null) continue; //todo: log (tambem na solicitacao de cartao)

                            numero = sessao.Query<Entidades.NumeroCartao>()
                                .Where(nc => nc.Contrato.ID == contrato.ID && nc.Ativo)
                                .FirstOrDefault();

                            if (numero == null) continue; //todo: log (tambem na solicitacao de cartao)

                            if (contrato.EnderecoReferenciaID > 0)
                            {
                                endereco = sessao.Query<Entidades.Endereco>()
                                    .Where(e => e.ID == contrato.EnderecoReferenciaID)
                                    .SingleOrDefault();
                            }
                            else
                            {
                                endereco = sessao.Query<Entidades.Endereco>()
                                    .Where(e => e.ID == contrato.EnderecoCobrancaID)
                                    .SingleOrDefault();
                            }

                            if (endereco == null) continue;

                            #region conteudo do arquivo

                            CsvRow linha = new CsvRow();

                            if (sb.Length > 0) { sb.Append(Environment.NewLine); }

                            linha.Add(contrato.Numero);
                            //sb.Append("'"); //todo: retirar
                            sb.Append(contrato.Numero); 
                            sb.Append(";");

                            linha.Add(titular.Beneficiario.CPF);
                            sb.Append(titular.Beneficiario.CPF);
                            sb.Append(";");

                            //if (contrato.ID <= 137)
                            //{
                            //    contrato.GerarSenha();
                            //}

                            //while (contrato.Senha.StartsWith("0"))
                            //{
                            //    contrato.GerarSenha();
                            //}

                            linha.Add(contrato.Senha.PadLeft(6, '0'));
                            sb.Append(contrato.Senha.PadLeft(6, '0'));
                            sb.Append(";");

                            if (titular.Beneficiario.DataNascimento != DateTime.MinValue)
                            {
                                linha.Add(titular.Beneficiario.DataNascimento.ToString("dd/MM/yyyy"));
                                sb.Append(titular.Beneficiario.DataNascimento.ToString("dd/MM/yyyy"));
                            }
                            else
                                linha.Add(" ");

                            sb.Append(";");

                            linha.Add(titular.RetiraAcentos(titular.Beneficiario.Nome).ToUpper()); 
                            sb.Append(titular.RetiraAcentos(titular.Beneficiario.Nome).ToUpper()); 
                            sb.Append(";");

                            string end = endereco.RetiraAcentos(endereco.Logradouro).ToUpper();
                            sb.Append(endereco.RetiraAcentos(endereco.Logradouro).ToUpper());

                            if (!string.IsNullOrEmpty(endereco.Numero))
                            {
                                sb.Append(" NR ");
                                sb.Append(endereco.Numero);

                                end += " NR " + endereco.Numero;
                            }

                            if (!string.IsNullOrEmpty(endereco.Complemento))
                            {
                                sb.Append(" ");
                                sb.Append(endereco.RetiraAcentos(endereco.Complemento).ToUpper());

                                end += " " + endereco.RetiraAcentos(endereco.Complemento).ToUpper();
                            }
                            linha.Add(end);
                            sb.Append(";");

                            linha.Add(endereco.RetiraAcentos(endereco.Bairro).ToUpper());
                            sb.Append(endereco.RetiraAcentos(endereco.Bairro).ToUpper());
                            sb.Append(";");

                            linha.Add(endereco.CEP);
                            sb.Append(endereco.CEP);
                            sb.Append(";");

                            linha.Add(endereco.RetiraAcentos(endereco.Cidade).ToUpper());
                            sb.Append(endereco.RetiraAcentos(endereco.Cidade).ToUpper());
                            sb.Append(";");

                            linha.Add(endereco.RetiraAcentos(endereco.UF).ToUpper());
                            sb.Append(endereco.RetiraAcentos(endereco.UF).ToUpper());
                            sb.Append(";");

                            linha.Add(contrato.Matricula);
                            sb.Append(contrato.Matricula);

                            //writer.WriteRow(linha);

                            #endregion

                            log.Add(new Entidades.AgendaExportacaoKitItemLog { Agenda = agenda, Titular = titular });

                            contrato.KitSolicitado = true;
                            sessao.Update(contrato);
                        }

                        log.ForEach(l => sessao.Save(l));

                        agenda.DataConclusao = DateTime.Now;
                        sessao.Update(agenda);

                        /****** Gera o arquivo *******/

                        //writer.Close(); writer.Dispose();

                        string caminhoBase = ConfigurationManager.AppSettings["appExportKitCaminhoFisico"];

                        string arquivo = string.Concat(caminhoBase, agenda.ID, ".csv");
                        System.IO.File.WriteAllText(arquivo, sb.ToString(), Encoding.GetEncoding("iso-8859-1"));

                        string ftpBase = ConfigurationManager.AppSettings["ftpKit"] + agenda.ID.ToString() + ".csv";

                        FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpBase);
                        ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                        ftpRequest.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["ftpLogin"], ConfigurationManager.AppSettings["ftpSenha"]);

                        StreamReader sourceStream = new StreamReader(arquivo);
                        byte[] fileContents = Encoding.GetEncoding("iso-8859-1").GetBytes(sourceStream.ReadToEnd()); //Encoding.UTF8.GetBytes(sourceStream.ReadToEnd()); //Encoding.GetEncoding("iso-8859-1").GetBytes(sourceStream.ReadToEnd());
                        sourceStream.Close();
                        ftpRequest.ContentLength = fileContents.Length;

                        Stream requestStream = ftpRequest.GetRequestStream();
                        requestStream.Write(fileContents, 0, fileContents.Length);
                        requestStream.Close();

                        FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                        ftpResponse.Close();

                        /****************************/

                        trans.Commit();

                        //File.Delete(arquivo);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        if (ex.InnerException == null)
                            return ex.Message;
                        else
                            return ex.InnerException.Message;
                    }
                }
            }

            return sb.ToString();
        }

        void adicionaItemLog(Entidades.AgendaImportacao agenda, ref List<Entidades.AgendaImportacaoItemLog> log, Entidades.ContratoBeneficiario titular, int linha, string chave, Entidades.Enuns.AgendaImportacaoItemLogStatus status, string msg)
        {
            Entidades.AgendaImportacaoItemLog itemLog = new Entidades.AgendaImportacaoItemLog(agenda);
            itemLog.Chave    = chave;
            itemLog.Linha    = linha;
            itemLog.Mensagem = msg;
            itemLog.Status   = status;
            itemLog.Titular  = titular;

            log.Add(itemLog);
        }

        bool fazerDownloadSeNecessario(Entidades.EntidadeBase agenda, string arquivo, TipoArquivoDownload tipo)
        {
            //return true;
            string config = ConfigurationManager.AppSettings["fazerDownload"];
            if (string.IsNullOrEmpty(config) || config.ToLower() == "false") return true;

            string caminhoVirtual = "", caminhoLocal = "", _arquivo = "";

            if (tipo == TipoArquivoDownload.Importacao)
            {
                _arquivo = string.Concat(agenda.ID, Path.GetExtension(arquivo));
                caminhoLocal = string.Concat(ConfigurationManager.AppSettings["appImportCaminhoFisico"], _arquivo);
                caminhoVirtual = string.Concat(ConfigurationManager.AppSettings["appUrl"], "/files/import/", _arquivo);

                if (File.Exists(caminhoLocal)) File.Delete(caminhoLocal);
            }
            else if (tipo == TipoArquivoDownload.AtribuicaoProcedimento)
            {
                _arquivo = string.Concat(agenda.ID, Path.GetExtension(arquivo));
                caminhoLocal = string.Concat(ConfigurationManager.AppSettings["appImportProcCaminhoFisico"], _arquivo);
                caminhoVirtual = string.Concat(ConfigurationManager.AppSettings["appUrl"], "/files/importProc/", _arquivo);

                if (File.Exists(caminhoLocal)) File.Delete(caminhoLocal);
            }
            else
                return false;

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(caminhoVirtual, caminhoLocal);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        /*****************************************************************/

        DataTable obterDadosImportacaoParaUnidades(string arquivoFonte)
        {
            if (string.IsNullOrEmpty(arquivoFonte)) return null;

            string connExcel = "";

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
        }

        public void ImportarProcedicedimentosParaUnidades(Int64[] unidadeIds)
        {
            //TODO: checar se o procedimento a ser inserido ja nao existe.
            if (unidadeIds == null || unidadeIds.Length == 0) return;

            string arquivoFonte = @"C:\Users\ACER E1 572 6830\Documents\biolabor.xlsx";

            DataTable dt = this.obterDadosImportacaoParaUnidades(arquivoFonte);

            if (dt == null) return;

            using (var sessao = ObterSessao())
            {
                using (var trans = sessao.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {
                        Entidades.Procedimento procedimento = null;

                        List<int> codigos = new List<int>();

                        Int64 idTabela = 3;
                        Entidades.TabelaPreco tabela = sessao.Get<Entidades.TabelaPreco>(idTabela);
                        //Entidades.TabelaPreco tabela = null; 

                        if (tabela == null) return;

                        foreach (DataRow row in dt.Rows)
                        {
                            if (codigos.Contains(toInt32(row["Codigo"]))) continue;

                            procedimento = sessao.Query<Entidades.Procedimento>()
                                .Where(p => p.Codigo == toString(row["Codigo"])).FirstOrDefault();

                            if (procedimento == null) continue;

                            foreach (Int64 id in unidadeIds)
                            {
                                Entidades.UnidadeProcedimento up = new Entidades.UnidadeProcedimento();
                                up.Procedimento                  = procedimento;
                                up.TabelaPreco                   = tabela;
                                up.Unidade                       = sessao.Get<Entidades.PrestadorUnidade>(id);
                                up.ValorSobrescrito              = up.ValorCalculado; //quando tem tabela de valor
                                //up.ValorSobrescrito             = toDecimal(row["valor"]); //quando NAO tem tabela de valor;

                                sessao.Save(up);
                            }

                            codigos.Add(toInt32(row["Codigo"]));
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                    }
                }
            }

        }

        /// <summary>
        /// Processa a agenda de importação de procedimentos de um prestador
        /// </summary>
        public void ImportarProcedicedimentosParaUnidades(Entidades.AgendaAtribuicaoProcedimento agenda)
        {
            if (agenda == null || agenda.DataConclusao.HasValue || !agenda.Ativa) return;

            bool download = fazerDownloadSeNecessario(agenda, agenda.Arquivo, TipoArquivoDownload.AtribuicaoProcedimento);

            DataTable dt = agenda.ObterDados();

            string temp = ""; DataRow __row = null;

            if (dt == null)
            {
                agenda.Erro = "Não foi possível ler o arquivo. Favor verificar.";
                agenda.Ativa = false;
                if (agenda.DataConclusao.HasValue == false) agenda.DataConclusao = DateTime.Now;
                AgendaAtribuicaoProcedimentoFacade.Instancia.Salvar(agenda);
                return;
            }

            using (var sessao = ObterSessao())
            {
                agenda = sessao.Query<Entidades.AgendaAtribuicaoProcedimento>()
                    .Fetch(i => i.Tabela)
                    .FetchMany(i => i.Contratos)
                    .Where(i => i.ID == agenda.ID)
                    .Single(); //para pegar todos os objetos do grafo

                Entidades.UnidadeProcedimento aux = null;

                string codigoConsulta = "10101012";

                using (var trans = sessao.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {

                        //delete os procedimentos ja assinalados para a unidade
                        string ids = "";
                        foreach (var unidade in agenda.Contratos)
                        {
                            if (ids.Length > 0) ids += ",";
                            ids += unidade.ID.ToString();
                        }

                        //TODO: Denis, descomentar bloco
                        IDbCommand cmdAux = sessao.Connection.CreateCommand();
                        trans.Enlist(cmdAux);
                        cmdAux.CommandText = "delete from unidade_procedimento where unidade_id in (" + ids + ")";
                        cmdAux.ExecuteNonQuery();
                        cmdAux.Dispose();
                        //////////////////////////////////////////////////////

                        Entidades.Procedimento procedimento = null;
                        List<int> codigos = new List<int>();

                        Entidades.TabelaPreco tabela = null;

                        if (!agenda.TabelaDePrecoViaPlanilha)
                        {
                            if (agenda.Tabela != null && agenda.Tabela.ID > 0)
                                tabela = sessao.Get<Entidades.TabelaPreco>(agenda.Tabela.ID);
                        }

                        //if (tabela == null) return;

                        agenda.Log = new List<Entidades.AgendaAtribProcedRESULTADO>();
                        Entidades.AgendaAtribProcedRESULTADO result = null;

                        bool tabelaOk = false; //bool atencao = false;
                        string iaux = "", especaux = "";
                        Entidades.UnidadeEspecialidade ue = null;

                        foreach (DataRow row in dt.Rows)
                        {
                            //atencao = false;
                            ue = null;

                            if (agenda.TabelaDePrecoViaPlanilha)
                            {
                                try
                                {
                                    tabela = obterTabelaDePreco(sessao, toString(row["tabela"]), out tabelaOk);

                                    if (!tabelaOk)
                                    {
                                        result.Ok = false;
                                        result.Mensagem = string.Concat("Procedimento ", row["Codigo"], ": não foi possível obter a tabela e preços.");
                                        agenda.Erro = "Atencao";
                                        agenda.Log.Add(result);
                                        continue;
                                    }
                                }
                                catch
                                {
                                    result.Ok = false;
                                    result.Mensagem = string.Concat("Procedimento ", row["Codigo"], ": não foi possível obter a tabela e preços.");
                                    agenda.Erro = "Atencao";
                                    agenda.Log.Add(result);
                                    continue;
                                }
                            }

                            result = new Entidades.AgendaAtribProcedRESULTADO(agenda);

                            temp = toString(row["valor"]);
                            __row = row;

                            if (dt.Columns.Contains("Especialidade"))
                            {
                                especaux = toString(row["Especialidade"]).Replace("\n", "");
                            }
                            else
                                especaux = "";

                            iaux = toString(row["Codigo"]).Replace("\n", "");

                            //if (iaux == "10101049") { int tmp = 0; }

                            if (string.IsNullOrWhiteSpace(iaux)) continue;

                            if (codigos.Contains(toInt32(row["Codigo"])) && iaux != codigoConsulta)
                            {
                                result.Ok = false;
                                result.Mensagem = string.Concat("Procedimento ", row["Codigo"], " informado mais de uma vez.");
                                agenda.Erro = "Atencao";
                                agenda.Log.Add(result);
                                continue;
                            }

                            if (iaux == codigoConsulta && string.IsNullOrWhiteSpace(especaux))
                            {
                                result.Ok = false;
                                result.Mensagem = string.Concat("Procedimento ", row["Codigo"], " informado sem especialidade.");
                                agenda.Erro = "Atencao";
                                agenda.Log.Add(result);
                                continue;
                            }

                            if (!string.IsNullOrWhiteSpace(especaux))
                            {
                                if (RetiraAcentos(especaux.ToLower()) == "clinico geral") especaux = "Clínico Geral";
                            }

                            if (!string.IsNullOrEmpty(iaux)) //if (iaux > 0)
                            {
                                if (iaux != codigoConsulta) //string.IsNullOrWhiteSpace(especaux) || 
                                {
                                    procedimento = sessao.Query<Entidades.Procedimento>()
                                        .Where(p => p.Codigo == iaux).FirstOrDefault();
                                }
                                else
                                {
                                    procedimento = sessao.Query<Entidades.Procedimento>()
                                        .Where(p => p.Codigo == iaux && p.Especialidade.ToLower() == especaux.ToLower()) //.Where(p => p.Codigo == iaux && RetiraAcentos(p.Especialidade.ToLower()) == RetiraAcentos(especaux.ToLower()))
                                        .FirstOrDefault();
                                }
                            }
                            else
                                procedimento = null;

                            if (procedimento == null)
                            {
                                result.Ok = false;
                                result.Mensagem = string.Concat("Procedimento ", row["Codigo"], " não localizado.");
                                agenda.Erro = "Atencao";
                                agenda.Log.Add(result);
                                continue;
                            }

                            foreach(var unidade in agenda.Contratos) //foreach (Int64 id in unidadeIds)
                            {
                                //especialidades
                                ue = EspecialidadeFacade.Instance.CarregarEspecialidadeDaUnidade(unidade.ID, procedimento.Especialidade, sessao);
                                if (ue == null)
                                {
                                    ue = new Entidades.UnidadeEspecialidade();
                                    ue.Unidade = sessao.Get<Entidades.PrestadorUnidade>(unidade.ID);
                                    ue.Especialidade = EspecialidadeFacade.Instance.Carregar(procedimento.Especialidade, sessao);

                                    if (ue.Especialidade != null)
                                    {
                                        sessao.SaveOrUpdate(ue);
                                    }
                                }

                                aux = sessao.Query<Entidades.UnidadeProcedimento>()
                                    .Fetch(upr => upr.Procedimento)
                                    .Fetch(upr => upr.TabelaPreco)
                                    .Fetch(upr => upr.Unidade)
                                    .Where(upr => upr.Procedimento.ID == procedimento.ID && upr.Unidade.ID == unidade.ID)
                                    .FirstOrDefault();

                                if (aux != null) //ATUALIZA
                                {
                                    aux.TabelaPreco = tabela;
                                    aux.Importado = true;

                                    if (tabela != null)
                                        aux.ValorSobrescrito = aux.ValorCalculado;        //quando tem tabela de valor
                                    else
                                        aux.ValorSobrescrito = toDecimal(row["valor"]);  //quando NAO tem tabela de valor;

                                    sessao.Update(aux);

                                    result.Ok = true;
                                    result.Procedimento = procedimento;
                                    result.Mensagem = string.Concat("Unidade ", unidade.Nome, " ja possui o procedimento ", row["Codigo"], ". Procedimento atualizado.");
                                    result.ContratoDePrestador = sessao.Get<Entidades.PrestadorUnidade>(unidade.ID);
                                    agenda.Log.Add(result);
                                }
                                else
                                {
                                    Entidades.UnidadeProcedimento up = new Entidades.UnidadeProcedimento();
                                    up.Procedimento = procedimento;
                                    up.TabelaPreco = tabela;
                                    up.Unidade = sessao.Get<Entidades.PrestadorUnidade>(unidade.ID);
                                    up.Importado = true;

                                    if (tabela != null)
                                        up.ValorSobrescrito = up.ValorCalculado;        //quando tem tabela de valor
                                    else
                                        up.ValorSobrescrito = toDecimal(row["valor"]);  //quando NAO tem tabela de valor;

                                    sessao.Save(up);

                                    if (tabela == null && up.ValorSobrescrito == decimal.Zero)
                                    {
                                        result.Mensagem = "Não foi possível atribuir o valor: " + row["Codigo"];
                                        result.Ok = false;

                                        if (string.IsNullOrEmpty(agenda.Erro)) agenda.Erro = "Atencao";
                                    }
                                    else
                                    {
                                        result.Ok = true;
                                    }

                                    result.Procedimento = procedimento;
                                    result.ContratoDePrestador = sessao.Get<Entidades.PrestadorUnidade>(unidade.ID);
                                    agenda.Log.Add(result);
                                }
                            }

                            codigos.Add(toInt32(row["Codigo"]));
                        }

                        agenda.DataConclusao = DateTime.Now;
                        agenda.Processado = true;
                        sessao.SaveOrUpdate(agenda);

                        agenda.Log.ForEach(l => sessao.Save(l));

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public void ImportarProcedicedimentosParaUnidades_TESTE(Entidades.AgendaAtribuicaoProcedimento agenda)
        {
            if (agenda == null) return;

            bool download = fazerDownloadSeNecessario(agenda, agenda.Arquivo, TipoArquivoDownload.AtribuicaoProcedimento);

            DataTable dt = agenda.ObterDados();

            string temp = ""; DataRow __row = null;

            if (dt == null)
            {
                agenda.Erro = "Não foi possível ler o arquivo. Favor verificar.";
                agenda.Ativa = false;
                if (agenda.DataConclusao.HasValue == false) agenda.DataConclusao = DateTime.Now;
                AgendaAtribuicaoProcedimentoFacade.Instancia.Salvar(agenda);
                return;
            }

            using (var sessao = ObterSessao())
            {
                agenda = sessao.Query<Entidades.AgendaAtribuicaoProcedimento>()
                    .Fetch(i => i.Tabela)
                    .FetchMany(i => i.Contratos)
                    .Where(i => i.ID == agenda.ID)
                    .Single(); //para pegar todos os objetos do grafo

                Entidades.UnidadeProcedimento aux = null;

                string codigoConsulta = "10101012";

                using (var trans = sessao.BeginTransaction(IsolationLevel.Snapshot))
                {
                    try
                    {

                        //delete os procedimentos ja assinalados para a unidade
                        string ids = "";
                        foreach (var unidade in agenda.Contratos)
                        {
                            if (ids.Length > 0) ids += ",";
                            ids += unidade.ID.ToString();
                        }

                        //TODO: Denis, descomentar bloco
                        IDbCommand cmdAux = sessao.Connection.CreateCommand();
                        trans.Enlist(cmdAux);
                        cmdAux.CommandText = "delete from unidade_procedimento where unidade_id in (" + ids + ")";
                        //cmdAux.ExecuteNonQuery();
                        cmdAux.Dispose();
                        //////////////////////////////////////////////////////

                        Entidades.Procedimento procedimento = null;
                        List<int> codigos = new List<int>();

                        Entidades.TabelaPreco tabela = null;

                        if (!agenda.TabelaDePrecoViaPlanilha)
                        {
                            if (agenda.Tabela != null && agenda.Tabela.ID > 0)
                                tabela = sessao.Get<Entidades.TabelaPreco>(agenda.Tabela.ID);
                        }

                        //if (tabela == null) return;

                        agenda.Log = new List<Entidades.AgendaAtribProcedRESULTADO>();
                        Entidades.AgendaAtribProcedRESULTADO result = null;

                        bool tabelaOk = false; //bool atencao = false;
                        string iaux = "", especaux = "";
                        Entidades.UnidadeEspecialidade ue = null;

                        foreach (DataRow row in dt.Rows)
                        {
                            //atencao = false;
                            ue = null;

                            if (agenda.TabelaDePrecoViaPlanilha)
                            {
                                try
                                {
                                    tabela = obterTabelaDePreco(sessao, toString(row["tabela"]), out tabelaOk);

                                    if (!tabelaOk)
                                    {
                                        result.Ok = false;
                                        result.Mensagem = string.Concat("Procedimento ", row["Codigo"], ": não foi possível obter a tabela e preços.");
                                        agenda.Erro = "Atencao";
                                        agenda.Log.Add(result);
                                        continue;
                                    }
                                }
                                catch
                                {
                                    result.Ok = false;
                                    result.Mensagem = string.Concat("Procedimento ", row["Codigo"], ": não foi possível obter a tabela e preços.");
                                    agenda.Erro = "Atencao";
                                    agenda.Log.Add(result);
                                    continue;
                                }
                            }

                            result = new Entidades.AgendaAtribProcedRESULTADO(agenda);

                            temp = toString(row["valor"]);
                            __row = row;

                            if (dt.Columns.Contains("Especialidade"))
                            {
                                especaux = toString(row["Especialidade"]).Replace("\n", "");
                            }
                            else
                                especaux = "";

                            iaux = toString(row["Codigo"]).Replace("\n", "");

                            //if (iaux == "10101049") { int tmepo = 0; }

                            if (string.IsNullOrWhiteSpace(iaux)) continue;

                            if (codigos.Contains(toInt32(row["Codigo"])) && iaux != codigoConsulta)
                            {
                                result.Ok = false;
                                result.Mensagem = string.Concat("Procedimento ", row["Codigo"], " informado mais de uma vez.");
                                agenda.Erro = "Atencao";
                                agenda.Log.Add(result);
                                continue;
                            }

                            if (iaux == codigoConsulta && string.IsNullOrWhiteSpace(especaux))
                            {
                                result.Ok = false;
                                result.Mensagem = string.Concat("Procedimento ", row["Codigo"], " informado sem especialidade.");
                                agenda.Erro = "Atencao";
                                agenda.Log.Add(result);
                                continue;
                            }

                            if (!string.IsNullOrWhiteSpace(especaux))
                            {
                                if (RetiraAcentos(especaux.ToLower()) == "clinico geral") especaux = "Clínico Geral";
                            }

                            if (!string.IsNullOrEmpty(iaux)) //if (iaux > 0)
                            {
                                if (iaux != codigoConsulta) //string.IsNullOrWhiteSpace(especaux) || 
                                {
                                    procedimento = sessao.Query<Entidades.Procedimento>()
                                        .Where(p => p.Codigo == iaux).FirstOrDefault();
                                }
                                else
                                {
                                    procedimento = sessao.Query<Entidades.Procedimento>()
                                        .Where(p => p.Codigo == iaux && p.Especialidade.ToLower() == especaux.ToLower()) //.Where(p => p.Codigo == iaux && RetiraAcentos(p.Especialidade.ToLower()) == RetiraAcentos(especaux.ToLower()))
                                        .FirstOrDefault();
                                }
                            }
                            else
                                procedimento = null;

                            if (procedimento == null)
                            {
                                result.Ok = false;
                                result.Mensagem = string.Concat("Procedimento ", row["Codigo"], " não localizado.");
                                agenda.Erro = "Atencao";
                                agenda.Log.Add(result);
                                continue;
                            }

                            foreach (var unidade in agenda.Contratos) //foreach (Int64 id in unidadeIds)
                            {
                                //especialidades
                                ue = EspecialidadeFacade.Instance.CarregarEspecialidadeDaUnidade(unidade.ID, procedimento.Especialidade, sessao);
                                if (ue == null)
                                {
                                    ue = new Entidades.UnidadeEspecialidade();
                                    ue.Unidade = sessao.Get<Entidades.PrestadorUnidade>(unidade.ID);
                                    ue.Especialidade = EspecialidadeFacade.Instance.Carregar(procedimento.Especialidade, sessao);

                                    if (ue.Especialidade != null)
                                    {
                                        //sessao.SaveOrUpdate(ue);
                                    }
                                }

                                aux = sessao.Query<Entidades.UnidadeProcedimento>()
                                    .Fetch(upr => upr.Procedimento)
                                    .Fetch(upr => upr.TabelaPreco)
                                    .Fetch(upr => upr.Unidade)
                                    .Where(upr => upr.Procedimento.ID == procedimento.ID && upr.Unidade.ID == unidade.ID)
                                    .FirstOrDefault();

                                if (aux != null) //ATUALIZA
                                {
                                    aux.TabelaPreco = tabela;
                                    aux.Importado = true;

                                    if (tabela != null)
                                        aux.ValorSobrescrito = aux.ValorCalculado;        //quando tem tabela de valor
                                    else
                                        aux.ValorSobrescrito = toDecimal(row["valor"]);  //quando NAO tem tabela de valor;

                                    //sessao.Update(aux);

                                    result.Ok = true;
                                    result.Procedimento = procedimento;
                                    result.Mensagem = string.Concat("Unidade ", unidade.Nome, " ja possui o procedimento ", row["Codigo"], ". Procedimento atualizado.");
                                    result.ContratoDePrestador = sessao.Get<Entidades.PrestadorUnidade>(unidade.ID);
                                    agenda.Log.Add(result);
                                }
                                else
                                {
                                    Entidades.UnidadeProcedimento up = new Entidades.UnidadeProcedimento();
                                    up.Procedimento = procedimento;
                                    up.TabelaPreco = tabela;
                                    up.Unidade = sessao.Get<Entidades.PrestadorUnidade>(unidade.ID);
                                    up.Importado = true;

                                    if (tabela != null)
                                        up.ValorSobrescrito = up.ValorCalculado;        //quando tem tabela de valor
                                    else
                                        up.ValorSobrescrito = toDecimal(row["valor"]);  //quando NAO tem tabela de valor;

                                    sessao.Save(up);

                                    if (tabela == null && up.ValorSobrescrito == decimal.Zero)
                                    {
                                        result.Mensagem = "Não foi possível atribuir o valor: " + row["Codigo"];
                                        result.Ok = false;

                                        if (string.IsNullOrEmpty(agenda.Erro)) agenda.Erro = "Atencao";
                                    }
                                    else
                                    {
                                        result.Ok = true;
                                    }

                                    result.Procedimento = procedimento;
                                    result.ContratoDePrestador = sessao.Get<Entidades.PrestadorUnidade>(unidade.ID);
                                    agenda.Log.Add(result);
                                }
                            }

                            codigos.Add(toInt32(row["Codigo"]));
                        }

                        agenda.DataConclusao = DateTime.Now;
                        agenda.Processado = true;
                        //sessao.SaveOrUpdate(agenda);

                        //agenda.Log.ForEach(l => sessao.Save(l));

                        trans.Rollback();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        Entidades.TabelaPreco obterTabelaDePreco(ISession sessao, string codigoTabela, out bool tabelaOk)
        {
            tabelaOk = false;
            Entidades.TabelaPreco tabela = null;

            switch (codigoTabela.ToLower())
            {
                case "a":
                {
                    tabela = sessao.Get<Entidades.TabelaPreco>((long)12);
                    tabelaOk = true;
                    break;
                }
                case "b":
                {
                    tabela = sessao.Get<Entidades.TabelaPreco>((long)3);
                    tabelaOk = true;
                    break;
                }
                case "c":
                {
                    tabela = sessao.Get<Entidades.TabelaPreco>((long)10);
                    tabelaOk = true;
                    break;
                }
                case "d":
                {
                    tabela = sessao.Get<Entidades.TabelaPreco>((long)11);
                    tabelaOk = true;
                    break;
                }
                case "e":
                {
                    tabela = sessao.Get<Entidades.TabelaPreco>((long)8);
                    tabelaOk = true;
                    break;
                }
                case "esp":
                {
                    tabela = null;
                    tabelaOk = true;
                    break;
                }
            }

            return tabela;
        }

        /*****************************************************************/

        string stoString(object param)
        {
            if (param == null || param == DBNull.Value)
                return string.Empty;
            else
                return Convert.ToString(param);
        }

        string stoStringNULL(object param)
        {
            if (param == null || param == DBNull.Value)
                return null;
            else
                return Convert.ToString(param);
        }

        DateTime stoDateTime(object param)
        {
            if (param == null || param == DBNull.Value)
                return DateTime.MinValue;

            string[] arr = Convert.ToString(param).Split('/');

            if (arr.Length != 3) return DateTime.MinValue;
            arr[2] = arr[2].Replace("00:00:00", "").Trim();

            DateTime data = DateTime.MinValue;
            try
            {
                if (Convert.ToInt32(arr[2].Trim()) < 2000 && arr[2].Trim().Length == 2)
                {
                    arr[2] = "20" + arr[2].Trim();
                }
                else if (Convert.ToInt32(arr[2].Trim()) < 2000 && arr[2].Trim().Length == 3)
                {
                    arr[2] = "20" + Convert.ToInt32(arr[2].Trim());
                }

                data = new DateTime(Int32.Parse(arr[2].Trim()), Int32.Parse(arr[1].Trim()), Int32.Parse(arr[0].Trim()));
            }
            catch
            {
                return DateTime.MinValue;
            }

            return data;
        }

        DateTime? stoDateTimeNull(object param)
        {
            if (param == null || param == DBNull.Value)
                return null;

            string[] arr = Convert.ToString(param).Split('/');

            if (arr.Length != 3) return null;

            DateTime data = new DateTime(Int32.Parse(arr[2]), Int32.Parse(arr[1]), Int32.Parse(arr[0]));

            return data;
        }

        int straduzTipoContrato(object tipo)
        {
            string _t = stoString(tipo);

            if (_t == "1")
                return 1;
            else if (_t == "2")
                return 2;
            else if (_t == "3")
                return 3;
            else if (_t == "4")
                return 4;
            else if (_t == "5")
                return 5;
            else
                return 1;
        }

        //String traduzTipoConta(Object tipoConta)
        //{
        //    if (tipoConta == null || tipoConta == DBNull.Value)
        //        return "1"; //conta corrente

        //    return "";
        //}

        /// <summary>
        /// TODO
        /// </summary>
        int traduzEstadoCivil(Object param)
        {
            if (param == null || param == DBNull.Value)
                return 1;

            return 1;
        }
        int traduzSexo(Object param)
        {
            if (param == null || param == DBNull.Value)
                return 0;

            if (Convert.ToString(param).ToUpper() == "M")
                return 1;
            else
                return 0;
        }
        int traduzTipoPessoa(Object param)
        {
            if (param == null || param == DBNull.Value)
                return 1;

            if (Convert.ToString(param).ToUpper() == "PJ")
                return 2;
            else
                return 1;
        }
        Object traduzTipoContrato(Object param)
        {
            String tipo = toString(param);

            if (tipo.ToUpper() == "NOVO")
                return 1;
            else if (tipo.ToUpper() == "ADMINISTRATIVA")
                return 2;
            else if (tipo.ToUpper() == "MIGRACAO")
                return 3;
            else if (tipo.ToUpper() == "COMPRA DE CARENCIA")
                return 4;
            else if (tipo.ToUpper() == "ESPECIAL")
                return 5;

            return null;
        }
        int traduzTipoEndereco(Object param)
        {
            if (toString(param).ToUpper().Trim() == "RESIDENCIAL")
                return Convert.ToInt32(Endereco.TipoEndereco.Residencial);
            else
                return Convert.ToInt32(Endereco.TipoEndereco.Comercial);
        }

        String traduzTipoAcomodacaoParaColuna(Object param)
        {
            if(toString(param) == "QP")
                return " tabelavaloritem_qParticular ";
            else
                return " tabelavaloritem_qComum ";
        }

        ////////////////////////////////////////////////////////////////////////////////////////

        public void ImportarCorretores()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM CORRETORES", conn);
                adp.Fill(dsOrigem, "corretores");
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            Usuario corretor = null;

            try
            {
                IList<Usuario> ret = null;
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    ret = Usuario.CarregarCorretorPorDoc(toString(row["CPF_CNPJ"]), pm);
                    if (ret != null)
                        corretor = ret[0];
                    else
                        corretor = new Usuario();

                    corretor.Agencia                = toString(row["AGENCIA"]);
                    corretor.AlteraValorContratos   = false;
                    corretor.Apelido                = toString(row["APELIDO"]);
                    corretor.Ativo = true;
                    corretor.Banco = toString(row["BANCO"]);
                    corretor.CategoriaID = null;
                    corretor.Celular = toString(row["CELULAR"]);
                    corretor.CelularOperadora = toString(row["CEL_OPERADORA"]);
                    corretor.Codigo = ""; // ToString(row["CEL_OPERADORA"]);
                    corretor.Conta = toString(row["CONTA"]);
                    corretor.ContaTipo = toString(row["TP_CONTA"]);
                    corretor.DataNascimento = toDateTime(row["DATA_NASC"]);
                    if (corretor.DataNascimento.Year < 1753)
                        corretor.DataNascimento = DateTime.MinValue;

                    corretor.DDD1 = toString(row["DDD_1"]);
                    corretor.DDD2 = toString(row["DDD_2"]);
                    corretor.DDD3 = toString(row["CEL_DDD"]);
                    corretor.Documento1 = toString(row["CPF_CNPJ"]);
                    corretor.Documento2 = toString(row["RG_IE"]);
                    corretor.Email = toString(row["EMAIL"]);
                    corretor.EntrevistadoEm = toDateTime(row["DATA_ENTREVISTA"]);
                    corretor.EntrevistadoPor = toString(row["ENTREVISTADOR"]);
                    corretor.EstadoCivil = traduzEstadoCivil(row["ENTREVISTADOR"]);
                    corretor.Favorecido = toString(row["FAVORECIDO"]);
                    corretor.Fone1 = toString(row["TEL1"]);
                    corretor.Fone2 = toString(row["TEL2"]);
                    corretor.LiberaContratos = false;
                    corretor.Nome = toString(row["NOME_CORR"]);
                    corretor.Obs = toString(row["OBS"]);
                    corretor.PerfilID = corretorPerilId;
                    corretor.Ramal1 = toString(row["RAMAL_1"]);
                    corretor.Ramal1 = toString(row["RAMAL_2"]);
                    corretor.Senha = corretor.Documento1;
                    corretor.Sexo = traduzSexo(row["SEXO"]);
                    corretor.SystemUser = false;
                    corretor.TipoPessoa = traduzTipoPessoa(row["TIPO"]);

                    pm.Save(corretor);
                }

                dsOrigem.Dispose();
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
            }
        }
        public void ImportarCorretores_Acerto()
        {
            String qry = "select usuario_id, usuario_filialId, usuariofilial_filialId from usuario inner join usuario_filial on usuariofilial_usuarioId=usuario_id where usuario_filialid is null and usuario_perfilid=" + corretorPerilId;
            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];

            StringBuilder sb = new StringBuilder();
            foreach (DataRow row in dt.Rows)
            {
                if (sb.Length > 0) { sb.Append(" ; "); }
                sb.Append("UPDATE usuario SET usuario_filialId = ");
                sb.Append(row["usuariofilial_filialId"]);
                sb.Append(" WHERE usuario_id=");
                sb.Append(row["usuario_id"]);
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }
        }

        /// <summary>
        /// Importar os Beneficiários de um MDB. No caso de Windows 7 colocar o 
        /// MDB em uma pasta que tenha permissão, não utilize o Desktop.
        /// </summary>
        /// <param name="MDBPath">Caminho do MDB.</param>
        public void ImportarBeneficiarios()
        {
            #region Table Header 

            const String NomeColumn = "NOME_BENEFICIARIO";
            const String SexoColumn = "SEXO";
            const String CPFColumn = "CPF";
            const String RGColumn = "RG";
            const String DataNascimentoColumn = "NASCIMENTO";
            const String EmailColumn = "EMAIL";
            const String NomeMaeColumn = "NOME_MAE";
            const String DDD1Column = "DDD1";
            const String Telefone1Column = "TEL1";
            const String Ramal1Column = "RAMAL1";
            const String DDD2Column = "DDD2";
            const String Telefone2Column = "TEL2";
            const String Ramal2Column = "RAMAL2";
            const String CelDDDColumn = "CEL_DDD";
            const String CelColumn = "CEL";
            const String CelOperadoraColumn = "CEL_OPERADORA";
            //const String TipoLogr1Column = "TIPO_LOGR1";
            //const String Logr1Column = "LOGRADOURO1";
            //const String NumLogr1Column = "NUM_LOGR1";
            //const String ComplLogr1Column = "COMPL_LOGR1";
            //const String Bairro1Column = "BAIRRO1";
            //const String Cidade1Column = "CIDADE1";
            //const String UF1Column = "UF1";
            //const String CEP1Column = "CEP1";
            //const String TipoEnd1Column = "TIPO_END1";
            //const String TipoLogr2Column = "TIPO_LOGR2";
            //const String Logr2Column = "LOGRADOURO2";
            //const String NumLogr2Column = "NUM_LOGR2";
            //const String ComplLogr2Column = "COMPL_LOGR2";
            //const String Bairro2Column = "BAIRRO2";
            //const String Cidade2Column = "CIDADE2";
            //const String UF2Column = "UF2";
            //const String CEP2Column = "CEP2";
            //const String TipoEnd2Column = "TIPO_END2";

            #endregion

            String connectionString = String.Concat("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=", mdbPath, ";User Id=Admin;");

            OleDbConnection connectionMDB = new OleDbConnection(connectionString);

            try
            {
                connectionMDB.Open();
            }
            catch (Exception ex) { throw ex; }

            OleDbCommand cmdMDB = connectionMDB.CreateCommand();

            cmdMDB.CommandType = CommandType.Text;
            cmdMDB.CommandText = "SELECT * FROM beneficiarios";

            OleDbDataReader drBeneficiario = cmdMDB.ExecuteReader();

            #region BeneficiarioImportVars 

            String beneficiarioNome;
            String beneficiarioSexo;
            String beneficiarioCPF;
            String beneficiarioRG;
            String beneficiarioDataNascimento;
            String beneficiarioEmail;
            String beneficiarioNomeMae;
            String beneficiarioDDD1;
            String beneficiarioTelefone1;
            String beneficiarioRamal1;
            String beneficiarioDDD2;
            String beneficiarioTelefone2;
            String beneficiarioRamal2;
            String beneficiarioCelDDD;
            String beneficiarioCel;
            String beneficiarioCelOperadora;
            //String beneficiarioTipoLogr1;
            //String beneficiarioLogr1;
            //String beneficiarioNumLogr1;
            //String beneficiarioComplLogr1;
            //String beneficiarioBairro1;
            //String beneficiarioCidade1;
            //String beneficiarioUF1;
            //String beneficiarioCEP1;
            //String beneficiarioTipoEnd1;
            //String beneficiarioTipoLogr2;
            //String beneficiarioLogr2;
            //String beneficiarioNumLogr2;
            //String beneficiarioComplLogr2;
            //String beneficiarioBairro2;
            //String beneficiarioCidade2;
            //String beneficiarioUF2;
            //String beneficiarioCEP2;
            //String beneficiarioTipoEnd2;
            Beneficiario beneficiario = null;
            //Endereco beneficiarioEndereco1 = null;
            //Endereco beneficiarioEndereco2 = null;

            #endregion

            Int32 i = 0;

            PersistenceManager PMTransaction = new PersistenceManager();
            PMTransaction.BeginTransactionContext();

            while (drBeneficiario.HasRows && drBeneficiario.Read())
            {
                beneficiario = new Beneficiario();
                //beneficiarioEndereco1 = new Endereco();
                //beneficiarioEndereco2 = new Endereco();

                beneficiarioNome = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeColumn)).ToString() : null;
                beneficiarioSexo = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(SexoColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(SexoColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(SexoColumn)).ToString() : null;
                beneficiarioCPF = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CPFColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CPFColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CPFColumn)).ToString() : null;
                beneficiarioRG = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(RGColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(RGColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(RGColumn)).ToString() : null;
                beneficiarioDataNascimento = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DataNascimentoColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DataNascimentoColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DataNascimentoColumn)).ToString() : null;

                beneficiario.ImportID = toInt32(drBeneficiario["ID"]);

                try
                {
                    beneficiarioEmail = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(EmailColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(EmailColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(EmailColumn)).ToString() : null;
                }
                catch (Exception) { beneficiarioEmail = String.Empty; }

                try
                {
                    beneficiarioNomeMae = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeMaeColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeMaeColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NomeMaeColumn)).ToString() : null;
                }
                catch (Exception) { beneficiarioNomeMae = String.Empty; }

                beneficiarioDDD1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD1Column)).ToString() : null;
                beneficiarioTelefone1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone1Column)).ToString() : null;
                beneficiarioRamal1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal1Column)).ToString() : null;
                beneficiarioDDD2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(DDD2Column)).ToString() : null;
                beneficiarioTelefone2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Telefone2Column)).ToString() : null;
                beneficiarioRamal2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Ramal2Column)).ToString() : null;
                beneficiarioCelDDD = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelDDDColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelDDDColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelDDDColumn)).ToString() : null;
                beneficiarioCel = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelColumn)).ToString() : null;
                beneficiarioCelOperadora = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelOperadoraColumn)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelOperadoraColumn)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CelOperadoraColumn)).ToString() : null;

                #region comentado 

                //beneficiarioTipoLogr1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr1Column)).ToString() : null;
                //beneficiarioLogr1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr1Column)).ToString() : null;
                //beneficiarioNumLogr1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr1Column)).ToString() : null;
                //beneficiarioComplLogr1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr1Column)).ToString() : null;
                //beneficiarioBairro1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro1Column)).ToString() : null;
                //beneficiarioCidade1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade1Column)).ToString() : null;
                //beneficiarioUF1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF1Column)).ToString() : null;
                //beneficiarioCEP1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP1Column)).ToString() : null;
                //beneficiarioTipoEnd1 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd1Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd1Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd1Column)).ToString() : null;

                //beneficiarioTipoLogr2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoLogr2Column)).ToString() : null;
                //beneficiarioLogr2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Logr2Column)).ToString() : null;
                //beneficiarioNumLogr2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(NumLogr2Column)).ToString() : null;
                //beneficiarioComplLogr2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(ComplLogr2Column)).ToString() : null;
                //beneficiarioBairro2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Bairro2Column)).ToString() : null;
                //beneficiarioCidade2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(Cidade2Column)).ToString() : null;
                //beneficiarioUF2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(UF2Column)).ToString() : null;
                //beneficiarioCEP2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(CEP2Column)).ToString() : null;
                //beneficiarioTipoEnd2 = (drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd2Column)) != null && !(drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd2Column)) is System.DBNull)) ? drBeneficiario.GetValue(drBeneficiario.GetOrdinal(TipoEnd2Column)).ToString() : null;

                #endregion

                //beneficiario.ID = Beneficiario.CarregarPorParametro(beneficiario.ImportID, PMTransaction); Beneficiario.CarregarPorParametro(beneficiarioNome, beneficiarioNomeMae, PMTransaction); // .VerificaExistenciaCPF(beneficiarioCPF, Convert.ToDateTime(beneficiarioDataNascimento), beneficiarioNomeMae, PMTransaction);

                beneficiario.Nome = beneficiarioNome;
                beneficiario.CPF = beneficiarioCPF;
                beneficiario.Sexo = (!String.IsNullOrEmpty(beneficiarioSexo)) ? (beneficiarioSexo.Equals("M")) ? "1" : "2" : null;
                beneficiario.RG = beneficiarioRG;

                beneficiario.DataNascimento = Convert.ToDateTime(beneficiarioDataNascimento);
                if (beneficiario.DataNascimento.Year <= 1753) { beneficiario.DataNascimento = DateTime.MinValue; }

                beneficiario.Email = beneficiarioEmail;
                beneficiario.NomeMae = beneficiarioNomeMae;

                if (!String.IsNullOrEmpty(beneficiarioDDD1) && !String.IsNullOrEmpty(beneficiarioTelefone1))
                {
                    beneficiario.Telefone = String.Concat("(", Convert.ToInt32(beneficiarioDDD1).ToString(), ") ", beneficiarioTelefone1);
                    beneficiario.Ramal = beneficiarioRamal1;
                }

                if (!String.IsNullOrEmpty(beneficiarioDDD2) && !String.IsNullOrEmpty(beneficiarioTelefone2))
                {
                    beneficiario.Telefone2 = String.Concat("(", Convert.ToInt32(beneficiarioDDD2).ToString(), ") ", beneficiarioTelefone2);
                    beneficiario.Ramal2 = beneficiarioRamal2;
                }

                if (!String.IsNullOrEmpty(beneficiarioCelDDD) && !String.IsNullOrEmpty(beneficiarioCel) && !String.IsNullOrEmpty(beneficiarioCelOperadora))
                {
                    beneficiario.Celular = String.Concat("(", Convert.ToInt32(beneficiarioCelDDD).ToString(), ") ", beneficiarioCel);
                    beneficiario.CelularOperadora = beneficiarioCelOperadora;
                }

                try
                {
                    PMTransaction.Save(beneficiario);
                }
                catch (Exception)
                {
                    PMTransaction.Rollback();
                    throw;
                }

                #region comentado 
                //beneficiarioEndereco1.DonoId = beneficiario.ID;
                //beneficiarioEndereco1.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                //beneficiarioEndereco1.Logradouro = String.Concat(beneficiarioTipoLogr1.Replace(":", String.Empty), " ", beneficiarioLogr1);
                //beneficiarioEndereco1.Numero = beneficiarioNumLogr1;
                //beneficiarioEndereco1.Complemento = beneficiarioComplLogr1;
                //beneficiarioEndereco1.Bairro = beneficiarioBairro1;
                //beneficiarioEndereco1.Cidade = beneficiarioCidade1;
                //beneficiarioEndereco1.UF = beneficiarioUF1;
                //beneficiarioEndereco1.CEP = beneficiarioCEP1;
                //beneficiarioEndereco1.Tipo = (!String.IsNullOrEmpty(beneficiarioTipoEnd1)) ? (beneficiarioTipoEnd1.Equals("RESIDENCIA")) ? (int)Endereco.TipoEndereco.Residencial : (int)Endereco.TipoEndereco.Comercial : 0; ;

                //try
                //{
                //    beneficiarioEndereco1.Importar(PMTransaction);
                //}
                //catch (Exception)
                //{
                //    PMTransaction.Rollback();
                //    throw;
                //}

                //if (!String.IsNullOrEmpty(beneficiarioLogr2))
                //{
                //    beneficiarioEndereco2.DonoId = beneficiario.ID;
                //    beneficiarioEndereco2.DonoTipo = (int)Endereco.TipoDono.Beneficiario;
                //    beneficiarioEndereco2.Logradouro = String.Concat(beneficiarioTipoLogr2.Replace(":", String.Empty), " ", beneficiarioLogr2);
                //    beneficiarioEndereco2.Numero = beneficiarioNumLogr2;
                //    beneficiarioEndereco2.Complemento = beneficiarioComplLogr2;
                //    beneficiarioEndereco2.Bairro = beneficiarioBairro2;
                //    beneficiarioEndereco2.Cidade = beneficiarioCidade2;
                //    beneficiarioEndereco2.UF = beneficiarioUF2;
                //    beneficiarioEndereco2.CEP = beneficiarioCEP2;
                //    beneficiarioEndereco2.Tipo = (!String.IsNullOrEmpty(beneficiarioTipoEnd2)) ? (beneficiarioTipoEnd2.Equals("RESIDENCIA")) ? (int)Endereco.TipoEndereco.Residencial : (int)Endereco.TipoEndereco.Comercial : 0; ;

                //    try
                //    {
                //        beneficiarioEndereco2.Importar(PMTransaction);
                //    }
                //    catch (Exception)
                //    {
                //        PMTransaction.Rollback();
                //        throw;
                //    }
                //}
                #endregion

                i++;

                PMTransaction.Commit();
            }

            PMTransaction.Dispose();
            PMTransaction = null;

            drBeneficiario.Close();
            drBeneficiario.Dispose();
            drBeneficiario = null;

            cmdMDB.Dispose();
            cmdMDB = null;

            connectionMDB.Close();
            connectionMDB.Dispose();
            connectionMDB = null;
        }

        public void ImportarEnderecosBeneficiarios()
        {
            DataSet dsEnderecos = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM ENDERECOS", conn);
                adp.Fill(dsEnderecos, "data");
                adp.Dispose();
                OleDbCommand ocmd = conn.CreateCommand();

                PersistenceManager pm = null;
                Object ret = null;

                String qryDonoEnd = "SELECT ID FROM BENEFICIARIOS WHERE ID=";
                foreach (DataRow rowEnd in dsEnderecos.Tables[0].Rows)
                {
                    ocmd.CommandText = qryDonoEnd + toString(rowEnd["idNewSys"]);
                    using (OleDbDataReader odr = ocmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (odr.Read())
                        {
                            pm = new PersistenceManager();
                            pm.BeginTransactionContext();

                            try
                            {
                                //DateTime dtNascimento = toDateTime(odr["NASCIMENTO"]);
                                //if(dtNascimento == DateTime.MinValue)
                                //{
                                //    throw new ApplicationException("Data de nascimento inválida.");
                                //}
                                //ret = Beneficiario.CarregarPorParametro(toString(odr["NOME_BENEFICIARIO"]), toString(odr["NOME_MAE"]), pm, toDateTime(odr["NASCIMENTO"]), toString(odr["CPF"])); //Beneficiario.VerificaExistenciaCPF(toString(odr["CPF"]), dtNascimento, toString(odr["NOME_MAE"]), pm);
                                ret = Beneficiario.CarregarPorParametro(odr["ID"], pm);

                                if (ret != null)
                                {
                                    Endereco end = new Endereco();
                                    end.Bairro = toString(rowEnd["BAIRRO"]);
                                    end.CEP = toString(rowEnd["CEP"]);
                                    end.Cidade = toString(rowEnd["CIDADE"]);
                                    end.Complemento = toString(rowEnd["COMPL_LOGR"]);
                                    end.DonoId = ret;
                                    end.DonoTipo = Convert.ToInt32(Endereco.TipoDono.Beneficiario);
                                    end.Logradouro = toString(rowEnd["TIPO_LOGR"]) + " " + toString(rowEnd["LOGRADOURO"]);
                                    end.Numero = toString(rowEnd["NUM_LOGR"]);
                                    end.Tipo = traduzTipoEndereco(toString(rowEnd["TIPO"]));
                                    end.UF   = toString(rowEnd["UF"]);

                                    #region Seta ID

                                    #region Parameters

                                    String[] Params = new String[10];
                                    String[] Values = new String[10];

                                    Params[0] = "@donoId";
                                    Params[1] = "@donoTipo";
                                    Params[2] = "@logradouro";
                                    Params[3] = "@numero";
                                    Params[4] = "@complemento";
                                    Params[5] = "@bairro";
                                    Params[6] = "@cidade";
                                    Params[7] = "@uf";
                                    Params[8] = "@cep";
                                    Params[9] = "@tipo";

                                    Values[0] = (end.DonoId != null && end.DonoId.ToString().Length > 0) ? end.DonoId.ToString() : String.Empty;
                                    Values[1] = (end.DonoTipo > -1) ? end.DonoTipo.ToString() : "0";
                                    Values[2] = (!String.IsNullOrEmpty(end.Logradouro)) ? end.Logradouro : String.Empty;
                                    Values[3] = (!String.IsNullOrEmpty(end.Numero)) ? end.Numero : String.Empty;
                                    Values[4] = (!String.IsNullOrEmpty(end.Complemento)) ? end.Complemento : String.Empty;
                                    Values[5] = (!String.IsNullOrEmpty(end.Bairro)) ? end.Bairro : String.Empty;
                                    Values[6] = (!String.IsNullOrEmpty(end.Cidade)) ? end.Cidade : String.Empty;
                                    Values[7] = (!String.IsNullOrEmpty(end.UF)) ? end.UF : String.Empty;
                                    Values[8] = (!String.IsNullOrEmpty(end.CEP)) ? end.CEP : String.Empty;
                                    Values[9] = (end.Tipo > -1) ? end.Tipo.ToString() : "0";

                                    #endregion

                                    String strSQL = String.Concat("SELECT ",
                                          "      endereco_id ",
                                          "FROM endereco ",
                                          "WHERE endereco_donoId = @donoId AND endereco_donoTipo = @donoTipo AND endereco_logradouro = @logradouro AND endereco_numero = @numero AND ",
                                          "      endereco_complemento = @complemento AND endereco_bairro = @bairro AND endereco_cidade = @cidade AND ",
                                          "      endereco_uf = @uf AND endereco_cep = @cep AND endereco_tipo = @tipo");

                                    end.ID = LocatorHelper.Instance.ExecuteScalar(strSQL, Params, Values, pm);

                                    //if (end.ID != null)
                                    //{
                                    //    int aret = 0;
                                    //}

                                    #endregion

                                    //if (end.ID == null) { pm.Save(end); }
                                    pm.Save(end);
                                }
                                else
                                {
                                    int j = 0;
                                }

                                pm.Commit();
                            }
                            catch// (Exception ex)
                            {
                                pm.Rollback();
                                throw; //ex;
                            }
                            finally
                            {
                                pm = null;
                            }
                        }

                        odr.Close();
                    }
                }
            }
        }



        public void ImportarPropostas(ref List<ErroSumario> errors)
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT PROPOSTAS.*, ID_BENEFICIARIO, CPF, NASCIMENTO, NOME, NOME_MAE FROM PROPOSTAS, BENEFICIARIOS_PROPOSTA, BENEFICIARIOS WHERE NUM_CONTRATO=NUM_CONTRATO_FK AND TIT_DEP='T' AND CNPJ_OPERADORA=CNPJ_OPERADORA_FK AND ID=ID_BENEFICIARIO AND num_contrato IN ('21009443','21009444', '21009446') ORDER BY NUM_CONTRATO", conn); //AND num_contrato='21009446' //and num_contrato in ('C2081614','21009443','2036528','21009446','21009444','127317','21004348','84404')
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            //String[] arrNm, arrVl;
            PersistenceManager pm = null; String sql = "";
            Plano planoTemp = null; Object ret = null;

            int i = 0;
            //errors = new List<ErroSumario>();
            foreach (DataRow row in dsOrigem.Tables[0].Rows)
            {
                i++;
                if (toString(row["NUM_CONTRATO"]).Trim().Length == 0)
                {
                    ErroSumario erro = new ErroSumario();
                    erro.ContratoAdmNumero = toString(row["CONTRATO_ADM"]);
                    erro.MSG = "Sem número de contrato. Contrato ADM: " + toString(row["CONTRATO_ADM"]);
                    errors.Add(erro);
                    continue;
                }
                //if (toString(row["NUM_CONTRATO"]).Trim() == "0000001") { continue; }
                if (toString(row["CEP_END_COBR"]).Trim().Length == 0)
                {
                    ErroSumario erro = new ErroSumario();
                    erro.ContratoAdmNumero = toString(row["CONTRATO_ADM"]);
                    erro.MSG = "Sem endereço de cobrança. Contrato: " + toString(row["NUM_CONTRATO"]);
                    errors.Add(erro);
                    continue;
                }

                pm = new PersistenceManager();
                pm.BeginTransactionContext();

                try
                {
                    #region 

                    Contrato contrato = new Contrato();
                    contrato.Adimplente = true;             //TODO: ATENÇÃO - rodar rotina que checa adimplência
                    contrato.Admissao = toDateTime(row["DATA_ADM"]);
                    contrato.Cancelado = false;
                    contrato.CobrarTaxaAssociativa = true;  //TODO: checar
                    contrato.CorretorTerceiroCPF   = null;
                    contrato.CorretorTerceiroNome  = null;
                    contrato.Data = toDateTime(row["DATA_DIGT"]);
                    contrato.DataCancelamento = DateTime.MinValue;

                    ret = LocatorHelper.Instance.ExecuteScalar("SELECT usuario_id FROM usuario WHERE usuario_perfilId=" + corretorPerilId + " AND usuario_documento1='" + toString(row["CPF_CNPJ_CORR"]) + "'", null, null, pm); //Usuario.CarregarCorretorPorDoc(toString(row["CPF_CNPJ_CORR"]), pm);
                    if (ret != null)
                    {
                        contrato.DonoID = ret;
                        ret = null;
                    }
                    else
                    {
                        ErroSumario erro = new ErroSumario();
                        erro.ContratoAdmNumero = toString(row["CONTRATO_ADM"]);
                        erro.Codigo = toString(row["PLANO"]);
                        erro.SubPlano = toString(row["NUM_CONTRATO"]);
                        erro.MSG = "Proposta não possui um corretor ou corretor não localizado no cadastro existente. Documento: " + toString(row["CPF_CNPJ_CORR"]);
                        errors.Add(erro);
                        continue;
                        //throw new ApplicationException("Proposta não possui um corretor ou corretor não localizado no cadastro existente. Documento: " + toString(row["CPF_CNPJ_CORR"]));
                    }

                    contrato.EmailCobranca = null;

                    contrato.Numero = toString(row["NUM_CONTRATO"]);
                    contrato.NumeroID = null;
                    contrato.NumeroMatricula = toString(row["MATRICULA_NUM"]);
                    contrato.Obs = null;

                    contrato.OperadoraID = Operadora.CarregarIDPorCNPJ(toString(row["CNPJ_OPERADORA"]).Replace(".", ""), pm);
                    if (contrato.OperadoraID == null)
                    {
                        //pm.Rollback();
                        //continue;
                        throw new ApplicationException("Operadora não localizada. Cnpj: " + toString(row["CNPJ_OPERADORA"]));
                    }

                    contrato.ContratoADMID = ContratoADM.CarregarID(toString(row["CONTRATO_ADM"]), contrato.OperadoraID, pm);
                    if (contrato.ContratoADMID == null)
                    {
                        //pm.Rollback();
                        ErroSumario erro = new ErroSumario();
                        erro.ContratoAdmNumero = toString(row["CONTRATO_ADM"]) + " (nao localizado)";
                        erro.Codigo = toString(row["PLANO"]);
                        //erro.SubPlano = toString(row["NUM_CONTRATO"]);
                        erro.MSG = "Contrato ADM não localizado: " + toString(row["CONTRATO_ADM"]) + " | Proposta: " + toString(row["NUM_CONTRATO"]);
                        errors.Add(erro);
                        continue;

                        //throw new ApplicationException("Contrato ADM não localizado. Número: " + toString(row["CONTRATO_ADM"]));
                    }

                    contrato.ID = Contrato.CarregaContratoID(contrato.OperadoraID, contrato.Numero, pm);

                    if (contrato.ID != null)
                    {
                        pm.Rollback();
                        continue;
                        //throw new ApplicationException("Proposta ja cadastradao: " + toString(row["NUM_CONTRATO"]));
                    }

                    #region ID dos endereços

                    //if (toString(row["CEP_END_COBR"]).Replace("-", "").Trim() == "05025020")
                    //{
                    //    int j = 0;
                    //}

                    sql = String.Concat("SELECT endereco_id ",
                        "   FROM endereco ",
                        "       INNER JOIN beneficiario ON endereco_donoId=beneficiario_id AND endereco_donoTipo=", Convert.ToInt32(Endereco.TipoDono.Beneficiario),
                        "   WHERE ",
                        "       endereco_cep='", toString(row["CEP_END_COBR"]).Replace("-", "").Trim(), "' AND ",
                        "       importId=", Convert.ToInt32(row["ID_BENEFICIARIO"]));

                    contrato.EnderecoCobrancaID = LocatorHelper.Instance.ExecuteScalar(sql, null, null, pm);
                    if (contrato.EnderecoCobrancaID == null)
                    {
                        //pm.Rollback();
                        //continue;
                        throw new ApplicationException("Endereço de cobrança não localizado: " + toString(row["CEP_END_COBR"]));
                    }

                    sql = String.Concat("SELECT endereco_id ",
                        "   FROM endereco ",
                        "       INNER JOIN beneficiario ON endereco_donoId=beneficiario_id AND endereco_donoTipo=", Convert.ToInt32(Endereco.TipoDono.Beneficiario),
                        "   WHERE ",
                        "       endereco_cep='", toString(row["CEP_END_REF"]).Replace("-", "").Trim(), "' AND ",
                        "       importId=", Convert.ToInt32(row["ID_BENEFICIARIO"]));

                    contrato.EnderecoReferenciaID = LocatorHelper.Instance.ExecuteScalar(sql, null, null, pm);
                    if (contrato.EnderecoReferenciaID == null)
                    {
                        //pm.Rollback();
                        //continue;
                        throw new ApplicationException("Endereço de referência não localizado: " + toString(row["CEP_END_REF"]));
                    }

                    #endregion

                    contrato.EstipulanteID = Estipulante.CarregaID(toString(row["ESTIPULANTE"]), pm);
                    if (contrato.EstipulanteID == null)
                    {
                        throw new ApplicationException("Estipulantes não localizado: " + toString(row["ESTIPULANTE"]));
                    }

                    contrato.OperadorTmktID = null;     //TODO
                    contrato.Pendente = false;

                    contrato.PlanoID = Plano.CarregarID(contrato.ContratoADMID, toString(row["PLANO"]), toString(row["SUB_PLANO"]), pm);
                    if (contrato.PlanoID == null)
                    {
                        ErroSumario erro = new ErroSumario();
                        erro.ContratoAdmNumero = toString(row["CONTRATO_ADM"]);
                        erro.Codigo = toString(row["PLANO"]);
                        erro.SubPlano = toString(row["NUM_CONTRATO"]);
                        erro.MSG = "Plano não localizado. Código e subplano: " + toString(row["PLANO"]) + " | " + toString(row["SUB_PLANO"]) + " | PROPOSTA: " + toString(row["NUM_CONTRATO"]) + " | CONTRATO ADM: " + toString(row["CONTRATO_ADM"]);
                        errors.Add(erro);
                        //pm.Rollback();
                        continue;
                        //throw new ApplicationException("Plano não localizado. Código e subplano: " + toString(row["PLANO"]) + " | " + toString(row["SUB_PLANO"]));
                    }

                    contrato.Rascunho = false;
                    contrato.ResponsavelCPF = toString(row["CPF_RESP"]);
                    contrato.ResponsavelDataNascimento = toDateTime(row["DATA_NASC_RESP"]);
                    contrato.ResponsavelNome = toString(row["RESPONSÁVEL"]);
                    contrato.ResponsavelParentescoID = null; //TODO
                    contrato.ResponsavelRG = toString(row["RG_RESP"]);
                    contrato.ResponsavelSexo = null;        //TODO

                    contrato.SuperiorTerceiroCPF = null;
                    contrato.SuperiorTerceiroNome = null;

                    planoTemp = new Plano(contrato.PlanoID);
                    pm.Load(planoTemp);
                    if (planoTemp.Codigo == toString(row["PLANO"]) && planoTemp.SubPlano == toString(row["SUB_PLANO"]))
                        contrato.TipoAcomodacao = Convert.ToInt32(Contrato.eTipoAcomodacao.quartoComun);
                    else
                        contrato.TipoAcomodacao = Convert.ToInt32(Contrato.eTipoAcomodacao.quartoParticular);

                    contrato.TipoContratoID = traduzTipoContrato(row["TIPO_PROPOSTA"]);
                    if (contrato.TipoContratoID == null)
                    {
                        throw new ApplicationException("Tipo de contrato não localizado: " + toString(row["TIPO_PROPOSTA"]));
                    }

                    contrato.Vigencia = toDateTime(row["DATA_VIGENCIA"]);
                    if (contrato.Vigencia == DateTime.MinValue)
                    {
                        throw new ApplicationException("Vigência inválida: " + toString(row["DATA_VIGENCIA"]));
                    }

                    contrato.Vencimento = toDateTime(row["PRIM_VENC"]);
                    if (contrato.Vencimento == DateTime.MinValue)
                    {
                        throw new ApplicationException("Vencimento inválido: " + toString(row["PRIM_VENC"]));
                    }

                    pm.Save(contrato);
                    pm.Commit();

                    #endregion
                }
                catch// (Exception ex)
                {
                    pm.Rollback();
                    throw;// ex;
                }
                finally
                {
                    pm = null;
                }
            }

            dsOrigem.Dispose();
        }

        public void ImportarPropostaBeneficiarios()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT BENEFICIARIOS_PROPOSTA.*, ID, NOME, NOME_MAE, NASCIMENTO, CPF, NOME_MAE, DATA_ADM, DATA_VIGENCIA, VALOR_PRÊMIO, VALORES_ADICIONAIS, CNPJ_OPERADORA, CONTRATO_ADM, CEP_END_COBR FROM BENEFICIARIOS_PROPOSTA, BENEFICIARIOS, PROPOSTAS WHERE ID_BENEFICIARIO=ID AND NUM_CONTRATO=NUM_CONTRATO_FK AND CNPJ_OPERADORA=CNPJ_OPERADORA_FK AND num_contrato IN ('21009443','21009444', '21009446') ORDER BY NUM_CONTRATO_FK", conn); //AND num_contrato IN('11018462', '21017564')
                adp.Fill(dsOrigem, "contrato_beneficiarios");
                adp.Dispose();
            }

            Object operadoraId = null, contratoAdmId = null;
            PersistenceManager pm = null; ContratoADMParentescoAgregado parentesco = null;
            foreach (DataRow row in dsOrigem.Tables[0].Rows)
            {
                if (toString(row["NUM_CONTRATO_FK"]).Trim().Length == 0) { continue; }
                if (toString(row["CEP_END_COBR"]).Trim().Length == 0) { continue; }

                pm = new PersistenceManager();
                pm.BeginTransactionContext();

                ContratoBeneficiario cb = null;

                try
                {
                    cb = new ContratoBeneficiario();
                    cb.Altura = toDecimal(row["ALTURA"]);
                    cb.Ativo = true;

                    cb.BeneficiarioID = Beneficiario.CarregarPorParametro(row["ID"], pm); //Beneficiario.CarregarPorParametro(toString(row["NOME_BENEFICIARIO"]), toString(row["NOME_MAE"]), pm, toDateTime(row["NASCIMENTO"]), toString(row["CPF"]));
                    if (cb.BeneficiarioID == null)
                    {
                        pm.Rollback();
                        continue;
                    }

                    cb.CarenciaCodigo = null; //TODO
                    cb.CarenciaContratoTempo = toInt32(row["TEMPO_CONTRATO"]);
                    cb.CarenciaMatriculaNumero = toString(row["MATRICULA_ANT"]);
                    cb.CarenciaOperadoraID = null; //TODO: !!!
                    cb.CarenciaOperadoraDescricao = toString(row["COMPRA_CAR_OPERADORA"]);

                    operadoraId   = Operadora.CarregarIDPorCNPJ(toString(row["CNPJ_OPERADORA"]), pm);
                    cb.ContratoID = Contrato.CarregaContratoID(operadoraId, toString(row["NUM_CONTRATO_FK"]), pm);

                    if (cb.ContratoID == null)
                    {
                        pm.Rollback();
                        continue;
                    }

                    cb.Data = toDateTime(row["DATA_ADM"]);
                    cb.DataCasamento = DateTime.MinValue;
                    cb.EstadoCivilID = EstadoCivil.CarregarID(toString(row["EST_CIVIL"]), operadoraId, pm);

                    if (cb.EstadoCivilID == null)
                    {
                        EstadoCivil ec = new EstadoCivil();
                        ec.Codigo = toString(row["EST_CIVIL"]);
                        ec.Descricao = toString(row["EST_CIVIL"]);
                        ec.OperadoraID = operadoraId;
                        pm.Save(ec);
                        cb.EstadoCivilID = ec.ID;
                    }

                    cb.NumeroSequencial = toInt32(row["SEQUENCIA"]);

                    if(toString(row["PARENTESCO"]).Trim().ToUpper() != "TITULAR")
                    {
                        contratoAdmId = ContratoADM.CarregarID(toString(row["CONTRATO_ADM"]), pm);
                        parentesco = ContratoADMParentescoAgregado.Carregar(contratoAdmId, toString(row["PARENTESCO"]).Trim().ToUpper(), pm);
                        if (parentesco != null)
                            cb.ParentescoID = parentesco.ID;
                        else
                        {
                            parentesco = new ContratoADMParentescoAgregado();
                            parentesco.ContratoAdmID = contratoAdmId;
                            parentesco.ParentescoDescricao = toString(row["PARENTESCO"]).Trim().ToUpper();
                            parentesco.ParentescoCodigo = parentesco.ParentescoDescricao;
                            parentesco.ParentescoTipo   = Convert.ToInt32(Parentesco.eTipo.Dependente);
                            pm.Save(parentesco);
                            cb.ParentescoID = parentesco.ID;
                        }
                    }
                    else
                        cb.ParentescoID = null;

                    cb.Peso = toDecimal(row["PESO"]);
                    cb.Status = Convert.ToInt32(ContratoBeneficiario.eStatus.Incluido);

                    if (toString(row["TIT_DEP"]).ToUpper().Trim() == "T")
                        cb.Tipo = Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Titular);
                    else
                        cb.Tipo = Convert.ToInt32(ContratoBeneficiario.TipoRelacao.Dependente);

                    cb.Valor    = toDecimal(row["VALOR_PRÊMIO"]) + toDecimal(row["VALORES_ADICIONAIS"]);
                    cb.Vigencia = toDateTime(row["DATA_VIGENCIA"]);

                    //cb.ID = ContratoBeneficiario.CarregaID(cb.ContratoID, cb.BeneficiarioID, pm);

                    pm.Save(cb);
                    pm.Commit();
                }
                catch
                {
                    pm.Rollback();
                    throw;
                }
                finally
                {
                    pm = null;
                }
            }

            dsOrigem.Dispose();
        }



        public void ImportarCobrancas()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT COBRANCAS.*, CEP_END_COBR FROM COBRANCAS, PROPOSTAS WHERE NUM_CONTRATO=NUM_CONTRATO_FK AND CNPJ_OPERADORA=CNPJ_OPERADORA_FK AND (COBRANÇA_PARCELA = 8 OR COBRANÇA_PARCELA = 8) ORDER BY NUM_CONTRATO_FK, COBRANÇA_PARCELA", conn);
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            PersistenceManager pm = null;
            Object propostaId = null, operadoraId = null;
            Cobranca cob = null;
            List<Cobranca> cobrancas = new List<Cobranca>();

            pm = new PersistenceManager();
            pm.BeginTransactionContext();

            foreach (DataRow row in dsOrigem.Tables[0].Rows)
            {
                if (toString(row["NUM_CONTRATO_FK"]).Trim().Length == 0) { continue; }
                //if (toString(row["NUM_CONTRATO"]).Trim() == "0000001") { continue; }
                if (toString(row["CEP_END_COBR"]).Trim().Length == 0) { continue; }

                try
                {
                    operadoraId = Operadora.CarregarIDPorCNPJ(toString(row["CNPJ_OPERADORA_FK"]), pm);
                    if (operadoraId == null)
                    {
                        throw new ApplicationException("Operadora não localizada: " + toString(row["CNPJ_OPERADORA_FK"]));
                    }

                    propostaId = Contrato.CarregaContratoID(operadoraId, toString(row["NUM_CONTRATO_FK"]), pm);
                    if (propostaId == null)
                    {
                        throw new ApplicationException("Proposta não localizada: Núm.:" + toString(row["NUM_CONTRATO_FK"]) + " OperadoraID: " + operadoraId.ToString());
                    }

                    cob = new Cobranca();
                    cob.ArquivoIDUltimoEnvio    = null;
                    cob.Cancelada               = Convert.ToBoolean(row["Cobrança_cancelada"]);
                    cob.CobrancaRefID           = null;
                    cob.ComissaoPaga            = Convert.ToBoolean(row["Cobrança_comissaoPaga"]);
                    cob.DataCriacao             = DateTime.Now;
                    cob.DataPgto                = toDateTime(row["Cobrança_dataPagto"]);
                    cob.DataVencimento          = toDateTime(row["Cobrança_dataVencimento"]);
                    cob.Pago                    = Convert.ToBoolean(row["Cobrança_pago"]);
                    cob.Parcela                 = toInt32(row["Cobrança_parcela"]);
                    cob.PropostaID              = propostaId;
                    cob.Tipo                    = toInt32(row["Cobrança_tipo"]);
                    cob.Valor                   = toDecimal(row["Cobrança_valor"]);
                    cob.ValorPgto               = toDecimal(row["Cobrança_valorPagto"]);

                    if (cob.DataPgto.Year <= 1753)
                        cob.DataPgto = DateTime.MinValue;

                    if (cob.DataVencimento.Year <= 1753)
                        cob.DataVencimento = DateTime.MinValue;

                    cobrancas.Add(cob);

                    //pm.Save(cob);
                    //pm.Commit();
                }
                catch
                {
                    pm.Rollback();
                    throw;
                }
            }

            pm.Commit();
            pm = null;

            dsOrigem.Dispose();

            if (cobrancas.Count > 0)
            {
                pm = new PersistenceManager();
                pm.BeginTransactionContext();

                try
                {
                    foreach (Cobranca cobranca in cobrancas)
                    {
                        pm.Save(cobranca);
                    }

                    pm.Commit();
                }
                catch
                {
                    pm.Rollback();
                    throw;
                }
                finally
                {
                    pm = null;
                }
            }
        }

        public String ImportarCobrancas_V2(int parcela)
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                //OleDbDataAdapter adp = new OleDbDataAdapter("SELECT COBRANCAS.*, CEP_END_COBR FROM COBRANCAS, PROPOSTAS WHERE NUM_CONTRATO=NUM_CONTRATO_FK AND CNPJ_OPERADORA=CNPJ_OPERADORA_FK AND (COBRANÇA_PARCELA = " + parcela.ToString() + ") ORDER BY NUM_CONTRATO_FK, COBRANÇA_PARCELA", conn);
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM COBRANCAS WHERE COBRANÇA_PARCELA = " + parcela.ToString() + " ORDER BY NUM_CONTRATO_FK, COBRANÇA_PARCELA", conn); //and num_contrato_fk IN ('21009443','21009444', '21009446') 
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            Object propostaId = null, operadoraId = null;

            StringBuilder sb = new StringBuilder();
            DateTime data = DateTime.MinValue;

            #region 

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                int i = 0, j = 0;
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    if (toString(row["NUM_CONTRATO_FK"]).Trim().Length == 0) { continue; }
                    ////if (toString(row["NUM_CONTRATO"]).Trim() == "0000001") { continue; }
                    //if (toString(row["CEP_END_COBR"]).Trim().Length == 0) { continue; }

                    try
                    {
                        cmd.CommandText = "SELECT operadora_id FROM operadora WHERE operadora_cnpj='" + toString(row["CNPJ_OPERADORA_FK"]).Replace(".", "").Replace("/","").Replace("-","") + "'";
                        operadoraId = cmd.ExecuteScalar();
                        //operadoraId = Operadora.CarregarIDPorCNPJ(toString(row["CNPJ_OPERADORA_FK"]), pm);
                        if (operadoraId == null)
                        {
                            //throw new ApplicationException("Operadora não localizada: " + toString(row["CNPJ_OPERADORA_FK"]));
                            continue;
                        }

                        cmd.CommandText = "SELECT contrato_id FROM contrato WHERE contrato_operadoraId=" + operadoraId + " AND contrato_numero='" + toString(row["NUM_CONTRATO_FK"]) + "'";
                        propostaId = cmd.ExecuteScalar();
                        //propostaId = Contrato.CarregaContratoID(operadoraId, toString(row["NUM_CONTRATO_FK"]), pm);
                        if (propostaId == null)
                        {
                            //throw new ApplicationException("Proposta não localizada: Núm.:" + toString(row["NUM_CONTRATO_FK"]) + " OperadoraID: " + operadoraId.ToString());
                            continue;
                        }

                        sb.Append("IF NOT EXISTS(SELECT cobranca_id FROM cobranca WHERE cobranca_propostaId=");
                        sb.Append(propostaId);
                        sb.Append(" AND cobranca_parcela="); sb.Append(row["Cobrança_parcela"]);
                        sb.Append(") BEGIN ");

                        sb.Append("INSERT INTO cobranca (cobranca_nossoNumero, cobranca_propostaId, cobranca_cancelada,cobranca_comissaoPaga,cobranca_dataCriacao,cobranca_dataPagto, cobranca_dataVencimento,cobranca_pago,cobranca_parcela,cobranca_tipo,cobranca_valor,cobranca_valorPagto) VALUES (");
                        sb.Append("'");
                        sb.Append(row["Nosso_Numero"]);
                        sb.Append("',");
                        sb.Append(propostaId);
                        sb.Append(",");
                        sb.Append(Convert.ToInt32(Convert.ToBoolean(row["Cobrança_cancelada"])));
                        sb.Append(",");
                        sb.Append(Convert.ToInt32(Convert.ToBoolean(row["Cobrança_comissaoPaga"])));
                        sb.Append(",'");
                        sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        #region data pagamento
                        data = toDateTime(row["Cobrança_dataPagto"]);
                        if (data != DateTime.MinValue && data.Year > 1753)
                        {
                            sb.Append("','");
                            sb.Append(data.ToString("yyyy-MM-dd"));
                            sb.Append("',");
                        }
                        else
                        {
                            sb.Append("',NULL,");
                        }
                        #endregion

                        #region data vencimento
                        data = toDateTime(row["Cobrança_dataVencimento"]);
                        if (data != DateTime.MinValue && data.Year > 1753)
                        {
                            sb.Append("'");
                            sb.Append(data.ToString("yyyy-MM-dd 23:59:59"));
                            sb.Append("',");
                        }
                        else
                        {
                            sb.Append("NULL,");
                        }
                        #endregion

                        sb.Append(Convert.ToInt32(Convert.ToBoolean(row["Cobrança_pago"])));
                        sb.Append(",");
                        sb.Append(toInt32(row["Cobrança_parcela"]));
                        sb.Append(",");
                        sb.Append(toInt32(row["Cobrança_tipo"]));
                        sb.Append(",'");
                        sb.Append(toDecimal(row["Cobrança_valor"]).ToString("N2").Replace(".", "").Replace(",", "."));
                        sb.Append("','");
                        sb.Append(toDecimal(row["Cobrança_valorPagto"]).ToString("N2").Replace(".", "").Replace(",", "."));
                        sb.Append("') ");
                        sb.Append(" END  ");

                        #region update

                        //sb.Append(" ELSE BEGIN ");

                        //sb.Append("UPDATE cobranca SET cobranca_pago=");
                        //sb.Append(Convert.ToInt32(Convert.ToBoolean(row["Cobrança_pago"])));
                        //sb.Append(", cobranca_valor='");
                        //sb.Append(toDecimal(row["Cobrança_valor"]).ToString("N2").Replace(".", "").Replace(",", "."));
                        //sb.Append("', cobranca_valorPagto='");
                        //sb.Append(toDecimal(row["Cobrança_valorPagto"]).ToString("N2").Replace(".", "").Replace(",", "."));
                        //sb.Append("', cobranca_dataPagto=");

                        //#region data pagamento
                        //data = toDateTime(row["Cobrança_dataPagto"]);
                        //if (data != DateTime.MinValue && data.Year > 1753)
                        //{
                        //    sb.Append("'");
                        //    sb.Append(data.ToString("yyyy-MM-dd"));
                        //    sb.Append("'");
                        //}
                        //else
                        //{
                        //    sb.Append("NULL");
                        //}
                        //#endregion

                        //sb.Append(", cobranca_comissaoPaga=");
                        //sb.Append(Convert.ToInt32(Convert.ToBoolean(row["Cobrança_comissaoPaga"])));

                        //sb.Append(" WHERE cobranca_propostaId="); sb.Append(propostaId);
                        //sb.Append(" AND cobranca_parcela="); sb.Append(row["Cobrança_parcela"]);

                        //sb.Append(" END ");
                        #endregion
                        sb.Append(Environment.NewLine);

                        i++; j++;

                        if (j == 250)
                        {
                            cmd.CommandText = sb.ToString();
                            cmd.ExecuteNonQuery();
                            sb.Remove(0, sb.Length);
                            j = 0;
                        }
                        
                    }
                    catch
                    {
                        throw;
                    }

                    //break;///////////////////////////////
                }

                if (sb.Length > 0)
                {
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                    sb.Remove(0, sb.Length);
                }
            }
            #endregion

            dsOrigem.Dispose();

            return sb.ToString();
        }

        public void GerarCobrancasComoENVIADAS()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM boletos", conn);
                adp.Fill(dsOrigem, "boletos");
                adp.Dispose();
                conn.Close();
            }

            PersistenceManager pm = null;

            Contrato contrato = null;
            Cobranca cobranca = null, cobrancaTemp = null; 
            List<CobrancaComposite> composite = null;
            foreach (DataRow row in dsOrigem.Tables[0].Rows)
            {
                pm = new PersistenceManager();
                pm.BeginTransactionContext();

                contrato = Contrato.CarregarParcialPorCodCobranca(row["ID_COBRANCA"], pm);

                if (contrato == null || contrato.ID == null) { pm.Rollback(); pm.Dispose(); continue; }

                cobranca = Cobranca.CarregarPor(contrato.ID, Convert.ToInt32(row["PARCELA"]), Convert.ToInt32(Cobranca.eTipo.Normal), pm);

                if (cobranca != null && cobranca.ID != null)
                {
                    cobranca.ArquivoIDUltimoEnvio = -1;
                    pm.Save(cobranca);
                }
                else
                {
                    cobrancaTemp = Cobranca.CarregarPor(contrato.ID, (Convert.ToInt32(row["PARCELA"])-1), Convert.ToInt32(Cobranca.eTipo.Normal), pm);
                    if (cobrancaTemp == null || cobrancaTemp.ID == null) { pm.Rollback(); pm.Dispose(); continue; }

                    cobranca = new Cobranca();
                    cobranca.ArquivoIDUltimoEnvio = -2;
                    cobranca.Cancelada = false;
                    cobranca.CobrancaRefID = null;
                    cobranca.ComissaoPaga = true;
                    cobranca.DataCriacao = DateTime.Now;
                    cobranca.DataVencimento = cobrancaTemp.DataVencimento.AddMonths(1);
                    cobranca.DataVencimentoISENCAOJURO = cobrancaTemp.DataVencimentoISENCAOJURO;
                    cobranca.Pago = false;
                    cobranca.Parcela = Convert.ToInt32(row["PARCELA"]);
                    cobranca.PropostaID = contrato.ID;
                    cobranca.Tipo = (int)Cobranca.eTipo.Normal;
                    cobranca.Valor = Contrato.CalculaValorDaProposta(contrato.ID, cobranca.DataVencimento, pm, false, true, ref composite);
                    pm.Save(cobranca);
                }

                pm.Commit();
            }
        }

        public void ImportarFiliaisParaProdutores()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM FILIAL", conn);
                adp.Fill(dsOrigem, "resultset");
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                IList<Usuario> produtores = null;
                Object filialId = null;
                DateTime data = DateTime.MinValue;
                UsuarioFilial uf = null;
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    produtores = Usuario.CarregarCorretorPorDoc(toString(row["CORRETOR_DOC"]), pm);
                    if (produtores == null)
                    {
                        throw new ApplicationException("Corretor não encontrado: " + toString(row["CORRETOR_DOC"]));
                    }
                    if (produtores.Count > 1)
                    {
                        throw new ApplicationException("Mais de um corretor encontrado: " + toString(row["CORRETOR_DOC"]));
                    }

                    filialId = LocatorHelper.Instance.ExecuteScalar("SELECT filial_id FROM filial WHERE filial_nome='" + toString(row["FILIAL"]).Trim() + "'", null, null, pm);
                    if (filialId == null || filialId == DBNull.Value)
                    {
                        throw new ApplicationException("Filial não encontrada: " + toString(row["FILIAL"]));
                    }

                    data = toDateTime(row["DATA"]);
                    if (data == DateTime.MinValue)
                    {
                        throw new ApplicationException("Data inválida: " + toString(row["DATA"]));
                    }

                    uf = new UsuarioFilial();
                    uf.Data = data;
                    uf.FilialID = filialId;
                    uf.UsuarioID = produtores[0].ID;
                    pm.Save(uf);
                }

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                dsOrigem.Dispose();
                pm.Dispose();
                pm = null;
            }
        }

        public void ImportarGruposDeVendaParaProdutores()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM GRUPO_DE_VENDA", conn);
                adp.Fill(dsOrigem, "resultset");
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                IList<Usuario> produtores = null;
                Object grupoId = null;
                DateTime data = DateTime.MinValue;
                UsuarioGrupoVenda ugv = null;
                DataTable dt = dsOrigem.Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    produtores = Usuario.CarregarCorretorPorDoc(toString(row["CORRETOR_DOC"]), pm);
                    if (produtores == null)
                    {
                        throw new ApplicationException("Corretor não encontrado: " + toString(row["CORRETOR_DOC"]));
                    }
                    if (produtores.Count > 1)
                    {
                        throw new ApplicationException("Mais de um corretor encontrado: " + toString(row["CORRETOR_DOC"]));
                    }

                    grupoId = LocatorHelper.Instance.ExecuteScalar("SELECT grupovenda_id FROM grupo_venda WHERE grupovenda_descricao='" + toString(row["GRUPO"]).Trim() + "'", null, null, pm);
                    if (grupoId == null || grupoId == DBNull.Value)
                    {
                        throw new ApplicationException("Grupo de Venda não encontrado: " + toString(row["GRUPO"]));
                    }

                    data = toDateTime(row["DATA_VIG"]);
                    if (data == DateTime.MinValue)
                    {
                        throw new ApplicationException("Data inválida: " + toString(row["DATA_VIG"]));
                    }

                    ugv = new UsuarioGrupoVenda();
                    ugv.Data = data;
                    ugv.GrupoVendaID = grupoId;
                    ugv.UsuarioID = produtores[0].ID;
                    pm.Save(ugv);
                }

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                dsOrigem.Dispose();
                pm.Dispose();
                pm = null;
            }
        }

        public void ImportarConfiguracoesDeEquipes()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM SUBORDINACAO WHERE corretor_doc <> '03235631801'", conn);
                adp.Fill(dsOrigem, "resultset");
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                IList<Usuario> subordinados = null; IList<Usuario> superiores = null;
                //Object perfilId = null;
                DateTime data = DateTime.MinValue;
                SuperiorSubordinado ss = null;
                DataTable dt = dsOrigem.Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    if (toString(row["CORRETOR_DOC"]).Trim() == "03235631801") //manobra
                    {
                        subordinados = new List<Usuario>();
                        Usuario us = new Usuario();
                        us.ID = 603;
                        subordinados.Add(us);
                    }
                    else
                    {
                        subordinados = Usuario.CarregarCorretorPorDoc(toString(row["CORRETOR_DOC"]).Trim(), pm);
                    }

                    if (subordinados == null)
                    {
                        throw new ApplicationException("Subordinado não encontrado: " + toString(row["CORRETOR_DOC"]));
                    }
                    //if (subordinados.Count > 1)
                    //{
                    //    throw new ApplicationException("Mais de um subordinado encontrado: " + toString(row["CORRETOR_DOC"]));
                    //}

                    if (toString(row["CORRETOR_DOC"]).Trim() == "03235631801") //manobra
                    {
                        superiores = new List<Usuario>();
                        Usuario us = new Usuario();
                        us.ID = 15;
                        superiores.Add(us);
                    }
                    else
                    {
                        superiores = Usuario.CarregarCorretorPorDoc(toString(row["SUPERIOR_DOC"]).Trim(), pm);
                    }
                    if (superiores == null)
                    {
                        throw new ApplicationException("Superior não encontrado: " + toString(row["SUPERIOR_DOC"]));
                    }
                    //if (superiores.Count > 1)
                    //{
                    //    throw new ApplicationException("Mais de um superior encontrado: " + toString(row["SUPERIOR_DOC"]));
                    //}

                    data = toDateTime(row["DATA_VIG"]);
                    if (data == DateTime.MinValue)
                    {
                        throw new ApplicationException("Data inválida: " + toString(row["DATA_VIG"]));
                    }

                    ss = new SuperiorSubordinado();
                    ss.Data = data;
                    //ss.SuperiorPerfilID = perfilId;
                    ss.SubordinadoID = subordinados[0].ID;
                    ss.SuperiorID    = superiores[0].ID;
                    pm.Save(ss);
                }

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                dsOrigem.Dispose();
                pm.Dispose();
                pm = null;
            }
        }
        public void ImportarConfiguracoesDeEquipes_Acerto()
        {
            DataTable dtOrigem = LocatorHelper.Instance.ExecuteQuery("SELECT * FROM superior_subordinado", "resultset").Tables[0];
            StringBuilder sb = new StringBuilder();

            foreach (DataRow rowOrigem in dtOrigem.Rows)
            {
                sb.Append("UPDATE usuario SET usuario_superiorId=");
                sb.Append(rowOrigem["ss_superiorId"]);
                sb.Append(" WHERE usuario_id=");
                sb.Append(rowOrigem["ss_subordinadoId"]);
                sb.Append("; "); 
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);
                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
                dtOrigem.Dispose();
            }
        }

        public void ImportarTabelasDeValor()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM TABELA ORDER BY CONTRATO,COD_PLANO,COD_SUBPLANO,ACOMODACAO,IDADE_INI", conn);
                adp.Fill(dsOrigem, "resultset");
                adp.Dispose();
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                DataTable dt = dsOrigem.Tables[0];
                Object contratoAdmId  = null, planoId = null, tabelaId = null;
                DataTable dtLinhasItem = null;
                DataRow[] rows = null;
                String sql = "", qcValor = "", qpValor = "";
                TabelaValor tabela = null;
                foreach (DataRow row in dt.Rows)
                {
                    contratoAdmId = ContratoADM.CarregarID(toString(row["CONTRATO"]), pm);
                    if (contratoAdmId == null)
                    {
                        continue;
                    }

                    planoId = Plano.CarregarID(contratoAdmId, toString(row["COD_PLANO"]), toString(row["COD_SUBPLANO"]), pm);
                    if (planoId == null)
                    {
                        continue;
                    }

                    //checa se a linha do item da tabela de valor ja existe.
                    sql = String.Concat("SELECT tabelavalor_id, tabela_valor_item.* ",
                        " FROM tabela_valor ",
                        "   LEFT JOIN tabela_valor_item ON tabelavalor_id=tabelavaloritem_tabelaid ",
                        " WHERE tabelavalor_contratoId=", contratoAdmId, " AND tabelavaloritem_planoId=", planoId);

                    dtLinhasItem = LocatorHelper.Instance.ExecuteQuery(sql, "resultset", pm).Tables[0];

                    if (dtLinhasItem != null && dtLinhasItem.Rows != null && dtLinhasItem.Rows.Count > 0)
                    {
                        //achou. agora, checa se existe a linha para o intervalo de idade 
                        rows = dtLinhasItem.Select("tabelavaloritem_idadeInicio=" + toString(row["IDADE_INI"]) + " AND tabelavaloritem_idadeFim=" + toString(row["IDADE_FIN"]));

                        if (rows != null && rows.Length > 0)
                        {
                            //achou a linha. atualiza.
                            sql = String.Concat("UPDATE tabela_valor_item SET ",
                                traduzTipoAcomodacaoParaColuna(row["ACOMODACAO"]), 
                                "='",
                                toDecimal(row["VALOR_TAB1"]).ToString("N2").Replace(".", "").Replace(",", "."), 
                                "' WHERE tabelavaloritem_id=", 
                                toString(rows[0]["tabelavaloritem_id"]));

                            NonQueryHelper.Instance.ExecuteNonQuery(sql, pm);
                        }
                        else
                        {
                            //nao achou a linha, insere.
                            qcValor = "0.00"; qpValor = "0.00";
                            if (toString(row["ACOMODACAO"]) == "QP")
                                qpValor = toDecimal(row["VALOR_TAB1"]).ToString("N2").Replace(".", "").Replace(",", ".");
                            else
                                qcValor = toDecimal(row["VALOR_TAB1"]).ToString("N2").Replace(".", "").Replace(",", ".");

                            sql = String.Concat("INSERT INTO tabela_valor_item (tabelavaloritem_tabelaid, tabelavaloritem_planoId, tabelavaloritem_idadeInicio, tabelavaloritem_idadeFim, tabelavaloritem_qComum, tabelavaloritem_qParticular) VALUES (",
                                dtLinhasItem.Rows[0]["tabelavalor_id"], ",",
                                planoId, ",",
                                row["IDADE_INI"], ",",
                                row["IDADE_FIN"], ",",
                                "'", qcValor, "',",
                                "'", qpValor, "')");

                            NonQueryHelper.Instance.ExecuteNonQuery(sql, pm);
                        }
                    }
                    else
                    {
                        //nao achou.checa se a tabela existe.
                        //se nao existe, insere a tabela. em seguida, insere o item da tabela
                        tabelaId = LocatorHelper.Instance.ExecuteScalar("SELECT tabelavalor_id FROM tabela_valor WHERE tabelavalor_contratoId=" + contratoAdmId, null, null, pm);
                        tabela = new TabelaValor();

                        if (tabelaId == null)
                        {
                            tabela.ContratoID = contratoAdmId;
                            //tabela.Data = agora;
                            tabela.Inicio = new DateTime(2010, 01, 01, 0, 0, 0, 0);
                            tabela.Fim = tabela.Inicio.AddYears(2);
                            pm.Save(tabela);
                        }
                        else
                        {
                            tabela.ID = tabelaId;
                        }

                        //nao achou a linha, insere.
                        qcValor = "0.00"; qpValor = "0.00";
                        if (toString(row["ACOMODACAO"]) == "QP")
                            qpValor = toDecimal(row["VALOR_TAB1"]).ToString("N2").Replace(".", "").Replace(",", ".");
                        else
                            qcValor = toDecimal(row["VALOR_TAB1"]).ToString("N2").Replace(".", "").Replace(",", ".");

                        sql = String.Concat("INSERT INTO tabela_valor_item (tabelavaloritem_tabelaid, tabelavaloritem_planoId, tabelavaloritem_idadeInicio, tabelavaloritem_idadeFim, tabelavaloritem_qComum, tabelavaloritem_qParticular) VALUES (",
                            tabela.ID, ",",
                            planoId, ",",
                            row["IDADE_INI"], ",",
                            row["IDADE_FIN"], ",",
                            "'", qcValor, "',",
                            "'", qpValor, "')");

                        NonQueryHelper.Instance.ExecuteNonQuery(sql, pm);
                    }
                }

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                dsOrigem.Dispose();
            }
        }

        public void ImportarAtendimentos()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM ocorrencias where id > 108000", conn);
                adp.Fill(dsOrigem, "ocorrencias");
                adp.Dispose();
            }

            Object propostaId = null;

            StringBuilder sb = new StringBuilder();
            DateTime data = DateTime.MinValue;

            #region

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                int i = 0, j = 0;
                String titulo = "";
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    try
                    {
                        cmd.CommandText = "SELECT contrato_id FROM contrato WHERE contrato_codCobranca=" + toString(row["ID_COBRANCA"]);
                        propostaId = cmd.ExecuteScalar();
                        if (propostaId == null)
                        {
                            continue;
                        }

                        if (toString(row["DESCRIC_OCORR"]).Length > 30)
                            titulo = toString(row["DESCRIC_OCORR"]).Substring(0, 29) + "(...)";
                        else
                            titulo = toString(row["DESCRIC_OCORR"]);

                        titulo = titulo.Replace("'", "´");

                        sb.Append("INSERT INTO _atendimento (atendimento_propostaId, atendimento_titulo,atendimento_texto,atendimento_dataInicio,atendimento_dataPrevisao, atendimento_dataTermino,atendimento_data,atendimento_cadastrado,atendimento_resolvido) VALUES (");
                        sb.Append(propostaId);
                        sb.Append(",'");
                        sb.Append(titulo);
                        sb.Append("','");
                        sb.Append(toString(row["DESCRIC_OCORR"]).Replace("'", "´"));

                        #region data inicio
                        data = toDateTime(row["DATA_OCORR"]);
                        if (data != DateTime.MinValue && data.Year > 1753)
                        {
                            sb.Append("','");
                            sb.Append(data.ToString("yyyy-MM-dd"));
                            sb.Append("',");
                        }
                        else
                        {
                            sb.Append("',NULL,");
                        }
                        #endregion

                        #region data prevista
                        data = toDateTime(row["DATA_PREVISTA"]);
                        if (data != DateTime.MinValue && data.Year > 1753)
                        {
                            sb.Append("'");
                            sb.Append(data.ToString("yyyy-MM-dd 23:59:59"));
                            sb.Append("',");
                        }
                        else
                        {
                            sb.Append("NULL,");
                        }
                        #endregion

                        #region data termino
                        data = toDateTime(row["DATA_CONCLUSAO"]);
                        if (data != DateTime.MinValue && data.Year > 1753)
                        {
                            sb.Append("'");
                            sb.Append(data.ToString("yyyy-MM-dd 23:59:59"));
                            sb.Append("',");
                        }
                        else
                        {
                            sb.Append("NULL,");
                        }
                        #endregion

                        #region data
                        data = toDateTime(row["DATA_OCORR"]);
                        if (data != DateTime.MinValue && data.Year > 1753)
                        {
                            sb.Append("'");
                            sb.Append(data.ToString("yyyy-MM-dd 23:59:59"));
                            sb.Append("',");
                        }
                        else
                        {
                            sb.Append("NULL,");
                        }
                        #endregion

                        sb.Append("'");
                        sb.Append(toString(row["CADASTRADO_POR"]).Replace("'", "´"));
                        sb.Append("','");
                        sb.Append(toString(row["RESOLVIDO_POR"]).Replace("'", "´"));
                        sb.Append("');");
                        sb.Append(Environment.NewLine);

                        i++; j++;

                        if (j == 500)
                        {
                            cmd.CommandText = sb.ToString();
                            cmd.ExecuteNonQuery();
                            sb.Remove(0, sb.Length);
                            j = 0;
                        }

                    }
                    catch
                    {
                        throw;
                    }
                }

                if (sb.Length > 0)
                {
                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                    sb.Remove(0, sb.Length);
                }
            }
            #endregion

            dsOrigem.Dispose();

            #region
            /*
            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            try
            {
                AtendimentoTemp atendimento = null; Contrato contrato = null;
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    contrato = Contrato.CarregarParcialPorCodCobranca(row["ID_COBRANCA"], pm);
                    if (contrato == null) { continue; }

                    atendimento = new AtendimentoTemp();

                    atendimento.Data       = toDateTime(row["DATA_OCORR"]);
                    atendimento.DataInicio = toDateTime(row["DATA_OCORR"]);
                    atendimento.DataFim    = toDateTime(row["DATA_CONCLUSAO"]);
                    atendimento.PropostaID = contrato.ID;
                    atendimento.Texto      = toString(row["DESCRIC_OCORR"]);

                    pm.Save(atendimento);
                }

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }
            */
            #endregion
        }


        //--------------------------------------------------------------------------------//

        //public String ArrumaCODsDeCobrancaParaProposta()
        //{
        //    DataSet dsOrigem = new DataSet();

        //    //using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
        //    //{
        //    //    conn.Open();
        //    //    OleDbDataAdapter adp = new OleDbDataAdapter("SELECT NUM_CONTRATO, CNPJ_OPERADORA, ID_PROPOSTA FROM PROPOSTAS", conn); //AND num_contrato='S0409' 
        //    //    adp.Fill(dsOrigem, "propostas");
        //    //    adp.Dispose();
        //    //}

        //    using (SqlConnection conn = new SqlConnection(sqlConn))
        //    {
        //        conn.Open();
        //        SqlDataAdapter adp = new SqlDataAdapter("select * from contrato where contrato_codcobranca is null and ", conn); //AND num_contrato='S0409' 
        //        adp.Fill(dsOrigem, "propostas");
        //        adp.Dispose();
        //    }

        //    //PersistenceManager pm = new PersistenceManager();
        //    //pm.BeginTransactionContext();

        //    using (SqlConnection conn = new SqlConnection(sqlConn))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = conn.CreateCommand();
        //        cmd.Transaction = conn.BeginTransaction();

        //        try
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            int i = 0; Object operadoraId = null; Object propostaId = null;

        //            foreach (DataRow row in dsOrigem.Tables[0].Rows)
        //            {
        //                cmd.CommandText = String.Concat("SELECT operadora_id FROM operadora WHERE operadora_cnpj='", toString(row["CNPJ_OPERADORA"]), "'");
        //                operadoraId = cmd.ExecuteScalar();
        //                if (operadoraId == null) { continue; }

        //                cmd.CommandText = String.Concat("SELECT contrato_id FROM contrato WHERE contrato_numero='", toString(row["NUM_CONTRATO"]), "' AND contrato_operadoraId=", operadoraId);
        //                propostaId = cmd.ExecuteScalar();
        //                if (propostaId == null) { continue; }

        //                sb.Append("\nUPDATE contrato SET contrato_codcobranca=");
        //                sb.Append(toString(row["ID_PROPOSTA"]));
        //                sb.Append(" WHERE contrato_id=");
        //                sb.Append(propostaId);

        //                i++;
        //            }

        //            cmd.CommandText = sb.ToString();
        //            cmd.ExecuteNonQuery();
        //            cmd.Transaction.Commit();
        //            return sb.ToString();
        //        }
        //        catch
        //        {
        //            cmd.Transaction.Rollback();
        //            throw;
        //        }
        //        finally
        //        {
        //        }
        //    }
        //}

        public String ArrumaCODsDeCobrancaParaProposta()
        {
            DataSet dsOrigem = new DataSet();

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter("select contrato_id from contrato where contrato_codcobranca is null or contrato_codcobranca=0", conn); //AND num_contrato='S0409' 
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd  = conn.CreateCommand();
                cmd.Transaction = conn.BeginTransaction();
                cmd.CommandText = "select max(contrato_codcobranca) from contrato";

                int i = Convert.ToInt32(cmd.ExecuteScalar());

                try
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (DataRow row in dsOrigem.Tables[0].Rows)
                    {
                        sb.Append("\nUPDATE contrato SET contrato_codcobranca=");
                        sb.Append(i);
                        sb.Append(" WHERE contrato_id=");
                        sb.Append(row["contrato_id"]);

                        i++;
                    }

                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    return sb.ToString();
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
                finally
                {
                }
            }
        }

        public void ArrumaDocumentos()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM DEPARA", conn); //AND num_contrato='S0409' 
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            String qry = "SELECT usuario_id FROM usuario where usuario_documento1='";

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            Object ret = null;

            StringBuilder sb = new StringBuilder();

            try
            {
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    if (toString(row["de"]) == toString(row["para"])) { continue; }
                    if (sb.Length > 0) { sb.Append(" ; "); }

                    ret = LocatorHelper.Instance.ExecuteScalar(qry + toString(row["para"]) + "'", null, null, pm);

                    if (ret != null)
                    {
                        //o cpf PARA já existe no banco
                        //pm.Rollback();
                        //return;
                        continue;
                    }

                    sb.Append("UPDATE usuario SET usuario_documento1='");
                    sb.Append(toString(row["para"]));
                    sb.Append("' WHERE usuario_documento1='");
                    sb.Append(toString(row["de"]));
                    sb.Append("'");
                }

                //NonQueryHelper.Instance.ExecuteNonQuery(sb.ToString(), pm);

                pm.Commit();
            }
            catch
            {
                pm.Rollback();
                throw;
            }
            finally
            {
                pm.Dispose();
                pm = null;
            }
        }

        public void ArrumaStatusDePropostas2(ref List<ErroSumario> erros)
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT num_contrato,cnpj_operadora,status,DATA_CANCEL,obs FROM PROPOSTAS WHERE num_contrato IN ('2066669','2073536','26756','7030563','E702173','E706977','E712459') ORDER BY NUM_CONTRATO", conn); //AND num_contrato='S0409' 
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
                conn.Close();
            }

            Object operadoraId = null;
            erros = new List<ErroSumario>();

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                int i = 0;
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    try
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = "SELECT operadora_id FROM operadora WHERE operadora_cnpj='" + toString(row["cnpj_operadora"]) + "'";
                        operadoraId = cmd.ExecuteScalar();

                        if (operadoraId == null)
                        {
                            ErroSumario erro = new ErroSumario();
                            erro.MSG = "Operadora nao localizada: " + toString(row["cnpj_operadora"]);
                            erros.Add(erro);
                            continue;
                        }

                        if (toString(row["status"]) == "1")
                        {
                            cmd.CommandText = "UPDATE contrato SET contrato_inativo=0, contrato_cancelado=0, contrato_dataCancelamento=null, contrato_obs='" + toString(row["obs"]) + "' WHERE contrato_numero='" + toString(row["num_contrato"]) + "' AND contrato_operadoraId=" + operadoraId; //, new String[] { "@contrato_obs" }, new String[] { toString(row["obs"]) }, pm);
                            cmd.ExecuteNonQuery();
                        }
                        else if (toString(row["status"]) == "2") //inativo
                        {
                            cmd.CommandText = String.Concat("UPDATE contrato SET contrato_inativo=1, contrato_cancelado=0, contrato_dataCancelamento='", toDateTime(row["DATA_CANCEL"]).ToString("yyyy-MM-dd"), "', contrato_obs='", toString(row["obs"]), "' WHERE contrato_numero='", toString(row["num_contrato"]), "' AND contrato_operadoraId=", operadoraId); //, new String[] { "@contrato_obs" }, new String[] { toString(row["obs"]) }, pm);
                            cmd.ExecuteNonQuery();
                        }
                        else //cancelado
                        {
                            cmd.CommandText = String.Concat("UPDATE contrato SET contrato_inativo=0, contrato_cancelado=1, contrato_dataCancelamento='", toDateTime(row["DATA_CANCEL"]).ToString("yyyy-MM-dd"), "', contrato_obs='", toString(row["obs"]), "' WHERE contrato_numero='", toString(row["num_contrato"]), "' AND contrato_operadoraId=", operadoraId); //, new String[] { "@contrato_obs" }, new String[] { toString(row["obs"]) }, pm);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    catch (Exception ex)
                    {
                        ErroSumario erro = new ErroSumario();
                        erro.MSG = ex.Message + " | " + toString(row["num_contrato"]);
                        erros.Add(erro);

                        continue;
                    }

                    i++;
                }
            }
        }

        public void ArrumaStatusDePropostas(ref List<ErroSumario> erros)
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT num_contrato,cnpj_operadora,status,DATA_CANCEL,obs FROM PROPOSTAS ORDER BY NUM_CONTRATO", conn); //AND num_contrato='S0409' 
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
                conn.Close();
            }

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();
            Object operadoraId = null;

            erros = new List<ErroSumario>();

            foreach (DataRow row in dsOrigem.Tables[0].Rows)
            {
                try
                {
                    operadoraId = Operadora.CarregarIDPorCNPJ(toString(row["cnpj_operadora"]), pm);

                    if (operadoraId == null)
                    {
                        ErroSumario erro = new ErroSumario();
                        erro.MSG = "Operadora nao localizada: " + toString(row["cnpj_operadora"]);
                        erros.Add(erro);
                        continue;
                    }

                    if (toString(row["status"]) == "1")
                    {
                        NonQueryHelper.Instance.ExecuteNonQuery("UPDATE contrato SET contrato_inativo=0, contrato_cancelado=0, contrato_dataCancelamento=null, contrato_obs=@contrato_obs WHERE contrato_numero='" + toString(row["num_contrato"]) + "' AND contrato_operadoraId=" + operadoraId, new String[] { "@contrato_obs" }, new String[] { toString(row["obs"]) }, pm);
                    }
                    else if (toString(row["status"]) == "2") //inativo
                    {
                        NonQueryHelper.Instance.ExecuteNonQuery("UPDATE contrato SET contrato_inativo=1, contrato_cancelado=0, contrato_dataCancelamento='" + toDateTime(row["DATA_CANCEL"]).ToString("yyyy-MM-dd") + "', contrato_obs=@contrato_obs WHERE contrato_numero='" + toString(row["num_contrato"]) + "' AND contrato_operadoraId=" + operadoraId, new String[] { "@contrato_obs" }, new String[] { toString(row["obs"]) }, pm);
                    }
                    else //cancelado
                    {
                        NonQueryHelper.Instance.ExecuteNonQuery("UPDATE contrato SET contrato_inativo=0, contrato_cancelado=1, contrato_dataCancelamento='" + toDateTime(row["DATA_CANCEL"]).ToString("yyyy-MM-dd") + "', contrato_obs=@contrato_obs WHERE contrato_numero='" + toString(row["num_contrato"]) + "' AND contrato_operadoraId=" + operadoraId, new String[] { "@contrato_obs" }, new String[] { toString(row["obs"]) }, pm);
                    }
                }

                catch (Exception ex)
                {
                    ErroSumario erro = new ErroSumario();
                    erro.MSG = ex.Message + " | " + toString(row["num_contrato"]);
                    erros.Add(erro);

                    continue;
                }
            }

            pm.Commit();
        }

        public String ArrumaCPFs()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM correcao_cpf", conn);
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                int j = 0;

                try
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (DataRow row in dsOrigem.Tables[0].Rows)
                    {
                        if (sb.Length > 0) { sb.Append(";"); sb.Append(Environment.NewLine); }

                        sb.Append("UPDATE beneficiario SET beneficiario_cpf='");
                        sb.Append(row["CPF"]);
                        sb.Append("' WHERE importId=");
                        sb.Append(row["ID"]);

                        j++;

                        if (j == 200)
                        {
                            cmd.CommandText = sb.ToString();
                            cmd.ExecuteNonQuery();
                            sb.Remove(0, sb.Length);
                            j = 0;
                        }
                    }

                    if (sb.Length > 0)
                    {
                        cmd.CommandText = sb.ToString();
                        cmd.ExecuteNonQuery();
                    }

                    return sb.ToString();
                }
                catch
                {
                    throw;
                }
                finally
                {
                }
            }
        }

        public String ChecaCPFs()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM correcao_cpf", conn);
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                StringBuilder sb = new StringBuilder();
                object ret = null;

                try
                {
                    foreach (DataRow row in dsOrigem.Tables[0].Rows)
                    {
                        cmd.CommandText = String.Concat("SELECT beneficiario_id from beneficiario where importId=",
                            row["ID"]);

                        ret = cmd.ExecuteScalar();

                        if (ret == null || ret == DBNull.Value)
                        {
                            sb.Append("ID: ");
                            sb.Append(row["ID"]);
                            sb.Append(" | CPF: ");
                            sb.Append(row["CPF"]);
                            sb.Append(Environment.NewLine);
                        }
                    }

                    return sb.ToString();
                }
                catch
                {
                    throw;
                }
                finally
                {
                }
            }
        }

        public void ArrumaCobrancaDeTaxasAssociativasEmPropostas()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM DESCONTO", conn);
                adp.Fill(dsOrigem, "DESCONTO");
                adp.Dispose();
            }

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                Object propostaId = null;
                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    cmd.CommandText = "SELECT contrato_id FROM contrato WHERE contrato_codcobranca=" + row["ID_COBRANCA"];
                    propostaId = cmd.ExecuteScalar();

                    if (propostaId == null) { throw new ApplicationException("Proposta não localizada."); }

                    cmd.CommandText = "UPDATE contrato SET contrato_cobrartaxaassociativa=1 WHERE contrato_id=" + propostaId;
                    cmd.ExecuteNonQuery();
                }
            }

            dsOrigem.Dispose();
        }

        //--------------------------------------------------------------------------------//

        public void DuplicaLayoutsCustomizados()
        {
            LayoutArquivoCustomizado lac = null;
            IList<ItemLayoutArquivoCustomizado> itens = null;
            IList<Operadora> operadoras = Operadora.CarregarTodas(true);

            PersistenceManager pm = new PersistenceManager();
            pm.BeginTransactionContext();

            foreach (Operadora operadora in operadoras)
            {
                lac = new LayoutArquivoCustomizado(1);
                pm.Load(lac);
                itens = ItemLayoutArquivoCustomizado.Carregar(lac.ID, pm);

                lac.ID   = null;
                lac.Tipo = (int)LayoutArquivoCustomizado.eTipoTransacao.SINCRONIZACAO_SEG;
                lac.OperadoraID = operadora.ID;
                pm.Save(lac);

                foreach (ItemLayoutArquivoCustomizado _item in itens)
                {
                    _item.ID = null;
                    _item.LayoutID = lac.ID;
                    pm.Save(_item);
                }
            }

            pm.Commit();
        }

        public void ArrumaBeneficiariosDuplicados()
        {
            String qry = "select beneficiario_cpf, COUNT(beneficiario_cpf) as qtd from beneficiario group by beneficiario_cpf, beneficiario_cpf having COUNT (beneficiario_cpf) > 1";
            DataSet ds = new DataSet();
            /*

            select beneficiario_nome, beneficiario_cpf, COUNT(beneficiario_cpf) from 
            beneficiario 
            where beneficiario_cpf <> '99999999999'
            group by beneficiario_nome, beneficiario_cpf, beneficiario_cpf 
            having COUNT (beneficiario_cpf) > 1 

            */
            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlDataAdapter adp = new SqlDataAdapter(qry, conn);
                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count == 0) { return; }

                String nome = "";
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    //
                }
            }
        }

        public void DuplicaDependentes()
        {
            String operadoraId   = "16";
            String estipulanteId = "9";
            String contratoAdmId = "53";

            String qry = "SELECT contratoadm_id FROM contratoadm WHERE contratoadm_operadoraid=" + operadoraId + " AND contratoadm_id <> " + contratoAdmId;

            DataTable dtContratos = LocatorHelper.Instance.ExecuteQuery(qry, "contratosadm").Tables[0];

            qry = "SELECT * FROM contratoAdm_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=";
            DataTable dtDependentes = null;
            DataRow[] result = null;
            foreach (DataRow contrato in dtContratos.Rows)
            {
                dtDependentes = LocatorHelper.Instance.ExecuteQuery(qry + contrato["contratoadm_id"], "depend").Tables[0];
            }
        }

        public String SetaReativacoes()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM REATIVACOES", conn);
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            String qry = "SELECT contrato_id, contrato_obs FROM contrato where (contrato_inativo <> 0 OR contrato_cancelado <> 0) AND contrato_numero='";

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = conn.BeginTransaction();
                String obs = "";

                try
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (DataRow row in dsOrigem.Tables[0].Rows)
                    {
                        cmd.CommandText = String.Concat(qry, toString(row["PROPOSTA"]), "'");

                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (!dr.Read())
                            {
                                //nao achou a proposta!
                                dr.Close(); dr.Dispose();
                                continue;
                            }

                            if (sb.Length > 0) { sb.Append(";"); sb.Append(Environment.NewLine); }

                            obs = String.Concat(toString(dr["contrato_obs"]),Environment.NewLine, toString(row["OBS"]));

                            sb.Append("UPDATE contrato SET contrato_cancelado=0, contrato_inativo=0, contrato_dataCancelamento=null, contrato_obs='");
                            sb.Append(obs.Replace("'", "´"));
                            sb.Append("' WHERE contrato_id=");
                            sb.Append(dr["contrato_id"]);

                            dr.Close(); dr.Dispose();
                        }
                    }

                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    return sb.ToString();
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
                finally
                {
                }
            }
        }

        public String SetaInativacoes()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM INATIVACOES", conn);
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            String qry = "SELECT contrato_id, contrato_obs FROM contrato where (contrato_inativo <> 1 AND contrato_cancelado <> 1) AND contrato_numero='";

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = conn.BeginTransaction();
                String obs = "";
                DateTime data;

                try
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (DataRow row in dsOrigem.Tables[0].Rows)
                    {
                        cmd.CommandText = String.Concat(qry, toString(row["PROPOSTA"]), "'");

                        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            if (!dr.Read())
                            {
                                //nao achou a proposta!
                                dr.Close(); dr.Dispose();
                                continue;
                            }

                            if (sb.Length > 0) { sb.Append(";"); sb.Append(Environment.NewLine); }

                            obs = String.Concat(toString(dr["contrato_obs"]), Environment.NewLine, toString(row["OBS"]));
                            data = toDateTime(row["DATA_CANCELAMENTO"]);

                            sb.Append("UPDATE contrato SET contrato_cancelado=0, contrato_inativo=1, contrato_dataCancelamento='");
                            sb.Append(data.ToString("yyyy-MM-dd"));
                            sb.Append("', contrato_obs='");
                            sb.Append(obs.Replace("'", "´"));
                            sb.Append("' WHERE contrato_id=");
                            sb.Append(dr["contrato_id"]);

                            dr.Close(); dr.Dispose();
                        }
                    }

                    cmd.CommandText = sb.ToString();
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    return sb.ToString();
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
                finally
                {
                }
            }
        }

        public void GeraParentescos()
        {
            int operadoraId = 17;
            Object count;

            String cmd = null;

            //IList<Estipulante> estipulantes = Estipulante.Carregar(false);
            //foreach (Estipulante estipulante in estipulantes)
            //{
                IList<ContratoADM> contratos = ContratoADM.Carregar(operadoraId, true);
                foreach (ContratoADM contrato in contratos)
                {
                    count = LocatorHelper.Instance.ExecuteScalar("SELECT COUNT(*) FROM contratoAdm_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=" + contrato.ID + " AND contratoAdmparentescoagregado_parentescoDescricao LIKE 'ESPOS%'", null, null);
                    if (count == null || count == DBNull.Value || Convert.ToInt32(count) == 0)
                    {
                        cmd = String.Concat(
                            "INSERT INTO contratoAdm_parentesco_agregado (contratoAdmparentescoagregado_contratoAdmId,contratoAdmparentescoagregado_parentescoDescricao,contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo) VALUES (", contrato.ID, ", 'ESPOSO(A)',2,1)");
                        NonQueryHelper.Instance.ExecuteNonQuery(cmd, null);
                    }

                    count = LocatorHelper.Instance.ExecuteScalar("SELECT COUNT(*) FROM contratoAdm_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=" + contrato.ID + " AND contratoAdmparentescoagregado_parentescoDescricao LIKE 'FILH%'", null, null);
                    if (count == null || count == DBNull.Value || Convert.ToInt32(count) == 0)
                    {
                        cmd = String.Concat(
                            "INSERT INTO contratoAdm_parentesco_agregado (contratoAdmparentescoagregado_contratoAdmId,contratoAdmparentescoagregado_parentescoDescricao,contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo) VALUES (", contrato.ID, ", 'FILHO(A)',3,1)");
                        NonQueryHelper.Instance.ExecuteNonQuery(cmd, null);
                    }

                    count = LocatorHelper.Instance.ExecuteScalar("SELECT COUNT(*) FROM contratoAdm_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=" + contrato.ID + " AND contratoAdmparentescoagregado_parentescoDescricao LIKE 'IRM%'", null, null);
                    if (count == null || count == DBNull.Value || Convert.ToInt32(count) == 0)
                    {
                        cmd = String.Concat(
                            "INSERT INTO contratoAdm_parentesco_agregado (contratoAdmparentescoagregado_contratoAdmId,contratoAdmparentescoagregado_parentescoDescricao,contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo) VALUES (", contrato.ID, ", 'IRMA(O)',4,1)");
                        NonQueryHelper.Instance.ExecuteNonQuery(cmd, null);
                    }

                    count = LocatorHelper.Instance.ExecuteScalar("SELECT COUNT(*) FROM contratoAdm_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=" + contrato.ID + " AND contratoAdmparentescoagregado_parentescoDescricao LIKE 'OUTR%'", null, null);
                    if (count == null || count == DBNull.Value || Convert.ToInt32(count) == 0)
                    {
                        cmd = String.Concat(
                            "INSERT INTO contratoAdm_parentesco_agregado (contratoAdmparentescoagregado_contratoAdmId,contratoAdmparentescoagregado_parentescoDescricao,contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo) VALUES (", contrato.ID, ", 'OUTROS',4,1)");
                        NonQueryHelper.Instance.ExecuteNonQuery(cmd, null);
                    }

                    count = LocatorHelper.Instance.ExecuteScalar("SELECT COUNT(*) FROM contratoAdm_parentesco_agregado WHERE contratoAdmparentescoagregado_contratoAdmId=" + contrato.ID + " AND contratoAdmparentescoagregado_parentescoDescricao LIKE '%PAI%'", null, null);
                    if (count == null || count == DBNull.Value || Convert.ToInt32(count) == 0)
                    {
                        cmd = String.Concat(
                            "INSERT INTO contratoAdm_parentesco_agregado (contratoAdmparentescoagregado_contratoAdmId,contratoAdmparentescoagregado_parentescoDescricao,contratoAdmparentescoagregado_parentescoCodigo,contratoAdmparentescoagregado_parentescoTipo) VALUES (", contrato.ID, ", 'PAI / MAE',1,1)");
                        NonQueryHelper.Instance.ExecuteNonQuery(cmd, null);
                    }
                }
            //}
        }


        public void Arruma1Cobranca()
        {
            String qry = "select contrato_id, contrato_numero, contrato_data, contrato_admissao, cobranca_id from contrato left join cobranca on contrato_id=cobranca_propostaId where cobranca_id is null ";

            DataTable dt = LocatorHelper.Instance.ExecuteQuery(qry, "resultset").Tables[0];

            Cobranca cobranca = null;

            PersistenceManager pm = null;
            pm = new PersistenceManager();
            pm.BeginTransactionContext();
            List<CobrancaComposite> composite = null;

            int i = 0;
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (i == 500) { break; }
                    cobranca                = new Cobranca();
                    cobranca.Cancelada      = false;
                    cobranca.ComissaoPaga   = true;
                    cobranca.DataCriacao    = toDateTime(row["contrato_data"]);
                    cobranca.DataPgto       = toDateTime(row["contrato_admissao"]);
                    cobranca.DataVencimento = cobranca.DataPgto;
                    cobranca.Pago           = true;
                    cobranca.Parcela        = 1;
                    cobranca.PropostaID     = row["contrato_id"];
                    cobranca.Tipo           = (int)Cobranca.eTipo.Normal;
                    cobranca.Valor          = Contrato.CalculaValorDaProposta(row["contrato_id"], cobranca.DataPgto, pm, true, true, ref composite);

                    cobranca.ValorNominal = cobranca.Valor;
                    cobranca.ValorPgto    = cobranca.Valor;

                    pm.Save(cobranca);
                    i++;
                }

                pm.Commit();

            }
            catch(Exception ex)
            {
                //if(pm != null)
                //pm.Rollback();
                throw;
            }
            finally
            {
                dt.Dispose();
                //if (pm != null)
                //pm.Dispose();
            }
        }

        public void SetaCobrancasPAGAS()
        {
            DataSet dsOrigem = new DataSet();

            using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + mdbPath + ";Persist Security Info=False;"))
            {
                conn.Open();
                OleDbDataAdapter adp = new OleDbDataAdapter("SELECT * FROM baixa", conn);
                adp.Fill(dsOrigem, "propostas");
                adp.Dispose();
            }

            using (SqlConnection conn = new SqlConnection(sqlConn))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                Object propostaId = null;
                Object cobrancaId = null;
                DateTime data = DateTime.MinValue;

                foreach (DataRow row in dsOrigem.Tables[0].Rows)
                {
                    cmd.CommandText = "SELECT contrato_id FROM contrato WHERE contrato_codCobranca=" + row["id"];
                    propostaId = cmd.ExecuteScalar();

                    cmd.CommandText = "SELECT cobranca_id FROM cobranca WHERE cobranca_parcela=" + row["parcela"] + " AND cobranca_propostaId=" + propostaId;
                    cobrancaId = cmd.ExecuteScalar();

                    cmd.CommandText = String.Concat("UPDATE cobranca SET cobranca_pago=1, cobranca_valorPagto='",
                    toDecimal(row["Cobrança_valor"]).ToString("N2").Replace(".", "").Replace(",", "."), 
                    "', cobranca_dataPagto=");

                    #region data pagamento
                    data = toDateTime(row["dt_pagto"]);
                    if (data != DateTime.MinValue && data.Year > 1753)
                    {
                        cmd.CommandText += "'";
                        cmd.CommandText += data.ToString("yyyy-MM-dd");
                        cmd.CommandText += "'";
                    }
                    else
                    {
                        cmd.CommandText += "NULL";
                    }
                    #endregion

                     cmd.CommandText += " WHERE cobranca_id=" + cobrancaId;
                     cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
