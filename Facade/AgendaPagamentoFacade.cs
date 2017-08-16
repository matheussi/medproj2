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

    public class AgendaPagamentoFacade : FacadeBase
    {
        AgendaPagamentoFacade() { }

        #region singleton 

        static AgendaPagamentoFacade _instancia;
        public static AgendaPagamentoFacade Instancia
        {
            get
            {
                if (_instancia == null) { _instancia = new AgendaPagamentoFacade(); }
                return _instancia;
            }
        }
        #endregion

        public AgendaPagamento Salvar(AgendaPagamento agenda)
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

        public AgendaPagamento Carregar(long id)
        {
            using (var sessao = ObterSessao())
            {
                return sessao.Query<AgendaPagamento>()
                    .FetchMany(i => i.Itens).ThenFetch(i => i.Atendimento)
                    .Where(i => i.ID == id)
                    .Single();
            }
        }

        /// <summary>
        /// Atenção com problema cartesiano aqui
        /// </summary>
        public AgendaPagamento CarregarCompleto(long id)
        {
            using (var sessao = ObterSessao())
            {
                var agenda = sessao.Query<AgendaPagamento>()
                    .FetchMany(a => a.Itens).ThenFetch(i => i.Atendimento).ThenFetchMany(a => a.Procedimentos)
                    .Where(a => a.ID == id)
                    .Single();

                foreach (var item in agenda.Itens)
                {
                    //item.Atendimento = sessao.Query<Atendimento>()
                    //    .Fetch(a => a.Unidade) //.ThenFetch(u => u.Banco)
                    //    .Where(a => a.ID == item.Atendimento.ID)
                    //    .Single();

                    item.Atendimento.Unidade.Banco = sessao.Query<Banco>()
                        .Where(b => b.Unidade.ID == item.Atendimento.Unidade.ID)
                        .Single();
                }

                return agenda;

                //return sessao.Query<AgendaPagamento>()
                //    .FetchMany(a => a.Itens).ThenFetch(i => i.Atendimento).ThenFetchMany(a => a.Procedimentos)
                //    .FetchMany(a => a.Itens).ThenFetch(i => i.Atendimento).ThenFetch(at => at.Unidade).ThenFetch(u => u.Banco)
                //    .Where(a => a.ID == id)
                //    .Single();
            }
        }

        
        public List<AgendaPagamento> Carregar(DateTime de, DateTime ate)
        {
            List<AgendaPagamento> lista = null;

            using (var sessao = ObterSessao())
            {
                lista = sessao.Query<AgendaPagamento>()
                    .Where(i => i.DataCriacao >= de && i.DataCriacao <= ate)
                    .OrderByDescending(i => i.DataCriacao)
                    .Take(300)
                    .ToList();
            }

            return lista;
        }

        public List<AgendaPagamento> CarregarPendentes()
        {
            List<AgendaPagamento> lista = null;

            using (var sessao = ObterSessao())
            {
                lista = sessao.Query<AgendaPagamento>()
                    .Where(i => i.DataProcessamento <= DateTime.Now && i.Processado == false && i.Ativa == true)
                    .OrderByDescending(i => i.DataCriacao)
                    .Take(50)
                    .ToList();
            }

            return lista;
        }

        /// <summary>
        /// Logica: pega o periodo, a primeira data e a segunda. verifica quantas quinzenas tem, separa em bloco
        /// atribui cada atendimento a um bloco
        /// </summary>
        public void Processar(AgendaPagamento agenda)
        {
            if (agenda == null || agenda.DataConclusao.HasValue) return;

            using (var sessao = ObterSessao())
            {
                List<Atendimento> atendimentos = sessao.Query<Atendimento>()
                    .Where(a => a.Data >= agenda.PeriodoDe && a.Data <= agenda.PeriodoAte && a.Unidade.TipoPagto == agenda.TipoPagto && a.Pago == false && a.FormaPagto == FormaPagtoAtendimento.Cartao)
                    .ToList();

                if(agenda.Itens == null) agenda.Itens = new List<AgendaPagamentoItem>();

                if (atendimentos != null && atendimentos.Count > 0)
                {
                    using (var trans = sessao.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        try
                        {
                            foreach (Atendimento atendimento in atendimentos)
                            {
                                atendimento.Pago = true;

                                AgendaPagamentoItem item = new AgendaPagamentoItem();

                                item.Atendimento = atendimento;
                                item.Agenda = agenda;
                                item.Valor = atendimento.Procedimentos.Sum(p => p.Valor);

                                sessao.Update(atendimento);

                                sessao.Save(item);
                            }

                            agenda.Processado = true;
                            agenda.DataConclusao = DateTime.Now;
                            sessao.Update(agenda);

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
        }
    }
}
