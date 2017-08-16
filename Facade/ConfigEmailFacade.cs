namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using LinqKit;
    using System.Linq;
    using System.Text;
    using System.Data;
    using MedProj.Entidades;
    using System.Collections.Generic;

    using NHibernate;
    using NHibernate.Linq;
    using MedProj.Entidades.Enuns;

    public class ConfigEmailFacade: FacadeBase
    {
        ConfigEmailFacade() { }

        #region Singleton 

        static ConfigEmailFacade _instancia;
        public static ConfigEmailFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new ConfigEmailFacade(); }
                return _instancia;
            }
        }
        #endregion

        public long Salvar(ConfigEmailAviso obj)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        if (obj.TemId)
                        {
                            sessao.Update(obj);
                        }
                        else
                        {
                            sessao.Save(obj);
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return obj.ID;
        }

        public ConfigEmailAviso Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                ConfigEmailAviso obj = sessao.Query<ConfigEmailAviso>()
                    .Fetch(co => co.Texto)
                    .FetchMany(o => o.Contratos)
                    .Where(t => t.ID == id).Single();

                return obj;
            }
        }

        public void Excluir(long id)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        ConfigEmailAviso obj = sessao.Get<ConfigEmailAviso>(id);
                        sessao.Delete(obj);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<ConfigEmailAviso> Carregar(string nome = null)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<ConfigEmailAviso>()
                    .OrderBy(p => p.Tipo)
                    .OrderBy(p => p.Descricao)
                    .ToList();
            }
        }

        /// <summary>
        /// Carrega por associado pj (estipulante)
        /// </summary>
        public List<ConfigEmailAviso> CarregarPorAssoc(long associadoPjId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<ConfigEmailAviso>()
                    .Where(c => c.AssociadoPj.ID == associadoPjId)
                    .OrderBy(p => p.Tipo)
                    .OrderBy(p => p.Descricao)
                    .ToList();
            }
        }

        /// <summary>
        /// Carrega por associado pj (estipulante) - SOMENTE marcado como TODOS OS CONTRATOS
        /// </summary>
        public List<ConfigEmailAviso> CarregarPorAssoc(long associadoPjId, long contratoAdmId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<ConfigEmailAviso>()
                    .Where(c => c.AssociadoPj.ID == associadoPjId && c.ContratoAdm.ID == contratoAdmId && c.TodosContratos)
                    .OrderBy(p => p.Tipo)
                    .OrderBy(p => p.Descricao)
                    .ToList();
            }
        }

        public List<ConfigEmailAviso> CarregarPor(long contratoAdmId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<ConfigEmailAviso>()
                    .Where(c => c.ContratoAdm.ID == contratoAdmId)
                    .OrderBy(p => p.Tipo)
                    .OrderBy(p => p.Descricao)
                    .ToList();
            }
        }

        public List<ConfigEmailAviso> CarregarPor(long contratoAdmId, long contratoId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<ConfigEmailAviso>()
                    .Where(c => (c.ContratoAdm.ID == contratoAdmId && !c.TodosContratos) && (c.Contratos.Any(co => co.ID == contratoId)))
                    .OrderBy(p => p.Tipo)
                    .OrderBy(p => p.Descricao)
                    .ToList();
            }
        }

        public List<InstanciaSrcVO> CarregarParaProcessamento(ConfigEmailAviso config, ISession sessao, ITransaction tran)
        {
            List<InstanciaSrcVO> lista = null;

            //string qry = "", dataCriacaoCriterio = " and cobranca_dataCriacao >= '2000-01-25'"; //
            string qry = "", qry2 = "", dataCriacaoCriterio = " and cobranca_dataCriacao >= '" + config.DataCriacao.ToString("yyyy-MM-dd") +  "'"; //todo: denis, a data correta é 2017-02-13

            if (config.Tipo == TipoConfig.AvisoDePagamento)
            {
                qry = string.Concat(
                        "select top 50 beneficiario_id,beneficiario_nome,beneficiario_email,cei_cobrancaId,cobranca_id,cobranca_valor,cobranca_dataPagto,cobranca_dataVencimento,cobranca_pago,cobranca_propostaId,cobranca_qtdVidas ",
                        "   from beneficiario ",
                        "       inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 ",
                        "       inner join cobranca on cobranca_propostaid = contratobeneficiario_contratoid ",
                        "       inner join contrato on contrato_id = cobranca_propostaId ",
                        "       left join config_email_instancia on cobranca_id = cei_cobrancaId and cei_tipo=", Convert.ToInt16(config.Tipo).ToString(), " and cei_configId= ", config.ID.ToString(),
                        "   where ", //contrato_id=143274 and 
                        "       contrato_tipoPessoa = 1 and cei_cobrancaId is null and cobranca_pago = 1", dataCriacaoCriterio);

                //TODO  para resolver o problema que ocorrerá de um novo aviso de pagamento enviar todas 
                //      as cobrancas, ja avisadas, novamente, fazer and cobranca_datacriacao > config.datacriacao
            }
            else if (config.Tipo == TipoConfig.AvisoDeVencimentoProximo)
            {
                qry = string.Concat(
                        "select top 50 beneficiario_id,beneficiario_nome,beneficiario_email,cei_cobrancaId,cobranca_id,cobranca_valor,cobranca_dataPagto,cobranca_dataVencimento,cobranca_pago,cobranca_propostaId,cobranca_qtdVidas ",
                        "   from beneficiario ",
                        "       inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 ",
                        "       inner join cobranca on cobranca_propostaid = contratobeneficiario_contratoid ",
                        "       inner join contrato on contrato_id = cobranca_propostaId ",
                        "       left join config_email_instancia on cobranca_id = cei_cobrancaId and cei_configId= ", config.ID.ToString(), //and cei_tipo=", Convert.ToInt16(config.Tipo).ToString(), " 
                        "   where ", //contrato_id=143274 and 
                        "       contrato_tipoPessoa = 1 and ",
                        "       cei_cobrancaId is null and ",
                        "       cobranca_cancelada=0 and contrato_inativo=0 and contrato_cancelado=0 and ",
                        "       cobranca_pago = 0 and ",
                        "       datediff(day, getdate(), cobranca_datavencimento) = ", config.DiasAntesVencimento.ToString(),
                        dataCriacaoCriterio);

                //data de vencimento que deveria existir
                DateTime venctoReferencia = DateTime.Now.AddDays(config.DiasAntesVencimento);

                qry2 = string.Concat(
                    "select contrato_id,contrato_qtdVidas,contratoadm_dtvc,beneficiario_id,beneficiario_nome,beneficiario_email ",
                    "   from beneficiario ",
                    "       inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 ",
                    "       inner join contrato on contrato_id = contratobeneficiario_contratoId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                    "       inner join contratoadm on contratoadm_id = contrato_contratoadmid and contratoadm_dtvc is not null ",
                    "   where ",
//                  "       contratoadm_dtvc=", vencimentoReferencia2.Day, " and ",
                    "       contrato_inativo=0 and contrato_cancelado=0 and contrato_tipoPessoa=1 ",
                    "       and contrato_id not in (",
                    "           select cobranca_propostaid from cobranca where cobranca_cancelada=0 and month(cobranca_datavencimento)=", venctoReferencia.Month, " and year(cobranca_datavencimento)=", venctoReferencia.Year, 
                    "       ) ",
                    "       and datediff(day, getdate(), concat('", venctoReferencia.Year, "-", venctoReferencia.Month, "-',contratoadm_dtvc))=", config.DiasAntesVencimento);
            }
            else if (config.Tipo == TipoConfig.AvisoDeVencimentoPassado)
            {
                DateTime venctoReferencia = DateTime.Now.AddDays(config.DiasAntesVencimento * -1);

                qry = string.Concat(
                        "select top 50 beneficiario_id,beneficiario_nome,beneficiario_email,cei_cobrancaId,cobranca_id,cobranca_valor,cobranca_dataPagto,cobranca_dataVencimento,cobranca_pago,cobranca_propostaId,cobranca_qtdVidas ",
                        "   from beneficiario ",
                        "       inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 ",
                        "       inner join cobranca on cobranca_propostaid = contratobeneficiario_contratoid ",
                        "       inner join contrato on contrato_id = cobranca_propostaId ",
                        "       left join config_email_instancia on cobranca_id = cei_cobrancaId and cei_configId= ", config.ID.ToString(), //and cei_tipo=", Convert.ToInt16(config.Tipo).ToString(), " 
                        "   where ", //contrato_id=143274 and 
                        "       contrato_tipoPessoa = 1 and ",
                        "       cei_cobrancaId is null and ",
                        "       cobranca_cancelada=0 and contrato_inativo=0 and contrato_cancelado=0 and ",
                        "       cobranca_pago = 0 and ",
                        "       datediff(day, cobranca_datavencimento, getdate()) = ", config.Frequencia.ToString(),
                        dataCriacaoCriterio);
            }
            else
                return null;

            if (config.AssociadoPj != null)
            {
                qry  += " and contrato_estipulanteId = " + config.AssociadoPj.ID;
                if(!string.IsNullOrEmpty(qry2)) qry2 += " and contrato_estipulanteId = " + config.AssociadoPj.ID;
            }

            if (config.ContratoAdm != null)
            {
                qry  += " and contrato_contratoAdmId = " + config.ContratoAdm.ID;
                if (!string.IsNullOrEmpty(qry2)) qry2 += " and contrato_contratoAdmId = " + config.ContratoAdm.ID;
            }


            string tipoCond = " and ce_tipo= " + Convert.ToInt32(config.Tipo).ToString();

            if (config.TodosContratos == false) //NAO EH TODOS OS CONTRATOS
            {
                qry  += string.Concat(" and contrato_id in (select cec_contratoId from config_email_contratos where cec_configId=", config.ID , ")");
                if (!string.IsNullOrEmpty(qry2)) qry2 += string.Concat(" and contrato_id in (select cec_contratoId from config_email_contratos where cec_configId=", config.ID, ")");
            }
            else if (config.TodosContratos) //TODOS OS CONTRATOS 
            {
                qry  += string.Concat(" and contrato_id not in (select cec_contratoId from config_email_contratos inner join config_email on ce_id=cec_configId where ce_ativo=1 and cec_configId <> ", config.ID, tipoCond, ")");
                if (!string.IsNullOrEmpty(qry2)) qry2 += string.Concat(" and contrato_id not in (select cec_contratoId from config_email_contratos inner join config_email on ce_id=cec_configId where ce_ativo=1 and cec_configId <> ", config.ID, tipoCond, ")");

                if (config.ContratoAdm == null && config.AssociadoPj == null)
                {
                    qry  += string.Concat(" and contrato_estipulanteId not in (select ce_estipulanteId from config_email where ce_ativo=1 and (ce_estipulanteId = contrato_estipulanteId) and ce_todosContratos=1 and ce_id <> ", config.ID, tipoCond, ")");
                    qry  += string.Concat(" and contrato_contratoAdmId not in (select ce_contratoAdmId from config_email where ce_ativo=1 and (ce_contratoAdmId = contrato_contratoAdmId) and ce_todosContratos=1 and ce_id <> ", config.ID, tipoCond, ")");

                    if (!string.IsNullOrEmpty(qry2)) qry2 += string.Concat(" and contrato_estipulanteId not in (select ce_estipulanteId from config_email where ce_ativo=1 and (ce_estipulanteId = contrato_estipulanteId) and ce_todosContratos=1 and ce_id <> ", config.ID, tipoCond, ")");
                    if (!string.IsNullOrEmpty(qry2)) qry2 += string.Concat(" and contrato_contratoAdmId not in (select ce_contratoAdmId from config_email where ce_ativo=1 and (ce_contratoAdmId = contrato_contratoAdmId) and ce_todosContratos=1 and ce_id <> ", config.ID, tipoCond, ")");

                    //qry += string.Concat(" and contrato_contratoAdmId not in (select ce_contratoAdmId from config_email inner join config_email on ce_id = cec_configId where ce_ativo=1 and ce_todosContratos=1 and ce_id <> ", config.ID, ")");
                    //qry += string.Concat(" and contrato_estipulanteId not in (select ce_estipulanteId from config_email inner join config_email on ce_id = cec_configId where ce_ativo=1 and ce_todosContratos=1 and ce_id <> ", config.ID, ")");
                }
                else if (config.AssociadoPj != null && config.ContratoAdm == null)
                {
                    qry  += string.Concat(" and contrato_contratoAdmId not in (select ce_contratoAdmId from config_email where ce_ativo=1 and (ce_contratoAdmId = contrato_contratoAdmId) and ce_todosContratos=1 and ce_id <> ", config.ID, tipoCond, ")");
                    if (!string.IsNullOrEmpty(qry2)) qry2 += string.Concat(" and contrato_contratoAdmId not in (select ce_contratoAdmId from config_email where ce_ativo=1 and (ce_contratoAdmId = contrato_contratoAdmId) and ce_todosContratos=1 and ce_id <> ", config.ID, tipoCond, ")");

                    //qry += string.Concat(" and contrato_estipulanteId not in (select ce_estipulanteId from config_email where ce_ativo=1 and (ce_estipulanteId = contrato_estipulanteId) and ce_todosContratos=1 and ce_id <> ", config.ID, tipoCond, ")");
                }
            }

            lista = new List<InstanciaSrcVO>();
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

            using (IDbCommand cmd = sessao.Connection.CreateCommand())
            {
                tran.Enlist(cmd);
                cmd.CommandText = qry;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    InstanciaSrcVO vo = null;

                    while (dr.Read())
                    {
                        #region qry  

                        vo = new InstanciaSrcVO();

                        try
                        {
                            vo.BeneficiarioID   = Convert.ToInt64(dr["beneficiario_id"]);
                            vo.BeneficiarioMAIL = CToString(dr["beneficiario_email"]);
                            vo.BeneficiarioNM   = CToString(dr["beneficiario_nome"]);
                            vo.CobrancaID       = Convert.ToInt64(dr["cobranca_id"]);
                            vo.CobrancaPAGA     = CToBool(dr["cobranca_pago"]);

                            if(dr["cobranca_dataPagto"] != DBNull.Value)
                                vo.CobrancaDtPagto  = Convert.ToDateTime(dr["cobranca_dataPagto"], cinfo);

                            vo.CobrancaDtVenct  = Convert.ToDateTime(dr["cobranca_dataVencimento"], cinfo);
                            vo.CobrancaValor    = Convert.ToDecimal(dr["cobranca_valor"], cinfo);
                            vo.PropostaID       = Convert.ToInt64(dr["cobranca_propostaId"]);

                            if (dr["cobranca_qtdVidas"] != DBNull.Value)
                                vo.QtdVidas     = Convert.ToInt32(dr["cobranca_qtdVidas"]);
                            else
                                vo.QtdVidas     = 1;
                        }
                        catch(Exception ex)
                        {
                            if (ex.InnerException != null)
                            {
                                if (ex.InnerException.Message.Length <= 500)
                                    vo.ERRO = ex.InnerException.Message;
                                else
                                    vo.ERRO = ex.InnerException.Message.Substring(0, 499);
                            }
                            else
                            {
                                if (ex.Message.Length <= 500)
                                    vo.ERRO = ex.Message;
                                else
                                    vo.ERRO = ex.Message.Substring(0, 499);
                            }
                        }

                        lista.Add(vo);

                        #endregion
                    }

                    dr.Close();
                }

                if (!string.IsNullOrEmpty(qry2))
                {
                    cmd.CommandText = qry2;

                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        InstanciaSrcVO vo = null;

                        while (dr.Read())
                        {
                            #region qry2 

                            vo = new InstanciaSrcVO();

                            try
                            {
                                vo.BeneficiarioID = Convert.ToInt64(dr["beneficiario_id"]);
                                vo.BeneficiarioMAIL = CToString(dr["beneficiario_email"]);
                                vo.BeneficiarioNM = CToString(dr["beneficiario_nome"]);
                                vo.CobrancaID = -1;
                                vo.CobrancaPAGA = false;
                                vo.PropostaID = Convert.ToInt64(dr["proposta_id"]);

                                if (dr["contrato_qtdVidas"] != DBNull.Value)
                                    vo.QtdVidas = Convert.ToInt32(dr["contrato_qtdVidas"]);
                                else
                                    vo.QtdVidas = 1;
                            }
                            catch (Exception ex)
                            {
                                if (ex.InnerException != null)
                                {
                                    if (ex.InnerException.Message.Length <= 500)
                                        vo.ERRO = ex.InnerException.Message;
                                    else
                                        vo.ERRO = ex.InnerException.Message.Substring(0, 499);
                                }
                                else
                                {
                                    if (ex.Message.Length <= 500)
                                        vo.ERRO = ex.Message;
                                    else
                                        vo.ERRO = ex.Message.Substring(0, 499);
                                }
                            }

                            if (!existeContratoId(vo.PropostaID, lista)) lista.Add(vo);

                            #endregion
                        }

                        dr.Close();
                    }
                }
            }

            return lista;
        }

        bool existeContratoId(object contratoId, List<InstanciaSrcVO> lista)
        {
            if (lista == null || lista.Count == 0) return false;

            foreach (var vo in lista)
            {
                if (Convert.ToString(contratoId) == Convert.ToString(vo.PropostaID))
                    return true;
            }

            return false;
        }


        /*********************************************************************************/

        public long SalvarTexto(ConfigEmailTexto obj)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        if (obj.TemId)
                        {
                            sessao.Update(obj);
                        }
                        else
                        {
                            sessao.Save(obj);
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return obj.ID;
        }

        public ConfigEmailTexto CarregarTexto(long id)
        {
            using (var sessao = ObterSessao())
            {
                ConfigEmailTexto obj = sessao.Query<ConfigEmailTexto>()
                    .Where(t => t.ID == id).Single();

                return obj;
            }
        }

        public List<ConfigEmailTexto> CarregarTextos()
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<ConfigEmailTexto>()
                    .OrderBy(p => p.Descricao)
                    .ToList();
            }
        }
    }
}
/*
 
 select contrato_id,contratoadm_dtvc,beneficiario_id,beneficiario_nome,beneficiario_email
   from beneficiario 
       inner join contrato_beneficiario on beneficiario_id=contratobeneficiario_beneficiarioId and contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 
       inner join contrato on contrato_id = contratobeneficiario_contratoId and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1
	   inner join contratoadm on contratoadm_id = contrato_contratoadmid and contratoadm_dtvc is not null
       --left join config_email_instancia on cobranca_id = cei_cobrancaId and cei_configId=1 --and cei_tipo=", Convert.ToInt16(config.Tipo).ToString(), 
   where 
	   --contratoadm_dtvc=15 and
       contrato_tipoPessoa = 1 
	   and contrato_id not in (
		select cobranca_propostaid from cobranca where 
			month(cobranca_datavencimento)=4 and year(cobranca_datavencimento)=2017
			and cobranca_cancelada=0 ) and contrato_id=142881
		and datediff(day, getdate(), concat('2017-04-', contratoadm_dtvc)) = 23
	order by contrato_id
 
 
 
 
 */