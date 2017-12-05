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

    public class RelatorioFacade : FacadeBase
    {
        RelatorioFacade() { }

        #region singleton 

        static RelatorioFacade _instancia;
        public static RelatorioFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new RelatorioFacade(); }
                return _instancia;
            }
        }
        #endregion

        public List<AdimplenciaVO> cobrancasNaoGeradas(string associadoPjId, DateTime? de, DateTime? ate)
        {
            //string cond = string.Concat(" and contrato_estipulanteId = ", associadoPjId);

            string cond = "";

            if (associadoPjId != "0")
                cond = string.Concat(" and contrato_estipulanteId = ", associadoPjId);
            else
                cond = string.Concat(" and contrato_estipulanteId > ", associadoPjId);

            string qry = "";

            qry = string.Concat(
                "select contrato_id,contrato_numero,beneficiario_nome,contrato_numeroid,estipulante_descricao,contratoadm_descricao,beneficiario_razaoSocial, beneficiario_cpf ",
                "   from contrato ",
                "       inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                "       inner join beneficiario on contratobeneficiario_beneficiarioId = beneficiario_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                "       inner join estipulante on estipulante_id = contrato_estipulanteId ",
                "       inner join contratoADM on contratoadm_id = contrato_contratoAdmId ",
                "   where ",
                "       estipulante_ativo=1 and contrato_inativo=0 and contrato_cancelado=0 and contrato_rascunho=0 and contrato_data <= '", ate.Value.ToString("yyyy-MM-dd"), "' ",//de.Value.AddMonths(1).ToString("yyyy-MM-dd"), "' ",
                "       and contrato_id not in (select cobranca_propostaid from cobranca where cobranca_cancelada=0 and cobranca_dataVencimento between '", de.Value.ToString("yyyy-MM-dd"), "' and '", ate.Value.ToString("yyyy-MM-dd 23:59:59.998"), "') ",
                cond,
                "   order by estipulante_descricao,contratoadm_descricao,beneficiario_nome");

            List<AdimplenciaVO> vos = new List<AdimplenciaVO>();

            using (var sessao = ObterSessao())
            {
                IDbCommand cmd = sessao.Connection.CreateCommand();
                cmd.CommandText = qry;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                    while (dr.Read())
                    {
                        AdimplenciaVO vo = new AdimplenciaVO();

                        vo.AssociadoPJ = dr.GetString(4);
                        vo.BeneficiarioNome = dr.GetString(2);
                        vo.ContratoADM = dr.GetString(5);
                        vo.ContratoID = dr.GetInt64(0);
                        vo.ContratoNumero = dr.GetString(1);

                        if (dr["beneficiario_razaoSocial"] != null && dr["beneficiario_razaoSocial"] != DBNull.Value && Convert.ToString(dr["beneficiario_razaoSocial"]).Trim() != "")
                        {
                            vo.BeneficiarioNome = Convert.ToString(dr["beneficiario_razaoSocial"]).Trim();
                        }

                        //vo.CobrancaVencimento = Convert.ToDateTime(dr["cobranca_dataVencimento"], cinfo);

                        //if (dr["cobranca_qtdVidas"] != null && dr["cobranca_qtdVidas"] != DBNull.Value)
                        //    vo.CobrancaVidas = Convert.ToInt32(dr["cobranca_qtdVidas"]);

                        //if (vo.CobrancaVidas == 0) vo.CobrancaVidas = 1;

                        //vo.CobrancaValorPago = Convert.ToDecimal(dr["cobranca_valorPagto"], cinfo);

                        //if (dr["cobranca_dataPagto"] != DBNull.Value)
                        //    vo.CobrancaDataPago = Convert.ToDateTime(dr["cobranca_dataPagto"], cinfo);

                        //vo.CobrancaValorPendente = Convert.ToDecimal(dr["cobranca_valor"], cinfo);

                        vo.BeneficiarioDocumento = CToString(dr["beneficiario_cpf"]);
                        vo.EstipulanteNome = CToString(dr["estipulante_descricao"]);

                        vos.Add(vo);
                    }
                }
            }

            return vos;
        }

        List<AdimplenciaVO> adimplenciaInadimplencia(string associadoPjId, DateTime? de, DateTime? ate, bool adimplentes)
        {
            string cond = "";
            
            if(associadoPjId != "0")
                cond = string.Concat(" and contrato_estipulanteId = ", associadoPjId);
            else
                cond = string.Concat(" and contrato_estipulanteId > ", associadoPjId);

            string innerCond = "";

            string qry = "";

            if (adimplentes)
            {
                if (de.HasValue || ate.HasValue)
                {
                    if (de.HasValue)
                    {
                        innerCond = string.Concat(innerCond, " and cobranca_dataPagto >= '", de.Value.ToString("yyyy-MM-dd"), "' ");
                    }

                    if (ate.HasValue)
                    {
                        innerCond = string.Concat(innerCond, " and cobranca_dataPagto <= '", ate.Value.ToString("yyyy-MM-dd 23:59:59"), "' ");
                    }
                }

                qry = string.Concat(
                    "select contrato_id,contrato_numero,beneficiario_nome,contrato_numeroid,estipulante_descricao,contratoadm_descricao,beneficiario_razaoSocial,cobranca_dataVencimento,cobranca_qtdVidas,cobranca_valorPagto,cobranca_dataPagto,cobranca_valor,beneficiario_cpf, cobranca_parcela",
                    "   from contrato ",
                    "       inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                    "       inner join beneficiario on contratobeneficiario_beneficiarioId = beneficiario_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                    "       inner join estipulante on estipulante_id = contrato_estipulanteId ",
                    "       inner join contratoADM on contratoadm_id = contrato_contratoAdmId ",
                    "       inner join cobranca on cobranca_propostaId = contrato_id and cobranca_pago=1 ",
                    "   where  cobranca_pago=1 ", innerCond,

//                    "       estipulante_ativo=1 and contrato_inativo=0 and contrato_cancelado=0 and contrato_rascunho=0 ",
//                    "       and cobranca_cancelada=0 and cobranca_pago=1 ", innerCond, //and cobranca_dataVencimento > getdate()
////                  "       and contrato_id not in (select cobranca_propostaid from cobranca where cobranca_cancelada=0 and cobranca_pago=0 and cobranca_dataVencimento < getdate()", innerCond, ") ",
////                  "       and (select count(cobranca_id) from cobranca where cobranca_propostaId = contrato_id and cobranca_cancelada=0) > 0 ",

                    cond,
                    "   order by estipulante_descricao,contratoadm_descricao,beneficiario_nome,cobranca_datavencimento");
            }
            else
            {
                if (de.HasValue || ate.HasValue)
                {
                    if (de.HasValue)
                    {
                        innerCond = string.Concat(innerCond, " and cobranca_dataVencimento >= '", de.Value.ToString("yyyy-MM-dd"), "' ");
                    }

                    if (ate.HasValue)
                    {
                        innerCond = string.Concat(innerCond, " and cobranca_dataVencimento <= '", ate.Value.ToString("yyyy-MM-dd 23:59:59"), "' ");
                    }
                }

                qry = string.Concat(
                    "select contrato_id,contrato_numero,beneficiario_nome,contrato_numeroid,estipulante_descricao,contratoadm_descricao,beneficiario_razaoSocial,cobranca_dataVencimento,cobranca_qtdVidas,cobranca_valorPagto,cobranca_dataPagto,cobranca_valor, beneficiario_cpf, cobranca_parcela ",
                    "   from contrato ",
                    "       inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                    "       inner join beneficiario on contratobeneficiario_beneficiarioId = beneficiario_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                    "       inner join estipulante on estipulante_id = contrato_estipulanteId ",
                    "       inner join contratoADM on contratoadm_id = contrato_contratoAdmId ",
                    "       inner join cobranca on cobranca_propostaId = contrato_id and cobranca_pago=0  and cobranca_dataVencimento < getdate() ",
                    "   where ",
                    "       estipulante_ativo=1 and contrato_inativo=0 and contrato_cancelado=0 and contrato_rascunho=0 ",
                    "       and cobranca_cancelada=0 and cobranca_pago=0 ", innerCond, //and cobranca_dataVencimento < getdate()
//                  "       and contrato_id in (select cobranca_propostaid from cobranca where cobranca_cancelada=0 and cobranca_pago=0 and cobranca_dataVencimento < getdate()", innerCond, ") ",
//                  "       and (select count(cobranca_id) from cobranca where cobranca_propostaId = contrato_id and cobranca_cancelada=0) > 0 ",
                    cond,
                    "   order by estipulante_descricao,contratoadm_descricao,beneficiario_nome,cobranca_datavencimento");
            }

            List<AdimplenciaVO> vos = new List<AdimplenciaVO>();

            using (var sessao = ObterSessao())
            {
                IDbCommand cmd = sessao.Connection.CreateCommand();
                cmd.CommandText = qry;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                    while (dr.Read())
                    {
                        AdimplenciaVO vo = new AdimplenciaVO();

                        vo.AssociadoPJ = dr.GetString(4);
                        vo.BeneficiarioNome = dr.GetString(2);
                        vo.ContratoADM = dr.GetString(5);
                        vo.ContratoID = dr.GetInt64(0);
                        vo.ContratoNumero = dr.GetString(1);

                        vo.Parcela = CToInt(dr["cobranca_parcela"]);

                        if (dr["beneficiario_razaoSocial"] != null && dr["beneficiario_razaoSocial"] != DBNull.Value && Convert.ToString(dr["beneficiario_razaoSocial"]).Trim() != "" )
                        {
                            vo.BeneficiarioNome = Convert.ToString(dr["beneficiario_razaoSocial"]).Trim();
                        }

                        vo.CobrancaVencimento = Convert.ToDateTime(dr["cobranca_dataVencimento"], cinfo);

                        if(dr["cobranca_qtdVidas"] != null && dr["cobranca_qtdVidas"] != DBNull.Value)
                            vo.CobrancaVidas = Convert.ToInt32(dr["cobranca_qtdVidas"]);

                        if (vo.CobrancaVidas == 0) vo.CobrancaVidas = 1;

                        vo.CobrancaValorPago = Convert.ToDecimal(dr["cobranca_valorPagto"], cinfo);

                        if(dr["cobranca_dataPagto"] != DBNull.Value)
                            vo.CobrancaDataPago = Convert.ToDateTime(dr["cobranca_dataPagto"], cinfo);

                        vo.CobrancaValorPendente = Convert.ToDecimal(dr["cobranca_valor"], cinfo);

                        vo.BeneficiarioDocumento = CToString(dr["beneficiario_cpf"]);
                        vo.EstipulanteNome = CToString(dr["estipulante_descricao"]);

                        vos.Add(vo);
                    }
                }
            }

            return vos;
        }

        public List<AdimplenciaVO> RelatorioAdimplentes(string associadoPjId, DateTime? de, DateTime? ate)
        {
            return adimplenciaInadimplencia(associadoPjId, de, ate, true);
        }

        public List<AdimplenciaVO> RelatorioInadimplentes(string associadoPjId, DateTime? de, DateTime? ate)
        {
            return adimplenciaInadimplencia(associadoPjId, de, ate, false);
        }

        public List<AdimplenciaVO> RelatorioIUGU_PAGO(string associadoPjId, DateTime? de, DateTime? ate)
        {
            string innerCond = "";

            if (de.HasValue || ate.HasValue)
            {
                if (de.HasValue)
                {
                    innerCond = string.Concat(innerCond, " and cobranca_dataPagto >= '", de.Value.ToString("yyyy-MM-dd"), "' ");
                }

                if (ate.HasValue)
                {
                    innerCond = string.Concat(innerCond, " and cobranca_dataPagto <= '", ate.Value.ToString("yyyy-MM-dd 23:59:59"), "' ");
                }
            }

            if (!string.IsNullOrEmpty(associadoPjId))
            {
                innerCond = string.Concat(innerCond, " and contrato_estipulanteid=", associadoPjId);
            }

            string qry = string.Concat(
                    "select contrato_id,contrato_numero, beneficiario_nome,beneficiario_razaoSocial,beneficiario_cpf,estipulante_descricao,contratoadm_descricao,cobranca_parcela,produtoitemcobranca_qtvidas,cobranca_datavencimento,cobranca_dataPagto,cobranca_valor, cobranca_valorPagto,sum(produtoitemcobranca_produtoitemvalor * produtoitemcobranca_qtvidas) as totalProd ",
                    "   from contrato ",
                    "       inner JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=0 ",
                    "       inner JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                    "       inner join cobranca on cobranca_propostaid = contrato_id and cobranca_iugu_url is not null and cobranca_iugu_url <> '' ",
                    "       inner join operadora on operadora_id = contrato_operadoraid ",
                    "       inner join estipulante on estipulante_id = contrato_estipulanteid ",
                    "       inner join contratoadm on contratoadm_id = contrato_contratoadmid ",
                    "       left join produto_item_cobranca on cobranca_id = produtoitemcobranca_cobrancaid ",
                    "   where ",
                    "       contrato_utilizarIugu=1 and cobranca_iugu_id is not null and cobranca_iugu_id <> '' and cobranca_pago=1 ", innerCond,
                    "   group by contrato_id,contrato_numero, beneficiario_nome,beneficiario_razaoSocial,beneficiario_cpf,estipulante_descricao,contratoadm_descricao,cobranca_parcela,produtoitemcobranca_qtvidas,cobranca_datavencimento,cobranca_dataPagto,cobranca_valor,cobranca_valorPagto,cobranca_dataPagto ",
                    "   order by beneficiario_nome,cobranca_dataPagto ");

            List<AdimplenciaVO> vos = new List<AdimplenciaVO>();

            using (var sessao = ObterSessao())
            {
                IDbCommand cmd = sessao.Connection.CreateCommand();
                cmd.CommandText = qry;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                    while (dr.Read())
                    {
                        AdimplenciaVO vo = new AdimplenciaVO();

                        vo.BeneficiarioNome = CToString(dr["beneficiario_nome"]);

                        //////////////////////////////////////
                        vo.ContratoCNPJ = CToString(dr["beneficiario_cpf"]);
                        vo.AssociadoPJ = CToString(dr["estipulante_descricao"]);
                        vo.ContratoADM = CToString(dr["contratoadm_descricao"]);
                        vo.Parcela = CToInt(dr["cobranca_parcela"]);
                        vo.CobrancaVidas = CToInt(dr["produtoitemcobranca_qtvidas"]);
                        //////////////////////////////////////

                        vo.ContratoID = dr.GetInt64(0);
                        vo.ContratoNumero = CToString(dr["contrato_numero"]);

                        if (dr["beneficiario_razaoSocial"] != null && dr["beneficiario_razaoSocial"] != DBNull.Value && Convert.ToString(dr["beneficiario_razaoSocial"]).Trim() != "")
                        {
                            vo.BeneficiarioNome = Convert.ToString(dr["beneficiario_razaoSocial"]).Trim();
                        }

                        vo.CobrancaVencimento = Convert.ToDateTime(dr["cobranca_dataVencimento"], cinfo);

                        if (dr["cobranca_dataPagto"] != DBNull.Value)
                        {
                            vo.CobrancaDataPago = Convert.ToDateTime(dr["cobranca_dataPagto"], cinfo);
                            vo.DataAExibir = vo.CobrancaDataPago.ToString("dd/MM/yyyy");
                        }

                        vo.CobrancaValorPago = Convert.ToDecimal(dr["cobranca_valorPagto"], cinfo);
                        vo.CobrancaValorPendente = Convert.ToDecimal(dr["cobranca_valor"], cinfo);
                        vo.TotalProdutoValor = Convert.ToDecimal(dr["totalProd"], cinfo);

                        vos.Add(vo);
                    }
                }
            }

            return vos;
        }

        public List<AdimplenciaVO> RelatorioIUGU_PENDENTE(string associadoId, DateTime? de, DateTime? ate)
        {
            string innerCond = "";

            if (de.HasValue || ate.HasValue)
            {
                if (de.HasValue)
                {
                    innerCond = string.Concat(innerCond, " and cobranca_dataVencimento >= '", de.Value.ToString("yyyy-MM-dd"), "' ");
                }

                if (ate.HasValue)
                {
                    innerCond = string.Concat(innerCond, " and cobranca_dataVencimento <= '", ate.Value.ToString("yyyy-MM-dd 23:59:59"), "' ");
                }
            }

            innerCond = string.Concat(innerCond, " and cobranca_dataVencimento < getdate() ");

            if (!string.IsNullOrEmpty(associadoId))
            {
                innerCond = string.Concat(innerCond, " and contrato_estipulanteid=", associadoId);
            }

            string qry = string.Concat(
                    "select contrato_id,contrato_numero, beneficiario_nome,beneficiario_razaoSocial,beneficiario_cpf,estipulante_descricao,contratoadm_descricao,cobranca_parcela,produtoitemcobranca_qtvidas,cobranca_datavencimento,cobranca_dataPagto,cobranca_valor, cobranca_valorPagto,sum(produtoitemcobranca_produtoitemvalor * produtoitemcobranca_qtvidas) as totalProd ",
                    "   from contrato ",
                    "       inner JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=0 ",
                    "       inner JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                    "       inner join cobranca on cobranca_propostaid = contrato_id and cobranca_iugu_url is not null and cobranca_iugu_url <> '' ",
                    "       inner join operadora on operadora_id = contrato_operadoraid ",
                    "       inner join estipulante on estipulante_id = contrato_estipulanteid ",
                    "       inner join contratoadm on contratoadm_id = contrato_contratoadmid ",
                    "       left join produto_item_cobranca on cobranca_id = produtoitemcobranca_cobrancaid ",
                    "   where ",
                    "       contrato_utilizarIugu=1 and cobranca_iugu_id is not null and cobranca_iugu_id <> '' and cobranca_pago=0 and cobranca_cancelada=0 ", innerCond,
                    "   group by contrato_id,contrato_numero, beneficiario_nome,beneficiario_razaoSocial,beneficiario_cpf,estipulante_descricao,contratoadm_descricao,cobranca_parcela,produtoitemcobranca_qtvidas,cobranca_datavencimento,cobranca_dataPagto,cobranca_valor,cobranca_valorPagto,cobranca_dataPagto ",
                    "   order by beneficiario_nome,cobranca_datavencimento ");

            List<AdimplenciaVO> vos = new List<AdimplenciaVO>();

            using (var sessao = ObterSessao())
            {
                IDbCommand cmd = sessao.Connection.CreateCommand();
                cmd.CommandText = qry;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                    while (dr.Read())
                    {
                        AdimplenciaVO vo = new AdimplenciaVO();

                        vo.ContratoNumero = CToString(dr["contrato_numero"]);

                        //////////////////////////////////////
                        vo.ContratoCNPJ     = CToString(dr["beneficiario_cpf"]);
                        vo.AssociadoPJ      = CToString(dr["estipulante_descricao"]);
                        vo.ContratoADM      = CToString(dr["contratoadm_descricao"]);
                        vo.Parcela          = CToInt(dr["cobranca_parcela"]);
                        vo.CobrancaVidas    = CToInt(dr["produtoitemcobranca_qtvidas"]);
                        //////////////////////////////////////

                        vo.BeneficiarioNome = CToString(dr["beneficiario_nome"]);
                        vo.ContratoID = dr.GetInt64(0);

                        if (dr["beneficiario_razaoSocial"] != null && dr["beneficiario_razaoSocial"] != DBNull.Value && Convert.ToString(dr["beneficiario_razaoSocial"]).Trim() != "")
                        {
                            vo.BeneficiarioNome = Convert.ToString(dr["beneficiario_razaoSocial"]).Trim();
                        }

                        vo.CobrancaVencimento = Convert.ToDateTime(dr["cobranca_dataVencimento"], cinfo);
                        vo.DataAExibir = vo.CobrancaVencimento.ToString("dd/MM/yyyy");

                        if (dr["cobranca_dataPagto"] != DBNull.Value)
                            vo.CobrancaDataPago = Convert.ToDateTime(dr["cobranca_dataPagto"], cinfo);

                        vo.CobrancaValorPago = Convert.ToDecimal(dr["cobranca_valorPagto"], cinfo);
                        vo.CobrancaValorPendente = Convert.ToDecimal(dr["cobranca_valor"], cinfo);
                        vo.TotalProdutoValor = Convert.ToDecimal(dr["totalProd"], cinfo);

                        vos.Add(vo);
                    }
                }
            }

            return vos;
        }

        public List<AdimplenciaVO> RelatorioIUGU_GERADAS(string associadoId, DateTime? de, DateTime? ate)
        {
            string innerCond = "";

            if (de.HasValue || ate.HasValue)
            {
                if (de.HasValue)
                {
                    innerCond = string.Concat(innerCond, " and cobranca_dataCriacao >= '", de.Value.ToString("yyyy-MM-dd"), "' ");
                }

                if (ate.HasValue)
                {
                    innerCond = string.Concat(innerCond, " and cobranca_dataCriacao <= '", ate.Value.ToString("yyyy-MM-dd 23:59:59"), "' ");
                }
            }

            if (!string.IsNullOrEmpty(associadoId))
            {
                innerCond = string.Concat(innerCond, " and contrato_estipulanteid=", associadoId);
            }

            string qry = string.Concat(
                    "select contrato_id,contrato_numero, beneficiario_nome,beneficiario_razaoSocial,beneficiario_cpf,estipulante_descricao,contratoadm_descricao,cobranca_parcela,produtoitemcobranca_qtvidas,cobranca_datavencimento,cobranca_dataPagto,cobranca_valor, cobranca_valorPagto,sum(produtoitemcobranca_produtoitemvalor * produtoitemcobranca_qtvidas) as totalProd ",
                    "   from contrato ",
                    "       inner JOIN contrato_beneficiario ON contrato_id=contratobeneficiario_contratoId AND contratobeneficiario_ativo=1 AND contratobeneficiario_tipo=0 ",
                    "       inner JOIN beneficiario ON beneficiario_id=contratobeneficiario_beneficiarioId AND contratobeneficiario_ativo=1 ",
                    "       inner join cobranca on cobranca_propostaid = contrato_id and cobranca_iugu_url is not null and cobranca_iugu_url <> '' ",
                    "       inner join operadora on operadora_id = contrato_operadoraid ",
                    "       inner join estipulante on estipulante_id = contrato_estipulanteid ",
                    "       inner join contratoadm on contratoadm_id = contrato_contratoadmid ",
                    "       left join produto_item_cobranca on cobranca_id = produtoitemcobranca_cobrancaid ",
                    "   where ",
                    "       contrato_utilizarIugu=1 and cobranca_iugu_id is not null and cobranca_iugu_id <> '' and cobranca_pago=0 and cobranca_cancelada=0 ", innerCond,
                    "   group by contrato_id,contrato_numero, beneficiario_nome,beneficiario_razaoSocial,beneficiario_cpf,estipulante_descricao,contratoadm_descricao,cobranca_parcela,produtoitemcobranca_qtvidas,cobranca_datavencimento,cobranca_dataPagto,cobranca_valor,cobranca_valorPagto,cobranca_dataPagto ",
                    "   order by beneficiario_nome,cobranca_datavencimento ");

            List<AdimplenciaVO> vos = new List<AdimplenciaVO>();

            using (var sessao = ObterSessao())
            {
                IDbCommand cmd = sessao.Connection.CreateCommand();
                cmd.CommandText = qry;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                    while (dr.Read())
                    {
                        AdimplenciaVO vo = new AdimplenciaVO();

                        vo.BeneficiarioNome = CToString(dr["beneficiario_nome"]);

                        //////////////////////////////////////
                        vo.ContratoCNPJ = CToString(dr["beneficiario_cpf"]);
                        vo.AssociadoPJ = CToString(dr["estipulante_descricao"]);
                        vo.ContratoADM = CToString(dr["contratoadm_descricao"]);
                        vo.Parcela = CToInt(dr["cobranca_parcela"]);
                        vo.CobrancaVidas = CToInt(dr["produtoitemcobranca_qtvidas"]);
                        //////////////////////////////////////

                        vo.ContratoID = dr.GetInt64(0);
                        vo.ContratoNumero = CToString(dr["contrato_numero"]);

                        if (dr["beneficiario_razaoSocial"] != null && dr["beneficiario_razaoSocial"] != DBNull.Value && Convert.ToString(dr["beneficiario_razaoSocial"]).Trim() != "")
                        {
                            vo.BeneficiarioNome = Convert.ToString(dr["beneficiario_razaoSocial"]).Trim();
                        }

                        vo.CobrancaVencimento = Convert.ToDateTime(dr["cobranca_dataVencimento"], cinfo);
                        vo.DataAExibir = vo.CobrancaVencimento.ToString("dd/MM/yyyy");

                        if (dr["cobranca_dataPagto"] != DBNull.Value)
                            vo.CobrancaDataPago = Convert.ToDateTime(dr["cobranca_dataPagto"], cinfo);

                        vo.CobrancaValorPago = Convert.ToDecimal(dr["cobranca_valorPagto"], cinfo);
                        vo.CobrancaValorPendente = Convert.ToDecimal(dr["cobranca_valor"], cinfo);
                        vo.TotalProdutoValor = Convert.ToDecimal(dr["totalProd"], cinfo);

                        vos.Add(vo);
                    }
                }
            }

            return vos;
        }

        public List<AdimplenciaVO> cobrancasNaoGeradas_IUGU(string associadoPjId, DateTime? de, DateTime? ate)
        {
            string cond = "";

            if (associadoPjId != "")
                cond = string.Concat(" and contrato_estipulanteId = ", associadoPjId);
            else
                cond = string.Concat(" and contrato_estipulanteId > 0 ");

            string qry = "";

            qry = string.Concat(
                "select contrato_id,contrato_numero,beneficiario_nome,contrato_numeroid,estipulante_descricao,contratoadm_descricao,beneficiario_razaoSocial, beneficiario_cpf ",
                "   from contrato ",
                "       inner join contrato_beneficiario on contratobeneficiario_contratoId = contrato_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                "       inner join beneficiario on contratobeneficiario_beneficiarioId = beneficiario_id and contratobeneficiario_tipo=0 and contratobeneficiario_ativo=1 ",
                "       inner join estipulante on estipulante_id = contrato_estipulanteId ",
                "       inner join contratoADM on contratoadm_id = contrato_contratoAdmId ",
                "   where ",
                "       contrato_utilizarIugu=1 and estipulante_ativo=1 and contrato_inativo=0 and contrato_cancelado=0 and contrato_rascunho=0 and contrato_data <= '", ate.Value.ToString("yyyy-MM-dd"), "' ",//de.Value.AddMonths(1).ToString("yyyy-MM-dd"), "' ",
                "       and contrato_id not in (select cobranca_propostaid from cobranca where cobranca_cancelada=0 and cobranca_dataVencimento between '", de.Value.ToString("yyyy-MM-dd"), "' and '", ate.Value.ToString("yyyy-MM-dd 23:59:59.998"), "') ",
                cond,
                "   order by estipulante_descricao,contratoadm_descricao,beneficiario_nome");

            List<AdimplenciaVO> vos = new List<AdimplenciaVO>();

            using (var sessao = ObterSessao())
            {
                IDbCommand cmd = sessao.Connection.CreateCommand();
                cmd.CommandText = qry;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");

                    while (dr.Read())
                    {
                        AdimplenciaVO vo = new AdimplenciaVO();

                        vo.AssociadoPJ = dr.GetString(4);
                        vo.BeneficiarioNome = dr.GetString(2);
                        vo.ContratoADM = dr.GetString(5);
                        vo.ContratoID = dr.GetInt64(0);
                        vo.ContratoNumero = dr.GetString(1);

                        if (dr["beneficiario_razaoSocial"] != null && dr["beneficiario_razaoSocial"] != DBNull.Value && Convert.ToString(dr["beneficiario_razaoSocial"]).Trim() != "")
                        {
                            vo.BeneficiarioNome = Convert.ToString(dr["beneficiario_razaoSocial"]).Trim();
                        }

                        //vo.CobrancaVencimento = Convert.ToDateTime(dr["cobranca_dataVencimento"], cinfo);

                        //if (dr["cobranca_qtdVidas"] != null && dr["cobranca_qtdVidas"] != DBNull.Value)
                        //    vo.CobrancaVidas = Convert.ToInt32(dr["cobranca_qtdVidas"]);

                        //if (vo.CobrancaVidas == 0) vo.CobrancaVidas = 1;

                        //vo.CobrancaValorPago = Convert.ToDecimal(dr["cobranca_valorPagto"], cinfo);

                        //if (dr["cobranca_dataPagto"] != DBNull.Value)
                        //    vo.CobrancaDataPago = Convert.ToDateTime(dr["cobranca_dataPagto"], cinfo);

                        //vo.CobrancaValorPendente = Convert.ToDecimal(dr["cobranca_valor"], cinfo);

                        vo.BeneficiarioDocumento = CToString(dr["beneficiario_cpf"]);
                        vo.EstipulanteNome = CToString(dr["estipulante_descricao"]);

                        vos.Add(vo);
                    }
                }
            }

            return vos;
        }

        [Serializable]
        public class AdimplenciaVO
        {
            public long ContratoID { get; set; }
            public string ContratoNumero { get; set; }
            public string ContratoCNPJ { get; set; }
            public string BeneficiarioNome { get; set; }
            public string BeneficiarioDocumento { get; set; }
            public string AssociadoPJ { get; set; }
            public string ContratoADM { get; set; }
            public string EstipulanteNome { get; set; }
            public int Parcela { get; set; }

            //Adimplentes
            public DateTime CobrancaVencimento { get; set; }
            public int CobrancaVidas { get; set; }
            public decimal CobrancaValorPago { get; set; }
            public DateTime CobrancaDataPago { get; set; }

            //Inadimplentes
            public decimal CobrancaValorPendente { get; set; }

            //IUGU
            public decimal TotalProdutoValor { get; set; }
            public decimal TotalCoberturaValor 
            {
                get 
                {
                    if(CobrancaValorPendente > TotalProdutoValor)
                        return CobrancaValorPendente - TotalProdutoValor; 
                    else
                        return TotalProdutoValor - CobrancaValorPendente; 
                }
            }
            public string DataAExibir { get; set; }
        }
    }
}
