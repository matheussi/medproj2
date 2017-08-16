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

    public class ArquivoBaixaFacade : FacadeBase
    {
        #region Singleton 

        static ArquivoBaixaFacade _instance;
        public static ArquivoBaixaFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new ArquivoBaixaFacade(); }
                return _instance;
            }
        }
        #endregion

        private ArquivoBaixaFacade() { }

        public long Salvar(ArquivoBaixa arquivo)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        sessao.SaveOrUpdate(arquivo);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

            return arquivo.ID;
        }

        public List<ArquivoBaixa> Carregar(DateTime criacaoDe, DateTime criacaoAte, bool processados)
        {
            List<ArquivoBaixa> arquivos = null;

            using (var sessao = ObterSessao())
            {
                var query = from arq in sessao.Query<ArquivoBaixa>().Where(a => a.Criacao >= criacaoDe && a.Criacao <= criacaoAte)
                            select new { arq.ID, arq.Nome, arq.Criacao, arq.Processamento, arq.Tipo };

                if (processados)
                    query = query.Where(a => a.Processamento != null).OrderByDescending(a => a.Criacao);
                else
                    query = query.Where(a => a.Processamento == null).OrderByDescending(a => a.Criacao);

                arquivos = query.AsEnumerable()
                    .Select(ar => new ArquivoBaixa 
                                    { 
                                        ID = ar.ID, 
                                        Nome = ar.Nome, 
                                        Criacao = ar.Criacao, 
                                        Processamento = ar.Processamento, 
                                        Tipo = ar.Tipo
                                    }
                           )
                    .ToList();
            }

            return arquivos;
        }

        public bool ProcessarBaixa(long arquivoId)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        ArquivoBaixa arquivo = sessao.Get<ArquivoBaixa>(arquivoId);

                        //if (arquivo.Processamento.HasValue) { throw new ApplicationException("Tentativa de baixar arquivo ja processado."); }

                        arquivo.Processamento = DateTime.Now;
                        List<ArquivoBaixaItem> itens = Processar(arquivo, sessao);

                        sessao.SaveOrUpdate(arquivo);

                        tran.Commit();

                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        List<ArquivoBaixaItem> Processar(ArquivoBaixa arquivo, ISession sessao)
        {
            if (string.IsNullOrEmpty(arquivo.Corpo)) return null;

            if (arquivo.Tipo == TipoArquivoBaixa.Itau)
                return ProcessarItauRetorno(arquivo, sessao);
            else if (arquivo.Tipo == TipoArquivoBaixa.DepositoIdentificadoItau)
                return ProcessarDepositoIdentificadoItauRetorno(arquivo, sessao);
            else
                return null;
        }
        List<ArquivoBaixaItem> ProcessarItauRetorno(ArquivoBaixa arquivo, ISession sessao)
        {
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            String[] arrLinhas = arquivo.Corpo.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<ArquivoBaixaItem> itens = new List<ArquivoBaixaItem>();

            IDbCommand cmd = sessao.Connection.CreateCommand();
            sessao.Transaction.Enlist(cmd);

            bool pagoPreviamente = false;
            string linhaAtual = "", aux = ""; object ret = null;
            for (int i = 0; i < arrLinhas.Length; i++)
            {
                linhaAtual = arrLinhas[i];

                if (linhaAtual == null || linhaAtual.Trim() == "") { continue; }

                aux = linhaAtual.Substring(0, 1);
                if (aux == "0" || aux == "9") { continue; } //se é cabecalho ou trailler, ignorar

                pagoPreviamente = false;

                ArquivoBaixaItem item = new ArquivoBaixaItem();
                item.Arquivo = arquivo;
                item.Data = arquivo.Processamento.Value;

                //nosso numero
                aux = linhaAtual.Substring(62, 8);

                item.Identificacao = aux;
                //item.Cobranca = sessao.Query<Cobranca>().Fetch(c => c.Contrato).Where(c => c.NossoNumero == aux).FirstOrDefault();
                item.Cobranca = sessao.Query<Cobranca>().Fetch(c => c.Contrato).Where(c => c.ID == Convert.ToInt64(aux)).FirstOrDefault();

                //item.Contrato = sessao.Get<Contrato>(item.Cobranca.Contrato.ID);

                if (item.Cobranca == null)
                {
                    item.Status = TipoItemBaixa.TituloNaoEncontrado;
                    sessao.SaveOrUpdate(item);
                }
                else
                {
                    //TODO: checar por pagto. em duplicidade

                    item.Contrato = sessao.Get<Contrato>(item.Cobranca.Contrato.ID);

                    if (item.Contrato == null)
                    {
                        item.Status = TipoItemBaixa.TituloNaoEncontrado;
                        sessao.SaveOrUpdate(item);
                        continue;
                    }

                    if (item.Contrato.Tipo == TipoPessoa.Fisica)
                    {
                        //TODO: remover a property Titular, mapear os objetos corretamente
                        cmd.CommandText = "SELECT beneficiario_nome from contrato_beneficiario inner join beneficiario on beneficiario_id=contratobeneficiario_beneficiarioid WHERE contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 and contratobeneficiario_contratoid=" + item.Cobranca.Contrato.ID.ToString();
                        ret = cmd.ExecuteScalar();
                    }
                    else
                    {
                        //TODO: remover a property Titular, mapear os objetos corretamente
                        cmd.CommandText = "SELECT beneficiario_razaoSocial from contrato_beneficiario inner join beneficiario on beneficiario_id=contratobeneficiario_beneficiarioid WHERE contratobeneficiario_ativo=1 and contratobeneficiario_tipo=0 and contratobeneficiario_contratoid=" + item.Cobranca.Contrato.ID.ToString();
                        ret = cmd.ExecuteScalar();
                    }

                    aux = linhaAtual.Substring(108, 2); //OCORRENCIA
                    if (aux == "17")
                    {
                        item.Status = TipoItemBaixa.AlteracaoRejeitada;
                        sessao.SaveOrUpdate(item);
                        continue;
                    }
                    else if (aux != "06") //se não é liquidação
                    {
                        item.Status = TipoItemBaixa.OcorrenciaSemLiquidacao;
                        sessao.SaveOrUpdate(item);
                        continue;
                    }

                    if (item.Cobranca.Cancelada) { item.Status = TipoItemBaixa.TituloCanceladoLiquidado; item.Cobranca.Cancelada = false; }
                    else if (item.Cobranca.Pago) { item.Status = TipoItemBaixa.PagamentoDuplicado; }
                    else                         { item.Status = TipoItemBaixa.TituloLiquidado; }

                    item.Cobranca.DataBaixa = arquivo.Processamento.Value;

                    //data pagamento
                    aux = linhaAtual.Substring(110, 6);
                    item.Cobranca.DataPagamento = CToDateTime(aux, 0, 0, 0, true);
                    item.DataCredito = item.Cobranca.DataPagamento;

                    //valor pagamento (valor principal + tarifa)
                    item.Cobranca.ValorPago = (Convert.ToDecimal(linhaAtual.Substring(253, 13)) / Convert.ToDecimal(100)) + (Convert.ToDecimal(linhaAtual.Substring(175, 13)) / Convert.ToDecimal(100)); //Convert.ToDecimal(linhaAtual.Substring(153, 12), cinfo) / 100;
                    item.ValorPago = item.Cobranca.ValorCobrado;


                    if (ret != null && ret != DBNull.Value)
                        item.Titular = Convert.ToString(ret);
                    else
                        item.Titular = "";

                    if (item.Cobranca.Pago) pagoPreviamente = true;

                    item.Cobranca.Pago = true;

                    ////////////////TODO: Denis, remover isso após a correção da movimentacao cnab de alteracao de cobranca
                    //if (item.Cobranca.Pago && item.Cobranca.ValorCobrado != item.Cobranca.ValorPago)
                    //    item.Cobranca.ValorPago = item.Cobranca.ValorCobrado;
                    ///////////////////////////////////////////////////////////////////////////////////////////////

                    if(item.Status != TipoItemBaixa.PagamentoDuplicado)
                        sessao.SaveOrUpdate(item.Cobranca);

                    sessao.SaveOrUpdate(item);

                    //só atualiza o saldo se a cobrança nao tiver sido paga previamente
                    if (!pagoPreviamente) AtualizaSaldo(item, sessao, true);
                }
            }

            cmd.Dispose();

            return itens;
        }
        List<ArquivoBaixaItem> ProcessarDepositoIdentificadoItauRetorno(ArquivoBaixa arquivo, ISession sessao)
        {
            System.Globalization.CultureInfo cinfo = new System.Globalization.CultureInfo("pt-Br");
            String[] arrLinhas = arquivo.Corpo.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<ArquivoBaixaItem> itens = new List<ArquivoBaixaItem>();

            IDbCommand cmd = sessao.Connection.CreateCommand();
            sessao.Transaction.Enlist(cmd);

            string linhaAtual = "", aux = ""; //object ret = null;
            ContratoBeneficiario cb = null;
            for (int i = 0; i < arrLinhas.Length; i++)
            {
                linhaAtual = arrLinhas[i];

                if (linhaAtual == null || linhaAtual.Trim() == "") { continue; }

                aux = linhaAtual.Substring(13, 2);
                if (aux == "00" || aux == "99") { continue; } //se é cabecalho ou trailler, ignorar

                ArquivoBaixaItem item = new ArquivoBaixaItem();
                item.Arquivo = arquivo;
                item.Data = arquivo.Processamento.Value;

                //data credito
                aux = linhaAtual.Substring(15, 6);
                item.DataCredito = CToDateTime(aux, 0, 0, 0, false);

                //valor pagamento
                aux = linhaAtual.Substring(54, 17);
                item.ValorPago = Convert.ToDecimal(aux, cinfo) / 100;

                //data recebimento
                aux = linhaAtual.Substring(28, 6);
                item.DataRemessa = CToDateTime(aux, 0, 0, 0, false);

                //identificacao (numero do contrato ou cpf)
                aux = linhaAtual.Substring(34, 16);

                item.Identificacao = Convert.ToInt64(aux).ToString().PadLeft(11, '0'); //CPF ou Cartao

                if (item.Identificacao.Length <= 11)
                {
                    cb = sessao.Query<ContratoBeneficiario>()
                        .Fetch(c => c.Contrato)
                        .Where(c => c.Beneficiario.CPF == item.Identificacao && c.Contrato.Cancelado == false && c.Contrato.Inativo == false)
                        .SingleOrDefault();
                }
                else
                {
                    cb = sessao.Query<ContratoBeneficiario>()
                        .Fetch(c => c.Contrato)
                        .Fetch(c => c.Beneficiario)
                        .Where(c => c.Contrato.Numero == item.Identificacao)
                        .SingleOrDefault();
                }

                if (cb != null)
                {
                    item.Contrato = cb.Contrato; 

                    if (cb.Beneficiario != null) item.Titular = cb.Beneficiario.Nome;
                }

                if (item.Contrato == null)
                {
                    item.Status = TipoItemBaixa.DepositoNaoIdentificado;
                    sessao.SaveOrUpdate(item);
                }
                else
                {
                    //TODO: checar por pagto. ou processamento em duplicidade /////////////////////////////

                    if (item.Contrato.Cancelado || item.Contrato.Inativo) item.Status = TipoItemBaixa.DepositoBaixadoContratoCancelado;
                    else                                                  item.Status = TipoItemBaixa.DepositoBaixado;

                    //agencia origem
                    aux = linhaAtual.Substring(21, 4);
                    item.AgenciaOrigem = aux;

                    //lote
                    if (string.IsNullOrEmpty(arquivo.NumeroLote))
                    {
                        aux = linhaAtual.Substring(25, 3);
                        arquivo.NumeroLote = aux;
                    }

                    sessao.SaveOrUpdate(item);

                    this.AtualizaSaldo(item, sessao, true);
                }
            }

            cmd.Dispose();

            return itens;
        }

        public List<ArquivoBaixaItem> CarregarItens(long arquivoId)
        {
            List<ArquivoBaixaItem> itens = null;

            using (var sessao = ObterSessao())
            {
                itens = sessao.Query<ArquivoBaixaItem>()
                    .Fetch(i => i.Arquivo)
                    .Fetch(i => i.Cobranca)
                    .ThenFetch(c => c.Contrato)
                    .Where(i => i.Arquivo.ID == arquivoId)
                    .ToList();

                #region comentado 

                //var query = from item in sessao.Query<ArquivoBaixaItem>().Where(i => i.Arquivo.ID == arquivoId)
                //            join arquivo in sessao.Query<ArquivoBaixa>() on item.Arquivo.ID equals arquivo.ID
                //            join cobr in sessao.Query<Cobranca>() on item.Cobranca.ID equals cobr.ID into cob from subcobr in cob.DefaultIfEmpty()
                //            select new { item.ID, item.Arquivo, item.Cobranca, item.Data, item.Status };

                //itens = query.AsEnumerable()
                //    .Select(item => new ArquivoBaixaItem
                //        {
                //            ID = item.ID,
                //            Arquivo = item.Arquivo,
                //            Cobranca = item.Cobranca,
                //            Data = item.Data,
                //            Status = item.Status
                //        }
                //    ).ToList();

                //var query = from item in sessao.Query<ArquivoBaixaItem>().Where(i => i.Arquivo.ID == arquivoId)
                //            join arquivo in sessao.Query<ArquivoBaixa>() on item.Arquivo.ID equals arquivo.ID
                //            join cobr in sessao.Query<Cobranca>() on item.Cobranca.ID equals cobr.ID 
                //            select new { item.ID, item.Arquivo, item.Cobranca = cobr != null ? cobr : null, item.Data, item.Status };

                //itens = query.AsEnumerable()
                //    .Select(item => new ArquivoBaixaItem
                //    {
                //        ID = item.ID,
                //        Arquivo = item.Arquivo,
                //        Cobranca = item.Cobranca,
                //        Data = item.Data,
                //        Status = item.Status
                //    }
                //    ).ToList();

                #endregion
            }

            return itens;
        }

        public void AtualizaSaldo(long contratoId, TipoMovimentacao tipo, decimal valor, string comentario, long usuarioId, DateTime? data = null)
        {
            if (data == null) data = DateTime.Now;

            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    try
                    {
                        var saldo = sessao.Query<Saldo>().Where(s => s.Contrato.ID == contratoId).SingleOrDefault();
                        if (saldo == null)
                        {
                            saldo = new Saldo();
                            saldo.Contrato = new Contrato();
                            saldo.Contrato.ID = contratoId;
                        }

                        saldo.Movimentar(tipo, valor, data);
                        sessao.SaveOrUpdate(saldo);

                        SaldoMovimentacaoHistorico hist = new SaldoMovimentacaoHistorico();
                        hist.Contrato = new Contrato();
                        hist.Contrato.ID = contratoId;
                        hist.Data = data.Value;
                        hist.Descricao = comentario;
                        hist.UsuarioId = usuarioId;

                        sessao.SaveOrUpdate(hist);

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                    }
                }
            }
        }

        void AtualizaSaldo(ArquivoBaixaItem item, ISession sessao, bool cobrarTaxa)
        {
            if (item.Contrato.Tipo == TipoPessoa.Juridica) return;

            Saldo saldo = null;
            long cid = 0; decimal valor = 0; DateTime data = DateTime.MinValue;

            if (item.Cobranca != null)
            {
                cid = item.Cobranca.Contrato.ID;
                valor = item.Cobranca.ValorPago;
                data = item.Cobranca.DataBaixa.Value;
                saldo = sessao.Query<Saldo>().Where(s => s.Contrato.ID == item.Cobranca.Contrato.ID).SingleOrDefault();
            }
            else
            {
                cid = item.Contrato.ID;
                valor = item.ValorPago;
                data = item.DataCredito.Value;
                saldo = sessao.Query<Saldo>().Where(s => s.Contrato.ID == item.Contrato.ID).SingleOrDefault();
            }

            if (saldo == null)
            {
                saldo = new Saldo();
                saldo.Contrato = new Contrato();
                saldo.Contrato.ID = cid;
            }

            saldo.Movimentar(TipoMovimentacao.Credito, valor, data);

            decimal taxa = 0;

            sessao.SaveOrUpdate(saldo);

            if (cobrarTaxa)
            {
                taxa = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["taxaBoleto"], new System.Globalization.CultureInfo("pt-Br"));

                saldo.Movimentar(TipoMovimentacao.Debito, taxa, data);
            }

            SaldoMovimentacaoHistorico hist = new SaldoMovimentacaoHistorico();
            hist.Contrato = new Contrato();
            hist.Contrato.ID = cid;
            hist.Data = data;
            hist.Descricao = string.Concat("Crédito: R$ ", valor.ToString("N2"));

            sessao.SaveOrUpdate(hist);

            if (cobrarTaxa)
            {
                string creditoDescricao = string.Concat(hist.Descricao, " em ", hist.Data.ToString("dd/MM/yyyy HH:mm"));
                long id = hist.ID;

                hist = new SaldoMovimentacaoHistorico();
                hist.Contrato = new Contrato();
                hist.Contrato.ID = cid;
                hist.Data = data;
                hist.Descricao = string.Concat("Débito Taxa (ref.", id.ToString(), " - ", creditoDescricao, "): R$ ", taxa.ToString("N2"));
                sessao.SaveOrUpdate(hist);
            }
        }
    }
}
