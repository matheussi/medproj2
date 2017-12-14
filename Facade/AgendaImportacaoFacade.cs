namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Data;
    using MedProj.Entidades;
    using System.Collections.Generic;

    using NHibernate;
    using NHibernate.Linq;
    using MedProj.Entidades.Enuns;

    public class AgendaImportacaoFacade : FacadeBase
    {
        AgendaImportacaoFacade() { }

        #region singleton 

        static AgendaImportacaoFacade _instancia;
        public static AgendaImportacaoFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new AgendaImportacaoFacade(); }
                return _instancia;
            }
        }
        #endregion

        public AgendaImportacao Salvar(AgendaImportacao agenda)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        sessao.SaveOrUpdate(agenda);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return agenda;
        }

        public AgendaImportacao Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<AgendaImportacao>()
                    .Fetch(i => i.Operadora)
                    .Fetch(i => i.AssociadoPj)
                    .Fetch(i => i.Autor)
                    .Fetch(i => i.Contrato)
                    .Fetch(i => i.Filial)
                    .Fetch(i => i.Plano)
                    .Where(i => i.ID == id)
                    .Single();
            }
        }

        public AgendaImportacao CarregarPendenteParaProcessamento(DateTime data)
        {
            AgendaImportacao agenda = null;

            using (var sessao = ObterSessao())
            {
                agenda = sessao.Query<AgendaImportacao>()
                    .Fetch(i => i.Operadora)
                    .Fetch(i => i.AssociadoPj)
                    .Fetch(i => i.Autor)
                    .Fetch(i => i.Contrato)
                    .Fetch(i => i.Filial)
                    .Fetch(i => i.Plano)
                    .Where(i => i.DataConclusao.HasValue == false && i.Ativa == true && i.DataProcessamento <= data)
                    .OrderBy(i => i.DataCriacao)
                    .FirstOrDefault();

                sessao.Close();
            }

            return agenda;
        }

        public List<AgendaImportacao> Carregar(DateTime de, DateTime ate, AgendaStatus status)
        {
            List<AgendaImportacao> lista = null;

            using (var sessao = ObterSessao())
            {
                if (status == AgendaStatus.Concluido)
                {
                    lista = sessao.Query<AgendaImportacao>()
                        .Fetch(i => i.Operadora)
                        .Fetch(i => i.AssociadoPj)
                        .Fetch(i => i.Autor)
                        .Fetch(i => i.Contrato)
                        .Fetch(i => i.Filial)
                        .Fetch(i => i.Plano)
                        .Where(i => i.DataConclusao.HasValue && i.DataCriacao >= de && i.DataCriacao <= ate)
                        .OrderByDescending(i => i.DataCriacao)
                        .Take(250)
                        .ToList();
                }
                else if (status == AgendaStatus.Pendente)
                {
                    lista = sessao.Query<AgendaImportacao>()
                        .Fetch(i => i.Operadora)
                        .Fetch(i => i.AssociadoPj)
                        .Fetch(i => i.Autor)
                        .Fetch(i => i.Contrato)
                        .Fetch(i => i.Filial)
                        .Fetch(i => i.Plano)
                        .Take(250)
                        .Where(i => i.DataConclusao.HasValue == false && i.DataCriacao >= de && i.DataCriacao <= ate)
                        .OrderByDescending(i => i.DataCriacao)
                        .ToList();
                }
                else
                {
                    lista = sessao.Query<AgendaImportacao>()
                        .Fetch(i => i.Operadora)
                        .Fetch(i => i.AssociadoPj)
                        .Fetch(i => i.Autor)
                        .Fetch(i => i.Contrato)
                        .Fetch(i => i.Filial)
                        .Fetch(i => i.Plano)
                        .Where(i => i.DataCriacao >= de && i.DataCriacao <= ate)
                        .OrderByDescending(i => i.DataCriacao)
                        .Take(250)
                        .ToList();
                }
            }

            return lista;
        }

        public List<AgendaImportacaoItemLog> CarregarLog(long agendaId)
        {
            List<AgendaImportacaoItemLog> log = null;

            using (var sessao = ObterSessao())
            {
                log = sessao.Query<AgendaImportacaoItemLog>()
                    .Fetch(l => l.Agenda)
                    .Fetch(l => l.Titular).ThenFetch(t => t.Contrato)
                    .Where(l => l.Agenda.ID == agendaId)
                    .ToList();
            }

            return log;
        }

        public DataTable CarregarLogV2(long agendaId)
        {
            using (var sessao = ObterSessao())
            {
                using (IDbCommand cmd = sessao.Connection.CreateCommand())
                {
                    cmd.CommandText = string.Concat(
                        "select importacaolog_linha,importacaolog_mensagem,importacaolog_status,contrato_id,contrato_numeroid,contratobeneficiario_id,contrato_numero,beneficiario_rg,contrato_produto,numerocontrato_via,numerocontrato_cv,contrato_senha,beneficiario_nome,contratoadm_descricao,beneficiario_cpf,beneficiario_dataNascimento,contrato_ramo,contrato_numeroApolice,contrato_vigencia,contrato_validade,contrato_admissao,endereco_logradouro,endereco_numero,endereco_complemento,endereco_bairro,endereco_cidade,endereco_uf,endereco_cep,contrato_numeromatricula,contrato_caminhoArquivo ",
                        "   from importacao_log ",
                        "       left join contrato_beneficiario on contratobeneficiario_id = importacaolog_titularId ",
                        "       left join beneficiario on beneficiario_id = contratobeneficiario_beneficiarioId ",
                        "       left join contrato on contrato_id = contratobeneficiario_contratoid and contratobeneficiario_tipo=0 ",
                        "       left join numero_contrato on numerocontrato_id= contrato_numeroId ",
                        "       left join contratoadm on contratoadm_id = contrato_contratoadmid ",
                        "       left join endereco on endereco_donotipo=0 and endereco_donoid=beneficiario_id ",
                        "   where ",
                        "       importacaolog_agendaid=", agendaId,
                        "   order by importacaolog_linha");

                    #region DataTable

                    DataTable dt = new DataTable();
                    dt.Columns.Add("CARTAO");
                    dt.Columns.Add("ABREVIADO");
                    dt.Columns.Add("RG"); //////////////////////

                    dt.Columns.Add("PRODUTO");
                    dt.Columns.Add("VIA");
                    dt.Columns.Add("CVV");
                    dt.Columns.Add("VALIDADE");
                    dt.Columns.Add("SENHA");


                    dt.Columns.Add("NOME_BENEFICIARIO");

                    dt.Columns.Add("CONTRATOPJ");//////////////


                    dt.Columns.Add("CPF_TITULAR");
                    dt.Columns.Add("DT_NASCIMENTO");

                    dt.Columns.Add("RAMO");
                    dt.Columns.Add("APOLICE");

                    dt.Columns.Add("INICIO_DO_RISCO");///////////////
                    dt.Columns.Add("FIM_DA_VIGENCIA");//////////////
                    dt.Columns.Add("DATA_DE_EMISSAO");//////////////

                    dt.Columns.Add("LOGRADOURO");
                    dt.Columns.Add("NUMERO");
                    dt.Columns.Add("COMPLEMENTO");
                    dt.Columns.Add("BAIRRO");
                    dt.Columns.Add("CEP");
                    dt.Columns.Add("CIDADE");
                    dt.Columns.Add("UF");
                    dt.Columns.Add("MATRICULA");

                    dt.Columns.Add("PATH");

                    dt.Columns.Add("MSG");
                    dt.Columns.Add("LINHA");
                    dt.Columns.Add("STATUS");

                    #endregion

                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            DataRow nova = dt.NewRow();

                            nova["CARTAO"] = string.Concat("'", Convert.ToString(dr["contrato_numero"]));
                            nova["CPF_TITULAR"] = string.Concat("'", dr["beneficiario_cpf"]);

                            if (CToString(dr["contrato_senha"]).StartsWith("0"))
                                nova["SENHA"] = string.Concat("'", CToString(dr["contrato_senha"]));
                            else
                                nova["SENHA"] = CToString(dr["contrato_senha"]);

                            nova["RAMO"] = CToString(dr["contrato_ramo"]);
                            nova["APOLICE"] = CToString(dr["contrato_numeroApolice"]);

                            nova["DT_NASCIMENTO"] = Convert.ToDateTime(dr["beneficiario_dataNascimento"]).ToString("dd/MM/yyyy");
                            nova["NOME_BENEFICIARIO"] = CToString(dr["beneficiario_nome"]);
                            nova["ABREVIADO"] = Abreviar2(CToString(dr["beneficiario_nome"]));
                            nova["RG"] = CToString(dr["beneficiario_rg"]);

                            nova["CONTRATOPJ"] = CToString(dr["contratoadm_descricao"]);
                            nova["PRODUTO"] = CToString(dr["contrato_produto"]);

                            nova["LOGRADOURO"] = CToString(dr["endereco_logradouro"]);
                            nova["NUMERO"] = CToString(dr["endereco_numero"]);
                            nova["COMPLEMENTO"] = CToString(dr["endereco_complemento"]);
                            nova["BAIRRO"] = CToString(dr["endereco_bairro"]);
                            nova["CEP"] = CToString(dr["endereco_cep"]);
                            nova["CIDADE"] = CToString(dr["endereco_cidade"]);
                            nova["UF"] = CToString(dr["endereco_uf"]);

                            nova["MATRICULA"] = CToString(dr["contrato_numeromatricula"]);

                            nova["VIA"] = CToString(dr["numerocontrato_via"]);
                            nova["CVV"] = CToString(dr["numerocontrato_cv"]);
                            nova["VALIDADE"] = "CONSULTE NOSSO SITE";

                            nova["INICIO_DO_RISCO"] = Convert.ToDateTime(dr["contrato_vigencia"]).ToString("dd/MM/yyyy");
                            nova["FIM_DA_VIGENCIA"] = Convert.ToDateTime(dr["contrato_vigencia"]).AddDays(-1).ToString("dd/MM/yyyy");
                            nova["DATA_DE_EMISSAO"] = Convert.ToDateTime(dr["contrato_admissao"]).ToString("dd/MM/yyyy");
                            nova["PATH"]            = CToString(dr["contrato_caminhoArquivo"]);

                            nova["MSG"] = CToString(dr["importacaolog_mensagem"]);
                            nova["LINHA"] = CToString(dr["importacaolog_linha"]);
                            nova["STATUS"] = CToString(dr["importacaolog_status"]);

                            dt.Rows.Add(nova);
                        }
                    }

                    return dt;
                }
            }
        }
        string Abreviar2(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;

            string[] nome = s.Trim().Split(' ');

            if (nome.Length <= 2) return s;

            if (nome.Length == 3)
            {
                if (nome[2].Length <= 3) return s;
                else return string.Concat(nome[0], " ", nome[2]);
            }
            else
                return string.Concat(nome[0], " ", nome[nome.Length - 1]);
        }
    }
}
