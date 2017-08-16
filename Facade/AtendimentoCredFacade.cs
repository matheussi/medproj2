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
    using System.Data.Common;

    public class AtendimentoCredFacade : FacadeBase
    {
        #region singleton 

        static AtendimentoCredFacade _instance;
        public static AtendimentoCredFacade Instance
        {
            get
            {
                if (_instance == null) { _instance = new AtendimentoCredFacade(); }
                return _instance;
            }
        }
        #endregion

        private AtendimentoCredFacade() { }

        public long Salvar(Atendimento atendimento, IList<AtendimentoProcedimento> procedimentos)
        {
            if(atendimento == null || procedimentos == null) return 0;

            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    //se pagamento via cartao, por seguranca, verifica saldo
                    if (atendimento.FormaPagto == FormaPagtoAtendimento.Cartao)
                    {
                        var saldo = sessao.Query<Saldo>().Where(s => s.Contrato.ID == atendimento.Contrato.ID).FirstOrDefault();
                        if (saldo == null)
                        {
                            tran.Rollback();
                            sessao.Close();
                            return -1;
                        }

                        decimal _valor = 0;
                        foreach(var p in procedimentos)
                        {
                            _valor += decimal.Round(p.Valor, 2);
                        }

                        if (decimal.Round(saldo.Atual, 2) < decimal.Round(_valor, 2)) //if (saldo.Atual < procedimentos.Sum(p => p.Valor))
                        {
                            tran.Rollback();
                            sessao.Close();
                            return -1;
                        }
                    }

                    if (atendimento.UsuarioMaster != null)
                    {
                        atendimento.UsuarioMaster = sessao.Get<Usuario>(atendimento.UsuarioMaster.ID); //.QueryOver<Usuario>().get
                    }

                    //tran.Commit();
                    //return -1;

                    this.salvaAtendimento(atendimento, procedimentos, sessao);

                    if (atendimento.FormaPagto == FormaPagtoAtendimento.Cartao)
                    {
                        this.atualizaSaldo(atendimento.Contrato, procedimentos, sessao, atendimento.Data);
                    }

                    this.salvaHistoricoDeMovimentacao(atendimento, procedimentos, sessao);

                    tran.Commit();
                }
            }

            return atendimento.ID;
        }

        void salvaAtendimento(Atendimento atendimento, IList<AtendimentoProcedimento> procedimentos, ISession sessao)
        {
            sessao.SaveOrUpdate(atendimento);

            foreach (AtendimentoProcedimento ap in procedimentos)
            {
                ap.Atendimento.ID = atendimento.ID;
                sessao.Save(ap);
            }
        }

        public void CancelarProcedimento(long procedimentoAtendimentoId, long usuarioId)
        {
            using (var sessao = ObterSessao())
            {
                using (ITransaction tran = sessao.BeginTransaction())
                {
                    AtendimentoProcedimento proc = sessao.Get<AtendimentoProcedimento>(procedimentoAtendimentoId);
                    proc.Cancelado = true;
                    proc.DataCancelado = DateTime.Now;
                    proc.Usuario = sessao.Get<Usuario>(usuarioId);

                    sessao.Update(proc);

                    Atendimento atend = sessao.Get<Atendimento>(proc.Atendimento.ID);

                    string descricaoCancelamento = "";
                    if (atend.FormaPagto == FormaPagtoAtendimento.Cartao)
                    {
                        Saldo saldo = sessao.Query<Saldo>().Where(s => s.Contrato.ID == proc.Atendimento.Contrato.ID).SingleOrDefault();

                        if (saldo == null)
                        {
                            saldo = new Saldo();
                            saldo.Contrato = new Contrato();
                            saldo.Contrato.ID = proc.Atendimento.Contrato.ID;
                        }

                        saldo.Movimentar(TipoMovimentacao.Credito, proc.Valor, proc.DataCancelado);
                        sessao.SaveOrUpdate(saldo);

                        descricaoCancelamento = string.Concat("Crédito cancelamento (", proc.Procedimento.Nome, "): R$ ", proc.Valor.ToString("N2"));
                    }
                    else
                        descricaoCancelamento = string.Concat("Dinheiro cancelamento (", proc.Procedimento.Nome, "): R$ ", proc.Valor.ToString("N2"));

                    SaldoMovimentacaoHistorico hist = new SaldoMovimentacaoHistorico();
                    hist.Contrato = new Contrato();
                    hist.Contrato.ID = proc.Atendimento.Contrato.ID;
                    hist.Data = proc.DataCancelado.Value;
                    hist.Descricao = descricaoCancelamento; // string.Concat("Crédito cancelamento (", proc.Procedimento.Nome, "): R$ ", proc.Valor.ToString("N2"));
                    sessao.Save(hist);

                    //TODO: centralizar operacos de atualizacao de saldo e historico
                    //this.atualizaSaldo(atendimento.Contrato, procedimentos, sessao, atendimento.Data);
                    //this.salvaHistoricoDeMovimentacao(atendimento, procedimentos, sessao);

                    tran.Commit();
                }
            }
        }

        void atualizaSaldo(Contrato contrato, IList<AtendimentoProcedimento> procedimentos, ISession sessao, DateTime data)
        {
            Saldo saldo = sessao.Query<Saldo>().Where(s => s.Contrato.ID == contrato.ID).SingleOrDefault();

            if (saldo == null)
            {
                saldo = new Saldo();
                saldo.Contrato = new Contrato();
                saldo.Contrato.ID = contrato.ID;
            }

            decimal valorTotal = procedimentos.Sum(p => p.Valor);

            saldo.Movimentar(TipoMovimentacao.Debito, valorTotal, data);
            sessao.SaveOrUpdate(saldo);
        }

        void salvaHistoricoDeMovimentacao(Atendimento atendimento, IList<AtendimentoProcedimento> procedimentos, ISession sessao)
        {
            if (procedimentos == null) return;

            foreach (AtendimentoProcedimento ap in procedimentos)
            {
                SaldoMovimentacaoHistorico hist = new SaldoMovimentacaoHistorico();
                hist.Contrato = new Contrato();
                hist.Contrato.ID = atendimento.Contrato.ID;
                hist.Data = atendimento.Data;

                if(atendimento.FormaPagto == FormaPagtoAtendimento.Cartao)
                    hist.Descricao = string.Concat("Débito (", ap.Procedimento.Nome, "): R$ ", ap.Valor.ToString("N2"));
                else
                    hist.Descricao = string.Concat("Pagto. Dinheiro (", ap.Procedimento.Nome, "): R$ ", ap.Valor.ToString("N2"));

                sessao.SaveOrUpdate(hist);
            }
        }

        public object CarregarPorParametros(long unidadeId, string numeroCartao, DateTime de, DateTime ate)
        {
            using (var sessao = ObterSessao())
            {
                var lista =
                (
                    from a in sessao.Query<Atendimento>()
                    join c in sessao.Query<Contrato>() on a.Contrato.ID equals c.ID
                    join cb in sessao.Query<ContratoBeneficiario>() on c.ID equals cb.Contrato.ID
                    join b in sessao.Query<Beneficiario>() on cb.Beneficiario.ID equals b.ID
                    join ai in sessao.Query<AtendimentoProcedimento>() on a.ID equals ai.Atendimento.ID

                    where (a.Unidade.ID == unidadeId && c.Numero == numeroCartao && a.Data >= de && a.Data <= ate)
                    group ai by new { a.ID, a.Data, b.Nome } into g 

                    select new
                    {
                        ID      = g.Key.ID,
                        Data    = g.Key.Data,
                        Titular = g.Key.Nome
                    }
                )
                .ToList();

                return lista;
            }
        }

        public DataTable CarregarPorParametros(long unidadeId, string nome, string cpf, string numeroCartao, DateTime de, DateTime ate)
        {
            using (var sessao = ObterSessao())
            {
                using (IDbCommand cmd = sessao.Connection.CreateCommand())
                {
                    cmd.CommandText = string.Concat(
                        "select atendimentocred_id, atendimentocred_data, beneficiario_nome, contrato_numero, atendimentocred_formapago, ",
                        "(select count(atendimentocredproc_id) from atendimentoCred_procedimento where atendimentocredproc_atendimentoId=atendimentocred_id) as Qtd, ",
                        "(select sum(atendimentocredproc_valor) from atendimentoCred_procedimento where atendimentocredproc_atendimentoId=atendimentocred_id and atendimentocredproc_cancelado=0) as Valor ",
                        "   from atendimentoCred ",
                        "       inner join contrato on contrato_id = atendimentocred_contratoId ",
                        "       inner join contrato_beneficiario on contrato_id = contratobeneficiario_contratoId and contratobeneficiario_tipo = 0 ",
                        "       inner join beneficiario on beneficiario_id = contratobeneficiario_beneficiarioId ",
//                      "       left join prestador_unidade pu on pu.id = atendimentocred_unidadeId ",
//                      "       inner join atendimentoCred_procedimento on atendimentocred_id = atendimentocredproc_atendimentoId ",
                        "   where ",
                        "       atendimentocred_unidadeId = ", unidadeId, " and atendimentocred_data between '", de.ToString("yyyy-MM-dd"), "' and '", ate.ToString("yyyy-MM-dd 23:59:59:995"), "' ");

                    if (!string.IsNullOrWhiteSpace(numeroCartao))
                    {
                        cmd.CommandText += " and atendimentocred_numeroCartao = @atendimentocred_numeroCartao ";
                        var parameter = cmd.CreateParameter();
                        parameter.ParameterName = "@atendimentocred_numeroCartao";
                        parameter.Value = numeroCartao;
                        parameter.DbType = DbType.String;
                        cmd.Parameters.Add(parameter);
                    }

                    if (!string.IsNullOrWhiteSpace(nome))
                    {
                        cmd.CommandText += " and beneficiario_nome like '%" + nome + "%'";
                        //var parameter = cmd.CreateParameter();
                        //parameter.ParameterName = "@atendimentocred_numeroCartao";
                        //parameter.Value = nome;
                        //parameter.DbType = DbType.String;
                        //cmd.Parameters.Add(parameter);
                    }

                    if (!string.IsNullOrWhiteSpace(cpf))
                    {
                        cmd.CommandText += " and beneficiario_cpf = @beneficiario_cpf ";
                        var parameter = cmd.CreateParameter();
                        parameter.ParameterName = "@beneficiario_cpf";
                        parameter.Value = cpf.Replace(".", "").Replace("-", "");
                        parameter.DbType = DbType.String;
                        cmd.Parameters.Add(parameter);
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Data");
                    dt.Columns.Add("Titular");
                    dt.Columns.Add("Numero");
                    dt.Columns.Add("Qtd");
                    dt.Columns.Add("Valor");
                    dt.Columns.Add("FormaPagto");

                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            DataRow row = dt.NewRow();

                            row["ID"] = dr.GetInt64(0);
                            row["Data"] = dr[1];
                            row["Titular"] = dr[2];
                            row["Numero"] = dr[3];

                            if (Convert.ToInt16(dr[4]) == 0) row["FormaPagto"] = "Cartão";
                            else                             row["FormaPagto"] = "Dinheiro";

                            if (dr[5] != DBNull.Value) row["Qtd"] = dr[5];
                            else                       row["Qtd"] = 0;

                            if (dr[6] != DBNull.Value) row["Valor"] = dr[6];
                            else                       row["Valor"] = "0,00";

                            dt.Rows.Add(row);
                        }
                    }

                    return dt;
                }
            }
        }

        public DataTable CarregarPorParametros(long? unidadeId, string nome, string cpf, string numeroCartao, DateTime de, DateTime ate, long? prestadorId, FormaPagtoAtendimento forma)
        {
            using (var sessao = ObterSessao())
            {
                using (IDbCommand cmd = sessao.Connection.CreateCommand())
                {
                    cmd.CommandText = string.Concat(
                        "select atendimentocred_id, atendimentocred_data, beneficiario_nome, contrato_numero, atendimentocred_formapago, pu.Nome, ",
                        "(select count(atendimentocredproc_id) from atendimentoCred_procedimento where atendimentocredproc_atendimentoId=atendimentocred_id) as Qtd, ",
                        "(select sum(atendimentocredproc_valor) from atendimentoCred_procedimento where atendimentocredproc_atendimentoId=atendimentocred_id and atendimentocredproc_cancelado=0) as Valor ",
                        "   from atendimentoCred ",
                        "       inner join contrato on contrato_id = atendimentocred_contratoId ",
                        "       inner join contrato_beneficiario on contrato_id = contratobeneficiario_contratoId and contratobeneficiario_tipo = 0 ",
                        "       inner join beneficiario on beneficiario_id = contratobeneficiario_beneficiarioId ",
                        "       inner join prestador_unidade pu on pu.id = atendimentocred_unidadeId ",
//                      "       left join prestador pre on pu.Owner_ID = pre.ID ",
//                      "       inner join atendimentoCred_procedimento on atendimentocred_id = atendimentocredproc_atendimentoId ",
                        "   where ",
                        "       atendimentocred_data between '", de.ToString("yyyy-MM-dd"), "' and '", ate.ToString("yyyy-MM-dd 23:59:59:995"), "' ");

                    if (!string.IsNullOrWhiteSpace(numeroCartao))
                    {
                        cmd.CommandText += " and atendimentocred_numeroCartao = @atendimentocred_numeroCartao ";
                        var parameter = cmd.CreateParameter();
                        parameter.ParameterName = "@atendimentocred_numeroCartao";
                        parameter.Value = numeroCartao;
                        parameter.DbType = DbType.String;
                        cmd.Parameters.Add(parameter);
                    }

                    if (prestadorId != null)
                    {
                        cmd.CommandText += " and pu.Owner_ID = " + prestadorId.Value.ToString(); //" and pre.ID = " + prestadorId.Value.ToString();
                    }

                    if (unidadeId != null)
                    {
                        cmd.CommandText += " and atendimentocred_unidadeId = " + unidadeId.Value.ToString();
                    }

                    if (forma != FormaPagtoAtendimento.Indefinido)
                    {
                        cmd.CommandText += " and atendimentocred_formapago = " + Convert.ToInt32(forma).ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(nome))
                    {
                        cmd.CommandText += " and beneficiario_nome like '%" + nome + "%'";
                        //var parameter = cmd.CreateParameter();
                        //parameter.ParameterName = "@atendimentocred_numeroCartao";
                        //parameter.Value = nome;
                        //parameter.DbType = DbType.String;
                        //cmd.Parameters.Add(parameter);
                    }

                    if (!string.IsNullOrWhiteSpace(cpf))
                    {
                        cmd.CommandText += " and beneficiario_cpf = @beneficiario_cpf ";
                        var parameter = cmd.CreateParameter();
                        parameter.ParameterName = "@beneficiario_cpf";
                        parameter.Value = cpf.Replace(".", "").Replace("-", "");
                        parameter.DbType = DbType.String;
                        cmd.Parameters.Add(parameter);
                    }

                    //cmd.CommandText += " order by pu.Nome, atendimentocred_data ";
                    cmd.CommandText += " order by atendimentocred_data, pu.Nome ";

                    DataTable dt = new DataTable();
                    dt.Columns.Add("ID");
                    dt.Columns.Add("Data");
                    dt.Columns.Add("Titular");
                    dt.Columns.Add("Numero");
                    dt.Columns.Add("Qtd");
                    dt.Columns.Add("Valor");
                    dt.Columns.Add("FormaPagto");
                    dt.Columns.Add("Unidade");

                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            DataRow row = dt.NewRow();

                            row["ID"] = dr.GetInt64(0);
                            row["Data"] = dr[1];
                            row["Titular"] = dr[2];
                            row["Numero"] = dr[3];

                            if (Convert.ToInt16(dr[4]) == 0) row["FormaPagto"] = "Cartão";
                            else row["FormaPagto"] = "Dinheiro";

                            row["Unidade"] = dr[5];

                            if (dr[6] != DBNull.Value) row["Qtd"] = dr[6];
                            else row["Qtd"] = 0;

                            if (dr[7] != DBNull.Value) row["Valor"] = dr[7];
                            else row["Valor"] = "0,00";

                            dt.Rows.Add(row);
                        }
                    }

                    return dt;
                }
            }
        }

        public List<AtendimentoProcedimento> CarregarProcedimentosDoAtendimento(long atendimentoId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<AtendimentoProcedimento>()
                    //.Fetch(ap => ap.Especialidade)
                    .Fetch(ap => ap.Procedimento)
                    .Fetch(ap => ap.Atendimento).ThenFetch(a => a.Contrato)
                    .Where(ap => ap.Atendimento.ID == atendimentoId)
                    .ToList();
            }
        }

        public AtendimentoProcedimento CarregarProcedimentosDeAtendimento(long procedimentoAtendimentoId)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<AtendimentoProcedimento>()
                    .Fetch(ap => ap.Atendimento)
                    //.Fetch(ap => ap.Especialidade)
                    .Fetch(ap => ap.Procedimento)
                    .Where(ap => ap.ID == procedimentoAtendimentoId)
                    .Single();
            }
        }

        public Atendimento Carregar(long idAtendimento)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<Atendimento>()
                    .Fetch(a => a.Contrato)
                    .Fetch(a => a.Unidade)
                    .FetchMany(a => a.Procedimentos).ThenFetch(ap => ap.Procedimento)
                    .Where(a => a.ID == idAtendimento)
                    .Single();
            }
        }
    }
}
