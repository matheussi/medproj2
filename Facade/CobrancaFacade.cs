namespace LC.Web.PadraoSeguros.Facade
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Data;
    using MedProj.Entidades;
    using System.Configuration;
    using System.Collections.Generic;

    using NHibernate;
    using NHibernate.Linq;
    using MedProj.Entidades.Enuns;
    using BoletoNet;
    using System.IO;

    public class CobrancaFacade : FacadeBase
    {
        CobrancaFacade() { }

        internal class CobrancaMovCNAB_VO
        {
            public string CobrancaId { get; set; }
            public string MovimentacaoCNABId { get; set; }
        }

        #region singleton 

        static CobrancaFacade _instancia;
        public static CobrancaFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new CobrancaFacade(); }
                return _instancia;
            }
        }
        #endregion


        Cobranca fillFrom(IDataReader dr)
        {
            Cobranca c = new Cobranca();

            c.Contrato = new Contrato();
            c.Contrato.ContratoBeneficiario = new ContratoBeneficiario();
            c.Contrato.ContratoBeneficiario.Beneficiario = new Beneficiario();

            c.Contrato.ContratoBeneficiario.Beneficiario.ID = dr.GetInt64(0);
            c.Contrato.ContratoBeneficiario.Beneficiario.Nome = dr.GetString(1);
            c.Contrato.ID = dr.GetInt64(2);
            c.ID = Convert.ToInt64(dr[3]);
            c.DataCriacao = dr.GetDateTime(4);
            c.DataVencimento = dr.GetDateTime(5);
            c.ValorCobrado = dr.GetDecimal(6);

            return c;
        }

        public List<Cobranca> CarregarPendenciasCNAB(TipoFiltroPendenciaCNAB tipo, DateTime? criacaoDe, DateTime? criacaoAte)
        {
            List<Cobranca> lista = new List<Cobranca>();

            string qry = string.Concat(
                "select beneficiario_id,beneficiario_nome, contrato_id, cobranca_id,cobranca_datacriacao,cobranca_datavencimento,cobranca_valor ",
                "   from cobranca ",
                "       inner join contrato on contrato_id = cobranca_propostaId ",
                "       inner join contrato_beneficiario on contratobeneficiario_contratoid = contrato_id ",
                "       inner join beneficiario on beneficiario_id = contratobeneficiario_beneficiarioid ",
                "   where ",
                "       cobranca_cancelada=0 and cobranca_pago = 0 and cobranca_arquivoUltimoEnvioId is null ");

            using (var contexto = ObterSessao())
            {
                IDbCommand cmd = contexto.Connection.CreateCommand();

                if (tipo == TipoFiltroPendenciaCNAB.Hoje)
                {
                    qry += string.Concat(
                        " and day(cobranca_datacriacao) = ", DateTime.Now.Day, 
                        " and month(cobranca_datacriacao) = ", DateTime.Now.Month, 
                        " and year(cobranca_datacriacao) = ", DateTime.Now.Year,
                        " order by cobranca_datacriacao");

                    //lista = contexto.Query<Cobranca>()
                    //    .Fetch(c => c.Contrato)
                    //    .ThenFetch(cn => cn.ContratoBeneficiario)
                    //    .ThenFetch(cb => cb.Beneficiario)
                    //    .Where(c => c.Cancelada == false && c.Pago == false && !c.ArquivoEnviadoId.HasValue && c.DataCriacao.Day == DateTime.Now.Day && c.DataCriacao.Month == DateTime.Now.Month && c.DataCriacao.Year == DateTime.Now.Year)
                    //    .OrderBy(c => c.DataCriacao)
                    //    .ToList();
                }
                else if (tipo == TipoFiltroPendenciaCNAB.Ontem)
                {
                    DateTime data = DateTime.Now.AddDays(-1);

                    qry += string.Concat(
                        " and day(cobranca_datacriacao) = ", data.Day,
                        " and month(cobranca_datacriacao) = ", data.Month,
                        " and year(cobranca_datacriacao) = ", data.Year,
                        " order by cobranca_datacriacao");

                    //lista = contexto.Query<Cobranca>()
                    //    .Fetch(c => c.Contrato)
                    //    .ThenFetch(cn => cn.ContratoBeneficiario)
                    //    .ThenFetch(cb => cb.Beneficiario)
                    //    .Where(c => c.Cancelada == false && c.Pago == false && !c.ArquivoEnviadoId.HasValue && c.DataCriacao.Day == data.Day && c.DataCriacao.Month == data.Month && c.DataCriacao.Year == data.Year)
                    //    .OrderBy(c => c.DataCriacao)
                    //    .ToList();
                }
                else if (criacaoDe.HasValue && criacaoAte.HasValue && criacaoDe <= criacaoAte)
                {
                    qry += string.Concat(
                        " and cobranca_datacriacao >= '", criacaoDe.Value.ToString("yyyy-MM-dd"), "' ",
                        " and cobranca_datacriacao <= '", criacaoAte.Value.ToString("yyyy-MM-dd 23:59:59:995"), "' ",
                        " order by cobranca_datacriacao");

                    //lista = contexto.Query<Cobranca>()
                    //    .Fetch(c => c.Contrato)
                    //    .ThenFetch(cn => cn.ContratoBeneficiario)
                    //    .ThenFetch(cb => cb.Beneficiario)
                    //    .Where(c => c.Cancelada == false && c.Pago == false && !c.ArquivoEnviadoId.HasValue && c.DataCriacao >= criacaoDe && c.DataCriacao <= criacaoAte)
                    //    .OrderBy(c => c.DataCriacao)
                    //    .ToList();
                }

                cmd.CommandText = qry;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(this.fillFrom(dr));
                    }
                }

                return lista;
            }
        }

        public List<Cobranca> CarregarPendenciasCNAB_Alteracoes(TipoFiltroPendenciaCNAB tipo, DateTime? criacaoDe, DateTime? criacaoAte)
        {
            List<Cobranca> lista = new List<Cobranca>();

            string qry = string.Concat(
                "select beneficiario_id,beneficiario_nome, contrato_id, cobranca_id,cobranca_datacriacao,cobranca_datavencimento,cobranca_valor ",
                "   from cobranca ",
                "       inner join contrato on contrato_id = cobranca_propostaId ",
                "       inner join contrato_beneficiario on contratobeneficiario_contratoid = contrato_id ",
                "       inner join beneficiario on beneficiario_id = contratobeneficiario_beneficiarioid ",
                "       inner join cobranca_pendenciaCnab on pendenciacnab_cobrancaid = cobranca_id and pendenciacnab_processado=0 ",
                "   where ",
                "       cobranca_cancelada=0 and cobranca_pago=0 and pendenciacnab_processado=0 ");

            using (var contexto = ObterSessao())
            {
                IDbCommand cmd = contexto.Connection.CreateCommand();

                if (tipo == TipoFiltroPendenciaCNAB.Hoje)
                {
                    qry += string.Concat(
                        " and day(pendenciacnab_data) = ", DateTime.Now.Day,
                        " and month(pendenciacnab_data) = ", DateTime.Now.Month,
                        " and year(pendenciacnab_data) = ", DateTime.Now.Year,
                        " order by pendenciacnab_data");
                }
                else if (tipo == TipoFiltroPendenciaCNAB.Ontem)
                {
                    DateTime data = DateTime.Now.AddDays(-1);

                    qry += string.Concat(
                        " and day(pendenciacnab_data) = ", data.Day,
                        " and month(pendenciacnab_data) = ", data.Month,
                        " and year(pendenciacnab_data) = ", data.Year,
                        " order by pendenciacnab_data");
                }
                else if (criacaoDe.HasValue && criacaoAte.HasValue && criacaoDe <= criacaoAte)
                {
                    qry += string.Concat(
                        " and pendenciacnab_data >= '", criacaoDe.Value.ToString("yyyy-MM-dd"), "' ",
                        " and pendenciacnab_data <= '", criacaoAte.Value.ToString("yyyy-MM-dd 23:59:59:995"), "' ",
                        " order by pendenciacnab_data");
                }

                cmd.CommandText = qry;

                using (IDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(this.fillFrom(dr));
                    }
                }

                return lista;
            }
        }

        /// <summary>
        /// Calcula a data de validade para pagamento do boleto, a partir de sua criação. 
        /// Margem para registro do boleto no banco.
        /// </summary>
        public DateTime calculaValidadeBoleto(DateTime dataCriacao)
        {
            DateTime dataValidade = new DateTime(
                dataCriacao.AddDays(1).Year,
                dataCriacao.AddDays(1).Month,
                dataCriacao.AddDays(1).Day);

            if (dataCriacao.Hour >= 18)
            {
                dataValidade = new DateTime(
                    dataCriacao.AddDays(2).Year,
                    dataCriacao.AddDays(2).Month,
                    dataCriacao.AddDays(2).Day);
            }

            return dataValidade;
        }

        /// <summary>
        /// O vencimento não pode ser menor que a data de validade para pagto do boleto.
        /// </summary>
        public DateTime calculaVencimento(DateTime dataCriacao, DateTime dataVencimento)
        {
            DateTime dataValidade = calculaValidadeBoleto(dataCriacao);

            if (dataVencimento < dataValidade)
            {
                dataVencimento = new DateTime(
                    dataValidade.AddDays(1).Year,
                    dataValidade.AddDays(1).Month,
                    dataValidade.AddDays(1).Day,
                    23, 59, 59, 996);
            }

            return dataVencimento;
        }

        void addInstruecoesParaArquivoCNAB(ref Boleto b1, DateTime vencimento, DateTime dataCriacao)
        {
            Instrucao_Itau item1 = new Instrucao_Itau(5); //5=instrucoes do titulo ; 10= nao protestar
            b1.Instrucoes.Add(item1);

            Instrucao_Itau item2 = new Instrucao_Itau(10); //5=instrucoes do titulo ; 10= nao protestar
            b1.Instrucoes.Add(item2);

            //Instrucao_Itau item2 = new Instrucao_Itau();
            //item2.Descricao = "Não receber apos " + vencimento.AddMonths(3).ToString("dd/MM/yyyy");
            //b1.Instrucoes.Add(item2);

            //Instrucao_Itau item3 = new Instrucao_Itau();
            //item3.Descricao = "Boleto valido a partir de  " + calculaValidadeBoleto(dataCriacao).ToString("dd/MM/yyyy");
            //b1.Instrucoes.Add(item3);
        }

        Cobranca fullFillFrom(IDataReader dr)
        {
            Cobranca c = new Cobranca();

            c.Contrato = new Contrato();
            c.Contrato.ContratoBeneficiario = new ContratoBeneficiario();
            c.Contrato.EnderecoReferencia = new MedProj.Entidades.Endereco();
            c.Contrato.ContratoBeneficiario.Beneficiario = new Beneficiario();

            c.ID                = Convert.ToInt64(dr["cobranca_id"]);
            c.DataCriacao       = Convert.ToDateTime(dr["cobranca_dataCriacao"]);
            c.DataVencimento    = Convert.ToDateTime(dr["cobranca_dataVencimento"]);
            c.ValorCobrado      = Convert.ToDecimal(dr["cobranca_valor"]);

            c.Contrato.ContratoBeneficiario.Beneficiario.ID   = Convert.ToInt64(dr["beneficiario_id"]);
            c.Contrato.ContratoBeneficiario.Beneficiario.Nome = Convert.ToString(dr["beneficiario_nome"]);
            c.Contrato.ContratoBeneficiario.Beneficiario.CPF  = Convert.ToString(dr["beneficiario_cpf"]);

            c.Contrato.ID = Convert.ToInt64(dr["contrato_id"]);

            c.Contrato.EnderecoReferencia.ID            = Convert.ToInt64(dr["endereco_id"]);
            c.Contrato.EnderecoReferencia.Bairro        = Convert.ToString(dr["endereco_bairro"]);
            c.Contrato.EnderecoReferencia.CEP           = Convert.ToString(dr["endereco_cep"]);
            c.Contrato.EnderecoReferencia.Cidade        = Convert.ToString(dr["endereco_cidade"]);
            c.Contrato.EnderecoReferencia.Complemento   = Convert.ToString(dr["endereco_complemento"]);
            c.Contrato.EnderecoReferencia.DonoId        = Convert.ToInt64(dr["endereco_donoId"]);
            c.Contrato.EnderecoReferencia.Logradouro    = Convert.ToString(dr["endereco_logradouro"]);
            c.Contrato.EnderecoReferencia.Numero        = Convert.ToString(dr["endereco_numero"]);
            c.Contrato.EnderecoReferencia.Tipo          = Convert.ToInt32(dr["endereco_tipo"]);
            c.Contrato.EnderecoReferencia.UF            = Convert.ToString(dr["endereco_uf"]);

            //if (alteracaoCNAB)
            //{
            //    c.
            //}


            return c;
        }

        public string GerarArquivoCNAB(string[] cobrancaIds, bool alteracao = false)
        {
            List<Cobranca> lista = new List<Cobranca>();
            List<CobrancaMovCNAB_VO> vos = new List<CobrancaMovCNAB_VO>();

            string nomeArquivo = string.Concat(
                ConfigurationManager.AppSettings["appRemessaCaminhoFisico"],
                DateTime.Now.ToString("yyyyMMddHHmmss"), ".txt");

            string qry = "";

            if (alteracao)
            {
                qry = string.Concat(
                    "select * ",
                    "   from cobranca ",
                    "       inner join contrato on contrato_id = cobranca_propostaId ",
                    "       inner join contrato_beneficiario on contratobeneficiario_contratoid = contrato_id ",
                    "       inner join beneficiario on beneficiario_id = contratobeneficiario_beneficiarioid ",
                    "       inner join endereco on endereco_donoId = beneficiario_id and endereco_donoTipo = ", Convert.ToInt32(TipoDono.Beneficiario),
                    "       inner join cobranca_pendenciaCnab on cobranca_id= pendenciacnab_cobrancaid and pendenciacnab_processado = 0", 
                    "   where ",
                    "       cobranca_id in (", String.Join(",", cobrancaIds), ")",
                    "   order by contrato_id,cobranca_datacriacao");
            }
            else
            {
                qry = string.Concat(
                    "select * ",
                    "   from cobranca ",
                    "       inner join contrato on contrato_id = cobranca_propostaId ",
                    "       inner join contrato_beneficiario on contratobeneficiario_contratoid = contrato_id ",
                    "       inner join beneficiario on beneficiario_id = contratobeneficiario_beneficiarioid ",
                    "       inner join endereco on endereco_donoId = beneficiario_id and endereco_donoTipo = ", Convert.ToInt32(TipoDono.Beneficiario),
                    "   where ",
                    "       cobranca_id in (", String.Join(",", cobrancaIds), ")",
                    "   order by contrato_id,cobranca_datacriacao");
            }
            using (var contexto = ObterSessao())
            {
                //lista = contexto.Query<Cobranca>()
                //        .Fetch(co => co.Contrato)
                //        .ThenFetch(cn => cn.ContratoBeneficiario)
                //        .ThenFetch(cb => cb.Beneficiario)
                //        .Where(co => cobrancaIds.Contains(co.ID.ToString()))
                //        .OrderBy(co => co.Contrato.ID)
                //        .ToList();

                using (ITransaction tran = contexto.BeginTransaction())
                {
                    IDbCommand cmd = contexto.Connection.CreateCommand();
                    tran.Enlist(cmd);
                    cmd.CommandText = qry;

                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(this.fullFillFrom(dr));

                            if (alteracao)
                            {
                                vos.Add(new CobrancaMovCNAB_VO
                                {
                                    CobrancaId = Convert.ToString(dr["cobranca_id"]),
                                    MovimentacaoCNABId = Convert.ToString(dr["pendenciacnab_id"])
                                });
                            }
                        }

                        dr.Close();
                        dr.Dispose();
                    }

                    #region selects 

                    if (lista.Count == 0)
                    {
                        //pode nao ter achado devido a nao ter endereço do beneficiario
                        //tenta pelo endereco cadastrado o contrato, primeiro de cobranca
                        qry = string.Concat(
                            "select * ",
                            "   from cobranca ",
                            "       inner join contrato on contrato_id = cobranca_propostaId ",
                            "       inner join contrato_beneficiario on contratobeneficiario_contratoid = contrato_id ",
                            "       inner join beneficiario on beneficiario_id = contratobeneficiario_beneficiarioid ",
                            "       inner join endereco on endereco_id = contrato_enderecoCobrancaId ",
                            "   where ",
                            "       cobranca_id in (", String.Join(",", cobrancaIds), ")",
                            "   order by contrato_id,cobranca_datacriacao");

                        cmd.CommandText = qry;

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                lista.Add(this.fullFillFrom(dr));

                                if (alteracao)
                                {
                                    vos.Add(new CobrancaMovCNAB_VO
                                    {
                                        CobrancaId = Convert.ToString(dr["cobranca_id"]),
                                        MovimentacaoCNABId = Convert.ToString(dr["pendenciacnab_id"])
                                    });
                                }
                            }

                            dr.Close();
                            dr.Dispose();
                        }

                        if (lista.Count == 0)
                        {
                            //pode nao ter achado devido a nao ter endereço do beneficiario
                            //tenta pelo endereco cadastrado o contrato, agora pelo de referencia
                            qry = string.Concat(
                                "select * ",
                                "   from cobranca ",
                                "       inner join contrato on contrato_id = cobranca_propostaId ",
                                "       inner join contrato_beneficiario on contratobeneficiario_contratoid = contrato_id ",
                                "       inner join beneficiario on beneficiario_id = contratobeneficiario_beneficiarioid ",
                                "       inner join endereco on endereco_id = contrato_enderecoReferenciaId ",
                                "   where ",
                                "       cobranca_id in (", String.Join(",", cobrancaIds), ")",
                                "   order by contrato_id,cobranca_datacriacao");

                            cmd.CommandText = qry;

                            using (IDataReader dr = cmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    lista.Add(this.fullFillFrom(dr));

                                    if (alteracao)
                                    {
                                        vos.Add(new CobrancaMovCNAB_VO
                                        {
                                            CobrancaId = Convert.ToString(dr["cobranca_id"]),
                                            MovimentacaoCNABId = Convert.ToString(dr["pendenciacnab_id"])
                                        });
                                    }
                                }

                                dr.Close();
                                dr.Dispose();
                            }
                        }
                    }
                    #endregion

                    if (lista.Count == 0)
                    {
                        tran.Rollback();
                        cmd.Dispose();
                        contexto.Close();
                        return null;
                    }

                    try
                    {
                        #region gera arquivo de remessa no banco 

                        MedProj.Entidades.Remessa remessa = new MedProj.Entidades.Remessa();
                        remessa.QTDBoletos = cobrancaIds.Length;
                        remessa.Arquivo = nomeArquivo;
                        if (alteracao) remessa.Tipo = TipoRemessaCnab.Alteracao;
                        contexto.Save(remessa);

                        #endregion

                        #region parametros cedente 

                        string[] dadosCedente = new string[] { 
                            ConfigurationManager.AppSettings["cedenteCNPJ"],
                            ConfigurationManager.AppSettings["cedenteNOME"],
                            ConfigurationManager.AppSettings["cedenteAG"],
                            ConfigurationManager.AppSettings["cedenteAGDV"],
                            ConfigurationManager.AppSettings["cedenteCONTA"],
                            ConfigurationManager.AppSettings["cedenteCONTADV"]
                        };
                        #endregion

                        Cedente c = new Cedente(dadosCedente[0], dadosCedente[1], dadosCedente[2], dadosCedente[3], dadosCedente[4], dadosCedente[5]);

                        string carteira   = ConfigurationManager.AppSettings["cedenteCARTEIRA"];
                        string logalPagto = ConfigurationManager.AppSettings["cedenteLOCALPAGTO"];
                        Boletos boletos   = new Boletos();

                        int index = 0;
                        foreach (Cobranca cobranca in lista)
                        {
                            //cobranca.ArquivoEnviadoId = remessa.ID;
                            //contexto.Update(cobranca);

                            if (!alteracao)
                            {
                                cmd.CommandText = string.Concat("update cobranca set cobranca_arquivoUltimoEnvioId=", remessa.ID, " where cobranca_id=", cobranca.ID);
                                cmd.ExecuteNonQuery();
                            }

                            cmd.CommandText = string.Concat("insert into remessa_cobranca (rc_remessaId,rc_cobrancaId) values(", remessa.ID, ", ", cobranca.ID, ")");
                            cmd.ExecuteNonQuery();

                            if (alteracao)
                            {
                                cmd.CommandText = string.Concat("update cobranca_pendenciaCnab set pendenciacnab_processado=1, pendenciacnab_processadoData=getdate() where pendenciacnab_id=", vos[index].MovimentacaoCNABId);
                                cmd.ExecuteNonQuery();
                            }

                            Boleto b1 = new Boleto(
                                cobranca.DataVencimento,
                                cobranca.ValorCobrado,
                                carteira,
                                cobranca.ID.ToString(),
                                c,
                                new EspecieDocumento(341, "1"));

                            b1.NumeroDocumento = cobranca.ID.ToString().PadLeft(8, '0'); //"00000001";

                            #region dados sacado 

                            b1.Sacado = new Sacado(cobranca.Contrato.ContratoBeneficiario.Beneficiario.CPF, cobranca.Contrato.ContratoBeneficiario.Beneficiario.Nome);
                            b1.Sacado.Endereco.End    = cobranca.Contrato.EnderecoReferencia.Logradouro;
                            b1.Sacado.Endereco.Bairro = cobranca.Contrato.EnderecoReferencia.Bairro;
                            b1.Sacado.Endereco.Cidade = cobranca.Contrato.EnderecoReferencia.Cidade;
                            b1.Sacado.Endereco.CEP    = cobranca.Contrato.EnderecoReferencia.CEP;
                            b1.Sacado.Endereco.UF     = cobranca.Contrato.EnderecoReferencia.UF;
                            b1.Sacado.Endereco.Numero = cobranca.Contrato.EnderecoReferencia.Numero;
                            b1.LocalPagamento         = logalPagto;

                            #endregion

                            this.addInstruecoesParaArquivoCNAB(ref b1, cobranca.DataVencimento, cobranca.DataCriacao);

                            b1.Banco = new BoletoNet.Banco(341);

                            boletos.Add(b1);
                            index++;
                        }

                        BoletoNet.Banco banco = new BoletoNet.Banco(341);

                        ArquivoRemessa ar = new ArquivoRemessa(TipoArquivo.CNAB400);
                        string msg = "";
                        bool ok = ar.ValidarArquivoRemessa("", banco, c, boletos, 1, out msg);

                        if (!ok) { tran.Rollback(); contexto.Close(); return ""; }

                        FileStream stream = new FileStream(@nomeArquivo, FileMode.Create);
                        ar.GerarArquivoRemessa("", banco, c, boletos, stream, 1);
                        stream.Close();
                        stream.Dispose();

                        if (alteracao) //movimentacao cnab de alteracao de cobranca
                        {
                            List<string> novo = new List<string>();
                            string[] lines = File.ReadAllLines(nomeArquivo);

                            foreach (string line in lines)
                            {
                                if (!line.StartsWith("1")) //cabechalho ou rodape, apenas adiciona
                                {
                                    novo.Add(line);
                                }
                                else
                                {
                                    novo.Add(string.Concat(line.Substring(0, 108), "31", line.Substring(110)));
                                }
                            }

                            File.Delete(nomeArquivo);
                            File.WriteAllLines(nomeArquivo, novo.ToArray());
                        }

                        tran.Commit();

                        return nomeArquivo;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }

                } //fecha a transacao
            } 
        }

        [Obsolete("Usar GerarArquivoCNAB(string[], bool)", false)]
        public string GerarArquivoCNAB_Alteracoes(string[] cobrancaIds)
        {
            List<Cobranca> lista = new List<Cobranca>();

            string nomeArquivo = string.Concat(
                ConfigurationManager.AppSettings["appRemessaCaminhoFisico"],
                DateTime.Now.ToString("alt-yyyyMMddHHmmss"), ".txt");

            string qry = string.Concat(
                "select * ",
                "   from cobranca ",
                "       inner join contrato on contrato_id = cobranca_propostaId ",
                "       inner join contrato_beneficiario on contratobeneficiario_contratoid = contrato_id ",
                "       inner join beneficiario on beneficiario_id = contratobeneficiario_beneficiarioid ",
                "       inner join endereco on endereco_donoId = beneficiario_id and endereco_donoTipo = ", Convert.ToInt32(TipoDono.Beneficiario),
                "   where ",
                "       cobranca_id in (", String.Join(",", cobrancaIds), ")",
                "   order by contrato_id,cobranca_datacriacao");

            using (var contexto = ObterSessao())
            {
                using (ITransaction tran = contexto.BeginTransaction())
                {
                    IDbCommand cmd = contexto.Connection.CreateCommand();
                    tran.Enlist(cmd);
                    cmd.CommandText = qry;

                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(this.fullFillFrom(dr));
                        }

                        dr.Close();
                        dr.Dispose();
                    }

                    if (lista.Count == 0)
                    {
                        //pode nao ter achado devido a nao ter endereço do beneficiario
                        //tenta pelo endereco cadastrado o contrato, primeiro de cobranca
                        qry = string.Concat(
                            "select * ",
                            "   from cobranca ",
                            "       inner join contrato on contrato_id = cobranca_propostaId ",
                            "       inner join contrato_beneficiario on contratobeneficiario_contratoid = contrato_id ",
                            "       inner join beneficiario on beneficiario_id = contratobeneficiario_beneficiarioid ",
                            "       inner join endereco on endereco_id = contrato_enderecoCobrancaId ",
                            "   where ",
                            "       cobranca_id in (", String.Join(",", cobrancaIds), ")",
                            "   order by contrato_id,cobranca_datacriacao");

                        cmd.CommandText = qry;

                        using (IDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                lista.Add(this.fullFillFrom(dr));
                            }

                            dr.Close();
                            dr.Dispose();
                        }

                        if (lista.Count == 0)
                        {
                            //pode nao ter achado devido a nao ter endereço do beneficiario
                            //tenta pelo endereco cadastrado o contrato, agora pelo de referencia
                            qry = string.Concat(
                                "select * ",
                                "   from cobranca ",
                                "       inner join contrato on contrato_id = cobranca_propostaId ",
                                "       inner join contrato_beneficiario on contratobeneficiario_contratoid = contrato_id ",
                                "       inner join beneficiario on beneficiario_id = contratobeneficiario_beneficiarioid ",
                                "       inner join endereco on endereco_id = contrato_enderecoReferenciaId ",
                                "   where ",
                                "       cobranca_id in (", String.Join(",", cobrancaIds), ")",
                                "   order by contrato_id,cobranca_datacriacao");

                            cmd.CommandText = qry;

                            using (IDataReader dr = cmd.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    lista.Add(this.fullFillFrom(dr));
                                }

                                dr.Close();
                                dr.Dispose();
                            }
                        }
                    }

                    if (lista.Count == 0)
                    {
                        tran.Rollback();
                        cmd.Dispose();
                        contexto.Close();
                        return null;
                    }

                    try
                    {
                        #region gera arquivo de remessa no banco

                        MedProj.Entidades.Remessa remessa = new MedProj.Entidades.Remessa();
                        remessa.QTDBoletos = cobrancaIds.Length;
                        remessa.Arquivo = nomeArquivo;
                        remessa.Tipo = TipoRemessaCnab.Alteracao;
                        contexto.Save(remessa);

                        #endregion

                        #region parametros cedente

                        string[] dadosCedente = new string[] { 
                            ConfigurationManager.AppSettings["cedenteCNPJ"],
                            ConfigurationManager.AppSettings["cedenteNOME"],
                            ConfigurationManager.AppSettings["cedenteAG"],
                            ConfigurationManager.AppSettings["cedenteAGDV"],
                            ConfigurationManager.AppSettings["cedenteCONTA"],
                            ConfigurationManager.AppSettings["cedenteCONTADV"]
                        };
                        #endregion

                        Cedente c = new Cedente(dadosCedente[0], dadosCedente[1], dadosCedente[2], dadosCedente[3], dadosCedente[4], dadosCedente[5]);

                        string carteira = ConfigurationManager.AppSettings["cedenteCARTEIRA"];
                        string logalPagto = ConfigurationManager.AppSettings["cedenteLOCALPAGTO"];
                        Boletos boletos = new Boletos();

                        foreach (Cobranca cobranca in lista)
                        {
                            //cobranca.ArquivoEnviadoId = remessa.ID;
                            //contexto.Update(cobranca);
                            cmd.CommandText = string.Concat("update cobranca_pendenciaCnab set pendenciacnab_processado=1,pendenciacnab_processadoData=getdate() where pendenciacnab_cobrancaid=", cobranca.ID); //cmd.CommandText = string.Concat("update cobranca set cobranca_arquivoUltimoEnvioId=", remessa.ID, " where cobranca_id=", cobranca.ID);
                            cmd.ExecuteNonQuery();

                            Boleto b1 = new Boleto(
                                cobranca.DataVencimento,
                                cobranca.ValorCobrado,
                                carteira,
                                cobranca.ID.ToString(),
                                c,
                                new EspecieDocumento(341, "1"));

                            b1.NumeroDocumento = cobranca.ID.ToString().PadLeft(8, '0'); //"00000001";

                            #region dados sacado

                            b1.Sacado = new Sacado(cobranca.Contrato.ContratoBeneficiario.Beneficiario.CPF, cobranca.Contrato.ContratoBeneficiario.Beneficiario.Nome);
                            b1.Sacado.Endereco.End = cobranca.Contrato.EnderecoReferencia.Logradouro;
                            b1.Sacado.Endereco.Bairro = cobranca.Contrato.EnderecoReferencia.Bairro;
                            b1.Sacado.Endereco.Cidade = cobranca.Contrato.EnderecoReferencia.Cidade;
                            b1.Sacado.Endereco.CEP = cobranca.Contrato.EnderecoReferencia.CEP;
                            b1.Sacado.Endereco.UF = cobranca.Contrato.EnderecoReferencia.UF;
                            b1.Sacado.Endereco.Numero = cobranca.Contrato.EnderecoReferencia.Numero;
                            b1.LocalPagamento = logalPagto;

                            #endregion

                            this.addInstruecoesParaArquivoCNAB(ref b1, cobranca.DataVencimento, cobranca.DataCriacao);

                            b1.Banco = new BoletoNet.Banco(341);

                            boletos.Add(b1);
                        }

                        BoletoNet.Banco banco = new BoletoNet.Banco(341);

                        ArquivoRemessa ar = new ArquivoRemessa(TipoArquivo.CNAB400);
                        string msg = "";
                        bool ok = ar.ValidarArquivoRemessa("", banco, c, boletos, 1, out msg);

                        if (!ok) { tran.Rollback(); contexto.Close(); return ""; }

                        FileStream stream = new FileStream(@nomeArquivo, FileMode.Create);
                        //ar.GerarArquivoRemessa("", banco, c, boletos, stream, 1);
                        stream.Close();
                        stream.Dispose();

                        tran.Commit();

                        return nomeArquivo;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }

                } //fecha a transacao
            }
        }

        public MedProj.Entidades.Remessa CarregarRemessa(long id)
        {
            using (var contexto = ObterSessao())
            {
                var obj = contexto.Query<MedProj.Entidades.Remessa>()
                    .Where(r => r.ID == id)
                    .SingleOrDefault();

                return obj;
            }
        }

        public List<MedProj.Entidades.Remessa> CarregarRemessa(DateTime de, DateTime ate, TipoRemessaCnab tipo = TipoRemessaCnab.Novo)
        {
            using (var contexto = ObterSessao())
            {
                var lista = contexto.Query<MedProj.Entidades.Remessa>()
                    .Where(r => r.Data >= de && r.Data <= ate && r.Tipo == tipo)
                    .OrderByDescending(c => c.Data)
                    .ToList();

                return lista;
            }
        }

        public DataTable CarregaAdicionais(LC.Web.PadraoSeguros.Entity.Contrato contrato, LC.Framework.Phantom.PersistenceManager pm)
        {
            return LC.Web.PadraoSeguros.Entity.Cobranca.CarregaAdicionais(contrato, pm);
            //DataTable dt = null;

            #region Verifica se tem configuração de adicional para o boleto

            //object configId = null;

            ////primeiro verifica se tem alguma config especifica para o contrato
            //configId = LC.Framework.Phantom.LocatorHelper.Instance.ExecuteScalar(
            //    string.Concat(
            //    "select ca_id from config_adicional ",
            //    "   inner join config_adicional_contratos on ca_id = cac_configId ",
            //    "   where ca_ativo=1 and cac_contratoId = ", contrato.ID,
            //    "   order by ca_id desc"),
            //    null, null, pm);

            //if (configId == null || configId == DBNull.Value)
            //{
            //    //Não achou, verifica se tem alguma config especifica para o contratoADM
            //    configId = LC.Framework.Phantom.LocatorHelper.Instance.ExecuteScalar(
            //        string.Concat(
            //        "select ca_id from config_adicional ",
            //        "   where ca_ativo=1 and ca_todosContratos=1 and ca_contratoAdmId = ", contrato.ContratoADMID,
            //        "   order by ca_id desc"),
            //        null, null, pm);
            //}

            //if (configId == null || configId == DBNull.Value)
            //{
            //    //Não achou, verifica se tem alguma config especifica para o Estipulante
            //    configId = LC.Framework.Phantom.LocatorHelper.Instance.ExecuteScalar(
            //        string.Concat(
            //        "select ca_id from config_adicional ",
            //        "   where ca_ativo=1 and ca_todosContratos=1 and ca_contratoAdmId is null and ca_estipulanteId = ", contrato.EstipulanteID,
            //        "   order by ca_id desc"),
            //        null, null, pm);
            //}

            //if (configId == null || configId == DBNull.Value)
            //{
            //    //Não achou, verifica se tem alguma config para TODOS
            //    configId = LC.Framework.Phantom.LocatorHelper.Instance.ExecuteScalar(
            //        string.Concat(
            //        "select ca_id from config_adicional ",
            //        "   where ca_ativo=1 and ca_todosContratos=1 and ca_contratoAdmId is null and ca_estipulanteId is null ",
            //        "   order by ca_id desc"),
            //        null, null, pm);
            //}

            //if (configId != null && configId != DBNull.Value)
            //{
            //    //localizou, recupera o valor e o texto
            //    dt = LC.Framework.Phantom.LocatorHelper.Instance.ExecuteQuery(
            //        "select ca_id as ID, ca_texto as Texto, ca_valor as Valor from config_adicional where ca_id=" + configId,
            //        "result", pm).Tables[0];
            //}

            #endregion

            //return dt;
        }
    }
}
